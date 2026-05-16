# Contract: Control Catalog

## Purpose

The control catalog is the machine-readable source of truth for supported
controls, examples, public contracts, visual states, events, accessibility
metadata, tests, and evidence.

## Required Location

```text
src/Controls/catalog.yml
```

An additional generated Markdown summary may be produced for docs and readiness
evidence, but validation must read the structured catalog.

## Required Fields

Each supported control row must provide:

| Field | Required | Meaning |
|-------|----------|---------|
| `id` | yes | Stable lowercase control id. |
| `displayName` | yes | Human-readable name. |
| `category` | yes | Display, input, selection, navigation, layout, feedback, data, chart, graph, or custom. |
| `module` | yes | Public F# module that exposes the control. |
| `purpose` | yes | Short description of when to use the control. |
| `requiredAttributes` | yes | Attributes required for supported use. Empty list is allowed. |
| `commonAttributes` | yes | Commonly used content/layout/style/state attributes. |
| `events` | yes for interactive controls | Supported message-oriented events. |
| `visualStates` | yes | Normal, disabled, hover, pressed, focus, selected, validation, loading, or category-specific states. |
| `accessibility` | yes | Role, accessible name source, state metadata, focus behavior, keyboard operation, contrast evidence. |
| `examples` | yes | Runnable reference gallery or generated product example paths. |
| `tests` | yes | Semantic, interaction, layout/rendering, or accessibility test paths. |
| `evidence` | yes | Readiness evidence paths. |
| `supportStatus` | yes | `supported`, `experimental`, or `internal`. Only `supported` counts toward success criteria. |

## Minimum Supported Catalog

The first release must include at least 30 supported controls or supported
control variants across these categories:

- Display: text block, label, image, icon, separator, badge
- Input: button, icon button, text box, text area, numeric input, check box,
  radio group, switch, slider
- Selection and data: list view, list box, multi-select list, combo box,
  tree view, data grid
- Layout: stack, grid, dock, wrap, border, panel, scroll viewer, split view
- Navigation and overlays: tabs, menu, context menu, toolbar, tooltip, dialog,
  toast, overlay
- Feedback: progress bar, spinner, validation message
- Charts and graphs: line chart, bar chart, pie chart, scatter plot, graph view
- Extension: custom control wrapper

The implementation may rename controls to fit final public API naming, but the
catalog must still cover the functional categories above.

## Validation Contract

`ControlsCatalogCheck` or the equivalent `Verify` step must fail when:

- fewer than 30 rows are marked `supported`
- a supported row has no public module or no `.fsi` member
- a supported row has no runnable example
- a supported interactive row has no interaction test
- a supported row has undocumented visual states
- a supported row lacks accessibility metadata
- a supported row lacks readiness evidence
- chart or graph rows are owned by a separate Charts capability
- generated product examples use framework gallery source instead of
  product-owned code

Failures must name the control id, missing field, and expected artifact path.
