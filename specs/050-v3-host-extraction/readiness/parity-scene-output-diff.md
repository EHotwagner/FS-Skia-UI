# Parity — scene-output re-derivation (FR-008 / SC-002, the merge gate)

**Verdict:** PASS — 0-byte diff for all three seeds.

The moved-and-retyped host re-derives the committed Stage-0 deterministic scene-output golden
**byte-identically** for `basic-viewer`, `effects-gallery`, and `screenshot-gallery`. `Parity.Tests`
was repointed onto the `FS.Skia.UI.Scene` vocabulary (`tests/Parity.Tests/SceneOutput.fs` now
`open FS.Skia.UI.Scene`) and **retained** as the parity harness (FR-007).

- Authoritative command: `dotnet test tests/Parity.Tests --filter "FullyQualifiedName~scene-output"`
- Result: `Passed! - Failed: 0, Passed: 4, Total: 4` (3 seeds + the determinism guard).
- Golden: `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`.
- Oracle is pure deterministic scene-value serialization (`Scene.describe` / `Scene.diagnostics` /
  `Scene.renderReadbackEvidence`); it does not invoke the Vulkan host, so it is fully verifiable
  headlessly.

This gate was clean **before** any legacy `Lib` host source was deleted (ADR 0011 deletion order).
The `Scene.diagnostics` image `File.Exists` check was added to the canonical `FS.Skia.UI.Scene`
vocabulary so the `basic-viewer` golden (`diagnostics: 1`, "Invalid image resource declaration.")
re-derives exactly.

failure class: ScenePartityRegression (loud — names the seed + first divergent line).
next action on failure: re-run the focused filter above; scene-output is authoritative over screenshots.
