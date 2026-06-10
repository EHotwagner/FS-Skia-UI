# Phase 1 Data Model: Retained-Tree Reconciliation on the Render Path (091 / E2)

All types below are **framework-internal**. The consumer-facing surface is unchanged: the only contract a
consumer writes is the existing pure `view : 'model -> Control<'msg>` (and `update`/`Init`/`Subscriptions`).
No observable/mutable view-model, data binding, or dependency property is introduced (permanent roadmap
non-goals).

## Existing entities (unchanged — inputs/reused)

| Entity | Where | Role in 091 |
|--------|-------|-------------|
| `Control<'msg>` (`Kind`, `Key: ControlId option`, `Attributes`, `Children`, `Content`, `Accessibility`) | `Types.fsi:242` | The lowered IR. The **previous** tree is now retained; the **next** tree is `host.View size model`. Unchanged. |
| `ControlId = string`, `ControlKind = string` | `Types.fsi:7/9` | Control identity / kind. Unchanged. |
| `ControlRenderResult<'msg>` (`Scene`, `Layout`, `Bounds`, `Diagnostics`, `EventBindings`, `NodeCount`) | `Types.fsi:285` | Produced today by a full `renderTree`; in 091 it is produced from the retained structure for a localized change (output identical). Unchanged shape. |
| `NodePatch`/`UpdatePatch`/`ChildOp`/`FieldChange`/`AttrChange`/`ReconcileResult` | `Reconcile.fsi` | The 067 operation set — now **consumed by the render path**, not only by property tests. Unchanged. |
| `ControlDiagnostic` (incl. `KeyCollision`) | `Types.fsi:136` | The existing diagnostics channel; `ReconcileResult.Diagnostics` flow through it at the wiring boundary. Unchanged. |
| `ControlRuntimeModel.FocusedControl : ControlId option` | `ControlRuntime.fsi:42` | Identity-bearing state that must survive an unrelated re-render. Unchanged type; re-keyed to the stable identity. |
| `AnimationState<'a>` / `Animation` / `applyAt` (TimeSpan clock) — instantiated as `AnimationState<Transform>` in `RetainedUiState` | `Scene/Animation.fsi` | The per-control animation clock for the FR-003 survives-proof. Reused as-is. |
| Host loop refs: `pointerState`, `focusedText`, `textModels`, `latest` | `ControlsElmish.fs:331` | The interpreter-edge home where the retained structure also lives. |
| `currentModel`/`currentScene` | `SkiaViewer.fs:2320/2364/2437` | The repaint seam; retained diff replaces the unconditional re-render. |

## New entities (framework-internal — `module internal RetainedRender`)

### `RetainedNode<'msg>` — one retained control + its cached render
```
type internal RetainedNode<'msg> =
    { Identity: RetainedId                 // stable across frames for a matched (ChildKeep/Update) node
      Control: Control<'msg>               // the prev lowered control this fragment was built from
      Fragment: RenderFragment             // cached LayoutNode + Scene fragment + computed Rect
      Children: RetainedNode<'msg> list }  // retained subtree, mirroring Control.Children order
```
- **Validation:** `Children` order mirrors the lowered `Control.Children`; `Identity` is unique within a
  sibling list. A `Fragment` is only reused when its node's patch is `Keep`/`ChildKeep`.

### `RetainedId` — the stable identity the diff confers
```
type internal RetainedId = RetainedId of uint64   // monotonic within a host loop; NOT the path-derived ControlId
```
- **Distinction:** `RetainedId` is the **diff-conferred** identity (survives a positional shift); the
  existing path-derived `ControlId` (`Control.fs:1052`) is *not* stable across shifts and is the reason
  today's focus/text state resets. Per-control state (focus, animation clock, text model) re-keys to
  `RetainedId`.
- **Lifecycle:** minted on `ChildInsert`/`Replace`/initial build; **carried over** on `ChildKeep`/`Update`;
  **dropped** on `ChildRemove`/`Replace` (so a replaced node does not spuriously retain identity — SC-001
  negative).
- **Determinism:** minted from a per-host monotonic counter seeded at host start (no `Date.now`/randomness),
  so identical frame sequences mint identical ids across runs/processes (SC-005).

