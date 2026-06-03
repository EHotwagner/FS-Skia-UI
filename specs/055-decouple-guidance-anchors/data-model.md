# Phase 1 Data Model: Decoupled Guidance Anchors

All types live in `build/Governance/Guidance.fs` (build-tooling scope, not a
tracked product surface baseline). The gate entry point `runGeneratedGuidanceScan`
in `Guidance.fsi` is unchanged; the types below are exported only if a unit test
references them directly.

## Core types

```fsharp
/// A literal string consumed by tooling/parsers. Matched verbatim
/// (case-insensitive substring), exactly as the pre-055 table did.
type ContractToken =
    { Token: string          // e.g. "[skillist: []]", "skillist:", "Control<'msg>"
      Files: string list }   // governed files that must contain it verbatim

/// How an obligation's concept anchors are evaluated.
type MatchMode =
    | AnyOf   // satisfied when ANY concept anchor is present (the US1 unlock)
    | AllOf   // satisfied only when ALL concept anchors are present (conjunctive rules)

/// A rule a source of truth imposes that some derived guidance file must reflect.
/// Checked by presence-of-concept, not exact wording, so prose may be reworded.
type GuidanceObligation =
    { Id: string             // stable handle surfaced in diagnostics, e.g. "skillist-structured"
      SourceOfTruth: string  // origin of the rule, e.g. "constitution:Local Agent Skills"
      Concepts: string list  // short concept anchors (NOT full sentences)
      Mode: MatchMode
      Files: string list }   // derived guidance files that must reflect the obligation

/// What a check enforces, after decoupling.
type GuidanceCheck =
    { Tag: string                       // finding tag, e.g. "task-skillist-guidance"
      Tokens: ContractToken list        // verbatim-pinned machine tokens
      Obligations: GuidanceObligation list
      Forbidden: ContractToken list }   // stale terms that MUST NOT appear (controls boundary)
```

## Pure evaluator (testability core, R6)

```fsharp
/// Pure: given a lookup from relative path to file content (None = missing file),
/// produce the findings for one check. No IO. Findings reuse the existing
/// ValidationFinding string-with-[tag] convention.
val evaluateGuidanceCheck:
    lookup: (string -> string option) -> check: GuidanceCheck -> string list

/// Thin IO wrapper kept in the front-end: reads each governed file once and
/// delegates to evaluateGuidanceCheck. The three validators
/// (validateTaskSkillistGuidance / validateControlsBoundaryGuidance /
/// validateSerializedRunnerGuidance) become `evaluateGuidanceCheck (realLookup model)`
/// applied to their respective GuidanceCheck values. runGeneratedGuidanceScan
/// aggregates them exactly as today.
```

### Evaluation rules

- **Token present** (each `ContractToken`): for every file in `Files`, the token
  must appear (case-insensitive substring). Missing ⇒
  `"{file}: missing `{token}` [{tag}]"`. Identical to pre-055 behavior.
- **Forbidden** (`controls-boundary` only): the assembled stale terms must NOT
  appear in the combined governed content ⇒ on hit,
  `"generated controls guidance contains stale term `{token}` [{tag}]"`.
- **Obligation reflected** (each `GuidanceObligation`): for every file in `Files`,
  evaluate `Mode` over `Concepts`. `AnyOf` fails when none present; `AllOf` fails
  when any missing. Failure ⇒
  `"{file}: obligation '{id}' ({source}) not reflected [{tag}]"`.
- **Missing file**: `"{file}: missing file [{tag}]"` (preserved).

## Per-check tables (mapping from the pre-055 literal tables)

### task-skillist-guidance

