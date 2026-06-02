# Phase 1 Data Model — Relocated `AgentValidation`

This feature **moves** an existing capability; it introduces **no new** types and changes
no field, case, or signature. The model below is the inventory of what relocates, recorded
so the parity check (FR-004/SC-003) and the surface-baseline edit (D4) are exhaustive. All
types move from `FS.Skia.UI.AgentValidation` → `FS.Skia.UI.Build.AgentValidation` unchanged.

## Value types (contract + verdict vocabulary)

| Type | Kind | Notes |
|---|---|---|
| `ValidationGate`, `ValidationRuleId`, `FeatureId`, `ChangedPath` | type aliases (`string`) | identifiers |
| `ChangedPathSourceKind` | DU | `ActiveFeatureMetadata` \| `GitMergeBaseDiff` \| `Unavailable` |
| `ChangedPathSource` | record | `Kind`, `Feature`, `MergeBase`, `Paths`, `Diagnostics` |
| `ValidationAuthority` | DU | `InnerLoop`/`FocusedAuthority`/`AgentReadyAuthority`/`MaintainerVerify`/`AutomationFinal` |
| `ValidationContractDefaults` | record | parsed contract defaults |
| `ValidationContractTier` | record | tier metadata |
| `ValidationContractRule` | record | one routing rule |
| `ValidationContract` | record | the parsed `validation.contract.yml` |
| `ValidationContractDiagnostic` | record | one accept/reject diagnostic |
| `ValidationContractParseResult` | DU | `ValidationContractAccepted` \| `ValidationContractRejected` |
| `ValidationFailureOwner`, `ValidationFailureClass`, `TimeoutClass`, `ValidationCost` | DUs | gate-failure taxonomy |
| `TargetMetadata` | record | per-target metadata |
| `AgentVerdictStatus` | DU | verdict status |
| `AgentVerdict` | record | the emitted agent verdict |
| `ValidationGateOutcome` | DU | `GateFailed`/`GateMissingEvidence`/`GateStalePrerequisite`/`GateUnsupportedHost` |
| `ValidationGateResult` | record | per-gate result |

## MVU boundary (Principle IV — relocated intact, behaviour preserved)

The capability carries an existing Elmish/MVU boundary. It moves whole; `update` stays pure
and I/O stays at the interpreter edge.

| Element | Symbol | Notes |
|---|---|---|
| `Model` | `ValidationSelectionModel` | `Feature`, `ChangedPathSource`, `SelectedRuleIds`, `RequiredGates`, `Authority`, `Degraded`, `Diagnostics` |
| `Msg` | `ValidationSelectionMsg` | `ContractLoaded`, `ActiveFeatureMetadataLoaded/Unavailable`, `GitMergeBaseDiffLoaded/Unavailable`, `SelectionFailed` |
| `Effect` | `ValidationSelectionEffect` | I/O requested as data |
| interpreter inputs | `ValidationSelectionInterpreterInputs` | the edge's dependencies |
| `init` | `ValidationSelection.init` | `feature -> Model * Effect list` |
| `update` | `ValidationSelection.update` | pure `Msg -> Model -> Model * Effect list` |
| pure helper | `ValidationSelection.selectRules` | `changedPaths -> contract -> ruleIds * gates * authority` |
| interpreter | `ValidationSelectionInterpreter.{readActiveFeatureMetadata, runGitMergeBaseDiff, loadValidationContract, writeSelectionReport, interpret}` | executes file reads + `git` at the edge — **unchanged** |

## Modules (public functions)

| Module | Functions |
|---|---|
| `AgentVerdict` | `aggregate`, `toJson`, `toMarkdown` |
| `ValidationContract` | `parse`, **`knownGates`** (the relocated gate allowlist — FR-008/SC-005) |
| `ValidationSelection` | `init`, `update`, `selectRules` |
| `ValidationSelectionInterpreter` | `readActiveFeatureMetadata`, `runGitMergeBaseDiff`, `loadValidationContract`, `writeSelectionReport`, `interpret` |

## Anchored invariants

- **`knownGates`** is the canonical valid-gate allowlist; after the move it is governance
  config in `FS.Skia.UI.Build`, extensible without touching `src/**` (FR-008/SC-005).
- **Behaviour parity**: every type/field/case/signature above is byte-identical pre/post
  move (D5); the only public-surface delta in the whole repo is the monolith baseline
  shedding the 48 `FS.Skia.UI.AgentValidation.*` lines (D4).
