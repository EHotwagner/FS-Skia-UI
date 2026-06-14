# Generated Product Validation

package-set: Pinned

product-verdict: `ProductDefectFail`
- step `package-resolution`: passed=true classification=- package-set=Pinned
- step `generated-verify`: passed=false classification=ProductDefect package-set=Pinned
- step `bounded-smoke`: passed=false classification=ProductDefect package-set=Pinned
- step `scene-evidence`: passed=false classification=ProductDefect package-set=Pinned
- step `window-diagnostics`: passed=false classification=ProductDefect package-set=Pinned
- step `window-options`: passed=false classification=ProductDefect package-set=Pinned
- step `image-evidence`: passed=false classification=ProductDefect package-set=Pinned
- step `persistent-launch`: passed=false classification=ProductDefect package-set=Pinned

Category: `SemanticTestFailure`
Elapsed: `00:00:07.1204877`
Command context: `./fake.sh build -t PackLocal && ./fake.sh build -t GeneratedProductCheck`
Generated consumer root: `/home/developer/projects/FS-Skia-UI/artifacts/generated-products/121-live-host-idle-parking/app-source`
Local package feed: `/home/developer/.local/share/nuget-local`

## Evidence

- Restore log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/restore.log`
- Generated Verify log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/generated-verify.log`
- Bounded smoke log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/bounded-smoke.log`
- Bounded smoke evidence: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/bounded-smoke.txt`
- Scene evidence log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/scene-evidence.log`
- Scene evidence output: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/headless-scene-evidence.txt`
- Persistent launch diagnostics log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/persistent-launch-diagnostics.log`
- Window diagnostics log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/window-diagnostics.log`
- Window diagnostics output: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/window-diagnostics.txt`
- Window options log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/window-options.log`
- Window options output: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/window-options.txt`
- Image evidence log: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/image-evidence.log`
- Image evidence output: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/game-image-evidence.png`

## Contract Output

- package-resolution: `validated`
- exact-package-match: `True`
- generated-test-execution: `validated`
- generated-tests-ran: `False`
- default-interactive-launch: `False`
- bounded-evidence-validation: `False`
- close-reason-validation: `False`
- window-diagnostics-validation: `False`
- window-options-validation: `False`
- image-evidence-validation: `False`
- authoritative: `False`
- failure-class: `missing-generated-test-execution`

## Package Resolution

- exact-match: `True`
- failure-class: `none`
- package-sources: `https://api.nuget.org/v3/index.json`
- restore-warning-count: `0`

Requested packages:
- requested FS.Skia.UI.Controls=0.1.127-preview.1
- requested FS.Skia.UI.Controls.Elmish=0.1.127-preview.1
- requested FS.Skia.UI.Elmish=0.1.127-preview.1
- requested FS.Skia.UI.KeyboardInput=0.1.127-preview.1
- requested FS.Skia.UI.Layout=0.1.127-preview.1
- requested FS.Skia.UI.Scene=0.1.127-preview.1
- requested FS.Skia.UI.SkiaViewer=0.1.127-preview.1

Resolved packages:
- resolved FS.Skia.UI.Controls=0.1.127-preview.1
- resolved FS.Skia.UI.Controls.Elmish=0.1.127-preview.1
- resolved FS.Skia.UI.Elmish=0.1.127-preview.1
- resolved FS.Skia.UI.KeyboardInput=0.1.127-preview.1
- resolved FS.Skia.UI.Layout=0.1.127-preview.1
- resolved FS.Skia.UI.Scene=0.1.127-preview.1
- resolved FS.Skia.UI.SkiaViewer=0.1.127-preview.1

Restore warnings:

## Generated Test Execution

- generated-tests-exist: `True`
- generated-tests-ran: `False`
- generated-verify-ran: `False`
- authoritative: `False`
- failure-class: `missing-generated-test-execution`

## Diagnostics

- feature-context: SPECKIT_FEATURE_DIR=/home/developer/projects/FS-Skia-UI/artifacts/generated-products/121-live-host-idle-parking/app-source/specs/seed-087-generated-verify (FR-001 seeded resolvable feature)
- generated consumer restore from local packages: ok
- package resolution: exact-match=true
- generated consumer Verify: failed: generated consumer Verify failed with exit code 1. See /home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/generated-consumer-validation/generated-verify.log
