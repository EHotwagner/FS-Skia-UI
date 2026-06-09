# Generated Validation (086) — scaffold

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=true
generated-tests-ran=true
authoritative=true
failure-class=none

## Scope

Feature 086 changes generated products through the neutral controls-first scaffold
(neutral `Model.fs`/`View.fs`, real-control default `view` via `Control.renderTree`,
re-pointed `LayoutEvidence`/`EvidenceCommands`/`Program`) and the controls-family pointer-host
governed default (`ControlsElmish.runInteractiveApp` + `interactiveHost`), plus the generalized
per-family host-lock governance assertions and the `fs-skia-viewer-host` warm-up doc.

`exact-package-match=true` / `generated-tests-ran=true` / `authoritative=true`: the generated
`app` product was generated, restored against the **pinned** `FS.Skia.UI.* 0.1.91-preview.1`
packages, **built cleanly**, and its **29 tests passed** (including the rewritten neutral
BehaviorTests and the SC-003 pointer-dispatch test, which uses only 085-available APIs).
`failure-class=none` for the authoritative build+test signal.

`GeneratedProductCheck` then fails at the app/source generated **Verify** step with the
documented non-authoritative environment reason: *"Cannot resolve the feature to validate:
no SPECKIT_FEATURE_DIR override is set and …/086-…/app-source/.specify/feature.json has no
usable feature_directory entry."* (`readiness/generated-product-verify/app-source/verify.log`,
`readiness/generated-product-check.md`). This is the documented non-authoritative aggregate
(memory `generated-product-check-env-failure`) — not a product defect; the authoritative
generated-product signal (build + 29 tests) is green. A real regression (`rendered.Bounds`,
an 086-only field absent from the pinned 085 package) was caught by this gate and fixed.
