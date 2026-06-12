# Quickstart: Narrow Runtime Visual-State Updates (feature 112)

How to exercise and validate the feature.

## Run the new tests

```bash
# Targeted-vs-oracle scene parity, touched-node count, precedence (Controls.Tests)
dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "112"
```

What they assert:
- `Feature112TargetedStampParityTests` — the targeted stamp's rendered scene + resolved
  per-control visual states equal the preserved full-tree oracle's, for hover-move /
  focus-move / press-toggle over keyed / nested / unkeyed-sibling / consumer-set trees
  (FR-005/SC-002).
- `Feature112TouchedCountTests` — `RuntimeStateTouchedNodeCount` « node count for a
  localized hover/focus/press change, equals the affected identities + ancestor paths,
  and is `0` for a no-change frame (FR-001/FR-004/FR-007/SC-001/SC-003/SC-006).
- `Feature112PrecedenceTests` — a consumer-set `Disabled`/`Selected` control keeps its
  state under targeting; a derived hover/focus does not override it (FR-003/SC-004).

## Observe the touched-node delta (FSI / test)

```fsharp
open FS.Skia.UI.Controls
// build a tree, a prev model (hover on A), a cur model (hover on B), stamp both ways:
let r = ControlRuntime.applyRuntimeVisualStateTargeted prevModel curModel prevStamped fresh
printfn "touched=%d of %d nodes" r.RuntimeStateTouchedNodeCount (Control.count fresh)
// r.Stamped renders byte-identically to ControlRuntime.applyRuntimeVisualState curModel fresh
```

## Run the escalated gate set (controls-public-surface)

```bash
# regenerate the surface + per-package baselines for the new internal ControlRuntime seam
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

Note: the public Controls surface baseline (type-level) does not change (the seam is
`internal`); the **per-package** Controls surface gains the internal type + val
(regenerated). At-rest rendered-output byte-identity is the standing Scene-parity golden
suite run under `Dev` (no scene/geometry golden delta).
