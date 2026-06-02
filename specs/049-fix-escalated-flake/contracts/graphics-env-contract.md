# Contract: Deterministic Graphics-Environment Selection

**Scope**: Internal build front-end (`build/**`). No public package / `.fsi` surface
is involved. This contract governs how the compiled front-end and the processes it
spawns select a graphics backend.

## C1 — Backend selection is deterministic and self-applied

Given the ambient environment, the front-end MUST classify the display state and,
**only** when both a Wayland (`WAYLAND_DISPLAY`) and an X11 (`DISPLAY`) display are
advertised, force the X11 path:

| Condition | `WAYLAND_DISPLAY` | `GDK_BACKEND` | `SDL_VIDEODRIVER` |
|-----------|-------------------|---------------|-------------------|
| Wayland + X11 both present | **removed** | `x11` | `x11` |
| Wayland only | unchanged | unchanged | unchanged |
| X11 only | unchanged | unchanged | unchanged |
| neither | unchanged | unchanged | unchanged |

No operator action (env export, manual rerun) is required (FR-002, FR-006, SC-003).

## C2 — Normalization propagates to every spawned child

Every process the front-end launches — `dotnet test`, `dotnet fsi`, and nested
`bash ./fake.sh build -t <target>` (generated-product `Dev`/`Test`/`Verify`) — MUST
run under the normalized environment. This is guaranteed two ways:

1. **Inheritance**: the front-end normalizes its own process environment at startup,
   and children are spawned with `UseShellExecute=false`, so they inherit it.
2. **Edge re-application**: each child's `startInfo.Environment` is normalized at the
   spawn site, independent of inheritance (FR-003).

A verifiable assertion: a child launched by the front-end under a DualDisplay
ambient environment MUST observe **no** `WAYLAND_DISPLAY` and MUST observe
`GDK_BACKEND=x11`.

## C3 — Safety on already-working hosts

On headed Linux (single display) and non-Linux developer hosts the condition in C1
is false, so the front-end and all children behave **identically** to before this
feature, including identical visual output (FR-007, SC-004).

## C4 — Bounded failure, never an indefinite hang

A graphics-initializing child MUST complete within its existing timeout or be killed
at the bound. On a timeout kill, the front-end MUST emit a diagnostic that (a) names
a probable graphics-backend initialization failure as a candidate cause and (b)
points to `runtime-limitations.md`, so an environment failure is distinguishable
from a product regression (FR-005, SC-005).

## C5 — Real failures are never masked

The front-end MUST NOT rewrite, suppress, or ignore a child's nonzero exit code to
hide a failure. A genuine test/product failure MUST still surface as a failure
(FR-008, SC-006). FR-004 is satisfied by removing the teardown-crash *cause*, not by
exit-code manipulation.

## C6 — No target-surface change

No FAKE target is added, removed, or renamed; `validation.contract.yml`,
`TargetMetadata`, and `TargetMetadataDrift` outputs are unchanged. The escalated path
remains serialized and order-sensitive.
