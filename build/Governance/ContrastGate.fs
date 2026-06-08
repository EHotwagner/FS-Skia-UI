module FS.Skia.UI.Build.ContrastGate

open System
open System.Globalization
open System.Text.RegularExpressions
open FS.Skia.UI.Scene
open FS.Skia.UI.Color

type ValidatedPairing =
    { Foreground: string
      Background: string
      Role: Role }

type PairingOutcome =
    { Theme: string
      Pairing: ValidatedPairing
      ForegroundColor: Color
      BackgroundColor: Color
      Measured: float
      Required: float
      Passed: bool }

let graphicThreshold = 3.0

// The explicit, documented set (FR-009): how the shipped themes are actually drawn —
// body/emphasis/error/secondary text on the app background (Text), plus the accent used as a
// control outline / focus ring / interactive icon (GraphicOrUi). NOT the cartesian product.
let validatedPairings =
    [ { Foreground = "foreground"; Background = "background"; Role = Text }
      { Foreground = "accent"; Background = "background"; Role = Text }
      { Foreground = "danger"; Background = "background"; Role = Text }
      { Foreground = "muted"; Background = "background"; Role = Text }
      { Foreground = "accent"; Background = "background"; Role = GraphicOrUi } ]

let private requiredFor role textRequiredRatio =
    match role with
    | Text -> textRequiredRatio
    | GraphicOrUi -> graphicThreshold
    | Decorative -> 0.0 // recorded, never enforced

let evaluateTheme (theme: string) (tokens: Map<string, Color>) (textRequiredRatio: float) =
    validatedPairings
    |> List.choose (fun pairing ->
        match Map.tryFind pairing.Foreground tokens, Map.tryFind pairing.Background tokens with
        | Some fg, Some bg ->
            let resolvedFg = Contrast.compositeOver bg fg
            let measured = Contrast.ratio resolvedFg bg
            let required = requiredFor pairing.Role textRequiredRatio

            let passed =
                match pairing.Role with
                | Decorative -> true
                | _ -> measured >= required

            Some
                { Theme = theme
                  Pairing = pairing
                  ForegroundColor = resolvedFg
                  BackgroundColor = bg
                  Measured = measured
                  Required = required
                  Passed = passed }
        | _ -> None)

// Parse the renderer's color literal ("Colors.rgba 31uy 41uy 55uy 255uy") back into a Color,
// and the number literal ("4.5") for contrastRequiredRatio. The DTCG facts are alias-resolved
// by DesignTokenGen.parse, so the gate reads the same values the generated module ships.
let private colorLiteralRx =
    Regex(@"Colors\.rgba\s+(\d+)uy\s+(\d+)uy\s+(\d+)uy\s+(\d+)uy", RegexOptions.Compiled)

let private parseColorLiteral (rendered: string) =
    let m = colorLiteralRx.Match rendered

    if m.Success then
        Some
            { Red = byte (int m.Groups.[1].Value)
              Green = byte (int m.Groups.[2].Value)
              Blue = byte (int m.Groups.[3].Value)
              Alpha = byte (int m.Groups.[4].Value) }
    else
        None

let private parseNumberLiteral (rendered: string) =
    match Double.TryParse(rendered, NumberStyles.Float, CultureInfo.InvariantCulture) with
    | true, value -> Some value
    | false, _ -> None

let outcomesFromFacts (facts: DesignTokenGen.DesignTokenFact list) =
    let themes =
        facts |> List.map (fun f -> f.Theme) |> List.distinct

    themes
    |> List.collect (fun theme ->
        let themeFacts = facts |> List.filter (fun f -> f.Theme = theme)

        let colors =
            themeFacts
            |> List.choose (fun f ->
                match f.Kind with
                | DesignTokenGen.Color -> parseColorLiteral f.Rendered |> Option.map (fun c -> f.Name, c)
                | _ -> None)
            |> Map.ofList

        let textRequiredRatio =
            themeFacts
            |> List.tryPick (fun f ->
                if f.Name = "contrastRequiredRatio" then parseNumberLiteral f.Rendered else None)
            |> Option.defaultValue 4.5

        evaluateTheme theme colors textRequiredRatio)

