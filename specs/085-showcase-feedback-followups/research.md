# Phase 0 Research — 085 ControlsShowcase Feedback Follow-ups

All findings below were triaged against **current source** (not a stale build) on
2026-06-09. Each resolves a NEEDS-CLARIFICATION or a technology/integration choice
for the four capabilities + doc/skill work.

## D0. Route tier today vs. predicted escalation (FR-018)

- **Observed**: `./fake.sh build -t Route` on the *current* working tree prints
  `tier=focused-authority`, `gates=Dev, GeneratedGuidanceCheck, TemplateDrift`,
  `matched-rules=specify-catchall`. It does **not** escalate yet.
- **Decision**: This is correct and expected. The current diff is **spec-only**
  (`specs/085-…/**` + `.specify/feature.json`). FR-018's escalation prediction is
  about the **implementation** diff — once edits land in `src/Controls/**/*.fsi`,
  `src/SkiaViewer/SkiaViewer.fsi`, `template/**`, `.agents/skills/**`, and the
  governance templates, `Route` escalates to `maintainer-verify` (Tier 1).
- **Rationale**: `Route` reads the working-tree diff; it cannot escalate on paths
  that have not been touched. The plan therefore instructs implementers to **re-run
  `Route` after the contract-bearing edits exist** and only then run the serialized
  six-target order. Treating FR-018 as "must escalate now" would be a false reading.
- **Alternatives considered**: Forcing a dogfood escalation via feature metadata —
  rejected; the natural path-based escalation is the contract and needs no override.

## D1. Skill home for the host-contract facts (FR-011) — name collision

- **Finding**: FR-011/DOC-SKILL assert "there is no `fs-skia-skiaviewer` skill" and
  prescribe a **new** `.agents/skills/fs-skia-skiaviewer/`. That premise is **factually
  off**: a package-owned skill already exists at `src/SkiaViewer/skill/SKILL.md` with
  `name: fs-skia-skiaviewer`, registered in `constitution.md` (Local Agent Skills).
  Minting a second skill with the same `name:` would collide under `SkillSyncCheck`.
- **Decision (confirmed with requester)**: Author a **new** repo-local skill
  `.agents/skills/fs-skia-viewer-host/` (`name: fs-skia-viewer-host`) carrying the
  FR-011 facts (host input surface incl. the new pointer seam; preview-vs-tree;
  windowed-fullscreen blur caveat + workaround). The existing package-owned
  `fs-skia-skiaviewer` skill is **left unchanged**.
- **Rationale**: Honors FR-011's literal `.agents/skills/` placement, avoids the
  `name:` collision, and keeps the package skill's narrower "viewer host contracts"
  scope intact. The new skill is the **consumer-facing** host-usage home.
- **Currency consequence**: a brand-new `.agents/skills/<id>` requires
  `RefreshSurfaceBaselines` to regenerate `skillist-reference.md` and the `.claude`
  mirror; verify via `Governance.Tests` (watch the trailing-newline drift gotcha).
  Confirm whether the constitution's capability-skill inventory must also list it
  (add it if `SkillSyncCheck`/`SkillQualityCheck` requires; otherwise leave the
  inventory to package + already-registered governance skills).
- **Spec reconciliation**: FR-011/DOC-SKILL text should be read as satisfied by the
  distinct-named skill; the `fs-skia-skiaviewer` wording in the spec is superseded by
  this decision (record in plan Complexity/Deviations).

## D2. FR-001 — live nested-tree renderer

- **Decision**: Add `Control.renderTree : theme: Theme -> size: Size ->
  control: Control<'msg> -> ControlRenderResult<'msg>` to `src/Controls/Control.fsi`
  (+ `.fs`). It performs a **real recursive Yoga layout** over the full nested tree
  at the supplied output `size`, then paints **every** node (nested containers and
  their children), returning the existing `ControlRenderResult<'msg>` (which already
  carries `Scene`, `Layout: LayoutNode`, `EventBindings`, `Diagnostics`, `NodeCount`).
- **Rationale**: Reusing `ControlRenderResult` is deliberate — the host's pointer
  hit-testing (D3) needs `Layout` bounds keyed by `ControlId` correlated with
  `EventBindings`. `Size` is the existing `Scene.Size` record. The Yoga engine is the
  `fs-skia-layout` capability; nesting + real paint is exactly what `render` (the
  Feature-080 single-control **preview**) does **not** do.
- **FR-003 preservation**: `Control.render` / `Widget.render` are **untouched**; the
  Feature-080 preview goldens (`ControlFidelityCheck`) stay green. `renderTree` is
  strictly additive — a new `val`, new goldens of its own (two structurally different
  trees ⇒ visibly different `Scene`, SC-001).
- **Alternatives considered**: (a) Overloading/replacing `render` with a size param —
  rejected, breaks the 080 contract and the preview goldens. (b) Returning a bare
  `Scene` — rejected, the host needs the `Layout`+`EventBindings` for hit-testing.

## D3. FR-004/005/006/009 — pointer routing + size-aware host (additive variant)

