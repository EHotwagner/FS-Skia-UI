# Feature Specification: View Memoization and Stable Dependency Contracts (Control-Internal Memoization + Stability Diagnostics)

**Feature Branch**: `113-view-memoization`
**Created**: 2026-06-12
**Status**: Draft
**Input**: User description: "do next part."

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **next part** of that report's staged plan after feature 112
(which delivered Phase 4: narrow runtime visual-state stamping) — namely **Phase 5:
View Memoization and Stable Dependency Contracts**, also the report's "Do next"
priority **#2** ("Add stable-dependency diagnostics and control-owned memoization").
Everything from Phase 6 onward (viewport virtualization, paint/damage caches, layout
caches, backend review) remains **out of scope** — see *Unsupported scope*.

## Why this feature (context)

Features 109–112 hardened the retained hot path: honest frame metrics + a perf corpus
(109), retained pointer routing (110), a frame scheduler that skips `host.View` on a
model-unchanged frame (111), and a targeted runtime visual-state stamp that no longer
walks the whole tree (112). What remains on the report's "Do next" list is the other
half of cross-framework practice: **stop recomputing expensive pure control subtrees
when their declared inputs have not changed**, and **make the unstable inputs that
defeat reuse visible**.

The report (Phase 5 goal) is explicit: "Avoid recomputing expensive pure control
subtrees when their declared dependencies have not changed." Two concrete problems it
names:

1. **No memoization boundary.** Even with a perfect keyed diff, the framework still
   re-lowers a control's subtree every frame the control is rebuilt — including
   expensive, purely-derived transforms such as DataGrid row/column projection and
   per-control style resolution — regardless of whether the inputs that determine that
   subtree changed. React's `memo`/`useMemo`, Compose's `remember`/skipping, and
   SwiftUI's dependency-local bodies all exist to cut exactly this work.
