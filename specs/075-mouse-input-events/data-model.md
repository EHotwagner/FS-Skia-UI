# Phase 1 Data Model: Mouse Input & Pointer Events

Types are grouped by the package that owns them. All are immutable F# records /
discriminated unions; `update` is pure. `ControlId = LayoutNodeId = string`
(existing aliases) is the control address used throughout.

## Host contract (FS.Skia.UI.SkiaViewer)

### `ViewerPointerButton` (new)
The host-level button identity carried on press/release.

| Case | Meaning |
|------|---------|
| `PrimaryButton` | left button |
| `SecondaryButton` | right button |
| `MiddleButton` | wheel-click button |

### `ViewerEvent` (extended)
Existing cases unchanged except:
- `PointerPressed of x: float * y: float * button: ViewerPointerButton`
- `PointerReleased of x: float * y: float * button: ViewerPointerButton`
- `PointerScrolled of x: float * y: float * deltaX: float * deltaY: float` *(new)*
- `PointerExited` *(new — window-leave/blur, drives FR-007 cancel)*

`PointerMoved of x * y` is unchanged. Compatibility note: arity change on
press/release is source-breaking only for matchers; the sole existing matcher
returns `None`.

## Pointer front door (FS.Skia.UI.Controls — new `Pointer` module)

### `PointerButton`
Framework-neutral button identity (decoupled from the host enum).
`Primary | Secondary | Middle`.

### `PointerPhase`
The kind of raw pointer sample fed into coordination.
`Moved | Pressed | Released | Wheel | Exited`.

### `PointerSample`
Neutral, host-independent input value the consumer produces from a
`ViewerEvent` (or any source). Validation: `X`/`Y` are in the same coordinate
space as layout bounds (consumer applies device-pixel scaling / pixel-snap policy
already used for layout, per FR coordinate-space edge case).

| Field | Type | Notes |
|-------|------|-------|
| `Phase` | `PointerPhase` | which transition this sample represents |
| `X` | `float` | window-space x (mapped to layout space) |
| `Y` | `float` | window-space y |
| `Button` | `PointerButton option` | present for `Pressed`/`Released`; `None` for `Moved`/`Wheel`/`Exited` |
| `DeltaX` | `float` | wheel x-delta (0.0 unless `Wheel`) |
| `DeltaY` | `float` | wheel y-delta (signed; 0.0 unless `Wheel`) |

### `PressCandidate`
A press in flight for one button (the click/drag candidate).

| Field | Type | Notes |
|-------|------|-------|
| `Control` | `ControlId` | control that received the press |
| `StartX` | `float` | press position (threshold origin) |
| `StartY` | `float` | |
| `Dragging` | `bool` | set once movement passes `DragThreshold` |

### `PointerState` (the Model)
Durable coordination state, owned alongside `ControlRuntimeModel`.

| Field | Type | Notes |
|-------|------|-------|
| `Hover` | `ControlId option` | current single hover target (FR-003) |
| `Presses` | `Map<PointerButton, PressCandidate>` | independent per-button tracking (FR-013, SC-008) |
| `LastX` | `float` | last sampled position (for drag/move math) |
| `LastY` | `float` | |
| `DragThreshold` | `float` | movement separating click from drag (FR-006); default `4.0` px |

`init : ?threshold:float -> PointerState` — empty hover, empty presses,
configurable threshold (default 4.0).

### `PointerOrigin`
`Pointer` — single case in v1; exists so consumer-facing events are type-tagged
as pointer-originated (FR-011) and future origins can be added without reshaping.
**How FR-011 is satisfied**: every consumer-facing effect is a `PointerInteraction`
value, which is a *distinct type* from the keyboard/text/focus effects — so a
consumer can already tell a `Click` apart from a keyboard activation by which
effect type it routed. `PointerOrigin` is the explicit, future-proof tag for that
distinction (carried alongside the interaction when a consumer flattens pointer
and keyboard effects into one unified message stream). The discrimination is
therefore structural — no behavioral branch is needed beyond the type/tag — and is
asserted by T013 rather than by a dedicated reducer test.

### `PointerInteraction` (the Effect — consumer-facing)
Ordered effects emitted by `update`. Every control-addressed case carries
`ControlId`; click/drag carry `PointerButton`; all carry the resolved position.

| Case | Payload | Requirement |
|------|---------|-------------|
| `HoverEnter` | `control: ControlId * x * y` | FR-003 |
| `HoverLeave` | `control: ControlId` | FR-003 |
| `Pressed` | `control * PointerButton * x * y` | FR-004 |
| `Released` | `control * PointerButton * x * y` | FR-005 |
| `Click` | `control * PointerButton * x * y` | FR-005/FR-013 |
| `DragBegin` | `control * PointerButton * startX * startY` | FR-006 |
| `DragMove` | `control * PointerButton * x * y` | FR-006 |
| `DragEnd` | `control * PointerButton * x * y` | FR-006 |
| `DragCancelled` | `control: ControlId option` | FR-007/SC-004 |
| `Scroll` | `control * deltaX * deltaY * x * y` | FR-014 |
| `FocusMovedByPointer` | `control: ControlId` | FR-004 (focus to pressed focusable) |
| `Diagnostic` | `PointerDiagnostic` | FR-010 (stale/miss) |

