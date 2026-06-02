# Feature Specification: V3 Stage 2 — Relocate `AgentValidation` out of the Runtime Monolith

**Feature Branch**: `051-relocate-agentvalidation`
**Created**: 2026-06-02
**Status**: Draft
**Input**: User description: "implement the next part" of the V3 modular-distribution plan
(`docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md` §Stage 2)

## Context

The V3 modular-distribution programme is retiring the legacy `FS.Skia.UI` monolith (`src/Lib`). Stage 0
(feature `048`) stood up the parity oracle and per-package surface baselines; Stage 1 (feature `050`)
extracted the Vulkan/Skia host into the `FS.Skia.UI.SkiaViewer` package and closed the modularity leak.
After Stage 1, `src/Lib` retains only non-host residue: the governance contract parser
`FS.Skia.UI.AgentValidation` (`AgentValidation.fs` 835 LOC + `AgentValidation.fsi` 261 LOC), a
retyped `KeyboardInput`, and a `Parity` helper.

`AgentValidation` is **governance tooling, not runtime** — it parses `validation.contract.yml`
(`ValidationContract`), models gate selection (`ValidationSelection`,
`ValidationSelectionInterpreter`), and produces agent verdicts (`AgentVerdict`). It has never been
shipped to generated products. Yet it lives under the runtime `src/Lib` and is published as part of the
broad `FS.Skia.UI` package, and its `knownGates` allowlist — the canonical set of valid build-gate
names — is therefore **runtime code**.

This placement has two concrete costs the plan calls out:

1. **The contract validator's known-gate allowlist lives in the runtime monolith.** Stage 0 had to
   *defer* wiring a `Routing.fs` rule for the additive `PerPackageSurfaceDiff` target, because rendering
   that gate into `validation.contract.yml` requires `knownGates` to accept it, and editing `knownGates`
   in `src/Lib/AgentValidation.fs` would be a runtime change Stage 0 forbade (see Routing.fs:210–220).
2. **`src/Lib` cannot shed its last ~1,096 LOC of non-host code** while `AgentValidation` is pinned to
   it, blocking the Stage 5 deletion of the monolith.

**Stage-0 finding (corrects the original plan premise):** the build front-end already does *not*
reference the monolith — `build/Governance/FS.Skia.UI.Build.fsproj` has no `ProjectReference` to
`src/Lib`, and `build/Governance/Front/Support.fs` carries its own minimal selection/verdict types. At
the current SHA the **only** consumer of `FS.Skia.UI.AgentValidation` is
`tests/Governance.Tests/AgentValidationFrameworkTests.fs`, and `Governance.Tests`' `ProjectReference`
to `src/Lib/Lib.fsproj` exists *solely* for it. So this stage is a clean **relocate-and-repoint**, not
an entanglement unwind.

This feature is **governance/build-tooling only**: it moves no host or rendering code and ships no
behaviour change to generated products. It relocates the `AgentValidation` module from `src/Lib` into
the `FS.Skia.UI.Build` governance library, repoints its one test consumer, drops the now-unused
`Governance.Tests → Lib` reference, and thereby turns `knownGates` into governance config that Stage 5
can extend without touching runtime.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Governance parser lives in the governance library (Priority: P1)

As the maintainer retiring the monolith, I need the `AgentValidation` contract parser, selection model,
interpreter, and verdict types to live in the `FS.Skia.UI.Build` governance library instead of the
runtime `src/Lib`, so that governance tooling is no longer published as runtime surface and `src/Lib`
sheds its last non-host code ahead of deletion.

**Independent test**: After the move, the `AgentValidation` capability is compiled and exported by
`FS.Skia.UI.Build`; no `AgentValidation.fs(i)` remains under `src/Lib`; the governance test suite that
exercises contract parsing, selection MVU, interpreter behaviour, and verdict (de)serialization passes
against the relocated module; and a behaviour-parity check confirms the relocated parser accepts and
rejects the same contracts/verdicts as before (same diagnostics, same `knownGates` set).

### User Story 2 - The build→runtime test coupling is removed (Priority: P2)

