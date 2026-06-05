# Quickstart — Internal Keyed Reconciliation (feature 067)

The reconciler is **internal-only**: it is not on any public `.fsi` and cannot be
called from a generated product or an FSI session against the packed package. You
exercise it from an in-assembly test (`tests/Controls.Tests/`), which reaches the
`module internal Reconcile` via `[<assembly: InternalsVisibleTo("Controls.Tests")>]`.

## Worked example — US1 keyed reorder

```fsharp
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Reconcile

// helper: a keyed leaf
let node key = { Kind = "TextBlock"; Key = Some key; Attributes = []
                 Children = []; Content = Some key; Accessibility = None }

let parent kids = { Kind = "Stack"; Key = Some "root"; Attributes = []
                    Children = kids; Content = None; Accessibility = None }

let prev = parent [ node "a"; node "b"; node "c" ]   // [a; b; c]
let next = parent [ node "c"; node "a"; node "b" ]   // [c; a; b]

let result = Reconcile.diff prev next

// US1 / SC-001: keyed children matched by Key — no subtree replacement.
match result.Patch with
| NodePatch.Update u ->
    // every child op is a ChildKeep/ChildMove keyed to a/b/c; ZERO Replace ops,
    // and the moved nodes carry NodePatch.Keep (no attribute sub-patch).
    u.Children |> List.forall (function
        | ChildKeep (_, NodePatch.Keep) | ChildMove (_, _, NodePatch.Keep) -> true
        | _ -> false)
| _ -> false
```

## Round-trip property (FR-008 / SC-002)

```fsharp
// Over ≥1000 generated (prev, next) pairs of Control<int>:
Reconcile.apply prev (Reconcile.diff prev next).Patch  // ≡ next, structurally
```

## Edge cases to remember

- **Root kind change** (`"Stack"` → `"Grid"`): `NodePatch.Replace next` — not an
  attribute diff (FR-006).
- **Duplicate keys** in one sibling list: first occurrence wins; later collisions
  surface a `ControlDiagnostic { Code = KeyCollision; Severity = Warning }` on
  `result.Diagnostics`. The diff stays total and deterministic (FR-011, SC-007).
- **Empty trees**: empty→non-empty = all `ChildInsert`; non-empty→empty = all
  `ChildRemove`; both empty = `NodePatch.Keep`.
- **Identical trees**: `NodePatch.Keep` (the empty/no-op patch).

## Validation

Run `./fake.sh build -t Route` first and run only the gates it prints (escalates to
the `controls-public-surface` set). On the maintainer-verify path, run the
serialized six-target order sequentially (Dev → GeneratedGuidanceCheck →
TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit). The
public-surface baseline must show **zero** delta (SC-005).
