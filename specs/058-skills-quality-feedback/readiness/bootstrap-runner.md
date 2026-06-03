# Bootstrap Runner Evidence

- target: `Verify`
- timestamp-utc: `2026-06-03T18:47:52.2686378+00:00`
- dotnet-sdk-status: `pass`
- fake-tool-restore-status: `pass`
- package-cache-status: `failed: FAKE runner did not start after tool restore: Possible reasons for this include:
  * You misspelled a built-in dotnet command.
  * You intended to execute a .NET program, but dotnet-fake does not exist.
  * You intended to run a global tool, but a dotnet-prefixed executable with this name could not be found on the PATH.
Could not execute because the specified command or file was not found.
`
- wrapper-status: `pass`
- warning-classification: runner-warning-classification: repeated netstandard script-load warning is warning-noise unless target exits nonzero; CoreCLR/VSTest/socket/thread startup failures are environment-failure evidence
- passed: `False`
- remediation-command: Run `dotnet tool restore`, then `dotnet fake --version`; clear stale `.fake/build.fsx/paket-files/paket.restore.cached` only if the FAKE runner still cannot start.
