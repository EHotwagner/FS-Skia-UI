# US3 — slotted content composes with E1–E4 (SC-003)

**Authoritative test:** `Feature095SlotCompositionTests` → `095 US3 slotted content composes with E1/E3/E4 (SC-003)`.
**Renderer mode:** DeterministicRenderOnly ([[fs-skia-evidence-mode]]).
**Failure class:** product-defect (a slotted-content routing/style/focus dead-zone is a defect).

## Result: PASS — slotted content is a first-class sub-tree

- **E1 (dispatch)** — a clickable control filled into `Button.Leading` dispatches its authored
  message through the existing flat per-`ControlId` mechanism: `Control.dispatch` of a `click` event
  carrying the slotted child's `ControlId` returns `[ 99 ]`.
- **E3 (style resolve)** — a `Variant StyleVariant.Danger` class attached to the slotted child
  changes its resolved paint vs. the un-classed fill — it resolves through the E3 resolver unchanged.
- **E4 (tab order)** — a focusable control filled into a non-focusable host's slot appears as a stop
  in `Focus.order` (`ids` contains `"leadIcon"`). (A focusable host is itself a single tab stop and
  is not descended — so the host in the proof is a non-focusable `Panel`.)

E5 owns none of this machinery: the fill lands in `Children`, so E1–E4 apply by construction
(FR-005). See [sc004-retained-identity.md](./sc004-retained-identity.md) for the E2 retained-identity
proof across a sibling shift.
