# Runtime limitations + permanent non-goals — feature 096 (T009)

## Supported runtime

The bridge runs wherever the framework runs: a **.NET 10 desktop** host rendering through **Vulkan**
via the **SkiaSharp preview** native binding. Targets are Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these are out of
scope for the framework and therefore for this feature.

## Scope handling (FR-003/005/008/009)

- **Single carrier channel** — the bridge writes only the pre-existing `Attr.visualState` attribute
  (the E3 carrier `ControlInternals.visualStateOf` reads). There is **no** second / parallel
  consumer-state channel (FR-003).
- **No new `VisualState` case** — the projection returns only existing cases; the runtime-derivable
  tail is `Pressed > Selected > Focused > Hover > Normal`.
- **No new token literal / no second contrast policy** — any styling flows through E3's `Style.resolve`
  over DTCG-sourced tokens; `ContrastCheck` stays the sole contrast authority (FR-008).
- **Total and silent** — a `Normal`-and-unset node is a no-op (emits nothing); the bridge never throws
  and emits no diagnostics (FR-005). An id named by no interaction state derives `Normal`.
- **`ControlId` domain only** — the bridge binds in the `ControlId` domain (never `RetainedId`); the
  host resolves `focused` (`RetainedId`) back to its `ControlId` before deriving.
- **Non-migrated kinds** — derive a state but produce no visible change (the geometry is not yet routed
  through `Style.resolve`); no render-output delta.
- **No data-binding / observable / dependency-property / selector / template surface** is introduced
  (FR-009). The `view : 'model -> Control<'msg>` consumer contract is unchanged.

## Failure diagnostics

No new failure path is introduced. Missing-artifact failures are the existing readiness-gate classes
(a required readiness file absent or malformed → the owning gate reports it). The bridge itself has no
runtime error path to diagnose (pure, total projection + pure, total tree walk).
