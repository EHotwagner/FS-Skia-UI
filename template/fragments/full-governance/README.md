# Full Governance Fragment

Adds product evidence, drift, generated guidance, and readiness workflow checks.

Evidence graph and audit targets invoke copied Spec Kit scripts through `bash`
and preserve command, exit-code, output-path, and diagnostic fields in readiness
reports. Redirected generated `Verify` logs are text artifacts under
`readiness/logs/`; review them as text and scan for embedded NUL bytes when
capturing release evidence.
