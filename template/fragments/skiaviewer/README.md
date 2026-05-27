# SkiaViewer Fragment

Adds viewer host package references and generated product viewer guidance.

Viewer-backed graphical profiles must use the persistent generated host as the
default executable path:

```fsharp
Viewer.runApp viewerOptions Product.Program.generatedHost
```

Bounded smoke, first-frame, frame-count, scene metadata, and unsupported-host
diagnostics are CI and reviewer-diagnostic helpers. They do not substitute for
supported-host persistent graphical launch readiness. Successful default launch
output must include `mode=interactive-window`, `window-visible=observed:true`,
and `accessible-window=true`; unsupported hosts must report diagnostics instead
of claiming accessibility.
