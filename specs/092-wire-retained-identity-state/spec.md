# Feature Specification: Wire Retained Identity Into Live Interactive State

**Feature Branch**: `092-wire-retained-identity-state`
**Created**: 2026-06-10
**Status**: Draft
**Input**: User description: "create specs from your findings" — code review of features 086–091 surfaced that feature 091 wired the keyed reconciler onto the render path for *scene reuse* but did not connect the stable cross-frame identity it computes to the live interactive state path, and left several render-path correctness gaps.

## Context & Motivation *(informative)*

Feature 091 (E2) wired the parked 067 keyed reconciler onto the live render path through
`module internal RetainedRender`. A code review of the landed feature found that the
*render-reuse* half is real and well-tested, but the *cross-frame-identity* half — the
stated E2 linchpin ("focus/text/animation state survives a positional shift") — is proven
only by tests that **manually seed** the identity-keyed state map. The running interactive
host never populates or reads that map: focus and text input remain keyed by the
**unstable, path-derived `ControlId`** carried over from feature 090. So in a real app, a
control whose position shifts between frames still loses its focus and its in-progress text.

The review also found correctness gaps on the wired path: a work-reduction measure that
contradicts its own documented invariant whenever a layout shift occurs, a fragment-reuse
key that ignores theme, a first-frame path that renders twice and defers duplicate-key
diagnostics, and pre-existing focus/text-targeting defects (from feature 090) on exactly the
code that must change to deliver the identity wiring.

This feature closes that gap: it makes cross-frame identity survival real in the running
host and brings the wired path's measured/documented behavior into agreement with what it
actually does.

## Clarifications

### Session 2026-06-10

- Q: How should theme changes interact with render-fragment reuse? → A: Fold theme into the
  fragment reuse key — a theme change invalidates affected fragments and they repaint
  (future-proofs E-series theme switching); no constant-theme precondition is relied upon.
- Q: How should the work-reduction record handle unchanged-but-shifted nodes (recomputed today
  without being counted)? → A: Add a distinct "shifted" counter tracking shifted-but-unchanged
  work separately from changed work, and correct the documented relationship to describe
  changed + shifted explicitly; produced output is unchanged.
- Q: Should two unkeyed same-kind sibling fields each be independently focusable, or is an
  explicit key required? → A: Fully disambiguate unkeyed siblings — click-to-focus resolves to
  the specific clicked control via its stable per-node identity, no key required; the 086/090
  unkeyed-sibling collision is removed, not documented as a constraint.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Focus and in-progress text survive a position change in the running app (Priority: P1)

A person using an interactive FS.Skia.UI app has focused a text field and typed a few
characters. An unrelated model update inserts or removes a sibling above that field (e.g. a
banner appears, a list row is added), shifting the field's position in the tree. The field
keeps its focus and the characters already typed remain — typing continues from where they
left off, with no reset.

**Why this priority**: This is the headline benefit of the E2 reconciler work and the one
the review found unmet in the live host. Without it, 091's identity machinery delivers no
user-visible value.

**Independent Test**: Drive the actual interactive host adapter (not a hand-seeded identity
map) through two model transitions where an unrelated change shifts a focused, partially-typed
control's position; assert focus and draft text are preserved across the shift, and that a
rebuild-every-frame baseline loses them.

**Acceptance Scenarios**:

1. **Given** a focused text field with draft text, **When** an unrelated update shifts its
   tree position, **Then** the field remains focused and its draft text is unchanged.
2. **Given** a control carrying per-control state (focus / in-flight animation / text model),
   **When** the diff matches it across a shift (not a Replace), **Then** that state is carried
   to the next frame keyed by its stable identity.
3. **Given** a control whose kind or key changed (a genuine Replace), **When** the next frame
   is produced, **Then** its prior per-control state is dropped (no false identity carry).

---

### User Story 2 - Any focusable field accepts focus and preserves its current value (Priority: P2)

A person clicks a text field to focus it and starts typing. This works whether or not the
field carries an explicit key, whether it is single-line or multi-line, and whether or not it
already had a value — the first keystroke appends to the existing value rather than erasing it.

**Why this priority**: The review found the live focus path silently fails for unkeyed
fields, fields wrapped in a keyed container, and pre-filled fields, and treats every field as
single-line. These defects sit on the exact code path US1 must rewire, so they are fixed
together. Without this, US1's survival guarantee only reaches directly-keyed, empty,
single-line fields.

**Independent Test**: Click-to-focus a text field reached three ways — directly keyed,
unkeyed, and an unkeyed child under a keyed container — and in each case type a character into
a pre-filled multi-line field; assert focus resolves, the field type is honored, and the
keystroke appends rather than replaces.

**Acceptance Scenarios**:

