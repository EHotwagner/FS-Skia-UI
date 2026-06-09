# Feature Specification: ControlsShowcase Consumer Feedback Follow-ups — Live Tree Rendering, Pointer Routing, Key Normalization & Authoring Guidance

**Feature Branch**: `085-showcase-feedback-followups`
**Created**: 2026-06-09
**Status**: Draft
**Input**: User description: "create specs from the feedback from the sibling repo controlsshowcase"

## Context & Triage *(informative)*

A consumer built a 52-control typed Elmish **Controls Gallery** in a generated
`FS.Skia.UI` project (`ControlsShowcase`) and captured per-phase Spec Kit feedback
under `specs/001-controls-gallery/feedback/{specify,clarify,plan,implement}-2026-06-09.md`.
The project was generated from the current package/template line (it pins
`FS.Skia.UI.* 0.1.89-preview.1` and reflects the shipped catalog), so every finding
below is triaged against the **current framework source**, not a stale build.

Per the house pattern (one consolidated "consumer friction follow-ups" feature per
cohort — e.g. 060–063, 084) and the single-feature rule, this is **one** feature
consolidating the ControlsShowcase feedback, **not** one spec per item. The scope was
confirmed with the requester as the **full bundle, delivering both heavy framework
capabilities** (a real nested-tree renderer and a pointer-routing host), **both small
code fixes** (key normalization and surface-size into the host view), **and** the
documentation/skill corrections (including a **new `fs-skia-viewer-host` skill**).

Each finding was triaged against current source (see the dedicated source-triage pass).
Two findings were **already resolved** by feature 084 / the current skill set and carry
no new work; the rest are open and in scope.

