# Generated Product Check

schema_version: 1.0

Generated-product structural rule lifecycle:
- `product-app-present`: required — exactly one product app project
- `product-test-suite-present`: required — exactly one product test suite
- `required-files-present`: required — required generated files present
- `api-surface-bundled`: required — bundled docs/api-surface stays in lockstep with source
- `effects-boundary-doc`: required — self-contained docs/effects-boundary.md
- `no-demo-identifiers`: required — generated starter carries no demo identifiers
- `consumer-facing-skills`: required — generated capability skills are consumer-facing
- `widgets-skill-present`: required — generated app carries the widgets skill
- `no-stale-charts`: required — no stale Charts package or skill
- `controls-elmish-reference`: required — generated app references Controls.Elmish
- `persistent-host-wiring`: required — generated app wires the persistent viewer host
- `no-bounded-only-default`: required — no bounded-only/print-only default launch path
- `claude-skill-peers`: required — .claude skill peers present for every .agents skill
- `no-framework-source`: required — no copied framework implementation/source/specs

Contract changelog:
- 1.0 `(baseline)` added: Structural contract codified with an explicit schema version and deprecation window (feature 046).

PASS: generated product file lists, selected skills, Controls-owned form/chart/graph/DataGrid authoring, Controls.Elmish adapter references, consumer-mode package references, stale Charts exclusions, full product governance command logs, and framework-source exclusions passed.

| Row | File list | Verify log |
|-----|-----------|------------|
| app/source | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-file-lists/app-source.txt` | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-product-verify/app-source/verify.log` |
| app/package | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-file-lists/app-package.txt` | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-product-verify/app-package/verify.log` |
| headless-scene/source | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-file-lists/headless-scene-source.txt` | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-product-verify/headless-scene-source/verify.log` |
| governed/source | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-file-lists/governed-source.txt` | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-product-verify/governed-source/verify.log` |
| sample-pack/source | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-file-lists/sample-pack-source.txt` | `/home/developer/projects/FS-Skia-UI/specs/116-paint-cache-damage-rects/readiness/generated-product-verify/sample-pack-source/verify.log` |
