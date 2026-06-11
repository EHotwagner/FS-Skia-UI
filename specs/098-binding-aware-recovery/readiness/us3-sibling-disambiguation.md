# US3 — same-kind unkeyed siblings disambiguate by path (SC-003, SC-004, FR-006, FR-007)

evidence-kind=fscheck-property + structural-agreement
status=pass
authoritative=true
command=dotnet test tests/Controls.Tests/Controls.Tests.fsproj
failure-class=product-defect

## Same-kind-sibling distinctness (≥1000 generated cases)

Two (or more) unkeyed same-kind bound siblings mint **distinct** structural ids (their paths `"0.0"` /
`"0.1"`, never a single shared `Kind` id) and route only to their own bindings — no collision, no
cross-routing. Property-tested over generated nested-Stack trees with same-kind Button leaves:

- **determinism** — `boundIdsOf` / `collectBoundsWith` / `eventBindingsOf` over the same tree produce
  identical `Bounds` ids, `EventBindings`, and `BoundIds` across runs (≥1000 cases).
- **distinctness** — any two distinct nodes have distinct canonical ids; the path scheme has no `Kind`
  collision (≥1000 cases).
- **concrete routing** — a Click targeted at the second of two unkeyed bound siblings dispatches **only**
  the second's message, and the first only the first's (no cross-routing).

## Single canonical scheme spans Bounds / EventBindings / BoundIds / recovery (SC-003)

For a laid-out node the id in `Bounds`, the id in `EventBindings` (when bound), the `BoundIds` membership
key, and the id `nearestAuthored` returns are **the same value** (`Key ?? path`) — no node reports `Kind`
from one surface and `path` from another. `render.BoundIds` is **populated** from its bound nodes while
`render.Bounds` stays `[]`.

## Tests (tests/Controls.Tests/Feature098UnifiedSchemeTests.fs)

- "property: boundIdsOf / Bounds / EventBindings are deterministic across runs (FR-006)" — 1000 cases
- "property: distinct unkeyed same-kind nodes get distinct canonical ids (SC-004)" — 1000 cases
- "two unkeyed same-kind bound siblings route only to their own binding (no cross-routing)"
- "single scheme spans Bounds / EventBindings / BoundIds / recovery (SC-003)"
- "render.BoundIds is populated while render.Bounds stays empty (FR-002)"

result=Controls.Tests 282/282 pass; read from the real suites, not assumed.
