# Feature tier record (T004)

- **Tier**: 1 — escalated (`maintainer-verify` / broad-validation path), because the change
  touches `template/**`, public `src/**/*.fsi`, and governance paths.
- **Route output** (working-tree at setup, spec-docs only): tier=agent-ready,
  gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit.
  Route escalates further as `template/**` and `src/**/*.fsi` edits land; the authoritative
  broad-validation set is the escalated serialized six-target order in Phase 9.
- **Affected layers**: Scene (Translate/SizedText), Controls (renderTree multi-axis layout +
  Bounds + hitTest + Stack.orientation), Controls.Elmish (reuses shipped InteractiveAppHost —
  no new surface), SkiaViewer (key warm-up FIFO), and the generated template.
- **Public-API impact**: additive only — Scene `Translate`/`SizedText` cases + `translate`/
  `sizedText` + descriptors; Controls `ControlRenderResult.Bounds`, `Control.hitTest`,
  `Stack.orientation`. No field-meaning changes; `Layout` field kept; 080 preview byte-identical.
- **Governance risk level**: broad (see governance-risk-levels.md).
- **Principle IV (MVU)**: applies to **US2 only**, satisfied by reusing the shipped
  `InteractiveAppHost`/`runInteractiveApp` seam (`routeInteractivePointer` is the pure
  window-free transition under test). No new effect algebra.
- **Required evidence obligations**: neutral-scaffold grep (SC-001), real-controls render +
  live screenshot (SC-002), pointer dispatch (SC-003), side-by-side bounds (SC-004),
  per-control bounds + hit-test (SC-005), scene translate/sized-text (SC-006), keystroke
  delivery (SC-007), generalized host-lock assertion for the game family (SC-008).
