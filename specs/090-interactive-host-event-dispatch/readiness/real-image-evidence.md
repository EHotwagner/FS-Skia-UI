# Real Image Evidence (090)

evidence-kind=render-target-responds-proof
status=captured
artifact-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=false

## What this proves

Feature 090 captures a **responds-proof**: a before/after pair of rendered frames
around a real dispatched interaction on the production `Control.renderTree` path
(`ControlsElmish.captureRespondsProof`). The artifact is a decodable render-target
capture (`artifact-decodable=true`) that proves **scene rendering changed in
response to input** (`proves-scene-rendering=true`).

This render-target artifact does **not** itself prove desktop-window visibility
(`proves-desktop-visibility=false`) — decodable image/render-target evidence proves
the render path and the input→visible-change, but pixel-readback alone cannot prove
desktop visibility. Desktop visibility is proven **separately** (a distinct
evidence class): the live Vulkan/Skia window **was** opened and observed visible in
this session (`window-visible=Observed true`, `first-frame-presented=true`;
`live-window-launch.md`, `interactive-visible-window.md`). No on-screen pixel grab
is claimed as the image artifact (no external screenshot tool in this session).

The captured pairs live under `responds-proof/<case>/{before,after}.txt` +
`responds-proof.txt` for the representative cases (leaf-keyed `onClick`,
container-keyed composite, focused text), each with a bare `verdict=Responsive`.
