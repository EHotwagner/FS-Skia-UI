# Feature Specification: Live Interactive Control Responsiveness — Authored Event-Binding Dispatch in `runInteractiveApp`, Keyed-Ancestor Pointer Recovery, Text-Input Routing Boundary & a Responds-vs-Renders Runtime Proof

**Feature Branch**: `090-interactive-host-event-dispatch`
**Created**: 2026-06-10
**Status**: Draft
**Input**: User description: "create specs from the feedback from the sibling repo controlsshowcase2"

## Context & Triage *(informative)*

A second generated consumer — a **53-module Controls Gallery** (`ControlsShowcase2`, feature
`001-controls-showcase-gallery`) generated from `FS.Skia.UI` — left per-phase Spec Kit feedback
under
`ControlsShowcase2/specs/001-controls-showcase-gallery/feedback/{specify,plan,analyze,implement}-2026-06-10.md`.
The **implement** phase was raised to **severity: major** after live testing: the build, **39/39**
tests, evidence, and both merge gates (`EvidenceAudit` PASS) were all green, yet **the running app
did not respond to user input in the live window** — clicks produced no visible change and
number-key page switching stalled after clicks. A gallery whose entire purpose is live, interactive
controls shipping "green-everything-but-dead-window" is the headline finding of this round.

This is the **next** consumer after `ControlsShowcase1` (which drove 086/089). Feature **089**
shipped the `VERIFY-IMPL-1` *workflow discipline* — "an interactive-UI story is not done until it
has been **run and used** on the production render path." `ControlsShowcase2` is the first consumer
to operate under that discipline, and running it is exactly what surfaced the **underlying framework
defect the discipline detects but does not fix**: the interactive host renders faithfully but never
dispatches the event bindings a consumer authors. That defect — and the pointer/keyboard recovery
primitives around it — is this feature.

All findings below were triaged against **current framework source** so already-shipped items are
**not** re-specified.

### Confirmed present and therefore OUT of scope (verified in source)

| Reported item | Current-state evidence (already shipped / working) |
|---------------|-----------------------------------------------------|
| **Run-and-use implement discipline** ("run it and use it before done"; renders-vs-responds *workflow* gate) | Shipped by **089 VERIFY-IMPL-1** (merged `e4311449`): `.agents/skills/speckit-implement` now requires a run-and-use step on the production render path for interactive-UI stories. This feature adds the *framework-side capturable proof* that discipline asks for, not the discipline itself. |
| **WCAG contrast / relative-luminance helper** (consumer hand-wrote `relativeLuminance` + `contrastRatio` in `Model.fs`) | Already public: `src/Color/Contrast.fsi` exposes `relativeLuminance: Color -> float`, `ratio: Color -> Color -> float`, `check`, `verdict`, `compositeOver`, `checkPaint`. Consumer should consume `FS.Skia.UI.Color.Contrast` instead of re-deriving. |
| **Skillist-token registry validator** + **`name:`-vs-directory resolution echo** | Already shipped: `build/Governance/Evidence/Audit.fs:150` resolves each declared skill id via `SkillRegistry` and rejects unresolved ids; **089 EVGRAPH-ECHO-1** added the `id → SKILL.md path` echo to `EvidenceGraph` output. The analyze-phase skillist mismatch resolves mechanically today. |
| **Repaint-on-update** ("static first frame while state changes invisibly") | Not a defect: `src/SkiaViewer/SkiaViewer.fs:2469` re-renders `host.View` after every `host.Update` (`dispatchHostMsg` recomputes `currentScene`). The live loop repaints per dispatched message. |
| **Number-key delivery / key warm-up** ("shortcuts blocked after clicks") | The pre-ready key **warm-up FIFO** shipped in 086 (`src/SkiaViewer/SkiaViewer.fs:1380`+, drain at `:1505`). The consumer's "blocked after clicks" was traced in its own feedback to a self-built relaunch loop stealing focus, not a framework key-delivery defect. |
| **Keyed-*leaf* hit-test** (a directly-keyed Button/CheckBox/Slider returns its `withKey` id) | Already correct: `src/Controls/Control.fs:1052` derives `id = c.Key |> Option.defaultValue path`. A directly-keyed leaf routes fine; only **container-keyed** controls are affected (see `KEYED-ANCESTOR-1` below). |
| **Multi-file external-tree `source-spec.md` snapshot** (specify-phase curl loop) | Already in `speckit-specify` (085 FR-016); the consumer used it successfully. The "tiny helper" ask is a non-blocking tooling nicety, not a framework deliverable. |

