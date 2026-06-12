module Feature087GovernanceTests

// Feature 087 — Governance Gate Hardening. Failing-first (Principle I/VI)
// governance coverage for the six hardening changes. This file is the shared
// scaffold authored at T005; each user story appends its red-green testList in
// its own phase (the section markers below name the owning task):
//   * US1 (T007/T008) — per-step product-defect vs environment aggregation (FR-002).
//   * US2 (T012)      — static pinned-vs-local package-skew detection (FR-003/004).
//   * US3 (T016)      — per-package baseline byte-idempotence (FR-005/006).
//   * US4 (T019)      — three-state verdict invariant (FR-007/008/011).
//   * US5 (T023)      — [S*] propagation over ExplicitDeps only (FR-009).
//   * US6 (T026)      — skill-loading provenance parse/validate (FR-010).
// The foundation testList below proves the T004 single-source value types and the
// T006 evidence-formats.md provenance mirror. All assertions call the real pure
// functions or read the shipped doc — no log scraping.

open Expecto
open FsCheck
open FS.Skia.UI.Build.Evidence
open GovernanceTestSupport

[<Tests>]
let feature087FoundationTests =
    testList "feature 087 foundation — single-source value types (T004/T006)" [

        // --- T004: AuditVerdict three-state + passing predicate ---------------
        test "T004 AuditVerdict has the three labels and isPassingVerdict agrees" {
            Expect.equal (EvidenceFormatSchema.auditVerdictLabel Pass) "Pass" "Pass label"
            Expect.equal
                (EvidenceFormatSchema.auditVerdictLabel PassWithAcceptedDeferrals)
                "PassWithAcceptedDeferrals" "PassWithAcceptedDeferrals label"
            Expect.equal (EvidenceFormatSchema.auditVerdictLabel Fail) "Fail" "Fail label"
            Expect.isTrue (EvidenceFormatSchema.isPassingVerdict Pass) "Pass is passing"
            Expect.isTrue
                (EvidenceFormatSchema.isPassingVerdict PassWithAcceptedDeferrals)
                "PassWithAcceptedDeferrals is passing"
            Expect.isFalse (EvidenceFormatSchema.isPassingVerdict Fail) "Fail is not passing"
        }

        // --- T004: classification / package-set labels ------------------------
        test "T004 StepClassification and PackageSet carry their contract labels" {
            Expect.equal (EvidenceFormatSchema.stepClassificationLabel ProductDefect) "ProductDefect" "ProductDefect label"
            Expect.equal (EvidenceFormatSchema.stepClassificationLabel Environment) "Environment" "Environment label"
            Expect.equal (EvidenceFormatSchema.packageSetLabel LocalPacked) "LocalPacked" "LocalPacked label"
            Expect.equal (EvidenceFormatSchema.packageSetLabel Pinned) "Pinned" "Pinned label"
        }

        // --- T004: provenance label/parse round-trip (FsCheck) ----------------
        test "T004 LoadProvenance label/parse round-trips for both cases (FsCheck)" {
            let roundTrips (p: LoadProvenance) =
                EvidenceFormatSchema.parseProvenance (EvidenceFormatSchema.loadProvenanceLabel p) = Some p
            // Closed two-case union — enumerate via a bool generator.
            Check.QuickThrowOnFailure(fun (b: bool) ->
                roundTrips (if b then Captured else Asserted))
            Expect.equal (EvidenceFormatSchema.parseProvenance "captured") (Some Captured) "captured parses"
            Expect.equal (EvidenceFormatSchema.parseProvenance "asserted") (Some Asserted) "asserted parses"
            Expect.equal (EvidenceFormatSchema.parseProvenance "fabricated") None "unknown value rejected"
        }

        // --- T006: evidence-formats.md mirrors the 9th provenance column ------
        test "T006 the shipped evidence-formats.md documents the Provenance column" {
            let doc = read "template/base/docs/evidence-formats.md"
            Expect.stringContains doc "Provenance" "the regenerated doc lists the 9th column"
            Expect.stringContains doc "provenance ∈ { captured, asserted }" "the doc documents the closed value set"
        }

        // --- T006: the schema single source carries the 9-column row ----------
        test "T006 skillLoadingColumns is the 9-column row ending in Provenance" {
            Expect.equal (List.length EvidenceFormatSchema.skillLoadingColumns) 9 "row has nine columns"
            Expect.equal (List.last EvidenceFormatSchema.skillLoadingColumns) "Provenance" "ninth column is Provenance"
        }
    ]

