# Feature Scope

Task: T002

Tier: Tier 1 contracted framework, generated-template, and governance change.

Affected packages:

- `FS.Skia.UI.Scene`
- `FS.Skia.UI.SkiaViewer`
- `FS.Skia.UI.Testing`
- generated template projects that reference Scene, SkiaViewer, Elmish, and Testing

Generated-template ownership:

- generated source, tests, docs, capability README fragments, template inclusion policy, and validation targets are in scope when viewer launch names, shape examples, report helpers, screenshot evidence, or effect-boundary guidance change

Required real evidence paths:

- `specs/022-breakout-demo-feedback/readiness/generated-viewer-guidance.md`
- `specs/022-breakout-demo-feedback/readiness/scene-shape-evidence.md`
- `specs/022-breakout-demo-feedback/readiness/screenshot-evidence.md`
- `specs/022-breakout-demo-feedback/readiness/effect-boundary-guidance.md`
- `specs/022-breakout-demo-feedback/readiness/evidence-report-conventions.md`

Deferred scope:

- no new game mechanics
- no Breakout demo rebuild
- no guarantee of screenshot capture on hosts that cannot expose it
- no unrelated Controls, chart, graph, or DataGrid work
- no release automation rewrite
- no redefinition of persistent-launch evidence covered by feature 021

