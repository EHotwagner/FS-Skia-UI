# Unit-Test Evidence — update + relocated validators (T020 / T021 / T022 / FR-013 / SC-005)

All tests run in `tests/Governance.Tests` against the compiled, relocated library.

## Pure `update` effect-list tests (T020) — `BuildEngineUpdateTests.fs`
`update : BuildMsg -> BuildModel -> BuildModel * BuildEffect list` is now directly unit-tested for
the first time (it was un-testable inside the 4,767-line FSX). Assertions are on **typed
`BuildEffect` lists** with **no repo-tree I/O** (the model is derived once via `init`; `update`
itself touches no filesystem/git/process — the `Engine/Update.fsi` surface compiler-enforces this,
Principle IV):
- `StartTarget Route` ⇒ `[ RouteSelect ]`; model unchanged.
- `StartTarget BuildWorkflowCheck` ⇒ `[ WorkflowSelfCheck ]`.
- `TargetCompleted "Restore"` ⇒ `[]`, `CompletedTargets = ["Restore"]`.
- `StartTarget Clean` ⇒ seven `CleanDirectoryContents` effects.
- `StartTarget CapabilityCheck` ⇒ `[ CapabilityCatalogCheck; RequireFiles(...) ]`.
- `StartTarget Dev` ⇒ contains the dev-verdict `WriteFile` + aggregate-hang `WriteStructuredReport`.
- determinism: same (msg, model) ⇒ same result.

## Relocated-validator tests (T021)
- `GuidanceValidatorTests.fs` — `Guidance.runGeneratedGuidanceScan` over the **real** repository
  guidance writes a `# Generated Guidance Check` PASS report.
- `PreflightValidatorTests.fs` — `Preflight.collectProcessHealth` over the **real** host writes a
  `# Process Health Evidence` report.
- `GeneratedProductValidatorTests.fs` — `GeneratedProduct.runDependencyOwnershipReport` over the
  **real** `src/*` project files writes a non-empty Markdown report.

## Result
- New suites: **10 passed, 0 failed** (`dotnet test --filter` on the four suites).
- Full `Governance.Tests`: **304 passed, 0 failed** (300 pre-existing + 4 new groups' tests),
  including the migrated command-contract suite that now asserts the new launcher/tool surface.

## Failing-first (RED→GREEN) disclosure
The skill's stash-control RED capture (reverting each relocated body to a skeleton to record a RED
run, then GREEN) was **not** separately performed: the bodies were relocated verbatim and were green
on first authoring of the tests. The GREEN evidence above is real; the discrete RED snapshot step is
the one process element not independently captured (honest disclosure, no synthetic evidence).

Captured: 2026-06-01T14:44:26Z
