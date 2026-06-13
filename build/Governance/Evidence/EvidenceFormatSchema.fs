namespace FS.Skia.UI.Build.Evidence

// --- feature 087 single-source engine value types -------------------------
//
// These six types are the single source for the governance-gate-hardening
// changes. They live here (the earliest-compiled Evidence file) so the audit
// verdict (Audit.fs), the JSON renderers (Render.fs), and the interpreter edge
// (Front/Governance.fs) all reference ONE definition and cannot drift. Pure
// data; visibility lives in the .fsi (Principle II).

/// FR-007: the three-state merge-gate verdict (replaces the binary Pass|Fail).
/// `PassWithAcceptedDeferrals` is reachable ONLY with zero unaccepted synthetic
/// and zero blocking hits (the FR-011 invariant); it can never mask a block.
type AuditVerdict =
    | Pass
    | PassWithAcceptedDeferrals
    | Fail

/// FR-008: a durable accepted-deferral record, written to
/// readiness/synthetic-evidence.json (not solely a logged flag). Counts feed
/// seh-audit-summary.json.
type AcceptedDeferral =
    { TaskId: string
      Justification: string
      RealEvidencePath: string
      AwaitedHostCapability: string }

/// FR-010: skill-loading-evidence provenance — `Captured` (observed during the
/// run, recorded at the load action before code changes) vs `Asserted`
/// (hand-authored timestamp).
type LoadProvenance =
    | Captured
    | Asserted

/// FR-002: per-step generated-product failure classification. Only meaningful on
/// a failing step; an `Environment` step never suppresses a `ProductDefect`.
type StepClassification =
    | ProductDefect
    | Environment

/// FR-004: the explicit package-source tag every generated-product report states
/// (LocalPacked = TemplateCheck's locally-packed .nupkg; Pinned =
/// GeneratedProductCheck's pinned/published version).
type PackageSet =
    | LocalPacked
    | Pinned

/// FR-002: a per-step generated-product result. `Classification` is only meaningful when
/// `Passed = false`. `PackageSet` records which package source the step exercised (FR-004).
type GeneratedProductStepResult =
    { Step: string
      Passed: bool
      Classification: StepClassification
      PackageSet: PackageSet }

/// FR-002: the overall generated-product verdict aggregated over per-step results.
type GeneratedProductVerdict =
    | ProductPass
    | EnvironmentNonAuthoritative
    | ProductDefectFail

/// FR-003: a static pinned-vs-local package-skew finding — a public-API symbol
/// referenced by generated source/tests that is present in the local-packed
/// surface but absent from the pinned surface. Computed from existing surface
/// baselines, no network restore. A non-empty finding set blocks before merge.
type PackageSkewFinding =
    { Symbol: string
      File: string
      PinnedVersion: string
      LocalVersion: string }

type EvidenceFormatClass =
    | ReadinessContract
    | SkillLoadingEvidence
    | WindowVisibility
    | SehAcceptance

type EvidenceFormatSchema =
    { FileName: string
      FormatClass: EvidenceFormatClass
      RequiredTokens: string list
      TableColumns: string list option
      OrderingRules: string list
      ResolvedPathPattern: string option
      Blocking: bool }