2. **Unstable inputs are invisible.** The report warns (echoing React's "one
   always-new prop defeats memoization") that an always-new attribute, event closure,
   rebuilt row list, or unstable key silently makes the retained path unable to skip
   work — and today nothing in the framework surfaces that this is happening.

This feature adds (a) a **control-internal memoization seam** keyed by a stable
identity (`ControlId`) plus a caller-supplied **dependency value**, which reuses the
prior lowered subtree when the dependency compares equal and never hides a semantic
change; (b) its application to a **representative** expensive control-internal
transform, proven **byte-identical** to the non-memoized result; (c) **public
`MemoHitCount` / `MemoMissCount`** frame metrics (golden-asserted via the deterministic
`Perf.runScript` corpus); and (d) a **stable-dependency diagnostic** report that flags
always-new attributes/events that break equality, plus an author-facing stable-props
guidance page.

Per the report's "Prefer high-level control-internal memoization first" and "No
correctness depends on memoization," this feature deliberately keeps the memoization
seam **internal / control-owned** — it does **not** add a public consumer-facing
`Control.memo` / `Widget.memo` primitive (that is deferred). Memoization is a pure
performance optimization: removing it would change timing/metrics only, never output.

## Clarifications

### Session 2026-06-12

- Q: Should this feature ship a public consumer-facing `Control.memo` / `Widget.memo`
  primitive, or only a control-internal memoization seam? → A: **Control-internal
  first.** Ship an **internal** memoization seam keyed by `ControlId` + a
  caller-supplied dependency value, applied control-internally (a representative
  expensive transform — DataGrid row/column projection and/or `Style.resolve`), proven
  byte-identical. **No** new public `Control.memo` / `Widget.memo` consumer API this
  rung (deferred). Lower contract risk; matches the report's "prefer control-internal
  memoization first."
- Q: How should `MemoHitCount` / `MemoMissCount` be surfaced — public golden-asserted
  `FrameMetrics` fields (like 109/110/111) or internal counts (like 112's
  `RuntimeStateTouchedNodeCount`)? → A: **Public `FrameMetrics` fields.** Unlike 112's
  runtime-state stamp (live-host only), control-internal memoization runs on the
  **deterministic `Perf.runScript` render path**, so the counts are reproducible in the
  corpus and golden-assertable. This is a **breaking `ControlsElmish.fsi` `FrameMetrics`
  change** (two new fields) and incurs corpus-golden churn, accepted to keep the metric
  observable and regression-proof — matching how 109/110/111 added deterministic metric
  fields.
- Q: How far should the stable-dependency diagnostics go this feature — a report tool,
  or an enforced governance gate? → A: **Report/diagnostic tool only.** Ship a
  stability-diagnostic report function (in the analogue of 101's `layoutDriftReport`)
  that identifies always-new attributes/events on a control tree, asserted in
  `Controls.Tests`, plus the author-facing stable-props guidance doc. It is **NOT**
  wired as an enforced CI gate — consumers legitimately use event closures, so failing
  the build on them would be too aggressive this rung; an enforced gate may come later.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - An expensive control-internal transform is reused when its inputs are unchanged (Priority: P1)

A framework maintainer renders a frame in which a control whose subtree is derived
from an expensive pure transform (e.g. a DataGrid projecting rows/columns, or a
control resolving its style) is rebuilt, but the inputs that determine that transform
did not change since last frame. Today the transform re-runs every such frame. After
this feature, the memoization seam — keyed by the control's stable `ControlId` and a
caller-supplied dependency value — returns the previously-lowered subtree unchanged,
and the frame records a memo **hit** instead of recomputing.

**Why this priority**: This is the report's Phase 5 headline and the load-bearing
mechanism. It is independently valuable: even with no other change, a control whose
declared inputs are stable across frames stops paying for its derivation each frame.

**Independent Test**: Render the same model twice through the deterministic
`Perf.runScript` path for a scenario containing a memoizable control whose dependency
is unchanged across the two frames; assert the second frame records a memo hit
(`MemoHitCount > 0`, `MemoMissCount` for that site `0`) and that the produced subtree
is reused (reference-equal where the seam guarantees reuse), while the rendered scene
is byte-identical to the non-memoized build.

**Acceptance Scenarios**:

1. **Given** a memoizable control whose dependency value is **equal** to the prior
   frame's, **When** the frame is built, **Then** the prior lowered subtree is reused
   and the frame records a memo **hit** for that site.
2. **Given** a memoizable control whose dependency value **differs** from the prior
   frame's, **When** the frame is built, **Then** the subtree is recomputed and the
   frame records a memo **miss** for that site.
3. **Given** a first/cold frame (no prior subtree for the identity), **When** the frame
   is built, **Then** it is a memo **miss** (nothing to reuse) and the subtree is
   computed normally.

---

### User Story 2 - Memoized output is byte-identical to the non-memoized build (Priority: P1)

A maintainer must trust that memoization changed *nothing* observable. For every frame,
the rendered scene produced **with** the memoization seam active must be byte-identical
to the scene produced **without** it (memoization disabled / always-miss). A dependency
value that is wrong (too coarse) must surface through a test or diagnostic, never as a
stale frame.

**Why this priority**: A faster-but-stale frame is unacceptable — "No correctness
depends on memoization." Scene parity (memo-on vs memo-off) is the gate that lets the
optimization land. P1 because the mechanism (US1) cannot be accepted without it.

**Independent Test**: For a representative set of scenarios and frame sequences,
produce each frame's scene with memoization active and with it disabled (forced
always-miss); assert the rendered scenes are equal frame-for-frame. Include a scenario
that mutates the memoized control's real inputs and assert the memoized build reflects
the change (no staleness) — i.e. the dependency value is correct.

**Acceptance Scenarios**:

1. **Given** any scenario in the corpus, **When** each frame is built with memoization
   active and with it disabled, **Then** the rendered scenes are byte-identical
   frame-for-frame.
2. **Given** a memoized control whose real inputs change, **When** the next frame is
   built, **Then** the memoized build reflects the change (a miss occurs; no stale
   subtree is reused).
3. **Given** memoization is disabled entirely, **When** the corpus runs, **Then** every
   rendered scene is unchanged from the pre-feature baseline (memoization is purely
   additive).

---

### User Story 3 - Memo work is observable as deterministic metrics (Priority: P2)

A maintainer needs `MemoHitCount` and `MemoMissCount` in the per-frame metrics so a
regression that defeats reuse (e.g. an always-new dependency) shows up as misses
instead of silently costing CPU.

**Why this priority**: The report requires tracking `MemoHitCount`/`MemoMissCount`.
P2 because it hardens and proves US1/US2 rather than delivering the mechanism itself.

**Independent Test**: Run a corpus scenario that holds a memoized control's inputs
stable across frames and assert hits accrue on the steady-state frames; run a variant
that perturbs the inputs every frame and assert misses accrue. Both counts are
deterministic and golden-asserted.

**Acceptance Scenarios**:

1. **Given** a steady-state frame where a memoized control's dependency is unchanged,
   **When** the frame is built, **Then** `MemoHitCount` increments for that site and
   `MemoMissCount` does not.
2. **Given** a frame where a memoized control's dependency changed (or a cold first
   frame), **When** the frame is built, **Then** `MemoMissCount` increments for that
   site.
