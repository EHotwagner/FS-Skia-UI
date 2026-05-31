# Effects Boundary (Principle IV applicability) — 039

| Field | Value |
|---|---|
| Authoritative command | n/a — design record (no stateful/I-O runtime workflow added) |
| Artifact path | this file |
| Failure class | mvu-boundary-applicability |
| Next action on failure | If a future task adds stateful/I-O runtime behaviour, escalate it through the MVU/effect boundary before marking `[X]` |

## Determination

**Principle IV (Elmish/MVU is the boundary for stateful or I/O workflows) is
Not Applicable to every task in feature 039.**

Rationale: this feature produces documents, fixtures, and two build-tooling
projects. It adds no stateful runtime workflow and no I/O-bearing product
behaviour. The spike target is a trivial console action that calls one pure
function (`Spike.run : unit -> string`) and prints its result. There is no
`Model`/`Msg`/`Effect`/interpreter surface to design, because there is no owned
workflow state or product-side I/O.

The ordinary path applies instead: spec → `.fsi` (the library's `Spike.fsi`) →
exercise (`dotnet run -- SpikeHello`) → implementation.
