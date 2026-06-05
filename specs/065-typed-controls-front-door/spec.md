# Feature Specification: Typed Controls Front Door

**Feature Branch**: `065-typed-controls-front-door`
**Created**: 2026-06-05
**Status**: Draft
**Input**: User description: "implement the first part of docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md"

## Overview

Controls today are authored through a weakly typed list of string-keyed
attributes (`Attr<'msg> list`). A typo in an attribute name, a missing required
value, or a wrong payload type is not caught until runtime — and sometimes not at
all. This feature introduces an additive, compiler-checked authoring surface so
control authors get the F# compiler as a guardrail: each control exposes an
immutable typed `Props` record (with defaults and, for stateful controls, an
`init`/`update` pair), and every typed authoring call lowers to the **same**
control representation the existing path produces. Nothing downstream of authoring
changes, and the existing string-keyed API keeps working untouched.

The scope is deliberately a **six-control reference slice** that exercises every
distinct authoring mechanic once. It is the first feature of a larger roadmap; the
remaining controls, design-token work, catalog regeneration, and reconciliation
are explicitly later, separate features.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Author a control with compile-time safety (Priority: P1)

A developer building a UI with `FS.Skia.UI.Controls` wants to add a button and
have the compiler catch mistakes (misnamed field, missing required value, wrong
event payload type) before the app runs.

**Independent test**: Author a `Button` through the typed surface, set its text,
intent, and click message via the typed record, and confirm (a) it compiles, (b)
introducing a wrong field type or wrong message type fails to compile, and (c) the
rendered button behaves identically to one authored the legacy way.

**Acceptance scenarios**:

1. **Given** the typed surface, **When** a developer writes a `Button` with a
   record literal that sets `Text`, `Intent`, and `OnClick`, **Then** it compiles
   and produces a working button.
2. **Given** the typed surface, **When** a developer assigns a value of the wrong
   type to a `Props` field (e.g. an `int` where a string is expected, or a message
   of the wrong `'msg` type to `OnClick`), **Then** the program fails to compile.
3. **Given** the typed surface, **When** a developer omits an optional field,
   **Then** the documented default from `defaults` is used.

### User Story 2 - Compose controls into a layout (Priority: P1)

A developer wants to nest typed controls inside a container (a `Stack`) and have
children be type-checked as part of the same surface.

**Independent test**: Build a `Stack` whose children include a `TextBlock` and a
`Button` authored through the typed surface, and confirm the composition compiles
and renders the children in order.

**Acceptance scenarios**:

1. **Given** the typed surface, **When** a developer puts typed control values into
   a `Stack`'s children, **Then** the composition compiles and renders the children.
2. **Given** an existing legacy control value, **When** a developer needs to drop it
   into a typed container during migration, **Then** a documented bridge accepts the
   legacy value into the typed children list.

### User Story 3 - Author a stateful control (Priority: P2)

A developer wants to use a control that owns ephemeral UI state (a text box with
validation, or a data grid with a visible range) through the typed surface, and
have its state transitions remain pure and testable.

**Independent test**: Author a `TextBox` through the typed surface, drive a state
transition through its `update`, and confirm the resulting state and effects match
the existing stateful-control behavior exactly (the typed surface delegates, it
does not fork the behavior).

**Acceptance scenarios**:

1. **Given** the typed `TextBox`/`DataGrid` surface, **When** a developer calls
   `init` from typed props, **Then** the initial state and effects equal those of
   the existing stateful control.
2. **Given** a state value and a message, **When** the typed `update` runs, **Then**
   the resulting state and effects equal the existing control's `update` result.

### User Story 4 - Existing code keeps working (Priority: P1)

A developer with an existing app built on the current string-keyed API upgrades to
the version that adds the typed surface and expects zero changes required.

**Independent test**: Compile and run the existing samples/tests that use the
legacy authoring API after the typed surface is added, and confirm no source
changes are needed and no behavioral difference appears.

**Acceptance scenarios**:

1. **Given** existing code using the legacy authoring API, **When** the typed
   surface is added, **Then** the existing code compiles unchanged.
2. **Given** the existing test suite, **When** it runs against the new version,
   **Then** every existing test passes with no behavioral diff.

### Edge Cases

- A typed control authored with only its defaults must lower to a valid control.
- A typed event callback that is left unset (no message) must lower to a control
  with no event binding — not a binding that dispatches a null/default message.
- Bridging a legacy control into a typed container, then lowering the container,
  must reproduce the original legacy control unchanged.
- The typed module names must not collide with or shadow the existing legacy
  per-control modules of the same name.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The Controls package MUST expose a new public typed wrapper type that
  every typed authoring call returns, declared in a curated public signature file.
- **FR-002**: The typed wrapper MUST be lowerable to the existing control
  representation through a single, explicit, documented accessor, and MUST be
  constructible from an existing control value (a bidirectional bridge) for
  migration.
- **FR-003**: Six controls MUST each expose a typed immutable `Props` record, a
  `defaults` value, and a `view`: `TextBlock`, `Button`, `CheckBox`, `TextBox`,
  `Stack`, `DataGrid`. The two stateful controls (`TextBox`, `DataGrid`) MUST also
  expose `init` and `update`.
- **FR-004**: Every typed `view` MUST lower to a control that is **structurally
  equal** to what the equivalent legacy authoring call produces today (verified by
  a per-control parity test). Attribute ordering MAY differ and MUST be normalized
  out of the comparison.
- **FR-005**: No typed `Props` field may carry an untyped/`obj` payload or a
  string-named event. Required values MUST be non-optional fields; optional values
  MUST get their values from `defaults`.
