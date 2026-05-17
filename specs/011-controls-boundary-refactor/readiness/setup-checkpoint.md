# Setup Checkpoint

Status: setup checkpoint captured before foundation work.

## Completed Setup Evidence

- Required readiness files have been created under
  `specs/011-controls-boundary-refactor/readiness/`.
- Package, stale-reference, target, dependency, and traceability inventories
  were captured from the current repository.
- The existing task graph was present before implementation and will be
  regenerated after each task status update.

## Open Risks Before Foundation

- No dedicated `src/Controls.Elmish/` package exists yet.
- No `src/Controls/ControlRuntime.*` surface exists yet.
- DataGrid still lives under the legacy Charts package.
- The legacy Charts source project, tests, sample, script, and surface baseline
  are still present.
- Controls currently references `src/Lib/Lib.fsproj`, which conflicts with the
  target boundary unless justified or removed.
- Some generated product package expectations still mention `FS.Skia.UI.Charts`.

## Next Phase

Foundation begins with failing contract, package-boundary, KeyboardInput,
ControlRuntime, adapter, and generated-guidance tests.
