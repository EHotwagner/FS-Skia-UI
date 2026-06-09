# Phase 0 Research: Interactive Non-Game Consumer Fitness

**Feature**: `086-interactive-consumer-fitness` | **Date**: 2026-06-09

All NEEDS CLARIFICATION items below are the design choices the spec explicitly
deferred to `/speckit-plan` (spec Assumptions: translate shape, sized-text shape,
family marker, key warm-up mitigation) plus the two layout-mechanism choices
implied by FR-007/FR-008/FR-009 and FR-011/FR-012. Each is resolved against
**current source** (confirmed by reading the cited files this session).

---

## D1 — Scene translate/offset primitive shape (FR-013, SCENE-XLATE-1)

**Decision**: Add a single new `SceneNode` case `Translate of (float * float) * Scene`
(a `(dx, dy)` offset wrapping a child `Scene`), rendered by pushing a canvas
translation matrix around the wrapped sub-scene — **not** by walking and rewriting
every node's coordinates.

**Rationale**: `src/Scene/Scene.fsi:319-341` already models composition wrappers as
nesting cases (`ClipNode of Clip * Scene`, `ColorSpaceNode of ColorSpace * Scene`,
`PerspectiveNode of PerspectiveTransform * Scene`). `Translate` follows that exact,
idiomatic shape — a wrapper case the renderer interprets with a Skia
`canvas.Translate`/`Save`/`Restore`, so it offsets *every* descendant kind
(`Path`/`Points`/`Vertices`/`Chart`) uniformly with zero per-case coordinate math.
This is what kills the consumer's fragile hand-rolled `shiftNode` (which mishandles
origin-less nodes). A matrix push is also back-compat: existing scenes never see a
`Translate` node.

**Alternatives considered**:
- *`Group`-with-origin* (add an offset field to `Group`): rejected — changes the
  arity of an existing widely-pattern-matched case (`Group of Scene list`), breaking
  every existing match and the `Scene.describe`/golden surface. A new case is purely
  additive.
- *A `Transform of Matrix * Scene` general case*: rejected for this feature as
  over-built (Principle III); `PerspectiveNode` already covers the general-transform
  need, and the spec asks specifically for translate. A pure-offset case keeps the
  hit-test/measure story trivial.

**Surface impact**: `Scene.fsi` gains the case + a `Scene.translate dx dy scene`
smart constructor; `Scene.describe` gains a `TranslateElement` descriptor; the
renderer in `Scene.fs` handles the new case. Per-package + cross-package Scene
baselines recapture.

---

## D2 — Sized `Scene.Text` shape (FR-014, SCENE-TEXT-1)

**Decision**: Add a new sibling case `SizedText of (float * float) * string * float * Color`
(position, text, **font size**, color) and a `Scene.sizedText` constructor. Leave the
existing `Text of (float * float) * string * Color` case **untouched** so a `Text`
with no explicit size keeps its current default rendering (FR-014 back-compat clause,
Edge Case "Text node with no explicit size").

