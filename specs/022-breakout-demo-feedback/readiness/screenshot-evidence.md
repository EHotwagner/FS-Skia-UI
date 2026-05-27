# Screenshot Evidence

## Public API Transcript

command=dotnet fsi specs/022-breakout-demo-feedback/readiness/screenshot-evidence.fsx
status=unsupported
exit-code=0
status=unsupported
command=--screenshot-evidence
output=readiness/game-screenshot-evidence.txt
evidence-kind=screenshot
renderer-mode=skia
unsupported-host-reason=screenshot capture is unavailable for this viewer host
fallback=deterministic-scene-evidence
screenshot-path=none
width=none
height=none
frames-rendered=none
diagnostics=status=unsupported|evidence-kind=screenshot|fallback=deterministic-scene-evidence|scene-capabilities=3
validation-accepted=true
validation-diagnostics=none

## Matrix

| Host capability | Status | Screenshot proof | Fallback | Owner |
|-----------------|--------|------------------|----------|-------|
| supported host | blocked: capture capability missing | none | n/a | SkiaViewer host screenshot capture |
| current unsupported host | unsupported | none | deterministic-scene-evidence | SkiaViewer |
