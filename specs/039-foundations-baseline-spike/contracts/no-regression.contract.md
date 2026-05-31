# Contract: No Regression (runtime, surface, and existing targets unchanged)

This contract encodes the feature's core safety promise: the foundations work changed **nothing** in the runtime, the public contract, or any existing build target. (FR-009, FR-010, FR-011; SC-006.)

## Invariants under contract

| # | Invariant | Verification | Maps to |
|---|---|---|---|
| 1 | No runtime source edited | `git diff --name-only` touches nothing under `src/Scene`, `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Controls`, `src/Controls.Elmish`, `src/Lib` | FR-009 |
| 2 | No public surface diff | `PackageSurfaceCheck` and `FsiTranscripts` pass with **no baseline diff** | FR-009, SC-006 |
| 3 | Existing targets unchanged in behaviour/output | `Dev`, `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` produce identical behaviour/output to baseline | FR-010 |
| 4 | net10 / CPM conventions held | new projects inherit `Directory.Build.props`; every new package version lives in `Directory.Packages.props`; **no** `FSharp.Compiler.Service` anywhere | FR-012 |
| 5 | Evidence engine not ported/moved | no validator moved out of `build.fsx`; no Python ported; no `Route`/two-tier/single-source-generation introduced | FR-011 |
| 6 | Solution additions are additive | adding `build/Build.fsproj` + `build/Governance/FS.Skia.UI.Build.fsproj` to `FS-Skia-UI.sln` changes no existing target's output | FR-010 |

## Verification sequence (canonical serialized FAKE order — never concurrent)

Run, in order, capturing logs to `readiness/logs/`:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `PackageSurfaceCheck` / `FsiTranscripts` for invariant 2.

**Pass condition (SC-006)**: the full serialized sequence is green **and** the public surface shows no baseline diff. Any red gate or any surface diff is a contract failure that blocks the feature.

> The spike's own `dotnet run --project build/Build.fsproj` is **not** FAKE-backed and does not touch `.fake` state; it is still run separately from the FAKE sequence to honour the never-concurrent-FAKE rule.