**Rationale**: `Scene.fsi:332` shows `Text` carries no size; `TextRun`/`FontSpec`
(`:168-178`) carries `Size: float` but is the heavyweight run path. Chrome text
(a nav-rail label) wants the simple positional API *plus* one size number. A new
case is additive and leaves the default-font golden behavior of bare `Text`
provably unchanged (the renderer routes `Text` through the existing default-size
path and `SizedText` through an explicit-size path that reuses the same glyph
layout as `TextRun`'s `FontSpec.Size`).

**Alternatives considered**:
- *Add an optional `float option` to `Text`*: rejected — same arity-break problem
  as D1's `Group` alternative; every existing `Text(pos, s, c)` match breaks.
- *Tell consumers to use `TextRun`*: rejected — that is the status quo the consumer
  found too heavy for one-line chrome labels; the spec explicitly asks the simple
  node to gain a size.

**Surface impact**: `Scene.fsi` case + constructor + `SizedTextElement` descriptor;
renderer handles it; Scene baselines recapture.

---

## D3 — `renderTree` horizontal `Stack` + documented horizontal kinds (FR-007)

**Decision**: Introduce a `Stack.orientation` attribute (an `Attr` reading
`"orientation" = "horizontal" | "vertical"`, default vertical) and change
`directionOf` in `Control.fs:1011-1018` from `directionOf kind` to take the whole
control so it returns `Row` when **either** the kind is a documented horizontal
container (`toolbar`/`split-view`/`wrap`/`grid`/`dock`) **or** the control carries
`orientation = horizontal`.

**Rationale**: `Control.fs:1011-1018` keys direction purely on `kind`, so a `Stack`
(kind `"stack"`) always falls to `Column` (LAYOUT-1). The `stack` catalog entry
(`Catalog.fs:141`) already documents Stack as *"vertical or horizontal child
composition"* — the orientation is a legitimate Stack property that simply isn't
wired. Reading an `orientation` attribute is the minimal idiomatic fix and reuses the
existing `ControlInternals.boolValue`/string-attr helper pattern (`Control.fs:42-99`).

**Alternatives considered**:
- *Make `stack` unconditionally Row*: rejected — breaks every existing vertical Stack
  (including the scaffold's own `controlsExampleView`).
- *A new `hstack` kind*: rejected — adds catalog surface and a typed front door for a
  property the existing Stack already documents; heavier than an attribute.

**Surface impact**: `Control.fs` (`directionOf` reads orientation), a `Stack.orientation`
(or `Stack.horizontal`) builder in the Stack module (`Control.fs:1215`), and the typed
front door if Stack has one. No `.fsi` *type* change — additive builder + behavior.

---

## D4 — Same-kind sibling bounds keying (FR-008) and explicit size (FR-009)

**Decision**: Replace the `Map`-keyed-by-`Key ?? Kind` bounds resolution
(`Control.fs:1069-1072,1101-1117`) with a **stable structural path id** assigned during
`toLayout` — each node gets a unique `LayoutNodeId` derived from its position in the
tree (e.g. parent-path + sibling index, preferring an explicit `Key` when present),
threaded identically into both `toLayout` and `paint` so lookups never collide.
FR-009 (explicit width/height reflected in bounds) already works in `toLayout`
(`Control.fs:1038-1047` reads `hasAttr "width"/"height"`); the collision was *masking*
it, so de-colliding the keys fixes FR-009 as a side effect — verified by an
explicit-size assertion.

**Rationale**: The defect (LAYOUT-1) is that `id = c.Key |> Option.defaultValue c.Kind`
makes two unkeyed `"panel"` siblings share one `Map` entry, so both paint at one box.
Yoga/`Layout.evaluate` already assigns every `LayoutNode` a distinct `Id` and returns
distinct `ComputedBounds` per node (`Types.fsi:147,162-165`); the bug is purely in how
`Control.renderTree` *re-keys* them. A deterministic structural path id (stable,
collision-free, derived without `Date.now`/randomness) is the standard fix and keeps
the layout pure/total.

**Alternatives considered**:
- *Auto-suffix duplicate kinds (`panel`, `panel#1`)*: rejected — order-fragile and
  leaks into any id a consumer might observe.
- *Require consumers to key every sibling*: rejected — the spec's Edge Case and FR-008
  explicitly require **unkeyed** same-kind siblings to lay out distinctly; pushing the
  burden to the consumer is the gap, not the fix.

**Surface impact**: internal to `Control.fs` (the id-assignment + paint walk). The
**public** consequence is D5 (the bounds map), since the same path ids key it.

---

## D5 — Per-`ControlId` computed bounds on `ControlRenderResult` (FR-011, FR-012)

**Decision**: Add a field `Bounds: (ControlId * SceneRect) list` (or a `Map<ControlId, Rect>`)
to `ControlRenderResult<'msg>` (`Types.fsi:285-290`) carrying the **evaluated** absolute
bounds of every rendered control keyed by its `ControlId`, populated from
`result.Bounds` (the `LayoutResult` that `renderTree` already computes at
`Control.fs:1067` and currently **discards**). Keep `Layout = root` for back-compat
(it is the input tree and some callers may read it), but add the computed map as the
hit-test source of truth. Provide a helper `Control.hitTest : ControlRenderResult<'msg> -> x -> y -> ControlId option` (FR-012) that resolves a point to the containing control, layered over the existing `Layout.hitTestComputed` (`Layout.fsi:22`).

**Rationale**: BOUNDS-1 is precise: `renderTree` computes `result = Layout.evaluate ...`
(`:1067`), derives `boundsById` (`:1069-1072`), uses it to paint, then returns
`Layout = root` (the *un-evaluated* input) and throws the computed bounds away
(`:1119-1123`). The fix is to surface what is already computed. Keying by `ControlId`
(not the internal `LayoutNodeId`) is what a Scene host needs — `EventBindings` are
already keyed by `ControlId` (`Types.fsi:279-282`), so a host can join bounds↔binding
by the same key and route a pointer hit to its message. `Layout.hitTestComputed`
already exists; FR-012's helper wraps it for the `ControlId` domain.

**Alternatives considered**:
- *Return the raw `LayoutResult`*: rejected — leaks `LayoutNodeId` (internal path ids
  from D4) to consumers; they want `ControlId`.
- *Replace `Layout = root` with the evaluated result*: rejected — that is a breaking
  change to an existing field's meaning; **additive** field is Tier-1-clean and keeps
  the 080 contract.

**Surface impact**: `Types.fsi` `ControlRenderResult` gains one field; `Control.fsi`
gains the `hitTest` helper; `Control.fs` populates it. Controls baselines recapture.
A `ControlId → Rect` needs `Rect` visible in the Controls public surface (it is —
`Scene.Rect` is already used in `ControlRenderResult.Scene`).

---

## D6 — Governed pointer host as the controls-family default (FR-004/005/006)

**Decision**: Introduce a **product-family marker** in the template (a generated-time
profile/value the scaffold and its `GovernanceTests` branch on: `controls` vs `game`).
The **controls** family's default `Program.fs` launch becomes
`ControlsElmish.runInteractiveApp options host` (the shipped 085 pointer host); the
**game** family keeps `Viewer.runApp viewerOptions generatedHost`. Generalize the
governance assertion (`GovernanceTests.fs:105`, `BehaviorTests.fs:289`) from the literal
`"Viewer.runApp viewerOptions generatedHost"` string to **"the source contains the
persistent interactive host call appropriate to its family"** — i.e. assert
`runInteractiveApp` for the controls family and `Viewer.runApp ... generatedHost` for
the game family.

**Rationale**: HOST-GOV-1 is a *posture* gap: `runInteractiveApp` /
`InteractiveAppHost` with `MapPointer` already ship (`ControlsElmish.fsi:45-53,140-141`,
confirmed this session), but the governance test hard-pins the keyboard-only literal,
so making the pointer host the default *fails governance*. Spec FR-006 + the Edge Case
("must not weaken the game family's guarantee") require this be a **per-family choice**,
not a removal — hence the family marker. The spec Assumption explicitly authorizes
"introducing a minimal family marker" if family isn't first-class yet.

**Open sub-question resolved**: family marker mechanism = a template generation profile
value (the scaffold already uses `//#if (profile == ...)` switches in `Model.fs`/`View.fs`),
so the controls-vs-game split rides the existing profile machinery rather than inventing
a new config axis.

**Alternatives considered**:
- *Drop the keyboard host, make pointer host universal*: rejected — violates FR-006 and
  SC-008 (game family keeps its guarantee).
- *Add an `--interactive` runtime flag*: rejected — SC-003 requires pointer interaction
  **without** adding a flag or a second host.

**Surface impact**: template `Program.fs` (family-branched default launch), the two
governance/behavior test files (generalized assertion), the template family-marker
profile. No `src/**` `.fsi` change for this item (the host already ships).

---

## D7 — Viewer host keyboard warm-up mitigation (FR-015, FR-016)

**Decision**: **Buffer/queue** key events: in `SkiaViewer.fs` (`~1480-1511`), the
`keyDownHandler`/`keyUpHandler` are wired synchronously the moment the window opens,
but the dispatch target (the input pipeline) is not yet ready, so early events are
dropped. Introduce a small bounded pre-ready queue: events captured before the
pipeline signals ready are enqueued and flushed in order once ready; after ready,
dispatch is direct. Document the warm-up window + the buffering mitigation in the
`fs-skia-viewer-host` skill (FR-016).

**Rationale**: KEY-WARMUP-1 places the defect in the host input pipeline, unreachable
by the consumer (who only supplies `MapKey`). Buffering is preferred over an
"input-ready signal" because SC-007 requires *every* early keystroke to be **delivered**
(not merely observable as "input wasn't ready") — a readiness signal alone would still
drop the keystroke. The queue is bounded (drop-oldest with a diagnostic past a cap) to
respect Principle VII (no unbounded growth, explicit degradation).

**Alternatives considered**:
- *Expose an input-ready signal only*: allowed by FR-015's "or", but rejected as the
  primary because it satisfies SC-007 only if the consumer also gates input on it;
  buffering delivers unconditionally. (The readiness signal may still be exposed as a
  diagnostic, but buffering is the mitigation.)
- *Delay window-open until pipeline ready*: rejected — couples window visibility to
  input init and risks regressing the 084 window-visibility evidence.

**Surface impact**: `SkiaViewer.fs` host input path; possibly a small addition to
`SkiaViewer.fsi` if a readiness diagnostic is surfaced; the `fs-skia-viewer-host` skill
doc. This is the live-window FR — its evidence needs a compiled host (per memory
[[feature-085-interactive-host-and-vulkan-window-block]]: fsi can't open a Vulkan
window; a tiny compiled self-closing host is the capture path).

---

## Cross-cutting: evidence path realities (confirmed from memory + source)

- A **live-window** capture (SC-002 real-controls screenshot, SC-003 pointer dispatch,
  SC-007 warm-up) needs a **compiled** self-closing `GeneratedAppHost`/interactive host;
  `fsi` fails Surface/`UnsupportedEnvironment` for a real window. Render-target PNGs
  (the production `controlsExampleView → Control.renderTree` path) **do** work headless.
- `GeneratedProductCheck` **always fails locally** (generated `Verify` can't resolve a
  feature; `Map.empty` env) — a known non-authoritative environment failure
  [[generated-product-check-env-failure]]. Plan records its output but does not treat
  the local failure as a gate block.
- Pointer-dispatch behavior (SC-003) is testable **headlessly** via
  `ControlsElmish.routeInteractivePointer` (`ControlsElmish.fsi:118-131`), which the 085
  surface exposes precisely so a test exercises the real adapter path without a window.
