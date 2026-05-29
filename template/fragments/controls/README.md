# Controls Fragment

Adds the `FS.Skia.UI.Controls` package reference, Skia-rendered Controls
guidance, product-owned example views, product test coverage, and generated
controls guidance. Generated app skill installation still receives the
source-owned `fs-skia-ui-widgets` skill.

Generated products use one Elmish-style Controls path for ordinary controls,
rich text, chart controls, graph controls, and DataGrid. Product models own
business data and messages; Controls declarations stay generic over
`Control<'msg>`.

Generated products must not copy framework galleries, framework samples,
framework readiness evidence, historical specs, or framework implementation
projects.
