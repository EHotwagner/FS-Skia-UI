# Build Wall-Clock (T023 / FR-014 / SC-007) — recorded, NOT a merge gate

Measured through the compiled front-end (`./fake.sh build -t <t>` = `dotnet run --project
build/Build.fsproj -- ...`) on this Linux host. Per FR-014/SC-007 this is a recorded observation,
not a gate: parity (behaviour) is the gate; a non-improvement does not block the feature.

## Compiled front-end, observed per-target (from dev-gate run)
| Target | Duration |
|---|---|
| SkillSyncCheck | ~0.05 s |
| Restore | ~34 s |
| Build | ~42 s |
| SampleContractSmoke | ~10 s |
| Route (no deps) | ~0.09 s (warm exe) |
| Test (full 11-project matrix, --sequenced) | several minutes (dominates Dev wall-clock) |

## Notes
- Cold cost now includes compiling `build/Build.fsproj` + `FS.Skia.UI.Build` once (`dotnet run`),
  versus the prior `dotnet fake` FSX-recompile of build.fsx. Warm `dotnet run` reuses the built exe.
- The `Dev` wall-clock is dominated by the sequential `Test` matrix (unchanged behaviour — the
  relocation did not alter which tests run), not by the front-end itself; the per-target governance
  work (Route, SkillSyncCheck, etc.) is sub-second to seconds.
- No regression attributable to the relocation: the governance/validation work runs in-process in
  the compiled library exactly as before (verbatim relocation).

Captured: 2026-06-01T14:51:58Z
