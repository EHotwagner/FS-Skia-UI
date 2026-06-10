# Phase 1 Data Model: Live Interactive Host Event Dispatch

090 introduces **no new persisted/runtime entity types** — it consumes existing render/pointer/focus
data. This file records the existing shapes the design joins and the small new public surfaces.

## Existing shapes consumed (no change)

### `ControlEventBinding<'msg>` — `src/Controls/Types.fsi:279`
```fsharp
type ControlEventBinding<'msg> =
    { ControlId: ControlId      // authored id: Key |> defaultValue Kind
      EventKind: string         // "click" | "changed" | "selected" | <lowercased on*-suffix>
      Dispatch: ControlEvent -> 'msg }
```
Computed for the whole tree by `ControlInternals.recursively ControlInternals.eventBindings`
(`Control.fs:1169`) and surfaced as `ControlRenderResult.EventBindings`. **The join key is
`(ControlId, EventKind)`.**

### `PointerInteraction` — `src/Controls/Pointer.fsi:72`
Each interactive case carries `control: ControlId` — the **structural** hit id (path or Key). Event-kind
mapping the host applies: `Click`→`"click"`; value-change interactions →`"changed"`; otherwise the
interaction is not binding-eligible and goes straight to `MapPointer`.

### `ControlRenderResult<'msg>` — `src/Controls/Types.fsi:285`
Provides `Layout` (already used), `Bounds: (ControlId * Rect) list` (authored-id keyed), and
`EventBindings` (newly consumed). The recovery (below) re-derives structural-path → node identity from
the same `Control<'msg>` tree the host already holds (`host.View size model`).

### Focus + text — `ControlRuntime.fsi:42/54`, `TextInput.fsi`
`FocusedControl: ControlId option`; `FocusControl of ControlId option`; `TextInputModel`,
`TextInputMsg` (`CompositionStarted/Committed`, …), pure `TextInput.update`. The text seam delivers a
keystroke to the `FocusedControl`'s `TextInput.update` and folds the result.

## New public surfaces (FSI-first, Tier 1)

### 1. Nearest-keyed-ancestor recovery (FR-004 / 004a / 005) — `src/Controls/**`
A pure, total, option-returning resolver over the render result + hit id:
```
nearestAuthored : result: ControlRenderResult<'msg> -> hit: ControlId -> ControlId option
```
- **Input** `hit`: a structural `ControlId` (e.g. from `hitTest`/a `PointerInteraction`).
- **Output**: the **nearest** ancestor (incl. self) carrying a `Key` or an authored binding, as its
  authored `ControlId`; `None` if none on the path. Directly-keyed leaf → itself.
- **Invariants**: total (defined for every input), deterministic (same input → same output, no
  clock/randomness, resume-safe), identity-at-rest (a directly-keyed leaf is a fixed point), never
  invents a `Kind`/root id the consumer did not author (FR-004a). *(Exact name/arity drafted in FSI per
  Principle I; `nearestAuthored` is the working name.)*

### 2. Binding-dispatch join in `routeInteractivePointer` (FR-001 / FR-003) — `src/Controls.Elmish/**`
No new type — a **behavioral** change to the existing
`routeInteractivePointer : host -> state -> size -> model -> input -> PointerState * 'msg list`.
Per interaction: `recover hit → lookup (ControlId, eventKind) in EventBindings → dispatch binding XOR
fall back to MapPointer`. State threading and signature unchanged; only the produced `'msg list` now
includes authored-binding messages. Precedence: **binding consumes the interaction**; `MapPointer` sees
only unconsumed interactions (no double-dispatch).

### 3. Focus-aware text-routing seam (FR-008) — `src/Controls.Elmish/**`
An additive seam delivering a keystroke/committed text to `FocusedControl`'s `TextInput` model, folded to
a product `'msg`. Existing `MapKey` field is **unchanged** (research D4). Final shape (an optional host
field vs. a host-level function) drafted FSI-first; documented in the published contract.

### 4. Responds-proof capture (FR-006 / FR-007) — `src/Controls.Elmish/**` (calling SkiaViewer capture)
```
captureRespondsProof : host -> state -> size -> model -> input -> RespondsProof
```
where `RespondsProof` records the **before** frame, the **after** frame (post-route+fold+repaint), and a
**verdict** (`Responsive` when the frames differ, `Inert` when identical). Emitted as a decodable
artifact pair + verdict line. Distinct evidence class from a render-only screenshot and the offscreen
route probe. *(Name/shape drafted FSI-first.)*

## State transitions (live loop, after this feature)

```
native pointer sample ──► routeInteractivePointer
   render host.View ─► Pointer.update ─► interactions
   for each interaction:
       recover authored id (nearestAuthored)
       ├─ binding (id, kind) found ─► dispatch binding.Dispatch event      (authored wins)
       └─ none                      ─► interpretPointerOutcome host.MapPointer  (fallback)
   ─► 'msg list ─► host.Update fold ─► repaint (SkiaViewer.fs:2469)

native key/text event ──► focus-aware seam
   FocusedControl names a text control ─► TextInput.update ─► fold        (FR-008)
   else ─► MapKey (unchanged)

responds-proof ──► render(before) ; route+fold+repaint ; render(after) ; verdict = (before ≠ after)
```

No new effects, subscriptions, or interpreter cases; `host.Update` folding is otherwise unchanged
(additive — no authored binding ⇒ identical behavior to today).
</content>
