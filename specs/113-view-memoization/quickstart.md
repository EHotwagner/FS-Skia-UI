# Quickstart: View Memoization and Stable Dependency Contracts

## What this feature adds

- An **internal** control-internal memoization seam (`FS.Skia.UI.Controls`) keyed by
  `ControlId` + a caller-supplied dependency value, reusing the prior lowered subtree on
  a dependency match. Applied to the **DataGrid row/column projection**.
- Two new **public** `FrameMetrics` fields — `MemoHitCount` / `MemoMissCount` —
  deterministic and golden-asserted via `ControlsElmish.Perf.runScript`.
- A **public** stability-diagnostic report (`Controls` `Diagnostics`) flagging always-new
  attributes/events that defeat reuse (report-only, not a gate).
- An author-facing **stable-props guidance page** at `docs/controls/stable-props.md`.

No public `Control.memo` / `Widget.memo` primitive (deferred).

## Run the tests

```bash
# Memo seam (hit/miss/cold, reference-reuse) + memo-on/memo-off parity + no-staleness
dotnet test tests/Controls.Tests --filter Feature113

# MemoHitCount / MemoMissCount goldens over the deterministic Perf.runScript corpus
dotnet test tests/Elmish.Tests --filter Feature113

# Stability-diagnostic (stable tree → no findings; injected always-new input → flagged)
dotnet test tests/Controls.Tests --filter Feature113StabilityDiag
```

## Regenerate goldens & baselines (after the .fsi additions)

```bash
# Regenerate the perf corpus goldens so they carry the two new metric fields, then commit
PERF_CORPUS_REGEN=1 dotnet test tests/Elmish.Tests --filter Feature109CorpusTests

# Regenerate the top-level + per-package surface baselines (FrameMetrics fields,
# Diagnostics val, internal memo seam)
./fake.sh build -t RefreshSurfaceBaselines
```

## Observe a memo hit

Render the same model twice through `Perf.runScript` for a scenario with a memoizable
DataGrid whose data + theme are unchanged across the two frames: the second frame records
`MemoHitCount > 0` and `MemoMissCount = 0` for that site, and the rendered scene is
byte-identical to the always-miss (memo-off) build.

## Validate (escalated controls-public-surface set)

`Route` escalates because the Controls + Controls.Elmish `.fsi` surfaces change. Run
`Route` first and obey its printed minimal list. FAKE-backed commands run sequentially in
the deterministic order:

```bash
./fake.sh build -t Route          # prints the authoritative tier + minimal gate list
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```
