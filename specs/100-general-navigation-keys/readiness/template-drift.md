# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/ControlsGallery/Program.fs` | `sample-code` |
| `samples/DemoReel/Program.fs` | `sample-code` |
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls/Accessibility.fs` | `source-code` |
| `src/Controls/Accessibility.fsi` | `source-code` |
| `src/Controls/Focus.fs` | `source-code` |
| `src/Controls/Focus.fsi` | `source-code` |
| `src/Controls/Types.fs` | `source-code` |
| `src/Controls/Types.fsi` | `source-code` |
| `src/Controls/Widgets/Buttons.fs` | `source-code` |
| `src/Controls/Widgets/Pickers.fs` | `source-code` |
| `tests/Controls.Tests/AccessibilityTests.fs` | `test-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature094FocusTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature095SlotCompositionTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature098UnifiedSchemeTests.fs` | `test-code` |
| `tests/Controls.Tests/InteractionTests.fs` | `test-code` |
| `tests/Controls.Tests/ReconcileTests.fs` | `test-code` |
| `tests/Controls.Tests/TypedExpansionTests.fs` | `test-code` |
| `tests/Controls.Tests/TypedLoweringTests.fs` | `test-code` |
| `tests/Controls.Tests/TypedMigrationTests.fs` | `test-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Elmish.Tests/TypedControlsAdapterTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature100NavigationTests.fs` | `test-code` |
| `tests/Elmish.Tests/Feature100NavigationTests.fs` | `test-code` |

## Required Alignment Classes

- `samples/ControlsGallery/Program.fs` requires `sample-contract`
- `samples/ControlsGallery/Program.fs` requires `active-feature-evidence`
- `samples/DemoReel/Program.fs` requires `sample-contract`
- `samples/DemoReel/Program.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls/Accessibility.fs` requires `source-contract`
- `src/Controls/Accessibility.fs` requires `active-feature-evidence`
- `src/Controls/Accessibility.fsi` requires `source-contract`
- `src/Controls/Accessibility.fsi` requires `active-feature-evidence`
- `src/Controls/Focus.fs` requires `source-contract`
- `src/Controls/Focus.fs` requires `active-feature-evidence`
- `src/Controls/Focus.fsi` requires `source-contract`
- `src/Controls/Focus.fsi` requires `active-feature-evidence`
- `src/Controls/Types.fs` requires `source-contract`
- `src/Controls/Types.fs` requires `active-feature-evidence`
- `src/Controls/Types.fsi` requires `source-contract`
- `src/Controls/Types.fsi` requires `active-feature-evidence`
- `src/Controls/Widgets/Buttons.fs` requires `source-contract`
- `src/Controls/Widgets/Buttons.fs` requires `active-feature-evidence`
- `src/Controls/Widgets/Pickers.fs` requires `source-contract`
- `src/Controls/Widgets/Pickers.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/AccessibilityTests.fs` requires `test-evidence`
- `tests/Controls.Tests/AccessibilityTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature094FocusTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature094FocusTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature095SlotCompositionTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature095SlotCompositionTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature098UnifiedSchemeTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature098UnifiedSchemeTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/InteractionTests.fs` requires `test-evidence`
- `tests/Controls.Tests/InteractionTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/ReconcileTests.fs` requires `test-evidence`
- `tests/Controls.Tests/ReconcileTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedExpansionTests.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedExpansionTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedLoweringTests.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedLoweringTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedMigrationTests.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedMigrationTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/TypedControlsAdapterTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/TypedControlsAdapterTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature100NavigationTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature100NavigationTests.fs` requires `active-feature-evidence`
- `tests/Elmish.Tests/Feature100NavigationTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/Feature100NavigationTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/100-general-navigation-keys`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