### `PointerMsg` (internal transition input)
Produced from a `PointerSample`; reduced by `update`.
`Move of x*y | Down of PointerButton*x*y | Up of PointerButton*x*y |
Wheel of dx*dy*x*y | WindowExited | FocusLost`.
A helper `Pointer.toMsg : PointerSample -> PointerMsg option` maps a sample to a
message (`None` for a sample that carries no actionable transition).

**Trigger sources (FR-007).** `WindowExited` is the `toMsg` translation of a
`PointerSample` with `Phase = Exited`, which the host produces from window
mouse-leave **and** blur (see T015: `mouse-leave/blur → PointerExited`).
`FocusLost` has **no** `PointerPhase`/`PointerSample`/host-event source — it is a
consumer-supplied message a host may dispatch directly when *application-level*
input focus is lost without the pointer leaving the window (e.g. a logical focus
handoff). Both messages drive the identical cancel path (step 6); `FocusLost`
exists so that path is reachable independently of `Exited`. Hosts that already
fold blur into `PointerExited` (the default wiring) may never emit `FocusLost`.

### `PointerDiagnostic`
`{ Code: PointerDiagnosticCode; Message: string; Control: ControlId option;
X: float; Y: float }` where
`PointerDiagnosticCode = HitTestMiss | StaleTarget`.

## MVU bridge (FS.Skia.UI.Controls.Elmish)

`interpretPointerEffect` lowers a `PointerInteraction` into the existing
`AdapterCommand<'msg>`. A consumer supplies routers mapping the meaningful cases
to product messages; framework-only/no-op cases lower to `[]`; diagnostics lower
to `ReportAdapterDiagnostic` (a new `AdapterEffect` case is added only if the
existing ones cannot carry a pointer diagnostic — decided in contracts).

## State transitions (the heart of `update`)

Pure `update : PointerMsg -> LayoutResult -> PointerState -> PointerState *
PointerInteraction list`. Hit-test via `Layout.hitTestComputed policy result x y`.

1. **Move (no button held)** → `hit = hitTest`. If `hit <> Hover`:
   emit `HoverLeave(Hover?)` then `HoverEnter(hit?)`; set `Hover := hit`. Also
   issue `ControlRuntimeMsg.HoverControl hit`. No transition if `hit = Hover`.
2. **Move (button held with candidate `c`)** → update last position; if not yet
   `c.Dragging` and `dist((StartX,StartY),(x,y)) > DragThreshold`: set
   `Dragging`, emit `DragBegin(c.Control, btn, c.StartX, c.StartY)` +
   `ControlRuntimeMsg.StartDrag`. If already dragging: emit
   `DragMove(c.Control, btn, x, y)` + `MoveDrag`. (Hover updates suppressed while a
   drag is active for that button.)
3. **Down(btn, x, y)** → `hit = hitTest`. If `Some control`: add
   `Presses[btn] = {control; x; y; Dragging=false}`; emit `Pressed(control, btn,
   x, y)`; for the primary/focusable case also `FocusMovedByPointer(control)` +
   `ControlRuntimeMsg.PressControl`/`FocusControl`. If `None`: emit `Diagnostic
   HitTestMiss`.
4. **Up(btn, x, y)** → look up `Presses[btn]`. If candidate `Dragging`: emit
   `DragEnd(control, btn, x, y)` + `EndDrag`. Else `hit = hitTest`; if `hit =
   Some candidate.Control`: emit `Released` then `Click(control, btn, x, y)`. If
   `hit` differs/None: emit `Released?`/no click and clear pressed state. Remove
   `Presses[btn]`.
5. **Wheel(dx, dy, x, y)** → `hit = hitTest`. `Some control` → `Scroll(control,
   dx, dy, x, y)`; `None` → no effect (per US5 scenario 2) or `Diagnostic
   HitTestMiss` per FR-010 (consumer-selectable; default: silent miss for wheel
   over empty space, diagnostic for stale target).
6. **WindowExited / FocusLost** → for every candidate (and active drag): emit
   `DragCancelled`/clear; emit `HoverLeave(Hover?)`; reset `Presses := empty`,
   `Hover := None`; issue `ControlRuntimeMsg.CancelInteraction`/`FocusLost`.
   Guarantees no dangling pressed/dragging state (SC-004).

**Stale target (FR-010)**: when a hit id resolves but the `LayoutResult` no longer
contains a binding for an in-flight candidate's control (tree rebuilt), emit
`Diagnostic StaleTarget` and clear that candidate rather than dispatching a click
to a wrong control.

## Invariants (assert in tests)

- A move within the same control emits no hover transition (FR-003).
- A press/release pair is never dropped or reordered under interleaved moves
  (FR-008) — property-tested with random move bursts.
- A sequence produces `Click` XOR `DragBegin..DragEnd`, never both (FR-005/006).
- Each `PointerButton`'s press resolves independently; no cross-button
  misattribution (SC-008).
- `replay` of the same `PointerMsg list` yields identical effects (SC-005).
- After `WindowExited`/`FocusLost`, `Presses = empty` and no active drag (SC-004).
