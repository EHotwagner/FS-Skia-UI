# Phase 0 Research: Binding-Aware Ancestor Recovery (R3)

All spec-level NEEDS CLARIFICATION were resolved in the spec's Clarifications
session (2026-06-11). This file records the **internal mechanism** decisions the
spec deliberately leaves to the plan, each grounded in the current code.

## D1 — Canonical id scheme: `Key ?? structural-path`

**Decision**: Adopt `Key |> Option.defaultValue path` (the positional
`parent + "." + index` path, root `"0"`) as the single per-node `ControlId` across
`Bounds`, `EventBindings`, `BoundIds`, and `nearestAuthored` recovery. Keyed nodes are
unchanged; only the unkeyed fallback shifts `Kind → path`.

**Rationale**: It is already the id the Click interaction carries (the hit id from
`Layout.evaluate ... rendered.Layout`, whose `LayoutNode.Id = Key ?? path` via `toLayout`)
and the id `nearestAuthored` re-derives. The divergent half is `eventBindings`
(`Control.fs:194`, `Key ?? Kind`) and `collectBoundsWith`'s emitted `controlId`
(`:1332`, `Key ?? Kind`) — note `collectBoundsWith` already computes `layoutId = Key ?? path`
(`:1331`) for the bounds **lookup**, so the fix is literally to emit `layoutId` instead of
the separate `Kind`-based `controlId`. Unifying onto the path the dispatch path already
uses is the minimal, internally-consistent change.

**Alternatives considered**:
- *Keep `Kind` and teach recovery to return `Kind`*: rejected — `Kind` collides for
  same-kind siblings (two unkeyed `button`s both mint `"button"`), so it cannot
  disambiguate; it would carry the collision into recovery.
- *A fresh synthetic GUID per node*: rejected — not resume-stable, not derivable by the
  recovery walk from positional structure, and gratuitous churn to the payload.

## D2 — Threading the path into binding collection

**Decision**: Make binding collection **path-aware** by mirroring `collectBoundsWith`'s
inner `go (path) (c)` recursion (`Control.fs:1330`). `eventBindings` gains a `path`
parameter and derives `id = Key ?? path`; `eventBindingsOf` becomes a `go "0"` walk that
threads `path + "." + index` into each child (replacing today's path-free
`recursively eventBindings`). The same walk feeds the new `boundIdsOf`.

**Rationale**: `eventBindings` today has no path context (`Key ?? Kind` needs none). The
positional path is structural state that only the *walk* knows, so the id must be derived
during the walk, exactly as `collectBoundsWith` already does for bounds. Reusing that
walk's path discipline guarantees `EventBindings`, `Bounds`, and `BoundIds` agree
node-for-node (SC-003).

**Alternatives considered**:
- *Post-hoc remap `Kind`→`path` after a path-free collect*: rejected — impossible without
  re-walking for the path; just thread the path in the first place.
- *Carry the path on `Control<'msg>`*: rejected — pollutes the authored value type with
  derived render state; the path is a property of position in a specific tree, not of the
  control.

## D3 — `boundIdsOf` placement and shape

**Decision**: Add `ControlInternals.boundIdsOf : Control<'msg> -> Set<ControlId>` — a
`go "0"` walk collecting `Key ?? path` for every node where
`eventBindings path control` is non-empty. Surface it in `Control.fsi`'s internal
`ControlInternals` block (like `collectBoundsWith`/`eventBindingsOf`) so all four
`ControlRenderResult` construction sites (`render`, `renderTree`, the two
`RetainedRender` frames) populate `BoundIds` from one source — the retained path stays
byte-identical to the full rebuild by construction.

**Rationale**: A `Set<ControlId>` matches the spec's `BoundIds : Set<ControlId>` field and
gives recovery an O(log n) membership test. "Bound" = `eventBindings` non-empty (the
spec's definition: at least one `Event`-category attr lowering to a
`MessageValue`/`EventValue`). Computing it from the *same* path-aware `eventBindings`
keeps `BoundIds` and `EventBindings` in lockstep.

