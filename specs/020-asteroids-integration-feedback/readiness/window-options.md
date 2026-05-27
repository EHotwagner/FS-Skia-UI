# Window Options Evidence

status=validated
mode=interactive-window
command=./fake.sh build -t GeneratedProductCheck
validation-contract=Viewer.validateWindowLaunchBehavior viewerOptions.InitialSize
diagnostic-class=window-options
failure-class=none

The packed generated consumer option path was exercised by
`GeneratedProductCheck`; see
`readiness/generated-consumer-validation/window-options.log` and
`readiness/generated-consumer-validation/window-options.txt`.

status=honored mode=interactive-window command=--window-options option=initial-size requested=640x480 observed=640x480 diagnostic-class=window-options message=Initial window size is positive and can be requested.
status=honored mode=interactive-window command=--window-options option=resize requested=fixed-size observed=fixed-size diagnostic-class=window-options message=Resize policy can be honored by the viewer host.
status=honored mode=interactive-window command=--window-options option=maximize requested=not-maximizable observed=not-maximizable diagnostic-class=window-options message=Maximize policy can be honored by the viewer host.
status=honored mode=interactive-window command=--window-options option=startup-state requested=maximized observed=maximized diagnostic-class=window-options message=Maximized startup state can be requested.
status=honored mode=interactive-window command=--window-options option=startup-position requested=24,36 observed=24,36 diagnostic-class=window-options message=Startup coordinates can be requested.
status=unsupported mode=interactive-window command=--window-options option=backend requested=opengl observed=none diagnostic-class=window-options message=OpenGL backend preference is not supported by this viewer host.
