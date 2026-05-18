# Contract: Generated Consumer Validation

## Workflow

Generated consumer validation must support a path from fresh local package
output to graphical evidence:

1. Pack local FS.Skia.UI packages.
2. Print consumer package configuration.
3. Restore generated consumer against the local feed.
4. Run generated semantic tests.
5. Run bounded real viewer smoke when host support is available.
6. Run deterministic scene-level visual evidence or report unsupported host
   diagnostics.
7. Write readiness evidence.

## Required Behavior

- Validation separates package/feed drift, app source failures, real viewer
  startup failures, and unsupported host conditions.
- Evidence includes elapsed time and enough command context to reproduce the
  run.
- Supported local development machines complete the path within 10 minutes.

## Evidence

- Generated consumer validation transcript.
- First-frame or visual evidence output.
- Unsupported-environment diagnostics when applicable.
- Readiness: `readiness/generated-consumer-validation.md`.
