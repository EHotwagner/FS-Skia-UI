# Quickstart: Skia Controls Library

This quickstart describes the validation workflow expected after implementation
of the Skia controls library plan.

## 1. Validate Framework Development Workflow

```bash
./fake.sh build -t Dev
./fake.sh build -t CapabilityCheck
./fake.sh build -t SkillCheck
./fake.sh build -t DependencyReport
```

Expected outcome:

- framework projects restore, build, and run default tests
- `controls` appears in the capability catalog
- default app capabilities resolve to Scene, SkiaViewer, Elmish, KeyboardInput,
  Layout, and Controls
- Charts is no longer an active generated capability
- selected skills include `fs-skia-ui-widgets` and exclude generated
  `fs-skia-charts` and `fs-skia-layout`
- dependency report shows Controls ownership and no unexpected dependency leaks

## 2. Validate Controls Public Surface

```bash
./fake.sh build -t PackLocal
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t FsiTranscripts
```

Expected outcome:

- `FS.Skia.UI.Controls` packs to `~/.local/share/nuget-local/`
- all public Controls modules have curated `.fsi` signatures
- `readiness/surface-baselines/FS.Skia.UI.Controls.txt` exists and matches the
  approved surface
- FSI transcripts construct representative controls through the packed library
- compatibility impact for removed Charts ownership is documented

## 3. Validate Catalog, Interaction, Text, And Accessibility

```bash
./fake.sh build -t ControlsCatalogCheck
./fake.sh build -t ControlsInteractionCheck
./fake.sh build -t ControlsRenderingCheck
```

If these checks are folded into existing targets instead of added as standalone
targets, `./fake.sh build -t Verify` must produce the same evidence.

Expected outcome:

- at least 30 controls or variants are marked supported in the catalog
- every supported control has purpose, attributes, events where applicable,
  visual states, accessibility metadata, examples, tests, and evidence
- interaction tests dispatch expected messages exactly once
- text entry covers single-line, multi-line, cursor, selection, clipboard,
  validation, commit/cancel, and IME/composition diagnostics
- list/table validation covers 10,000 items
- reference gallery evidence covers three viewport sizes and two scale factors

## 4. Validate Generated Products

```bash
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
```

Expected outcome:

- default generated app references `FS.Skia.UI.Controls`
- default generated app does not reference `FS.Skia.UI.Charts`
- default generated app includes a product-owned controls example view
- generated products contain exactly one product app and one product test suite
  unless a profile explicitly allows more
- generated products contain full product governance
- generated products contain no framework samples, galleries, historical specs,
  readiness evidence, framework docs, framework README copy, or framework
  implementation projects
- generated products receive `fs-skia-ui-widgets` and do not receive stale
  chart or generated layout guidance skills

## 5. Validate Generated Product Commands

From each generated validation root:

```bash
./fake.sh build -t Dev
./fake.sh build -t Test
./fake.sh build -t Verify
```

Expected outcome:

- generated product controls example compiles and verifies
- generated product `Verify` runs product governance, evidence gates, drift
  checks, generated guidance checks, and readiness workflow
- generated product `Verify` does not run framework galleries, parity suite,
  framework package-surface maintenance, or framework template packaging checks

## 6. Validate Governance Evidence

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
./fake.sh build -t Ci
```

Expected readiness evidence under `specs/010-skia-controls-library/readiness/`:

- `control-catalog.md`
- `public-surface.md`
- `semantic-tests.md`
- `interaction-tests.md`
- `layout-rendering.md`
- `generated-product-usage.md`
- `local-skills.md`
- `dependency-report.md`
- `generated-guidance.md`
- `template-drift.md`
- `evidence-graph.md`
- `evidence-audit.md`
- `compatibility-impact.md`

## 7. Compatibility Scope Check

Review `readiness/compatibility-impact.md`.

Expected outcome:

- Charts capability/package/template/skill removal is documented
- chart and graph replacement path through Controls is documented
- lower-level Scene, SkiaViewer, Layout, and KeyboardInput composition guidance
  is documented
- out-of-scope migration, release automation, renderer backend, designer, rich
  text, and platform-native wrapper work remains excluded
