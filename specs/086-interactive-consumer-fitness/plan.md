# Implementation Plan: Interactive Non-Game Consumer Fitness

**Branch**: `086-interactive-consumer-fitness` | **Date**: 2026-06-09 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/086-interactive-consumer-fitness/spec.md`

## Summary

Make `FS.Skia.UI` a first-class host for an **interactive, non-game** controls
application by closing the six fitness gaps the ControlsShowcase1 consumer hit after
085. Seven design decisions (research D1–D7) implement sixteen FRs:

- **Neutral controls-first scaffold** (FR-001/002/003): replace the Tetris scaffold
  model/view with neutral names and a default `view` that rasterizes a real `Control`
  tree via `Control.renderTree` — keeping the durable governance/evidence tokens.
- **Pointer host as the controls-family governed default** (FR-004/005/006): branch the
  template on a product-family marker; controls default to the shipped 085
  `runInteractiveApp` pointer host; generalize the host-lock governance assertion;
  the game family keeps its keyboard host.
- **Multi-axis `renderTree`** (FR-007/008/009): honor a horizontal `Stack` orientation,
  key child bounds by a collision-free structural id so unkeyed same-kind siblings stop
  overlapping, and reflect explicit container size.
- **Per-`ControlId` bounds + hit-test** (FR-011/012): surface the evaluated bounds
  `renderTree` already computes-and-discards; add `Control.hitTest`.
- **Scene primitives** (FR-013/014): a `Translate` offset wrapper and a `SizedText` node.
- **Viewer key warm-up** (FR-015/016): a bounded pre-ready buffer so no early keystroke
  is dropped; documented in `fs-skia-viewer-host`.

Tier 1 (contracted change). The 080 single-control preview and the game family's
persistent-launch guarantee are explicitly preserved (FR-010, SC-008).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: SkiaSharp 4 preview (pinned); `FS.Skia.UI.Scene`,
`FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.SkiaViewer`,
`FS.Skia.UI.Layout` (all existing; no new package identities)
**Testing**: Expecto + FsCheck (semantic/property), FSI transcripts through the packed
libraries, generated-product evidence, golden-diff parity (DiffPlex) for the preserved
080 preview goldens
**Target Platform**: Windows and Linux (live-window evidence needs GPU passthrough /
a compiled host; render-target PNGs are headless)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Initial evaluation: PASS.** Tier 1 change; full artifact chain present (spec, this
plan, `.fsi` deltas in `contracts/`, surface baselines, semantic tests, doc/skill
update). No constitutional violation requiring justification. The only complexity-budget
item (Principle III) is the `SceneNode` additive cases and a `Stack` orientation
attribute — all plain unions/attributes, no SRTP/reflection/type-providers. The
viewer warm-up buffer uses a `mutable` bounded queue on the host input hot path,
disclosed at the use site per Principle III.

**Post-design re-evaluation: PASS.** Phase 1 introduces no new public-surface
construct beyond additive cases/fields/builders and one host helper; the MVU boundary
(Principle IV) is satisfied by reusing the shipped `InteractiveAppHost`/`runInteractiveApp`
update/effect seam — no new effect algebra. No `[S]` synthetic evidence is required for
the headless-testable FRs; the live-window FRs (SC-002/003/007) have a real compiled-host
evidence path (research cross-cutting section), so Principle V disclosure is **not**
triggered unless the GPU host is unavailable at capture time (then `[S]`/`[SEH]` per V).

### Repository Governance Decisions

- **Template ownership**: **Required.** The generated template changes substantially —
  neutral scaffold model/view (`template/base/src/Product/Model.fs`, `View.fs`),
  re-pointed durable evidence files (`LayoutEvidence.fs`, `EvidenceCommands.fs`,
  `Program.fs`, `WindowOptions.fs`), the controls-family default launch
  (`runInteractiveApp`), the generalized host-governance assertions
  (`GovernanceTests.fs`/`BehaviorTests.fs`), and a product-family marker on the existing
  `//#if (profile == ...)` machinery. `.template.config/template.json` is updated if the
  family marker adds a generation parameter; otherwise the change rides existing profile
  switches. `TemplateCheck`/`GeneratedProductCheck` exercise the neutral scaffold +
  controls-first default `view`.
- **Dependency impact**: **N/A — no dependency change.** No new NuGet packages, no
  `Directory.Packages.props` edit, no `docs/dependencies.md` / `DependencyReport` change;
  all work is within existing `FS.Skia.UI.*` packages.
