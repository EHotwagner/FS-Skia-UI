# Phase 0 Research: Runtime Visual-State Bridge (R1)

The spec carries **zero `NEEDS CLARIFICATION` markers** (roadmap §10.3 pins the
precedence model, stamp domain, and identity fast-path; a 2026-06-11 clarify
session resolved the widened set, public/internal split, and consumer-state
channel). This research therefore records the design **decisions** that ground the
spec's requirements in the verified source, not open questions.

---

## D1 — Type name and field reality: `ControlRuntimeModel`, `Selection` is text-range

**Decision**: The bridge consumes `ControlRuntimeModel` (the real type; the spec
prose abbreviates it to "`ControlRuntime`"). The four runtime-derivable states are:

| Derived state | Source field (`ControlRuntime.fs:35`–`44`)        |
|---------------|---------------------------------------------------|
| `Pressed`     | `PressedControls: Set<ControlId>` contains the id |
| `Selected`    | `Selection: ControlSelection option` whose `.ControlId` = the id |
| `Focused`     | `FocusedControl: ControlId option` = the id        |
| `Hover`       | `HoveredControl: ControlId option` = the id        |

**Rationale**: `Selection` is a **text-range** selection (`{ ControlId; Start; End }`),
not a "selected control set". FR-001 lists `Selection` as a derivation source, so a
control that owns the active text selection derives `Selected`. The widened
selection kinds' (`RadioGroup`/`Switch`) *semantic* "selected" is **consumer-set**
(it lowers to an `Attr.visualState Selected`), handled by the consumer-preservation
channel (D4), not by runtime `Selection`. Both paths converge on the same
`VisualState.Selected` resolution.

**Alternatives considered**: Adding a "selected controls" set to `ControlRuntimeModel`
— rejected: out of scope (no runtime-state change; FR-009 additive), and the
consumer channel already carries semantic selection.

---

## D2 — `deriveVisualState` precedence: closed, total, derived-only sub-order

**Decision**: `deriveVisualState : ControlRuntimeModel -> ControlId -> VisualState`
returns the highest-ranked **runtime-derivable** state for the id under the closed
order, else `Normal`:

```
Pressed > Selected > Focused > Hover > Normal
```

This is the lower segment of FR-002's full order
`Disabled > Validation > Loading > Pressed > Selected > Focused > Hover > Normal`.
The upper segment (`Disabled`/`Validation`/`Loading`) is **never produced by
runtime derivation** — those are consumer-set semantic states the runtime does not
track, so they only enter via the consumer-preservation channel (D4). Keeping
`deriveVisualState` defined against the full enum but only ever returning the lower
segment makes it total without inventing runtime interaction sources for semantic
states.

**Rationale**: A single ordered `match`/guard chain is total, deterministic, has no
per-kind branching (FR-002), and reads top-down. Implemented as a plain priority
check (highest first), not a fold or a comparison-key sort — simplest legible form
(Principle III).

**Alternatives considered**: A `VisualState`-ranking comparison function — rejected
as needless machinery; the ordered guard chain is clearer and equally total.

---

## D3 — The bridge: `applyRuntimeVisualState`, internal, structural recursion

**Decision**: `applyRuntimeVisualState : ControlRuntimeModel -> Control<'msg> ->
Control<'msg>` recurses the **structural** `Control.Children` field (`Types.fsi:289`).
For each node:

1. `let id = node.Key |> Option.defaultValue node.Kind` — the established identity
   scheme (`ControlsElmish.fs:581`), `ControlId = ControlKind = string`.
2. `let consumer = ControlInternals.visualStateOf node.Attributes` — reuses the
   internal reader (`Control.fsi:61`); absent ≡ `Normal`.
3. Arbitrate (D4) → either keep the node's attributes unchanged, or set/replace its
   `visualState` attribute, or strip to no-attribute.
4. Recurse `Children`, rebuilding the node.

