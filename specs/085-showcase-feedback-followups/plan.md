# Implementation Plan: ControlsShowcase Consumer Feedback Follow-ups

**Branch**: `085-showcase-feedback-followups` | **Date**: 2026-06-09 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/085-showcase-feedback-followups/spec.md`

## Summary

One consolidated follow-ups feature delivering, from the sibling `ControlsShowcase`
consumer feedback, **two heavy framework capabilities** — a real nested-`Control`-tree
renderer (`Control.renderTree`, FR-001, FR-002, FR-003) and a pointer-routing, size-aware
durable host (`InteractiveAppHost` + `Viewer.runInteractiveApp`, FR-004, FR-005, FR-006, FR-009) —
**two small code fixes** — `KeyboardInput.normalize` toolkit key-name families
(FR-007, FR-008) and the size-aware view that removes upscaling blur (FR-009, FR-010) — **and**
the documentation/skill corrections, including a **new `fs-skia-viewer-host` skill**
(FR-011, distinct-named to avoid colliding with the existing package-owned
`fs-skia-skiaviewer`) plus updates to `fs-skia-typed-controls`, `scaffold-map.md`,
`spec-template.md`, `evidence-formats.md`, and `speckit-specify` (FR-012..016).

**Approach**: every public addition is **additive** — `Control.render`/`Widget.render`
(Feature-080 preview) and `Viewer.runApp`/`GeneratedAppHost` (durable `GovernanceTests`
literal) are left byte-for-byte intact, satisfying FR-003/FR-006. The pointer path
reuses the already-shipped `Controls.Elmish` pipeline (hit-test × `EventBindings` by
`ControlId`, 4px click/drag fold) rather than reimplementing it. See
[research.md](./research.md) for the resolved decisions (D0–D6),
[data-model.md](./data-model.md), [contracts/](./contracts/public-surface.fsi.md), and
[quickstart.md](./quickstart.md).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: existing only — `FS.Skia.UI.Scene`, `FS.Skia.UI.Layout`
(Yoga), `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.KeyboardInput`,
`FS.Skia.UI.SkiaViewer`, SkiaSharp 4 preview. **No new dependency.**
**Testing**: Expecto semantic tests (Controls, KeyboardInput, SkiaViewer/Controls.Elmish),
golden/parity goldens for `renderTree` distinctness, FSI transcripts, generated-product
evidence; the escalated six-target FAKE order.
**Target Platform**: Windows and Linux (headless evidence path where live pointer
injection is unavailable — see research D6).
**Change classification**: **Tier 1 — escalated `maintainer-verify`**, triggered by the
implementation diff (new `src/Controls/**/*.fsi` + `src/SkiaViewer/SkiaViewer.fsi`
surface, `template/**` doc edits, new `.agents/skills/**`, governance-template edits).
On the current spec-only diff `Route` reports `focused-authority`; re-run `Route` after
the contract-bearing edits land (research D0).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Post-Phase-1 re-evaluation: PASS.** No new violations; the additive-surface design
keeps both preservation constraints (FR-003 preview goldens, FR-006 `runApp` literal)
and introduces no complexity requiring justification under Principle III.

### Repository Governance Decisions

- **Template ownership**: Template-affecting. `template/base/docs/scaffold-map.md`
  (FR-010, FR-013) and `template/base/docs/evidence-formats.md` (FR-015) change; the new
  capabilities are framework-package surface consumed by generated products. No
  `.template.config/template.json` content-manifest change is required (no new template
  file class added — edits are to existing shipped docs); `TemplateCheck`/`TemplateDrift`
  exercise the doc edits.
- **Dependency impact**: **Amended (research D3-AMEND)** — no new *package* dependency
  (`Directory.Packages.props`/`docs/dependencies.md` unchanged), but the interactive host
  moved from `SkiaViewer` to `Controls.Elmish` (the only acyclic home, since
  `PointerInteraction`/`interpretPointerOutcome` are Controls-package surface and the viewer
  is intentionally host-independent). `Controls.Elmish.fsproj` therefore gains a `SkiaViewer`
  ProjectReference; `DependencyReport` + per-package surface move accordingly.
- **Command-surface impact**: No new FAKE target. `RefreshSurfaceBaselines` MUST run
  (new public `.fsi` surface + new `.agents/skills/fs-skia-viewer-host`); the escalated
  serialized order applies — FAKE-backed targets are run **sequentially** in the
  deterministic order: `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`. `TargetMetadataDrift`
  re-checked (no gate added). FAKE-backed commands share `.fake` state — never concurrent.
- **Generated project impact**: Generated products gain `Control.renderTree`,
  `Viewer.runInteractiveApp`/`InteractiveAppHost`, and the fixed `normalize`; selected-
  Controls guidance gains the new `fs-skia-viewer-host` skill and the `fs-skia-typed-
  controls` consumer note. No change to default/minimal generated *contents* or generated
  `Dev` behavior beyond the new surface; placeholder/excluded-history scans unaffected.
- **Evidence paths**: `specs/085-showcase-feedback-followups/` —
  `evidence/render-distinctness/*.png` + diff (SC-001); `evidence/pointer-dispatch.md`
  (`key=value`, SC-002); `evidence/normalize-mapping.md` + test log (SC-003);
  `evidence/size-aware-render/*.png` + blur-workaround note (SC-004); the window-
  visibility evidence class (`interactive-visible-window.md`, `close-reason-separation.md`,
  `window-state-diagnostics.md`, `window-options.md`, `generated-validation.md`,
  `real-image-evidence.md`, feature-local `evidence-audit.md`) authored as `key=value`
  blocks (FR-015); surface baselines + per-package `.fsi.txt`; `RefreshSurfaceBaselines`
  log; `EvidenceGraph`/`EvidenceAudit` output.
- **`.fsi` / contract impact**: Additive public surface — `Control.renderTree`
  (`src/Controls/Control.fsi`), `InteractiveAppHost` + `Viewer.runInteractiveApp`
  (`src/SkiaViewer/SkiaViewer.fsi`). `KeyboardInput` `.fsi`/`ViewerKey` **unchanged**
  (behavior-only `normalize` fix). Surface-area baselines + per-package snapshots updated.
  Preserved unchanged: `Control.render`/`Widget.render` (080 goldens) and
  `GeneratedAppHost`/`Viewer.runApp` (GovernanceTests literal). Compatibility: purely
  additive — no migration required for existing consumers.
- **MVU/effect boundary**: I/O-bearing (pointer input + host loop). `Model`/`Msg` are
  consumer-owned (`'model`/`'msg`); `Msg` produced by `MapKey`/`MapPointer`/`Tick`.
  Pointer events become **data** (`PointerInteraction`, post hit-test + 4px fold) before
  `MapPointer`. `Effect` = `ViewerEffect list`; `init` = `host.Init`; `update` =
  `host.Update` (**pure**); the interpreter edge is `runInteractiveApp` (executes effects,
  drives render/dispatch). Reuses `ControlsElmish.interpretPointerOutcome`. Evidence:
  pure pointer-routing transition tests + headless host dispatch (research D6).
- **Synthetic evidence**: Where the headless host lacks live key/pointer injection,
  synthetic-event-through-the-real-adapter is the honest bar and is **not** `[S]` (it
  exercises the real host/adapter path, not literal fixtures). Any wholly-literal fixture
  (e.g. a canned `PointerInteraction` not delivered through the host) MUST carry `[S]`
  disclosure per Principle V. No `[SEH]` anticipated.
- **Test evidence**: Failing-first semantic tests — `renderTree` distinctness golden
  (two trees differ; nested children painted), pointer-dispatch host test (msg + model
  change), `normalize` mapping test (five spellings + unknown-regression), size-aware
  render test (two extents, no fixed-size upscale). Governance tests: surface baselines,
  `SkillSyncCheck`/`SkillQualityCheck`, `ControlFidelityCheck` (080 goldens still green).
- **Observability**: Pointer routing emits `ReportAdapterDiagnostic` for pointer
  diagnostics (existing `interpretPointerEffect` path); the host surfaces window-state
  diagnostics already. Missing-artifact-class failures and unsupported-environment
  (no live injection) messages are explicit, not silent (Principle VII).
- **Deferred scope**: Out of scope per spec — a compiled `SkillistIdResolution` gate and
  promoting the consumer's page-tour evidence helper into `fs-skia-evidence-mode`. No new
  windowing toolkit, platform/distribution targets, or Feature-080 preview redesign.
  Full live (non-synthetic) pointer injection is deferred where the environment lacks it.

## Project Structure

```
specs/085-showcase-feedback-followups/
  spec.md
  plan.md                      # this file
  research.md                  # Phase 0 — decisions D0–D6
  data-model.md                # Phase 1 — surface deltas + MVU boundary
  contracts/
    public-surface.fsi.md      # Phase 1 — .fsi deltas to draft in FSI first
  quickstart.md                # Phase 1 — build/validate/FSI/evidence
  evidence/                    # captured during /speckit.implement

# Framework source touched (implementation phase)
src/Controls/Control.fsi        # + Control.renderTree
src/Controls/Control.fs         # renderTree body (real Yoga layout + paint)
src/SkiaViewer/SkiaViewer.fsi   # + InteractiveAppHost, Viewer.runInteractiveApp
src/SkiaViewer/SkiaViewer.fs    # host variant + pointer routing loop
src/KeyboardInput/KeyboardInput.fs   # normalize: Number*/Digit*/Keypad*/Key* families
# tests: Controls.Tests, SkiaViewer.Tests (or Controls.Elmish.Tests), KeyboardInput.Tests

# Docs / skills / governance (implementation phase)
.agents/skills/fs-skia-viewer-host/SKILL.md   # NEW (+ generated .claude mirror)
.agents/skills/fs-skia-typed-controls/SKILL.md
.agents/skills/speckit-specify/SKILL.md
template/base/docs/scaffold-map.md
template/base/docs/evidence-formats.md
.specify/templates/spec-template.md
# (regenerate .claude tree + skillist-reference via RefreshSurfaceBaselines)
```
