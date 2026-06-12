# Focus-ring structural-Scene evidence (T010, SC-001/002/012)

evidence-kind=structural-scene
proof=exactly-one-focused
enforcing-test=tests/Controls.Tests/Feature108FocusTests.fs

Captured behaviour of `Focus.markFocused` (real `Control` tree; `VisualState` read over the public
`AttrValue.VisualStateValue` surface, the same attribute the renderer's `visualStateOf` reads):

| Case | focused id | focusedCount(stamped) | Result |
|------|-----------|------------------------|--------|
| keyed sibling (button "a","b") | Some "a" | 1 | only "a" carries Focused; "b" Normal |
| unkeyed same-kind siblings | Some "0.1" | 1 | only the path-"0.1" button (distinct from "0.0") |
| unkeyed root button | Some "0" | 1 | the root is focused via its path |
| at-rest (no focus) | None | 0 | `markFocused None tree` ≡ `tree` (`%A`-identical, SC-012) |
| stale / removed id | Some "missing" | 0 | nothing stamped, no throw |
| structural container | Some "0" (a stack) | 0 | non-focusable element never stamped (FR-004) |
| consumer Disabled | Some "a" (a=Disabled) | 0 | Disabled preserved; Focused does not override |

Exactly one focusable control carries the ring at a time (FR-003); keyed AND unkeyed focusable controls
are reachable (FR-002); the at-rest tree is byte-identical (SC-012). This is the production render path
(`markFocused` feeds the `view` output that `Control.renderTree` paints) — not a hand-built scene.
