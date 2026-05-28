# Screenshot Capture Contract

## Scope

Applies to SkiaViewer and generated graphical app screenshot evidence commands.

## Request

A screenshot request MUST include:

- command name
- output evidence record path
- requested width and height
- renderer mode
- timeout
- app or sample identity
- capture mode

The planned capture mode is `viewer-render-target-png`: launch the supported
viewer evidence path, wait for first-frame presentation, capture the rendered
output through the viewer-owned surface/pixel path, encode a PNG with
SkiaSharp, validate the written artifact, and close through evidence cleanup.

## Successful Result

A successful result MUST include:

- `status=ok`
- `evidence-kind=screenshot`
- `capture-source=live-viewer-window`
- `proves-screenshot=true`
- screenshot artifact path
- positive decoded width and height
- non-blank pixel validation result
- first-frame presentation before capture
- command, app/sample identity, host facts, capture mode, timestamp, and message

## Unsupported or Failed Result

Unsupported and failed results MUST preserve:

- viewer/open status
- first-frame status
- capture availability
- blocked stage
- classification
- diagnostic category
- host facts
- missing evidence fields
- fallback kind only when fallback diagnostics were produced

Unsupported or failed results MUST NOT include a successful screenshot claim.
Deterministic scene evidence and pixel-readback diagnostics remain separate
evidence kinds.
