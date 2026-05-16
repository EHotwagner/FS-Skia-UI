# US1 Validation

## Verdict

PASS: the default V3 product is generated as a consumer product for both
`app-source` and `app-package` rows.

## Evidence

| Evidence | Path |
|----------|------|
| Source file list | `specs/009-v3-modular-framework/readiness/generated-file-lists/app-source.txt` |
| Package file list | `specs/009-v3-modular-framework/readiness/generated-file-lists/app-package.txt` |
| Source generated Dev/Test/Verify logs | `specs/009-v3-modular-framework/readiness/generated-product-verify/app-source/` |
| Package generated Dev/Test/Verify logs | `specs/009-v3-modular-framework/readiness/generated-product-verify/app-package/` |
| Framework target log | `specs/009-v3-modular-framework/readiness/logs/us1-generated-product-check.txt` |

## Required Cleanliness Checks

- zero default framework samples
- zero framework galleries
- zero framework parity suite
- zero historical specs
- zero framework readiness directories
- zero framework docs
- zero framework README content
- zero framework implementation projects
- zero framework template package project
- selected project, Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts skills are present
- Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts are consumer-mode package references

## Command Checks

The generated product `Dev`, `Test`, and `Verify` commands were run through
the generated `fake.sh` wrapper for the `app-source` and `app-package` rows.
The generated product build excludes framework gallery, parity, package-surface
maintenance, template packaging, and framework-source maintenance targets.
