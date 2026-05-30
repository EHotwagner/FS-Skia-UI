# Advisory Capability Guidance

Command:

`dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`

Result:

- PASS: 50 tests passed, 0 failed.
- Guidance proves FS.Skia.UI hints are non-blocking advisory text.

Categories covered:

- rendering / scene: `fs-skia-scene`
- viewer / window host: `fs-skia-skiaviewer`
- Elmish workflow: `fs-skia-elmish`
- keyboard / input: `fs-skia-keyboard-input`
- layout: `fs-skia-layout`
- controls / forms / charts / graphs / DataGrid: `fs-skia-ui-widgets`
- generated game HUD readability, host update, host-warning classification, and evidence: `fs-skia-layout-evidence`
