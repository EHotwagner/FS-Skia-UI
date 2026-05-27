# Evidence Obligations

## Tier 1 Scope

This feature is a broad Tier 1 runtime and governance change. It changes generated graphical launch behavior, public SkiaViewer contracts, generated validation, visual evidence semantics, readiness gates, and audit expectations.

## Public API Impact

Expected public API changes are owned by `src/SkiaViewer/SkiaViewer.fsi` and must cover close reasons, observed values, window behavior requests, option results, launch outcomes, visual evidence artifacts, lifecycle `Model`/`Msg`/`Effect`, public `init`, pure `update`, and interpreter boundaries.

Surface baselines must be refreshed only after implementation using the planned `RefreshSurfaceBaselines` and `PackageSurfaceCheck` targets.

## Generated Product Impact

The generated default executable path must launch the persistent interactive visible-window mode. Bounded first-frame, screenshot/image, pixel-readback, and metadata/hash evidence must require explicit commands or flags. Generated tests and generated validation must prove package resolution, generated test execution, visible-window diagnostics, close reason separation, window options, and image evidence claims.

## Package Impact

No new runtime dependency is planned. Package contents and generated package consumers may change because public viewer capabilities and generated validation artifacts change. Any newly required native screenshot or window-inspection dependency must update package pinning, package guidance, dependency documentation, and `DependencyReport` evidence.

## Unsupported Scope

Out of scope: new game engines, new generated game mechanics, unrelated controls/charts/DataGrid work, release automation, marketplace distribution, and guarantees for unsupported desktop sessions beyond clear diagnostics and fallback evidence.

## Required Evidence Paths

- `readiness/interactive-visible-window.md`
- `readiness/close-reason-separation.md`
- `readiness/window-state-diagnostics.md`
- `readiness/window-options.md`
- `readiness/real-image-evidence.md`
- `readiness/generated-validation.md`
- `readiness/evidence-audit.md`

## Elmish/MVU Evidence Obligations

Window visibility is stateful and I/O-bearing. Public evidence must exercise
`init` and pure `update` paths for lifecycle transitions, assert emitted
effects, and separately run the interpreter or generated host path where the
filesystem, process, native window, render, image capture, or diagnostic effects
are executed. `update` evidence alone is not enough for `[US*]` tasks.

## Fake Window-Loop Limits

Fake or synthetic window-loop fixtures are allowed only for the approved
unreachable/error classifications: hidden, unmapped, off-screen, minimized-only,
surface-less, corrupt metadata, missing generated-validation fields, invalid
arguments, and hostile artifact paths. They must be marked `[S]` unless the task
is pure test/design work, and they cannot replace supported-host visible-window
evidence, real image evidence, generated test execution, or package-resolution
evidence.

## Real Interpreter Evidence

Supported-host readiness must run through the public generated executable or the
public SkiaViewer entry point wired to the interpreter boundary. Required real
interpreter evidence includes desktop/session prechecks, native window creation,
visibility/focusability facts where observable, first-frame persistence,
input/close observation, image artifact creation/decodability when requested,
and generated validation output.

## Supported-Host Visible-Window Evidence

On a supported desktop host, `interactive-visible-window.md` must prove
`mode=interactive-window`, an accessible visible window, first-frame
presentation without evidence self-close, and completion only after
user/app/host/failure close. Unsupported hosts may record diagnostics, but an
unsupported-host-only record is not a successful visible-window substitute.

## Validation Obligations

Small validation covers isolated readiness/documentation changes. Medium
validation covers one implementation area, one public contract, or one generated
workflow. Broad validation is required before completion because this feature
changes public runtime behavior, generated templates, package validation,
readiness artifacts, and audit governance. Broad validation runs through
`./fake.sh build -t Verify`, `EvidenceGraph`, and `EvidenceAudit`.

Non-authoritative aggregate results must be recorded with the failing class,
stage, elapsed duration, last observed command, focused rerun command, focused
rerun result, and reviewer disposition. A focused pass after an aggregate
timeout is not a product pass; it remains non-authoritative aggregate evidence
until the aggregate blocker is isolated or fixed.
