# Governance Risk Levels — 059-speckit-tasks-validation-feedback

This feature classifies validation effort by the blast radius of each change.

- **Small** — a single skill-content edit, a single readiness note, or one prose
  guidance line. Focused validation is the single named gate over the touched
  artifact (e.g. `SkillQualityCheck` for a skill edit).
- **Medium** — gate/library-internal changes in the compiled evidence engine
  (`build/Governance/Evidence/**`, `build/Governance/Guidance.fs`). Focused
  validation is `./fake.sh build -t Dev` plus the affected gate.
- **Broad** — consumer-contract changes (`template/**`, `.specify/**`, public
  `.fsi`, `.agents/skills/**`, governance paths). Broad validation is the
  serialized maintainer-verify pipeline.

Broad validation is required only if `./fake.sh build -t Route` escalates the
change (it does here — `template/base/build.fsx`, `.specify/**`,
`.agents/skills/**`, and `build/Governance/**/*.fsi` all escalate). The broad
gate run below is the **required evidence** for the escalated tasks (T034–T037).
The required broad gates are, run sequentially:

- `./fake.sh build -t Dev`
- `./fake.sh build -t GeneratedGuidanceCheck`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedProductCheck`
- `./fake.sh build -t EvidenceGraph`
- `./fake.sh build -t EvidenceAudit`

Aggregate FAKE results are **non-authoritative**: the per-target verdict is the
authority. See `aggregate-hang-diagnostics.md`.
