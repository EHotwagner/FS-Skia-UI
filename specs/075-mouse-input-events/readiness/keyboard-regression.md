# Keyboard-only regression (SC-006 / FR-012)

Pointer support is purely additive and consumer-initiated: an application that
never constructs a `PointerState`, never translates `ViewerEvent.Pointer*`, and
never calls `interpretPointerEffect` behaves exactly as before. The host already
returned `None` for pointer events; the new `ViewerEvent` fields are inert unless a
consumer maps them.

**Evidence**: the existing, unchanged `KeyboardInputGallery` sample re-runs green
against the feature-075 build (no source change to the sample):

```
status=ok
sample=KeyboardInputGallery
active-layout=default
available-layouts=default
mode-stack=["symbols"]
held=["H"]
effects=4
compact-labels=1
expanded-stack=1
hidden=KeyboardStateDisplayHidden
last-command=Some "move.left"
recovered-pressed=[]
```

Command: `dotnet run --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke`.

The keyboard reducer, its `interpretKeyboardEffect` bridge, and the
`KeyboardInput` surface baseline are unchanged by this feature (only `Controls`,
`Controls.Elmish`, and `SkiaViewer` moved). The whole package test suite is green
under `Dev` (`readiness/logs/dev.txt`, Status Ok), including the unchanged
`KeyboardInput.Tests` (7 passed) and `Input.Tests` keyboard adapter test that
asserts `PointerMoved` is ignored by the keyboard adapter.
