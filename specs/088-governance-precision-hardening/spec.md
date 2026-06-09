# Feature Specification: Governance Precision Hardening

**Feature Branch**: `088-governance-precision-hardening`  
**Created**: 2026-06-09  
**Status**: Draft  
**Input**: User description: "create specs for all three tiers" — the three tiers of governance-system improvement surfaced by the general analysis: (1) single-source-of-truth / typed gate identity, (2) target granularity & routing precision, (3) governance code health.

## Overview

The build governance system (`FS.Skia.UI.Build`, rooted at `build/Governance/**`) advertises a
core principle — governance artifacts are **generated from a single source, not hand-synced** —
and a routing system that prints the **minimal** gate list for a change. A general analysis found
three classes of gap where reality diverges from those promises. This feature closes them in three
independently shippable tiers, ordered by worth:

- **Tier 1 (P1) — Typed gate identity / single source.** Gate identity is a bare `string` in
  several places, so the compiler cannot enforce the single-source promise; new or renamed gates
  drift silently.
- **Tier 2 (P2) — Target granularity & routing precision.** Heavy gates bundle cheap and expensive
  work behind one name, and broad path globs escalate doc-only edits to the full heavy gate set.
- **Tier 3 (P3) — Governance code health.** Behavior-preserving cleanup of the duplication and
  oversized hotspots that make the above changes (and future ones) error-prone.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Typed gate identity eliminates silent drift (Priority: P1)

A maintainer adds a new governance gate (a new `Targets.Target` case) or renames an existing one.
Today they must hand-update several parallel string structures — `focusedGateContract`'s string
match, `knownGates`, `ProductChecksRun` lists — and any omission fails **silently and wrong**: an
unhandled gate falls through to a `VerificationDegraded` verdict with no readiness path, with no
compile error and no test failure. After this tier, gate identity is the `Target` type end-to-end,
the parallel lists are generated from the single `Targets` enumeration, and a missed gate is a
**compile error**, not a silent runtime degrade.

**Why this priority**: Highest worth — it upholds the architecture the project explicitly
advertises, the compiler does the enforcement (no new drift gate needed), and it removes a recurring
"forgot to update file N" failure mode. It is also a prerequisite that makes Tiers 2–3 safer.

**Independent Test**: On a branch that adds a throwaway `Target` case but deliberately omits it from
the (now-generated) downstream lists, the build **fails to compile** (or a generation-currency gate
fails) rather than producing a degraded-but-green run. Conversely, removing the hand-maintained
`knownGates`/`ProductChecksRun` literals and regenerating them from `Targets` produces byte-identical
results to the prior hand-maintained lists.

**Acceptance Scenarios**:

1. **Given** the `focusedGateContract` lookup keyed by `Target` (not `string`), **When** a new
   `Target` case is added without a corresponding contract arm, **Then** the project does not compile.
2. **Given** `knownGates` and `Verify`'s `ProductChecksRun` are derived from the `Targets`
   enumeration / prerequisite graph, **When** the build runs, **Then** the derived values equal the
   previously hand-maintained lists (no behavior change), and `validation.contract.yml` and all
   currency gates (`TargetMetadataDrift`, `SkillSyncCheck`) still pass.
3. **Given** a gate previously falling through to the `VerificationDegraded` wildcard
   (e.g. `ContrastCheck`, `ControlFidelityCheck`, `PerPackageSurfaceDiff`, `SkillContractPathCheck`),
   **When** its assumption check runs, **Then** it resolves to an explicit, correct verdict contract
   rather than the silent degraded fallback.

---

### User Story 2 - Granular targets and precise routing avoid needless heavy work (Priority: P2)

A maintainer makes a documentation- or comment-only edit under a broad governed path
(`template/**`, `src/Controls/**`), or a change that needs only the structural half of a heavy gate.
Today `Route` escalates them to the full heavy gate set — `GeneratedProductCheck`'s expensive
consumer restore/build/verify, or a doc edit dragging in medium-cost gates. After this tier,
`GeneratedProductCheck`'s cheap structural scan is separable from its expensive consumer validation,
and `Route` returns a lighter gate list for change classes that provably need less.

**Why this priority**: Best day-to-day feedback-loop savings, at low risk (new routing rules and a
target split that preserve the existing public umbrella). Sits on top of Tier 1's typed targets.

