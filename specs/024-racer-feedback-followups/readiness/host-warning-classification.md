# Host Warning Classification

## T022/T023 Red Classifier Tests

Recorded at: 2026-05-28T08:44:00+02:00

Added focused tests for:

- `colorreload-gtk-module`
- `window-decorations-gtk-module`
- first-frame/render success gating
- raw warning preservation
- mixed unrelated warning/error text remaining fatal and visible

Red evidence:

| Command | Expected result | Evidence |
|---------|-----------------|----------|
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"` | FAIL, mixed unrelated warning text is still classified benign | `readiness/logs/t022-t023-testing-tests-red.txt` |

## T024 Classifier Implementation

Recorded at: 2026-05-28T08:46:00+02:00

Implemented:

- exact known GTK marker matching for `colorreload-gtk-module` and
  `window-decorations-gtk-module`
- launch/render/layout/package success gating
- raw warning preservation
- mixed unrelated warning or error text remains `UnknownWarning` and fatal

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --logger "console;verbosity=minimal"` | PASS, 36 tests | `readiness/logs/t024-testing-tests.txt` |

## T025 Real GTK Warning Classification

Recorded at: 2026-05-28T08:34:39+02:00

Real non-synthetic input:

- Source transcript: `specs/021-persistent-launch-evidence/readiness/logs/t040-repeated-launches.txt`
- Current classifier transcript: `readiness/logs/t025-real-gtk-classification.txt`
- Current generated launch attempt: `readiness/logs/t025-real-launch-output.txt`

The source transcript is preserved repository evidence from a real generated
viewer launch. Attempt 01 contains both known GTK module messages and a
successful first-frame launch line:

- `Gtk-Message: ... Failed to load module "colorreload-gtk-module"`
- `Gtk-Message: ... Failed to load module "window-decorations-gtk-module"`
- `status=ok mode=persistent-evidence command=--launch-evidence`
- `first-frame-presented=true`

The current classifier was run against that real transcript via
`readiness/fsi/t025-real-gtk-classification.fsx`.

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet fsi specs/024-racer-feedback-followups/readiness/fsi/t025-real-gtk-classification.fsx` | PASS, `warning-class=BenignEnvironmentWarning`, `fatal=false`, `launch-succeeded=true`, `rendering-succeeded=true`, `layout-readable=true` | `readiness/logs/t025-real-gtk-classification.txt` |

The current feature generated app launch attempt did not produce acceptance
evidence because the existing generated app artifact is stale: template code
expects the new additive `ScreenshotEvidenceResult` fields while its package
references still resolve an older `FS.Skia.UI.SkiaViewer` surface. That failure
is preserved in `readiness/logs/t025-real-launch-output.txt` and is not counted
as successful host-warning evidence.
