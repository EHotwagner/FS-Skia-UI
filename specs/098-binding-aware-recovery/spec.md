# Feature Specification: Binding-Aware Ancestor Recovery

**Feature Branch**: `098-binding-aware-recovery`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: "create the next part" of `docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md` — the controls architecture-evolution roadmap. Steps E1–E5 (features 090, 091+092, 093, 094, 095) have landed; the roadmap's §10 post-implementation audit then defines five live-path remediation features **R1–R5** with the recommended order **R1 → {R3, R2} → R4 → R5**. R1 (the runtime visual-state bridge) shipped as feature 096 and R2 (incremental measure / partial re-layout) shipped as feature 097. This feature is **R3 — binding-aware ancestor recovery** (roadmap §10.5): it makes an **unkeyed** authored control respond in the live host, closing the last instance of the original dead-window class — authoring `Button.onClick` the documented, obvious way (without `withKey`) must dispatch.

## Context & Motivation *(informative)*

E1 (feature 090) wired authored `EventBindings` into the interactive host: a pointer Click recovers the nearest authored control id via `Control.nearestAuthored` and dispatches its binding, with `MapPointer` as the fallback. But the audit (roadmap §10.1/§10.5) found authored dispatch works **only for keyed controls**. An *unkeyed* `Button.onClick` — the documented, obvious authoring — is still a dead button.

The current code confirms the root cause exactly:

- **Recovery only sees keys.** `Control.nearestAuthored` (`src/Controls/Control.fs:1459`) walks `result.Layout` and treats a node as authored only when `node.Id <> path` (`:1461`) — which, because `toLayout` sets `Id = Key |> defaultValue path`, is true **only when the node carries an explicit `Key`**. So it can only ever return a `Key` or `None`. An unkeyed-but-bound node is invisible to it: its hit resolves to `None`, `bindingMessagesFor` returns `None` (`src/Controls.Elmish/ControlsElmish.fs:155`–`178`), and the interaction falls through to `MapPointer` — unmapped → nothing happens.
- **An id-scheme divergence underneath.** `EventBindings` key each binding by `Key ?? Kind` (`eventBindings`, `src/Controls/Control.fs:194`), and `collectBoundsWith` likewise emits the public `Bounds` list keyed by `Key ?? Kind` (`controlId`, `:1332`) even though it *looks up* the bounds by the layout path `Key ?? path` (`layoutId`, `:1331`). Recovery and the hit interaction, however, live in the layout-path domain (`Key ?? path`): the Click id comes from `Layout.evaluate ... rendered.Layout` whose node ids are `Key ?? path` (`ControlsElmish.fs:228`, `:217`–`232`), and `nearestAuthored` re-derives the same positional path. The two schemes **only agree when a `Key` is present**. The `Kind` half of the binding/bounds scheme is therefore (a) never reachable through recovery (which returns only keys), and (b) a same-kind-sibling **collision** source: two unkeyed `button`s both mint `ControlId = "button"` in `Bounds` and `EventBindings`, conflating distinct controls onto one id.

The consequence is that the roadmap's E1 promise — "a control authored the documented way actually responds" — holds for keyed authoring but breaks for the common unkeyed case, and the binding/bounds id scheme can collide for same-kind siblings. R3 closes both: it **unifies the per-node `ControlId` derivation** so bounds, `EventBindings`, and recovery share one scheme (`Key ?? structural-path`), and makes recovery **binding-aware** — an ancestor is authored when it carries a `Key` **or** is bound — so an unkeyed bound node recovers its own path id and dispatches. It is architecture-preserving and non-goal-preserving: it introduces no routed/bubbling event system, no command system, no new public event type — it corrects an id-derivation divergence and widens an existing recovery predicate so existing data already produced by `renderTree` routes correctly.

## Clarifications

### Session 2026-06-11

- Q: Unifying the per-node id scheme to `Key ?? path` changes the `ControlId` reported in the public
  `Bounds` list and in `ControlEvent.ControlId` for **unkeyed** controls (today `Kind`, after R3 the
  structural path like `"0.1"`). Is that observable-payload change acceptable, or must the reported
  payload id stay `Kind`-shaped? → A: **Adopt the unified path scheme as the single canonical id; the
  reported payload changes for unkeyed controls and that is acceptable and documented.** Keyed
  authoring is unchanged (the `Key` is still the id). The old `Kind` scheme was already broken for
  same-kind siblings (collision), so canonicalizing to the path is a net correctness gain, not a
  regression. The change is additive for keyed consumers and only *adds* dispatch for the
  previously-dead unkeyed case.
