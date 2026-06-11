# Surface baseline — zero-drift confirmation (feature 101, R7, T006/T015)

authoritative-command=git diff --stat -- '*.fsi'
artifact-path=src/Controls/*.fsi
status=pass
failure-class=unexpected-surface-drift
next-action=if any .fsi moved, recapture via PerPackageSurface.captureCurrent and note the exact symbol delta

## SC-005 — no public/internal `.fsi` signature change

R7 was designed for **zero** surface delta: the drift report, the formatter, and the behavioral probe
are **test-local** in `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs`, and the US2 name
tokens are `[<Literal>] private` inside the internal `ControlInternals` module. The category-honoring
units assert through the already-exposed `RetainedRender.step` rather than exposing
`layoutDirtySet` (an internal `.fsi` edit there was rejected precisely to hold this baseline — see
`category-honoring.md`).

## Evidence

- pre-change reference (T006): the committed `FS.Skia.UI.Controls` per-package internal `.fsi.txt`
  baseline and the public api-surface baseline, captured before any edit.
- post-change confirmation (T015): `git diff --stat -- '*.fsi'` over the full working tree returns
  **empty** — no `.fsi` file changed anywhere. The complete tracked diff is exactly:
  - `src/Controls/Control.fs` (name-token constants + comment correction; `.fs` only)
  - `src/Controls/RetainedRender.fs` (comment correction; `.fs` only)
  - `tests/Controls.Tests/Controls.Tests.fsproj` (+1 `<Compile Include>`)
  - `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs` (new test file)
  - `.specify/feature.json`, `AGENTS.md` (feature-pointer/doc; pre-existing feature setup)
- per-package internal baseline: because no `.fsi` moved, the `FS.Skia.UI.Controls` internal
  `.fsi.txt` capture is **unchanged** vs the reference (no symbol added, removed, or reordered);
  `PerPackageSurfaceDiff` was **not** in the Route-printed gate set for this diff (it appears only on a
  per-package baseline move), consistent with zero internal-surface delta.

surface-delta=none
public-surface-delta=none
internal-fsi-delta=none
