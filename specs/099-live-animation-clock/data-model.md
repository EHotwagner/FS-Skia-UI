# Phase 1 Data Model: Animation Clock on Retained Identity (R4)

All entities are **internal** framework state carried on the retained UI model; none are
consumer-visible. Types are sketched for design; exact field names are settled in the
`.fsi` during implementation.

## Entities

### Per-identity animation clock

The accumulated-elapsed animation state attached to one retained control identity. It is
the generalized form of the existing carried slot
(`RetainedUiState.Animation`, `RetainedRender.fs:26`).

| Field | Type | Notes |
|-------|------|-------|
| `Anim` | `FS.Skia.UI.Scene.Animation` | The feature-073 multi-channel motion (Opacity/Transform/Color tweens), Start = the from-appearance, End = the target appearance. |
| `Elapsed` | `System.TimeSpan` | Accumulated injected delta; the **sole** time coordinate. |
| `Target` | `VisualState` | The visual state this clock is animating toward; used to detect a retarget. |

- Stored as the value of `RetainedUiState.Animation : <thisType> option` — `None` ⇒ the
  identity is at rest (no animation output).
- Keyed in `StateByIdentity : Map<RetainedId, RetainedUiState>` by the stable `RetainedId`
  (unchanged from E2).

**Validation / invariants**

- `Elapsed ≥ TimeSpan.Zero` always (advance never rewinds; non-positive delta is a no-op).
- **`defaultTransitionDuration = 150 ms`** (`EaseOut`, opacity/tint channel) is the single
  framework default applied when a tween is started/retargeted (research §R4). It is a
  pinned constant — not a per-control consumer knob — so the determinism goldens are
  reproducible: the same injected-delta sequence reaches the settled end after the same
  fixed number of frames.
- A clock is **settled** when `Animation.isSettled` holds for `Elapsed` (all tweens past
  `Duration`). A settled clock whose `Target = Normal` is **dropped** (set to `None`) so
  the identity returns to byte-identical at-rest output.
- Determinism: the clock is a pure function of the ordered sequence of injected deltas.

### Transition trigger

The event of R1's bridge flipping a control's derived `VisualState`. Not a stored entity —
it is **derived each frame** by comparing the stamped `VisualState`
(`ControlRuntime.applyRuntimeVisualState`/`deriveVisualState`) for an identity against the
carried clock's `Target`.

| Observed condition | Action |
|--------------------|--------|
| no clock + desired ≠ `Normal` | **start** a tween Normal-appearance → desired-appearance |
| clock + desired ≠ `Target` | **retarget** from the current sampled value → desired-appearance |
| clock + desired = `Target` | advance only (no retarget) |
| desired = `Normal`, clock settled | **drop** the clock (None) ⇒ at-rest restored |

### Injected frame delta

The per-frame `TimeSpan` delta supplied by the host loop's tick
(`InteractiveAppHost.Tick : TimeSpan -> 'msg option`). The **sole** time input to the
clock. Edge values:

| Delta | Effect |
|-------|--------|
| zero | clock unchanged (still deterministic) |
| negative / non-positive | **no-op** — never rewinds (host never emits these) |
| very large | tween **clamps** to its settled end (no overshoot past target) |

### Sampled animation output

The per-frame interpolated paint value produced by `Animation.applyAt Elapsed` over the
clock's `Anim` (opacity / transform / color), wrapped onto the identity's painted node in
the retained paint pass. **Absent** when the identity has no active clock — emitting **no**
animation attribute, byte-identical to the pre-R4 static render (identity-at-rest).

## Relationships

```
RetainedRender
  └── StateByIdentity : Map<RetainedId, RetainedUiState>     (carry + GC, E2)
        └── RetainedUiState.Animation : Clock option         (this feature fills it)
              ├── advanced by  ← Injected frame delta (wrapped host.Tick)
              ├── retargeted by ← Transition trigger (stamped VisualState flip, R1)
              └── sampled into ← Sampled output (Animation.applyAt, paint pass)
  liveIds filter drops the whole RetainedUiState (incl. its Clock) for removed identities
```

## State transitions (per identity, per frame)

1. **Advance** — if a clock exists, `Elapsed <- Elapsed + max(Zero, delta)`; recompute
   settled status.
2. **Trigger** — derive desired `VisualState`; start / retarget / drop per the trigger
   table above (retarget starts from the current sampled value).
3. **Sample & paint** — if a clock exists, paint the node through `Animation.applyAt
   Elapsed`; else paint as static (no attribute).
4. **Carry / GC** — `StateByIdentity` is filtered by `liveIds`; a removed identity's clock
   is gone next frame.

## Invariants asserted by tests

- **Determinism (FR-006 / SC-004)**: identical injected-delta sequences ⇒ identical
  sampled output; no wall-clock consulted.
- **Identity-at-rest (FR-005 / SC-003)**: no active clock ⇒ no animation attribute,
  byte-identical to the pre-R4 golden, zero at-rest recompute/output count.
- **Survival (FR-004 / SC-002)**: a clock keyed by a stable `RetainedId` continues from
  its prior `Elapsed` across a sibling-shifting re-render and completes, driven through the
  real seam (replacing the hand-seeded precondition).
- **GC (FR-007 / SC-005)**: a removed identity's clock is absent the following frame.
- **Scoped repaint (FR-010 / SC-006)**: advancing a clock repaints only its subtree; no
  whole-tree repaint/re-measure (R2 preserved).
- **Multi-clock independence (FR-010 / SC-006)**: each `RetainedId` advances its own clock;
  two identities at different `Elapsed` advance by their own injected delta and one clock
  completing/dropping does not perturb another's `Elapsed` or sampled output (asserted in
  T013).