## D4 — `nearestAuthored` widening

**Decision**: Read `result.BoundIds` inside `nearestAuthored`. At each node compute its
canonical id `cid = if node.Id <> path then node.Id else path` (i.e. `Key ?? path` —
`node.Id` already equals `Key ?? path`, so `cid = node.Id`); treat the node as authored
when `node.Id <> path` (keyed) **OR** `Set.contains cid result.BoundIds` (bound). Return
the nearest authored ancestor (including self); `None` when none on the path qualifies.

**Rationale**: `LayoutNode.Id` is already `Key ?? path`, so `node.Id` *is* the canonical
id at that node — no extra derivation needed; the only new input is `BoundIds` membership.
A directly-keyed leaf stays a fixed point (its hit id is its `Key`, `node.Id = hit`,
authored-here true). The unkeyed-bound node now returns `Some node.Id` (its path) where it
returned `None` before. This is a one-predicate widening of the existing walk — no
control-flow restructure.

**Alternatives considered**:
- *Thread `boundIds` as a new argument to `nearestAuthored`*: rejected by the spec's
  Clarification (a field on `ControlRenderResult`, read from the result the recovery walk
  already takes — no new threading at every call site).

## D5 — `Control.dispatch` consistency (the one decision the spec doesn't name)

**Decision**: Thread the path into `Control.dispatch` (`Control.fs:1480`) too, so its
per-node `event.ControlId = Some binding.ControlId` matching uses the unified `Key ?? path`
scheme — eliminating the last residual `Key ?? Kind` derivation in the codebase.

**Rationale**: `dispatch` calls `ControlInternals.eventBindings current` (`:1486`); once
`eventBindings` is path-aware, `dispatch` must supply the path. FR-001 requires the
`Key ?? Kind` scheme be **replaced**, not merely shadowed — leaving `dispatch` on `Kind`
would mean the same node has a `Kind`-keyed binding here and a `path`-keyed binding in the
render result, the exact divergence R3 removes. The existing keyed regression suite
(`InteractionTests.fs`, every case uses the key `"save-button"`) is unaffected because the
keyed branch (`Key` is the id) is byte-identical; the `event.ControlId = None` wildcard
path is also unchanged. No public-payload change for keyed `dispatch` consumers.

**Verification**: `InteractionTests.fs` (8 keyed cases + typed parity) must stay green
unchanged; no test passes an unkeyed `Kind` id to `dispatch` today.

## D6 — `render` (single-control preview) participation

**Decision** (from spec Clarification): `render` adopts the unified `Key ?? path` scheme
for its `EventBindings` and emits a **populated** `BoundIds` (from its bound nodes),
mirroring its already-populated `EventBindings`. `render.Bounds` stays `[]` (unchanged).

**Rationale**: Keeps the single-canonical-scheme invariant (SC-003) true on *every*
surface, not only the live dispatch path, while preserving the preview's deliberate
"no hit-testable geometry" contract (`Bounds = []`). The live dispatch path uses
`renderTree`, so this is about invariant consistency, not new dispatch capability.

## D7 — Focus path is out of scope (FR-008)

**Decision**: Do not touch `resolveFocus`/`RetainedRender.retainedHitTest`/`RetainedId`.

**Rationale**: The 092 retained focus path is a separate, already-working `RetainedId`
domain. R3 corrects only the 090 `Layout.evaluate` + `nearestAuthored` + `EventBindings`
dispatch seam. A `focus-nonregression` artifact proves focus resolution is unchanged.

## Open items

None. All four spec clarifications are settled, and D2/D3/D5 settle the internal mechanism.
The change is data-only (id derivation + one widened predicate + one new set field); no
layout math, no Scene/pixel change, no new dependency.
