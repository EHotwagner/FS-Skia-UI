---
name: fs-skia-reconciliation
description: Understand and work with the internal keyed VDOM diff over the lowered Control<'msg> IR (feature 067) — its key-first-then-positional matching, the NodePatch/ChildOp operation set, the totality/determinism/identity-at-rest/round-trip invariants, and the module's disposition (internal, property-tested, deliberately unwired, parked). Use when reading the diff invariants, extending the property tests, or scoping the deferred render-path wiring.
compatibility: F# net10.0; FS.Skia.UI.Controls package, `module internal Reconcile` — assembly-internal, no public-surface entry (zero baseline delta). Parked spike; not wired into the render path.
metadata:
  author: fs-skia-ui
  source: specs/067-keyed-reconciliation/plan.md
---

# fs-skia-reconciliation

The keyed **VDOM diff** over the lowered `Control<'msg>` IR (feature 067). It computes a
minimal patch from a previous control tree to a next one, so a future renderer can mutate in
place instead of rebuilding. This skill teaches the diff's invariants, key handling, and the
`NodePatch`/`ChildOp` operation set, **and** records the module's disposition — because the
single most important fact about `Reconcile` is non-obvious from a glance at the source: it is
a **deliberately-parked internal spike**, fully property-tested but **not wired into the live
render path**. It is intentional, not dead code, not abandoned.

## Scope / when to use

Use this skill when:

- You are reading `src/Controls/Reconcile.fsi`/`.fs` and need the diff invariants without
  reverse-engineering them from the implementation or source comments.
- You are extending or auditing the Expecto/FsCheck property tests in `tests/Controls.Tests`
  that reach the module via `[<assembly: InternalsVisibleTo("Controls.Tests")>]`.
- You are scoping the **deferred** feature that would wire keyed reconciliation into the
  render path, and need to know what exists today and what the integration point is.

Do **not** use this skill to wire `Reconcile` into the render path as part of an unrelated
change: that is a separate, out-of-scope future feature (see **Disposition** below). And do
**not** promote the module to public to "document it via the surface baseline" — it stays
`module internal` by design (zero public-surface delta, SC-005 of 067).

## Disposition (the single source for the module's status)

`Reconcile` is:

- **`module internal`** — assembly-internal accessibility, genuinely unreachable from package
  consumers (mirrors the `module internal SceneRenderer` precedent). It is deliberately **not**
  in the Controls capability `contracts:` list, so `ApiSurfaceGen`/`PackageSurfaceCheck` emit
  **no** public-surface entry — zero baseline delta.
- **Property-tested** — the Expecto/FsCheck suite reaches `diff`/`apply` via
  `[<assembly: InternalsVisibleTo("Controls.Tests")>]`.
- **Deliberately unwired** — `diff`/`apply` are pure functions that nothing in the live render
  path calls today. Feature 067 shipped the module unwired on purpose.
- **Parked, not abandoned** — the invariants and tests are the durable foundation an eventual
  wiring feature builds on.

**Deferred future work (out of scope here):** wiring `Reconcile.diff` into the render/diff
path. Today the host re-renders the full lowered `Control<'msg>` tree each frame; the
integration point a future feature would touch is that **full-tree render/redraw step** —
it would call `Reconcile.diff prev next`, then apply the resulting `NodePatch` to the
retained visual tree instead of rebuilding it (and could surface `Diagnostics` such as
`KeyCollision` at that boundary). That wiring is a distinct, larger feature and must be
specified separately; if it lands, **this disposition is the thing that gets updated**, so the
module's status has one source of truth.

## Driven-library API and the diff contract

The whole module is `src/Controls/Reconcile.fsi` (read it for the authoritative signatures).
Two entry points, both pure:

- `diff : prev:Control<'msg> -> next:Control<'msg> -> ReconcileResult<'msg>` — pure, total,
  deterministic. Never throws.
- `apply : prev:Control<'msg> -> patch:NodePatch<'msg> -> Control<'msg>` — applies a patch
  produced by `diff prev _` back onto `prev`, reconstructing a tree structurally equal to
  `next`. Exists to prove the round-trip invariant.

**Key-first, then positional matching.** Children are matched by their `Key` first; the
unkeyed residuals are then matched positionally. A `Kind` mismatch on a matched pair is **not**
a field update — it yields a whole-subtree `Replace`.

**The operation set.**

- `NodePatch<'msg>` — one node's diff: `Keep` | `Replace of Control<'msg>` | `Update of
  UpdatePatch<'msg>`. `Update` recurses into children.
