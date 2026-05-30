# Focused Governance Tests

Command:

`dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`

Result:

```text
Passed! - Failed: 0, Passed: 50, Skipped: 0, Total: 50
```

Covered behavior:

- Token-aware title matching.
- Filename-context exclusions.
- Whole-word positive trigger fixtures.
- Registry mismatch diagnostics.
- Existing graph protection source checks.
- Guidance trigger coverage.
- Advisory FS.Skia.UI capability hints.
- Graph-only success and failure output labels.
