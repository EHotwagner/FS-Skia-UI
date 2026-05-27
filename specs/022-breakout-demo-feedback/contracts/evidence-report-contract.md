# Contract: Evidence Report Conventions

Generated evidence commands must write predictable key-value reports and echo
the same content to stdout.

## Required Behavior

- Create parent directories before writing.
- Write fields in stable order.
- Echo written lines to stdout.
- Normalize statuses to `ok`, `unsupported`, or `failed`.
- Include `status`, `command`, and `output` when an output file exists.
- Include `unsupported-host-reason` and fallback fields for unsupported host
  paths.
- Return consistent exit behavior for success, unsupported, and failure cases
  so governance automation can classify command results.

## Acceptance

- At least three generated evidence commands share the same required field
  ordering and status vocabulary.
- Report helpers are available through public or generated helper contracts
  rather than hand-rolled independently by each command.
- Governance checks fail on missing status fields, ambiguous unsupported-host
  wording, or stdout/file mismatch.
