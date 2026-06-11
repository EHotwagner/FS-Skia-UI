# Phase 1 Data Model: True Visual-State Cross-Fade (R6)

## Entities

### `AnimationClock` (internal — `src/Controls/RetainedRender.fsi`)

The per-identity clock from feature 099. R6 adds **one field** — the prior-state painted snapshot
to cross-fade *from*.

| Field | Type | Meaning | R6 change |
|-------|------|---------|-----------|
| `Anim` | `FS.Skia.UI.Scene.Animation` | The tween sampled on paint. The **opacity** tween is the live channel (next layer fade-in `0→1`). | unchanged shape; doc reconciled — the standalone `Color` tween is **not** the cross-fade mechanism (FR-009) |
| `Elapsed` | `System.TimeSpan` | Accumulated **injected** delta (sole time coordinate; no wall-clock). | unchanged |
| `Target` | `VisualState` | The state this clock animates toward (used to detect a retarget). | unchanged |
| **`From`** | `FS.Skia.UI.Scene.Scene list` | **NEW.** The *prior* state's static own-scene snapshot, captured at transition start, composited **under** the next own-scene fading out (`1→0`). Empty list = nothing to fade from (first entry / no prior paint) ⇒ behaves like today's fade-in. | **added** |

> Naming: `From` (the prior snapshot) pairs with the next own-scene supplied by the assemble walk
> (the "to"). The field is a `Scene list` to match `Fragment.OwnScene`'s type, so the matched prior
> fragment is carried verbatim with no conversion.

**Construction / lifecycle (`updateClockForState`, extended):**
- *At rest staying at rest* (`None, Normal`) → `None` (no clock). Unchanged.
- *Same state already targeted* (`Some c, d when d = c.Target`) → `Some c` (advance-only, `Keep`).
  Unchanged. `From` is retained as-is.
- *Fresh transition / first non-Normal entry* → new clock with `Anim = fade-in (start opacity 0 or
  current sampled opacity on mid-flight)`, `Elapsed = 0`, `Target = desired`, **`From = matched prior
  retained node's `Fragment.OwnScene`** (the snapshot to fade out). On a **mid-flight retarget**,
  `From = previous target's own snapshot` (the layer that was fading in becomes the one fading out),
  `Elapsed = 0` (FR-007).
- *Settled return-to-`Normal`* → dropped (`None`), discarding `From`. Unchanged rule (FR-008).

### `RetainedUiState.Animation` (internal)
Carries `AnimationClock option` keyed by `RetainedId`. R6 adds no new field here; the clock it holds
gains `From`. GC is unchanged — only live identities carry state.

### `VisualState` / `VisualStateValue` (`src/Controls/Types.*`)
Unchanged. The `Reconcile.attrValueEqual` `VisualStateValue a, VisualStateValue b -> a = b` case
(feature 099) keeps a **held** state a scoped `Keep` — R6 must not regress this (a held state must
not re-fire or repaint every frame, FR-008/SC-006).

## State transitions (cross-fade lifecycle)

```
        Normal (at rest, no clock)
           │  state flips to Hover/Focused (R1 stamp)
           ▼
  Transition START: From = prior OwnScene snapshot; next layer fades 0→1; prior layer fades 1→0
           │  injected deltas accumulate in Elapsed
           ▼
  MID-FLIGHT: assemble composites [ applyAt(fade-out) From ; applyAt(Anim) next ]
           │  (a) settle (Elapsed ≥ Duration)        (b) state changes again
           ▼                                           ▼
  SETTLED: clock dropped if Target=Normal,          RETARGET: From = previous target snapshot,
  else clockActive=false ⇒ node paints              Elapsed=0, next = freshest target snapshot
  ownStatic verbatim (byte-identical)
```

## Invariants (asserted by tests / evidence)

| # | Invariant | Source FR / SC |
|---|-----------|----------------|
| INV-1 | **At-rest byte-identity**: no clock ⇒ assemble fast path returns cached `SubtreeScene`; no animation attribute emitted. | FR-004 / SC-002 |
| INV-2 | **Final-frame byte-identity**: a settled clock is inactive/dropped ⇒ node paints `ownStatic`; frame == snapped static for every channel. | FR-005 / SC-003 |
| INV-3 | **Mid-flight strictly-between**: for a region painted in both states, the composited color at an intermediate `Elapsed` lies strictly between the prior and next endpoint colors. | FR-002 / SC-001 |
| INV-4 | **Determinism**: identical injected-delta sequences ⇒ identical sampled frames; non-positive delta never rewinds; past-duration settles canonically (no overshoot). | FR-006 / SC-004 |
| INV-5 | **Retarget continuity**: a second mid-flight state change fades from the previous target snapshot (no snap to a stale at-rest endpoint). | FR-007 |
| INV-6 | **Held state scoped repaint**: a settled, held state stays a `Keep` (single repaint, not per-frame). | FR-008 / SC-006 |
| INV-7 | **Doc↔behavior agreement**: every channel the `AnimationClock` doc names is driven; the dropped color-tween claim is removed. | FR-009 / SC-005 |

## Closed channel set (FR-003)

The animated quantity is the node's **own painted appearance** (two cached `Style.resolve`-derived
snapshots blended by opacity). There is no per-property animation surface, no consumer-facing tween
configuration, and no animated channel that `Style.resolve` does not already produce upstream.
