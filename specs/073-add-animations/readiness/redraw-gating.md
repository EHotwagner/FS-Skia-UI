# Redraw gating — Add Animations (073)

FR-006 / SC-004 / SC-007: the animation tick subscription gates redraws at the
**framework-request level** — it emits frame-delta messages only while at least one
animation is active and goes silent once all settle. The host's internal present loop
is unchanged (rewriting it for per-widget redraw regions is deferred scope).

## Mechanism

`FS.Skia.UI.Elmish.Animation.tickSubscription isAnimating toMsg interval : 'model -> Sub<'msg>`
plugs into `Program.withSubscription`. Elmish diffs the returned `Sub` each update:

- `isAnimating model = true` ⇒ the `Sub` carries one entry (SubId
  `["fs-skia-ui"; "animation-tick"]`) whose `Subscribe` dispatches an immediate
  `AnimationTick interval` and then one per `interval` via a `System.Threading.Timer`.
- `isAnimating model = false` ⇒ `Sub.none` (empty). The Elmish subscription diff sees
  the entry disappear and **stops** it — no idle redraw.

## Evidence (active-emits / settled-silent)

`tests/Elmish.Tests/AnimationTickTests.fs` exercises the real Elmish `Sub` plumbing
(no mocks; a recording dispatcher mirrors the `AdapterCmd` precedent):

- **emits while active**: an active model yields a non-empty `Sub`; starting it
  dispatches exactly `[ AnimationTick interval ]` (the immediate first frame).
- **settled silent**: a settled model yields `Sub.none`; starting it dispatches
  nothing.
- **stable scoped SubId**: `["fs-skia-ui"; "animation-tick"]`.
- **removed-widget edge**: transitioning the model from running → settled yields the
  entry then its absence; the Elmish diff stops the removed sub cleanly (disposing the
  active subscription does not throw).

The far-future test interval (30 s) keeps the recurring timer from firing inside the
synchronous test window, so the assertion is deterministic (only the immediate frame
is recorded), not a real-timer wait.

## US2 value-glide observation

Driving `AnimationState.advance` over ticks toward a target, then a mid-flight
`AnimationState.retarget`, continues from the displayed value with no jump back to the
original start (`readiness/fsi/animation-session.txt`: `Start=50.00 Current=50.00
Target=0.00`). While the state `isActive`, `isAnimating` holds and the subscription
emits; once `AnimationState.isActive` is false the subscription self-suspends.
