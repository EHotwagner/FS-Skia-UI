# US2 — the E1 text seam is preserved (SC-003)

A focused text control still receives typed/committed/composed text through the **unchanged** E1
`routeFocusedText` path. The host consults `routeFocusedText` BEFORE `routeFocusedKey` (R3 step 1),
so text delivery is not regressed by the E4 generalization.

- evidence-kind=text-seam-preserved
- renderer-mode=DeterministicRenderOnly
- status=pass
- driven-through=`ControlsElmish.routeFocusedText` (the 092 seam, unchanged)

## Results

- A focused `TextBox` pre-filled "hi" + `InsertText "X"` through `routeFocusedText` → the
  `RetainedId`-keyed draft becomes "hiX" (first keystroke appends to the pre-filled value, FR-005)
  and the control's `onChanged` binding still dispatches `TextChanged "hiX"`.
- The full 092 live-survival + focus-resolution suite continues to pass unchanged (46/46 in
  `Elmish.Tests`), confirming no text-delivery regression.

## Authoritative tests

- `Feature094FocusRoutingTests` / `094 US2 E1 text seam preserved (SC-003)`
- `Feature092LiveSurvivalTests` (the unchanged E1/E2 text suite, still green)
