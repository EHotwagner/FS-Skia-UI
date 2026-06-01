# Full Governance Fragment

Adds product evidence, drift, generated guidance, and readiness workflow checks.

Evidence graph and audit targets run in-process through the packaged
`FS.Skia.UI.Build` engine (no copied Python or `run-audit.sh`) and preserve
command, exit-code, output-path, and diagnostic fields in readiness reports.
Redirected generated `Verify` logs are text artifacts under `readiness/logs/`;
review them as text and scan for embedded NUL bytes when capturing release
evidence.
