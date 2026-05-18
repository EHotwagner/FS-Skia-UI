# Headless Scene Evidence Readiness

## Scope

Readiness evidence for deterministic scene-level visual evidence that does not
open a native viewer window and remains separate from bounded real viewer
startup evidence.

## Setup Notes

- Tier: Tier 1 contracted scene/testing evidence change.
- Affected areas: `src/Scene/`, `src/Testing/`, generated product validation,
  and readiness output.
- Public contract impact: `.fsi` signatures must cover scene evidence requests,
  results, renderer mode, output format, evidence path/value, and
  unsupported-environment failures.
- Synthetic policy: deterministic non-window scenes may be synthetic fixtures
  when disclosed, but readiness must distinguish deterministic scene evidence
  from real viewer startup evidence.

## Evidence

- Focused public Scene evidence tests:
  `readiness/logs/scene-us4-evidence-tests.txt`
  - Deterministic hash evidence records stable output size, renderer mode, and
    evidence value without opening a viewer window.
  - Metadata evidence writes to a real filesystem path and includes
    `size=128x96`, capability count, and deterministic hash.
  - PNG evidence helper returns stable bytes for the same generated app scene.
  - Unsupported renderer capability returns a structured
    `UnsupportedEnvironment` failure naming `renderer` as the blocked stage.
- Generated product validation scan:
  `readiness/logs/generated-product-us4-scene-evidence.txt`
  - Confirms generated app template exposes `--scene-evidence`.
  - Confirms generated app template uses public `SceneEvidence.render`.
  - Confirms generated scene evidence uses `RendererMode = "deterministic-scene"`
    and writes `readiness/headless-scene-evidence.txt`.

## Independent Validation

Run:

```bash
dotnet run --project tests/Scene.Tests/Scene.Tests.fsproj
./fake.sh build -t GeneratedProductCheck
```

Scene evidence remains separate from bounded real viewer startup evidence:
bounded smoke lives under the SkiaViewer path and may report unsupported desktop
hosts, while scene evidence uses deterministic scene-level rendering and a
non-window readiness path.

## Requirement Mapping

- FR-013: public `SceneEvidence.render`, `renderHash`, and `renderPng` provide
  official deterministic non-window visual evidence.
- FR-014: unsupported renderer capability returns explicit
  `UnsupportedEnvironment` diagnostics.
- FR-014a: renderer mode is recorded as `deterministic-scene`, separate from
  the live viewer renderer.
- FR-019: evidence names renderer mode, blocked stage, diagnostic category, and
  evidence path.
- SC-007: generated product scan confirms generated apps expose deterministic
  scene evidence through `--scene-evidence`.