- **FR-006**: The two stateful typed controls MUST reuse the existing stateful
  models, messages, and effects — they MUST NOT introduce parallel state types.
  Their `init`/`update` MUST delegate to the existing behavior, not fork it.
- **FR-007**: The existing string-keyed authoring API (legacy create/attribute
  builders and all existing per-control modules) MUST remain unchanged and continue
  to compile and behave identically. The change MUST be additive-only at the public
  surface.
- **FR-008**: The typed event callbacks MUST produce the same event bindings the
  legacy path produces, such that dispatching them yields the same `'msg` (for
  command events) or applies the same payload mapping (for value-changed events).
- **FR-009**: Authoring through the typed surface MUST require no change to the
  existing Elmish adapter; a typed `view` is consumed by finishing with the lowering
  accessor.
- **FR-010**: The typed modules MUST be named so they do not collide with or shadow
  the existing legacy per-control modules.
- **FR-011**: The Controls package MUST NOT gain any new dependency as a result of
  this feature; in particular it MUST NOT take a dependency on an Elmish library.

> Interacting / conflicting requirements: FR-004 (structural parity) vs. FR-005
> (no `obj` in the typed surface) — where the legacy IR stores a value as an
> untyped payload, the typed surface MUST still expose a strongly typed field and
> lower it into that payload at the boundary; parity is asserted on the lowered IR,
> not on the typed field shape.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Active package path is `FS.Skia.UI.Controls`
  (`src/Controls/`, `Controls.fsproj`). Package **contents** change (new typed
  files added); package **identity** is unchanged; package **version** will bump as
  part of the additive surface release. No legacy Charts package migration is
  involved. No new package dependency is added (FR-011).
- **Public contract impact**: Yes — new public `.fsi` signatures are added (the
  typed wrapper type and module, plus six typed control modules). Existing `.fsi`
  signatures are unchanged. The package public-surface baseline changes
  (additive-only) and MUST be regenerated and reviewed in the diff.
- **State workflow impact**: No new stateful workflow, I/O, command, effect, or
  interpreter behavior is introduced. The typed `TextBox`/`DataGrid` façades reuse
  and delegate to the existing models/effects; updates stay pure (no I/O).
- **Layout/rendering impact**: No change to layout, rendering, screenshots, Skia,
  Vulkan, or diagnostics. Typed views lower to the same IR, so the existing render
  path is reused byte-for-byte. Render evidence is captured to prove no visual diff.
- **Evidence obligations**: Required real evidence paths —
  `specs/065-typed-controls-front-door/readiness/typed-controls-front-door.md` and
  `specs/065-typed-controls-front-door/readiness/package-surface-expectations.md`
  (both already named by the `controls-public-surface` routing rule). Supporting
  evidence: `readiness/typed-lowering-parity.md` and `readiness/controls-rendering.md`.
- **Unsupported scope**: Out of scope — design tokens / Penpot work; keyed VDOM
  diff / reconciliation; catalog regeneration from typed source; migrating the other
  41 controls; deprecating or removing the legacy API; any Elmish adapter
  convergence onto the typed wrapper. These are sequenced as later features
  (066–071+).
- **Build-target impact**: No build-target definitions change. Routing already
  escalates `src/Controls/**` to the `controls-public-surface` rule; the
  `Route`-printed gates (`ControlsCatalogCheck`, `ControlsInteractionCheck`,
  `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`,
  `GeneratedProductCheck`) apply, plus the serialized escalated order (`Dev`,
  `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`). The public-surface baseline regeneration is an
  expected, reviewed change.

## Success Criteria *(mandatory)*

- **SC-001**: A developer can author each of the six slice controls entirely through
  the typed surface, and a wrong field type or wrong event-message type is a
  compile error rather than a runtime fault.
- **SC-002**: For all six controls, the typed authoring call lowers to a control
  structurally equal to the legacy authoring call (100% parity across the six,
  verified by an automated parity test).
- **SC-003**: 100% of the existing Controls test suite passes unchanged, and the
  existing samples compile and run with no source edits, after the typed surface is
  added.
- **SC-004**: The Controls package gains zero new dependencies (verified by a
  dependency-governance guard).
- **SC-005**: `Route` over the branch diff prints the `controls-public-surface`
  escalation and every printed gate passes, including the public-surface check
  against the regenerated, intentionally-updated baseline.
- **SC-006**: Both routing-required evidence artifacts exist and are populated, so
  `Route --enforce` does not flag a missing artifact.

## Key Entities

- **Typed wrapper (`Widget`)**: the opaque public return type of every typed `view`;
  carries the lowered control representation and is the single seam to the existing
  IR.
- **`Props` record (per control)**: the immutable, compiler-checked authoring
  surface for one control — its fields drawn from a fixed taxonomy (identity,
  content, data, behavior, variant, layout, theme/style, accessibility, events).
- **`defaults` (per control)**: the canonical starting value for a control's `Props`,
  from which authors override only what they need.
- **Stateful model/msg/effect (reused)**: for `TextBox` and `DataGrid`, the existing
  state, message, and effect types that the typed façade delegates to.

## Assumptions

- The typed modules live under a distinct namespace segment (a `Typed` namespace) so
  the six typed modules keep clean names without shadowing the legacy modules
  (resolves naming collision; revisited in clarify).
- The typed wrapper is a **sealed wrapper** over the control IR (not a bare type
  alias), to leave room for later reconciliation metadata without a future surface
  break (revisited in clarify).
- The legacy API is **kept as a permanent peer** for this feature; any deprecation
  is a later, separate decision (revisited in clarify).
- The stateful typed controls **reuse** the existing models rather than introducing
  fresh ones (revisited in clarify).
- Lowering is **real** (parity-tested), so no synthetic-output disclosure marker is
  required for the typed views.
