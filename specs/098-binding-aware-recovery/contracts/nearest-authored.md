# Contract: binding-aware recovery and dispatch lookup

**Packages**: `FS.Skia.UI.Controls` (`nearestAuthored`), `FS.Skia.UI.Controls.Elmish` (`bindingMessagesFor`)
**Surface**: `nearestAuthored` signature **unchanged** (behavior widens); no public-signature change in Elmish.

## `Control.nearestAuthored`

```fsharp
val nearestAuthored: result: ControlRenderResult<'msg> -> hit: ControlId -> ControlId option
```

**Behavior (widened, FR-003)**: walking the hit node's path over `result.Layout`, a node is
*authored* when it **carries a `Key`** (its `LayoutNode.Id <> path`) **OR** its canonical id
(`= node.Id = Key ?? path`) is in `result.BoundIds`. Returns the nearest authored ancestor
including self; `None` only when nothing on the path is keyed or bound.

| Input | Before R3 | After R3 |
|---|---|---|
| directly-keyed leaf, hit on it | `Some Key` (fixed point) | `Some Key` — unchanged (FR-005) |
| container-keyed, hit inner unkeyed-unbound | `Some containerKey` | `Some containerKey` — unchanged |
| **unkeyed bound leaf**, hit on it | `None` (dead) | `Some path` — **new dispatch** |
| unkeyed unbound leaf under unkeyed **bound** container | `None` | `Some containerPath` — climbs to bound container |
| unkeyed unbound leaf, no keyed/bound ancestor | `None` | `None` — unchanged (→ `MapPointer`) |
| keyed-but-unbound node, hit on it | `Some Key` | `Some Key`; binding lookup finds nothing → `MapPointer` |

**Properties**: pure, total, deterministic; reads existing render data only (now incl.
`result.BoundIds`); no layout-math change.

## `bindingMessagesFor` (dispatch lookup, FR-004)

Logic **unchanged** (`ControlsElmish.fs:155`):

1. On a `Click(control, …)`, recover `authored = nearestAuthored rendered control`.
2. If `Some authored`: filter `rendered.EventBindings` by `ControlId = authored` AND
   `EventKind ∈ {click, changed, selected}`; if non-empty, dispatch those messages with
   `ControlEvent.ControlId = Some authored`; **`MapPointer` is NOT consulted**.
3. If `None`, or no click-equivalent binding matches: return `None` → host falls back to
   `MapPointer` with the raw interaction.

**Why it now resolves the unkeyed case**: the recovered id and the `EventBindings` keys
share the unified `Key ?? path` scheme, so an unkeyed-bound id matches its binding.

**Precedence (FR-004/FR-005)**: authored binding wins; `MapPointer` is consulted **only**
when recovery is `None` or no matching binding — never both (no double-dispatch). The
`disabledOrReadOnly` guard is preserved.

## `Control.dispatch` (standalone API, D5)

```fsharp
val dispatch: event: ControlEvent -> control: Control<'msg> -> 'msg list
```

Threads the structural path so its `event.ControlId = Some binding.ControlId` matching uses
the unified scheme. Keyed callers (the entire `InteractionTests.fs` suite) and the
`event.ControlId = None` wildcard are byte-identical; only the unkeyed `Kind`-id matching
(unused by any current test/consumer) changes to the path scheme.

## Out of scope (FR-008)

`resolveFocus` / `RetainedRender.retainedHitTest` / the `RetainedId` domain are untouched.
A `focus-nonregression` artifact proves focus resolution is unchanged.