### Open cluster addressed by this feature (triaged against current source)

| # | Sev | Source phase | Finding | Current-state evidence |
|---|-----|--------------|---------|------------------------|
| LIVE-DISPATCH-1 | **major (root cause)** | implement | **`runInteractiveApp` never dispatches authored control `EventBindings` (`onClick`/`onChanged`).** A consumer who authors controls the documented, obvious way (`Button.onClick`, `CheckBox.onChanged`) gets a live window where nothing is wired — the bindings are dead. Routing goes **only** through `host.MapPointer : PointerInteraction -> 'msg option`. Worse, the **published host contract is actively misleading**: the `.fsi` doc claims the host hit-tests "`Layout.hitTestComputed` × `EventBindings` by `ControlId`," which the implementation does not do. | `src/Controls.Elmish/ControlsElmish.fs:168` computes `rendered = Control.renderTree …` but uses only `rendered.Layout` (`:176`); `rendered.EventBindings` is **never referenced** in the host. Routing is purely `interpretPointerOutcome host.MapPointer interactions` (`:183`). Yet `ControlsElmish.fsi:135` documents "`Layout.hitTestComputed` × `EventBindings` by `ControlId`" — a contract the code does not honor. `EventBindings` is fully computed (`src/Controls/Control.fs:1169`, `src/Controls/Types.fsi:294`) and otherwise unused by the live host. |
| KEYED-ANCESTOR-1 | major (contributing) | implement | **Container-keyed controls are unroutable by hit-test.** `Layout.hitTestComputed` returns the **deepest laid-out positional node id** (`"0.1"`, `"button"`). A directly-keyed leaf returns its `withKey` id, but a control whose key sits on a **container** (date/time/color picker, combo/list box, split-view, context-menu, dialog) returns an inner child's positional id, so `MapPointer` cannot recover which authored control was hit. There is **no public "nearest keyed ancestor" helper**. | `src/Controls/Control.fs:1052` (`id = c.Key |> Option.defaultValue path`) keys per node, but the bounds `ControlId` fallback is `Kind`/positional (`:1129`), and `Layout.hitTestComputed` resolves to the deepest node; no API walks from a hit positional id to the nearest ancestor that carries a `withKey`/binding. |
| TEXT-INPUT-1 | minor | implement | **The pointer host has no focused-control text-input routing.** `TextBox`/`TextArea`/`NumericInput` cannot receive typed characters in `runInteractiveApp`: `host.MapKey` is **stateless** (no model/focus parameter), so there is no built-in "deliver this keystroke to the focused text control." Those controls are inherently non-interactive in the default host today, and nothing flags that. | `src/Controls.Elmish/ControlsElmish.fsi:50` `MapKey: ViewerKey -> bool -> 'msg option` carries no focus/model context; `SkiaViewer.fs:2490` invokes it without focus state. Focus *is* tracked internally (`ControlRuntimeMsg.FocusControl`) but is not observable from the host seam. |
| RESPONDS-EVIDENCE-1 | major | implement | **The evidence regime can pass on a *rendered* app that does not *respond*.** It proved the production render path (real screenshots, `proves-screenshot=true`) and even offscreen pointer *routing* (an offscreen `routeInteractivePointer` probe), yet the assembled live app was inert. No evidence obligation requires an **observed input → visible-change in the running window** (not a static-frame screenshot, not an offscreen route probe). | 089 added the *implement-skill discipline* but no **framework-side, capturable runtime proof** distinguishes "renders" from "responds." The offscreen probe (`runInteractivePointerOnce`, `ControlsElmish.fsi:130`) confirms model-layer routing only; passing it while the live window stays dead is exactly the observed outcome. |

### Deferred (real but out of this feature's theme — not interactivity)

These lower-severity governance/docs findings are genuinely open but orthogonal to live-interactive
responsiveness; bundling them would violate the single-feature discipline. They are recorded here so
the triage is complete and a future governance-docs feature can pick them up:

- **Demonstrable-control count in `catalog.yml`** (plan): `summary.supportedCount` is hand-tallied
  (52) and not machine-derivable from the 53 `module:`/`typedModule:` rows (Collections backs 7
  controls); no field distinguishes "demonstrable control" from "backing module."
- **Readiness-file value grammar** (implement): `build/Governance/Evidence/Scans.fs:30`
  (`parseKeyValues`/`truthy`/`falsy`, exact token sets) is documented only in engine source, not in
  consumer-facing `docs/evidence-formats.md`.
