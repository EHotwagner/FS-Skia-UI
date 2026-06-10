# Runtime Limitations — Feature 092

This feature re-keys the interactive host's **focus/text/clock state** onto the stable `RetainedId`
and folds the 090/091 render-path defects (focus-on-click via the retained tree, value+line-mode
seeding, all-`onChanged`-bindings dispatch, shifted-work counter, theme in the reuse key, single
first-frame paint + frame-0 diagnostics). It reads existing layout/hit-test/render data and
introduces **no new rendering machinery**. All correctness evidence is captured **headless/offscreen**
through the real adapter seam; **no live Vulkan window is required** ([[fs-skia-evidence-mode]]).

## Platform runtime envelope (unchanged by this feature)

- **.NET 10 desktop** is the supported host (`net10.0`, Windows + Linux desktop).
- Live windows render through **Vulkan** via the **SkiaSharp preview** native backend.
- **unsupported macOS/mobile/browser** — out of scope; no headed window path is validated there.
- **no software-renderer fallback** — a headless/over-SSH environment without a GPU/display cannot
  present a live window; the 092 proofs need no live window (render-only structural/identity
  capture). Dual Wayland/X11 sessions must force the X11 path to avoid the `libdecor-gtk` hazard.

## Unsupported-scope handling + failure diagnostics

- **Correctness-wins fallback (FR-010).** The added `ShiftedNodeCount` measurement and theme-reuse
  gating **never alter the produced scene**: output stays byte-identical to a full rebuild. If a
  measurement and correctness ever conflicted, correctness wins (the count is advisory).
- **Frame-0 `KeyCollision` (FR-009).** A duplicate-key collision present in the FIRST tree is now
  surfaced by `RetainedRender.init` (091 surfaced it a frame late) through the existing
  `ControlDiagnostic` de-dup channel; the path stays **total** (no throw) on malformed input.
- **MapKey widening (FR-006).** `InteractiveViewerHost.MapKey : 'msg list`; `[]` = unhandled. The
  consumer `InteractiveAppHost.MapKey` stays `'msg option` (lifted via `Option.toList`), and
  `GeneratedAppHost.MapKey` is unchanged — see `governance-risk-levels.md`.
- **Focus/tab-traversal & full editor UX deferred to E4.** Caret/selection/IME-UX/undo/redo/clipboard
  text-editing remain out of scope (trajectory item E4); the seam delivers a keystroke to the
  focused control only.
- **Non-authoritative `GeneratedProductCheck`.** `GeneratedProductCheck` drives a real consumer
  restore/build/`Verify`; locally it can fail for **environment** reasons (the generated `Verify`
  cannot resolve an active feature: no template `.specify/feature.json` + a `Map.empty`
  environment) — a **non-authoritative environment-failure**, recorded in `logs/`, NOT a product
  defect.
