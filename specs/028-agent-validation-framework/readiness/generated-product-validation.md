# Generated Product Validation

Category: `Completed`
Elapsed: `00:00:20.7844064`
Command context: `./fake.sh build -t PackLocal && ./fake.sh build -t GeneratedProductCheck`
Generated consumer root: `/home/developer/projects/FS-Skia-UI/artifacts/generated-products/028-agent-validation-framework/app-source`
Local package feed: `/home/developer/.local/share/nuget-local`

## Evidence

- Restore log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/restore.log`
- Generated Verify log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/generated-verify.log`
- Bounded smoke log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/bounded-smoke.log`
- Bounded smoke evidence: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/bounded-smoke.txt`
- Scene evidence log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/scene-evidence.log`
- Scene evidence output: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/headless-scene-evidence.txt`
- Persistent launch diagnostics log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/persistent-launch-diagnostics.log`
- Window diagnostics log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/window-diagnostics.log`
- Window diagnostics output: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/window-diagnostics.txt`
- Window options log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/window-options.log`
- Window options output: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/window-options.txt`
- Image evidence log: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/image-evidence.log`
- Image evidence output: `/home/developer/projects/FS-Skia-UI/specs/028-agent-validation-framework/readiness/generated-consumer-validation/game-image-evidence.png`

## Contract Output

- package-resolution: `validated`
- exact-package-match: `True`
- generated-test-execution: `validated`
- generated-tests-ran: `True`
- default-interactive-launch: `True`
- bounded-evidence-validation: `True`
- close-reason-validation: `True`
- window-diagnostics-validation: `True`
- window-options-validation: `True`
- image-evidence-validation: `True`
- authoritative: `True`
- failure-class: `none`

## Package Resolution

- exact-match: `True`
- failure-class: `none`
- package-sources: `/home/developer/.local/share/nuget-local, https://api.nuget.org/v3/index.json`
- restore-warning-count: `0`

Requested packages:
- requested FS.Skia.UI.Controls=0.1.27-preview.1
- requested FS.Skia.UI.Controls.Elmish=0.1.27-preview.1
- requested FS.Skia.UI.Elmish=0.1.27-preview.1
- requested FS.Skia.UI.KeyboardInput=0.1.27-preview.1
- requested FS.Skia.UI.Layout=0.1.27-preview.1
- requested FS.Skia.UI.Scene=0.1.28-preview.1
- requested FS.Skia.UI.SkiaViewer=0.1.29-preview.1

Resolved packages:
- resolved FS.Skia.UI=0.1.27-preview.1
- resolved FS.Skia.UI.Controls=0.1.27-preview.1
- resolved FS.Skia.UI.Controls.Elmish=0.1.27-preview.1
- resolved FS.Skia.UI.Elmish=0.1.27-preview.1
- resolved FS.Skia.UI.KeyboardInput=0.1.27-preview.1
- resolved FS.Skia.UI.Layout=0.1.27-preview.1
- resolved FS.Skia.UI.Scene=0.1.28-preview.1
- resolved FS.Skia.UI.SkiaViewer=0.1.29-preview.1

Restore warnings:

## Generated Test Execution

- generated-tests-exist: `True`
- generated-tests-ran: `True`
- generated-verify-ran: `True`
- authoritative: `True`
- failure-class: `none`

## Diagnostics

- generated consumer restore from local packages: ok
- package resolution: exact-match=true
- generated consumer Verify: ok
- generated consumer bounded smoke: ok
- bounded viewer smoke reached requested evidence
- generated consumer scene evidence: ok
- headless scene evidence captured
- generated consumer window diagnostics: ok
- window diagnostics validation captured
- generated consumer window options: ok
- window options validation captured
- generated consumer image evidence: ok
- image evidence validation captured
- generated consumer persistent launch diagnostics: ok
- persistent launch diagnostics captured separately from bounded evidence
- supported-host persistent launch evidence normalized