3. **Given** an idle frame with no memoizable control evaluated, **When** the frame is
   recorded, **Then** both counts are `0` (no spurious memo accounting).

---

### User Story 4 - Unstable inputs that defeat reuse are diagnosable (Priority: P2)

A maintainer (or control author) needs to find the always-new attributes/events that
break equality — `UntypedValue`s rebuilt each frame, event closures, rebuilt row
lists, unstable keys — because those silently make memoization (and the keyed diff)
unable to skip work. This feature adds a **stability-diagnostic report** that, given a
control tree built across two frames, identifies the attributes/events that compared
unequal despite no semantic change, plus an author-facing **stable-props guidance**
page.

**Why this priority**: Diagnosability is the report's companion deliverable to
memoization ("Add diagnostics that identify always-new attributes/events that break
equality"). P2 because it supports the optimization rather than being the optimization;
it is a report tool, **not** an enforced gate this rung (clarified 2026-06-12).

**Independent Test**: Build a fixture tree twice — once with a stable attribute set and
once with an injected always-new attribute/closure — and assert the stability-diagnostic
report flags the unstable input in the second case and reports nothing in the first.

**Acceptance Scenarios**:

1. **Given** a tree whose attributes/events are stable across two builds, **When** the
   stability-diagnostic report runs, **Then** it reports **no** instability findings.
2. **Given** a tree with an always-new attribute/event closure across two builds,
   **When** the report runs, **Then** it flags that attribute/event as a reuse-breaking
   instability, naming the control and the input.
3. **Given** the stable-props guidance page, **When** an author reads it, **Then** it
   names the concrete reuse-breaking patterns (rebuilt `UntypedValue`, per-frame
   closures, rebuilt lists, unstable keys) and how to make them stable.

---

## Requirements *(mandatory)*

### Functional Requirements

**Memoization seam (Phase 5 core)**

- **FR-001**: The framework MUST provide a **control-internal** memoization seam that,
  keyed by a control's stable `ControlId` plus a **caller-supplied dependency value**,
  reuses the previously-lowered subtree for that identity when the dependency value
  compares **equal** to the prior frame's, and recomputes (a miss) otherwise.
- **FR-002**: The seam MUST be **internal / control-owned**. This feature does **NOT**
  add a public consumer-facing `Control.memo` / `Widget.memo` primitive (deferred,
  clarified 2026-06-12). The seam is consumed by control internals (not authored by app
  consumers).
- **FR-003**: The seam MUST be applied to at least one **representative** expensive
  control-internal transform — DataGrid row/column projection and/or `Style.resolve` —
  proving the mechanism on a real high-value site rather than a synthetic one.
- **FR-004**: A memo **hit** MUST reuse the prior lowered subtree (reference-equal where
  the seam guarantees reuse) without re-running the memoized transform; a **miss** MUST
  recompute it and store the result keyed by identity + dependency for the next frame.
- **FR-005**: The dependency value MUST be a **deterministic** value supplied by the
  caller (the control internal), not an object-identity accident. Equality of dependency
  values MUST be the sole reuse condition; the seam MUST NOT reuse across unequal
  dependencies.

**Correctness & parity (Phase 5 invariants)**

- **FR-006**: Memoization MUST be a **pure performance optimization**: for every frame,
  the rendered scene produced with the seam active MUST be **byte-identical** to the
  scene produced with memoization **disabled** (forced always-miss). No correctness may
  depend on memoization.
- **FR-007**: The seam MUST NOT hide a **semantic change**: when a memoized control's
  real inputs change, the dependency value MUST change too, producing a miss and a fresh
  subtree (no staleness). A dependency value that is too coarse MUST be caught by the
  memo-on/memo-off parity test (FR-006), not shipped as a stale frame.
- **FR-008**: Memoization MUST be safely disableable (e.g. an internal switch / always-
  miss mode) so the parity oracle in FR-006 can run, and so memoization can be turned
  off with **zero** change to rendered output.

**Observability (Phase 5 metrics)**

- **FR-009**: The framework MUST expose deterministic `MemoHitCount` and `MemoMissCount`
  per-frame metrics counting memo hits and misses that occurred building that frame.
  Both are **public `FrameMetrics` fields** (clarified 2026-06-12), reproducible and
  **golden-asserted** via the `Perf.runScript` corpus (memoization runs on the
  deterministic render path). An idle frame that evaluates no memoizable control reports
  both as `0`.
- **FR-010**: A steady-state frame whose memoized control inputs are unchanged MUST
  report hits (and no miss for those sites); a frame whose inputs changed, or a cold
  first frame, MUST report the corresponding misses — so a regression that defeats reuse
  is visible as misses in the goldens.

**Stability diagnostics (Phase 5 diagnosability)**

- **FR-011**: The framework MUST provide a **stability-diagnostic report** that, given a
  control (sub)tree evaluated across two frames, identifies attributes/events that
  compared **unequal** despite no semantic change — the always-new inputs that defeat
  reuse (rebuilt `UntypedValue`, per-frame event closures, rebuilt lists, unstable
  keys) — naming the control and the offending input.
- **FR-012**: The stability-diagnostic report MUST be a **report/diagnostic tool**
  asserted in `Controls.Tests`, **NOT** an enforced CI gate this feature (clarified
  2026-06-12). It MUST report **no** findings for a tree whose inputs are stable across
  builds, and flag the instability for a tree with an injected always-new input.
- **FR-013**: The framework MUST ship an author-facing **stable-props guidance** page
  documenting the concrete reuse-breaking patterns and how to make inputs stable.

**Behaviour preservation (cross-cutting)**

- **FR-014**: This feature is **additive performance + diagnostics only**. At-rest
  rendered output, control geometry, focus/keyboard routing semantics, and every
  dispatch outcome MUST remain **byte-identical** to the pre-feature state. The only
  intended observable changes are (a) reused subtrees on memo hits, (b) the new
  `MemoHitCount`/`MemoMissCount` `FrameMetrics` fields, and (c) the new
  stability-diagnostic report + guidance doc.
- **FR-015**: Feature 112's targeted runtime stamp, feature 111's scheduler/view-skip,
  feature 110's retained routing, and the retained render pipeline are unchanged; this
  feature only adds a memoization boundary inside the existing control-lowering path and
  the two metric fields.

> Interacting / conflicting requirements:
> - **FR-001/FR-004 (reuse the prior subtree on a dependency match) vs FR-006/FR-007
>   (byte-identical, no staleness)** — resolution: reuse is permitted **only** when the
>   caller-supplied dependency value is equal, and the dependency value is defined to
>   capture **every** input that can change the memoized subtree. Any input change
>   changes the dependency → a miss → a fresh subtree. The memo-on/memo-off parity test
>   (FR-006) is the authority that a dependency value is not too coarse; a stale frame
>   is a failing test, never shipped. When in doubt the seam misses (recomputes) — it
>   never reuses across an unequal or unknown dependency.
> - **FR-009 (`MemoHitCount`/`MemoMissCount` golden-asserted) vs the live host** —
>   resolution: unlike 112's runtime-state stamp (live-only), control-internal
>   memoization runs on the **deterministic `Perf.runScript` render path**, so the
>   counts are reproducible there and are the authoritative golden evidence; the live
>   `OnFrameMetrics` sink reports the same fields. The counts therefore harden without
>   depending on a live window.
> - **FR-002 (no public memo primitive) vs the report's "Consider Control.memo/Widget.memo"**
>   — resolution: the report says *consider* the primitive and *prefer control-internal
>   memoization first*; this feature ships the control-internal seam and **defers** the
>   public consumer primitive to a later rung, keeping the public contract minimal and
>   the byte-identity proof tractable.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls` — a new **internal** memoization
  seam (consumed by control internals such as `DataGrid` row/column projection and/or
  `Style.resolve`), and a **public** stability-diagnostic report constructor in the
  existing `Diagnostics` module — and `FS.Skia.UI.Controls.Elmish` — the `FrameMetrics`
  record gains the public `MemoHitCount`/`MemoMissCount` fields, threaded from the
  retained step / control-lowering path and surfaced through `Perf.runScript` and the
  live `OnFrameMetrics` sink. No package identity changes; package **contents** change
  and versions bump on merge. The memoization seam itself is **internal** (tests reach
  it via `InternalsVisibleTo`); the metric fields and the diagnostic report are public.
- **Public contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change —
  two new public fields (`MemoHitCount`, `MemoMissCount`), so the top-level surface
  baseline changes (precedent: 109/110/111 each added `FrameMetrics` fields). A new
  **public** stability-diagnostic constructor/`val` in `Controls` `Diagnostics.fsi`
  (returning `ControlDiagnostic`s), and a new **internal** memoization seam `val
  internal` in the owning `Controls` `.fsi` (consumed by control internals). No public
  consumer `Control.memo`/`Widget.memo` primitive (deferred, clarified 2026-06-12).
  `Route` is expected to escalate to the **controls-public-surface** tier because the
  Controls package `.fsi` surface changes; run `Route` first and obey its printed list.
- **State workflow impact**: None to MVU semantics — `Update`, effects, subscriptions,
  commands, and interpreter behaviour are unchanged. Dispatch *outcomes* are
  byte-identical (FR-014); only *whether a pure subtree is recomputed or reused*
  changes.
- **Layout/rendering impact**: None to rendered output — at-rest scene, geometry, and
  the retained step are byte-identical (FR-006/FR-014). Memoization reuses a subtree
  that is **structurally equal** to the one the non-memoized path would lower; it
  changes *whether the transform re-runs*, not *what is drawn*. No Vulkan/Skia/visual-
  output change; no unsupported-environment diagnostic change. (A representative memoized
  site MAY be a DataGrid-internal transform; DataGrid rendered output stays byte-
  identical.)
- **Evidence obligations**: memo-on/memo-off scene-parity evidence over the corpus
  (FR-006) including a real-input-change no-staleness case (FR-007); `MemoHitCount`/
  `MemoMissCount` evidence for a steady-state (hits) vs perturbed/cold (misses) frame
  (FR-009/FR-010); stability-diagnostic evidence (stable tree → no findings; injected
  always-new input → flagged, FR-011/FR-012); the stable-props guidance page (FR-013);
  at-rest byte-identity (the standing Scene-parity golden suite under `Dev`); the
  regenerated `Perf.runScript` corpus goldens carrying the two new metric fields;
  skill-loading evidence; the window-visibility not-applicable set;
  `readiness/evidence-audit.md` with a verdict token; the generated-validation
  package-resolution tokens. The escalated `maintainer-verify` readiness set applies
  because of the Controls `.fsi` change.
- **Unsupported scope**: This feature is **Phase 5 only**. Explicitly OUT: a **public
  consumer `Control.memo` / `Widget.memo`** primitive (deferred); viewport
  **virtualization** for DataGrid/List (Phase 6); damage rectangles / Skia picture /
  paint caches (Phase 7); text-measurement / layout-boundary caches (Phase 8);
  `SkiaViewer` backend / render-thread / compositor review (Phase 9); any **enforced
  stability gate** (the diagnostic is report-only this rung). Full migration of all 52
  controls to memoized transforms is OUT — only a **representative** site is memoized.
  No renderer rewrite, no Avalonia/WPF redesign, no platform/release/distribution scope.
- **Build-target impact**: Escalated to the controls-public-surface set is expected
  because the Controls (and `Controls.Elmish` `FrameMetrics`) `.fsi` surfaces change;
  run `Route` first and obey its printed minimal list (`Dev`, the package/per-package
  surface diffs, `FsiTranscripts`, the controls catalog/doc/interaction/rendering
  checks, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`).
  `RefreshSurfaceBaselines` must regenerate the top-level + per-package baselines after
  the `FrameMetrics`/`Diagnostics`/seam additions, and the `Perf.runScript` corpus
  goldens must be regenerated (`PERF_CORPUS_REGEN=1`) to carry the two new metric
  fields. No new gate (the stability diagnostic is report-only).

