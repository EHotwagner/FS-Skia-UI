# Host Warning Classification

Feature: `021-persistent-launch-evidence`
Updated UTC: `2026-05-27T15:07:11Z`

## Real Launch Facts

- evidence-path: `specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md`
- status: `ok`
- mode: `persistent-evidence`
- window-opened: `true`
- first-frame-presented: `true`
- input-dispatch: `not-required`
- exit-path: `true`
- controlled-close: `self-closed-for-evidence=true`

## Classification Evidence

- verifier: `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --filter "FullyQualifiedName~host warning classification"`
- verifier-log: `specs/021-persistent-launch-evidence/readiness/logs/t028-host-warning-tests.txt`
- known benign marker: `Failed to load plugin 'libdecor-gtk.so'`
- benign result: `warning-class=BenignEnvironmentWarning`
- benign fatal flag: `false`
- required supporting facts: launch succeeded, rendering succeeded, layout readable or explicitly unsupported without readability claim, package succeeded

Known GTK/module warning text is preserved as non-blocking only when the real
launch, first-frame/render, controlled exit, layout/package supporting facts
pass. The classifier keeps the raw warning text, warning class, fatal flag,
evidence path, supporting facts, and diagnostics.

## Fatal Preservation

The same verifier asserts these failure classes remain fatal even when benign
warning text is present:

- `LaunchFailure`
- `RenderingFailure`
- `LayoutFailure`
- `PackageFailure`

Artifact-write failures are covered by the persistent-launch artifact validator:
missing or contradictory artifact fields are rejected and do not become benign
environment warnings.
