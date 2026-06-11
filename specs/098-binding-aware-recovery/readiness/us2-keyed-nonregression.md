# US2 — keyed and container-keyed dispatch remain non-regressive (SC-002, FR-005)

evidence-kind=live-adapter-dispatch + standalone-dispatch
status=pass
authoritative=true
command=dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj ; dotnet test tests/Controls.Tests/Controls.Tests.fsproj
failure-class=product-defect

## Routing-seam non-regression (tests/Elmish.Tests/Feature098DispatchTests.fs)

Re-runs the 090 representative dispatch cases through the R3 routing seam — identical dispatched messages,
identical recovered ids:

- **AS1** — a directly-keyed leaf with a binding resolves to its `Key` (a fixed point) and dispatches,
  unchanged from 090.
- **AS2** — a container-keyed composite: a Click on an inner **unkeyed, unbound** positional node climbs to
  the keyed container and dispatches the container's binding, unchanged from 090.
- **AS3** — a control with both a `Key` and a binding: the binding is found by the unified id (the `Key`),
  dispatching **exactly once** (no double-dispatch).
- a `MapPointer`-only consumer (no authored bindings) is **bit-for-bit unchanged** — see
  `fallback-and-mappointer.md`.

## Control.dispatch keyed regression (tests/Controls.Tests/InteractionTests.fs)

The path-threaded `Control.dispatch` (D5) keeps the keyed branch byte-identical: the 8 keyed
`"save-button"` cases + typed parity + the `event.ControlId = None` wildcard stay **green unchanged**. No
current test or consumer passes an unkeyed `Kind` id to `dispatch`, so there is no payload regression for
keyed `dispatch` consumers.

## Unification keyed-branch identity (Feature098UnifiedSchemeTests + Feature090RecoveryTests)

Keyed nodes keep `Key` as their id across `Bounds` / `EventBindings` / `BoundIds` / recovery; only the
unkeyed fallback shifted `Kind → path`. The 090 container-keyed recovery (R1) still climbs to the container
key for an unkeyed **unbound** inner node (fixed-point preserved).

result=Elmish.Tests 55/55, Controls.Tests 282/282 — all keyed/container-keyed paths unchanged.
