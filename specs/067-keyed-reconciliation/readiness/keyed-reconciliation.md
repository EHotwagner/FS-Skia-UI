# Keyed reconciliation — algorithm & property results (067)

Feature-specific evidence required by spec §Evidence obligations. Records the
algorithm, the keys-first matching rule, the duplicate-key first-occurrence
diagnostic, and the round-trip / determinism property results. The reconciler is
`module internal FS.Skia.UI.Controls.Reconcile` (`src/Controls/Reconcile.fs`);
the per-story independent validation path is the in-assembly Expecto/FsCheck test
`tests/Controls.Tests/ReconcileTests.fs`.

## Patch algebra

`diff : prev -> next -> ReconcileResult<'msg>` where
`ReconcileResult = { Patch: NodePatch<'msg>; Diagnostics: ControlDiagnostic list }`.

- `NodePatch = Keep | Replace of Control | Update of UpdatePatch`.
- `UpdatePatch = { AttrChanges; ContentChange; AccessibilityChange; Children }`.
- `AttrChange = AttrSet of Attr | AttrRemoved of name` — sorted by `Name` (FR-009).
- `FieldChange<'a> = Unchanged | ChangedTo of 'a` — distinguishes "set to `None`"
  from "unchanged" for `Content`/`Accessibility` without `'a option option`.
- `ChildOp = ChildKeep of prevIndex*patch | ChildMove of fromIndex*toIndex*patch
  | ChildInsert of index*node | ChildRemove of key*index`.

`apply : prev -> NodePatch -> Control` reconstructs `next` from `prev` + the patch
and exists solely to prove the round-trip invariant.

## Node rule

A matched pair is a whole-subtree `Replace next` when its `Kind` **or** `Key`
differs (FR-006; a key change is an identity change, and `Update` has no channel
to carry a new `Key` — child matches always share their key, so this only ever
fires at the root). Otherwise the node yields an `Update` of the attribute,
content, accessibility, and child-op diffs, canonicalized to `Keep` when all four
channels are empty (identical-subtree no-op — the round-trip identity case).

- **Attributes** diff by `Name` (order-independent, FR-007); the change list is
  sorted by `Name` (FR-009). `AttrValue` carries a function case (`EventValue`)
  and an opaque `obj` case, so values are compared by a total custom comparator
  (structural for the data cases, reference/boxed-`Object.Equals` for the
  opaque/function/`'msg` cases). The comparator is conservative-safe: a spurious
  "changed" merely re-emits the next value, which `apply` writes verbatim, so the
  round-trip still holds.
- **Content/Accessibility** diff by structural `=` into a `FieldChange`.

## Keys-first matching rule (FR-003 / FR-010), within one sibling list

1. Bucket prev children by `Key`, **first occurrence wins**; each later duplicate
   emits a `KeyCollision` diagnostic.
2. For each next child in order: if it is keyed and the key is an unclaimed prev
   first-occurrence, match it (claiming that prev node); a keyed next child whose
   key is duplicated **within next** emits a `KeyCollision` and is treated as an
   insert.
3. Residual **unkeyed** next children match the residual unkeyed prev children
   **positionally** among themselves (keys win across the whole list first; the
   leftover unkeyed match by position — the single documented rule for the mixed
   case).
4. Prev nodes never claimed become `ChildRemove` (prev order); next nodes never
   matched become `ChildInsert` (next order).
5. Move detection is a simple deterministic forward scan: the first in-order match
   is `ChildKeep`; a match whose prev index falls before the running maximum is
   `ChildMove`. This is **not** LIS move-minimization (scoped out — correctness,
   not a benchmarked fast path).

Non-`ChildRemove` ops are emitted in next order, so `apply` rebuilds the next
child list by folding them in list order.

## Duplicate-key diagnostic (FR-011)

Duplicate keys within one sibling list (prev or next) resolve by first-occurrence
and surface `{ Code = KeyCollision; Severity = Warning; ControlId = <colliding
key>; ControlKind = <parent kind>; Message = "Duplicate key '…' within the
children of a '…' node; first occurrence wins."; EvidencePath = None }` on
`ReconcileResult.Diagnostics`. The diff stays total (never throws, SC-007) and
deterministic on such input.

## Property + test results (greened)

`tests/Controls.Tests/ReconcileTests.fs`, run via the in-assembly Expecto host —
**77 passed, 0 failed, 0 errored** (12 new + 65 pre-existing):

- **US1** (SC-001): `[a;b;c] → [c;a;b]` yields zero `Replace` ops; child ops are
  `ChildKeep`/`ChildMove` keyed to a/b/c; a moved-but-unchanged node carries
  `NodePatch.Keep`.
- **US2** (SC-003): a single changed attribute yields exactly one `AttrSet` naming
  that attribute and leaves the sibling node `Keep`; a content-only difference
  records exactly one `ContentChange` and nothing else.
- **US3**: `[a;b] → [a;b;c]` ⇒ exactly one `ChildInsert` for `c`; `[a;b;c] →
  [a;c]` ⇒ exactly one `ChildRemove` for `b`.
- **US4** (FR-010): unkeyed sibling lists diff identically on repeat; a mixed list
  matches keyed-by-key then residual-unkeyed positionally.
- **Edges**: root `Kind` change → `Replace`; duplicate keys → first-occurrence +
  `KeyCollision` `Warning`; empty→non-empty all-inserts, non-empty→empty
  all-removes, both-empty `Keep`; identical trees → `Keep`.
- **Round-trip (SC-002 / FR-008)**: `apply prev (diff prev next).Patch ≡ next`
  (structural, attribute-order-canonicalized) — FsCheck **passed 1000 tests**.
- **Determinism (SC-004)**: `diff prev next = diff prev next` — FsCheck **passed
  1000 tests**.

`Control<'msg>` does not satisfy F#'s `equality` constraint (the `EventValue`
function case), so the round-trip/determinism oracle compares structural
`sprintf "%A"` reprs of the function-free generated trees rather than `=`.
