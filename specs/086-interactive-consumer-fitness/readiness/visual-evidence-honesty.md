# Visual-evidence honesty — feature 086

Honesty vocabulary for visual proof on this feature. A render claim must distinguish a
**decodable image** with real **non-trivial content** from metadata-only substitutes.

- decodable image; image dimensions; non-trivial content; renderer mode; fallback
  classification; unsupported reason.
- metadata-only reports do not satisfy visual proof; 1x1 fallback images do not satisfy
  visual proof; layout-only bounds claims do not satisfy visual proof.
- A `renderTree` `Bounds` list (layout-only) proves geometry, NOT pixels — the headless
  render-target PNG (decoded, lit pixels) is the visual proof for Scene primitives (US5).
- Live-window vs render-target host-warning classification: a missing GPU/display session is
  a **benign/unsupported** host fact (non-failing), not a product defect; a blank/!decodable
  surface on a supported host is a **blocking** product defect.