- **Must-survive evidence-CLI token manifest** (plan): the exhaustive must-survive scan-token list
  lives only in `GovernanceTests.fs` assertions; `docs/scaffold-map.md` lists categories, not the
  exhaustive flag list.
- **Durable symbol manifest** (implement): the ~40 names the "untouched" generated `Program.fs`
  imports from `Model`/`View`/… are discoverable only by reading `Program.fs`; no generated manifest
  enumerates the symbol contract a model-swap must preserve.
- **Spec-Kit niceties** (specify/analyze): a multi-file-GitHub-tree snapshot helper; a
  "success-criterion → ≥1 assertion task" analyze linter; hardening multi-file `after_implement`
  hook discovery so the mandatory feedback hook can't be missed.

**Change classification.** **Escalated / `maintainer-verify` (Tier 1).** LIVE-DISPATCH-1,
KEYED-ANCESTOR-1, and TEXT-INPUT-1 change **public runtime contracts** in `src/Controls/**` and
`src/Controls.Elmish/**` (new/changed `.fsi` signatures and corrected host-contract docs), which
ship to consumers and are emitted into `template/**`'s api-surface — `Route` is expected to escalate.
A new public `.fsi` signature in the controls/host packages is introduced, so surface baselines
(per-package and the published api-surface tree) must be recaptured, and the serialized six-target
order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph`
→ `EvidenceAudit`) run. RESPONDS-EVIDENCE-1 adds a runtime evidence obligation; if it touches the
`.agents` skill tree it must be regenerated into `.claude` via `RefreshSurfaceBaselines`
(`SkillSyncCheck`-enforced).

## Clarifications

### Session 2026-06-10

- Q: Does reaching Avalonia-class parity require redesigning the controls/event/styling
  architecture? → A: **No redesign.** The maintainer confirmed the architecture verdict: keep the
  immutable `Control<'msg>` + MVU core and evolve it toward **declarative-retained
  (SwiftUI/Jetpack-Compose-class) capability parity**, *not* retained-mode XAML/data-binding
  architecture parity. This feature (090) is **step 1** of that trajectory.
- Q: When a hit control has both an authored `EventBinding` and a matching `host.MapPointer`
  clause for the same interaction, what is the precedence? → A: **Authored binding wins;
  `MapPointer` is a fallback** that fires only for interactions no authored binding consumed (no
  double-dispatch).
- Q: Should feature 090 commit to a resolution for the text-input boundary (FR-008), or keep the
  (a)-seam / (b)-document either/or open? → A: **Commit to (a): build the focus-aware text-routing
  seam now**, so `TextBox`/`TextArea`/`NumericInput` are typeable in `runInteractiveApp` (not
  document-only). It wires the existing `ControlRuntime.FocusedControl` + `TextInput` machinery; a
  complete editor (caret/selection/IME) stays out of scope and the broader focus/tab-traversal system
  remains trajectory item **E4**.
- Q: What does the nearest-keyed-ancestor recovery (FR-004) return when the hit node has no
  keyed-or-bound ancestor anywhere in its path? → A: **None / unresolved** — the recovery is an
  option-returning (partial) function; on `None` the host falls back to `MapPointer` with the raw
  positional interaction (per FR-003). It never invents a `Kind`-based or root id the consumer did
  not author.
- Q: Is 090 scoped to the host dispatch mechanism (+ representative end-to-end verification), or a
  catalog-wide guarantee that every typed view exposes authored bindings? → A: **Host mechanism +
  representative verification.** 090 wires dispatch/recovery/text-seam/proof and proves the path
  end-to-end on a representative sample (a leaf-keyed, a container-keyed, and a text control); it does
  **not** audit/retrofit all 52 typed `Widgets/*.fs` views. Any per-control "typed view exposes no
  binding" gap is flagged for a separate fitness pass (overlapping the 089 typed-surface work).

**Architecture evolution trajectory *(informative — maintainer-confirmed 2026-06-10)*.** The dead-window
finding is the first symptom of a longer arc, not a one-off bug. 090 is scoped to step E1 only; E2–E5
are **future features** named here so 090 is understood as the first rung and downstream work has a
recorded target. The framework's existing strengths — the Yoga-style two-pass `Layout` core, design
tokens, accessibility metadata, the typed `Props` front door, and the already-built-but-parked
feature-067 reconciler — are the foundation this path builds on; none are discarded.

