# Visual evidence honesty (080)

Visual proof for this feature keeps screenshot proof, rasterized scene proof,
layout readability proof, fallback classification, and unsupported proof
separate (per `fs-skia-evidence-mode`). Accepted visual proof names a decodable
image, image dimensions, non-trivial content, renderer mode, fallback
classification, and unsupported reason.

- **Authoritative command**: `dotnet run --project tests/ControlsPreview.Harness -- --render`
  / `-- --fidelity`; decode the committed PNGs.
- **Artifact path**: `docs/img/controls/*.png` (decodable, 320×160), renderer
  mode `viewer-render-target` (`ViewerRenderTargetPng`); `readiness/control-fidelity.md`
  carries the decoded-content verdict.
- **Failure class** (rejection phrases): metadata-only reports do not satisfy
  visual proof; 1x1 fallback images do not satisfy visual proof; layout-only
  bounds claims do not satisfy visual proof. Native-Skia-absent decode is a
  **blocking warning**, not a silent pass.
- **Fallback classification**: render-only deterministic evidence
  (`DeterministicRenderOnly`) — this is **not** a readable-layout claim; readable
  layout is out of scope for catalog previews.
- **Unsupported reason**: `custom-control` is honestly `Unsupported` (no canonical
  sample to depict render-only); it commits no image.
- **Next action**: confirm each Demonstrative preview is a decodable 320×160 PNG
  with non-trivial content beyond the title band (T014/T026).

_Placeholder created in T002; decoded confirmations land in T014/T026._
