# FSI evidence — pointer front door (feature 075)

The pointer front door is exercised through **packed/built libraries** by the
`FsiTranscripts` gate, which runs `scripts/controls-prelude.fsx` and
`scripts/controls-elmish-prelude.fsx` and captures their output to the sibling
files in this directory:

- `controls-prelude.txt` — `FS.Skia.UI.Controls` `Pointer` reducer (T008/T013/T020/T023/T026)
- `controls-elmish-prelude.txt` — `FS.Skia.UI.Controls.Elmish` bridge (T012/T017)

Both scripts `#r` the built `FS.Skia.UI.Controls.dll` / `FS.Skia.UI.Controls.Elmish.dll`,
so this is real evidence over the shipped surface — not a unit test against
internal helpers. Re-capture with `./fake.sh build -t FsiTranscripts`.

## Front door — `Pointer.replay` over a scripted hover/click/drag/scroll sequence

Layout: two side-by-side 100×40 buttons (`buttonA` at `[0,100)`, `buttonB` at
`[100,200)`). Script: hover A → hover B → click B → press A → drag past the 4px
threshold → drag-move → drag-end → wheel over B.

```
pointer-origin=Pointer
pointer-effects=[HoverEnter ("buttonA", 50.0, 20.0); HoverLeave "buttonA";
 HoverEnter ("buttonB", 150.0, 20.0);
 PressedDown ("buttonB", Primary, 150.0, 20.0); FocusMovedByPointer "buttonB";
 ReleasedUp ("buttonB", Primary, 150.0, 20.0); Click ("buttonB", Primary, 150.0, 20.0);
 PressedDown ("buttonA", Primary, 50.0, 20.0); FocusMovedByPointer "buttonA";
 DragBegin ("buttonA", Primary, 50.0, 20.0); DragMove ("buttonA", Primary, 50.0, 38.0);
 DragEnd ("buttonA", Primary, 50.0, 38.0);
 Scroll ("buttonB", 0.0, -3.0, 150.0, 20.0)]
pointer-deterministic=true
pointer-final-hover=Some "buttonB" presses=0
```

This single transcript proves, against the built library:

- **US1 / FR-003 (hover, T013)** — ordered `HoverLeave("buttonA")` then
  `HoverEnter("buttonB")`; every effect is a `PointerInteraction` carrying
  `PointerOrigin.Pointer` (`pointer-origin=Pointer`), type-distinct from keyboard
  effects (FR-011).
- **US2 / FR-004/FR-005 (click + focus, T017)** — press+release over the same
  control (`buttonB`) yields exactly one `Click`, preceded by `FocusMovedByPointer`.
- **US3 / FR-006 (drag, T020)** — press on `buttonA` + movement past the 4px
  threshold yields one `DragBegin`, ordered `DragMove`, one `DragEnd`, and **no**
  `Click` (click XOR drag).
- **US5 / FR-014 (wheel, T026)** — wheel over `buttonB` yields `Scroll` with the
  signed `-3.0` delta addressed to the control under the pointer.
- **FR-009/SC-005 (determinism)** — `pointer-deterministic=true`: replaying the
  identical sequence yields byte-identical effects.

US4 (per-button discrimination, T023) and the FR-007 cancel path are additionally
asserted by the deterministic Expecto suite
(`tests/Controls.Tests/PointerInteractionTests.fs`, 25 tests incl. two 500-case
FsCheck properties — all green under `Dev`).

## MVU bridge — `interpretPointerOutcome` lowering (T012/T017)

```
controls-elmish-075 pointerCommandCount=3 productMsgs=[Save] diagnosticCount=1
```

- The `(PointerInteraction list, ControlRuntimeMsg list)` produced by
  `Pointer.update` lowers in one call: two `DispatchControlRuntimeMessage` (the
  runtime `PressControl`/`ReleaseControl`) + one `DispatchProductMessage Save`
  (from the routed primary `Click`) = `pointerCommandCount=3`,
  `productMsgs=[Save]`.
- A `Diagnostic HitTestMiss` lowers to a single `ReportAdapterDiagnostic`
  (`diagnosticCount=1`) — reusing the existing `AdapterEffect`, no new case added.
