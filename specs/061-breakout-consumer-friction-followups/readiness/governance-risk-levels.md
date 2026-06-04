# Governance Risk Levels — 061-breakout-consumer-friction-followups

Per the tasks.md risk taxonomy, this feature classifies validation effort by the blast
radius of each change. (Authoritative command, artifact path, failure class, and next
action are named per level.)

- **Small** — a single skill-text or readiness-doc edit (the FR-010 keyboard duplicate-DU
  pitfall extension, the FR-011 arcade-helper convention docs in `fs-skia-elmish` /
  `fs-skia-layout-readability`). Focused validation is the one owning currency gate:
  `./fake.sh build -t SkillSyncCheck` / `./fake.sh build -t SkillQualityCheck`.
  Artifact: `readiness/skill-quality-check.md` / `readiness/skill-sync-check.md`.
  Failure class: governance. Next action: fix the skill, regenerate `.claude` via
  `RefreshSurfaceBaselines`, rerun the gate.

- **Medium** — a coupled skill+contract+stale-ref change (FR-003 fourth feedback prompt)
  or an authoring-template change (FR-006/008/009). Focused validation is
  `GeneratedGuidanceCheck` + `TemplateCheck` / `TemplateDrift` + `GeneratedProductCheck`.
  Artifacts: `readiness/generated-guidance.md`, `readiness/template/**`,
  `readiness/target-metadata-drift.md`. Failure class: governance / template. Next action:
  fix the template/skill, regenerate via `RefreshSurfaceBaselines`, rerun the gate.

- **Broad** — the `build/Governance/**` output change (FR-004 self-describing
  readiness-contract diagnostic, FR-005 single `product-defect` spelling, FR-007
  `EvidenceGraph` verdict line). **Broad validation** (`EvidenceGraph` + `EvidenceAudit`
  + Governance unit tests) is the **required evidence** before merge because the audit/graph
  terminal output is consumer-facing. Run the FAKE-backed targets **sequentially** (shared
  `.fake` state). Aggregate/headless results are a **non-authoritative aggregate** — see
  `aggregate-hang-diagnostics.md`; the authoritative merge verdict is `EvidenceAudit`
  `verdict=PASS`. Next action: address the first failing gate, then re-run the set.
