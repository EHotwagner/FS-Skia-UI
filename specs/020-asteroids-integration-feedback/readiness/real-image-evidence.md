# Real Image Evidence

requested-image-evidence=true
evidence-kind=screenshot
artifact-kind=image
artifact-decodable=true
image-artifact=readiness/generated-consumer-validation/game-image-evidence.png
proves-scene-rendering=true
proves-desktop-visibility=false
failure-class=none

Source evidence:

- `readiness/generated-consumer-validation/image-evidence.log`
- `readiness/generated-consumer-validation/game-image-evidence.png`
- `readiness/generated-consumer-validation/game-image-evidence.png.metadata.txt`

The image artifact is decodable scene-rendering evidence. Desktop visibility is
proved separately by `interactive-visible-window.md` and the persistent launch
diagnostics, not by pixel readback or image metadata.
