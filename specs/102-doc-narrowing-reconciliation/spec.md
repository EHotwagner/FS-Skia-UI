# Feature Specification: Documented-Narrowing Reconciliation

**Feature Branch**: `102-doc-narrowing-reconciliation`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: "create next part" — selected the next rung of
`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, namely
**R8 (Documented-narrowing reconciliation; doc/surface, no behavior change)** from §11.5.

## Context & Source

This feature implements **R8** from the roadmap's second-pass audit
(`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, §11.5).
R8 is the last of three follow-ups (R6/R7/R8) the second-pass audit added after R1–R5
(features 096–100) landed; R7 shipped as feature 101. §11.6 recommends the two
low-risk rungs (**{R7, R8} first**) before the one behavior-changing rung (R6), and R7
is already merged — so R8 is the next rung to build (maintainer decision, 2026-06-11).

R8 is a **pure honesty pass**. It is **architecture-preserving and non-goal-preserving**
and, by design, changes **no observable rendering output and no runtime behavior**
(with one *optional*, decision-gated dead-code removal called out below). Every item is
either a wording correction in the roadmap report or a small in-source surface tidy that
makes a doc-comment or a type's advertised capability agree with what actually ships.

### The problem in one paragraph

Several R1/R2/R3/R5 deliverables were written to the *intended* capability, the
implementation honestly **narrowed** them (and usually said so in the `.fsi`/source), but
the roadmap prose — and in a few cases an in-source comment or an advertised role surface —
was never updated to match. None of these is a bug: each is a place where the
**description is slightly broader than the shipped truth**. R8 closes each gap by
reconciling the words to the code (or, for one item, the dead code to the words), so a
future reader of the report or the source is not misled into thinking a capability is
present that is not. There is no functional change to make; the deliverable is *agreement*.

### The six narrowings to reconcile

1. **R1 — `deriveVisualState` order (report wording).** §10.3 describes
   `deriveVisualState` as the full 8-level arbiter (`Disabled > … > Normal`). The function
   implements only the 5-level **runtime tail** (`Pressed > Selected > Focused > Hover >
   Normal`); the head semantic states and the consumer-out-ranks-derived arbitration live
   in `applyRuntimeVisualState`. The `.fsi` already documents the two-function split; §10.3
   does not. **Reconcile §10.3** to describe the split (or fold the full order behind one
   documented entry point).
2. **R1 — dead derived `Selected` (source/surface).** `deriveVisualState` derives a
   `Selected` state from a text-range `Selection` that the live host
   (`ControlsElmish`) **never populates**, so on the real path only a *consumer-set*
   `Selected` ever fires. **Decide and record**: either remove the dead derivation, or
   annotate it explicitly as forward-looking (so a reader knows it does not fire today).
3. **R2 — cache wording (report wording).** §10.4 names a "measured intrinsic size …
   keyed by retained identity". R2 (feature 097) shipped a computed-**`Bounds`** cache
   only, keyed by structural **`LayoutNodeId`**, not an intrinsic-size memo keyed by
   retained identity. **Reconcile §10.4** wording to the shipped cache (R7 already recorded
   the intrinsic-size-memo deferral decision, FR-008 of feature 101).
4. **R2 — Yoga rationale (source comment).** The point-scale-rounding disable in
   `src/Layout/Layout.fs` documents the **correctness** motive (the INV-1 incremental==full
   equivalence) but **not** the maintainer's "blast-radius nil, Controls integer geometry
   unaffected" approval rationale. **Add** that approval rationale to the source comment.
5. **R5 — value-role surface (source/surface + report).** `navIntentFor`
   (`src/Controls/Focus.fs`) classes `Chart`/`Graph`/`Progress` as **value roles**, but
   `Accessibility.defaultFor` gives those roles `Navigation = None` / non-focusable, so they
   **never route on arrow keys by default**; and "segmented" is referenced as a selection
   role although **no `Segmented` `AccessibilityRole` exists**. **Reconcile** by either
   dropping those roles from the value branch with a note **or** documenting why they are
   classed-but-not-routed, and **correct the "segmented" mention**. (Actually *enabling*
   default routing for those roles is a behavior change and is out of R8's default scope —
   see the conflicting-requirements note.)
