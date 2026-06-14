---
title: Controls Catalog
category: Controls
categoryindex: 8
index: 2
description: The authoritative catalog of every supported FS.Skia.UI control, grouped by category, each linking to its detail page and generated from the single-source control catalog.
---

# Controls Catalog

This is the authoritative answer to **"what controls exist, and what does each do?"** —
every supported `FS.Skia.UI.Controls` control, grouped by category. Each entry links to a
detail page with prose, a usage example where one exists, a rendered preview (or an honest
note where a control cannot be previewed), and a link to its generated API reference.

The list below is **generated** from the single-source control catalog
(`CatalogGen.catalogFacts`) — the same source that drives `src/Controls/catalog.yml` and
`src/Controls/Catalog.fs` — so it can never drift from the controls the framework actually
ships. The `ControlsCatalogDocsCheck` gate fails if it does. To change it, edit the catalog
single source and run `./fake.sh build -t RefreshSurfaceBaselines`; never hand-edit the
generated region below.

New here? Start with the [Controls in the Spec Kit workflow](spec-kit-workflow.html)
narrative, which explains where controls are chosen, authored, and validated.

<!-- BEGIN GENERATED: catalog-docs/index -->
**52 supported controls**, grouped by category.

### Display

| Control | Purpose |
|---------|---------|
| [Text Block](text-block.html) | Static model-owned text display. |
| [Rich Text](rich-text.html) | Skia-specific rich text display with measurement, clipping, effects, diagnostics, and accessibility metadata. |
| [Label](label.html) | Short form label text. |
| [Image](image.html) | Image placeholder or drawing-surface reference. |
| [Icon](icon.html) | Named icon glyph or product symbol. |
| [Separator](separator.html) | Visual divider between regions. |
| [Badge](badge.html) | Compact status label. |

### Input

| Control | Purpose |
|---------|---------|
| [Button](button.html) | Pointer and keyboard activatable command. |
| [Icon Button](icon-button.html) | Icon-only activatable command. |
| [Text Box](text-box.html) | Plain single-line text entry. |
| [Text Area](text-area.html) | Plain multi-line text entry. |
| [Numeric Input](numeric-input.html) | Model-owned numeric value editor. |
| [Slider](slider.html) | Continuous numeric value selection. |
| [Toggle Button](toggle-button.html) | On/off command with product-owned pressed state. |
| [Split Button](split-button.html) | Primary action plus a popup menu of secondary commands. |
| [Date Picker](date-picker.html) | Typed date entry with a popup calendar. |
| [Time Picker](time-picker.html) | Typed time entry with hour and minute segments. |

### Selection

| Control | Purpose |
|---------|---------|
| [Check Box](check-box.html) | Boolean choice with checked state. |
| [Radio Group](radio-group.html) | Single selection from a visible option set. |
| [Switch](switch.html) | Compact Boolean setting. |
| [List Box](list-box.html) | Single-selection list box. |
| [Multi Select List](multi-select-list.html) | Multiple-selection list with model-owned selected keys. |
| [Combo Box](combo-box.html) | Compact selection list. |
| [Color Picker](color-picker.html) | Palette swatch color selection. |

### Data

| Control | Purpose |
|---------|---------|
| [List View](list-view.html) | Bounded visible-range list display. |
| [Tree View](tree-view.html) | Hierarchical item display. |
| [Data Grid](data-grid.html) | Table-like bounded visible-range data control with product-owned rows, selection, focus, sort, and filter metadata. |

### Layout

| Control | Purpose |
|---------|---------|
| [Stack](stack.html) | Ordered vertical or horizontal child composition. |
| [Grid](grid.html) | Structured child composition. |
| [Dock](dock.html) | Docked region composition. |
| [Wrap](wrap.html) | Wrapping child layout. |
| [Border](border.html) | Single child with border and padding. |
| [Panel](panel.html) | General-purpose child surface. |
| [Scroll Viewer](scroll-viewer.html) | Scrollable child viewport. |
| [Split View](split-view.html) | Resizable two-region layout. |

### Navigation

| Control | Purpose |
|---------|---------|
| [Tabs](tabs.html) | Model-owned active page selection. |
| [Menu](menu.html) | Command menu selection. |
| [Context Menu](context-menu.html) | Contextual command menu. |
| [Toolbar](toolbar.html) | Compact command group. |

### Overlay

| Control | Purpose |
|---------|---------|
| [Tooltip](tooltip.html) | Auxiliary hover/focus explanation. |
| [Dialog](dialog.html) | Modal content region. |
| [Overlay](overlay.html) | Layered child content. |

### Feedback

| Control | Purpose |
|---------|---------|
| [Toast](toast.html) | Transient status message. |
| [Progress Bar](progress-bar.html) | Determinate progress indicator. |
| [Spinner](spinner.html) | Indeterminate progress indicator. |
| [Validation Message](validation-message.html) | Validation text tied to model state. |

### Chart

| Control | Purpose |
|---------|---------|
| [Line Chart](line-chart.html) | Controls-owned line data visualization. |
| [Bar Chart](bar-chart.html) | Controls-owned bar data visualization. |
| [Pie Chart](pie-chart.html) | Controls-owned part-to-whole visualization. |
| [Scatter Plot](scatter-plot.html) | Controls-owned point cloud visualization. |

### Graph

| Control | Purpose |
|---------|---------|
| [Graph View](graph-view.html) | Controls-owned node and edge visualization. |

### Custom

| Control | Purpose |
|---------|---------|
| [Custom Control](custom-control.html) | Product-owned wrapper; renderTree paints a labeled placeholder, not the custom Render/Draw content — build must-show geometry from primitive controls (Border/TextBlock/Stack). |
<!-- END GENERATED: catalog-docs/index -->
