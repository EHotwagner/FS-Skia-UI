# US4 Validation

Task T061 updates the Elmish viewer samples:

- `samples/InteractiveViewer` exercises keyboard input, pointer input, resize, timer-driven state, screenshot requests, lifecycle initialization, and shutdown through `ViewerEffect`.
- `samples/ScreenshotGallery` exercises screenshot requests, render-frame recovery diagnostics, render retry, and shutdown through `ViewerEffect`.

Task T062 evidence:

- MVU transition and emitted-effect assertions: `readiness/logs/t062-mvu-sample-tests.txt`
- Interactive sample contract smoke: `readiness/smoke/t061-interactiveviewer-contract.txt`
- Screenshot sample contract smoke: `readiness/smoke/t061-screenshotgallery-contract.txt`
- Real screenshot interpreter evidence from the safe Vulkan host path: `readiness/screenshots/t055-basicviewer.png`
- Screenshot interpreter log: `readiness/smoke/t055-basicviewer-screenshot.txt`

The sample contract smokes run through the public executable entry points and assert emitted host effects rather than calling internal helpers.
