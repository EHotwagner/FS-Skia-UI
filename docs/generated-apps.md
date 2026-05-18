# Generated Apps

Generated graphical apps must validate user-reachable input and rendering
paths through public package surfaces. For feature `013-tetris-demo-integration`
the generated app guidance must name:

- Interactive run command: `dotnet run --project src/Product/Product.fsproj`.
- Viewer-key driven start, options, primary interaction, pause/back, and
  restart/exit flows where those screens exist.
- Bounded real-viewer smoke command:
  `dotnet run --project src/Product/Product.fsproj -- --bounded-smoke <path>`
  and its unsupported-host behavior.
- Startup-focused diagnostics remain the default bounded smoke mode; generated
  apps may expose a separate frame-diagnostics smoke command that enables the
  frame category with an explicit sample limit.
- Deterministic scene-level visual evidence command:
  `dotnet run --project src/Product/Product.fsproj -- --scene-evidence <path>`.
- Local package restore setup, including feed path, package identities,
  versions, consumer package configuration, and restore command.

Generated app tests should drive keyboard flows through normalized viewer key
events, not backend-specific raw string comparisons. Validation failures must
identify the app flow, input value, screen, rendering stage, diagnostic
category, package identity, or evidence path needed to act on the failure.

Generated smoke reports should include a diagnostic mode and captured
diagnostic categories so startup failures can be read without repeated
frame-loop noise. Frame-loop messages should appear only in an explicit
frame-focused run.

Generated consumers must use local NuGet packages produced by `PackLocal` for
package validation. They must not copy repository implementation source to
stand in for package consumption.

Repository validation for generated app flows is produced by `TemplateCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `PackLocal`, and
`GeneratedProductCheck`. The active feature stores the generated consumer
summary at `readiness/generated-product-validation.md` and detailed logs under
`readiness/generated-consumer-validation/`.
