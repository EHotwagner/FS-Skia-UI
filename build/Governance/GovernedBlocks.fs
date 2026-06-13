module FS.Skia.UI.Build.GovernedBlocks

open System
open System.Text.RegularExpressions

// Feature 057 (US1, FR-002/FR-003/FR-006): single-source the duplicated governance
// corpus. This generalizes `ConstitutionFragments` from "first sentence of a principle"
// to "arbitrary keyed prose block spliced into N home files", adding a per-target
// `RenderMode` so the placeholder-bearing canonical text can be rendered verbatim (the
// template/twin form) or with its placeholders substituted (the concrete repo form).
//
// `Guidance.fs` remains the single home of the rule *set* (FR-004); this store changes
// only how the prose that satisfies those rules is *carried* into the home files. Pure:
// render + currency are over in-memory text; the file read/write stays at the
// `Interpret.fs` edge (Principle IV).

/// How a governed block's canonical (placeholder-bearing) text is rendered into a target.
type RenderMode =
    /// Placeholders preserved — the generic template / twin form.
    | Verbatim
    /// `[KEY]` placeholders filled with repo values — the concrete `constitution.md` form.
    | Substituted of Map<string, string>

/// The canonical, single-source value for a generated prose block. `Targets` lists every
/// home file the block is spliced into, each with its render mode. `Tokens`/`Obligations`
/// cross-reference the `Guidance.fs` rules this block satisfies (so the catalogue and the
/// silent-drift audit can pair them).
type GovernedBlock =
    { Id: string
      CanonicalText: string
      Targets: (string * RenderMode) list
      Tokens: string list
      Obligations: string list }

/// Status of one `gov/<id>` region found on disk relative to a fresh render.
type RegionStatus =
    | Current
    | Stale
    | Missing

/// Currency result for one (file, block) pair.
type BlockCurrency =
    { FilePath: string
      BlockId: string
      Status: RegionStatus }

let private regenCommand = "./fake.sh build -t RefreshSurfaceBaselines"

let private normalizeNewlines (text: string) = text.Replace("\r\n", "\n")

/// Render the canonical text for one target: verbatim, or with each `[KEY]` placeholder
/// substituted by its mapped value.
let renderText (mode: RenderMode) (canonicalText: string) : string =
    match mode with
    | Verbatim -> canonicalText
    | Substituted substitutions ->
        substitutions
        |> Map.fold (fun (acc: string) key value -> acc.Replace("[" + key + "]", value)) canonicalText

// One BEGIN/END pair for the `gov/<id>` marker family (distinct from the
// `constitution/<id>` family `ConstitutionFragments` owns). Singleline so `.` crosses
// newlines; the non-greedy inner stops at the first matching END marker.
let private regionRe =
    Regex(
        @"<!-- BEGIN GENERATED: gov/(?<id>[^\s]+) -->(?<inner>.*?)<!-- END GENERATED: gov/\k<id> -->",
        RegexOptions.Singleline ||| RegexOptions.Compiled)

let private beginMarker id = sprintf "<!-- BEGIN GENERATED: gov/%s -->" id
let private endMarker id = sprintf "<!-- END GENERATED: gov/%s -->" id

let private expectedInner (mode: RenderMode) (block: GovernedBlock) =
    "\n" + renderText mode block.CanonicalText + "\n"

/// Ids of every `gov/<id>` region present in a file, in document order.
let regionIds (fileText: string) : string list =
    [ for m in regionRe.Matches(normalizeNewlines fileText) -> m.Groups.["id"].Value ]

/// Splice the block's rendered text into every `gov/<block.Id>` region of `fileText`,
/// preserving every byte outside the markers (FR-006). Unknown marker ids are left
/// byte-for-byte untouched (honest drift). Idempotent on an already-current file.
let splice (block: GovernedBlock) (mode: RenderMode) (fileText: string) : string =
    regionRe.Replace(
        fileText,
        (fun (m: Match) ->
            if m.Groups.["id"].Value = block.Id then
                beginMarker block.Id + expectedInner mode block + endMarker block.Id
            else
                m.Value))

