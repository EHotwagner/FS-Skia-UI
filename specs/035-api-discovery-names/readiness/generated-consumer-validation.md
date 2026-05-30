# Generated Consumer Validation Evidence

Status: setup placeholder.

Synthetic red evidence:

- T007 added the design-approved malformed generated-guidance fixture test
  `Generated guidance scanner Synthetic rejects reflection-first package discovery fixtures`.
- The malformed fixture is disclosed with a `SYNTHETIC` code comment at the
  use site in `tests/Governance.Tests/GeneratedGuidanceTests.fs`.
- Focused command:
  `dotnet test tests/Governance.Tests/Governance.Tests.fsproj -m:1 --logger "console;verbosity=minimal"`.
- Expected red log:
  `specs/035-api-discovery-names/readiness/logs/t007-generated-guidance-synthetic-red.txt`.
- Red failure class: GeneratedGuidanceCheck scanner has not yet implemented
  rejection of reflection-first and repository-source-copy authoring advice.

Red evidence:

- T008 added clean package-consumer validation tests in
  `tests/Package.Tests/GeneratedConsumerValidationTests.fs`.
- Focused command:
  `dotnet test tests/Package.Tests/Package.Tests.fsproj -m:1 --logger "console;verbosity=minimal"`.
- Expected red log:
  `specs/035-api-discovery-names/readiness/logs/t008-generated-consumer-red.txt`.
- Red failure class: missing clean package-consumer restore/build artifact
  evidence, package-only consumption proof, and actionable validation
  diagnostics.

consumer-project: `specs/035-api-discovery-names/readiness/package/clean-consumer`
restore-log: `specs/035-api-discovery-names/readiness/logs/generated-consumer-restore.txt`
build-log: `specs/035-api-discovery-names/readiness/logs/generated-consumer-build.txt`
local-package-feed: `~/.local/share/nuget-local`
package-version: `local`
result: pass
project-references: none
copied-src-files: none
repository-source-inspection: false
assembly-reflection-authoring: false
package-references:
- FS.Skia.UI.Scene
- FS.Skia.UI.Controls
- FS.Skia.UI.SkiaViewer
failure-class: restore
failure-class: project-reference
failure-class: copied-src
failure-class: reflection-authoring
failure-class: compile
next-action: rerun `PackLocal` and `GeneratedProductCheck` sequentially before final audit.

Required final contents:

- clean generated/package-consumer project path
- local package restore log path
- build log path
- package references proving no project references or copied `src/` files
- no-reflection and no repository-source authoring confirmation
- diagnostics and next actions for any validation failure class

Next action: populate during package-consumer validation and final integration
tasks.
