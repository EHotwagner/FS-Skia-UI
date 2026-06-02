# Parity sign-off

command: `dotnet test tests/Parity.Tests/Parity.Tests.fsproj`
artifact path: this file + `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`.
failure class: SceneOutputParity.
next action: none — the oracle re-derives byte-identically.

## Sign-off

- The deterministic scene-output oracle (`SceneOutput.fs`/`SceneOutputTests.fs`, format
  `scene-output/v1`) re-derives **byte-identically** to the Stage-0 golden for the three seeds
  (`basic-viewer`/`effects-gallery`/`screenshot-gallery`): `dotnet test tests/Parity.Tests` → 4 tests,
  4 passed, 0 failed. This is the merge-gate sign-off (FR-005, SC-004).
- `Parity.Tests` is now **Scene-only**: the old-vs-new `Parity`-helper report bridge (`Tests.fs`) was
  removed and the `Lib.fsproj` reference dropped. The oracle stays in place (no migration to
  `Scene.Tests` — that would churn the hardcoded fixture path and the governance scanning lists that
  reference `tests/Parity.Tests`).
- Reference-frame screenshot re-capture remains headless-GPU-infeasible (disclosed corroboration; no
  software-renderer fallback in CI), so scene-output is the authoritative oracle.
