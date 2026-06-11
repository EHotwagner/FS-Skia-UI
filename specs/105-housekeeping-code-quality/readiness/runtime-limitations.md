# Runtime limitations + permanent non-goals — feature 105 (housekeeping code-quality)

## Supported runtime

Feature 105 touches the framework wherever it runs: a **.NET 10 desktop** host rendering
through **Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop
(`net10.0`). **unsupported macOS/mobile/browser** — there is **no software-renderer fallback**;
those are out of scope for the framework and therefore for this feature.

Feature 105 is a behaviour-preserving internal refactor: it adds **no** runtime code path,
window, GPU, or wall-clock dependency. The lowering output, layout, visual state, charts,
DataGrid, Vulkan/Skia rendering, screenshots, and unsupported-environment diagnostics are all
unchanged; the internal Scene-stage and renderer-mode DUs keep their serialized output strings
byte-identical. So the change is platform-independent and introduces no new runtime failure
mode.

## Zero-`.fsi`-delta cross-reference (FR-011/FR-012, SC-007)

Every artifact introduced is internal: `module internal WidgetLowering` (no `.fsi`), the
`ChangeAdapters` / `AttrKey` / `SlotName` helpers in `Control.fs` (absent from `Control.fsi`),
the internal `EvidenceStage` DU in `Scene.fs`, and the internal renderer-mode DU in
`SkiaViewer.fs`. Each DU crosses to/from a string at exactly one edge, so the public
`StandardAttributeName`, `AttrValue.SlotFillsValue`, `SceneEvidenceFailure`
(`BlockedStage`/`DiagnosticCategory`), and `RendererMode` string fields are unchanged.
`git diff -- 'src/**/*.fsi'` is empty.

## Out of scope / permanent non-goals

- **Deferred audit items** (FR-013): §2.1 file splits (`SkiaViewer.fs`/`Control.fs`/
  `Vulkan.fs`), §5B `ControlId` single-case wrapper + SkiaViewer public diagnostic/mode field
  conversions, §2.4 mutable-heavy refactors, §4 `AttrValue<'msg>` custom equality.
- **Keep-as-string identifiers** (FR-010): `ControlKind`, public display/serialization output
  fields, consumer metadata keys (`columnKey`/`rowKey`), `ControlEvent.Kind` — deliberately
  open sets, untouched.
- **No new behaviour, no new public API, no full control-set migration** — this is cleanup only.

## Failure diagnostics

No new runtime failure path is introduced. The internal DUs add compile-time exhaustiveness
on the internal matches only; the public/serialized strings and diagnostic messages are
unchanged. The existing product suites stay green and byte-identical, which is the evidence
that no behaviour token changed.
