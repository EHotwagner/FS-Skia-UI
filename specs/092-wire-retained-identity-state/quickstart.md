# Quickstart: Focus + Draft Text Survive a Positional Shift (FSI-first)

This is the honest-audience walkthrough (Constitution I): exercise the wired focus/text path the
way a host does, prove the headline benefit, and prove the rebuild-every-frame baseline fails the
same proof. Render-only — no Vulkan window (`fs-skia-evidence-mode`).

## What success looks like

```
frame 0:  Column [ TextBox(key=editor, value="hi") ]
          → user clicks the editor → it focuses → user types "x"  →  draft = "hix"

frame 1:  Column [ Banner("new!"); TextBox(key=editor, value="hi") ]   // unrelated insert shifts editor down
          → editor is STILL focused, draft is STILL "hix"
          → user types "y"  →  draft = "hixy"   (continues; did not reset)
```

## Steps (against the real adapter seam, no manual StateByIdentity seeding)

1. `RetainedRender.init theme size frame0` → initial retained structure (single paint, FR-009).
2. `RetainedRender.retainedHitTest clickX clickY retained` → the editor's stable `RetainedId`
   (works whether the editor is keyed, unkeyed, or wrapped — FR-004).
3. Focus that id; the seam seeds its `TextInput` from the editor's current value `"hi"` and its
   line mode (FR-005), then applies keystroke `x` → `StateByIdentity[id].Text.DraftText = "hix"`.
4. `RetainedRender.step theme size retained frame1` → `editor` is matched across the shift, so its
   `RetainedId` (and its `StateByIdentity` entry) is carried (FR-001/2/3). `WorkReduction` reports
   `ShiftedNodeCount ≥ 1`, `RecomputedNodeCount < BaselineNodeCount` (FR-007).
5. Apply keystroke `y` to the still-focused id → `DraftText = "hixy"`. Focus and draft survived the
   shift — no reset.

## Baseline that must FAIL the same proof

Re-`init` every frame (the pre-091 rebuild-every-frame behavior) mints a *new* id for `editor`
after the shift, so the id-keyed focus/draft state is lost — assert the draft resets / focus is
not carried. This is what makes step 4 a real proof and not a tautology.

## Invariants to re-confirm (FR-010)

- Round-trip: `step(...).Render.Scene` is byte-identical to `Control.renderTree theme size frame1`.
- Theme change (FR-008): with a different theme on frame 1, the output equals a full rebuild under
  the new theme (no stale-theme fragment reused).
- Chained 3+ frames keep round-trip parity (multi-frame, SC-004).
- Totality / determinism / identity-at-rest unchanged.

## Evidence written

`specs/092-wire-retained-identity-state/readiness/live-survival/` (survival + baseline-fails),
`.../focus-resolution/`, `.../work-reduction/`, `.../theme-reuse/`, `.../multi-frame/`. Authoritative
proofs are structural `Scene`/identity equality (capability-hash render funcs are not pixel encoders).
