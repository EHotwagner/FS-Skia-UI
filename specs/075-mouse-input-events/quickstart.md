# Quickstart: Mouse Input & Pointer Events

This walks a consumer through attaching mouse hover/click/drag/scroll to controls
**without writing any coordinate math or hit-testing** (SC-007). It mirrors the
existing keyboard wiring in `samples/InteractiveViewer` and
`samples/KeyboardInputGallery`.

## 1. The shape (FSI sketch — Principle I)

```fsharp
#r "nuget: FS.Skia.UI.Controls"
#r "nuget: FS.Skia.UI.Controls.Elmish"
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish

// Model owns the coordination state next to your control runtime model.
type Model = { Pointer: PointerState; (* ... your app state ... *) }

// init
let pointer0 = Pointer.init ()            // default 4.0px click-vs-drag threshold

// A scripted, host-free sequence proves the shape interactively:
let layout : FS.Skia.UI.Layout.LayoutResult = (* computed for your view *) ...
let policy : FS.Skia.UI.Layout.PixelSnapPolicy = ...

let msgs =
    [ Pointer.Move(10.0, 10.0)            // hover-enter buttonA
      Pointer.Down(Primary, 10.0, 10.0)   // press buttonA
      Pointer.Up(Primary, 10.0, 10.0) ]   // -> Click(buttonA, Primary)

let finalState, effects = Pointer.replay policy layout msgs pointer0
// effects = [ HoverEnter("buttonA",..); PressedDown(..); ReleasedUp(..); Click("buttonA", Primary, ..) ]
```

If this reads awkwardly in FSI, the surface is wrong — fix it before `.fs`.

## 2. Translate host events to neutral samples (the only host-coupled glue)

`FS.Skia.UI.Controls` is host-independent, so the consumer maps
`ViewerEvent.Pointer*` to a `PointerSample` — a few lines in your `update`,
exactly like keyboard:

```fsharp
open FS.Skia.UI.SkiaViewer.Host

let toSample event =
    match event with
    | PointerMoved(x, y)             -> Some { Phase = Moved;    X = x; Y = y; Button = None;            DeltaX = 0.; DeltaY = 0. }
    | PointerPressed(x, y, b)        -> Some { Phase = Pressed;  X = x; Y = y; Button = Some(toBtn b);   DeltaX = 0.; DeltaY = 0. }
    | PointerReleased(x, y, b)       -> Some { Phase = Released; X = x; Y = y; Button = Some(toBtn b);   DeltaX = 0.; DeltaY = 0. }
    | PointerScrolled(x, y, dx, dy)  -> Some { Phase = Wheel;    X = x; Y = y; Button = None;            DeltaX = dx; DeltaY = dy }
    | PointerExited                  -> Some { Phase = Exited;   X = 0.; Y = 0.; Button = None;          DeltaX = 0.; DeltaY = 0. }
    | _ -> None
```

## 3. Reduce and dispatch (MVU — Principle IV)

```fsharp
let update msg model =
    match msg with
    | ViewerInput event ->
        match toSample event |> Option.bind Pointer.toMsg with
        | Some pmsg ->
            let pointer, interactions, runtimeMsgs =
                Pointer.update policy (layoutOf model) pmsg model.Pointer
            let cmd =
                ControlsElmish.interpretPointerOutcome routeInteraction interactions runtimeMsgs
                |> AdapterCmd.toCmd routeEffect
            { model with Pointer = pointer }, cmd
        | None -> model, AdapterCmd.none

// route only the interactions you care about; the rest are ignored (FR-012 opt-out).
let routeInteraction =
    function
    | Click(id, Primary, _, _)        -> Some (Activate id)
    | Click(id, Secondary, _, _)      -> Some (OpenContext id)   // FR-013; YOU render the menu
    | Scroll(id, _, dy, _, _)         -> Some (ScrollBy(id, dy)) // FR-014
    | DragMove(id, _, x, y)           -> Some (DragTo(id, x, y))
    | _                               -> None
```

A keyboard-only app simply never builds a `PointerState` and behaves unchanged
(SC-006).

## 4. Run the sample

`samples/PointerInteractionGallery` demonstrates hover highlight, primary click,
right-click context signal, drag, and wheel scroll end-to-end. Smoke log is
captured to `specs/075-mouse-input-events/readiness/sample-smoke/`.

## 5. Validate (escalated maintainer-verify path)

Run `Route` first, then only the gates it prints. Expected serialized order
(FAKE-backed targets are sequential — never concurrent):

```bash
./fake.sh build -t Route            # authoritative tier + gate list for this diff
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck     # known local env-failure (see project memory)
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# new .fsi surface:
./fake.sh build -t RefreshSurfaceBaselines   # + per-package PerPackageSurface.captureCurrent
```

## Acceptance mapping

| Success criterion | Proven by |
|-------------------|-----------|
| SC-001 hover correctness/ordering | `Pointer.update` move tests + property test (no dup/skip transitions) |
| SC-002 click iff same control | press/release-same vs press/release-different tests |
| SC-003 drag begin/move/end, sub-threshold = click | threshold tests |
| SC-004 cancel on exit/focus-loss | `WindowExited`/`FocusLost` tests (Presses empty, no drag) |
| SC-005 determinism/replay | `Pointer.replay` twice = identical effects |
| SC-006 keyboard-only unchanged | existing keyboard sample re-run |
| SC-007 no consumer coordinate math | this quickstart's app code references only `ControlId`-level messages |
| SC-008 per-button independence | overlapping primary+secondary press tests |
| SC-009 wheel delta to control / none on empty | wheel-over-control vs wheel-over-empty tests |
