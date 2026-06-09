# Feature Specification: Interactive Non-Game Consumer Fitness — Neutral Controls-First Scaffold, Pointer-Aware Governed Host, Multi-Axis Layout, Scene-Host Hit-Testing & Composition Primitives

**Feature Branch**: `086-interactive-consumer-fitness`
**Created**: 2026-06-09
**Status**: Draft
**Input**: User description: "create specs from the feedback from the sibling repo controlsshowcase1"

## Context & Triage *(informative)*

A second consumer built a 52-control typed Elmish **Controls Gallery** in a generated
`FS.Skia.UI` project (`ControlsShowcase1`) and captured per-phase Spec Kit feedback under
`specs/001-controls-gallery/feedback/{specify,clarify,plan,tasks,analyze,implement}-2026-06-09.md`.
This consumer was generated from the **post-085** package/template line — it pins
`FS.Skia.UI.* 0.1.91-preview.1`, the version that already shipped feature 085's
`Control.renderTree`, the pointer-aware `InteractiveAppHost` / `ControlsElmish.runInteractiveApp`,
the `"Number5"`/`"KeyL"` key normalization, the size-aware host `View`, and the
`fs-skia-viewer-host` skill. Every finding below is therefore triaged against **current
framework source** (those 085 deliverables are confirmed present and are *not* re-specified
here) and reflects what is **still open** after 085.

The `implement` feedback is **severity: major**; the `specify`/`clarify`/`plan`/`tasks`/`analyze`
feedback is **minor** and clusters into separate subsystems (external-tree source-spec
snapshotting, a skillist-name registry validator, typed-front-door discoverability, and a
verify-during-implement skill discipline). Per the single-feature rule, the requester scoped
**this** feature to the **interactive non-game consumer fitness** cluster only — the runtime +
scaffold work that makes the framework a first-class host for an interactive, non-game controls
application. The other clusters are explicitly out of scope here (see Out of Scope) and remain
candidates for follow-on `/speckit-specify` runs.

Each finding was triaged against current source. Where the consumer's observed symptom and the
current source disagree, the table records the **precise mechanism** rather than the symptom.

