# Contract — Relocated `AgentValidation` public surface

`AgentValidation` is **build/governance tooling**, never shipped to generated products. Its
only "external interface" is the curated `.fsi` that the governance library exposes and that
the governance test suite consumes. This document pins the **invariant**: the logical surface
is unchanged by the relocation — only its home namespace moves.

## Boundary

- **Before**: `namespace FS.Skia.UI.AgentValidation`, compiled by `src/Lib/Lib.fsproj`,
  published in the `FS.Skia.UI` monolith package.
- **After**: `namespace FS.Skia.UI.Build.AgentValidation`, compiled by
  `build/Governance/FS.Skia.UI.Build.fsproj` (after the `Spike` pair), published in the
  `FS.Skia.UI.Build` build-tooling package.

## Surface invariant (curated `.fsi`)

Every type and `val` below MUST remain present with identical shape after the move (the only
permitted edits are the `namespace` line and the doc-comment phrase
`"…exposed by this FS.Skia.UI package."` → `"…exposed by the FS.Skia.UI.Build governance
library."`):

- **Identifiers**: `ValidationGate`, `ValidationRuleId`, `FeatureId`, `ChangedPath`.
- **Change-source**: `ChangedPathSourceKind`, `ChangedPathSource`.
- **Authority/contract**: `ValidationAuthority`, `ValidationContractDefaults`,
  `ValidationContractTier`, `ValidationContractRule`, `ValidationContract`,
  `ValidationContractDiagnostic`, `ValidationContractParseResult`.
- **Failure taxonomy**: `ValidationFailureOwner`, `ValidationFailureClass`, `TimeoutClass`,
  `ValidationCost`, `TargetMetadata`.
- **Verdict**: `AgentVerdictStatus`, `AgentVerdict`, `ValidationGateOutcome`,
  `ValidationGateResult`.
- **MVU**: `ValidationSelectionModel`, `ValidationSelectionMsg`,
  `ValidationSelectionEffect`, `ValidationSelectionInterpreterInputs`.
- **Modules**:
  - `AgentVerdict.{aggregate, toJson, toMarkdown}`
  - `ValidationContract.{parse, knownGates}`
  - `ValidationSelection.{init, update, selectRules}`
  - `ValidationSelectionInterpreter.{readActiveFeatureMetadata, runGitMergeBaseDiff,
    loadValidationContract, writeSelectionReport, interpret}`

## Non-collision contract (FR-011)

`build/Governance/Front/Support.fs` (module `FS.Skia.UI.Build.Front.Support`) keeps its own
minimal `ValidationSelectionModel` / `ValidationSelectionMsg` / `ValidationSelectionEffect` /
`AgentVerdict`. Because the relocated module lives in the distinct namespace
`FS.Skia.UI.Build.AgentValidation`, the fully-qualified names differ and neither shadows the
other. `Support.fs` is **not modified** by this feature.

## Surface-baseline contract (FR-010 / SC-006)

- `readiness/surface-baselines/FS.Skia.UI.txt` (monolith aggregate, validated by
  `PackageSurfaceCheck`) loses its 48 `FS.Skia.UI.AgentValidation.*` lines — the **only**
  public-surface change in the repo.
- No `readiness/surface-baselines/FS.Skia.UI.Build.txt` is created: the build-tooling library
  is excluded from surface tooling (`readiness/per-package-surface-expectations.md`).
- The eight runtime per-package baselines are byte-unchanged
  (`PerPackageSurfaceDiff`/`PackageSurfaceCheck` green).

## Consumer contract (FR-009)

`validation.contract.yml` is **not** edited (currency vs `Routing.fs` preserved). The
template and every generated profile build/restore/run exactly as before; the default `app`
is byte-unchanged. The only package delta is the `FS.Skia.UI` package no longer carrying the
module and `FS.Skia.UI.Build` now carrying it.
