# Serialized Escalated Gate Set (T025) — all six green

This `build.fsx`/launcher/governance-path change escalates to the full six-target set. All runs
go through the **compiled front-end** (`./fake.sh` = `dotnet run --project build/Build.fsproj`).
Captured: 2026-06-01T19:51:33Z

| # | Gate | Result | Notes |
|---|------|--------|-------|
| 1 | `Dev` | ✅ PASS | EXIT 0, ~1m38s. Full 11-project test matrix green: Lib 42, Scene 11, SkiaViewer 48, Elmish 4, KeyboardInput 7, Layout 23, Controls 36, Testing 38, Parity 5, Smoke (direct Expecto), Governance 310. |
| 2 | `GeneratedGuidanceCheck` | ✅ PASS | Relocated `Guidance` validator green through the front-end. |
| 3 | `TemplateCheck` | ✅ PASS | EXIT 0, ~2m02s. Packs/installs the template, instantiates 8 profiles, and runs each generated project's `Dev` — all 8 (source/package × app/headless-scene/governed/sample-pack) PASS. |
| 4 | `GeneratedProductCheck` | ✅ PASS | ~1m44s. GenerateV3Products + ScanV3GeneratedProducts + ValidateGeneratedConsumer all succeed. |
| 5 | `EvidenceGraph` | ✅ PASS | DAG acyclic, skillist + skill-loading evidence valid, no `[S*]`. |
| 6 | `EvidenceAudit` | ✅ PASS | `verdict=PASS`; 0 synthetic, 0 diff-scan, 0 readiness-contract; total-blockers=0. |

## Root cause found & fixed (was first mis-attributed to a headless flake)

`Dev` and `TemplateCheck` (the latter via `TemplateCheck → TemplateSmoke → Test`) initially hung
in the framework `Test` aggregate. The cause was a **feature-045 regression**: deleting `build.fsx`
broke five test files that discovered the repository root by walking up for a `build.fsx` marker.
With the marker gone the walk reached the filesystem root where `Directory.GetParent` returns null,
and `Option.defaultValue dir |> find` recursed on the same directory → **infinite loop**. Because
those are module-level `let repositoryRoot` bindings, the loop ran during Expecto test discovery,
hanging the test process at startup with zero output.

Fixed by switching the marker to `FS-Skia-UI.sln` (which exists) and adding a filesystem-root
termination guard (`None -> dir`) in:
- `tests/Elmish.Tests/ControlsElmishAdapterContractTests.fs`
- `tests/Controls.Tests/{CatalogTests,PublicSurfaceTests,ControlRuntimeContractTests,TypedControlContractTests}.fs`

`tests/Package.Tests/Tests.fs` additionally read `build.fsx` *content*; those reads were redirected
to the relocated `build/Governance` sources. After the fix, all individual suites and the full
aggregate `Dev`/`TemplateCheck` gates pass.
