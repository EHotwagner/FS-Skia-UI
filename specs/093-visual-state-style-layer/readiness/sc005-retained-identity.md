# SC-005 — state-driven look survives a sibling-shifting re-render (live retained path)

The state-driven appearance is consistent across an unrelated re-render under
E2's retained identity — demonstrated through the **live** retained path
(`RetainedRender.init`/`step`), **not** a hand-seeded `StateByIdentity` map (the
092 gap this explicitly avoids repeating).

## Wiring

The control's `VisualState` rides its attributes (`Attributes.visualState`,
absent ≡ `Normal`, parity-preserving). Because the state travels **with** the
control, it passes through the keyed reconciler diff: a `Keep`-matched control
retains its `RetainedId` and its state attribute across a positional shift, so
`faithfulContent` re-resolves the same state-driven look at the new box. No
change was made to the 067/091/092 identity scheme or the byte-identity-critical
`paintNode`/`RetainedRender` signatures.

## Evidence (real, in-repo)

`tests/Controls.Tests/Feature093RetainedStateTests.fs`:

- Frame 1: a keyed `Disabled` + `Primary`-classed button is the Stack's only
  child; `RetainedRender.init`.
- Frame 2: an unrelated sibling is **prepended**, shifting the button down;
  `RetainedRender.step`.
- Assertions:
  1. the button's `RetainedId` is **stable** across the shift;
  2. the retained path paints it via the resolver at its new box;
  3. the surviving look is the **Disabled** state's (`Fill = Muted`), which
     **differs** from the Normal-state render — so the state genuinely drove the
     look and survived, rather than being reset.

## Result

PASS — the hover/disabled/selected look survives a sibling-shifting model update
through the live retained path.
