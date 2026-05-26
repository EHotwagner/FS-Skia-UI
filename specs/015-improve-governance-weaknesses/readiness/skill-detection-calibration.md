# Skill-Detection Calibration

Status: scaffolded. Calibration cases will be filled by US2 implementation.

Required case coverage:

| Case | Expected confidence | Reviewer disposition |
|------|---------------------|----------------------|
| Obvious evidence graph validation task | high | accepted |
| Ambiguous governance wording | medium | reviewer chooses owner |
| Indirect ownership through generated guidance | medium | accepted with rationale |
| False positive wording match | low | rejected with rationale |
| Valid empty skill list | none | accepted as empty |

## Calibration Results

| Case | Candidate skill | Matched signals | Confidence | Ambiguity | Reviewer disposition | Diagnostic |
|------|-----------------|-----------------|------------|-----------|----------------------|------------|
| Obvious match accepted | speckit-evidence-graph | task-text, command-name | high | none | accepted | EvidenceGraph validation task names graph/parser behavior |
| Ambiguous match | speckit-evidence-audit, speckit-evidence-graph | task-text | medium | graph/audit wording overlap | reviewer chooses owner | Diagnostics mention both graph and audit signals |
| Indirect capability ownership | speckit-tasks | generated guidance, template path | medium | indirect | accepted with rationale | Template guidance is task-generation owned |
| False positive | speckit-evidence-graph | isolated word graph | low | none | rejected with rationale | Graph wording appears in unrelated prose |
| Valid empty skill list | none | none | none | none | accepted as empty | Phase 6 FAKE aggregate orchestration has no current local owner |

Runtime target: under 30 seconds for graph-only calibration on this feature.

Phase 6 currently keeps `[skillist: []]` because no local capability skill owns
FAKE aggregate timeout verdict reporting. This empty selection must be revisited
if a build-orchestration governance skill is added.

## Task-Generation Assumptions

- Every task has structured `deps` and `skillist` metadata in
  `tasks.deps.yml`.
- Every task line mirrors the structured `skillist` value using
  `[skillist: ...]`.
- Non-empty `skillist` entries are minimal and ordered by implementation need.
- Phase 6 aggregate timeout tasks intentionally use `[skillist: []]` because
  current local skills own Spec Kit evidence, task generation, implementation
  guidance, and constitution workflows, but not FAKE aggregate orchestration.
- Empty `skillist` choices are reviewable calibration cases rather than proof
  that no future skill could apply.

## Initial Skillist Rationale

| Skill id | Applied where | Rationale |
|----------|---------------|-----------|
| speckit-evidence-graph | Graph/parser, task metadata, skill-match assessment | Owns DAG validation and task metadata diagnostics |
| speckit-evidence-audit | Merge-readiness, synthetic propagation, diff scan, risk evidence | Owns readiness-blocking audit behavior |
| speckit-tasks | Task templates and generated task guidance | Owns generated task metadata and skill selection instructions |
| speckit-implement | Implementation guidance and pre-task skill loading | Owns implement-time skill loading and status discipline |
