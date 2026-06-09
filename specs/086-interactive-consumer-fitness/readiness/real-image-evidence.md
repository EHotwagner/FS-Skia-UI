# Real image evidence — feature 086

| Field | Value |
|-------|-------|
| evidence-kind | render-target-png |
| status | partial — Scene/Controls render-target PNGs decoded in tests (US5); production-path real-controls render + live screenshot pending (US1/US2) |
| artifact-decodable | true (Feature086SceneTranslateTests decodes PNGs via SKBitmap.Decode) |
| proves-scene-rendering | true |
| proves-desktop-visibility | false — decodable render-target/pixel-readback alone cannot prove desktop visibility; that needs a live persistent window screenshot (SC-002) |

Pixel-readback alone cannot prove desktop visibility; the live persistent-window screenshot
(real-controls-live-screenshot.png) is the desktop-visibility proof and is pending US2 capture.
