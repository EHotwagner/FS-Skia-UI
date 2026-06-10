module DesignTokenGenTests

// Feature 069 — DTCG design-token generation. The 10 FS.Skia.UI.Controls theme primitives
// (x light/dark) are generated from the single DTCG source
// `src/Controls/design-tokens.tokens.json` (whole-file) into `src/Controls/DesignTokens.fs`.
// These tests mirror the Feature-066 CatalogTests generation block: byte-identity of the
// committed generated module, currency PASS on the committed tree, splice idempotency/
// determinism, drift FAIL on a hand-mutated module, all-Missing on a whole-file removal, and
// deterministic alias resolution. The error-path tests (T011) are SEH-approved.

open System.IO
open Expecto
open FS.Skia.UI.Build

let private repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "FS-Skia-UI.sln")) then
            dir
        else
            match Directory.GetParent dir |> Option.ofObj with
            | Some p -> find p.FullName
            | None -> dir

    find __SOURCE_DIRECTORY__

let private tokensJsonPath = Path.Combine(repositoryRoot, DesignTokenGen.tokensJsonRel)
let private designTokensFsPath = Path.Combine(repositoryRoot, DesignTokenGen.designTokensFsRel)
let private tokensJson () = File.ReadAllText tokensJsonPath
let private designTokensFs () = File.ReadAllText designTokensFsPath

/// Run a generation that must fail and return the failure message (else fail the test).
let private captureFailure (f: unit -> unit) : string =
    try
        f ()
        failtest "expected a generation failure but none was raised"
    with ex ->
        ex.Message

[<Tests>]
let designTokenContractTests =
    // T009 (US1): failing-first contract — the generator surface and the curated .fsi exist.
    testList "Feature 069 design-token generator contract" [
        test "DesignTokenGen exposes parse/renderValue/renderModule/splice/currency/currencyDrift" {
            let facts = DesignTokenGen.parse (tokensJson ())
            Expect.equal (List.length facts) 24 "parse yields 12 tokens (10 Theme primitives + success/warning) x 2 themes"

            let fact = facts |> List.find (fun f -> f.Theme = "light" && f.Name = "foreground")
            Expect.equal (DesignTokenGen.renderValue fact) fact.Rendered "renderValue returns the rendered literal"

            let moduleText = DesignTokenGen.renderModule facts
            Expect.stringContains moduleText "module DesignTokens =" "renderModule emits the token module"

            let spliced = DesignTokenGen.splice (tokensJson ())
            Expect.equal spliced moduleText "splice = renderModule(parse(source))"

            let currency = DesignTokenGen.currency (tokensJson ()) (designTokensFs ())
            Expect.equal (List.length currency) 24 "currency reports one status per token"
            Expect.isEmpty (DesignTokenGen.currencyDrift currency) "a clean tree produces no drift diagnostics"
        }

        test "the curated DesignTokens.fsi declares the Light/Dark token surface" {
            let fsi = File.ReadAllText(Path.Combine(repositoryRoot, "src", "Controls", "DesignTokens.fsi"))
            Expect.stringContains fsi "module DesignTokens =" "fsi declares the DesignTokens module"
            Expect.stringContains fsi "module Light =" "fsi declares Light"
            Expect.stringContains fsi "module Dark =" "fsi declares Dark"

            for name in [ "foreground"; "background"; "accent"; "danger"; "muted"; "fontFamily"; "fontSize"; "density"; "cornerRadius"; "contrastRequiredRatio" ] do
                Expect.stringContains fsi (sprintf "val %s :" name) (sprintf "fsi declares val %s" name)
        }
    ]

