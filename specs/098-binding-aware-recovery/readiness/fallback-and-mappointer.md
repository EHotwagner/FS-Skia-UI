# Recovery-None fallback + MapPointer invariance (SC-001, SC-005, FR-004, FR-005)

evidence-kind=live-adapter-dispatch
status=pass
authoritative=true
command=dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj
failure-class=product-defect

## Recovery-None → MapPointer (SC-005)

An unkeyed **unbound** leaf with no bound/keyed ancestor recovers `None`; the host then falls back to
`MapPointer` with the raw interaction, exactly as 090 — no spurious binding is invented:

- with a `MapPointer` clause mapping the raw `Click("0.0", …)`, the interaction routes through `MapPointer`.
- with **no** `MapPointer`, nothing dispatches (recovery stayed `None`; no invented id).

A directly-keyed leaf is a recovery **fixed point** (returns its own `Key`); an unkeyed-bound node is a
fixed point too (returns its own path). Tested in `us1-unkeyed-dispatch.md` and the property suite.

## MapPointer-only invariance (FR-005)

A consumer with **no** authored bindings anywhere is **bit-for-bit unchanged**: the raw interaction still
routes through `MapPointer` and produces exactly its mapped message. R3 is additive — the `BoundIds` set is
empty for such a consumer, so `nearestAuthored` returns `None` for every hit and the pre-R3 `MapPointer`
path is taken verbatim.

## Precedence preserved (FR-004 — no double-dispatch)

An authored binding **wins**; `MapPointer` is consulted **only** when recovery is `None` or no
click-equivalent binding matches — never both. The `disabledOrReadOnly` guard is preserved (a disabled
bound node does not dispatch).

## Tests (tests/Elmish.Tests/Feature098DispatchTests.fs)

- "US1 AS3: an unbound unkeyed leaf recovers None and falls back to MapPointer"
- "US2: a MapPointer-only consumer is bit-for-bit unchanged"
- "US1 AS1 / US2 AS3" — binding-wins / no-double-dispatch precedence.

result=Elmish.Tests 55/55 pass.