It is **internal** (omitted from `ControlRuntime.fsi`), reached by tests through the
already-declared `InternalsVisibleTo` for `Controls.Tests` and `Elmish.Tests`
(`Controls.fsproj:19,29`). `ControlRuntime.fs` compiles after `Control.fs`
(`Controls.fsproj:55,65`), so `ControlInternals.visualStateOf` and `Attr.visualState`
are in scope.

**Rationale**: The structural `Children` field is the canonical tree the reconciler
and `renderNode` walk; recursing it keeps the stamped attribute on the exact node
the diff and geometry key by the same `ControlId`. Internal disposition keeps the
new public surface minimal (one projection), per the clarify decision.

**Alternatives considered**: Walking child controls embedded in `Children`-category
attributes — rejected: 095 lowers slot fills **into** `Control.Children`, so the
structural field is already the unified child channel; an attribute walk would
double-visit or miss the canonical nodes.

---

## D4 — Consumer-vs-derived arbitration (FR-003): one channel, `Normal`-slot fill

**Decision**: The **single** authoritative source of consumer intent is the
pre-existing `visualState` attribute on the lowered control (typed semantic Props
already lower to it). Arbitration per node:

- `consumer <> Normal` → **preserve**: leave attributes untouched (do **not**
  re-stamp), so a consumer `Disabled`/`Validation`/`Loading`/`Selected` always wins
  over any derived interaction state, regardless of FR-002 rank.
- `consumer = Normal` → fill the slot with `derived = deriveVisualState model id`:
  - `derived = Normal` → **emit no attribute** (strip nothing because there was
    nothing; keep the node byte-identical) → FR-005 identity-at-rest.
  - `derived <> Normal` → **set** `Attr.visualState derived` (replace any prior
    `Normal`-valued attribute so the last-writer reader sees the derived state).

This cleanly resolves the FR-002 vs FR-003 tension the spec flags: FR-002 orders
states **of the same (derived) origin**; FR-003 makes any consumer non-`Normal`
out-rank **all** derived states because derived only fills a `Normal` slot. A
consumer-`Selected` control the runtime reports `Pressed` resolves `Selected`.

**Rationale**: One channel = no parallel/second consumer-state source (FR-003,
FR-009). Reading the pre-existing attribute means the bridge composes with the typed
Props lowering with no new plumbing.

**Alternatives considered**: A separate consumer-state map keyed by `ControlId` —
rejected by the clarify decision (would be a second channel violating FR-003).

---

## D5 — Host wiring: assembling a `ControlRuntimeModel` at `renderRetained`

**Decision**: The live host does **not** maintain a `ControlRuntimeModel`; it tracks
`pointerState: PointerState` (`Hover: ControlId option`, `Presses` whose candidates
carry `.Control: ControlId`) and `focused: RetainedId option`. In `renderRetained`
(`ControlsElmish.fs:555`), **before** init/step, assemble a read-only
`ControlRuntimeModel`:

- `HoveredControl` = `pointerState.Value.Hover` (already `ControlId`).
- `PressedControls` = `pointerState.Value.Presses |> Map.toSeq |> Seq.map (fun (_, c) -> c.Control) |> Set.ofSeq`.
- `FocusedControl` = `focused.Value` resolved **`RetainedId` → `ControlId`** by
  finding the node in the **prior** retained tree (`retained.Value`) via
  `tryFindNode` and reading `node.Control.Key |> Option.defaultValue node.Control.Kind`.
  On the first frame (`retained.Value = None`) `focused` is `None` → `FocusedControl = None`.
- `Selection`/`Caret`/etc. = the `fst (ControlRuntime.init ())` defaults (the host tracks no
  text-range selection here); leaving them empty is correct — derivation simply
  never yields `Selected` from runtime in that case, and consumer `Selected` still
  flows through D4.

Then `let bridged = applyRuntimeVisualState runtimeModel (host.View size model)` and
feed `bridged` to `RetainedRender.init`/`step`.