1. **Given** an unkeyed text field (or one nested under a keyed container), **When** the user
   clicks it, **Then** it becomes focused (the click resolves to the correct control).
2. **Given** a pre-filled field, **When** the user focuses it and types one character, **Then**
   the result is the existing value plus that character — the existing value is not discarded.
3. **Given** a multi-line field, **When** it is focused and edited, **Then** it behaves as
   multi-line, not single-line.
4. **Given** a control with more than one change binding, **When** it changes, **Then** every
   matching binding is dispatched (not only the first).

---

### User Story 3 - Work-reduction reporting is honest under layout shifts (Priority: P3)

A maintainer reading the per-frame work-reduction evidence sees numbers that match the
documented relationship for *every* localized change — including one that shifts unchanged
siblings — not only the no-geometry-shift case.

**Why this priority**: Output is already correct; this is about the measured/documented
contract being self-consistent so the evidence can be trusted as the SC-003 proof. Lower
urgency than user-visible behavior.

**Independent Test**: Run a localized change that shifts an unchanged sibling subtree (e.g.
insert a sibling above a fixed-size leaf) and assert the work-reduction record satisfies the
documented relationship; the prior suite only exercised the no-shift case.

**Acceptance Scenarios**:

1. **Given** a localized change that shifts an unchanged sibling, **When** the frame is
   produced, **Then** the reported recomputed-node count and changed-subtree bound satisfy the
   relationship stated in their own documentation (or that documentation is corrected to match
   the real behavior, with the distinction between "changed" and "shifted-but-unchanged" work
   made explicit).
2. **Given** any localized change, **When** work-reduction is reported, **Then** the recomputed
   count is strictly less than a full rebuild.

---

### User Story 4 - Render-path hygiene: theme changes, first frame, and standing collisions (Priority: P3)

Operators and future maintainers get predictable behavior from the wired path's edges: a
theme change repaints affected content rather than reusing stale-themed fragments; the first
frame is not painted twice; and a duplicate-key collision present from the very first frame is
surfaced, not hidden until a later frame.

**Why this priority**: All three are latent or low-impact today (theme is fixed per-frame in
the current host; the double-paint is a one-time cost; first-frame collisions are rare) but
each is a correctness/clarity trap as the framework evolves toward theme switching (E-series).

**Independent Test**: (a) Produce two frames with different themes and assert reused fragments
reflect the new theme (theme participates in the fragment reuse key — no constant-theme
precondition is relied upon, per the 2026-06-10 clarification / FR-008); (b) assert the first
frame performs a single paint; (c) assert a duplicate-key tree is reported on the first frame it
appears.

**Acceptance Scenarios**:

1. **Given** two consecutive frames with different themes, **When** the second is produced,
   **Then** no fragment painted under the old theme is reused unchanged — theme is part of the
   fragment reuse key and the affected fragments repaint (no constant-theme precondition is
   relied upon, per the 2026-06-10 clarification / FR-008).
2. **Given** the first frame of a host loop, **When** it is rendered, **Then** the node set is
   measured/painted once, not twice.
3. **Given** a tree containing duplicate sibling keys from its first appearance, **When** it is
   first rendered, **Then** the duplicate-key diagnostic is surfaced (once, de-duped), not
   deferred to a later frame.

---

### Edge Cases

- A focused control is **removed** entirely (not shifted): its per-control state is dropped and
  focus clears; no dangling identity retains state.
- The focused field's identity is carried but its *value in the model* also changed in the same
  frame: live draft text and the model value must reconcile without the keystroke being lost or
  doubled.
- Two unkeyed same-kind siblings are both focusable: clicking one must focus that one
  specifically, not collapse to a shared identity (the unkeyed-sibling identity collision noted
  in the 086/090 review must not reappear through this path).
- A theme change coincides with a structural change in the same frame.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The live interactive host MUST key per-control interactive state (focus target,
  text-input model, and per-control animation clock) by the reconciler's stable cross-frame
  identity rather than the path-derived control id, so the state survives an unrelated
  positional shift in the actual running host (not only in tests that pre-seed the state map).
- **FR-002**: The wired render path MUST populate and read the identity-keyed state map it
  maintains (FR-001 fixes *which key* is used; FR-002 is the distinct guarantee that the map is
  actually *populated and consulted* on the live path); no interactive-state lookup may depend on
  the unstable path-derived control id for cross-frame continuity.
- **FR-003**: When the diff matches a control across frames, its interactive state MUST be
  carried to the matched identity; when the diff Replaces a control (kind/key change) or removes
  it, its prior interactive state MUST be dropped.
- **FR-004**: Click-to-focus MUST resolve to the correct control regardless of whether that
  control is directly keyed, unkeyed, or an unkeyed descendant of a keyed container, using a
  single consistent identity scheme shared between hit-testing and focus resolution. Two unkeyed
  same-kind sibling controls MUST each be independently focusable — a click MUST resolve to the
  specific control hit (via its stable per-node identity), never collapse to a shared id. No
  explicit key may be required for distinct focus.
