# Contract — Consumer Repoint Matrix (FR-006 / SC-005)

Every consumer that referenced the deleted `Lib` host/scene modules MUST build green after repointing.
Consumers that referenced `Lib` **only** for residue (`AgentValidation`, `Parity`) keep a reduced
reference until that residue's own stage.

## Repointed off the deleted modules → `Scene` + `SkiaViewer`

| Project | Before | After | Acceptance |
|---|---|---|---|
| `samples/BasicViewer` | `Lib` | `Scene` + `SkiaViewer` | builds + runs; parity seed source |
| `samples/EffectsGallery` | `Lib` | `Scene` + `SkiaViewer` | builds + runs; parity seed source |
| `samples/ScreenshotGallery` | `Lib` | `Scene` + `SkiaViewer` | builds + runs; parity seed source |
| `samples/InteractiveViewer` | `Lib` | `Scene` + `SkiaViewer` (+ `Elmish` if used) | builds + runs |
| `samples/DemoReel` | `Lib`,`SkiaViewer`,`Layout`,`Controls`,`Elmish` | drop `Lib`; keep rest | builds + runs |
| `tests/Lib.Tests` | `Lib` | `Scene` + `SkiaViewer` | host/scene assertions pass |
| `tests/Smoke.Tests` | `Lib` | `Scene` + `SkiaViewer` | smoke passes |
| `tests/Package.Tests` | `Lib`(+Layout/Controls cond.) | `Scene` + `SkiaViewer` for host/scene; keep others | passes |
| `tests/Parity.Tests` | `Lib` | repointed to drive moved host (+ `Parity` residue) | **retained**; 0-byte parity |

## Reduced reference retained (residue only — not repointed here)

| Project | Keeps `Lib` for | Retires in |
|---|---|---|
| `tests/Governance.Tests` | `AgentValidation` only | Stage 2 (relocation) |
| `samples/ParityGallery` | `Parity` helper only | Stage 4 (bridge retirement) |

## Acceptance (SC-005)

- No project references the **deleted** `Lib` modules (host/duplicate-scene).
- All repointed samples and tests restore/build/run green against the split packages.
- `Parity.Tests` is **retained** as the parity harness (not removed in this stage).
- `TemplateCheck`/`GeneratedProductCheck` confirm the default `app` still restores/builds/runs with the
  monolith absent from its transitive graph.