As an agent validating later stages, I need the `Governance.Tests` project to consume `AgentValidation`
from the governance library and to no longer reference the runtime monolith, so that the governance test
suite proves the parser without any link back into `src/Lib`.

**Independent test**: `tests/Governance.Tests/Governance.Tests.fsproj` has **no** `ProjectReference` to
`src/Lib/Lib.fsproj`; the suite still references only `FS.Skia.UI.Build`; the full `Governance.Tests`
suite is green; and a grep confirms no remaining `open FS.Skia.UI.AgentValidation` (the monolith
namespace) anywhere in the test tree.

### User Story 3 - `knownGates` becomes Route-gating-ready governance config (Priority: P3)

As the maintainer planning the Stage 5 closeout, I need the `knownGates` allowlist to live in the
governance library so that adding a new gate (e.g. `PerPackageSurfaceDiff`) to the contract is a
governance-config edit rather than a runtime-monolith edit, unblocking the per-package Route rule the
Stage 0 finding deferred.

**Independent test**: `knownGates` is defined within `FS.Skia.UI.Build` (not `src/Lib`); a reviewer can
confirm that adding a gate name to it and rendering it into `validation.contract.yml` touches only
governance/build paths and no `src/**` runtime file. (Actually *adding* `PerPackageSurfaceDiff` to the
contract and wiring its Routing rule remains Stage 5 scope — this story only proves the precondition is
met.)

### Edge Cases

- **Namespace collision with the front-end's shadow types.** `build/Governance/Front/Support.fs`
  already declares its own minimal `ValidationSelectionModel` / `ValidationSelectionMsg` /
  `ValidationSelectionEffect` / `AgentVerdict` types. The relocated module defines richer same-named
  types under its own namespace; the feature MUST place the relocated module so these do not collide or
  silently shadow, and MUST NOT change the front-end's existing behaviour.
- **Curated `.fsi` boundary (Principle II).** `FS.Skia.UI.Build` modules expose curated signature
  files. The relocated `AgentValidation` MUST retain an explicit `.fsi` (its current 261-LOC surface),
  adapted only as the new namespace requires, so the governance library's public boundary stays
  intentional.
- **Compile order.** The governance library compiles in a fixed file order. The relocated module MUST
  be inserted at a position consistent with its dependencies (it depends only on standard libraries /
  existing governance primitives) without reordering unrelated modules.
- **Generated products unaffected.** `AgentValidation` was never shipped to products; moving it MUST
  produce **zero** change to any generated `app`/profile, and the generated-consumer gates MUST stay
  green.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The `AgentValidation` capability (`ValidationContract` parsing, `ValidationSelection`
  model/update, `ValidationSelectionInterpreter`, `AgentVerdict` (de)serialization, the supporting
  types, and the `knownGates` allowlist) MUST be relocated from `src/Lib` into the `FS.Skia.UI.Build`
  governance library (`build/Governance/**`), preserving its behaviour.
- **FR-002**: After the move, **no** `AgentValidation.fs` or `AgentValidation.fsi` MUST remain under
  `src/Lib`, and `src/Lib/Lib.fsproj` MUST no longer compile them.
- **FR-003**: The relocated module MUST keep an explicit curated `.fsi` signature file (Principle II),
  exposing the same logical surface it exposes today, adapted only for the new namespace.
- **FR-004**: The relocated module's behaviour MUST be **parity-equivalent** to the original: it accepts
  and rejects the same `validation.contract.yml` inputs with the same diagnostics, exposes the same
  `knownGates` set, runs the same selection MVU transitions, and (de)serializes `AgentVerdict`
  identically.
- **FR-005**: `tests/Governance.Tests/AgentValidationFrameworkTests.fs` (and any other governance test
  exercising the parser) MUST be repointed to consume `AgentValidation` from `FS.Skia.UI.Build`, and
  MUST pass.
- **FR-006**: `tests/Governance.Tests/Governance.Tests.fsproj` MUST drop its `ProjectReference` to
  `src/Lib/Lib.fsproj` (which existed solely for `AgentValidation`); the suite MUST reference only
  `FS.Skia.UI.Build` for this capability and remain green.
