# Governance risk levels (feature 077)

Records the tier/layer/public-API/MVU/evidence facts for this change (T003) and the
focused-vs-broad validation map (tasks.md "Governance risk levels").

- **Tier**: Tier 1 (contracted) — adds a routed governance gate `PhaseHookParityCheck` to
  the public validation contract (`validation.contract.yml`) and to
  `AgentValidation.knownGates`, and changes consumer-facing Spec Kit phase-skill text.
- **Affected layer**: governance `build/Governance/**` + canonical `.agents/skills/**`
  (mirrored to `.claude/skills/**`). No product-library `.fsi` surface change.
- **Public-API impact**: none on product libraries. New build-tooling module
  `FS.Skia.UI.Build.PhaseHookParity` ships its curated `.fsi` (Principle II).
- **Elmish/MVU applicability**: reuse of the existing governance Engine boundary — a new
  `StartTarget PhaseHookParityCheck` `Msg` emits a `PhaseHookScan` effect; the check itself
  is the pure `PhaseHookParity.checkCorpus`; the interpreter edge
  (`Front/Governance.fs:runPhaseHookParityCheck`) reads the roster SKILL.md files, writes the
  report, and `failwith`s on findings. No new MVU surface (Principle IV).
- **Evidence obligations**: guard PASS on the repaired tree
  (`phase-hook-parity-check.md`) + red→green guard test
  (`tests/Governance.Tests/PhaseHookParityTests.fs`) + `.agents`↔`.claude` sync
  (`skill-sync.md`) + generated-output propagation (`template-check.md`).

## Focused-vs-broad map

- **Small** (skill-text edits): `PhaseHookParityCheck` + `SkillSyncCheck`.
- **Medium** (new rule/gate + Engine wiring): `Dev` + `PhaseHookParityCheck` +
  `TargetMetadataDrift`.
- **Broad** (consumer propagation + contract currency): the full escalated serial order —
  `Dev` → `PhaseHookParityCheck` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`. This is the **broad validation**
  path required once both the skills and the gate are in place.

### Required evidence per tier

Each tier names its **required evidence** artifact(s):

- **Small** → `phase-hook-parity-check.md` + `skill-sync.md`.
- **Medium** → the `Dev` test log + `phase-hook-parity-check.md` + the regenerated
  `validation.contract.yml` (TargetMetadataDrift currency).
- **Broad** → `template-check.md` + `generated-guidance-validation.md` +
  `generated-product-check.md` + `evidence-graph.md` + `evidence-audit.md`.
