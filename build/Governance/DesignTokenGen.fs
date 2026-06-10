module FS.Skia.UI.Build.DesignTokenGen

open System
open System.Globalization
open System.Text
open System.Text.Json
open System.Text.RegularExpressions

// Feature 069 (US1, FR-001/FR-003/FR-006): make the DTCG document
// `src/Controls/design-tokens.tokens.json` the single source for the 10 FS.Skia.UI.Controls
// theme primitives, generated deterministically (whole-file) into `src/Controls/DesignTokens.fs`.
// This mirrors CatalogGen (the typed catalog single source -> generated catalog rows): the
// canonical values live in one place and the renderer lowers them to F#. Pure:
// parse/renderValue/renderModule/splice/currency are over in-memory text; the file read/write
// stays at the Engine/Interpret.fs edge (Principle IV).

type TokenKind =
    | Color
    | Dimension
    | Number
    | FontFamily

type DesignTokenFact =
    { Theme: string
      Name: string
      Kind: TokenKind
      Rendered: string }

type RegionStatus =
    | Current
    | Stale
    | Missing

type TokenCurrency =
    { Token: string
      Theme: string
      FilePath: string
      Status: RegionStatus }

let tokensJsonRel = "src/Controls/design-tokens.tokens.json"
let designTokensFsRel = "src/Controls/DesignTokens.fs"

let private regenCommand = "./fake.sh build -t RefreshSurfaceBaselines"

// ---------------------------------------------------------------------------------------
// The canonical taxonomy. The 10 Theme primitives plus (feature 093, E3) the two
// variant-only colour tokens `success`/`warning` the style resolver reads — each in both
// themes, iterated in a fixed order so generation is deterministic (SC-006) regardless of
// JSON property ordering. `success`/`warning` feed `Style.resolve`'s Success/Warning variants
// (FR-008: variant colours come from the DTCG source, never inline literals); they are NOT
// `Theme` fields. A token the mapping requires but the DTCG source omits is a loud generation
// failure (FR-006), never a silent skip.
// ---------------------------------------------------------------------------------------
let private themeOrder = [ "light"; "dark" ]

let private tokenOrder =
    [ "foreground"
      "background"
      "accent"
      "danger"
      "success"
      "warning"
      "muted"
      "fontFamily"
      "fontSize"
      "density"
      "cornerRadius"
      "contrastRequiredRatio" ]

let private pascalTheme (theme: string) =
    theme.Substring(0, 1).ToUpperInvariant() + theme.Substring(1)

let private requireString (context: string) (value: JsonElement) : string =
    match value.GetString() |> Option.ofObj with
    | Some s -> s
    | None -> failwithf "design-tokens: %s must be a string" context

let private kindOfType (typ: string) =
    match typ with
    | "color" -> Color
    | "dimension" -> Dimension
    | "number" -> Number
    | "fontFamily" -> FontFamily
    | other -> failwithf "design-tokens: unknown $type '%s' (expected color | dimension | number | fontFamily)" other

let private typeAnnotation kind =
    match kind with
    | Color -> "Color"
    | FontFamily -> "string option"
    | Dimension
    | Number -> "float"

// ---------------------------------------------------------------------------------------
// Value renderers. Each lowers an alias-resolved concrete DTCG value to the exact F# literal
// today's Theme.fs uses, so the migration diff for the values is empty (FR-003).
// ---------------------------------------------------------------------------------------
let private renderColorHex (raw: string) : string =
    let hex = raw.TrimStart('#')

    if hex.Length <> 8 then
        failwithf "design-tokens: color value '%s' must be an 8-digit #rrggbbaa hex string" raw

    let byteAt (i: int) =
        try
            Convert.ToByte(hex.Substring(i, 2), 16)
        with _ ->
            failwithf "design-tokens: color value '%s' is not valid hexadecimal" raw

    sprintf "Colors.rgba %duy %duy %duy %duy" (byteAt 0) (byteAt 2) (byteAt 4) (byteAt 6)