- **FR-007**: No test, sample, or runtime project, and no governance build path, may reference
  `FS.Skia.UI.AgentValidation` (the monolith namespace) after the move; a repository grep MUST return no
  remaining consumers of that namespace.
- **FR-008**: `knownGates` MUST live in `FS.Skia.UI.Build` after the move so that extending it (and
  rendering a new gate into `validation.contract.yml`) becomes a governance-config edit touching no
  `src/**` runtime file. (Adding `PerPackageSurfaceDiff` itself remains Stage 5 scope.)
- **FR-009**: The move MUST NOT change generated-product output or the consumer contract: the template
  and every generated profile build/restore/run exactly as before, and the generated-consumer gates stay
  green. `AgentValidation` is not shipped to products, so the only expected delta is that the published
  `FS.Skia.UI` package no longer carries it.
- **FR-010**: The relocation MUST NOT alter any host, scene, layout, or rendering behaviour. No
  per-package public-surface baseline of any *runtime* package changes as a result of this feature; only
  the monolith's surface shrinks by the removed module.
- **FR-011**: The front-end's existing minimal selection/verdict types in
  `build/Governance/Front/Support.fs` MUST be left functioning; the relocated module MUST NOT collide
  with or silently override them, and the build front-end's behaviour MUST be unchanged by this feature.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: The published `FS.Skia.UI` monolith package **loses** the `AgentValidation`
  surface (it shrinks by ~1,096 LOC); the `FS.Skia.UI.Build` governance library **gains** it. No
  package is renamed, added, or re-versioned, and `FS.Skia.UI.Build` is build-tooling (not a shipped
  consumer package). No Charts/Controls/DataGrid authoring change; no legacy Charts migration guidance.
- **Public contract impact**: The monolith's public `.fsi` shrinks (the `AgentValidation` module is
  removed); the governance library gains a curated `.fsi` for the relocated module. **No runtime split
  package's `.fsi` changes.** No documented product API or sample contract changes — `AgentValidation`
  was never a product-facing surface.
- **State workflow impact**: The relocated module carries the same `ValidationSelection` MVU
  (model/msg/effect/interpreter) and `AgentVerdict` logic; **behaviour is preserved** (FR-004). No new
  I/O, command, effect, subscription, or interpreter behaviour is introduced — the interpreter's file
  reads / git edges are unchanged.
- **Layout/rendering impact**: **None.** No layout, charts, DataGrid, rendering, screenshot, Vulkan,
  Skia, visual-output, or unsupported-environment-diagnostic behaviour changes.
- **Evidence obligations**: Governance test suite green against the relocated module; a behaviour-parity
  demonstration (same accept/reject diagnostics + same `knownGates`); proof the monolith no longer
  compiles `AgentValidation` and that `Governance.Tests` no longer references `src/Lib`; a grep showing
  no remaining `FS.Skia.UI.AgentValidation` consumers; generated-consumer gates green. Evidence
  graph/audit PASS on real (non-synthetic) evidence.
- **Unsupported scope**: No monolith deletion or unpublish (Stage 5). No new Routing rule and **no**
  addition of `PerPackageSurfaceDiff` to `validation.contract.yml` (Stage 5 — this stage only makes it
  *possible*). No sample-pack policy or `Parity.Tests`/`ParityGallery` retirement (Stages 3–4). No home
  for the residual `KeyboardInput`. No host/scene/rendering change. No separate `FS.Skia.UI.Charts`
  package. No template-profile expansion.