- **Constraint**: Adding fields to the existing `GeneratedAppHost` record would
  break every existing construction site **and** the durable `GovernanceTests`
  `Viewer.runApp viewerOptions generatedHost` literal (FR-006). F# records have no
  optional fields.
- **Decision**: Introduce an **additive** host variant, leaving `GeneratedAppHost`
  and `Viewer.runApp` byte-for-byte:
  - New record `InteractiveAppHost<'model,'msg>` in `src/SkiaViewer/SkiaViewer.fsi`:
    - `Init: unit -> 'model * ViewerEffect list`
    - `Update: 'msg -> 'model -> 'model * ViewerEffect list`
    - `View: Size -> 'model -> SceneNode`  *(size-aware — FR-009)*
    - `MapKey: ViewerKey -> bool -> 'msg option`
    - `MapPointer: PointerInteraction -> 'msg option`  *(pointer seam — FR-004)*
    - `Tick: TimeSpan -> 'msg option`
    - `Diagnostics: ViewerDiagnosticsOptions`
  - New entry point `Viewer.runInteractiveApp : options: ViewerOptions ->
    host: InteractiveAppHost<'model,'msg> -> Result<ViewerLaunchOutcome,
    ViewerRunFailure>`.
- **Pointer wiring**: `runInteractiveApp` renders the current view through
  `Control.renderTree` (size-aware), then on each `ViewerEvent.Pointer*`
  (`Pressed/Moved/Released/Scrolled/Exited`) **hit-tests** the rendered
  `ControlRenderResult.Layout` bounds by `ControlId`, builds `PointerInteraction`
  values (applying the existing **4px click/drag fold**), and routes through
  `ControlsElmish.interpretPointerOutcome host.MapPointer` → `msg` → `host.Update`.
  No new hit-test or fold logic is invented (per spec Assumption) — it reuses the
  shipped `Controls.Elmish` pipeline.
- **Why hit-test via `Layout`, not new binding fields**: `ControlEventBinding` =
  `{ ControlId; EventKind; Dispatch }` carries **no bounds**. Correlating
  `EventBindings` with `LayoutNode` bounds by `ControlId` keeps
  `ControlEventBinding` unchanged (no extra public surface) and reuses what
  `renderTree` already returns.
- **MVU/effect boundary (Constitution IV)**: `host.Update` stays **pure**; pointer
  events become **data** (`PointerInteraction`) before reaching `MapPointer`; the
  interpreter edge is `runInteractiveApp`'s loop. This satisfies the Elmish boundary
  for the new I/O-bearing input path.
- **Alternatives considered**: (a) Add `MapPointer` to `GeneratedAppHost` directly —
  rejected, record-construction + GovernanceTests-literal break (FR-006). (b) A
  callback-style `ViewerEffect` for size — folded into the size-aware `View` instead,
  which is the simpler seam the spec's interacting-requirements note prefers.

### D3-AMEND (implementation-time, 2026-06-09) — host package home + View type

Two corrections were forced once the implementation diff met the real package graph
(confirmed with the requester during `/speckit.implement`):

