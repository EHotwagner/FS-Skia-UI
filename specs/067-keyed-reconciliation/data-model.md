# Phase 1 Data Model — Internal Keyed Reconciliation

All types live in `module internal FS.Skia.UI.Controls.Reconcile`. They are
**internal** — never exported on the package's public api-surface (R1). `'msg` is
opaque payload threaded through unchanged; the diff never interprets it.

## Inputs (existing IR — unchanged)

`Control<'msg> = { Kind: ControlKind; Key: ControlId option; Attributes: Attr<'msg> list; Children: Control<'msg> list; Content: string option; Accessibility: AccessibilityMetadata option }`
(`src/Controls/Types.fsi:231`). No field is added or changed (FR-002, Assumptions).

## New types

### `FieldChange<'a>`
```
type FieldChange<'a> =
    | Unchanged          // field is identical in prev and next
    | ChangedTo of 'a    // field set to this value in next (value may itself be None)
```
Used for `Content: string option` and `Accessibility: AccessibilityMetadata option`
so "set the field to `None`" is distinct from "field unchanged" without `'a option option`.

### `AttrChange<'msg>`
```
type AttrChange<'msg> =
    | AttrSet of Attr<'msg>        // added, or value changed (matched by Name) — FR-007
    | AttrRemoved of name: string  // present in prev, absent in next
```
The emitted `AttrChange list` is **sorted by `Name`** for determinism (FR-009).

### `NodePatch<'msg>`
```
type NodePatch<'msg> =
    | Keep                            // structurally equal subtree — no-op
    | Replace of Control<'msg>        // kind mismatch or unmatched node — whole subtree (FR-006)
    | Update of UpdatePatch<'msg>     // same Kind (+ same Key/position) — targeted change
```

### `UpdatePatch<'msg>`
```
and UpdatePatch<'msg> =
    { AttrChanges: AttrChange<'msg> list          // FR-007, sorted by Name
      ContentChange: FieldChange<string option>   // FR-004 content
      AccessibilityChange: FieldChange<AccessibilityMetadata option>  // FR-004 a11y
      Children: ChildOp<'msg> list }              // FR-005 recursion, ordered
```
An `Update` whose three change channels are all empty/`Unchanged` and whose
`Children` are all `ChildKeep`-with-`Keep` is canonicalized to `Keep` (so identical
trees → empty patch; SC, Edge "Identical trees").

### `ChildOp<'msg>`
```
and ChildOp<'msg> =
    | ChildKeep   of index: int * patch: NodePatch<'msg>          // matched in place; recurse
    | ChildMove   of fromIndex: int * toIndex: int * patch: NodePatch<'msg>  // matched, reordered (US1)
    | ChildInsert of index: int * node: Control<'msg>            // next-only child (US3)
    | ChildRemove of key: ControlId option * index: int         // prev-only child (US3)
```
`index`/`toIndex` are positions in the **next** sibling list; `fromIndex` is the
position in the **prev** list. A `ChildMove` carrying a `Keep` patch is the
"reorder with no attribute sub-patch" case of US1 AC#2.

### `ReconcileResult<'msg>`
```
type ReconcileResult<'msg> =
    { Patch: NodePatch<'msg>
      Diagnostics: ControlDiagnostic list }   // KeyCollision etc. — FR-011, SC-007
```
`Diagnostics` reuses the existing `ControlDiagnostic` / `ControlDiagnosticCode`
(`Types.fsi`); duplicate keys emit `{ Code = KeyCollision; Severity = Warning;
ControlId = <colliding key>; ControlKind = <parent kind>; Message = ...;
EvidencePath = None }`.

## Operations (signatures — see contracts/reconcile.fsi)

- `diff : prev:Control<'msg> -> next:Control<'msg> -> ReconcileResult<'msg>`
  — pure, total (never throws), deterministic (FR-001/009, SC-004/007).
- `apply : prev:Control<'msg> -> patch:NodePatch<'msg> -> Control<'msg>`
  — pure reconstruction used to prove the round-trip (FR-008); applying a patch to
  the tree it was diffed against yields a tree structurally equal to `next`.

## Invariants (property-checked)

1. **Round-trip** (FR-008/SC-002): `apply prev (diff prev next).Patch ≡ next`
   structurally, ∀ generated `(prev, next)`, ≥1000 cases, no counterexample.
2. **Determinism** (FR-009/SC-004): `diff prev next = diff prev next`
   (byte/structurally identical) across repeated calls and processes.
3. **Totality** (SC-007): `diff` returns for every input, including duplicate-key,
   empty-tree, and kind-mismatch cases — never throws.
4. **Keyed reorder preserves identity** (US1/SC-001): reordering N keyed siblings
   yields a patch with **zero** `Replace` ops; reordered nodes appear as
   `ChildMove`.
5. **Targeted update** (US2/SC-003): a single changed attribute yields exactly one
   `AttrSet` and no other touched node.
6. **Kind mismatch ⇒ replace** (FR-006): a matched pair with differing `Kind`
   produces `Replace next`, never an `Update`.

## State transitions

None — the model is immutable data describing a transformation; the reconciler is
a pure stateless function (Constitution Principle IV: N/A).
