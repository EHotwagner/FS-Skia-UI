# US1 — fill a named slot to re-skin shape (SC-001)

**Authoritative test:** `Feature095SlotCompositionTests` → `095 US1 slot placement (SC-001)`.
**Command:** `dotnet run --project tests/Controls.Tests/Controls.Tests.fsproj --no-build -- --filter-test-list "095 US1 slot placement"`
**Renderer mode:** DeterministicRenderOnly ([[fs-skia-evidence-mode]]).
**Failure class:** product-defect (a misplaced or dropped fill is a slot-lowering defect).

## Result: PASS

- Filling `Button.Leading` injects the supplied sub-tree into the lowered Button's `Children`
  (`child-keys = [Some "leading-icon"]`), and lowering **consumes** the internal slot carrier
  (`has-slot-attr = false`) — a single source of truth.
- Filling `Button.Leading` **and** `Button.Trailing` lands in two **distinct, ordered** regions
  (`[Some "L"; Some "T"]`) — no collision, no swap.
- `Panel.Header` lands **before** content and `Panel.Footer` **after** (`[Some "H"; Some "B"; Some "F"]`)
  — the composite-container region case.
- `slotFor` resolves a present name even when its content is empty, and returns `None` for an absent
  name (absent ≠ empty edge case).

The fills land in `Children`, so they travel through the keyed reconciler and inherit E1–E4 + E2
identity by construction (next: US3). The same placement is reproduced from the packed library in
[fsi-transcript.md](./fsi-transcript.md).
