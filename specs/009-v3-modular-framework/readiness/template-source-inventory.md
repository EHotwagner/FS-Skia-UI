# Template Source Inventory

Date: 2026-05-16

## Current Template Shape

| Asset | Current path | Notes |
|-------|--------------|-------|
| Template metadata | `.template.config/template.json` | V2 template copies from repository root and excludes historical specs and selected optional paths. |
| Generated AGENTS guidance | `.template.config/generated/AGENTS.md` | Replaces source feature-specific AGENTS guidance in generated products. |
| Template package project | `.template.package/FS.Skia.UI.Template.fsproj` | Used by `TemplatePack` for package artifact validation. |
| Command wrappers | `fake.sh`, `fake.cmd` | Copied into generated products by V2 template. |
| Build workflow | `build.fsx` | V2 target graph validates source/package template rows and generated `Dev`. |

## Current Generated-Output Exclusions

The V2 template excludes `.fake/`, `.git/`, `.specify/feature.json`,
`.template.config/`, `.template.package/`, `artifacts/`, build outputs, and
historical `specs/**` content. The minimal profile additionally excludes
Charts, Layout, parity/smoke tests, and most sample galleries.

## V3 Required Change

V3 generation must be product-first. The template source needs
`template/base/`, capability fragments under `template/fragments/`, a
machine-readable capability catalog, and profile rows for `app`,
`headless-scene`, `governed`, and `sample-pack`.

The generated product must contain product code, product tests, product docs,
selected skills, command wrappers, and full product governance. It must not
copy framework samples, galleries, historical specs, framework readiness,
framework docs, framework README content, implementation projects, or template
maintenance roots in normal consumer mode.
