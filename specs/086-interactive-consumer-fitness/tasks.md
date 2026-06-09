# Tasks: Interactive Non-Game Consumer Fitness

**Feature branch**: `086-interactive-consumer-fitness`
**Spec**: `specs/086-interactive-consumer-fitness/spec.md`
**Plan**: `specs/086-interactive-consumer-fitness/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed by the evidence audit, never written by hand. See
`readiness/task-graph.md` for the propagated view.

No `[SEH]` synthetic error-handling task is approved for this feature. Per the
plan, the only `[S]` risk is the live-window captures (SC-002/003/007) **if** the
GPU/window host is unavailable at capture time; such a task is marked `[S]` with a
recorded unsupported-host reason — not `[X]`, not `[SEH]`.

## Vertical-slice rule (US phases)

A `[US*]` task may only be `[X]` when the change is reachable from a user-facing
entry point and that path was actually exercised — an FSI session against the
packed library, a render-target PNG / live screenshot under `readiness/`, or a
compiled-host run. Core/layout/model changes whose unit tests pass green do **not**
satisfy `[X]` alone. For the stateful US2 (pointer host) story, `[X]` also requires
MVU evidence: the pure `routeInteractivePointer` transition tested and the real
`runInteractiveApp` interpreter exercised via the persistent launch.

## Success-criterion → assertion mapping

| SC | Enforcing assertion / artifact |
|----|--------------------------------|
| SC-001 | Neutral-scaffold grep (`readiness/neutral-scaffold-grep.txt`) + durable `GovernanceTests.fs` stays model-agnostic, compiles before replaceable `BehaviorTests.fs` |
| SC-002 | Production-path real-controls render (`readiness/real-controls-render.png`) + live screenshot (`readiness/real-controls-live-screenshot.png`) |
| SC-003 | `routeInteractivePointer` dispatch test + governance passes with pointer host default (`readiness/pointer-dispatch.txt`) |
| SC-004 | Horizontal-Stack non-overlap + explicit-size Expecto test (`readiness/rendertree-sidebyside-bounds.txt`) |
| SC-005 | `ControlRenderResult.Bounds` + `Control.hitTest` test (`readiness/percontrol-bounds-hittest.txt`) |
| SC-006 | `translate`/`sizedText` uniform-offset + fit test (`readiness/scene-translate-sizedtext.txt`) |
| SC-007 | Compiled-host keystroke-delivery smoke (`readiness/key-warmup-delivery.txt`) |
| SC-008 | Generalized host-lock assertion: game family still passes with the keyboard host |

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed targets
  are never marked `[P]` — they share `.fake` state and run sequentially.
- **[US1]**…**[US6]** — user-story scope.
- Tier: the whole feature is **Tier 1 (escalated / `maintainer-verify`)**, matching
  the spec's overall tier, so per-task `[T1]`/`[T2]` annotations are omitted.

Every task mirrors its structured `skillist` from `tasks.deps.yml` as
`[skillist: ...]`. The escalated serialized FAKE order (`Dev` →
`GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
`EvidenceGraph` → `EvidenceAudit`) is run sequentially in Phase 9.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the `086` feature directory and link spec + plan; run `./fake.sh build -t Route --enforce` to record the escalated `maintainer-verify` tier and the named required evidence artifacts
- [X] T002 [P] [skillist: fs-skia-template-update] Record the post-085 baseline this feature builds on (`FS.Skia.UI.* 0.1.91-preview.1`) and the packable-project + separate-track template version-bump-on-merge obligation
- [X] T003 [P] [skillist: []] Create `specs/086-interactive-consumer-fitness/readiness/` with audit-enforced discoverable placeholders — `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `real-image-evidence.md` — plus per-SC evidence stubs
- [X] T004 [skillist: fs-skia-evidence-mode] Record feature Tier (1, escalated), affected layers (Scene / Controls / Controls.Elmish / SkiaViewer + template), public-API impact, governance risk level, and required evidence obligations; note Principle IV (MVU) applies to **US2 only** and is satisfied by reusing the shipped `InteractiveAppHost`/`runInteractiveApp` seam (no new effect algebra)

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-scene] Draft the Scene `.fsi` deltas in `src/Scene/Scene.fsi` — additive `SceneNode.Translate` and `SceneNode.SizedText` cases, `translate`/`sizedText` constructors, and `TranslateElement`/`SizedTextElement` descriptors (FR-013/014 contract)
- [X] T006 [P] [skillist: fs-skia-ui-widgets] Draft the Controls `.fsi` deltas — `ControlRenderResult.Bounds : (ControlId * Rect) list` in `src/Controls/Types.fsi`, and `hitTest` + `Stack.orientation` in `src/Controls/Control.fsi` (FR-007/008/009/011/012 contract; `Layout` field kept for back-compat)
- [X] T007 [P] [skillist: fs-skia-skiaviewer] Draft any additive `src/SkiaViewer/SkiaViewer.fsi` warm-up readiness diagnostic surface (FR-015/016 contract)
- [X] T008 [skillist: []] Exercise the draft `.fsi` deltas from FSI against the packed libraries (representative `renderTree → Bounds/hitTest` and `translate`/`sizedText` paths) and capture `readiness/fsi-session.txt`
- [X] T009 [skillist: []] Capture pre-change per-package and cross-package surface baselines for the Scene and Controls modules (and SkiaViewer if its `.fsi` changes)
- [X] T010 [P] [skillist: fs-skia-evidence-mode] Author the governance/runtime/honesty readiness content into the Phase-1 placeholders — `governance-risk-levels.md`, `runtime-limitations.md`, `aggregate-hang-diagnostics.md`, `generated-guidance-validation.md`, `visual-evidence-honesty.md`, `window-visibility.md`, `real-image-evidence.md` — each naming the authoritative command, artifact path, failure class, next action, and the live-window-vs-render-target host-warning classification

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — Neutral, controls-first scaffold (US1)

### Tests First (Principle I, Principle VI)

- [X] T011 [P] [US1] [skillist: fs-skia-template-update] Rewrite `template/base/tests/Product.Tests/BehaviorTests.fs` for the neutral model — assert real controls render; drop the "grid-style playfield" / "tally/stage/upcoming" / "circular entities" assertions — while keeping durable `GovernanceTests.fs` model-agnostic so it compiles before `BehaviorTests.fs` (SC-001 enforcement)

### Implementation

- [X] T012 [US1] [skillist: fs-skia-ui-widgets] Replace the game `Model.fs` with a neutral application model — `Page` navigation states, content-region cursor/selection, generic status fields (no `Initial|Options|Main|Paused|Ended`, `ActiveColumn`/`ActiveRow`/`Tally`/`Stage`/`NextToken`) per data-model §6 (FR-001)
- [X] T013 [US1] [skillist: fs-skia-scene] Rewrite the default `view` in `template/base/src/Product/View.fs` to rasterize the real `controlsExampleView` through `Control.renderTree` (production tree-render path), replacing the hand-drawn `Group([...])` rectangle/grid/text geometry (FR-003)
- [X] T014 [US1] [skillist: fs-skia-evidence-mode] Re-point the durable evidence files (`LayoutEvidence.fs`, `EvidenceCommands.fs`, `Program.fs`, `WindowOptions.fs`) from game/playfield → app/content-region framing while preserving every durable governance-scanned token (`--scene-evidence`, `SceneEvidence.render`, `RendererMode = "deterministic-scene"`, the visual-evidence honesty vocabulary, the window `diagnostic-class=*` facts) (FR-002)
- [X] T015 [US1] [skillist: []] Run the neutral-scaffold grep over the generated product source (model/view/tests) and capture `readiness/neutral-scaffold-grep.txt` — expect zero game tokens outside the durable governance tokens (SC-001)
- [X] T016 [US1] [skillist: fs-skia-skiaviewer] Capture the production-render-path real-controls evidence headlessly (render-target PNG of `controlsExampleView → Control.renderTree`) → `readiness/real-controls-render.png` + `.metadata.txt`, using honest "real controls, not placeholder geometry" vocabulary (SC-002 helper evidence; not a persistent-launch claim)

**Checkpoint**: A freshly generated project reads in neutral terms and renders real controls via the production path.

---

## Phase 4: User Story 3 — Real side-by-side layout from a control tree (US3)

*Precedes US2 so the pointer host hit-tests a correct layout.*

### Tests First

- [X] T017 [P] [US3] [skillist: fs-skia-ui-widgets] Add failing-first Expecto tests — a horizontal-orientation `Stack` lays children along the row axis (FR-007); two structurally similar **unkeyed** same-kind siblings receive distinct non-overlapping bounds at different x-coordinates (FR-008); an explicit container width/height is reflected in computed bounds (FR-009) (SC-004)
- [X] T018 [P] [US3] [skillist: fs-skia-ui-widgets] Add the Feature-080 single-control preview golden-parity test asserting `Control.render`/`Widget.render` output stays byte-identical (FR-010 regression guard)

### Implementation

- [X] T019 [US3] [skillist: fs-skia-ui-widgets] Make `directionOf` in `src/Controls/Control.fs` return `Row` for a horizontal-orientation `Stack` (and the documented horizontal kinds) so a side-by-side composition no longer collapses to a vertical column (FR-007)
- [X] T020 [US3] [skillist: fs-skia-ui-widgets] Replace the `Key ?? Kind` `Map` keying with a collision-free deterministic structural `LayoutNodeId` (derived from tree path / sibling index, preferring an explicit `Key`) threaded identically into layout and paint, so unkeyed same-kind siblings stop overlapping and an explicit container size surfaces (FR-008/009; data-model §4 — no clock/randomness, resume-safe)
- [X] T021 [US3] [skillist: fs-skia-ui-widgets] Confirm the 080 preview path is unchanged and capture the side-by-side non-overlap + explicit-size layout evidence via FSI/Expecto → `readiness/rendertree-sidebyside-bounds.txt` (SC-004, FR-010)

**Checkpoint**: `renderTree` lays a real rail+content composition side-by-side without overlap.

---

## Phase 5: User Story 2 — Pointer interaction in the governed default app (US2)

### Tests First

- [X] T022 [P] [US2] [skillist: fs-skia-elmish] Add a headless pointer-dispatch test via `ControlsElmish.routeInteractivePointer` — a synthetic press/release at a control's bounds dispatches that control's bound message and updates model state (pure transition under test; SC-003)
- [X] T023 [P] [US2] [skillist: fs-skia-template-update] Generalize the host-lock assertions in `GovernanceTests.fs` (`:105`) and `BehaviorTests.fs` (`:289`) from the literal `Viewer.runApp viewerOptions generatedHost` to "the per-family persistent interactive host" (controls → `runInteractiveApp`, game → `Viewer.runApp ... generatedHost`); assert the game family still passes (FR-005, SC-008)

### Implementation

- [X] T024 [US2] [skillist: fs-skia-template-update] Add the product-family marker (`controls` vs `game`) on the existing `//#if (profile == ...)` machinery; update `template/base/.template.config/template.json` only if the marker introduces a new generation parameter (FR-004, D6)
- [X] T025 [US2] [skillist: fs-skia-elmish] Set the controls-family default launch in `template/base/src/Product/Program.fs` to `ControlsElmish.runInteractiveApp options host`, keeping the game-family default `Viewer.runApp viewerOptions generatedHost` (FR-004/006)
- [X] T026 [US2] [skillist: fs-skia-skiaviewer] Persistent graphical launch of the controls-family default app via a compiled self-closing pointer host (reachable from the default executable path) — capture the live window showing real styled controls (SC-002) and a synthetic pointer press/release dispatching a control's message (SC-003) → `readiness/real-controls-live-screenshot.png` + `.metadata.txt` + `readiness/pointer-dispatch.txt`; record window visibility honestly (`deferred`/`observed:true`) in `readiness/window-visibility.md`