// ---------------------------------------------------------------------------
// US-phase test sections appended in their own phases (kept in this file so the
// feature's red-green log is one place):
//
//   [<Tests>] let feature087Us1Tests = ...   // T007/T008 (FR-002)
//   [<Tests>] let feature087Us2Tests = ...   // T012 (FR-003/004)
//   [<Tests>] let feature087Us3Tests = ...   // T016 (FR-005/006)
//   [<Tests>] let feature087Us4Tests = ...   // T019 (FR-007/008/011)
//   [<Tests>] let feature087Us6Tests = ...   // T026 (FR-010)
// ---------------------------------------------------------------------------

// --- US4 (T019, FR-007/008/011): three-state verdict invariant -------------

let private mkSeh087 (unaccepted: string list) : SehSummary =
    { AcceptedSehTasks = []
      UnacceptedSyntheticTasks = unaccepted
      AutoSyntheticTasks = []
      LateSehTasks = []
      Diagnostics = [] }

let private mkDeferral087 (id: string) : AcceptedDeferral =
    { TaskId = id
      Justification = sprintf "%s deferred: host capability not yet available" id
      RealEvidencePath = sprintf "specs/087/readiness/%s-real.txt" id
      AwaitedHostCapability = "native-keystroke-injection" }

/// verdict with only diff-blocking as the blocking lever (other counts 0).
let private verdict087 (unaccepted: string list) (deferrals: AcceptedDeferral list) (blocking: int) =
    Audit.verdict [] (mkSeh087 unaccepted) deferrals blocking 0 0 0 0 0

[<Tests>]
let feature087Us4Tests =
    testList "feature 087 US4 — three-state verdict (FR-007/008/011)" [

        // Three seeded inputs → three distinct verdicts (T022, SC-007).
        test "FR-007 SC-007 three seeded inputs produce Pass / PassWithAcceptedDeferrals / Fail" {
            Expect.equal (verdict087 [] [] 0).Verdict Pass "clean: no synthetic, no blocking → Pass"
            Expect.equal
                (verdict087 [ "T900" ] [ mkDeferral087 "T900" ] 0).Verdict
                PassWithAcceptedDeferrals
                "one synthetic, accepted via deferral, no blocking → PassWithAcceptedDeferrals"
            Expect.equal (verdict087 [ "T900" ] [] 0).Verdict Fail "one UNaccepted synthetic → Fail"
        }

        // FR-011 invariant: an accepted deferral can never mask a blocking hit.
        test "FR-011 an accepted deferral never masks a blocking hit" {
            let res = verdict087 [ "T900" ] [ mkDeferral087 "T900" ] 1   // accepted synthetic BUT a diff-scan blocker
            Expect.equal res.Verdict Fail "a blocking hit forces Fail even with every synthetic accepted"
            Expect.equal res.AcceptedSyntheticCount 1 "the synthetic is still counted as accepted"
            Expect.equal res.UnacceptedSyntheticCount 0 "no unaccepted synthetic remains"
        }

        // FR-007/008: counts are separated and the deferral records are surfaced.
        test "FR-008 accepted/unaccepted counts are separated and deferrals recorded" {
            let res = verdict087 [ "T900"; "T901" ] [ mkDeferral087 "T900" ] 0
            Expect.equal res.AcceptedSyntheticCount 1 "one accepted"
            Expect.equal res.UnacceptedSyntheticCount 1 "one unaccepted → still Fail"
            Expect.equal res.Verdict Fail "an unaccepted synthetic remains → Fail"
            Expect.equal (List.length res.AcceptedDeferrals) 1 "the accepted deferral is recorded"
        }

        // FsCheck (T019): the full FR-011 invariant over arbitrary inputs.
        test "FR-011 PassWithAcceptedDeferrals requires zero unaccepted synthetic AND zero blocking (FsCheck)" {
            Check.QuickThrowOnFailure(fun (rawIds: int list) (acceptCount: int) (blockCount: int) ->
                let ids = rawIds |> List.map (fun i -> sprintf "T%03d" (abs i % 1000)) |> List.distinct
                let acceptN = if List.isEmpty ids then 0 else (abs acceptCount) % (List.length ids + 1)
                let deferrals = ids |> List.truncate acceptN |> List.map mkDeferral087
                let blk = (abs blockCount) % 4
                let res = Audit.verdict [] (mkSeh087 ids) deferrals blk 0 0 0 0 0
                let shouldFail = res.UnacceptedSyntheticCount > 0 || blk > 0
                let failInvariant = (res.Verdict = Fail) = shouldFail
                let pwadInvariant =
                    match res.Verdict with
                    | PassWithAcceptedDeferrals ->
                        res.UnacceptedSyntheticCount = 0 && blk = 0 && res.AcceptedSyntheticCount > 0
                    | _ -> true
                failInvariant && pwadInvariant)
        }

        // T021/FR-008: durable accepted-deferral records parse from JSON.
        test "FR-008 parseAcceptedDeferrals reads taskId + justification, rejects empty justification" {
            let json =
                """{ "acceptedDeferrals": [
                       { "taskId": "T900", "justification": "deferred", "realEvidencePath": "p", "awaitedHostCapability": "c" },
                       { "taskId": "T901", "justification": "", "realEvidencePath": "p", "awaitedHostCapability": "c" } ] }"""
            let parsed = Audit.parseAcceptedDeferrals (Some json)
            Expect.equal (List.length parsed) 1 "the empty-justification record is rejected (Constitution §V)"
            Expect.equal parsed.[0].TaskId "T900" "the valid record is parsed"
            Expect.equal (Audit.parseAcceptedDeferrals None) [] "absent file → no deferrals"
            Expect.equal (Audit.parseAcceptedDeferrals (Some "not json")) [] "malformed JSON → no deferrals (tolerant)"
        }
    ]

