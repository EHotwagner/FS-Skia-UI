# Phase 0 Research: Animation Clock on Retained Identity (R4)

The spec's **Assumptions** and **Key Entities** already settle the headline choices;
they are recorded here as resolved decisions, followed by the plan-level mechanism
decisions Phase 0 must close. No open `NEEDS CLARIFICATION` remains.

## Settled by spec (recorded)

- **Paint-level, not layout.** Animation targets transform / opacity / color via the
  existing feature-073 `Animation` shape — never reflowable size/position. This is what
  keeps R2 incremental measure and the scoped-repaint reduction intact.
- **Automatic, visual-state-driven.** The transition is triggered by R1's bridge flipping
  a control's derived `VisualState`; there is no new consumer animation authoring API.
- **Single framework default transition.** A short duration + a standard easing from the
  feature-073 set; not a per-control consumer knob.
- **Injected-delta-only clock.** The sole time input is the host's per-frame `Tick` delta;
  no `Date.now`/wall-clock (determinism constitution + the environment's no-wall-clock
  constraint; `Date.now`/`new Date()` are unavailable here anyway).

---

## R1 — Where the injected delta enters the advance

**Decision.** Wrap `host.Tick` inside `runInteractiveApp`. The wrapper, given the
per-frame `TimeSpan` delta, **advances every live per-identity clock in
`retained.Value.StateByIdentity` in place**, then delegates to `host.Tick delta` for the
consumer's own tick message. `renderRetained` (called each frame by the viewer's `View`)
then paints the already-advanced clocks.

**Rationale.** The retained state lives in the `retained` ref the host already owns
(`ControlsElmish.fs:584–597`); advancing it in the `Tick` wrapper keeps the advance on
the **injected-delta** path and out of the consumer's pure `update`. Delegating to
`host.Tick` preserves the consumer's tick message (no swallow, no double-dispatch).

**Alternatives considered.** A `pendingDelta` ref accumulated in `Tick` and consumed at
the top of `renderRetained` — rejected as an extra indirection with the same effect and a
risk of double-counting if `View` runs more/less often than `Tick`. Advancing inside the
consumer `update` — rejected: it would put framework time state in the consumer's pure
reducer and require a public `Msg`, violating "the `view` contract is unchanged."

## R2 — Carried-slot type generalization

