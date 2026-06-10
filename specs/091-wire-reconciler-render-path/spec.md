# Feature Specification: Retained-Tree Reconciliation on the Render Path — Wiring the Parked Keyed Reconciler for Cross-Frame Control Identity & Partial Updates (Roadmap E2)

**Feature Branch**: `091-wire-reconciler-render-path`
**Created**: 2026-06-10
**Status**: Draft
**Input**: User description: "create the first part of docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md"

## Context & Triage *(informative)*

The Controls Architecture Evolution Roadmap
(`docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`, maintainer-confirmed
2026-06-10) records a strategic decision: **no redesign** — the controls subsystem keeps its
immutable `Control<'msg>` + MVU core and **evolves** toward declarative-retained
(SwiftUI/Jetpack-Compose-class) capability parity, not retained-mode XAML/data-binding architecture
parity. The roadmap lays out a five-step evolution (E1–E5), each an independently shippable Spec Kit
feature routed through the existing governance gates.

- **E1 — Live interactivity** is already specified and clarified as feature **090**
  (`specs/090-interactive-host-event-dispatch/`): authored-binding dispatch, keyed-ancestor recovery,
  the focus-aware text seam, and a responds-vs-renders proof. It is *table stakes* — nothing
  downstream is observable while the live window is inert.
- **E2 — Wire the parked reconciler** is **this feature (091)**, and is the roadmap's explicit next
  spec (§9, recommendation 2). It is described as **the linchpin**: stable cross-frame control
  identity is the precondition for E3 (visual-state styling), E4 (focus/traversal), and per-control
  animation, and it moves the framework from rebuild-every-frame immediate-mode to
  declarative-retained, scaling past redraw-the-world performance.

This is **not a new algorithm.** Feature **067** already shipped the keyed VDOM diff
(`src/Controls/Reconcile.fsi`, `module internal Reconcile`) — pure, total, deterministic, fully
property-tested (round-trip over ≥1,000 generated cases), and **deliberately parked / unwired**. 091
is the **wiring + invariant-preservation** feature that promotes that asset onto the live render path.
The `fs-skia-reconciliation` skill's "Disposition" section is the single source of truth for the
module's status and **must be updated** when this feature lands (it currently says "parked, not wired").

### Current-state evidence (grounded against source)

| Fact | Source |
|------|--------|
| The reconciler exists, is pure/total/deterministic, and is **internal-only / unwired** | `src/Controls/Reconcile.fsi` (`module internal Reconcile`; `diff`/`apply`; `NodePatch`/`UpdatePatch`/`ChildOp`/`ReconcileResult`); `.agents/skills/fs-skia-reconciliation/SKILL.md` ("deliberately-parked internal spike … not wired into the live render path") |
| Children match **key-first, then positional**; a `Kind` mismatch yields a whole-subtree `Replace` | `src/Controls/Reconcile.fsi` (`diff` doc); feature 067 FR-003/FR-006/FR-010 |
| The render path **rebuilds the whole tree every frame** (no retained identity) | `src/Controls/Control.fs:1014` `renderTree` produces a fresh `ControlRenderResult` from the model each call; `Reconcile.diff`/`apply` are never called from `render`/`renderTree` |
| The live host **re-renders the full lowered tree after every dispatched message** | `src/SkiaViewer/SkiaViewer.fs:2364` `dispatchHostMsg` recomputes `currentScene <- host.View currentModel` (also `:2320`, `:2437`); this is the O(whole-tree) redraw-the-world loop E2 replaces internally |
| Reconciler tests reach the module via `InternalsVisibleTo("Controls.Tests")` | `src/Controls/Reconcile.fsi` header; 067 property suite |
| Animation is applied **post-render at the Scene layer**, not attached to a control identity | roadmap §2.7 (feature 073); there is no control-level animation clock keyed to a retained identity today |

### Why E2 depends on E1 (091 depends on 090)

