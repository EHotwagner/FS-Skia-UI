# Feature Specification: Foundations Two-Tier Development Process

**Feature Branch**: `042-foundations-two-tier-process`
**Created**: 2026-06-01
**Status**: Draft
**Input**: User description: "@docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md @docs/reports/2026-05-31-1049-foundations-implementation-plan.md implement the next part of the plan." → resolved with the maintainer to **Stage 1** of the foundations implementation plan: the two-tier development process (authoritative tiers, framework-author vs consumer-agent axis, a `Route` entry point, and a tier-selection gate).

## Overview

The foundations programme (companion reports
[`2026-05-31-0908-foundations-rewrite-analysis.md`](../../docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md)
and [`2026-05-31-1049-foundations-implementation-plan.md`](../../docs/reports/2026-05-31-1049-foundations-implementation-plan.md))
identifies its single highest-leverage relief as **Stage 1**: stop applying consumer-grade
governance ceremony to routine framework changes. Today the de-facto default is "run the full
serialized six-target order on everything," which the analysis calls the *suffocation* — ~12–14 h
of process overhead per feature, most of it attention spent satisfying gates rather than changing
the framework. `validation.contract.yml` already lists tiers (`inner-loop`, `focused-authority`,
`agent-ready`, `maintainer-verify`, `automation-final`) and path `routing_rules`, but **nothing
selects a tier and enforces only that tier's gates** for a given change, and there is no
**framework-author vs consumer-agent** axis.

This feature makes the tiers *authoritative and enforced* and gives the maintainer and agents a
single entry point — **`./fake.sh build -t Route`** — that answers "what must I run for *this*
change?" so reading prose and guessing is replaced by a deterministic gate list. Routine
framework-internal work resolves to the light **inner-loop** tier (`Dev` only); changes that touch
the consumer contract (`template/**`, `.specify/**`, governance paths, public `src/**/*.fsi`)
**escalate** to the existing focused/agent-ready tiers — a public `src/**/*.fsi` surface edit
escalates rather than adding a check to inner-loop. A
small set of explicitly **dogfood**-marked features still run the full pipeline so the harness
cannot rot.

Per the maintainer's decision (clarification below), and because the governance library
`FS.Skia.UI.Build` **now exists** (shipped across features 039/040/041), the tier-selection logic is
implemented as **compiled F#** in the library from the start (`Routing.fs`), *not* as the throwaway
`scripts/build/select-tier.fsx` the plan's interim option described. This pulls Stage 5.5's
`Routing.fs` forward — mirroring how feature 041 pulled Stage 5's target-typing forward — so no
`dotnet fsi` selector and no FSharp Compiler Services dependency are ever introduced, and the
contract becomes build-time-checked typed values and predicates sharing the typed `Target` union.

This is a **build-tooling and process** change only. It does not touch the runtime
(`Scene → SkiaViewer → Elmish`, the declarative boundary, the public `.fsi` product surface), does
not change the build front-end *form* (a dedicated build project / `build.fsx` retirement remains
Stage 5), does not port any Bash/Python (Stage 4), and ships no package into any generated product.

## Clarifications

### Session 2026-06-01

- Q: Which foundations stage is this feature? → A: **Stage 1** — the two-tier development process
  (resolved with the maintainer before specifying).
- Q: How should tier-selection/routing be implemented, given the governance library now exists
  (post-041)? → A: **Compiled F# in the library now.** Pull Stage 5.5's `Routing.fs` forward:
  tiers, the `framework-author`/`consumer-agent` axis, and routing rules become typed values and
  predicates (`Diff -> Tier`) in `FS.Skia.UI.Build`, sharing the typed `Target` DU; the `Route`
  target calls them in-process. **No** `scripts/build/select-tier.fsx`, **no** `dotnet fsi`, **no**
  FCS. `validation.contract.yml` is demoted to a generated/derived view of `Routing.fs` (or read
  behind the typed model), never a hand-maintained second source.
- Q: `validation.contract.yml` is also read by `build.fsx` and the `TargetMetadataDrift` check
  (target-reference alignment) and feeds `AgentReady` — does demoting it break those consumers? →
  A: No. The file is **retained but generated from `Routing.fs`** with a generation-currency check,
  so existing consumers keep reading a coherent file while `Routing.fs` is the single source of
  truth; drift becomes structurally impossible rather than detected.
