# Visual Evidence Honesty — Feature 090

This feature produces a **render-only** responds-proof: a before/after pair of
rendered frames captured around a real dispatched interaction on the production
`Control.renderTree` path (`ControlsElmish.captureRespondsProof`). The honesty
discipline:

- The responds-proof is a **render-only** artifact (`mode=render-only`). It does
  **not** claim a live desktop-window screenshot or desktop visibility.
- It is a **distinct evidence class** from a render-only screenshot of a single
  frame and from the offscreen `runInteractivePointerOnce` route probe (model
  layer only). The distinction is the **input→visible-change pairing**: before a
  dispatched interaction ≠ after it.
- An **inert** app (renders but does not respond) yields identical before/after
  frames and an `Inert` verdict — it cannot be passed off as a responds-proof, so
  a metadata-only or wrong-path substitution cannot satisfy it.
- The before/after frames are honest renders of the **production** surface the host
  draws (`host.View` → `Control.renderTree`), not a bespoke parallel scene built to
  produce a pretty picture (089 production-render-path discipline).

No 1x1 fallback image, metadata-only report, or layout-only bounds claim is
offered as visual proof.