// --- US5 (T023, FR-009): [S*] propagation over ExplicitDeps only -----------

let private emptySeh087 : SehMetadata =
    { Annotation = false
      ApprovalLabel = false
      DesignSource = ""
      SyntheticInputClass = ""
      ExpectedErrorBehavior = ""
      Rationale = ""
      AcceptanceStatus = ""
      Diagnostics = [] }

let private mkTask087 (id: string) (declared: DeclaredStatus) (explicit: string list) (phase: string list) : TaskRecord =
    { Id = id
      Declared = declared
      Phase = None
      Story = None
      Tier = None
      Parallel = false
      LineNo = 0
      Title = id
      SkillistMirror = Some []
      Skillist = []
      ExplicitDeps = explicit
      PhaseDeps = phase
      SkillMatchAssessments = []
      Seh = emptySeh087 }

let private effOf (tasks: TaskRecord list) (id: string) =
    let g = Graph.ofTasks tasks
    let resolved, _ = Graph.propagate g (Graph.topoSort g)
    resolved |> List.find (fun r -> r.Task.Id = id) |> fun r -> r.Effective

[<Tests>]
let feature087Us5Tests =
    testList "feature 087 US5 — [S*] propagation over ExplicitDeps only (FR-009)" [

        // A leaf [S] whose output nothing CONSUMES (only a phase-checkpoint edge
        // points at it) must NOT taint the downstream task. Fails before T024
        // (taint filtered over allDeps), passes after (ExplicitDeps only).
        test "FR-009 a phase-edge-only downstream of an [S] leaf is never [S*]" {
            let tasks =
                [ mkTask087 "T100" Synthetic [] []                 // leaf [S], nothing consumes it
                  mkTask087 "T200" Done [] [ "T100" ] ]            // phase-checkpoint edge ONLY
            Expect.equal (effOf tasks "T200") (DeclaredEff Done)
                "a phase-checkpoint edge alone must not propagate [S*]"
        }

        // Positive control (guards against over-correction): a REAL data
        // dependency on the same [S] leaf still propagates [S*]. Passes before
        // and after T024.
        test "FR-009 a real ExplicitDeps edge to an [S] leaf still propagates [S*]" {
            let tasks =
                [ mkTask087 "T100" Synthetic [] []
                  mkTask087 "T300" Done [ "T100" ] [] ]           // real data dep
            Expect.equal (effOf tasks "T300") AutoSynthetic
                "a real ExplicitDeps edge to a synthetic leaf must propagate [S*]"
        }

        // FsCheck (T023): for any synthetic leaf, the downstream is [S*] iff the
        // edge is an ExplicitDeps edge — never when it is phase-only.
        test "FR-009 taint follows ExplicitDeps iff explicit (FsCheck)" {
            let downstreamTainted (explicitEdge: bool) =
                let explicit = if explicitEdge then [ "T100" ] else []
                let phase = if explicitEdge then [] else [ "T100" ]
                let tasks =
                    [ mkTask087 "T100" Synthetic [] []
                      mkTask087 "T200" Done explicit phase ]
                effOf tasks "T200" = AutoSynthetic
            Check.QuickThrowOnFailure(fun (explicitEdge: bool) ->
                downstreamTainted explicitEdge = explicitEdge)
        }
    ]

// --- US6 (T026, FR-010): skill-loading provenance + at-implementation gap ---

let private mkSkillTask087 (id: string) (declared: DeclaredStatus) (skills: string list) : TaskRecord =
    { mkTask087 id declared [] [] with
        Skillist = skills
        SkillistMirror = Some skills }

