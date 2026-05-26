# Research: Synthetic Error Evidence

## Decision: Represent approved malformed-input/error-path work with `[SEH]` plus `synthetic-error-handling-approved`

**Rationale**: A visible task annotation is easy to scan in `tasks.md`, while the label gives scripts and inventory checks a stable machine-readable value. Requiring both prevents accidental acceptance from a single ambiguous marker.

**Alternatives considered**:
- Reuse `[S]` only: rejected because it cannot distinguish approved error-handling synthetic evidence from ordinary synthetic evidence.
- Add a new completion status: rejected because the user explicitly wants accepted synthetic status, and `[S]` should remain the honest evidence classification.
- Use only a free-text rationale: rejected because it is hard to validate consistently.

## Decision: Preserve `[S]` task status for `[SEH]` tasks

**Rationale**: The evidence is still synthetic. Keeping `[S]` preserves Principle V visibility, PR disclosure, code/test banners, and reviewer expectations while allowing a narrow audit PASS.

**Alternatives considered**:
- Convert approved tasks to `[X]`: rejected because it hides synthetic evidence.
- Add `[X-SEH]`: rejected because it confuses completion status with evidence class and would require more propagation rules.

## Decision: `EvidenceAudit` returns PASS when every synthetic task is valid design-approved `[SEH]`

**Rationale**: The clarification on 2026-05-26 selected PASS semantics. The audit must still show accepted synthetic counts separately from real-evidence tasks so visibility is not lost.

**Alternatives considered**:
- Always fail with a waiver: rejected because it does not satisfy the selected clarification and keeps necessary malformed-input tests as false blockers.
- Warn-only verdict: rejected because existing workflows appear to reason about PASS/FAIL gates, and a third verdict would add ambiguity.

## Decision: Design-phase provenance is mandatory

**Rationale**: The central risk is implementation-time synthetic cleanup. Recording the design-phase source location gives the audit a concrete way to reject labels introduced during implementation or after a failed audit.

**Alternatives considered**:
- Trust task text: rejected because late edits can look identical without provenance.
- Require a separate approval file only: rejected because it would fragment the task and inventory review unless linked back to the task.

## Decision: `[SEH]` eligibility is limited to malformed input and explicit error paths

**Rationale**: The feature should not become a general shortcut for mocks, unavailable services, unsupported hosts, product gaps, or speed-only fixtures. Eligibility examples and non-eligible examples must be included in planning and task guidance.

**Alternatives considered**:
- Allow all negative tests: rejected because many negative tests can and should use real product behavior.
- Allow all synthetic tests with rationale: rejected because that duplicates the broader `--accept-synthetic` override and weakens Principle V.

## Decision: Update constitution and templates together

**Rationale**: The current constitution says unresolved `[S]` and `[S*]` always block merge readiness and that `--accept-synthetic` still reports failure. This feature intentionally changes that for valid `[SEH]`, so the governing policy and generated task guidance must move together.

**Alternatives considered**:
- Change only `EvidenceAudit`: rejected because generated tasks would not know how to classify eligible work before implementation.
- Change only guidance: rejected because audit behavior would still block approved tasks.
