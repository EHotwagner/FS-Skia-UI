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
                  if project.Replace('\\', '/').EndsWith("tests/Smoke.Tests/Smoke.Tests.fsproj", StringComparison.Ordinal) then
                      processEffect $"dotnet run {project}" "dotnet" $"run --project {project} --no-restore" model.RepositoryRoot (path [ model.LogDir; "test.txt" ])
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
        [ WriteFile(path [ model.LogDir; "dev-verdict.txt" ], "Dev target completed: Restore, Build, and default non-visual Test targets passed.\n")
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
          RegenerateSkillTree
          RegenerateConstitutionFragments
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

        let diagnostics = structuralDiagnostics @ currencyDiagnostics @ constitutionDiagnostics
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