E2 makes a *live host worth optimizing*. The cross-frame survival of **focus** (and an in-flight
animation) is only observable once the live window actually responds to input — which is exactly what
E1/090 delivers (authored-binding dispatch + focus-aware text seam + the responds-proof primitive).
091 reuses 090's responds-proof artifact as the durable evidence primitive for "focus/animation
survives an unrelated re-render."

**Change classification.** **Escalated / `maintainer-verify` (Tier 1) expected.** The change touches
`src/Controls/**` (promoting `Reconcile` onto the render path, integrating with `Control.fs`
`render`/`renderTree`), `src/Controls.Elmish/**` (the host loop), and `src/SkiaViewer/**` (the
`dispatchHostMsg` repaint seam) — the controls-public-surface / package-surface routing rules apply,
so `Route` is expected to escalate and the serialized six-target order
(`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` →
`EvidenceAudit`) is expected to run. Any new public `.fsi` signature (e.g. a retained-render entry or
a partial-update option) requires recapturing per-package and published `docs/api-surface` baselines.
This feature also updates the `.agents/skills/fs-skia-reconciliation` disposition (and any other
touched skill), which must be regenerated into `.claude` via `RefreshSurfaceBaselines`
(`SkillSyncCheck`-enforced). Run only the gates `Route` prints.

### Trajectory position *(informative)*

091 is **E2** of the maintainer-confirmed E1→E5 arc. It does **not** add styling (E3), a general
focus/keyboard-traversal model (E4), or lookless slots (E5); those build *on top of* the retained
identity 091 establishes. The permanent non-goals of the roadmap — XAML, data binding, dependency/
attached properties, a lookless `ControlTemplate` engine, CSS-selector styling — remain explicitly
**rejected** (not "deferred") and are out of scope here.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A control keeps its identity across an unrelated re-render (Priority: P1)

A consumer hosts an MVU app and writes the usual pure `view : 'model -> Control<'msg>`. Between two
frames the model changes in a way that does **not** affect a given keyed control (e.g. a counter
elsewhere ticks). With reconciliation wired into the render path, that control is recognized as **the
same control** across the two frames — its identity is preserved (key-first, then positional) rather
than the whole tree being rebuilt and the control re-created from scratch.

**Why this priority**: This is the defining behavior and the entire reason E2 is "the linchpin."
Stable cross-frame identity is the substrate every later step (focus survival, animation continuity,
visual-state transitions, partial updates) stands on. If only this story ships, the feature delivers
its core value.

**Independent test**: From a harness, drive two successive renders where the second model differs only
in a region unrelated to a keyed control K. Confirm the wired render path matches K across frames (a
`ChildKeep`/`Update`, **not** a `Replace`) and that any per-control identity-bearing state attached to
K survives. Confirm a control whose `Kind` actually changed across the two frames is `Replace`d (no
false identity).

### User Story 2 - Focus (and an in-flight animation) survives an unrelated state change (Priority: P1)

A consumer focuses a control (e.g. clicks a text control via the E1/090 seam) or starts a per-control
animation, then an unrelated part of the model updates and the window re-renders. Because the focused/
animating control retains its identity across the re-render, **focus is not reset** and the animation
**continues from where it was** rather than restarting — the "focus/shortcuts reset after an unrelated
update" class of bug (observed as the ControlsShowcase2 "shortcuts blocked after clicks" symptom) is
structurally eliminated.

**Why this priority**: This is the first *user-visible* payoff of retained identity and the concrete
defect class E2 closes. It is co-equal P1 with US1 because identity that nothing observes is not yet
proven useful; this story proves it through focus/animation continuity.

**Independent test**: Render a tree with a focusable control, set focus, dispatch an unrelated model
update, re-render, and confirm `ControlRuntime.FocusedControl` still names the same control. Separately,
start a per-control animation, advance one frame with an unrelated model change, and confirm the
animation clock for that control continues (does not reset to its start). Capture an E1/090-style
**responds/survives proof** that an inert or rebuild-every-frame baseline fails.

### User Story 3 - A localized change re-paints/re-measures only the changed subtree (Priority: P2)

