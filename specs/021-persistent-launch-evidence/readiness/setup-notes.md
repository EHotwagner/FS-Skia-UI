# Setup Notes

- feature-tier: Tier 1 contracted framework, generated-template, and governance change
- affected packages: `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Testing`
- generated-project impact: explicit evidence-mode command and readiness artifact while preserving normal persistent launch
- build-target impact: `Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `EvidenceGraph`, and `EvidenceAudit`
- public-API impact: expected `.fsi` changes in `src/SkiaViewer/SkiaViewer.fsi` and `src/Testing/Testing.fsi`
- unsupported scope: hosts without desktop prerequisites must be classified at the exact blocked stage
- broad validation obligation: targeted package tests, generated checks, template checks, surface checks, evidence graph, evidence audit, and `Verify`

## MVU Applicability

Persistent launch is I/O-bearing. The implementation must model owned workflow
state through `Model`, user/external transitions through `Msg`, requested I/O
through `Effect`, startup effects through `init`, pure state changes through
`update`, and filesystem/window/input work through an edge interpreter.

Before any MVU-bearing `[US*]` task is marked `[X]`, evidence must assert both
the next model and emitted effects through public paths and must exercise the
interpreter against real dependencies where safe.

## Readiness Discovery

The final audit must discover:

- `readiness/persistent-launch-evidence.md`
- `readiness/window-observation-diagnostics.md`
- `readiness/host-warning-classification.md`
- `readiness/generated-guidance.md`
- `readiness/evidence-audit.md`

Missing facts must produce actionable diagnostics and must not be converted into
passing persistent-launch evidence.

Unsupported or blocked hosts must report the first precise blocked stage that
prevents accepted evidence. Missing `status`, `mode`, `command`,
`window-opened`, `input-dispatch`, `exit-path`, `blocked-stage`,
`classification`, `category`, `message`, or first-frame facts must be named in
diagnostics. A supported-host pass claim is invalid unless `window-opened=true`,
`first-frame-presented=true`, `exit-path=true`, and `blocked-stage=none` are
all present in the artifact.

## Build-Target Coverage

- `Verify`: broad governed workflow; must include readiness preflight and remain non-authoritative when a focused evidence gate fails.
- generated `Test`: generated product semantic tests must cover default persistent launch separation from explicit evidence flags.
- `GeneratedProductCheck`: must validate generated source, product tests, explicit evidence command shape, and normal launch preservation.
- `GeneratedGuidanceCheck`: must validate app-qualified names and evidence-separation wording.
- `TemplateCheck`: must validate generated output includes the readiness command and product tests without copying framework implementation artifacts.
- `EvidenceGraph`: must refresh `readiness/task-graph.md` and `readiness/task-graph.json` after each task status change.
- `EvidenceAudit`: must discover required readiness files, synthetic propagation, diff-scan output, and persistent-launch artifact contract fields.
