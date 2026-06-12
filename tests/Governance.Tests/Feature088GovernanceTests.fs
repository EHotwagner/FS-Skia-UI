module Feature088GovernanceTests

// Feature 088 (Governance Precision Hardening) — failing-first governance tests over the real
// typed values: the routable-gate projection (Targets), the re-keyed focusedGateContract
// (Front.Helpers), the GeneratedProductCheck split (Engine.Update), and the doc-only routing
// relaxation (Routing). All assertions are byte-for-byte against the prior hand-maintained
// literals, so a regression in the single-source derivation fails here.
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Preflight
open FS.Skia.UI.Build.Engine.Model
open FS.Skia.UI.Build.Engine.Update
open GovernanceTestSupport

let private model = fst (init repositoryRoot)

// The pre-088 hand-maintained AgentValidation.knownGates literal (set-equality oracle, SC-002).
let private priorKnownGates =
    [ "AgentReady"
      "ControlsCatalogCheck"
      "ControlsCatalogGenerationCheck"
      "ControlsCatalogDocsCheck"
      // Feature 106 (US2, FR-007): the Controls documentation-coverage gate joins the routable set.
      "ControlsDocCoverageCheck"
      "ControlFidelityCheck"
      "DesignTokenDrift"
      "ContrastCheck"
      "ControlsInteractionCheck"
      "ControlsRenderingCheck"
      "Dev"
      "EvidenceAudit"
      "EvidenceGraph"
      "FsiTranscripts"
      "GeneratedGuidanceCheck"
      "GeneratedProductCheck"
      "PackageSurfaceCheck"
      "PerPackageSurfaceDiff"
      "PhaseHookParityCheck"
      "PrePublishCheck"
      "Publish"
      "SkillContractPathCheck"
      "SkillQualityCheck"
      "SkillSyncCheck"
      "SymbolCrossCheck"
      "TargetMetadataDrift"
      "TemplateUpdateSkillPackageCheck"
      "TemplateCheck"
      "TemplateDrift"
      "Verify"
      "Ci" ]

// The pre-088 hand-maintained Verify ProductChecksRun literal (byte-for-byte/in-order oracle).
let private priorProductChecksRun =
    [ "Dev"
      "PackageSurfaceCheck"
      "FsiTranscripts"
      "ControlsCatalogCheck"
      "ControlsInteractionCheck"
      "ControlsRenderingCheck"
      "DependencyReport"
      "TemplateCheck"
      "GeneratedProductCheck"
      "GeneratedGuidanceCheck"
      "TemplateDrift"
      "EvidenceAudit" ]

let private diff paths : Routing.Diff = { ChangedPaths = paths }