**Checkpoint**: A mouse click on a live control in the governed default app fires its action; governance passes with the pointer host as the controls-family default.

---

## Phase 6: User Story 4 — Map a pointer to a control from a Scene-based host (US4)

### Tests First

- [X] T027 [P] [US4] [skillist: fs-skia-ui-widgets] Add failing-first tests — `ControlRenderResult.Bounds` exposes every laid-out control's **evaluated** absolute box keyed by `ControlId`, and `Control.hitTest` resolves an inside point to that control's id and a gap point to `None` (SC-005)

### Implementation

- [X] T028 [US4] [skillist: fs-skia-ui-widgets] Populate `ControlRenderResult.Bounds` in `Control.fs:renderTree` from the already-computed-then-discarded `LayoutResult` bounds, keeping `Layout = root` for back-compat (FR-011, data-model §5)
- [X] T029 [US4] [skillist: fs-skia-ui-widgets] Add `Control.hitTest : ControlRenderResult<'msg> -> float -> float -> ControlId option`, layered over `Layout.hitTestComputed` (FR-012)
- [X] T030 [US4] [skillist: []] Capture per-control bounds + hit-test evidence via FSI/Expecto → `readiness/percontrol-bounds-hittest.txt` (SC-005)

**Checkpoint**: A Scene-based host can resolve, from the public render result alone, which control contains any point.

