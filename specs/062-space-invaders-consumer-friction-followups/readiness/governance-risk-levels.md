# Governance Risk Levels — 062-space-invaders-consumer-friction-followups

Per the tasks.md risk taxonomy, this feature classifies validation effort by the blast
radius of each change. (Authoritative command, artifact path, failure class, and next
action are named per level.)

- **Small** — a single content/skill/doc change-set (the FR-009 `Result.Ok`/`Result.Error`
  shadowing pitfall note in `fs-skia-skiaviewer`, the FR-003 `scaffold-map.md` page, the
  FR-010/011 skill references in `fs-skia-elmish` / `fs-skia-layout-readability`). Focused
  validation is the one owning currency gate Route prints for that diff:
  `./fake.sh build -t SkillSyncCheck` / `./fake.sh build -t SkillQualityCheck`.
  Artifact: `readiness/skill-quality-check.md` / `readiness/skill-sync-check.md`.
  Failure class: governance. Next action: fix the skill, regenerate `.claude` via
  `RefreshSurfaceBaselines`, rerun the gate.

- **Medium** — the self-describing-diagnostics and symbol-diff change-sets in
  `build/Governance/**` (FR-004 `Dev` self-describing output, FR-005 per-class
  schema-print diagnostics, FR-007 effective-DAG render, FR-008 symbol set-difference).
  Focused validation is `Dev` + the Evidence gates (`EvidenceGraph` + `EvidenceAudit`) +
  Governance unit tests; broad validation when the schema constants or render output
  change. Artifacts: `readiness/evidence-graph.md`, `readiness/evidence-audit.md`,
  `readiness/readiness-recoverability.md`. Failure class: governance. Next action: fix the
  governance source, rerun the gate.

- **Broad** — the FR-010 Tier-1 helper change-set (new `FS.Skia.UI.SkillSupport` `.fsi`
  surface + a new per-package surface baseline). **Broad validation** is the
  **required evidence** before merge: the full serialized six-target order plus
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff`. Run the FAKE-backed targets
  **sequentially** (shared `.fake` state). Aggregate/headless results from any broad run are
  recorded as a **non-authoritative aggregate** — see `aggregate-hang-diagnostics.md`; the
  authoritative merge verdict is the per-target gate (`EvidenceAudit` `verdict=PASS`), not
  the aggregate. Next action: address the first failing gate, then re-run the set.