When the model change affects only one control (or a small subtree), the wired render path applies the
reconciler's `NodePatch`/`ChildOp` so that **only the changed subtree** is re-measured and re-painted;
unchanged subtrees are reused. This replaces the current O(whole-tree) redraw-the-world loop with
O(changed-subtree) work — the performance precondition for non-trivial apps (large collections,
frequent updates) to stay smooth.

**Why this priority**: Partial updates are the performance unlock the roadmap calls out, but they
depend on US1's identity being in place first. P2 because the framework is correct and useful with
identity alone (US1/US2); partial updates are the efficiency dividend on top.

**Independent test**: Render a tree of N controls, change one leaf's attribute, re-render through the
wired path, and confirm a **measured reduction** in re-measure/re-paint work versus the
rebuild-every-frame baseline (e.g. only the changed subtree's nodes are re-evaluated; the count of
re-measured/re-painted nodes is bounded by the changed subtree, not N). Confirm the rendered output is
identical to a full rebuild of the same `next` model (golden-diff parity).

### User Story 4 - Determinism and the 067 invariants hold on the live path (Priority: P2)

A maintainer must be able to trust that wiring a diff onto the live render path did not weaken the
project's constitution. The totality, determinism, identity-at-rest, and round-trip invariants that
feature 067 guarantees for `Reconcile` continue to hold **now that the module is on the live path**,
proven by the 067 property tests promoted to cover the wired path plus golden diffs over real renders.
Mutation introduced by applying patches to a retained structure stays **framework-internal**; the
consumer's `view` contract remains pure MVU.

**Why this priority**: E2 is the roadmap's highest-risk step precisely because it wires a module that
*can* mutate. The guarantee that the constitution survives the wiring is what makes E2 shippable. P2
because it is a cross-cutting correctness obligation over US1–US3 rather than a standalone capability.

**Independent test**: Promote the 067 round-trip/determinism/totality property tests so they exercise
the wired render path (not just `diff`/`apply` in isolation): for randomly generated `(prev, next)`
model pairs, the wired path's retained output is structurally equal to a full rebuild of `next`
(round-trip), identical across repeated runs (determinism), and never throws (totality). Identical
inputs across frames produce no spurious patches (identity-at-rest). Golden-diff parity holds between
the wired path and the full-rebuild baseline.

## Requirements *(mandatory)*

### Functional Requirements

**Wire the reconciler onto the render path (RETAINED-PATH-1)**

- **FR-001**: The framework MUST wire the existing `Reconcile.diff` into the render path so that, given
  the previous and next lowered `Control<'msg>` trees, the next frame is produced by **diffing against
  the previous and applying the resulting `NodePatch`/`ChildOp` to a retained structure**, rather than
  by rebuilding the whole tree from the model each frame. The reconciler's existing key-first-then-
  positional matching and `Kind`-mismatch→`Replace` rule (feature 067) MUST be the matching rule used.
- **FR-002**: The wiring MUST establish a **retained tree that persists between frames** and that any
  introduced mutation operates on. Mutation MUST stay **framework-internal**: the consumer-facing
  surface MUST remain the pure `view : 'model -> Control<'msg>` MVU contract — no observable/mutable
  view-model, data binding, or dependency-property system is introduced (those are permanent roadmap
  non-goals).
- **FR-003**: A control that is "the same" across two frames (matched by key first, then position) MUST
  **retain its identity** across the re-render, so identity-bearing per-control state — at minimum
  **focus** (`ControlRuntime.FocusedControl`) and a **per-control animation clock** — survives an
  unrelated model change instead of being reset by a rebuild. A control whose match resolves to
  `Replace` (e.g. `Kind` changed) MUST NOT spuriously retain identity.

**Partial update (PARTIAL-UPDATE-1)**

- **FR-004**: The wired render path MUST drive **partial re-render/re-layout**: when a model change
  affects only a subtree, only that changed subtree is re-measured and re-painted, and unchanged
  subtrees are reused — moving the live loop from O(whole-tree) to O(changed-subtree) work for a
  localized update. The integration point is the full-tree render/redraw step the host runs today
  (`Control.renderTree`; the `SkiaViewer` `dispatchHostMsg` repaint at `src/SkiaViewer/SkiaViewer.fs:2364`).
