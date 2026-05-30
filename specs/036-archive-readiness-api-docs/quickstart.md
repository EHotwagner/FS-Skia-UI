# Quickstart: Archive Readiness And API Docs

## 1. Build The Planning Context

Review the active plan and contracts:

```bash
sed -n '1,220p' specs/036-archive-readiness-api-docs/plan.md
find specs/036-archive-readiness-api-docs/contracts -type f | sort
```

## 2. Add Failing-First Tests

Add or extend Expecto tests for:

- archive inventory required fields and current-vs-archived labels
- stale-reference scanner blocking active surfaces only
- generated guidance rejecting archived evidence as current gate evidence
- API-reference generator decision dimensions and required package samples
- fsdocs spike blocker or sample output fields

Likely test areas:

```text
tests/Governance.Tests/
tests/Package.Tests/
```

## 3. Produce Archive And Evidence Maps

Create the readiness reports:

```text
specs/036-archive-readiness-api-docs/readiness/archive-inventory.md
specs/036-archive-readiness-api-docs/readiness/current-evidence-map.md
specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.md
```

The inventory should keep historical files in place by default and mark them
as archived, retained, roadmap/deferred, replaced, or removable.

## 4. Compare API Reference Generators

Use the current source-shaped reference as the baseline:

```bash
dotnet fsi scripts/generate-package-api-reference.fsx
```

Evaluate fsdocs only as a spike unless dependency governance accepts it:

```bash
dotnet tool install fsdocs-tool --tool-path ./.tools/fsdocs
./.tools/fsdocs/fsdocs build --projects src/Scene/Scene.fsproj src/Controls/Controls.fsproj src/SkiaViewer/SkiaViewer.fsproj --output artifacts/fsdocs-spike/036-archive-readiness-api-docs --strict
```

If local tool installation or project cracking fails, record the command, log
path, blocker, and next action in
`readiness/fsharp-formatting-spike.md`.

## 5. Update Guidance

Update active docs and template guidance so generated consumers know:

- current feature readiness paths are authoritative for current gates
- archived readiness is historical audit context
- source-shaped `.fsi` package API reference remains the authoritative agent
  reference
- fsdocs output is secondary/hybrid unless the decision record proves
  otherwise

## 6. Verify Sequentially

FAKE-backed commands share `.fake` state. Run them sequentially:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Add these only if package/template outputs change:

```bash
./fake.sh build -t PackLocal
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t FsiTranscripts
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
```

Record command order and logs under
`specs/036-archive-readiness-api-docs/readiness/`.
