# Contract: Host Animation Seam (Tick → advance, flip → retarget)

**Scope**: `src/Controls.Elmish/ControlsElmish.fs` (`runInteractiveApp` internals) and
`src/Controls/RetainedRender.fs[i]`. The **public** `InteractiveAppHost` /
`runInteractiveApp` surface is **unchanged**; this contract governs internal wiring driven
by the already-present `Tick` delta.

## C1 — Tick advances the live clocks, then delegates

The host wraps `host.Tick`. Given a per-frame `delta : TimeSpan`:

1. advance **every** live per-identity clock in `retained.Value.StateByIdentity` by
   `delta` (see C3) **before** the next `renderRetained`;
2. return `host.Tick delta` as the consumer message (delegation is total — the consumer's
   tick is never swallowed and never double-dispatched).

- The advance is the **only** writer of the carried `Animation` slot from the host loop;
  there is no `Date.now`/wall-clock read anywhere on this path.
- When no identity has an active clock, the advance is a no-op and the wrapper is
  observably identical to passing `host.Tick` through unchanged.

## C2 — A stamped VisualState flip starts or retargets a tween

Each frame, `applyRuntimeVisualState` stamps each identity's derived `VisualState`
(R1, unchanged). For each live identity, compare the desired state to the carried clock's
`Target`:

| Condition | Result |
|-----------|--------|
| no clock, desired ≠ `Normal` | **start** a tween: Normal-appearance → desired-appearance, `Elapsed = 0`, `Target = desired` |
| clock, desired ≠ `Target` | **retarget** from the **current sampled value** → desired-appearance; `Target = desired` (no snap to start) |
| clock, desired = `Target` | advance only |
| desired = `Normal`, clock settled | **drop** the clock (`None`) — at-rest restored |

- The default tween uses the single framework duration + easing (research §R4); there is
  **no** consumer knob and **no** new authoring API.
- Retarget continuity: a transition triggered mid-flight re-aims from the current sampled
  value, never resetting to the start (spec edge case).

## C3 — Advance is total and deterministic

`advance (delta : TimeSpan) (clock)` :

- `delta ≤ TimeSpan.Zero` ⇒ **no-op** (clock unchanged; never rewinds);
- `delta > Zero` ⇒ `Elapsed <- Elapsed + delta`, recompute sampled/settled;
- `Elapsed` past the tween `Duration` ⇒ **clamp** to the settled end value (no overshoot);
- pure: output depends only on the prior clock and `delta`. Replaying an identical delta
  sequence reproduces identical state (FR-006).

## C4 — Carry and GC reuse E2 (no parallel identity scheme)

- Clocks are carried across frames by the existing `RetainedId`-keyed `StateByIdentity`
  map; a stable identity keeps its clock across a sibling-shifting re-render (FR-008).
- The existing `liveIds` filter (`RetainedRender.fs:363–371`) drops a removed identity's
  clock with the rest of its retained state on the next frame (FR-007). No new GC code.

## Non-regression fixed points

- A host with no migrated interactive controls behaves byte-identically to pre-R4 (the
  advance touches nothing; no clock is ever started).
- The consumer's `update`/`view : 'model -> Control<'msg>` and `Model`/`Msg` are untouched.
- The E1 text seam, E4 focus routing, and R1 visual-state stamping are unchanged; R4 only
  adds the clock advance + retarget atop the stamped state.