- `UpdatePatch<'msg>` — a targeted in-place change for a matched same-`Kind` node:
  `AttrChanges`, `ContentChange`, `AccessibilityChange`, `Children`.
- `AttrChange<'msg>` — `AttrSet of Attr<'msg>` | `AttrRemoved of name`, matched by `Attr.Name`;
  the emitted list is sorted by `Name` for deterministic output.
- `FieldChange<'a>` — `Unchanged` | `ChangedTo of 'a`; avoids `'a option option` when a
  `Content`/`Accessibility` field is set to `None`.
- `ChildOp<'msg>` — ordered child operation: `ChildKeep of index * patch` |
  `ChildMove of fromIndex * toIndex * patch` | `ChildInsert of index * node` |
  `ChildRemove of key * index`. **Index conventions:** `ChildKeep`/`ChildMove` carry the
  matched child's **prev**-list index as their source; the child's next-list position is its
  ordinal among the non-`ChildRemove` ops (emitted in next order). `ChildInsert.index` and
  `ChildMove.toIndex` are **next**-list positions; `ChildRemove.index` is the **prev**-list
  position.

**Diagnostics, never exceptions.** `ReconcileResult<'msg>` carries `Patch` plus
`Diagnostics : ControlDiagnostic list`. Duplicate keys among a node's children produce a
`KeyCollision` diagnostic rather than a throw — the function stays total.

**The invariants (what the property tests assert).**

- **Totality** — `diff` is defined for every pair of trees; it never throws (collisions become
  diagnostics).
- **Determinism** — same `(prev, next)` ⇒ same result, including attribute ordering
  (canonicalized by `Name`).
- **Identity-at-rest** — `diff x x` produces a no-op patch (`Keep`, no child ops).
- **Round-trip** — `apply prev (diff prev next).Patch` is structurally equal to `next`, up to
  the attribute ordering the diff canonicalizes.

## Runnable example — read the invariants, exercise the round-trip

```fsharp
open FS.Skia.UI.Controls   // Reconcile is internal; visible to Controls.Tests via InternalsVisibleTo

// Identity-at-rest: diffing a tree against itself is a no-op Keep.
let result = Reconcile.diff tree tree
match result.Patch with
| Reconcile.NodePatch.Keep -> ()        // expected
| _ -> failwith "identity-at-rest violated"
result.Diagnostics |> List.isEmpty      // true for well-keyed input
```

```fsharp
// Round-trip: applying the patch reconstructs `next` structurally (up to attr ordering).
let r = Reconcile.diff prev next
let rebuilt = Reconcile.apply prev r.Patch
// rebuilt ≡ next  (structural equality, attributes canonicalized by Name)

// A Kind mismatch on a matched pair is a whole-subtree Replace, not an Update.
match (Reconcile.diff (stackOf [..]) (buttonOf [..])).Patch with
| Reconcile.NodePatch.Replace _ -> ()   // expected — different Kind
| _ -> failwith "Kind-mismatch should Replace"
```

The property tests live in `tests/Controls.Tests`; run them with
`dotnet test tests/Controls.Tests/Controls.Tests.fsproj` (they reach the internals via
`InternalsVisibleTo`). The module routes to the inner-loop tier — `./fake.sh build -t Dev`
covers a source-internal edit; it has no public `.fsi` surface to escalate.

## Persistent problems

When the diff or its round-trip will not reconcile after reasonable in-repo attempts,
**official online docs first** (the F#/.NET docs; React's reconciliation/keys notes are the
canonical prior art for keyed-VDOM diffing), then community sources (forums, issue trackers,
changelogs). Record findings and resolving links in `specs/<feature>/feedback/` and, for
durable lessons, in this skill's **Sources** line. Offline, record "research blocked — <why>"
rather than hard-failing the phase.

## Related

- [[fsharp-graph-algorithms]] — the keyed matching and ordered child-op emission lean on the
  same DAG/topological reasoning that skill teaches.
- [[fs-skia-typed-controls]] authors the typed front door that lowers to the `Control<'msg>`
  IR this diff operates over.
- [[fsharp-build-orchestration]] — the property-test harness (Expecto + FsCheck) that proves
  the totality/determinism/round-trip invariants.

## Sources / links

- Feature 067 plan: `specs/067-keyed-reconciliation/plan.md`; signatures in
  `src/Controls/Reconcile.fsi`.
- React reconciliation & keys (keyed-VDOM-diff prior art):
  https://react.dev/learn/preserving-and-resetting-state
- F#/.NET docs: https://learn.microsoft.com/en-us/dotnet/fsharp/
