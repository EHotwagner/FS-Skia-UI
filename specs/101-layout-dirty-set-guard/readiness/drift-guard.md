# Drift guard — US1 evidence (feature 101, R7, T010)

authoritative-command=dotnet run --project tests/Controls.Tests -c Debug -- --filter-test-list "Feature101"
artifact-path=tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs
status=pass
failure-class=layout-dirty-set-drift
next-action=if RED, read the named attribute + direction and either add the name to layoutAffectingAttrNames (under-coverage) or remove it (over-coverage)

## The guard (SC-001 / FR-001 / FR-002 / FR-003 / FR-007)

R7 converts the correct-but-unguarded R2 invariant — that the incremental dirty classifier's covered
name set (`ControlInternals.layoutAffectingAttrNames`) equals the names `toLayout` actually reads — into
an **enforced** one, via two parts that are exercised against the real dependency:

1. **Pure drift report** `layoutDriftReport (discovered) (covered) : DriftFinding list`
   (`DriftFinding = Uncovered of string | OverBroad of string`) — exact set-difference both directions,
   order-stable; `formatDrift` names each attribute AND its direction; empty → an explicit "no drift"
   string.
2. **Behavioral probe** `discoverLayoutDrivingNames size` — toggles each corpus attribute name on
   representative fixtures and observes whether the REAL `ControlInternals.evaluateLayout` root
   `LayoutNode` changes (structural `%A` comparison; `LayoutNode` has a `Measure` function field so it
   lacks `=`). The discovered set is the union over (corpus × fixtures).

The **load-bearing gate**:
`layoutDriftReport (discoverLayoutDrivingNames size) ControlInternals.layoutAffectingAttrNames = []`.

## Negative directions named by `formatDrift` (failing-first, both directions)

- under-coverage: `layoutDriftReport {width;height;padding} {width;height}` → `[Uncovered "padding"]`;
  `formatDrift` emits `un-covered layout input: 'padding' (toLayout reads it but the classifier does
  not dirty on it)`.
- over-coverage: `layoutDriftReport {width} {width;orientation}` → `[OverBroad "orientation"]`;
  `formatDrift` emits `over-broad classifier entry: 'orientation' (the classifier lists it but toLayout
  never reads it)`.
- both directions, sorted: `layoutDriftReport {a;b} {b;c}` → `[Uncovered "a"; OverBroad "c"]`.

## Positive gate passes today; the RED it produces on real drift

- discovered = `{width; height; orientation}` = `layoutAffectingAttrNames` = covered → report `[]`
  (asserted directly, plus `discovered = {width;height;orientation}` asserted explicitly, plus the
  non-layout names `background`/`foreground`/`text`/`value`/`selected`/`padding`/`margin` asserted NOT
  discovered).
- **Demonstrated RED** (red→green, then reverted): temporarily making `toLayout` read a `padding`
  attribute (`Padding = floatValue "padding" …`) makes the probe discover `padding`; the gate then
  FAILS with `actual: [Uncovered "padding"]` and the message `un-covered layout input: 'padding' …` —
  i.e. the exact stale-cached-bounds bug class is caught the instant `toLayout` reads an un-covered
  corpus name, as a fast explicit named Expecto failure under `Dev`, not a silent wrong-layout symptom.
  The drift was reverted and the suite is GREEN (12/12).

## Documented coverage boundary (FR-007 observability)

The gate proves equality over names **reachable in the corpus**. `probeCorpus` is built from concrete,
traceable sources — `layoutAffectingAttrNames`, the `Attr`-builder / `Control.fs` attribute-name
vocabulary, and explicit non-layout names — read directly from the test source (not assumed), so a
future `toLayout` that reads a real control attribute is caught and a name no fixture can make
observable is honestly reported non-driving. Same "representative" discipline feature 097 used for its
≥1000-case property; documented at the test site.

Full suite result: `Feature101 layout dirty-set anti-drift guard (R7) – 12 passed, 0 failed`.
