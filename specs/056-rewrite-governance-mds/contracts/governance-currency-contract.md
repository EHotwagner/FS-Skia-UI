# Contract: Governance Currency Preservation

This feature exposes no new external interface. Its "contract" is the set of
preservation invariants the rewrite must satisfy — the exact currency contract
that feature 055 encoded in `build/Governance/Guidance.fs`, restated here as the
acceptance surface every rewritten file is checked against. This document is the
authority a reviewer and the `GeneratedGuidanceCheck` gate use to accept or reject
a rewrite. **None of these values is edited by this feature; the rewrite satisfies
them.**

## C1 — Contract tokens (verbatim, per home file)

Each token MUST remain a (case-insensitive) substring of **every** file listed.

| Token | Home files |
|---|---|
| `[skillist: []]` | `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md` |
| `skillist:` | `.specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml` |
| `deps:` | `.specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml` |
| `[SEH]` | tasks-template ×2, speckit-tasks SKILL + command, speckit-implement SKILL + command, constitution.md + constitution-template ×2 |
| `synthetic-error-handling-approved` | same set as `[SEH]` |
| `loaded_at` | speckit-implement SKILL + `speckit.implement.md` |
| `work_started_at` | speckit-implement SKILL + `speckit.implement.md` |
| `readiness/skill-loading-evidence.md` | speckit-implement SKILL + `speckit.implement.md` |
| `FS.Skia.UI.Controls` | controls/elmish fragment READMEs, controls fragment SKILL, base README, base product.md, `src/Controls/skill/SKILL.md` |
| `Control<'msg>` | controls/elmish fragment READMEs, controls fragment SKILL, `src/Controls/skill/SKILL.md` |
| `DataGrid` | controls fragments, base README, base product.md, `src/Controls/skill/SKILL.md`, spec-template ×2 |
| `FS.Skia.UI.Controls.Elmish` | controls fragment SKILL, elmish README, base README, base product.md, `src/Controls/skill/SKILL.md` |
| `ControlsElmish.program` | `template/fragments/elmish/README.md` |

> The authoritative `Files` lists live in `taskSkillistGuidanceCheck` and
> `controlsBoundaryGuidanceCheck`; this table is the human-readable mirror.

## C2 — Semantic obligations (concept anchors must stay matchable)

AnyOf → at least one anchor present per home file. AllOf → **all** anchors present
per home file (the fragile rules).

| Id | Mode | Concept anchors |
|---|---|---|
| `skillist-structured` | AnyOf | structured skillist / structured \`skillist\` |
| `skillist-minimal-ordered` | AnyOf | minimal ordered / declared order |
| `skillist-confidence-fields` | AllOf | confidence · matched signals · reviewer disposition |
| `skill-breadth` | AnyOf | small, medium, and broad |
| `aggregate-non-authoritative` | AnyOf | non-authoritative aggregate |
| `graph-before-after` | AnyOf | before and after every status change / graph before/after |
| `persistent-launch` | AnyOf | persistent launch rules / persistent graphical launch task / MUST reject viewer-backed default executable paths |
| `seh-discipline` | AnyOf | malformed parser input / convenience mocks / implementation-time relabeling |
| `tasks-skill-gate` | AllOf | Compulsory skill evaluation · Visible skill mirror · Declared skill ids resolve |
| `implement-skill-loading` | AllOf | Resolve every declared skill id · loaded paths · reviewer exception · implementation batch records · red-green evidence log |
| `constitution-skill-gates` | AllOf | mandatory post-generation skill evaluation gate · mandatory pre-task skill loading gate · \`skillist\` field |
| `tasks-post-gen-timing` | AnyOf | After task generation |
| `deps-skillist-doc` | AnyOf | ordered list of applicable capability skill identifiers |
| `controls-skia-rendered` | AnyOf | Skia-rendered |
| `controls-no-charts-shim` | AllOf | legacy Charts package · no compatibility shim |
| `fake-sequential` | AllOf | FAKE-backed · .fake · sequential · not safe to run concurrently |

## C3 — Forbidden terms (must stay absent)

`FS.Skia.UI.Charts`, `fs-skia-charts`, `chart-only`, `DataGrid as chart`,
`DataGrid-as-chart`, `renderer-neutral`, `renderer neutral`,
`host-loop ownership`, `host loop ownership`, plus reflection-first /
repository-source-copy advice phrases. No rephrasing may reintroduce any.

## C4 — Generation currency

- `.claude/skills/**` regenerated from `.agents/skills/**` via
  `RefreshSurfaceBaselines`; `SkillSyncCheck` green.
- `validation.contract.yml` stays generated from `Routing.fs` (unedited);
  `TargetMetadataDrift` green.
- `active`/`preset` template twins satisfy `active-preset-parity`.

## C5 — Readability (human contract)

Every rule previously conveyed remains extractable by a reader — tightness may not
make an obligation ambiguous, incomplete, or unreadable even if its currency
keyword still technically matches (FR-005, SC-006). This is reviewer-judged, not
gate-checked.

## Acceptance

A rewrite is accepted iff: C1 ✓ (all tokens present per file), C2 ✓ (all
obligations resolve per file/mode), C3 ✓ (no forbidden term), C4 ✓ (generation
current), C5 ✓ (reviewer confirms readability), `GeneratedGuidanceCheck`/
`SkillSyncCheck`/`TargetMetadataDrift`/`TemplateCheck` green, and a recorded
mutation still fails the gate (drift detection intact).
