# Feature Specification: Internal Keyed Reconciliation

**Feature Branch**: `067-keyed-reconciliation`
**Created**: 2026-06-05
**Status**: Draft
**Input**: User description: "implement the next part of docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md" — roadmap feature **067 — Internal keyed reconciliation** (§13): a keyed VDOM diff over the lowered `Control<'msg>` IR, internal only.

## Overview

Features `065` (typed controls front door) and `066` (typed catalog generation)
have landed. The next roadmap step is the **internal keyed reconciliation**
foundation: a pure diff that, given a previous and a next `Control<'msg>` tree,
computes the minimal, deterministic set of changes between them. Children are
matched by their stable `Key` (`Control.Key: ControlId option`) rather than by
list position, so reorders, insertions, and removals are recognized as such and
node identity is preserved instead of being destroyed by a positional shift.

This feature is **internal-only**. It adds no public authoring API, does not
change the `Control<'msg>` IR, and is **not yet wired into** the live render
path. It is the data structure and algorithm that a later feature (incremental
rendering / the `Widget` reconciliation metadata anticipated by the sealed
wrapper in `065` §3.2) will consume. The deliverable here is a correct,
property-proven diff engine — not a rendering change.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Keyed children survive reordering (Priority: P1)

A framework author holds a previous control tree and a next tree in which the
same keyed children appear in a different order (e.g. a reordered list). The
reconciler matches the children by `Key` and emits **move/keep** operations that
preserve each node's identity, rather than reporting every position as changed.

**Why this priority**: This is the defining behavior of *keyed* reconciliation
and the reason the feature exists — without it, a reorder destroys and recreates
every node, which would later discard focus, caret, and per-control runtime
state. If only this story ships, the feature delivers its core value.

**Independent Test**: Build two trees with three keyed children `[a; b; c]` and
`[c; a; b]`; assert the produced patch contains move operations keyed to
`a`/`b`/`c` and **zero** replace operations.

**Acceptance Scenarios**:

1. **Given** a parent whose previous children are keyed `[a; b; c]` and whose
   next children are keyed `[c; a; b]`, **When** the trees are reconciled,
   **Then** the patch matches each child by key and contains no node replacement.
2. **Given** a keyed child whose attributes are unchanged but whose position
   moved, **When** reconciled, **Then** the patch records a move with no
   attribute-update sub-patch for that node.

### User Story 2 - Minimal patch for in-place changes (Priority: P2)

When a node keeps its key (or position) and kind but its attributes, content, or
accessibility metadata change, the reconciler emits a targeted **update** patch
describing only what changed, rather than replacing the subtree.

**Why this priority**: Targeted updates are what make a future incremental
renderer cheaper than a full re-render; this is the second pillar after keyed
matching.

**Independent Test**: Reconcile two single-node trees identical except for one
attribute value; assert the patch is a single attribute-update for that name.

**Acceptance Scenarios**:

1. **Given** two same-kind, same-key nodes differing only in one attribute value,
   **When** reconciled, **Then** the patch is an attribute update naming that
   attribute, and no other node is touched.
2. **Given** two same-kind nodes differing only in `Content`, **When** reconciled,
   **Then** the patch records a content change and nothing else.

### User Story 3 - Insertion and removal detection (Priority: P2)

When children are added or removed between the previous and next tree, the
reconciler emits explicit **insert** and **remove** operations identifying the
affected nodes (by key when present, by index otherwise).

**Independent Test**: Reconcile `[a; b]` → `[a; b; c]`; assert one insert for `c`.
Reconcile `[a; b; c]` → `[a; c]`; assert one remove for `b`.

**Acceptance Scenarios**:

1. **Given** a next tree with one additional keyed child, **When** reconciled,
   **Then** the patch contains exactly one insert for that child and leaves the
   others as keep/no-op.
2. **Given** a next tree missing one previously-present keyed child, **When**
   reconciled, **Then** the patch contains exactly one remove for that child.

### User Story 4 - Deterministic unkeyed fallback (Priority: P3)

When children carry no `Key`, the reconciler falls back to a deterministic
positional diff so the output is stable and reproducible (mixed keyed/unkeyed
sibling lists resolve by a single documented rule rather than implementation
accident).

**Independent Test**: Reconcile two unkeyed sibling lists twice; assert byte-for-
byte identical patch output across runs.