| # | Sev | Finding | Current-state evidence |
|---|-----|---------|------------------------|
| RENDER-1 | major (USER) | `Widget.render`/`Control.render` is the **Feature-080 single-control PREVIEW** (per-control schematic geometry; renders only the OUTER container of a nested tree), **not** a tree renderer. A consumer that builds a scene `view` as `(Widget.render theme shellWidget).Scene` gets the **same schematic for every page** — the costliest wrong assumption of the consumer's implement phase (all 10 pages looked identical live until screenshot inspection caught it). There is no public API to faithfully rasterize a full nested `Control` tree to a `Scene`. | `src/Controls/Control.fsi:26` (`Control.render : Theme -> Control<'msg> -> ControlRenderResult<'msg>`); `src/Controls/Widget.fsi`; `specs/080-control-render-fidelity/plan.md:9-13` (preview-only). No `renderTree`. |
| PTR-1 | major | The durable scene host exposes a **keyboard seam only** — `GeneratedAppHost.MapKey : ViewerKey -> bool -> 'msg option` — and **no `MapPointer`**. The windowing layer *does* raise `ViewerEvent.Pointer{Pressed,Moved,Released,Scrolled,Exited}`, but those events have nowhere to go and never reach the model. The interactive pointer pipeline already ships (`ControlRenderResult.EventBindings : ControlEventBinding list`; `ControlsElmish.interpretPointerOutcome`/`interpretPointerEffect`, incl. the 4px click/drag fold), but the package ships **no `Viewer.runApp`-equivalent that takes an `AdapterProgram`** to wire it. A wiring/host-contract gap, not a missing capability. | `src/SkiaViewer/SkiaViewer.fsi` `GeneratedAppHost` record = `Init|Update|View|MapKey|Tick|Diagnostics` (no `MapPointer`); `src/Controls.Elmish/ControlsElmish.fs:92-98` (`interpretPointerOutcome`, internal/unwired). |
| KEY-1 | major | `ViewerKeyboard.normalize` classifies only **bare** single-char keys (`"5"` → `Digit 5`, `"l"` → `Letter 'L'`) plus named/arrow/function keys; the windowing toolkit delivers `"Number5"`/`"Key0"`/`"KeyL"`, all of which fall through to `Unknown raw`. So digit-page shortcuts **and** `L` silently no-op'd in the live window even though the model + host `MapKey` contract were correct. | `src/KeyboardInput/KeyboardInput.fs:192-233` (final arm `| _ -> Unknown raw`; no `Number*`/`Digit*`/`Keypad*`/`Key*` spellings); `src/KeyboardInput/KeyboardInput.fsi:9-21` (`ViewerKey`). |
| SIZE-1 | major | `GeneratedAppHost.View : 'model -> SceneNode` is handed **no output/window size**, making resolution-independent rendering impossible — a consumer must pin a fixed coordinate space (e.g. 640×480 = `InitialSize`). Under the **windowed-fullscreen default** (shipped by 084) that fixed scene is laid out small then upscaled to the work area → crisp vector content looks **blurry**. The host renders correctly (Vulkan-backed Skia straight onto the canvas, swapchain sized from the real window) — only `view` can't know the size. | `src/SkiaViewer/SkiaViewer.fsi` `GeneratedAppHost.View : 'model -> SceneNode` (no size param). Default = windowed-fullscreen (feature 084). |
| DOC-SKILL | minor | The host-contract, preview-vs-tree, and windowed-fullscreen-blur facts have **no governance skill home** (the existing `fs-skia-skiaviewer` is the *package-owned* viewer skill at `src/SkiaViewer/skill/SKILL.md`, so a new `.agents/` skill MUST take a distinct name — **`fs-skia-viewer-host`** — to avoid a `SkillSyncCheck` name collision). `fs-skia-typed-controls` lacks a **consumer-side** note (catalog controls all ship typed modules; author via `FS.Skia.UI.Controls.Typed.*`; verify presence via package/`catalog.yml`, **not** `docs/api-surface/`) and a typed-surface probe recipe. `docs/scaffold-map.md` lacks the caveat that the typed front door is **absent** from `docs/api-surface/`. | No `.agents/skills/fs-skia-viewer-host/`; `.agents/skills/fs-skia-typed-controls/SKILL.md` (no consumer note/probe); `template/base/docs/scaffold-map.md` (no Typed/api-surface caveat). |
| GOV-DOC | minor | The `spec-template.md` **Framework Governance Prompts** section requires naming package paths / `.fsi` impact / build targets, yet the spec-quality checklist asserts "No implementation details" — every feature re-derives that the governance section is **exempt**. Separately, evidence token parsing is **`key=value` lines**, not markdown tables (a table with the same tokens does **not** satisfy the validators) — undocumented. The `speckit-specify` skill snapshots a **single-file** external URL well but has no recipe for a **multi-file** source (enumerate a GitHub tree, fetch per file, assemble `source-spec.md` with per-file headers). | `.specify/templates/spec-template.md:26-44` (no exemption line); no `key=value` contract note in `template/base/docs/evidence-formats.md`/skills; `.agents/skills/speckit-specify/SKILL.md:103-112` (single-file only). |
| DONE-1 | resolved | Skill-id vs directory-name mismatch (the consumer's `fs-skia-ui-widgets` directory vs `name: controlsshowcase-widgets`) hard-blocked the DAG gate. **Already addressed**: the `speckit-tasks` skill now mandates declaring each file's `name:` value. No new work; a *compiled* enforcing gate remains a deferred candidate (see Out of Scope). | `.agents/skills/speckit-tasks/SKILL.md:154-158` ("Declared skill ids resolve from skill names"). |
| DONE-2 | resolved | The window-visibility evidence contract expanding to seven files (incl. `generated-validation.md`, **not** `generated-guidance-validation.md`) and the audit surfacing only `total-blockers=N` were major consumer friction. **Already fixed by feature 084** (evidence-formats regenerated to the full 7-file list; per-blocker stdout legibility, FR-008/FR-009). No new work. | `template/base/docs/evidence-formats.md:40-76`; `specs/084-window-options-consumer-followups/` (T019–T023). |

**Change classification.** **Escalated / `maintainer-verify` (Tier 1).** This change
adds public surface to `src/Controls/**/*.fsi` (`renderTree`), `src/SkiaViewer/SkiaViewer.fsi`
(pointer seam + size-aware `View`), and `src/KeyboardInput/KeyboardInput.fs` (normalize
mapping; the `.fsi` union is unchanged), edits `template/**` docs (`scaffold-map.md` and the
windowed-fullscreen-blur note), adds a **new skill** under `.agents/skills/fs-skia-viewer-host/`
(with generated `.claude` mirror), and edits governance paths (`.specify/templates/spec-template.md`,
the `speckit-specify`/`fs-skia-typed-controls`/`fs-skia-evidence-mode` skills). `Route` is
expected to escalate it; run the serialized six-target order, regenerate the skill tree with
`RefreshSurfaceBaselines`, and recapture surface baselines.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Faithful live rendering of a nested control tree (Priority: P1)

A consumer building a multi-page app composes a nested `Control` tree per page and
shows it in the durable scene window. Today every page renders the same per-control
preview schematic; the consumer wants each distinct tree to render as a distinct,
faithful scene.

**Why this priority**: This was the single costliest wrong assumption in the consumer's
implement phase — a build shipped where all ten pages looked identical live. It blocks
the core promise of "show real controls live."

**Independent test**: Compose two structurally different nested trees, rasterize each
through the new tree-render API to a `Scene`, and confirm the two scenes differ
(per-page screenshot diff is non-empty) and that nested children — not just the outer
container — are laid out and painted.

### User Story 2 - Pointer-driven interaction in the durable host (Priority: P1)

A consumer wants a mouse click on a live control to trigger that control's action
(e.g. a button press dispatches its message and the model updates), using the host
window rather than only headless behavior tests.

**Why this priority**: The pointer pipeline already ships but is unwired in the host;
without it, "demonstrate controls live and interactive" is keyboard + render-only, and
the honest `[X]` bar for live-verify tasks cannot be met for pointer behaviors.

**Independent test**: Launch the host with an adapter program, deliver a synthetic
`PointerPressed`/`PointerReleased` at a control's bounds, and observe the bound message
dispatched and the model state changed (including the 4px click-vs-drag fold).

### User Story 3 - Keyboard shortcuts recognized from real toolkit key names (Priority: P2)

A consumer wires digit and letter shortcuts (e.g. number keys to switch pages, `L` to
toggle a layer) and expects them to work in the live window, where the toolkit delivers
names like `"Number5"`/`"KeyL"`.

**Why this priority**: Real bug with a silent failure mode (no-op, no error), but
narrower than the rendering/pointer gaps and locally worked around by the consumer.

**Independent test**: Feed `"Number5"`, `"Digit5"`, `"Keypad5"`, `"Key5"`, and `"KeyL"`
through `ViewerKeyboard.normalize` and confirm `Digit 5` / `Letter 'L'`; feed an
unrecognized name and confirm it still yields `Unknown raw` (no regression).

### User Story 4 - Resolution-independent rendering without upscaling blur (Priority: P2)

A consumer wants live content to render sharply under the windowed-fullscreen default,
without pinning a fixed coordinate space that gets upscaled and blurred.

**Why this priority**: A direct consequence of the 084 windowed-fullscreen default;
high visible-quality impact but with a documented flag workaround in the interim.

**Independent test**: With the size-aware host `View`, render at two different surface
extents and confirm content is laid out to the actual extent (no fixed-size upscaling);
confirm the blur workaround is documented (one flag/setting).

### User Story 5 - Accurate authoring guidance for catalog consumers (Priority: P3)

An agent authoring a whole-catalog consumer needs guidance that matches reality: where
the typed front door lives (not `docs/api-surface/`), that `Control.render` is a preview
not a tree renderer, the host input surface, the windowed-fullscreen blur caveat, the
`key=value` evidence contract, the governance-section exemption, and a multi-file source
snapshot recipe.

**Why this priority**: Low cost, high leverage — each note prevents a repeat of a
multi-hour consumer investigation — but it follows the capabilities it documents.

**Independent test**: A new `fs-skia-viewer-host` skill and the updated
`fs-skia-typed-controls`/`scaffold-map`/`spec-template`/`speckit-specify` artifacts exist,
pass `SkillSyncCheck`/quality checks, and each states the corresponding fact verbatim;
the `.claude` mirror is generated from `.agents`.

## Requirements *(mandatory)*

### Functional Requirements

**Live tree rendering (RENDER-1)**

- **FR-001**: The framework MUST provide a public API that rasterizes a nested
  `Control<'msg>` tree to a `Scene`/`SceneNode` using real layout (the Yoga layout
  engine) and paint — distinct from the Feature-080 single-control preview. It MUST
  accept at least the active `Theme` and an output size.
- **FR-002**: The tree renderer MUST lay out and paint **nested** containers and their
  children (e.g. stacks/grids within a shell), such that two structurally different
  trees produce **visibly different** scenes — not the same outer-container schematic.
- **FR-003**: The existing single-control preview (`Control.render`/`Widget.render`) MUST
  remain behavior-unchanged (the Feature-080 contract and its goldens are preserved); the
  tree renderer is **additive**.

**Pointer routing in the durable host (PTR-1)**

- **FR-004**: The framework MUST provide a durable host launch path that accepts an
  adapter/Elmish program and routes the raised `ViewerEvent.Pointer*` events
  (`Pressed`/`Moved`/`Released`/`Scrolled`/`Exited`) by hit-testing the rendered layout
  and dispatching through `ControlRenderResult.EventBindings` /
  `ControlsElmish.interpretPointerOutcome`, including the 4px click-vs-drag fold.
- **FR-005**: A pointer press/release on a live control MUST reach the model — the bound
  message is dispatched and the model updates — observable from the host, not only from
  headless behavior tests.
- **FR-006**: The new pointer capability MUST be **additive** and MUST keep the existing
  `Viewer.runApp viewerOptions generatedHost` call literal reachable so the durable
  `GovernanceTests` assertion is not broken (add a `MapPointer` seam and/or a new
  `runApp`-equivalent rather than replacing the existing entry point).

**Keyboard normalization (KEY-1)**

- **FR-007**: `ViewerKeyboard.normalize` MUST map the common windowing-toolkit key-name
  spelling families to the correct `ViewerKey`: digit spellings `Number{n}`/`Digit{n}`/
  `Keypad{n}`/`Key{n}` → `Digit n`, and single-letter spellings `Key{X}` → `Letter X`
  (case-insensitive), in addition to the existing bare-character handling.
- **FR-008**: `normalize` MUST remain total — any name it does not recognize MUST still
  yield `Unknown raw` (no exceptions, no regression to existing recognized names).

**Resolution-independent rendering (SIZE-1)**

- **FR-009**: The durable host MUST make the current output/surface size available to the
  view function (a size-aware `View`, e.g. `Size -> 'model -> SceneNode`, and/or a
  `runApp`/`ViewerEffect` callback exposing the current surface extent) so a consumer can
  render resolution-independently instead of pinning a fixed coordinate space.
- **FR-010**: Documentation MUST explain that the windowed-fullscreen default scales a
  fixed-size scene up (causing blur) and MUST give the interim workaround (e.g.
  `--window-startup normal` for 1:1, or raising the design resolution / `InitialSize`).

> Interacting requirements: FR-009 changes the host `View` signature (a public-surface
> change) and FR-006 requires the existing `runApp`/`GovernanceTests` literal to stay
> reachable — resolve by making the size-aware view an **additive** seam (new
> member/overload or a size-carrying host variant), not an in-place signature break.

**Authoring guidance — new + updated skills and docs (DOC-SKILL, GOV-DOC)**

- **FR-011**: A new skill `fs-skia-viewer-host` MUST be authored under `.agents/skills/`
  (distinct-named from the existing package-owned `fs-skia-skiaviewer` to avoid a
  `SkillSyncCheck` name collision)
  (with its generated `.claude/skills/` mirror) documenting: the host **input surface**
  (keyboard via `MapKey`; pointer via the FR-004 seam), the **preview-vs-tree** distinction
  (`Control.render` is a per-control preview; the FR-001 API renders a tree), and the
  **windowed-fullscreen blur** caveat + workaround (FR-010).
- **FR-012**: The `fs-skia-typed-controls` skill MUST gain a **consumer-side** note: every
  catalog control ships a typed module; author whole-catalog consumers via
  `FS.Skia.UI.Controls.Typed.*`; verify a control's availability from the **package /
  `catalog.yml` `module:` fields**, **not** `docs/api-surface/` (which omits the typed
  front door). It MUST include a typed-surface probe recipe (enumerate the typed modules
  deterministically).
- **FR-013**: `template/base/docs/scaffold-map.md` MUST state that the typed front door is
  **not** present in `docs/api-surface/` (which exposes only the legacy `X.create` builder
  surface) and how to enumerate the typed surface instead.
- **FR-014**: `.specify/templates/spec-template.md` MUST state that the **Framework
  Governance Prompts** section is **exempt** from the "no implementation details" rule,
  so the resolution is not re-derived per feature.
- **FR-015**: The evidence-format guidance (`template/base/docs/evidence-formats.md` and/or
  the `fs-skia-evidence-mode` skill) MUST document that evidence token parsing reads
  **`key=value` lines** and that a markdown table containing the same tokens does **not**
  satisfy the validators.
- **FR-016**: The `speckit-specify` skill MUST include a recipe for snapshotting a
  **multi-file** external URL source: enumerate a directory/tree (e.g. a GitHub tree via
  the contents API), fetch each file, and assemble a single provenance `source-spec.md`
  with per-file headers.

**Governance currency**

- **FR-017**: All new public `.fsi` surface (FR-001, FR-004/FR-006, FR-009) MUST be
  reflected in the surface baselines; the new `fs-skia-viewer-host` skill MUST be
  registered and the `.claude` tree regenerated via `RefreshSurfaceBaselines`, passing
  `SkillSyncCheck` / `SkillQualityCheck` / `TargetMetadataDrift`.
- **FR-018**: `./fake.sh build -t Route` MUST escalate this change; the escalated
  serialized six-target order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`) MUST pass, with the
  required evidence artifacts present.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: `FS.Skia.UI.Controls` gains the public tree-render API (FR-001);
  `FS.Skia.UI.SkiaViewer` gains the pointer seam and size-aware view (FR-004/FR-006/FR-009);
  `FS.Skia.UI.KeyboardInput` changes `normalize` behavior (FR-007). `FS.Skia.UI.Build`
  changes only if a new governance gate is added (none required; the skillist gate is a
  deferred candidate). All packable projects (including `FS.Skia.UI.Build` under
  `build/Governance`) MUST be version-bumped and packed at merge. No legacy Charts package
  migration is involved.
- **Public contract impact**: New `.fsi` signatures in `src/Controls/**` (tree renderer)
  and `src/SkiaViewer/SkiaViewer.fsi` (pointer seam + size-aware `View`/callback). The
  `KeyboardInput` `ViewerKey` union is unchanged; only `normalize` behavior changes. The
  existing `Viewer.runApp` literal and the Feature-080 preview contract are preserved.
- **State workflow impact**: New input/effect routing — pointer events flow through
  `interpretPointerOutcome` to `Dispatch`/`msg`; the adapter/host program seam is extended.
  No change to existing command/subscription interpreters beyond additive pointer wiring.
- **Layout/rendering impact**: Yes — a new Yoga-backed nested-tree rasterizer to `Scene`
  (FR-001/FR-002), size-aware rendering (FR-009), and the windowed-fullscreen blur
  documentation (FR-010). The Skia/Vulkan paint path is reused, not changed.
- **Evidence obligations**: Per-page render-distinctness screenshots (US1), live
  pointer-dispatch evidence (US2), `normalize` mapping test evidence (US3), size-aware /
  no-blur render evidence (US4), and the window-visibility evidence class
  (`interactive-visible-window.md`, `close-reason-separation.md`,
  `window-state-diagnostics.md`, `window-options.md`, `generated-validation.md`,
  `real-image-evidence.md`, feature-local `evidence-audit.md`), authored as `key=value`
  blocks (FR-015). `EvidenceGraph` + `EvidenceAudit` must be green.
- **Unsupported scope**: A full live pointer confirmation may require a key/pointer
  injection tool absent from the headless environment — synthetic-event + headless-adapter
  evidence is the honest bar where live injection is unavailable. No new windowing toolkit,
  no new platform/distribution targets, no redesign of the Feature-080 preview.
- **Build-target impact**: No new FAKE target is required. `RefreshSurfaceBaselines` must
  run (new skill + new public surface); `TemplateCheck`/`GeneratedGuidanceCheck`/
  `GeneratedProductCheck` are exercised by the template-doc edits; `EvidenceGraph`/
  `EvidenceAudit` gate the feature.

## Success Criteria *(mandatory)*

- **SC-001**: A consumer app with N structurally distinct nested pages renders N visibly
  distinct live scenes — a per-page screenshot diff between any two different pages is
  non-empty (today it is empty: identical schematics).
- **SC-002**: A pointer press on a live control in the durable host dispatches that
  control's bound action and changes model state in ≥1 demonstrated control, observable
  from the host (not only from headless tests).
- **SC-003**: All five toolkit key spellings `Number5`, `Digit5`, `Keypad5`, `Key5`, and
  `KeyL` normalize to the correct `Digit`/`Letter` (0 silent `Unknown` no-ops for the
  covered families), while unrecognized names still normalize to `Unknown raw`.
- **SC-004**: Live content renders sharply under the windowed-fullscreen default with the
  size-aware view, OR sharp output is reachable via exactly one documented flag/setting.
- **SC-005**: An agent can confirm any catalog control's availability from the package /
  `catalog.yml` without DLL reflection or grepping `docs/api-surface/`, following the
  updated `fs-skia-typed-controls` guidance.
- **SC-006**: `Route` escalates the change and the escalated six-target order passes; the
  new `fs-skia-viewer-host` skill and all new public surface are baseline-current
  (`SkillSyncCheck`/`TargetMetadataDrift`/surface checks green), with zero regression to
  the Feature-080 preview goldens and the durable `Viewer.runApp` `GovernanceTests` literal.

## Out of Scope / Deferred *(informative)*

- **Already resolved (no work)**: DONE-1 (skill-id-resolves-to-`name:` is documented in the
  `speckit-tasks` skill) and DONE-2 (evidence-formats 7-file drift + audit stdout
  legibility, shipped by feature 084).
- **Deferred candidates** (recorded, not delivered here): a *compiled* `SkillistIdResolution`
  governance gate that enforces DONE-1 at `Route`/audit time rather than by documentation;
  and promoting the consumer's deterministic "gallery evidence" page-tour helper
  (page tour → `EvidenceOutcome` record; per-page + dark screenshot set) into a documented
  `fs-skia-evidence-mode` snippet. Both are below the bar for this feature's committed scope.

## Assumptions

- "ControlsShowcase" refers to the sibling repo at `../ControlsShowcase`, whose feedback
  records under `specs/001-controls-gallery/feedback/` are the authoritative source.
- The pointer pipeline (`ControlRenderResult.EventBindings`, `interpretPointerOutcome`,
  the 4px fold) is correct and reusable; this feature **wires** it into a host rather than
  reimplementing hit-testing or the click/drag fold.
- A new public tree-render API and a new pointer-routing host entry point are acceptable
  additive public surface (confirmed by the requester's "deliver both" scope choice);
  exact names are a planning decision, with `Control.renderTree` and a `runApp`-with-
  adapter variant as the working candidates.
- Where the headless environment lacks a live key/pointer injection tool, synthetic-event
  evidence through the host/adapter path is the honest verification bar.
- Single-feature rule: this consolidated follow-ups feature is the correct unit; the heavy
  capabilities are delivered here rather than split into separate features, per the
  requester's scope decision.
