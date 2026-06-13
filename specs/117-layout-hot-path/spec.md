# Feature Specification: Layout Hot-Path Improvements

**Feature Branch**: `117-layout-hot-path`  
**Created**: 2026-06-13  
**Status**: Draft  
**Input**: User description: "@docs/reports/2026-06-12-1422-controls-performance-framework-research.md do next part."

> **Source.** This is **Phase 8 (Layout Hot-Path Improvements)** of the staged
> controls-performance plan in
> `docs/reports/2026-06-12-1422-controls-performance-framework-research.md`.
> Phases 0–7 shipped as features 109–116 (Phase 7 = paint cache + damage rects =
> feature 116, merged 2026-06-13). Phase 8 is the next rung: keep incremental
> layout correct while reducing avoidable measure work and making dirty
> propagation observable. The remaining Phase 9 (backend/host-mode review) is out
> of scope for this feature.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Text-heavy frames stop re-measuring identical text (Priority: P1)

A consumer renders a control surface whose labels, button captions, and cells
repeat the same text/font many times per frame (and across frames, when nothing
about that text changed). Today every label leaf re-runs `Scene.measureText`,
and the auto-fit caption path (`fittedFontSize`) runs a binary search that calls
`measureText` 1 + up to 16 (≈17) times for a single caption. A consumer scrolling, hovering,
or animating over a text-dense surface pays full text shaping cost on every
frame even though the measured text and font are identical.

This story adds a deterministic text-measurement cache keyed by the full set of
inputs that affect a measured result (text string, font family, font size, font
weight, and the relevant measurement constraints). A measurement whose key was
seen and is still resident is reused; a new or evicted key is measured and
stored. The cache is observable through two new deterministic frame-metric
counters (hits and misses) so evidence can prove that a text-heavy scenario
produces cache hits and that a first/cold frame produces misses.

**Why this priority**: This is the single largest avoidable repeated cost on the
layout hot path identified by the report's Phase 8, and it is the only item with
a clear, byte-identical, deterministically-assertable mechanism. It is
independently shippable and testable.

**Independent Test**: Drive a `Perf.runScript` scenario that lays out a surface
with many repeated identical captions/labels, then re-lays it out on a frame
where the text inputs did not change. Assert that the second frame reports text
cache hits and (near) zero misses, while the cold first frame reports misses.
Assert at-rest rendered output is byte-identical to the pre-cache baseline.

---

### User Story 2 - Layout dirty propagation is observable by count (Priority: P2)

A maintainer extending a high-density control needs to know how far a layout
invalidation propagated: how many nodes were *flagged dirty* for this frame
versus how many were *actually re-measured*. Today `RemeasuredNodeCount` reports
re-measures, but the size of the dirty set that drove incremental layout (the
nodes whose layout was invalidated, before fixed-size-ancestor pinning reduced
the actual measure work) is not surfaced. Without it, "deep layout scenarios
have clear dirty propagation counts" cannot be asserted, and a regression that
silently widens the dirty set is invisible until it shows up as re-measures.

This story surfaces the layout-invalidated node count as a deterministic frame
metric, distinct from the existing `RemeasuredNodeCount`, so a localized
style/visual-state change can be proven to invalidate (and re-measure) zero or
near-zero nodes, and a geometry change can be proven to invalidate exactly its
dirty propagation.

**Why this priority**: Observability hardens US1 and the existing incremental
layout (features 097/101) against regression, but it is reporting-only and does
not itself reduce work, so it ranks below the cache.

**Independent Test**: Drive scripted frames that (a) change only a style/visual
attribute and (b) change a geometry-affecting attribute (width/height/
orientation). Assert the style-only frame reports zero layout-invalidated and
zero re-measured nodes, and the geometry frame reports a bounded, explainable
invalidated count that is `<= RemeasuredNodeCount` (direction corrected 2026-06-13).

---

### User Story 3 - Style-only and visual-state-only updates remeasure nothing (Priority: P3)

A consumer hovers, focuses, presses, or animates a control. None of those change
geometry-affecting attributes. The framework already routes these through
incremental layout, but this rung makes the guarantee explicit and
regression-proof: a style-only or runtime-visual-state-only update must
re-measure **zero** layout nodes and produce **zero** text-measure cache misses
for unchanged text, while at-rest output stays byte-identical.

**Why this priority**: This is largely an assertion over behavior the prior rungs
already produce; it formalizes the report's Phase 8 acceptance criterion as a
deterministic gate rather than introducing a new mechanism.

**Independent Test**: A scripted hover/focus/visual-state frame over a
text-bearing control asserts `RemeasuredNodeCount = 0`, the new
layout-invalidated count `= 0`, and text-cache misses `= 0` (all unchanged text
served from cache), with byte-identical rendered output.

### Edge Cases

- **Cache correctness over font change**: changing only the font weight, family,
  or size for the same text MUST miss (produce a different measured result),
  never serve a stale hit. The key must include every input that changes the
  measured width/height. The available-space constraint (e.g. the box driving the
  `fittedFontSize` search) is **not** a keyed input: `Scene.measureText` is
  unconstrained, so the constraint never changes the measured result — each
  candidate font size the search tries is already a distinct key.