6. **R3 — preview-path annotation (source).** The lone residual `Key ?? Kind`
   (`src/Controls/Control.fs:1131`, formerly `:1122`) is the legacy 080 single-control
   **preview** path — *not* the dispatch/recovery surface R3 (feature 098) unified onto
   `Key ?? path`. **Annotate** it so a future reader does not mistake it for the divergence
   R3 removed.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The roadmap report matches what ships (Priority: P1)

A maintainer (or a future contributor) reads the controls-architecture-evolution roadmap
to understand the shipped capability surface. After R8, every prose claim that previously
overstated the implementation is corrected: §10.3 describes the **two-function**
visual-state arbitration that actually ships (5-level runtime tail + head/arbitration
split), §10.4 describes the **computed-`Bounds` cache keyed by `LayoutNodeId`** that
actually ships (not an "intrinsic-size memo keyed by retained identity"), and the "segmented"
selection-role mention is corrected to the roles that actually exist. The reader can trust
the report as an accurate map of the code.

**Why this priority**: The roadmap is the canonical narrative artifact the team and audits
rely on; a report that overstates the implementation is the exact defect R8 exists to fix.
This is the must-have outcome.

**Independent Test**: For each reconciled section (§10.3, §10.4, the §11.5/parity "segmented"
and value-role mentions), confirm the prose now matches the cited source (function split,
cache key/type, existing roles). A reviewer reading only the report and the cited source
lines finds no contradiction.

**Acceptance Scenarios**:

1. **Given** roadmap §10.3, **When** it describes `deriveVisualState`, **Then** it states
   the function realizes only the 5-level runtime tail and names `applyRuntimeVisualState`
   as the home of the head semantic states and the consumer-out-ranks-derived arbitration
   (matching the `.fsi`).
2. **Given** roadmap §10.4, **When** it describes the R2 layout cache, **Then** it names a
   computed-`Bounds` cache keyed by structural `LayoutNodeId`, and does **not** claim an
   intrinsic-size memo keyed by retained identity (cross-referencing feature 101's recorded
   deferral).
3. **Given** any roadmap mention of a "segmented" selection role, **When** it is read,
   **Then** it is corrected to the roles that actually exist (no nonexistent `Segmented`
   `AccessibilityRole` is implied).

### User Story 2 - In-source narrowings are honest at the point of use (Priority: P1)

A contributor reading the source encounters each previously-misleading site already
reconciled: the dead `Selected`-from-`Selection` derivation is either gone or annotated as
forward-looking; the Yoga point-scale-rounding disable comment records the maintainer's
blast-radius approval alongside the correctness motive; the `Chart`/`Graph`/`Progress`
value-role classification carries a note explaining it does not route by default (or the
roles are dropped from the value branch); and the residual preview-path `Key ?? Kind`
carries a note that it is the legacy 080 preview path, not the R3-unified dispatch surface.
No contributor is left to rediscover a narrowing the team already knows about.

**Why this priority**: The whole value of R8 is that the next reader does not re-derive
these findings from scratch or, worse, "fix" a deliberate narrowing. Recording them at the
point of use is the must-have, equal in priority to US1.

**Independent Test**: Grep each cited site (`src/Controls/ControlRuntime.fs` Selected
derivation, `src/Layout/Layout.fs` Yoga comment, `src/Controls/Focus.fs` value-role branch,
`src/Controls/Control.fs:1131` preview id) and confirm the annotation/removal is present and
accurate. Each is independently inspectable.

**Acceptance Scenarios**:

1. **Given** the `Selected`-from-`Selection` derivation, **When** the codebase after R8 is
   inspected, **Then** the derivation is either removed **or** carries an explicit
   forward-looking annotation stating the live host does not populate `Selection`.
2. **Given** the Yoga point-scale-rounding disable comment, **When** it is read, **Then** it
   states both the INV-1 correctness motive (already present) **and** the maintainer's
   blast-radius/Controls-integer-geometry approval rationale.
3. **Given** the `navIntentFor` value-role branch, **When** it is read, **Then** the
   `Chart`/`Graph`/`Progress` classification is either removed or carries a note that they
   are not routed by default (because `defaultFor` gives them no `NavRange`).
4. **Given** the residual preview-path `Key ?? Kind`, **When** it is read, **Then** a note
   identifies it as the legacy 080 single-control preview path, distinct from the R3-unified
   `Key ?? path` dispatch/recovery id.

### User Story 3 - Zero behavior, determinism, or non-goal change (Priority: P1)

