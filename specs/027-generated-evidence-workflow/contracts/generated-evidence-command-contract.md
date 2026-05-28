# Contract: Generated Evidence Commands

## Scope

Applies to generated project targets and command wrappers that expose evidence graph, evidence audit, generated guidance, or template validation results.

## Required Behavior

- `EvidenceGraph` must run authoritative graph validation for the selected generated feature or generated governance package.
- `EvidenceAudit` must depend on a valid graph result and run authoritative audit validation.
- A command must fail when authoritative validation fails.
- A command must not write a pass or completion-only report when validation is skipped, unavailable, or placeholder-only.
- Normal generated interactive launch must remain separate from evidence validation commands.

## Required Report Fields

- `command`
- `target`
- `feature-directory` or generated project identity
- `authority`
- `status`
- `exit-code`
- `validation-area`
- `report-path`
- `message`
- `diagnostics`

## Pass Conditions

- `authority` is `authoritative` or `delegated-authoritative`.
- Exit code is zero.
- Required graph/audit artifacts are written.
- No validation area reports a blocking failure.

## Failure Conditions

- Authoritative validation exits non-zero.
- The command cannot locate the feature or readiness package.
- The command writes only placeholder completion text.
- The command reports pass while validation was skipped.

## Verification

- Governance tests create deliberately broken generated evidence packages and assert generated commands fail.
- Template validation confirms generated targets do not contain success-only `EvidenceGraph completed` or `EvidenceAudit completed` behavior unless backed by authoritative validation output.
- Readiness evidence records command, exit code, output artifact, and failure/pass reason.