## Success Criteria *(mandatory)*

- **SC-001**: A steady-state frame whose memoized control's inputs are unchanged reuses
  the prior lowered subtree and records a memo **hit** (no recompute of the memoized
  transform), observable as `MemoHitCount > 0` / `MemoMissCount = 0` for that site.
- **SC-002**: For **100%** of corpus frames, the scene produced with memoization active
  is byte-identical to the scene produced with memoization disabled (forced always-miss)
  — and to the pre-feature baseline.
- **SC-003**: Changing a memoized control's real inputs produces a miss and a fresh
  subtree in the next frame (no stale reuse); a too-coarse dependency value fails the
  memo-on/memo-off parity test rather than shipping.
- **SC-004**: `MemoHitCount` and `MemoMissCount` are deterministic, golden-asserted
  `FrameMetrics` fields: a steady-state scenario accrues hits, a perturbed/cold scenario
  accrues misses, and an idle frame reports both `0`.
- **SC-005**: The stability-diagnostic report flags an injected always-new attribute/
  event as a reuse-breaking instability (naming the control + input) and reports nothing
  for a stable tree; the stable-props guidance page documents the reuse-breaking
  patterns.
- **SC-006**: At-rest rendered output, control geometry, focus/keyboard routing
  semantics, and all dispatch outcomes are byte-identical to the pre-feature state;
  disabling memoization changes nothing observable except the metric counts.