- Q: How is `boundIds` surfaced to recovery — a new field on `ControlRenderResult`, or an extra
  argument threaded into `nearestAuthored`? → A: **A new `BoundIds : Set<ControlId>` field on
  `ControlRenderResult`**, populated by `renderTree`/`render` in the unified path scheme, so the
  recovery walk reads it from the render result it already takes (no new threading at every call
  site). This is a public-surface addition that escalates the change to the Tier-1 surface-baseline
  route, as expected for a `Control.fsi` change.
- Q: After unification, should the **focus** path (092's `resolveFocus`/`retainedHitTest`, returning a
  `RetainedId`) also change? → A: **No.** R3 corrects the **dispatch** (binding) path only — the 090
  `Layout.evaluate` + `nearestAuthored` + `EventBindings` lookup seam. The 092 retained focus path
  (`RetainedId` domain) is a separate, already-working concern and is out of scope; R3 must not
  regress it.
- Q: How does the single-control `render` preview path (which emits `Bounds = []` but a populated
  `EventBindings`) participate in the unified id scheme and the new `BoundIds` field? → A: **Unify
  everywhere and populate `render.BoundIds`.** `render` adopts the same `Key ?? path` canonical scheme
  as `renderTree` (so SC-003's single-scheme invariant holds across *all* surfaces, not just the live
  dispatch path), and `render.BoundIds` is **populated** from its bound nodes — mirroring its already
  populated `EventBindings`, not its (deliberately empty) `Bounds`. `render.Bounds` stays `[]`
  (unchanged); only the id *scheme* of its `EventBindings` shifts for unkeyed controls (`Kind → path`).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - An unkeyed authored button responds in the live host (Priority: P1)

A consumer writes the documented, obvious thing — `Button.create [ Button.onClick Clicked ]` with **no** `withKey` — and runs the app. Today the button renders but clicking it does nothing (recovery returns `None`, the interaction falls through to an unmapped `MapPointer`). With R3 the click dispatches the authored `Clicked` message.

**Why this priority**: This is the headline R3 capability and the direct fix for the audit finding that E1's dispatch promise covers only keyed controls. The unkeyed case is the *default* authoring path, so this is the last instance of the original "renders-but-dead-window" class for authored controls.

**Independent Test**: In the live-adapter routing seam (`routeInteractivePointer`), render a view containing a single **unkeyed** `Button.onClick` over its bounds, deliver a press+release Click at the button, and assert the authored message is dispatched (and `MapPointer` is *not* consulted for it).

**Acceptance Scenarios**:

1. **Given** an unkeyed control authored with `onClick`, **When** a Click lands on it, **Then** the authored binding dispatches and `MapPointer` is not consulted for that interaction.
2. **Given** a nested unkeyed bound control inside an unbound, unkeyed container, **When** a Click lands on the inner control, **Then** recovery resolves to the inner bound node and its binding dispatches.
3. **Given** an unkeyed, **unbound** control (no `onClick`/`onChanged`/`onSelected`), **When** a Click lands on it and no bound ancestor exists on its path, **Then** recovery returns `None` and the interaction falls back to `MapPointer` exactly as before (no spurious dispatch).

---

### User Story 2 - Keyed and container-keyed dispatch remain non-regressive (Priority: P1)

A consumer who already uses `withKey` (a keyed leaf, or a container-keyed composite whose inner positional hit must recover the container id) must see exactly the same behavior after R3 as before. The fix adds dispatch for the unkeyed case without altering the keyed paths.

**Why this priority**: R3 changes the canonical id scheme and the recovery predicate, both on the live dispatch path that 090/091/092 already exercise. Equal priority with US1 because the unkeyed fix is only adoptable if it provably does not disturb the keyed behavior the E-series shipped.

**Independent Test**: Re-run the 090 representative dispatch cases (a keyed leaf dispatches; a container-keyed composite recovers the container id from an inner hit) through the R3 code and assert identical dispatched messages and identical recovery ids.

**Acceptance Scenarios**:

1. **Given** a directly-keyed leaf with a binding, **When** a Click lands on it, **Then** recovery resolves to its `Key` (a fixed point) and its binding dispatches — unchanged from 090.
2. **Given** a container-keyed composite, **When** a Click lands on an inner *unkeyed, unbound* positional node, **Then** recovery climbs to the keyed container and dispatches the container's binding — unchanged from 090.
3. **Given** a control with both a `Key` **and** a binding, **When** a Click lands on it, **Then** the binding is found by the unified id (the `Key`), with no double-dispatch.

---

### User Story 3 - Same-kind unkeyed siblings disambiguate by path (Priority: P2)

A consumer lays out two unkeyed `button`s (or any two same-kind controls) side by side, each with its own `onClick`. Today both mint `ControlId = "button"` in `Bounds` and `EventBindings`, colliding onto one id; a hit on either could route to the wrong binding. With R3's unified path scheme each gets a distinct structural id (`"0.0"`, `"0.1"`), so a click on the second button dispatches the second button's message.

**Why this priority**: It is the correctness payoff of unifying the id scheme (the `Kind`-keyed collision is removed), reusing the 087/092 same-kind-sibling learnings. Lower urgency than US1/US2 because it depends on the same unification, but it is what makes the canonicalization a net gain rather than a lateral move.

**Independent Test**: Render two unkeyed same-kind bound siblings with distinct messages; deliver a Click to each; assert each dispatches its own message (no cross-routing), and that their `Bounds`/`EventBindings` ids are distinct.

**Acceptance Scenarios**:

1. **Given** two unkeyed same-kind bound siblings, **When** the `Bounds` and `EventBindings` are computed, **Then** their `ControlId`s are distinct (the structural paths), not a single shared `Kind` id.
2. **Given** those two siblings, **When** a Click lands on the second, **Then** the second sibling's binding dispatches and the first's does not.

---

### Edge Cases