[<Tests>]
let designTokenGenerationTests =
    // T010 (US1): generator semantics + drift + determinism + alias resolution.
    testList "Feature 069 design-token generation" [
        test "the committed DesignTokens.fs is a current, byte-identical regeneration (SC-002, SC-005)" {
            let render = DesignTokenGen.splice (tokensJson ())
            Expect.equal (designTokensFs ()) render "DesignTokens.fs is byte-identical to a fresh render of the DTCG source"

            let currency = DesignTokenGen.currency (tokensJson ()) (designTokensFs ())
            Expect.isTrue (DesignTokenGen.isCurrent currency) "every generated token is current on the committed tree"
        }

        test "regeneration is deterministic — regenerate twice yields byte-identical output (SC-006)" {
            let once = DesignTokenGen.splice (tokensJson ())
            let twice = DesignTokenGen.splice (tokensJson ())
            Expect.equal once twice "splice is deterministic"
        }

        test "alias resolution is deterministic — a dark.danger -> light.danger alias resolves to the light value" {
            // Feature 083 changed the *shipped* dark.danger to a concrete accessible value
            // (WCAG conformance, ContrastCheck). Derive an alias fixture from the real source so
            // this 069 contract — that the parser resolves a `{group.token}` alias deterministically
            // to the target's concrete value — is still proven on representative, valid input.
            let aliased = (tokensJson ()).Replace("#ff9592ff", "{light.danger}")
            let facts = DesignTokenGen.parse aliased
            let darkDanger = facts |> List.find (fun f -> f.Theme = "dark" && f.Name = "danger")
            let lightDanger = facts |> List.find (fun f -> f.Theme = "light" && f.Name = "danger")
            Expect.equal darkDanger.Rendered "Colors.rgba 185uy 28uy 28uy 255uy" "dark.danger alias resolves to the light value"
            Expect.equal darkDanger.Rendered lightDanger.Rendered "the alias resolves to exactly light.danger"
        }

        test "the drift gate flags a hand-mutated token and names the token, theme, and regenerate command (SC-005, FR-004)" {
            let original = designTokensFs ()

            let mutated =
                original.Replace(
                    "let foreground : Color = Colors.rgba 31uy 41uy 55uy 255uy",
                    "let foreground : Color = Colors.rgba 0uy 0uy 0uy 255uy")

            Expect.notEqual mutated original "the hand-mutation actually changed the file"

            let currency = DesignTokenGen.currency (tokensJson ()) mutated
            Expect.isFalse (DesignTokenGen.isCurrent currency) "a hand-mutated token is not current"

            let drift = DesignTokenGen.currencyDrift currency
            Expect.isNonEmpty drift "a stale token yields a drift diagnostic"

            let foregroundDrift =
                drift |> List.filter (fun d -> d.Contains "'foreground'" && d.Contains "light")

            Expect.isNonEmpty foregroundDrift "the diagnostic names the divergent token and theme"
            Expect.isTrue (foregroundDrift |> List.forall (fun d -> d.Contains "./fake.sh build -t RefreshSurfaceBaselines")) "the diagnostic names the regenerate command"

            Expect.equal (DesignTokenGen.splice (tokensJson ())) original "one regeneration restores the mutated file byte-for-byte"
        }

        test "a removed/whole-file-missing generated module is reported all-Missing (loud, never silent)" {
            let currency = DesignTokenGen.currency (tokensJson ()) ""
            Expect.equal (List.length currency) 24 "currency still reports every required token"
            Expect.isTrue (currency |> List.forall (fun c -> c.Status = DesignTokenGen.Missing)) "a missing module reports every token Missing"

            let drift = DesignTokenGen.currencyDrift currency
            Expect.isNonEmpty drift "the missing module fails loudly with diagnostics"
            Expect.isTrue (drift |> List.forall (fun d -> d.Contains "is missing")) "every diagnostic reports a missing token"
        }
    ]

(* SYNTHETIC FIXTURE: all tests in this list (T011) use malformed/cyclic DTCG documents that
   cannot be sourced as real, representative input. They validate explicit generation-failure
   behavior — approved [SEH] / synthetic-error-handling-approved at task generation. *)