let private renderFloat (d: float) : string =
    if Double.IsInteger d then
        sprintf "%.1f" d
    else
        d.ToString("R", CultureInfo.InvariantCulture)

let private renderConcrete (kind: TokenKind) (value: JsonElement) : string =
    match kind with
    | Color -> renderColorHex (requireString "color $value" value)
    | Dimension
    | Number -> renderFloat (value.GetDouble())
    | FontFamily ->
        if value.ValueKind = JsonValueKind.Null then "None"
        else sprintf "Some \"%s\"" (requireString "fontFamily $value" value)

// ---------------------------------------------------------------------------------------
// Parse + deterministic alias resolution with cycle detection. A DTCG alias is the string
// "{group.token}"; resolution follows the chain to a concrete value, failing loudly on a
// cycle or an unresolvable reference (FR-006). No partial fact list is ever returned.
// ---------------------------------------------------------------------------------------
let private aliasRegex = Regex(@"^\{(?<ref>[^}]+)\}$", RegexOptions.Compiled)

let parse (tokensJsonText: string) : DesignTokenFact list =
    use doc =
        try
            JsonDocument.Parse(tokensJsonText)
        with ex ->
            failwithf "design-tokens: malformed DTCG JSON — %s" ex.Message

    let root = doc.RootElement

    // (theme, token) -> ($type, $value) for every declared token.
    let raw = System.Collections.Generic.Dictionary<string * string, string * JsonElement>()

    for themeProp in root.EnumerateObject() do
        if not (themeProp.Name.StartsWith("$")) then
            for tokenProp in themeProp.Value.EnumerateObject() do
                let typ =
                    match tokenProp.Value.TryGetProperty("$type") with
                    | true, t -> requireString (sprintf "token '%s.%s' $type" themeProp.Name tokenProp.Name) t
                    | false, _ -> failwithf "design-tokens: token '%s.%s' is missing $type" themeProp.Name tokenProp.Name

                let value =
                    match tokenProp.Value.TryGetProperty("$value") with
                    | true, v -> v
                    | false, _ -> failwithf "design-tokens: token '%s.%s' is missing $value" themeProp.Name tokenProp.Name

                raw[(themeProp.Name, tokenProp.Name)] <- (typ, value)

    let aliasTarget (value: JsonElement) =
        if value.ValueKind = JsonValueKind.String then
            match value.GetString() |> Option.ofObj with
            | Some s ->
                let m = aliasRegex.Match(s)
                if m.Success then Some(m.Groups.["ref"].Value) else None
            | None -> None
        else
            None

    // Resolve (theme, token) to its concrete ($type, $value), following aliases.
    let rec resolve (chain: (string * string) list) (theme: string) (name: string) : string * JsonElement =
        let key = (theme, name)

        if List.contains key chain then
            let cycle = (key :: chain) |> List.rev |> List.map (fun (t, n) -> sprintf "%s.%s" t n) |> String.concat " -> "
            failwithf "design-tokens: cyclic alias detected at '%s.%s' (%s)" theme name cycle

        match raw.TryGetValue key with
        | false, _ -> failwithf "design-tokens: alias references missing token '%s.%s'" theme name
        | true, (typ, value) ->
            match aliasTarget value with
            | Some refPath ->
                let parts = refPath.Split('.')

                if parts.Length <> 2 then
                    failwithf "design-tokens: malformed alias '{%s}' (expected '{group.token}')" refPath

                resolve (key :: chain) parts.[0] parts.[1]
            | None -> (typ, value)

    [ for theme in themeOrder do
          for name in tokenOrder do
              match raw.TryGetValue((theme, name)) with
              | false, _ ->
                  failwithf "design-tokens: required token '%s.%s' is missing from %s" theme name tokensJsonRel
              | true, (declaredType, _) ->
                  let kind = kindOfType declaredType
                  let _, concrete = resolve [] theme name

                  { Theme = theme
                    Name = name
                    Kind = kind
                    Rendered = renderConcrete kind concrete } ]

let renderValue (fact: DesignTokenFact) : string = fact.Rendered