| # | Sev | Finding | Current-state evidence |
|---|-----|---------|------------------------|
| SCAFFOLD-1 | major | The **durable scaffold ships a Tetris game baked into its naming and structure.** A non-game consumer (a Controls Gallery, an invoice app, anything) has no playfield/tally/gameplay region, so adopting the scaffold forces carrying meaningless game vocabulary forward or re-pointing game-named durable code onto unrelated concepts (HUD→app bar, gameplay→content region). The governance spine only needs *stable* tokens, not *game* tokens, so neutral naming costs nothing. | `template/base/**` scaffold: `Model.fs:34-54` (`type Screen = Initial\|Options\|Main\|Paused\|Ended`; `ActiveColumn`/`ActiveRow`/`Tally`/`Stage`/`NextToken`); `View.fs:58-125` (`playfieldLayout`, "tally"/"stage"/"upcoming" labels, circular entities); `GovernanceTests.fs:209-211` and `BehaviorTests.fs:209-226` (assert "grid-style playfield", "tally/stage/upcoming", "circular or elliptical entities"). Durable governance tokens in `EvidenceCommands.fs`/`WindowOptions.fs`/`Program.fs` are framed around the game, not "the app". |
| VIEW-1 | major | The scaffold's primary `view : Model -> SceneNode` **hand-draws a bespoke Scene and never calls `Control.renderTree`** on a real control tree. The path of least resistance for a consumer is therefore to keep drawing a parallel placeholder scene — exactly the mistake that shipped a "complete"-looking but **non-interactive mockup** (all pages identical; control *id strings* drawn instead of real controls). `renderTree` exists (085) but the scaffold doesn't use it. | `template/base/**` `View.fs:67-125`: `view` returns a hand-built `Group([...])` of `Rectangle`/grid/`Text` nodes; the real `controlsExampleView` (`View.fs:27-53`) is never rasterized via `Control.renderTree`. |
| HOST-GOV-1 | major | **Governance hard-locks the default launch to the keyboard-only host.** `GovernanceTests`/`BehaviorTests` assert the default path is `Viewer.runApp viewerOptions generatedHost` — `GeneratedAppHost` has **no `MapPointer`** by design, so it cannot hit-test controls. The pointer-aware `InteractiveAppHost` / `runInteractiveApp` ships (085) but using it as the default **fails governance**, so a controls product's "persistent launch" task looks satisfiable while being keyboard-only — mouse clicks do nothing. A governed-default *posture* gap, not a missing capability. | `template/base/**` `GovernanceTests.fs:105` and `BehaviorTests.fs:289` (`Expect.stringContains defaultBranch "Viewer.runApp viewerOptions generatedHost"`); `src/SkiaViewer/SkiaViewer.fsi:514-520` (`GeneratedAppHost` = `Init\|Update\|View\|MapKey\|Tick\|Diagnostics`, no `MapPointer`); `src/Controls.Elmish/ControlsElmish.fsi:45-53` (`InteractiveAppHost`, `MapPointer: PointerInteraction -> 'msg option` at :51), `:140-141` (`runInteractiveApp`). |
| LAYOUT-1 | major | **`renderTree` cannot lay out a real side-by-side composition.** It maps only fixed kinds (`toolbar`/`split-view`/`wrap`/`grid`/`dock`) to a horizontal row; a **horizontal-orientation `Stack`** falls through to `Column`. Worse, child bounds are keyed by `Key ?? Kind` in a `Map`, so **unkeyed same-kind siblings collide** and paint at the same box → visible overlap. This forced the consumer to abandon a controls-built rail+content shell for a fragile absolute-coordinate Scene workaround. (The Feature-080 single-control *preview* is a separate path and is unaffected.) | `src/Controls/Control.fs:1011-1018` (`directionOf`: only listed kinds → `Row`); `:1069-1072` (`boundsById` is `Map` keyed by `b.NodeId` = `Key ?? Kind`); `:1101-1117` (`paint` looks up by the same id, so duplicate ids resolve to one box). `Attr.width`/`height` ARE read for containers (`:1038-1047`) but the collision masks it. |
| BOUNDS-1 | major | **A Scene-based host cannot map a pointer coordinate to a control.** `ControlRenderResult` returns `Layout = root` — the **un-evaluated input tree** — while the computed `result.Bounds` is discarded; and it exposes `EventBindings` (by `ControlId`) but **no per-`ControlId` computed bounds**. So a consumer who needs absolute layout (the Scene host) gets raw pointer events but no way to know which control was hit; the control-tree host hit-tests automatically but only flows a single vertical column. The consumer can have correct layout OR automatic control interaction, not both. | `src/Controls/Control.fs:1067` (`result = Layout.evaluate ...`), `:1069-1072` (`boundsById` computed then dropped), `:1119-1123` (`Layout = root`, not `result`); `src/Controls/Types.fsi:285-290` (`ControlRenderResult` = `Scene\|Layout\|Diagnostics\|EventBindings\|NodeCount`), `:279-282` (`ControlEventBinding` carries `ControlId` but no bounds). |
| SCENE-XLATE-1 | minor | **No translate/offset Scene primitive.** Placing a rendered control sub-scene into a region requires a hand-written `shiftNode dx dy` that walks every `SceneNode` case and offsets coordinates — fragile for `Path`/`Points`/`Vertices`/`Chart`, which carry no simple origin. | `src/Scene/Scene.fsi:319-341` (`SceneNode`): `Group of Scene list` carries no offset; no `Translate`/`Group`-with-origin case. |
| SCENE-TEXT-1 | minor | **`Scene.Text` carries no font size.** Chrome like a nav rail can't size text to its column, so long titles overflow at the default (large) font. (`TextRun` does carry size via `FontSpec`, but the simple `Text` node does not.) | `src/Scene/Scene.fsi:332` (`Text of (float * float) * string * Color`); `:168-178` (`TextRun`/`FontSpec` with `Size: float`). |
| KEY-WARMUP-1 | minor | **Keyboard input is dead for ~a few seconds after the window gains focus.** The viewer host wires key handlers synchronously with no buffering of events raised before the input pipeline is ready and no "input-ready" signal, so the first keystrokes after focus are silently dropped, then input begins working. Framework host concern; the consumer only supplies `MapKey`. | `src/SkiaViewer/SkiaViewer.fs:~1480-1520` (key handlers added synchronously on `keyboard.add_KeyDown`; no pre-ready queue, no readiness signal). |

**Change classification.** **Escalated / `maintainer-verify` (Tier 1).** This change edits the
generated-project scaffold under `template/**` (neutral naming + a controls-first default `view`
+ the host-governance assertions), adds public surface to `src/Scene/Scene.fsi` (a translate
primitive and a sized-text capability), `src/Controls/Types.fsi` (per-`ControlId` bounds on
`ControlRenderResult`), and changes `src/Controls/Control.fs` layout behavior, with possible
public additions in `src/SkiaViewer/**` (key warm-up) and `src/Controls.Elmish/**` (governed
pointer host wiring). It also touches a governance skill note (`fs-skia-viewer-host`, key warm-up
caveat). `Route` is expected to escalate it; run the serialized six-target order
(`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph`
→ `EvidenceAudit`), regenerate the skill tree with `RefreshSurfaceBaselines`, and recapture the
affected surface baselines.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A neutral, controls-first scaffold a non-game consumer can extend (Priority: P1)

A consumer generates a new project to build a non-game app (a controls gallery, a form, a
dashboard). They expect the scaffold to read in neutral application terms and to render real
controls out of the box, so they extend the production render path instead of re-pointing game
vocabulary or hand-drawing a parallel scene.

**Why this priority**: This is the root cause of the consumer's costliest implement-phase loss —
the Tetris framing plus a bespoke-Scene `view` produced a non-interactive mockup that passed all
tests and gates. Fixing the starting point prevents the whole failure class for every future
non-game consumer.

**Independent test**: Generate a fresh project; grep its product source (`Model`/`View`/tests)
for game vocabulary (`playfield`, `tally`, `stage`, `gameplayRegion`, `Initial|Options|Main|Paused|Ended`,
"circular entities") and confirm **none** appears outside the durable governance tokens; launch
the default app unmodified and confirm the window shows **real styled controls** (matching the
scaffold's control tree), not placeholder rectangles or control-id text.

### User Story 2 - Pointer interaction works in the governed default app (Priority: P1)

A consumer clicks a control in the default generated app and the control's action fires (a button
press dispatches its message; the model updates) — without adding an `--interactive` flag, a
second host, or fighting governance.

**Why this priority**: "Demonstrate controls live and interactive" is the core promise of a
controls product; today the governed default is keyboard-only, so the wired-up persistent-launch
task is satisfiable while mouse input does nothing.

**Independent test**: Generate a project, launch its default app, deliver a synthetic pointer
press/release at a control's bounds, and observe the bound message dispatched and model state
changed — and confirm the governance suite passes with the pointer-aware host as the default
(the host-lock assertion accepts a persistent interactive host, not only the keyboard-only call).

### User Story 3 - Real side-by-side layout from a control tree (Priority: P1)

A consumer composes a shell with a fixed-left navigation rail beside a content region using
controls (a horizontal stack / dock / grid) and expects the children to be laid out side-by-side
without overlapping — no absolute-coordinate Scene workaround.

**Why this priority**: The layout gap is what forced the consumer off the control-tree host
entirely; without multi-axis layout, "build the UI from controls" collapses to a single vertical
column.

**Independent test**: Build a horizontal stack (and a dock) containing two structurally similar,
**unkeyed** sibling children; rasterize via `renderTree`; confirm the two children receive
**non-overlapping** bounds at distinct x-coordinates (not the same box), and that an explicit
width/height on a container is reflected in its computed bounds.

### User Story 4 - Map a pointer to a control from a Scene-based host (Priority: P2)

A consumer who needs absolute layout (the Scene host) wants to take a pointer coordinate and
learn which rendered control it landed on, so they can route the click — getting both correct
layout and control interaction at once.

**Why this priority**: This is the other half of the "two hosts, neither sufficient alone" trap;
it closes the gap so a consumer doesn't have to choose between layout and interaction.

**Independent test**: Render a control tree via `renderTree`; from the public render result,
resolve the absolute bounds of every control by `ControlId`; confirm a point inside a given
control's bounds maps back to that control's id (and a point in the gap maps to none).

### User Story 5 - Compose and size sub-scenes without fragile hand-rolled code (Priority: P2)

A consumer places a rendered control sub-scene into a region of a larger scene and sizes chrome
text (e.g. a nav-rail label) to fit its column, using framework primitives rather than a custom
coordinate-walking shift function.

**Why this priority**: Lower blast radius than layout/hit-testing, but each missing primitive
cost the consumer fragile bespoke code (a `shiftNode` that mishandles `Path`/`Points`/`Vertices`/`Chart`,
and clipping rail labels at the default font).

**Independent test**: Offset a sub-scene (including one containing `Path`/`Points`/`Chart` nodes)
by a known delta using the translate primitive and confirm every node's effective coordinates
shift uniformly; render a `Text` node at a small explicit size and confirm it fits a narrow
column without clipping.

### User Story 6 - No dropped keystrokes when the window gains focus (Priority: P3)

A consumer presses keys immediately after the live window appears/gains focus and every keystroke
is acted on — none are silently swallowed during a startup window.

**Why this priority**: Real but narrow silent-failure mode that affects first impressions of every
interactive app; locally unavoidable by the consumer because it lives in the host input pipeline.

**Independent test**: In the live interactive window, issue a known sequence of keystrokes within
the first seconds after focus and confirm all are delivered to `MapKey` (none dropped); confirm
the warm-up behavior is documented in `fs-skia-viewer-host`.

### Edge Cases

- A control tree with **many unkeyed siblings of the same kind** (the collision case) must lay
  out distinctly — keying must be derived so identical-kind siblings don't share a bounds entry.
- Translating a sub-scene that contains nodes with **no simple origin** (`Path`, `Points`,
  `Vertices`, `Chart`) must still offset correctly or be explicitly defined.
- A `Text` node with **no explicit size** must keep its current default rendering (back-compat).
- A controls product's governed host change must not weaken the **game** product family's existing
  persistent-launch guarantee.

## Requirements *(mandatory)*

### Functional Requirements

**Neutral, controls-first scaffold (SCAFFOLD-1, VIEW-1)**

- **FR-001**: The generated-project scaffold MUST use **domain-neutral** names in its product
  source (model, view, and tests) — no game/Tetris vocabulary (`playfield`, `gameplayRegion`,
  `tally`, `stage`, `upcoming`, `ActiveColumn`/`ActiveRow`/`NextToken`, the `Initial|Options|Main|Paused|Ended`
  screen states, "circular entities"). Neutral equivalents (e.g. `Page`/`View`, `contentRegion`,
  `canvas`/`contentArea`, generic status fields) MUST be used instead.
- **FR-002**: The scaffold's durable, governance-scanned tokens (the evidence/window vocabulary
  in `LayoutEvidence.fs`/`EvidenceCommands.fs`/`WindowOptions.fs`/`Program.fs`) MUST be **preserved as stable tokens**
  but framed around "the app"/"the content region" rather than "the game"/"the playfield". The
  governance spine MUST continue to pass on the neutral scaffold.
- **FR-003**: The scaffold's default `view` MUST render a **real `Control` tree** via the
  production tree-render path (`Control.renderTree`), not hand-drawn placeholder geometry — so a
  consumer extends the production render path by default. The default generated app, unmodified,
  MUST display actual styled controls.

> Interacting / conflicting requirements: FR-001 (neutral product names) vs FR-002 (stable
> governance tokens) — resolve as: **product-domain names are free to change; the durable
> governance/evidence/window tokens stay stable** (only their surrounding framing becomes
> neutral). Renaming for neutrality MUST NOT drop or rename a governance-scanned token.

**Pointer-aware governed default host (HOST-GOV-1)**

- **FR-004**: A generated **non-game/controls** product MUST be able to declare a **pointer-aware
  persistent host** (the `InteractiveAppHost` / `runInteractiveApp` pipeline that routes
  `PointerInteraction`) as its **governed default launch**, such that a mouse click on a live
  control dispatches that control's bound message.
- **FR-005**: The host-governance assertion MUST be generalized to require **"a persistent
  interactive host appropriate to the product family"** rather than the specific keyboard-only
  `Viewer.runApp ... generatedHost` call, so the pointer-aware default passes governance.
- **FR-006**: The existing **game** product family's persistent-launch guarantee MUST remain
  intact (it MAY continue to use the keyboard-only `GeneratedAppHost`); the change MUST be a
  per-family choice, not a removal of the keyboard host.

> Interacting / conflicting requirements: FR-004/FR-005 (pointer host as default) vs the existing
> hard-locked `Viewer.runApp ... generatedHost` assertion (and FR-006) — resolve as: the
> governance check asserts the **presence of a governed persistent interactive host per family**,
> and the controls family selects the pointer-aware one while the game family keeps the
> keyboard-only one. Neither family loses its persistent-launch guarantee.

**Multi-axis layout in `renderTree` (LAYOUT-1)**

- **FR-007**: `Control.renderTree` MUST honor a **horizontal-orientation `Stack`** (and the
  documented horizontal container kinds) by laying its children out along the row axis — a
  side-by-side composition MUST NOT collapse to a vertical column.
- **FR-008**: `renderTree` MUST lay out **same-kind sibling containers without overlap** — child
  bounds resolution MUST NOT collapse unkeyed same-kind siblings to a single shared box. Two
  structurally similar unkeyed siblings MUST receive distinct, non-overlapping bounds.
- **FR-009**: An explicit width/height on a container MUST be reflected in that container's
  computed bounds in `renderTree` output.
- **FR-010**: The Feature-080 single-control **preview** path (`Control.render`/`Widget.render`)
  MUST remain behavior-unchanged; the multi-axis layout work is confined to the tree-render path.

**Per-control bounds for Scene-host hit-testing (BOUNDS-1)**

- **FR-011**: The public `renderTree` result MUST expose the **computed absolute bounds of every
  rendered control keyed by `ControlId`** (the evaluated layout result), not merely the
  un-evaluated input layout tree, so a Scene-based host can map a pointer coordinate to the
  control it hit.
- **FR-012**: A consumer MUST be able to resolve, from the public render result alone, which
  control (if any) contains a given point — closing the "correct layout OR automatic interaction,
  not both" gap for the Scene-host path.

**Scene composition primitives (SCENE-XLATE-1, SCENE-TEXT-1)**

- **FR-013**: `FS.Skia.UI.Scene` MUST provide a **translate/offset primitive** (e.g. a `Translate`
  node or a `Group` with an origin) that shifts an entire sub-scene by a delta, correctly
  offsetting **all** node kinds including `Path`/`Points`/`Vertices`/`Chart` — replacing the need
  for a hand-written coordinate-walking shift.
- **FR-014**: The simple `Scene.Text` node MUST support an **explicit font size** (a sized text
  primitive or a size field), so chrome text can be sized to its container; a `Text` with no
  explicit size MUST keep its current default rendering.

**Viewer host keyboard warm-up (KEY-WARMUP-1)**

- **FR-015**: The viewer host MUST NOT silently drop keystrokes issued after the window gains
  focus during input-pipeline warm-up — it MUST either **buffer/queue** key events raised before
  the pipeline is ready and deliver them once ready, **or** expose an **input-ready signal** a
  consumer can observe.
- **FR-016**: The keyboard warm-up behavior and the chosen mitigation MUST be documented in the
  `fs-skia-viewer-host` skill so consumers know what to expect.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This section is
> *expected* to name concrete packages, `.fsi` signatures, build targets, and evidence paths.

- **Package impact**: No package identities change. Package **contents/behavior** change in
  `FS.Skia.UI.Scene` (translate + sized-text primitives), `FS.Skia.UI.Controls` (multi-axis
  `renderTree` + per-`ControlId` bounds on `ControlRenderResult`), possibly `FS.Skia.UI.SkiaViewer`
  (key warm-up) and `FS.Skia.UI.Controls.Elmish` (governed pointer host wiring). All packable
  projects (incl. `FS.Skia.UI.Build` under `build/Governance`) MUST be version-bumped and packed
  on merge. The generated-project **template** (`FS.Skia.UI.Template`) changes (neutral scaffold +
  controls-first `view` + host-governance assertions) on its own version track.
- **Public contract impact**: `.fsi` signatures change/add — `src/Scene/Scene.fsi` (translate
  primitive, sized text), `src/Controls/Types.fsi` (per-`ControlId` bounds on
  `ControlRenderResult`), and potentially `src/SkiaViewer/SkiaViewer.fsi` /
  `src/Controls.Elmish/ControlsElmish.fsi`. Per-package and cross-package surface baselines MUST
  be recaptured.
- **State workflow impact**: The governed default launch for the controls product family changes
  to the pointer-routing `InteractiveAppHost`/`runInteractiveApp` pipeline (effects/commands
  unchanged in shape; the pointer seam is already shipped). The viewer host input-delivery path
  changes (warm-up buffering or readiness signal).
- **Layout/rendering impact**: `renderTree` layout changes (horizontal orientation, sibling
  bounds, width/height); new Scene translate + sized-text rendering; the default generated `view`
  rasterizes a real control tree. The Feature-080 single-control preview goldens MUST be
  preserved. Real render evidence MUST exercise the **production** render path
  (`controlsExampleView` → `Control.renderTree`), not a bespoke author-built scene.
- **Evidence obligations**: A neutral-scaffold grep proof (no game vocabulary in product source);
  a generated-product live-launch screenshot showing real controls via the production render path
  (not placeholder geometry); a pointer-dispatch behavior artifact; a `renderTree` side-by-side
  non-overlap layout artifact; a per-`ControlId` bounds hit-test artifact; window-visibility /
  persistent-launch evidence for the pointer-aware governed default.
- **Unsupported scope**: No new platforms/distribution. The deferred minor clusters (external-tree
  source-spec snapshotting, skillist-name registry validator, typed-front-door catalog/probe
  discoverability, `/speckit-implement` run/verify discipline) are **out of scope** for this
  feature.
- **Build-target impact**: Escalated `maintainer-verify`. Run the serialized six-target order
  (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`,
  `EvidenceAudit`); regenerate the skill tree via `RefreshSurfaceBaselines`; recapture per-package
  and cross-package surface baselines. `TemplateCheck`/`GeneratedProductCheck` exercise the neutral
  scaffold and controls-first default `view`.

## Success Criteria *(mandatory)*

- **SC-001**: A freshly generated project's product source (model, view, tests) contains **zero**
  game/Tetris identifiers outside the durable governance tokens (grep returns none), and the
  governance suite still passes.
- **SC-002**: The default generated app, launched **unmodified**, shows **real styled controls**
  in the live window — its screenshots match the scaffold's control tree, with no placeholder
  rectangles or control-id text.
- **SC-003**: A user can **click a control** in the default generated app and see the control's
  action take effect, **without** adding a flag or a second host, and the governance suite passes
  with the pointer-aware host as the controls-family default.
- **SC-004**: Two structurally similar, unkeyed sibling containers laid out horizontally render
  **side-by-side at distinct, non-overlapping positions** (not the same box), and a container's
  explicit size is reflected in its rendered bounds.
- **SC-005**: From the public render result alone, a consumer can resolve the absolute bounds of
  **every** rendered control and map any point to the control that contains it (or to none).
- **SC-006**: A sub-scene (including `Path`/`Chart` nodes) can be offset by a known delta with a
  single framework primitive and all nodes shift uniformly; a nav-rail label sized to its column
  renders without clipping at the default window size.
- **SC-007**: Every keystroke issued within the first seconds after the window gains focus is
  delivered to the consumer's key handler (none silently dropped), and the warm-up behavior is
  documented in `fs-skia-viewer-host`.
- **SC-008**: The game product family retains its existing persistent-launch guarantee (its
  governance assertion still passes with the keyboard-only host).

## Key Entities *(include if feature involves data)*

- **Scaffold product model/view**: the generated project's neutral application model (pages/views,
  content region) and its default `view` that rasterizes a real `Control` tree.
- **Governed host (per product family)**: the persistent launch host selected per family — the
  pointer-aware `InteractiveAppHost`/`runInteractiveApp` for controls products, the keyboard-only
  `GeneratedAppHost`/`Viewer.runApp` for the game family.
- **`ControlRenderResult` bounds map**: the computed absolute bounds of each rendered control,
  keyed by `ControlId`, exposed for Scene-host hit-testing.
- **Scene composition primitives**: a translate/offset node and a sized text capability in
  `FS.Skia.UI.Scene`.

## Assumptions

- ControlsShowcase1 is generated from the **post-085** line (`0.1.91-preview.1`); the 085
  deliverables (`renderTree`, pointer host, key normalization, size-aware `View`,
  `fs-skia-viewer-host`) exist and are **not** re-specified — this feature builds on them.
- "Product family" is an existing concept the template/governance can branch on (game vs.
  non-game/controls); if it is not yet first-class, introducing a minimal family marker is in
  scope for FR-004/FR-005/FR-006.
- The single-control preview path (Feature-080) and its goldens are the contract to preserve;
  layout changes are confined to the tree-render path.
- The translate primitive's exact shape (a `Translate` node vs. a `Group`-with-origin) and the
  sized-text shape (a new node vs. a size field on `Text`) are design choices for `/speckit-plan`;
  the requirement is the capability, correctly offsetting all node kinds and sizing chrome text.
- The deferred minor clusters are tracked separately and are not blockers for this feature.

## Out of Scope

- **External-tree source-spec snapshotting** — a turnkey "enumerate a GitHub tree → sorted fetch →
  assemble a provenance-headed `source-spec.md`" routine for `speckit-specify` (specify-phase
  feedback).
- **Skillist-name registry validator** — a check that cross-references every `[skillist: ...]`
  token in `tasks.md`/`tasks.deps.yml` against installed skills and fails on an unresolved token
  (the `controlsshowcase1-widgets` defect; analyze-phase feedback).
- **Clarify source-check helper** — a "consult the snapshotted `source-spec.md` before forming
  questions" step + an unpinned-"fixed"-term diff (clarify-phase feedback).
- **EvidenceGraph skill-path echo** — echoing each resolved `skill-id → SKILL.md path` in gate
  output (tasks-phase feedback).
- **Typed-front-door discoverability** — enumerating `FS.Skia.UI.Controls.Typed` in
  `docs/api-surface/` or a generated `catalog.yml`, plus a reflection-probe helper and a
  `fs-skia-typed-controls` recipe for recovering `Props` field names / `view` arity
  (plan/implement-phase feedback).
- **Verify-during-implement discipline** — `/speckit-implement` invoking the `run`/`verify` skills
  to launch + interact with an interactive-UI feature on the production render path before any
  interactive story is marked done, plus an evidence check that captures the production render
  path (implement-phase root-cause).
- New platforms, distribution, or release changes.
