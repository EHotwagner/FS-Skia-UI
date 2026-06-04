namespace FS.Skia.UI.Build.Evidence

open System.Collections.Generic
open System.Text.RegularExpressions

type ScanInput =
    { ReadinessDir: string
      FeatureText: string
      ReadinessFiles: (string * string) list }

module Scans =

    let private kvRe =
        Regex(@"(?<!\S)([A-Za-z0-9_-]+)=([^=]*?)(?=\s+[A-Za-z0-9_-]+=|$)", RegexOptions.Compiled)

    let private lower (s: string) = s.ToLowerInvariant()

    // --- shared helpers (mirror the heredoc python) -------------------------

    let private fileMap (input: ScanInput) : Map<string, string> =
        input.ReadinessFiles |> Map.ofList

    let private readText (files: Map<string, string>) (rel: string) : string =
        match files.TryFind rel with
        | Some t -> t
        | None -> ""

    let private exists (files: Map<string, string>) (rel: string) : bool = files.ContainsKey rel

    let private parseKeyValues (text: string) : Dictionary<string, string> =
        let values = Dictionary<string, string>()
        for line in text.Replace("\r\n", "\n").Split('\n') do
            let stripped = line.Trim()
            if stripped = "" || stripped.StartsWith("#") || not (stripped.Contains "=") then
                ()
            else
                let matches = kvRe.Matches stripped
                if matches.Count > 0 then
                    for m in matches do
                        values.[m.Groups.[1].Value.Trim().ToLowerInvariant()] <- m.Groups.[2].Value.Trim()
                else
                    let idx = stripped.IndexOf '='
                    values.[stripped.Substring(0, idx).Trim().ToLowerInvariant()] <- stripped.Substring(idx + 1).Trim()
        values

    let private getv (values: Dictionary<string, string>) (key: string) : string =
        match values.TryGetValue key with
        | true, v -> v
        | _ -> ""

    let private missingKeys (values: Dictionary<string, string>) (required: string list) : string list =
        required |> List.filter (fun k -> not (values.ContainsKey k) || (getv values k).Trim() = "")

    let private truthy (v: string) =
        [ "true"; "yes"; "observed:true"; "verified"; "ok" ] |> List.contains (lower v)

    let private falsy (v: string) =
        [ "false"; "no"; "observed:false"; "missing"; "none" ] |> List.contains (lower v)

    let private pathOf (input: ScanInput) (name: string) = input.ReadinessDir.TrimEnd('/') + "/" + name

    let private result area (hits: ScanHit list) =
        { Area = area; Hits = hits; BlockingCount = List.length hits }

    let private anyMarker (featureText: string) (markers: string list) =
        let lo = lower featureText
        markers |> List.exists (fun m -> lo.Contains m)

    // Feature 062 (FR-005): the window-visibility evidence-format schema text
    // (required key=value keys per file + the `diagnostic-class` value set),
    // single-sourced from EvidenceFormatSchema so the printed diagnostic cannot
    // drift from the keys/classes this scan enforces. Printed on a failing
    // window-visibility class so the shape is recoverable without decompiling.
    let windowVisibilitySchemaText () : string =
        EvidenceFormatSchema.renderClass WindowVisibility

    // --- readiness-contract -------------------------------------------------

    let readinessContract (input: ScanInput) : ScanResult =
        let files = fileMap input
        // Feature 062 (FR-005, D5): the enforced (file, tokens, reason) list is
        // single-sourced in EvidenceFormatSchema so the printed diagnostic and the
        // generated evidence-formats.md reference cannot drift from this scan.
        let checks = EvidenceFormatSchema.readinessContractChecks
        let hits =
            [ for (fileName, terms, reason) in checks do
                  if not (exists files fileName) then
                      yield
                          { Path = pathOf input fileName
                            Reason = sprintf "missing %s" fileName
                            Status = Some "missing"
                            Missing = None
                            MissingTerms = Some terms
                            MissingSections = Some []
                            // FR-004 (061): carry the FULL enforced token list so the audit
                            // front-end can print the complete required shape per failing
                            // file. Single source = the same `terms` that enforce the rule,
                            // so the printed schema can never drift (RC-2). Not serialized
                            // into readiness-contract-hits.json, so existing goldens are
                            // unchanged.
                            Required = Some terms
                            Blocking = Some true
                            ValidationArea = Some "readiness-contract" }
                  else
                      let text = lower (readText files fileName)
                      let missing = terms |> List.filter (fun t -> not (text.Contains(lower t)))
                      if not (List.isEmpty missing) then
                          yield
                              { Path = pathOf input fileName
                                Reason = reason
                                Status = Some "incomplete"
                                Missing = Some missing
                                MissingTerms = Some missing
                                MissingSections = Some []
                                // FR-004 (061): full enforced token list (see above).
                                Required = Some terms
                                Blocking = Some true
                                ValidationArea = Some "readiness-contract" } ]
        result "readiness-contract" hits

    // --- persistent-launch --------------------------------------------------

    let private requiredLaunchFields =
        [ "status"; "mode"; "command"; "window-opened"; "input-dispatch"; "exit-path"; "blocked-stage"
          "classification"; "category"; "message" ]

    let persistentLaunch (input: ScanInput) : ScanResult =
        let files = fileMap input
        let requiresPersistent =
            anyMarker input.FeatureText
                [ "supported-host persistent launch"; "persistent graphical launch"; "persistent viewer contract"
                  "mode=persistent-window"; "mode=interactive-window" ]
        if not requiresPersistent then
            result "persistent-launch" []
        else
            let hits = ResizeArray<ScanHit>()
            let artifacts = ResizeArray<string * Dictionary<string, string>>()
            let boundedHelpers = ResizeArray<string>()
            let unsupportedLaunches = ResizeArray<string>()

            let candidates =
                input.ReadinessFiles
                |> List.filter (fun (rel, _) ->
                    let ext = (Option.defaultValue "" (Option.ofObj (System.IO.Path.GetExtension rel))).ToLowerInvariant()
                    [ ".txt"; ".md"; ".log" ] |> List.contains ext)
                |> List.sortBy fst

            for (relative, text) in candidates do
                let lowerText = lower text
                let values = parseKeyValues text
                let relativeLower = lower relative
                let excluded =
                    relativeLower.StartsWith("audit-fixtures/")
                    || relativeLower.StartsWith("audit-rejections/")
                    || [ "governance-risk-levels.md"; "aggregate-hang-diagnostics.md"; "runtime-limitations.md"; "task-graph.md" ]
                       |> List.contains relativeLower
                if not excluded then
                    let hasBoundedHelper =
                        getv values "smoke" = "bounded-viewer"
                        || getv values "mode" = "persistent-evidence"
                        || ([ "frames-rendered"; "frame-count"; "scene-evidence-format" ] |> List.exists values.ContainsKey)
                    if hasBoundedHelper then boundedHelpers.Add relative
                    let isPersistent =
                        ([ "interactive-window"; "persistent-window" ] |> List.contains (getv values "mode"))
                        || lowerText.Contains "mode=interactive-window"
                        || lowerText.Contains "mode=persistent-window"
                    let status = getv values "status"
                    if isPersistent then
                        artifacts.Add(relative, values)
                        if status = "unsupported" then unsupportedLaunches.Add relative
                        if status = "ok" && relativeLower.Contains "supported-host-persistent-launch" then
                            let missing = requiredLaunchFields |> List.filter (fun f -> getv values f = "")
                            if not (List.isEmpty missing) then
                                hits.Add
                                    { ScanHit.basic relative "missing persistent launch fields" with
                                        Missing = Some missing }

            let supportedOk =
                artifacts
                |> Seq.exists (fun (_, values) ->
                    getv values "status" = "ok"
                    && ([ "interactive-window"; "persistent-window" ] |> List.contains (getv values "mode"))
                    && getv values "window-opened" = "true"
                    && ([ "true"; "verified"; "false"; "not-verified" ] |> List.contains (getv values "input-dispatch"))
                    && getv values "exit-path" = "true"
                    && (requiredLaunchFields |> List.forall (fun f -> getv values f <> "")))

            if not supportedOk then
                hits.Add
                    { ScanHit.basic (pathOf input "supported-host-persistent-launch.txt")
                          "missing supported-host persistent launch evidence" with
                        Required = Some requiredLaunchFields }
                if boundedHelpers.Count > 0 then
                    hits.Add(ScanHit.basic (String.concat ", " boundedHelpers) "bounded-only substitution")
                if unsupportedLaunches.Count > 0 then
                    hits.Add(
                        ScanHit.basic (String.concat ", " unsupportedLaunches) "unsupported-host-only persistent launch evidence")

            result "persistent-launch" (List.ofSeq hits)

    // --- persistent-gui-runtime --------------------------------------------

    let persistentGui (input: ScanInput) : ScanResult =
        let files = fileMap input
        let requires =
            anyMarker input.FeatureText
                [ "persistent gui runtime"; "persistent-gui-runtime"; "interactive-lifecycle.md"; "evidence-launch-mode.md"
                  "game-visual-evidence.md"; "package-resolution.md"; "generated-verify.md" ]
        if not requires then
            result "persistent-gui-runtime" []
        else
            let hits = ResizeArray<ScanHit>()
            let requiredFiles =
                [ "interactive-lifecycle.md"; "evidence-launch-mode.md"; "container-session-diagnostics.md"
                  "package-resolution.md"; "generated-verify.md"; "game-visual-evidence.md"
                  "task-workflow-guidance.md"; "evidence-audit.md" ]
            for fileName in requiredFiles do
                if not (exists files fileName) then
                    hits.Add { ScanHit.basic (pathOf input fileName) "missing required readiness files" with Missing = Some [ fileName ] }

            let interactiveValues = parseKeyValues (readText files "interactive-lifecycle.md")
            if exists files "interactive-lifecycle.md" then
                let mode = getv interactiveValues "mode"
                let selfClosed = getv interactiveValues "self-closed-for-evidence"
                let firstFrameOnly = getv interactiveValues "first-frame-only"
                if mode = "persistent-evidence" || selfClosed = "true" || firstFrameOnly = "true" then
                    hits.Add(ScanHit.basic (pathOf input "interactive-lifecycle.md") "bounded-only substitution for interactive evidence")

            let packageValues = parseKeyValues (readText files "package-resolution.md")
            if exists files "package-resolution.md" then
                let missing = missingKeys packageValues [ "exact-match"; "requested-version"; "resolved-version"; "package-source" ]
                if not (List.isEmpty missing) then
                    hits.Add { ScanHit.basic (pathOf input "package-resolution.md") "missing readiness acceptance keywords" with Missing = Some missing }
                let exact = lower (getv packageValues "exact-match")
                if exact <> "true" && exact <> "yes"
                   || lower (getv packageValues "package-resolution") = "nu1603"
                   || ([ "true"; "yes" ] |> List.contains (lower (getv packageValues "mismatch"))) then
                    hits.Add(ScanHit.basic (pathOf input "package-resolution.md") "unresolved package mismatch")

            let generatedValues = parseKeyValues (readText files "generated-verify.md")
            if exists files "generated-verify.md" then
                let missing = missingKeys generatedValues [ "generated-tests-exist"; "generated-tests-ran"; "authoritative"; "failure-class" ]
                if not (List.isEmpty missing) then
                    hits.Add { ScanHit.basic (pathOf input "generated-verify.md") "missing readiness acceptance keywords" with Missing = Some missing }
                let testsExist = lower (getv generatedValues "generated-tests-exist")
                let testsRan = lower (getv generatedValues "generated-tests-ran")
                let authoritative = lower (getv generatedValues "authoritative")
                if ([ "true"; "yes" ] |> List.contains testsExist) && not ([ "true"; "yes" ] |> List.contains testsRan) then
                    hits.Add(ScanHit.basic (pathOf input "generated-verify.md") "generated tests exist but did not run")
                elif [ "false"; "no" ] |> List.contains authoritative then
                    hits.Add(ScanHit.basic (pathOf input "generated-verify.md") "generated verification is non-authoritative")

            let visualValues = parseKeyValues (readText files "game-visual-evidence.md")
            let visualLower = lower (readText files "game-visual-evidence.md")
            if exists files "game-visual-evidence.md" then
                let missing = missingKeys visualValues [ "supported-host"; "evidence-kind"; "board-readable"; "input-or-progress-observed" ]
                if not (List.isEmpty missing) then
                    hits.Add { ScanHit.basic (pathOf input "game-visual-evidence.md") "missing readiness acceptance keywords" with Missing = Some missing }
                let supported = [ "true"; "yes" ] |> List.contains (lower (getv visualValues "supported-host"))
                let kind = lower (getv visualValues "evidence-kind")
                let board = lower (getv visualValues "board-readable")
                let progress = lower (getv visualValues "input-or-progress-observed")
                let textOnly =
                    ([ "metadata"; "text"; "scene-metadata"; "text-only" ] |> List.contains kind)
                    || visualLower.Contains "text-only visual metadata"
                if supported && textOnly then
                    hits.Add(ScanHit.basic (pathOf input "game-visual-evidence.md") "text-only visual metadata on supported host")
                elif ([ "screenshot"; "pixel-readback" ] |> List.contains kind)
                     && (not ([ "true"; "yes" ] |> List.contains board) || not ([ "true"; "yes" ] |> List.contains progress)) then
                    hits.Add(ScanHit.basic (pathOf input "game-visual-evidence.md") "visual game evidence missing board/input proof")

            result "persistent-gui-runtime" (List.ofSeq hits)

    // --- window-visibility --------------------------------------------------

    let windowVisibility (input: ScanInput) : ScanResult =
        let files = fileMap input
        let requires =
            anyMarker input.FeatureText
                [ "fix window visibility"; "interactive-visible-window.md"; "close-reason-separation.md"
                  "real-image-evidence.md"; "generated-validation.md" ]
        if not requires then
            result "window-visibility" []
        else
            let hits = ResizeArray<ScanHit>()
            let requiredFiles =
                [ "interactive-visible-window.md"; "close-reason-separation.md"; "window-state-diagnostics.md"
                  "window-options.md"; "real-image-evidence.md"; "generated-validation.md"; "evidence-audit.md" ]
            for fileName in requiredFiles do
                if not (exists files fileName) then
                    hits.Add { ScanHit.basic (pathOf input fileName) "missing required window visibility readiness file" with Missing = Some [ fileName ] }

            let interactiveValues = parseKeyValues (readText files "interactive-visible-window.md")
            if exists files "interactive-visible-window.md" then
                // Feature 062 (FR-005, D5): single-sourced in EvidenceFormatSchema.
                let missing =
                    missingKeys interactiveValues EvidenceFormatSchema.interactiveVisibleWindowKeys
                if not (List.isEmpty missing) then
                    hits.Add { ScanHit.basic (pathOf input "interactive-visible-window.md") "missing visible-window readiness fields" with Missing = Some missing }
                let statusOk = [ "ok"; "pass"; "success" ] |> List.contains (lower (getv interactiveValues "status"))
                let taskbarOnly =
                    truthy (getv interactiveValues "taskbar-entry")
                    && (falsy (getv interactiveValues "window-visible") || falsy (getv interactiveValues "accessible-window"))
                let processOnly = truthy (getv interactiveValues "process-running") && truthy (getv interactiveValues "process-only")
                if statusOk && (taskbarOnly || processOnly) then
                    hits.Add(ScanHit.basic (pathOf input "interactive-visible-window.md") "process/taskbar-only visible-window substitution")

            let closeValues = parseKeyValues (readText files "close-reason-separation.md")
            if exists files "close-reason-separation.md" then
                let closeReason = lower (getv closeValues "close-reason")
                let userClose = truthy (getv closeValues "user-close-observed")
                let evidenceClose =
                    truthy (getv closeValues "evidence-close-observed")
                    || ([ "evidence-close"; "framework-close"; "host-close"; "bounded-evidence-close" ] |> List.contains closeReason)
                if evidenceClose && userClose then
                    hits.Add(ScanHit.basic (pathOf input "close-reason-separation.md") "evidence close reported as user close")

            let diagnosticsText = readText files "window-state-diagnostics.md"
            let diagnosticsLower = lower diagnosticsText
            let diagnosticsValues = parseKeyValues diagnosticsText
            if exists files "window-state-diagnostics.md" then
                // Feature 062 (FR-005, D5): single-sourced in EvidenceFormatSchema.
                let requiredClasses = EvidenceFormatSchema.windowDiagnosticClasses
                let missingClasses =
                    requiredClasses
                    |> List.filter (fun cls ->
                        not (diagnosticsLower.Contains(sprintf "diagnostic-class=%s" cls))
                        && not (diagnosticsLower.Contains(sprintf "failure-class=%s" cls)))
                if not (List.isEmpty missingClasses) then
                    hits.Add { ScanHit.basic (pathOf input "window-state-diagnostics.md") "missing diagnostic classes" with Missing = Some missingClasses }
                let requiredFacts = [ "native-handle"; "visible"; "focusable"; "renderable-surface"; "input-devices" ]
                let missingFacts = missingKeys diagnosticsValues requiredFacts
                if not (List.isEmpty missingFacts) then
                    hits.Add { ScanHit.basic (pathOf input "window-state-diagnostics.md") "missing observable-vs-unsupported native facts" with Missing = Some missingFacts }
                let statusOk = [ "ok"; "pass"; "success" ] |> List.contains (lower (getv diagnosticsValues "status"))
                let diagnosticTaskbarOnly = truthy (getv diagnosticsValues "taskbar-entry")
                if statusOk && diagnosticTaskbarOnly then
                    hits.Add(ScanHit.basic (pathOf input "window-state-diagnostics.md") "process/taskbar-only success claim")
                let unsupportedOnly =
                    truthy (getv diagnosticsValues "unsupported-host-only")
                    || (diagnosticsLower.Contains "unsupported-host-only" && not (diagnosticsLower.Contains "observed:true"))
                    || (lower (getv diagnosticsValues "visible") = "unsupported"
                        && lower (getv diagnosticsValues "renderable-surface") = "unsupported"
                        && not (diagnosticsLower.Contains "observed:true"))
                let interactiveStatusOk = [ "ok"; "pass"; "success" ] |> List.contains (lower (getv interactiveValues "status"))
                let interactiveVisibleUnsupported = [ "unsupported"; "unavailable" ] |> List.contains (lower (getv interactiveValues "window-visible"))
                if unsupportedOnly && (interactiveStatusOk || interactiveVisibleUnsupported) then
                    hits.Add(ScanHit.basic (pathOf input "window-state-diagnostics.md") "unsupported-host-only visible-window claim")

            let optionsLower = lower (readText files "window-options.md")
            if exists files "window-options.md" then
                let requiredOptions = [ "resize"; "maximize"; "startup-state"; "startup-position"; "backend" ]
                let missingOptions =
                    requiredOptions
                    |> List.filter (fun opt ->
                        not (optionsLower.Contains(sprintf "option=%s" opt))
                        && not (optionsLower.Contains(sprintf "%s=" opt))
                        && not (optionsLower.Contains(sprintf "%s=" (opt.Replace("-", "_")))))
                if not (List.isEmpty missingOptions) then
                    hits.Add { ScanHit.basic (pathOf input "window-options.md") "missing option rows" with Missing = Some missingOptions }
                let unsupportedRequested =
                    [ "requested=opengl"; "requested=software"; "requested=minimized"; "requested=fullscreen"; "unsupported-setting=true" ]
                    |> List.exists optionsLower.Contains
                let unsupportedDiagnosed =
                    [ "status=unsupported"; "status=degraded"; "status=failed"; "diagnostic-class=window-options"; "failure-class=window-options" ]
                    |> List.exists optionsLower.Contains
                if unsupportedRequested && not unsupportedDiagnosed then
                    hits.Add(ScanHit.basic (pathOf input "window-options.md") "silently ignored unsupported window option")
                let optionFailureHidden =
                    (optionsLower.Contains "status=failed" || optionsLower.Contains "failed=true")
                    && (optionsLower.Contains "diagnostic-class=app-lifecycle" || optionsLower.Contains "failure-class=app-lifecycle")
                    && not (optionsLower.Contains "diagnostic-class=window-options")
                    && not (optionsLower.Contains "failure-class=window-options")
                if optionFailureHidden then
                    hits.Add(ScanHit.basic (pathOf input "window-options.md") "window-options failure hidden under app-lifecycle")

            let imageText = readText files "real-image-evidence.md"
            let imageLower = lower imageText
            let imageValues = parseKeyValues imageText
            if exists files "real-image-evidence.md" then
                let imageKind = lower (getv imageValues "evidence-kind")
                let imageStatus = lower (getv imageValues "status")
                let requested = truthy (getv imageValues "requested-image-evidence") || ([ "screenshot"; "image" ] |> List.contains imageKind)
                let metadataOnly =
                    ([ "metadata"; "hash"; "text"; "text-only" ] |> List.contains (lower (getv imageValues "artifact-kind")))
                    || imageLower.Contains "metadata-only screenshot"
                let decodableValue =
                    let v1 = getv imageValues "artifact-decodable"
                    let v2 = if v1 = "" then getv imageValues "image-decodable" else v1
                    let v3 = if v2 = "" then getv imageValues "decodable" else v2
                    lower v3
                let undecodable = falsy decodableValue
                let unsupportedScreenshot = imageKind = "screenshot" && imageStatus = "unsupported"
                if requested && (metadataOnly || undecodable) then
                    hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "metadata-only screenshot claim")
                if unsupportedScreenshot && (getv imageValues "unsupported-host-reason").Trim() = "" then
                    hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "unsupported screenshot missing unsupported-host reason")
                if unsupportedScreenshot
                   && (truthy (getv imageValues "proves-desktop-visibility")
                       || truthy decodableValue
                       || lower (getv imageValues "artifact-kind") = "image"
                       || (getv imageValues "image-artifact").Trim() <> "") then
                    hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "unsupported screenshot cannot prove desktop visibility")
                if [ "image"; "screenshot"; "pixel-readback"; "metadata-hash" ] |> List.contains imageKind then
                    if not (imageValues.ContainsKey "proves-scene-rendering") || not (imageValues.ContainsKey "proves-desktop-visibility") then
                        hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "missing visual evidence proof fields")
                if imageKind = "pixel-readback" && truthy (getv imageValues "proves-desktop-visibility") then
                    hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "pixel-readback cannot prove desktop visibility")
                if requested && decodableValue <> "" && not (truthy decodableValue || falsy decodableValue) then
                    hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "corrupt image metadata record")
                let artifactPath =
                    let v1 = getv imageValues "image-artifact"
                    (if v1 = "" then getv imageValues "artifact-path" else v1).Trim()
                if artifactPath <> "" && (System.IO.Path.IsPathRooted artifactPath || artifactPath.Split('/', '\\') |> Array.contains "..") then
                    hits.Add(ScanHit.basic (pathOf input "real-image-evidence.md") "hostile artifact path")

            let generatedValues = parseKeyValues (readText files "generated-validation.md")
            if exists files "generated-validation.md" then
                let missing = missingKeys generatedValues [ "exact-package-match"; "generated-tests-ran"; "authoritative"; "failure-class" ]
                if not (List.isEmpty missing) then
                    hits.Add { ScanHit.basic (pathOf input "generated-validation.md") "missing generated validation fields" with Missing = Some missing }
                let exact = lower (getv generatedValues "exact-package-match")
                if exact <> "true" && exact <> "yes"
                   || lower (getv generatedValues "package-resolution") = "nu1603"
                   || ([ "true"; "yes" ] |> List.contains (lower (getv generatedValues "package-mismatch"))) then
                    hits.Add(ScanHit.basic (pathOf input "generated-validation.md") "unresolved package mismatch")
                let testsExist = lower (getv generatedValues "generated-tests-exist")
                let testsRan = lower (getv generatedValues "generated-tests-ran")
                if ([ "true"; "yes" ] |> List.contains testsExist) && not ([ "true"; "yes" ] |> List.contains testsRan) then
                    hits.Add(ScanHit.basic (pathOf input "generated-validation.md") "missing generated test execution")

            result "window-visibility" (List.ofSeq hits)
