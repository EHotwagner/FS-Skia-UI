// DesignTokenGen.fsi — the DTCG parser/generator for the 10 FS.Skia.UI.Controls theme
// primitives (feature 069, FR-001/FR-003/FR-004/FR-006). Build-tooling scope
// (`build/Governance`, net10.0); NOT a tracked runtime surface baseline (same status as
// CatalogGen.fsi / SkillTreeGen.fsi). Pure generation + currency over in-memory text; the
// only filesystem read/write stays at the Engine/Interpret.fs edge (Principle IV).
//
// Mirrors CatalogGen.fsi (RegionStatus / *Currency / render*/splice/currency/isCurrent/
// currencyDrift). Difference: the single source is the checked-in DTCG JSON document (parsed
// here via `parse`), not a compiled F# fact value, and the generated module is whole-file
// (data-model §6). currencyDrift still names the divergent token per-token (FR-004).
module FS.Skia.UI.Build.DesignTokenGen

/// What an alias-resolved DTCG token lowers to in F#.
type TokenKind =
    | Color
    | Dimension
    | Number
    | FontFamily

/// One alias-resolved token in one theme — the unit of generation and currency
/// (mirrors CatalogGen.TypedCatalogFact). `Rendered` is the exact F# value literal.
type DesignTokenFact =
    { Theme: string          // "light" | "dark"
      Name: string           // e.g. "foreground"
      Kind: TokenKind
      Rendered: string }     // e.g. "Colors.rgba 31uy 41uy 55uy 255uy" | "14.0" | "None"

/// Status of one generated token relative to a fresh render.
type RegionStatus =
    | Current
    | Stale
    | Missing

/// Currency result for one token in the generated module.
type TokenCurrency =
    { Token: string
      Theme: string
      FilePath: string
      Status: RegionStatus }

/// Repo-relative home files: the DTCG single source and the generated module.
val tokensJsonRel: string     // = "src/Controls/design-tokens.tokens.json"
val designTokensFsRel: string // = "src/Controls/DesignTokens.fs"

/// Parse the DTCG JSON text into the alias-resolved fact table (the single source, FR-001).
/// Resolves DTCG aliases ("{group.token}") deterministically. A malformed document, an
/// unresolvable/cyclic alias, or a token the Theme mapping requires but is missing raises a
/// generation failure naming the offending token — never a partial result (FR-006, edge cases).
/// Facts are returned in canonical (theme, token) order for determinism (SC-006). Pure over
/// the input string.
val parse: tokensJsonText: string -> DesignTokenFact list

/// Render one fact to its F# value literal (byte-identical to today's Theme.fs literal — FR-003).
/// `parse` precomputes the literal (hex color -> `Colors.rgba R G B A`; dimension/number -> float
/// w/ decimal point; fontFamily null -> `None`, concrete -> `Some "<name>"`), so this returns the
/// resolved `Rendered`. Pure.
val renderValue: fact: DesignTokenFact -> string

/// Render the whole generated module text from the fact table (whole-file generation,
/// data-model §6), including the `GENERATED — do not edit` banner and the regenerate command. Pure.
val renderModule: facts: DesignTokenFact list -> string

/// The whole-file regeneration applied during RefreshSurfaceBaselines: parse the DTCG source and
/// produce the current generated module text. Pure over the DTCG input string.
val splice: tokensJsonText: string -> string

/// Compare the on-disk generated module against a fresh render of the DTCG source, per token.
/// Pure. Clean iff every status is Current; a missing/unparseable generated file yields Missing.
val currency: tokensJsonText: string -> designTokensFsText: string -> TokenCurrency list

/// True when every token is current.
val isCurrent: currency: TokenCurrency list -> bool

/// Actionable drift diagnostics — one per stale/missing token, naming the token, the theme, the
/// generated file, and `./fake.sh build -t RefreshSurfaceBaselines` (FR-004). Empty when current.
val currencyDrift: currency: TokenCurrency list -> string list