**Decision.** Generalize the **internal** `RetainedUiState.Animation` field from
`AnimationState<Transform> option` to a feature-073 **multi-channel paint carrier** that
can express opacity / transform / color (per the spec's Key Entities "transform / opacity
/ color per the Scene `Animation` shape"). Concretely, carry the feature-073 `Animation`
record plus an accumulated `Elapsed : TimeSpan` and the current target `VisualState`
(enough to detect a retarget), e.g. an internal record
`{ Anim : Animation; Elapsed : TimeSpan; Target : VisualState }` — exact field names a
data-model detail. Sampling uses `Animation.applyAt` by elapsed.

**Rationale.** The roadmap's worked examples are a **focus-ring fade (opacity)** and a
**press tint (color)** — neither is expressible by the `Transform`-only slot. Feature 073
already ships the `Animation` record (Opacity/Transform/Color tweens) and `applyAt` as the
pure sample-to-`SceneNode` primitive with **identity-at-rest lowering**, which is exactly
the at-rest byte-identity FR-005 needs — so reuse it rather than re-typing per channel.

**Alternatives considered.** Keep `AnimationState<Transform>` and animate a transform-only
effect (e.g. a press depress-scale) — rejected as less faithful to the roadmap's
fade/tint headline and unable to fade a focus ring. Add a *parallel* second slot for
opacity/color — rejected as two carry slots where one generalized slot suffices (and the
`liveIds` GC already handles one slot cleanly).

**Cost.** This is the single `.fsi` line that escalates the diff (internal
`RetainedRender.fsi`); the survival test that constructs the old slot
(`Feature092LiveSurvivalTests.startedClock`) is rewritten anyway (see R3/US2).

## R3 — Turning a VisualState flip into a tween start/retarget

**Decision.** Per identity, compare the **desired target** derived from the stamped
`VisualState` (already produced by `applyRuntimeVisualState` pre-reconcile,
`ControlRuntime.fs:229`) against the carried clock's `Target`:

- no carried clock + a non-`Normal` desired state → **start** a tween from the at-rest
  (Normal) appearance toward the active-state appearance;
- carried clock whose `Target` differs from the desired state → **retarget** from the
  **current sampled value** (`AnimationState.retarget` semantics — no snap to start),
  re-aiming toward the new state's appearance;
- desired state returns to `Normal` → retarget toward the at-rest appearance; once
  settled, drop the clock so the identity emits no output (FR-005 / at-rest restored);
- desired state equals the clock's `Target` → no retarget (just advance).

**Rationale.** Realizes the spec edge cases ("new transition while one is in flight
re-aims from the current sampled value"; "return to `Normal` animates back then emits
nothing") directly on top of feature-073's `retarget`, which is built to start from the
current value. The stamped `VisualState` is the single trigger source — no second state
machine, honoring "R1 is the transition trigger."

**Alternatives considered.** Diff the prior vs new retained tree's painted attributes to
detect change — rejected as indirect and entangled with reconciler internals; the stamped
`VisualState` is the authoritative, already-computed signal.

## R4 — Default transition channel(s), duration, easing

**Decision.** A single framework default: a **short** duration of **exactly 150 ms**
(the `defaultTransitionDuration` data-model constant, within the ~120–160 ms band) with
**`EaseOut`** from the feature-073 set, applied to the
paint-level delta between the Normal style and the active visual-state style for the
representative R1-migrated interactive kinds. The representative proof animates an
**opacity/tint** channel (focus-ring fade / press tint) — enough to show ≥1 intermediate
sampled appearance; full per-channel style-diff animation across all 52 controls is **out
of scope** (tracked with E3/R1).

**Rationale.** `EaseOut` reads as a responsive UI settle; a sub-200 ms default keeps
transitions crisp and keeps the survival/test frame counts small and deterministic. Using
the existing `Style.resolve` output for Normal vs active gives the start/end values without
inventing new style machinery.

## R5 — Sample-on-paint placement and identity-at-rest

**Decision.** Sample inside the retained paint pass (`RetainedRender.step`/`init`, at the
point each node's identity is looked up in `StateByIdentity`). For an identity with an
**active** carried clock, wrap/derive its painted node from `Animation.applyAt elapsed`
(opacity/transform/color); for an identity with **no** clock (or a settled one that has
been dropped), paint exactly as today — **no** animation attribute, byte-identical to the
pre-R4 golden. Only identities with an active tween contribute a per-frame change.

**Rationale.** `Animation.applyAt`'s deliberate identity-at-rest lowering means a settled
or absent animation is byte-identical to the static render — FR-005 holds *by
construction*. Keeping the fast path for at-rest identities means the presence of *one*
active animation does not invalidate the at-rest path for *other* controls (resolves the
FR-002 vs FR-005/FR-010 interaction the spec flags).

## R6 — Determinism and delta edge cases

**Decision.** The clock is a **pure function of accumulated injected deltas**: advance adds
the delta to `Elapsed` and recomputes sampled values; no other input. Edge handling:
**zero** delta advances nothing; **non-positive** delta is a **no-op** (never rewinds —
safe failure, the host never emits negative deltas); a **very large** delta **clamps** the
tween to its settled end (no overshoot past target). Replaying an identical delta sequence
yields identical output — property-tested with FsCheck over randomized fixed-delta
sequences (≥1000 cases), asserting run-to-run equality and that no wall-clock is consulted.

**Rationale.** Matches the determinism constitution and the spec's Edge Cases; `advance`
+ `applyAt` are already pure in feature 073, so the host only has to refrain from
introducing a nondeterministic source.

## R7 — Carry across frames and GC

**Decision.** Reuse the existing `RetainedId`-keyed `StateByIdentity` map for
carry-across-frames and the `liveIds` filter (`RetainedRender.fs:363–371`) for GC — a
removed identity's clock is dropped with the rest of its retained state on the next frame.
**No parallel identity scheme** (FR-008). The generalized carried-slot type changes the
*value* stored, not the keying or the filter.

**Rationale.** E2 already proved this carry/GC machinery; R4 only fills the slot it
leaves empty. The survival proof (US2) is exactly that the carried clock keeps advancing
across a sibling-shifting re-render because its `RetainedId` is stable.

---

## Decisions summary

| # | Decision |
|---|----------|
| R1 | Wrap `host.Tick`; advance live clocks in `retained.Value` by the injected delta, then delegate to `host.Tick`. |
| R2 | Generalize the internal `RetainedUiState.Animation` slot to a feature-073 multi-channel paint carrier (opacity/transform/color + elapsed + target state). |
| R3 | Stamped `VisualState` ≠ clock target → start/retarget from the current sampled value; return-to-`Normal` settles then drops the clock. |
| R4 | One default transition: ~120–160 ms, `EaseOut`, opacity/tint channel for the representative kinds; full-52 out. |
| R5 | Sample on paint via `Animation.applyAt`; at-rest identities emit no attribute and stay byte-identical. |
| R6 | Pure delta accumulation; zero/non-positive = no-op (no rewind), very-large = clamp to end; FsCheck determinism ≥1000 cases. |
| R7 | Reuse `StateByIdentity` carry + `liveIds` GC; no parallel identity scheme. |
