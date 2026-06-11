# Window-State Diagnostics (098, R3)

status=not-applicable

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no desktop session window is opened by this feature.
- diagnostic-class=window-visibility — not-applicable; no window is created, so there is no
  visible/invisible window state to observe.
- diagnostic-class=app-lifecycle — no persistent app lifecycle is started; the deterministic routing-seam
  dispatch / keyed-regression / FsCheck-distinctness / single-scheme suites run to completion and exit.
- diagnostic-class=product-defect — none observed; no window code path is exercised by this feature.

## Observable-vs-unsupported native facts

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

No taskbar-entry or process-only success is claimed, and no unsupported-host-only visibility claim is made —
there is simply no window in scope. The recovery/dispatch is exercised through the pure `boundIdsOf`
derivation, the pure widened `nearestAuthored`, and the live-adapter `routeInteractivePointer` routing seam
(off-window), not a windowed host. The `Scene`/`Layout`/`Bounds` rectangles are byte-identical; only the
unkeyed `ControlId` labels change (FR-007).