- **Unbound, unkeyed leaf, no bound/keyed ancestor**: recovery returns `None`; the host falls back to `MapPointer` with the raw interaction (no invented id, no spurious dispatch) — preserving the 090 fallback contract.
- **Hit on an unkeyed bound node nested under a keyed container**: the *nearest* authored ancestor (incl. self) wins — the inner bound node, not the outer keyed container — matching the existing "nearest including self" recovery semantics.
- **Hit on an unkeyed unbound node nested under an unkeyed *bound* container**: recovery climbs to the bound container (it is authored by virtue of being bound) and dispatches its binding.
- **Keyed leaf is a fixed point**: a directly-keyed node's hit id IS its `Key`, so recovery returns itself — unchanged (the 090 non-regression invariant).
- **A node with a `Key` but no binding**: still recoverable as authored (it carries a `Key`); recovery may return it even though it has no binding — the binding lookup then finds nothing and the host falls back to `MapPointer`, exactly as for the keyed-but-unbound case today (`bindingMessagesFor` returns `None` on an empty match).
- **Click-equivalent kinds only**: the binding-eligible kinds stay the existing closed set (`click`/`changed`/`selected`, `ControlsElmish.fs:150`); R3 widens *which nodes are recoverable*, not *which event kinds dispatch on a Click*.
- **Disabled / read-only control**: a disabled bound node does not dispatch (the existing `disabledOrReadOnly` guard in `dispatch`/binding filtering is preserved); R3 does not bypass it.
- **`ControlEvent.ControlId` payload for unkeyed controls**: after unification the reported id is the structural path (e.g. `"0.1"`) rather than the `Kind` string. This is the documented, accepted payload change (Clarifications); keyed controls report their `Key` unchanged.
- **Bounds list consumers**: any consumer reading the public `Bounds` list and matching on the `Kind` string for an unkeyed control must now match the structural path. This is part of the documented canonicalization; keyed entries are unchanged.
- **Single-control `render` preview**: `render` keeps `Bounds = []` but adopts the unified `Key ?? path` scheme for its `EventBindings` and emits a **populated** `BoundIds` (the bound nodes), so the canonical-scheme invariant holds on the preview surface too even though it exposes no hit-testable geometry. The dispatch/recovery path itself uses `renderTree`, not `render`.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The framework MUST derive **one canonical `ControlId` per node** so that the public `Bounds` list, `EventBindings`, and `nearestAuthored` recovery all share a **single scheme: `Key ?? structural-path`** (the positional `parent + "." + index` path `toLayout`/`collectBoundsWith` already mint). The `Key ?? Kind` scheme used today by `eventBindings` (`src/Controls/Control.fs:194`) and by `collectBoundsWith`'s emitted `controlId` (`:1332`) MUST be replaced by the path scheme. Keyed nodes are unaffected (the `Key` remains the id); only the unkeyed fallback changes from `Kind` to the structural path.
- **FR-002**: `renderTree` **and** `render` MUST emit a **`BoundIds : Set<ControlId>`** — the set of canonical ids (FR-001 scheme) of nodes that carry at least one event binding (`eventBindings control` non-empty). The set MUST be in the same path scheme as `EventBindings`, so a recovered id can be looked up directly. Both paths adopt the unified scheme (so the single-canonical-scheme invariant of SC-003 holds across *all* surfaces, not only the live dispatch path); `render.BoundIds` is **populated** from its bound nodes, mirroring `render`'s already-populated `EventBindings` rather than its deliberately empty `Bounds`. `render.Bounds` stays `[]` (unchanged); only the id *scheme* of its `EventBindings` shifts for unkeyed controls (`Kind → path`).
- **FR-003**: `nearestAuthored` MUST become **binding-aware**: walking the hit node's path, it treats an ancestor (including self) as authored when that ancestor **carries a `Key`** OR its canonical id is in `BoundIds`. It MUST return the nearest such ancestor's canonical id — returning `Some pathId` for an unkeyed-but-bound node (today it returns `None`). When no node on the path is keyed or bound, it MUST still return `None` (host falls back to `MapPointer`). A directly-keyed leaf MUST remain a fixed point (FR-005 non-regression).
- **FR-004**: Dispatch (`bindingMessagesFor`, `src/Controls.Elmish/ControlsElmish.fs:155`) MUST look up the recovered id in `EventBindings` using the **same unified scheme**, so a recovered unkeyed-bound id matches its binding. The existing precedence is preserved: an authored binding **wins**; `MapPointer` is consulted **only** when recovery is `None` or no click-equivalent binding matches — never both (no double-dispatch).
- **FR-005**: All E1 (feature 090) dispatch behavior MUST remain **non-regressive**: a keyed leaf dispatches; a container-keyed composite recovers the container id from an inner unkeyed-unbound hit; the binding-wins precedence and the `MapPointer` fallback are unchanged; `MapPointer`-only consumers (no authored bindings) are bit-for-bit unchanged (additive).
- **FR-006**: The unified path scheme MUST **disambiguate unkeyed same-kind siblings**: two same-kind unkeyed nodes MUST mint distinct canonical ids (their structural paths), so a hit on one routes only to that one's binding — removing the `Kind`-keyed collision. This MUST be property-tested for determinism and same-kind-sibling distinctness.
- **FR-007**: The reported `ControlEvent.ControlId` payload and the public `Bounds` `ControlId` for **unkeyed** controls change from the `Kind` string to the structural path (the documented canonicalization). Keyed controls' reported id MUST stay their `Key`. The change MUST be documented (the canonicalization is a net correctness gain that removes a collision and adds previously-dead dispatch). Where a downstream consumer observes the payload id, the canonicalization MUST be the single, consistent scheme (no node reports `Kind` from one surface and path from another).
- **FR-008**: The change MUST be **scoped to the dispatch path**. The 092 retained focus path (`resolveFocus`/`RetainedRender.retainedHitTest`, returning a `RetainedId`) MUST NOT be altered or regressed. Focus stability and the retained identity domain are a separate concern out of scope for R3.
- **FR-009**: The feature MUST be **additive and non-goal-preserving**: it introduces no routed/bubbling/tunneling event system, no command system, no new public event type, no framework-level focus traversal change, and no change to the `view : 'model -> Control<'msg>` consumer contract. Events stay **flat per-`ControlId` bindings**; R3 only corrects the id under which a node's existing binding is found. No data-binding, observable, dependency/attached-property, lookless-template, or CSS-selector capability is introduced (permanent roadmap non-goals).

> Interacting / conflicting requirements: FR-005 (keyed dispatch non-regressive) vs FR-001 (unify the id scheme) — resolution: unification changes only the **unkeyed fallback** (`Kind → path`); the keyed branch (`Key` is the id) is byte-identical, so keyed dispatch and recovery are fixed points and cannot regress. FR-007 (payload id may change for unkeyed controls) vs FR-005 (non-regression) — resolution: "non-regression" is scoped to **dispatch behavior and keyed authoring**, not to the literal `Kind`-string payload of an unkeyed control, whose old value was already collision-prone; the payload canonicalization is the accepted, documented cost of correctness and is the *same* scheme everywhere (FR-007).

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** Unlike the rest of the
> spec, this section is *expected* to name concrete packages, `.fsi` signatures, build targets,
> effects, and evidence paths — that is its purpose.

