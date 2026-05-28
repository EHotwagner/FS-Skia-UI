# Contract: Audit Readiness Diagnostics

## Scope

Applies to readiness contract checks performed by evidence audit scripts and generated audit commands.

## Required Behavior

- Missing readiness files must be reported by exact path.
- Incomplete readiness files must report the missing required terms or sections.
- Diagnostics must be visible in command output and persisted to a readiness artifact.
- Diagnostics must distinguish graph failure, readiness contract failure, synthetic evidence failure, diff-scan failure, and unsupported host classification failure.

## Diagnostic Record

```text
path=<readiness path>
status=<missing|incomplete|invalid|pass>
reason=<human-readable reason>
missing-terms=<comma-separated terms when applicable>
missing-sections=<comma-separated sections when applicable>
blocking=<true|false>
```

## Console Output Requirements

For each blocking readiness contract hit, output must include:

- The affected readiness file name.
- Whether the file is missing or incomplete.
- The missing terms or sections when the file exists.
- The command or validation area that found the issue.

## Verification

- Negative fixtures omit terms from known readiness files and assert the output names those terms.
- Missing-file fixtures assert the output names the absent file.
- Passing fixtures assert diagnostics are empty or explicitly marked non-blocking.