- **E1 — Live interactivity (this feature, 090):** authored `EventBindings` dispatch in
  `runInteractiveApp` (LIVE-DISPATCH-1), nearest-keyed-ancestor identity recovery for container-keyed
  controls (KEYED-ANCESTOR-1), the text-input routing boundary (TEXT-INPUT-1), and a responds-vs-renders
  runtime proof (RESPONDS-EVIDENCE-1). **Table stakes** — nothing downstream matters while the live
  window is inert.
- **E2 — Wire the parked reconciler into the render path** (feature 067 `Reconcile`, currently internal
  and unwired): gives each control a **stable identity across frames**. This is the linchpin unlock —
  it is the precondition for stable focus, per-control animation, visual-state transitions, and
  efficient partial updates — and it moves the framework from rebuild-every-frame immediate-mode to
  declarative-retained, scaling past redraw-the-world performance. Future feature.
- **E3 — Visual-state / style layer** over the existing design tokens: style classes + a
  state→style resolution (`Normal/Hover/Pressed/Focused/Selected/Disabled/Validation`) so styling is
  declarative without a CSS-selector engine. Builds on E2's identity. Future feature.
- **E4 — Focus / keyboard-traversal / input-routing system**: generalizes 090's focus-aware text
  seam (which delivers keystrokes to the focused *text* control) into a full focus model — tab order,
  traversal, and focused-control key delivery across **all** control kinds. Future feature.
- **E5 — Template / slot composition (optional, demand-driven):** a lookless-composition mechanism,
  pursued **only if** real consumers need to re-skin control shape. Not committed.

**Explicit non-goals (the rejected redesign).** Permanently out — *not* "deferred": XAML; a
data-binding / observable property graph; attached/dependency properties with coercion/inheritance; a
lookless `ControlTemplate` engine; and CSS-selector styling. Adopting these would mean discarding the
F#/MVU/determinism core (pure reducers, identity-at-rest, golden-diff evidence) to chase a model a
mature incumbent already owns; the project deliberately does not.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A clicked control authored with `onClick` produces its effect in the live window (Priority: P1)

A consumer authors controls the documented, obvious way — `Button.onClick`, `CheckBox.onChanged`,
etc. — and hosts the app with `runInteractiveApp`. When they click a control in the **running
window**, the bound message is dispatched, `host.Update` folds it, and the change is visible on the
next frame. The consumer does not have to discover that the default host ignores authored bindings,
and does not have to reimplement every control's routing inside `host.MapPointer`.

**Why this priority**: This is the major, severity-flagged root cause — the gap that let "39/39
tests + both gates + 11 real screenshots, all green" ship as a dead window. It is the single
standout framework deliverable: the framework's own interactive host must honor the framework's own
authored event bindings, or the documented authoring pattern is a trap.

**Independent test**: From a generated/headless harness, author a control tree with at least one
`onClick` and one `onChanged` binding, host it via `runInteractiveApp` (or its exposed
`routeInteractivePointer`/`runInteractivePointerOnce` seam exercised the way the live host wires it),
synthesize a press+release at the bound control's bounds, and confirm the **bound** message is
dispatched and folds into the model — without the consumer having authored any `MapPointer` clause
for that control. Confirm the published host contract (`ControlsElmish.fsi` doc) **accurately**
states whether and how authored `EventBindings` fire, with no claim the implementation does not honor.

### User Story 2 - Container-keyed controls (pickers, combo/list boxes, dialogs) are routable from a click (Priority: P1)

A consumer keys a composite control on its **container** (a date/time/color picker, combo/list box,
split-view, context-menu, or dialog) and clicks anywhere inside it in the running window. The host
recovers the authored control identity — the **nearest keyed ancestor** of the deepest hit positional
node — so the control's bound message fires, instead of an opaque inner positional id (`"0.1"`,
`"button"`) that resolves to nothing.

**Why this priority**: Without this, an entire class of the catalog's controls (every composite
keyed on a container) is unroutable in the live host even after US1 — the click lands on an inner
positional child whose id the consumer never authored. It is a co-equal contributor to the dead-window
outcome.

**Independent test**: Author a composite control whose `withKey` sits on its container, lay it out,
hit-test a point over an inner child, and confirm a public helper resolves the hit to the **nearest
ancestor carrying a key/binding** (the authored container id), and that the host routes the bound
message for that resolved id. Confirm a directly-keyed leaf still resolves to itself (no regression).

