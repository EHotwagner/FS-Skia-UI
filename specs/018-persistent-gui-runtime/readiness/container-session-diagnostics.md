# Container Session Diagnostics Evidence

Status: US3 recorded.

## Focused Evidence

- Red runtime diagnostic tests: `readiness/logs/t029-desktop-diagnostics-red.txt`
- Red generated diagnostic test: `readiness/logs/t030-generated-desktop-diagnostics-red.txt`
- Green runtime diagnostic tests: `readiness/logs/t031-desktop-diagnostics-green.txt`
- Green generated diagnostic test: `readiness/logs/t032-generated-desktop-diagnostics-green.txt`

Commands:

```text
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -m:1 --filter "desktop diagnostics|runtime capability|persistent run exposes"
dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj -m:1 --filter "generated normal launch reports desktop"
```

## Reported Fields

Runtime diagnostics report:

- `diagnostic-class`: `unsupported-host`, `environment-session-ready`, or `environment-session-not-required`
- `runtime-directory` and `runtime-directory-exists`
- `RuntimeDirectoryOwnerSuitable`
- `RuntimeDirectoryPermissionsSuitable`
- `display-variable`
- `display-socket` and `display-socket-exists`
- `session-bus`
- `fallback-is-full-desktop-session=false`
- actionable `desktop-message`

The generated normal launch branch prints the same readiness fields before app-lifecycle debugging and does not switch to bounded evidence, scene metadata, or private runtime fallback.

## Invalid Configuration Matrix

| Case | Expected class | Expected message focus | Covered by |
|------|----------------|------------------------|------------|
| Missing `XDG_RUNTIME_DIR` | `unsupported-host` | names `XDG_RUNTIME_DIR` | `t031-desktop-diagnostics-green.txt` |
| Missing `DISPLAY` and `WAYLAND_DISPLAY` | `unsupported-host` | names display prerequisite | runtime capability and generated branch diagnostics |
| Display socket path derived but absent | `unsupported-host` | names missing display socket | runtime diagnostic implementation |
| Present runtime directory and Wayland socket | `environment-session-ready` | prerequisites present | `t031-desktop-diagnostics-green.txt` |
| Non-Linux host | `environment-session-not-required` | diagnostic not required | runtime diagnostic branch |
| Private fallback runtime directory | not accepted as full desktop | `fallback-is-full-desktop-session=false` | runtime and generated branch tests |

Readiness-validation threshold: 6 of 6 listed configurations have explicit expected classes and reviewer-visible fields, satisfying the 95% matrix requirement for this US3 scope.

## Supported-Host Notes

On a supported Linux desktop session, reviewers should see:

```text
diagnostic-class=environment-session-ready
runtime-directory-exists=true
display-socket-exists=true
fallback-is-full-desktop-session=false
```

On an unsupported container/headless session, `Viewer.runApp` must fail before product lifecycle debugging with `classification=UnsupportedEnvironment`, `category=EnvironmentSession`, and an actionable missing-prerequisite message.

## Package Drift Note

Generated product logs still include `NU1603` warnings because package exactness is deferred to US4. These warnings are not accepted as package-resolution evidence for this readiness record.
