# Phase 1 Data Model: Housekeeping Code-Quality Remediation

This is a refactor; it introduces **no domain entities** and **no public types**.
The "entities" below are the new internal artifacts and the consolidation map.

## New shared module

### `module internal WidgetLowering` (`src/Controls/Widgets/WidgetLowering.fs`, no `.fsi`)

Compiled before `Widgets/Primitives.fs`. Members (all generic in `'msg` where the
originals were):

| Member | Shape | Replaces |
|--------|-------|----------|
| `withKeyOpt` | `string option -> Control<'msg> -> Control<'msg>` | 9 verbatim copies |
| `onString` | `string -> (string -> 'msg) -> Attr<'msg>` | 4 copies |
| `onStringList` | `string -> (string list -> 'msg) -> Attr<'msg>` | 1 copy (`CollectionsWidgets`) |
| `a11y` | accessibility-metadata builder (role/name + `Accessibility.keyboard true ["Enter";"Space"]`) | near-identical `Buttons.fs:35` + `Pickers.fs:40` |
| `intentToString` | intent/`StyleVariant` → `string` | `Primitives.fs:52` + `Input.fs:42` |

## New `Control.fs` module-scope helpers (internal, absent from `Control.fsi`)

| Member | Shape | Replaces |
|--------|-------|----------|
| `tryParseFloat` | `string -> float option` | the twice-duplicated nested `match Double.TryParse` |
| `onChangedBool` | `(bool -> 'msg) -> Attr<'msg>` | `onChanged` ×2 (CheckBox, Switch) |
| `onChangedFloat` | `(float -> 'msg) -> Attr<'msg>` | `onChanged` ×2 (Slider, NumericInput) |
| `onChangedString` | `(string -> 'msg) -> Attr<'msg>` | `onChanged` ×4 (TextBox, TextArea, RadioGroup, Tabs) |

## New internal DUs (closed sets, string boundaries)

| DU | Cases | Home | String boundary (stays public/serialized string) |
|----|-------|------|----------------------------------------------------|
| `AttrKey` | closed control-intrinsic names (`Text`,`Value`,`StyleClasses`,`VisualState`,`Slot`,`Accessibility`,`Nodes`,`RichTextRuns`,`Orientation`,`Width`,`Height`, + the DataGrid intrinsics `Rows`/`VisibleRange`/`Columns`/`SelectedRows`/`FocusedCell`) | `Control.fs` (internal) | `name : AttrKey -> string`; the public `StandardAttributeName` DU is **unchanged** |
| `SlotName` | `Leading \| Trailing \| Header \| Footer` | `Control.fs` (internal) | public `AttrValue.SlotFillsValue : (string * Control) list` carrier unchanged; parsed at the `slotRegions`/`lowerSlots` edge |
| `EvidenceStage` | `Scene \| Renderer` | `Scene.fs` (internal) | `BlockedStage`/`DiagnosticCategory` record fields stay `string` via one projection |
| renderer-mode DU | `Default \| Skia \| DeterministicScene \| UnsupportedHost \| MetadataHash \| PixelReadback` | `SkiaViewer.fs` (internal) | public `RendererMode : string` field unchanged; parsed once at the dispatch edge |

**Invariant**: every DU is matched **exhaustively** internally and crosses to/from
a string at exactly one edge, so a mistyped internal identifier is a compile error
while no public/serialized string format changes.

## Qualifier removals (US2) — exact set

- `module private` → `module` (×10): `ButtonsLowering`, `NavigationLowering`,
  `PickersLowering`, `ChartLowering`, `CollectionLowering`, `ContainerLowering`,
  `DisplayLowering`, `InputLowering`, `OverlayLowering`, `LegacyControls`.
- `let private` → `let` (×3 Reconcile): `attrValueEqual`, `diffAttrs`, `isKeepOp`.
- `let private` → `let` (×4 RetainedRender): `childPath`, `clockDuration`,
  `fadeAnimation`, `currentOpacity`.
- **Comments retained** on every site; **keep-list untouched** (FR-006).

## Compile-order constraint

`src/Controls/Controls.fsproj`: insert `<Compile Include="Widgets/WidgetLowering.fs" />`
between `CustomControl.fs` (line 91) and `Widgets/Primitives.fs` (line 92) — after
`Control.fs`/`Reconcile.fs`/`RetainedRender.fs`, before every consuming widget
module. No `.fsi` entry (internal module).
