# Verification Verdict Evidence

## Verify bootstrap

- verdict-category: `environment-failure`
- authoritative-product-evidence: `False`
- exit-code: `1`
- health-snapshot-path: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/bootstrap-runner.md`
- log-path: `/home/developer/projects/FS-Skia-UI/specs/121-live-host-idle-parking/readiness/bootstrap-runner.md`
- recommended-rerun-environment: fresh shell, fresh container, or CI runner
- product-checks-run: (none)
- product-failures: (none)
- environment-failures: failed: FAKE runner did not start after tool restore: Possible reasons for this include:
  * You misspelled a built-in dotnet command.
  * You intended to execute a .NET program, but dotnet-fake does not exist.
  * You intended to run a global tool, but a dotnet-prefixed executable with this name could not be found on the PATH.
Could not execute because the specified command or file was not found.



