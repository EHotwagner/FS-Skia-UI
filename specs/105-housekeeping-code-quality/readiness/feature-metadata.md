# Feature metadata record (T004)

- **Feature**: 105-housekeeping-code-quality (active per `.specify/feature.json`).
- **Tier**: 2 (internal change). No public `.fsi` delta under the default choices (D3–D6).
- **Affected layer**: `FS.Skia.UI.Controls` (`Widgets/*.fs`, `Control.fs`, `Reconcile.fs`,
  `RetainedRender.fs`, `DataGrid.fs`, new `Widgets/WidgetLowering.fs`), `FS.Skia.UI.Scene`
  (`Scene.fs`), `FS.Skia.UI.SkiaViewer` (`SkiaViewer.fs`) — all `.fs` bodies.
- **Public-API impact**: none — zero public `src/**/*.fsi` delta (SC-007); no baseline recapture.
- **Elmish/MVU applicability**: **N/A** — no stateful or I/O-bearing behaviour changes. The
  `SkiaViewer` Elmish update and the `Controls.Elmish` host loop are read only for the
  renderer-mode DU edge; their `Model`/`Msg`/`Effect`/interpreter contracts are unchanged.
- **Evidence obligations**: (1) the routed gate set green; (2) the `Feature105ParityTests`
  parity guard green (SC-006); (3) Controls + Controls.Elmish suites green and unchanged
  (SC-005); (4) `git diff -- 'src/**/*.fsi'` empty (SC-007); (5) deferred / keep-as-string items
  untouched (SC-008); (6) `EvidenceGraph` + `EvidenceAudit` verdict PASS with 0 synthetic.
- **Synthetic evidence**: none. The parity guard compares real lowering output; no mocks,
  fakes, placeholders, or forced error fixtures. No `[S]`/`[SEH]` tasks.

## T002 — audit citation re-verification (working tree)

Re-verified before editing (line numbers had shifted from the audit):
- `onChanged` inline parsers: `Control.fs:1606/1611/1616/1621/1628/1633/1639/1683` (8 sites). ✓
- `slotRegions`: `Control.fs:99`. ✓
- `StandardAttributeName`: public DU at `Types.fs` / `Types.fsi` (unchanged, D3). ✓
- `RetainedRender` `let private`: `childPath:73`, `clockDuration:87`, `fadeAnimation:100`,
  `currentOpacity:123` (cited four), plus uncited `fadeOutAnimation:113`, `firstFrameCollisions:203`
  (left as-is). ✓
- `Reconcile` `let private`: `attrValueEqual:46`, `diffAttrs:69`, `isKeepOp:90` (cited three),
  plus uncited `applyAttrChanges:229` (left as-is). ✓
- 10 `module private *Lowering` declarations across `src/Controls/Widgets/`. ✓
- Scene evidence stage strings: `Scene.fs:736/738/742/744` (`"scene"`/`"renderer"`). ✓
- Renderer-mode case-insensitive dispatch: `SkiaViewer.fs:2016/2023/2047` (+ `2573`). ✓
