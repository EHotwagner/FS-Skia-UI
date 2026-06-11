# Runtime limitations + permanent non-goals — feature 102 (R8, T002/T003)

## Supported runtime

R8 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through **Vulkan**
via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; these are out of
scope for the framework and therefore for this feature. R8 adds **no** runtime code on any host path:
it is a documentation/internal-comment honesty pass (roadmap prose + descriptive source comments) with
zero behavior change, so it is platform-independent and introduces no new runtime, window, GPU, or
wall-clock dependency.

## FR-003 / §10.4 intrinsic-size-memo deferral cross-reference (SC-006)

R8 reconciles roadmap §10.4 to describe the **shipped** R2 cache — a computed-`Bounds` cache keyed by
structural `LayoutNodeId` — and removes the "measured intrinsic size … keyed by retained identity"
claim. Landing the intrinsic-size memo itself is the recorded deferral of feature 101 (R7, FR-008); R8
only reconciles the wording and cross-references that decision. No memo is added here.

## Out of scope / permanent non-goals (FR-009)

- **R6 visual-state cross-fade** (the one behavior-changing follow-up) is out of scope.
- **Enabling** default navigation routing for `Chart`/`Graph`/`Progress`, or adding a `Segmented`
  `AccessibilityRole`, is out of scope — R8 only *documents* the current narrowing.
- **Landing** the R2 intrinsic-size memo is feature 101's recorded deferral, not R8.
- **Permanent roadmap non-goals preserved**: no data binding, no dependency/attached properties, no
  CSS selectors, and no lookless template engine. R8 adds none of these — it edits prose and comments.

## Failure diagnostics

No new runtime failure path is introduced. Every edit is a descriptive comment or report-prose change;
no logic, no `.fsi` signature, and no diagnostic message changes. The existing R1/R2/R4/R5 property and
unit suites stay green and byte-identical, which is the evidence that no comment was parsed as a
behavior token (FR-010).
