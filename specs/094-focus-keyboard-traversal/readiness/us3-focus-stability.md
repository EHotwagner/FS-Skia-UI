# US3 — focus survives re-renders via the live retained path (SC-004, FR-006)

Focus survives a sibling-shifting re-render via the **live** retained path (`RetainedRender.step`),
resolving to the same control — **not** a hand-seeded `StateByIdentity` map (the 092 gap this
explicitly avoids repeating). Pointer↔keyboard focus composition (FR-006) is asserted at the same
seam.

- evidence-kind=focus-stability
- renderer-mode=DeterministicRenderOnly
- status=pass
- hand-seeded-focus=false
- driven-through=`RetainedRender.step` + `ControlsElmish.resolveFocus` + `routeFocusedKey`

## Results — SC-004 (stability)

- frame 0: focus the `Button` "btn" → its stable `RetainedId`.
- the UNRELATED shift: insert a banner above (`RetainedRender.step`). `idOfKey "btn"` on the stepped
  retained tree equals the original `RetainedId` — btn keeps its identity across the positional shift.
- the SAME focused `RetainedId` still routes activation on the post-shift tree:
  `routeFocusedKey s.Retained focused order Enter false` → `[Clicked]`.

## Results — FR-006 (pointer↔keyboard composition)

- a pointer press over the slider's box → `resolveFocus` returns the slider's `RetainedId`; the node
  is focusable, so the host adopts it as focus, and a subsequent `Tab` continues traversal from that
  position (`[FocusControl (Some "btn")]`).
- a press over the non-focusable banner resolves to a node that is **not** focusable, so the host
  leaves the current `FocusedControl` unchanged (it is not silently cleared).

## Authoritative tests

- `Feature094FocusRoutingTests` / `094 US3 focus stability over the live retained path (SC-004)`
- `Feature094FocusRoutingTests` / `094 US3 pointer<->keyboard composition (FR-006)`
