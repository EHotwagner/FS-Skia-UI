---
title: Controls Gallery — Charts & Graphs Page Spec
category: Controls Showcase specs
categoryindex: 8
---

# Controls Gallery — Charts & Graphs Page Spec

Inherits the shell, palette, pointer contract, keyboard contract, and
determinism rules from [`00-controls-gallery-overview.md`](00-controls-gallery-overview.md).

## Goal

Demonstrate every data-visualization control — line, bar, pie, scatter, and the
node/edge graph — rendering multi-series data in the categorical palette, with
pointer hover highlighting and click selection of series, slices, points, and
nodes.

## Controls Demonstrated

| Control | Module | Required | Events | Demonstrates |
|---------|--------|----------|--------|--------------|
| line-chart | LineChart | `series` | `onSelected` | Multi-series trend lines |
| bar-chart | BarChart | `series` | `onSelected` | Grouped/stacked bars |
| pie-chart | PieChart | `values` | `onSelected` | Part-to-whole slices |
| scatter-plot | ScatterPlot | `series` | `onSelected` | Point clouds across series |
| graph-view | GraphView | `nodes` | `onSelected` | Nodes and edges |

## User Experience

The page is a dashboard grid of five visualizations sharing one dataset theme and
the categorical palette. Hovering a series, slice, point, or node highlights it and
shows a value readout; clicking selects it and fires `onSelected`, updating a shared
"Selection detail" panel. A legend maps each series color to its name.

## Layout

- A heading `Charts & Graphs` and description.
- A 2×3 `Grid`: line chart, bar chart, pie chart, scatter plot, graph view, and a
  "Selection detail" `Panel` in the sixth cell.
- A shared legend mapping series names to the categorical palette colors.
- Each chart sized to a fixed aspect with axis labels where applicable.

## Mouse & Pointer Interactions

- `HoverEnter` over a line/series, bar, pie slice, scatter point, or graph node
  highlights it (raised emphasis) and shows a value tooltip; `HoverLeave` clears it.
- `Click` a series/bar/slice/point/node fires `onSelected` with its key, marks it
  selected, and populates the "Selection detail" panel.
- Wheel `Scroll` over the scatter plot or graph view zooms its view (where
  supported); the status strip narrates pointer kind, target, and coordinates.
- Clicking empty chart space clears the selection.

## Keyboard

- `Tab` moves focus between charts; `Arrow` keys move the highlighted item within
  the focused chart's series/slices/points/nodes.
- `Enter` selects the highlighted item; `Esc` clears the selection.

## Core Behaviors

- Each chart consumes the shared dataset and assigns colors from the categorical
  palette in order, consistently across charts and the legend.
- Hover highlight and click selection are independent: hover is transient, selection
  persists until changed or cleared.
- The pie chart's slices sum to the whole; labels show share percentages.
- The graph view lays out nodes and draws edges; selecting a node emphasizes its
  incident edges.
- Charts re-render correctly on theme/accent change without losing selection.

## Data Model

- A shared multi-series dataset (`ChartSeries` list of `ChartPoint`s) and a pie
  `ChartPoint` value list.
- A graph node list and an edge list.
- The current hovered key and the selected key per chart (or one shared selection).
- The "Selection detail" snapshot (chart, key, value).

## Visual / Palette Requirements

- Series colors come from the categorical palette in fixed order; the legend
  matches exactly.
- Hovered items raise emphasis (brighter fill / thicker stroke); selected items show
  an accent ring or outline.
- Axes, gridlines, and labels use border-divider and foreground-muted; values meet
  contrast against the chart background.

## App State

Track: shared dataset; graph nodes/edges; per-chart hovered and selected keys;
selection-detail snapshot; seed.

## Determinism and Evidence

- Accept a seed governing generated series values; the same seed yields identical
  charts.
- Evidence mode hovers one item in each chart, then clicks to select one series, one
  slice, one point, and one node.
- Evidence outcome: per-chart selected keys, the selection-detail snapshot, hover
  count, and close reason.
- Screenshot evidence shows the line and bar charts with a highlighted series, the
  pie with a selected slice, and the graph with a selected node and emphasized
  edges.

## Acceptance Criteria

- All five visualizations render the shared dataset in the categorical palette,
  matching the legend.
- Hover highlights items and shows value readouts; selection persists and fires
  `onSelected`.
- The pie slices sum to the whole with percentage labels.
- Selecting a graph node emphasizes its incident edges.
- Theme/accent changes re-render charts without losing the current selection.

## Out of Scope

- Live streaming / animated data updates.
- Chart export to image or data formats.
- Editing data points by dragging them.
