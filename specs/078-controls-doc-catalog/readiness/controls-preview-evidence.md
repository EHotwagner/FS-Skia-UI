# Controls Preview Evidence (078) — per-control render honesty

- status: PASS — all 52 controls rendered through the deterministic render-only path
- authoritative-command: `Testing.readPngArtifact` validation + the
  `ControlsCatalogDocsCheck` preview clause (PNG decodable / non-1×1 / non-trivial)
- artifact-path: `specs/078-controls-doc-catalog/readiness/controls-preview-evidence.md`
- failure-class: none

## How the previews were produced (real, render-only)

For each of the 52 controls: the control was built through the typed front door
(`FS.Skia.UI.Controls.Typed.<Control>` `defaults`/`view`, with stateful controls
initialised via `init`), lowered with `Widget.toControl`, rendered with
`Control.render Theme.light`, wrapped as `SceneNode.Group [ result.Scene ]`, and
rasterised by `SkiaViewer.captureScreenshotEvidence` with
`CaptureMode = ViewerRenderTargetPng` (every result `status=ScreenshotOk`). The PNG
was committed to `docs/img/controls/<id>.png` and embedded on the detail page.

This is **render-only** evidence — off-window raster output of the real control
scene, not a desktop-window screenshot and not a fabricated image. Each PNG is
320×160, 8-bit RGBA, decodable, non-1×1, with non-trivial content (the control
rendered against the `DesignTokens.Light` theme). No 1×1, metadata-only, or
placeholder image is committed. The `ControlsCatalogDocsCheck` gate revalidates
decodability / dimensions / non-trivial size on every run
(`previews-present: 52`), and the strict `fsdocs` site build copies all 52 into
`output/img/controls/` with resolving `<img>` links.

## Per-control record

| Control id | Display name | Mode | Decodable | Dimensions | Bytes | Classification |
|------------|--------------|------|-----------|------------|-------|----------------|
| text-block | Text Block | render-only (ViewerRenderTargetPng) | yes | 320×160 | 678 | real-render |
| rich-text | Rich Text | render-only (ViewerRenderTargetPng) | yes | 320×160 | 687 | real-render |
| label | Label | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| image | Image | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| icon | Icon | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| separator | Separator | render-only (ViewerRenderTargetPng) | yes | 320×160 | 679 | real-render |
| badge | Badge | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| button | Button | render-only (ViewerRenderTargetPng) | yes | 320×160 | 604 | real-render |
| icon-button | Icon Button | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| text-box | Text Box | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| text-area | Text Area | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| numeric-input | Numeric Input | render-only (ViewerRenderTargetPng) | yes | 320×160 | 505 | real-render |
| check-box | Check Box | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| radio-group | Radio Group | render-only (ViewerRenderTargetPng) | yes | 320×160 | 731 | real-render |
| switch | Switch | render-only (ViewerRenderTargetPng) | yes | 320×160 | 625 | real-render |
| slider | Slider | render-only (ViewerRenderTargetPng) | yes | 320×160 | 505 | real-render |
| list-view | List View | render-only (ViewerRenderTargetPng) | yes | 320×160 | 678 | real-render |
| list-box | List Box | render-only (ViewerRenderTargetPng) | yes | 320×160 | 657 | real-render |
| multi-select-list | Multi Select List | render-only (ViewerRenderTargetPng) | yes | 320×160 | 830 | real-render |
| combo-box | Combo Box | render-only (ViewerRenderTargetPng) | yes | 320×160 | 670 | real-render |
| tree-view | Tree View | render-only (ViewerRenderTargetPng) | yes | 320×160 | 690 | real-render |
| data-grid | Data Grid | render-only (ViewerRenderTargetPng) | yes | 320×160 | 1138 | real-render |
| stack | Stack | render-only (ViewerRenderTargetPng) | yes | 320×160 | 604 | real-render |
| grid | Grid | render-only (ViewerRenderTargetPng) | yes | 320×160 | 584 | real-render |
| dock | Dock | render-only (ViewerRenderTargetPng) | yes | 320×160 | 525 | real-render |
| wrap | Wrap | render-only (ViewerRenderTargetPng) | yes | 320×160 | 555 | real-render |
| border | Border | render-only (ViewerRenderTargetPng) | yes | 320×160 | 803 | real-render |
| panel | Panel | render-only (ViewerRenderTargetPng) | yes | 320×160 | 608 | real-render |
| scroll-viewer | Scroll Viewer | render-only (ViewerRenderTargetPng) | yes | 320×160 | 980 | real-render |
| split-view | Split View | render-only (ViewerRenderTargetPng) | yes | 320×160 | 711 | real-render |
| tabs | Tabs | render-only (ViewerRenderTargetPng) | yes | 320×160 | 544 | real-render |
| menu | Menu | render-only (ViewerRenderTargetPng) | yes | 320×160 | 595 | real-render |
| context-menu | Context Menu | render-only (ViewerRenderTargetPng) | yes | 320×160 | 806 | real-render |
| toolbar | Toolbar | render-only (ViewerRenderTargetPng) | yes | 320×160 | 609 | real-render |
| tooltip | Tooltip | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| dialog | Dialog | render-only (ViewerRenderTargetPng) | yes | 320×160 | 603 | real-render |
| toast | Toast | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| overlay | Overlay | render-only (ViewerRenderTargetPng) | yes | 320×160 | 863 | real-render |
| progress-bar | Progress Bar | render-only (ViewerRenderTargetPng) | yes | 320×160 | 505 | real-render |
| spinner | Spinner | render-only (ViewerRenderTargetPng) | yes | 320×160 | 670 | real-render |
| validation-message | Validation Message | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| line-chart | Line Chart | render-only (ViewerRenderTargetPng) | yes | 320×160 | 723 | real-render |
| bar-chart | Bar Chart | render-only (ViewerRenderTargetPng) | yes | 320×160 | 674 | real-render |
| pie-chart | Pie Chart | render-only (ViewerRenderTargetPng) | yes | 320×160 | 667 | real-render |
| scatter-plot | Scatter Plot | render-only (ViewerRenderTargetPng) | yes | 320×160 | 713 | real-render |
| graph-view | Graph View | render-only (ViewerRenderTargetPng) | yes | 320×160 | 745 | real-render |
| toggle-button | Toggle Button | render-only (ViewerRenderTargetPng) | yes | 320×160 | 363 | real-render |
| split-button | Split Button | render-only (ViewerRenderTargetPng) | yes | 320×160 | 1447 | real-render |
| date-picker | Date Picker | render-only (ViewerRenderTargetPng) | yes | 320×160 | 1648 | real-render |
| time-picker | Time Picker | render-only (ViewerRenderTargetPng) | yes | 320×160 | 887 | real-render |
| color-picker | Color Picker | render-only (ViewerRenderTargetPng) | yes | 320×160 | 555 | real-render |
| custom-control | Custom Control | render-only (ViewerRenderTargetPng) | yes | 320×160 | 634 | real-render |