[<Tests>]
let feature087Us6Tests =
    testList "feature 087 US6 — skill-loading provenance + at-implementation gap (FR-010)" [

        // A present provenance value outside { captured, asserted } is rejected;
        // a valid value passes; a legacy 8-column row (no provenance) is lenient.
        test "FR-010 a present provenance must be captured|asserted (legacy 8-col lenient)" {
            let skillPath = ".agents/skills/fsharp-parsing/SKILL.md"
            let skills = Map.ofList [ "fsharp-parsing", [ skillPath ] ]
            let row prov =
                sprintf "| T500 | fsharp-parsing | %s | loaded | 2026-06-09T10:00:00Z | 2026-06-09T11:00:00Z | x.fs | none | %s |" skillPath prov
            let tasks = [ mkSkillTask087 "T500" Done [ "fsharp-parsing" ] ]
            let validate text =
                Audit.validateSkillLoadingEvidence tasks skills (Some text) (fun _ -> true) id
            let bogus = validate (row "bogus")
            Expect.isTrue (bogus |> List.exists (fun e -> e.Contains "invalid provenance")) "bogus provenance is rejected"
            let captured = validate (row "captured")
            Expect.isFalse (captured |> List.exists (fun e -> e.Contains "provenance")) "captured is accepted"
            // legacy 8-column row (no 9th cell) — no provenance error.
            let legacy =
                validate (sprintf "| T500 | fsharp-parsing | %s | loaded | 2026-06-09T10:00:00Z | 2026-06-09T11:00:00Z | x.fs | none |" skillPath)
            Expect.isFalse (legacy |> List.exists (fun e -> e.Contains "provenance")) "legacy 8-column row is lenient"
        }

        // Existing ordering rule unchanged: loaded_at must precede work_started_at.
        test "FR-010 existing loaded_at < work_started_at rule is unchanged" {
            let skillPath = ".agents/skills/fsharp-parsing/SKILL.md"
            let skills = Map.ofList [ "fsharp-parsing", [ skillPath ] ]
            let reversed =
                sprintf "| T500 | fsharp-parsing | %s | loaded | 2026-06-09T11:00:00Z | 2026-06-09T10:00:00Z | x.fs | none | captured |" skillPath
            let tasks = [ mkSkillTask087 "T500" Done [ "fsharp-parsing" ] ]
            let errs = Audit.validateSkillLoadingEvidence tasks skills (Some reversed) (fun _ -> true) id
            Expect.isTrue (errs |> List.exists (fun e -> e.Contains "loaded_at must be earlier")) "reverse timestamps still rejected"
        }

        // The at-implementation gap reports a PENDING declaring task's missing
        // load — not deferred to the [X] flip (which validateSkillLoadingEvidence
        // only enforces for Done/Synthetic).
        test "FR-010 SC-009 declared-but-unloaded skill surfaces at implementation time" {
            let skillPath = ".agents/skills/fsharp-parsing/SKILL.md"
            let doneRow =
                sprintf "| T500 | fsharp-parsing | %s | loaded | 2026-06-09T10:00:00Z | 2026-06-09T11:00:00Z | x.fs | none | captured |" skillPath
            let tasks =
                [ mkSkillTask087 "T500" Done [ "fsharp-parsing" ]
                  mkSkillTask087 "T501" Pending [ "fsharp-parsing" ] ]   // pending, declares a skill, no row
            let gaps = Audit.skillLoadingGapsAtImplementation tasks (Some doneRow)
            Expect.equal gaps [ ("T501", "fsharp-parsing") ]
                "the pending declaring task's missing load surfaces before [X]"
            // And the loaded Done task is NOT reported as a gap.
            Expect.isFalse (gaps |> List.contains ("T500", "fsharp-parsing")) "a loaded skill is not a gap"
        }
    ]

// --- Phase 9 (T031, FR-011): every true-positive gate still blocks -----------

