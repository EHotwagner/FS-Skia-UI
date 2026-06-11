# Implementation Plan: Incremental Measure / Partial Re-Layout (R2)

**Branch**: `097-incremental-partial-relayout` | **Date**: 2026-06-11 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/097-incremental-partial-relayout/spec.md`

## Summary

R2 finishes E2's partial-update promise. Today the live retained path (091+092) makes
**paint** partial — `Reconcile.diff` produces a patch, the retained walk reuses unchanged
painted subtrees, and `WorkReductionRecord` proves `RecomputedNodeCount < BaselineNodeCount`.
But **measure** is still O(whole-tree) every frame: `RetainedRender.step` calls full-tree
`ControlInternals.evaluateLayout size next` (`src/Controls/RetainedRender.fs:141`), which runs
full `Layout.evaluate` (`src/Layout/Layout.fs` `evaluate`), and the paint-reuse decision even
*depends* on that full re-measure (`box = pr.Fragment.Box`, `RetainedRender.fs:210`). The public
`Layout.evaluateIncremental` (`src/Layout/Layout.fsi:10`) already has the right dirty-set
signature but its body is a **stub** (`src/Layout/Layout.fs` — calls full `evaluate`, stamps
`Revision + 1`, echoes `changedNodeIds` into `Invalidated`).

This feature:

1. **Makes `Layout.evaluateIncremental` a genuine incremental evaluator** (FR-001): given the
   previous `LayoutResult`, a dirty set `changedNodeIds: LayoutNodeId list`, available space, and
   the new root, it re-measures only the dirty nodes and their conservatively-propagated flex
   containers, reuses cached bounds for everything else, and returns `Bounds` **byte-identical** to
   a full `Layout.evaluate`. The **public signature is unchanged** — only the body changes.
2. **Reports the honest re-measured set** in `LayoutResult.Invalidated` (FR-001a) — the requested
   dirty set *after* flex-line + fixed-size-ancestor propagation (FR-004), replacing today's
   verbatim echo. `Revision` still advances (`previous.Revision + 1`). Only `Bounds` are
   constrained to byte-identity; `Invalidated`/`Revision` are incremental metadata.
3. **Derives the layout-dirty set directly from `ReconcileResult.Patch`** (FR-003): a node is
   layout-dirty iff its `UpdatePatch.AttrChanges` sets/removes an attr whose `Category` is
   `AttrCategory.Layout`, **or** it carries any `ChildOp` (`ChildInsert`/`ChildRemove`/`ChildMove`).
   `Keep`, `Replace` (handled as new), and non-layout `Update` do not mark a node measure-dirty.
   Classification is driven by the existing `AttrCategory.Layout` tag — never a hand-maintained
   attribute-name list.
4. **Propagates dirt conservatively** (FR-004): a dirty flex child dirties its **whole nearest flex
   container/line**; dirt then climbs to the **first ancestor whose own `Size` is explicit /
   content-independent** (a concrete `LayoutIntent.Size`, not auto/content-derived) and stops —
   that ancestor redistributes internally without moving its own box, so its ancestors stay clean.
   A fully content-sized chain propagates to the root (the correct result).
5. **Maintains a per-node measure/bounds cache keyed by retained identity** (FR-002) on the
   internal retained tree (`RetainedNode`), so an unchanged subtree's measure + bounds survive
   across frames and are reused (translated if an ancestor moved) without recomputation. The cache
   is **pure** — keyed on intrinsic-measure inputs, no clock/randomness/escaping mutation
   (constitution III).
6. **Swaps the incremental evaluator onto the render path** (FR-005): a new **internal** incremental
   variant of `ControlInternals.evaluateLayout` threads the previous `LayoutResult`/cache and the
   patch-derived dirty set; `RetainedRender.step` calls it instead of the unconditional full
   `evaluateLayout`, preserving the existing reuse-driven paint walk and every E2 invariant.
7. **Extends `WorkReductionRecord`** (FR-006) with a **re-measured node count** alongside the
   existing `RecomputedNodeCount`/`ShiftedNodeCount`, so partial measure is measurable: strictly
   below baseline for a localized edit, equal to baseline for a genuine whole-tree relayout, zero
   for an empty patch.

A hard **equivalence invariant** (FR-007) — `evaluateIncremental` (carrying its cache) byte-identical
to full `evaluate` over randomized trees and cumulative edit sequences — is the gate that makes the
fast path adoptable. Output rendering stays byte-identical to the pre-R2 build for **every** frame
(FR-008). It is architecture- and non-goal-preserving (FR-009): no virtualization, no new layout
algorithm, no new public layout type, no consumer-contract change.

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Existing **`FS.Skia.UI.Layout`**
(`Layout.evaluate`/`evaluateIncremental`, `LayoutResult`, `LayoutNode`, `LayoutIntent`,
`ComputedBounds`) and **`FS.Skia.UI.Controls`** (`Reconcile` patch types, `AttrCategory`,
`ControlInternals.evaluateLayout`/`toLayout`, `RetainedRender`, `WorkReductionRecord`).
**Testing**: Expecto (`Layout.Tests`, `Controls.Tests`), FsCheck property tests for the
equivalence invariant (≥1000 generated `(tree, edit-sequence)` cases incl. cumulative
cache-staleness sequences), FSI transcript for the public `evaluateIncremental`, real in-repo
readiness artifacts under `specs/097-incremental-partial-relayout/readiness/`.
**Target Platform**: Windows and Linux (`net10.0`).

**Key grounding facts (verified in source):**

- `Layout.evaluateIncremental` is **public** in `src/Layout/Layout.fsi:10` with signature
  `previous: LayoutResult -> changedNodeIds: LayoutNodeId list -> available: AvailableSpace ->
  root: LayoutNode -> LayoutResult`. Its body in `src/Layout/Layout.fs` is the stub
  (`let result = evaluate available root in { result with Revision = previous.Revision + 1L;
  Invalidated = changedNodeIds |> List.distinct }`). R2 changes **only this body**.
- `LayoutResult = { Bounds: ComputedBounds list; Diagnostics: LayoutDiagnostic list;
  Invalidated: LayoutNodeId list; Revision: int64 }` (`src/Layout/Types.fsi`). `ComputedBounds =
  { NodeId: LayoutNodeId; Bounds: LayoutBounds; Visibility: LayoutVisibility }`. **No field is
  added** — the cache lives Controls-side.
- `LayoutNodeId = string`. `LayoutNode = { Id; Intent; Measure: ContentMeasure option;
  Content: Scene option; Children: LayoutNode list }`. `LayoutIntent.Size: LayoutSize` where
  `LayoutSize = { Width: float option; Height: float option }` — a **fixed-size** node (FR-004) is
  one whose `Size` is concrete on the relevant (constraining) axis (`Some`), not content-derived
  (`None`).
- `ControlInternals.evaluateLayout size control` (`src/Controls/Control.fs:1219`) calls
  `toLayout "0" control` → full `Layout.evaluate available root`, then projects
  `result.Bounds` into `boundsById : Map<LayoutNodeId, Rect>` and returns `root, boundsById`.
  The dirty set is in the **`LayoutNodeId` (layout path) domain** `toLayout "0"` mints, the same
  domain `evaluateIncremental` and `LayoutResult.Bounds` use — never the `ControlId`/`RetainedId`
  identity domains.
- `Reconcile` (`src/Controls/Reconcile.fsi`) is `module internal`. `NodePatch = Keep | Replace |
  Update of UpdatePatch`; `UpdatePatch = { AttrChanges: AttrChange list; ContentChange;
  AccessibilityChange; Children: ChildOp list }`; `AttrChange = AttrSet of Attr | AttrRemoved of
  name`; `ChildOp = ChildKeep | ChildMove | ChildInsert | ChildRemove`. The patch is the ready-made
  dirty source.
- `Attr = { Name: string; Category: AttrCategory; Value: AttrValue }` and `AttrCategory` includes
  `Layout` (`src/Controls/Types.fsi`) — the authoritative classifier. R2 reads `attr.Category =
  AttrCategory.Layout` off the `AttrSet`/`AttrRemoved` payloads.
- `RetainedRender.step` (`src/Controls/RetainedRender.fs:130+`) already: diffs (`Reconcile.diff`),
  calls `ControlInternals.evaluateLayout size next` (line 141), tracks `mutable recomputed/
  changedBound/shifted` counters at the interpreter edge, and reuses subtrees when
  `box = pr.Fragment.Box && not themeChanged` (line 210). `WorkReductionRecord` (internal,
  `RetainedRender.fsi:70`) = `{ BaselineNodeCount; RecomputedNodeCount; ChangedSubtreeBound;
  ShiftedNodeCount }`; `RetainedNode` (internal) = `{ Identity; Control; Fragment; Children }` with
  `RenderFragment = { OwnScene; SubtreeScene; Box }` — the measure cache rides here.
- `themeChanged` already forces a full repaint (`RetainedRender.fs:144`); theme is a **paint**
  concern, not a measure concern — a theme-only change does **not** dirty measure (FR-008 edge).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Tier**: **Tier 2 (internal change) with one public-behavior nuance.** R2 changes the **body** of
the already-public `Layout.evaluateIncremental` (signature unchanged) and the observable value of
`LayoutResult.Invalidated` (stub echo → honest post-propagation set, FR-001a). No public signature,
type, or field is added or moved; the measure cache and extended `WorkReductionRecord` are
**internal**. Surface-area baselines (`FS.Skia.UI.Layout`, per-package, cross-package) are
**unchanged** (SC-006). Per the spec's Build-target section, if no `.fsi` moves the change routes to
the lighter inner-loop tier plus the Layout/Controls determinism tests; **`Route` is authoritative**
— run it first and run exactly the gates it prints. The observable-`Invalidated` change is covered by
the new equivalence + `Invalidated`-reporting tests, not by a baseline move.

> **Tier ruling (maintainer decision, 2026-06-11).** The constitution defines Tier 2 as "internal
> cleanup with **no behavioral change**" and Tier 1 as any change that "alters observable behavior
> covered by existing specs"; R2's honest-`Invalidated` value change (FR-001a) is an observable change
> to a public function, which surfaces a genuine classification tension. **Resolution: this stays Tier
> 2.** The decisive Tier-2 markers hold — no `.fsi` symbol is added or moved and every surface-area
> baseline (`FS.Skia.UI.Layout`, per-package, cross-package) is committed **unchanged** (SC-006) — and
> the behavior delta is *fully disclosed* (FR-001a, FR-001) and *test-covered* (T014 equivalence, T015
> `Invalidated`-honesty), so it cannot regress silently. `Route` remains authoritative for the gate set;
> if any `.fsi` is in fact forced to move during implementation, this ruling lapses and the change
> escalates to Tier 1 / the serialized six-target path. Recorded here so merge-readiness review does not
> re-litigate the label.

**Principle compliance:**

- **I (Spec→FSI→Tests→Impl)**: `evaluateIncremental` is already drafted in `Layout.fsi`; R2 exercises
  the real (no-longer-stub) behavior via an FSI transcript and the equivalence property suite
  **before** finalizing the `.fs` body. ✅
- **II (Visibility in `.fsi`)**: No `.fsi` symbol added or moved. The measure cache rides the
  **internal** `RetainedNode`/`RenderFragment`; the extended `WorkReductionRecord` field is internal;
  the incremental `evaluateLayout` seam is `module internal ControlInternals`. Tests reach internals
  via the existing `InternalsVisibleTo` (`Controls.Tests`). No access modifiers in `.fs`. ✅
- **III (Idiomatic simplicity)**: Plain recursion over `LayoutNode.Children` / the patch tree, a
  `Map<LayoutNodeId, _>` cache, a `match` on `NodePatch`/`ChildOp`/`AttrCategory`. Any `mutable`
  accumulator (re-measure counter) is confined to the existing `RetainedRender.step` interpreter
  edge with a one-line disclosure comment. No SRTP/reflection/custom operators/type providers. ✅
- **IV (MVU boundary)**: The incremental evaluator and the measure cache are **pure** functions of
  `(previous LayoutResult, dirty set, new tree)`; they own no mutable state beyond the per-step
  counters/cache already confined in `RetainedRender.step` (the interpreter edge). The existing
  `LayoutWorkflowModel`/`Msg`/`Effect` surface is **untouched** — no new effect/command/subscription.
  ✅
- **V (Synthetic disclosure)**: None planned. The equivalence invariant is property-tested over
  **generated** (not canned) trees and edit sequences; byte-identity is structural `Bounds`/`Scene`
  equality on the real evaluators; the metric is read from the real wired `step`. No `[S]` expected.
- **VI (Test evidence)**: Failing-first — the equivalence property suite fails against today's stub
  only in the *metric*/`Invalidated` sense (stub `Bounds` already match because it cheats by calling
  full `evaluate`), so the **decisive** failing-first tests are (a) the re-measure-count assertions
  (FR-006: localized < baseline, whole-tree = baseline, empty = 0 — all fail against the
  always-full-measure stub) and (b) `Invalidated` = honest post-propagation set (FR-001a: fails
  against the verbatim-echo stub). The equivalence suite then guards that making measure partial
  never diverges `Bounds`. ✅
- **VII (Observability)**: The evaluator is total; a dirty node id absent from the cache falls back to
  a full re-measure of that subtree (conservative, never silent divergence). No new failure path or
  diagnostic; existing layout `Diagnostic` surfacing is preserved verbatim. ✅

### Repository Governance Decisions

- **Template ownership**: N/A — no `.template.config/template.json`, sample, skill, or
  command-surface change. R2 is internal framework behavior; a generated project consuming the
  retained host gains partial measure **automatically** with no scaffold change. The `dotnet new`
  template only refreshes its package **pins** on merge via the standard version-bump flow (separate
  track).
- **Dependency impact**: N/A — no new package. `Directory.Packages.props`, `docs/dependencies.md`,
  and `DependencyReport` are unaffected (no dependency added or version-floated).
- **Command-surface impact**: No new gate, target, or wrapper. The equivalence property test is added
  to the existing `Layout.Tests`/`Controls.Tests` projects; `WorkReductionRecord` assertions extend
  the existing `Controls.Tests`. `Route` is authoritative: the change touches `src/Layout/**` +
  `src/Controls/**`; if no `.fsi` signature moves it routes to the inner-loop tier (`Dev`) plus the
  layout/controls determinism tests. If any `.fsi` is forced to change (not intended), it escalates
  to the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck →
  EvidenceGraph → EvidenceAudit` path. FAKE-backed targets run **sequentially** (shared `.fake`
  state). Run `./fake.sh build -t Route` first and run exactly the gates it prints.
- **Generated project impact**: None to default/minimal generated contents or generated `Dev`
  behavior. The incremental evaluator and measure cache are internal to `FS.Skia.UI.Layout` /
  `FS.Skia.UI.Controls`; a generated project consuming `runInteractiveApp` gains the partial-measure
  speedup with no new selected-Controls guidance and no placeholder/excluded-history scan delta.
- **Evidence paths**: All under `specs/097-incremental-partial-relayout/readiness/`:
  - `partial-remeasure.md` — US1/SC-001: a localized leaf edit (content-only, no `AttrCategory.Layout`
    change, no child op) re-measures **only** its enclosing flex-line subtree via the extended
    `WorkReductionRecord` re-measure count, with a `Scene` byte-identical to a full-rebuild frame.
  - `equivalence-property.md` — US2/FR-007/SC-002: `evaluateIncremental` (carrying its cache)
    byte-identical to full `evaluate` over ≥1000 generated `(tree, edit-sequence)` cases, including
    cumulative multi-edit sequences that stress cache staleness; zero divergences.
  - `remeasure-metric.md` — US3/FR-006/SC-003: `WorkReductionRecord` reports a re-measure reduction
    AND a re-paint reduction for a localized update, a re-measure count **equal to baseline** for a
    genuine whole-tree relayout, and **zero** for an empty patch.
  - `dirty-derivation.md` — SC-004: a layout attr (`AttrCategory.Layout`) dirties the nearest flex
    line and propagates up to (and including) the first fixed-`Size` ancestor and **stops** (subtree
    under a fixed-`Size` container does not dirty its ancestors); a fully content-sized chain dirties
    up to the root; a non-layout attr (content/style/state/`visualState`) dirties **no** measure.
  - `invalidated-honest.md` — SC-008: post-incremental `LayoutResult.Invalidated` reports the actual
    re-measured set (⊋ the single requested node, bounded by the fixed-size-ancestor subtree) for a
    localized edit, and empty for an empty patch — not the verbatim requested set.
  - `byte-identity-at-rest.md` — FR-008/SC-005: an at-rest frame (all-`Keep` patch) re-measures
    nothing (re-measure count 0) and renders `Scene`-byte-identical to the un-incremental build;
    every tested frame (localized + whole-tree) is byte-identical to the pre-R2 build.
  - `e2-invariants.md` — SC-007: all E2 determinism invariants
    (`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`, `Keep → reuse`, first-frame full
    paint, `KeyCollision` diagnostics) still hold on the incremental-layout-wired path.
  - Surface baselines (`FS.Skia.UI.Layout`, per-package, cross-package) committed **unchanged** as the
    public-contract evidence (SC-006).
- **`.fsi` / contract impact**: **No `.fsi` symbol changes.** `Layout.evaluateIncremental` keeps its
  signature (body-only change); `LayoutResult` keeps its shape (`Revision`/`Invalidated` already
  present — only `Invalidated`'s **runtime value** changes, FR-001a). The measure cache rides the
  internal `RenderFragment`/`RetainedNode`; the extended `WorkReductionRecord` field is internal.
  Compatibility: a direct caller of `evaluateIncremental` now gets real partial measure + an honest
  `Invalidated` instead of a full re-measure + echoed input — `Bounds` are unchanged, so no migration
  is required. **Escalation guard**: if equivalence/caching turns out to need a public signature or
  `LayoutResult` field, that escalates to a surface-baseline recapture; the design intent (cache on
  the retained node, reuse `LayoutResult.Bounds`) is precisely to **avoid** any public change — see
  research §R5.
- **MVU/effect boundary**: `Model`/`Msg`/`Effect`/`init`/`update` = **unchanged** (the existing
  `LayoutWorkflowModel` and the host's pointer/focus reducers own all state). New code is **pure**:
  `evaluateIncremental` (previous + dirty set + tree → result), the dirty-set derivation (patch →
  `LayoutNodeId` set), and the measure-cache reuse (cache + tree → bounds). Interpreter edge = the
  existing `RetainedRender.step` closure, which already confines the monotonic id counter and work
  counters; R2 adds the re-measure counter and the cache there. No new effect/command/subscription.
- **Synthetic evidence**: None planned. The equivalence invariant uses **generated** trees/edits
  (FsCheck), not hardcoded fixtures; the metric is read from the real wired `step`; parity is
  structural `Bounds`/`Scene` equality on the real evaluators. If any real-evidence path proves
  infeasible at implementation time, the task is marked `[S]` with full Principle-V disclosure — not
  expected.
- **Test evidence**: Failing-first: (a) re-measure-count assertions (localized < baseline, whole-tree
  = baseline, empty = 0) that fail against the always-full-measure stub; (b) `Invalidated` =
  honest post-propagation set, failing against the verbatim-echo stub; (c) the equivalence property
  suite (≥1000 `(tree, edit-sequence)` cases, cumulative cache-staleness) guarding `Bounds`
  byte-identity; (d) dirty-derivation unit cases (`AttrCategory.Layout` vs non-layout; fixed-size
  ancestor stop; content-sized chain to root; each `ChildOp`); (e) at-rest `Scene` byte-identity +
  zero re-measure; (f) E2-invariant re-checks on the wired path; (g) an FSI transcript exercising the
  public `evaluateIncremental`. Governance: surface baselines committed unchanged.
- **Observability**: No new diagnostics or log paths. The evaluator is total — a cache miss or an
  unrecognized dirty id falls back to a full re-measure of that subtree (conservative; the
  equivalence invariant guarantees correctness, never silent divergence). Existing layout
  `Diagnostic` output is preserved verbatim; the re-measure count surfaces through the existing
  internal `WorkReductionRecord` already consumed by the determinism tests.
- **Deferred scope**: Out — virtualization / windowing of large collections (§6.2, a later layer);
  the runtime visual-state bridge (R1, shipped as 096); binding-aware unkeyed dispatch (R3); the live
  animation clock + animated transitions (R4); general navigation-key delivery (R5); any new layout
  algorithm or new public layout type; any change to computed geometry (R2 is performance-and-metric
  only); and the permanent non-goals (CSS selectors, attached/dependency properties, lookless
  templates, data binding).

**Post-design re-check**: No new violation introduced by Phase 1. The design changes one public
function **body** (signature preserved), adds an internal measure cache on an existing internal type,
an internal dirty-set derivation, an internal incremental `evaluateLayout` seam, and one internal
`WorkReductionRecord` field; all principle checks above still hold, and the public surface baseline is
unchanged. ✅

## Project Structure

```
specs/097-incremental-partial-relayout/
├── spec.md
├── plan.md                      # this file
├── research.md                  # Phase 0 — decisions & rationale
├── data-model.md                # Phase 1 — entities, dirty-set & cache model
├── quickstart.md                # Phase 1 — how to exercise incremental layout
├── contracts/
│   └── incremental-layout.md    # evaluateIncremental behavior + dirty-set derivation + cache contract
├── checklists/
│   └── requirements.md          # (existing) spec quality checklist
└── readiness/                   # evidence artifacts (created during implement)

src/Layout/                      # FS.Skia.UI.Layout (signature unchanged)
├── Layout.fsi                   # (unchanged) evaluateIncremental already declared
└── Layout.fs                    # stub evaluateIncremental body -> genuine incremental evaluator
                                 #   + per-node measure-reuse helper (pure)

src/Controls/                    # FS.Skia.UI.Controls (internal-only additions)
├── Control.fs                   # + internal incremental variant of ControlInternals.evaluateLayout
│                                #   (threads previous LayoutResult + cache + dirty set)
├── Reconcile.fs(i)              # (consumed only) patch is the dirty source — no change to diff
├── RetainedRender.fsi           # + internal re-measure count field on WorkReductionRecord;
│                                #   measure cache rides RenderFragment/RetainedNode (internal)
└── RetainedRender.fs            # step: derive dirty set from patch, drive evaluateIncremental
                                 #   instead of full evaluateLayout, carry/translate cached bounds,
                                 #   count re-measured nodes (interpreter-edge mutable)

tests/Layout.Tests/             # equivalence property suite (tree x edit-sequence, cumulative);
                                 #   dirty-derivation + fixed-size-ancestor unit cases; FSI transcript
tests/Controls.Tests/           # re-measure metric assertions, at-rest byte-identity,
                                 #   dirty-set-from-patch, E2-invariant re-checks on the wired path
```

## Phase 2 (next): `/speckit-tasks`

Phase 2 produces `tasks.md` + `tasks.deps.yml` (story-grouped, `skillist`-tagged, acyclic). This
plan stops after Phase 1 design.
