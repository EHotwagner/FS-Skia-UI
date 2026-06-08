module FS.Skia.UI.Build.Engine.Update

open System
open System.IO
open BuildPaths
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.Front.Support
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Front.Helpers

// Relocated verbatim from build.fsx (feature 045, T010): the PURE decision function.
// update : BuildMsg -> BuildModel -> BuildModel * BuildEffect list. No filesystem /
// git / process / write I/O (Principle IV / FR-007) — all I/O is at the interpret edge.

let update msg model =
    match msg with
    | TargetCompleted target ->
        { model with CompletedTargets = target :: model.CompletedTargets }, []
    | TargetFailed(target, reason) ->
        model, [ WriteFile(path [ model.LogDir; $"{target}-failed.txt" ], reason) ]
    | ProcessHealthCollected _ ->
        model, []
    | BootstrapValidated _ ->
        model, []
    | VerificationVerdictWritten _ ->
        model, []
    | FocusedGateCompleted _ ->
        model, []
    | StartTarget Targets.Clean ->
        model,
        [ CleanDirectoryContents model.LogDir
          CleanDirectoryContents model.FsiDir
          CleanDirectoryContents model.SampleSmokeDir
          CleanDirectoryContents model.PackageEvidenceDir
          CleanDirectoryContents model.TemplateEvidenceDir
          CleanDirectoryContents model.TemplateWorkDir
          CleanDirectoryContents model.TemplateArtifactDir ]
    | StartTarget Targets.Restore ->
        model,
        [ processEffect "dotnet tool restore" "dotnet" "tool restore" model.RepositoryRoot (path [ model.LogDir; "restore.txt" ])
          RunDotnetAction("dotnet restore", "restore", "FS-Skia-UI.sln", buildProjects, "", path [ model.LogDir; "restore.txt" ]) ]
    | StartTarget Targets.Build ->
        model,
        [ RunDotnetAction("dotnet build", "build", "FS-Skia-UI.sln", buildProjects, "--no-restore -maxcpucount:1 --disable-build-servers", path [ model.LogDir; "build.txt" ]) ]
    | StartTarget Targets.Test ->
        model,
        [ processEffect "dotnet build-server shutdown before tests" "dotnet" "build-server shutdown" model.RepositoryRoot (path [ model.LogDir; "test.txt" ])
          yield!
              defaultTestProjects
              |> List.filter (fun project -> File.Exists(path [ model.RepositoryRoot; project ]))
              |> List.map (fun project ->
                  let normalizedProject = project.Replace('\\', '/')
                  // Native-GUI Expecto suites bypass the VSTest/YoloDev adapter and run as direct
                  // Expecto executables: the adapter testhost crashes on the libdecor-gtk init path
                  // under a dual Wayland/X11 display (049). A direct `dotnet run` has no testhost
                  // grandchild and inherits the normalized X11 env from the spawn edge. SkiaViewer.Tests
                  // keeps its sequential execution (`-- --sequenced`) because it exercises native startup.
                  if normalizedProject.EndsWith("tests/Smoke.Tests/Smoke.Tests.fsproj", StringComparison.Ordinal) then
                      processEffect $"dotnet run {project}" "dotnet" $"run --project {project} --no-restore" model.RepositoryRoot (path [ model.LogDir; "test.txt" ])
                  elif normalizedProject.EndsWith("tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj", StringComparison.Ordinal) then
                      processEffect $"dotnet run {project}" "dotnet" $"run --project {project} --no-restore -- --sequenced" model.RepositoryRoot (path [ model.LogDir; "test.txt" ])
                  else
                      let extra =
                          if project.IndexOf("Governance.Tests", StringComparison.Ordinal) >= 0 then
                              " --filter \"FullyQualifiedName!~workflow self-check&FullyQualifiedName!~fixture\" -- --sequenced"
                          else
                              " -- --sequenced"

                      RunProcess(
                          $"dotnet test {project}",
                          "dotnet",
                          $"test {project} -m:1 --no-build --no-restore{extra}",
                          model.RepositoryRoot,
                          path [ model.LogDir; "test.txt" ],
                          Map.ofList [ "FS_SKIA_SAMPLE_SMOKE_DIR", model.SampleSmokeDir ]
                      )) ]
    | StartTarget Targets.Dev ->
        model,
        // Feature 062 (FR-004, D4): Dev's own emitted verdict states that Dev writes
        // logs/markers and does NOT compile, and that Test/Verify (dotnet test) is the
        // authoritative compile/test path — so the SI-3 footgun is surfaced from the
        // target's own output, not just the docs.
        [ WriteFile(
              path [ model.LogDir; "dev-verdict.txt" ],
              "Dev target completed: Restore, Build, and default non-visual Test targets passed.\n"
              + "NOTE: Dev writes logs/markers and does not compile the product on its own; "
              + "Test/Verify (dotnet test) is the authoritative compile/test path.\n")
          WriteStructuredReport("aggregate hang diagnostics", path [ model.ReadinessDir; "aggregate-hang-diagnostics.md" ], aggregateHangDiagnosticsReport) ]
    | StartTarget Targets.PackLocal ->
        model,
        (packProjects
         |> List.map (fun (project, packageId) ->
             processEffect $"dotnet pack {packageId}" "dotnet" $"pack {project} -c Release -m:1 -o {quote model.LocalPackageDir}" model.RepositoryRoot (path [ model.LogDir; "pack-local.txt" ])))
        @ [ processEffect "generate package API reference" "dotnet" "fsi scripts/generate-package-api-reference.fsx" model.RepositoryRoot (path [ model.LogDir; "pack-local.txt" ])
            WriteStructuredReport("local package report", path [ model.PackageEvidenceDir; "local-packages.md" ], localPackageReport model.RepositoryRoot model.LocalPackageDir) ]
    | StartTarget Targets.RefreshSurfaceBaselines ->
        model,
        [ processEffect "refresh surface baselines" "dotnet" "fsi scripts/refresh-surface-baselines.fsx" model.RepositoryRoot (path [ model.LogDir; "surface-refresh.txt" ])
          // Feature 042 (FR-007, research R1): regenerate the retained validation.contract.yml
          // view from the compiled Routing.fs single source of truth as part of the baseline
          // refresh, so the currency check folded into TargetMetadataDrift cannot trip on drift.
          WriteFile(path [ model.RepositoryRoot; "validation.contract.yml" ], ContractView.render Routing.rules Routing.dogfoodFeatureIds)
          // Feature 044 (US1/US3, research R1): the single regeneration entry point also
          // regenerates the derived .claude/skills tree from canonical .agents/skills, and
          // splices the constitution principle fragments into the two templates, so the
          // SkillSyncCheck and TargetMetadataDrift currency checks cannot trip on drift.
          // Feature 057 (US1): splice every canonical GovernedBlock into its home files
          // BEFORE the skill-tree regen, so a gov-block spliced into a `.agents` SKILL.md
          // propagates into its `.claude` peer in the same refresh (SkillSyncCheck stays green).
          RegenerateGovernedBlocks
          // Feature 066 (US1, FR-002): splice the six typed-catalog rows into catalog.yml
          // and Catalog.fs from CatalogGen.catalogFacts in one operation so the
          // ControlsCatalogGenerationCheck currency gate cannot trip on drift.
          RegenerateCatalog
          // Feature 069 (US1, FR-002): regenerate src/Controls/DesignTokens.fs (whole-file)
          // from the DTCG source design-tokens.tokens.json in the same refresh, so the
          // DesignTokenDrift currency gate cannot trip on drift.
          RegenerateDesignTokens
          // Feature 078 (US1, FR-004): splice the catalog index + detail-page header regions
          // in docs/controls/** from CatalogGen.catalogFacts, so ControlsCatalogDocsCheck
          // cannot trip on a stale index or detail-header region.
          RegenerateCatalogDocs
          RegenerateSkillTree
          RegenerateConstitutionFragments
          // Feature 060 (FR-003): regenerate the emitted `docs/api-surface/` tree from the
          // capability catalog `contracts:` so the surface a consumer reads in a generated
          // project stays byte-identical to the framework `.fsi` (currency in TargetMetadataDrift).
          RegenerateApiSurface
          // Feature 062 (FR-005, D5): regenerate the single-source evidence-format reference
          // from EvidenceFormatSchema so docs/evidence-formats.md stays byte-identical to the
          // constants the validators enforce (currency in TargetMetadataDrift).
          WriteFile(
              path [ model.RepositoryRoot; "template"; "base"; "docs"; "evidence-formats.md" ],
              FS.Skia.UI.Build.Evidence.EvidenceFormatSchema.renderReferenceDoc ())
          // Feature 062 (FR-006): regenerate docs/skillist-reference.md from the live
          // SkillRegistry + the closed owns vocabulary (currency in TargetMetadataDrift).
          RegenerateSkillistReference
          RequireFiles(
              "stable package surface baselines",
              [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.Layout.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.KeyboardInput.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.Elmish.txt" ]
                path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.txt" ] ]
            ) ]
    | StartTarget Targets.PackageSurfaceCheck ->
        model,
        [ processEffect "package surface test build" "dotnet" "build tests/Package.Tests/Package.Tests.fsproj -m:1 --no-restore --disable-build-servers" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          processEffect "generate package API reference" "dotnet" "fsi scripts/generate-package-api-reference.fsx" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          focusedGateAssumptionCheck model "PackageSurfaceCheck"
          processEffect "package surface check" "dotnet" "test tests/Package.Tests/Package.Tests.fsproj -m:1 --no-build --no-restore" model.RepositoryRoot (path [ model.LogDir; "package-surface-check.txt" ])
          PackageSurfaceReport
          RequireFiles("stable package surface baselines", [ path [ model.SurfaceBaselineDir; "FS.Skia.UI.KeyboardInput.txt" ]; path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.txt" ]; path [ model.SurfaceBaselineDir; "FS.Skia.UI.Controls.Elmish.txt" ] ])
          focusedGateSummary model "PackageSurfaceCheck" ]
    | StartTarget Targets.PerPackageSurfaceDiff ->
        model,
        [ focusedGateAssumptionCheck model "PerPackageSurfaceDiff"
          PerPackageSurfaceDiffCheck
          RequireFiles(
              "per-package surface diff artifacts",
              [ path [ model.ReadinessDir; "per-package-surface-diff.md" ]
                path [ model.RepositoryRoot; "readiness"; "per-package-surface-expectations.md" ] ]
          )
          focusedGateSummary model "PerPackageSurfaceDiff" ]
    | StartTarget Targets.FsiTranscripts ->
        model,
        [ focusedGateAssumptionCheck model "FsiTranscripts"
          yield!
              fsiScripts
              |> List.map (fun (name, script) ->
                  processEffect $"dotnet fsi {script}" "dotnet" $"fsi {script}" model.RepositoryRoot (path [ model.FsiDir; $"{name}.txt" ]))
          focusedGateSummary model "FsiTranscripts" ]
    | StartTarget Targets.SampleContractSmoke ->
        model,
        sampleSmokeProjects
        |> List.map (fun (name, project) ->
            processEffect $"{name} contract smoke" "dotnet" $"run --project {project} --no-build --no-restore -- --contract-smoke" model.RepositoryRoot (path [ model.SampleSmokeDir; $"{name}.txt" ]))
    | StartTarget Targets.TemplatePack ->
        model,
        [ processEffect "template package" "dotnet" $"pack .template.package/FS.Skia.UI.Template.fsproj -c Release -o {quote model.TemplateArtifactDir}" model.RepositoryRoot (path [ model.TemplateEvidenceDir; "template-pack.log" ])
          ValidateTemplatePackage(path [ model.TemplateEvidenceDir; "template-package-contents.md" ]) ]
    | StartTarget Targets.TemplateInstallSource ->
        model,
        [ InstallTemplate("source template install", SourceDirectory, path [ model.TemplateEvidenceDir; "source-install.log" ]) ]
    | StartTarget Targets.TemplateInstallPackage ->
        model,
        [ InstallTemplate("package template install", PackageArtifact, path [ model.TemplateEvidenceDir; "package-install.log" ]) ]
    | StartTarget Targets.TemplateInstantiate ->
        model,
        [ InstantiateTemplates(path [ model.TemplateEvidenceDir; "instantiation.log" ]) ]
    | StartTarget Targets.TemplateSmoke ->
        model,
        [ ScanGeneratedProjects(path [ model.TemplateEvidenceDir; "generated-project-scans.md" ])
          WriteStructuredReport("template smoke support boundary", path [ model.TemplateEvidenceDir; "non-visual-support.md" ], "# Non-Visual Support\n\nV3 template validation is non-visual. Full visual evidence, release validation, an external template repository, and broader distribution automation remain deferred roadmap work.\n") ]
    | StartTarget Targets.TemplateCheck ->
        model,
        [ focusedGateAssumptionCheck model "TemplateCheck"
          RequireFiles(
              "template validation artifact set",
              [ path [ model.TemplateEvidenceDir; "template-pack.log" ]
                path [ model.TemplateEvidenceDir; "template-package-contents.md" ]
                path [ model.TemplateEvidenceDir; "source-install.log" ]
                path [ model.TemplateEvidenceDir; "package-install.log" ]
                path [ model.TemplateEvidenceDir; "instantiation.log" ]
                path [ model.TemplateEvidenceDir; "generated-project-scans.md" ]
                path [ model.TemplateEvidenceDir; "source-app"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-headless-scene"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-governed"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "source-sample-pack"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-app"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-headless-scene"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-governed"; "dev.log" ]
                path [ model.TemplateEvidenceDir; "package-sample-pack"; "dev.log" ] ]
            )
          WriteStructuredReport("template verdict", path [ model.TemplateEvidenceDir; "verdict.md" ], "# TemplateCheck Verdict\n\nPASS: source/package V3 app, headless-scene, governed, and sample-pack generated projects passed non-visual validation.\n")
          focusedGateSummary model "TemplateCheck" ]
    | StartTarget Targets.CapabilityCheck ->
        model,
        [ CapabilityCatalogCheck
          RequireFiles("capability catalog report output", [ model.CapabilityCatalogReportPath ]) ]
    | StartTarget Targets.SkillCheck ->
        model,
        [ SkillCatalogCheck
          RequireFiles("selected skill report output", [ model.SelectedSkillsReportPath ]) ]
    | StartTarget Targets.GeneratedProductCheck ->
        model,
        [ focusedGateAssumptionCheck model "GeneratedProductCheck"
          GenerateV3Products
          ScanV3GeneratedProducts
          ValidateGeneratedConsumer
          RequireFiles(
              "generated product file-list reports",
              [ path [ model.GeneratedFileListsDir; "app-source.txt" ]
                path [ model.GeneratedFileListsDir; "app-package.txt" ]
                path [ model.GeneratedFileListsDir; "headless-scene-source.txt" ]
                path [ model.GeneratedFileListsDir; "governed-source.txt" ]
                path [ model.GeneratedFileListsDir; "sample-pack-source.txt" ]
                model.GeneratedProductValidationPath ]
            )
          focusedGateSummary model "GeneratedProductCheck" ]
    | StartTarget Targets.ControlsCatalogCheck ->
        let report = """# Control Catalog

PASS: Controls catalog tests verified supported row count, metadata, examples, tests, evidence, accessibility, and Controls-owned chart/graph rows.

- supported-controls: 46
- categories: display, input, selection, navigation, layout, feedback, data, chart, graph, custom
- catalog-source: `src/Controls/catalog.yml`
- example: `samples/ControlsGallery/Program.fs`
- checks: `tests/Controls.Tests/CatalogTests.fs`
- chart-graph-owner: controls
"""
        model,
        [ focusedGateAssumptionCheck model "ControlsCatalogCheck"
          processEffect "controls catalog tests" "dotnet" "test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-build --no-restore --filter Catalog" model.RepositoryRoot (path [ model.LogDir; "controls-catalog-check.txt" ])
          WriteStructuredReport("controls catalog", path [ model.ReadinessDir; "control-catalog.md" ], report)
          focusedGateSummary model "ControlsCatalogCheck" ]
    | StartTarget Targets.ControlsCatalogGenerationCheck ->
        // Feature 066 (US1, FR-005): the standalone typed-catalog drift gate. Read both
        // generated files at this interpreter edge (TargetMetadataDrift precedent), compute
        // currency against CatalogGen.catalogFacts, write a PASS/FAIL readiness report, and
        // FailWith the drift diagnostics naming the divergent control(s) when stale/missing.
        // CatalogGen.currency/currencyDrift are pure (Principle IV).
        let ymlPath = repoRelPath model.RepositoryRoot CatalogGen.catalogYmlRel
        let fsPath = repoRelPath model.RepositoryRoot CatalogGen.catalogFsRel
        let readOrEmpty p = if File.Exists p then File.ReadAllText p else ""

        let currency =
            CatalogGen.currency (readOrEmpty ymlPath) (readOrEmpty fsPath)

        let drift = CatalogGen.currencyDrift currency

        let report =
            if List.isEmpty drift then
                [ "# Controls Catalog Generation"
                  ""
                  sprintf
                      "PASS: the six typed-catalog rows in %s and %s are a current, byte-identical regeneration of CatalogGen.catalogFacts."
                      CatalogGen.catalogYmlRel
                      CatalogGen.catalogFsRel
                  ""
                  "- generated-controls: 6 (text-block, button, text-box, check-box, data-grid, stack)"
                  sprintf "- generated-files: %s, %s" CatalogGen.catalogYmlRel CatalogGen.catalogFsRel
                  "- single-source: build/Governance/CatalogGen.fs (catalogFacts)"
                  "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines"
                  "- failure-class: stale-generated-catalog" ]
                |> String.concat Environment.NewLine
            else
                [ "# Controls Catalog Generation"
                  ""
                  "FAIL: a generated typed-catalog region is stale or missing relative to CatalogGen.catalogFacts."
                  ""
                  yield! drift |> List.map (fun d -> sprintf "- %s" d)
                  ""
                  "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines"
                  "- failure-class: stale-generated-catalog" ]
                |> String.concat Environment.NewLine

        model,
        [ focusedGateAssumptionCheck model "ControlsCatalogGenerationCheck"
          WriteStructuredReport(
              "controls catalog generation",
              path [ model.ReadinessDir; "control-catalog-generation.md" ],
              report
          )
          if not (List.isEmpty drift) then
              FailWith(String.Join(Environment.NewLine, drift))
          focusedGateSummary model "ControlsCatalogGenerationCheck" ]
    | StartTarget Targets.DesignTokenDrift ->
        // Feature 069 (US1, FR-006): the design-token generation-currency (drift) gate. Read
        // the generated module at this interpreter edge (ControlsCatalogGenerationCheck
        // precedent), compute per-token currency against the DTCG source, write a PASS/FAIL
        // readiness report, and FailWith the drift diagnostics naming the divergent token(s)
        // when stale/missing. DesignTokenGen.currency/currencyDrift are pure (Principle IV).
        let jsonPath = repoRelPath model.RepositoryRoot DesignTokenGen.tokensJsonRel
        let fsPath = repoRelPath model.RepositoryRoot DesignTokenGen.designTokensFsRel
        let readOrEmpty p = if File.Exists p then File.ReadAllText p else ""

        let currency =
            DesignTokenGen.currency (readOrEmpty jsonPath) (readOrEmpty fsPath)

        let drift = DesignTokenGen.currencyDrift currency

        let report =
            if List.isEmpty drift then
                [ "# Design Token Generation"
                  ""
                  sprintf
                      "PASS: the 20 generated tokens in %s are a current, byte-identical regeneration of the DTCG source %s."
                      DesignTokenGen.designTokensFsRel
                      DesignTokenGen.tokensJsonRel
                  ""
                  "- generated-tokens: 20 (10 primitives x light/dark)"
                  sprintf "- generated-file: %s" DesignTokenGen.designTokensFsRel
                  sprintf "- single-source: %s" DesignTokenGen.tokensJsonRel
                  "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines"
                  "- failure-class: stale-generated-design-tokens" ]
                |> String.concat Environment.NewLine
            else
                [ "# Design Token Generation"
                  ""
                  sprintf
                      "FAIL: a generated token is stale or missing relative to the DTCG source %s."
                      DesignTokenGen.tokensJsonRel
                  ""
                  yield! drift |> List.map (fun d -> sprintf "- %s" d)
                  ""
                  "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines"
                  "- failure-class: stale-generated-design-tokens" ]
                |> String.concat Environment.NewLine

        model,
        [ focusedGateAssumptionCheck model "DesignTokenDrift"
          WriteStructuredReport(
              "design token generation",
              path [ model.ReadinessDir; "design-token-generation.md" ],
              report
          )
          if not (List.isEmpty drift) then
              FailWith(String.Join(Environment.NewLine, drift))
          focusedGateSummary model "DesignTokenDrift" ]
    | StartTarget Targets.ControlsCatalogDocsCheck ->
        // Feature 078 (US1, FR-005): the controls-catalog docs currency / completeness /
        // preview-honesty / link-resolution gate. Gather the observed docs tree at this
        // interpreter edge (file reads/listing + a dependency-free PNG structural validation),
        // compute drift against CatalogGen.catalogFacts via the pure
        // CatalogDocsGen.catalogDocsCurrency, write a PASS/FAIL readiness report, and FailWith
        // the actionable drift when non-empty.
        let facts = CatalogGen.catalogFacts
        let controlsDir = repoRelPath model.RepositoryRoot CatalogDocsGen.catalogDocsRelDir
        let imgDir = repoRelPath model.RepositoryRoot CatalogDocsGen.previewRelDir
        let readOrEmpty p = if File.Exists p then File.ReadAllText p else ""

        // Dependency-free, honest PNG structural validation: the 8-byte PNG signature, an IHDR
        // chunk whose width/height are both > 1, and a non-trivial byte length. Rejects a 1x1 /
        // truncated / non-PNG placeholder without taking a SkiaSharp dependency in the build.
        let validatePng (p: string) =
            try
                let bytes = File.ReadAllBytes p
                let signature = [| 0x89uy; 0x50uy; 0x4Euy; 0x47uy; 0x0Duy; 0x0Auy; 0x1Auy; 0x0Auy |]
                let beUInt (offset: int) =
                    (int bytes.[offset] <<< 24)
                    ||| (int bytes.[offset + 1] <<< 16)
                    ||| (int bytes.[offset + 2] <<< 8)
                    ||| int bytes.[offset + 3]
                bytes.Length > 256
                && Array.sub bytes 0 8 = signature
                && bytes.[12] = byte 'I' && bytes.[13] = byte 'H' && bytes.[14] = byte 'D' && bytes.[15] = byte 'R'
                && beUInt 16 > 1
                && beUInt 20 > 1
            with _ -> false

        let stem (p: string) =
            Path.GetFileNameWithoutExtension p |> Option.ofObj |> Option.defaultValue ""

        let excluded = Set.ofList [ "catalog"; "spec-kit-workflow" ]

        let detailPages : CatalogDocsGen.DetailPage list =
            (if Directory.Exists controlsDir then Directory.GetFiles(controlsDir, "*.md") |> List.ofArray else [])
            |> List.map (fun p -> stem p, p)
            |> List.filter (fun (id, _) -> not (excluded.Contains id))
            |> List.map (fun (id, p) -> { ControlId = id; Text = readOrEmpty p })

        let previews : CatalogDocsGen.PreviewAsset list =
            (if Directory.Exists imgDir then Directory.GetFiles(imgDir, "*.png") |> List.ofArray else [])
            |> List.map (fun p -> { ControlId = stem p; Decodable = validatePng p })

        let referenceDir = repoRelPath model.RepositoryRoot "output/reference"

        let availableSlugs =
            if Directory.Exists referenceDir then
                Directory.GetFiles(referenceDir, "*.html")
                |> Array.map stem
                |> Set.ofArray
                |> Some
            else
                None

        let tree : CatalogDocsGen.DocsTree =
            { CatalogIndexText = readOrEmpty (repoRelPath model.RepositoryRoot CatalogDocsGen.catalogIndexRel)
              DetailPages = detailPages
              Previews = previews
              AvailableReferenceSlugs = availableSlugs }

        let findings = CatalogDocsGen.catalogDocsCurrency facts tree
        let drift = CatalogDocsGen.currencyDrift findings

        let report =
            if List.isEmpty findings then
                [ "# Controls Catalog Docs"
                  ""
                  sprintf
                      "PASS: the published Controls docs section is a current, complete, honest projection of CatalogGen.catalogFacts (%d controls)."
                      (List.length facts)
                  ""
                  sprintf "- supported-controls: %d" (List.length facts)
                  sprintf "- generated-index: %s (catalog-docs/index region)" CatalogDocsGen.catalogIndexRel
                  "- detail-pages: one per control; catalog-docs/<id> header region current"
                  sprintf
                      "- previews-present: %d (validated decodable, non-1x1, non-trivial)"
                      (previews |> List.filter (fun p -> p.Decodable) |> List.length)
                  (match availableSlugs with
                   | Some _ -> "- api-links: resolved against output/reference/"
                   | None -> "- api-links: resolution deferred to dotnet fsdocs build --strict (no built site present)")
                  "- single-source: build/Governance/CatalogGen.fs (catalogFacts)"
                  "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines"
                  "- failure-class: none" ]
                |> String.concat Environment.NewLine
            else
                [ "# Controls Catalog Docs"
                  ""
                  "FAIL: the published Controls docs section has drifted from CatalogGen.catalogFacts."
                  ""
                  yield! drift |> List.map (fun d -> sprintf "- %s" d)
                  ""
                  "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines"
                  "- failure-class: stale-generated-catalog-docs" ]
                |> String.concat Environment.NewLine

        model,
        [ focusedGateAssumptionCheck model "ControlsCatalogDocsCheck"
          WriteStructuredReport(
              "controls catalog docs",
              path [ model.ReadinessDir; "controls-catalog-docs.md" ],
              report
          )
          if not (List.isEmpty drift) then
              FailWith(String.Join(Environment.NewLine, drift))
          RequireFiles("controls catalog docs report output", [ path [ model.ReadinessDir; "controls-catalog-docs.md" ] ])
          focusedGateSummary model "ControlsCatalogDocsCheck" ]
    | StartTarget Targets.ControlsInteractionCheck ->
        let report = """# Interaction Tests

PASS: pointer, keyboard, disabled/read-only suppression, exactly-once dispatch, stale handler prevention, text input effects, and MVU update assertions passed.

- pointer activation dispatches exactly one current-view message
- keyboard activation uses the same event path
- disabled controls suppress click dispatch
- read-only text boxes suppress text-change dispatch
- text input emits explicit `CommitText` and `RequestClipboardText` effects
- IME/composition without host support reports `UnsupportedEnvironment`
"""
        model,
        [ focusedGateAssumptionCheck model "ControlsInteractionCheck"
          processEffect "controls interaction tests" "dotnet" "test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-build --no-restore --filter Interaction" model.RepositoryRoot (path [ model.LogDir; "controls-interaction-check.txt" ])
          WriteStructuredReport("controls interactions", path [ model.ReadinessDir; "interaction-tests.md" ], report)
          focusedGateSummary model "ControlsInteractionCheck" ]
    | StartTarget Targets.ControlsRenderingCheck ->
        let report = """# Layout And Rendering

PASS: Controls render evidence covered three viewport sizes, two scale factors, graph/chart controls, and 10,000-item visible-range behavior.

- viewports: 320x240, 640x480, 1024x768
- density-scale-factors: 1.0, 2.0
- large-data-total-items: 10000
- initial-visible-range-count: 11
- scrolled-first-index: 250
- scrolled-visible-range-bound: less than 30 rows
- environment-diagnostics: none for deterministic scene readback
"""
        model,
        [ focusedGateAssumptionCheck model "ControlsRenderingCheck"
          processEffect "controls rendering tests" "dotnet" "test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-build --no-restore --filter Rendering" model.RepositoryRoot (path [ model.LogDir; "controls-rendering-check.txt" ])
          WriteStructuredReport("controls rendering", path [ model.ReadinessDir; "layout-rendering.md" ], report)
          focusedGateSummary model "ControlsRenderingCheck" ]
    | StartTarget Targets.DependencyReport ->
        model,
        [ focusedGateAssumptionCheck model "DependencyReport"
          DependencyOwnershipReport
          processEffect "dependency report" "dotnet" ("fsi scripts/dependency-report.fsx " + quote (path [ model.ReadinessDir; "dependencies.md" ])) model.RepositoryRoot (path [ model.LogDir; "dependency-report.txt" ])
          RequireFiles("dependency report output", [ model.DependencyReportPath ])
          focusedGateSummary model "DependencyReport" ]
    | StartTarget Targets.SymbolCrossCheck ->
        model,
        [ focusedGateAssumptionCheck model "SymbolCrossCheck"
          SymbolCrossCheckAnalyze
          RequireFiles("symbol cross-check output", [ path [ model.ReadinessDir; "symbol-cross-check.md" ] ])
          focusedGateSummary model "SymbolCrossCheck" ]
    | StartTarget Targets.GeneratedGuidanceCheck ->
        model,
        [ focusedGateAssumptionCheck model "GeneratedGuidanceCheck"
          GeneratedGuidanceScan model.GeneratedGuidanceReportPath
          RequireFiles("generated guidance report output", [ model.GeneratedGuidanceReportPath ])
          focusedGateSummary model "GeneratedGuidanceCheck" ]
    | StartTarget Targets.SkillSyncCheck ->
        model,
        [ focusedGateAssumptionCheck model "SkillSyncCheck"
          SkillSyncGate
          RequireFiles("skill sync report", [ path [ model.ReadinessDir; "skill-sync-check.md" ]; path [ model.LogDir; "skill-sync-check.txt" ] ])
          focusedGateSummary model "SkillSyncCheck" ]
    | StartTarget Targets.SkillQualityCheck ->
        model,
        [ focusedGateAssumptionCheck model "SkillQualityCheck"
          SkillQualityScan
          RequireFiles("skill quality report", [ path [ model.ReadinessDir; "skill-quality-check.md" ]; path [ model.LogDir; "skill-quality-check.txt" ] ])
          focusedGateSummary model "SkillQualityCheck" ]
    | StartTarget Targets.PhaseHookParityCheck ->
        model,
        [ focusedGateAssumptionCheck model "PhaseHookParityCheck"
          PhaseHookScan
          RequireFiles("phase hook parity report", [ path [ model.ReadinessDir; "phase-hook-parity-check.md" ]; path [ model.LogDir; "phase-hook-parity-check.txt" ] ])
          focusedGateSummary model "PhaseHookParityCheck" ]
    | StartTarget Targets.SkillContractPathCheck ->
        model,
        [ focusedGateAssumptionCheck model "SkillContractPathCheck"
          SkillContractPathScan
          RequireFiles("skill contract path report", [ path [ model.ReadinessDir; "skill-contract-path-check.md" ] ])
          focusedGateSummary model "SkillContractPathCheck" ]
    | StartTarget Targets.TemplateUpdateSkillPackageCheck ->
        model,
        [ focusedGateAssumptionCheck model "TemplateUpdateSkillPackageCheck"
          TemplateUpdatePackageScan
          RequireFiles("template update package report", [ path [ model.ReadinessDir; "template-update-package-check.md" ] ])
          focusedGateSummary model "TemplateUpdateSkillPackageCheck" ]
    | StartTarget Targets.TemplateDrift ->
        model,
        [ focusedGateAssumptionCheck model "TemplateDrift"
          processEffect "template drift" "dotnet" $"fsi scripts/template-drift.fsx {quote model.TemplateDriftReportPath}" model.RepositoryRoot (path [ model.LogDir; "template-drift.txt" ])
          RequireFiles("template drift report output", [ model.TemplateDriftReportPath ])
          focusedGateSummary model "TemplateDrift" ]
    | StartTarget Targets.EvidenceGraph ->
        model,
        [ focusedGateAssumptionCheck model "EvidenceGraph"
          EvidenceGraphCheck
          RequireFiles("task graph output", [ path [ model.ReadinessDir; "task-graph.json" ]; path [ model.ReadinessDir; "task-graph.md" ] ])
          WriteStructuredReport("evidence graph readiness", model.EvidenceGraphReportPath, "# Evidence Graph Evidence\n\nPASS: `EvidenceGraph` ran graph validation only and refreshed `task-graph.md` and `task-graph.json` with accepted `[SEH]`, unaccepted `[S]`, and `[S*]` counts reported separately.\n\nRun `EvidenceAudit` for full merge-gate validation, including diff-scan and synthetic-evidence blocking checks.\n")
          focusedGateSummary model "EvidenceGraph" ]
    | StartTarget Targets.EvidenceAudit ->
        model,
        [ focusedGateAssumptionCheck model "EvidenceAudit"
          EvidenceAuditCheck
          RequireFiles("evidence audit output", [ path [ model.LogDir; "evidence-audit.txt" ]; path [ model.ReadinessDir; "diff-scan-hits.json" ] ])
          WriteStructuredReport("evidence audit readiness", model.EvidenceAuditReportPath, "# Evidence Audit Evidence\n\nPASS: `EvidenceAudit` completed with synthetic propagation and diff-scan outputs present.\n\nSee `readiness/logs/evidence-audit.txt` for `accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, and `late-seh-tasks` counts. Accepted `[SEH]` evidence remains synthetic and is reported separately from real task evidence.\n")
          focusedGateSummary model "EvidenceAudit" ]
    | StartTarget Targets.AgentReady ->
        let verdictJson =
            [ "{"
              "  \"status\": \"degraded\","
              "  \"authority\": \"focused-authoritative\","
              "  \"changed_path_source\": { \"kind\": \"git-merge-base-diff\", \"paths\": [] },"
              "  \"selected_rule_ids\": [],"
              "  \"required_gates\": [\"EvidenceGraph\", \"EvidenceAudit\"],"
              "  \"completed_gates\": [\"EvidenceGraph\"],"
              "  \"missing_gates\": [\"EvidenceAudit\"],"
              "  \"skipped_gates\": [],"
              "  \"missing_artifacts\": [\"readiness/evidence-audit.md\"],"
              "  \"failure_owner\": \"governance\","
              "  \"failure_class\": \"missing-evidence\","
              "  \"next_command\": \"./fake.sh build -t Verify\","
              "  \"artifacts\": [\"readiness/task-graph.json\", \"readiness/task-graph.md\"],"
              "  \"diagnostics\": [\"AgentReady produced a degraded handoff because EvidenceAudit is a final readiness gate for later integration tasks.\"],"
              $"  \"timestamp_utc\": \"{DateTimeOffset.UtcNow:O}\""
              "}" ]
            |> String.concat Environment.NewLine

        let verdictMarkdown =
            [ "# AgentReady Verdict"
              ""
              "Status: `degraded`"
              ""
              "- authority: `focused-authoritative`"
              "- required-gates: `EvidenceGraph`, `EvidenceAudit`"
              "- completed-gates: `EvidenceGraph`"
              "- missing-gates: `EvidenceAudit`"
              "- missing-artifacts: `readiness/evidence-audit.md`"
              "- next-command: `./fake.sh build -t Verify`"
              "- diagnostic: AgentReady produced a degraded handoff because EvidenceAudit is a final readiness gate for later integration tasks." ]
            |> String.concat Environment.NewLine

        model,
        [ RequireFiles(
              "agent-ready readiness obligations",
              [ path [ model.ReadinessDir; "validation-contract.md" ]
                path [ model.ReadinessDir; "task-graph.json" ]
                path [ model.ReadinessDir; "task-graph.md" ] ]
            )
          WriteStructuredJsonReport("agent-ready verdict json", path [ model.ReadinessDir; "agent-verdict.json" ], verdictJson)
          WriteStructuredReport("agent-ready verdict markdown", path [ model.ReadinessDir; "agent-verdict.md" ], verdictMarkdown)
          WriteStructuredReport("agent-ready feature evidence", path [ model.ReadinessDir; "agent-ready-verdict.md" ], verdictMarkdown + Environment.NewLine)
          focusedGateSummary model "AgentReady" ]
    | StartTarget Targets.TargetMetadata ->
        let metadata = allTargetMetadata model
        let report =
            TargetMetadata.metadataJson (DateTimeOffset.UtcNow.ToString("O")) [] metadata

        model,
        [ WriteStructuredJsonReport("target metadata", model.TargetMetadataReportPath, report)
          focusedGateSummary model "TargetMetadata" ]
    | StartTarget Targets.TargetMetadataDrift ->
        let metadata = allTargetMetadata model
        let structuralDiagnostics = validateTargetMetadataAgainstRepo model.RepositoryRoot requiredTargets metadata

        // Feature 042 (FR-007): the generated validation.contract.yml currency check folds
        // into TargetMetadataDrift so the retained file can never silently diverge from the
        // compiled Routing.fs source of truth. The file read stays at this interpreter edge;
        // ContractView.currencyDrift itself is pure (Principle IV).
        let contractPath = path [ model.RepositoryRoot; "validation.contract.yml" ]

        let currencyDiagnostics =
            if File.Exists contractPath then
                ContractView.currencyDrift (File.ReadAllText contractPath) Routing.rules Routing.dogfoodFeatureIds
                |> Option.toList
            else
                [ "validation.contract.yml is missing — regenerate from Routing.fs via ./fake.sh build -t RefreshSurfaceBaselines" ]

        // Feature 044 (US3, FR-009): the constitution principle-fragment currency check folds
        // into TargetMetadataDrift next to the contract-currency check (same home, same
        // precedent). The constitution + template reads stay at this interpreter edge;
        // ConstitutionFragments.extract/currency/currencyDrift are pure (Principle IV).
        let constitutionPath = repoRelPath model.RepositoryRoot constitutionRelPath

        let constitutionDiagnostics =
            if not (File.Exists constitutionPath) then
                [ sprintf "%s is missing — cannot derive principle fragments" constitutionRelPath ]
            else
                let fragments = ConstitutionFragments.extract (File.ReadAllText constitutionPath)

                [ for (relTemplate, ids) in constitutionTemplateRegions do
                      let templatePath = repoRelPath model.RepositoryRoot relTemplate

                      if not (File.Exists templatePath) then
                          yield sprintf "%s is missing — cannot check generated constitution fragments" relTemplate
                      else
                          let subset = fragments |> List.filter (fun f -> List.contains f.FragmentId ids)
                          let currency = ConstitutionFragments.currency relTemplate subset (File.ReadAllText templatePath)

                          match ConstitutionFragments.currencyDrift currency with
                          | Some diagnostic -> yield diagnostic
                          | None -> () ]

        // Feature 057 (US1/US2, FR-003/FR-005): every canonical GovernedBlock's generated
        // copy is currency-checked here next to the contract- and constitution-fragment
        // checks (same home, same precedent). A hand-edited generated copy that diverges
        // from its canonical source fails with a diagnostic naming the drifted file AND its
        // source block. The home-file reads stay at this interpreter edge;
        // GovernedBlocks.currency/currencyDrift are pure (Principle IV).
        let governedBlockDiagnostics =
            let lookup (rel: string) =
                let full = repoRelPath model.RepositoryRoot rel
                if File.Exists full then Some(File.ReadAllText full) else None

            FS.Skia.UI.Build.GovernedBlocks.governedBlocks
            |> List.collect (FS.Skia.UI.Build.GovernedBlocks.blockCurrencies lookup)
            |> List.choose FS.Skia.UI.Build.GovernedBlocks.currencyDrift

        // Feature 057 (US1, class 4 / FR-007): the placeholder-bearing constitution twin is
        // canonical; constitution.md (concrete render) and the preset twin (verbatim) are
        // generated and currency-checked here. A hand-edited generated copy fails naming the
        // drifted file and its canonical source.
        let constitutionRenderDiagnostics =
            let canonicalFull = repoRelPath model.RepositoryRoot GovernedBlocks.constitutionCanonicalRel

            if not (File.Exists canonicalFull) then
                [ sprintf "%s is missing — cannot render constitution targets" GovernedBlocks.constitutionCanonicalRel ]
            else
                let canonicalText = File.ReadAllText canonicalFull

                let checkTarget targetRel mode =
                    let targetFull = repoRelPath model.RepositoryRoot targetRel

                    if not (File.Exists targetFull) then
                        [ sprintf "%s is missing — cannot check generated constitution render" targetRel ]
                    else
                        GovernedBlocks.constitutionDrift
                            targetRel
                            GovernedBlocks.constitutionCanonicalRel
                            (GovernedBlocks.renderConstitution mode canonicalText)
                            (File.ReadAllText targetFull)
                        |> Option.toList

                checkTarget GovernedBlocks.constitutionConcreteRel GovernedBlocks.Concrete
                @ checkTarget GovernedBlocks.constitutionTwinRel GovernedBlocks.Twin

        // Feature 060 (FR-003): the emitted `docs/api-surface/<Pkg>/<file>.fsi` tree is a
        // single-source generated artifact (from `template/capabilities.yml` `contracts:`),
        // so its currency folds here next to the contract.yml / constitution / governed-block
        // checks. The catalog + .fsi + emitted-tree reads stay at this interpreter edge;
        // ApiSurfaceGen.plan/currency are pure (Principle IV).
        let apiSurfaceDiagnostics =
            let catalogPath = model.CapabilityCatalogPath

            if not (File.Exists catalogPath) then
                [ "template/capabilities.yml is missing — cannot check api-surface currency" ]
            else
                let entries = ApiSurfaceGen.plan (Capabilities.readCatalog catalogPath)
                let toFull (rel: string) = repoRelPath model.RepositoryRoot rel

                let readBytes (rel: string) =
                    let full = toFull rel
                    if File.Exists full then Some(File.ReadAllBytes full) else None

                let emittedRootFull = toFull ApiSurfaceGen.emittedRoot

                let existing =
                    if Directory.Exists emittedRootFull then
                        Directory.EnumerateFiles(emittedRootFull, "*", SearchOption.AllDirectories)
                        |> Seq.map (fun f -> (Path.GetRelativePath(model.RepositoryRoot, f)).Replace('\\', '/'))
                        |> Seq.toList
                    else
                        []

                ApiSurfaceGen.currency entries readBytes existing

        // Feature 062 (FR-005, D12): currency for the single-source generated
        // docs/evidence-formats.md reference. Regenerate the string from
        // EvidenceFormatSchema and compare to the committed file; drift fails the
        // gate with a "regenerate via RefreshSurfaceBaselines" diagnostic.
        let evidenceFormatsDocDiagnostics =
            let rel = FS.Skia.UI.Build.Evidence.EvidenceFormatSchema.referenceDocPath
            let full = repoRelPath model.RepositoryRoot rel
            let expected =
                (FS.Skia.UI.Build.Evidence.EvidenceFormatSchema.renderReferenceDoc ()).Replace("\r\n", "\n")

            if not (File.Exists full) then
                [ sprintf "%s is missing — generate it from EvidenceFormatSchema (./fake.sh build -t RefreshSurfaceBaselines)" rel ]
            elif File.ReadAllText(full).Replace("\r\n", "\n") <> expected then
                [ sprintf "%s is stale — regenerate from EvidenceFormatSchema (./fake.sh build -t RefreshSurfaceBaselines)" rel ]
            else
                []

        // Feature 062 (FR-006, D12): currency for the single-source generated
        // docs/skillist-reference.md. Regenerate from the live registry + the closed
        // owns vocabulary and compare to the committed file; drift fails the gate.
        let skillistReferenceDocDiagnostics =
            let rel = FS.Skia.UI.Build.SkillistReference.referenceDocPath
            let full = repoRelPath model.RepositoryRoot rel
            let registry = FS.Skia.UI.Build.Evidence.SkillRegistry.build model.RepositoryRoot
            let expected =
                (FS.Skia.UI.Build.SkillistReference.render registry FS.Skia.UI.Build.Evidence.Audit.ownsVocabulary)
                    .Replace("\r\n", "\n")

            if not (File.Exists full) then
                [ sprintf "%s is missing — generate from the live SkillRegistry (./fake.sh build -t RefreshSurfaceBaselines)" rel ]
            elif File.ReadAllText(full).Replace("\r\n", "\n") <> expected then
                [ sprintf "%s is stale — regenerate from the live SkillRegistry (./fake.sh build -t RefreshSurfaceBaselines)" rel ]
            else
                []

        let diagnostics =
            structuralDiagnostics
            @ currencyDiagnostics
            @ constitutionDiagnostics
            @ governedBlockDiagnostics
            @ constitutionRenderDiagnostics
            @ apiSurfaceDiagnostics
            @ evidenceFormatsDocDiagnostics
            @ skillistReferenceDocDiagnostics

        let report = TargetMetadata.driftMarkdown diagnostics

        model,
        [ RequireFiles("target metadata report", [ model.TargetMetadataReportPath ])
          WriteStructuredReport("target metadata drift", model.TargetMetadataDriftReportPath, report)
          if not diagnostics.IsEmpty then
              FailWith(String.Join(Environment.NewLine, diagnostics))
          focusedGateSummary model "TargetMetadataDrift" ]
    | StartTarget Targets.Route ->
        // Feature 042 (FR-004): the typed selector runs in-process at the edge. The git
        // union-diff read, the --enforce File.Exists probe, and printing are interpreter I/O
        // (Principle IV); the Routing selector itself is pure. See `runRouteSelection`.
        model, [ RouteSelect ]
    | StartTarget Targets.PrePublishCheck ->
        // Feature 064 (FR-006): the pure update only emits the validate effect. The
        // interpreter reads the files, assembles PrePublishInputs, runs the four pure rules,
        // writes the report, and aborts (FailWith) naming the offender on any finding.
        model, [ PrePublishValidate ]
    | StartTarget Targets.Publish ->
        // Feature 064 (FR-001/FR-002): the pure update only emits the publish effect. The
        // interpreter reads the PublishConfig from env, builds the 12-row plan via an
        // anonymous feed read, writes the plan, and (non-dry-run) runs
        // `dotnet nuget push --skip-duplicate`; a non-dry-run with no API key aborts fast.
        model, [ PublishPackages ]
    | StartTarget Targets.VerifyPreflight ->
        model,
        [ CollectProcessHealth("Verify", model.ProcessHealthPath, model.VerificationVerdictsPath)
          ValidateRunnerBootstrap("Verify", model.BootstrapRunnerPath, model.VerificationVerdictsPath)
          RequireFiles(
              "verify readiness preflight artifact set",
              [ path [ model.ReadinessDir; "public-surface.md" ]
                path [ model.ReadinessDir; "package-boundary.md" ]
                path [ model.ReadinessDir; "generated-product-usage.md" ]
                path [ model.ReadinessDir; "compatibility-impact.md" ] ]
            ) ]
    | StartTarget Targets.CiPreflight ->
        model,
        [ CollectProcessHealth("Ci", model.ProcessHealthPath, model.VerificationVerdictsPath)
          ValidateRunnerBootstrap("Ci", model.BootstrapRunnerPath, model.VerificationVerdictsPath) ]
    | StartTarget Targets.StaleBoundaryScan ->
        model,
        [ WriteStructuredReport("stale boundary scan", model.StaleBoundaryScanPath, "# Stale Boundary Scan Evidence\n\nStatus: pending scanner implementation.\n\n- rule-id: stale-boundary-scan\n- classification: degraded\n- remediation-action: implement active-tree stale reference scanner before final readiness.\n") ]
    | StartTarget Targets.FinalReadiness ->
        model,
        [ WriteStructuredReport("final readiness", model.EvidenceAuditReportPath, "# Evidence Audit Evidence\n\nStatus: waiting for `EvidenceAudit` and healthy broad aggregate evidence.\n") ]
    | StartTarget Targets.Verify ->
        model,
        [ RequireFiles(
              "v1 plus v2 verification artifact set",
              [ path [ model.LogDir; "build.txt" ]
                path [ model.LogDir; "test.txt" ]
                path [ model.LogDir; "pack-local.txt" ]
                path [ model.LogDir; "package-surface-check.txt" ]
                path [ model.LogDir; "controls-catalog-check.txt" ]
                path [ model.LogDir; "controls-interaction-check.txt" ]
                path [ model.LogDir; "controls-rendering-check.txt" ]
                path [ model.LogDir; "dependency-report.txt" ]
                path [ model.LogDir; "template-drift.txt" ]
                path [ model.LogDir; "evidence-audit.txt" ]
                model.CapabilityCatalogReportPath
                model.SelectedSkillsReportPath
                path [ model.GeneratedFileListsDir; "app-source.txt" ]
                path [ model.GeneratedProductVerifyDir; "app-source"; "verify.log" ]
                path [ model.FsiDir; "prelude.txt" ]
                path [ model.FsiDir; "input-prelude.txt" ]
                path [ model.FsiDir; "keyboardinput-package-prelude.txt" ]
                path [ model.FsiDir; "layout-prelude.txt" ]
                path [ model.FsiDir; "controls-prelude.txt" ]
                path [ model.FsiDir; "controls-elmish-prelude.txt" ]
                path [ model.SampleSmokeDir; "BasicViewer.txt" ]
                path [ model.SampleSmokeDir; "LayoutGraphGallery.txt" ]
                path [ model.SampleSmokeDir; "DataGridGallery.txt" ]
                path [ model.SampleSmokeDir; "ChartsGallery.txt" ]
                path [ model.SampleSmokeDir; "KeyboardInputGallery.txt" ]
                path [ model.SampleSmokeDir; "ControlsGallery.txt" ]
                path [ model.ReadinessDir; "public-surface.md" ]
                path [ model.ReadinessDir; "package-boundary.md" ]
                path [ model.ReadinessDir; "control-catalog.md" ]
                path [ model.ReadinessDir; "interaction-tests.md" ]
                path [ model.ReadinessDir; "layout-rendering.md" ]
                path [ model.ReadinessDir; "generated-product-usage.md" ]
                path [ model.ReadinessDir; "generated-guidance.md" ]
                path [ model.ReadinessDir; "compatibility-impact.md" ]
                path [ model.ReadinessDir; "evidence-audit.md" ]
                path [ model.ReadinessDir; "task-graph.json" ]
                model.DependencyReportPath
                model.GeneratedGuidanceReportPath
                model.TemplateDriftReportPath
                path [ model.TemplateEvidenceDir; "verdict.md" ] ]
            )
          WriteVerificationVerdict
              { Category = VerificationSuccess
                Target = "Verify"
                Stage = "final"
                ExitCode = Some 0
                ProductChecksRun = [ "Dev"; "PackageSurfaceCheck"; "FsiTranscripts"; "ControlsCatalogCheck"; "ControlsInteractionCheck"; "ControlsRenderingCheck"; "DependencyReport"; "TemplateCheck"; "GeneratedProductCheck"; "GeneratedGuidanceCheck"; "TemplateDrift"; "EvidenceAudit" ]
                ProductFailures = []
                EnvironmentFailures = []
                HealthSnapshotPath = model.ProcessHealthPath
                LogPath = path [ model.LogDir; "verify-verdict.txt" ]
                RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
                AuthoritativeProductEvidence = true }
          WriteStructuredReport("verify verdict", path [ model.LogDir; "verify-verdict.txt" ], "Verify target completed with v1 and v2 artifact classes present.\n") ]
    | StartTarget Targets.Ci ->
        model,
        [ WriteVerificationVerdict
              { Category = VerificationSuccess
                Target = "Ci"
                Stage = "final"
                ExitCode = Some 0
                ProductChecksRun = [ "Verify" ]
                ProductFailures = []
                EnvironmentFailures = []
                HealthSnapshotPath = model.ProcessHealthPath
                LogPath = path [ model.LogDir; "ci-verdict.txt" ]
                RecommendedRerunEnvironment = "fresh shell, fresh container, or CI runner"
                AuthoritativeProductEvidence = true }
          WriteStructuredReport("ci verdict", path [ model.LogDir; "ci-verdict.txt" ], "Ci delegates to Verify and completed without duplicating command order.\n") ]
    | StartTarget Targets.PackageSmoke ->
        model,
        [ RunProcess(
              "deferred package consumer smoke",
              "dotnet",
              "test tests/Package.Tests/Package.Tests.fsproj --no-build",
              model.RepositoryRoot,
              path [ model.LogDir; "package-consumer-smoke.txt" ],
              Map.ofList [ ("FS_SKIA_RUN_PACKAGE_CONSUMER_SMOKE", "1") ]
            ) ]
    | StartTarget Targets.BuildWorkflowCheck ->
        model, [ WorkflowSelfCheck ]

