# Bounded Viewer Smoke Readiness

## Scope

Readiness evidence for first-frame, frame-count, and bounded duration viewer
runs that exit without external shell timeouts and report structured success or
pre-frame failure evidence.

## Setup Notes

- Tier: Tier 1 contracted public/API and generated-consumer validation change.
- Affected areas: `src/SkiaViewer/`, smoke tests, generated consumer smoke
  commands, build targets, and readiness logs.
- Public contract impact: `.fsi` signatures must cover bounded run requests,
  evidence, failures, and MVU-shaped lifecycle boundaries where owned.
- Command-surface impact: `GeneratedProductCheck`, `Verify`, `Ci`, and related
  smoke/readiness targets may change.
- Synthetic policy: forced pre-frame failures and unsupported host
  classification may use disclosed synthetic fixtures, but final bounded
  startup readiness needs real viewer smoke evidence on a supported host or
  explicit unsupported-host diagnostics.

## Evidence

- `src/SkiaViewer/SkiaViewer.fsi` exposes `ViewerRunRequest`,
  `ViewerRunEvidence`, `ViewerRunFailure`, `ViewerRunModel`, `ViewerRunMsg`,
  `ViewerRunEffect`, and `Viewer.runBounded`.
- `tests/SkiaViewer.Tests/Tests.fs` exercises first-frame evidence, exact
  frame-count completion, positive frame count/timeout/duration validation,
  forced pre-frame stage classification, pure `RunStarted`/`RecordFrame`
  lifecycle transitions, emitted effects, and internal timeout completion.
- Verification log:
  `specs/013-tetris-demo-integration/readiness/logs/us2-skiaviewer-tests.txt`.
- Public FSI smoke log:
  `specs/013-tetris-demo-integration/readiness/logs/us2-bounded-viewer-smoke-fsi.txt`.
- Current host result: `status=unsupported`, `blocked-stage=Renderer`,
  `classification=UnsupportedEnvironment`, because the live bounded viewer
  path is not available in this host. The run returned this as structured
  pre-frame failure evidence without shell timeout or stderr scraping.
- Generated template command: `template/base/src/Product/Program.fs` now
  exposes `--bounded-smoke [path]`, writes a bounded smoke report, exits `0`
  for supported-host success or explicit unsupported-host diagnostics, and
  exits non-zero for product-defect failures.

## Requirement Mapping

- FR-007: bounded request targets are represented by `ViewerEvidenceTarget`.
- FR-008: success evidence includes frames rendered, elapsed time, initial
  output size, renderer mode, diagnostic summary, and evidence path.
- FR-009: pre-frame failures include blocked stage, classification, diagnostic
  category, message, and last diagnostic summary.
- FR-014a: generated consumer smoke has an explicit bounded command path.
- FR-019: evidence is written under this feature readiness directory.
- SC-003/SC-004: current environment produces explicit unsupported-host output;
  supported desktop evidence must be rerun where the live viewer path is
  available.