- **Unbounded growth**: the text-measure cache MUST be bounded (a deterministic
  eviction policy with a fixed cap), so a scenario that measures a very large
  number of distinct strings cannot grow memory without bound; eviction MUST be
  deterministic so goldens are stable.
- **Empty / whitespace text**: an empty or whitespace caption must measure and
  cache without error and remain byte-identical to today.
- **Auto-fit captions** (`fittedFontSize` binary search): the multiple
  `measureText` calls for distinct candidate sizes are distinct keys; the cache
  helps across frames (same caption + same box ⇒ same fitted-size search path)
  and MUST NOT change the chosen fitted size.
- **Multi-pass / intrinsic layout**: the codebase performs a single measure pass
  with no intrinsic-sizing path. If no such path is introduced, no multi-pass
  metric is added (a no-op against the report's optional Phase 8 task 5); the
  drift guard MUST keep it that way unless a multi-pass path is deliberately
  introduced.
- **Theme switch**: a theme change alters font family/size, so text measurements
  legitimately miss and re-measure; this is correct, not a cache failure.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The framework MUST add a deterministic text-measurement cache over
  the layout text-measurement primitive (`Scene.measureText`) used by control
  leaf measurement and by the auto-fit caption path (`fittedFontSize`). A
  measurement request whose full key is resident MUST reuse the cached result
  without re-invoking the underlying measurement.
- **FR-002**: The cache key MUST include every input that affects the measured
  result: the text string, font family, font size, and font weight. Two requests
  differing in any keyed input MUST be distinct cache entries (no stale hit across
  a differing input). A measurement constraint (e.g. the available-space box in
  the `fittedFontSize` search) is **not** keyed: `Scene.measureText` is
  unconstrained, so the constraint does not change the measured width/height —
  each candidate size the search tries is already a distinct key.
- **FR-003**: The text-measure cache MUST be bounded by a fixed capacity with a
  **deterministic** eviction policy, so that (a) memory cannot grow without bound
  over a many-distinct-string scenario and (b) hit/miss/eviction outcomes are
  reproducible for golden assertions on identical input order.
- **FR-004**: Adding the cache MUST be byte-identical at rest and for every
  scenario: the measured width/height returned for any key MUST equal the
  un-cached result, so no rendered output, layout box, or existing metric changes
  value because the cache exists. The cache is a pure performance optimization;
  correctness MUST NOT depend on it (an always-miss oracle MUST produce identical
  output and identical layout).
- **FR-005**: The framework MUST expose two new deterministic `FrameMetrics`
  integer fields counting text-measure cache **hits** and **misses** for the
  frame, `0` on a frame that measures no text, golden-asserted via
  `Perf.runScript`. They follow the established hit/miss counter pattern
  (`MemoHitCount`/`MemoMissCount`, `PictureCacheHitCount`/`PictureCacheMissCount`).
- **FR-006**: The framework MUST expose a new deterministic `FrameMetrics`
  integer field reporting the **layout-invalidated node count** for the frame —
  the size of the **pre-pinning** dirty set fed into incremental layout — distinct
  from the existing `RemeasuredNodeCount`. It MUST be `0` on an idle frame and on a
  style-only / visual-state-only frame, and `<= RemeasuredNodeCount` on a
  geometry-changing frame. **(Direction corrected during implementation,
  2026-06-13.)** In this codebase the dirty set fed into incremental layout is the
  small patch-derived **self-dirty** set, and `Layout.evaluateIncremental` then
  **expands** each dirty node up to its first fixed-size ancestor and re-measures
  that whole boundary subtree, so the post-pinning re-measured set is a *superset*
  of the pre-pinning dirty set — i.e. `LayoutInvalidatedNodeCount <=
  RemeasuredNodeCount` (the honest, code-guaranteed direction). The metric still
  surfaces dirty propagation distinct from re-measures and is bounded/explainable.
  See `readiness/layout-invalidated-authority.md`. Golden-asserted via `Perf.runScript`.
- **FR-007**: A style-only or runtime-visual-state-only update (hover, focus,
  press, animation tick) MUST re-measure zero layout nodes
  (`RemeasuredNodeCount = 0`), report zero layout-invalidated nodes, and produce
  zero text-measure cache misses for unchanged text (all served from cache),
  while remaining byte-identical at rest.
- **FR-008**: The existing layout dirty-set drift guard (feature 101's
  `layoutDriftReport` / single-sourced `layoutAffectingAttrNames`) MUST remain in
  force and MUST continue to cover any layout-affecting attribute. This rung adds
  no new geometry-driving attribute; if one is added the guard MUST still report
  an empty drift.
- **FR-009**: No multi-pass / intrinsic-layout metric is added because no
  multi-pass/intrinsic path exists. If this rung does not introduce one, the
  layout pass-count contract is unchanged; the feature MUST NOT introduce a
  multi-pass path as a side effect of caching.
- **FR-010**: New evidence MUST include a text-heavy `Perf.runScript` scenario
  whose deterministic metric goldens demonstrate cold-frame misses followed by
  warm-frame hits, plus a style-only frame proving zero re-measure / zero
  invalidated / zero miss.

> Interacting / conflicting requirements: **byte-identity (FR-004) vs. caching
> (FR-001)** — the cache may change *how fast* a measurement is produced but never
> *what* value is produced; resolve any divergence in favor of correctness by
> treating the un-cached measurement as the oracle and the cache as a transparent
> accelerator. **Bounded cache (FR-003) vs. high hit rate (FR-005)** — under
> eviction pressure hit rate may fall; the cap (memory bound + deterministic
> eviction) wins, and goldens assert the deterministic outcome at the chosen cap,
> not a maximal hit rate.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This
> section is *expected* to name concrete packages, `.fsi` signatures, build
> targets, and evidence paths.

- **Package impact**: No package identity or dependency change. Affected packable
  libraries are `FS.Skia.UI.Controls` (layout/text-measure path, `Control.fs` /
  `RetainedRender.fs`) and `FS.Skia.UI.Controls.Elmish` (the `FrameMetrics`
  carrier and `Perf.runScript` evidence). Version bump on merge per the standard
  packable-library bump (continuing the `0.1.x-preview.1` series). No Charts
  package migration.
- **Public contract impact**: `FS.Skia.UI.Controls.Elmish`'s `FrameMetrics`
  `.fsi` gains new public integer fields (two text-cache counters + one
  layout-invalidated count) — a public surface addition. This **escalates Route to
  the controls-public-surface gate set** (consistent with 109/110/113/114/116).
  Every `FrameMetrics` construction site (samples, FSI preludes,
  `RefreshSurfaceBaselines`-tracked sites) must add the new fields. The
  text-measure cache itself is internal (no public-surface delta beyond the
  metric carriers, matching the 113/114/116 pattern). XML-doc gate: `///` before
  each new field, with the attribute-before-doc-before-type ordering preserved.
- **State workflow impact**: No change to stateful workflow, I/O, commands,
  effects, subscriptions, or interpreter behavior. The cache is render-time
  internal state with no observable effect ordering.
- **Layout/rendering impact**: Layout text-measurement is accelerated but
  byte-identical; no change to chosen layout boxes, fitted font sizes, DataGrid
  geometry, charts, screenshots, Vulkan/Skia output, or unsupported-environment
  diagnostics. Damage rects / picture cache (116) and virtualization (114) are
  unaffected.
- **Evidence obligations**: `specs/117-layout-hot-path/readiness/**` —
  deterministic `Perf.runScript` metric goldens for the new text-heavy and
  style-only scenarios (FR-010), the focused-gate readiness record, the
  sample-smoke set, the `evidence-audit.md` verdict, and `generated-validation.md`
  (`package-resolution=resolved`, `package-mismatch=false`). Byte-identity proof
  via an always-miss text-cache oracle test.
- **Unsupported scope**: Out of scope — Phase 9 (backend/host-mode review,
  render-thread/compositor work), structural-wrapper flattening that would alter
  control semantics, intrinsic/multi-pass layout introduction, GPU/layer caching,
  and any timing-based pass/fail gate. Structural-wrapper flattening (report
  task 4) is explicitly deferred: it risks semantic change and has no
  byte-identical guarantee, so it is not attempted here.
- **Build-target impact**: Run only what `./fake.sh build -t Route` prints
  (expected: the escalated controls-public-surface set because `FrameMetrics`
  `.fsi` changes). No change to the *definitions* of `Dev`, `TemplateCheck`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`, or
  `EvidenceAudit`; they run as the routed gate set requires.

## Success Criteria *(mandatory)*

- **SC-001**: On a warm text-heavy scripted frame where no text input changed,
  the text-measure cache hit count is greater than zero and the miss count is
  zero (every measured text served from cache).
- **SC-002**: On the cold first frame of the same scenario, the text-measure
  cache reports misses (cold population) and zero hits for first-seen keys.
- **SC-003**: A style-only or visual-state-only frame (hover/focus/press/anim
  tick) over a text-bearing control re-measures zero layout nodes, reports zero
  layout-invalidated nodes, and produces zero text-measure cache misses.
- **SC-004**: For any measurement key, the cached measured width and height equal
  the un-cached (always-miss oracle) result — rendered output and all existing
  metrics are byte-identical to the pre-feature baseline across the whole
  evidence corpus.
- **SC-005**: The text-measure cache never exceeds its fixed capacity, and a
  scenario that measures more distinct strings than the cap completes with
  bounded memory and a deterministic, reproducible hit/miss/eviction sequence.
- **SC-006**: A geometry-changing frame reports a layout-invalidated node count
  that is bounded, explainable, and **less than or equal to** the re-measured node
  count (the pre-pinning dirty set is a subset of the post-pinning re-measured
  boundary subtrees; direction corrected 2026-06-13), giving deep-layout scenarios
  clear dirty-propagation counts.
- **SC-007**: The feature merges with the routed controls-public-surface gate set
  and the evidence audit passing with zero synthetic tasks.
