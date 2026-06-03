# Governance Risk Levels — 058-skills-quality-feedback

Per the tasks.md risk taxonomy, this feature classifies validation effort by the
blast radius of each change.

- **Small** — a single skill-content edit or a single readiness note. Focused
  validation is `SkillQualityCheck` (or the named gate) over the touched skill.
- **Medium** — gate/library-internal changes (`SkillQuality`, a `SkillSupport`
  family). Focused validation is `Dev` plus the affected gate.
- **Broad** — consumer-contract changes (`template/**`, a new `.fsi`, governance
  paths, package pins). Broad validation is the serialized six-target
  maintainer-verify pipeline, and is the **required evidence** for T031, T035,
  T036, T037, T038 (i.e. broad validation is required for those tasks).

Aggregate FAKE results are **non-authoritative**: the per-target verdict is the
authority. See `aggregate-hang-diagnostics.md` for the aggregate-run caveat.
