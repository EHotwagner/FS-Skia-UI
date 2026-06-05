# Implementation Plan: Internal Keyed Reconciliation

**Branch**: `067-keyed-reconciliation` | **Date**: 2026-06-05 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/067-keyed-reconciliation/spec.md`

## Summary

Add a **pure, internal** keyed VDOM diff over the lowered `Control<'msg>` IR: a
total function `diff : prev -> next -> ReconcileResult<'msg>` that matches sibling
children by their stable `Control.Key` (falling back to deterministic positional
matching for unkeyed nodes), and emits a `NodePatch<'msg>` describing the minimal
set of operations (keep, targeted update, insert, remove, move, whole-subtree
replace) that transform `prev` into `next`. A companion `apply : prev -> patch ->
Control<'msg>` exists solely to prove the **round-trip invariant** (`apply prev
(diff prev next).Patch ≡ next`) under FsCheck.

The deliverable is the data structure + algorithm only. It is **internal-only**
(`module internal FS.Skia.UI.Controls.Reconcile`), adds **zero** public API, does
not change the `Control<'msg>` IR, and is **not wired into** the render path. A
later feature (incremental rendering / `Widget` reconciliation metadata, sealed in
`065` §3.2) will consume it.

**Technical approach**: one new compile unit pair `src/Controls/Reconcile.fsi` /
`Reconcile.fs`, inserted after `Control.fs`, depending only on `Types`/`Control`.
The module is `internal` (genuine assembly-internal accessibility, matching the
existing `module internal SceneRenderer` precedent), so the public api-surface is
byte-for-byte unchanged and `PackageSurfaceCheck` sees no delta. Property tests
reach it via a single `[<assembly: InternalsVisibleTo("Controls.Tests")>]`.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (matches `Controls.fsproj`)
**Primary Dependencies**: none new. The reconciler depends only on the existing
`FS.Skia.UI.Controls` `Types`/`Control` modules; **no `Fable.Elmish`**, no
renderer dependency (FR-013). Tests add an FsCheck `<PackageReference>` to
`Controls.Tests.fsproj` (version already pinned in `Directory.Packages.props`:
`FsCheck 3.3.3`) — a test-only reference, not a product dependency.
**Testing**: Expecto (existing harness) + FsCheck for the round-trip and
determinism properties; new `tests/Controls.Tests/ReconcileTests.fs`. Failing-first.
**Target Platform**: Windows and Linux (pure F#; no platform/Skia/Vulkan surface).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification**: **Tier 2 (internal change)** — adds no public API
surface, introduces no dependency, changes no inter-project contract or
existing-spec behavior. Per Change Classification, `.fsi` contracts and surface
baselines remain untouched. (The reconciler *does* gain a curated `Reconcile.fsi`,
but it is declared `module internal` and is **not** added to the package's
api-surface contract list, so the public surface is unchanged — see
`.fsi` / contract impact below.)

**Principle alignment**:
- **I (Spec → FSI → Tests → Impl)**: the internal contract is sketched as
  `contracts/reconcile.fsi` and exercised by failing-first tests before the `.fs`
  body. Because the surface is internal, the "FSI transcript through the packed
  library" sub-requirement is satisfied by in-assembly Expecto tests reaching the
  internal module via `InternalsVisibleTo`, not by a public FSI session (there is
  no public symbol to load).
- **III (Idiomatic simplicity)**: pure functions over records + DUs; no SRTP, no
  type providers, no custom operators, no non-trivial computation expressions. The
  matching pass may use a `mutable` accumulator / `for` loop where it reads plainer
  than a fold (disclosed at the use site per Principle III) — keyed-bucket
  construction is the one candidate. No justification-requiring feature is used.
- **IV (MVU boundary)**: **N/A — pure, stateless diff.** The reconciler owns no
  durable state, performs no I/O, and emits no effects; it is a pure function over
  immutable IR, so the Model/Msg/Effect/update boundary does not apply.
- **V (Synthetic evidence)**: **none planned.** The diff and `apply` are real; the
  round-trip property runs real generated inputs through both. No `[S]`/`[SEH]`
  disclosure is anticipated. The evidence file states this explicitly.
- **VI (Test evidence)**: failing-first unit tests (US1–US4 + edge cases) and
  FsCheck properties (round-trip ≥1000 cases, determinism) that fail before the
  `.fs` body exists and pass after.
- **VII (Observability)**: duplicate-key input surfaces a `KeyCollision`
  `ControlDiagnostic` (existing `ControlDiagnosticCode.KeyCollision`) rather than
  failing silently or throwing; the function is total (FR-011, SC-007).

### Repository Governance Decisions

- **Template ownership**: **N/A — no template change.** The reconciler is internal
  framework code under `src/Controls/**`; it adds no source/docs/samples/Spec-Kit
  asset/command-surface the generated template ships, so
  `.template.config/template.json` and `template/capabilities.yml` are untouched.
  (Deliberately: the new `Reconcile.fsi` is **not** added to the Controls
  capability `contracts:` list, keeping the emitted api-surface unchanged.)
- **Dependency impact**: **N/A — no product dependency change.**
  `Directory.Packages.props` already pins `FsCheck 3.3.3`; this feature only adds a
  test-project `<PackageReference Include="FsCheck" />` to `Controls.Tests.fsproj`.
  No new entry in `docs/dependencies.md`, no generated-template inclusion, and
  `DependencyReport` coverage is unchanged (test-only references are not product
  dependencies). `Controls.fsproj`'s reference set (`Scene`, `Layout`,
  `KeyboardInput`) is unchanged — in particular **no `Fable.Elmish`** (FR-013).
