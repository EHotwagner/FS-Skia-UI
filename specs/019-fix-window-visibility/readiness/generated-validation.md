# Generated Validation Evidence

exact-package-match=true
generated-tests-exist=true
generated-tests-ran=true
authoritative=true
failure-class=none

Status: PASS for T049 generated-product validation.

Authoritative command sequence:

- `./fake.sh build -t PackLocal`
- `./fake.sh build -t TemplateCheck`
- `./fake.sh build -t GeneratedProductCheck`

Primary validation record:

- `specs/019-fix-window-visibility/readiness/generated-product-validation.md`

Focused logs:

- `specs/019-fix-window-visibility/readiness/logs/t047-retry-pack-local-presenter-tests.txt`
- `specs/019-fix-window-visibility/readiness/logs/t047-retry-template-check-presenter-tests.txt`
- `specs/019-fix-window-visibility/readiness/logs/t047-retry-generated-product-check-presenter-tests.txt`

## Restore And Package Evidence

- Local package feed: `/home/developer/.local/share/nuget-local`
- Generated consumer root: `/home/developer/projects/FS-Skia-UI/artifacts/generated-products/019-fix-window-visibility/app-source`
- Restore log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/restore.log`
- Exact package match: `True`
- Restore warning count: `0`
- Failure class: `none`

Requested packages:

- `FS.Skia.UI.Controls=0.1.18-preview.1`
- `FS.Skia.UI.Controls.Elmish=0.1.18-preview.1`
- `FS.Skia.UI.Elmish=0.1.18-preview.1`
- `FS.Skia.UI.KeyboardInput=0.1.18-preview.1`
- `FS.Skia.UI.Layout=0.1.18-preview.1`
- `FS.Skia.UI.Scene=0.1.18-preview.1`
- `FS.Skia.UI.SkiaViewer=0.1.18-preview.1`

Resolved packages:

- `FS.Skia.UI=0.1.18-preview.1`
- `FS.Skia.UI.Controls=0.1.18-preview.1`
- `FS.Skia.UI.Controls.Elmish=0.1.18-preview.1`
- `FS.Skia.UI.Elmish=0.1.18-preview.1`
- `FS.Skia.UI.KeyboardInput=0.1.18-preview.1`
- `FS.Skia.UI.Layout=0.1.18-preview.1`
- `FS.Skia.UI.Scene=0.1.18-preview.1`
- `FS.Skia.UI.SkiaViewer=0.1.18-preview.1`

`FS.Skia.UI` is expected after the presenter bridge: `FS.Skia.UI.SkiaViewer` uses the repo-owned Vulkan/Skia presenter to commit real swapchain frames for persistent interactive windows.

## Generated Test Command Evidence

- Generated Verify log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/generated-verify.log`
- Generated tests exist: `True`
- Generated tests ran: `True`
- Generated Verify ran: `True`
- Latest generated test result: `Passed: 17, Failed: 0`
- Authoritative: `True`
- Failure class: `none`

The generated Verify log retains earlier failed attempts for traceability. The latest run passes after qualifying generated test calls to `Product.Program.update`.

## Interactive Validation

- Persistent launch diagnostics log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/persistent-launch-diagnostics.log`
- Default interactive launch: `True`
- Close reason validation: `True`
- Latest persistent result: `status=ok`
- Window opened: `true`
- Window visible: `observed:true`
- Accessible window: `true`
- First frame presented: `true`
- User close observed: `true`
- Self closed for evidence: `false`
- Input dispatch: `true`
- Renderer mode: `skia`
- Failure class: `none`

The supported-host launch was manually confirmed visible and interactable while `GeneratedProductCheck` was running.

## Bounded Evidence Validation

- Bounded smoke log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/bounded-smoke.log`
- Bounded smoke evidence: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/bounded-smoke.txt`
- Bounded evidence validation: `True`
- Frames rendered: `1`
- Renderer mode: `vulkan`
- Failure class: `none`

## Option Validation

- Window diagnostics log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/window-diagnostics.log`
- Window diagnostics output: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/window-diagnostics.txt`
- Window options log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/window-options.log`
- Window options output: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/window-options.txt`
- Window diagnostics validation: `True`
- Window options validation: `True`
- Failure class: `none`

Option rows include initial size, resize policy, maximize policy, startup state, startup position, and backend preference, with unsupported OpenGL reported as an explicit unsupported backend preference rather than a silent fallback.

## Image Evidence Validation

- Scene evidence log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/scene-evidence.log`
- Scene evidence output: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/headless-scene-evidence.txt`
- Image evidence log: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/image-evidence.log`
- Image evidence output: `specs/019-fix-window-visibility/readiness/generated-consumer-validation/game-image-evidence.png`
- Image evidence validation: `True`
- Image decodable: `True`
- Proves scene rendering: `true`
- Proves desktop visibility: `false`
- Failure class: `none`