- **Command-surface impact**: **Required (validation only, no new targets).** No new
  `build.fsx` target; run the escalated serialized order
  (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `EvidenceGraph` → `EvidenceAudit`) plus `Route --enforce` and
  `RefreshSurfaceBaselines`. FAKE-backed commands share `.fake` state and are **not**
  concurrency-safe — run them **sequentially** in the deterministic order above; safe
  non-FAKE reads/greps may parallelize.
- **Generated project impact**: **Required.** Default generated contents change (neutral
  model/view, real-control default `view`, controls-family pointer-host launch,
  rewritten `BehaviorTests.fs`). Placeholder/excluded-history scans must still pass on
  the neutral source; the durable must-survive source-scan tokens
  (`--scene-evidence`, `SceneEvidence.render`, `RendererMode = "deterministic-scene"`,
  visual-evidence honesty vocabulary) MUST remain present (`scaffold-map.md` §
  Must-survive). Selected-Controls guidance and the `fs-skia-viewer-host` local skill
  (warm-up note) update.
- **Evidence paths**: readiness artifacts —
  `readiness/neutral-scaffold-grep.txt` (SC-001 zero game tokens),
  `readiness/real-controls-render.png` + `.metadata.txt` (SC-002 headless render-target
  helper evidence via `controlsExampleView → Control.renderTree`),
  `readiness/real-controls-live-screenshot.png` + `.metadata.txt` (SC-002 production
  render path, real controls, persistent launch),
  `readiness/pointer-dispatch.txt` (SC-003 via `routeInteractivePointer`),
  `readiness/rendertree-sidebyside-bounds.txt` (SC-004 non-overlap + explicit size),
  `readiness/percontrol-bounds-hittest.txt` (SC-005),
  `readiness/scene-translate-sizedtext.txt` (SC-006),
  `readiness/key-warmup-delivery.txt` (SC-007 compiled-host keystroke delivery),
  plus FSI transcripts, packed-library test logs, surface-baseline diffs, the
  `Route`/six-target logs, and `RefreshSurfaceBaselines` output. The live-window PNGs
  require a compiled self-closing host (fsi cannot open a Vulkan window).
- **`.fsi` / contract impact**: **Required (Tier 1).** `src/Scene/Scene.fsi`
  (`Translate`, `SizedText` cases + `translate`/`sizedText` + descriptors);
  `src/Controls/Types.fsi` (`ControlRenderResult.Bounds`); `src/Controls/Control.fsi`
  (`hitTest`, `Stack.orientation`); possibly an additive `src/SkiaViewer/SkiaViewer.fsi`
  readiness diagnostic. All additive (no field meaning changes; `Layout` field kept).
  Per-package and cross-package surface baselines recapture; compatibility note: purely
  additive, no consumer break; the 080 preview path is byte-identical (FR-010).
