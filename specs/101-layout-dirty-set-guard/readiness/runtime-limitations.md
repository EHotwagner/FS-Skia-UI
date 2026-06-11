# Runtime limitations + permanent non-goals — feature 101 (R7, T005)

## Supported runtime

The anti-drift guard runs wherever the framework runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding. Targets are Windows and Linux desktop
(`net10.0`). **unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these
are out of scope for the framework and therefore for this feature. R7 itself adds **no** runtime code
on any host path — it is a build/test-time guard over the existing pure `evaluateLayout` /
`layoutDirtySet` / `layoutAffectingAttrNames`, so it is platform-independent and the new tests run
in-process with no window, GPU, or wall-clock dependency.

## FR-008 intrinsic-size-memo deferral (SC-006, research D6)

R2 (feature 097) shipped a computed-`Bounds` cache only. The **optional intrinsic-size memo** named in
roadmap §10.4 is **DEFERRED** and is **not** landed in R7:

- No profiled workload shows the fixed-size-ancestor boundary re-measure is hot, so the memo would be
  an **un-profiled** cache.
- R7's charter is anti-drift hardening with **zero behavior change**; adding a new cache would
  introduce a second structure to validate against the incremental-≡-full property and would widen
  scope beyond the guard, risking the zero-delta guarantee.
- The §10.4 wording reconciliation (R2 cached `Bounds` only; the memo is optional/deferred) is
  **delegated to R8** per FR-008. This decision is recorded here so R8 can reconcile the roadmap text
  without ambiguity. If a future profile shows the boundary re-measure hot, the memo lands keyed by
  retained identity per §10.4 and is gated by the same incremental-≡-full property — as its own
  change, not R7.

## Out of scope / permanent non-goals (FR-009)

- **No expansion of the layout-driving attribute set.** R7 *guards* un-guarded additions; it does not
  *make* them. The shipping set stays `{width;height;orientation}`.
- **R6 visual-state cross-fade** and the **R8 doc-narrowing reconciliations** (Yoga point-scale
  rationale, R1/R5 surface notes) are out of scope.
- **Collection virtualization** is out of scope.
- **Permanent roadmap non-goals preserved**: no data binding, no dependency/attached properties, no
  CSS selectors, and no lookless template engine. R7 adds none of these — it is internal classifier
  wiring + a test-only gate.

## Failure diagnostics

No new runtime failure path is introduced. `layoutDriftReport` / `formatDrift` are pure, total, and
never throw; the probe runs the real total `evaluateLayout`. The actionable signal is the guard
**itself**: a contributor who makes `toLayout` read an un-covered attribute (or lists an unused name in
`layoutAffectingAttrNames`) gets a fast, explicit Expecto failure under `Dev` naming the drifting
attribute and its direction (`un-covered layout input: 'padding' …` / `over-broad classifier entry:
'orientation' …`), instead of a silent stale-bounds symptom. The documented coverage boundary
(corpus-bounded discovery) is recorded at the test site.
