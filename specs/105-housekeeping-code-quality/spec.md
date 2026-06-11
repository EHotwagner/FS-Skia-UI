# Feature Specification: Housekeeping Code-Quality Remediation

**Feature Branch**: `105-housekeeping-code-quality`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: `@docs/reports/2026-06-11-1424-housekeeping-code-quality-audit.md`

## Context & Source

This feature implements the low-risk, behavior-preserving batches of the
**Housekeeping Code-Quality Audit**
(`docs/reports/2026-06-11-1424-housekeeping-code-quality-audit.md`, generated
2026-06-11T14:24Z). The audit is a maintainability pass over `src/**` — not a
feature change. It concludes the codebase is healthy (signature-file coverage
~98%, equality discipline deliberate, no architectural rot); the findings are
**localized accumulation**: copy-paste helpers in the typed-widget lowering
layer, a thin layer of redundant access qualifiers the `.fsi` files already
enforce, and a handful of internal stringly-typed identifiers drawn from closed
sets.

Every item in scope is **behavior-preserving** and most are mechanical. The
deliverable is a cleaner, less duplicative, more type-checked internal source —
with **no change to any observable rendering output, public contract, or runtime
behavior**.

### Scope follows the audit's recommended sequencing

The audit explicitly tiers its findings by risk and recommends an order
(report §"Recommended Sequencing"). This feature takes the **three low-risk
batches** that stay within the framework-internal source and carry real safety
upside, and **defers** the higher-risk / contract-escalating items to their own
scoped passes:

**In scope (the three low-risk batches):**

1. **§1 Duplication** — collapse the 13 copy-paste lowering helpers
   (`withKeyOpt` ×9, `onString`/`onStringList` ×4+1) into one shared internal
   `WidgetLowering` module; collapse the 8 inline `onChanged` parsers in
   `Control.fs` into `onChangedBool` / `onChangedFloat` / `onChangedString`
   built on a named `tryParseFloat`; fold the smaller `intentStyle`→string and
   accessibility-metadata duplications into a shared helper.
2. **§3 Redundant access qualifiers** — drop the ~17 redundant in-source
   `private` keywords where the `.fsi` (or an enclosing `module internal`) is
   already the encapsulation boundary, keeping the explanatory comments.
3. **§5A Internal stringly-typed identifiers → DUs** — route internal
   attribute-name reads through a typed key (subsuming the §2.2/§2.3
   stringly-lookup smell), and introduce internal DUs for slot names, the Scene
   evidence stage/category, and the renderer-mode dispatch comparison.