- **MVU/effect boundary**: The interactive default launch is stateful/I/O-bearing but
  reuses the **already-shipped** `InteractiveAppHost` seam — `Model`/`Msg` are the
  generated product's; `Update: 'msg -> 'model -> 'model * ViewerEffect list` and the
  `MapPointer`/`MapKey` routers are the boundary; `runInteractiveApp` is the edge
  interpreter; `routeInteractivePointer` is the pure, window-free transition under test.
  No new effect algebra. The viewer warm-up buffer is host-edge plumbing (not product
  state).
- **Synthetic evidence**: None planned for headless-testable FRs (FR-007..FR-014 and the
  pointer route all have real evidence). The only `[S]` risk is the live-window captures
  (SC-002/003/007) **if** the GPU/window host is unavailable at capture — then the task
  is marked `[S]` (or `[SEH]` only if it validates an unsupported-host error path), the
  unsupported-host reason is recorded, and the real compiled-host path stays documented
  per Principle V. No mocks/fakes substitute for the layout/bounds/scene logic.
- **Test evidence**: failing-first Expecto semantic tests per FR — horizontal-Stack row
  layout (FR-007), unkeyed same-kind non-overlap + explicit size (FR-008/009),
  `Bounds`/`hitTest` resolution (FR-011/012), `translate` uniform offset over
  `Path`/`Chart` + composition + `sizedText` fit (FR-013/014), 080-preview golden parity
  (FR-010), pointer dispatch via `routeInteractivePointer` (SC-003); generalized
  governance assertions on the neutral scaffold (`GovernanceTests.fs`) + rewritten
  `BehaviorTests.fs`; a compiled-host smoke for warm-up delivery (SC-007).
- **Observability**: the warm-up buffer emits a structured diagnostic on drop-oldest
  past its cap (Principle VII — explicit degradation, no silent loss); the host already
  emits input-mapping-unavailable warnings (`SkiaViewer.fs:1512-1518`). Missing
  live-window evidence fails its artifact-class with an unsupported-host reason rather
  than silently passing. `renderTree` keeps its `Diagnostics` channel.
- **Deferred scope**: the four minor consumer clusters are **out of scope** here
  (external-tree source-spec snapshotting; skillist-name registry validator;
  typed-front-door catalog/probe discoverability; `/speckit-implement` run/verify
  discipline) — tracked as candidate follow-on `/speckit-specify` runs. No new platforms,
  distribution, or release automation. An input-ready *signal* (FR-015's "or" branch)
  beyond the buffer is deferred unless a consumer asks for it.

## Project Structure

```
specs/086-interactive-consumer-fitness/
├── spec.md
├── plan.md                  # this file
├── research.md              # Phase 0 — D1–D7 design decisions
├── data-model.md            # Phase 1 — entities/shape changes
├── contracts/
│   ├── scene-primitives.fsi.md       # FR-013/014 Scene deltas
│   ├── controls-rendertree.fsi.md    # FR-007..012 Controls deltas
│   └── host-and-scaffold.md          # FR-001..006, 015/016 template+host+warm-up
├── quickstart.md            # Phase 1 — validate + entry points
└── checklists/

Framework source touched:
  src/Scene/Scene.fsi · Scene.fs                       # Translate, SizedText (FR-013/014)
  src/Controls/Types.fsi                               # ControlRenderResult.Bounds (FR-011)
  src/Controls/Control.fsi · Control.fs                # hitTest, Stack.orientation, multi-axis
                                                       #   layout, collision-free keying (FR-007..012)
  src/SkiaViewer/SkiaViewer.fs (· .fsi maybe)          # key warm-up buffer (FR-015/016)
  .agents/skills/fs-skia-viewer-host/SKILL.md          # warm-up doc (regen → .claude)

Generated template touched:
  template/base/src/Product/Model.fs · View.fs         # neutral scaffold + real-control view (FR-001/003)
  template/base/src/Product/LayoutEvidence.fs ·        # re-point game→content region, keep tokens (FR-002)
    EvidenceCommands.fs · Program.fs · WindowOptions.fs
  template/base/tests/Product.Tests/GovernanceTests.fs # generalized host-lock assertion (FR-005)
  template/base/tests/Product.Tests/BehaviorTests.fs   # rewritten for neutral model
  template/base/.template.config/template.json         # product-family marker (if a new param)
```

## Implementation Phasing (story-ordered, P1 → P3)

Aligns with the spec's prioritized user stories so the highest-value fix lands first.

1. **US1 (P1) — Neutral controls-first scaffold** (FR-001/002/003): rename the scaffold
   model/view, re-point durable evidence files keeping their tokens, make the default
   `view` rasterize `controlsExampleView` via `renderTree`, rewrite `BehaviorTests.fs`.
   Independent test: neutral-grep returns none + governance passes + default app shows
   real controls.
2. **US3 (P1) — Multi-axis layout** (FR-007/008/009/010): `directionOf` reads
   orientation; collision-free structural keying; explicit-size assertion; 080-golden
   parity. *Precedes* US2 so the pointer host hit-tests a correct layout.
3. **US2 (P1) — Pointer host as governed default** (FR-004/005/006): family marker;
   controls default → `runInteractiveApp`; generalize the host-lock assertion; game
   family unchanged. Headless dispatch via `routeInteractivePointer`.
4. **US4 (P2) — Per-`ControlId` bounds + hit-test** (FR-011/012): surface
   `ControlRenderResult.Bounds`; add `Control.hitTest`.
5. **US5 (P2) — Scene primitives** (FR-013/014): `Translate` + `SizedText`.
6. **US6 (P3) — Viewer key warm-up** (FR-015/016): bounded pre-ready buffer + skill doc.

Then: `RefreshSurfaceBaselines`, recapture Scene+Controls surface baselines, run the
escalated serialized six-target order, capture readiness evidence.