- **FR-005**: The wired path's rendered output MUST be **identical to a full rebuild of the same `next`
  model** — partial update is an internal efficiency, never a visible-output difference. This MUST be
  proven by golden-diff parity between the wired path and the full-rebuild baseline.

**Invariant preservation on the live path (INVARIANTS-LIVE-1)**

- **FR-006**: The wired path MUST preserve, **now on the live render path**, the determinism invariants
  feature 067 already guarantees for `Reconcile` in isolation: **totality** (never throws for any
  `(prev, next)`, including duplicate-key and empty-tree cases — surfacing `KeyCollision`/diagnostics
  at the wiring boundary rather than failing), **determinism** (identical frame inputs produce identical
  output across runs and processes), **identity-at-rest** (structurally identical successive frames
  produce no spurious patches/re-renders), and the **round-trip** equivalence to a full rebuild of
  `next`. These MUST be covered by promoting the 067 property tests to exercise the wired path.
- **FR-007**: Any `ReconcileResult.Diagnostics` produced on the live path (e.g. `KeyCollision` from
  duplicate keys in a sibling list) MUST be surfaced through the existing diagnostics channel at the
  wiring boundary, not silently dropped; the live path MUST remain total in their presence.

**Compatibility & scope discipline (SCOPE-1)**

- **FR-008**: The change MUST be **additive to the consumer surface**: an existing MVU consumer's
  `view`/`update`/`Init`/`Subscriptions` contract is unchanged and requires **no rewrite** to benefit.
  The change is to the *internal* render path, not the `view` contract (roadmap §6.4). If a public
  entry point or option is introduced to select/observe the retained path, it MUST be additive and the
  surface baselines recaptured.
- **FR-009**: This feature delivers **cross-frame identity + partial updates only**. It MUST NOT add a
  visual-state/style layer (roadmap **E3**), a general focus/keyboard-traversal model (**E4**), or
  lookless slot composition (**E5**); and it MUST NOT introduce any rejected non-goal (XAML, data
  binding, dependency/attached properties, lookless `ControlTemplate` engine, CSS-selector styling).
  Attaching a *per-control animation clock* to a retained identity is **in scope** only to the extent
  it proves FR-003's identity survival; the broader animation-retargeting work is sequenced after E2.
- **FR-010**: The `fs-skia-reconciliation` skill's **Disposition** section (the single source of truth
  for the module's status) MUST be updated from "deliberately unwired / parked" to reflect that the
  reconciler is now wired into the render path, and regenerated into the `.claude` mirror
  (`SkillSyncCheck`).

> Interacting / conflicting requirements: **partial-update efficiency (FR-004) vs. output fidelity
> (FR-005) vs. determinism (FR-006).** Resolve as: correctness wins. The retained/partial path is an
> internal optimization whose output MUST be byte-for-byte equal to a full rebuild of `next` (golden
> parity) and MUST satisfy the round-trip/determinism invariants; if any conflict arises, the wired
> path falls back to producing the full-rebuild-equivalent result rather than a faster-but-divergent
> one. Efficiency is measured (FR-004) but never at the cost of FR-005/FR-006.

> Interacting / conflicting requirements: **internal mutation (FR-002) vs. the determinism/identity-at-
> rest constitution (FR-006).** Resolve as (the reason 067 was written pure/total): mutation is
> confined to the framework-internal retained structure produced by applying `NodePatch`/`ChildOp`; the
> consumer surface stays pure MVU, and the observable behavior (round-trip equality to a full rebuild,
> deterministic output, no spurious re-render at rest) is what the gates assert — not the absence of
> any internal mutation.

