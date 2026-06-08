# Usage Coherence Review (US2) — 079-doc-preview-examples (T014/T015)

Authoritative command: manual review of each `ControlSampleDefinition.UsageNote` +
sample content (`tests/ControlsPreview.Harness/PreviewSamples.fs`) against the documented
usage on `docs/controls/<id>.md` (its required attributes / Overview).
Artifact path: this file. Failure class: `image-prose-contradiction`.
(The gate-owned `controls-catalog-docs.md` records header/preview **currency**; this file
records image↔prose **coherence** so the gate's overwrite does not erase the review.)

## Result: 0 contradictions across ≥8 detail pages spanning every family

Each sample populates exactly the attribute(s) the detail page documents as required, so the
rendered preview and the page prose stay coherent (FR-006). Reviewed pages span all distinct
families (display, labelled input, slider, checkbox/switch, list-box, data-grid, chart,
composed layout, overlay, navigation):

| Control | Family | Documented requires | Sample depicts | Coherent |
|---------|--------|---------------------|----------------|----------|
| text-block | display/text | `text` | Text = "Status: all systems nominal" | yes |
| text-box | labelled input | `value` | Value = "jane@example.com" | yes |
| slider | input | `value` | Value = 0.5 (mid-track) | yes |
| check-box | selection | `text` | Text + Checked = true | yes |
| switch | selection | (boolean) | Checked = true (on) | yes |
| list-box | selection | `items` | Items + highlighted "Beta" | yes |
| data-grid | data | `columns`, `rows` | 2 columns + 2 rows + selected row | yes |
| line-chart | chart | `series` | a plotted sample series | yes |
| stack | composed layout | `children` | three child labels | yes |
| dialog | overlay | `children` | title + child content (one frame) | yes |
| tabs | navigation | `items` | items + active page "Profile" | yes |

The **data-grid** case is the explicit FR-006 example ("a control documented as requiring
`columns`/`rows` depicts columns and rows") — the sample shows a Name/Qty header row over two
data rows with a selected row, matching the page's "product-owned rows ... columns/rows".

Motion/interaction-bearing controls (dialog, toast, overlay, spinner, tooltip,
context-menu) are shown as a **single representative static frame**, consistent with their
detail-page prose and the spec's out-of-scope on animation — the limitation is stated, not
faked. No preview contradicts its documented usage.
