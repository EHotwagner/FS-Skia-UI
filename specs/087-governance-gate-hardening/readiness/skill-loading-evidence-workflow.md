# Skill-loading-evidence workflow — feature 087 (placeholder)

- **Authoritative command**: `./fake.sh build -t EvidenceAudit` (validates
  `skill-loading-evidence.md` rows) and `./fake.sh build -t EvidenceGraph`.
- **Artifact path**: `readiness/skill-loading-evidence.md` (one row per
  `(task, declared-skill)`), `readiness/skill-loading-evidence-provenance.md`
  (at-implementation gap report).
- **Failure class**: a declared-but-unloaded skill, a late load
  (`loaded_at >= work_started_at`), a non-ISO-8601 timestamp, or a missing 9th
  `provenance` column (FR-010). Gap is surfaced **at the declaring task's
  implementation point**, not deferred to the `[X]` flip.
- **Next action**: rows authored as skills are loaded (T027/T028/T029); the
  `provenance ∈ { captured, asserted }` column distinguishes observed loads from
  hand-authored timestamps.