- Q: What is the framework-author inner-loop gate set? → A: `Dev` **only**; nothing else (no spec
  directory, no skillist, no synthetic-evidence inventory, no constitution gate). A public
  `src/**/*.fsi` surface edit does **not** add a check to inner-loop — it **escalates** to
  `focused-authority` (the `package-surface` rule), where `PackageSurfaceCheck` runs.
- Q: Is this feature itself dogfooded? → A: **Yes.** Per the programme meta-process, Stage 1 and
  Stage 4 are the named dogfood features, so this feature runs the **full** Spec Kit + serialized
  evidence pipeline for itself even though the capability it ships would otherwise route it light.
- Q: What change set does `Route` reason over (the `Diff`)? → A: The **union** of the feature
  branch's diff against the default-branch merge-base (`git merge-base HEAD master`…`HEAD`) **and**
  the uncommitted/untracked working-tree changes. Most defensive for an enforcement gate — nothing
  already committed or still unsaved can escape escalation.
- Q: How is the `framework-author` vs `consumer-agent` developer-class determined at runtime? → A:
  **Default `framework-author`**; consumer-contract **paths** (`template/**`, `.specify/**`,
  `src/**/*.fsi`, governance/build-target paths) escalate the tier **regardless** of class; an
  optional `--developer-class consumer-agent` flag raises the floor explicitly. The light path is
  the default; no required argument.
- Q: Where does the `dogfood: true` marker live? → A: As a **typed list of dogfood feature ids in
  `Routing.fs`** (the compiled single source of truth), because the dogfood set is governance
  *policy* that belongs with the routing policy (build-time-checked, unit-testable, ADR D6). The
  generated `validation.contract.yml` view reflects it; Route forces the full pipeline when the
  active feature id is in that list.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A routine framework change runs the light tier, not the full six (Priority: P1)

A maintainer (or framework-author agent) edits only `src/Scene/*.fs`. Instead of running the full
serialized six-target order out of caution, they run `./fake.sh build -t Route`. It prints
`developer-class=framework-author`, `tier=inner-loop`, and the minimal gate list — `Dev` only (a
public `src/**/*.fsi` edit would instead escalate to `focused-authority`). They run exactly that and
ship. The ~12–14 h ceremony is not
applied to a change that does not touch the consumer contract.

**Independent test**: With a working tree containing only a `src/Scene/*.fs` edit, `Route` prints
`inner-loop` → `Dev` (and *not* `TemplateCheck`/`GeneratedProductCheck`/`EvidenceGraph`/
`EvidenceAudit`). Asserted directly against the typed selector in `Governance.Tests`.

### User Story 2 - A consumer-contract change escalates automatically (Priority: P1)

A change touches `template/base/**` (or `.specify/**`, or a public `src/**/*.fsi`). Running
`Route` prints the **escalated** tier and its required gates (e.g. `TemplateCheck`,
`GeneratedProductCheck`, plus the final `EvidenceGraph`/`EvidenceAudit`). The tier is chosen by
*what changed* and *who is changing it*, not by the author remembering a prose rule. Unknown paths
default-deny (the existing `unknown_gate_rejection` is preserved), so a change can never silently
route lighter than it should.

**Independent test**: With a `template/base/**` edit, `Route` prints the escalated gate set
including `TemplateCheck` and `GeneratedProductCheck`; with an unrecognised path, it routes to the
broad fallback rather than emitting an empty/under-specified gate list.

### User Story 3 - `Route --enforce` blocks an under-evidenced escalated change (Priority: P1)

A maintainer attempts to ship a change that touches a public `.fsi` but has not produced the
required `readiness/package-surface-expectations.md` evidence artifact. `Route --enforce` fails with
a precise diagnostic naming the missing artifact and the tier that requires it. After the artifact
is added, `--enforce` passes. The contract is now *enforced*, not merely *documented*.

**Independent test**: Simulate an `src/**/*.fsi` diff without
`readiness/package-surface-expectations.md` present; `Route --enforce` exits non-zero naming that
artifact. Add the artifact; `Route --enforce` exits zero.

### User Story 4 - The agent reads a gate list, not 23,000 lines of prose (Priority: P2)

