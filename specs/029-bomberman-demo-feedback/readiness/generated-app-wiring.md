# Generated App Wiring

Status: complete.

Tasks: T033-T041
Captured: 2026-05-29T12:14:00+02:00

## Generated Checkout

Path: `artifacts/template-check/029-bomberman-demo-feedback/source-app`

## Pure MVU and Host Boundary Tests

Commands:

```text
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "generated app host|generated host MVU" --logger "console;verbosity=minimal"
dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj --logger "console;verbosity=minimal"
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated app guidance|persistent launch contract|generated graphical template" --logger "console;verbosity=minimal"
```

Results:

- SkiaViewer.Tests: 2 passed, 0 failed.
- Elmish.Tests: 4 passed, 0 failed.
- Governance.Tests: 3 passed, 0 failed.

Covered paths:

- public generated host value
- pure init/update transitions
- emitted viewer effects at host boundary
- key mapping
- tick mapping
- first-frame and close effects
- generated guidance naming `Product.Program.view`, `Product.Program.generatedHost`, and `Product.Program.update`

## Persistent Launch Smoke

Command:

```text
timeout 10s dotnet run --project src/V3DotnetAppSource/V3DotnetAppSource.fsproj
```

Result:

- Exit code: 124 from `timeout`, meaning the process remained alive until killed.
- Log: `readiness/generated-persistent-launch.log`
- Output contained GTK module warnings only:
  - `Failed to load module "colorreload-gtk-module"`
  - `Failed to load module "window-decorations-gtk-module"`

This validates that the generated default executable attempts persistent graphical launch and does not self-close through the bounded evidence path.

## Evidence Mode Separation

Generated source and tests assert that explicit evidence commands remain opt-in:

- default branch calls `Viewer.runApp viewerOptions generatedHost`
- `--launch-evidence`, `--bounded-smoke`, and screenshot/visual evidence commands stay out of the normal launch branch
- app-owned commands remain distinct from viewer effects until `interpretAtHostBoundary`
