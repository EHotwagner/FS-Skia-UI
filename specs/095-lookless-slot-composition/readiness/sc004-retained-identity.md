# SC-004 — slotted content keeps E2 retained identity across a sibling shift (live path)

**Authoritative test:** `Feature095SlotCompositionTests` →
`095 US3 slotted retained identity across a sibling shift (live path, SC-004)`.
**Renderer mode:** DeterministicRenderOnly ([[fs-skia-evidence-mode]]).
**Failure class:** product-defect (a slotted control losing identity on a shift is the 092 regression).

## Result: PASS — proven through the LIVE retained path, not a hand-seeded map

The proof drives the real wired retained path (`RetainedRender.init` then `RetainedRender.step`),
**not** a hand-seeded `StateByIdentity` map (the 092 gap this deliberately avoids):

- `frame0` = a keyed `Panel` (`panelP`) whose `Header` slot is filled with a keyed, focusable control
  (`field`), inside a stack.
- `frame1` = the **092-case sibling shift**: a banner is inserted **above** the panel in the stack.
- After `RetainedRender.step`, the retained node for the slotted `field` (located by walking
  `RetainedNode.Children`) keeps the **same `RetainedId`** it had in `frame0` — its E2 retained
  identity (and therefore its focus/text) survives the shift.
- The wired frame's `Render.Scene` is **byte-identical** to a full `Control.renderTree` rebuild of
  `frame1` — slotted content is a first-class sub-tree on the production render path, not a parallel
  slot channel.

The keyed reconciler matches `panelP` (and thence `field`) key-first across the shift; because the
slot fill lives in `Children`, it inherits that retained identity with no E2 code change.
