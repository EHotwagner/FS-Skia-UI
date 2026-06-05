# Phase 0 Research: Typed Controls Front Door

All facts below were read from source on 2026-06-05, not reconstructed from prior
reports. Each decision resolves a `NEEDS CLARIFICATION` candidate (the five open
questions Q1–Q5 from the source report) into a committed direction already baked
into the spec's Assumptions.

## Grounding (verified current state)

| Fact | Evidence |
| --- | --- |
| Core IR is string-keyed; `Attr` has an `obj` escape hatch | `src/Controls/Types.fsi:231` (`Control<'msg>`), `:239` (`Attr<'msg>`), `:256` (`UntypedValue of obj`) |
| Legacy authoring entry points (per-control `*.create` + `Attr` builders) | `src/Controls/Control.fsi` — `Control.create`, `module Button`/`TextBlock`/`CheckBox`/`Stack`/`TextBox` |
| Stateful MVU shape already exists | `src/Controls/TextInput.fsi` (`TextInputModel`/`Msg`/`Effect`, `init`/`update`/`interpretEffect`), mirrored in `DataGrid.fsi` |
| Elmish split already correct | `Controls.fsproj` references only `Scene`, `Layout`, `KeyboardInput` — no `Fable.Elmish` |
| Routing pre-wired for this feature | `build/Governance/Routing.fs` rule `controls-public-surface`, paths `["src/Controls/**"]`, tier `FocusedAuthority`, names `readiness/typed-controls-front-door.md` + `readiness/package-surface-expectations.md` |
| Contract test scaffold present | `tests/Controls.Tests/TypedControlContractTests.fs` (currently asserts the `Standard*` surface) |

## Decision Q1 — Legacy `Attr`/`*.create` API: deprecate or keep?

**Decision**: Keep the legacy string-keyed API as a permanent peer for this
feature. No deprecation flag, no removal.

**Rationale**: `FS.Skia.UI.Controls` is shipped (v0.1.68-preview.1). FR-007/SC-003
require existing code to compile and behave identically with zero source edits.
Deprecation is a separate, later decision once the typed surface covers more than
the six-control slice.

**Alternatives considered**: Mark legacy `create` `[<Obsolete>]` now (rejected —
emits consumer warnings for a still-canonical API and pre-commits a policy this
feature has no mandate for).

## Decision Q2 — Typed module naming (avoid colliding with legacy `Button`/`TextBox`)

**Decision**: Typed modules live under a distinct `FS.Skia.UI.Controls.Typed`
namespace, keeping the clean names `Button`, `TextBox`, `Stack`, etc.

**Rationale**: The legacy `module Button`/`module TextBox` already exist in
`Control.fsi` in the base namespace. A separate namespace segment prevents
shadowing (FR-010, edge case "must not collide with or shadow") while letting the
typed surface read naturally (`Typed.Button.view`). `Widget` itself stays in the
base namespace because it is the shared lowering seam render and the adapter call.

**Alternatives considered**: `*Widget` suffix (`ButtonWidget`) — rejected as
noisier at every call site; shadowing legacy modules in the same namespace —
rejected (ambiguity, violates FR-010).

## Decision Q3 — `AdapterProgram.View` Widget overload now or later?

**Decision**: Wait. No adapter change this feature. Consumers finish a typed
`view` with `Widget.toControl`, and the existing `AdapterProgram.View: 'model ->
Control<'msg>` consumes the result unchanged.

**Rationale**: FR-009 requires no adapter change. Converging the adapter onto
`Widget`/`Cmd<'msg>` is feature 068. Keeping the adapter untouched holds the blast
radius to `src/Controls/**` and keeps the dependency split intact (the adapter
lives in `Controls.Elmish`, which owns `Fable.Elmish`; the base package must not).

## Decision Q4 — `Widget<'msg>`: sealed wrapper or bare alias?

**Decision**: Sealed wrapper over the lowered IR, not a type alias.

**Rationale**: A sealed type with an internal `{ Lowered: Control<'msg> }` record
field (a) leaves room for later keyed-reconciliation metadata (feature 067)
without a future public-surface break, (b) forces consumers through the explicit,
greppable `Widget.toControl` seam, and (c) keeps `Widget` and `Control` distinct
during the preview window. Principle II is satisfied: the `.fsi` exposes only the
sealed type and the module functions; the record field is private to the `.fs`.

**Alternatives considered**: `type Widget<'msg> = Control<'msg>` alias — rejected
(no room for provenance metadata; lowering seam disappears; an accidental
`Control` could be passed where a typed `Widget` is expected).

## Decision Q5 — Stateful controls: reuse existing models or fresh ones?

**Decision**: Reuse. Typed `TextBox` reuses `TextInputModel`/`TextInputMsg`/
`TextInputEffect`; typed `DataGrid` reuses `DataGridModel`/`DataGridMsg`/
`DataGridEffect`. Their typed `init`/`update` delegate to the existing
`TextInput.update`/`DataGrid.update`.

**Rationale**: FR-006 forbids parallel state types. Delegation keeps the typed
façade thin and guarantees behavioral identity (the interaction tests assert the
typed `update` result equals the existing control's `update` result). The edge
interpreter (`TextInput.interpretEffect`) is reused unchanged.

**Alternatives considered**: Fresh per-control typed models — rejected (doubles
the state surface, risks behavioral drift, violates FR-006).

## Cross-cutting: FR-004 (parity) vs FR-005 (no `obj`) tension

The legacy IR stores some values as `UntypedValue of obj`. The typed surface MUST
still expose a strongly typed `Props` field and lower it into that payload at the
boundary. Parity is asserted on the **lowered** `Control<'msg>` (after normalizing
attribute order), not on the typed field shape — so a typed field can be richer
than its lowered representation without breaking parity. This is the design rule
for every `view` implementation.

## Outcome

No `NEEDS CLARIFICATION` remains. All five decisions are reflected in the spec's
Assumptions and the design artifacts (data-model, contracts).
