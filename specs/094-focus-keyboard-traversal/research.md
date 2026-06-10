# Phase 0 Research: Focus, Keyboard Traversal & Input Routing (E4)

All NEEDS CLARIFICATION from Technical Context are resolved below. The spec carried no
`[NEEDS CLARIFICATION]` markers; the open design choices it deferred to planning are decided here
as informed defaults consistent with the landed E1 (090) / E2 (091/092) / E3 (093) patterns.

## R1 — Traversal keys are engine-level, NOT per-control `NavigationKeys` (the central decision)

**Decision**: Tab / Shift+Tab are **global traversal keys owned by the focus engine**, derived
from the computed tab order — they are **not** entries in any control's
`KeyboardOperation.NavigationKeys`. `NavigationKeys` is reserved for **intra-control** movement
(slider / radio / menu arrows). `Accessibility.defaultFor` is corrected to stop seeding every
focusable control's `NavigationKeys` with `["Tab"; "Shift+Tab"]`, and `Accessibility.validate`'s
rule "a focusable control must carry non-empty `NavigationKeys`" is relaxed so an activation-only
control (a `Button`: `ActivationKeys = ["Enter"; "Space"]`, `NavigationKeys = []`) is **valid**.

**Rationale**: The current `defaultFor` (`Accessibility.fs:78`) seeds
`keyboard focusable ["Enter"; "Space"] ["Tab"; "Shift+Tab"]` for every focusable role, and
`validate` (`Accessibility.fs:85`) errors when a focusable control has empty `NavigationKeys`.
Under the spec's FR-007 / the "focused control claims the traversal key" edge case — *a control's
own `NavigationKeys` consumption wins per-key, and only an unconsumed Tab drives traversal* — if
Tab lives in **every** control's `NavigationKeys`, every control consumes Tab and **global
traversal can never fire**. The two cannot both be true. Resolving traversal as engine-level (the
default; FR-002) and reserving `NavigationKeys` for genuine intra-control arrows (FR-007's slider /
radio case) is the only reading consistent with both FR-002 and FR-007. A control that
*legitimately* consumes Tab (a multi-line text area inserting a tab character) opts in by listing
`"Tab"` in its `ActivationKeys` (or via the E1 text-consumption path) — that is the explicit
"control wins the key" branch, not the default.

**Scope/safety**: `defaultFor` is consumed broadly (default metadata for every control kind), but
since **no traversal existed before** and nothing read `NavigationKeys` for routing, removing Tab
from the defaults changes no current behavior — it only unblocks E4. The corrected representative
metadata: `Button` → `Activation=["Enter";"Space"]`, `Navigation=[]`; `Slider` →
`Navigation=["ArrowLeft";"ArrowRight"]`; `RadioGroup` → `Navigation=["ArrowUp";"ArrowDown"]`;
`TextBox` → E1 text-consumption path; `StaticText` → `Focusable=false`. This is a behavioral fix to
`Accessibility.fs` only — `Accessibility.fsi` signatures are unchanged.

**Alternatives considered**: (a) Keep Tab in `NavigationKeys` and special-case Tab in `route` to
always mean traversal — rejected: it makes `NavigationKeys` lie about its own semantics and breaks
the multi-line-text-area edge case (which needs Tab to be consumable). (b) Add a separate
`TraversalKeys` field to `KeyboardOperation` — rejected: it grows the public accessibility
primitive the spec says E4 must **not** add to (FR-008, "adds no new accessibility primitives"),
and traversal keys are universal, not per-control.

## R2 — Tab-order derivation: `FocusOrder`-then-layout with a stable tiebreak

**Decision**: `Focus.order` walks the lowered `Control<'msg>` tree in document/pre-order, keeps
only controls whose `Accessibility.Keyboard.Focusable = true`, then orders them by
`(FocusOrder ascending with None last, then document-order index)`. The document-order index is the
stable deterministic tiebreak (FR-001). Layout order is the document/pre-order walk of the tree
(the same order `Control.renderTree` lowers children), which already reflects the computed layout
sequence; no separate layout query is required for ordering.

**Rationale**: FR-001 mandates "focusable controls ordered by `FocusOrder` ascending, with
`FocusOrder = None` controls following in layout/document order, and a stable deterministic
tiebreak." Pre-order document index is deterministic, total, and matches the child order the
reconciler and renderer already use, so the tab order, the render order, and the diff order all
agree. Sorting by `(focusOrder |> Option.defaultValue Int32.MaxValue, docIndex)` is a stable sort
key giving exactly the spec's ordering: explicit `FocusOrder` ascending, `None` after all explicit
values, ties broken by document order.

**Alternatives considered**: Geometric (top-to-bottom, left-to-right by `Bounds`) ordering for the
`None` group — rejected as the *primary* key: it diverges from document order under absolute
positioning and is non-obvious to a consumer reading the view source. Document order is the
predictable default; a consumer who wants a different order sets `FocusOrder` explicitly.

## R3 — Key routing precedence at the host (generalizing E1)

