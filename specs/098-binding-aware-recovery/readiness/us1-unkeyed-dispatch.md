# US1 — an unkeyed authored button responds in the live host (SC-001)

evidence-kind=live-adapter-dispatch
status=pass
authoritative=true
command=dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj
seam=ControlsElmish.routeInteractivePointer (the exact seam runInteractiveApp wires) + bindingMessagesFor
failure-class=product-defect

## What is proven (an artifact an un-fixed build cannot produce)

Driven through the **real** live-adapter routing seam with a real press+release Click over the computed
bounds (no hand-seeded binding, no mock):

- **AS1** — a view with a single **unkeyed** `Button.onClick` dispatches its authored message; the
  competing `MapPointer` clause for the same control's path id is **not** consulted (binding wins, no
  double-advance). Before R3 this button was dead (`nearestAuthored` was key-only → `None`).
- **AS2** — a **nested** unkeyed bound control inside an unbound, unkeyed container: a Click on the inner
  control recovers the inner bound node (its path id `"0.0.0"`) and its binding dispatches.
- **AS3** — see `fallback-and-mappointer.md`: an unkeyed **unbound** leaf with no bound/keyed ancestor
  recovers `None` and falls back to `MapPointer` (no spurious dispatch).

## Tests (tests/Elmish.Tests/Feature098DispatchTests.fs)

- "US1 AS1: an unkeyed Button.onClick dispatches; MapPointer is not consulted"
- "US1 AS2: a nested unkeyed bound control inside an unbound container dispatches"
- "US1 AS3: an unbound unkeyed leaf recovers None and falls back to MapPointer"

## Mechanism (verified by the FSI transcript, fsi-transcript.md)

`renderTree` emits `BoundIds` = the canonical `Key ?? path` ids of every bound node (the unkeyed button's
id is its path `"0.0"`). A Click resolves to that path id; `nearestAuthored` now treats the node as
authored because its id ∈ `BoundIds` and returns `Some "0.0"`; `bindingMessagesFor` looks up `"0.0"` in
`EventBindings` (same unified scheme), finds the `onClick` binding, and dispatches. `MapPointer` is not
consulted.

result=Elmish.Tests 55/55 pass (7 new Feature098 dispatch cases green).
