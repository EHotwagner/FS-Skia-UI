# Quickstart: Frame Scheduler & Phase-Invalidation Model (feature 111)

How to exercise and validate the feature.

## Run the new tests

```bash
# US1 cause classification, US2 phase record, US3 view-skip byte-identity
dotnet run --project tests/Elmish.Tests -c Debug -- --filter-test-list "111"

# The updated feature-109 honesty + corpus tests (animation-tick ViewCalled flip)
dotnet run --project tests/Elmish.Tests -c Debug -- --filter-test-list "109"
```

What they assert:
- `Feature111FrameCauseTests` — every produced frame's `FrameCause` matches its
  trigger (idle/move/discrete/key/tick), byte-stable across repeated runs (FR-001/
  SC-001).
- `Feature111PhaseRecordTests` — the four phase bools per frame: idle = all false;
  animation-only tick = `ViewCalled false`, `PaintRan true`; model frame =
  view+diff+layout+paint (FR-002/SC-002/SC-004).
- `Feature111ViewSkipTests` — an animation-only tick and a model-unchanged frame
  perform **no** `host.View` (`ViewCalled = false`, `FullRenderCount = 0`) while the
  rendered scene is byte-identical to the pre-feature output (FR-003/FR-004/SC-003/
  SC-007).

## Regenerate the corpus goldens (after the field + view-skip land)

```bash
PERF_CORPUS_REGEN=1 dotnet run --project tests/Elmish.Tests -c Debug -- --filter-test-list "corpus"
```

Expected delta (record in `readiness/`):
- Every golden line gains `FrameCause=...` + `DiffRan=... LayoutRan=... PaintRan=...`.
- `text-entry-while-animating` tick frames: `ViewCalled true → false`,
  `FullRenderCount 1 → 0`, `PaintRan = true` (view-free animation ticks).
- Model/key frames keep `ViewCalled = true`; no rendered-scene/geometry delta.

## Run the escalated gate set (controls-public-surface)

```bash
# regenerate the surface + per-package baselines for the new FrameCause type + fields
./fake.sh build -t RefreshSurfaceBaselines

# then Route, and run only what it prints (sequentially — shared .fake state):
./fake.sh build -t Route
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Note: unlike feature 110, the **top-level** surface baseline
`readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` changes (new public
`FrameCause` type + cases), in addition to the per-package surface.

## Exercise from FSI

```fsharp
#r "src/Controls.Elmish/bin/Debug/net10.0/FS.Skia.UI.Controls.Elmish.dll"  // + deps
open FS.Skia.UI.Controls.Elmish
// A move and a tick frame: read FrameCause + the phase bools.
ControlsElmish.Perf.runScript host size [ FrameInput.Pointer(HoverEnter("btn", 5.0, 5.0)) ]
|> List.iter (fun f -> printfn "cause=%A view=%b diff=%b layout=%b paint=%b" f.FrameCause f.ViewCalled f.DiffRan f.LayoutRan f.PaintRan)
```
(Layout-bearing frames need the test host where the Yoga native is loaded; move/idle
frames run in plain FSI.)
