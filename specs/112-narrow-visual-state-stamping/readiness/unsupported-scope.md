# Unsupported scope & failure diagnostics (feature 112, T007)

- Phase 5+ is OUT: view/control memoization + stable-dependency diagnostics (Phase 5), viewport
  virtualization (Phase 6), damage rects / picture / paint caches (Phase 7), text / layout-boundary
  caches (Phase 8), SkiaViewer backend / render-thread / compositor review (Phase 9).
- The full-tree `ControlRuntime.applyRuntimeVisualState` stamp is PRESERVED as the parity oracle and the
  fallback (FR-005); it is not removed.
- Narrowing the reconciler DIFF (as opposed to the stamp) is OUT — this feature narrows only the stamp.
- Features 110 (retained routing) and 111 (frame scheduler / view-skip) are UNCHANGED (FR-009).
- The targeted path degrades to the full oracle on a model-change / first / structurally-misaligned frame
  (the per-node child-count-mismatch self-heal), so it never produces a stale render (FR-006).
- Principle IV (MVU) is N/A — no Model/Msg/Effect/interpreter change. The interactive-UI run-and-use gate
  is N/A — the feature ships an internal stamp optimization observable via the internal count + the
  preserved live render path, not a new interactive surface.
