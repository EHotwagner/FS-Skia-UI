# Foundation Checkpoint

Status: foundation complete.

## Completed Evidence

- Public-surface red tests were added and logged in `readiness/logs/t008-public-surface-red.txt`.
- Package-boundary red tests were added and logged in `readiness/logs/t009-package-boundary-red.txt`.
- KeyboardInput red tests were added and logged in `readiness/logs/t010-keyboardinput-red.txt`.
- ControlRuntime red tests were added and logged in `readiness/logs/t011-controlruntime-red.txt`.
- Controls.Elmish red tests were added and logged in `readiness/logs/t012-elmish-adapter-red.txt`.
- Generated-guidance red tests were added and logged in `readiness/logs/t013-generated-guidance-red.txt`.
- Controls, KeyboardInput, Controls.Elmish, catalog, surface baseline, and FSI evidence are recorded in:
  - `readiness/logs/t014-controls-contracts.txt`
  - `readiness/logs/t015-keyboardinput-contracts.txt`
  - `readiness/logs/t016-controls-elmish-contracts.txt`
  - `readiness/logs/t017-catalog-contracts.txt`
  - `readiness/logs/t018-package-surface-check.txt`
  - `readiness/logs/t019-build-wiring-package-tests.txt`
  - `readiness/logs/t020-boundary-diagnostics.txt`
  - `readiness/logs/t021-controls-runtime-fsi.txt`
  - `readiness/logs/t021-keyboardinput-fsi.txt`
  - `readiness/logs/t021-controls-elmish-fsi.txt`

## Implemented Foundation Surface

- Added `src/Controls/ControlRuntime.*`, `RichText.*`, and `DataGrid.*`.
- Expanded `src/KeyboardInput/KeyboardInput.*` with runtime state, messages,
  effects, diagnostics, and state display.
- Added `src/Controls.Elmish/` as the dedicated adapter package.
- Refreshed Controls, KeyboardInput, and Controls.Elmish surface baselines.
- Updated Controls catalog metadata for `rich-text` and Controls-owned
  `DataGrid`.

## Open Story Risks

- Legacy `src/Charts/` source, tests, sample, and baseline still exist.
- `src/Controls/Controls.fsproj` still references `src/Lib/Lib.fsproj`.
- Generated guidance still lacks the final Skia-rendered Controls path and
  adapter wording.
- User-story work still needs packed-package or user-facing evidence before
  `[US*]` tasks can be marked `[X]`.

## Unsupported Scope

Renderer-neutral controls, new renderer backends, platform-native wrappers,
formal accessibility certification, automated external-app migration, and
release publishing automation remain out of scope.