[<Tests>]
let feature087Fr011Tests =
    testList "feature 087 FR-011 — true-positive gates still block (SC-010)" [

        // diff-scan: an added placeholder marker in the diff is a blocking hit. The marker is
        // built at runtime by concatenation so THIS test source carries no literal diff-scan
        // token (the same idiom the engine uses for the removed-Charts reference).
        test "FR-011 SC-010 diff-scan still blocks a seeded placeholder marker" {
            let patternsYml = read ".specify/extensions/evidence/audit-patterns.yml"
            let marker = "TO" + "DO"
            let diff =
                "diff --git a/src/Product/Seed.fs b/src/Product/Seed.fs\n"
                + "+++ b/src/Product/Seed.fs\n"
                + "@@ -0,0 +1,1 @@\n"
                + sprintf "+    let placeholder = 1 // %s: seeded violation\n" marker
            let result = DiffScan.scan None patternsYml diff
            Expect.isNonEmpty result.Blocking "a seeded placeholder marker in the diff must still block"
        }

        // additive-surface: a REMOVED public symbol (a non-additive surface change) is drift.
        test "FR-011 SC-010 a non-additive surface removal still drifts" {
            let baseline: FS.Skia.UI.Build.PerPackageSurface.Surface =
                { PackageId = "FS.Skia.UI.Sample"; NormalizedText = "type T =\n    member X: int\n    member Y: int" }
            let current: FS.Skia.UI.Build.PerPackageSurface.Surface =
                { PackageId = "FS.Skia.UI.Sample"; NormalizedText = "type T =\n    member X: int" }   // Y removed
            match FS.Skia.UI.Build.PerPackageSurface.diffPackage baseline current with
            | Some drift -> Expect.isNonEmpty drift.Changes "removing a public member must still be flagged as drift"
            | None -> failtest "a removed public member must produce drift (non-additive surface change still blocks)"
        }

        // window-visibility: the trigger fires but the required readiness files are absent.
        test "FR-011 SC-010 window-visibility still blocks when required files are absent" {
            let input: ScanInput =
                { ReadinessDir = "/x/readiness"
                  FeatureText = "this feature touches interactive-visible-window.md"
                  ReadinessFiles = [] }
            let result = Scans.windowVisibility input
            Expect.isGreaterThan result.BlockingCount 0 "a triggered window-visibility scan with no evidence must still block"
        }

        // persistent-launch: an interactive-window feature with no supported-host evidence.
        test "FR-011 SC-010 persistent-launch still blocks when supported-host evidence is absent" {
            let input: ScanInput =
                { ReadinessDir = "/x/readiness"
                  FeatureText = "the generated app launches mode=interactive-window"
                  ReadinessFiles = [] }
            let result = Scans.persistentLaunch input
            Expect.isGreaterThan result.BlockingCount 0 "a persistent-launch feature with no supported-host evidence must still block"
        }

        // synthetic-honesty: an UNaccepted synthetic task forces Fail (the three-state verdict).
        test "FR-011 SC-010 an unaccepted synthetic still fails the audit verdict" {
            let res = verdict087 [ "T900" ] [] 0
            Expect.equal res.Verdict Fail "an unaccepted synthetic must still fail the audit (no relaxation)"
        }
    ]

// --- US1 (T007, FR-002): per-step product-defect vs environment aggregation -

[<Tests>]
let feature087Us1Tests =
    testList "feature 087 US1 — per-step product-defect vs environment verdict (FR-002)" [

        let step name passed cls : GeneratedProductStepResult =
            { Step = name; Passed = passed; Classification = cls; PackageSet = Pinned }

        // SC-001: every step passing → ProductPass (a real green).
        test "FR-002 SC-001 all steps passing is ProductPass" {
            let steps = [ step "Verify" true ProductDefect; step "smoke" true Environment ]
            Expect.equal (EvidenceFormatSchema.generatedProductVerdict steps) ProductPass "clean run passes"
        }

        // SC-002: an Environment classification never suppresses a ProductDefect in the SAME
        // run — a failing ProductDefect step forces ProductDefectFail even alongside a failing
        // Environment step. Fails to compile before T007/T010 add the verdict; passes after.
        test "FR-002 SC-002 an Environment failure never suppresses a ProductDefect failure" {
            let steps =
                [ step "Verify" false ProductDefect      // real product defect
                  step "window-options" false Environment ] // concurrent env obstacle
            Expect.equal (EvidenceFormatSchema.generatedProductVerdict steps) ProductDefectFail
                "a product defect is authoritative even with a concurrent environment obstacle"
        }

        // An env-only failure is non-authoritative (reported, not a defect, not a clean green).
        test "FR-002 an environment-only failure is EnvironmentNonAuthoritative" {
            let steps = [ step "Verify" true ProductDefect; step "image-evidence" false Environment ]
            Expect.equal (EvidenceFormatSchema.generatedProductVerdict steps) EnvironmentNonAuthoritative
                "an environment-only obstacle is non-authoritative"
        }

        // The failure-category → classification map: product/contract categories are
        // ProductDefect; only a genuine unsupported-host obstacle is Environment.
        test "FR-002 classifyGeneratedCategory keeps product defects authoritative" {
            Expect.equal (EvidenceFormatSchema.classifyGeneratedCategory "SemanticTestFailure") ProductDefect "Verify failure is a product defect"
            Expect.equal (EvidenceFormatSchema.classifyGeneratedCategory "PackageDrift") ProductDefect "package drift is a product defect"
            Expect.equal (EvidenceFormatSchema.classifyGeneratedCategory "UnsupportedHost") Environment "an unsupported host is an environment obstacle"
        }

        // FsCheck: verdict = ProductDefectFail iff at least one failing ProductDefect step,
        // regardless of how many Environment failures accompany it (SC-002 invariant).
        test "FR-002 verdict is ProductDefectFail iff a ProductDefect step failed (FsCheck)" {
            Check.QuickThrowOnFailure(fun (flags: (bool * bool) list) ->
                let steps =
                    flags
                    |> List.mapi (fun i (passed, isDefect) ->
                        step (sprintf "s%d" i) passed (if isDefect then ProductDefect else Environment))
                let hasDefectFail = steps |> List.exists (fun s -> not s.Passed && s.Classification = ProductDefect)
                (EvidenceFormatSchema.generatedProductVerdict steps = ProductDefectFail) = hasDefectFail)
        }
    ]

