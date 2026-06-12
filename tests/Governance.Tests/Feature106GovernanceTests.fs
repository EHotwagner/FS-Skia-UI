module Feature106GovernanceTests

// Feature 106 (US2, FR-007) — the Controls documentation-coverage gate. `ControlsDocCoverage`
// is a pure analysis over `(path, content)` pairs, so these tests feed in-memory `.fsi`
// fixtures (no temp files, no mocks): a planted placeholder is the RED proof the gate detects
// the real failure mode; a short-but-meaningful summary is the anti-false-positive GREEN; a
// generic sentence shared across many members is the anti-evasion DuplicateOnly. The final
// test runs `analyze` over the REAL `src/Controls/**/*.fsi` surface and asserts zero findings
// (the feature's SC-002 acceptance), so the suite fails if any boilerplate regresses.

open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.ControlsDocCoverage

let private repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "FS-Skia-UI.sln")) then
            dir
        else
            match Directory.GetParent dir |> Option.ofObj with
            | Some p -> find p.FullName
            | None -> dir

    find __SOURCE_DIRECTORY__

let private placeholderFixture =
    """namespace FS.Skia.UI.Controls

/// Public contract module exposed by this FS.Skia.UI package.
module Attr =
    /// Public contract function exposed by this FS.Skia.UI package.
    val text: value: string -> Attr<'msg>
"""

let private substantiveFixture =
    """namespace FS.Skia.UI.Controls

/// Attribute builders (`Attr`) shared across the control kinds.
module Attr =
    /// Sets a control's display text (`Attr.text`). Accepted by text-bearing controls.
    val text: value: string -> Attr<'msg>
"""

// Short but meaningful — the gate must NOT flag legitimately terse summaries (spec edge case).
let private terseFixture =
    """namespace FS.Skia.UI.Controls

/// The `dark` theme palette.
val dark: Theme
"""

// A generic sentence copy-pasted across many members with no member-specific token (the
// "mechanically reworded placeholder" evasion). 9 ≥ the duplicate threshold of 8.
let private duplicateOnlyFixture =
    let entry i =
        sprintf "    /// Returns a configured value.\n    val member%d: int -> int\n" i

    "namespace FS.Skia.UI.Controls\n\n/// Helpers grouped under `M`.\nmodule M =\n"
    + (String.concat "" [ for i in 1..9 -> entry i ])

[<Tests>]
let controlsDocCoverageTests =
    testList "Feature 106 controls doc-coverage gate" [
        test "isPlaceholderSummary matches all three boilerplate wordings, normalized" {
            Expect.isTrue
                (isPlaceholderSummary "Public contract function exposed by this FS.Skia.UI package.")
                "function variant"
            Expect.isTrue
                (isPlaceholderSummary "Public contract type exposed by this FS.Skia.UI package.")
                "type variant"
            Expect.isTrue
                (isPlaceholderSummary "Public contract   module   exposed by this FS.Skia.UI package.")
                "module variant, extra whitespace normalized"
            Expect.isFalse
                (isPlaceholderSummary "Sets a control's display text.")
                "a substantive summary is not a placeholder"
        }

        // RED: the gate detects the real failure mode (the placeholder boilerplate).
        test "analyze flags placeholder boilerplate summaries (RED)" {
            let findings = analyze [ "src/Controls/Placeholder.fsi", placeholderFixture ]
            Expect.isNonEmpty findings "the placeholder fixture must produce findings"
            Expect.all findings (fun f -> f.Reason = Placeholder) "every finding is a Placeholder"
            Expect.equal (List.length findings) 2 "both the module and the val placeholders are flagged"
        }

        // GREEN: a substantive surface produces no findings (anti-false-positive).
        test "analyze returns no findings for substantive summaries (GREEN)" {
            Expect.isEmpty
                (analyze [ "src/Controls/Substantive.fsi", substantiveFixture ])
                "substantive summaries must pass"
        }

        // Anti-false-positive: a short but meaningful summary is accepted.
        test "analyze does not flag a short but meaningful summary" {
            Expect.isEmpty
                (analyze [ "src/Controls/Terse.fsi", terseFixture ])
                "a legitimately terse, member-specific summary must pass"
        }

        // Anti-evasion: a generic sentence shared across many members is DuplicateOnly.
        test "analyze flags a reworded placeholder shared across many members (DuplicateOnly)" {
            let findings = analyze [ "src/Controls/Duplicate.fsi", duplicateOnlyFixture ]
            Expect.isNonEmpty findings "the duplicate-only fixture must produce findings"
            Expect.all findings (fun f -> f.Reason = DuplicateOnly) "every finding is DuplicateOnly"
            Expect.equal (List.length findings) 9 "all nine identical token-less summaries are flagged"
        }

        // SC-002 acceptance: the real Controls public surface carries zero findings.
        test "analyze returns zero findings over the real src/Controls/**/*.fsi surface" {
            let controlsRoot = Path.Combine(repositoryRoot, "src", "Controls")

            let files =
                Directory.GetFiles(controlsRoot, "*.fsi", SearchOption.AllDirectories)
                |> Array.sort
                |> Array.toList
                |> List.map (fun full ->
                    let rel =
                        full.Substring(repositoryRoot.Length).TrimStart([| '/'; '\\' |]).Replace("\\", "/")

                    rel, File.ReadAllText full)

            let findings = analyze files

            Expect.isEmpty
                findings
                (sprintf
                    "the Controls public surface must carry no placeholder/empty/duplicate-only summaries; findings:\n%s"
                    (String.concat "\n" (failureDiagnostics findings)))
        }
    ]