**Independent Test**: Stage a doc-only edit (e.g. a `template/**/*.md` file) and run
`./fake.sh build -t Route`; the printed gate list excludes `GeneratedProductCheck` (and other heavy
gates) and routes to a documentation-appropriate minimal set. Stage a change that needs only the
structural scan and confirm `Route` returns the cheap structural target, not the full consumer
validation. In both cases, the previously-escalated change classes (real template source, real
`.fsi` surface changes) still route to the full set.

**Acceptance Scenarios**:

1. **Given** `GeneratedProductCheck` is decomposed into a cheap structural target
   (generate + structural scan + file-list evidence) and an expensive consumer-validation target
   (restore + build + generated `Verify`), **When** the umbrella `GeneratedProductCheck` runs,
   **Then** it produces the same evidence artifacts and verdict as before (public contract preserved).
2. **Given** routing rules distinguish doc-only paths from source paths under `template/**` and
   `src/Controls/**`, **When** a doc-only change is routed, **Then** the gate list omits the heavy
   gates; **When** a real source/contract change is routed, **Then** the full gate set is unchanged.
3. **Given** the structural seam is clean (the cheap scan does not consume the expensive step's
   output), **When** the structural target fails, **Then** it fails fast before the expensive
   consumer validation runs.

---

### User Story 3 - Governance code health reduces future drift surface (Priority: P3)

A maintainer extending a generated-product scan or a per-target handler today navigates a
~2,200-line grab-bag module and a ~990-line single `match`, and copies one of two near-identical
scan functions. After this tier, the identified duplication is extracted to shared helpers and the
worst hotspots are made navigable — all behavior-preserving, with no public surface or evidence
change.

**Why this priority**: Real but lowest leverage and highest churn; valuable mainly as it lowers the
cost and risk of Tiers 1–2 and future work. Deliberately bounded to avoid large speculative rewrites.

**Independent Test**: After refactoring, the full escalated six-target order produces byte-identical
evidence artifacts and verdicts to a pre-refactor baseline (pure refactor — no governance behavior
changes), and no public `.fsi` surface or `validation.contract.yml` content changes.

**Acceptance Scenarios**:

1. **Given** the two near-identical generated-row scan functions share extracted validators,
   **When** both scans run, **Then** their findings are unchanged from baseline.
2. **Given** behavior-preserving extraction of the identified hotspots, **When** the governance test
   suite runs, **Then** all tests pass with no expected-output (golden) changes.

---

### Edge Cases

- A gate exists in the `Targets` enumeration but is intentionally **not** a routable consumer gate
  (e.g. `Clean`, `Restore`, `Build`): generated `knownGates` / contract derivations must preserve the
  current routable-vs-internal distinction, not blindly include every `Target`.
- A path matches **both** a doc-only relaxation rule and a source rule (e.g. a commit touching both a
  `.md` and a `.fsi` under `src/Controls/**`): the existing tier-composition (max tier, union of
  gates) must still hold so the heavier classification wins — relaxation never weakens a mixed change.
- The `GeneratedProductCheck` umbrella is referenced directly by other routed gate lists and by
  `Verify`: the split must keep the umbrella name resolvable so no downstream reference breaks.
- Governance / build-infrastructure paths (`build.fsx`, `scripts/build/**`, `.specify/**`,
  governance paths) are **excluded** from doc-only relaxation by default (conservative): a comment in
  build infrastructure can affect build semantics.

## Requirements *(mandatory)*

### Functional Requirements

**Tier 1 — Typed gate identity / single source (P1)**

- **FR-001**: The focused-gate contract lookup MUST be keyed by the `Targets.Target` type rather than
  a bare `string`, such that every gate that can reach the lookup is matched by an exhaustive
  (compiler-checked) match, and adding a `Target` case without a contract arm is a compile error.
- **FR-002**: Gates that today fall through the contract wildcard to `VerificationDegraded` MUST
  instead resolve to explicit, correct verdict contracts (or be provably unreachable by the lookup).
- **FR-003**: The `knownGates` allowlist MUST be derived from the single `Targets` enumeration (the
  set of routable gate names) rather than a hand-maintained string list, with the derived value equal
  to the prior list.
- **FR-004**: `Verify`'s `ProductChecksRun` list (and any sibling hand-maintained gate-name lists in
  the engine) MUST be derived from the `Targets` prerequisite graph / enumeration rather than string
  literals, with the derived value equal to the prior list.
- **FR-005**: Gate-name call sites that pass bare strings (e.g. `focusedGateAssumptionCheck model
  "GateName"`) MUST pass the `Target` value (or a single canonical name derived from it), so a rename
  is a single-source change.

