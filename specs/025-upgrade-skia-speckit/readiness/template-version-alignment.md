# Template Version Alignment

Evidence:

- Pre-upgrade command: `./fake.sh build -t TemplateCheck`
- Pre-upgrade log: `specs/025-upgrade-skia-speckit/readiness/logs/pre-template-check.log`
- Pre-upgrade exit code: `0`
- Post-edit command: `./fake.sh build -t TemplateCheck`
- Post-edit log: `specs/025-upgrade-skia-speckit/readiness/logs/post-template-check.log`
- Post-edit exit code: `0`
- Generated profile evidence directory: `specs/025-upgrade-skia-speckit/readiness/template/`

## Spec Kit And Template Metadata

| Asset | Selected value | Evidence | Status |
|-------|----------------|----------|--------|
| Spec Kit release metadata | `0.8.16` | `https://api.github.com/repos/github/spec-kit/releases/latest` | recorded |
| Root init options | `0.8.16` | `.specify/init-options.json` | updated |
| Integration metadata | `0.8.16` | `.specify/integration.json`, `.specify/integrations/*.manifest.json` | updated |
| Template package | `0.1.24-preview.1` | `.template.package/FS.Skia.UI.Template.fsproj` | updated |

## Generated Package Pins

The generated `template/base/Directory.Packages.props` remains aligned to the
current repository package posture:

| Package | Generated pin | Repository package version |
|---------|---------------|----------------------------|
| FS.Skia.UI.Scene | `0.1.25-preview.1` | `0.1.25-preview.1` |
| FS.Skia.UI.SkiaViewer | `0.1.25-preview.1` | `0.1.25-preview.1` |
| FS.Skia.UI.Elmish | `0.1.24-preview.1` | `0.1.24-preview.1` |
| FS.Skia.UI.KeyboardInput | `0.1.24-preview.1` | `0.1.24-preview.1` |
| FS.Skia.UI.Layout | `0.1.24-preview.1` | `0.1.24-preview.1` |
| FS.Skia.UI.Controls | `0.1.24-preview.1` | `0.1.24-preview.1` |
| FS.Skia.UI.Controls.Elmish | `0.1.24-preview.1` | `0.1.24-preview.1` |
| FS.Skia.UI.Testing | `0.1.25-preview.1` | `0.1.25-preview.1` |

Checked profiles: `app`, `governed`, `headless-scene`, and `sample-pack`.
broad-package dependency status: focused generated profiles do not add
`FS.Skia.UI` central pins or package references.

Validation commands: `GeneratedGuidanceCheck` and `TemplateCheck` passed before
the edit; post-edit `TemplateCheck` passed with
`FS.Skia.UI.Template.0.1.24-preview.1.nupkg`, package install evidence, and
source/package `Dev` runs for `app`, `governed`, `headless-scene`, and
`sample-pack`.
