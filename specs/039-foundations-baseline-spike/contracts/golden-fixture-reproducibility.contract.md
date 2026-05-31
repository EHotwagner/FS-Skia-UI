# Contract: Golden-Fixture Reproducibility (Stage 4 parity oracle)

This contract makes the archived golden fixtures a valid parity oracle: re-running the existing evidence engine on the same features at the same commit regenerates the committed fixtures **byte-for-byte**. (FR-002, FR-003; SC-002.)

## Fixtures under contract

For each selected feature `F` ∈ { `038-authoring-guidance-consistency`, `037-authoring-audit-robustness`, `017-synthetic-error-evidence` } (or a recorded substitute), the committed set at `tests/Governance.Tests/fixtures/evidence-golden/F/`:

- `task-graph.json` — graph data emitted by the existing `EvidenceGraph` path
- `task-graph.md` — graph Markdown emitted by the existing `EvidenceGraph` path
- `audit-counts.txt` — the audit status/count block (`accepted-seh-tasks`, `unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks`)

## Capture method (existing engine, unchanged — FR-011)

Run the existing FAKE evidence targets (which shell to `.specify/extensions/evidence/scripts/bash/run-audit.sh`) against feature `F`, then archive the regenerated `task-graph.json` / `task-graph.md` and the audit count block. No edit to the Python/Bash evidence path is permitted.

## Expected behaviour

| # | Given | When | Then | Maps to |
|---|---|---|---|---|
| 1 | the committed fixtures for `F` at the baseline SHA | the same evidence commands are re-run on `F` | regenerated outputs are **byte-for-byte identical** to the committed fixtures | AS-3 (US2), SC-002 |
| 2 | a re-run that differs | the difference is examined | the non-determinism is removed (deterministic re-capture) **or** `F` is substituted and the substitution recorded in the baseline | Edge Case |
| 3 | the fixture set | it is committed | the baseline doc records the exact SHA and the exact commands used to capture/reproduce | AS-4 (US2), SC-001 |

## Verification command (reviewer)

For each feature `F`, regenerate into a scratch location and `diff` against the committed fixture; an empty diff for all three files across all three features is the pass condition (100% byte-for-byte, SC-002). Exact commands are in `quickstart.md`.

## Designation

This fixture set is the **Stage 4 parity oracle**: when the Python evidence engine is later ported to F#, its output must match these fixtures byte-for-byte before the Python is deleted.