[<Tests>]
let feature088GovernanceTests =
    testList "Feature 088 governance precision hardening" [

        // US1 / T008 / SC-003: no routable gate falls through to a degraded verdict.
        test "every routable gate resolves to a non-degraded focused-gate contract (SC-003)" {
            for gate in Targets.routableGates do
                let contract = Front.Helpers.focusedGateContract model gate

                Expect.notEqual
                    contract.VerdictCategory
                    VerificationDegraded
                    (sprintf "routable gate %s must resolve to an authoritative contract" (Targets.name gate))
        }

        // US1 / T009 / SC-002: the derived allowlist equals the prior literal as a set.
        test "routableGates derive the prior knownGates set (SC-002)" {
            Expect.equal
                (Set.ofList (Targets.routableGates |> List.map Targets.name))
                (Set.ofList priorKnownGates)
                "routableGates |> map name must set-equal the prior knownGates literal"

            Expect.equal
                (Set.ofList AgentValidation.ValidationContract.knownGates)
                (Set.ofList priorKnownGates)
                "the derived AgentValidation.knownGates must set-equal the prior literal"
        }

        // US1 / T009 / SC-002: ProductChecksRun is byte-for-byte and in order.
        test "productCheckGates equal the prior ProductChecksRun literal byte-for-byte and in order (SC-002)" {
            Expect.equal
                (Targets.productCheckGates |> List.map Targets.name)
                priorProductChecksRun
                "productCheckGates |> map name must equal the prior ProductChecksRun literal in order"
        }

        // US1 / T010 / SC-001: the contract resolves EVERY runnable target (the exhaustive,
        // wildcard-free match means a new unclassified Target case is a compile error — proven
        // structurally here by resolving all of allTargets without exception).
        test "focusedGateContract resolves every runnable target (SC-001 exhaustiveness)" {
            for target in Targets.allTargets do
                let contract = Front.Helpers.focusedGateContract model target
                Expect.equal contract.TargetName (Targets.name target) "contract carries the target's canonical name"
        }

        // US2 / T015 / SC-005: the split sub-targets emit the EXISTING effects; the umbrella
        // delegates (does not re-emit) so nothing runs twice.
        test "GeneratedProductStructure emits generate+scan but not consumer validation (FR-006)" {
            let _, effects = update (StartTarget Targets.GeneratedProductStructure) model
            Expect.contains effects GenerateV3Products "structural sub-target generates the products"
            Expect.contains effects ScanV3GeneratedProducts "structural sub-target runs the structural scan"
            Expect.isFalse
                (List.contains ValidateGeneratedConsumer effects)
                "structural sub-target does NOT run consumer validation (fails fast first)"
        }

        test "GeneratedConsumerValidation emits the consumer validation effect (FR-006)" {
            let _, effects = update (StartTarget Targets.GeneratedConsumerValidation) model
            Expect.contains effects ValidateGeneratedConsumer "consumer sub-target validates the generated consumer"
            Expect.isFalse
                (List.contains GenerateV3Products effects)
                "consumer sub-target does NOT regenerate (depends on the structural sub-target)"
        }

        test "GeneratedProductCheck umbrella composes both sub-targets and re-emits no scan/validation (FR-007)" {
            let prereqs = Targets.directPrerequisites Targets.GeneratedProductCheck
            Expect.contains prereqs Targets.GeneratedProductStructure "umbrella composes the structural sub-target"
            Expect.contains prereqs Targets.GeneratedConsumerValidation "umbrella composes the consumer sub-target"

            let _, effects = update (StartTarget Targets.GeneratedProductCheck) model
            Expect.isFalse (List.contains GenerateV3Products effects) "umbrella delegates generation (no double-run)"
            Expect.isFalse (List.contains ScanV3GeneratedProducts effects) "umbrella delegates scanning (no double-run)"
            Expect.isFalse (List.contains ValidateGeneratedConsumer effects) "umbrella delegates validation (no double-run)"
        }

        // US2 / T016 / SC-004: doc-only diffs route light; mixed/source re-escalate unchanged.
        test "doc-only template change routes to EvidenceGraph and excludes the heavy template set (SC-004)" {
            let sel = Routing.select Routing.FrameworkAuthor (diff [ "template/base/docs/getting-started.md" ])
            Expect.contains sel.MatchedRuleIds "template-docs" "the doc-only template rule matches"
            Expect.contains sel.Gates Targets.EvidenceGraph "doc-only still validates the task DAG"
            Expect.isFalse (List.contains Targets.GeneratedProductCheck sel.Gates) "doc-only excludes GeneratedProductCheck"
            Expect.isFalse (List.contains Targets.TemplateCheck sel.Gates) "doc-only excludes TemplateCheck"
        }

        test "doc-only controls change routes light and excludes the heavy controls gates (SC-004)" {
            let sel = Routing.select Routing.FrameworkAuthor (diff [ "src/Controls/widgets.md" ])
            Expect.contains sel.MatchedRuleIds "controls-docs" "the doc-only controls rule matches"
            Expect.contains sel.Gates Targets.EvidenceGraph "doc-only still validates the task DAG"
            Expect.isFalse (List.contains Targets.GeneratedProductCheck sel.Gates) "doc-only excludes GeneratedProductCheck"
            Expect.isFalse (List.contains Targets.ControlsCatalogCheck sel.Gates) "doc-only excludes the heavy controls gates"
        }

        test "template source change keeps the full template set unchanged (SC-004)" {
            let sel = Routing.select Routing.FrameworkAuthor (diff [ "template/base/Directory.Build.props" ])
            Expect.contains sel.MatchedRuleIds "generated-template" "a non-doc template change matches the source rule"
            Expect.contains sel.Gates Targets.GeneratedProductCheck "source template change keeps GeneratedProductCheck"
            Expect.contains sel.Gates Targets.TemplateCheck "source template change keeps TemplateCheck"
        }

        test "mixed controls doc+source re-escalates to the full set (controls-docs suppressed) (SC-004)" {
            let sel = Routing.select Routing.FrameworkAuthor (diff [ "src/Controls/widgets.md"; "src/Controls/Controls.fsi" ])
            Expect.isFalse
                (List.contains "controls-docs" sel.MatchedRuleIds)
                "the doc-only rule is suppressed when a non-doc path is present in the subtree"
            Expect.contains sel.Gates Targets.GeneratedProductCheck "mixed change keeps the full controls/generated set"
        }

        // US3 / T022 / FR-012: the consolidated consumer NuGet.config is byte-identical.
        test "consumer NuGet.config is public-feed only and byte-identical to the prior literal (FR-012)" {
            let expected =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
                + "<configuration>\n"
                + "  <packageSources>\n"
                + "    <clear />\n"
                + "    <add key=\"nuget\" value=\"https://api.nuget.org/v3/index.json\" />\n"
                + "  </packageSources>\n"
                + "</configuration>\n"

            Expect.equal
                ((GeneratedProduct.consumerNuGetConfigContent model).Replace("\r\n", "\n"))
                expected
                "the consolidated renderer must reproduce the public-feed-only consumer config exactly"
        }
    ]
