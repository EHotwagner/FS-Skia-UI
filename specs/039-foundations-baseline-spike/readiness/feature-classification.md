# Feature Classification & Evidence Obligations — 039 (T003)

## Change classification

- **Tier**: **Tier 1.** The feature introduces two new projects
  (`build/Build.fsproj`, `build/Governance/FS.Skia.UI.Build.fsproj`), a new
  inter-project contract (front-end → governance-library project reference),
  and a new library identity (`FS.Skia.UI.Build`) that will later be packaged.
  Tier 1 obligations apply, **scoped to the new build-tooling projects only**.

## Affected layer

- **Build-tooling projects only** under a new top-level `build/**` root.
- **No runtime under `src/**`.** None of `src/Scene`, `src/SkiaViewer`,
  `src/Elmish`, `src/KeyboardInput`, `src/Layout`, `src/Controls`,
  `src/Controls.Elmish`, `src/Lib` is edited.
- Additive solution/central-package/dependency-doc wiring only.

## Public-API impact

- **No tracked runtime surface diff.** The eight runtime packages and their
  surface baselines (`PackageSurfaceCheck`, `FsiTranscripts`) must show **no
  diff** (SC-006).
- **One new build-tooling `.fsi`** (`build/Governance/Spike.fsi`, a single
  `val run : unit -> string`). This is a *new* build-tooling surface, **not**
  part of the tracked runtime surface baselines (Principle II still applies:
  visibility lives in the `.fsi`).

## Principle IV (MVU/effect boundary)

- **Not Applicable** to every task in this feature. No stateful or I/O-bearing
  runtime workflow is added; the spike target is a trivial pure/console action.
  See [`effects-boundary.md`](./effects-boundary.md).

## Synthetic evidence

- **None anticipated.** Baseline counts are real `wc`/`git` measurements at a
  real commit; golden fixtures are real outputs of the existing evidence
  engine; the spike runs a real compiled target. No `[S]`/`[SEH]` tasks are
  planned. If the spike fails, the recorded blocker is a real, reproducible
  observation — not synthetic.

## Evidence obligations (from the plan)

1. Baseline → `docs/reports/_baselines/2026-05-31-foundations.md` (SHA-pinned).
2. Golden fixtures → `tests/Governance.Tests/fixtures/evidence-golden/<feature>/`.
3. Spike outcome → `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`.
4. ADRs → `docs/adr/0001..0005-*.md`.
5. Meta-process → `plan.md` §Programme Meta-Process (linked from baseline).
6. No-regression → canonical serialized FAKE sequence captured to
   `readiness/logs/` + surface gates with no baseline diff.