- **Package impact**: No package identity or set changes. The id unification (`eventBindings`, `collectBoundsWith`), the `BoundIds` emission from `renderTree`/`render`, and the binding-aware `nearestAuthored` land in the existing **`FS.Skia.UI.Controls`** package (`src/Controls/Control.fs`, `src/Controls/Control.fsi`, `src/Controls/Types.fsi` for the `ControlRenderResult.BoundIds` field). The dispatch lookup correction lands in the existing **`FS.Skia.UI.Controls.Elmish`** package (`src/Controls.Elmish/ControlsElmish.fs` `bindingMessagesFor`). No DTCG token, no new control type. All packable libraries are version-bumped and template pins refreshed on merge per the standard flow.
- **Public contract impact**: This feature **changes public surface**. (1) `ControlRenderResult<'msg>` gains a `BoundIds : Set<ControlId>` field (`src/Controls/Types.fsi:345`), and (2) the canonical `ControlId` value for **unkeyed** controls changes from `Kind` to the structural path in the public `Bounds` list and (3) in the reported `ControlEvent.ControlId` payload. `nearestAuthored`'s signature is unchanged (still `result -> hit -> ControlId option`); its *behavior* widens (also recovers bound nodes). These move the `FS.Skia.UI.Controls` surface baseline and escalate to the Tier-1 controls-public-surface route; baselines (api-surface + per-package `.fsi.txt`) are recaptured.
- **State workflow impact**: None to the consumer state model. `nearestAuthored` stays pure/total/deterministic over the already-computed render result; `bindingMessagesFor` is unchanged except for the unified-id lookup. No new effect, command, subscription, or interpreter behavior. `ControlRuntime`, `Pointer`, and the focus seam are untouched.
- **Layout/rendering impact**: **None to rendered geometry or pixels.** R3 changes id derivation and recovery only; the `Scene`, `Layout`, and computed `Bounds` *rectangles* are byte-identical — only the `ControlId` *labels* on unkeyed bounds change (FR-007). No new Skia/Vulkan surface. Evidence is the live-adapter routing seam (`routeInteractivePointer`) showing an unkeyed binding dispatch, plus structural/property tests; the existing responds-vs-renders proof primitive (E1) applies — an inert/un-fixed build fails the unkeyed-dispatch artifact.
- **Evidence obligations**: Real, in-repo readiness artifacts under `specs/098-binding-aware-recovery/readiness/` proving: (a) an unkeyed authored `Button.onClick` dispatches in the live-adapter routing seam, and a nested unkeyed bound control dispatches via recovery (US1) — an artifact an un-fixed build cannot produce; (b) keyed-leaf and container-keyed recovery remain non-regressive vs the 090 cases (US2/FR-005); (c) a single canonical `ControlId` scheme spans `Bounds`, `EventBindings`, and recovery, with unkeyed same-kind siblings disambiguated by path, property-tested for determinism and distinctness (US3/FR-006); (d) `MapPointer`-only consumers and the recovery-`None` fallback are unchanged (FR-005); (e) the 092 retained focus path is not regressed (FR-008).
- **Unsupported scope**: Out of scope — the runtime visual-state bridge (R1, shipped as 096), incremental measure / partial re-layout (R2, shipped as 097), the live animation clock and animated transitions (R4), general navigation-key delivery (R5); any routed/bubbling/tunneling event system, command system, or new public event type; any change to the 092 retained focus path; any catalog-wide retrofit of all 52 typed views' binding surfaces (a separate fitness pass, per the E1 scope bound). CSS selectors, attached/dependency properties, lookless templates, data binding remain permanent non-goals.
- **Build-target impact**: Run `Route` first and run only the gates it prints. The change touches public `src/Controls/**/*.fsi` (the `BoundIds` field and the canonical-id behavior), so it **escalates** to the serialized maintainer-verify path: `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`. Surface baselines (api-surface + per-package `.fsi.txt`) are recaptured for `FS.Skia.UI.Controls`. No new gate is added; the recovery/disambiguation property tests are added to the existing Controls/Elmish test projects.

## Success Criteria *(mandatory)*