**Out of scope (deferred per the audit's own risk tiering):**

- **§2.1 File splits** (`SkiaViewer.fs`, `Control.fs`, `Vulkan.fs`) — a separate,
  dedicated extraction pass; highest risk, touches hot paths, do last.
- **§5B `ControlId` single-case wrapper** and the **SkiaViewer public
  diagnostic/mode fields** — these cross the public `.fsi` surface, escalate to
  the maintainer-verify path, and ripple through `catalog.yml`/`ApiSurfaceGen`/
  byte-identity tests; each is its own scoped change.
- **§2.4 mutable-heavy / ref-threaded blocks** and **§4 custom equality on
  `AttrValue<'msg>`** — explicitly judgment calls / deliberately no-op; the audit
  recommends leaving them.
- **§5C keep-as-string identifiers** (`ControlKind`, public display/serialization
  strings, consumer metadata keys, `ControlEvent.Kind`) — deliberately open sets;
  no action.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - One source of truth for the lowering helpers (Priority: P1)

A maintainer extending or fixing the typed-widget lowering layer changes a helper
(e.g. the key-application or string-event adapter) **once** and every widget
module inherits the fix. The nine verbatim `withKeyOpt` copies, the four+ `onString`/
`onStringList` copies, and the eight inline `onChanged` parsers in `Control.fs`
no longer drift apart, because each is defined a single time in a shared internal
home and referenced everywhere else.

**Why this priority**: The audit calls this "the single highest-value,
lowest-risk cleanup in the report." Duplicated helpers are the place where a
future fix is most likely to be applied inconsistently; consolidating them is the
must-have outcome.

**Independent Test**: Grep the widget modules and `Control.fs` for the duplicated
helper bodies; confirm each helper body appears **once** (in the shared module /
at `Control.fs` module scope) and every former call site references it. Confirm
the lowered `Control<'msg>` produced for each affected widget is unchanged
(structurally equal) versus before the consolidation.

**Acceptance Scenarios**:

1. **Given** the typed-widget modules, **When** `withKeyOpt` is searched for,
   **Then** exactly one definition exists (in the shared `WidgetLowering`-style
   module compiled before the widget modules) and the nine former copies are gone.
2. **Given** the string-event adapters, **When** `onString` / `onStringList` are
   searched for, **Then** each is defined once in the same shared module and the
   four+ copies are gone.
3. **Given** the per-kind builders in `Control.fs`, **When** `onChanged` is
   searched for, **Then** the eight inline parsers are replaced by references to
   `onChangedBool` / `onChangedFloat` / `onChangedString`, and the float shape
   delegates to a named `tryParseFloat` (no 217-char inline lambda remains).
4. **Given** any widget whose lowering used a consolidated helper, **When** it is
   lowered after the change, **Then** the resulting `Control<'msg>` is
   structurally identical to the pre-change output.

### User Story 2 - The `.fsi` is the single, obvious visibility boundary (Priority: P2)

A contributor reading the source sees visibility expressed in exactly one place —
the `.fsi` (or the enclosing `module internal`) — without redundant `private`
keywords restating a boundary the signature file already enforces. The ~17 noise
qualifiers are gone, the explanatory comments that document *why* each module is
hidden remain, and no genuine encapsulation is weakened.

**Why this priority**: Pure noise reduction with zero functional change; valuable
for readability but lower-impact than removing duplication. The audit is explicit
that the load-bearing qualifiers (the `.fsi`-less `module internal SceneRenderer`,
the `InternalsVisibleTo` test seams, and the `let private` helpers inside the
*exposed* `ControlInternals`) must be **kept**.

**Independent Test**: For each of the ~17 cited sites, confirm the redundant
`private` is removed, the documenting comment is retained, and the module/member
remains invisible to external consumers (absent from the `.fsi` or inside a
`module internal`). Confirm the load-bearing qualifiers listed as "keep as-is" are
untouched.

**Acceptance Scenarios**:

1. **Given** the widget-lowering modules declared `module private <Name>Lowering`,
   **When** the change lands, **Then** the redundant `private` is dropped, the
   "file-scoped lowering helpers, hidden by `<X>.fsi`" comment remains, and the
   module stays absent from its `.fsi`.
2. **Given** the `let private` helpers inside already-`internal` modules
   (`Reconcile`, `RetainedRender`), **When** the change lands, **Then** the
   redundant `private` is dropped and the helpers remain internal (test seams
   intact via `InternalsVisibleTo`).
3. **Given** the load-bearing qualifiers (`module internal SceneRenderer`, the
   `InternalsVisibleTo` test seams, the `let private` helpers inside exposed
   `ControlInternals`), **When** the change lands, **Then** they are **unchanged**.

### User Story 3 - Internal identifiers from closed sets are type-checked (Priority: P2)

A contributor working the internal attribute-reader, slot, Scene-evidence, or
renderer-mode dispatch paths gets a **compile error** for a mistyped identifier
instead of a silent runtime miss. Internal attribute-name reads route through a
typed key, and the internal slot names, Scene evidence stage/category, and
renderer-mode comparison become DUs with exhaustive matches — all without moving
any public surface (the public ids that deliberately stay strings are untouched).

**Why this priority**: Real safety upside (typos become compile errors), but the
change is broader-touch than the §3 noise removal and must be done carefully to
stay behavior-preserving; lower priority than the duplication win. Subsumes the
§2.2 stringly-typed-lookup and §2.3 nested-lambda smells.

**Independent Test**: Confirm the internal attribute reads in `Control.fs` (and
`DataGrid.fs`) go through the typed key rather than raw string literals; confirm
the slot, Scene-stage/category, and renderer-mode internal comparisons are DU
matches; confirm the public-surface string identifiers called out as "keep"
(`ControlKind`, public diagnostic/mode output fields, consumer metadata keys,
`ControlEvent.Kind`) are unchanged; confirm rendering/evidence output is identical.

**Acceptance Scenarios**:

1. **Given** the internal attribute-name reads in `Control.fs`/`DataGrid.fs`,
   **When** the change lands, **Then** the closed control-intrinsic names
   (text/value/styleClasses/visualState/slot/orientation/width/height/…) are read
   through a typed key, and a mistyped internal name is a compile error.
2. **Given** the slot fills and `slotRegions`, **When** the change lands, **Then**
   the `"leading"`/`"trailing"`/`"header"`/`"footer"` strings are carried as an
   **internal** `SlotName` DU (no public `SlotName` surface is introduced — feature
   095's deliberate omission is preserved).
3. **Given** the Scene evidence `BlockedStage`/`DiagnosticCategory` and the
   renderer-mode dispatch comparison, **When** the change lands, **Then** each is
   an internal DU with an exhaustive match, with at most one parse-at-the-edge so
   the public output field stays a string.
4. **Given** the identifiers the audit lists as deliberately-string
   (`ControlKind`, public display/serialization output, consumer metadata keys,
   `ControlEvent.Kind`), **When** the change lands, **Then** they are **unchanged**.

### User Story 4 - Zero behavior, output, or contract change (Priority: P1)

A consumer's running app, every golden/parity artifact, and every existing gate
behave **exactly** as before. No rendering output changes, no parity/golden
evidence moves, no determinism property is perturbed, and the public `.fsi`
surface is unchanged except for any deliberate, baseline-recaptured DU expansion
explicitly recorded in the plan. This is a refactor: same observable behavior,
cleaner internals.

**Why this priority**: "Behavior-preserving" is the audit's banner constraint. A
housekeeping pass that accidentally perturbs output or contract would be a
regression and would defeat its own purpose. Preserving every invariant is a
must-have gate, equal in priority to US1.

**Independent Test**: Run the routed gate set; confirm the Controls and
Controls.Elmish suites stay green, parity/golden evidence is unchanged, and the
only surface delta (if any) is a recorded, baseline-recaptured DU expansion with
no consumer-observable effect.

**Acceptance Scenarios**:

1. **Given** the full housekeeping change, **When** the routed gates run, **Then**
   rendering output, parity evidence, and all existing property/unit suites are
   unchanged.
2. **Given** the consolidation and DU work, **When** a widget is lowered or a
   control is rendered, **Then** the produced `Control<'msg>` / scene is
   byte-/structurally identical to the pre-change output.
3. **Given** the change, **When** the public `.fsi` baselines are diffed, **Then**
   there is **zero** public-surface delta except for any deliberate DU expansion
   that the plan explicitly elects and recaptures.

### Edge Cases

- **Compile-order dependency for the shared module.** The new shared
  `WidgetLowering`-style module MUST be compiled **before** the widget modules in
  the Controls fsproj ordering; otherwise the references do not resolve. The plan
  fixes the fsproj insertion point.
- **`private` also hides from sibling modules in the same file.** Each cited file
  currently contains a single such module, so removal is safe **today**. The
  documenting comment is retained so a future second module in the same file does
  not silently gain access without a maintainer noticing.
- **Expanding a *public* DU is a surface change.** `StandardAttributeName`
  (`Types.fs:80`) is referenced by the public catalog `ControlSchema`. If the
  internal-reader typing is implemented by *adding cases to that public DU*, that
  is a deliberate public-surface addition requiring baseline recapture and
  escalated routing. The lower-risk alternative is an internal-only key type that
  leaves the public DU untouched; the plan selects and records which.
- **Internal DU with a string boundary.** The Scene-stage and renderer-mode DUs
  must keep their **public output/serialized** field a string (only the internal
  *comparison* is typed), parsing once at the edge — so no evidence text or public
  field format changes.
- **Annotations/comments must not be load-bearing.** Retained or added comments
  must be purely descriptive and MUST NOT be parsed by any governance gate as a
  status/behavior token (e.g. literal filenames or bare gate tokens that trip the
  window-visibility or diff-scan audits).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The verbatim `withKeyOpt` helper currently duplicated in nine
  widget-lowering modules MUST be defined **once** in a single shared internal
  module (compiled before the widget modules) and referenced by every former
  call site; the nine copies MUST be removed.
- **FR-002**: The `onString` (and `CollectionsWidgets`' `onStringList`)
  string-event adapter, duplicated across four+ modules, MUST be folded into the
  same shared internal module and referenced everywhere; the copies MUST be
  removed.
- **FR-003**: The eight inline `onChanged` parsers in `Control.fs` MUST be
  replaced by three module-scope helpers — `onChangedBool`, `onChangedFloat`,
  `onChangedString` — and the float shape MUST delegate to a named
  `tryParseFloat : string -> float option`, eliminating the 217-char inline
  lambda with the nested `match Double.TryParse`.
- **FR-004**: The smaller duplications SHOULD be folded in opportunistically — the
  `intentStyle` enum→string mapping (`Primitives.fs`/`Input.fs`) promoted to one
  shared `intentToString`, and the near-identical accessibility-metadata builder
  (`Buttons.fs`/`Pickers.fs`) extracted to a shared `a11y` helper — provided each
  stays behavior-preserving.
- **FR-005**: The ~17 redundant in-source `private` keywords cited in §3 (the
  `module private <Name>Lowering` declarations and the `let private` helpers
  inside already-`internal` `Reconcile`/`RetainedRender`) MUST be removed, and the
  explanatory "hidden by `<X>.fsi`" comments MUST be retained.
- **FR-006**: The load-bearing qualifiers MUST be **preserved unchanged**: the
  `.fsi`-less `module internal SceneRenderer` (exhaustiveness guard), every
  `InternalsVisibleTo`-backed `internal` test seam (`Reconcile`, `RetainedRender`,
  `ControlInternals`, `ControlRuntime`, the `ControlsElmish` internals), and the
  `let private` helpers inside the *exposed* `ControlInternals`.
- **FR-007**: The internal attribute-name reads in `Control.fs` (and
  `DataGrid.fs`) MUST be routed through a typed key for the closed
  control-intrinsic names (text/value/styleClasses/visualState/slot/orientation/
  width/height/…) so a mistyped internal name becomes a compile error rather than
  a silent runtime miss (subsuming §2.2/§2.3).
- **FR-008**: The internal slot names MUST be carried as an **internal** `SlotName`
  DU over the closed `leading`/`trailing`/`header`/`footer` set, keeping the
  carrier internal — **no** public `SlotName` surface is introduced (feature 095's
  deliberate omission is preserved).
- **FR-009**: The internal Scene evidence `BlockedStage`/`DiagnosticCategory`
  strings and the renderer-mode dispatch comparison MUST each become an internal
  DU with an exhaustive match, parsing at most once at the edge so that every
  **public** output/serialized field remains a string with an unchanged format.
- **FR-010**: The identifiers the audit designates keep-as-string MUST be
  **unchanged**: `ControlKind` (deliberately open via `StandardControlKind` +
  `Custom of string`), the public display/serialization output fields in
  `SkiaViewer.fsi`, consumer metadata keys (e.g. DataGrid `columnKey`/`rowKey`),
  and `ControlEvent.Kind`.
- **FR-011**: The change MUST NOT alter any observable rendering output,
  parity/golden evidence, determinism property, or runtime behavior. The lowered
  `Control<'msg>` for every affected widget and the rendered scene MUST be
  byte-/structurally identical to the pre-change output.
- **FR-012**: The change MUST NOT change the public `.fsi` surface **except** for a
  deliberate, explicitly-recorded DU expansion (e.g. adding cases to the public
  `StandardAttributeName`) that the plan elects; if elected, the affected
  cross-package and per-package surface baselines MUST be recaptured and the route
  escalated accordingly. The default, lower-risk choice is an internal-only key
  type with **zero** public-surface delta.
- **FR-013**: The deferred items MUST remain out of scope: the §2.1 file splits,
  the §5B `ControlId` wrapper and SkiaViewer public diagnostic/mode fields, the
  §2.4 mutable-heavy refactors, and the §4 `AttrValue<'msg>` custom-equality
  change. They are named here only to bound scope.
- **FR-014**: Any comment or annotation retained or added MUST be purely
  descriptive and MUST NOT be interpreted by any governance gate as a
  status/behavior token that alters routing, evidence verdicts, or audit outcomes.

> Interacting / conflicting requirements: FR-007 (route internal reads through a
> typed key) and FR-012 (no public-surface change) can pull opposite directions if
> the typed key is implemented by **expanding the public `StandardAttributeName`
> DU**, which adds public cases. **Resolution**: FR-011/FR-012's behavior- and
> contract-preservation is the banner constraint — the **default** is an
> internal-only key type that leaves the public DU untouched (zero surface delta).
> Expanding the public DU is permitted only as a deliberate choice recorded in the
> plan, with baselines recaptured and routing escalated. Likewise FR-005
> (drop redundant `private`) is bounded by FR-006 (keep load-bearing qualifiers):
> only the ~17 sites the audit certifies redundant are touched; the keep-list is
> never modified.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** This
> section deliberately names concrete source files, `.fsi`/baseline surfaces,
> build targets, and evidence paths.

- **Package impact**: No package-identity change. In-source edits touch
  **FS.Skia.UI.Controls** (`src/Controls/Widgets/*.fs`, `src/Controls/Control.fs`,
  `src/Controls/Reconcile.fs`, `src/Controls/RetainedRender.fs`,
  `src/Controls/DataGrid.fs`, `src/Controls/Types.fs`, plus a new shared
  `WidgetLowering`-style module) and **FS.Skia.UI.Scene** / **FS.Skia.UI.SkiaViewer**
  (`Scene.fs`, `SkiaViewer.fs`) for the internal Scene-stage / renderer-mode DUs.
  The standard post-merge version bump of all packable libs applies per the merge
  flow. No legacy Charts migration.
- **Public contract impact**: **No public `.fsi` signature change expected** under
  the default choices (shared module + helpers are internal; the typed
  attribute-key is internal-only; the new DUs are internal with string boundaries).
  The **only** possible public delta is the optional FR-012 expansion of the public
  `StandardAttributeName` DU — if elected, recapture the affected cross-package and
  per-package surface baselines and route Tier-1 accordingly. No sample contract
  changes.
- **State workflow impact**: None. No commands, effects, subscriptions,
  interpreters, or stateful-workflow behavior change.
- **Layout/rendering impact**: **None observable.** Lowering output, layout,
  visual state, charts, DataGrid, Vulkan, Skia, screenshots, and
  unsupported-environment diagnostics are unchanged; the renderer-mode/Scene-stage
  DUs keep their serialized output strings byte-identical.
- **Evidence obligations**: (1) the routed gate set green; (2) parity/golden
  evidence unchanged (no row moves); (3) the Controls and Controls.Elmish property
  and unit suites still green; (4) byte-/structural identity of the lowered
  `Control<'msg>` for affected widgets demonstrated; (5) standard `EvidenceGraph` +
  `EvidenceAudit` artifacts with a verdict token and no synthetic tasks.
- **Unsupported scope**: the §2.1 file splits (`SkiaViewer.fs`/`Control.fs`/
  `Vulkan.fs`); the §5B `ControlId` wrapper and SkiaViewer public diagnostic/mode
  field conversions (consumer-contract changes, separate features); the §2.4
  mutable-heavy refactors; the §4 `AttrValue<'msg>` custom-equality change; any
  change to `ControlKind` or the other deliberately-string public identifiers.
- **Build-target impact**: Run `./fake.sh build -t Route` first and run only the
  gates it prints. The audit predicts the `src/Controls/**` and `Control.fs`
  changes route to the light inner-loop tier (`Dev`) "unless a public `.fsi` is
  edited"; **note** that prior features (101/102) observed that *any*
  `src/Controls/**/*.fs` edit can escalate to the `controls-public-surface`
  maintainer-verify path even with no `.fsi` delta — so confirm via `Route` and be
  prepared for the escalated set. If the optional FR-012 DU expansion is taken, the
  route escalates and per-package/cross-package baselines are recaptured
  (`PerPackageSurface.captureCurrent` + the standard cross-package capture). No new
  gate is added.

## Success Criteria *(mandatory)*

- **SC-001**: The duplicated `withKeyOpt` (9 copies), `onString`/`onStringList`
  (4+1 copies), and `onChanged` (8 inline parsers) are reduced to **one** shared
  definition each — verified by grep showing a single body per helper and every
  former site referencing it; ~50 lines of duplication removed.
- **SC-002**: No 217-char inline float-parse lambda remains in `Control.fs`; the
  float `onChanged` path delegates to a single named `tryParseFloat`.
- **SC-003**: The ~17 redundant `private` keywords cited in §3 are removed with
  their explanatory comments retained, and **all** load-bearing qualifiers on the
  audit's keep-list are unchanged — verified by inspecting each cited site.
- **SC-004**: Internal attribute-name reads, slot names, Scene evidence
  stage/category, and renderer-mode dispatch are type-checked (typed key / DUs),
  such that a mistyped internal identifier is a **compile error** — demonstrated by
  the code compiling only against the closed set.
- **SC-005**: **Zero** change to observable rendering output, parity/golden
  evidence, or determinism properties; the Controls and Controls.Elmish suites are
  green and unchanged — confirmed by the routed gate set with `EvidenceAudit`
  reporting no synthetic work.
- **SC-006**: The lowered `Control<'msg>` for every affected widget is
  byte-/structurally identical before and after — demonstrated by a parity check
  over the consolidated helpers.
- **SC-007**: The public `.fsi` surface is unchanged **except** for the optional,
  explicitly-recorded FR-012 DU expansion; if that expansion is **not** taken,
  there is **zero** public-surface delta and no baseline recapture is required.
- **SC-008**: The keep-as-string and deferred items (FR-010, FR-013) are
  demonstrably untouched — no `ControlKind`, public output-field, consumer-metadata,
  `ControlEvent.Kind`, file-split, `ControlId`-wrapper, mutable-block, or
  custom-equality change appears in the diff.

## Assumptions

- The audit's file:line citations were spot-verified against the working tree at
  the time of writing; the plan re-verifies them before editing, since line
  numbers may have shifted.
- The audit's **recommended sequencing** is the authoritative scope guide: the
  three low-risk batches (§1, §3, §5A) are in scope; the file splits (§2.1),
  `ControlId` wrapper / public-field conversions (§5B), mutable-block refactors
  (§2.4), and custom-equality change (§4) are deferred to their own passes.
- The **default** implementation choices are the zero-surface-delta ones: the
  shared helper module and the typed attribute-key are internal; the new slot /
  Scene-stage / renderer-mode DUs are internal with string boundaries. Expanding a
  public DU is taken only with explicit justification and baseline recapture.
- "Behavior-preserving" means byte-/structural identity of the lowered
  `Control<'msg>` and of rendered output; the enforcement is the existing,
  unchanged gate suite plus a parity check over the consolidated helpers — this
  feature adds no new gate and no new public property.
- The shared lowering module's compile position in the Controls fsproj is the one
  ordering constraint introduced; everything else is reference-rewiring within
  existing compilation units.

## Out of Scope

- **§2.1 File splits** of `SkiaViewer.fs`, `Control.fs`, and `Vulkan.fs` — a
  separate, dedicated extraction pass with stable `.fsi` surfaces; highest risk,
  scheduled last by the audit.
- **§5B `ControlId` single-case wrapper** (`type ControlId = string` →
  `ControlId of string`) — public, escalates, threads through `Key`/event-binding/
  positional-path; its own scoped change.
- **§5B SkiaViewer public diagnostic/mode field conversions** (`DiagnosticClass`,
  `ViewerLaunchOutcome.Mode`) — consumer-contract break for modest gain; only if
  that `.fsi` is revised for another reason.
- **§2.4 mutable-heavy / ref-threaded refactors** (`Testing.fs` pixel-analysis,
  `SkiaViewer.fs` Elmish ref cells, `KeyboardInput.fs` `parseYaml`) — judgment
  calls flagged for awareness, not urgent.
- **§4 `AttrValue<'msg>` custom equality** — the audit recommends **leave as-is**;
  the current explicit `attrValueEqual` is the cleaner design for a generic type.
- **§5C keep-as-string identifiers** — `ControlKind`, public display/serialization
  strings, consumer metadata keys, `ControlEvent.Kind`; deliberately open sets.
- **Enabling any new behavior**, adding any public API, or migrating the full
  control set — this is a behavior-preserving internal cleanup only.
