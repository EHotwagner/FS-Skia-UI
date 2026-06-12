# Phase 1 Data Model: Narrow Runtime Visual-State Updates

This feature adds **one** internal result type and **one** internal function to
`ControlRuntime`, and stores **one** extra ref in the live host loop. No new package,
no public surface, no MVU type change.

## Entity: `RuntimeStampResult<'msg>` (internal, `ControlRuntime`) — new

| Field | Type | Semantics |
|-------|------|-----------|
| `Stamped` | `Control<'msg>` | The runtime-visual-state-stamped tree — byte-identical to `applyRuntimeVisualState cur fresh` (the full oracle). |
| `RuntimeStateTouchedNodeCount` | `int` | How many nodes the targeted walk **rebuilt** this frame (the changed-state paths: affected identities + ancestor paths). `0` on a no-change frame; far below the node count on a localized change. |

| Aspect | Value |
|--------|-------|
| Visibility | internal (declared in `ControlRuntime.fsi`, hidden from consumers; tests reach it via `InternalsVisibleTo`). |
| Determinism | pure function of the inputs; identical inputs → identical result + count. |

## Entity: `ControlRuntime.applyRuntimeVisualStateTargeted` (internal) — new

```fsharp
val internal applyRuntimeVisualStateTargeted:
    prev: ControlRuntimeModel ->
    cur: ControlRuntimeModel ->
    prevStamped: Control<'msg> ->
    fresh: Control<'msg> ->
        RuntimeStampResult<'msg>
```

| Aspect | Value |
|--------|-------|
| Input | the previous + current runtime models; the previous frame's **stamped** tree; the current **fresh** (un-stamped) view tree (same structure as `prevStamped` on the model-unchanged path). |
| Output | `RuntimeStampResult` (the stamped tree + the touched-node count). |
| Rule | parallel walk; node reused (from `prevStamped`) when `finalState cur = finalState prev` and no descendant changed; else rebuilt from `fresh` with `finalState cur` stamped. `finalState M node = consumer-set state if non-Normal, else deriveVisualState M id`. |
| Fallback | a structural misalignment (child-count mismatch) signals the caller to use the full oracle (FR-006). |
| Reached by | `FS.Skia.UI.Controls.Elmish` within the assembly boundary; `Controls.Tests` via `InternalsVisibleTo`. |

**Validation rule**: `Stamped` is structurally equal to
`applyRuntimeVisualState cur fresh` (the full oracle) for every input (FR-005/SC-002);
`RuntimeStateTouchedNodeCount = 0` when `cur` and `prev` derive the same final state for
every node (SC-003); a consumer-set non-`Normal` node is never rebuilt by a derived
hover/focus change (FR-003/SC-004).

## Entity: Full-tree stamp oracle (internal, preserved)

`ControlRuntime.applyRuntimeVisualState` (`ControlRuntime.fs:233`) is **retained
unchanged** as the parity oracle and the fallback. It is no longer the normal live
route for a model-unchanged frame; the live host calls the targeted stamp there and the
oracle only on a model-changing / first / misaligned frame.

## Entity: Live host read/store set (internal, `Controls.Elmish`)

- `retained.Value.Root.Control` — the previous frame's **stamped** tree (the targeted
  base; already in the host loop).
- `viewFor size model` — the current **fresh** un-stamped view tree + the
  model-unchanged signal (feature 111).
- `assembleRuntimeModel (Some prev)` — the **current** runtime model.
- **NEW** `lastRuntimeModel: ControlRuntimeModel option ref` — the **previous** frame's
  runtime model, stored at the interpreter edge so the next frame computes the changed
  set. Seeded on the first stamp; updated each frame.

## Path selection (which frames take which route)

| Frame | Route |
|-------|-------|
| First frame (`retained = None`) | full oracle on the fresh tree (no prior stamped tree) |
| Model changed (`viewFor` rebuilt the tree) | full oracle on the fresh tree (whole tree rebuilt anyway) |
| Model unchanged (host-owned hover/focus/press; `viewFor` cache hit) + prior model present | **targeted stamp** (the Phase 4 hot path) |
| Targeted walk detects structural misalignment | fall back to the full oracle (FR-006) |

## State / lifecycle

No new persistent state beyond the `lastRuntimeModel` ref. The targeted stamp is pure
beyond reading the two trees + two models. Per-identity focus/animation/text state in
`RetainedRender.StateByIdentity` is unchanged; the retained step, layout, and routing
are unchanged (FR-009).