Desktop visibility is proven separately by the persistent launch diagnostics; image evidence remains scene-rendering evidence and does not claim desktop visibility.

## Verdict

- Package resolution: `validated`
- Generated test execution: `validated`
- Default interactive launch: `True`
- Bounded evidence validation: `True`
- Close reason validation: `True`
- Window diagnostics validation: `True`
- Window options validation: `True`
- Image evidence validation: `True`
- Authoritative: `True`
- Failure class: `none`

## Elapsed-Time Validation

Prepared supported-host validation command:

- `./fake.sh build -t GeneratedProductCheck`

Timing evidence:

- Log: `specs/019-fix-window-visibility/readiness/logs/t051-generated-product-check-elapsed.txt`
- Bash timer elapsed seconds: `100`
- FAKE reported runtime: `1 minute, 39 seconds`
- Exit code: `0`
- Required limit from SC-007/T051: less than `300` seconds on a prepared supported host
- Verdict: PASS

## Manual-Ready Timing

Prepared supported-host command:

- `dotnet run --project artifacts/generated-products/019-fix-window-visibility/app-source/src/Product/Product.fsproj --no-restore`

Timing evidence:

- Log: `specs/019-fix-window-visibility/readiness/logs/t052-command-launch-manual-ready.txt`
- Started at: `2026-05-27T03:56:15+02:00`
- Ended at: `2026-05-27T03:56:20+02:00`
- Elapsed seconds: `5`
- Exit code: `0`
- Required limit from SC-008/T052: manual interactive testing can begin within `30` seconds
- Environment-variable workaround required: `false`
- Window opened: `true`
- Window visible: `observed:true`
- Accessible window: `true`
- First frame presented: `true`
- User close observed: `true`
- Verdict: PASS

## Supported Host Matrix And SC-003 Rule

SC-003 acceptance rule:

- Minimum repeated-launch attempt count per prepared supported host: `20`
- Passing threshold per host: at least `19/20` launches produce a visible, focusable, interactable generated game window.
- Overall threshold across the supported host matrix: at least `95%` visible/focusable/interactable launches.
- Every non-visible or non-focusable launch must be classified before app lifecycle debugging as one of:
  - `environment-session`
  - `window-visibility`
  - `window-options`
  - `package-verification`
  - `app-lifecycle`
  - `unsupported-host`

Supported host matrix:

| Host | Session/backend | Status | Coverage in this feature | Required classification for exceptions |
|------|-----------------|--------|--------------------------|----------------------------------------|
| Linux desktop/container with GPU passthrough and Wayland socket | `WAYLAND_DISPLAY=wayland-0`, `XDG_RUNTIME_DIR=/run/user/1000`, Vulkan/Skia presenter | Supported prepared host | Covered by `GeneratedProductCheck`; generated window manually confirmed visible/interactable; latest persistent launch reported `window-visible=observed:true`, `accessible-window=true`, `user-close-observed=true` | `environment-session` when display/runtime/session bus is missing; `window-visibility` when taskbar-only/minimized/unmapped; `app-lifecycle` only after visible-window diagnostics pass |
| Linux X11 or XWayland session | `DISPLAY` with X socket, Vulkan/Skia presenter | Supported when the host can present/focus native windows | Not separately counted in this T050 record; same diagnostics apply when used as a prepared supported host | `environment-session` for missing/inaccessible X socket; `window-visibility` for process/taskbar-only or inaccessible window |
| Windows desktop session | Native desktop with Vulkan-capable GPU/driver | Supported by contract | Not exercised in this Linux container evidence set | `environment-session` for unavailable desktop interaction; `renderer`/`swapchain` for Vulkan setup failures; `window-visibility` for inaccessible native window |
| Headless Linux/CI without display socket | No `DISPLAY` and no `WAYLAND_DISPLAY` | Unsupported for normal interactive launch; still valid for bounded/evidence commands | Covered by unsupported-host diagnostic tests and prior readiness logs | `unsupported-host` or `environment-session`; must not be reported as visible-window success |

Current host/session coverage:

- `XDG_RUNTIME_DIR=/run/user/1000`
- `WAYLAND_DISPLAY=wayland-0`
- `DISPLAY=:1`
- `DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus`
- Persistent generated launch diagnostic class: `environment-session-ready`
- Persistent generated launch backend: `skia` through the Vulkan/Skia presenter bridge
- Latest persistent generated launch result: `status=ok`, `window-visible=observed:true`, `accessible-window=true`, `first-frame-presented=true`, `user-close-observed=true`

Repeated-launch counting is defined here for SC-003. T051/T052 record elapsed-time and manual-ready timing evidence for the prepared supported host; broad validation in T056/T057 uses this matrix and exception classification rule.