### User Story 3 - An interactive story must prove an observed input→visible-change in the running window (Priority: P2)

A maintainer (or agent) finishing an interactive-UI feature can produce, and the evidence regime
**requires**, a runtime proof that a real input in the **running window** produced a **visible change**
— distinct from a screenshot of a (possibly static) frame and distinct from an offscreen route probe.
A build whose live window is inert cannot satisfy this proof, so "renders" can no longer pass as
"responds."

**Why this priority**: This is the gate whose absence let the dead window ship green. It complements
089's *implement-skill discipline* with the *framework-side capturable artifact* that discipline asks
for. P2 because it depends on US1/US2 making the live window actually responsive first — a gate with
nothing to prove against is premature.

**Independent test**: Confirm the framework exposes a way to capture an **input→visible-change** proof
(e.g. before/after the same dispatched live interaction, the rendered output differs in a recorded,
decodable artifact), that this proof is **distinct** from a render-only screenshot and from an
offscreen route probe, and that an inert app (renders but does not respond) **fails** to produce it.
Confirm the obligation is expressed durably so it binds future interactive-UI features.

### User Story 4 - Text controls are typeable in the live host via a focus-aware routing seam (Priority: P3)

A consumer places a `TextBox`/`TextArea`/`NumericInput` in a `runInteractiveApp` window, clicks it to
focus it, and types — and the characters reach **that** control. The interactive host delivers
keystrokes (and committed/composed text) to the currently focused text control through a documented
seam built on the existing focus state and `TextInput` pipeline, so text input is no longer silently
inert.

**Why this priority**: Lowest blast radius of the cluster — it affects only text-entry controls — but
it closes the last "silently non-interactive" surface the consumer hit. Scoped to the routing seam;
a full editor UX and general focus/tab-traversal are trajectory item E4 (see Clarifications).

**Independent test**: In a harness, render a tree with a focusable text control, set focus (via a
pointer click on it, exercising the focus-on-click path), deliver a keystroke through the host's
text-routing seam, and confirm the character reaches the **focused** text control's `TextInput` model
(and not an unfocused one). Confirm the published `.fsi`/docs document the seam.

## Requirements *(mandatory)*

### Functional Requirements

**Authored event-binding dispatch in the live host (LIVE-DISPATCH-1)**

- **FR-001**: The framework's interactive host (`runInteractiveApp`, via the
  `routeInteractivePointer`/`runInteractivePointerOnce` seam it wires) MUST dispatch a hit control's
  authored `EventBindings` (e.g. `onClick`/`onChanged`) when that control is interacted with in the
  running window — so a consumer who authors controls the documented way sees clicks produce their
  bound effect, without reimplementing routing in `host.MapPointer`.
- **FR-002**: The published host contract (`ControlsElmish.fsi` doc comments and any consumer-facing
  host docs) MUST **accurately** describe whether and how authored `EventBindings` fire in the
  interactive host. The current doc claim that the host hit-tests "`Layout.hitTestComputed` ×
  `EventBindings` by `ControlId`" MUST be made true by FR-001 or corrected to match the
  implementation — the contract MUST NOT promise dispatch the code does not perform.
- **FR-003**: `host.MapPointer` MUST remain available as a host-level seam (it is not removed by
  FR-001). When both an authored `EventBinding` and a `MapPointer` clause could respond to the same
  interaction, **the authored binding wins and `MapPointer` is a fallback**: if the hit control has a
  matching authored binding, the host dispatches **only** that binding's message for that
  interaction; `MapPointer` is consulted **only** for interactions no authored binding consumed. The
  host MUST NOT dispatch both for the same interaction (no double-advance of the model).

> Interacting / conflicting requirements: authored per-control `EventBindings` (FR-001) vs the
> host-level `MapPointer` seam (FR-003). Resolve as (clarified 2026-06-10): the host joins the hit
> control's resolved keyed `ControlId` with its `EventBindings`; if a matching binding exists it
> dispatches that bound message and **does not** also invoke `MapPointer` for that interaction.
> `MapPointer` fires only for interactions with no consuming authored binding (e.g. unbound controls
> or host-level semantic routing). A control with **no** authored binding behaves exactly as today
> (routed only if `MapPointer` maps it), so the change is **additive** — existing `MapPointer`-only
> consumers are not broken, and no consumer sees two messages folded for one click.

**Keyed-ancestor pointer recovery (KEYED-ANCESTOR-1)**

