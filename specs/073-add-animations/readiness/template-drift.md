# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Elmish/Elmish.fsproj` | `source-code` |
| `src/Scene/Scene.fsproj` | `source-code` |
| `tests/Elmish.Tests/Elmish.Tests.fsproj` | `test-code` |
| `tests/Parity.Tests/Parity.Tests.fsproj` | `test-code` |
| `tests/Scene.Tests/Scene.Tests.fsproj` | `test-code` |
| `src/Elmish/AnimationTick.fs` | `source-code` |
| `src/Elmish/AnimationTick.fsi` | `source-code` |
| `src/Scene/Animation.fs` | `source-code` |
| `src/Scene/Animation.fsi` | `source-code` |
| `tests/Elmish.Tests/AnimationTickTests.fs` | `test-code` |
| `tests/Parity.Tests/AnimationOutput.fs` | `test-code` |
| `tests/Parity.Tests/AnimationOutputTests.fs` | `test-code` |
| `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-mid.txt` | `test-code` |
| `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-settled.txt` | `test-code` |
| `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-start.txt` | `test-code` |
| `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-mid.txt` | `test-code` |
| `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-settled.txt` | `test-code` |
| `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-start.txt` | `test-code` |
| `tests/Scene.Tests/AnimationTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Elmish/Elmish.fsproj` requires `source-contract`
- `src/Elmish/Elmish.fsproj` requires `active-feature-evidence`
- `src/Scene/Scene.fsproj` requires `source-contract`
- `src/Scene/Scene.fsproj` requires `active-feature-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `test-evidence`
- `tests/Elmish.Tests/Elmish.Tests.fsproj` requires `active-feature-evidence`
- `tests/Parity.Tests/Parity.Tests.fsproj` requires `test-evidence`
- `tests/Parity.Tests/Parity.Tests.fsproj` requires `active-feature-evidence`
- `tests/Scene.Tests/Scene.Tests.fsproj` requires `test-evidence`
- `tests/Scene.Tests/Scene.Tests.fsproj` requires `active-feature-evidence`
- `src/Elmish/AnimationTick.fs` requires `source-contract`
- `src/Elmish/AnimationTick.fs` requires `active-feature-evidence`
- `src/Elmish/AnimationTick.fsi` requires `source-contract`
- `src/Elmish/AnimationTick.fsi` requires `active-feature-evidence`
- `src/Scene/Animation.fs` requires `source-contract`
- `src/Scene/Animation.fs` requires `active-feature-evidence`
- `src/Scene/Animation.fsi` requires `source-contract`
- `src/Scene/Animation.fsi` requires `active-feature-evidence`
- `tests/Elmish.Tests/AnimationTickTests.fs` requires `test-evidence`
- `tests/Elmish.Tests/AnimationTickTests.fs` requires `active-feature-evidence`
- `tests/Parity.Tests/AnimationOutput.fs` requires `test-evidence`
- `tests/Parity.Tests/AnimationOutput.fs` requires `active-feature-evidence`
- `tests/Parity.Tests/AnimationOutputTests.fs` requires `test-evidence`
- `tests/Parity.Tests/AnimationOutputTests.fs` requires `active-feature-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-mid.txt` requires `test-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-mid.txt` requires `active-feature-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-settled.txt` requires `test-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-settled.txt` requires `active-feature-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-start.txt` requires `test-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-start.txt` requires `active-feature-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-mid.txt` requires `test-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-mid.txt` requires `active-feature-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-settled.txt` requires `test-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-settled.txt` requires `active-feature-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-start.txt` requires `test-evidence`
- `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-glide-start.txt` requires `active-feature-evidence`
- `tests/Scene.Tests/AnimationTests.fs` requires `test-evidence`
- `tests/Scene.Tests/AnimationTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/073-add-animations`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
