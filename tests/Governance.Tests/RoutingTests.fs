module RoutingTests

(* Feature 042 / US1, US2, US3, US5 — failing-first (Principle I/VI) typed-selector tests
   for the two-tier development-process router in FS.Skia.UI.Build.Routing. Each case calls
   the real `select` / `selectForFeature` / `unmetArtifacts` / `enforceDiagnostic` functions
   over literal `Diff` values and asserts the TYPED `Selection` (tier + gate list), never a
   string/IO scrape. Before Routing.fs existed these did not compile (SC-004 / FR-010). *)

open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Routing

let private diff paths = { ChangedPaths = paths }

[<Tests>]
let routingTests =
    testList "Feature 042 Routing selector (typed cases)" [

        // US1 — a routine framework change runs the light inner-loop tier (SC-001).
        test "src/Scene/*.fs only routes framework-author to inner-loop / [Dev]" {
            let selection = select FrameworkAuthor (diff [ "src/Scene/Foo.fs" ])
            Expect.equal selection.Tier InnerLoop "framework-internal .fs stays inner-loop"
            Expect.equal selection.Gates [ Targets.Dev ] "inner-loop runs Dev only — no surface check"
            Expect.isFalse (selection.Gates |> List.contains Targets.PackageSurfaceCheck) "no PackageSurfaceCheck for a .fs-only change"
        }

        // US1 — the empty diff is a deterministic inner-loop default, never a failure.
        test "empty diff routes deterministically to inner-loop / [Dev]" {
            let selection = select FrameworkAuthor (diff [])
            Expect.equal selection.Tier InnerLoop "no changes resolves to inner-loop"
            Expect.equal selection.Gates [ Targets.Dev ] "empty diff still resolves to Dev only"
        }

        // US2 — a public .fsi surface edit escalates via the package-surface rule (F1).
        test "src/**/*.fsi escalates and requires PackageSurfaceCheck" {
            let selection = select FrameworkAuthor (diff [ "src/Scene/Foo.fsi" ])
            Expect.equal selection.Tier FocusedAuthority ".fsi surface edit escalates to focused-authority"
            Expect.contains selection.Gates Targets.PackageSurfaceCheck "package-surface rule requires PackageSurfaceCheck"
            Expect.contains selection.Gates Targets.FsiTranscripts "package-surface rule requires FsiTranscripts"
        }

        // Feature 053 / US2 (C1) — a public split-package `.fsi` edit additionally requires
        // PerPackageSurfaceDiff, so an unrecorded per-package surface change fails the gate.
        test "src/<InScopePackage>/**/*.fsi requires PerPackageSurfaceDiff at focused-authority" {
            let selection = select FrameworkAuthor (diff [ "src/Scene/Foo.fsi" ])
            Expect.equal selection.Tier FocusedAuthority ".fsi surface edit escalates to focused-authority"
            Expect.contains selection.Gates Targets.PerPackageSurfaceDiff "package-surface rule requires PerPackageSurfaceDiff (053 FR-007)"
            Expect.contains selection.Gates Targets.PackageSurfaceCheck "package-surface rule still requires PackageSurfaceCheck"
            Expect.contains selection.Gates Targets.FsiTranscripts "package-surface rule still requires FsiTranscripts"
        }

        // US2 — a template change escalates to the template gate set.
        test "template/base/** escalates to TemplateCheck + GeneratedProductCheck" {
            let selection = select FrameworkAuthor (diff [ "template/base/Program.fs" ])
            Expect.notEqual selection.Tier InnerLoop "consumer-contract template change never routes inner-loop"
            Expect.contains selection.Gates Targets.TemplateCheck "generated-template rule requires TemplateCheck"
            Expect.contains selection.Gates Targets.GeneratedProductCheck "generated-template rule requires GeneratedProductCheck"
        }

        // US2 — a generated-guidance change under .specify escalates.
        test ".specify/templates/** escalates (generated-guidance)" {
            let selection = select FrameworkAuthor (diff [ ".specify/templates/tasks-template.md" ])
            Expect.notEqual selection.Tier InnerLoop ".specify guidance change escalates"
            Expect.contains selection.Gates Targets.GeneratedGuidanceCheck "generated-guidance rule requires GeneratedGuidanceCheck"
        }

        // Edge — a mixed diff takes the HIGHEST applicable tier (escalation wins, FR-003).
        test "mixed framework + template diff escalates (highest tier wins)" {
            let selection = select FrameworkAuthor (diff [ "src/Scene/Foo.fs"; "template/base/Program.fs" ])
            Expect.notEqual selection.Tier InnerLoop "a mixed diff that touches the consumer contract escalates"
            Expect.contains selection.Gates Targets.TemplateCheck "the escalation rule's gates apply to the mixed diff"
        }

        // Edge — an unknown path default-denies to the broad fallback, never an empty success.
        test "unknown path default-denies to the broad Verify fallback" {
            let selection = select FrameworkAuthor (diff [ "weird/path.txt" ])
            Expect.equal selection.Tier MaintainerVerify "an unmatched non-inner-loop path routes to the broad fallback"
            Expect.contains selection.Gates Targets.Verify "default-deny resolves to Verify, never empty"
            Expect.isNonEmpty selection.Gates "default-deny is never an empty success"
        }

        // FR-002 — the ConsumerAgent floor raises the base tier even for an otherwise
        // inner-loop diff. (The task's illustrative `docs/x.md` is itself a routing-rule
        // path; a framework-internal path is the unambiguous floor demonstration.)
        test "ConsumerAgent floor raises an inner-loop diff to focused-authority" {
            let frameworkAuthor = select FrameworkAuthor (diff [ "src/Scene/Floor.fs" ])
            let consumerAgent = select ConsumerAgent (diff [ "src/Scene/Floor.fs" ])
            Expect.equal frameworkAuthor.Tier InnerLoop "framework-author keeps the light base tier"
            Expect.equal consumerAgent.Tier FocusedAuthority "consumer-agent floor raises the base tier"
        }

        // F2 — the broadened template/** and .specify/** coverage escalates the new paths.
        test "broadened coverage: template/capabilities.yml and .specify/extensions.yml escalate" {
            let template = select FrameworkAuthor (diff [ "template/capabilities.yml" ])
            let specify = select FrameworkAuthor (diff [ ".specify/extensions.yml" ])
            Expect.equal template.Tier FocusedAuthority "template/** now covers capabilities.yml (F2)"
            Expect.equal specify.Tier FocusedAuthority ".specify/** catch-all now covers extensions.yml (F2)"
        }

        // US5 — a dogfood feature forces the full pipeline even on a would-be inner-loop diff (SC-005).
        test "dogfood feature 042 forces the full pipeline on an inner-loop diff" {
            let inner = select FrameworkAuthor (diff [ "src/Scene/Foo.fs" ])
            let dogfood = selectForFeature FrameworkAuthor "042" (diff [ "src/Scene/Foo.fs" ])
            Expect.equal inner.Tier InnerLoop "the same diff routes inner-loop without the dogfood override"
            Expect.isTrue dogfood.DogfoodForced "feature 042 is a dogfood feature"
            Expect.equal dogfood.Tier MaintainerVerify "dogfood forces maintainer-verify"
            Expect.equal dogfood.Gates fullPipelineGates "dogfood forces the full serialized gate set"
        }

        // US5 — a non-dogfood feature is not overridden.
        test "a non-dogfood feature id leaves the diff's own selection intact" {
            let selection = selectForFeature FrameworkAuthor "999" (diff [ "src/Scene/Foo.fs" ])
            Expect.isFalse selection.DogfoodForced "999 is not a dogfood feature"
            Expect.equal selection.Tier InnerLoop "a non-dogfood inner-loop diff stays inner-loop"
        }

        // US3 — the pure --enforce core reports the missing escalated artifact (SC-003).
        test "unmetArtifacts reports the package-surface artifact when absent and clears when present" {
            let selection = select FrameworkAuthor (diff [ "src/Scene/Foo.fsi" ])
            let absent = unmetArtifacts Set.empty selection
            Expect.contains absent "readiness/package-surface-expectations.md" "the escalated .fsi tier requires the surface artifact"

            let present = unmetArtifacts (Set.ofList [ "readiness/package-surface-expectations.md" ]) selection
            Expect.isEmpty present "no artifact is unmet once it is present"
        }

        // US3 — the enforce diagnostic names the artifact and the requiring tier (FR-005).
        test "enforceDiagnostic names the missing artifact and the requiring tier" {
            let selection = select FrameworkAuthor (diff [ "src/Scene/Foo.fsi" ])
            let message = enforceDiagnostic selection [ "readiness/package-surface-expectations.md" ]
            Expect.stringContains message "readiness/package-surface-expectations.md" "the diagnostic names the missing artifact"
            Expect.stringContains message "focused-authority" "the diagnostic names the requiring tier"
        }
    ]