> Interacting / conflicting requirements: FR-003/FR-004 (derive from `Targets`) vs. the edge case
> that not every `Target` is a routable gate — resolution: derive from the **routable-gate
> projection** of `Targets`, not the raw DU, preserving today's internal-vs-routable split.

**Tier 2 — Target granularity & routing precision (P2)**

- **FR-006**: `GeneratedProductCheck` MUST be decomposable into (a) a cheap structural target
  (`GenerateV3Products` + `ScanV3GeneratedProducts` + file-list evidence) and (b) an expensive
  consumer-validation target (`ValidateGeneratedConsumer` + validation report), with the cheap target
  ordered before and independent of the expensive one.
- **FR-007**: The existing `GeneratedProductCheck` name MUST remain a resolvable umbrella that
  composes the two sub-targets and produces the same evidence artifacts and verdict as before (public
  contract preserved — no consumer-visible regression).
- **FR-008**: `Route` MUST classify documentation-only changes (e.g. `**/*.md`) under `template/**`
  and `src/Controls/**` to a documentation-appropriate minimal gate list that excludes the heavy
  gates, while real source/contract changes under those paths continue to route to the full set.
- **FR-009**: Routing for build-infrastructure and governance paths (`build.fsx`, `scripts/build/**`,
  `validation.contract.yml`, `.specify/**`) MUST NOT be relaxed for doc-only edits by default
  (conservative posture retained).
- **FR-010**: Over-broad dependency chains that pull in work a target's body does not consume SHOULD
  be tightened where the seam is clean (e.g. revisit `TemplateSmoke -> [...; Test]` and
  `GeneratedProductCheck -> [...; Dev; TemplateCheck]`), without changing any gate's effective
  coverage. Tightening that would change effective coverage is out of scope.

> Interacting / conflicting requirements: FR-008 (relax doc-only) vs. the mixed-change edge case —
> resolution: relaxation adds **narrower** rules; the existing max-tier / union-of-gates composition
> ensures any non-doc file in the same change re-escalates.

**Tier 3 — Governance code health (P3, behavior-preserving)**

- **FR-011**: The two near-identical generated-row scan functions (`scanGeneratedRow`,
  `scanV3GeneratedRow`) MUST share extracted common validators (file enumeration, package-reference
  validation) with no change to their findings.
- **FR-012**: Identified boilerplate duplication (e.g. the paired NuGet-config templates) SHOULD be
  consolidated, behavior-preserving.
- **FR-013**: All Tier 3 changes MUST be behavior-preserving: byte-identical evidence artifacts and
  verdicts versus a pre-refactor baseline, and no public `.fsi` surface or `validation.contract.yml`
  content change.
- **FR-014**: Larger structural re-splits (e.g. decomposing the ~2,200-line `GeneratedProduct.fs` by
  domain, or table-driven dispatch for the ~990-line engine `match`) are **out of scope** for this
  feature and explicitly deferred unless a follow-up requests them.

### Framework Governance Prompts *(mandatory)*

> Exempt from the "no implementation details" rule (feature 085, FR-014): this section names concrete
> surfaces by design.

- **Package impact**: No package identity, contents, or version changes. This is build-governance
  internal work in `FS.Skia.UI.Build` (`build/Governance/**`, packed from `build/Governance`). No
  controls/chart/graph/DataGrid authoring change; no legacy Charts migration.
- **Public contract impact**: No product `.fsi` / public API / sample-contract / surface-baseline
  change is intended. Internal `FS.Skia.UI.Build` signatures change (e.g. `Targets.fsi`,
  `Front/Helpers` signature, `AgentValidation` signature) — these are governance-internal, not
  product public surface. The set of public **build targets** is preserved: `GeneratedProductCheck`
  remains an addressable umbrella (FR-007); any new sub-targets are additive.
- **State workflow impact**: No product MVU/effects/interpreter behavior change. The build engine's
  `update`/effect wiring (`build/Governance/Engine/**`) changes mechanically (typed keying, derived
  lists, umbrella composition) with no change to effective gate behavior.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshot, Vulkan,
  Skia, or unsupported-environment diagnostic change.
