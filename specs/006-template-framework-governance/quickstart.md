# Quickstart: Template Framework Governance

This quickstart defines the validation path for v1 after implementation.

## 1. Restore Local Tools

```bash
dotnet tool restore
```

Expected result: the repo-local FAKE tool restores successfully from `.config/dotnet-tools.json`.

## 2. Run Fast Local Verification

```bash
./fake.sh build -t Dev
```

Expected result:

- Solution restore completes.
- Solution build completes.
- Default non-visual test set passes.
- The command exits successfully in 10 minutes or less on a supported development machine.

## 3. Run Full V1 Verification

```bash
./fake.sh build -t Verify
```

Expected result:

- `Dev` succeeds.
- Package surface checks read `readiness/surface-baselines/*.txt`.
- Required FSI transcripts are captured under `specs/006-template-framework-governance/readiness/fsi/`.
- Required sample smoke output is captured under `specs/006-template-framework-governance/readiness/sample-smoke/`.
- Task graph output is produced under `specs/006-template-framework-governance/readiness/`.
- Evidence audit output is produced under `specs/006-template-framework-governance/readiness/`.
- Missing required artifact classes fail with actionable output.

## 4. Produce Local Packages

```bash
./fake.sh build -t PackLocal
```

Expected result: packable projects produce `.nupkg` files under `~/.local/share/nuget-local/`.

Package consumer smoke is intentionally not required by v1. It belongs to the deferred package validation roadmap.

## 5. Refresh Stable Surface Baselines

```bash
./fake.sh build -t RefreshSurfaceBaselines
./fake.sh build -t PackageSurfaceCheck
```

Expected result:

- Refresh writes `readiness/surface-baselines/*.txt`.
- Surface checks read the same stable baseline files.
- Neither target requires editing `specs/002-skia-feature-parity/readiness/surface-baselines/`.
- Stale or missing public contract names fail the target.

## 6. Validate Automation And Guidance

Inspect touched automation and task guidance:

- `.specify/workflows/speckit/workflow.yml`, if changed
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md`
- `.agents/skills/speckit-tasks/SKILL.md`, if changed
- `docs/build.md`
- `docs/evidence.md`
- `docs/testing.md`

Expected result: these references call canonical targets such as `Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `EvidenceGraph`, and `EvidenceAudit` instead of duplicating command order.