/// Compare a file's committed `gov/<block.Id>` region against a fresh render.
/// Missing when the file carries no region for the block; Stale when the inner text
/// differs; Current otherwise.
let currency (filePath: string) (block: GovernedBlock) (mode: RenderMode) (fileText: string) : BlockCurrency =
    let normalized = normalizeNewlines fileText

    let matching =
        [ for m in regionRe.Matches normalized do
              if m.Groups.["id"].Value = block.Id then
                  yield m.Groups.["inner"].Value ]

    let status =
        match matching with
        | [] -> Missing
        | inners when inners |> List.forall (fun inner -> normalizeNewlines inner = expectedInner mode block) -> Current
        | _ -> Stale

    { FilePath = filePath
      BlockId = block.Id
      Status = status }

/// Actionable currency diagnostic naming the drifted file AND its canonical source
/// (the `gov/<id>` block), plus the regeneration command. None when current.
let currencyDrift (currency: BlockCurrency) : string option =
    match currency.Status with
    | Current -> None
    | Stale ->
        Some(
            sprintf
                "%s is stale — its generated `gov/%s` region no longer matches its canonical source (GovernedBlocks `%s`). Regenerate via %s."
                currency.FilePath
                currency.BlockId
                currency.BlockId
                regenCommand)
    | Missing ->
        Some(
            sprintf
                "%s is missing the generated `gov/%s` region for canonical source GovernedBlocks `%s`. Regenerate via %s."
                currency.FilePath
                currency.BlockId
                currency.BlockId
                regenCommand)

/// Every (file, block, mode) currency pair for a block across all its targets, given a
/// lookup from relative path to on-disk content (None = missing file).
let blockCurrencies (lookup: string -> string option) (block: GovernedBlock) : BlockCurrency list =
    block.Targets
    |> List.map (fun (filePath, mode) ->
        match lookup filePath with
        | None -> { FilePath = filePath; BlockId = block.Id; Status = Missing }
        | Some content -> currency filePath block mode content)

// ---------------------------------------------------------------------------------------
// Canonical store. The single-source values for the generated prose blocks. Each block's
// `CanonicalText` is the one true text; `RefreshSurfaceBaselines` splices it into every
// `Targets` file and `TargetMetadataDrift` asserts each region matches. See the FR-001
// duplication catalogue for the full class map.
// ---------------------------------------------------------------------------------------

/// `gov/visual-proof-phrases` — the exact visual-proof rejection phrases pinned by
/// `AsteroidsFeedbackSkillGuidanceTests`. Sole verbatim carrier within the layout-evidence
/// SKILL; duplicated cross-file into the template-owned `product.md` (catalogue class 3 →
/// GenerateAndCheck). The `.claude` peer is regenerated by `SkillSyncCheck`.
let visualProofPhrases =
    { Id = "visual-proof-phrases"
      CanonicalText =
        "Exact visual proof rejection phrases for scans: metadata-only reports do not satisfy visual proof; 1x1 fallback images do not satisfy visual proof; layout-only bounds claims do not satisfy visual proof."
      Targets =
        [ ".agents/skills/fs-skia-evidence-mode/SKILL.md", Verbatim
          "template/base/docs/product.md", Verbatim ]
      Tokens = []
      Obligations = [ "visual-proof-echo" ] }

/// `gov/owner-phrases` — the exact owner / host-warning classification phrases pinned by
/// `AsteroidsFeedbackSkillGuidanceTests`. Same collapse class as `visual-proof-phrases`.
let ownerPhrases =
    { Id = "owner-phrases"
      CanonicalText =
        "Exact owner phrases for scans: framework runtime; generated template workflow; documentation discoverability; consumer authoring; persistent-window blocking; display/session availability; auto-close smoke; benign warning; blocking warning; deferred warning; name-collision guidance."
      Targets =
        [ ".agents/skills/fs-skia-evidence-mode/SKILL.md", Verbatim
          "template/base/docs/product.md", Verbatim ]
      Tokens = []
      Obligations = [ "owner-phrases-echo" ] }

/// The authoritative governed-block inventory. Adding/removing a block is a contract
/// change (currency-checked by `TargetMetadataDrift`), not an implementation detail.
let governedBlocks : GovernedBlock list =
    [ visualProofPhrases
      ownerPhrases ]

// ---------------------------------------------------------------------------------------
// Constitution single-sourcing (class 4, FR-007). The placeholder-bearing
// `.specify/templates/constitution-template.md` is the **canonical** source. Two render
// modes are generated and currency-checked:
//   * the preset twin (`.specify/presets/.../constitution-template.md`) — Verbatim
//     (byte-identical copy of the canonical twin).
//   * `.specify/memory/constitution.md` — the concrete repo form: placeholders filled,
//     the LOCKED/REQUIRED/TAILORABLE editorial comments stripped, and the generic
//     local-skill intro/enumeration replaced with this repo's concrete capability-skill
//     prose.
// The concrete transform is an ordered list of exact (find, replace) edits. A golden unit
// test (`GovernedBlocksTests`) asserts `renderConstitution Concrete canonical =
// constitution.md` and `twin = canonical`, so the edit list cannot silently drift from the
// committed files.
// ---------------------------------------------------------------------------------------