- **Evidence obligations**: Real evidence that the change is behavior-preserving and currency-clean —
  `Route` output before/after for representative diffs (doc-only vs. source); the escalated
  six-target order (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`) passing; `TargetMetadataDrift` and `SkillSyncCheck` clean;
  regenerated `validation.contract.yml` byte-identical (Tier 1/3) or intentionally updated with diff
  rationale (Tier 2 new targets/rules).
- **Unsupported scope**: No visual/release/platform/distribution change. Deferred: large module
  re-splits and table-driven engine dispatch (FR-014); relaxation of governance/build-infra routing
  (FR-009); any change to effective gate coverage (FR-010).
- **Build-target impact**: `Route`/`Routing.fs` changes (new doc-only rules, possibly tightened
  dependency chains). `GeneratedProductCheck` is split into sub-targets behind an umbrella; new
  sub-target names are added to `Targets`. `TargetMetadataDrift` must be re-satisfied (regenerated
  `validation.contract.yml`). `Verify`/`Ci` composition unchanged in effective coverage. `Dev`,
  `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, `EvidenceAudit` behavior unchanged.

## Key Entities

- **`Targets.Target`** — the discriminated union that is the intended single source of gate identity;
  all gate-name strings and parallel lists should derive from it (or its routable-gate projection).
- **Focused-gate contract** — the per-gate record (prerequisites, command, log/readiness paths, stale
  assumptions, verdict category) currently looked up by string in `Front/Helpers.focusedGateContract`.
- **`knownGates` / `ProductChecksRun`** — hand-maintained gate-name lists to be made derived.
- **Routing rule** — a (path-glob pattern set → tier + gate list) entry in `Routing.fs`; new
  doc-only rules are added; existing source rules are preserved.
- **`GeneratedProductCheck` umbrella + structural / consumer sub-targets** — the split target family.

## Success Criteria *(mandatory)*

- **SC-001**: Adding a `Target` case without wiring its gate contract is caught at **compile time**
  (or by a currency gate), not as a silent green-but-degraded run — demonstrated by a deliberate
  omission failing the build.
- **SC-002**: Zero hand-maintained gate-name string lists remain for `knownGates` and `Verify`'s
  `ProductChecksRun`; both are generated and equal to their prior hand-maintained values.
- **SC-003**: Every routable gate resolves to an explicit verdict contract; no routable gate relies on
  the `VerificationDegraded` wildcard fallback.
- **SC-004**: For a documentation-only change under `template/**` or `src/Controls/**`, `Route`
  returns a gate list that excludes `GeneratedProductCheck` and the other heavy gates; for a matched
  real source/contract change, the routed gate set is unchanged from today.
- **SC-005**: The `GeneratedProductCheck` umbrella produces evidence artifacts and a verdict identical
  to the pre-split behavior; the cheap structural target can run and fail independently of (and before)
  the expensive consumer validation.
- **SC-006**: All Tier 1 and Tier 3 changes are behavior-preserving: the escalated six-target order
  and the full governance test suite pass with no golden/expected-output changes and a byte-identical
  regenerated `validation.contract.yml`.
- **SC-007**: Each tier is independently shippable — Tier 1 can merge and pass all gates without Tier
  2 or Tier 3 present, and vice versa. **Verified as a design/ordering property** (recorded argument),
  not by an isolated per-tier gate run: the tiers touch disjoint files (Tier 1/3 are byte-identical to
  baseline; only Tier 2 regenerates `validation.contract.yml`), and each tier's checkpoint establishes
  its slice's contract posture at the point it lands. See `contracts/behavior-preserving.md`
  §"Verification method".

## Assumptions

- "All three tiers" means **one feature** with three independently shippable user stories (matching
  this repo's pattern, e.g. 087 bundling multiple gates), not three separate feature directories. The
  `/speckit-specify` workflow creates exactly one feature per invocation.
- The `GeneratedProductCheck` split keeps the existing name as an umbrella (no consumer-visible target
  removal); new sub-target names are additive. (Informed guess over introducing a breaking rename.)
- Doc-only routing relaxation is limited to `template/**` and `src/Controls/**`; build-infrastructure,
  `validation.contract.yml`, and `.specify/**` keep their conservative classification (FR-009).
- Dependency-chain tightening (FR-010) is opportunistic and only where it provably does not change
  effective coverage; if any tightening is ambiguous, it is deferred rather than risked.
- Tier 3 is bounded to the specific duplication/hotspots identified; large speculative rewrites are
  out of scope (FR-014).
- The analysis-identified file locations (`build/Governance/Front/Helpers.fs`,
  `build/Governance/AgentValidation.fs`, `build/Governance/Engine/Update.fs`,
  `build/Governance/Targets.fs`, `build/Governance/Routing.fs`, `build/Governance/GeneratedProduct.fs`)
  are the working surfaces; planning will confirm exact line ranges.
