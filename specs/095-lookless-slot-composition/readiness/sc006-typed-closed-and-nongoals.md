# SC-006 — typed-closed slots + non-goal inspection

**Authoritative test:** `Feature095SlotCompositionTests` → `095 US1 typed closure + non-goals (SC-006)`.
**Does-not-compile fixture:** [`sc006-undeclared-slot.fsx.txt`](./sc006-undeclared-slot.fsx.txt) (kept as
text so it is NOT added to a compiled project — its purpose is to demonstrate a compile error).
**Renderer mode:** DeterministicRenderOnly ([[fs-skia-evidence-mode]]).
**Failure class:** product-defect (a free-form slot escape hatch or a binding surface would be a defect).

## Result: PASS

### Closure is enforced by the typed `Props` fields (FR-001)

The only sanctioned public authoring path is the typed slot fields: `ButtonProps.Leading` /
`.Trailing`, `PanelProps.Header` / `.Footer`. There is:

- **no** public `Attr.slot : string -> ...` builder (the carrier `ControlInternals.slotFill` is
  `module internal`), and
- **no** public `SlotName` type.

A consumer therefore literally cannot reference a region a kind does not declare — filling
`Button.Header` does **not compile** because `ButtonProps` has no `Header` field (see the fixture).
This is a compile-time error, not a silent runtime drop.

### Non-goal structural inspection (FR-008)

The lowered IR carries each slot fill as an ordinary `Control<'msg>` child — a static value the
consumer's own `view` computed — **not** a binding, observable, `DataContext`, template instance,
or dependency/attached property. The test asserts every fill child is a concrete lowered control
(`Kind = "text-block"`), and the public surface adds **no** `DataContext` / binding / template
type (confirmed against the recaptured surface baselines, [surface-baselines.md](./surface-baselines.md)).
