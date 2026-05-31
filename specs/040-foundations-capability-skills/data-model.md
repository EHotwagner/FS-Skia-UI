# Phase 1 Data Model: Foundations F# Capability Skills

The "data" here is governance/authoring content plus the small typed model the two gates operate on.
Entities below map to the `build/Governance` types (each public module gets a `.fsi`) and to the
authoring schema enforced by the contracts.

## Entity: CapabilitySkill

One refined cookbook skill, existing byte-identically in both trees.

| Field | Type | Notes |
|---|---|---|
| `slug` | string | kebab-case, e.g. `fsharp-parsing`; matches frontmatter `name` and both dir names. |
| `claudePath` | path | `.claude/skills/<slug>/SKILL.md`. |
| `agentsPath` | path | `.agents/skills/<slug>/SKILL.md`. |
| `ownedCapabilities` | Capability list | the C-numbers this skill owns (see CapabilityOwnership). |
| `consumingStages` | string list | plan stages that consume it (FR-009). |
| `codeBlocks` | CodeBlock list | extracted ` ```fsharp ` blocks (FR-012/FR-014). |

**Validation rules**
- Exactly six instances: the slugs in FR-001 (`fsharp-parsing`, `fsharp-graph-algorithms`,
  `fsharp-code-generation`, `fsharp-io-globbing`, `fsharp-shell-process`,
  `fsharp-build-orchestration`).
- Frontmatter MUST carry `name` (= `slug`), one-line `description`, `compatibility` (scopes to
  `build/Governance` net10.0, build-tooling-only), and `metadata.source` = the capability report
  (FR-003).
- Body MUST contain, per owned capability: the adopt verdict + rejected/deferred alternatives
  (FR-004), an API walkthrough of the adopted library, and ≥1 runnable F# example (FR-012); plus a
  Cautions section (FR-006), a Consuming-stages note (FR-009), and a Sources/links section (FR-013).
- Parity-critical skills (`fsharp-parsing`, `fsharp-graph-algorithms`) MUST reproduce the exact
  grammars/rules (FR-005) and state the Stage-0 golden-fixture byte-parity obligation.
- MUST NOT appear in any `tasks.deps.yml` `skillist` / `tasks.md` mirror (FR-007).

## Entity: CodeBlock

One ` ```fsharp ` fenced block extracted from a SKILL.md by the tangler.

| Field | Type | Notes |
|---|---|---|
| `skillSlug` | string | owning skill. |
| `blockIndex` | int | 1-based, per skill, in document order. |
| `startLine` | int | line of the opening fence in `SKILL.md` (diagnostics, R4). |
| `source` | string | the F# text between the fences (verbatim, the single source of truth). |
| `generatedModule` | string | `Skill.<slug_underscored>.Block<NN>` wrapper name (R1). |

**Validation rules**
- `source` MUST be valid F# *module contents* (declarations or `let _ = expr`); no block may depend
  on another block's bindings (R1).
- Every block MUST compile in the generated examples project against the pinned adopt-set packages
  (FR-014/SC-007). The compile is the evidence; there is no hand-duplicated copy of the example.

## Entity: SkillPair

The byte-identity unit checked by `SkillSyncCheck`.

| Field | Type | Notes |
|---|---|---|
| `slug` | string | |
| `claudeHash` | hex string | SHA-256 of `claudePath` bytes. |
| `agentsHash` | hex string | SHA-256 of `agentsPath` bytes. |
| `inSync` | bool | `claudeHash = agentsHash`. |

**Validation rules**
- All six pairs MUST be `inSync` for the gate to PASS (FR-002/SC-002).
- On any `inSync = false`, the gate FAILs and the message names the drifted `slug` and both hashes
  (FR-011). Missing either file is a FAIL (no silent skip; Principle VII).

## Entity: CapabilityOwnership (C1–C21 → exactly one skill)

| Capability(s) | Family | Owning skill |
|---|---|---|
| C1, C2, C3, C4, C5, C16, C21 | parsing + JSON I/O + regex | `fsharp-parsing` (C5 JSON-write folded with C4, R7) |
| C6, C7, C8, C9 | graph algorithms | `fsharp-graph-algorithms` |
| C10, C11, C12 | document/artifact gen + F# source gen + quotations caveat | `fsharp-code-generation` |
| C13, C14 | file discovery + glob (fnmatch) | `fsharp-io-globbing` |
| C15, C17 | git + process wrapping | `fsharp-shell-process` |
| C18, C19, C20 | orchestration + diffing + testing | `fsharp-build-orchestration` |

**Validation rules**
- Union of owned capabilities = {C1..C21}; intersection across skills = ∅ (SC-001: 100% coverage,
  single ownership). FR-001 phrases parsing ownership as C1–C4, C16, C21; C5 is co-located with C4 in
  the parsing skill per the report's §3.4 JSON read/write pairing.
- The report's **eight** capability families (report §2, line 76) collapse onto the six skills as:
  *parsing* + *JSON I/O* → `fsharp-parsing`; *graph algorithms* → `fsharp-graph-algorithms`;
  *document/artifact generation* + *F# source generation* → `fsharp-code-generation`;
  *file discovery & globbing* → `fsharp-io-globbing`; *shell/process/git wrapping* →
  `fsharp-shell-process`; *build orchestration/CLI* + *diffing* + *testing* → `fsharp-build-orchestration`.
  This is why the spec says "eight families" (SC-001/US3) while this table has six owning-skill rows.

## Entity: AdoptVerdict (content shape, per capability)

Captured in prose inside each skill, not a runtime type. Each owned capability records: adopted
package(s) + version where pinned; rejected/deferred alternatives each with a one-line reason
(FR-004). Authoritative values come from report §3–§9 and the §9 decision table and are NOT re-opened
(spec Assumptions).

## State / lifecycle

No durable runtime state. The only "transition" is authoring: a skill is **refined** (content uplift
to the cookbook bar, FR-010/SC-006), the tangler **regenerates** `build/SkillExamples/Generated/*.fs`
from the current SKILL.md text on every `SkillExamplesCheck` run, and the hasher **recomputes** pair
hashes on every `SkillSyncCheck` run. Generated `.fs` files are derived artifacts — never hand-edited,
regenerated deterministically from the skills.