A consumer's running app, and every existing gate, behaves **exactly** as before R8. No
rendering output changes, no parity/golden evidence moves, no determinism property is
perturbed, and no permanent non-goal (data binding, dependency properties, CSS selectors,
template engine) is introduced. The only surface that may move is the *optional*
dead-derivation removal, which — if taken — is a deliberate, baseline-recaptured surface
tidy with no consumer-observable effect (the derivation never fired on the live path).

**Why this priority**: R8's defining constraint is "no behavior change". A reconciliation
pass that accidentally perturbs behavior would be a regression and would defeat its own
purpose. Preserving every invariant is a must-have gate, equal in priority to US1/US2.

**Independent Test**: Run the routed gate set for the change; confirm parity/golden evidence
is unchanged, the R1/R2/R4/R5 property and unit suites stay green, and — if the
dead-derivation removal is taken — the only surface delta is the recaptured baseline for
that one removal, with no other public `.fsi` signature change.

**Acceptance Scenarios**:

1. **Given** the full reconciliation (wording + comments + annotations), **When** the routed
   gates run, **Then** rendering output, parity evidence, and all existing property/unit
   suites are unchanged.
2. **Given** the value-role reconciliation, **When** it is the documentation/annotation
   option (the default), **Then** arrow-key routing behavior for `Chart`/`Graph`/`Progress`
   is **unchanged** (still not routed by default) — R8 documents the narrowing, it does not
   enable routing.
3. **Given** the optional dead-derivation removal, **When** it is taken, **Then** the only
   surface change is the recaptured baseline for `deriveVisualState`, and no
   consumer-observable visual-state result changes (the derived `Selected` never fired on
   the live host).

### Edge Cases

- **Dead-derivation removal changes a public signature.** If removing the `Selected`-from-
  `Selection` derivation also drops a now-unused parameter/input from a public
  `deriveVisualState` signature, that is a Tier-1 surface change and must recapture the
  affected baseline and route accordingly. The **default** (lower-risk) choice is to
  *annotate* rather than remove, keeping the surface byte-identical; the plan selects.
- **"Reconcile wording vs land the change."** §10.4's intrinsic-size-memo wording could be
  reconciled by *describing the shipped cache* (the R8 default) — landing the memo itself is
  R7/feature-101's already-recorded deferral, **not** R8 work.
- **Value-role: document vs enable.** Giving `Chart`/`Graph`/`Progress` real default
  `NavRange`s would make them route on arrows — a **behavior change** that moves a parity
  row. R8's default is to *document/drop with a note* (no behavior change); enabling routing
  is explicitly out of R8 default scope (would be a separate, evidence-carrying feature).
- **Report is not a shipped package artifact.** The roadmap report is repo documentation,
  not a packaged surface; its edits do not bump package versions or move surface baselines.
  Only the in-source items (comments/annotations, optional removal) can touch a baseline.
- **Annotation must not be load-bearing.** Comments and annotations added by R8 must be
  purely descriptive — no comment may be parsed by a gate as a behavior token in a way that
  changes routing or evidence outcomes.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Roadmap §10.3 MUST be reconciled to describe `deriveVisualState` as realizing
  only the 5-level runtime tail (`Pressed > Selected > Focused > Hover > Normal`), with the
  head semantic states and consumer-out-ranks-derived arbitration attributed to
  `applyRuntimeVisualState` — matching the `.fsi` two-function split (or, alternatively,
  folding the full order behind one documented entry point and describing that).
- **FR-002**: The dead `Selected`-from-`Selection` derivation in `deriveVisualState` MUST be
  either (a) removed, or (b) annotated in source as forward-looking with an explicit note
  that the live host (`ControlsElmish`) does not populate `Selection`, so only consumer-set
  `Selected` fires today. The plan records which option is taken and why; option (b) is the
  default zero-surface-delta choice.
- **FR-003**: Roadmap §10.4 MUST be reconciled to describe the shipped R2 cache — a computed
  `Bounds` cache keyed by structural `LayoutNodeId` — and MUST NOT claim an intrinsic-size
  memo keyed by retained identity, cross-referencing feature 101's recorded intrinsic-size-
  memo deferral (FR-008 of feature 101).
- **FR-004**: The Yoga point-scale-rounding disable comment in `src/Layout/Layout.fs` MUST
  record the maintainer's blast-radius approval rationale ("blast-radius nil, Controls
  integer geometry unaffected") alongside the existing INV-1 correctness motive.
