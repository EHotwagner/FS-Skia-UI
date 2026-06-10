# Contract: Live Input→Visible-Change Responds-Proof (FR-006 / FR-007)

**Surface**: a capturable framework artifact (working name `captureRespondsProof`) over the running host,
plus a **durable evidence obligation** in the `.agents`/`.claude` evidence-mode skill tree.

## Behavior

Given a host + a synthesized live interaction (the same `ViewerPointerInput` the live loop feeds):
render **before**, apply the interaction (route → `host.Update` fold → repaint, exactly as
`SkiaViewer.fs:2469`), render **after**, and emit both frames + a **verdict**:
`Responsive` when before ≠ after, `Inert` when identical.

## Guarantees

- **P1 (capturable proof).** The framework can capture that a real input on the running host produced a
  **visible change** in rendered output (SC-004).
- **P2 (distinct evidence class).** The proof is recognized as distinct from (a) a render-only screenshot
  (one frame, no interaction) and (b) the offscreen `runInteractivePointerOnce` route probe (model layer
  only) — neither of which satisfies the FR-007 obligation (FR-006 interacting-requirements note).
- **P3 (inert fails).** An app that renders but does not respond yields **identical** before/after and
  the proof's verdict is `Inert` — it **cannot** be passed off as a responds-proof (SC-004).
- **P4 (durable obligation).** The "responds, not just renders" obligation is recorded durably so it
  **binds future interactive-UI stories**; where it lives in the `.agents` skill tree it is regenerated
  into the `.claude` mirror and `SkillSyncCheck` enforces byte-identity (FR-007, SC-006).

## Evidence honesty

Render-only / offscreen capture remains valid for what it proves (render path; model-layer routing) — the
proof **extends**, not contradicts, 089's production-render-path rule. The before/after frames are honest
render-only artifacts ([[fs-skia-evidence-mode]]); no live Vulkan window is required to capture them
(render-target PNG, [[controls-preview-render-pipeline]]).

## Verification

- Responsive host (a counter incremented by an `onClick`): before ≠ after → `Responsive` (P1).
- Inert host (the 090-pre-fix behavior, or a host whose binding is dropped): before = after → `Inert`
  (P3) — this is the failing-first test that the dead-window regime could not previously catch.
- Governance: the obligation text is present + `.claude`↔`.agents` byte-identical (P4).
</content>