A framework-author agent starts a change and, per the updated `CLAUDE.md`/`AGENTS.md`, runs `Route`
first and runs only the gates it prints. The blanket "serialized six-target order" instruction is
reframed as the `maintainer-verify`/escalated path, reserved for consumer-contract and dogfood work.
The agent spends its context on the change, not on re-deriving which gates apply.

**Independent test**: `CLAUDE.md` and `AGENTS.md` instruct "run `Route` first; run only the gates it
prints," and present the serialized six-target order as the escalated/maintainer-verify path rather
than the universal default. A guidance test asserts both files contain the `Route`-first instruction
and no longer present the six-target order as the unconditional default.

### User Story 5 - Dogfood features still exercise the full harness (Priority: P2)

A feature is marked `dogfood: true`. Regardless of which tier its diff would otherwise select,
`Route` resolves it to the full pipeline, so the consumer-grade governance path stays continuously
exercised and cannot rot. This very feature (Stage 1) is one of the named dogfood features.

**Independent test**: A feature carrying the `dogfood: true` marker resolves through `Route` to the
full gate set even when its only change is a `src/Scene/*.fs` edit that would otherwise route
`inner-loop`.

### Edge Cases

- **Mixed diff** (a `src/Scene/*.fs` edit *and* a `template/base/**` edit in one working tree): the
  selector MUST take the **highest** applicable tier (escalate), never the lightest — a change that
  matches any escalation rule routes to that rule's gates.
- **Public `.fsi` vs internal `.fs`**: an `src/**/*.fsi` change escalates (surface contract); a
  sibling `src/**/*.fs`-only change in the same package does not. The surface check is required only
  when an `.fsi` actually changed.
- **Empty / no diff**: `Route` MUST print a deterministic result (e.g. the inner-loop default or an
  explicit "no changes") rather than failing or emitting nothing.
- **Unknown path** not matched by any routing rule: default-deny to the broad fallback
  (`unknown_gate_rejection` preserved), never an empty success.
- **Generated `validation.contract.yml` is stale** (hand-edited or not regenerated from
  `Routing.fs`): the generation-currency check MUST fail with a "regenerate from `Routing.fs`"
  diagnostic, so the YAML can never silently diverge from the compiled source of truth.
- **A new escalation path is needed**: because routing rules are typed predicates over the typed
  `Target` DU, naming a gate that is not a real target is a **compile error**, not a runtime
  surprise.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST implement tier selection as **compiled F#** in `FS.Skia.UI.Build`
  (`Routing.fs` + a curated `Routing.fsi`): typed `Tier` values, a typed `DeveloperClass`
  (`FrameworkAuthor` | `ConsumerAgent`) axis, and routing rules expressed as **predicates over a
  diff** (`Diff -> Tier`) whose required-gate lists reference the typed `Targets.Target` union (so a
  mistyped/renamed gate fails to compile). No `scripts/build/select-tier.fsx`, no `dotnet fsi`
  selector, and no FSharp Compiler Services / runtime-script-loading dependency is introduced
  anywhere.
- **FR-002**: The selector MUST add the **framework-author vs consumer-agent** axis. The
  developer-class **defaults to `framework-author`** (the light path is the default; no required
  argument); an optional `--developer-class consumer-agent` flag raises the floor explicitly.
  Default tiers: `framework-author` → `inner-loop` (gates: `Dev` only — a public `src/**/*.fsi`
  surface edit **escalates** rather than adding a check here); `consumer-agent`, and any change
  matching `template/**`, `.specify/**`,
  `validation.contract.yml`, the governance/build-target paths, or `src/**/*.fsi`, **escalate**
  (the consumer-contract **paths force escalation regardless of developer-class**) to the existing
  focused-authority / agent-ready tiers. Unmatched paths MUST default-deny to the broad fallback
  (the existing `unknown_gate_rejection` is preserved).
- **FR-002a**: The `Diff` the selector reasons over MUST be the **union** of (a) the feature
  branch's diff against the default-branch merge-base (`git merge-base HEAD master`…`HEAD`) and (b)
  the uncommitted/untracked working-tree changes, so that neither an already-committed branch change
  nor an unsaved working-tree edit can escape escalation. Selection MUST be a pure function of the
  resulting path set (git invocation stays at the `Route` target edge so the selector is unit-
  testable without git).