- **FR-005**: The `navIntentFor` value-role classification of `Chart`/`Graph`/`Progress`
  MUST be reconciled with their `Accessibility.defaultFor` non-routing default — either by
  removing them from the value branch with a note, or by annotating that they are
  classed-but-not-routed-by-default — **without** enabling default arrow-key routing for
  them.
- **FR-006**: Every roadmap/source mention of a "segmented" selection role MUST be corrected
  to reflect that no `Segmented` `AccessibilityRole` exists (name the roles that do, or drop
  the mention with a note).
- **FR-007**: The residual preview-path `Key ?? Kind` (`src/Controls/Control.fs:1131`) MUST
  carry a source annotation identifying it as the legacy 080 single-control **preview** path,
  distinct from the R3-unified `Key ?? path` dispatch/recovery id, so it is not mistaken for
  the divergence feature 098 removed.
- **FR-008**: R8 MUST NOT change any observable rendering output, parity/golden evidence,
  determinism property, or runtime behavior. The only permissible surface delta is the
  optional FR-002(a) dead-derivation removal, which — if taken — recaptures the affected
  baseline and is verified to leave every consumer-observable visual-state result unchanged.
- **FR-009**: R8 MUST preserve all permanent non-goals: it introduces no data binding, no
  observable/dependency-property graph, no CSS-selector engine, and no template engine. The
  change is documentation, source comments/annotations, and at most one dead-code removal.
- **FR-010**: Any comment or annotation R8 adds MUST be purely descriptive and MUST NOT be
  interpreted by any governance gate as a status/behavior token that alters routing,
  evidence verdicts, or audit outcomes (e.g. avoid bare gate-significant tokens / literal
  filenames that trip the window-visibility or diff-scan audits).

> Interacting / conflicting requirements: FR-005/FR-006 name *two* ways to reconcile the
> value-role surface — "give default `NavRange`s" vs "document/drop". These pull opposite
> directions on behavior. **Resolution**: R8's banner constraint (FR-008, no behavior change)
> wins — the **default** reconciliation is documentation/drop-with-a-note, which keeps
> arrow-key routing for `Chart`/`Graph`/`Progress` exactly as today (not routed). *Enabling*
> default routing is a behavior change that moves a parity row and is **out of R8 default
> scope**; if a maintainer wants it, it is a separate feature carrying its own golden/parity
> evidence. Likewise FR-002 offers remove-vs-annotate: the default is annotate (zero surface
> delta); removal is permitted only as a deliberate, baseline-recaptured choice recorded in
> the plan.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section
> names concrete reports, source files, `.fsi`/baseline surfaces, build targets, and
> evidence paths deliberately.

- **Package impact**: No package-identity or package-content change from the documentation
  items (the roadmap report is repo docs, not a packaged surface). The in-source items touch
  **FS.Skia.UI.Controls** (`src/Controls/ControlRuntime.fs`, `src/Controls/Focus.fs`,
  `src/Controls/Accessibility.fs`, `src/Controls/Control.fs`) and **FS.Skia.UI.Layout**
  (`src/Layout/Layout.fs`) as comments/annotations (and, if FR-002(a) is taken, one
  removal). The standard post-merge version bump of all packable libs applies per the merge
  flow. No legacy Charts migration.
- **Public contract impact**: **No public `.fsi` signature change expected** under the
  default choices (annotate, not remove). If FR-002(a) (dead-derivation removal) is taken and
  it drops a now-unused input from a public `deriveVisualState` signature, that is the *only*
  public-surface delta — recapture the affected cross-package and per-package surface
  baselines and route Tier-1 accordingly. No sample contract changes.
- **State workflow impact**: None. No commands, effects, subscriptions, interpreters, or
  stateful-workflow behavior change.
- **Layout/rendering impact**: **None observable.** Layout/visual-state/navigation *behavior*
  is unchanged; only descriptive comments and report prose change. No charts/DataGrid/Vulkan/
  Skia/screenshot/visual change and no unsupported-environment diagnostic change.
- **Evidence obligations**: (1) the routed gate set green (the framework-internal source-
  comment items route inner-loop `Dev`; any per-package/public surface move from FR-002(a)
  escalates per the standard rule — confirm via `Route`); (2) parity/golden evidence
  unchanged (no row moves — explicitly *not* the R6 case); (3) the R1/R2/R4/R5 property and
  unit suites still green; (4) standard `EvidenceGraph` + `EvidenceAudit` artifacts with a
  verdict token and no synthetic tasks. The diff against the roadmap report is itself the
  reconciliation evidence for the documentation items.