- **Command-surface impact**: **No build-target semantics change.** No edit to
  `build.fsx`/`build/Governance/**`, `Dev`, `Verify`, `Ci`, `TemplateCheck`,
  `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`,
  or `EvidenceAudit`. `Route` already escalates `src/Controls/**` to the
  `controls-public-surface` gate set (`Routing.fs:128`); no routing edit is needed.
  FAKE-backed commands share `.fake` state and are **not** safe to run
  concurrently — run them sequentially in the deterministic order below; safe
  non-FAKE reads/checks may still parallelize. Example order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t Verify`
- **Generated project impact**: **N/A — no generated-product change.** Default and
  minimal generated contents, selected-Controls guidance, local skills, validation
  logs, placeholder/excluded-history scans, and generated `Dev` behavior are all
  unchanged; the reconciler is unreachable from generated products (it is
  assembly-internal and unwired).
- **Evidence paths**: under `specs/067-keyed-reconciliation/readiness/`:
  - `readiness/typed-controls-front-door.md` — required by the
    `controls-public-surface` routing rule; records that 067 adds zero public
    surface and lands inside the typed-controls package as internal code.
  - `readiness/package-surface-expectations.md` — required by the same rule;
    records the **zero** public-surface delta (SC-005) and the regenerated-baseline
    confirmation.
  - `readiness/keyed-reconciliation.md` — feature-specific (spec §Evidence
    obligations): the algorithm, the keys-first matching rule, the duplicate-key
    first-occurrence diagnostic, and the round-trip / determinism property results.
  - Plus the FAKE-emitted gate logs the escalated path produces (FSI transcripts,
    package surfaces, interaction/rendering, generated-product verify, evidence
    graph/audit) under the same `readiness/` dir, as in `065`/`066`.
- **`.fsi` / contract impact**: **No public contract change (Tier 2).** No existing
  public `.fsi` signature, documented API, sample contract, or surface baseline
  changes (FR-002). The new `Reconcile.fsi` declares `module internal` and is
  **excluded** from the Controls capability `contracts:` list, so `ApiSurfaceGen` /
  `PackageSurfaceCheck` emit no new entry and the baseline stays byte-for-byte
  identical (SC-005). No compatibility/migration note is required (additive,
  internal).
- **MVU/effect boundary**: **N/A — pure stateless diff** (Principle IV). No
  `Model`/`Msg`/`Effect`/`Cmd<Msg>`/`init`/`update`/interpreter: the reconciler
  performs no I/O and owns no state. The generic parameter `'msg` is carried
  through unchanged (it is opaque payload inside `Attr`/`Control`); the diff never
  dispatches or interprets it.
- **Synthetic evidence**: **none.** No mocks, fakes, placeholders, canned
  responses, or in-memory substitutes — `diff` and `apply` are real and the
  property test exercises real generated trees. No `[S]`/`[S*]`/`[SEH]` is
  expected; if any task ships placeholder logic it must carry the Principle V
  disclosure, but the plan's intent is fully-real evidence.
- **Test evidence**: failing-first semantic tests in
  `tests/Controls.Tests/ReconcileTests.fs` (committed red, then greened):
  US1 keyed-reorder (zero replaces), US2 single-attribute/content update, US3
  insert/remove, US4 unkeyed positional + mixed keyed/unkeyed determinism, and the
  edge cases (root kind change → replace, duplicate keys → first-occurrence +
  `KeyCollision` diagnostic, empty trees, identical trees → empty patch). FsCheck
  properties: round-trip over ≥1000 generated `(prev, next)` pairs (FR-008/SC-002)
  and determinism (SC-004). Governance: the existing escalated gate set
  (`ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`,
  `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`,
  `FsiTranscripts`, `GeneratedProductCheck`) plus the serialized six-target order.
