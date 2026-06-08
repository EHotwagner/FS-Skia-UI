# Contract — `ControlFidelityCheck` Build Target & Report

**Surface**: governance/build contract (escalated). Registered in compiled F#
(`build/Governance`), rendered into `validation.contract.yml`. This is the
contract that drives the spec's escalation.

## Target registration (all required; a missing case is a compile error)

1. `Targets.Target` (`Targets.fs/.fsi`): add case `ControlFidelityCheck`.
2. `Targets.allTargets`: include it (registry order, near `ControlsCatalogDocsCheck`).
3. `Targets.name`: `ControlFidelityCheck -> "ControlFidelityCheck"`.
4. `Targets.spec`/timeout/cost/owner: `timeoutClass = "medium"` (render-capable,
   native startup), `cost = "medium"`, `failureOwner = "product"`.
5. `AgentValidation.knownGates`: add `"ControlFidelityCheck"`.
6. `Routing.fs`: add to the existing `controls-catalog-docs` rule's
   `RequiredGates`, and extend its `Paths` so editing the harness fidelity
   sources/fixtures triggers the gate:
   ```
   "tests/ControlsPreview.Harness/**"
   ```
   (plus the existing `docs/controls/**`, `docs/img/controls/**`,
   `build/Governance/CatalogDocsGen.*`, `CatalogGen.fs`, `src/Controls/catalog.yml`).
   This makes the fidelity gate a **required** part of evidence for any
   preview-asset change (FR-012).
7. Regenerate `validation.contract.yml` via `./fake.sh build -t
   RefreshSurfaceBaselines`; `TargetMetadataDrift` enforces currency (no hand-edit).

## Execution (render-capable; SkiaSharp-free build side)

`Engine/Update.fs` `StartTarget Targets.ControlFidelityCheck` emits a process
effect that shells out to the render-capable harness, mirroring the existing
`SkiaViewer.Tests -- --sequenced` pattern (`Update.fs:61`):

```
dotnet run --project tests/ControlsPreview.Harness --no-restore -- --fidelity
```

- The harness (`Fidelity.fs`) decodes each committed `docs/img/controls/<id>.png`,
  re-renders each Demonstrative control for its `Scene.describe` kinds, checks the
  `ContentSignature`, and writes the report.
- Separation guarantee (FR-008): `FS.Skia.UI.Build` adds **no** SkiaSharp
  reference; the SkiaSharp-free `ControlsCatalogDocsCheck` byte-floor currency
  gate is unchanged and still runs in GPU-free CI.

## Pass / fail semantics

- **PASS** iff: every Demonstrative control's committed PNG satisfies its
  `ContentSignature` (pixel + primitive-kind); every `Unsupported` control has no
  committed PNG; the sample set is total over `catalogFacts` (fail-closed); and
  every fixture matches its expected verdict.
- **FAIL** with an actionable message **naming the control and the missing
  signature component** when: a signature is unmet (kind absent / coverage below
  threshold), a Demonstrative control has no PNG, a catalog id lacks a
  declaration, or a fixture's verdict is wrong.
- **Blocking host warning** (not silent pass) when native Skia is unavailable so
  decoding cannot run — classified per `fs-skia-evidence-mode`.

## Retained fixture matrix (SC-003)

`tests/ControlsPreview.Harness/fixtures/fidelity/`:
- `lowfi/<id>.png` — committed 079-style label-on-box renders (sourced from
  pre-fix `main`). The gate MUST report each as **fail** against `<id>`'s
  signature.
- `faithful/<id>.png` — the regenerated faithful counterpart. The gate MUST
  report each as **pass**.

This is the durable red→green guard: a label-on-box can never again pass the gate.

## Report — `readiness/control-fidelity.md`

Decoded-content report; one row per catalog control plus a fixture-matrix section:

| field | meaning |
|-------|---------|
| control id | catalog id |
| declared | `Signature` summary or `UnsupportedNoPreview` |
| coverage | non-background fraction outside the title band (or n/a) |
| distinct colors | distinct non-background colors outside band (or n/a) |
| present kinds | `Scene.describe` kinds observed on the live render |
| verdict | pass / fail |
| failure reason | populated on fail, names the missing component |

Fixture matrix: every `lowfi` row = fail (expected), every `faithful` row = pass
(expected); any deviation fails the gate.