// ---------------------------------------------------------------------------------------
// Whole-file generation (data-model §6). The committed DesignTokens.fs IS this render of the
// DTCG source, so byte-identity is by construction; DesignTokenDrift re-renders and compares.
// ---------------------------------------------------------------------------------------
let renderModule (facts: DesignTokenFact list) : string =
    let factOf theme name =
        facts
        |> List.tryFind (fun f -> f.Theme = theme && f.Name = name)
        |> Option.defaultWith (fun () -> failwithf "design-tokens: fact table is missing '%s.%s'" theme name)

    let sb = StringBuilder()
    let line (s: string) = sb.Append(s).Append('\n') |> ignore

    line "// GENERATED — do not edit. Source of truth: src/Controls/design-tokens.tokens.json"
    line (sprintf "// Regenerate via: %s" regenCommand)
    line "namespace FS.Skia.UI.Controls"
    line ""
    line "open FS.Skia.UI.Scene"
    line ""
    line "module DesignTokens ="

    themeOrder
    |> List.iteri (fun i theme ->
        if i > 0 then line ""
        line (sprintf "    module %s =" (pascalTheme theme))

        for name in tokenOrder do
            let fact = factOf theme name
            line (sprintf "        let %s : %s = %s" name (typeAnnotation fact.Kind) fact.Rendered))

    sb.ToString()

let splice (tokensJsonText: string) : string = renderModule (parse tokensJsonText)

// ---------------------------------------------------------------------------------------
// Per-token currency over the whole-file generated module. Each on-disk `let <name> : <ty> =
// <value>` is read under its `module <Theme>` and compared to a fresh render of the DTCG
// source. A missing/unparseable generated file yields all-Missing (loud, never silent).
// ---------------------------------------------------------------------------------------
let private moduleRegex = Regex(@"^    module (?<m>\w+) =", RegexOptions.Compiled)
let private letRegex = Regex(@"^        let (?<n>\w+) : .+ = (?<v>.+)$", RegexOptions.Compiled)

let private renderedOnDisk (designTokensFsText: string) : Map<string * string, string> =
    let lines = designTokensFsText.Replace("\r\n", "\n").Split('\n')
    let mutable currentTheme = ""
    let found = System.Collections.Generic.Dictionary<string * string, string>()

    for l in lines do
        let mm = moduleRegex.Match l

        if mm.Success then
            currentTheme <- mm.Groups.["m"].Value.ToLowerInvariant()
        elif currentTheme <> "" then
            let lm = letRegex.Match l

            if lm.Success then
                found[(currentTheme, lm.Groups.["n"].Value)] <- lm.Groups.["v"].Value

    found |> Seq.map (fun kv -> kv.Key, kv.Value) |> Map.ofSeq

let currency (tokensJsonText: string) (designTokensFsText: string) : TokenCurrency list =
    let facts = parse tokensJsonText
    let onDisk = renderedOnDisk designTokensFsText

    facts
    |> List.map (fun fact ->
        let status =
            match Map.tryFind (fact.Theme, fact.Name) onDisk with
            | None -> Missing
            | Some actual -> if actual = fact.Rendered then Current else Stale

        { Token = fact.Name
          Theme = fact.Theme
          FilePath = designTokensFsRel
          Status = status })

let isCurrent (currency: TokenCurrency list) : bool =
    currency |> List.forall (fun c -> c.Status = Current)

let currencyDrift (currency: TokenCurrency list) : string list =
    currency
    |> List.choose (fun c ->
        match c.Status with
        | Current -> None
        | Stale ->
            Some(
                sprintf
                    "%s is stale — its generated token '%s' (%s theme) no longer matches the DTCG source %s. Regenerate via %s."
                    c.FilePath
                    c.Token
                    c.Theme
                    tokensJsonRel
                    regenCommand)
        | Missing ->
            Some(
                sprintf
                    "%s is missing the generated token '%s' (%s theme) declared by the DTCG source %s. Regenerate via %s."
                    c.FilePath
                    c.Token
                    c.Theme
                    tokensJsonRel
                    regenCommand))
