---
title: Design Decisions
category: Design
categoryindex: 4
index: 5
description: Rationale for the major FS.Skia.UI architecture and governance decisions.
---

# Design Decisions

This document records the major architectural choices behind FS.Skia.UI. It is
not a full ADR archive; it is the current decision log for the choices that
shape subsystem design, testing, packaging, and template governance.

## DD-001: Elmish Program Boundary

**Decision:** Applications use `Model`, `Msg`, `init`, `update`, `view`, and
explicit effects through `ViewerProgram<'model, 'msg>`.

**Rationale:** UI state and workflow transitions must be testable without a
window or GPU. Elmish gives contributors one consistent way to express
stateful behavior across samples, runtime hosts, keyboard input, layout
workflow, and build workflow governance.

**Consequences:** `update` paths should stay pure. Host work enters through
messages and leaves through effects. New stateful features need model/message
coverage and effect assertions before they are treated as complete. See
[src/Lib/Library.fsi](../src/Lib/Library.fsi) for `ViewerProgram` and
`ViewerEffect`, [src/Lib/KeyboardInput.fsi](../src/Lib/KeyboardInput.fsi) for
keyboard input's reducer surface, [src/Layout/Layout.fsi](../src/Layout/Layout.fsi)
for layout workflow functions, and [build.fsx](../build.fsx) for the local
build workflow effect algebra.

## DD-002: Declarative Scene Model

**Decision:** Public UI output is an immutable `Scene`, not direct Skia drawing
callbacks.

**Rationale:** Scene values can be inspected, tested, transformed, composed,
and reused by charts, layout, keyboard displays, and samples. Direct drawing
callbacks would couple product code to renderer lifetime and make non-visual
testing much weaker.

**Consequences:** Renderer-specific details stay behind `Viewer.run`.
Subsystems should return scenes and diagnostics, not Skia objects. The public
`Scene` contract is in [src/Lib/Library.fsi](../src/Lib/Library.fsi), and pure
subsystem scene builders live in [src/Charts](../src/Charts/) and
[src/Layout](../src/Layout/).

## DD-003: Vulkan-Only Desktop Renderer

**Decision:** The first renderer is Windows/Linux desktop with Vulkan, Silk.NET,
and SkiaSharp. There is no OpenGL, CPU, browser, mobile, or fallback renderer.

**Rationale:** A narrow renderer target keeps the first public surface coherent
and makes unsupported environment diagnostics explicit. Silent fallback would
hide driver/platform failures and weaken evidence.

**Consequences:** Startup failures are reported through `RenderDiagnostic`.
Documentation and tests must avoid implying broader platform support until a
future feature adds and verifies it. The Vulkan/Silk.NET/SkiaSharp host code is
implemented in [src/Lib/Library.fs](../src/Lib/Library.fs).

**Roadmap boundary:** Current support remains .NET desktop with the Vulkan
renderer path and SkiaSharp preview dependency risk. macOS, mobile, browser,
OpenGL, CPU/software rendering, and fallback renderer support are separate
future platform-expansion features, not current support.

## DD-004: Pure Widget Packages Over Host-Owned Widgets

**Decision:** Charts, DataGrid, layout, and graph features are pure scene
builders and reducers. They do not own windows, renderer state, or host effects.

**Rationale:** This keeps `FS.Skia.UI.Charts` and `FS.Skia.UI.Layout`
composable and usable from samples or products that own their own Elmish state.

**Consequences:** Interaction state such as selection, sorting, viewport,
focus, and visibility belongs to the caller. Subsystems provide hit testing,
validation, computed bounds, and scene output. Chart and table code lives in
[src/Charts](../src/Charts/); layout and graph code lives in
[src/Layout](../src/Layout/).

## DD-005: Visibility Lives In Signature Files

**Decision:** Public runtime surface is governed by `.fsi` files and package
surface baselines.

**Rationale:** F# signature files make the exported API explicit and reviewable.
Baselines catch accidental public surface drift.

**Consequences:** Runtime public API changes require matching `.fsi`,
semantic tests, package surface review, and evidence updates. The runtime
signature files are in [src/Lib](../src/Lib/), [src/Charts](../src/Charts/),
and [src/Layout](../src/Layout/); package baselines are in
[readiness/surface-baselines](../readiness/surface-baselines/).

## DD-006: Non-Visual Contract Smoke As The Fast Path

**Decision:** Every current sample has a `--contract-smoke` path that exercises
public APIs without opening a live window.

**Rationale:** Live Vulkan/window validation is valuable but environment-heavy.
Contract smoke keeps the default `Verify` and `Ci` gates useful on developer and
automation machines that can build and run non-visual tests.

**Consequences:** Contract smoke is not a substitute for explicit visual
evidence when a feature requires pixel or live-renderer proof. Deferred visual
scope must remain documented until a future target owns it. The sample hosts
live in [samples](../samples/), with smoke coverage in
[tests/Smoke.Tests](../tests/Smoke.Tests/).

## DD-007: Repository As Governed Template Source

**Decision:** The repository itself owns the `fs-skia-ui` template and validates
both source-directory and local-package template installation.

**Rationale:** Generated products should inherit the current build, dependency,
documentation, evidence, and Spec Kit governance without manual copying from
historical feature directories.

**Consequences:** Template-owned source changes must stay aligned with template
metadata, docs, dependency policy, generated guidance, build targets, or an
explicit deferral. `TemplateDrift` enforces this boundary through
[scripts/template-drift.fsx](../scripts/template-drift.fsx), while template
metadata lives in [.template.config/template.json](../.template.config/template.json).

## DD-008: Central Package Management

**Decision:** Direct external package versions are centralized in
`Directory.Packages.props`.

**Rationale:** Runtime, test, sample, and generated-project dependency versions
must be reviewable in one place. This is especially important while SkiaSharp 4
is still preview-risk.

**Consequences:** Repo-owned project files use versionless external
`PackageReference` entries. Inline package versions are allowed only for
documented validation-only local package checks. Versions are declared in
[Directory.Packages.props](../Directory.Packages.props), and the report script
is [scripts/dependency-report.fsx](../scripts/dependency-report.fsx).
