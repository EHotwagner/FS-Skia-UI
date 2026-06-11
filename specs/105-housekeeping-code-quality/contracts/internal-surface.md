# Contract: Internal surface (zero public `.fsi` delta)

This feature is **Tier 2**. The controlling contract is that **no public `.fsi`
line changes**. Every artifact introduced is internal; every DU keeps its
public/serialized representation a byte-identical string.

## Public-surface invariant (the gate)

```
git diff --stat origin/main...HEAD -- 'src/**/*.fsi'   # MUST be empty
```

- `WidgetLowering` — `module internal`, **no `.fsi`** → nothing captured in any
  per-package or cross-package surface baseline.
- `onChangedBool/Float/String`, `tryParseFloat` — `Control.fs` module-scope,
  **absent from `Control.fsi`**.
- `AttrKey` — internal to `Control.fs`, absent from `Control.fsi`. The public
  `StandardAttributeName` DU (`Types.fsi:86`) is **unchanged** (13 cases).
- `SlotName` — internal to `Control.fs`. The public `AttrValue.SlotFillsValue`
  carrier type in `Types.fsi` is **unchanged** (`(string * Control<'msg>) list`).
- `EvidenceStage` — internal to `Scene.fs`. The public `SceneEvidenceFailure`
  record fields `BlockedStage`/`DiagnosticCategory` stay `string` in `Scene.fsi`.
- renderer-mode DU — internal to `SkiaViewer.fs`. The public `RendererMode`
  string field(s) in `SkiaViewer.fsi` are **unchanged**.

If `Route` escalates to `controls-public-surface` for routing reasons (101/102
precedent), the per-package/cross-package baselines still diff **clean** because
no `.fsi` changed — escalation selects gates, it does not imply a surface delta.

## Member shapes (informative — internal, not signature-pinned)

```fsharp
// WidgetLowering (module internal, no .fsi)
val withKeyOpt   : string option -> Control<'msg> -> Control<'msg>
val onString     : string -> (string -> 'msg) -> Attr<'msg>
val onStringList : string -> (string list -> 'msg) -> Attr<'msg>
val a11y         : (* role/name + keyboard Enter/Space metadata builder *)
val intentToString : (* intent -> string *)

// Control.fs module scope (internal)
val tryParseFloat   : string -> float option
val onChangedBool   : (bool -> 'msg)   -> Attr<'msg>
val onChangedFloat  : (float -> 'msg)  -> Attr<'msg>
val onChangedString : (string -> 'msg) -> Attr<'msg>

// Internal DUs
type internal AttrKey = Text | Value | StyleClasses | VisualState | Slot
                      | Accessibility | Nodes | RichTextRuns | Orientation
                      | Width | Height
                      | Rows | VisibleRange | Columns | SelectedRows | FocusedCell
val internal AttrKey.name : AttrKey -> string

type internal SlotName = Leading | Trailing | Header | Footer   // Control.fs
type internal EvidenceStage = Scene | Renderer                 // Scene.fs
// SkiaViewer.fs renderer-mode dispatch DU
type internal RendererModeKind =
    | Default | Skia | DeterministicScene | UnsupportedHost | MetadataHash | PixelReadback
```

> These shapes are illustrative; because they are internal they are **not** pinned
> by a surface baseline and may be adjusted during implementation as long as the
> public-surface invariant and the parity contract hold.
