# No-Regression Evidence — 039 (T024)

Non-authoritative aggregate record of the canonical serialized FAKE no-regression
sequence plus the runtime-untouched and surface invariants (FR-009, FR-010,
FR-012, SC-006). Each FAKE target was run **individually and sequentially**
(never concurrently), at SHA `34faf1ed61ec0ec2a8a2a81168517cb5ccf499d1`,
online, dotnet `10.0.300`, FAKE `6.1.4`.

## Runtime-untouched (authoritative, FR-009)

```bash
git diff --name-only HEAD -- 'src/**'        # (empty)   -> 0 changes
git status --porcelain -- 'src/**'           # (empty)   -> 0 untracked
git diff --name-only HEAD -- 'src/**/*.fsi'  # (empty)   -> 0 .fsi changes
```

**Result: PASS.** No runtime source under `src/**` is edited or added; no tracked
runtime `.fsi` surface changed. Changed top-level paths are build-tooling/docs/
governance only: `build/` (new), `docs/adr/` (new), `docs/reports/_baselines/`
(new), `tests/Governance.Tests/fixtures/` (new), `Directory.Packages.props`
(+Fake.Core.Target), `docs/reports/dependencies.md` (+build-tooling row),
`FS-Skia-UI.sln` (+2 projects, additive), `.specify/feature.json` + `AGENTS.md`
(in-flight feature context), and `specs/039-foundations-baseline-spike/`.

## Serialized FAKE gate sequence

| # | Target | Exit | Result |
|---|--------|-----:|--------|
| 1 | `Dev` | 0 | PASS — full solution compiles, incl. the two new build-tooling projects (`Status: Ok`) |
| 2 | `GeneratedGuidanceCheck` | 0 | PASS |
| 3 | `TemplateCheck` | 1 | **FAIL — pre-existing, not caused by this feature** (see below) |
| 4 | `GeneratedProductCheck` | 0 | PASS |
| 5 | `DependencyReport` | 0 | PASS — new `Fake.Core.Target` central entry + docs row accepted; no `src/**` violation; build-tooling packages not counted as shipped |
| 6 | `TemplateDrift` | 0 | PASS |
| — | `PackageSurfaceCheck` | 0 | PASS — **no runtime surface baseline diff** (SC-006) |
| — | `FsiTranscripts` | 1 | **FAIL — pre-existing, not caused by this feature** (see below) |
| — | `EvidenceGraph` | ok | PASS — 26 tasks, acyclic, 0 errors, no `[S*]` |
| — | `EvidenceAudit` | PASS | verdict=PASS; accepted-seh=0, unaccepted-synthetic=0, auto-synthetic=0, late-seh=0, diff-scan-hits=0, readiness-contract=0 blocking |

## Pre-existing failures (FsiTranscripts, TemplateCheck) — investigated, not this feature's

Both gates fail with the **same** root cause:

```
System.Exception: prelude transcript drift detected for FS.Skia.UI.Lib
Use ./fake.sh build -t FsiTranscripts to regenerate baselines after intended public API changes.
```

(`TemplateCheck` chains the generated-guidance transcript check, so it surfaces
the identical `FS.Skia.UI.Lib` transcript drift.)

**This is environmental / pre-existing, NOT caused by feature 039:**

- Feature 039 edits **no `src/**` runtime source** and does not touch
  `FS.Skia.UI.Lib` or any FSI prelude transcript baseline
  (`git diff --name-only HEAD -- 'src/**'` → 0; the drift is in `FS.Skia.UI.Lib`).
- **Control experiment (decisive):** with **all** of this feature's tracked
  edits stashed (`.specify/feature.json`, `AGENTS.md`,
  `Directory.Packages.props`, `FS-Skia-UI.sln`, `docs/reports/dependencies.md`),
  `./fake.sh build -t FsiTranscripts` **still fails identically** (exit 1, same
  `FS.Skia.UI.Lib` drift). Control log:
  `readiness/logs/fake-FsiTranscripts-stashed-control.txt`. The edits were then
  restored (`git stash pop`).
- The drift is toolchain-driven transcript regeneration (this machine's
  FSharp.Core `10.1.300` / SkiaSharp preview / .NET `10.0.300` produce a
  transcript that differs from the committed baseline). Regenerating the
  baseline is a runtime-surface action **out of scope** for this build-tooling
  feature (FR-009/FR-011: no runtime edits) and is left to the runtime owners.
- The **authoritative** public-surface invariant for this feature —
  `PackageSurfaceCheck` (SC-006) — **passed** with no baseline diff, and no
  runtime `.fsi` changed. So the feature introduces **no** surface regression;
  the FsiTranscripts/TemplateCheck reds are independent of it.

Honest status: the no-regression sequence is **green for everything this feature
can affect**; the two reds are a pre-existing `FS.Skia.UI.Lib` transcript-baseline
drift that predates and is independent of feature 039.

## FAKE-runner cache note (environment, not a product regression)

The first `DependencyReport` invocation failed at the FAKE *runner's own* paket
bootstrap (`NU1101`-style "Folder …/system.runtime.compilerservices.unsafe/6.1.2
doesn't exist. Did you restore group Main?"). This is FAKE-runner infrastructure,
not this feature's change — the gate *logic* (`scripts/dependency-report.fsx`)
ran clean via `dotnet fsi` (exit 0, "PASS: Central Package Management …")
independently. The documented remediation resolved it: restore the
`build.fsx.lock` "Main" group packages into the NuGet cache and remove the stale
`.fake` directory, after which `./fake.sh build -t DependencyReport` (and all
subsequent targets) ran green. No product or build-target behaviour was changed.

## Verdict

**No regression attributable to feature 039.** Runtime untouched (0 `src/**`
changes), public surface unchanged (`PackageSurfaceCheck` PASS, no `.fsi` diff),
the new build-tooling `PackageVersion` reflected without error in
`DependencyReport`, and the evidence graph/audit both PASS. The two red gates
(`FsiTranscripts`, `TemplateCheck`) are a **pre-existing, environment-driven
`FS.Skia.UI.Lib` FSI-transcript-baseline drift**, proven independent of this
feature by the stash control experiment above; resolving that baseline belongs
to the runtime owners and is out of this feature's scope.