---

## Phase 7: User Story 5 — Compose and size sub-scenes (US5)

### Tests First

- [X] T031 [P] [US5] [skillist: fs-skia-scene] Add failing-first tests — `Translate` uniformly offsets every descendant kind including `Path`/`Points`/`Vertices`/`Chart`, and nesting composes (sum of offsets); `SizedText` renders at an explicit size while a bare `Text` keeps its current default-font rendering (SC-006, FR-014 back-compat / Edge Case)

### Implementation

- [X] T032 [US5] [skillist: fs-skia-scene] Add `SceneNode.Translate` (offset wrapper) + `Scene.translate` + `TranslateElement` descriptor in `src/Scene/Scene.fsi`/`Scene.fs`; render via a canvas translation around the child so all node kinds shift uniformly (FR-013, data-model §1)
- [X] T033 [US5] [skillist: fs-skia-scene] Add `SceneNode.SizedText` + `Scene.sizedText` + `SizedTextElement` descriptor routing through the same `TextRun`/`FontSpec` glyph layout; leave the existing `Text` case unchanged (FR-014, data-model §2)
- [X] T034 [US5] [skillist: fs-skia-scene] Capture translate + sized-text evidence (uniform offset over a sub-scene containing `Path`/`Chart`; a nav-rail label sized to its column without clipping) → `readiness/scene-translate-sizedtext.txt` (SC-006)

