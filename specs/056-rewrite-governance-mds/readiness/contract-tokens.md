# Contract-Token & Obligation Preservation Inventory — Feature 056

The authority on what the rewrite may **not** cut. Enumerated from the live
`build/Governance/Guidance.fs` values (`taskSkillistGuidanceCheck`,
`controlsBoundaryGuidanceCheck`, `serializedRunnerObligation`) — read-only in
this feature's scope; the rewrite **satisfies** these values, it never edits
them. Each token/obligation is confirmed present/matchable in every home file
**after** the rewrite (T010), and the recorded mutation (T011) proves a deleted
obligation still fails the gate.

## C1 — Contract tokens (verbatim, case-insensitive substring, per home file)

| Token | Home files | Post-rewrite |
|-------|------------|--------------|
| `[skillist: []]` | tasks-template.md (+ preset twin) | ✓ present |
| `skillist:` | tasks-deps-template.yml | ✓ present |
| `deps:` | tasks-deps-template.yml | ✓ present |
| `[SEH]` | tasks-template ×2, speckit-tasks SKILL + command, speckit-implement SKILL + command, constitution.md + constitution-template ×2 | ✓ present |
| `synthetic-error-handling-approved` | same set as `[SEH]` | ✓ present |
| `loaded_at` | speckit-implement SKILL + speckit.implement.md | ✓ present |
| `work_started_at` | speckit-implement SKILL + speckit.implement.md | ✓ present |
| `readiness/skill-loading-evidence.md` | speckit-implement SKILL + speckit.implement.md | ✓ present |
| `FS.Skia.UI.Controls` | controls/elmish fragment READMEs, controls fragment SKILL, base README, base product.md, `src/Controls/skill/SKILL.md` | ✓ present (template/** + src out of edit scope) |
| `Control<'msg>` | controls/elmish fragment READMEs, controls fragment SKILL, `src/Controls/skill/SKILL.md` | ✓ present |
| `DataGrid` | controls fragments, base README, base product.md, `src/Controls/skill/SKILL.md`, spec-template ×2 | ✓ present |
| `FS.Skia.UI.Controls.Elmish` | controls fragment SKILL, elmish README, base README, base product.md, `src/Controls/skill/SKILL.md` | ✓ present |
| `ControlsElmish.program` | `template/fragments/elmish/README.md` | ✓ present (template/** out of edit scope) |

## C2 — Semantic obligations (concept anchors must stay matchable)

AnyOf → ≥1 anchor present per home file. AllOf → **all** anchors present per home
file (the fragile rules — an AllOf phrase may not be deleted).

| Id | Mode | Concept anchors | Post-rewrite |
|----|------|-----------------|--------------|
| `skillist-structured` | AnyOf | structured skillist / structured \`skillist\` | ✓ |
| `skillist-minimal-ordered` | AnyOf | minimal ordered / declared order | ✓ |
| `skillist-confidence-fields` | AllOf | confidence · matched signals · reviewer disposition | ✓ |
| `skill-breadth` | AnyOf | small, medium, and broad | ✓ |
| `aggregate-non-authoritative` | AnyOf | non-authoritative aggregate | ✓ |
| `graph-before-after` | AnyOf | before and after every status change / graph before/after | ✓ |
| `persistent-launch` | AnyOf | persistent launch rules / persistent graphical launch task / MUST reject viewer-backed default executable paths | ✓ |
| `seh-discipline` | AnyOf | malformed parser input / convenience mocks / implementation-time relabeling | ✓ |
| `tasks-skill-gate` | AllOf | Compulsory skill evaluation · Visible skill mirror · Declared skill ids resolve | ✓ |
| `implement-skill-loading` | AllOf | Resolve every declared skill id · loaded paths · reviewer exception · implementation batch records · red-green evidence log | ✓ |
| `constitution-skill-gates` | AllOf | mandatory post-generation skill evaluation gate · mandatory pre-task skill loading gate · \`skillist\` field | ✓ |
| `tasks-post-gen-timing` | AnyOf | After task generation | ✓ |
| `deps-skillist-doc` | AnyOf | ordered list of applicable capability skill identifiers | ✓ |
| `controls-skia-rendered` | AnyOf | Skia-rendered | ✓ |
| `controls-no-charts-shim` | AllOf | legacy Charts package · no compatibility shim | ✓ |
| `fake-sequential` | AllOf | FAKE-backed · .fake · sequential · not safe to run concurrently | ✓ (every `serializedRunnerObligation` home file) |

The `fake-sequential` AllOf obligation is applied per-path by
`validateSerializedRunnerGuidance` to: README.md, docs/reports/{build,testing,
evidence}.md, AGENTS.md, CLAUDE.md, the speckit-implement / speckit-evidence-graph
/ speckit-evidence-audit SKILL.md `.agents` + `.claude` copies, tasks-template ×2,
plan-template ×2, and the template/base FAKE-guidance files. Each file that
mentions a FAKE-backed command must keep all four facets plus a numbered
sequential order.

## C3 — Forbidden terms (must stay absent over combined governed content)

`FS.Skia.UI.Charts`, `fs-skia-charts`, `chart-only`, `DataGrid as chart`,
`DataGrid-as-chart`, `renderer-neutral`, `renderer neutral`,
`host-loop ownership`, `host loop ownership`, plus the reflection-first /
repository-source-copy advice phrases (`reflection-first`, `repository-source-copy`).
No rephrasing reintroduces any. Post-rewrite: **none present.**

## Survival confirmation

`./fake.sh build -t GeneratedGuidanceCheck` is **green** over the rewritten
corpus (transcript in [generated-guidance.md](./generated-guidance.md)),
confirming C1 ✓ (all tokens present per file), C2 ✓ (all obligations resolve per
file/mode), and C3 ✓ (no forbidden term). The negative proof that drift still
fails is in [rewrite-red-green.md](./rewrite-red-green.md).
