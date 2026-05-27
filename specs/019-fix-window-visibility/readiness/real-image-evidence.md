# Real Image Evidence

Status: packed generated consumer image evidence captured after T047.

## Requested Image Evidence

command=./fake.sh build -t GeneratedProductCheck
stdout=specs/019-fix-window-visibility/readiness/generated-consumer-validation/image-evidence.log
metadata=specs/019-fix-window-visibility/readiness/generated-consumer-validation/game-image-evidence.png.metadata.txt
requested-image-evidence=true
evidence-kind=image
path=specs/019-fix-window-visibility/readiness/generated-consumer-validation/game-image-evidence.png
image-artifact=specs/019-fix-window-visibility/readiness/generated-consumer-validation/game-image-evidence.png
artifact-kind=image
image-decodable=true
artifact-decodable=true
file-command=PNG image data
proves-scene-rendering=true
proves-desktop-visibility=false

The packed generated consumer command writes explicit image evidence fields and a decodable PNG artifact. Desktop-window visibility remains a separate claim and is not inferred from this image.

## Resolved Fallback Disclosure

The earlier T035 template fallback was used only for pre-change package compatibility while local template validation ran before T047. T047 replaced that readiness dependency with packed-package generated consumer evidence:

- `readiness/logs/t047-retry-pack-local-presenter-tests.txt`
- `readiness/logs/t047-retry-generated-product-check-presenter-tests.txt`
- `readiness/generated-consumer-validation/image-evidence.log`
- `readiness/generated-consumer-validation/game-image-evidence.png`

## Pixel Readback Fallback

evidence-kind=pixel-readback
fallback-reason=screenshot-unavailable
proves-scene-rendering=true
proves-desktop-visibility=false
status=not-run-for-T037

## Unsupported Host

evidence-kind=unsupported-host
unsupported-reason=not-applicable-for-T037-focused-run