// --- US2 (T012, FR-003/004): static pinned-vs-local package-skew detection --

module Skew = FS.Skia.UI.Build.PackageSkew

[<Tests>]
let feature087Us2Tests =
    testList "feature 087 US2 — static pinned-vs-local package-skew (FR-003/004)" [

        // A symbol referenced by generated source that is present in the local-packed build
        // but ABSENT from the pinned/published surface is a skew finding naming symbol +
        // file + version gap. A referenced symbol present in the surface is not. Fails to
        // compile before T013 adds the module; passes after.
        test "FR-003 SC-003 a referenced symbol absent from the pinned surface is a finding" {
            // Pinned/published surface has Control + Layout, but NOT the unreleased `Bounds`.
            let pinned = Set.ofList [ "Control"; "ControlRenderResult"; "Layout"; "render" ]
            let referenced =
                [ "Bounds", "tests/Product.Tests/BehaviorTests.fs"   // unreleased member → skew
                  "Control", "src/Product/View.fs" ]                 // present in pinned → fine
            let findings = Skew.detectSkew "0.1.92-preview.1" "0.1.93-preview.1" pinned referenced
            Expect.equal (List.length findings) 1 "exactly the unreleased reference is a finding"
            let f = findings.[0]
            Expect.equal f.Symbol "Bounds" "names the skewed symbol"
            Expect.equal f.File "tests/Product.Tests/BehaviorTests.fs" "names the referencing file"
            Expect.equal f.PinnedVersion "0.1.92-preview.1" "names the pinned version"
            Expect.equal f.LocalVersion "0.1.93-preview.1" "names the local version gap"
        }

        // SC-004: when every referenced symbol resolves in the pinned surface, the finding
        // set is empty — the real-tree-clean case, computed with no restore.
        test "FR-003 SC-004 all references resolved in the pinned surface yields no finding" {
            let pinned = Set.ofList [ "Control"; "render"; "Widget" ]
            let referenced = [ "Control", "a.fs"; "render", "b.fs" ]
            Expect.equal (Skew.detectSkew "v" "v" pinned referenced) [] "no skew when all references resolve"
        }

        // The surface extractor recovers declared type/member/field names from `.fsi` text,
        // and the reference extractor recovers FS.Skia.UI-rooted references from source.
        test "FR-003 surface and reference extraction recover the expected symbols" {
            let fsi =
                "namespace FS.Skia.UI.Sample\n\ntype ControlRenderResult =\n    { Bounds: int }\n    member _.Render () = ()\n"
            let surf = Skew.surfaceSymbols fsi
            Expect.isTrue (Set.contains "ControlRenderResult" surf) "type name extracted"
            Expect.isTrue (Set.contains "Bounds" surf) "record field extracted"
            Expect.isTrue (Set.contains "Render" surf) "member extracted"
            let tracked = Set.ofList [ "FS.Skia.UI.Controls"; "FS.Skia.UI.Scene" ]
            let refs =
                Skew.referencedSymbols tracked "p.fs" "open FS.Skia.UI.Controls\nlet x = FS.Skia.UI.Scene.Color.rgb 1 2 3\n"
            let names = refs |> List.map fst |> Set.ofList
            Expect.isTrue (Set.contains "rgb" names) "fully-qualified leaf reference extracted"
            Expect.isTrue (Set.contains "Controls" names) "opened namespace leaf extracted"
            // A reference rooted at the non-tracked build-tooling package contributes nothing.
            let buildRefs = Skew.referencedSymbols tracked "b.fs" "open FS.Skia.UI.Build\n"
            Expect.isEmpty buildRefs "a non-tracked (build-tooling) reference is out of scope"
        }

        // FsCheck: a finding exists for a referenced symbol iff it is absent from the pinned
        // surface (the set-difference invariant), for arbitrary symbol/surface pairs.
        test "FR-003 a reference is a finding iff absent from the pinned surface (FsCheck)" {
            Check.QuickThrowOnFailure(fun (symRaw: int) (inSurface: bool) ->
                let sym = sprintf "Sym%d" (abs symRaw % 100)
                let pinned = if inSurface then Set.ofList [ sym ] else Set.empty
                let findings = Skew.detectSkew "p" "l" pinned [ sym, "f.fs" ]
                List.isEmpty findings = inSurface)
        }

        // SC-004: the REAL tree — every FS.Skia.UI reference the actual generated template
        // source/tests make resolves in the actual captured per-package surface baselines.
        // Computed statically over committed files, no restore.
        test "FR-003 SC-004 the real generated template references all resolve in the captured surface" {
            let root = GovernanceTestSupport.repositoryRoot
            let pinnedSurface =
                let dir = System.IO.Path.Combine(root, "readiness", "per-package-surface")
                System.IO.Directory.GetFiles(dir, "*.fsi.txt")
                |> Array.collect (fun f -> Skew.surfaceSymbols (System.IO.File.ReadAllText f) |> Set.toArray)
                |> Set.ofArray
            let tracked = Set.ofList FS.Skia.UI.Build.PerPackageSurface.packagesInScope
            let referenced =
                [ System.IO.Path.Combine(root, "template", "base", "src")
                  System.IO.Path.Combine(root, "template", "base", "tests") ]
                |> List.filter System.IO.Directory.Exists
                |> List.collect (fun r ->
                    System.IO.Directory.GetFiles(r, "*.fs", System.IO.SearchOption.AllDirectories)
                    |> Array.toList
                    |> List.collect (fun f -> Skew.referencedSymbols tracked f (System.IO.File.ReadAllText f)))
            let findings = Skew.detectSkew "0.1.92-preview.1" "0.1.92-preview.1" pinnedSurface referenced
            Expect.isEmpty findings (sprintf "real tree must be skew-clean; got %A" findings)
        }

        // SC-003 on the real surface: a generated reference to an unreleased FS.Skia.UI symbol
        // (the 086 near-miss class) is caught against the real captured surface.
        test "FR-003 SC-003 a seeded unreleased FS.Skia.UI reference is caught against the real surface" {
            let root = GovernanceTestSupport.repositoryRoot
            let pinnedSurface =
                let dir = System.IO.Path.Combine(root, "readiness", "per-package-surface")
                System.IO.Directory.GetFiles(dir, "*.fsi.txt")
                |> Array.collect (fun f -> Skew.surfaceSymbols (System.IO.File.ReadAllText f) |> Set.toArray)
                |> Set.ofArray
            // A fully-qualified reference to a symbol that exists in no captured surface.
            let tracked = Set.ofList FS.Skia.UI.Build.PerPackageSurface.packagesInScope
            let seeded =
                Skew.referencedSymbols
                    tracked
                    "tests/Product.Tests/BehaviorTests.fs"
                    "let x = FS.Skia.UI.Controls.ControlRenderResult.UnreleasedBoundsV087\n"
            let findings = Skew.detectSkew "0.1.92-preview.1" "0.1.93-preview.1" pinnedSurface seeded
            Expect.isTrue
                (findings |> List.exists (fun f -> f.Symbol = "UnreleasedBoundsV087"))
                "the seeded unreleased reference is flagged"
        }
    ]

