# SC-005 — slot lowering is pure / deterministic / total (FsCheck)

**Authoritative test:** `Feature095SlotCompositionTests` → `095 US1 lowering properties (FsCheck, SC-005)`.
**Command:** `dotnet run --project tests/Controls.Tests/Controls.Tests.fsproj --no-build -- --filter-test-list "095 US1 lowering properties"`
**Renderer mode:** DeterministicRenderOnly ([[fs-skia-evidence-mode]]).
**Failure class:** product-defect (a non-deterministic or throwing lowering is a defect).

## Result: PASS — three properties, each over >= 1000 generated inputs

The generator produces both representative kinds (`button`, `panel`) with an arbitrary subset of
their declared regions filled (including none), each bound to an arbitrary fill control (including
the empty-content case):

1. **purity / determinism** — `lowerSlots c` produces an identical IR on repeat calls
   (`"Ok, passed 1000 tests."`).
2. **totality** — `lowerSlots` never throws for any `(kind, fills)` combination
   (`"Ok, passed 1000 tests."`).
3. **additive** — when no slot attribute is present, `lowerSlots` is the identity (byte-identical,
   `"Ok, passed 1000 tests."`).

Determinism is structural: the lowering is a fold over the slot-fill list and the kind's declared
region order, with no clock, randomness, or I/O.