- **FR-004**: The framework MUST expose a **public "nearest keyed ancestor" recovery** so that a hit
  on a deep positional node (`"0.1"`, `"button"`) inside a **container-keyed** control resolves to the
  nearest ancestor that carries a `withKey`/binding — i.e. the authored control identity — and the
  host MUST use this resolution when dispatching FR-001 bindings and when presenting the hit id to
  `MapPointer`.
- **FR-004a**: The recovery MUST be **option-returning (partial)**: when the hit node has **no**
  keyed-or-bound ancestor anywhere in its path, it returns **None** (unresolved). It MUST NOT invent a
  `Kind`-based or root-control id the consumer never authored. On `None`, the host falls back to
  `MapPointer` with the raw positional interaction (per FR-003).
- **FR-005**: FR-004 MUST be **non-regressive** for directly-keyed leaves: a leaf carrying its own
  `withKey` resolves to itself. The recovery only changes the previously-unroutable container-keyed
  case (and the unresolved case of FR-004a, which was already unroutable).
- **FR-005a**: This feature delivers the host **dispatch mechanism** and proves it **end-to-end on a
  representative sample** — at minimum one directly-keyed leaf control, one container-keyed composite,
  and one focused text control. It does **not** audit or retrofit the binding surface of all 52 typed
  front-door views (`src/Controls/Widgets/*.fs`); a per-control "typed view exposes no authored
  binding" gap discovered here is flagged for a separate fitness pass, not fixed catalog-wide in 090.

**Live input→visible-change runtime proof (RESPONDS-EVIDENCE-1)**

- **FR-006**: The framework MUST make it possible to capture a **responds** proof: that a real input
  applied to the running interactive host produced a **visible change** in the rendered output —
  distinct from (a) a render-only screenshot of a single frame and (b) an offscreen
  `routeInteractivePointer` route probe that exercises only the model layer.
- **FR-007**: The evidence regime MUST treat that responds-proof as the obligation that distinguishes
  "renders" from "responds" for an interactive-UI story, such that an app whose live window is inert
  (renders but does not respond) **cannot** satisfy it. The obligation MUST be expressed durably (so
  it binds future interactive-UI features) and, where it lives in the `.agents` Spec Kit skill tree,
  MUST be regenerated into the `.claude` mirror.

> Interacting / conflicting requirements: FR-006 (a new responds-proof) vs the existing allowance
> that offscreen/headless capture is valid evidence — resolve as: offscreen render capture and
> offscreen route probes remain valid for what they prove (the *render* path; model-layer *routing*),
> but they do **not** satisfy the FR-007 responds-obligation, which specifically requires the
> input→visible-change pairing on the running host. This extends, not contradicts, 089's
> production-render-path rule.

**Text-input routing boundary (TEXT-INPUT-1)**

- **FR-008**: The framework MUST provide a **focus-aware text-routing seam** in the interactive host
  that delivers a keystroke (and committed/composed text) to the **currently focused** text control
  (`TextBox`/`TextArea`/`NumericInput`), so those controls are typeable in `runInteractiveApp`. The
  seam MUST route through the existing focus state (`ControlRuntime.FocusedControl`) and the existing
  `TextInput` model/message/effect pipeline rather than introducing a parallel text model. Pointer
  click on a text control MUST be able to set focus so a subsequent keystroke reaches it. The
  published host contract MUST document the seam (no silent inertness).
- **FR-008a**: FR-008's scope is the **routing seam only** — a complete text-editing experience
  (multi-selection gestures, IME composition UX beyond the existing `Composition` hooks, undo/redo)
  and a general focus/tab-traversal model across all controls are **out of scope** for 090 and belong
  to trajectory item **E4**. 090 delivers keystroke-to-focused-text-control; E4 generalizes focus.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identity* change. Package **contents** change for the controls and
  Controls.Elmish packages: the interactive host (`src/Controls.Elmish/**`) gains binding dispatch
  (FR-001) and a focus-aware text-routing seam (FR-008); the controls package (`src/Controls/**`) gains
  a public keyed-ancestor recovery helper (FR-004). Versions follow the normal merge bump (the libs,
  including `FS.Skia.UI.Build`, are bumped at merge); this spec pins no version. No legacy Charts
  package migration is involved.
