# Integration Validation (T036, SC-001 governing) — COMPLETE

Full sequential FAKE validation order, run with the headless X11 backend forced
(`env -u WAYLAND_DISPLAY GDK_BACKEND=x11 SDL_VIDEODRIVER=x11`) so the container's
Wayland `libdecor-gtk` plugin path is not taken — see
`skiaviewer-tests-headless-flake` memory and `aggregate-hang-diagnostics.md`.
FAKE-backed targets were run **sequentially**, never concurrently.

| Gate | Result | Notes |
|------|--------|-------|
| `Dev` / `Build` | PASS | framework solution builds |
| `GeneratedGuidanceCheck` | PASS | US1 id resolution + skill-section/command checks |
| `CapabilityCheck` / `SkillCheck` | PASS | `capabilities.yml` `skill:` → consumer product-skills resolve, required sections present |
| `Governance.Tests` (271/271) | PASS | incl. updated `TemplateProfileTests` asserting `template.json` sources `template/product-skills/*` |
| `TemplateCheck` | PASS | `dotnet new fs-skia-ui` instantiates with consumer product-skills |
| `GeneratedProductCheck` | PASS (1m41s) | regenerates all 5 rows (app source+package, governed, headless-scene, sample-pack); Dev/Test/Verify green in each; consumer-skill scan (FR-005/006) + demo-identifier scan (FR-007) + bundled api-surface (FR-004) + effects page (FR-009) all clean |
| `PackageSurfaceCheck` | PASS | US3 RQA + US6 additive-constructor baselines |
| `EvidenceGraph` (T037) | PASS | 38 tasks, no cycles/dangling/ambiguity, statuses echoed |
| `EvidenceAudit` (T038) | PASS | merge-gate PASS; only advisory synthetic-banner hits (inventory headers), no blocking signals |

## Generated consumer skills are consumer-facing (FR-005/006, SC-004)

The consumer-skill scan in `scanV3GeneratedRow` ran clean (0 findings) across
every generated product. Capability-usage skills per row:

- `app-source` / `app-package`: fs-skia-scene, fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input, fs-skia-ui-widgets
- `governed-source`: fs-skia-scene, fs-skia-testing
- `headless-scene-source`: fs-skia-scene
- `sample-pack-source`: fs-skia-scene, fs-skia-skiaviewer, fs-skia-elmish

Each generated skill names no framework-only target/path and carries ≥1
consumer-runnable `fsharp` snippet; each skill's `name:` equals its destination
directory; Claude peers and the bundled `docs/api-surface/` reference are present.

## SC-001 (governing)

A freshly generated consumer project (all five rows / four profiles) **builds,
runs its tests, and produces its evidence using only local references** —
confirmed by `GeneratedProductCheck`. The bundled `docs/api-surface/` (US2),
`docs/effects-boundary.md` (US5), and the consumer-facing capability skills make
the API, effects boundary, and usage guidance readable locally without framework
source, framework-only targets, or DLL reflection.

## Generated-product evidence-audit drain fix

The generated product's `Verify` runs its own `EvidenceGraph`/`EvidenceAudit`.
The generated build script (`template/base/build.fsx`) captured the audit
subprocess with sequential `StandardOutput.ReadToEnd()` then
`StandardError.ReadToEnd()`, which deadlocks when the child fills the second pipe
— surfaced here because this feature's readiness evidence enlarges the audit diff
scan beyond the ~64 KB pipe buffer. Fixed to drain both streams concurrently
(`ReadToEndAsync` before `WaitForExit`) at both call sites, making
`GeneratedProductCheck` robust to audit output size. Logs:
`readiness/logs/generated-product-check-038.txt`.

## Non-authoritative aggregate note

Per repository policy, aggregate FAKE results never substitute for the focused
gates above. The headless `libdecor-gtk` Wayland flake (Test-host teardown crash)
is avoided by forcing the X11 backend; if it recurs, the focused control
`dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -m:1` is authoritative.
