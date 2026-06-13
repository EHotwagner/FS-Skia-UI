# Runtime limitations & failure diagnostics (feature 114)

## Documented evidence path

Feature 114 is a **viewport-virtualization contract + observability + offscreen-correctness** change
proven by deterministic, headless evidence; a live Vulkan window is **not required** (spec *Unsupported
scope* / Assumptions). The asserted surfaces:

- the overscan model `Collections.visibleRange ... overscan` and the `DataGrid` realized window, exercised
  from `Controls.Tests` — bounded + non-scaling + edge-clamp + transparent-small-grid
  (`Feature114OverscanTests`), default-0 byte-identity + opt-in correctness (`Feature114OverscanParityTests`);
- offscreen focus/selection + boundary-crossing relocation over the `DataGridModel` `update`
  (`Feature114OffscreenTests`);
- the a11y total + focused position from the logical model (`Feature114AccessibilityTests`);
- the deterministic `VirtualItemsMaterialized`/`VirtualItemsTotal` over `ControlsElmish.Perf.runScript`
  (`Feature114VirtualMetricsTests`) + the regenerated 109 perf-corpus goldens (incl. the 10000-row
  scenario);
- the standing Scene-parity golden suite under `Dev` for at-rest rendered-output + geometry byte-identity.

A live window CAN open via the X11 path, but it is not part of this feature's required evidence —
virtualization changes only *which* rows are materialized and *whether* an offscreen key relocates the
window, observable via the deterministic `Perf.runScript` metrics and the internal seam tests, not a live
window. The live render staying byte-identical at rest is covered by the Scene-parity suite under `Dev`.

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A regression that re-materializes every row, or an overscan exceeding `visible + 2*overscan`, surfaces
  as a moved `VirtualItemsMaterialized` golden (`Feature114VirtualMetricsTests` + the 109 corpus) instead
  of silent CPU/memory cost.
- A non-byte-identical default-overscan slice fails `Feature114OverscanParityTests` and/or the Scene-parity
  suite under `Dev`.
- An offscreen relocation that EXPANDS the window (instead of relocating) fails the bound assertion in
  `Feature114OffscreenTests`.
- An a11y total/position computed from the materialized slice instead of the logical model fails
  `Feature114AccessibilityTests`.

## Platform / runtime support boundary

Feature 114 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are out
of scope. The 114 evidence is GPU-free deterministic overscan/offscreen/a11y/metrics assembly, so it does
not depend on the live Vulkan surface.