| Pre-055 literal term | New classification |
|---|---|
| `[skillist: []]`, `skillist:`, `deps:`, `[SEH]`, `synthetic-error-handling-approved`, `loaded_at`, `work_started_at`, `readiness/skill-loading-evidence.md` | **ContractToken** (verbatim) |
| `structured`/`structured \`skillist\`` | Obligation `skillist-structured` (AnyOf: `["structured skillist"; "structured \`skillist\`"]`) · source `constitution:Local Agent Skills` |
| `minimal ordered skill set`, `skills in declared order` | Obligation `skillist-minimal-ordered` (AnyOf: `["minimal ordered"; "declared order"]`) |
| `confidence`, `matched signals`, `reviewer disposition` | Obligation `skillist-confidence-fields` (AllOf: `["confidence"; "matched signals"; "reviewer disposition"]`) |
| `small, medium, and broad` | Obligation `skill-breadth` (AnyOf: `["small, medium, and broad"]`) |
| `non-authoritative aggregate` (+ reporting) | Obligation `aggregate-non-authoritative` (AnyOf: `["non-authoritative aggregate"]`) |
| `before and after every status change` | Obligation `graph-before-after` (AnyOf: `["before and after every status change"; "graph before/after"]`) |
| `persistent launch rules`, `persistent graphical launch task`, `MUST reject viewer-backed default executable paths` | Obligation `persistent-launch` (AnyOf those phrases) |
| `malformed parser input`, `convenience mocks`, `implementation-time relabeling` | Obligation `seh-discipline` (AnyOf) · source `constitution:Principle V` |
| `Compulsory skill evaluation`, `Visible skill mirror`, `Declared skill ids resolve` | Obligation `tasks-skill-gate` (AllOf, short anchors) |
| `Resolve every declared skill id`, `loaded paths`, `reviewer exception`, `implementation batch records`, `red-green evidence log` | Obligation `implement-skill-loading` (AnyOf/AllOf as appropriate) |
| `mandatory post-generation skill evaluation gate`, `mandatory pre-task skill loading gate`, `\`skillist\` field` | Obligation `constitution-skill-gates` (AllOf) |
| `After task generation` | Obligation `tasks-post-gen-timing` (AnyOf) |

*(The implementer finalizes exact anchor strings against the live files; the rule is
one obligation per distinct semantic concept the old table encoded, each file in
`Files` covering every twin — template + `fsharp-opinionated` preset copy + command
copy + memory/constitution copy — so drift in one twin is still caught.)*

**Anchor-disjointness rule (FR-003):** every obligation concept anchor MUST be
chosen so it is *not a substring of any `ContractToken` in the same check*.
A bare anchor like `"skillist"` is invalid because the tokens `skillist:` /
`[skillist: []]` already guarantee it — the obligation could then never fail and
drift detection would be silently weaker than the pre-055 literal table. Anchors
are multi-word concept phrases (e.g. `"structured skillist"`), not single tokens.

### controls-boundary-guidance

| Pre-055 term | New classification |
|---|---|
| `FS.Skia.UI.Controls`, `Control<'msg>`, `DataGrid`, `FS.Skia.UI.Controls.Elmish`, `ControlsElmish.program` | **ContractToken** (verbatim, over combined content) |
| `Skia-rendered` | Obligation `controls-skia-rendered` (AnyOf) |
| `legacy Charts package`, `no compatibility shim` | Obligation `controls-no-charts-shim` (AllOf: both concepts) |
| all `forbidden` entries (`FS.Skia.UI.Charts`, `fs-skia-charts`, `chart-only`, `DataGrid as chart`, `renderer neutral`, `host loop ownership`, …) | **Forbidden** ContractToken (unchanged, must NOT appear) |

### sequential-fake-guidance

| Pre-055 term/logic | New classification |
|---|---|
| `FAKE-backed`, `.fake`, `sequential`, `not safe to run concurrently` | Obligation `fake-sequential` (AllOf — all four facets) · source `CLAUDE.md:FAKE concurrency rule` |
| regex: FAKE command present, numbered-order requirement, parallelism non-FAKE caveat | **machine logic** (unchanged structural assertions) |

## Prose-size accounting (FR-007)

```fsharp
type ProseSizeAccounting =
    { Baseline: int          // 6882 (corrected, feature 046)
      AgentsSkillsLines: int // find .agents/skills -name '*.md' | wc -l
      SpecifyLines: int      // find .specify -name '*.md' | wc -l
      Current: int           // sum
      Delta: int             // Current - Baseline
      RestatedTarget: string }
```

Rendered to `readiness/prose-size-accounting.md` with the reproduction commands.
The IO enumeration lives in the front-end; the report-rendering function is pure.