// --- feature 107 (US1, FR-001/002/003): package-skew hardening ---------------
// The recommended typed authoring path must pass governance with zero false findings:
// comment-only tokens contribute nothing, the typed front door resolves against the
// broadened captured surface, and genuinely-absent symbols are still caught.

[<Tests>]
let feature107SkewHardeningTests =
    testList "feature 107 US1 — package-skew hardening (FR-001/002/003)" [

        // FR-001 / SC-001: an `FS.Skia.UI.…` token that appears ONLY inside a comment (line,
        // doc, or block) is not a referenced symbol — prose/doc-comments naming a framework
        // namespace no longer produce a false skew finding.
        test "FR-001 a token only inside comments is not a referenced symbol" {
            let tracked = Set.ofList FS.Skia.UI.Build.PerPackageSurface.packagesInScope
            let source =
                "// see FS.Skia.UI.Controls.Typed for the typed front door\n"
                + "/// Authored through FS.Skia.UI.Controls.Typed.Label.view.\n"
                + "(* FS.Skia.UI.Scene.Color.rgb is documented here *)\n"
                + "let x = 1\n"
            Expect.isEmpty
                (Skew.referencedSymbols tracked "src/View.fs" source)
                "no reference is extracted from comment-only FS.Skia.UI tokens"
        }

        // FR-001 edge: a symbol appearing in BOTH a comment and live code is still found via its
        // live-code occurrence (comment-stripping must not blind the check to real references).
        test "FR-001 a symbol in both a comment and live code is still found via live code" {
            let tracked = Set.ofList FS.Skia.UI.Build.PerPackageSurface.packagesInScope
            let source =
                "// mentions FS.Skia.UI.Scene.Color.rgb in prose\n"
                + "let c = FS.Skia.UI.Scene.Color.rgb 1 2 3\n"
            let names =
                Skew.referencedSymbols tracked "src/View.fs" source |> List.map fst |> Set.ofList
            Expect.isTrue (Set.contains "rgb" names) "the live-code reference is still extracted"
        }

        // FR-002 / SC-005: captureCurrent now recurses into Controls/Widgets, so the typed front
        // door `FS.Skia.UI.Controls.Typed` and its members are known symbols — `open …Typed` and a
        // qualified typed-member reference are skew-clean (no per-control alias work-around needed).
        test "FR-002 the typed front door resolves against the broadened captured surface" {
            let controlsSurface =
                FS.Skia.UI.Build.PerPackageSurface.captureCurrent [ "FS.Skia.UI.Controls" ]
                |> List.head
                |> fun s -> Skew.surfaceSymbols s.NormalizedText
            Expect.isTrue (Set.contains "Typed" controlsSurface) "the Typed namespace segment is captured"
            Expect.isTrue (Set.contains "Label" controlsSurface) "a typed-front-door module is captured"
            Expect.isTrue (Set.contains "view" controlsSurface) "a typed-front-door member is captured"
            let tracked = Set.ofList FS.Skia.UI.Build.PerPackageSurface.packagesInScope
            let referenced =
                Skew.referencedSymbols
                    tracked
                    "src/View.fs"
                    "open FS.Skia.UI.Controls.Typed\nlet v = FS.Skia.UI.Controls.Typed.Label.view\n"
            Expect.isEmpty
                (Skew.detectSkew "v" "v" controlsSurface referenced)
                "open Typed + a qualified typed member resolve clean against the broadened surface"
        }

        // FR-003: the narrowing must not blind real detection — a typed-front-door reference to a
        // member that exists in NO captured surface is still a finding against the broadened surface.
        test "FR-003 an absent typed-front-door member is still a skew finding" {
            let controlsSurface =
                FS.Skia.UI.Build.PerPackageSurface.captureCurrent [ "FS.Skia.UI.Controls" ]
                |> List.head
                |> fun s -> Skew.surfaceSymbols s.NormalizedText
            let tracked = Set.ofList FS.Skia.UI.Build.PerPackageSurface.packagesInScope
            let referenced =
                Skew.referencedSymbols
                    tracked
                    "src/View.fs"
                    "let v = FS.Skia.UI.Controls.Typed.Label.unreleasedTypedMemberV107\n"
            Expect.isTrue
                (Skew.detectSkew "p" "l" controlsSurface referenced
                 |> List.exists (fun f -> f.Symbol = "unreleasedTypedMemberV107"))
                "a typo'd/unreleased typed member is still caught (capture broadening did not blanket-pass)"
        }
    ]

