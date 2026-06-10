# Phase 0 Research: Live Interactive Host Event Dispatch

All four findings were triaged against **current framework source** (spec Context & Triage). This file
records the seam-by-seam facts that ground the plan and resolves the four open design decisions
(D1–D4). No `NEEDS CLARIFICATION` remains.

## Seam facts (verified in source)

### The dead-window root cause is exactly one unused field

`routeInteractivePointer` (`src/Controls.Elmish/ControlsElmish.fs:168-186`) renders
`rendered = Control.renderTree host.Theme size (host.View size model)`, then uses **only**
`rendered.Layout` (`:176`) to build the `LayoutResult` it feeds to `Pointer.update`. The emitted
`interactions` are routed solely through `interpretPointerOutcome host.MapPointer` (`:183`).
`rendered.EventBindings` — fully computed at `Control.fs:1169`
(`ControlInternals.recursively ControlInternals.eventBindings control`) and typed at
`Types.fsi:294` — is **never referenced** by the host. The published doc at `ControlsElmish.fsi:135`
("`Layout.hitTestComputed` × `EventBindings` by `ControlId`") describes behavior the code does not
perform. **Decision:** consume the already-computed `EventBindings` in `routeInteractivePointer`;
correct the doc. **Rationale:** the data exists and is otherwise dead; this is the minimal,
additive fix. **Alternatives considered:** authoring a parallel binding registry (rejected — duplicates
existing `EventBindings`); moving routing into `SkiaViewer` (rejected — `PointerInteraction` /
`interpretPointerOutcome` are Controls surface, host lives in Controls.Elmish per
[[feature-085-interactive-host-and-vulkan-window-block]]).

### The id-space mismatch that makes container-keyed controls unroutable

Two id spaces collide:

| Producer | Keying rule | Source |
|----------|-------------|--------|
| Layout / hit-test (the id a `PointerInteraction` carries) | `c.Key \|> Option.defaultValue path` — **structural path** (`"0"`, `"0.1"`) when unkeyed | `Control.fs:1052` (`toLayout`) |
| `EventBindings` (the id a binding carries) | `control.Key \|> Option.defaultValue control.Kind` — **Kind** (`"button"`) when unkeyed | `Control.fs:116` (`eventBindings`) |

So a join by raw id only works when the **hit control is directly keyed** (both sides = `Key`). An
unkeyed leaf's interaction id is a path (`"0.1"`) but its binding id is its Kind (`"button"`) — they do
not match. A **container-keyed** composite is worse: the hit returns the *deepest inner positional* node
(`"0.1"`, the unkeyed child), whose path matches neither the container's `Key` nor any binding. This is
exactly KEYED-ANCESTOR-1. **The recovery is the bridge that maps a structural hit id → the authored
`ControlId` an `EventBinding` is keyed by.**

### Pointer interaction surface carries the hit ControlId

`PointerInteraction` (`Pointer.fsi:72-83`) cases (`Click`, `PressedDown`, `ReleasedUp`, `DragBegin`,
…) each carry `control: ControlId` — the structural id from the hit-test. The interaction → event-kind
mapping the host needs: `Click`→`"click"`; a value-changing interaction →`"changed"` (the binding
`EventKind` vocabulary is `eventKind` at `Control.fs:108-114`: `onClick`→`click`, `onChanged`→`changed`,
`onSelected`→`selected`, generic `on*`→lowercased suffix).

### Focus + TextInput already exist; only the host seam is missing

`ControlRuntime.FocusedControl: ControlId option` (`ControlRuntime.fsi:42`) and
`ControlRuntimeMsg.FocusControl of ControlId option` (`:54`) already track focus, and `Pointer.update`
already emits `FocusMovedByPointer` / focus runtime messages on click. A full `TextInput` MVU pipeline
exists (`TextInput.fsi`: `TextInputModel`, `TextInputMsg` incl. `CompositionStarted/Committed`,
`TextInputEffect`, pure `update`). What is missing is the **host seam**: `MapKey: ViewerKey -> bool ->
'msg option` (`ControlsElmish.fsi:51`) is **stateless** — no focus/model parameter — so the host cannot
deliver a keystroke to "the focused text control." `SkiaViewer.fs:2490` invokes `MapKey` with no focus
context. **The seam wires existing focus state + `TextInput.update`, not a new text model** (FR-008).

