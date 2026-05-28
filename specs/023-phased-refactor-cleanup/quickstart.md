# Quickstart: Phased Refactor Cleanup

## Baseline

Before editing any phase, record the current branch, git status, selected checks,
and any pre-existing failures in:

```text
specs/023-phased-refactor-cleanup/readiness/baseline-status.md
```

Suggested initial baseline commands:

```bash
git status --short
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
dotnet test tests/Scene.Tests/Scene.Tests.fsproj
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
./fake.sh build -t TemplateCheck
```

## Phase 1: Generated Evidence Cleanup

Goal: consolidate generated evidence/report writing behavior without moving
generated files yet.

Checks:

```bash
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
```

Record results in:

```text
specs/023-phased-refactor-cleanup/readiness/generated-evidence-cleanup.md
```

## Phase 2: Generated Template Split

Goal: split generated product responsibilities into separate files while
keeping generated commands, profiles, fields, and behavior stable.

Checks:

```bash
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
```

Record results in:

```text
specs/023-phased-refactor-cleanup/readiness/template-split-validation.md
```

## Phase 3: Build Governance Decomposition

Goal: move build helper responsibilities into loaded scripts while preserving
the FAKE command surface.

Run the focused target for each moved helper family, then run broader checks if
target wiring or readiness paths changed:

```bash
./fake.sh build -t Dev
./fake.sh build -t Verify
./fake.sh build -t EvidenceGraph
```

Record results in:

```text
specs/023-phased-refactor-cleanup/readiness/build-governance-decomposition.md
```

## Phase 4: Viewer Internal Boundary

Goal: split viewer diagnostics, host classification, visual evidence, screenshot
evidence, and window behavior behind the unchanged public facade.

Checks:

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
./fake.sh build -t EvidenceAudit
```

Record results in:

```text
specs/023-phased-refactor-cleanup/readiness/viewer-internal-boundary.md
```

## Acceptance

The feature is ready for task completion only when:

- public surface baselines remain unchanged,
- generated profiles instantiate, build, and pass validation,
- generated evidence commands emit the same required fields and statuses,
- FAKE target names and readiness paths are unchanged,
- unsupported screenshot hosts still produce explicit unsupported evidence,
- every phase readiness file records baseline status, checks run, and verdict.