> Interacting / conflicting requirements: **golden-parity byte-identity (FR-005/SC-004) vs. animation-
> clock continuity (FR-003/SC-002).** A full rebuild of `next` carries no in-flight per-control
> animation clock (the clock is retained-only state, not derivable from the `next` model), so a naive
> reading would have an animating control's wired output diverge from a rebuild. Resolve as: the
> per-control animation clock is applied **post-render at the Scene layer** (feature 073; spec §
> Current-state evidence), i.e. **outside** the `ControlRenderResult` that golden parity compares — so
> FR-005/SC-004 byte-identity holds over the *pre-animation* render result and is unaffected by an
> in-flight clock. Clock continuity (FR-003/SC-002) is proven by the **separate survives-proof**
> (before/after render diff), not by the golden-parity scenes. Golden-parity scenes therefore carry no
> in-flight animation; the survives-proof is where the clock's continuation is asserted.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identity* change. Package **contents** change for the controls
  package (`src/Controls/**` — `Reconcile` promoted from parked-internal to wired on the render path,
  integrated with `Control.fs` `render`/`renderTree` and a framework-internal retained structure),
  `src/Controls.Elmish/**` (host loop integration), and `src/SkiaViewer/**` (the `dispatchHostMsg`
  repaint seam). Versions follow the normal merge bump (the libs incl. `FS.Skia.UI.Build` are bumped at
  merge); this spec pins no version. No legacy Charts package migration is involved.
- **Public contract impact**: **Primarily internal wiring.** `Reconcile` stays `module internal` (no
  promotion to public just to wire it). If a public `.fsi` entry point or option is introduced (e.g. a
  retained-render entry or partial-update mode) it MUST be additive, and the per-package surface
  baselines and the published `docs/api-surface` tree (emitted to `template/base/docs/api-surface/`)
  MUST be recaptured. Behavior of the existing host/render path changes internally even where its
  signatures do not — documented honestly in the affected `.fsi` doc comments.
- **State workflow impact**: The **internal render/redraw path changes** — the live loop now diffs the
  next tree against a retained previous tree and applies a patch instead of rebuilding. This is additive
  to the consumer MVU surface (`view`/`update`/`Init`/`Subscriptions` unchanged, no consumer rewrite).
  Identity-bearing UI state (focus via `ControlRuntime.FocusedControl`; a per-control animation clock)
  now survives unrelated re-renders. No new effects, subscriptions, or interpreter behavior; `host.Update`
  folding is unchanged.
- **Layout/rendering impact**: **Render/layout *scheduling* changes; rendered output does not.** Layout
  math and what the renderer draws are unchanged; the wired path re-measures/re-paints only changed
  subtrees (FR-004) and MUST produce output identical to a full rebuild of `next` (FR-005, golden
  parity). No Vulkan/Skia API or unsupported-environment-diagnostic behavior changes.
- **Evidence obligations**: Real evidence paths — golden-diff parity between the wired path and the
  full-rebuild baseline; the 067 property tests promoted to exercise the wired path (round-trip,
  determinism, totality, identity-at-rest); a **focus/animation-survives-an-unrelated-update** proof
  reusing the E1/090 responds-proof primitive; a **measured per-frame work reduction** for a localized
  update vs the redraw-the-world baseline; recaptured per-package and `docs/api-surface` baselines (if
  any public surface changes); and the regenerated `.claude` `fs-skia-reconciliation` skill matching its
  updated `.agents` source (`SkillSyncCheck`). The serialized six-target order passing on the change.
- **Unsupported scope**: Out of scope — E3 visual-state/style layer, E4 focus/keyboard-traversal model,
  E5 lookless slots; collection virtualization (a later layer once identity + partial updates exist);
  broad per-control animation *retargeting* beyond proving FR-003's clock survival; any rejected non-goal
  (XAML, data binding, dependency/attached properties, lookless `ControlTemplate` engine, CSS selectors);
  new package identity/version; release/platform/distribution changes.