**Checkpoint**: A consumer offsets and sizes sub-scenes with framework primitives instead of a hand-rolled `shiftNode`.

---

## Phase 8: User Story 6 — No dropped keystrokes at window focus (US6)

### Tests First

- [S] T035 [P] [US6] [skillist: fs-skia-keyboard-input] Add a compiled-host warm-up smoke that issues a known keystroke sequence within a bounded window (≤2 s) after the window gains focus and asserts all are delivered to `MapKey` (none dropped) (SC-007)

### Implementation

- [S] T036 [US6] [skillist: fs-skia-skiaviewer] Add a bounded pre-ready FIFO in the `src/SkiaViewer/SkiaViewer.fs` host input path that buffers key events captured before the pipeline signals ready and flushes them in order once ready; past the cap it drops-oldest with a structured diagnostic (Principle VII — no silent loss), then dispatches directly (FR-015, data-model §9)
- [X] T037 [US6] [skillist: fs-skia-viewer-host] Document the keyboard warm-up window and the buffering mitigation in `.agents/skills/fs-skia-viewer-host/SKILL.md` (regenerated to `.claude` via `RefreshSurfaceBaselines`) (FR-016)
- [S] T038 [US6] [skillist: fs-skia-skiaviewer] Capture compiled-host keystroke-delivery evidence → `readiness/key-warmup-delivery.txt` (SC-007)