- **FR-005**: Focusing a field MUST seed the editing state from the field's current value (not
  empty) and MUST honor the field's line mode (single-line vs multi-line); the first keystroke
  after focus MUST append to / edit the existing value, never discard it.
- **FR-006**: A change on a focused control MUST dispatch every matching change binding, not
  only the first.
- **FR-007**: The per-frame work-reduction record MUST account for shifted-but-unchanged work
  (nodes recomputed only because an upstream change relaid them out) in a counter distinct from
  changed work, and its documentation MUST describe the relationship in terms of changed +
  shifted work explicitly. The documented relationship MUST hold for every localized change,
  including ones that shift unchanged sibling subtrees. The recomputed count MUST remain strictly
  less than a full rebuild for a localized change. Adding these counters MUST NOT alter the
  produced render output.
- **FR-008**: Theme MUST be part of the render-fragment reuse key: a fragment painted under one
  theme MUST NOT be reused unchanged under a different theme — a theme change invalidates the
  affected fragments and they repaint. The path MUST NOT rely on a constant-per-host-loop theme
  precondition.
- **FR-009**: The first frame of a host loop MUST measure/paint its node set once (no
  double render), and MUST surface a duplicate-key diagnostic present in that first frame rather
  than deferring it to a subsequent frame.
- **FR-010**: All four 067/091 invariants (totality, determinism, identity-at-rest, round-trip)
  MUST continue to hold on the wired path, and the round-trip output MUST remain byte-identical
  to a full rebuild of the next tree.

> Interacting / conflicting requirements: FR-005 (seed draft from current value) vs the live
> draft text the user is typing (FR-001/FR-003 carry the draft across frames) — resolution:
> the carried draft is authoritative while a control is focused; the model value re-seeds the
> draft only on initial focus acquisition, not on every re-render, so a same-frame model change
> never silently overwrites in-progress typing. FR-007 (honest accounting) vs FR-010
> (byte-identical output) — resolution: accounting/measurement changes MUST NOT alter the
> produced scene; if forced to choose, output fidelity wins and the measure is corrected to
> describe it.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).**

- **Package impact**: No package identity or set changes. Behavioral changes land in the
  existing `FS.Skia.UI.Controls` and `FS.Skia.UI.Controls.Elmish` packages
  (`src/Controls/RetainedRender.fs`, `src/Controls.Elmish/ControlsElmish.fs`) **and the
  `FS.Skia.UI.SkiaViewer` package** (`src/SkiaViewer/SkiaViewer.fs[i]`, for the FR-006 `MapKey`
  widening). All packable libraries are version-bumped and the template pins refreshed on merge
  per the standard flow.
- **Public contract impact**: This feature **does** move public surface (Tier 1). `module
  internal RetainedRender` stays internal (zero public-surface delta, mirroring 067/091), but its
  internal `RetainedRender.fsi` work-reduction/theme documentation MUST be brought into agreement
  with behavior (FR-007/FR-008). Two public surfaces change and their baselines MUST be
  recaptured: (1) **`SkiaViewer.fsi` — a breaking change**: `InteractiveViewerHost.MapKey` widens
  from `ViewerKey -> bool -> 'msg option` to `ViewerKey -> bool -> 'msg list` (FR-006; mechanical
  migration `Some m → [ m ]`, `None → []`, recorded in the public migration note). (2)
  **`Controls.Elmish.fsi`**: the focus/text-routing seam re-keys from the unstable `ControlId` onto
  the stable retained identity. **Any `src/Controls.Elmish/*.fsi` or `src/SkiaViewer/*.fsi`
  signature change escalates to the package-surface rule and per-package + cross-package surface
  baselines must be recaptured** (Controls.Elmish `.fsi` routes to the package-surface rule, not
  controls-public-surface).
- **State workflow impact**: Yes — this is the core of the feature. The interpreter-edge state
  held in the `ControlsElmish` host closure (focus target, text models, retained structure)
  changes from path-id keying to stable-identity keying. Mutation stays confined to the
  interpreter edge; the consumer `view`/`update` remain pure.
- **Layout/rendering impact**: Rendering output MUST remain byte-identical to a full rebuild
  (FR-010). Fragment-reuse keying changes (theme, FR-008) and first-frame paint (FR-009) affect
  the render path but not the produced scene for the supported constant-theme case. No new Skia
  / Vulkan surface; deterministic render-only evidence (no live window required).
