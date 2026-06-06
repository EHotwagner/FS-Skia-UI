# Controls rendering (070)

## Rendering is transparent to the typed surface (by parity)

Every typed `view` lowers to a `Control<'msg>` structurally equal to the legacy
builder output (see [typed-lowering-parity.md](./typed-lowering-parity.md)).
`Widget.render theme widget = Control.render theme (Widget.toControl widget)`, so
the typed surface renders through the **same** existing render path as the legacy
authoring API — there is no new rendering, layout, screenshot, Vulkan, or Skia
behavior to validate. The existing `RenderingTests.fs` / `AccessibilityTests.fs`
suites remain authoritative for the underlying `Control<'msg>` IR; because the
typed views produce that same IR, their render output is identical by
construction.

## Determinism

The lowering is pure (`Props -> Widget` for display/input/containers/overlay/
charts; `Props -> model -> Widget` for the stateful groups), with no wall-clock,
random, or I/O dependency. Re-rendering the same `Props` (and, for stateful
controls, the same model state) yields byte-identical IR.

## Deferred (tracked)

A persistent typed-authoring gallery panel (`samples/ControlsGallery/Program.fs`)
and a viewport render-smoke capture over ≥1 control per mechanic group at ≥2
viewports are sequenced as follow-up polish (T037–T039 in `tasks.md`). They are
not required to prove lowering correctness — the parity matrix already proves the
typed surface produces the existing IR — but they add a launch-level smoke for the
migrated surface and are noted as pending so the status is honest.
