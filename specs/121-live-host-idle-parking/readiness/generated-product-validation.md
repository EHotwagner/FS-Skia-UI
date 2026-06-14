# Generated Product Validation

package-set: Pinned

product-verdict: `ProductPass`
- step `package-resolution`: passed=true classification=- package-set=Pinned
- step `generated-verify`: passed=true classification=- package-set=Pinned
- step `bounded-smoke`: passed=true classification=- package-set=Pinned
- step `scene-evidence`: passed=true classification=- package-set=Pinned
- step `window-diagnostics`: passed=true classification=- package-set=Pinned
- step `window-options`: passed=true classification=- package-set=Pinned
- step `image-evidence`: passed=true classification=- package-set=Pinned
- step `persistent-launch`: passed=true classification=- package-set=Pinned

Category: `Completed`
Elapsed: `00:00:21.3218424`
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
- generated-tests-ran: `True`
- default-interactive-launch: `False`
- bounded-evidence-validation: `True`
- close-reason-validation: `True`
- window-diagnostics-validation: `True`
- window-options-validation: `True`
- image-evidence-validation: `True`
- authoritative: `False`
- failure-class: `interactive-launch-validation`

## Package Resolution

- exact-match: `True`
- failure-class: `none`
- package-sources: `https://api.nuget.org/v3/index.json`
- restore-warning-count: `0`

Requested packages:
- requested FS.Skia.UI.Controls=0.1.128-preview.1
- requested FS.Skia.UI.Controls.Elmish=0.1.128-preview.1
- requested FS.Skia.UI.Elmish=0.1.128-preview.1
- requested FS.Skia.UI.KeyboardInput=0.1.128-preview.1
- requested FS.Skia.UI.Layout=0.1.128-preview.1
- requested FS.Skia.UI.Scene=0.1.128-preview.1
- requested FS.Skia.UI.SkiaViewer=0.1.128-preview.1

Resolved packages:
- resolved FS.Skia.UI.Controls=0.1.128-preview.1
- resolved FS.Skia.UI.Controls.Elmish=0.1.128-preview.1
- resolved FS.Skia.UI.Elmish=0.1.128-preview.1
- resolved FS.Skia.UI.KeyboardInput=0.1.128-preview.1
- resolved FS.Skia.UI.Layout=0.1.128-preview.1
- resolved FS.Skia.UI.Scene=0.1.128-preview.1
- resolved FS.Skia.UI.SkiaViewer=0.1.128-preview.1

Restore warnings:

## Generated Test Execution

- generated-tests-exist: `True`
- generated-tests-ran: `True`
- generated-verify-ran: `True`
- authoritative: `True`
- failure-class: `none`

## Diagnostics

- feature-context: SPECKIT_FEATURE_DIR=/home/developer/projects/FS-Skia-UI/artifacts/generated-products/121-live-host-idle-parking/app-source/specs/seed-087-generated-verify (FR-001 seeded resolvable feature)
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
