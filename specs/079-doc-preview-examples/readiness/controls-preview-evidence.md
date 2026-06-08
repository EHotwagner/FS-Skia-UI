# Controls Preview Evidence (honesty ledger) — 079-doc-preview-examples

Authoritative command: `dotnet run --project tests/ControlsPreview.Harness -- --render`
(render-only path, render-capable host) then `./fake.sh build -t ControlsCatalogDocsCheck`
(SkiaSharp-free structural cross-check of the committed bytes).
Artifact path: this file. Failure class: `preview-evidence-inconsistent` / `count-mismatch`.

Every committed preview is a real **render-only** raster
(`Widget.toControl` -> `Control.render Theme.light` -> `SceneNode.Group` ->
`SkiaViewer.captureScreenshotEvidence`, `CaptureMode = ViewerRenderTargetPng`,
`status = ScreenshotOk`), validated decodable / non-1x1 / above the 420-byte
trivial-content floor. `custom-control` is honestly declared **unsupported** (no image;
its detail page carries `preview-status: unsupported`) - not a fabricated/1x1 placeholder.

## Per-control ledger (52 controls, catalog order)

| Control id | Display name | Renderer mode | Decodable | Dimensions | Bytes | Classification |
|------------|--------------|---------------|-----------|------------|-------|----------------|
| text-block | Text Block | render-only / ViewerRenderTargetPng | yes | 320×160 | 902 | demonstrative |
| rich-text | Rich Text | render-only / ViewerRenderTargetPng | yes | 320×160 | 687 | demonstrative |
| label | Label | render-only / ViewerRenderTargetPng | yes | 320×160 | 686 | demonstrative |
| image | Image | render-only / ViewerRenderTargetPng | yes | 320×160 | 667 | demonstrative |
| icon | Icon | render-only / ViewerRenderTargetPng | yes | 320×160 | 641 | demonstrative |
| separator | Separator | render-only / ViewerRenderTargetPng | yes | 320×160 | 679 | demonstrative |
| badge | Badge | render-only / ViewerRenderTargetPng | yes | 320×160 | 569 | demonstrative |
| button | Button | render-only / ViewerRenderTargetPng | yes | 320×160 | 584 | demonstrative |
| icon-button | Icon Button | render-only / ViewerRenderTargetPng | yes | 320×160 | 486 | demonstrative |
| text-box | Text Box | render-only / ViewerRenderTargetPng | yes | 320×160 | 903 | demonstrative |
| text-area | Text Area | render-only / ViewerRenderTargetPng | yes | 320×160 | 957 | demonstrative |
| numeric-input | Numeric Input | render-only / ViewerRenderTargetPng | yes | 320×160 | 545 | demonstrative |
| check-box | Check Box | render-only / ViewerRenderTargetPng | yes | 320×160 | 938 | demonstrative |
| radio-group | Radio Group | render-only / ViewerRenderTargetPng | yes | 320×160 | 637 | demonstrative |
| switch | Switch | render-only / ViewerRenderTargetPng | yes | 320×160 | 637 | demonstrative |
| slider | Slider | render-only / ViewerRenderTargetPng | yes | 320×160 | 574 | demonstrative |
| list-view | List View | render-only / ViewerRenderTargetPng | yes | 320×160 | 678 | demonstrative |
| list-box | List Box | render-only / ViewerRenderTargetPng | yes | 320×160 | 657 | demonstrative |
| multi-select-list | Multi Select List | render-only / ViewerRenderTargetPng | yes | 320×160 | 830 | demonstrative |
| combo-box | Combo Box | render-only / ViewerRenderTargetPng | yes | 320×160 | 670 | demonstrative |
| tree-view | Tree View | render-only / ViewerRenderTargetPng | yes | 320×160 | 690 | demonstrative |
| data-grid | Data Grid | render-only / ViewerRenderTargetPng | yes | 320×160 | 2230 | demonstrative |
| stack | Stack | render-only / ViewerRenderTargetPng | yes | 320×160 | 1199 | demonstrative |
| grid | Grid | render-only / ViewerRenderTargetPng | yes | 320×160 | 1025 | demonstrative |
| dock | Dock | render-only / ViewerRenderTargetPng | yes | 320×160 | 872 | demonstrative |
| wrap | Wrap | render-only / ViewerRenderTargetPng | yes | 320×160 | 1148 | demonstrative |
| border | Border | render-only / ViewerRenderTargetPng | yes | 320×160 | 856 | demonstrative |
| panel | Panel | render-only / ViewerRenderTargetPng | yes | 320×160 | 1028 | demonstrative |
| scroll-viewer | Scroll Viewer | render-only / ViewerRenderTargetPng | yes | 320×160 | 1296 | demonstrative |
| split-view | Split View | render-only / ViewerRenderTargetPng | yes | 320×160 | 1101 | demonstrative |
| tabs | Tabs | render-only / ViewerRenderTargetPng | yes | 320×160 | 638 | demonstrative |
| menu | Menu | render-only / ViewerRenderTargetPng | yes | 320×160 | 595 | demonstrative |
| context-menu | Context Menu | render-only / ViewerRenderTargetPng | yes | 320×160 | 806 | demonstrative |
| toolbar | Toolbar | render-only / ViewerRenderTargetPng | yes | 320×160 | 942 | demonstrative |
| tooltip | Tooltip | render-only / ViewerRenderTargetPng | yes | 320×160 | 915 | demonstrative |
| dialog | Dialog | render-only / ViewerRenderTargetPng | yes | 320×160 | 1059 | demonstrative |
| toast | Toast | render-only / ViewerRenderTargetPng | yes | 320×160 | 868 | demonstrative |
| overlay | Overlay | render-only / ViewerRenderTargetPng | yes | 320×160 | 1132 | demonstrative |
| progress-bar | Progress Bar | render-only / ViewerRenderTargetPng | yes | 320×160 | 569 | demonstrative |
| spinner | Spinner | render-only / ViewerRenderTargetPng | yes | 320×160 | 670 | demonstrative |
| validation-message | Validation Message | render-only / ViewerRenderTargetPng | yes | 320×160 | 883 | demonstrative |
| line-chart | Line Chart | render-only / ViewerRenderTargetPng | yes | 320×160 | 723 | demonstrative |
| bar-chart | Bar Chart | render-only / ViewerRenderTargetPng | yes | 320×160 | 674 | demonstrative |
| pie-chart | Pie Chart | render-only / ViewerRenderTargetPng | yes | 320×160 | 667 | demonstrative |
| scatter-plot | Scatter Plot | render-only / ViewerRenderTargetPng | yes | 320×160 | 713 | demonstrative |
| graph-view | Graph View | render-only / ViewerRenderTargetPng | yes | 320×160 | 745 | demonstrative |
| toggle-button | Toggle Button | render-only / ViewerRenderTargetPng | yes | 320×160 | 527 | demonstrative |
| split-button | Split Button | render-only / ViewerRenderTargetPng | yes | 320×160 | 1601 | demonstrative |
| date-picker | Date Picker | render-only / ViewerRenderTargetPng | yes | 320×160 | 2051 | demonstrative |
| time-picker | Time Picker | render-only / ViewerRenderTargetPng | yes | 320×160 | 1068 | demonstrative |
| color-picker | Color Picker | render-only / ViewerRenderTargetPng | yes | 320×160 | 1296 | demonstrative |
| custom-control | Custom Control | n/a (unsupported) | n/a | n/a | n/a | unsupported |

## Reconciled summary (FR-010, SC-005)

- **rendered (demonstrative) = 51**
- **unsupported (honest no-image) = 1**
- **reconciled: 51 + 1 == 52 supported controls** (no silent omission)

Smallest demonstrative preview: 486 bytes (icon-button); largest: 2230 bytes (data-grid);
near-empty 320x160 baseline: ~363 bytes; pinned trivial-content floor `T` = 420 bytes.
Cross-checked by `ControlsCatalogDocsCheck` (PASS) - see `controls-catalog-docs.md`.