// --- US3 (T016, FR-005/006): per-package baseline byte-idempotence ----------

module PPS = FS.Skia.UI.Build.PerPackageSurface

[<Tests>]
let feature087Us3Tests =
    testList "feature 087 US3 — per-package baseline byte-idempotence (FR-005/006)" [

        // The canonical serialization is a byte-stable fixed point: exactly one trailing
        // newline, and serializing again after a read-and-renormalize round-trip is
        // byte-equal — the trailing-newline/whitespace churn the rule must absorb (SC-006).
        // Fails to compile before T017 adds serializeBaseline; passes after.
        test "FR-006 SC-006 serializeBaseline is a byte-stable fixed point (no trailing-newline churn)" {
            let s: PPS.Surface =
                { PackageId = "FS.Skia.UI.Sample"
                  NormalizedText = "namespace A\n\ntype T = { X: int }" }
            let once = PPS.serializeBaseline s
            Expect.isTrue (once.EndsWith "}\n") "ends with exactly one newline"
            Expect.isFalse (once.EndsWith "}\n\n") "no double trailing newline"
            // Trailing-newline churn is absorbed: a surface text already carrying trailing
            // newlines serializes to exactly the same single-newline bytes.
            let withTrailingNls = { s with NormalizedText = s.NormalizedText + "\n\n\n" }
            Expect.equal (PPS.serializeBaseline withTrailingNls) once "trailing newlines collapse to exactly one"
            // Re-reading the written text (normalize, as loadBaselines does) and
            // re-serializing is byte-equal — a re-capture on an unchanged tree is byte-stable.
            let reread = { s with NormalizedText = PPS.normalize once }
            Expect.equal (PPS.serializeBaseline reread) once "writing twice on an unchanged surface is byte-equal"
        }

        // Capturing the REAL in-scope source surfaces twice serializes byte-equal, so one
        // RefreshSurfaceBaselines run followed by another leaves git status clean (SC-005/006).
        test "FR-005 capturing the real in-scope surfaces twice serializes byte-equal" {
            let ser () =
                PPS.captureCurrent PPS.packagesInScope |> List.map PPS.serializeBaseline
            Expect.equal (ser ()) (ser ()) "two captures serialize byte-equal"
        }
    ]
