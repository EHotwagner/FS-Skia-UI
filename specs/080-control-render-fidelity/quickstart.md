# Quickstart — Faithful Control Preview Rendering (080)

A render-capable host (native Skia) is required for rendering and the fidelity
gate; the SkiaSharp-free currency gate runs anywhere.

## 1. Reproduce the 079 defect (failing-first)

```bash
# The current schematic renderer draws label-on-a-box for Tier-3 controls.
dotnet run --project tests/ControlsPreview.Harness -- --render
# Inspect a Tier-3 preview — it is a single word on a box:
#   docs/img/controls/line-chart.png  (→ "LINE-CHART")
#   docs/img/controls/list-box.png    (→ "LIST-BOX")
#   docs/img/controls/icon.png        (→ "? HOME"  missing-glyph box)
```

## 2. Confirm the data-extraction bug

In FSI / a scratch test: a chart control built with `LineChart.series sampleSeries`
yields `Control.fs:chartValues = []` today (matches only `float list`, but the
attribute stores `UntypedValue(ChartSeries list)`). After the fix it yields the
series points.

## 3. Implement (order)

1. **Extraction** — fix `chartValues` (`src/Controls/Control.fs:159`) to read
   `ChartSeries list` / `ChartPoint list`.
2. **Faithful geometry** — replace the uniform `renderNode` body
   (`src/Controls/Control.fs:194`) with per-`Kind` geometry lowered to existing
   Scene primitives within bounds; stop emitting the opaque `Chart` node on the
   preview path (so `SceneRenderer.fs:394` off-canvas painter is bypassed).
3. **Sample data** — author font-safe sample data + the required
   `ContentSignature` field for every Demonstrative control in
   `tests/ControlsPreview.Harness/PreviewSamples.fs` (FR-014).
4. **Gate** — add `Fidelity.fs` (decode + signature check + fixtures) and a
   `--fidelity` mode in `Program.fs`.
5. **Fixtures** — commit `fixtures/fidelity/lowfi/*` (from pre-fix `main`) and
   `faithful/*`.
6. **Target + routing** — add `ControlFidelityCheck` (Targets DU/registry/name/
   spec, `knownGates`, `Update.fs` shellout, `Routing.fs` rule), then
   `./fake.sh build -t RefreshSurfaceBaselines` to regenerate
   `validation.contract.yml`.
7. **Regenerate + correct prose** — re-render all previews, regenerate
   `docs/controls/*.md` Preview sections, author `readiness/*` against decoded
   images.

## 4. Regenerate faithful previews

```bash
dotnet run --project tests/ControlsPreview.Harness -- --render
git diff --stat docs/img/controls/   # every preview moves to faithful geometry
```

## 5. Run the fidelity gate (red → green)

```bash
# Render-capable harness mode:
dotnet run --project tests/ControlsPreview.Harness -- --fidelity
# Expect: every Demonstrative PNG passes its signature; every lowfi fixture FAILS;
# every faithful fixture PASSES; report at readiness/control-fidelity.md.

# Governance target (shells out to the harness):
./fake.sh build -t ControlFidelityCheck
```

## 6. Escalated validation (sequential — shared .fake state)

```bash
./fake.sh build -t Route --enforce          # confirm tier + required artifacts
./fake.sh build -t Dev
./fake.sh build -t ControlFidelityCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck     # known non-authoritative local env failure
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# plus harness suite:
dotnet run --project tests/ControlsPreview.Harness -- --sequenced
```

## Done when

- SC-001: 100% of controls render faithful, control-specific previews or are
  honestly `Unsupported`; 0 label-on-box passes.
- SC-003: gate fails 100% of pre-fix/lowfi fixtures, passes 100% faithful.
- SC-004: every per-control claim matches decoded image content.
- SC-005: `ControlFidelityCheck` is required evidence for preview-asset changes.
- SC-006: no product/runtime control regression; non-preview tests still pass.