**Checkpoint**: Every keystroke issued in the first seconds after focus reaches the consumer's `MapKey`.

---

## Phase 9: Integration, Surface Baselines & Evidence Gates

*Escalated `maintainer-verify`. FAKE-backed targets share `.fake` state — run sequentially in the deterministic order below (never `[P]`).*

- [X] T039 [skillist: []] Regenerate the skill tree from the canonical `.agents` tree and refresh baselines via `./fake.sh build -t RefreshSurfaceBaselines`, then recapture the per-package and cross-package Scene + Controls `.fsi` surface baselines (additive deltas only)
- [X] T040 [skillist: []] Run `./fake.sh build -t Dev` — semantic/property tests green (horizontal-Stack, non-overlap, explicit size, `Bounds`/`hitTest`, `translate`/`sizedText`, 080 golden parity, `routeInteractivePointer`)
- [X] T041 [skillist: []] Run `./fake.sh build -t GeneratedGuidanceCheck`
- [X] T042 [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` — exercises the neutral scaffold + controls-first default `view`
- [X] T043 [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck` — record the known local env-failure as non-authoritative with its output
- [X] T044 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the DAG is acyclic, no dangling refs, and no `[S*]` surprises; confirm the echoed feature directory + task count match this feature
- [X] T045 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (synthetic propagation + diff-scan) or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the
source for the PR description's synthetic-evidence section. For `[SEH]` rows,
include the approval label, design-phase source, synthetic input class, expected
error behavior, and reviewer-visible acceptance status.

These `[S]` rows are **evidence-capture deferrals to an unsupported host**, exactly the
risk the plan anticipated (live-window / keystroke captures when GPU/window passthrough is
unavailable). They are **not** synthetic implementations — no mock/fake substitutes for the
layout/bounds/scene/host logic; the production code paths are real and compile, and the
headless render-target + headless MVU pointer routing carry real evidence. The live
compiled-host path stays documented for capture on a GPU/display host. None are `[SEH]`.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| ~~T026~~ | RESOLVED → [X]. Live controls-window captured on DISPLAY=:1 via the compiled self-closing harness (interactive-visible-window.md): window-visible=observed:true, first-frame-presented=true, self-closed via AppRequestedClose. | readiness/interactive-visible-window.md + readiness/supported-host-persistent-launch.txt + readiness/real-controls-render.png + readiness/pointer-dispatch.txt | — | n/a (not [SEH]) | n/a | n/a | n/a | n/a |
| T035 | Compiled-host warm-up keystroke smoke needs native keystroke INJECTION within the focus window (085 also deferred live native injection). The bounded pre-ready FIFO is real, deterministic, single-threaded host code that builds; the live window itself is proven (interactive-visible-window.md). | src/SkiaViewer/SkiaViewer.fs (runPersistentWindow warm-up FIFO); readiness/key-warmup-delivery.txt | keystroke-injection smoke on a host with native input injection | n/a (not [SEH]) | n/a | n/a | n/a | n/a |
| T036 | Warm-up FIFO implementation complete and compiles, but the `[US6]` vertical-slice (end-to-end keystroke delivery at focus) is only reachable through the live window, which is deferred. | src/SkiaViewer/SkiaViewer.fs; .agents/skills/fs-skia-viewer-host/SKILL.md (T037 [X]) | keystroke-delivery smoke on GPU/display host | n/a (not [SEH]) | n/a | n/a | n/a | n/a |
| T038 | Compiled-host keystroke-delivery evidence depends on the deferred T035 live smoke. | readiness/key-warmup-delivery.txt | keystroke-delivery smoke on GPU/display host | n/a (not [SEH]) | n/a | n/a | n/a | n/a |
