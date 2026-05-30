# Controls Fragment

Adds the `FS.Skia.UI.Controls` package reference, Skia-rendered Controls
guidance, product-owned example views, product test coverage, and generated
controls guidance. Generated app skill installation still receives the
source-owned `fs-skia-ui-widgets` skill.

Generated products use one Elmish-style Controls path for ordinary controls,
rich text, chart controls, graph controls, and DataGrid. Product models own
business data and messages; Controls declarations stay generic over
`Control<'msg>`.

When Controls are authored beside Scene primitives, generated examples should
fully qualify collision-prone names. Use `FS.Skia.UI.Scene.Rect`,
`FS.Skia.UI.Scene.Paint`, and `FS.Skia.UI.Scene.TextRun` for scene records, and
use Controls front doors such as `FS.Skia.UI.Controls.TextBlock.create`,
`FS.Skia.UI.Controls.TextBox.onChanged`, and
`FS.Skia.UI.Controls.Stack.children` for controls. Do not rely on namespace
open order to choose between overlapping names.

Generated products must not copy framework galleries, framework samples,
framework readiness evidence, historical specs, or framework implementation
projects.
