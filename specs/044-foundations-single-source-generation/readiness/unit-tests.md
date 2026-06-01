# Governance.Tests — typed results for the new generation/currency modules (T027)

All tests are **failing-first** by construction: each new test file references a
`FS.Skia.UI.Build` module that did not exist before this feature, so the project did not
compile (red) until the corresponding `.fs` was implemented (green). Each currency function
additionally has a stale-fixture case asserting a `Some`/non-empty diagnostic that passes
only once the implementation is correct.

## Focused — the three new modules + the reframed adapter

```
$ dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -- --filter-test-list "Feature 044"
EXPECTO! 23 tests run – 23 passed, 0 ignored, 0 failed, 0 errored. Success!
```

Test lists:
- **Feature 044 SkillTreeGen generation + currency** (`SkillTreeGenTests.fs`) — enumeration
  covers a synthetic 26th non-allowlisted skill (SC-002); the derived plan reproduces
  canonical bytes (SC-001); a tampered derived byte yields a `Some` currency diagnostic
  naming `RefreshSurfaceBaselines`; missing/orphan reported; empty + null-bytes canonical
  input each raise (Principle VII).
- **Feature 044 SkillSync currency adapter** (`SkillSyncTests.fs`, re-pointed) — faithful
  regeneration is current (PASS report); a drifted tree fails with a regen-naming message.
- **Feature 044 SkillistView render + splice + currency** (`SkillistViewTests.fs`) —
  `renderAnnotation` empty/non-empty; `spliceAnnotation` changes only the bracket token and
  raises when absent; `currency` flags stale/absent and passes current; active-feature scope.
- **Feature 044 ConstitutionFragments extract + splice + currency** (`ConstitutionFragmentsTests.fs`)
  — deterministic extraction (raises on a missing `### Principle` heading); `regions`
  locates marker pairs; `splice` preserves every out-of-marker byte and is idempotent;
  `currency` flags a stale region after a simulated principle edit and reports
  missing/unknown markers.

## Full suite — no regression from the SkillExamplesCheck retirement

```
$ dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj
EXPECTO! 326 tests run – 326 passed, 0 ignored, 0 failed, 0 errored. Success!
```

The full run includes the `workflow self-check` test, which compiles `build.fsx` fresh —
so it confirms the reframed `SkillSyncCheck` arm, the two new regeneration effects, the
`TargetMetadataDrift` constitution-fragment fold, and the `SkillExamplesCheck` retirement
all compile and wire coherently. `CommandContractTests` (Dev prereqs), `ReportParityTests`
+ `TargetMetadataTests` (target-metadata golden, now 37 rows), and `SkillSyncTests`
(re-pointed) all pass.

**Verdict: PASS** — 23/23 focused, 326/326 full; zero synthetic evidence.
