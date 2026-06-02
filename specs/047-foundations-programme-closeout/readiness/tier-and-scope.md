# Tier & scope (T001)

- **Tier**: **Tier 2** throughout — documentation / measurement / verification-record
  only. No product `.fsi`, surface-baseline, or `PackageVersion` change (SC-006). No
  story is Tier 1.
- **Routing**: `Route` **escalates** this feature because it touches `CLAUDE.md` /
  `AGENTS.md` / governance docs and the recurring-run schedule file. Captured live:
  `tier=agent-ready`, `gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph,
  EvidenceAudit`, `dogfood-forced=false`, `matched-rules=evidence-governance,
  specify-catchall, docs-only`. As the programme-closing feature it is additionally run
  as a **dogfood** candidate through the full serialized six-target set.
- **Affected surfaces**: `docs/reports/_baselines/2026-06-02-foundations-after.md`,
  `docs/adr/0006-foundations-programme-closeout.md`, `README.md`,
  `docs/reports/build.md`, `docs/reports/speckit.md`, `CLAUDE.md`, `AGENTS.md`,
  `.specify/schedules/foundations-dogfood-pipeline.yml`,
  `specs/047-foundations-programme-closeout/readiness/**`.
- **Public-API impact**: **none** — no product `.fsi`, surface-baseline, or
  `PackageVersion` change (SC-006).
- **Elmish/MVU applicability**: **N/A** — no stateful or I/O-bearing workflow; the
  measurement artifacts only *read* `git`/build outputs and add no `Model`/`Msg`/`Effect`.
- **Real-evidence obligations**: committed grep proofs (`scaffolding-proof.md`), the
  after-baseline with per-row reproduction commands + `after-baseline-repro.md`, the
  closing ADR, the dogfood retrospective + recurring-run mechanism,
  `runtime-untouched.md`, and the serialized escalated FAKE gate logs. **Zero synthetic.**
