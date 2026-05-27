# Package Boundary

Status: changed and validated.

The public layout-evidence records remain in `FS.Skia.UI.Scene`; generated
validation and warning classification helpers remain in `FS.Skia.UI.Testing`.
Viewer launch, filesystem, package restore, process, font host, and window
system effects stay outside pure Scene/Testing validation helpers.

Package evidence:

- `PackageSurfaceCheck`: passed and recorded in `readiness/evidence-audit.md`.
- `GeneratedProductCheck`: passed and recorded in
  `readiness/generated-product-validation.md`.
- `exact-package-match=true` is recorded in `readiness/generated-validation.md`.

No new runtime dependency was added for layout evidence. Approximate text
measurement remains deterministic and disclosed as approximate/unsupported where
exact host facts are unavailable.
