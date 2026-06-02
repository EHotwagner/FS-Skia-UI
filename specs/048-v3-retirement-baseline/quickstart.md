# Quickstart — V3 Stage 0 baseline, surface baselines & parity oracle

Reproduce every Stage-0 artifact from a clean checkout at the pin SHA. Stage 0 changes **no runtime
code** — `git diff` over `src/**` stays empty (SC-007). FAKE-backed commands share `.fake` state: run
them **sequentially** in the order shown.

## 0. Confirm the pin and escalation

```bash
git rev-parse HEAD                       # expect 031e5607… (or record the branch-point SHA)
./fake.sh build -t Route                 # confirms escalation + required-artifact list
./fake.sh build -t Route --enforce       # fails if an escalated artifact is missing
```

## 1. Reproduce the baseline report (SC-001/002/003)

Each headline metric is reproduced by its recorded command in
`docs/reports/_baselines/2026-06-02-v3-before.md`. Spot-check:

```bash
wc -l src/Lib/Library.fs src/Lib/KeyboardInput.fs src/Lib/AgentValidation.fs \
      src/Lib/VulkanStartup.fs src/Lib/VulkanResources.fs
# leak proof — SkiaViewer → monolith:
grep -n "Lib/Lib.fsproj" src/SkiaViewer/SkiaViewer.fsproj
dotnet list src/SkiaViewer/SkiaViewer.fsproj package --include-transitive   # (or the packed-graph dump named in the report)
```

The report's numbers MUST match the commands' output (SC-001).

## 2. Capture / re-derive the parity oracle (SC-003)

```bash
# Re-derive the scene-output golden from the current host and assert 0-byte diff:
dotnet test tests/Parity.Tests   # the v3-host-golden re-derivation test
ls tests/Parity.Tests/fixtures/v3-host-golden/scene-output/
cat tests/Parity.Tests/fixtures/v3-host-golden/capture-environment.md
```

Scene-output is authoritative; screenshots under `.../screenshots/` corroborate (headless flake).

## 3. Per-package surface baselines + diff (SC-004/005)

```bash
# Zero drift at the pin across the 8 split packages:
./fake.sh build -t PerPackageSurfaceDiff
ls readiness/per-package-surface/        # FS.Skia.UI.Scene.fsi.txt, …SkiaViewer…, … (8 files)

# Seeded-violation demo (one package drifts, others do not):
#   1) make a scratch edit to one public signature, e.g. src/Scene/Scene.fsi
#   2) ./fake.sh build -t PerPackageSurfaceDiff   → reports drift for FS.Skia.UI.Scene ONLY
#   3) git checkout -- src/Scene/Scene.fsi        → revert; re-run → zero drift
```

The existing aggregate `PackageSurfaceCheck` stays green and unchanged (FR-011).

## 4. ADRs (SC-006)

```bash
ls docs/adr/0007-*.md docs/adr/0008-*.md docs/adr/0009-*.md docs/adr/0010-*.md docs/adr/0011-*.md
grep -l "0007\|0008\|0009\|0010\|0011" docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md
```

## 5. Full escalated gate set (SC-008)

```bash
./fake.sh build -t Dev
./fake.sh build -t PerPackageSurfaceDiff
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit          # expect PASS on real, zero-synthetic evidence
```

## 6. Prove no runtime change (SC-007)

```bash
git diff --stat -- src/   # expect EMPTY: monolith, split packages, host, SceneConversion.fs untouched
```
