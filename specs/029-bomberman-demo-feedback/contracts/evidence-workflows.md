# Contract: Evidence Workflows

## Evidence Graph Invocation

Generated projects must invoke authoritative Spec Kit scripts through the shell interpreter.

```text
bash .specify/extensions/evidence/scripts/bash/run-audit.sh <feature-dir> --graph-only
bash .specify/extensions/evidence/scripts/bash/run-audit.sh <feature-dir>
```

The command contract must not require executable mode on copied scripts.

## Verify Log Contract

`Verify` and generated `Verify` logs are plain text artifacts.

Required properties:

- redirected stdout/stderr can be opened as text
- no embedded NUL bytes
- pass and fail cases retain command, exit code, target, and diagnostic context
- generated `Verify` remains authoritative only when generated tests run through the generated `Verify` path

## Screenshot Evidence Contract

Screenshot evidence reports use stable `key=value` lines.

Required fields for all statuses:

- `status`
- `command`
- `output`
- `mode`
- `evidence-kind`
- `app-or-sample`
- `host-facts`
- `capture-mode`
- `viewer-open-status`
- `first-frame-status`
- `capture-availability`
- `capture-source`
- `proves-screenshot`
- `message`
- `timestamp`
- `diagnostics`

Additional required fields for `status=ok`:

- `artifact-path`
- `screenshot-path`
- `width`
- `height`
- `pixel-content-validation=PixelContentNonBlank`

Additional required fields for `status=unsupported`:

- `unsupported-host-reason`
- `fallback`
- `blocked-stage`
- `classification`
- `category`

Invalid classifications:

- `unsupported` without capture probe detail
- `ok` with deterministic metadata only
- `ok` with blank, unreadable, missing, or out-of-readiness artifact path
- `unsupported` for app-command implementation errors

## Readiness Evidence Paths

The feature is not reviewable until these files exist and contain real evidence:

- `specs/029-bomberman-demo-feedback/readiness/evidence-graph-invocation.md`
- `specs/029-bomberman-demo-feedback/readiness/verify-log-cleanliness.md`
- `specs/029-bomberman-demo-feedback/readiness/screenshot-evidence-probe.md`
- `specs/029-bomberman-demo-feedback/readiness/generated-app-wiring.md`
- `specs/029-bomberman-demo-feedback/readiness/scene-layout-authoring.md`