let constitutionCanonicalRel = ".specify/templates/constitution-template.md"
let constitutionTwinRel = ".specify/presets/fsharp-opinionated/templates/constitution-template.md"
let constitutionConcreteRel = ".specify/memory/constitution.md"

type ConstitutionTarget =
    | Concrete
    | Twin

let private joinLines (xs: string list) = String.concat "\n" xs

// Each (find, replace) maps the canonical twin text to the concrete constitution.md text.
// Applied in order via String.Replace (each find is unique in the document).
let constitutionConcreteReplacements : (string * string) list =
    [ // R1 — title: drop the REQUIRED comment and fill [PROJECT_NAME].
      joinLines
          [ "<!-- REQUIRED: fill during /speckit.constitution -->"
            "# [PROJECT_NAME] Constitution" ],
      "# FS-Skia-UI Constitution"

      // R2 — strip the LOCKED principles banner (keep the two blank lines).
      joinLines
          [ ""
            ""
            "<!-- LOCKED: do not modify during /speckit.constitution without user override."
            "     These seven principles are the shared doctrine of the fsharp-opinionated"
            "     preset. Per-project amendment requires explicit user direction and SHOULD"
            "     be followed by a PR to the preset itself so the change propagates. -->"
            ""
            "## Core Principles" ],
      joinLines [ ""; ""; ""; "## Core Principles" ]

      // R3 — Principle V synthetic list wording.
      joinLines
          [ "substitutes, `NotImplementedException` placeholders, `failwith \"TODO\"`,"
            "canned responses," ],
      joinLines
          [ "substitutes, unfinished-code placeholder exceptions, TODO-style failing"
            "placeholders, canned responses," ]

      // R4 — Principle VI skip mechanism.
      "`[<Skip>]` attribute, or the test framework's skip mechanism)",
      "the relevant test-framework skip attribute, or the test framework's skip mechanism)"

      // R5 — strip the LOCKED banner before Change Classification.
      joinLines [ ""; "<!-- LOCKED -->"; "## Change Classification" ],
      joinLines [ ""; "## Change Classification" ]

      // R6 — strip the TAILORABLE banner before Engineering Constraints.
      joinLines
          [ "regardless of whether tests pass."
            ""
            "<!-- TAILORABLE: tune per project. Keep the stack-exclusivity rule unless the"
            "     project is intentionally polyglot. Pack output path, logging library,"
            "     and dependency policy are expected to differ per project. -->"
            ""
            "## Engineering Constraints" ],
      joinLines
          [ "regardless of whether tests pass."
            ""
            ""
            "## Engineering Constraints" ]

      // R7/R8/R9 — fill the Engineering-Constraints placeholders.
      "- Pack output location: [PACK_OUTPUT_PATH]",
      "- Pack output location: `~/.local/share/nuget-local/`"

      "- Structured-logging library: [LOGGING_LIBRARY]",
      "- Structured-logging library: not yet selected; see ADR when chosen."

      "- Project-specific constraints: [PROJECT_CONSTRAINTS]",
      "- Project-specific constraints: build a new FS-Skia-UI implementation inspired by SkiaViewer, using SkiaSharp 4 preview packages with explicit version pinning, Elmish for application workflow, and .NET `net10.0` unless a plan justifies a narrower target. Treat the upstream SkiaViewer repository as behavioral/reference material only unless license attribution and source reuse are documented in the spec and plan. The primary development environment is expected to provide GPU passthrough; OpenGL smoke failures must distinguish implementation defects from missing window-system or presentation setup rather than assuming GPU access is unavailable."

      // R10 — Local Agent Skills intro paragraph (generic → concrete capability framing).
      joinLines
          [ "Local skills under `.agents/skills/*/SKILL.md`, package-owned"
            "`src/*/skill/SKILL.md`, and template-owned skill paths are repository"
            "governance artifacts. Agents MUST use the applicable local skill whenever a"
            "task matches the skill's description, and MUST prefer local project or"
            "capability skills over generic guidance when both apply. If more than one"
            "skill applies, use the minimal set that covers the work and follow the" ],
      joinLines
          [ "Capability skills are package-owned `src/*/skill/SKILL.md` files,"
            "capability-owned template fragment skill paths declared by"
            "`template/capabilities.yml`, or explicitly registered repo-local governance"
            "skills under `.agents/skills/*/SKILL.md`. Agents MUST use the applicable"
            "capability skill whenever a task matches the skill's description, and MUST"
            "prefer capability skills over generic guidance when both apply. If more than"
            "one skill applies, use the minimal set that covers the work and follow the" ]

      // R11 — skill-inventory directive → the repo's enumerated capability-skill list.
      joinLines
          [ "When this constitution is instantiated or amended, enumerate the current"
            "`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, and template-owned skill"
            "inventory here and keep the list synchronized with local skill additions,"
            "removals, or renames." ],
      joinLines
          [ "Current FS.Skia.UI capability skills:"
            ""
            "- `fs-skia-scene` — work on dependency-light scene primitives and generated"
            "  product scene usage (`src/Scene/skill/SKILL.md`)."
            "- `fs-skia-skiaviewer` — work on viewer host contracts and generated product"
            "  viewer usage (`src/SkiaViewer/skill/SKILL.md`)."
            "- `fs-skia-elmish` — work on Elmish adapter contracts and generated product"
            "  Elmish wiring (`src/Elmish/skill/SKILL.md`)."
            "- `fs-skia-keyboard-input` — work on keyboard input contracts and generated"
            "  product keyboard guidance (`src/KeyboardInput/skill/SKILL.md`)."
            "- `fs-skia-layout` — work on Yoga-backed layout contracts and generated"
            "  product layout usage (`src/Layout/skill/SKILL.md`)."
            "- `fs-skia-ui-widgets` — work on Skia-rendered Controls, rich text, chart"
            "  controls, graph controls, DataGrid, and generated product Controls guidance"
            "  (`src/Controls/skill/SKILL.md`)."
            "- `fs-skia-testing` — work on generated product and package validation"
            "  helper contracts (`src/Testing/skill/SKILL.md`)."
            "- `fs-skia-samples` — work on optional generated product sample-pack content"
            "  for the non-runtime Samples capability"
            "  (`template/fragments/samples/skill/SKILL.md`)."
            "- `fs-skia-layout-readability` — work on generated game HUD readability,"
            "  gameplay-region bounds, and public scene/host/update naming guidance"
            "  (`.agents/skills/fs-skia-layout-readability/SKILL.md`)."
            "- `fs-skia-evidence-mode` — work on deterministic render-only evidence,"
            "  visual-proof honesty, and benign host warning classification"
            "  (`.agents/skills/fs-skia-evidence-mode/SKILL.md`)." ]

      // R12 — strip the LOCKED banner before Workflow & Quality Gates.
      joinLines [ ""; "<!-- LOCKED -->"; "## Workflow & Quality Gates" ],
      joinLines [ ""; "## Workflow & Quality Gates" ]

      // R13 — strip the LOCKED banner before Governance.
      joinLines [ ""; "<!-- LOCKED -->"; "## Governance" ],
      joinLines [ ""; "## Governance" ]

      // R14 — version line: drop the REQUIRED comment and fill the version placeholders.
      joinLines
          [ "<!-- REQUIRED -->"
            "**Version**: [CONSTITUTION_VERSION] | **Ratified**: [RATIFICATION_DATE] | **Last Amended**: [LAST_AMENDED_DATE]" ],
      "**Version**: 1.3.0 | **Ratified**: 2026-05-12 | **Last Amended**: 2026-05-27" ]

/// Render one constitution target from the canonical placeholder-bearing twin text.
let renderConstitution (target: ConstitutionTarget) (canonicalText: string) : string =
    let normalized = normalizeNewlines canonicalText

    match target with
    | Twin -> normalized
    | Concrete ->
        constitutionConcreteReplacements
        |> List.fold (fun (acc: string) (find, replace) -> acc.Replace(find, replace)) normalized

/// Currency for a generated constitution target: None when current, else a drift
/// diagnostic naming the drifted file and its canonical source.
let constitutionDrift (relPath: string) (sourceRel: string) (rendered: string) (actualText: string) : string option =
    if normalizeNewlines actualText = rendered then
        None
    else
        Some(
            sprintf
                "%s is stale — it no longer matches the render of its canonical source %s. Regenerate via %s."
                relPath
                sourceRel
                regenCommand)