**Decision**: Per delivered key, the host (`routeFocusedKey`) resolves in this fixed order, mirroring
E1's binding-wins / host-fallback shape (FR-007):
1. **Text seam (E1, unchanged)** — if the focused control is a text control and the key is a
   text-relevant key, deliver via the existing `routeFocusedText` (`TextInput` pipeline). Unchanged.
2. **`Focus.route` against the focused control's `KeyboardOperation`** —
   `Activate` (key ∈ `ActivationKeys`) → the control's authored **activation** binding (the same
   message a pointer activation dispatches, fired **once**); `Navigate` (key ∈ `NavigationKeys`) →
   the control's authored **value-change / selection** binding.
3. **Traversal** — if the key is an unconsumed Tab / Shift+Tab (`KeyRouting.Traverse move`), apply
   `Focus.traverse order current move` and emit `ControlRuntimeMsg.FocusControl next`.
4. **Fall-through** — otherwise (`KeyRouting.Fallthrough`) the key is a no-op for the control and
   falls through to `host.MapKey` (the existing host fallback).

**Rationale**: This is the direct generalization of E1's "authored binding wins, host fallback
second" precedence to keys. Checking the control's own consumption (steps 1–2) before traversal
(step 3) is exactly FR-007's "the focused control's own key consumption wins per-key; only an
unconsumed Tab drives traversal," which makes a multi-line text area's Tab and a radio group's
arrows un-stealable by traversal. `Focus.route` encodes steps 2–3 purely (membership tests +
the Tab test); the host owns step 1 (text, already shipped) and step 4 (fallback).

**Alternatives considered**: Traversal-first (Tab always traverses) — rejected: steals Tab from a
control that legitimately consumes it (the spec edge case). Single merged classifier including text
— rejected: it would duplicate / risk regressing the shipped, separately-tested E1 text seam; E4
explicitly preserves that path unchanged (SC-003).

## R4 — Focus identity & stability over the retained tree (consume E2, do not re-derive)

**Decision**: The pure `Focus` reducers operate on `ControlId` (the authored/lowered id), keeping
them free of the internal `RetainedId`. The **host seam** binds the focused `ControlId` to its
stable `RetainedId` via the E2 retained structure (`RetainedRender.retainedHitTest` for pointer
focus; `resolveFocus` already exists for click), so `FocusedControl` survives an unrelated
re-render exactly as the 092 text/clock state does. A focused control removed between frames reuses
E2's stale-target recovery (`RecoverStaleTarget` → resolve to the next stop at the removed control's
former position in tab order, or `None` if the order is empty — clarified 2026-06-10).

**Rationale**: FR-004 mandates E4 *consume* E2 identity and *not* re-derive or alter the
067/091/092 reconciler scheme. Keeping `RetainedId` out of the pure `Focus` surface (a) avoids
leaking the internal type into the new public `Focus.fsi`, and (b) keeps `order`/`traverse`/`route`
testable as plain pure functions. The host already holds the retained structure and the
`ControlId`↔`RetainedId` binding (092 `resolveFocus`/`routeFocusedText`); `routeFocusedKey` reuses it.

**Alternatives considered**: Threading `RetainedId` through the pure reducers — rejected: pollutes
the public surface with an internal type and couples the tab-order math to the reconciler.

## R5 — Focus indicator via E3's `Focused` visual-state (no parallel paint path)

**Decision**: The focus indicator renders through E3's `Focused` `VisualState` and the single
state→style resolver (feature 093). E4 ensures the focused control's `VisualState` resolves to
`Focused` (driven by `ControlRuntime.FocusedControl` over the retained identity) and adds **no**
procedural per-kind focus-paint branch. If E3 has not landed when E4 is implemented, the indicator
resolves through whatever path renders `Focused` at that time, still with no parallel branch.

**Rationale**: FR-005 / SC-005 require the indicator to go through E3 and forbid a second styling
path. The E3 precedence (visual state wins over class, 093 FR-003) guarantees the indicator is
always visible, so E4 adds no new precedence rule (spec's conflict-resolution note).

**Alternatives considered**: A dedicated focus-ring paint primitive — rejected: it is the parallel
procedural branch FR-005 forbids.

## R6 — Verification without a live window

**Decision**: The traversal / routing / order logic is proven by deterministic, offscreen results:
pure `Focus.order` / `traverse` / `route` outputs, FsCheck properties over ≥1000 generated
combinations (SC-006), and route-probes through the **real** `routeFocusedKey` adapter path (no
hand-seeded identity map — SC-001/SC-002/SC-004). The input→visible-change responds-proof reuses
the shipped E1 `captureRespondsProof` primitive (an inert host yields identical frames + `Inert`).
The E1 text-seam evidence is re-run unchanged (SC-003).

**Rationale**: The layout/rendering impact is only the focus indicator (R5); the deterministic
reducers and the real adapter route-probe cover the logic without a window, matching the 090/092/093
evidence pattern. This environment *does* have a GPU (a live Vulkan window opens via the X11 path),
so a live responds-proof remains available if needed, but is not required for the logic proofs.

**Alternatives considered**: Requiring a live windowed capture for every proof — rejected as
unnecessary for deterministic logic and slower; reserved for the single responds-proof.
