namespace FS.Skia.UI.Build.Evidence

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

    // --- single-source constant lists ---------------------------------------

    let skillLoadingColumns =
        [ "TaskId"; "DeclaredSkillId"; "ResolvedSkillPath"; "LoadResult"; "LoadedAt"
          "WorkStartedAt"; "EvidencePath"; "Exception" ]

    let skillLoadingOrderingRule = "loaded_at < work_started_at"

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
          [ ".NET 10 desktop"; "Vulkan"; "SkiaSharp preview"; "unsupported macOS/mobile/browser"; "no software-renderer fallback" ],
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
              OrderingRules = [ skillLoadingOrderingRule ]
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
