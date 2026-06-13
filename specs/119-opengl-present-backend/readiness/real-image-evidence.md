# real-image-evidence — feature 119 (US2, T022)

evidence-kind=real-image-evidence
status=applicable
artifact-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=false

Feature 119 launches a real persistent **OpenGL** window on display `:1` and captures an on-demand
screenshot through the offscreen readback routine (FR-004, decoupled from the live direct present).
The artifact `sample-smoke/gl-direct-present-frame.png` is a decodable PNG (640×480, 8-bit RGBA;
PNG magic confirmed) decoded with SkiaSharp.

artifact-decodable=true — the capture decodes as a valid PNG.
proves-scene-rendering=true — the captured pixels are the rendered scene, verified by sampling:
background corner `#ff12161e` = `rgb(18,22,30)`, text band `#ffebebf0` = `rgb(235,235,240)`, and
144 sampled orange pixels = the moving rect `rgb(255,138,0)`. These are the production
`SceneRenderer` pixels the live host draws.
proves-desktop-visibility=false — this is a **pixel-readback** capture (the offscreen readback
routine), and pixel-readback alone cannot prove desktop visibility. Desktop presentation is
evidenced separately by the live windowed run on display `:1`
(`supported-host-persistent-launch.txt`: window-visible=observed:true, 60 frames presented).