let failures (outcomes: PairingOutcome list) =
    outcomes
    |> List.filter (fun outcome ->
        match outcome.Pairing.Role with
        | Decorative -> false
        | _ -> not outcome.Passed)

let private roleName role =
    match role with
    | Text -> "Text"
    | GraphicOrUi -> "GraphicOrUi"
    | Decorative -> "Decorative"

let private hex (color: Color) =
    sprintf "#%02x%02x%02x%02x" color.Red color.Green color.Blue color.Alpha

let failureDiagnostics (outcomes: PairingOutcome list) =
    failures outcomes
    |> List.map (fun outcome ->
        sprintf
            "%s theme: %s on %s (%s) measures %.2f:1 but requires %.2f:1 — foreground %s over background %s"
            outcome.Theme
            outcome.Pairing.Foreground
            outcome.Pairing.Background
            (roleName outcome.Pairing.Role)
            outcome.Measured
            outcome.Required
            (hex outcome.ForegroundColor)
            (hex outcome.BackgroundColor))

let renderReport (outcomes: PairingOutcome list) =
    let themes =
        outcomes |> List.map (fun o -> o.Theme) |> List.distinct

    let failing = failures outcomes

    let header =
        [ "# Color Contrast Evidence"
          ""
          if List.isEmpty failing then
              "PASS: every enforced foreground/background pairing meets its required WCAG ratio in both themes (SC-001)."
          else
              sprintf "FAIL: %d enforced pairing(s) fell below threshold." (List.length failing)
          "" ]

    let renderTheme theme =
        let rows = outcomes |> List.filter (fun o -> o.Theme = theme)

        [ sprintf "## %s theme" theme
          ""
          "| Foreground | Background | Role | Foreground color | Background color | Measured | Required | Result |"
          "|------------|------------|------|------------------|------------------|----------|----------|--------|"
          yield!
              rows
              |> List.map (fun o ->
                  let result =
                      match o.Pairing.Role with
                      | Decorative -> "recorded"
                      | _ -> if o.Passed then "PASS" else "FAIL"

                  let required =
                      match o.Pairing.Role with
                      | Decorative -> "n/a"
                      | _ -> sprintf "%.2f:1" o.Required

                  sprintf
                      "| %s | %s | %s | %s | %s | %.2f:1 | %s | %s |"
                      o.Pairing.Foreground
                      o.Pairing.Background
                      (roleName o.Pairing.Role)
                      (hex o.ForegroundColor)
                      (hex o.BackgroundColor)
                      o.Measured
                      required
                      result)
          "" ]

    [ yield! header
      for theme in themes do
          yield! renderTheme theme
      yield "- regenerate: ./fake.sh build -t RefreshSurfaceBaselines (after a DTCG token edit)"
      yield "- gate: ./fake.sh build -t ContrastCheck"
      yield "- failure-class: sub-threshold-shipped-token"
      yield ""
      yield "## Independent validation / regression protection (US1, SC-005)"
      yield ""
      yield "The gate is independently falsifiable through the DTCG single source only:"
      yield ""
      yield "1. Drop a validated pairing below threshold — e.g. set `dark.danger` near"
      yield "   `dark.background` in `src/Controls/design-tokens.tokens.json`."
      yield "2. `./fake.sh build -t RefreshSurfaceBaselines` regenerates `DesignTokens.fs`."
      yield "3. `./fake.sh build -t ContrastCheck` then FAILS, naming the pairing, the measured"
      yield "   ratio, and the required ratio (see the `## Failing rows` section on a failing run)."
      yield "4. Restoring an accessible value (e.g. a Radix ramp step) makes the gate PASS again."
      if not (List.isEmpty failing) then
          yield ""
          yield "## Failing rows"
          yield ""
          yield! failureDiagnostics outcomes |> List.map (fun d -> sprintf "- %s" d) ]
    |> String.concat Environment.NewLine
