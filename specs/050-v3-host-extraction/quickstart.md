# Quickstart — V3 Stage 1 host extraction & scene-vocabulary unification

Reproduce the keystone move's evidence. **FAKE-backed commands share `.fake` state — run them
sequentially**, never concurrently. The parity diff **gates** deletion of the legacy `Lib` host.

## 0. Confirm escalation and required artifacts

```sh
./fake.sh build -t Route            # confirm dogfood escalation (src/**/*.fsi + consumer-contract)
./fake.sh build -t Route --enforce  # fail if a required evidence artifact is missing
```

## 1. Prove parity — the merge gate (FR-008 / SC-002)

Re-derive the moved host's deterministic scene-output and assert **0-byte** diff vs the committed
Stage-0 golden for all three seeds, BEFORE the legacy `Lib` host source is deleted (ADR 0011):

```sh
# Drives the moved-and-retyped host in src/SkiaViewer/Host and diffs each seed byte-for-byte:
dotnet test tests/Parity.Tests   # basic-viewer / effects-gallery / screenshot-gallery → 0-byte diff
# Golden: tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt
```

Reference-frame corroboration for `basic-viewer` where headless capture is feasible (scene-output
stays authoritative; the libdecor-gtk headless flake is mitigated by a **focused rerun**):

```sh
# If headless capture is infeasible, DISCLOSE it (do not fake) in:
#   specs/050-v3-host-extraction/readiness/parity-reference-frame.md
```

## 2. Close the leak — dependency proof (FR-004 / FR-009 / SC-001 / SC-003)

```sh
# (a) packed SkiaViewer no longer package-depends on the monolith:
#     pack SkiaViewer, then inspect its nuspec/dependency group → FS.Skia.UI ABSENT
# (b) generated default `app` resolves WITHOUT the monolith:
#     generate the default app, dump its transitive graph → FS.Skia.UI ABSENT
# Use the Stage-0 leak-proof reproduction command; record output in:
#   specs/050-v3-host-extraction/readiness/leak-proof.md
```

Sanity grep — there must be **no** `Lib` project reference left on `SkiaViewer`:

```sh
grep -n "Lib.fsproj" src/SkiaViewer/SkiaViewer.fsproj   # → no match (SC-001)
test ! -f src/SkiaViewer/SceneConversion.fs && echo "bridge deleted"   # FR-003
```

## 3. Per-package surface — record the move (FR-011 / SC-007)

```sh
./fake.sh build -t PerPackageSurfaceDiff   # clean against the UPDATED SkiaViewer baseline
# Baseline updated at: readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt
# Net delta must be empty or explicitly justified in:
#   specs/050-v3-host-extraction/readiness/per-package-surface-diff.md
```

The aggregate `PackageSurfaceCheck` must stay green and unweakened (it runs inside the gate set).

## 4. Confirm `Lib` is reduced to residue (SC-004)

```sh
# After the parity diff is clean, the host + duplicate scene modules are deleted from src/Lib.
# Library.fs must no longer contain Viewer/Diagnostics/Colors/Paint/Path/Scene/VulkanHost;
# VulkanStartup.fs(i)/VulkanResources.fs(i) have moved to src/SkiaViewer/Host.
# Lib retains only: AgentValidation, the duplicate KeyboardInput, and the Parity helper.
grep -nE "module (Viewer|Colors|Paint|Path|Scene|Diagnostics|VulkanHost)" src/Lib/Library.fs  # → no host/scene matches
```

## 5. Repointed consumers build green (FR-006 / SC-005)

```sh
# Samples and tests now reference Scene + SkiaViewer (not the deleted Lib modules):
dotnet build samples/BasicViewer samples/EffectsGallery samples/ScreenshotGallery \
             samples/InteractiveViewer samples/DemoReel
dotnet test  tests/Lib.Tests tests/Smoke.Tests tests/Package.Tests
# Reduced-reference consumers keep Lib for residue only:
#   Governance.Tests → AgentValidation (Stage 2); ParityGallery → Parity helper (Stage 4)
```

## 6. Full escalated gate set (SC-008) — sequential FAKE order

```sh
./fake.sh build -t Dev
./fake.sh build -t PerPackageSurfaceDiff   # explicit (no Routing rule selects it — Stage-0 deferral)
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck           # default `app` restores/builds/runs; leak closed
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit           # PASS on real, zero-synthetic evidence
```

If a FAKE failure looks race-like, rerun the affected target sequentially before product debugging.
For the known headless flake, a focused `Parity.Tests`/host rerun is authoritative over the aggregate.
