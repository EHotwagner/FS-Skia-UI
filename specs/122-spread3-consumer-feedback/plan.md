# Implementation Plan: Spread3 Consumer Feedback Remediation

**Branch**: `122-spread3-consumer-feedback` | **Date**: 2026-06-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/122-spread3-consumer-feedback/spec.md`

## Summary

Remediate the genuine framework gaps surfaced by the Spread3 consumer dogfood feedback,
after the dogfood-verify pass (see [research.md](./research.md)) narrowed the spec's maximal
reading to the items that still exist in the current tree:

1. **Black-frame blink (FR-001/002, P1).** The `DirectToSwapchain` idle-skip
   (`OpenGl.fs` `canIdleSkip`) skips `SwapBuffers` entirely on an unchanged scene, betting on a
   double-buffer model; on Wayland windowed-fullscreen's 3+ buffer swapchains the un-filled
   buffers rotate in as black. Replace the binary skip with a pure
   `GlHost.planPresent` decision and a bounded **re-present of the cached last-good frame** that
   keeps every swapchain buffer populated, then idles — preserving the feature-120/121 no-scene-walk
   idle win and byte-identical offscreen evidence.
2. **Window-behavior threading (FR-003/005, P1).** Add the additive
   `ControlsElmish.runInteractiveAppWithWindowBehavior` overload (the viewer layer already exposes
   `runInteractiveViewerWithWindowBehavior` + `ViewerWindowBehaviorRequest.StartupState`) and wire
   the generated `Program.fs` (app profile) to thread the parsed `--window-startup` into the actual
   launch, mirroring the game profile — so the documented scaffold-map remedy stops being inert.
3. **CustomControl honesty + NRE guard (FR-006/007, P2).** Null-guard `CustomControl.validate`/
   `create`; correct the catalog text + `fs-skia-ui-widgets` skill to state that `renderTree` paints
   a labeled placeholder (custom content is not rasterized) and to point at the primitive-control
   recipe.
4. **Doc/governance papercuts (FR-008/009/010/011/012, P2–P3).** `evidence-formats.md` token-shape;
   `scaffold-map.md` additive-files note; `tasks-template.md` widgets resolved-`name:` trap;
   `fs-skia-viewer-host` black-frame section; an optional no-dependency property-test note.

**Deferred (recorded):** FR-004 public present-sync/buffer-count knobs (FR-001 removes the need;
an internal `bufferFillDepth` constant suffices); behavioral CustomControl painting; the
generalizable formula-engine/grid recipes (SkillSupport triage).

## Technical Context

**Language/Version**: F# / .NET 10
**Primary Dependencies**: No new dependencies. Touches SkiaSharp/OpenGL host (`src/SkiaViewer/Host/OpenGl.fs`),
`FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.Controls`, the `dotnet new fs-skia-ui` template, governance
docs, and the `.agents`/`.claude` skill tree.
**Testing**: Expecto (pure `planPresent` golden, present-action host log, CustomControl null-guard,
parity), FAKE governance targets, generated-product evidence. Wayland windowed-fullscreen visual
blink is **not reproducible headless** → disclosed `[-]` manual item (NOT `[S]`).
**Target Platform**: Windows + Linux (defect is Linux/Wayland-specific; fix is backend-general and
must not regress X11/headless or the Vulkan path).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Source + docs + Spec Kit assets change. The `dotnet new fs-skia-ui`
  template changes (generated `Program.fs` app-profile launch threading; `docs/scaffold-map.md`;
  `docs/evidence-formats.md`) and the `.specify/templates/tasks-template.md` advisory changes — all
  require `TemplateCheck` + `GeneratedProductCheck` to stay green and a post-merge template re-pin
  (`/fs-skia-template-update`). No `.template.config/template.json` symbol/identity change. Library
  package *contents* change (host present path, `ControlsElmish` overload) → version bump + re-pack.
- **Dependency impact**: N/A — no dependency change. `Directory.Packages.props`/`docs/dependencies.md`/
  `DependencyReport` untouched (no new package reference; the property-style CustomControl test uses
  hand-rolled deterministic loops, no FsCheck add).
- **Command-surface impact**: No new build target. Run `./fake.sh build -t Route` first; the change
  escalates (template/**, public `src/**/*.fsi`, governance docs, `.specify/**`, skill tree) to the
  maintainer-verify path. Required (sequential, deterministic FAKE order): `Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` →
  `EvidenceAudit`, plus the controls/package-surface gates and `SkillSyncCheck` (skill-tree edits).
  FAKE-backed commands are not concurrency-safe — run one at a time.
- **Generated project impact**: Generated `Program.fs` (app profile) now threads parsed window
  behavior into the live controls launch; generated `docs/scaffold-map.md` + `docs/evidence-formats.md`
  gain documentation; the generated `fs-skia-ui-widgets` skill gains the CustomControl honesty note.
  Generated `Dev` behavior, placeholder/excluded-history scans unchanged. Default (no window flag)
  launch path is byte-identical.
- **Evidence paths**: `specs/122-spread3-consumer-feedback/` — `research.md`, `data-model.md`,
  `contracts/public-surface-delta.md`, `quickstart.md`, `tasks.md`; readiness under
  `specs/122-spread3-consumer-feedback/` window-visibility + generated-validation evidence; test
  output from the new Expecto suites (`tests/SkiaViewer.Tests` present-path, `tests/Controls.Tests`
  CustomControl); `evidence-audit.md` + `generated-validation.md` verdicts; `TemplateCheck` /
  `GeneratedProductCheck` artifacts under `artifacts/template-check/122-*`.
- **`.fsi` / contract impact**: One additive public signature —
  `ControlsElmish.runInteractiveAppWithWindowBehavior` in `src/Controls.Elmish/ControlsElmish.fsi`
  (+ XML doc). `GlHost.planPresent` + `PresentAction` exposed in `OpenGl.fsi` for the idle-transition
  test (mirrors the existing `shouldPresent`/`shouldAdvanceFrame` test seams). Surface baselines move;
  package-surface + XML-doc gates apply. CustomControl stays surface-stable (internal guard + catalog
  string only). Catalog description string change regenerates `docs/controls-catalog.md`.
- **MVU/effect boundary**: No change to the MVU `Model`/`Msg`/`Effect`/`update` contract. The present
  path and window-behavior threading are host-loop/launch concerns; `planPresent` is a **pure**
  decision function (no I/O); the cached-frame blit + `SwapBuffers` is the real interpreter at the
  host boundary, evidenced by the present-action host log.
- **Synthetic evidence**: None planned — no mocks/fakes/`[S]`. The one un-reproducible item (Wayland
  windowed-fullscreen visual blink) is disclosed as a `[-]` manual/deferred observation with rationale
  (no Wayland windowed-fullscreen compositor in CI), NOT a synthetic pass. `EvidenceAudit` must report
  0 synthetic.
- **Test evidence**: Failing-first — (a) `planPresent` golden asserting
  `PaintAndPresent → RepresentLastGood×(bufferFillDepth-1) → SkipPresent…` (red before the function
  exists); (b) host present-action log proving a static scene presents a populated buffer every frame
  (no undrawn buffer) while steady-state reaches `SkipPresent` (idle preserved); (c) offscreen
  byte-identical golden (readback path untouched); (d) CustomControl null-`Id`/null-effect
  validate/create no-throw test; (e) controls overload parity (default == `runInteractiveApp`);
  (f) governance tests for the doc/token edits where applicable.
- **Observability**: The host increments `skippedPresentCount` (idle) and now a `representedCount`
  (bounded re-present) — surfaced via the present-action log/diagnostics so a regression that
  reintroduces undrawn-buffer skips is visible. Unsupported-environment messaging unchanged.
- **Deferred scope**: FR-004 public present-sync/buffer-count knobs; behavioral CustomControl
  painting; deeper `GeneratedProduct.fs` skill-name-substitution fix (FR-010 ships doc-only);
  end-to-end Wayland visual confirmation; SkillSupport triage of the formula-engine/grid recipes.

**Initial Constitution Check: PASS** — additive `.fsi` only (Tier 1 handled via overload, no
breaking change), `.fsi`-first visibility preserved, MVU boundary intact, no synthetic evidence,
failing-first tests defined.

## Project Structure

```
specs/122-spread3-consumer-feedback/
  spec.md  research.md  data-model.md  quickstart.md  source-feedback.md
  contracts/public-surface-delta.md
  checklists/requirements.md
  tasks.md                 # (created by /speckit-tasks)

src/SkiaViewer/Host/OpenGl.fs            # FR-001/002: planPresent + bounded re-present + cached frame
src/SkiaViewer/Host/OpenGl.fsi           # expose PresentAction + planPresent (test seam)
src/Controls.Elmish/ControlsElmish.fs    # FR-005: runInteractiveAppWithWindowBehavior impl
src/Controls.Elmish/ControlsElmish.fsi   # FR-005: additive signature + XML doc
src/Controls/CustomControl.fs            # FR-006: null guards in validate/create
src/Controls/Catalog.fs                  # FR-007: honest custom-control description
docs/controls-catalog.md                 # FR-007: regenerated

template/base/src/Product/Program.fs     # FR-005: app-profile launch threads windowBehaviorRequest
template/base/docs/scaffold-map.md        # FR-009: additive-files note
template/base/docs/evidence-formats.md    # FR-008: key=value token shapes
.specify/templates/tasks-template.md      # FR-010: widgets resolved-name advisory

.agents/skills/fs-skia-viewer-host/SKILL.md   # FR-011: black-frame section
.agents/skills/fs-skia-ui-widgets/SKILL.md    # FR-007/012: CustomControl + no-dep property-test note
template/product-skills/fs-skia-ui-widgets/SKILL.md  # mirror of FR-007 note for generated products
# .claude/** skill mirrors regenerated via RefreshSurfaceBaselines (SkillSyncCheck)

tests/SkiaViewer.Tests/...               # planPresent golden + present-action host log
tests/Controls.Tests/...                 # CustomControl null-guard; catalog honesty
```

## Phase 0 — Research

Complete. See [research.md](./research.md): dogfood-verify verdict table + Decisions 1–4. All spec
deferrals (FR-004 knob set, FR-007 doc-vs-behavioral) resolved: FR-004 deferred, FR-007 doc-fix.

## Phase 1 — Design & Contracts

- **Data model**: [data-model.md](./data-model.md) — `PresentAction` DU, the `planPresent` decision,
  the host's `idleRepresentsRemaining`/`lastGoodFrame`/`bufferFillDepth` state, and the additive
  controls overload.
- **Contracts**: [contracts/public-surface-delta.md](./contracts/public-surface-delta.md) — the exact
  `.fsi` additions and the catalog/doc string deltas.
- **Quickstart**: [quickstart.md](./quickstart.md) — how to verify each FR.
- **Agent context**: `AGENTS.md` SPECKIT block updated to point at this plan.

**Post-Design Constitution Re-check: PASS** — design keeps the surface additive, the present
decision pure, the offscreen path untouched (byte-identical), and discloses the single
un-reproducible item honestly.

## Phase 2 — Task strategy (for /speckit-tasks)

Order: failing-first tests → present-path core (FR-001/002) → controls overload + template threading
(FR-003/005) → CustomControl guard + catalog (FR-006/007) → doc/governance/skill edits
(FR-008/009/010/011/012) → regenerate catalog + skill mirrors → routed gate set + EvidenceAudit.
Each FR maps to ≥1 task with a skill-loading-evidence row; the Wayland visual item is a single `[-]`
disclosed task.