## Key Entities

- **Memoization seam** (internal, `Controls`): keyed by `ControlId` + a caller-supplied
  deterministic dependency value; reuses the prior lowered subtree on a dependency match
  (hit), recomputes and stores otherwise (miss); never reuses across an unequal/unknown
  dependency. The mechanism this feature adds; consumed by control internals, not by app
  consumers.
- **Dependency value**: the deterministic value a control internal supplies that
  captures every input which can change the memoized subtree; equality of dependency
  values is the sole reuse condition. A too-coarse value is caught by the memo-on/memo-
  off parity test.
- **Representative memoized site** (control-internal): a real expensive transform —
  DataGrid row/column projection and/or `Style.resolve` — memoized to prove the seam on
  a high-value site (not a synthetic one); its rendered output stays byte-identical.
- **MemoHitCount / MemoMissCount**: public, deterministic `FrameMetrics` fields counting
  memo hits/misses while building a frame; golden-asserted via `Perf.runScript`; both
  `0` on a frame that evaluates no memoizable control. The observability surface this
  feature adds (clarified public, unlike 112's internal count, because memoization runs
  on the deterministic render path).
- **Stability-diagnostic report** (public, `Diagnostics`): given a (sub)tree evaluated
  across two builds, identifies attributes/events that compared unequal despite no
  semantic change — the always-new inputs (rebuilt `UntypedValue`, per-frame closures,
  rebuilt lists, unstable keys) that defeat reuse — naming the control + input. A
  report tool asserted in tests, **not** an enforced gate.
- **Stable-props guidance page**: author-facing documentation of the reuse-breaking
  patterns and how to make inputs stable.

## Assumptions

- The memoization cache lives in the existing retained per-identity state carried across
  frames (the same place `RetainedRender` holds prior layout/state), so a hit can reuse
  last frame's lowered subtree keyed by `ControlId`. (The plan phase decides the exact
  storage; the spec requires only that reuse be keyed by stable identity + dependency.)
- "Representative memoized site" means at least one of DataGrid row/column projection or
  `Style.resolve`; the plan picks the highest-value one(s). Full 52-control migration is
  out of scope.
- The memo-on/memo-off parity oracle (FR-008) is an internal test switch; it is not a
  public consumer-facing toggle.
- The stable-props guidance page lives under `docs/` (FSharp.Formatting site) alongside
  the existing controls docs; the plan picks the exact path.
- `MemoHitCount`/`MemoMissCount` are integer counts over a frame; they aggregate across
  all memoized sites evaluated that frame (per-site attribution is available in tests
  via the seam, not in the aggregate metric).
