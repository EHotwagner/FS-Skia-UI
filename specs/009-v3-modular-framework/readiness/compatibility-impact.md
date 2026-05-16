# Compatibility Impact

## Summary

V3 changes the distribution shape from a copied framework repository to
generated products that consume capability packages.

## Affected Packages

- `FS.Skia.UI.Scene`
- `FS.Skia.UI.SkiaViewer`
- `FS.Skia.UI.Elmish`
- `FS.Skia.UI.KeyboardInput`
- `FS.Skia.UI.Layout`
- `FS.Skia.UI.Charts`
- `FS.Skia.UI.Testing`

## Generated Product Impact

Default generated products become lean consumer applications with selected
package references, product-owned tests, product docs, product governance, and
selected local skills. Framework samples, galleries, parity suites, historical
specs, readiness evidence, framework docs, framework README content,
implementation projects, and template maintenance roots are excluded by
default.

## Migration Stance

V2 migration implementation support is out of scope for this feature. Existing
V2 generated products are not automatically migrated by this work. Reviewers
should treat V3 package identities and generated product shape as a new
distribution contract.

## Reviewer Notes

Review package-specific `.fsi` contracts, dependency ownership, generated
product file lists, selected skill inventories, and package surface baselines
before approval.