- **Public contract impact**: **Public `.fsi` signatures change.** New public surface in
  `src/Controls/**` (the nearest-keyed-ancestor helper, FR-004) and `src/Controls.Elmish/**`
  (binding-dispatch behavior + the focus-aware text-routing seam, FR-001/FR-008), plus a **corrected host
  contract doc** in `ControlsElmish.fsi` (FR-002, today's claim is false). Per-package surface
  baselines and the published `docs/api-surface` tree (emitted to `template/base/docs/api-surface/`)
  MUST be recaptured.
- **State workflow impact**: The interactive host's **dispatch behavior changes** (FR-001): the
  fold now also fires authored per-control bindings, not only `MapPointer` results. This is additive
  (no authored binding ⇒ unchanged behavior) but it is a real change to how product messages are
  produced in the live loop; `host.Update` folding is otherwise unchanged. No new effects,
  subscriptions, or interpreter behavior.
- **Layout/rendering impact**: No change to *what* the renderer draws or to layout math. FR-004 reads
  the existing layout/hit-test result to recover a keyed ancestor; FR-006's responds-proof observes
  rendered output before/after a live interaction but does not alter rendering. No Vulkan/Skia or
  unsupported-environment behavior changes.
- **Evidence obligations**: Real evidence paths — a captured **input→visible-change responds-proof**
  on the running host (FR-006), distinct from the existing render-only screenshots and the offscreen
  route probe; the recaptured per-package and `docs/api-surface` baselines passing currency; the
  corrected `ControlsElmish.fsi` host-contract doc; and, if RESPONDS-EVIDENCE-1 touches the skill
  tree, the regenerated `.claude` skills matching their `.agents` sources (`SkillSyncCheck`). The
  serialized six-target order passing on the change.
- **Unsupported scope**: Out of scope — the deferred governance/docs items listed in Context & Triage
  (catalog demonstrable-count, readiness value-grammar docs, must-survive token manifest, durable
  symbol manifest, Spec-Kit snapshot/linter/hook niceties); any new package identity or version;
  release/platform/distribution changes; and — beyond FR-008's focus-aware routing seam — a complete
  text-editing UX (selection gestures, IME UX beyond existing `Composition` hooks, undo/redo) and a
  general focus/tab-traversal model (trajectory item E4).
- **Build-target impact**: `Dev` (host + helper behavior and tests), `GeneratedGuidanceCheck` and
  `TemplateCheck` (recaptured api-surface emitted into the template), `GeneratedProductCheck`
  (generated-product currency), `EvidenceGraph` and `EvidenceAudit` (the new responds-proof
  obligation), and `RefreshSurfaceBaselines` (regenerate any touched `.claude` skill + surface
  baselines) must change/run. `TargetMetadataDrift` / `SkillSyncCheck` enforce currency of the
  generated artifacts.

## Success Criteria *(mandatory)*

- **SC-001**: A control authored with `onClick`/`onChanged` and hosted via `runInteractiveApp` has
  its bound message dispatched when interacted with in the running window — for **100%** of catalog
  controls that carry an authored binding — with the consumer authoring **zero** `MapPointer` clauses
  for those controls. A control with no authored binding behaves exactly as before (no regression for
  `MapPointer`-only consumers).
- **SC-002**: The published host contract accurately states whether and how authored `EventBindings`
  fire; there is **no** doc claim of dispatch the implementation does not perform (the current false
  "`Layout.hitTestComputed` × `EventBindings`" claim is either made true or corrected).
- **SC-003**: A click anywhere inside a **container-keyed** composite control (picker, combo/list box,
  split-view, context-menu, dialog) resolves to the authored container identity via a public
  nearest-keyed-ancestor recovery and routes its bound message; a directly-keyed leaf still resolves
  to itself.
- **SC-004**: An interactive-UI story can produce a captured **input→visible-change** responds-proof
  on the running host that an **inert** app (renders but does not respond) cannot produce; the proof
  is recognized by the evidence regime as distinct from a render-only screenshot and from an
  offscreen route probe.
- **SC-005**: A text control placed in a `runInteractiveApp` window, once focused (including via a
  pointer click on it), receives typed characters at that control — proven by a test that a keystroke
  delivered through the host's text-routing seam reaches the **focused** text control's `TextInput`
  model and not an unfocused one; the seam is documented in the published contract.
- **SC-006**: The serialized six-target order passes on the change, the recaptured per-package and
  `docs/api-surface` baselines are current, and any touched `.claude` skill mirror matches its
  `.agents` source.

## Assumptions

- `EventBindings` is already computed on every `ControlRenderResult`
  (`src/Controls/Control.fs:1169`, keyed by `ControlId` per `src/Controls/Types.fsi:294`); FR-001
  *consumes* that existing data in the host, it does not introduce a new way to author bindings.
- "Interactive-UI user story" (FR-007) keeps the 089/Assumptions identification: a story whose
  acceptance involves pointer/keyboard interaction with a live host; the responds-proof obligation
  applies to those stories and is a no-op for non-interactive ones.
- The nearest-keyed-ancestor recovery (FR-004) is expressible over the existing layout/hit-test
  result (`Layout.hitTestComputed` plus the per-node key/`ControlId` already derived at
  `src/Controls/Control.fs:1052`) without changing layout math.
- The responds-proof (FR-006) builds on the existing repaint-on-update loop
  (`src/SkiaViewer/SkiaViewer.fs:2469`) — which already re-renders after each dispatched message — so
  the proof is "the rendered output differs after a dispatched live interaction," not new rendering
  machinery.
- FR-008 commits to the **focus-aware text-routing seam** (clarified 2026-06-10). This is feasible
  within 090 because focus is already tracked (`ControlRuntime.FocusedControl`) and a full
  `TextInput` model/message/effect pipeline already exists (`src/Controls/TextInput.fsi`) — the seam
  wires the host to deliver a keystroke to the focused control's existing `TextInputMsg`, not a new
  focus or text model. General focus/tab-traversal across all controls remains trajectory item E4.
- Versioning/packing follows the repo's normal merge flow (libs incl. `FS.Skia.UI.Build` bumped at
  merge); this spec does not pin a target version.

