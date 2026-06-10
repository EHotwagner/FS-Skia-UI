# US1 — keyboard traversal in a predictable order (SC-001)

Tab / Shift+Tab advance `FocusedControl` through the focusable controls of mixed `FocusOrder` in
`FocusOrder`-then-layout order, wrap cyclically at both ends, and skip non-focusable controls. Pure
`Focus.order` + `Focus.traverse` results, plus the host-seam traversal wiring.

- evidence-kind=keyboard-traversal
- renderer-mode=DeterministicRenderOnly
- status=pass
- driven-through=`Focus.order` / `Focus.traverse` (public) and `ControlsElmish.routeFocusedKey`
  emitting `ControlRuntimeMsg.FocusControl` (the host seam `runInteractiveApp` wires)

## Results

- `Focus.order` over a tree of mixed `FocusOrder` yields `["act-0"; "nav-1"; "act-none"]` —
  FocusOrder ascending, `None` last, document-order tiebreak; the non-focusable static text and the
  non-focusable layout `Stack` are excluded (US1.3).
- `Focus.traverse`: `None + Next` → first, `None + Previous` → last; `Next` advances and wraps at the
  end; `Previous` reverses and wraps at the start; `Next` then `Previous` is identity; an empty
  `TabOrder` is a no-op (`Next`/`Previous` both `None`, never throws).
- Host seam: `routeFocusedKey r focused order (Unknown "Tab") false` → `[FocusControl (Some "sld")]`;
  `Shift+Tab` → the previous stop; Tab from the last stop wraps to the first. The host maps the next
  `ControlId` back to its stable `RetainedId` so traversal keeps tracking the moved focus.

## Authoritative tests

- `Feature094FocusTests` / `094 US1 tab order (Focus.order, SC-001)`
- `Feature094FocusTests` / `094 US1 traversal (Focus.traverse, SC-001)`
- `Feature094FocusRoutingTests` / `094 US1 traversal at the host seam (SC-001)`
