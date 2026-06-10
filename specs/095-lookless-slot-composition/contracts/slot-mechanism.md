# Contract: Slot Mechanism (IR level)

The IR-level contract for how named slots are declared, carried, and lowered. This is the
internal plumbing; the public consumer surface is in `typed-slot-surface.md`. Documented
honestly per the E1 lesson: **a slot lowers to `Control<'msg>`; it is not a data-bound
template.**

## New public types (`FS.Skia.UI.Controls`, `Types.fsi`)

```fsharp
type AttrCategory =
    | ...
    | Slot                                              // E5: category for slot-fill attributes

and AttrValue<'msg> =
    | ...
    | SlotFillsValue of (string * Control<'msg>) list   // E5: ordered name→fill association list
```

- `Slot` joins the closed `AttrCategory` DU. Adding it is a Tier-1 surface move.
- `SlotFillsValue` joins the closed `AttrValue<'msg>` DU. The carried list is an ordered
  association from **declared slot name** (internal plumbing) to **fill sub-tree**. A name
  **absent** from the list means the slot is *unfilled*; a name **present** means *filled*
  (even with empty content).

These two cases are public because the IR is open, but **no public builder constructs them
from a free-form string** (FR-001). Hand-constructing raw IR is going around the front door,
not an escape hatch the mechanism provides.

## Internal builder + extractors (`Control.fs`, `module internal ControlInternals`)

```fsharp
val slotFill    : fills: (string * Control<'msg>) list -> Attr<'msg>
val slotFillsOf : attrs: Attr<'msg> list -> (string * Control<'msg>) list
val slotFor     : name: string -> attrs: Attr<'msg> list -> Control<'msg> option
```

| Function | Behavior | Mirrors |
|---|---|---|
| `slotFill fills` | `create "slot" Slot (SlotFillsValue fills)` | `Attributes.styleClasses` (`Attributes.fs:71`) |
| `slotFillsOf attrs` | last `"slot"` attr → `SlotFillsValue`, else `[]` | `ControlInternals.styleClassesOf` (`Control.fs:50-61`) |
| `slotFor name attrs` | `slotFillsOf` then `List.tryPick (fun (n, c) -> if n = name then Some c else None)` | — |

**Last-writer convention**: a control carries at most one effective `Slot` attribute; the last
wins (same as `styleClasses`/`visualState`).

## Lowering contract

For a slot-bearing kind, the geometry/lowering function MUST:

1. **Extract** fills via `slotFillsOf attrs` (empty when no slot attr present).
2. **Per declared named region**, in the kind's fixed region order:
   - if `slotFor name` is `Some fill` → place `fill` at that region;
   - else → render that region's **default content** (the kind's existing chrome; peripheral
     regions default to **zero geometry**).
3. **Inject** every fill sub-tree into the lowered control's `Children`, ordered by region
   position, so the keyed reconciler, focus traversal, and binding dispatch include them with
   no new machinery.

### Invariants (testable)

| ID | Invariant | Verified by |
|---|---|---|
| SM-1 | **Purity / determinism**: identical `(kind, fills)` ⇒ identical `Control<'msg>` IR | FsCheck property, ≥1000 inputs (SC-005) |
| SM-2 | **Totality**: lowering never throws; every region has a default | FsCheck property (SC-005) |
| SM-3 | **Byte-identity**: no slot attr present ⇒ output structurally-`Scene`-equal to the pre-slot oracle across states | parity test vs frozen oracle (SC-002) |
| SM-4 | **Absent ≠ empty**: slot name absent ⇒ default region; present with empty content ⇒ empty region by choice | placement test (Edge Cases) |
| SM-5 | **Placement**: a filled slot's sub-tree appears at its region; two distinct slots ⇒ two distinct regions, no collision/swap | placement test (SC-001) |
| SM-6 | **Composition**: fills land in `Children`, inheriting E1 dispatch / E2 identity / E3 style / E4 focus unchanged | composition + retained-identity tests (SC-003/004) |
| SM-7 | **No reconciler change**: the feature 067/091/092 identity scheme is untouched | code review + retained-identity test (FR-004) |

## Non-goals (FR-008 — held by this contract)

The slot mechanism introduces **none** of: `DataContext`, binding expression / observable,
per-item template instantiation, `ControlTemplate` type, dependency/attached properties,
CSS-selector styling, a new top-level `Control` field, a second message channel. The line is
**"declarative, lowers to the `Control<'msg>` IR, no binding."**
