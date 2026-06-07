# Governance risk levels — Typed-Controls Plan Closeout (074)

This feature's governance risk is **small / focused**: a skill-source change (one edit, one
new skill) plus a historical-report refresh, with **no** `Routing.fs` rule, package, public
`.fsi`, or runtime change. `Route` is authoritative. FAKE-backed targets share `.fake` state —
run them **sequentially**, never concurrently.

## Small

The substantive authoring work routes here: editing
`.agents/skills/fsharp-code-generation/SKILL.md` (US1 C13 worked example), creating
`.agents/skills/fs-skia-reconciliation/SKILL.md` (US3), and refreshing the
forward-looking/status sections of the implementation-plan report (US2). A focused
`./fake.sh build -t Dev` plus the skill gates is sufficient evidence for this band.

## Medium

The skill **currency** obligation: after editing the canonical `.agents` source, regenerate
the `.claude` peers and skill index via `./fake.sh build -t RefreshSurfaceBaselines`. The
**required evidence** for this band is `SkillSyncCheck` (zero drift, SC-002), `SkillQualityCheck`
(rubric sections present for the new skill), and `SkillContractPathCheck` (referenced contract
paths resolve) — all PASS.

## Broad

Close-out. Because `.specify/feature.json` and `AGENTS.md` are in the working-tree diff, `Route`
escalates to **tier=agent-ready** and prints `Dev, TemplateCheck, GeneratedProductCheck,
GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, SkillContractPathCheck,
TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph, EvidenceAudit`.
**broad validation** runs exactly those printed gates, sequentially. Aggregate results (e.g.
`GeneratedProductCheck`'s known local environment failure) are **non-authoritative** and recorded
under `readiness/` and `readiness/logs/`; the authoritative verdict is each focused gate's own
result.