- **Observability**: duplicate keys within one sibling list emit a
  `ControlDiagnostic { Code = KeyCollision; Severity = Warning; ... }` naming the
  colliding key and the parent control, carried on `ReconcileResult.Diagnostics`
  (FR-011). The function never throws (SC-007); there is no silent mismatch. No new
  log path or unsupported-environment message is introduced (pure, no I/O).
- **Deferred scope**: current obligation is the correct, property-proven diff +
  its three readiness artifacts and the escalated gate pass. **Deferred** (not this
  feature): wiring into the live render/layout/incremental path, any public
  `Widget`/`Control`/adapter surface or `Cmd<'msg>` alignment (feature `068`),
  design-token/Penpot work (`069`), catalog regeneration, migrating the remaining
  41 controls (`070`), and any move-minimization (LIS) performance tuning — spec
  targets correctness, not a benchmarked fast path.

**Gate result**: PASS — no unjustified violations. Two `N/A`s (Template ownership,
MVU boundary) and one (Generated project) carry one-line rationales as required by
`GeneratedGuidanceCheck`.

## Project Structure

### Source (new — `src/Controls/`)

```
src/Controls/
  Reconcile.fsi        # NEW — `module internal Reconcile`: Patch types + diff/apply signatures
  Reconcile.fs         # NEW — pure implementation
  AssemblyInfo.fs      # NEW (or inline) — [<assembly: InternalsVisibleTo("Controls.Tests")>]
  Controls.fsproj      # EDIT — insert Reconcile.fsi/.fs after Control.fs; add AssemblyInfo.fs
```

