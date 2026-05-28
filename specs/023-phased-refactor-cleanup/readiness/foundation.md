# Foundation Evidence

Captured at: `2026-05-27T22:35:00+02:00`

## T005 Stable Behavior Contract Inventory

Stable generated surfaces:

- Generated command names: `--layout-evidence`, `--launch-evidence`,
  `--scene-evidence`, `--image-evidence`, `--screenshot-evidence`,
  `--pixel-readback-evidence`, `--visual-evidence`,
  `--window-diagnostics`, `--window-options`, and default `dotnet-run`.
- Report fields and statuses include `status`, `command`, `output`, `mode`,
  `evidence-kind`, `renderer-mode`, `unsupported-host-reason`, `fallback`,
  `blocked-stage`, `classification`, `category`, `diagnostics`,
  layout proof fields, and visual/screenshot artifact fields.
- Status vocabulary remains `ok`, `failed`, `unsupported`, plus existing
  option statuses from window behavior validation.
- Output paths remain the generated defaults under `readiness/`, including
  `layout-evidence.txt`, `headless-scene-evidence.txt`,
  `evidence-launch-mode.txt`, `game-image-evidence.png`,
  `game-screenshot-evidence.txt`, and visual evidence metadata paths.
- Exit-code meanings: successful evidence returns `0`; failed validation or
  unsupported/failing evidence returns non-zero where existing commands do so.
- Generated profile names and package IDs are stable; this feature does not
  rename profiles or packages.
- FAKE target names remain `Dev`, `Verify`, `Ci`, `PackLocal`,
  `DependencyReport`, `TemplateCheck`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit`.
- Readiness paths remain the five phase files named in `plan.md`.
- Public signatures and surface baselines remain unchanged for this Tier 2
  feature.

## T006 Generated Evidence/Report Behavior Characterization

Existing generated evidence behavior is covered by `template/base/tests/Product.Tests/Tests.fs`
and `tests/Testing.Tests/Tests.fs`.

Observed contracts:

- `template/base/src/Product/Program.fs` already defines
  `generatedEvidenceStatusText`, `generatedEvidenceExitCode`,
  `evidenceField`, and one local `writeEvidenceReport` wrapper used by layout,
  image, screenshot, and visual evidence paths.
- `writeEvidenceReport` writes `status`, `command`, and `output`, appends
  command-specific fields, creates the parent directory, echoes report lines to
  stdout, and returns the status-specific exit code.
- Current specialized writers still exist for bounded smoke and launch
  evidence: `writeBoundedSmokeReport`, `writeLaunchEvidenceReport`, and
  `writeLaunchFailureReport`.
- Required layout fields include `scene`, `output-size`, `proof-level`,
  `hud-region`, `gameplay-region`, `text-bounds`, `gameplay-bounds`,
  `overlap-status`, `measurement-mode`, `accepted`, and `diagnostics`.
- Required visual/screenshot fields include `mode`, `evidence-kind`,
  `supported-host`, `fallback-reason`, `unsupported-host-reason`, `fallback`,
  `screenshot-path`, `width`, `height`, `frames-rendered`, and `diagnostics`.
- `tests/Testing.Tests/Tests.fs` validates generated evidence outputs,
  unsupported-host required fields, image decodability metadata, and missing
  field diagnostics.

## T007 Duplication Classification

| Helper family | Classification | Rationale | Verification coverage |
|---------------|----------------|-----------|-----------------------|
| Generated evidence report writers | consolidate | Multiple generated writers own equivalent status/path/stdout semantics. | Product template tests, `Testing.Tests`, `TemplateCheck`, `GeneratedGuidanceCheck`. |
| Generated layout geometry helpers | intentional template copy | Generated products must stay standalone and not depend on repository-only helpers. | Layout evidence tests and generated product validation. |
| Build `ensureParent`, reports, process execution | consolidate repository-local | Repeated in one large FAKE script and test support; can move to loaded scripts without public target changes. | Governance tests and focused FAKE targets. |
| `parseScalar` / `parseInlineList` in build/test support | consolidate repository-local where feasible | Same capability-catalog parsing behavior appears in build and governance support. | Governance helper tests and capability validation. |
| Viewer image/path helpers | package-boundary copy | Viewer runtime owns filesystem/image evidence behavior behind its package facade. | `SkiaViewer.Tests` and viewer evidence checks. |
| Template/base `runProcess` | intentional template copy | Generated projects need a tiny local build script and must not import repo build internals. | Template smoke and generated product checks. |
| Compatibility package accumulation | deferred | Close to public API and out of scope for this Tier 2 feature. | Separate Tier 1 design required before changes. |

## T008 Viewer Behavior Characterization

Existing viewer behavior is characterized by `tests/SkiaViewer.Tests/Tests.fs`
behind the unchanged `src/SkiaViewer/SkiaViewer.fsi` facade.

Observed contracts:

- Screenshot evidence reports `ScreenshotUnsupported`, does not claim a
  screenshot artifact when unsupported, and points to
  `deterministic-scene-evidence`.
- Viewer MVU tests exercise public `init`, `update`, `updateRun`, and
  generated host paths, asserting emitted effects such as `OpenWindow`,
  `QueryNativeWindowState`, `EmitDiagnostic`, and `CloseWindow`.
- Host capability checks distinguish renderer mode, persistent windows,
  bounded smoke, keyboard capability, unsupported host reasons, and missing
  package capabilities.
- Desktop diagnostics classify missing Linux session prerequisites as
  `unsupported-host` and present Wayland/session prerequisites as
  `environment-session-ready`.
- Window behavior validation covers resize, maximize, startup state, startup
  position, backend settings, invalid coordinates, unsupported backend
  settings, and positive-size constraints.
- Visual evidence tests distinguish PNG image evidence from metadata/hash
  evidence and keep screenshot proof separate from deterministic fallback.
- Current baseline command recorded one host-environment crash in
  `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj`: after 32
  passing tests, test host crashed on `libdecor-gtk.so` initialization.

## T009 Batch Evidence Log Format

Every phase readiness file uses this table:

| Task | Command | Exit code | Risk | Changed ownership area | Pre-existing failure attribution | Verdict |
|------|---------|-----------|------|------------------------|----------------------------------|---------|

Field rules:

- `Task`: task id or task range.
- `Command`: exact command run from repository root.
- `Exit code`: numeric exit code, or `not run` with rationale.
- `Risk`: `small`, `medium`, or `broad`, using the risk-level definitions in
  `tasks.md`.
- `Changed ownership area`: generated evidence, generated template split,
  build governance, viewer internals, or integration.
- `Pre-existing failure attribution`: explicit baseline link or `none`.
- `Verdict`: `PASS`, `FAIL`, `PRE-EXISTING FAILURE`, `ADVISORY`, or
  `BLOCKED`.