### The responds-proof can build on the existing repaint loop

`SkiaViewer.fs:2469` (`dispatchHostMsg`) already re-renders `host.View` after every `host.Update`, so a
dispatched message repaints. A responds-proof therefore = **render → apply a real dispatched interaction
→ render again → assert the two differ**, captured as a decodable artifact. Render-target PNG capture is
already available headless ([[controls-preview-render-pipeline]],
`SkiaViewer.captureScreenshotEvidence`/`ViewerRenderTargetPng`), so the proof needs **no live Vulkan
window** ([[feature-085-interactive-host-and-vulkan-window-block]]: fsi can't open a window, but
render-target PNGs work; a compiled host can if a live window is later wanted).

## Design decisions

### D1 — Nearest-keyed-ancestor recovery: shape & home

**Decision:** a **public, pure, option-returning** function in `src/Controls/**` (alongside `hitTest` in
`Control.fs`, exported in `Types.fsi`/`Control.fsi`) that, given the **control tree** (or render result)
and a **hit structural `ControlId`**, walks from the deepest matching node up its ancestry and returns
the **nearest ancestor that carries a `withKey` or an authored `EventBinding`**, as the authored
`ControlId` (`Key`, else that node's `Kind`). Returns `None` when no keyed/bound ancestor exists
anywhere on the path (FR-004a).

Walk algorithm (pure, deterministic, no clock/randomness — resume-safe):
- The hit id is either a **dotted path** (`"0.1.2"`, unkeyed nodes) or a **Key** string (authored leaf).
- Re-derive the structural-path → `Control` map by the same `toLayout` path scheme
  (`path + "." + index`, `Control.fs:1079`) so each node's path and its `Key`/bindings are known.
- From the hit node, ascend path-parents (`"0.1.2"`→`"0.1"`→`"0"`); for each, if that node has a `Key`
  **or** a non-empty authored `EventBindings`, return its authored `ControlId`. First match wins
  (nearest). Exhausted with no match → `None`.
- A directly-keyed leaf's hit id **is** its `Key`, so it resolves to itself (FR-005, non-regressive)
  without ascending.

**Rationale:** purely a read over data `renderTree` already has (FR-004 assumption); no layout-math
change; option-return keeps the host honest (never invents an id, FR-004a). **Alternatives:** (a) bake
the resolution into `Pointer.update` so interactions already carry authored ids — rejected: changes the
pure pointer reducer's contract and the meaning of `PointerInteraction.control`, broader blast radius;
(b) key every node by path in `EventBindings` too — rejected: changes binding identity semantics and
breaks the documented `ControlId = Key|>Kind` contract consumers match on. The standalone helper is the
smallest public addition and is independently testable (US2 independent test).

### D2 — Precedence join (authored binding wins; MapPointer fallback)

**Decision:** in `routeInteractivePointer`, fold the `interactions` list as: for each interaction, (1)
recover the authored `ControlId` (D1); (2) look up `rendered.EventBindings` for a binding whose
`ControlId` = recovered id **and** `EventKind` = the interaction's event kind; (3) if found, dispatch
`binding.Dispatch event` (synthesizing a `ControlEvent` from the interaction) and **do not** offer this
interaction to `MapPointer`; (4) if not found (no binding, or recovery `None`), offer the **raw**
interaction to `MapPointer` exactly as today. Concatenate per-interaction results preserving order.

**Rationale:** matches the clarified precedence (authored wins, no double-dispatch, FR-003) and keeps the
change **additive** — an interaction with no consuming binding behaves bit-for-bit as today, so
`MapPointer`-only consumers are unbroken (SC-001). **Alternatives:** dispatch both then dedupe (rejected
— violates "no double-advance"); make `MapPointer` win (rejected — contradicts the clarification and
makes authored bindings still effectively dead when a catch-all `MapPointer` exists).

### D3 — Responds-proof mechanism & home

**Decision:** expose a capture that takes the host + a synthesized live interaction (the same
`ViewerPointerInput` the live loop feeds `routeInteractivePointer`), renders **before**, applies the
interaction (route → `host.Update` fold → recompute scene exactly as `dispatchHostMsg` does), renders
**after**, and emits **both frames + a diff verdict** (e.g. pixel-or-scene inequality) as a decodable
artifact. Recognized by the evidence regime as a **third** evidence class distinct from (a) a render-only
screenshot (one frame, no interaction) and (b) the offscreen `runInteractivePointerOnce` route probe
(model layer only, no render). An **inert** host (renders but no model change) produces **identical**
before/after → the proof **fails** (FR-006). Home: a thin wrapper over the existing
`routeInteractivePointer` + render-target capture; whether it lands in `SkiaViewer` or `Controls.Elmish`
is settled at implementation by which already imports both render-target capture and the host — research
leans `Controls.Elmish` (it already owns `routeInteractivePointer` and the host) calling
`SkiaViewer.captureScreenshotEvidence`.

**Rationale:** reuses the repaint-on-update loop (FR-006 assumption) and existing headless render-target
capture; no new rendering machinery; honest render-only evidence ([[fs-skia-evidence-mode]]).
**Alternatives:** require a live Vulkan window for the proof (rejected — not capturable in CI/fsi, and
the spec's "running window" is satisfiable by the running *host* + render diff; [[feature-085-…]] shows
window persistence needs a compiled exe and a display); diff only the model (rejected — that's the
existing route probe, which the spec explicitly says is insufficient).

### D4 — Text-routing seam signature (avoid a breaking MapKey change)

**Decision:** keep the existing `MapKey: ViewerKey -> bool -> 'msg option` field unchanged (additive
posture) and add the focus-aware delivery **as host-internal routing plus an optional seam field**, not
by widening `MapKey`. Concretely: when a key/text event arrives and `ControlRuntime.FocusedControl` names
a focusable text control present in the rendered tree, the host routes the keystroke to that control's
`TextInput.update` and folds the resulting product message; otherwise it falls through to `MapKey` exactly
as today. The new public surface is a documented seam (e.g. an optional `MapText`/focus-aware field on
`InteractiveAppHost`, or a host-level text-delivery function) — final signature drafted `.fsi`-first in
FSI per Constitution Principle I before the `.fs` body. The published contract documents the seam (no
silent inertness, FR-008).

**Rationale:** widening `MapKey` to carry focus/model would be a **breaking** signature change to a
shipped field; an additive seam preserves existing `MapKey` consumers and matches the "additive" theme of
the whole feature. The seam reuses `FocusedControl` + `TextInput` (FR-008 assumption), so no new focus or
text model. **Alternatives:** replace `MapKey` with a model-aware variant (rejected — breaking; and
general focused-key delivery across *all* control kinds is trajectory **E4**, FR-008a, not 090); a
parallel text model (rejected by FR-008 explicitly). Scope guard: caret/selection/IME-UX/undo and
tab-traversal stay out (FR-008a).

## Cross-cutting notes

- **Surface baselines.** New/changed public `.fsi` in two packages → recapture per-package snapshots
  (`PerPackageSurface.captureCurrent`, [[per-package-baseline-not-in-refresh-target]] — `RefreshSurfaceBaselines`
  alone does **not** regenerate per-package `.fsi.txt`) and the published api-surface tree.
- **Skill sync.** If FR-007 edits `.agents/skills/fs-skia-evidence-mode/SKILL.md`, regenerate the
  `.claude` mirror via `RefreshSurfaceBaselines`; `SkillSyncCheck` enforces byte-identity. Watch the
  trailing-newline drift gotcha ([[refresh-surface-baselines-skillist-reference]]).
- **Internal visibility.** Any helper the tests need but the public surface should not expose goes via
  `<InternalsVisibleTo>` item (not `AssemblyInfo.fs`), and note `Control<'msg>` has **no equality**
  ([[internal-module-in-controls-gotchas]]) — recovery/proof tests must compare by projected fields/ids,
  not by structural control equality.
- **GeneratedProductCheck** fails locally for environment reasons unrelated to this change
  ([[generated-product-check-env-failure]]) — treat that specific failure as non-authoritative, but still
  run it in the serialized order and capture the log.
</content>