- **Unsupported scope**: the R6 visual-state cross-fade (a separate, behavior-changing
  feature); *enabling* default navigation routing for `Chart`/`Graph`/`Progress` or adding a
  `Segmented` role (behavior/surface change, separate feature); landing the R2 intrinsic-size
  memo (feature 101's recorded deferral, not R8); any change to dispatch, animation,
  navigation, or styling behavior.
- **Build-target impact**: Run `./fake.sh build -t Route` first. The doc-only roadmap edit
  and the framework-internal `src/**/*.fs` comment/annotation edits route to the **inner-loop**
  tier (`Dev`) on their own; a doc-only subtree edit may trip the doc-rules check (broad
  subtree, per feature 088) — confirm via `Route`. If FR-002(a) moves a public/per-package
  surface, the route escalates (recapture baselines via `PerPackageSurface.captureCurrent`
  and, for the cross-package baseline, the standard capture). No new gate is added by R8.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of the six listed narrowings (R1 order wording, R1 dead `Selected`, R2
  cache wording, R2 Yoga rationale, R5 value-role + "segmented", R3 preview-path annotation)
  are reconciled — each verified by inspecting the cited report section or source site and
  confirming the description now matches the shipped code (or the dead code is removed).
- **SC-002**: A reviewer reading each reconciled roadmap section against its cited source
  lines finds **zero** remaining contradictions between prose and implementation for these
  six items.
- **SC-003**: **No** rendering output, parity/golden evidence, or determinism property
  changes; the R1/R2/R4/R5 property and unit suites are green and unchanged — confirmed by
  the routed gate set with `EvidenceAudit` reporting no synthetic work.
- **SC-004**: Arrow-key routing behavior for `Chart`/`Graph`/`Progress` is **unchanged**
  (still not routed by default) — the value-role item is reconciled by documentation, not by
  enabling routing — verified by the existing navigation suite passing without modification.
- **SC-005**: The public surface is unchanged **except** for the optional, explicitly-recorded
  FR-002(a) removal; if that removal is *not* taken, there is **zero** public `.fsi`
  signature delta and no baseline recapture is required.
- **SC-006**: The remove-vs-annotate decision (FR-002) and the value-role document-vs-drop
  decision (FR-005) are explicitly recorded in the feature's artifacts, so the chosen
  reconciliation is auditable and no ambiguity remains for a future reader.

## Assumptions

- The roadmap report `docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`
  is the authoritative narrative to reconcile against the code; editing it is in scope and
  does not require a package version bump (it is repo docs, not a packaged surface).
- The shipped behavior is **correct as-is** for every one of the six items; R8 changes the
  *description*, not the *behavior* (the one optional exception, FR-002(a), removes code that
  never executed on the live path, so it too is behavior-preserving for consumers).
- Feature 101 (R7) already recorded the intrinsic-size-memo deferral decision; R8's §10.4
  reconciliation describes the shipped `Bounds`/`LayoutNodeId` cache and cross-references that
  decision rather than re-opening it.
- The `.fsi` for the visual-state functions already documents the two-function split, so the
  R1 §10.3 reconciliation aligns the report to an already-honest surface (no `.fsi` change
  needed unless FR-002(a) removal is chosen).
- The default, lowest-risk reconciliation choices (annotate rather than remove; document
  rather than enable routing) keep R8 a zero-surface-delta, zero-behavior-change pass; the
  plan may elect the higher-touch options only with explicit justification and baseline
  recapture.
- The enforcement here is review/inspection plus the existing unchanged gate suite — R8 adds
  no new gate and no new property; its correctness is "everything stays green and the prose
  now matches the code".

## Out of Scope

- **R6** (true visual-state cross-fade) — the one behavior-changing follow-up; a separate
  feature.
- **Enabling** default navigation routing for `Chart`/`Graph`/`Progress`, or adding a
  `Segmented` `AccessibilityRole` — behavior/surface changes, separate features. R8 only
  *documents* the current narrowing.
- **Landing** the R2 intrinsic-size memo — feature 101's recorded deferral; R8 only
  reconciles the §10.4 wording to the shipped cache.
- Any change to dispatch (R3), animation (R4/R6), navigation routing (R5), the style
  resolver (E3/R1), or the legacy 080 preview render path beyond a clarifying annotation.
- Broad documentation rewrites beyond the six listed narrowings, or migrating the full
  control set / parity matrix (no parity row moves under R8).
