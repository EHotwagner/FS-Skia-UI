# Research: Generated Evidence Workflow Authority

## Generated Evidence Command Authority

**Decision**: Generated `EvidenceGraph` and `EvidenceAudit` targets must run, delegate to, or be checked against the same Spec Kit evidence graph and audit semantics used by repository governance. They must not write success-only completion reports when validation has not run.

**Rationale**: The lunar lander feedback showed generated commands printing completion while the real graph/audit scripts still failed. That creates false readiness. Authoritative generated targets preserve the user's mental model: a generated command named `EvidenceAudit` means evidence was audited.

**Alternatives considered**:

- Keep generated stubs but rename them as non-authoritative. Rejected as lower value because generated users still need a real evidence command.
- Keep stubs and rely on documentation. Rejected because it preserves the failure mode.
- Only validate in the root repository. Rejected because generated consumers need local confidence and reproducible output.

## Skill-Loading Evidence Workflow

**Decision**: Generate and validate skill-loading evidence from structured `tasks.deps.yml` `skillist` metadata. Required evidence granularity is exactly one row for each `(task id, skill id)` pairing, with task start and skill loaded timestamps proving skill load happened before task work began.

**Rationale**: The audit requirement is sound but brittle when humans manually write Markdown rows. Deriving expected rows from the same task metadata already used by the graph validator removes ambiguity and makes collapsed range rows obviously invalid.

**Alternatives considered**:

- Accept range rows such as `T002-T055`. Rejected because the graph validator cannot prove per-task coverage or timestamp ordering.
- Require manual rows only. Rejected because it caused after-the-fact bookkeeping failures.
- Treat equal timestamps as acceptable. Rejected because the existing governance rule intentionally requires skill loading before work starts.

## Audit Readiness Diagnostics

**Decision**: Audit diagnostics must print missing readiness files and missing required terms/sections in the command output and persist them in structured readiness artifacts.

**Rationale**: The current scan already knows the required terms. Hiding those terms forces users to inspect scripts instead of fixing evidence. Printing them makes failures actionable without weakening the contract.

**Alternatives considered**:

- Keep generic incomplete-file messages. Rejected because they are not actionable enough.
- Move all term requirements into docs only. Rejected because docs drift can hide actual enforcement.
- Print terms only in JSON. Rejected because command-line users need immediate feedback.

## Readiness Contract Discovery

**Decision**: Audit-enforced readiness files must be discoverable before implementation through explicit tasks or generated readiness placeholders/checklists.

**Rationale**: Missing files such as `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, and `runtime-limitations.md` were not obvious from task output. Discovery must happen before work starts, not after an audit failure.

**Alternatives considered**:

- Leave discovery to `EvidenceAudit`. Rejected because it delays contract feedback.
- Only document the files in a central guide. Rejected because individual feature tasks still need actionable work items.
- Generate placeholders for every possible readiness file in every feature. Deferred because task generation can scope required files more precisely.

## Generated FS.Skia.UI Guidance

**Decision**: Generated guidance must cover app message qualification, app vector to scene point conversion, semantic scene evidence limitations, and strict screenshot/fallback vocabulary.

**Rationale**: These were repeated practical friction points in a real generated game. Guidance belongs in generated app-facing docs and tests because it affects how consumers write evidence-safe apps.

**Alternatives considered**:

- Add public semantic scene annotation APIs now. Deferred as a broader public contract change.
- Rely on examples in historical specs. Rejected because generated app authors need guidance inside generated output.
- Treat screenshot fallback wording as documentation-only. Rejected because audit and generated tests must protect against proof vocabulary drift.