- **Build-target impact**: `Route` is expected to escalate this change (it touches governance paths and
  the monolith's public `.fsi`); run the gates `Route` prints. `GeneratedGuidanceCheck`, `TemplateCheck`
  / `TemplateDrift`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` are the consumer and
  evidence gates and must stay green. No change to the *behaviour* of `Dev`, `Verify`, `Ci`,
  `PackLocal`, or `DependencyReport`. `validation.contract.yml` is **not** edited in this feature
  (currency vs `Routing.fs` is preserved).

## Success Criteria *(mandatory)*

- **SC-001**: `git ls-files src/Lib/AgentValidation.*` returns **nothing**; `AgentValidation` compiles
  as part of `FS.Skia.UI.Build`. The monolith no longer carries the module.
- **SC-002**: The governance test suite exercising the parser/selection/interpreter/verdict passes
  against the relocated module, with **the same** number of assertions covering the same behaviours as
  before the move.
- **SC-003**: A behaviour-parity check confirms the relocated parser yields **identical** accept/reject
  diagnostics for the existing contract fixtures and an **identical** `knownGates` set vs the
  pre-move module.
- **SC-004**: `tests/Governance.Tests/Governance.Tests.fsproj` has **no** `ProjectReference` to
  `src/Lib/Lib.fsproj`, and a repository grep finds **zero** remaining references to the
  `FS.Skia.UI.AgentValidation` namespace outside git history.
- **SC-005**: `knownGates` is defined inside `FS.Skia.UI.Build`; a reviewer confirms that extending it
  and rendering the new gate into `validation.contract.yml` would touch only governance/build paths and
  **no** `src/**` runtime file (the Stage 0 deferral precondition is satisfied).
- **SC-006**: No runtime split package's per-package surface baseline drifts
  (`PerPackageSurfaceDiff`/`PackageSurfaceCheck` green); the only public-surface change is the monolith
  shedding `AgentValidation`. The generated-consumer gates (`TemplateCheck`/`TemplateDrift`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`) are green and the default `app` is byte-unchanged.
- **SC-007**: `validation.contract.yml` is unchanged (no new gate added in this stage); its currency vs
  `Routing.fs` still holds.
- **SC-008**: The gate set `Route` prints for this change is green, with `EvidenceAudit` returning PASS
  on real (zero-synthetic) evidence.

## Key Entities

- **`AgentValidation` module**: the governance contract parser — `ValidationContract` (+ parse
  result/diagnostics), `ValidationSelection*` MVU types, `ValidationSelectionInterpreter`, `AgentVerdict`
  (+ status/outcome/result), `TargetMetadata`, and the `knownGates` allowlist. Relocated `src/Lib` →
  `FS.Skia.UI.Build`.
- **`FS.Skia.UI.Build` governance library** (`build/Governance/**`): the move's destination; already the
  home of `Routing`, `PerPackageSurface`, `TargetMetadata`, `Capabilities`, and the evidence engine.
- **`FS.Skia.UI` monolith** (`src/Lib`): the source; after this stage it retains only `KeyboardInput`
  (retyped) and the `Parity` helper, pending Stages 3–5.
- **`Governance.Tests`**: the sole consumer of the monolith's `AgentValidation`; repointed at the
  governance library and de-referenced from `src/Lib`.
- **`knownGates`**: the build-gate allowlist whose relocation unblocks the Stage 0–deferred per-package
  Route rule.

## Assumptions

- The relocated module's namespace becomes `FS.Skia.UI.Build.AgentValidation` (consistent with
  `FS.Skia.UI.Build.Routing`, `.PerPackageSurface`, `.TargetMetadata`); the exact namespace/module
  placement and compile-order slot are finalized during `/speckit-plan`. Test `open` statements are
  updated accordingly.
- This is a **relocate-and-repoint** with behaviour preserved — not a redesign or a merge with the
  front-end's `Support.fs` shadow types. Reconciling/unifying those shadow types is out of scope; they
  are left untouched and functioning.
- At the current branch-point SHA the only consumer of `FS.Skia.UI.AgentValidation` is
  `tests/Governance.Tests/AgentValidationFrameworkTests.fs`, and `Governance.Tests`' `src/Lib` reference
  exists solely for it (Stage 0 finding, re-verified in this feature's exploration). If any other
  consumer is discovered, it is repointed too (FR-007).
- `AgentValidation` is governance tooling never shipped to generated products, so generated-product
  output is unaffected save for the monolith package shedding the module.
- `Route` escalates this change (governance paths + monolith public `.fsi`); the printed gate list is
  authoritative per the project contract (as in Stage 1, the actual tier may be `agent-ready` rather
  than full `dogfood` — run what `Route` prints).
- The plan's programme-progress table and Stage 2 section are updated after this feature lands (per the
  user's "update the plan afterwards" instruction), recording the actual gate set and any deviations.