1. **Package home = `Controls.Elmish`, not `SkiaViewer`.** `PointerInteraction` lives in
   the **Controls** package (`src/Controls/Pointer.fsi`) and `interpretPointerOutcome` in
   **Controls.Elmish**. The project graph is `SkiaViewer → {Scene, KeyboardInput}` and
   `Controls.Elmish → {Controls, KeyboardInput}` (siblings; SkiaViewer references neither
   Controls nor Controls.Elmish). Hosting `runInteractiveApp` in SkiaViewer would force a
   boundary-inverting `SkiaViewer → Controls.Elmish` edge (acyclic but pulls the whole
   Controls/Layout/adapter stack into the viewer, contradicting the plan's "no dependency
   change" claim and the documented "viewer is host-independent" boundary). Instead the new
   `InteractiveAppHost<'model,'msg>` + `Viewer.runInteractiveApp` land in
   **`src/Controls.Elmish/ControlsElmish.fsi`**, adding an acyclic `Controls.Elmish →
   SkiaViewer` edge — exactly the "Elmish wires viewer hosting into a program" boundary.
   This is a genuine **dependency change**: `Directory.Packages.props` is unaffected (no new
   package), but `Controls.Elmish.fsproj` gains a `SkiaViewer` ProjectReference and
   `DependencyReport`/per-package surface move. FR-004/FR-006 are read as capability
   requirements (the additive host + preserved `runApp` literal), not file-location
   requirements; tasks T006/T017/T024 file references are superseded accordingly.

2. **`View: Size -> 'model -> Control<'msg>`, not `... -> SceneNode`.** Pointer routing by
   `ControlId` requires a `Control` tree (so `Control.renderTree` yields `Scene` + `Layout`
   + `EventBindings`); a bare `SceneNode` carries no `ControlId`s and cannot be hit-tested.
   `runInteractiveApp` lowers the `Control<'msg>` view to a `SceneNode` for the native host
   via `Control.renderTree theme size view |> _.Scene`. The data-model's `SceneNode` return
   is corrected to `Control<'msg>`.

3. **Real pointer delivery is feasible.** `src/SkiaViewer/Host/Vulkan.fs` already subscribes
   to native mouse move/down/up/scroll and raises `Host.ViewerEvent.Pointer*`; only
   `Viewer.runApp`'s event mapper drops them (`-> None`). The interactive path surfaces those
   raw pointer events to the Controls.Elmish router via a pointer-aware SkiaViewer host
   variant, so the live window routes clicks through the same code the synthetic-event test
   drives (research D6 honest bar).

## D4. FR-007/008 — `normalize` toolkit key-name families

- **Decision**: Add match arms to `KeyboardInput.normalize` (`.fs` only — the
  `ViewerKey` **union is unchanged**, FR-007) **before** the terminal `Unknown raw`:
  - Digit families (lowercased): `number{n}`, `digit{n}`, `keypad{n}` where `{n}` is a
    single `0-9` ⇒ `Digit n`.
  - `key{suffix}` (lowercased `key…`): strip `key`, then single digit ⇒ `Digit n`,
    single letter ⇒ `Letter (ToUpperInvariant)`. This resolves the `Key5`-vs-`KeyL`
    ambiguity in one arm.
- **FR-008 totality**: the terminal `| _ -> Unknown raw` arm is preserved; any
  unrecognized name still yields `Unknown raw`, and existing recognized names
  (arrows/named/function/bare-char) are untouched (no regression).
- **Test (SC-003)**: `Number5/Digit5/Keypad5/Key5` ⇒ `Digit 5`; `KeyL` ⇒ `Letter 'L'`;
  an unrecognized name ⇒ `Unknown raw`. Failing-first semantic test in
  `KeyboardInput.Tests`.
- **Alternatives considered**: A regex pass — rejected; simple `StartsWith`/length
  guards are plainer (Constitution III) and total.

## D5. FR-010..016 — documentation & governance edits

| FR | Artifact | Edit |
|----|----------|------|
| FR-010 | new `fs-skia-viewer-host` skill + `template/base/docs/scaffold-map.md` | windowed-fullscreen scales a fixed scene up ⇒ blur; workaround = size-aware `View` **or** one documented flag (`--window-startup normal` for 1:1 / raise design resolution / `InitialSize`). |
| FR-011 | **new** `.agents/skills/fs-skia-viewer-host/SKILL.md` (+ generated `.claude` mirror) | host input surface (keyboard `MapKey`; pointer `MapPointer`/`runInteractiveApp`); preview-vs-tree (`Control.render` preview vs `Control.renderTree`); blur caveat. |
| FR-012 | `.agents/skills/fs-skia-typed-controls/SKILL.md` | consumer-side note: author whole-catalog consumers via `FS.Skia.UI.Controls.Typed.*`; verify availability from the package / `catalog.yml` `module:` field, **not** `docs/api-surface/`; deterministic typed-surface probe recipe. |
| FR-013 | `template/base/docs/scaffold-map.md` | typed front door is **absent** from `docs/api-surface/` (which exposes only the legacy `X.create` builder surface); how to enumerate the typed surface instead. |
| FR-014 | `.specify/templates/spec-template.md` | one-line note: the **Framework Governance Prompts** section is **exempt** from the "no implementation details" rule. |
| FR-015 | `template/base/docs/evidence-formats.md` (and/or `fs-skia-evidence-mode` skill) | evidence token parsing reads **`key=value` lines**; a markdown table with the same tokens does **not** satisfy the validators. |
| FR-016 | `.agents/skills/speckit-specify/SKILL.md` | recipe for a **multi-file** external source: enumerate a GitHub tree (contents API), fetch per file, assemble one `source-spec.md` with per-file headers. |

- **`catalog.yml` confirmation**: every control entry carries a `module:` field
  (e.g. `module: TextBlock`, `module: Button`) — this is the authoritative
  typed-front-door probe surface for FR-012 (not `docs/api-surface/`).
- **Currency (FR-017)**: new public `.fsi` (`renderTree`, `InteractiveAppHost`,
  `runInteractiveApp`) must move the surface baselines; the new skill must be
  registered and the `.claude` tree + `skillist-reference.md` regenerated via
  `RefreshSurfaceBaselines`; per-package `.fsi.txt` snapshots regenerated via
  `PerPackageSurface.captureCurrent` (RefreshSurfaceBaselines skips those).
  `SkillSyncCheck` / `SkillQualityCheck` / `TargetMetadataDrift` must stay green.

## D6. Evidence environment honesty

- A full **live** pointer confirmation may require a key/pointer injection tool absent
  from the headless host. **Decision**: the honest bar is **synthetic-event +
  headless-adapter** evidence through the host/adapter path (deliver a synthetic
  `PointerPressed`/`PointerReleased` at a control's bounds and observe the bound
  message dispatched + model change). Where live injection is genuinely unavailable,
  this is disclosed honestly per `fs-skia-evidence-mode` (benign/deferred), **not**
  marked `[S]` unless the evidence is wholly fabricated literal data.
