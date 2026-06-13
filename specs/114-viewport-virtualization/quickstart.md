# Quickstart: Viewport Virtualization (Phase 6)

How to build, test, and validate feature 114 locally. All FAKE-backed commands share
`.fake` state — run them **sequentially** in the order shown.

## 1. Route first

```bash
./fake.sh build -t Route            # prints the authoritative tier + minimal gate list
./fake.sh build -t Route --enforce  # additionally fails if an escalated change lacks evidence
```

`Route` is expected to **escalate to the controls-public-surface tier** because the
`Controls` (`Collections`/`DataGrid`/`Types`) and `Controls.Elmish` (`FrameMetrics`)
`.fsi` surfaces change. Run only the gates it prints.

## 2. Inner-loop build + unit tests

```bash
./fake.sh build -t Dev
```

Targeted test files (run via the test project; reach internal seams through
`InternalsVisibleTo "Controls.Tests"`):

- `tests/Controls.Tests/Feature114OverscanTests.fs` — bounded materialization, non-scaling,
  small-grid transparency, opt-in overscan edge-clamp, keyed reuse on scroll, 113 memo
  composition.
- `tests/Controls.Tests/Feature114OffscreenTests.fs` — offscreen focus/selection targeting,
  window relocation, boundary-crossing navigation, bound preserved.
- `tests/Controls.Tests/Feature114AccessibilityTests.fs` — a11y total + focused position
  from the logical model.
- `tests/Elmish.Tests/Feature114VirtualMetricsTests.fs` — `VirtualItemsMaterialized` /
  `VirtualItemsTotal` goldens over `Perf.runScript` (bounded, non-scaling 100/1000/10000,
  idle 0/0, aggregate).

## 3. Regenerate goldens + baselines (after the `.fsi`/model changes compile)

```bash
# Perf corpus goldens — carry the two new metric fields + the 10000-row assertion
PERF_CORPUS_REGEN=1 ./fake.sh build -t Dev

# Surface baselines — top-level (FrameMetrics fields) + per-package (Collections/DataGrid
# overscan, AccessibilityMetadata total/position)
./fake.sh build -t RefreshSurfaceBaselines
```

The DataGrid corpus goldens live at
`specs/109-perf-metrics-baseline/readiness/perf-corpus/datagrid-{100,1000,10000}.golden.txt`.

> Reminder (from feature 100): adding the `Overscan` field to `CollectionModel` /
> `DataGridModel` and the `Collection` field to `AccessibilityMetadata` forces a defaulted
> value at **every** construction site — including samples (`ControlsGallery`, `DemoReel`)
> and FSI preludes (`scripts/*-prelude.fsx`). Missing sites fail the build /
> `RefreshSurfaceBaselines`.

## 4. Escalated controls-public-surface gate set (sequential)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Plus the per-package/surface diffs, `FsiTranscripts`, and the controls catalog/doc/
interaction/rendering checks as printed by `Route`.

## 5. Verify the contract by hand (FSI sketch)

```fsharp
open FS.Skia.UI.Controls
// overscan-0 == today's slice
Collections.visibleRange 20.0 200.0 0.0 10000 0
// |> { FirstIndex = 0; Count = 11; Total = 10000 }   (byte-identical to pre-feature)

// overscan 5, scrolled into the middle: window relocates, count widens but stays bounded
Collections.visibleRange 20.0 200.0 2000.0 10000 5
// |> FirstIndex shifted back by 5, Count <= 11 + 2*5, Total = 10000
```

## Evidence checklist (escalated maintainer-verify set)

- [ ] bounded-materialization over 100/1000/10000 corpus (`<= visible + 2*overscan`, total
      scales) — `Feature114VirtualMetricsTests`
- [ ] default-overscan byte-identity vs pre-feature baseline — standing Scene-parity suite
- [ ] opt-in overscan correctness (real, edge-clamped, unshifted visible) —
      `Feature114OverscanTests`
- [ ] offscreen focus/selection addressability + boundary-crossing navigation —
      `Feature114OffscreenTests`
- [ ] a11y total + position reporting — `Feature114AccessibilityTests`
- [ ] `VirtualItemsMaterialized`/`VirtualItemsTotal` metric evidence (steady vs no-control
      frame) — `Feature114VirtualMetricsTests`
- [ ] regenerated `Perf.runScript` corpus goldens carrying the two new fields
- [ ] regenerated surface + per-package baselines
- [ ] `readiness/skill-loading-evidence.md`
- [ ] window-visibility not-applicable set
- [ ] `readiness/evidence-audit.md` with a verdict token
- [ ] generated-validation package-resolution tokens
