# Generated Validation (085) — scaffold

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Scope

Feature 085 changes generated products through the new framework surface
(`Control.renderTree`, `InteractiveAppHost` / `Viewer.runInteractiveApp`, the
fixed `KeyboardInput.normalize`) and the regenerated/hand-edited docs
(`template/base/docs/scaffold-map.md`, `template/base/docs/evidence-formats.md`)
plus the new `fs-skia-viewer-host` skill consumed by generated products.

`authoritative=false` because the local `GeneratedProductCheck` is a known
**non-authoritative aggregate** environment-failure (see
`aggregate-hang-diagnostics.md` and memory `generated-product-check-env-failure`):
it cannot resolve a generated feature locally. No package identity changes; there
is no package mismatch to resolve.

**T035 result (recorded):** TemplatePack, TemplateInstallSource/Package, Build, Test,
SampleContractSmoke, TemplateInstantiate, and TemplateSmoke all **passed**; the generated
product **built cleanly** (no compile error from the new `renderTree`/`InteractiveAppHost`
surface). `GeneratedProductCheck` then failed at the app/source generated **Verify** step with
the known environment reason: *"Cannot resolve the feature to validate: no SPECKIT_FEATURE_DIR
override is set and …/085-showcase-feedback-followups/app-source/.specify/feature.json has no
usable feature_directory entry."* (`readiness/generated-product-verify/app-source/verify.log`).
This is the documented non-authoritative aggregate (memory `generated-product-check-env-failure`),
not a product defect — the authoritative generated-product signal runs on a host that provides a
resolvable feature.