- **SC-001**: An unkeyed authored `Button.onClick` (and a nested unkeyed bound control) **dispatches in the live-adapter routing seam** — an artifact an un-fixed build cannot produce.
- **SC-002**: Keyed-leaf dispatch and container-keyed recovery are **byte-identical** to the 090 behavior (same dispatched messages, same recovered ids); `MapPointer`-only consumers are bit-for-bit unchanged.
- **SC-003**: A **single canonical `ControlId` scheme** (`Key ?? structural-path`) spans the public `Bounds` list, `EventBindings`, and `nearestAuthored` recovery — verified that all three agree for a given node.
- **SC-004**: Two unkeyed same-kind bound siblings mint **distinct** canonical ids and route only to their own bindings (no cross-routing, no collision) — property-tested for determinism and same-kind-sibling distinctness across ≥1000 generated cases.
- **SC-005**: An unkeyed control with **no** bound or keyed ancestor recovers `None` and falls back to `MapPointer` (no spurious dispatch); a keyed leaf remains a recovery fixed point.
- **SC-006**: `ControlRenderResult<'msg>` exposes `BoundIds : Set<ControlId>` in the unified scheme; recovery reads it from the render result; the `FS.Skia.UI.Controls` surface baseline is recaptured to reflect `BoundIds` and the canonical-id change.
- **SC-007**: The 092 retained focus path (`resolveFocus`/`retainedHitTest`/`RetainedId`) is **not regressed** — focus resolution behavior is unchanged.
- **SC-008**: The escalated six-target order (`Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit`) is green with `EvidenceAudit` reporting no synthetic/stub work.

## Assumptions

- E1 (feature 090) has landed: the interactive host recovers an authored id via `Control.nearestAuthored` and dispatches `EventBindings` with `MapPointer` as the fallback, binding-wins precedence — R3 corrects the recovery/id scheme on this seam and re-implements none of it.
- E2 (features 091 + 092) has landed: the retained tree and the 092 `RetainedId` focus path exist and work; R3 touches neither (FR-008).
- A node is "bound" exactly when `ControlInternals.eventBindings control` is non-empty (it has an `Event`-category attribute lowering to a `MessageValue`/`EventValue` binding); `BoundIds` is the set of canonical ids of such nodes.
- The structural path `toLayout`/`collectBoundsWith` already mint (`"0"`, `"0.1"`, …) is the right canonical fallback id: it is what the Click interaction already carries (the hit id from `Layout.evaluate ... rendered.Layout`) and what `nearestAuthored` already re-derives, so unifying onto it makes the dispatch path internally consistent.
- The accepted cost of unification is a changed `ControlId` payload/`Bounds` label for unkeyed controls (`Kind → path`); keyed authoring is unchanged. The old `Kind` scheme was already collision-prone for same-kind siblings, so the canonicalization is a net correctness gain (Clarifications).
- Per the architecture-evolution decision, this is MVU-core evolution finishing the E1 live dispatch path (the R-series completing the E-series), not a redesign; no routed event system, command system, or property graph is introduced.
- R3 is independent of R1/R2/R4/R5 (roadmap §10.8): it shares only the E1 dispatch seam; it neither depends on nor blocks the visual-state bridge (R1/096), incremental layout (R2/097), the animation clock (R4), or general navigation (R5).

## Key Entities

- **Canonical `ControlId`**: the single per-node id scheme `Key ?? structural-path`, unified across `Bounds`, `EventBindings`, and recovery — replacing today's divergent `Key ?? Kind` (bindings/bounds) vs `Key ?? path` (recovery/hit).
- **`BoundIds` set**: `ControlRenderResult.BoundIds : Set<ControlId>` — the canonical ids of nodes carrying at least one event binding, emitted by `renderTree`/`render`, read by binding-aware recovery.
- **Binding-aware recovery**: `nearestAuthored` widened to treat an ancestor as authored when it is **keyed or in `BoundIds`**, returning the nearest such ancestor's canonical id (now including unkeyed-bound nodes), `None` only when nothing on the path is keyed or bound.
- **Dispatch seam**: `bindingMessagesFor`/`routeInteractivePointer` — the live-adapter routing that looks up the recovered id in `EventBindings` (now in the unified scheme) with binding-wins precedence over `MapPointer`.
- **Same-kind-sibling disambiguation**: the correctness payoff of unification — distinct structural paths for two unkeyed same-kind nodes, removing the `Kind`-keyed collision.