- **Build-target impact**: `Dev` (render-path wiring + promoted property/golden tests),
  `GeneratedGuidanceCheck` and `TemplateCheck` (recaptured api-surface emitted into the template, if any
  surface changes), `GeneratedProductCheck` (generated-product currency), `EvidenceGraph` and
  `EvidenceAudit` (the new parity/survival/perf evidence), and `RefreshSurfaceBaselines` (regenerate the
  updated `fs-skia-reconciliation` `.claude` skill + any moved surface baselines) must change/run.
  `TargetMetadataDrift` / `SkillSyncCheck` enforce currency of the generated artifacts. Run only the
  gates `Route` prints.

## Success Criteria *(mandatory)*

- **SC-001**: A control unaffected by a model change retains its identity across the re-render — the
  wired render path matches it (`ChildKeep`/`Update`, not `Replace`) by key-first-then-positional rule —
  for **100%** of keyed controls whose `Kind` is unchanged across the two frames; a control whose `Kind`
  changed is `Replace`d (no false identity).
- **SC-002**: A focused control stays focused, and an in-flight per-control animation continues (does
  not restart), across an **unrelated** model update — proven by a captured survives-proof that a
  rebuild-every-frame (or inert) baseline fails to satisfy.
- **SC-003**: A localized single-control change re-measures/re-paints only the changed subtree, not the
  whole tree — demonstrated by a **measured reduction** in re-measured/re-painted node count (bounded by
  the changed subtree, not the total node count N) versus the full-rebuild baseline.
- **SC-004**: The wired path's rendered output is **identical** to a full rebuild of the same `next`
  model for every test scene — verified by golden-diff parity with zero diff.
- **SC-005**: The 067 invariants hold **on the wired path**: for ≥1,000 randomly generated `(prev, next)`
  model pairs the wired output is structurally equal to a full rebuild of `next` (round-trip), identical
  across repeated runs (determinism), never throws (totality), and successive identical frames produce
  no spurious re-render (identity-at-rest).
- **SC-006**: Duplicate-key (and other) reconciliation diagnostics produced on the live path are
  surfaced through the existing diagnostics channel, and the live path remains total in their presence.
- **SC-007**: The `fs-skia-reconciliation` skill's Disposition reflects the now-wired status and the
  `.claude` mirror matches its `.agents` source; an existing MVU consumer requires **zero** code changes
  to benefit (the `view` contract is unchanged).
- **SC-008**: `./fake.sh build -t Route` over the branch diff prints the expected escalation and **every
  printed gate passes** (the serialized six-target order is green when the change escalates to it).

## Key Entities

- **Previous / Next tree**: two lowered `Control<'msg>` values (existing IR; unchanged) — the inputs to
  the per-frame diff, as in feature 067. The "previous" is now the tree backing the **retained
  structure**, not a throwaway.
- **Retained structure**: the new framework-internal structure that persists between frames and that
  applying `NodePatch`/`ChildOp` mutates in place. It carries each control's **stable identity** (and is
  where focus / the animation clock attach). Never exposed as a mutable consumer surface.
- **Patch (`NodePatch`/`ChildOp`)**: the existing 067 operation set (`Keep`/`Replace`/`Update`;
  `ChildKeep`/`ChildMove`/`ChildInsert`/`ChildRemove`) — now consumed by the render path, not only by
  property tests.
- **Identity-bearing state**: per-control state that must survive a re-render when identity is preserved
  — at minimum `ControlRuntime.FocusedControl` and a per-control animation clock.

## Assumptions

- The keyed reconciler (`src/Controls/Reconcile.fsi`) already exists, is pure/total/deterministic, and
  is fully property-tested (feature 067) — 091 **wires and preserves invariants**, it does not design a
  new diff algorithm. The module's "deliberately parked / unwired" disposition in
  `.agents/skills/fs-skia-reconciliation` is the status this feature flips.
- The integration point is the full-tree render/redraw step the host runs today: `Control.renderTree`
  (`src/Controls/Control.fs:1014`) and the `SkiaViewer` repaint that recomputes `host.View` after each
  dispatched message (`src/SkiaViewer/SkiaViewer.fs:2364` `dispatchHostMsg`, also `:2320`/`:2437`).
