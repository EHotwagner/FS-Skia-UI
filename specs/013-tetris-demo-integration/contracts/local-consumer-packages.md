# Contract: Local Consumer Packages

## Command Output

Provide one documented workflow or command that emits:

- Local NuGet feed path, normally `~/.local/share/nuget-local/`.
- Package identities required by generated graphical consumers.
- Package versions expected by the generated consumer.
- `Directory.Packages.props` or package configuration snippet.
- Optional `nuget.config` snippet when needed.
- Restore command for the generated consumer.
- Drift diagnostics for missing or stale feed contents.

## Required Behavior

- Missing package output is setup drift, not app source failure.
- Stale package versions are reported before generated consumer build or
  rendering failures are attributed to app code.
- Package identities remain stable unless a separately planned package change
  says otherwise.

## Evidence

- Command or workflow transcript with feed path, package identities, versions,
  snippet, and restore command.
- Drift test or fixture for stale/missing local feed contents.
- Readiness: `readiness/local-consumer-packages.md`.