## Out of Scope

- Everything in the **OUT of scope** triage table (run-and-use discipline already in 089; WCAG
  contrast helper already in `src/Color/Contrast.fsi`; skillist validator + `EvidenceGraph` echo
  already shipped; repaint-on-update; key warm-up; keyed-leaf hit-test; multi-file snapshot helper) —
  **already shipped or working**; re-verifying them is not part of this feature.
- The **Deferred** governance/docs items (catalog demonstrable-count, readiness value-grammar docs,
  must-survive token manifest, durable symbol manifest, Spec-Kit snapshot/linter/hook niceties) —
  real but orthogonal to live-interactive responsiveness; a future governance-docs feature.
- Beyond FR-008's focus-aware routing seam: a complete text-editing experience (selection gestures,
  IME UX beyond existing `Composition` hooks, undo/redo) and a general focus/tab-traversal model —
  the latter is trajectory item E4. 090 delivers keystroke-to-focused-text-control only.
- A **catalog-wide audit/retrofit** of the 52 typed front-door views' (`src/Controls/Widgets/*.fs`)
  event-binding surfaces (FR-005a) — 090 proves the dispatch path on a representative sample; ensuring
  every typed view exposes authored bindings is a separate fitness pass (overlapping 089).
- New package identities/versions, release/platform/distribution changes, and any tooling outside
  `src/Controls/**`, `src/Controls.Elmish/**`, the published api-surface, and the `build/Governance` +
  `.agents`/`.claude` evidence/skill spine touched by RESPONDS-EVIDENCE-1.

## Dependencies

- The interactive host seam: `src/Controls.Elmish/ControlsElmish.fs(i)` (`InteractiveAppHost`,
  `runInteractiveApp`, `routeInteractivePointer`, `runInteractivePointerOnce`, `interpretPointerOutcome`).
- The render/hit-test seam: `src/Controls/Control.fs` (`renderTree`, per-node id derivation,
  `EventBindings`), `src/Controls/Types.fsi` (`ControlRenderResult`, `ControlEventBinding`),
  `src/Controls/Pointer.fsi`, and `FS.Skia.UI.Layout.Layout.hitTestComputed`.
- The live loop / repaint: `src/SkiaViewer/SkiaViewer.fs` (`dispatchHostMsg` repaint at `:2469`).
- The evidence/skill spine: `build/Governance/Evidence/**` and the `.agents`→`.claude` skill tree via
  `RefreshSurfaceBaselines` / `SkillSyncCheck` (only if RESPONDS-EVIDENCE-1 adds skill-tree guidance).
- Source feedback (sibling project, in its repo):
  `ControlsShowcase2/specs/001-controls-showcase-gallery/feedback/{specify,plan,analyze,implement}-2026-06-10.md`
  (the `implement` file, severity **major**, is the primary driver).
- Prior context already shipped: feature **089** (`VERIFY-IMPL-1` run-and-use discipline,
  `EVGRAPH-ECHO-1` skill-path echo) and **086** (interactive consumer fitness primitives, key warm-up).