- **FR-003**: For a working tree matching **multiple** rules, the selector MUST resolve to the
  **highest** applicable tier (escalation wins); it MUST never route a consumer-contract change down
  to `inner-loop`.
- **FR-004**: The system MUST add a typed `Route` case to the `Targets.Target` union (with its
  `TargetSpec`, dependency wiring, and `dispatchTargets`/metadata derivation) so that
  `./fake.sh build -t Route` runs the selector **in-process** against the current working-tree diff
  and prints the resolved `developer-class`, `tier`, and the minimal required gate list. `Route` is
  additive: no existing target's name, dependencies, outputs, or graph position changes.
- **FR-005**: `Route` MUST support an `--enforce` mode that fails (non-zero, with a precise
  diagnostic naming the missing artifact and the requiring tier) when a change selecting an escalated
  tier is being shipped without that tier's required `expected_artifacts` present; in non-enforce
  mode it prints the gate list without failing.
- **FR-006**: The system MUST express the dogfood set as a **typed list of feature ids in
  `Routing.fs`** (the compiled single source of truth, build-time-checked and unit-testable —
  dogfood selection is governance policy, ADR D6), reflected in the generated
  `validation.contract.yml` view. `Route` MUST resolve a feature whose id is in that list to the
  full gate set even when its diff would otherwise route `inner-loop`. Feature `042` MUST be in the
  list (this feature is a named dogfood feature).
- **FR-007**: `Routing.fs` MUST be the **single source of truth** for tiers, the developer-class
  axis, and routing rules. `validation.contract.yml` MUST be **retained but generated/derived from
  `Routing.fs`** (so existing consumers — `build.fsx`, the `TargetMetadataDrift` target-reference
  check, and `AgentReady` — keep reading a coherent file) and guarded by a **generation-currency
  check** that fails on hand-edit drift. There MUST be no hand-maintained second source of the tier/
  routing content.
- **FR-008**: The system MUST update `CLAUDE.md` and `AGENTS.md` to instruct **"run `Route` first;
  run only the gates it prints,"** and to reframe the blanket serialized six-target order as the
  `maintainer-verify`/escalated path (reserved for consumer-contract and dogfood work), no longer the
  unconditional default.
- **FR-009**: The system MUST document the tiered process and the `Route` entry point in
  `docs/reports/build.md` and `docs/reports/speckit.md` (tiers, the developer-class axis, how
  `Route` selects, and `--enforce`).
- **FR-010**: `tests/Governance.Tests` MUST gain at least **6** cases that call the typed selector
  directly and assert the resolved tier + gate list for representative diffs (≥1 each for:
  inner-loop `src/*.fs`, `.fsi` surface escalation, `template/base/**` escalation, `.specify/**`
  escalation, a mixed-diff escalation, a dogfood-forced full pipeline, an unknown-path
  default-deny, an empty-diff inner-loop default, and a `consumer-agent` floor). All pass;
  assertions are against typed values, not string/IO scraping.
- **FR-011**: Every new public F# module in `build/Governance` (the routing module and any helper)
  MUST carry a curated `.fsi` companion (Principle II); no access modifiers in `.fs`.
- **FR-012**: The library MUST build clean under repository conventions (`net10.0`,
  `TreatWarningsAsErrors`, `FS0078`-as-error, Central Package Management); no new `PackageVersion`
  outside `Directory.Packages.props`.
- **FR-013**: No product public surface MUST change — `PackageSurfaceCheck` and `FsiTranscripts`
  show no baseline diff, and nothing under the runtime `src/**` directories is edited.
- **FR-014**: No package MUST be shipped into any generated product by this feature;
  `FS.Skia.UI.Build` remains build-tooling only. Any new build-tooling dependency (none expected —
  routing needs no new package) MUST be pinned in `Directory.Packages.props` with a row in
  `docs/reports/dependencies.md` and recognised by `DependencyReport`.
