# Interactive Window Close Evidence

- Command: `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` and generated guidance checks.
- Mode: `interactive-window` for default persistent launch; explicit evidence helper mode remains separate as `persistent-evidence`.
- Window opened: validated by existing SkiaViewer launch outcome fields and persistent artifact validation.
- First frame: required by `PersistentLaunchArtifactValidation` and existing SkiaViewer run evidence.
- Input dispatch: `Viewer.dispatchKey` and generated host `OnKey` stay at the host boundary.
- Close request source: `ViewerMsg.AppCloseRequested`, `UserCloseObserved`, and `EvidenceCloseRequested` emit `CloseWindow`; generated app guidance qualifies `Product.Program.Msg.CloseRequested` as app-owned.
- Exit path: accepted persistent evidence requires `exit-path=true`, `blocked-stage=none`, clean close reason, and elapsed under the configured threshold.
- Failure classification: bounded helpers, first-frame-only runs, metadata-only runs, unsupported-host-only records, and evidence self-close are rejected as substitutes for supported-host persistent launch evidence.
- Supported-host classification: if the current host cannot open an accessible window, readiness must record unsupported-host facts rather than reporting success.
- Aggregate hang diagnostics: FAKE and generated product commands are serialized; race-like failures require sequential rerun before product debugging.