### `RenderFragment` — the reusable unit of measure/paint
```
type internal RenderFragment =
    { Layout: LayoutNode                   // cached Yoga result for this node + subtree
      Scene: SceneNode                     // cached painted fragment
      Bounds: (ControlId * Rect) list }    // contribution to ControlRenderResult.Bounds
```
- **Reuse rule:** an unchanged subtree (`Keep`/`ChildKeep`) contributes its cached `Fragment` verbatim;
  `Update` recomputes the node's own measure/paint and recurses; `Replace`/`ChildInsert` build fresh.

### `RetainedRender<'msg>` — the per-frame retained root + identity map
```
type internal RetainedRender<'msg> =
    { Root: RetainedNode<'msg>
      NextId: uint64                                  // monotonic identity counter
      StateByIdentity: Map<RetainedId, RetainedUiState> }  // focus/animation/text keyed by STABLE identity

and internal RetainedUiState =
    { Animation: AnimationState<Transform> option     // per-control clock (FR-003 survives-proof; D4)
      Text: TextInputModel option }                   // re-keyed text-input state (formerly path-keyed)
```
- Focus is the consumer-model `ControlRuntime.FocusedControl`; 091 remaps the *lookup* to `RetainedId`, it
  does not move focus out of the model.

## State transition — one frame on the wired path

```
prev: RetainedRender<'msg>        next model -> next: Control<'msg> = host.View size model
        │
        ▼
  result = Reconcile.diff prev.Root.Control next        // total; never throws
        │
        ├─ result.Diagnostics ──► surface via existing ControlDiagnostic channel (FR-007); never dropped
        ▼
  apply patch to prev, producing next RetainedRender:
        Keep / ChildKeep      → reuse Fragment;          carry RetainedId; reuse StateByIdentity entry
        Update                → recompute node measure/paint; recurse Children; carry RetainedId
        Replace               → build fresh Fragment;     mint RetainedId; DROP old state (no false identity)
        ChildInsert           → build fresh Fragment;     mint RetainedId
        ChildRemove           → drop Fragment + RetainedId + its StateByIdentity entry
        ChildMove             → reorder cached Fragment;   carry RetainedId
        │
        ▼
  ControlRenderResult<'msg>  (identical to renderTree theme size next — golden parity, FR-005)
        │
        ▼
  store next RetainedRender into the host ref (becomes prev for the following frame)
```

**Correctness-wins fallback (FR-005 vs FR-004/FR-006):** if assembling the partial result would diverge
from `renderTree theme size next`, the path falls back to a full `renderTree next` and rebuilds the
retained structure from it — output is always the full-rebuild-equivalent.

**Golden-parity vs animation continuity (FR-005/SC-004 vs FR-003/SC-002):** the per-control animation
clock (`RetainedUiState.Animation`) is retained-only state applied **post-render at the Scene layer**
(feature 073), i.e. outside the `ControlRenderResult` golden parity compares. Byte-identity therefore
holds over the pre-animation render result; clock continuity is asserted by the separate survives-proof,
not by the (animation-free) golden-parity scenes.

## Identity-bearing-state survival (the US1/US2 proof shape)

```
frame N:    focus = K ; K.animation.Elapsed = t
            unrelated model change (region ≠ K)
frame N+1:  diff matches K (ChildKeep/Update, NOT Replace)  ─► K keeps RetainedId
            ► ControlRuntime.FocusedControl still resolves to K   (focus survives, SC-002)
            ► K.animation.Elapsed = t + Δ   (continues, does NOT reset, SC-002)
   baseline (rebuild-every-frame): K's path id shifts ─► state lookup misses ─► focus/clock reset (proof fails)
```

## Work-reduction metric (the US3/SC-003 measurement)

```
localized single-leaf change:
  full-rebuild baseline  → N nodes re-measured/re-painted
  wired retained path    → only the changed subtree's nodes re-measured/re-painted  (bounded by subtree, not N)
  record: readiness/partial-update/work-reduction.txt  (baselineCount, wiredCount, subtreeBound)
```

## Invariants carried onto the wired path (FR-006 / SC-005)

| Invariant | Statement on the wired path |
|-----------|-----------------------------|
| Totality | The wired frame step never throws for any `(prev, next)` (incl. duplicate-key, empty-tree) — collisions become `KeyCollision` diagnostics. |
| Determinism | Identical frame inputs produce identical `ControlRenderResult` + identical minted `RetainedId`s across runs/processes. |
| Identity-at-rest | Structurally identical successive frames produce a `Keep` no-op patch — no re-measure/re-paint, no spurious id churn. |
| Round-trip | The retained-apply output is structurally equal to a full rebuild of `next` (golden parity, zero diff). |
