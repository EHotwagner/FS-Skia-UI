# Elmish Adapter Evidence

Status: setup placeholder, awaiting foundation and US1 implementation.

## Required Evidence

- Public adapter contract file for `FS.Skia.UI.Controls.Elmish`.
- Tests proving base Controls stays generic over product messages.
- Tests proving adapter-owned command, subscription, and program helpers translate keyboard and control effects.
- Readiness logs from adapter tests and FSI transcript paths.

## Setup Observation

The repository currently has `src/Elmish/Elmish.fsproj` for viewer-level Elmish
integration. The dedicated Controls Elmish adapter package required by this
feature is not present yet.

## Red Test Evidence

- `readiness/logs/t012-elmish-adapter-red.txt`: fails on the missing
  `src/Controls.Elmish/` package and adapter contract.

## Foundation Evidence

- `readiness/logs/t016-controls-elmish-contracts.txt`: adapter contract tests
  pass for command/subscription/program ownership outside base Controls.
- `readiness/logs/t032-adapter-runtime.txt`: stale control-target effects map
  to adapter diagnostics.

## US1 T033 Evidence

- `readiness/logs/t033-controls-elmish-build-after-restore.txt`: the adapter
  builds with direct dependencies on Controls, KeyboardInput, and Fable.Elmish,
  while Controls itself stays free of Elmish `Cmd` and host-loop references.
- `readiness/logs/t033-controls-elmish-fsi.txt`: public adapter FSI exercises
  keyboard-effect and control-effect interpretation through the dedicated
  Controls.Elmish surface.
- `readiness/logs/t033-elmish-tests.txt`: adapter tests pass after the
  Controls/Layout scene boundary is moved to the standalone Scene package.

## US1 Readiness Capture

| Evidence | Path | Verdict |
|----------|------|---------|
| Adapter build after dependency restore | `readiness/logs/t033-controls-elmish-build-after-restore.txt` | PASS |
| Adapter contract tests | `readiness/logs/t033-elmish-tests.txt` | PASS |
| Adapter FSI public program/effect interpretation | `readiness/logs/t033-controls-elmish-fsi.txt` | PASS |
| ControlsGallery adapter sample | `readiness/logs/t034-controlsgallery-contract-smoke.txt` | PASS |
| KeyboardInputGallery adapter sample | `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt` | PASS |

Base Controls records remain generic over product messages. Direct command,
subscription, and program wiring is demonstrated through
`FS.Skia.UI.Controls.Elmish`.

## T075 Adapter Check

| Evidence | Path | Verdict |
|----------|------|---------|
| Feature-specific interaction target | `readiness/logs/t075-controls-interaction-check.txt` | PASS |
| Command target governance contract | `readiness/logs/t075-command-contract-tests.txt` | PASS |
| Aggregate `Verify` attempt | `readiness/logs/t075-verify.txt` | ENVIRONMENT FAIL |

`ControlsBoundaryCheck` is not a declared split target in the current target
graph. The available boundary-specific target is `ControlsInteractionCheck`,
which passed and covers runtime/adapter interaction evidence. The focused
command-contract governance test also passed after aligning the expected `Test`
target dependency with the current graph.
