# Generated Guidance Validation

## T007 Red Governance Tests

Recorded at: 2026-05-28T08:15:20+02:00

Added governance tests in `tests/Governance.Tests/GeneratedGuidanceTests.fs`
for:

- Domain geometry names: require `WorldRect`, `WorldPoint`, and `TrackBounds`;
  reject stale app-domain `Rect`, `Point`, and `Size` collision wording.
- Screenshot wording: require live viewer-window capture after first frame and
  explicit separation from `deterministic-scene-evidence`.
- Detached Linux GUI launch wording: require `setsid`, log capture, stderr
  redirection, stdin from `/dev/null`, and reject stale simple backgrounding
  patterns.

Red evidence:

| Command | Expected result | Evidence |
|---------|-----------------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | FAIL, 3 new guidance failures | `readiness/logs/t007-governance-tests-red.txt` |

Observed failures:

- Missing `WorldRect` in generated guidance.
- Missing `live viewer-window capture` screenshot wording.
- Missing `setsid` detached launch guidance.

## T011/T012 US1 Test Coverage

Recorded at: 2026-05-28T08:23:00+02:00

The T007 red governance test
`generated guidance recommends domain geometry names and not primitive
collisions` satisfies the US1 test-first tasks:

- T011: requires at least three domain-specific examples:
  `WorldRect`, `WorldPoint`, and `TrackBounds`.
- T012: rejects stale app-domain recommendations named only `Rect`, `Point`,
  or `Size` when scene/layout primitives are in scope.

Evidence: `readiness/logs/t007-governance-tests-red.txt`.

## T013 US1 Guidance Update

Recorded at: 2026-05-28T08:24:00+02:00

Updated:

- `docs/generated-apps.md`
- `template/base/docs/product.md`
- `template/fragments/scene/README.md`

Accepted examples now present:

- `WorldRect`
- `WorldPoint`
- `TrackBounds`
- `CarPose`
- `CheckpointBounds`

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --logger "console;verbosity=minimal"` | US1 geometry naming test now passes; aggregate still fails on later screenshot wording and detached launch tests | `readiness/logs/t013-governance-tests.txt` |

## T014 US1 Validation

Recorded at: 2026-05-28T08:26:00+02:00

Checked files:

- `docs/generated-apps.md`
- `template/base/docs/product.md`
- `template/fragments/scene/README.md`

Accepted examples: `WorldRect`, `WorldPoint`, `TrackBounds`, `CarPose`,
`CheckpointBounds`.

Rejected stale patterns: app-domain recommendations named only `Rect`, `Point`,
or `Size`; local duplicate bounds records for generated game entities.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `./fake.sh build -t GeneratedGuidanceCheck` | PASS | `readiness/logs/t014-generated-guidance-check.txt` |
| `./fake.sh build -t TemplateCheck` | PASS | `readiness/logs/t014-template-check.txt` |
| `./fake.sh build -t TemplateDrift` | PASS | `readiness/logs/t014-template-drift.txt` |

## T031 Generated Product And Guidance Validation

Recorded at: 2026-05-28T08:34:39+02:00

Generated product validation initially exposed a package-version drift:
generated template code consumed the additive `ScreenshotEvidenceResult`
fields, but `FS.Skia.UI.SkiaViewer` was still pinned and packed as
`0.1.23-preview.1`. The fix bumped:

- `src/SkiaViewer/SkiaViewer.fsproj` to `0.1.24-preview.1`
- `template/base/Directory.Packages.props` `FS.Skia.UI.SkiaViewer` to
  `0.1.24-preview.1`

`PackLocal` then produced
`FS.Skia.UI.SkiaViewer.0.1.24-preview.1.nupkg`, allowing generated consumer
restore and tests to run against the current public surface.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `./fake.sh build -t PackLocal` | PASS, local SkiaViewer `0.1.24-preview.1` package produced | `readiness/logs/t031-pack-local.txt` |
| `./fake.sh build -t GeneratedProductCheck` | PASS | `readiness/logs/t031-generated-product-check.txt` |
| `./fake.sh build -t GeneratedGuidanceCheck` | PASS | `readiness/logs/t031-generated-guidance-check.txt` |
| `./fake.sh build -t TemplateCheck` | PASS | `readiness/logs/t031-template-check.txt` |
| `./fake.sh build -t TemplateDrift` | PASS | `readiness/logs/t031-template-drift.txt` |
