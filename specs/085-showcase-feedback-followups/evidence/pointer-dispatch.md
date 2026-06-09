# Pointer-dispatch evidence (085, US2 — SC-002, FR-004/FR-005)

evidence-kind=pointer-dispatch
status=ok
path=synthetic-through-real-adapter
host-observable=true

## Synthetic-through-real-adapter dispatch (the honest D6 bar)

A synthetic pointer press+release at a bound control's computed bounds, routed through the
EXACT step `runInteractiveApp` wires per native sample (`ControlsElmish.routeInteractivePointer`
→ `Control.renderTree` → `Layout.evaluate` → `Pointer.update` 4px click/drag fold →
`interpretPointerOutcome host.MapPointer`), dispatches the bound message and changes the model.
Proven green by `tests/SkiaViewer.Tests` → `Feature 085 interactive pointer host (US2)` (3/3 pass).

control-id=go
pointer-down=Pressed@center(go)
pointer-up=Released@center(go)
fold=click (within 4px threshold)
routed-msg=Increment
model-before=Count:0
model-after=Count:1
model-changed=true
update-emitted-effect=RenderScene
hit-test-miss-dispatches=none (press in empty space routes no msg)

## Host-observable (durable window)

The durable `runInteractiveApp` launch is observable from the host, not only headless tests:
a real visible window presented its first frame and self-closed (`window-visible=observed:true`,
`first-frame-presented=true`, `close-reason=AppRequestedClose`) — see
`readiness/interactive-visible-window.md` and `readiness/logs/interactive-launch.txt`. Live OS
pointer *injection* into that window is not scripted here; the dispatch contract is proven via
the synthetic-through-real-adapter path above (research D6: not `[S]` — it exercises the real
adapter pipeline, not a literal fixture).