`<Compile>` insertion (order matters in F#) — after `Control.fs`, before `Widget.fsi`:

```
... Control.fsi / Control.fs ...
Reconcile.fsi / Reconcile.fs     <- depends only on Types + Control
Widget.fsi / Widget.fs ...
```

`AssemblyInfo.fs` (carrying only the `InternalsVisibleTo` attribute) is added as
the first compile item so the attribute is assembly-scoped.

### Tests (new / edited — `tests/Controls.Tests/`)

```
tests/Controls.Tests/
  ReconcileTests.fs        # NEW — US1–US4 unit tests + FsCheck round-trip/determinism
  Controls.Tests.fsproj    # EDIT — add ReconcileTests.fs (before Program.fs) + FsCheck PackageReference
```

### Evidence (new — `specs/067-keyed-reconciliation/readiness/`)

```
readiness/
  typed-controls-front-door.md        # required by controls-public-surface rule
  package-surface-expectations.md     # required by controls-public-surface rule (zero delta)
  keyed-reconciliation.md             # feature-specific: algorithm + property results
  (+ FAKE-emitted gate logs from the escalated six-target run)
```

### Untouched (explicitly)

`Control.fs`, the renderer, layout, diagnostics emission, accessibility,
`Widget`, the 47-control catalog, `template/**`, `build/Governance/**`,
`Controls.fsproj` reference set, and every public `.fsi` contract.

## Routing & validation

Run **`./fake.sh build -t Route` first** and run only the gates it prints. Because
the diff touches `src/Controls/**`, `Route` escalates to the
`controls-public-surface` rule (`Routing.fs:128`, tier `FocusedAuthority`) and
prints: `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`,
`ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`,
`FsiTranscripts`, `GeneratedProductCheck`. `Route --enforce` requires the two
`readiness/*.md` artifacts the rule names.

Serialized six-target escalated/maintainer-verify order (run sequentially — FAKE
state is not concurrency-safe):

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

## Phase 0 — Research

See [research.md](./research.md). All Technical Context items are resolved (no
`NEEDS CLARIFICATION` remains): the internal-visibility mechanism, the patch data
model, the keys-first matching rule, move detection scope, the duplicate-key
diagnostic, and the FsCheck generator strategy are all decided there.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the `NodePatch<'msg>` / `UpdatePatch<'msg>` /
  `ChildOp<'msg>` / `AttrChange<'msg>` / `FieldChange<'a>` / `ReconcileResult<'msg>`
  algebra and its validation/round-trip rules.
- [contracts/reconcile.fsi](./contracts/reconcile.fsi) — the internal module
  signature sketch (`module internal Reconcile` with `diff`/`apply` + the patch
  types) that the failing-first tests pin and the implementation must satisfy.
- [quickstart.md](./quickstart.md) — how a maintainer exercises the diff from an
  in-assembly test, and the worked US1 reorder example.
- Agent context: `AGENTS.md` SPECKIT plan reference updated to this plan.

## Phase 2 — Task planning approach (not executed here)

`speckit-tasks` will produce the dependency-ordered breakdown. Expected shape
(mirrors the spec's user-story priorities, failing-first):

1. Wire-up: add `Reconcile.fsi`/`.fs` stubs + `AssemblyInfo.fs` + fsproj edits +
   FsCheck reference to the test project; confirm `Dev` builds.
2. Failing-first tests: `ReconcileTests.fs` US1 (P1) reorder, then US2/US3 (P2),
   US4 (P3), edge cases, and the FsCheck round-trip/determinism properties.
3. Implement `diff` (keyed matching → updates → child ops) and `apply` to green
   each story in priority order.
4. Evidence: author the three `readiness/*.md` artifacts.
5. `Route` + serialized six-target order; fix to green.

Critical path: wire-up → US1 red → `diff`/`apply` → round-trip property →
evidence → gates.

## Implementation status — LANDED (2026-06-05)

All 24 tasks (T001–T024) are `[X]` with real evidence; the Synthetic-Evidence
Inventory is empty (Principle V commitment to fully-real `diff`/`apply` held — no
`[S]`/`[S*]`/`[SEH]`). Authored failing-first (the round-trip property errored
against the `Replace next` stub) and greened by the real implementation.

**Delivered**

- `src/Controls/Reconcile.fsi` / `Reconcile.fs` — `module internal Reconcile`:
  the `FieldChange`/`AttrChange`/`NodePatch`/`UpdatePatch`/`ChildOp`/
  `ReconcileResult` algebra plus total, pure, deterministic `diff`/`apply`.
- `tests/Controls.Tests/ReconcileTests.fs` — US1–US4 + edge unit tests and the
  FsCheck round-trip (`apply prev (diff prev next).Patch ≡ next`) and determinism
  properties, **1000 cases each, no counterexample**. Full suite: **77 passed, 0
  failed, 0 errored**.
- Wire-up: `Reconcile.fsi`/`.fs` inserted after `Control.fs`; test-only `FsCheck`
  reference; internal reach via an SDK `<InternalsVisibleTo Include="Controls.Tests" />`
  MSBuild item.

**Deltas from the as-planned design (with rationale)**

1. **No `AssemblyInfo.fs`.** The plan called for a source `AssemblyInfo.fs`
   carrying `[<assembly: InternalsVisibleTo>]`, but the surface-area gate requires
   every `src/Controls/*.fs` to have a paired `.fsi`, which an attribute-only file
   cannot satisfy. Replaced with the SDK `<InternalsVisibleTo>` MSBuild item (same
   generated attribute, no unpaired source file).
2. **`Key`-mismatch ⇒ `Replace`.** A matched pair whose own `Key` differs is a
   whole-subtree `Replace` (not just `Kind`-mismatch). `UpdatePatch` has no channel
   to carry a new `Key`, and `apply`'s `{ prev with … }` would otherwise keep the
   old key — the round-trip property surfaced this. Child matches always share their
   key, so this only ever fires at the root; it is consistent with keyed identity.
3. **Custom `AttrValue` comparator + `%A` round-trip oracle.** `Control<'msg>` does
   not satisfy F#'s `equality` constraint (the `EventValue` function case), so the
   diff compares attribute values with a total custom comparator (structural for the
   data cases, reference/boxed-`Object.Equals` for the opaque/function/`'msg` cases,
   conservative-safe for the round-trip), and the property oracle compares
   attribute-order-canonicalized `sprintf "%A"` reprs rather than `=`.
4. **Per-package raw `.fsi` snapshot refreshed.** Adding the internal `Reconcile.fsi`
   appends to the raw `PerPackageSurfaceDiff` snapshot
   (`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`, +38 lines) — the
   gate's intended "reviewed `.fsi` change" flow. The **public** api-surface
   contract (`PackageSurfaceCheck`) is byte-stable, so **SC-005 holds**.

**Gate results (escalated `controls-public-surface` path)**

`Route` escalates to `controls-public-surface`; `Route --enforce` passes (both
required artifacts present). PASS: `Dev`, `PackageSurfaceCheck`,
`PerPackageSurfaceDiff`, `FsiTranscripts`, `ControlsCatalogCheck`,
`ControlsCatalogGenerationCheck`, `ControlsInteractionCheck`,
`ControlsRenderingCheck`, `GeneratedGuidanceCheck`, `TemplateDrift`,
`EvidenceGraph`, and **`EvidenceAudit` (verdict=PASS, total-blockers=0)** — the
authoritative merge gate. `GeneratedProductCheck` is **environment-degraded**: the
generated product's evidence-graph sub-step cannot self-resolve a feature in this
headless sandbox (empty generated `.specify/feature.json`) — a pre-existing
condition identical to merged 064/065/066, not a regression (see
`readiness/runtime-limitations.md`).

**Success criteria** — SC-001 (zero replaces on reorder), SC-002 (round-trip ≥1000
cases), SC-003 (single targeted attr update), SC-004 (determinism), SC-005 (zero
public-surface delta), SC-006 (Route gates pass), SC-007 (totality) all met.
