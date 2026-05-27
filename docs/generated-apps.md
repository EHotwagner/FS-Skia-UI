# Generated Apps

Generated graphical apps must validate user-reachable input and rendering
paths through public package surfaces. For feature `013-tetris-demo-integration`
the generated app guidance must name:

- Scene-returning function: `Product.Program.view`, with generated signatures
  describing it as returning `FS.Skia.UI.Scene.Scene`.
- Generated host value: `Product.Program.generatedHost`.
- App reducer: `Product.Program.update`.
- Interactive run command: `dotnet run --project src/Product/Product.fsproj`.
- Persistent host default: `Viewer.runApp viewerOptions Product.Program.generatedHost`.
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
- Layout readability evidence command:
  `dotnet run --project src/Product/Product.fsproj -- --layout-evidence <path> 1280 720`.
  Generated game products must also validate the documented constrained size
  640x480 and report named HUD/gameplay regions, HUD text bounds, active
  gameplay bounds, overlap status, proof level, and diagnostics.
- Local package restore setup, including feed path, package identities,
  versions, consumer package configuration, and restore command.

Generated app tests should drive keyboard flows through normalized viewer key
events, not backend-specific raw string comparisons. Validation failures must
identify the app flow, input value, screen, rendering stage, diagnostic
category, package identity, or evidence path needed to act on the failure.

Bounded smoke, frame diagnostics, and scene evidence are explicit CI and
reviewer-diagnostic commands. They are not interactive readiness substitutes
for the default persistent graphical launch path.
The default command for a viewer-backed graphical profile must attempt
`Viewer.runApp viewerOptions Product.Program.generatedHost`; commands that
only print metadata, count controls, run bounded smoke, emit scene evidence, or
exit without a persistent launch attempt are diagnostic helpers only. Tests that
exercise the reducer should call `Product.Program.update`, not an unqualified
or framework-owned update helper.

Generated smoke reports should include a diagnostic mode and captured
diagnostic categories so startup failures can be read without repeated
frame-loop noise. Frame-loop messages should appear only in an explicit
frame-focused run.

Generated consumers must use local NuGet packages produced by `PackLocal` for
package validation. They must not copy repository implementation source to
stand in for package consumption.

Bounded-only graphical apps need an explicit migration decision before they can
claim readiness: adopt the persistent generated host, declare headless or
non-interactive scope, or document a missing persistent viewer capability as a
blocking product/package gap.

Repository validation for generated app flows is produced by `TemplateCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`, `PackLocal`, and
`GeneratedProductCheck`. The active feature stores the generated consumer
summary at `readiness/generated-product-validation.md` and detailed logs under
`readiness/generated-consumer-validation/`.
