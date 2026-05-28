# Quickstart: Upgrade SkiaSharp And Spec Kit

## 1. Capture Baseline

```bash
./fake.sh build -t DependencyReport
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
```

Record baseline status under:

```text
specs/025-upgrade-skia-speckit/readiness/
```

## 2. Confirm Target Versions

Immediately before editing package or Spec Kit files, re-check official version
sources for:

- `SkiaSharp`
- `SkiaSharp.NativeAssets.Linux`
- `SkiaSharp.NativeAssets.Win32`
- Spec Kit release/version metadata

Record selected versions, sources, timestamps, affected files, and risks in:

```text
specs/025-upgrade-skia-speckit/readiness/version-selection.md
```

## 3. Add Failing-First Governance Coverage

Add or update tests before implementation for:

- SkiaSharp package-family alignment
- Spec Kit root and generated asset alignment
- generated template package pin alignment
- broad `FS.Skia.UI` dependency detection in generated profiles
- compatibility consumer inventory coverage
- package-surface baseline status

## 4. Apply Version And Asset Updates

Update central package declarations, generated template package pins, Spec Kit
metadata/assets, generated docs, and template package metadata as required by
the version-selection evidence.

Do not remove compatibility APIs during this step. If a public surface
difference appears, pause and follow:

```text
.fsi signature -> semantic/FSI evidence -> implementation -> surface baseline -> docs/release notes
```

## 5. Produce Compatibility Evidence

Create:

```text
specs/025-upgrade-skia-speckit/readiness/compatibility-consumer-inventory.md
specs/025-upgrade-skia-speckit/readiness/compatibility-public-surface-map.md
specs/025-upgrade-skia-speckit/readiness/compatibility-sample-migration.md
specs/025-upgrade-skia-speckit/readiness/compatibility-release-policy.md
specs/025-upgrade-skia-speckit/readiness/package-surface-baseline.md
```

## 6. Validate Template And Dependency Results

```bash
./fake.sh build -t DependencyReport
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
```

Record refreshed dependency and template evidence in:

```text
specs/025-upgrade-skia-speckit/readiness/dependency-report.md
specs/025-upgrade-skia-speckit/readiness/template-version-alignment.md
```

## 7. Final Governance Checks

```bash
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

Acceptance requires all readiness artifacts, no accidental public compatibility
surface changes, no accidental broad generated dependency, and no unresolved
synthetic or diff-scan blockers.
