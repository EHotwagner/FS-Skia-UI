# interactive-visible-window — applicability (feature 113, T002)

status=not-applicable
mode=render-only
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 113 ships no NEW persistent/graphical entry point — it is a control-internal memoization seam +
two additive `FrameMetrics` fields + a report-only stability diagnostic, proven through deterministic,
headless evidence (the internal `RetainedRender.memoize` seam via InternalsVisibleTo from Controls.Tests,
the `Perf.runScript` metrics, and the standing Scene-parity suite under Dev). A live Vulkan window is NOT
required; the existing `runInteractiveApp` window launch contract is unchanged (its signature is
untouched; only whether a pure subtree is recomputed or reused changes, with at-rest output
byte-identical, FR-014/SC-002).
