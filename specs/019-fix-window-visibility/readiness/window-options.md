# Window Options Evidence

status=validated
mode=interactive-window
command=./fake.sh build -t GeneratedProductCheck
validation-contract=Viewer.validateWindowLaunchBehavior viewerOptions.InitialSize
diagnostic-class=window-options
synthetic-fallback=false
tracking-issue=resolved-by-T047

The packed generated consumer option path was exercised by `GeneratedProductCheck`; see `readiness/generated-consumer-validation/window-options.log` and `readiness/generated-consumer-validation/window-options.txt`.
It proves generated option files and CLI flags produce one explicit row per option family and do not silently ignore unsupported backend requests. T047 verifies the packed `runAppWithWindowBehavior` path with the refreshed SkiaViewer package.

| Option | Requested value | Observed value | Status | Host message |
|--------|-----------------|----------------|--------|--------------|
| initial-size | 640x480 | 640x480 | honored | Initial window size is positive and can be requested. |
| resize | fixed-size | fixed-size | honored | Resize policy can be honored by the viewer host. |
| maximize | not-maximizable | not-maximizable | honored | Maximize policy can be honored by the viewer host. |
| startup-state | maximized | maximized | honored | Maximized startup state can be requested. |
| startup-position | 24,36 | 24,36 | honored | Startup coordinates can be requested. |
| backend | opengl | none | unsupported | OpenGL backend preference is not supported by this viewer host. |

status=honored mode=interactive-window command=--window-options option=resize requested=fixed-size observed=fixed-size diagnostic-class=window-options message=Resize policy can be honored by the viewer host.
status=honored mode=interactive-window command=--window-options option=maximize requested=not-maximizable observed=not-maximizable diagnostic-class=window-options message=Maximize policy can be honored by the viewer host.
status=honored mode=interactive-window command=--window-options option=startup-state requested=maximized observed=maximized diagnostic-class=window-options message=Maximized startup state can be requested.
status=honored mode=interactive-window command=--window-options option=startup-position requested=24,36 observed=24,36 diagnostic-class=window-options message=Startup coordinates can be requested.
status=unsupported mode=interactive-window command=--window-options option=backend requested=opengl observed=none diagnostic-class=window-options message=OpenGL backend preference is not supported by this viewer host.
