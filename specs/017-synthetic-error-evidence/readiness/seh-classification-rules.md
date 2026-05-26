# SEH Classification Rules

`[SEH]` is valid only before implementation starts and only with
`synthetic-error-handling-approved`.

Eligible classes: malformed parser input, corrupt file content, invalid command
arguments, protocol violations, missing required data, hostile payloads, forced
error-result fixtures, and unsupported-environment diagnostics only when the
synthetic input itself is the error-path condition.

Non-eligible classes: convenience mocks, incomplete integrations, unavailable
product capability, missing host support, placeholder outputs, speed-only
fixtures, unsupported-host substitutes, and ordinary in-memory substitutes.

Required inventory fields: task id, reason, real-evidence path or infeasible
rationale, label, design source, synthetic input class, expected error
behavior, and acceptance status.

Reviewer timing: classification is allowed during spec, plan, clarification,
or task generation. Implementation-time relabeling, readiness cleanup labels,
and labels added after an audit failure are rejected.

Evidence: `dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj
-- --filter "Synthetic error evidence governance"` passed 5 tests on
2026-05-26.