**Rationale**: This keeps the bridge in the **`ControlId` domain** (FR-004): the
runtime model and the freshly-lowered tree both key by `ControlId`, so the stamp
lands on the node the runtime named. The focus `RetainedId→ControlId` resolution
uses the prior tree because the new tree isn't reconciled yet — and the `ControlId`
(Key/Kind) is stable across the frame by construction (that is exactly the identity
E2 preserves). The stamp-before-`step` ordering is what turns a hover change into a
scoped `Update` patch (FR-004, SC-005).

**Alternatives considered**: (a) Maintaining a full `ControlRuntimeModel` ref in the
host updated by the pointer/focus reducers — heavier than needed and risks a second
source of truth; the per-frame assembly from the existing refs is exact and pure.
(b) Stamping in the `RetainedId` domain post-reconcile — rejected by FR-004 (domain
mismatch; the runtime keys `ControlId`).

---

## D6 — Widening the migrated geometry (FR-006)

**Decision**: Thread `classes`/`state` through and call `Style.resolve` in four
private geom functions, mirroring the existing `buttonGeom`/`checkboxGeom`:

- `sliderGeom` (`Control.fs:568`), `textFieldGeom` (`:878`), `radioGeom` (`:539`),
  `switchGeom` (`:588`).

Each adds `(classes: StyleClass list) (state: VisualState)` params, replaces its
hard-coded base colours with `Style.resolve theme baseStyle classes state`, and the
two `renderNode` dispatch sites (`Control.fs:~32` and `:961`) pass `classes`/`state`
(already computed as `styleClassesOf`/`visualStateOf` at the dispatch — `:945`).
`icon-button` already shares `buttonGeom`; no change. At `state = Normal` and
`classes = []`, `Style.resolve` is identity over `baseStyle`, so output is
**byte-identical** to today (verified by the byte-identity readiness artifact).

**Rationale**: Reuses E3's single resolver and the existing `VisualState`-threaded
private path — no new styling mechanism, no new public type, no new token literal
(FR-006, FR-008). `Style.fs` is unchanged (it already maps every `VisualState` case).

**Out of scope**: `toggle-button`/`list-box`/`multi-select-list`/`combo-box` and a
catalog-wide 52-control migration stay on `Normal`-only geometry (tracked with
E3/R5). Their item-level/virtualized selection geometry is a poor fit for this
focusable-surface widening.

---

## D7 — Evidence honesty (SceneEvidence is a capability hash)

**Decision**: Parity and byte-identity proofs are authoritative as **structural
`Scene` / resolved-style equality**, not pixel diffs. The responds-proof is a
separate live-path artifact (input → visible restyle on the real retained host).

**Rationale**: `SceneEvidence.renderPng`/`renderReadbackEvidence` are deterministic
**capability-hash** functions, not pixel encoders (established in features 091/093
and project memory). A real Vulkan window does open in this environment via the
compiled-exe X11 path, so the responds-proof is capturable; the pure derivation and
identity-at-rest claims are proven by `Scene`-equality which is exact and fast.

---

## Summary of decisions

| # | Decision | Primary requirement |
|---|----------|---------------------|
| D1 | Consume real `ControlRuntimeModel`; `Selection` = text-range → derived `Selected` | FR-001 |
| D2 | Closed ordered guard `Pressed>Selected>Focused>Hover>Normal`; total over full enum | FR-001, FR-002, SC-004 |
| D3 | Internal `applyRuntimeVisualState`, structural `Children` recursion, reuse `visualStateOf` | FR-004, FR-009 |
| D4 | One consumer channel (pre-existing attr); non-`Normal` preserved, `Normal` slot filled, no attr at `Normal` | FR-003, FR-005 |
| D5 | Assemble read-only `ControlRuntimeModel` from `pointerState`+`focused` (RetainedId→ControlId via prior tree); stamp pre-`step` in ControlId domain | FR-004, FR-007, SC-005 |
| D6 | Widen `slider`/`text-box`/`radio-group`/`switch` geometry through `Style.resolve` | FR-006, SC-006 |
| D7 | Structural `Scene`/resolved-style equality + live responds-proof; capability-hash honesty | FR-008, SC-002, SC-007 |