- **FR-015**: As a **designated dogfood feature**, this feature MUST run the full serialized FAKE
  gate sequence (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`) for its own validation, in the deterministic serialized order,
  never concurrently.
- **FR-016**: The `Route` work MUST be reversible: it is additive (a new target + a new library
  module + generated/derived contract). Removing it restores the prior "run the serialized order"
  default with no loss; existing targets keep their names, dependencies, and positions.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, version, or generated-package consumer changes.
  `FS.Skia.UI.Build` is build-tooling only and is not shipped into any generated product by this
  feature. No controls/chart/graph/DataGrid authoring change.
- **Public contract impact**: No *product* `.fsi`, documented public API, sample contract, or
  surface baseline changes. New build-tooling modules in `build/Governance` (routing) REQUIRE
  curated `.fsi` companions (Principle II); the internal "contract" surfaces here are those
  build-tooling `.fsi` files plus the generated `validation.contract.yml` view.
- **State workflow impact**: The `build.fsx` MVU `update`/effect-interpreter boundary is preserved.
  The selector is a **pure** function over a diff; I/O (reading the working-tree diff, reading
  evidence-artifact presence, printing) stays at the interpreter edge / the `Route` target body. No
  new long-lived stateful workflow.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan,
  Skia, or visual-output change.
- **Evidence obligations**: Real evidence only — captured `Route` output for the inner-loop,
  escalation, mixed-diff, dogfood, and unknown-path cases; the `Route --enforce` pass/fail
  transcripts; the new Governance.Tests results; the generation-currency check proving
  `validation.contract.yml` is derived from `Routing.fs`; and the full serialized FAKE gate logs
  (this is a dogfood feature, FR-015). No synthetic evidence anticipated.
- **Unsupported scope**: No build front-end migration or `build.fsx` retirement (Stage 5 — only the
  *target-typing* touch needed to add the `Route` case is in scope, consistent with what 041 already
  began), no Bash/Python port (Stage 4), no single-source generation of skills/constitution/skillist
  (Stage 2.2–2.5), no prose-trimming / contract-versioning (Stage 6), no runtime/visual/release/
  platform/distribution change.
- **Build-target impact**: A **new** `Route` target is added (typed `Target` case + metadata +
  dispatch wiring), plus a **generation-currency check** ensuring `validation.contract.yml` is
  derived from `Routing.fs` (either folded into an existing governance target such as
  `TargetMetadataDrift`/`AgentReady`, or a small dedicated check — resolved in planning). `Dev`,
  `Verify`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`,
  `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit` keep their meaning; the serialized six-target
  order is unchanged as the **escalated** path but is no longer the documented universal default.

## Key Entities

- **Tier**: a typed validation level (`inner-loop`, `focused-authority`, `agent-ready`,
  `maintainer-verify`, `automation-final`, and the legacy `tier1`/`tier2`) with its required gate
  list expressed as typed `Targets.Target` values.
- **DeveloperClass**: `FrameworkAuthor | ConsumerAgent` — the who-is-changing-what axis added to
  tier selection. Defaults to `FrameworkAuthor`; an optional `--developer-class consumer-agent`
  flag overrides it; consumer-contract paths escalate regardless of the class.
- **Diff**: the set of changed paths the selector reasons over — the **union** of the branch's diff
  vs the default-branch merge-base and the uncommitted/untracked working-tree changes.
- **RoutingRule**: a typed predicate over a `Diff` that yields a `Tier` (and its required gates +
  expected artifacts), replacing the YAML `routing_rules` as the source of truth.
- **GateSet**: the resolved minimal list of typed `Target`s a given change must run.
- **DogfoodMarker**: a typed list of dogfood feature ids in `Routing.fs` (governance policy in the
  compiled source of truth) forcing the full pipeline for those features regardless of the diff's
  tier; reflected in the generated `validation.contract.yml` view.
- **Route (target)**: the new typed `Target` case and FAKE entry point that runs the selector
  in-process and prints (or, with `--enforce`, enforces) the resolved tier + gate list.
- **ContractView**: the generated/derived `validation.contract.yml` emitted from `Routing.fs`,
  currency-checked so it cannot drift from the compiled source.

## Success Criteria *(mandatory)*

- **SC-001**: `./fake.sh build -t Route` on a working tree containing only a `src/Scene/*.fs` change
  prints `framework-author` → `inner-loop` → `Dev` (no surface check, no `.fsi` changed) and **not**
  the full six-target set; the captured output is recorded in the feature's readiness evidence.
- **SC-002**: `Route` on a `template/base/**` change prints the escalated gate set including
  `TemplateCheck` and `GeneratedProductCheck`; `Route` on an `src/**/*.fsi` change **escalates** (the
  `package-surface` rule) to a gate set including `PackageSurfaceCheck`; an unknown path routes to the
  broad fallback (never empty).
- **SC-003**: `Route --enforce` exits non-zero, naming `readiness/package-surface-expectations.md`,
  on a simulated `src/**/*.fsi` change lacking that artifact; it exits zero once the artifact is
  present. Both transcripts are recorded.
- **SC-004**: At least 6 Governance.Tests cases call the typed selector directly and assert the
  resolved tier + gate list for the representative diffs in FR-010; all pass and assert typed values,
  not strings.
- **SC-005**: A `dogfood: true`-marked feature resolves through `Route` to the full gate set even
  when its only change would otherwise route `inner-loop`; demonstrated and recorded.
- **SC-006**: No `scripts/build/select-tier.fsx`, no `dotnet fsi` selector, and no
  `FSharp.Compiler.*` dependency is introduced (grep over the build/library projects proves it);
  the routing logic is compiled F# in `FS.Skia.UI.Build`.
- **SC-007**: `Routing.fs` is the single source of truth and `validation.contract.yml` is generated
  from it; the generation-currency check **fails** on a hand-edited/stale contract and **passes**
  when freshly derived. Demonstrated by a scratch hand-edit that the check rejects.
- **SC-008**: `CLAUDE.md` and `AGENTS.md` instruct "run `Route` first; run only the gates it
  prints," and present the serialized six-target order as the escalated/maintainer-verify path, not
  the unconditional default; `docs/reports/build.md` and `docs/reports/speckit.md` document the
  tiered process and `Route`.
- **SC-009**: The full serialized FAKE gate sequence is green (modulo the documented pre-existing
  `FsiTranscripts`/`SkiaViewer.Tests` environment flakes recorded in feature 039),
  `PackageSurfaceCheck` shows no baseline diff, `git diff` over `src/**` is empty, and no new
  `PackageVersion` exists outside `Directory.Packages.props`.

## Assumptions

- The typed `Targets.Target` union shipped in feature 041 is the foundation the routing gate lists
  reference; adding a `Route` case + spec + dispatch wiring is the only *target-typing* touch
  required and is consistent with what 041 already began (the Stage-5 MVU-engine relocation and the
  dedicated build front-end / `build.fsx` retirement remain out of scope).
- Implementing routing as compiled F# now (rather than the plan's interim `select-tier.fsx`) is the
  maintainer's resolved decision and is consistent with ADR D6 (compiled-F# config) and the plan's
  own guidance to "pull the library work forward" rather than grow YAML parsing.
- Generating `validation.contract.yml` from `Routing.fs` keeps the file's existing consumers
  (`build.fsx:744`, the `TargetMetadataDrift` target-reference check, `AgentReady`) working
  unchanged while removing the second source of truth; the precise generation mechanism (emit-on-
  build vs read-behind-model) is a planning detail, but the hard requirement is a single source plus
  a currency check.
- The diff the selector reasons over is the **union** of the branch-vs-merge-base diff and the
  working-tree changes (resolved in clarification); git invocation stays at the `Route` target edge
  so selection is pure given the path set and unit-testable without invoking git inside the tests.
- Per the programme meta-process, Stage 1 is a **named dogfood feature**, so it runs the full Spec
  Kit + serialized evidence pipeline for itself, holding Invariants 1–6, even though the capability
  it ships would otherwise route routine framework work light.

## Dependencies

- Builds on `039-foundations-baseline-spike` (library skeleton, dedicated front-end, ADRs
  D1/D2/D6), `040-foundations-capability-skills` (the extract-into-`build/Governance` + curated-
  `.fsi` + Governance.Tests pattern), and `041-foundations-library-validators` (the typed
  `Targets.Target` union and `dispatchTargets` the `Route` case extends).
- Independent of the remaining library track (Stage 4 Python evidence port) and Stage 2.2–2.5
  (single-source generation). De-risks every later stage by giving framework work a light default
  tier. Pulls Stage 5.5's `Routing.fs` forward; the rest of Stage 5 (MVU engine relocation, build
  front-end form) still follows.
