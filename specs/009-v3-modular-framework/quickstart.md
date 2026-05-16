# Quickstart: V3 Modular Framework

This quickstart describes the validation workflow expected after implementation
of the V3 modular framework plan.

## 1. Validate Framework Development Workflow

```bash
./fake.sh build -t Dev
./fake.sh build -t CapabilityCheck
./fake.sh build -t SkillCheck
./fake.sh build -t DependencyReport
```

Expected outcome:

- framework projects restore, build, and run default tests
- every capability has complete catalog metadata
- every selected capability has a valid local agent skill
- dependency report shows Scene has no Elmish, Silk.NET, SkiaSharp, Yoga.Net,
  or YamlDotNet dependency

## 2. Validate Package Surfaces

```bash
./fake.sh build -t PackLocal
./fake.sh build -t PackageSurfaceCheck
```

Expected outcome:

- public capability packages pack successfully
- package-specific baselines exist for public packages
- no accidental exports appear outside `.fsi` contracts

## 3. Validate Generated Products

```bash
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
```

Expected outcome:

- source and packaged template paths generate app, headless, governed, and
  sample-pack profiles
- default app includes Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and
  Charts references
- default app contains exactly one product app and one product test suite
- default app contains full product governance
- default app contains no framework samples, galleries, historical specs,
  readiness evidence, framework docs, framework README copy, or framework
  implementation projects

## 4. Validate Generated Product Commands

From each generated validation root:

```bash
./fake.sh build -t Dev
./fake.sh build -t Test
./fake.sh build -t Verify
```

Expected outcome:

- generated product `Verify` runs product governance, evidence gates, drift
  checks, generated guidance checks, and readiness workflow
- generated product `Verify` does not run framework galleries, parity suite,
  framework package-surface maintenance, or framework template packaging checks

## 5. Validate Governance Evidence

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
./fake.sh build -t Ci
```

Expected readiness evidence under `specs/009-v3-modular-framework/readiness/`:

- `capability-catalog.md`
- `generated-file-lists/`
- `generated-product-verify/`
- `selected-skills.md`
- `package-surfaces/`
- `dependency-report.md`
- `generated-guidance.md`
- `template-drift.md`
- `task-graph.md`
- `logs/evidence-audit.txt`
- `diff-scan-hits.json`
- `compatibility-impact.md`

## 6. Compatibility Scope Check

Review `readiness/compatibility-impact.md`.

Expected outcome:

- package and generated product compatibility impact is documented
- V2 migration support is explicitly out of scope
- no V2 migration implementation tasks are included in this feature
