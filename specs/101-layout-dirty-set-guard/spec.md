# Feature Specification: Layout Dirty-Set Anti-Drift Guard

**Feature Branch**: `101-layout-dirty-set-guard`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: "create the next part of docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md" — selected rung **R7 (Layout dirty-set anti-drift guard, hardens R2)** from §11.4 of the roadmap.

## Context & Source

This feature implements **R7** from the roadmap's second-pass audit
(`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, §11.4).
R7 is one of three follow-ups (R6/R7/R8) the second-pass audit added after R1–R5
(features 096–100) landed. It is **architecture-preserving and non-goal-preserving**:
it hardens a correct-but-fragile invariant inside the feature-097 (R2) incremental
layout path. It introduces **no** data binding, dependency properties, CSS selectors,
or template engine, and changes **no** observable rendering output.

R7 was chosen as the next rung to build (maintainer decision, 2026-06-11); §11.6
recommends the low-risk hardening rungs ({R7, R8}) before the one behavior-changing
rung (R6).

### The problem in one paragraph

Feature 097 (R2) made per-frame layout incremental: a reconcile patch is classified
into a *dirty set* of nodes that must be re-measured, and everything else reuses the
previous frame's cached bounds. A node is treated as layout-dirty when its update
touches a **layout-affecting attribute**. Today that set is a hand-maintained literal,
`layoutAffectingAttrNames = Set.ofList [ "width"; "height"; "orientation" ]`
(`src/Controls/Control.fs:1207`), sitting next to the layout-lowering function
`toLayout` (`:1209`–`1229`) that actually reads those three attribute names. The
classifier is **correct today only by coincidence of maintenance**: `toLayout` derives
geometry from exactly those three names (gap/padding are hardcoded constants, `Wrap` is
`Kind`-driven so a `Kind` change is already a `Replace`, and min/max/flex/align are
never read from attributes). A source comment even *claims* the set is "single-sourced …
the same ground truth `toLayout` consumes" — but it is not single-sourced; it is two
independent lists that happen to agree, with nothing enforcing the agreement.

If a future feature exposes an **attribute-driven** layout input (e.g. an
attribute-driven padding, gap, flex, or align surface), `toLayout` would start reading
a new attribute name, but `layoutDirtySet`
(`src/Controls/RetainedRender.fs:244`–`303`) would still classify a change to that new
attribute as content-only — and **reuse stale cached bounds**, silently producing a
wrong layout on the live path — unless the author *remembers* to extend the
hand-maintained list (or tag the attribute `AttrCategory.Layout`, which the classifier
honors but which no attribute uses today). That latent missed-re-measure is the risk
R7 removes, **without changing any current behavior**.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A layout input cannot be added without updating the dirty classifier (Priority: P1)

A framework contributor extends the controls layer so that a new attribute drives
layout (for example, an attribute-driven `padding` read by the layout lowering). With
R7 in place, doing so **without** also teaching the incremental dirty classifier about
that input causes a **fast, explicit validation failure** at build/test time that names
the un-covered layout input — rather than shipping a silent stale-bounds bug that only
surfaces as a mis-rendered live frame.

**Why this priority**: This is the entire point of R7 — converting a
correct-but-unguarded invariant into an enforced one. It is the highest-value,
must-have outcome.

**Independent Test**: In a test fixture, simulate a layout input that the lowering
reads but the classifier does not cover (or vice-versa) and confirm the enforcement
check fails with a message identifying the drift. Reverting the simulated drift makes
the check pass. This is fully testable in isolation of US2/US3.

**Acceptance Scenarios**:

1. **Given** the layout lowering reads attribute name `N` to derive geometry, **When**
   `N` is absent from the incremental dirty classifier's covered set, **Then** the
   enforcement check fails and names `N` as an un-covered layout input.
2. **Given** the dirty classifier covers a name `M` that the layout lowering does *not*
   read, **When** the enforcement check runs, **Then** it fails and names `M` as an
   over-broad classifier entry (a wasted-re-measure drift).
3. **Given** an attribute tagged with the `Layout` category, **When** a change
   sets/removes it, **Then** the classifier dirties the node, and the enforcement check
   asserts that this honoring holds.
4. **Given** the lowering and classifier are in agreement (the shipping state), **When**
   the enforcement check runs, **Then** it passes.

### User Story 2 - The classifier and the lowering share one definition (Priority: P2)

A contributor adding a genuinely layout-driving attribute updates **one** place — the
single shared definition of layout-driving attribute names — and both the layout
lowering and the incremental dirty classifier pick up the change by construction. There
is no second list to remember to hand-sync.

**Why this priority**: Single-sourcing is the structural mechanism that makes US1's
guarantee cheap to keep true. It is the "make the comment's claim actually true" part
of R7. Important, but the enforcement (US1) is the safety net even if single-sourcing
is imperfect, so this is P2.

**Independent Test**: Trace the attribute names the layout lowering reads and the names
the dirty classifier keys on back to a single shared source; confirm there is no
independent second literal that could drift. Adding a name to the shared source updates
both consumers without any other edit.

**Acceptance Scenarios**:

1. **Given** the shared source of layout-driving attribute names, **When** a name is
   added to it, **Then** both the layout lowering and the dirty classifier reflect the
   addition with no other edit.
2. **Given** the codebase after R7, **When** searching for hand-maintained literals of
   layout-driving attribute names, **Then** exactly one authoritative definition exists.

### User Story 3 - Current behavior and determinism are fully preserved (Priority: P1)

A consumer's running app behaves **exactly** as it did before R7: incremental layout
still produces bounds byte-identical to a full evaluation, a content-only / style /
state / visual-state edit still re-measures nothing extra, and the existing
incremental-equals-full equivalence property still holds. R7 is a hardening change with
**zero** behavioral delta.

**Why this priority**: A hardening change that perturbs behavior would be a net
regression. Preserving R2's invariants is a must-have gate on R7, equal in priority to
US1.

**Independent Test**: Run the existing R2 evidence — the incremental-≡-full byte-identity
property over ≥1000 randomized edit sequences and the `WorkReductionRecord`
content-only-edit metric — and confirm both are unchanged by R7.

**Acceptance Scenarios**:

1. **Given** a randomized sequence of control-tree edits, **When** layout is evaluated
   incrementally vs fully, **Then** the resulting bounds are byte-identical (R2's INV-1
   continues to hold).
2. **Given** a content-only / style / state / visual-state edit, **When** the frame is
   rendered, **Then** the count of re-measured nodes is the same as before R7 (no extra
   re-measure introduced).

### Edge Cases

- **Attribute removal of a layout name** (`AttrRemoved`): the classifier already dirties
  on removal of a layout-driving name; R7 must keep this honored and covered by the
  enforcement, including the category-recovered-from-prev-node path.
- **Category-tagged but not name-listed**: an attribute tagged `Layout` that is *not* in
  the name set must still dirty (the category channel), and the enforcement asserts the
  two channels (name-set equality and category honoring) independently — name-set
  equality does not require the category-only attribute to be in the name list.
- **Over-coverage drift** (a non-layout name listed): must be caught as a perf/wasted
  re-measure drift, not silently tolerated — the two sets must be **exactly equal**, not
  merely "lowering ⊆ classifier".
- **`Kind`-driven layout (e.g. `Wrap`)**: a `Kind` change is a `Replace` (already
  dirty), so `Kind`-driven layout inputs are intentionally *not* attribute-name inputs;
  the enforcement must not demand `Kind`-driven inputs appear in the attribute-name set.
- **Hardcoded constants (gap/padding today)**: values not read from attributes are not
  attribute-name inputs and must not be required in the set — until/unless a future
  change makes them attribute-driven, at which point US1's guard fires.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The set of attribute names the incremental-layout dirty classifier keys on
  MUST be derived from, or gated against, the same single source of attribute names the
  layout lowering reads, so the two cannot silently diverge.
- **FR-002**: The build (a required gate or test) MUST fail when an attribute name that
  drives the layout lowering is **not** represented in the dirty classifier's covered
  set — i.e. an un-covered layout input cannot ship silently.
- **FR-003**: The build MUST also fail on **over-coverage** — a name in the classifier's
  set that the layout lowering does not read — so the two sets are enforced to be
  **exactly equal** (under-coverage risks stale bounds; over-coverage wastes
  re-measures).
- **FR-004**: The dirty classifier MUST continue to honor any attribute tagged with the
  `Layout` category (independent of the name set), and the enforcement MUST assert that
  this honoring holds, so a future category-tagged attribute needs no edit to the name
  set.
- **FR-005**: The change MUST NOT alter incremental-layout results: evaluated bounds
  remain byte-identical to a full layout evaluation, preserving feature-097 (R2)
  invariant INV-1 over the existing ≥1000-case randomized equivalence property.
- **FR-006**: A content-only / style / state / visual-state edit MUST NOT cause any
  additional re-measure relative to the pre-R7 baseline (the `WorkReductionRecord`
  re-measure count for such edits is unchanged).
- **FR-007**: The enforcement check MUST produce a human-legible failure that names the
  specific drifting attribute name(s) and direction (un-covered vs over-broad), so a
  contributor is pointed at the fix rather than left to diagnose a stale-bounds symptom.
- **FR-008**: The optional **intrinsic-size memo** named in roadmap §10.4 (R2 shipped a
  computed-`Bounds` cache only) is OUT of default scope for R7. R7 MUST record an
  explicit decision — defer it, or land it only if a measured workload shows the
  boundary re-measure is hot — and the §10.4 wording reconciliation is delegated to R8.
- **FR-009**: R7 MUST preserve all permanent non-goals: it introduces no data binding,
  no observable/dependency-property graph, no CSS-selector engine, and no template
  engine. The change is internal wiring + an enforcement gate only.

> Interacting / conflicting requirements: FR-003 (sets exactly equal) and FR-004
> (category honoring is an independent channel) could appear to conflict — a
> category-tagged attribute that is *not* in the name set must NOT trip the over-coverage
> failure. Resolution: the **name-set equality** of FR-002/FR-003 is asserted between the
> layout lowering's name-reads and the classifier's name set only; the **category channel**
> of FR-004 is asserted separately. A category-only attribute participates in the category
> assertion, never the name-set equality. Different implementers must treat these as two
> distinct, independently-asserted invariants.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section
> names concrete packages, `.fsi`/source surfaces, build targets, and evidence paths
> deliberately.

- **Package impact**: Internal change to **FS.Skia.UI.Controls** (the
  `layoutAffectingAttrNames` ↔ `toLayout` coupling in `src/Controls/Control.fs`, the
  `layoutDirtySet` classifier in `src/Controls/RetainedRender.fs`), and **optionally**
  **FS.Skia.UI.Layout** if the deferred intrinsic-size memo (FR-008) is landed. No
  package identity change; no public package-content change beyond internal wiring.
  Standard post-merge version bump of all packable libs applies (per the merge flow). No
  legacy Charts migration.
- **Public contract impact**: **No public `.fsi` signature change expected.**
  `layoutAffectingAttrNames` lives in the internal `ControlInternals` module and
  `layoutDirtySet` is `internal`; the single-sourcing may move a *per-package internal*
  surface baseline (recapture via `PerPackageSurface.captureCurrent`) but changes no
  documented public API or sample contract. If the shared definition is introduced as a
  new internal type/member, the per-package internal `.fsi.txt` snapshot is recaptured.
- **State workflow impact**: None. No commands, effects, subscriptions, interpreters, or
  stateful-workflow behavior change.
- **Layout/rendering impact**: Layout **classification internals** change; rendering
  **output is byte-identical** (R2 INV-1 preserved). No charts/DataGrid/Vulkan/Skia/
  screenshot/visual change and no unsupported-environment diagnostic change.
- **Evidence obligations**: (1) the enforcement gate's **negative** evidence — a
  deliberately-introduced name-set drift fails the check and a category-honoring
  violation fails the check; (2) the R2 **incremental-≡-full** byte-identity property over
  ≥1000 randomized edit sequences still green; (3) the `WorkReductionRecord`
  content-only-edit re-measure count unchanged; (4) standard `EvidenceGraph` +
  `EvidenceAudit` artifacts with a verdict token, no synthetic tasks.
- **Unsupported scope**: collection virtualization (a later layer, per §6.2); the full
  intrinsic-size memo unless profiling shows the boundary re-measure is hot (FR-008); the
  R8 documented-narrowing reconciliations (the §10.4 cache wording, the Yoga
  point-scale-rounding rationale) — those are R8, a separate feature; any behavior change
  to dispatch, animation, navigation, or styling.
- **Build-target impact**: Run `./fake.sh build -t Route` first; this is a
  framework-internal `src/Controls/**` change that, on its own, routes to the
  **inner-loop** tier (`Dev`). **Decision deferred to plan**: if the enforcement lands as
  an ordinary Controls/Layout unit/property test, only `Dev` runs and no governance
  registration is needed; if it lands as a **new FAKE/Governance gate**, that gate MUST be
  registered (`AgentValidation.knownGates`) and the generated `validation.contract.yml`
  regenerated (`TargetMetadataDrift`), escalating the route. A `Control.fsi`/internal
  per-package surface move escalates per the standard rule — confirm via `Route`.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of drift attempts are caught: introducing an attribute-driven layout
  input without updating the classifier, **or** a classifier entry the lowering does not
  read, causes a build/test failure that names the offending attribute — verified by a
  negative test for each direction.
- **SC-002**: There is exactly **one** authoritative definition of layout-driving
  attribute names; the layout lowering and the incremental dirty classifier both resolve
  to it, with **zero** independent hand-maintained second lists (verified by inspection
  and by the single-edit-updates-both test).
- **SC-003**: A content-only / style / state / visual-state edit re-measures the same
  number of nodes as before R7 (no extra re-measure), confirmed by the
  `WorkReductionRecord` metric being unchanged for such edits.
- **SC-004**: The incremental-layout result is byte-identical to a full evaluation over
  ≥1000 randomized edit sequences (R2's equivalence property continues to pass
  unchanged).
- **SC-005**: No public surface or consumer-observable behavior changes: no public `.fsi`
  signature change, parity/golden evidence unchanged, and the standard gate set for the
  routed tier is green with `EvidenceAudit` reporting no synthetic work.
- **SC-006**: The intrinsic-size-memo decision (FR-008) is explicitly recorded (deferred
  or landed-with-justification) in the feature's artifacts, so roadmap §10.4's wording can
  be reconciled by R8 without ambiguity.

## Assumptions

- The current 3-name set (`width`, `height`, `orientation`) is the correct, complete set
  of attribute-driven layout inputs **today**; R7 enforces that truth rather than
  expanding it. No new layout input is added by R7 itself.
- The `AttrCategory.Layout` channel is already honored by `layoutDirtySet` and remains
  the forward-compatible path for a future *categorized* layout attribute; R7 asserts the
  honoring but does not require any attribute to adopt the category now.
- "Single-sourced" is satisfied either by deriving the classifier set from the lowering's
  name-reads, or by a shared table both consume, or by a build-time equality gate between
  the two — the plan selects the mechanism; the spec requires only that drift becomes
  impossible-to-ship (enforced), not a specific representation.
- The enforcement check is expected to be a deterministic, in-process test/gate (no
  wall-clock, no external process), consistent with the determinism constitution and the
  R2 evidence style (e.g. `Check.One`, not a repo-absent `testProperty`).
- The optional intrinsic-size memo, if profiled and deferred, is recorded as a decision;
  if landed, it is keyed by retained identity per §10.4 and gated by the same
  incremental-≡-full property.

## Out of Scope

- The R6 visual-state cross-fade and the R8 documented-narrowing reconciliations (the
  §10.4 cache wording fix, the Yoga point-scale-rounding rationale comment, the R1/R5
  surface notes) — separate features.
- Expanding the set of layout-driving attributes (adding attribute-driven padding/gap/
  flex/align) — R7 guards against *un-guarded* additions; it does not make them.
- Collection virtualization and any general intrinsic-measure caching beyond the optional,
  decision-gated memo of FR-008.
- Any change to dispatch (R3), animation (R4/R6), navigation (R5), or the style resolver
  (E3/R1).