- **Evidence obligations**: Real, in-repo readiness artifacts under
  `specs/092-wire-retained-identity-state/readiness/` proving: (a) focus + draft text + animation
  clock survive a positional shift **through the live adapter path** (not a hand-seeded state
  map) and a rebuild-every-frame baseline fails the same proof; (b) click-to-focus resolves for
  keyed, unkeyed, and keyed-container-wrapped fields and a pre-filled multi-line field appends on
  first keystroke; (c) work-reduction satisfies its documented relationship under a
  sibling-shifting change; (d) round-trip byte-identity preserved. Parity / survival proofs are
  authoritative as structural `Scene` / identity equality (SceneEvidence render functions are
  deterministic capability-hash functions, not pixel encoders).
- **Unsupported scope**: Out of scope — caret position, text selection, IME composition, undo/
  redo, clipboard (deferred E4 text-editing scope); per-frame theme *switching UI* (only the
  reuse-correctness/precondition is in scope, not a theme-toggle feature); any XAML/data-binding/
  dependency-property/lookless-template/CSS-selector capability (permanent roadmap non-goals); a
  live windowed pixel-PNG capture path.
- **Build-target impact**: Run `Route` first and run only the gates it prints. Expected:
  framework-internal `src/**/*.fs` changes route to the inner-loop `Dev` tier; if any
  `src/Controls.Elmish/*.fsi` public signature changes, the change escalates (package-surface)
  and the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck →
  EvidenceGraph → EvidenceAudit` path applies. No new gate is added.

## Success Criteria *(mandatory)*

- **SC-001**: In the running interactive host (driven through its real input/render adapter, with
  no manual seeding of the identity-keyed state map), a focused, partially-typed control whose
  position is shifted by an unrelated update retains its focus, its exact draft text, **and its
  per-control animation clock** across the shift; a rebuild-every-frame baseline loses them under
  the identical sequence.
- **SC-002**: Click-to-focus succeeds for a directly-keyed field, an unkeyed field, and an
  unkeyed field nested under a keyed container; in a pre-filled multi-line field the first
  keystroke yields the prior value plus the new character (zero characters lost); and a control
  carrying more than one change binding dispatches **every** matching binding (not only the
  first, FR-006) — verified for 100% of these cases.
- **SC-003**: For a localized change that shifts an unchanged sibling subtree, the per-frame
  work-reduction record reports changed and shifted work in distinct counters, satisfies its
  documented changed + shifted relationship, and the recomputed-node count is strictly less than
  the full node count — demonstrated by a test that exercises a sibling-shifting change (not only
  the no-geometry-shift case).
- **SC-004**: The wired path's produced render result is byte-identical to a full rebuild of the
  next tree across at least 1000 generated frame pairs, and remains so across a chained sequence
  of 3 or more consecutive frames (multi-frame reconciliation, not only a single transition).
- **SC-005**: A duplicate-key tree present from the first frame surfaces its diagnostic on that
  first frame (de-duped to once per standing collision), and the first frame measures/paints its
  node set exactly once.
- **SC-006**: A theme change between two frames never results in a reused fragment showing the
  prior theme: the second frame's output is byte-identical to a full rebuild under the new theme,
  with theme participating in the fragment reuse key.
- **SC-007**: All four 067/091 invariants (totality, determinism, identity-at-rest, round-trip)
  continue to pass on the wired path after the change.

## Assumptions

- The interactive host loop uses a single, fixed theme per run today, but FR-008/SC-006 require
  theme to participate in the fragment reuse key regardless, so the path is correct ahead of the
  E-series theme-switching work. Wiring a theme-toggle *UI* is still out of scope; only
  reuse-correctness under a theme change is in scope.
- The reconciler's `Keep` patch continues to imply whole-subtree structural equality (the 067
  invariant the carry/reuse logic relies on); this feature does not change the diff algorithm,
  only how its results drive interactive state and how work is measured.
- "Draft text" survival means the in-progress edit buffer at the interpreter edge; final
  text-editing semantics (caret/selection/IME) remain deferred to the E4 scope.
- Per the architecture-evolution decision, this is incremental MVU-core evolution toward
  declarative-retained parity (E-series), not a redesign; no new public declarative API is
  introduced by this feature.

## Key Entities

- **Stable cross-frame identity**: the diff-conferred, monotonic per-control identity that
  persists across positional shifts (distinct from the unstable path-derived control id).
- **Per-control interactive state**: focus target, text-input/edit model, and per-control
  animation clock — the state that must be keyed by stable identity to survive a re-render.
- **Retained render structure**: the per-frame retained tree (previous lowered tree + cached
  render fragments + identity-keyed state) held at the interpreter edge of the host loop.
- **Work-reduction record**: the per-frame measure of baseline vs recomputed vs changed-subtree
  node counts used as the partial-update (SC-003) evidence.
- **Render fragment**: the cached, reusable measure+paint unit for one node, reused only when
  its paint inputs (own data, computed box, and — per FR-008 — theme) are provably unchanged.
