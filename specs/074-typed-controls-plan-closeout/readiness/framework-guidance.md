# Framework Guidance — Typed-Controls Plan Closeout (074)

Run `./fake.sh build -t Route` first and run **only** the gates it prints, sequentially
(`.fake` state is shared and not concurrency-safe). For this branch `Route` escalates to
`tier=agent-ready` (because `.specify/feature.json` and `AGENTS.md` are in the diff) and prints:
`Dev, TemplateCheck, GeneratedProductCheck, GeneratedGuidanceCheck, SkillSyncCheck,
SkillQualityCheck, SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift,
EvidenceGraph, EvidenceAudit`.

## Skill single-source pipeline (the one mechanical contract)

- Edit the canonical `.agents/skills/<name>/SKILL.md`. **Never** hand-edit the `.claude` peer.
- Regenerate with `./fake.sh build -t RefreshSurfaceBaselines` (regenerates the `.claude`
  peers and the `skillist-reference.md` index).
- `SkillRegistry` discovers skills by frontmatter `name:` — no hardcoded list to edit when
  adding `fs-skia-reconciliation`.
- `SkillSyncCheck` fails on any `.agents` ↔ `.claude` drift; `SkillQualityCheck` enforces the
  rubric sections; `SkillContractPathCheck` requires referenced contract paths to resolve.

## No rule / surface change

- No `Routing.fs` rule, `validation.contract.yml`, public `.fsi`, package, or surface-baseline
  change. `Reconcile` stays `module internal` and is not wired into the render path (FR-010).
- Catalog generation is **documented** (C13 in `fsharp-code-generation`), not changed — no
  catalog *content* row is added.

## Governance home

All rules live in `FS.Skia.UI.Build` (`build/Governance/**`); governance artifacts are generated
from a single source, not hand-synced. This feature exercises only the skill-tree generation half
of that pipeline (`.claude` ← `.agents`) plus a hand-edited historical report (no gate parses the
report — correctness is the US2 independent cross-check against `git log`).
