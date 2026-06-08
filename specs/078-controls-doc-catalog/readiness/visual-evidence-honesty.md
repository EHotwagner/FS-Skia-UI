# Visual-Evidence Honesty (078)

This feature commits per-control **render-only** preview PNGs as docs source
assets. The honesty contract (per `fs-skia-evidence-mode` and the visual-proof
rules) for every committed preview:

- It is produced through the existing **deterministic render-only** evidence path
  (off-window raster), not a fabricated or hand-drawn image.
- It is **decodable**, has **non-1×1** real dimensions, and carries **non-trivial**
  content — validated with `Testing.readPngArtifact`.
- A control that cannot be honestly rendered gets **no asset** and an explicit
  **unsupported note** on its detail page. A 1×1 image, a metadata-only
  "preview", or any placeholder is forbidden and rejected.
- A genuine `RenderingFailure` is preserved as a real diagnostic, never masked as
  an "unsupported" note; benign environment warnings are classified benign.

Per-control results are recorded in `controls-preview-evidence.md`; the
`ControlsCatalogDocsCheck` gate enforces present/decodable/non-orphan previews
and honest unsupported notes.
