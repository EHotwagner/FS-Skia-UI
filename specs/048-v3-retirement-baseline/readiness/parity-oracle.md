# Parity oracle re-derivation (T010, SC-003)

The deterministic scene-output golden re-derives **byte-identically** from the current
host for all three seeds. Scene-output is the **authoritative** parity signal;
screenshots corroborate.

## Re-derivation (assert mode — no capture env)
```
$ dotnet test tests/Parity.Tests/Parity.Tests.fsproj --filter "FullyQualifiedName~scene-output"
Passed!  - Failed: 0, Passed: 4, Skipped: 0, Total: 4
```
Four tests pass: `basic-viewer`, `effects-gallery`, `screenshot-gallery` each re-derive
byte-identically (0-byte diff, SC-003), plus a determinism-across-runs check.

## Goldens (authoritative)
```
$ ls tests/Parity.Tests/fixtures/v3-host-golden/scene-output/
basic-viewer.txt  effects-gallery.txt  screenshot-gallery.txt
```
Each golden records, from the current host's `Scene` value: `format: scene-output/v1`,
the seed id, the output size, the ordered element kinds (`Scene.describe`), the
diagnostics (`Scene.diagnostics`), and the readback evidence including the
environment-independent `DeterministicHash` (`Scene.renderReadbackEvidence`). Example
(`basic-viewer`): 12 elements, 1 frame-render diagnostic (invalid image resource), 9
capabilities, deterministic-hash `802f1f44…`.

## Corroboration screenshot (real Vulkan capture)
```
$ file tests/Parity.Tests/fixtures/v3-host-golden/screenshots/basic-viewer.png
PNG image data, 640 x 480, 8-bit/color RGBA, non-interlaced
```
Captured via `dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --screenshot-smoke
--output=…` on the GPU-passthrough host. `effects-gallery`/`screenshot-gallery` reference
frames are deferred at the pin (no non-interactive capture entry point); the authoritative
scene-output covers all three. See `capture-environment.md`.

## Authoritative vs corroboration
A reference-screenshot mismatch without a scene-output drift is an environment
(`UnsupportedEnvironment`) difference, not a regression. Only a scene-output drift is a
`ProductDefect`. This is the host-move merge gate for programme Stages 1 and 4 (ADR 0011).
