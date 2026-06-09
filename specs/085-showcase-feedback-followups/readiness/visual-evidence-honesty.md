# Visual evidence honesty (085)

Independent validation path for the visual deliverables, and the honesty rules
applied when capturing them.

## US1 — `Control.renderTree` distinctness (SC-001)

- **FSI distinctness check**: `Control.renderTree theme size pageA` vs
  `... pageB` for two structurally different trees ⇒ the returned `Scene` values
  differ and nested children (not just the outer container) are laid out/painted.
  Captured to `readiness/fsi-session.txt`.
- **Screenshot diff**: render two distinct pages to PNG via
  `Viewer.captureScreenshotEvidence` and confirm the per-page diff is **non-empty**
  (`evidence/render-distinctness/*.png` + diff), recorded in `real-image-evidence.md`.
- **Honesty rule**: a render-target PNG proves scene rendering, **not** desktop
  visibility; the two claims are kept separate. A metadata-only/1×1 fallback is
  never claimed as visual proof.

## US2/US4 — host launch and size-aware render

- Real visible-window launch evidence lives in the window-visibility class
  (`interactive-visible-window.md` et al.); off-window render-target rasters are
  labelled `proves-desktop-visibility=false`.
- Where live pointer injection is unavailable, synthetic-event-through-the-real-
  adapter is the honest bar (not `[S]`); see `runtime-limitations.md`.
