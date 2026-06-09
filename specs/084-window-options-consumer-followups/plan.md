# Implementation Plan: Window Startup Options (Fullscreen + Windowed-Fullscreen Default) & Invoice1/Spread1 Consumer Follow-ups

**Branch**: `084-window-options-consumer-followups` | **Date**: 2026-06-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/084-window-options-consumer-followups/spec.md`

## Summary

One consolidated consumer-friction feature (house pattern, e.g. 060–063) bundling
the Invoice1 + Spread1 feedback with the new window-startup options. Primary
technical thrust: add a **windowed-fullscreen** startup state (borderless coverage
of the monitor work area), make both **fullscreen** and **windowed fullscreen**
validate as **honored** (not `UnsupportedOption`), make **windowed fullscreen the
default**, and **wire the parsed window-behavior request into the generated app's
live launch** (`runAppWithWindowBehavior`) while keeping the durable
`Viewer.runApp viewerOptions generatedHost` literal reachable via a guarded branch.
Alongside: single-source the shipped `evidence-formats.md` so it documents the
engine's full seven-file window-visibility contract; make `EvidenceAudit` stdout
enumerate each blocker (reason + hit-file path) and report the resolved diff-scan
base ref; reconcile `scaffold-map.md` (project-named paths + a durable/must-re-point
split + a non-game remap example); document the `Verify`-embeds-audit relationship
and name `-t Test` as the mid-implementation green-test path; and make
`/speckit-analyze` degrade gracefully when `SymbolCrossCheck` is absent. See
[research.md](./research.md) for the per-finding decisions and the triage
refinements (the engine requires **seven** readiness files, not six; FR-012 is
already largely satisfied in the template).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: SkiaSharp 4 preview, Silk.NET.Windowing (existing — used
for `WindowOptions`/`WindowState`/`WindowBorder` and the monitor work-area query);
FAKE governance engine (`FS.Skia.UI.Build`). No new third-party dependency.
**Testing**: Expecto (`tests/SkiaViewer.Tests`, `tests/Governance.Tests`), FAKE
targets, FSI transcript against the packed surface, generated-product real visible-
window evidence.
**Target Platform**: Windows and Linux (display-capable host for visible-window
evidence; headless degrades to honest render-only).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: `template/base/**` changes — `src/Product/Program.fs`
  (guarded `runAppWithWindowBehavior` launch branch keeping the `runApp` literal),
  `src/Product/WindowOptions.fs` (new `windowed-fullscreen` flag value + default +
  `fullscreen`/`windowed-fullscreen` reclassified honored), regenerated
  `docs/evidence-formats.md` (seven-file window-visibility contract), hand-edited
  `docs/scaffold-map.md` (project-named paths, durable/must-re-point split, non-game
  remap), and `docs/product.md` / `README.md` (Verify-embeds-audit + `-t Test`
  guidance, FR-013). No new template **option** (no `.template.config/template.json`
  change — only file contents). The `fs-skia-template-update` skill's expected set
  is unaffected (no new pin).
- **Dependency impact**: N/A — no new dependency. Silk.NET.Windowing (already
  referenced by SkiaViewer) supplies the monitor work-area query for windowed
  fullscreen; `Directory.Packages.props`, `docs/dependencies.md`, and
  `DependencyReport` need no new row.
- **Command-surface impact**: No new FAKE target. Behavior/output changes to
  existing targets: `EvidenceAudit` (stdout per-blocker legibility + base_ref,
  FR-008/009), `Verify`/`Dev` documentation (FR-012/013, mostly already present),
  `RefreshSurfaceBaselines` regenerates `evidence-formats.md` from the extended
  `EvidenceFormatSchema` and the `.claude` skill mirror, `TemplateCheck` /
  `TemplateDrift` revalidate the changed template, `TargetMetadataDrift` enforces
  doc currency. `validation.contract.yml` is unchanged (no `Routing.fs` tier/gate
  change — escalation is by path globs already covering `template/**`, public
  `.fsi`, governance, and `.agents/skills/**`). FAKE-backed targets run sequentially
  in the serialized order (shared `.fake` state) — the escalated six-target path:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  (see Project Structure / quickstart).
- **Generated project impact**: Generated products change via the template:
  windowed-fullscreen default + new flag value, the live-launch wiring, the
  regenerated docs, and the corrected build-signal guidance. Generated `Dev`
  behavior is unchanged (still log-only — FR-012 is satisfied by guidance, not a
  target rewrite). `TemplateCheck` / `GeneratedProductCheck` re-validate.
- **Evidence paths**: This is a window-visibility feature (trigger literals present
  in the spec), so it ships the full seven-file readiness set under
  `specs/084-window-options-consumer-followups/` /readiness:
  `interactive-visible-window.md`, `window-state-diagnostics.md`,
  `window-options.md`, `close-reason-separation.md`, `real-image-evidence.md`
  (decodable windowed-fullscreen launch screenshot), `generated-validation.md`,
  `evidence-audit.md`. Plus: `readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt`
  (moved baseline), the moved cross-package surface baseline, regenerated
  `template/base/docs/evidence-formats.md`, regenerated `.claude/skills/speckit-analyze/**`,
  an FSI transcript exercising the packed `ViewerWindowStartupState` surface, and
  the escalated six-target evidence set (`Dev`, `GeneratedGuidanceCheck`,
  `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`).
- **`.fsi` / contract impact**: Tier 1. `src/SkiaViewer/SkiaViewer.fsi` gains the
  `WindowedFullscreen` union case on `ViewerWindowStartupState` (additive) and
  `defaultWindowBehavior` changes value (not signature). Per-package **and** cross-
  package surface baselines move and MUST be recaptured
  (`RefreshSurfaceBaselines` / `PerPackageSurface`). No other signature change.
  Compatibility: additive case (exhaustive matchers get a desirable warning); the
  default-value change is documented. See
  [contracts/skiaviewer-window-surface.md](./contracts/skiaviewer-window-surface.md).
- **MVU/effect boundary**: The viewer is the stateful boundary. `Model` =
  `ViewerModel` (carries `WindowBehavior`), `Msg` = viewer messages (`Start`, …),
  `Effect` = `ViewerEffect` with the existing `ApplyWindowOptions of
  ViewerWindowBehaviorRequest` carrier (**no new effect type**), `init`/`update`
  stay pure, the interpreter at the window edge applies the behavior. The new
  windowed-fullscreen mechanics live in `applyWindowBehaviorToOptions` (pure
  options transform) plus the edge interpreter that reads monitor work-area bounds.
  Pure transition tests assert validation classification + default; interpreter
  evidence is the real visible-window capture.
- **Synthetic evidence**: None planned for the framework change — validation
  classification and default are exercised against the real packed surface, and
  window states against a real display-capable host. The deliberate readiness gap
  used to verify audit legibility (SC-004) is a **test input**, not a stand-in for
  an unavailable dependency, so no `[S]` is required. If real visible-window
  evidence for a given state cannot be captured on the available host before merge,
  that task is `[S]` with the render-only honest-degradation note — not anticipated
  on the GPU-passthrough dev host.
- **Test evidence**: Failing-first Expecto in `tests/SkiaViewer.Tests`:
  `validateWindowBehavior`/`validateWindowLaunchBehavior` report `Honored` for
  `Fullscreen` and `WindowedFullscreen` and `UnsupportedOption` for `Minimized`
  (SC-002); `defaultWindowBehavior.StartupState = WindowedFullscreen` (SC-001);
  `applyWindowBehaviorToOptions` maps `WindowedFullscreen` to hidden border + work-
  area geometry. `tests/Governance.Tests`: rendered `evidence-formats.md` window-
  visibility file list equals `Scans.requiredFiles` (SC-003); audit stdout contains
  per-blocker reason + hit-file path and a base_ref line on a poisoned fixture
  (SC-004); `scaffold-map.md` contains the project-named paths + must-re-point class
  + non-game remap phrases (SC-005, extending `Feature062GovernanceTests`); the
  durable `GovernanceTests.fs` literal still passes after the guarded launch wiring
  (FR-005). FSI transcript exercises the packed surface (Principle I).
- **Observability**: `EvidenceAudit` stdout names each blocker (area, file, one-
  line reason, hit-file path) and the resolved diff-scan base ref or an explicit
  absence message (FR-008/009) — actionable, fail-loud, no silent empty diff-scan.
  Unsupported-environment window diagnostics no longer label fullscreen / windowed
  fullscreen host-unsupported on a capable host; headless degrades to honest
  render-only (no false visible-window claim). Exclusive fullscreen on an incapable
  host falls back with an honest diagnostic, not a false "honored".
- **Deferred scope**: GEN-1 generalizable code (parse→AST→topo recipe, KeyCommand
  note, money formatter) and the SKILL-1 scaffold-swap procedure skill are **out of
  scope**, recorded as bounded follow-up candidates. No new platforms/backends, no
  multi-monitor selection (default monitor only), no exclusive-fullscreen
  resolution/refresh switching beyond existing `WindowState.Fullscreen`, no release
  /distribution changes.

## Project Structure

```
src/SkiaViewer/
  SkiaViewer.fsi                  # EDIT: + WindowedFullscreen case; defaultWindowBehavior value note
  SkiaViewer.fs                   # EDIT: union case; defaultWindowBehavior=WindowedFullscreen;
                                  #       validateBehavior/validateLaunch reclassify Fullscreen+WindowedFullscreen honored;
                                  #       applyWindowBehaviorToOptions WindowedFullscreen→hidden border+work-area;
                                  #       edge interpreter reads monitor work-area bounds

build/Governance/Evidence/
  EvidenceFormatSchema.fs         # EDIT: WindowVisibility class renders all 7 Scans.requiredFiles + tokens (single source)
  GeneratedRunner.fs              # EDIT: echo per-blocker reason + hit-file path to stdout summary (FR-008)
  Render.fs                       # EDIT/REUSE: per-area diagnostics into summary; base_ref in stdout (FR-009)
  DiffScan.fs                     # EDIT: populate DiffScanResult.BaseRef (was hardcoded None)
build/Governance/Front/
  Governance.fs                   # EDIT: thread resolved merge-base/baseRef into EvidenceInputs → diff-scan (FR-009)

template/base/
  src/Product/WindowOptions.fs    # EDIT: + windowed-fullscreen flag value; default→windowed-fullscreen; reclassify honored
  src/Product/Program.fs          # EDIT: guarded runAppWithWindowBehavior branch; keep runApp literal reachable (FR-004/005)
  docs/evidence-formats.md        # REGENERATED from EvidenceFormatSchema (RefreshSurfaceBaselines)
  docs/scaffold-map.md            # EDIT (hand-authored): <ProjectName> paths; durable/must-re-point split; non-game remap
  docs/product.md                 # EDIT: Verify embeds EvidenceGraph+EvidenceAudit; name -t Test mid-impl path (FR-013)
  README.md                       # EDIT: same Verify/-t Test guidance (FR-013)

.agents/skills/speckit-analyze/SKILL.md   # EDIT (canonical): probe SymbolCrossCheck availability; skip-with-notice (FR-014)
.claude/skills/speckit-analyze/SKILL.md   # REGENERATED mirror (RefreshSurfaceBaselines / SkillSyncCheck)

readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt   # MOVED baseline (recapture)
(cross-package surface baseline)                              # MOVED (recapture)

tests/SkiaViewer.Tests/           # EDIT: classification, default, options-mapping tests (failing-first)
tests/Governance.Tests/           # EDIT: evidence-formats↔requiredFiles parity; audit-stdout legibility;
                                  #       scaffold-map phrases; durable-launch-literal survival
```

## Design notes / key decisions

See [research.md](./research.md) for the full per-finding rationale and the two
triage refinements that shape the plan:

- **Seven, not six, readiness files.** The engine's `Scans.requiredFiles` includes
  `real-image-evidence.md`; the doc generator's `WindowVisibility` schema must cover
  the full set and a test must assert parity, so the doc cannot silently drift again
  (FR-007 / R5).
- **FR-012 is mostly done.** The template already warns that `Dev` is log-only and
  names `-t Test`; the residual work is the **FR-013** Verify-embeds-audit statement
  (R9/R10), so the plan does not rewrite the `Dev` target.
- **base_ref is resolved but discarded.** The merge-base is already computed in
  `runEvidenceAuditCheck`; the only bug is that it is never threaded into
  `DiffScanResult.BaseRef`/stdout (R7) — a reporting fix, not a resolution fix.
- **Durable-literal preservation.** The guarded launch branch keeps
  `Viewer.runApp viewerOptions generatedHost` reachable for `GovernanceTests.fs:105`
  while wiring `runAppWithWindowBehavior` for flagged launches (R4 / FR-005).

`data-model.md` defines the extended startup-state set, the unchanged request/result
shapes, the seven-file schema entry, the surfaced blocker record, the populated
diff-scan base ref, and the clarified scaffold-map classification. `contracts/`
carries the `.fsi` surface delta, the `--window-startup` flag contract, and the
`EvidenceAudit` stdout contract. `quickstart.md` walks the FSI-first surface
exercise, real visible-window capture, and the escalated validation order.

**Constitution re-check (post-design).** Tier 1 obligations met: `.fsi` update +
baseline recapture planned, MVU boundary preserved (no new effect type), no new
dependency, synthetic disclosure regime addressed (none anticipated), test evidence
and observability specified, deferrals bounded. No gate violation; the change
escalates to the `maintainer-verify` six-target path as the spec predicts — confirm
with `./fake.sh build -t Route`.