**Acceptance Scenarios**:

1. **Given** sibling children with no keys, **When** reconciled, **Then** matching
   is positional and the result is identical on repeated runs.
2. **Given** a sibling list mixing keyed and unkeyed children, **When** reconciled,
   **Then** keyed nodes match by key first and the remaining unkeyed nodes match
   positionally among themselves, per the documented rule.

### Edge Cases

- **Root kind change**: previous and next roots have different `Kind` → a whole-
  subtree **replace**, not an attribute diff.
- **Duplicate keys** within one sibling list → resolved by a single documented
  rule (first-occurrence wins) and surfaced as a diagnostic rather than silently
  mis-matching; the diff must still be deterministic.
- **Empty trees**: previous empty → next non-empty yields all-inserts; the reverse
  yields all-removes; both-empty yields an empty patch.
- **Identical trees**: previous structurally equal to next yields an empty patch
  (no-op), which is the round-trip identity case.

> Interacting / conflicting requirements: keyed matching vs. positional fallback
> can both apply within one sibling list. Resolution: **keys win** — keyed nodes
> are matched by key across the whole sibling list first; the residual unkeyed
> nodes are then matched positionally among themselves. This keeps a single
> deterministic rule rather than leaving the mixed case to implementer judgment.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The reconciler MUST be a pure function from a previous and a next
  `Control<'msg>` tree to a deterministic patch value (no I/O, no mutation, no
  reliance on `Date.now`/randomness).
- **FR-002**: The feature MUST be **internal-only** — it adds no new public
  authoring API and changes no existing public `.fsi` signature. Any new module
  is internal (no public surface delta), so `PackageSurfaceCheck` sees no change.
- **FR-003**: Sibling children MUST be matched primarily by `Control.Key`; nodes
  with equal keys in the previous and next lists are treated as the **same** node
  regardless of position.
- **FR-004**: The patch MUST distinguish at minimum these operations: keep/no-op,
  attribute/content/accessibility update, child insert, child remove, child move
  (reorder), and whole-subtree replace.
- **FR-005**: Reconciliation MUST recurse into matched children so changes at any
  depth are described relative to their matched parent.
- **FR-006**: A `Kind` mismatch between two otherwise-matched nodes MUST produce a
  whole-subtree replace rather than an attribute diff.
- **FR-007**: Attribute differences MUST be computed by attribute `Name`, emitting
  updates for changed/added names and removals for dropped names, independent of
  attribute ordering within the list.
- **FR-008**: The diff MUST satisfy a **round-trip invariant**: applying the
  produced patch to the previous tree yields a tree structurally equal to the next
  tree. This invariant MUST be property-tested over randomly generated trees.
- **FR-009**: Output MUST be deterministic — identical `(previous, next)` inputs
  produce identical patches on every run and across processes.
- **FR-010**: When a sibling list contains no keys, matching MUST fall back to a
  deterministic positional diff; mixed keyed/unkeyed lists resolve keys-first then
  positionally (see the interacting-requirements note).
- **FR-011**: Duplicate keys within one sibling list MUST resolve by a documented
  first-occurrence rule and surface a diagnostic; the diff MUST remain
  deterministic and total (never throw) on such input.
- **FR-012**: The reconciler MUST NOT alter the existing render, layout,
  diagnostics, accessibility, or evidence behavior — it is additive and not wired
  into the live render path in this feature.
- **FR-013**: The reconciler MUST NOT introduce a dependency on `Fable.Elmish`
  into `FS.Skia.UI.Controls`, nor depend on the renderer; it operates purely on
  the `Control<'msg>` IR.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, or version change is required
  by the feature itself. The active authoring package is `FS.Skia.UI.Controls`
  (`src/Controls/**`); the reconciler lands inside it as **internal** code. No
  legacy Charts migration is involved. A version bump/pack is a post-merge concern
  owned by the `speckit-merge` skill, not this spec.
- **Public contract impact**: **None.** No public `.fsi` signature, documented
  public API, sample contract, or surface baseline changes. The new module is
  internal; the public surface baseline must remain byte-for-byte unchanged
  (this is itself an assertion, FR-002).
- **State workflow impact**: No change to stateful MVU workflows, effects,
  subscriptions, commands, or interpreter behavior. The reconciler is a pure diff
  over immutable IR and owns no runtime state.
