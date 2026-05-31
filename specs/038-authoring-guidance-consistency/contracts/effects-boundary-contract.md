# Contract: Canonical Effects-Boundary Page (US5)

**Satisfies:** FR-009 · SC-005

## Artifact

`template/base/docs/effects-boundary.md`, bundled into every generated project.

## Required contents

1. **Both categories named.**
   - Application commands at the MVU edge (pure reducer outputs, e.g.
     `DispatchHostCommand`).
   - Viewer effects at the host boundary (e.g. `OpenWindow`, `ApplyWindowOptions`,
     `RenderScene`, `CaptureScreenshot`, `EmitDiagnostic`).
2. **The boundary explained.** App commands are not viewer effects; viewer
   effects are produced at the host boundary and must not be appended to app
   command lists.
3. **Canonical `update`→host wiring.** `Viewer.runApp viewerOptions
   Product.Program.generatedHost`, with the pure `Product.Program.update` and the
   `Viewer.GeneratedAppHost` callbacks (`Init`/`Update`/`View`/`OnTick`/`OnKey`/
   `ShouldClose`).

## Rules

- Self-contained: an author follows it without reading `docs/reports/*` or source.
- Single source of truth: scattered framework-repo mentions
  (`docs/reports/generated-apps.md`, `runtime-design.md`) are repointed/aligned to
  this page.
- Matches how a generated project actually wires effects.

## Enforcement

`GeneratedGuidanceCheck` asserts the page is present in generated output and
covers both categories + the wiring.

## Evidence

`readiness/effects-boundary.md`.
