# Existing Evidence Command Inventory

Status: setup inventory recorded from repository sources.

Existing evidence kinds that must remain separate from screenshot proof:

| Evidence kind | Known command or path | Current purpose | Screenshot substitute? |
|---------------|-----------------------|-----------------|------------------------|
| Bounded smoke | generated `--bounded-smoke` and `--bounded-smoke-frame-diagnostics` paths referenced by `build.fsx` generated validation | Bounded viewer launch/render diagnostics | No |
| Persistent launch | generated persistent viewer host validation and `persistent-launch-evidence.txt` handling in `build.fsx` | Proves persistent interactive launch behavior on supported hosts | No |
| Deterministic scene | generated `--scene-evidence` command and `SceneEvidence.render` usage in generated products | Headless deterministic scene report | No |
| Layout/readability | existing layout evidence readiness such as `layout-rendering.md` and related controls rendering checks | Structural layout and rendering diagnostics | No |
| Image evidence | generated consumer `game-image-evidence.png` path referenced in `build.fsx` | Existing generated consumer image output used by validation | Not accepted unless replaced or validated through the new screenshot record contract |

Implementation must keep the new screenshot evidence workflow distinct from the
default interactive launch path and from all structural, launch, scene, layout,
or metadata-only evidence classes.
