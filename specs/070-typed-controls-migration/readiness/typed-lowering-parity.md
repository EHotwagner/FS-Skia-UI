# Typed lowering-parity matrix (41 controls, SC-002)

Each typed `view |> Widget.toControl` is asserted **structurally equal** to the
normalized legacy builder output (attribute order normalized, `EventValue`
closures canonicalized to the message they produce for a representative event).
The assertions live in `tests/Controls.Tests/TypedMigrationTests.fs`
(`Feature 070 lowering parity (41 controls, SC-002)`) and run green.

**Zero divergent controls.** All 41 lowerings are real; no `[S]` disclosure is
required (FR-011 / SC-010).

| # | id | Module | Lowers to | Parity test |
| --- | --- | --- | --- | --- |
| 1 | rich-text | RichText | `RichText.create (RichText.block Runs) []` | Display group |
| 2 | label | Label | `Label.create [ Label.text Text ]` | Display group |
| 3 | image | Image | `Image.create [ Image.source Value ]` | Display group |
| 4 | icon | Icon | `Icon.create [ Icon.name Text ]` | Display group |
| 5 | separator | Separator | `Separator.create []` | Display group |
| 6 | badge | Badge | `Badge.create [ Badge.text Text ]` | Display group |
| 7 | progress-bar | ProgressBar | `ProgressBar.create [ ProgressBar.value Value ]` | Display group |
| 8 | spinner | Spinner | `Spinner.create []` | Display group |
| 9 | validation-message | ValidationMessage | `ValidationMessage.create [ text; Attr.validation Severity ]` | Display group |
| 10 | icon-button | IconButton | `IconButton.create [ icon; enabled; style; onClick? ]` | Input group |
| 11 | numeric-input | NumericInput | `NumericInput.create [ value; readOnly; onChanged? ]` | Input group |
| 12 | radio-group | RadioGroup | `RadioGroup.create [ items; selected?; onChanged? ]` | Input group |
| 13 | switch | Switch | `Switch.create [ checked'; onChanged? ]` | Input group |
| 14 | slider | Slider | `Slider.create [ value; onChanged? ]` | Input group |
| 15 | text-area | TextArea | `TextArea.create [ value; readOnly; validation; onChanged? ] |> withKey` | delegation + group |
| 16 | list-view | ListView | `Control.standard (Custom "list-view") [ items; selectedKeys; visibleRange; onSelected? ]` | Stateful collections |
| 17 | list-box | ListBox | `Control.standard (Custom "list-box") [ … ]` | (via group + delegation) |
| 18 | multi-select-list | MultiSelectList | `Control.standard (Custom "multi-select-list") [ … ]` | (via group + delegation) |
| 19 | combo-box | ComboBox | `Control.standard (Custom "combo-box") [ … ]` | (via group + delegation) |
| 20 | tree-view | TreeView | `Control.standard (Custom "tree-view") [ … ]` | (via group + delegation) |
| 21 | grid | Grid | `Grid.create [ Grid.children … ]` | Container group |
| 22 | dock | Dock | `Dock.create [ Dock.children … ]` | Container group |
| 23 | wrap | Wrap | `Wrap.create [ orientation; spacing; children ]` | Container group |
| 24 | border | Border | `Border.create [ child; thickness; padding ]` | Container group |
| 25 | panel | Panel | `Panel.create [ Panel.children … ]` | Container group |
| 26 | scroll-viewer | ScrollViewer | `Control.standard (Custom "scroll-viewer") [ child; onChanged? ] |> withKey` | Container group |
| 27 | split-view | SplitView | `Control.standard (Custom "split-view") [ children; orientation; onChanged? ]` | Container group |
| 28 | tabs | Tabs | `Tabs.create [ items; selected?; onChanged? ]` | Navigation group |
| 29 | menu | Menu | `Menu.create [ items; onSelected? ]` | Navigation group |
| 30 | context-menu | ContextMenu | `Control.standard (Custom "context-menu") [ items; onSelected? ]` | Navigation group |
| 31 | toolbar | Toolbar | `Toolbar.create [ children; onClick? ]` | Navigation group |
| 32 | tooltip | Tooltip | `Tooltip.create [ Tooltip.text Text ]` | Overlay group |
| 33 | dialog | Dialog | `Dialog.create [ children; title?; selected; onSelected? ]` | Overlay group |
| 34 | toast | Toast | `Toast.create [ text; Attr.validation Severity ]` | Overlay group |
| 35 | overlay | Overlay | `Overlay.create [ child; selected ]` | Overlay group |
| 36 | line-chart | LineChart | `LineChart.create [ series; onSelected? ]` | Charts group |
| 37 | bar-chart | BarChart | `BarChart.create [ series; onSelected? ]` | Charts group |
| 38 | pie-chart | PieChart | `PieChart.create [ values; onSelected? ]` | Charts group |
| 39 | scatter-plot | ScatterPlot | `ScatterPlot.create [ series; onSelected? ]` | Charts group |
| 40 | graph-view | GraphView | `GraphView.create [ nodes; onSelected? ]` | Charts group |
| 41 | custom-control | CustomControl | `Widget.ofControl` round-trip (bridge, no schema) | custom-control test |

Interaction: every optional event set to `None` lowers to **no** event binding
(FR-005), asserted in `every optional event prop set to None lowers to no event
binding`. Stateful `init`/`update` equal the reused `TextInput`/`Collections`
model results (SC-003), asserted in the `Feature 070 stateful delegation` list.
