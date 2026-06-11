# Phase 1 Data Model: Binding-Aware Ancestor Recovery (R3)

R3 introduces no new domain type — it corrects one id derivation, adds one set field,
and widens one predicate. The "entities" below are the conceptual data shapes the change
turns on.

## Canonical `ControlId`

- **Definition**: `Key |> Option.defaultValue path`, where `path` is the positional
  `parent + "." + index` structural path (`toLayout`/`collectBoundsWith` mint it; root `"0"`).
- **Replaces**: the divergent `Key |> Option.defaultValue Kind` used today by
  `eventBindings` (`Control.fs:194`) and `collectBoundsWith`'s emitted `controlId` (`:1332`).
- **Invariant (SC-003)**: for any node, the id in `Bounds`, the id in `EventBindings`, the
  ids in `BoundIds`, and the id `nearestAuthored` returns are **the same value**.
- **Keyed nodes**: unchanged — the id is the `Key`.
- **Unkeyed nodes**: id changes `Kind → path` (the documented payload canonicalization,
  FR-007).
- **Distinctness (SC-004)**: two same-kind unkeyed siblings get distinct ids (their
  distinct paths `"0.0"`, `"0.1"`), removing the `Kind`-keyed collision.

## `BoundIds : Set<ControlId>`

- **New field** on `ControlRenderResult<'msg>` (Types.fsi `:345`, Types.fs `:287`).
- **Population rule**: the set of canonical ids of nodes whose
  `ControlInternals.eventBindings` is non-empty (a node carrying ≥1 `Event`-category attr
  lowering to a `MessageValue`/`EventValue` binding).
- **Scheme**: the *same* `Key ?? path` scheme as `EventBindings`, so a recovered id is a
  direct membership/lookup key.
- **Emitted by**: `Control.render`, `Control.renderTree`, and both `RetainedRender`
  frames — all via `ControlInternals.boundIdsOf`.
- **`render` specifics**: `render.BoundIds` is **populated** (mirrors its populated
  `EventBindings`); `render.Bounds` stays `[]`.

## Binding-aware recovery (`nearestAuthored`)

- **Signature**: unchanged — `result: ControlRenderResult<'msg> -> hit: ControlId -> ControlId option`.
- **Predicate (widened)**: a node is *authored* when it **carries a `Key`** (`node.Id <> path`)
  **OR** its canonical id is in `result.BoundIds`.
- **Result**: the nearest authored ancestor (including self) on the hit node's path;
  `None` only when nothing on the path is keyed or bound.
- **Fixed points (non-regression, FR-005)**:
  - directly-keyed leaf → returns its own `Key`;
  - container-keyed composite + inner unkeyed-unbound hit → climbs to the container `Key`;
  - unkeyed-unbound leaf with no keyed/bound ancestor → `None` (host → `MapPointer`).
- **New behavior**: unkeyed-bound node → returns `Some path` (was `None`).

## Dispatch lookup (`bindingMessagesFor`)

- **Unchanged logic** (`ControlsElmish.fs:155`): recover via `nearestAuthored`; if `Some
  authored`, filter `EventBindings` by `ControlId = authored` and click-equivalent
  `EventKind` (`click`/`changed`/`selected`); dispatch the matches; otherwise (or on
  `None`) fall back to `MapPointer`.
- **Why it now works**: the recovered id and the `EventBindings` keys share the unified
  scheme, so the unkeyed-bound id matches its binding. **Precedence preserved**: authored
  binding wins; `MapPointer` consulted only when recovery is `None` or no click-equivalent
  binding matches — never both (no double-dispatch).
- **`disabledOrReadOnly` guard**: preserved — a disabled bound node does not dispatch.

## `Control.dispatch` (standalone by-Control API)

- Threads the path so its `event.ControlId = Some binding.ControlId` matching uses the
  unified scheme (D5). Keyed callers and the `event.ControlId = None` wildcard are
  unchanged.

## State transitions

None. All of the above are **pure, total, deterministic** functions over an
already-computed `ControlRenderResult`/`Control<'msg>` tree — no clock, no randomness,
resume-safe. No `Model`/`Msg`/`Effect` is added.

## Property-test obligations (FsCheck, ≥1000 cases — SC-004)

- **Determinism**: `boundIdsOf`/`collectBoundsWith`/`eventBindingsOf` over the same tree
  produce identical results across runs.
- **Same-kind-sibling distinctness**: any two distinct unkeyed same-kind nodes have
  distinct canonical ids.
- **Single-scheme agreement**: for every laid-out node, its `Bounds` id = its
  `EventBindings` id (when bound) = its `BoundIds` membership key.
