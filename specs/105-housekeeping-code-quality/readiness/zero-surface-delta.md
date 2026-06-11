# Zero public-surface delta + deferred/keep-as-string untouched (T019) — SC-007 / SC-008 / FR-014

## SC-007 — zero public `.fsi` delta

```
$ git diff --name-only -- 'src/**/*.fsi'
(empty)
```

No `src/**/*.fsi` file is changed. Every artifact this feature introduces is internal:
`module internal WidgetLowering` (`src/Controls/Widgets/WidgetLowering.fs`, **no `.fsi`**),
`module internal AttrKeys` (`src/Controls/Internal/AttrKeys.fs`, **no `.fsi`**), the `ChangeAdapters` /
`SlotName` helpers in `Control.fs` (absent from `Control.fsi`), the `EvidenceStage` DU in
`Scene.fs` (absent from `Scene.fsi`), and the `RendererModeKind` DU in `SkiaViewer.fs` (absent
from `SkiaViewer.fsi`). Tier 2; **no per-package or cross-package baseline recapture required**
(the optional FR-012 public `StandardAttributeName` expansion was **not** elected, D3).

## Files touched (working tree)

- 17 `.fs` bodies (10 `Widgets/*.fs`, `Control.fs`, `DataGrid.fs`, `Reconcile.fs`,
  `RetainedRender.fs`, `Scene.fs`, `SkiaViewer.fs`).
- 2 new internal-only files with **no `.fsi`**: `Widgets/WidgetLowering.fs`, `AttrKeys.fs`.
- `Controls.fsproj` — two `<Compile Include>` entries (compile-order only; not a surface).
- 1 new test: `tests/Controls.Tests/Feature105ParityTests.fs` (+ its fsproj entry).

## SC-008 — deferred / keep-as-string items untouched

```
$ git diff --stat -- src/Controls/Types.fs src/Controls/Types.fsi \
    src/SkiaViewer/SkiaViewer.fsi src/Scene/Scene.fsi src/Controls/DataGrid.fsi
(empty)
```

- No `ControlId` single-case wrapper; `type ControlId = string` unchanged (§5B deferred).
- No `ControlKind` / `StandardControlKind` change (the only diff mentions are descriptive
  comment text); `ControlEvent.Kind` unchanged (FR-010).
- No SkiaViewer public diagnostic/mode field conversion (`DiagnosticClass`,
  `ViewerLaunchOutcome.Mode` unchanged); the public `RendererMode` field stays `string`.
- No `AttrValue<'msg>` custom-equality change (§4 deferred); no file split (§2.1 deferred).
- DataGrid consumer metadata keys `columnKey`/`rowKey` kept as strings (4 sites, untouched).

## FR-014 — comments carry no governance tokens

```
$ git diff -- 'src/**/*.fs' | grep '^+' | grep -E 'TODO|FIXME|XXX|HACK|NotImplementedException|\.md'
(none)
```

Every retained/added comment is purely descriptive — no literal evidence filename, no bare
gate/status token that the window-visibility or diff-scan audits could parse as a
behaviour/status signal. The `Double.TryParse`→`number-parse` comment rewording keeps the
SC-002 grep clean (one `Double.TryParse`, inside `tryParseFloat`).
