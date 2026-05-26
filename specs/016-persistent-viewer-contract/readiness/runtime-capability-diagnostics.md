# Runtime Capability Diagnostics Readiness

Status: scaffolded for supported-host, unsupported-host, missing-capability, and renderer-mode diagnostics.

## T033 Runtime Capability Tests

`Viewer.runtimeCapability()` test coverage now asserts:

- persistent window support is separated from unsupported host reasons
- bounded smoke support remains available as explicit helper capability
- keyboard input support is reported separately
- renderer mode is reported as `skia`
- missing package capabilities remain separate from unsupported host reasons

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "runtime capability"` passed.

## T034 Generated App Diagnostic Tests

Generated app source tests now require the default path to report:

- command
- unsupported-host reasons
- missing package capability
- blocked stage
- classification
- category
- reviewer-facing message

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated app default diagnostics"` passed.

## T035 Runtime Capability Implementation

`Viewer.runtimeCapability()` reports persistent window support, bounded smoke
support, keyboard support, renderer mode, unsupported host reasons, and missing
package capabilities separately. Persistent launch failures classify
unsupported hosts as `UnsupportedEnvironment` with blocked stage `Window` and
startup diagnostics; product configuration failures remain `ProductDefect`.

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed.

## T036 Generated App Diagnostic Wiring

The generated app default path now queries `Viewer.runtimeCapability()` before
launch and reports command, unsupported-host reasons, missing package
capability, blocked stage, classification, category, and message without
falling back to bounded simulation as success.

Verification:

- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --filter "persistent|diagnostic|bounded|default"` passed.
- `timeout 20s dotnet run --project template/base/src/Product/Product.fsproj --no-restore` reported `status=ok mode=persistent-window command=dotnet run --project src/Product/Product.fsproj ... missing-package-capability=none unsupported-host-reasons=none`.

## T037 Generated Product Diagnostic Artifacts

Generated consumer validation now records default persistent launch diagnostics
separately from bounded smoke and deterministic scene evidence:

- `readiness/generated-consumer-validation/persistent-launch-diagnostics.log`

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "persistent launch diagnostics"` passed.
- `./fake.sh build -t GeneratedProductCheck` passed and refreshed `readiness/generated-product-validation.md` with a distinct persistent launch diagnostics log.

## T038 Reviewer Classification Guide

Supported classification:

- `status=ok`
- `mode=persistent-window`
- `window-opened=true`
- `exit-path=true`
- `classification=none` or omitted in the typed success outcome
- `renderer-mode=skia`

Unsupported-environment classification:

- `status=unsupported`
- `mode=persistent-window`
- `blocked-stage=Window`
- `classification=UnsupportedEnvironment`
- `category=Startup`
- message names the missing host capability, such as `DISPLAY` or `WAYLAND_DISPLAY`

Missing-capability classification:

- `missing-package-capability` is non-`none`
- reviewer treats this as a product/package capability gap, not an unsupported host
- bounded smoke or scene evidence cannot close the gap

Renderer-mode classification:

- persistent launch and generated product diagnostics must report `renderer-mode=skia`
- bounded smoke may report its helper renderer mode separately

Two-minute SC-007 checklist:

1. Confirm the default command log contains `mode=persistent-window`.
2. Confirm `command=` names the generated app default executable.
3. Confirm supported-host evidence has `window-opened=true`, `input-dispatch=true` where keyboard behavior is declared, and `exit-path=true`.
4. If the status is `unsupported`, confirm the blocked stage, classification, category, and message identify host environment limits.
5. Confirm `missing-package-capability=none` before accepting unsupported-host diagnostics as host-only.
6. Confirm bounded smoke, first-frame, frame-count, and scene evidence are recorded separately from persistent launch readiness.
