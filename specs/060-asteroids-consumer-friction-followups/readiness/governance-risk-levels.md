# Governance Risk Levels — 060-asteroids-consumer-friction-followups

Per the tasks.md risk taxonomy, this feature classifies validation effort by the blast
radius of each change. (Authoritative command, artifact path, failure class, and next
action are named per level.)

- **Small** — a single skill-text or readiness-doc edit (e.g. the scene/keyboard pitfall
  notes, the layout-readability HUD pattern). Focused validation is the one owning currency
  gate: `./fake.sh build -t SkillSyncCheck` / `./fake.sh build -t SkillQualityCheck`.
  Artifact: `readiness/skill-quality-check.md` / `readiness/skill-sync-check.md`.
  Failure class: governance. Next action: fix the skill, regenerate `.claude` via
  `RefreshSurfaceBaselines`, rerun the gate.

- **Medium** — a new governance check or a generated-output change (the api-surface
  generator, `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`, the split tests).
  Focused validation is that gate plus `GeneratedProductCheck` / `TemplateCheck`.
  Artifacts: `readiness/skill-contract-path-check.md`,
  `readiness/template-update-package-check.md`, `readiness/target-metadata-drift.md`.
  Failure class: governance / template. Next action: fix the rule/generator, regenerate
  via `RefreshSurfaceBaselines`, rerun the gate.

- **Broad** — the routed `maintainer-verify` set (D10): `Dev`, `GeneratedGuidanceCheck`,
  `TemplateCheck`, `GeneratedProductCheck`, the new api-surface/skill-path/template-update
  gates, `SkillSyncCheck`, `TargetMetadataDrift`, `SkillQualityCheck`, `EvidenceGraph`,
  `EvidenceAudit`. **Broad validation** is the **required evidence** before merge; run the
  FAKE-backed targets **sequentially** (shared `.fake` state). Aggregate/headless results
  are **non-authoritative** — see `aggregate-hang-diagnostics.md`; the authoritative merge
  verdict is `EvidenceAudit` `verdict=PASS`. Next action: address the first failing gate,
  then re-run the set.
