# Compatibility Impact

## Verdict

PASS for documented in-scope compatibility changes.

## Active Ownership Change

Charts and graph widgets move under `FS.Skia.UI.Controls` ownership:

- generated capability selection no longer includes a separate `charts`
  capability
- default generated apps reference `FS.Skia.UI.Controls`, not
  `FS.Skia.UI.Charts`
- chart and graph catalog rows are Controls-owned
- chart and graph authoring guidance is provided by `fs-skia-ui-widgets`

## Preserved Boundaries

- `FS.Skia.UI.Scene` remains the lower-level scene composition layer.
- `FS.Skia.UI.SkiaViewer` remains the host/viewer boundary.
- `FS.Skia.UI.KeyboardInput` remains the keyboard binding capability.
- `FS.Skia.UI.Layout` remains a separate runtime package and capability.
- Existing Charts source remains compatibility source in the repository, but it
  is not active generated-product selection after this feature.

## Out Of Scope

This feature does not perform release publishing, package deprecation
automation, a migration tool, a new renderer backend, a platform support
promise, platform-native wrapper work, rich text editing, formal accessibility
certification, or designer tooling.

## Review Evidence

- `readiness/capability-catalog.md`
- `readiness/dependency-report.md`
- `readiness/generated-product-usage.md`
- `readiness/local-skills.md`
- `readiness/template-drift.md`
