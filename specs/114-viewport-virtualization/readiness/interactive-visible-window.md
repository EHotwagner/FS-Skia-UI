# interactive-visible-window — applicability (feature 114, T002)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 114 ships no NEW persistent/graphical entry point — it is a viewport-virtualization contract
(overscan defaulted to 0), two additive `FrameMetrics` fields, an additive `Collections`/`DataGrid`/`Types`
surface, and offscreen focus/selection addressability + a11y totals on the logical model — proven through
deterministic, headless evidence (the `Collections.visibleRange`/`DataGrid` realized window and the
`DataGridModel` `update` via Controls.Tests, the `Perf.runScript` metrics, and the standing Scene-parity
suite under Dev). A live Vulkan window is NOT required; the existing `runInteractiveApp` window launch
contract is unchanged (its signature is untouched; only which rows are materialized / whether an offscreen
key relocates the window changes, with at-rest output byte-identical, FR-006/FR-016).