[<Tests>]
let designTokenErrorPathTests =
    testList "Feature 069 design-token generation error paths" [
        test "Synthetic_malformed DTCG JSON raises a generation failure and emits no F#" {
            let malformed = "{ \"light\": { \"foreground\": { \"$type\": \"color\", \"$value\": "
            Expect.throws (fun () -> DesignTokenGen.parse malformed |> ignore) "malformed JSON raises"

            let message = captureFailure (fun () -> DesignTokenGen.splice malformed |> ignore)
            Expect.stringContains message "design-tokens" "the failure is the design-token generation failure (no partial F# emitted)"
        }

        test "Synthetic_cyclic alias raises a generation failure naming the offending token and emits no F#" {
            let cyclic =
                """{
                  "light": {
                    "foreground": { "$type": "color", "$value": "{light.background}" },
                    "background": { "$type": "color", "$value": "{light.foreground}" },
                    "accent": { "$type": "color", "$value": "#2563ebff" },
                    "danger": { "$type": "color", "$value": "#b91c1cff" },
                    "muted": { "$type": "color", "$value": "#64748bff" },
                    "fontFamily": { "$type": "fontFamily", "$value": null },
                    "fontSize": { "$type": "dimension", "$value": 14.0 },
                    "density": { "$type": "number", "$value": 1.0 },
                    "cornerRadius": { "$type": "dimension", "$value": 4.0 },
                    "contrastRequiredRatio": { "$type": "number", "$value": 4.5 }
                  }
                }"""

            let message = captureFailure (fun () -> DesignTokenGen.splice cyclic |> ignore)
            Expect.stringContains message "cyclic alias" "the failure reports a cyclic alias"
            Expect.stringContains message "foreground" "the failure names the offending token"
        }

        test "Synthetic_unresolvable alias raises a generation failure naming the missing token" {
            let unresolvable =
                """{
                  "light": {
                    "foreground": { "$type": "color", "$value": "{light.doesNotExist}" },
                    "background": { "$type": "color", "$value": "#f8fafcff" },
                    "accent": { "$type": "color", "$value": "#2563ebff" },
                    "danger": { "$type": "color", "$value": "#b91c1cff" },
                    "muted": { "$type": "color", "$value": "#64748bff" },
                    "fontFamily": { "$type": "fontFamily", "$value": null },
                    "fontSize": { "$type": "dimension", "$value": 14.0 },
                    "density": { "$type": "number", "$value": 1.0 },
                    "cornerRadius": { "$type": "dimension", "$value": 4.0 },
                    "contrastRequiredRatio": { "$type": "number", "$value": 4.5 }
                  },
                  "dark": {
                    "foreground": { "$type": "color", "$value": "#f1f5f9ff" },
                    "background": { "$type": "color", "$value": "#111827ff" },
                    "accent": { "$type": "color", "$value": "#60a5faff" },
                    "danger": { "$type": "color", "$value": "{light.danger}" },
                    "muted": { "$type": "color", "$value": "#94a3b8ff" },
                    "fontFamily": { "$type": "fontFamily", "$value": null },
                    "fontSize": { "$type": "dimension", "$value": 14.0 },
                    "density": { "$type": "number", "$value": 1.0 },
                    "cornerRadius": { "$type": "dimension", "$value": 4.0 },
                    "contrastRequiredRatio": { "$type": "number", "$value": 4.5 }
                  }
                }"""

            let message = captureFailure (fun () -> DesignTokenGen.splice unresolvable |> ignore)
            Expect.stringContains message "missing token" "the failure reports the unresolvable reference"
            Expect.stringContains message "doesNotExist" "the failure names the missing token"
        }

        test "Synthetic_a required token absent from the DTCG source fails loudly naming it" {
            let incomplete =
                """{
                  "light": {
                    "foreground": { "$type": "color", "$value": "#1f2937ff" }
                  },
                  "dark": {
                    "foreground": { "$type": "color", "$value": "#f1f5f9ff" }
                  }
                }"""

            let message = captureFailure (fun () -> DesignTokenGen.parse incomplete |> ignore)
            Expect.stringContains message "required token" "the failure reports a required token is missing"
            Expect.stringContains message "background" "the failure names a missing required token"
        }
    ]
