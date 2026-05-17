# US1 Independent Validation

Status: US1 validation path captured after T035 readiness evidence.

## Goal

Product developers can declare ordinary Controls records, rich text, custom
Skia rendering hooks, product-owned `ControlRuntime` state, product-owned
`FS.Skia.UI.KeyboardInput` state, and optional Controls.Elmish adapter wiring
through public package surfaces.

## Independent Validation Path

Run the following commands from the repository root:

| Step | Command | Evidence log |
|------|---------|--------------|
| Restore affected boundary projects | `dotnet restore src/Controls.Elmish/Controls.Elmish.fsproj` | `readiness/logs/t033-controls-elmish-restore.txt` |
| Build Controls without `src/Lib` coupling | `dotnet build src/Controls/Controls.fsproj -m:1 --no-restore` | `readiness/logs/t033-controls-build-final.txt` |
| Build adapter boundary | `dotnet build src/Controls.Elmish/Controls.Elmish.fsproj -m:1 --no-restore` | `readiness/logs/t033-controls-elmish-build-after-restore.txt` |
| Exercise Controls public FSI | `dotnet fsi scripts/controls-prelude.fsx` | `readiness/logs/t033-controls-fsi.txt` |
| Exercise KeyboardInput public FSI | `dotnet fsi scripts/keyboardinput-package-prelude.fsx` | `readiness/logs/t033-keyboardinput-fsi.txt` |
| Exercise adapter public FSI | `dotnet fsi scripts/controls-elmish-prelude.fsx` | `readiness/logs/t033-controls-elmish-fsi.txt` |
| Build Controls sample | `dotnet build samples/ControlsGallery/ControlsGallery.fsproj -m:1 --no-restore` | `readiness/logs/t034-controlsgallery-build.txt` |
| Smoke Controls sample | `dotnet run --no-build --project samples/ControlsGallery/ControlsGallery.fsproj -- --contract-smoke` | `readiness/logs/t034-controlsgallery-contract-smoke.txt` |
| Build KeyboardInput sample | `dotnet build samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -m:1 --no-restore` | `readiness/logs/t034-keyboardinputgallery-build.txt` |
| Smoke KeyboardInput sample | `dotnet run --no-build --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke` | `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt` |

Passing evidence shows:

- Controls stable records render and dispatch product messages through public
  `Control<'msg>` declarations.
- RichText measurement/rendering and custom Skia hooks are reachable from the
  Controls sample.
- `ControlRuntimeModel` and `KeyboardModel` are stored in product sample
  models, not hidden in Controls.
- KeyboardInput effects and ControlRuntime effects are interpreted through
  `FS.Skia.UI.Controls.Elmish`.
- Controls and Layout build through `FS.Skia.UI.Scene` rather than direct
  `src/Lib` coupling.

## Unsupported Or Deferred Conditions

- Native GPU/window smoke is not part of US1 evidence. The maintained US1
  sample evidence is contract smoke and FSI output; native window verification
  remains an Integration & Polish concern.
- `./fake.sh build -t EvidenceGraph` and other FAKE targets are currently
  blocked by the local FAKE package cache missing `FSharp.Core/6.0.7`; direct
  evidence graph script runs were used after task status updates.
- A full `tests/Smoke.Tests` run hung inside its nested `dotnet run` call.
  Direct `ControlsGallery` and `KeyboardInputGallery` contract-smoke commands
  completed and are the US1 sample evidence.
- Charts package removal, generated-product guidance, and full package-surface
  cleanup are later US2/US3/US4 work and are not required for the US1
  independent validation path.
