# Package Boundary Readiness

## Scope

This feature preserves package identities while adding package-owned viewer,
input, scene evidence, testing, and generated consumer validation surfaces.
Generated consumers must use FS.Skia.UI packages rather than repository
implementation source.

## Evidence

- `./fake.sh build -t PackLocal` passed and wrote
  `readiness/package/local-packages.md`.
- `template/base/src/Product/Product.fsproj` uses FS.Skia.UI
  `PackageReference` entries for generated product dependencies.
- Generated consumer validation restored from the local feed and wrote
  `readiness/generated-product-validation.md`.
- Dependency governance evidence is recorded in `readiness/dependencies.md`
  and `readiness/dependency-report.md`.

## Result

The feature keeps ownership boundaries package-based and reports local package
setup drift before app, input, or rendering failures.