- **Layout/rendering impact**: **None** in this feature. No layout, charts,
  DataGrid, rendering, screenshot, Vulkan, Skia, or unsupported-environment
  diagnostic behavior changes; the reconciler is not yet consumed by the renderer.
- **Evidence obligations**: The `controls-public-surface` routing rule
  (`build/Governance/Routing.fs`) matches `src/Controls/**` and requires
  `readiness/typed-controls-front-door.md` and
  `readiness/package-surface-expectations.md` under this feature's spec dir; both
  MUST be produced/updated (the latter recording the **zero** public-surface delta).
  A reconciliation-specific evidence artifact
  (`readiness/keyed-reconciliation.md`) MUST record the algorithm, the keys-first
  matching rule, the duplicate-key diagnostic, and the round-trip property-test
  results.
- **Unsupported scope**: No incremental renderer wiring, no `Widget`/adapter
  surface change, no design-token/Penpot work, no catalog change, no migration of
  the remaining 41 controls. Visual output, release, platform, and distribution
  are unchanged.
- **Build-target impact**: No change to `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, or `EvidenceAudit` semantics. Because the change touches
  `src/Controls/**`, `Route` escalates to the `controls-public-surface` gate set
  (`ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`,
  `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`,
  `FsiTranscripts`, `GeneratedProductCheck`); run only the gates `Route` prints.

## Key Entities

- **Previous / Next tree**: two `Control<'msg>` values (existing IR; unchanged) —
  the inputs to reconciliation. Each node carries `Kind`, `Key`, `Attributes`,
  `Children`, `Content`, and `Accessibility`.
- **Patch**: the new internal value describing the ordered set of operations that
  transform the previous tree into the next tree. Composed of node-level
  operations (keep, update, insert, remove, move, replace) and recursive child
  patches.
- **Match**: the internal pairing of a previous node with a next node, established
  by key (primary) or position (fallback), that decides whether a node is updated
  in place or replaced.

## Success Criteria *(mandatory)*

- **SC-001**: Reordering N keyed siblings produces a patch with zero subtree
  replacements — every reordered node is matched by key and preserved (verified by
  the User Story 1 test).
- **SC-002**: For any randomly generated `(previous, next)` tree pair, applying the
  produced patch to `previous` yields a tree structurally equal to `next` — the
  round-trip property holds across at least 1,000 generated cases with no
  counterexample (FR-008).
- **SC-003**: A single changed attribute on an otherwise-unchanged node produces a
  patch describing exactly that one attribute change and touching no other node.
- **SC-004**: Identical `(previous, next)` inputs produce identical patch output on
  repeated runs (deterministic; verified by re-running the same diff and comparing).
- **SC-005**: The regenerated public-surface baseline shows **no** delta — the
  feature adds zero public API (verified by `PackageSurfaceCheck` passing with an
  unchanged baseline).
- **SC-006**: `./fake.sh build -t Route` over the branch diff prints the
  `controls-public-surface` escalation and **every printed gate passes**.
- **SC-007**: The reconciler is total — it returns a patch (never throws) for every
  input pair the property generator produces, including duplicate-key and
  empty-tree edge cases.

## Assumptions

- The existing `Control<'msg>` IR already carries the data keyed reconciliation
  needs: `Key: ControlId option` for identity and `Children: Control<'msg> list`
  for structure. No IR change is required (confirmed against `src/Controls/Types.fsi`).
- "Internal only" means the reconciler is reachable from framework code and tests
  but is not exported on any public `.fsi`, consistent with §13 of the plan
  ("internal only") and the sealed-`Widget` rationale in §3.2.
- The reconciler is implemented in compiled F# and property-tested with the repo's
  existing Expecto + FsCheck harness (per `fsharp-graph-algorithms` /
  `fsharp-build-orchestration` skill guidance); no new test framework is added.
- Wiring the reconciler into an incremental renderer and any `Widget`
  reconciliation-metadata surface are deferred to a later feature, not this one.

## Out of Scope

- Wiring reconciliation into the live render/layout path (incremental rendering).
- Any public `Widget`/`Control`/adapter surface change or new exported API.
- Design tokens, Penpot, catalog regeneration, and migrating the remaining 41
  controls.
- Performance tuning beyond producing a correct, deterministic minimal-ish diff;
  this feature targets correctness, not a benchmarked fast path.
