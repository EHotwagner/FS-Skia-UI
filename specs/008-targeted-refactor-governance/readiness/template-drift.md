# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.template.config/template.json` | `template-manifest` |
| `README.md` | `documentation` |
| `build.fsx` | `command-surface` |
| `docs/template-profile.md` | `documentation` |
| `tests/Governance.Tests/TemplateProfileTests.fs` | `test-code` |

## Required Alignment Classes

- `.template.config/template.json` requires `template-profile`
- `.template.config/template.json` requires `active-feature-evidence`
- `README.md` requires `docs-alignment`
- `README.md` requires `active-feature-evidence`
- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/template-profile.md` requires `docs-alignment`
- `docs/template-profile.md` requires `active-feature-evidence`
- `tests/Governance.Tests/TemplateProfileTests.fs` requires `test-evidence`
- `tests/Governance.Tests/TemplateProfileTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, template-profile, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/008-targeted-refactor-governance`

## Diagnostics

- No drift blockers.