module EvidenceFormatSchema =

    let classLabel (c: EvidenceFormatClass) : string =
        match c with
        | ReadinessContract -> "readiness-contract"
        | SkillLoadingEvidence -> "skill-loading-evidence"
        | WindowVisibility -> "window-visibility"
        | SehAcceptance -> "seh-acceptance"

    // --- feature 087 label/parse helpers (single source) --------------------

    /// The JSON/string label for an `AuditVerdict` (the C1 verdict-state enum).
    let auditVerdictLabel (v: AuditVerdict) : string =
        match v with
        | Pass -> "Pass"
        | PassWithAcceptedDeferrals -> "PassWithAcceptedDeferrals"
        | Fail -> "Fail"

    /// Whether a verdict is a passing state (`Pass` or `PassWithAcceptedDeferrals`).
    let isPassingVerdict (v: AuditVerdict) : bool =
        match v with
        | Pass
        | PassWithAcceptedDeferrals -> true
        | Fail -> false

    /// The JSON/string label for a `StepClassification` (C5).
    let stepClassificationLabel (c: StepClassification) : string =
        match c with
        | ProductDefect -> "ProductDefect"
        | Environment -> "Environment"

    /// The JSON/string label for a `PackageSet` (C4/C5).
    let packageSetLabel (p: PackageSet) : string =
        match p with
        | LocalPacked -> "LocalPacked"
        | Pinned -> "Pinned"

    /// The JSON/string label for a `GeneratedProductVerdict` (FR-002).
    let generatedProductVerdictLabel (v: GeneratedProductVerdict) : string =
        match v with
        | ProductPass -> "ProductPass"
        | EnvironmentNonAuthoritative -> "EnvironmentNonAuthoritative"
        | ProductDefectFail -> "ProductDefectFail"

    /// FR-002: a genuine host-environment obstacle classifies as `Environment`; every
    /// product/contract failure category classifies as `ProductDefect`. Single-sourced so
    /// the engine and any reporter agree on which categories are non-authoritative.
    let classifyGeneratedCategory (category: string) : StepClassification =
        match category with
        | "UnsupportedHost"
        | "Completed" -> Environment
        | _ -> ProductDefect

    /// FR-002 (SC-002): the overall verdict — `ProductDefectFail` iff any step failed as a
    /// `ProductDefect`; an `Environment` failure can never suppress a `ProductDefect` in the
    /// same run (each step classified independently).
    let generatedProductVerdict (steps: GeneratedProductStepResult list) : GeneratedProductVerdict =
        if steps |> List.exists (fun s -> not s.Passed && s.Classification = ProductDefect) then
            ProductDefectFail
        elif steps |> List.exists (fun s -> not s.Passed) then
            EnvironmentNonAuthoritative
        else
            ProductPass

    /// The skill-loading-evidence `provenance` column value (C3, FR-010).
    let loadProvenanceLabel (p: LoadProvenance) : string =
        match p with
        | Captured -> "captured"
        | Asserted -> "asserted"

    /// Parse a `provenance` column value; `None` for any value outside the
    /// closed `{ captured, asserted }` set.
    let parseProvenance (s: string) : LoadProvenance option =
        match s.Trim().ToLowerInvariant() with
        | "captured" -> Some Captured
        | "asserted" -> Some Asserted
        | _ -> None

    // --- single-source constant lists ---------------------------------------

    // Feature 087 (FR-010): the row gains a 9th `Provenance` column
    // (`captured` | `asserted`). Single-sourced here so the validator, the
    // printed schema, and the generated evidence-formats.md share one list.
    let skillLoadingColumns =
        [ "TaskId"; "DeclaredSkillId"; "ResolvedSkillPath"; "LoadResult"; "LoadedAt"
          "WorkStartedAt"; "EvidencePath"; "Exception"; "Provenance" ]

    let skillLoadingOrderingRule = "loaded_at < work_started_at"

    // Feature 087 (FR-010): the closed value set for the 9th `Provenance` column.
    let skillLoadingProvenanceRule =
        "provenance ∈ { captured, asserted } (captured = observed during the run, recorded at the load action before code changes; asserted = hand-authored)"

    let skillLoadingPathPattern = ".agents/skills/<id>/SKILL.md"

    let windowDiagnosticClasses =
        [ "environment-session"; "window-visibility"; "app-lifecycle"; "product-defect" ]

    let interactiveVisibleWindowKeys =
        [ "status"; "mode"; "window-visible"; "accessible-window"; "first-frame-presented"
          "self-closed-for-evidence" ]

    // The complete window-visibility readiness file set the engine enforces, single-
    // sourced here so `Scans.windowVisibility` and the generated `evidence-formats.md`
    // reference the same ordered list and cannot drift (FR-007 / SC-003).
    let windowVisibilityFiles =
        [ "interactive-visible-window.md"; "close-reason-separation.md"; "window-state-diagnostics.md"
          "window-options.md"; "real-image-evidence.md"; "generated-validation.md"; "evidence-audit.md" ]

    let windowNativeFacts =
        [ "native-handle"; "visible"; "focusable"; "renderable-surface"; "input-devices" ]

    let windowOptionRows =
        [ "resize"; "maximize"; "startup-state"; "startup-position"; "backend" ]

    let closeReasonSeparationKeys =
        [ "close-reason"; "user-close-observed"; "evidence-close-observed" ]

    let realImageEvidenceKeys =
        [ "evidence-kind"; "status"; "artifact-decodable"; "proves-scene-rendering"; "proves-desktop-visibility" ]

    let generatedValidationKeys =
        [ "exact-package-match"; "generated-tests-ran"; "authoritative"; "failure-class" ]

    let sehAcceptanceTokens = [ "accepted-seh"; "synthetic-error-handling-approved" ]

    // The readiness-contract scan's enforced files, single-sourced here so
    // `Scans.readinessContract` and the generated reference share one list.
    let readinessContractChecks : (string * string list * string) list =
        [ "governance-risk-levels.md",
          [ "small"; "medium"; "broad"; "required evidence"; "broad validation" ],
          "governance risk level evidence is incomplete"
          "aggregate-hang-diagnostics.md",
          [ "verdict"; "stage"; "elapsed duration"; "last observed command"; "focused rerun"; "non-authoritative aggregate" ],
          "aggregate timeout verdict evidence is incomplete"
          "runtime-limitations.md",
          [ ".NET 10 desktop"; "OpenGL"; "SkiaSharp preview"; "unsupported macOS/mobile/browser"; "no software-renderer fallback" ],
          "runtime limitation evidence is incomplete" ]

    // --- the enumerated schema ----------------------------------------------

    let private readinessContractSchemas =
        readinessContractChecks
        |> List.map (fun (fileName, tokens, _) ->
            { FileName = fileName
              FormatClass = ReadinessContract
              RequiredTokens = tokens
              TableColumns = None
              OrderingRules = []
              ResolvedPathPattern = None
              Blocking = true })

    let schemas : EvidenceFormatSchema list =
        readinessContractSchemas
        @ [ { FileName = "skill-loading-evidence.md"
              FormatClass = SkillLoadingEvidence
              RequiredTokens = skillLoadingColumns
              TableColumns = Some skillLoadingColumns
              OrderingRules = [ skillLoadingOrderingRule; skillLoadingProvenanceRule ]
              ResolvedPathPattern = Some skillLoadingPathPattern
              Blocking = true }
            { FileName = "interactive-visible-window.md"
              FormatClass = WindowVisibility
              RequiredTokens = interactiveVisibleWindowKeys
              TableColumns = None
              OrderingRules = []
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "close-reason-separation.md"
              FormatClass = WindowVisibility
              RequiredTokens = closeReasonSeparationKeys
              TableColumns = None
              OrderingRules = [ "evidence close and user close stay separated (evidence-close-observed must not be reported as user-close-observed)" ]
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "window-state-diagnostics.md"
              FormatClass = WindowVisibility
              RequiredTokens = (windowDiagnosticClasses |> List.map (sprintf "diagnostic-class=%s")) @ windowNativeFacts
              TableColumns = None
              OrderingRules = [ "diagnostic-class ∈ { " + String.concat ", " windowDiagnosticClasses + " }" ]
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "window-options.md"
              FormatClass = WindowVisibility
              RequiredTokens = (windowOptionRows |> List.map (sprintf "option=%s"))
              TableColumns = None
              OrderingRules = [ "each option row carries status/observed; an unsupported option diagnoses under diagnostic-class=window-options (never silently ignored)" ]
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "real-image-evidence.md"
              FormatClass = WindowVisibility
              RequiredTokens = realImageEvidenceKeys
              TableColumns = None
              OrderingRules = [ "decodable image/screenshot evidence; pixel-readback alone cannot prove desktop visibility" ]
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "generated-validation.md"
              FormatClass = WindowVisibility
              RequiredTokens = generatedValidationKeys
              TableColumns = None
              OrderingRules = [ "exact-package-match must be true with the generated tests actually run and authoritative" ]
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "evidence-audit.md"
              FormatClass = WindowVisibility
              RequiredTokens = [ "verdict" ]
              TableColumns = None
              OrderingRules = [ "feature-local merge-gate audit record (file presence required)" ]
              ResolvedPathPattern = None
              Blocking = true }
            { FileName = "tasks.md (Synthetic-Evidence Inventory)"
              FormatClass = SehAcceptance
              RequiredTokens = sehAcceptanceTokens
              TableColumns = None
              OrderingRules = [ "acceptance status = accepted-seh; approval label = synthetic-error-handling-approved; no backticks" ]
              ResolvedPathPattern = None
              Blocking = true } ]

    let renderSchema (schema: EvidenceFormatSchema) : string =
        let sb = System.Text.StringBuilder()
        sb.Append(sprintf "%s: %s\n" (classLabel schema.FormatClass) schema.FileName) |> ignore
        sb.Append(sprintf "  required-tokens: %s\n" (String.concat ", " schema.RequiredTokens)) |> ignore
        match schema.TableColumns with
        | Some cols when not (List.isEmpty cols) ->
            sb.Append(sprintf "  columns (in order): %s\n" (String.concat " | " cols)) |> ignore
        | _ -> ()
        for rule in schema.OrderingRules do
            sb.Append(sprintf "  ordering: %s\n" rule) |> ignore
        match schema.ResolvedPathPattern with
        | Some p -> sb.Append(sprintf "  resolved-path: %s\n" p) |> ignore
        | None -> ()
        sb.Append(sprintf "  blocking: %b\n" schema.Blocking) |> ignore
        sb.ToString()

    let renderClass (formatClass: EvidenceFormatClass) : string =
        schemas
        |> List.filter (fun s -> s.FormatClass = formatClass)
        |> List.map renderSchema
        |> String.concat ""

    let referenceDocPath = "template/base/docs/evidence-formats.md"

    let renderReferenceDoc () : string =
        let sb = System.Text.StringBuilder()
        let line (s: string) = sb.Append(s).Append('\n') |> ignore
        line "# Evidence formats — required shapes"
        line ""
        line "<!-- GENERATED from FS.Skia.UI.Build.Evidence.EvidenceFormatSchema (feature 062, FR-005)."
        line "     Single-sourced from the constants the validators enforce, so this reference, the"
        line "     failing-class diagnostics, and the scans/audit/task-parser cannot drift. Do not edit"
        line "     by hand; regenerate with ./fake.sh build -t RefreshSurfaceBaselines. Currency-checked"
        line "     by TargetMetadataDrift. -->"
        line ""
        line "This reference lists, per evidence-format class, the complete required shape of each"
        line "enforced readiness file — so an author can recover the contract **before** triggering a"
        line "failure, without decompiling `FS.Skia.UI.Build.dll` or copying a sibling project (FR-005)."
        line ""
        // Group by class in a stable declared order.
        let classOrder = [ ReadinessContract; SkillLoadingEvidence; WindowVisibility; SehAcceptance ]
        for cls in classOrder do
            let entries = schemas |> List.filter (fun s -> s.FormatClass = cls)
            if not (List.isEmpty entries) then
                line (sprintf "## %s" (classLabel cls))
                line ""
                for s in entries do
                    line (sprintf "### `%s`" s.FileName)
                    line ""
                    line (sprintf "- required tokens: %s" (String.concat ", " s.RequiredTokens))
                    match s.TableColumns with
                    | Some cols when not (List.isEmpty cols) ->
                        line (sprintf "- columns (in order): %s" (String.concat " | " cols))
                    | _ -> ()
                    for rule in s.OrderingRules do
                        line (sprintf "- ordering: %s" rule)
                    match s.ResolvedPathPattern with
                    | Some p -> line (sprintf "- resolved-path: %s" p)
                    | None -> ()
                    line (sprintf "- blocking: %b" s.Blocking)
                    line ""
        sb.ToString()