- The wiring **replaces the internal render path** (it is not gated behind a per-call opt-in flag),
  while keeping the pure MVU `view` contract unchanged (roadmap §6.4). If implementation experience
  shows a safety flag is warranted, it is introduced additively and surface baselines recaptured — but
  the default behavior is the wired retained path.
- `Reconcile` stays `module internal`; wiring it does not require promoting it to public surface
  (consistent with 067 SC-005 / the `module internal SceneRenderer` precedent). New public surface, if
  any, is a small additive render/option seam.
- This feature **depends on E1/090** for a live, responsive host (so focus/animation survival is
  observable) and reuses 090's input→visible-change responds-proof as the survives-proof primitive.
- Per-control animation today is Scene-level (feature 073); 091 attaches an animation clock to a
  retained identity **only** as far as needed to prove FR-003 survival. Full animation retargeting is
  sequenced after E2 (roadmap §8, "Animation ↔ identity coupling").
- Versioning/packing follows the repo's normal merge flow (libs incl. `FS.Skia.UI.Build` bumped at
  merge); this spec does not pin a target version.

## Out of Scope

- **E3 — Visual-state / style layer** (style classes + state→style resolution over design tokens), **E4
  — Focus / keyboard-traversal / input-routing** (general tab order, traversal, focused-control key
  delivery for all kinds), and **E5 — Lookless slot composition.** These build *on top of* the retained
  identity 091 establishes and are separate future features.
- The **rejected redesign non-goals** (permanent, not deferred): XAML; a data-binding / observable
  property graph; attached/dependency properties with coercion/inheritance; a lookless `ControlTemplate`
  engine; CSS-selector styling.
- **Collection virtualization** — a later layer that can build on identity + partial updates once they
  exist; not delivered here.
- **Broad per-control animation retargeting** beyond proving FR-003's animation-clock survival; the full
  animation-on-retained-identity integration is sequenced after this feature.
- New package identities/versions, release/platform/distribution changes, and any tooling outside
  `src/Controls/**`, `src/Controls.Elmish/**`, `src/SkiaViewer/**`, the published api-surface, and the
  `.agents`/`.claude` skill spine touched by the disposition update.

## Dependencies

- **Feature 067 — the parked reconciler**: `src/Controls/Reconcile.fsi`/`.fs` (`module internal
  Reconcile`; `diff`/`apply`; `NodePatch`/`UpdatePatch`/`ChildOp`/`ReconcileResult`) and its
  Expecto/FsCheck property suite reached via `InternalsVisibleTo("Controls.Tests")` — the asset this
  feature wires and whose invariants it promotes to the live path.
- **Feature 090 (E1) — live interactivity**: `src/Controls.Elmish/**` (`runInteractiveApp`, the
  authored-binding dispatch, the focus-aware text seam) and the responds-vs-renders proof primitive — the
  prerequisite that makes a live host worth optimizing and makes focus/animation survival observable.
- The render/host seam: `src/Controls/Control.fs` (`render`/`renderTree`, per-node id derivation),
  `src/Controls/Types.fsi` (`ControlRenderResult`), `src/Controls/ControlRuntime.fsi`
  (`FocusedControl` and the durable UI state identity attaches to), `src/Controls.Elmish/**` (host loop).
- The live loop / repaint: `src/SkiaViewer/SkiaViewer.fs` (`dispatchHostMsg` repaint at `:2364`).
- Animation: feature 073 Scene-level `Tween`/`Animation`/`AnimationState` (`applyAt`) — the clock that
  attaches to a retained identity for the FR-003 survival proof.
- The skill/evidence spine: `.agents/skills/fs-skia-reconciliation` (Disposition update) regenerated into
  `.claude` via `RefreshSurfaceBaselines` / `SkillSyncCheck`; `build/Governance/**` gates.
- Source roadmap (in-repo, local file — no external `source-spec.md` snapshot required):
  `docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md` (§4 E2, §6 cross-cutting
  concerns, §8 risks, §9 recommendation).
