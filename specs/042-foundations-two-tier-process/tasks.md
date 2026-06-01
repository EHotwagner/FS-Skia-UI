# Tasks: Foundations Two-Tier Development Process (Stage 1)

**Feature branch**: `042-foundations-two-tier-process`
**Spec**: `specs/042-foundations-two-tier-process/spec.md`
**Plan**: `specs/042-foundations-two-tier-process/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real — captured
`Route` transcripts over crafted working trees, typed-selector unit tests over
literal `Diff` values exercising real routing / escalation / default-deny /
dogfood paths, the `--enforce` pass/fail transcripts, the currency-check
reject/accept demonstration, and the serialized FAKE gate logs (plan: Evidence
obligations — real evidence only; Principle V — no `[S]`/`[SEH]` anticipated).
The selector inputs are real crafted `Diff` values exercising real error/deny
paths, not malformed-input fixtures, so no `[SEH]` task is approved.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]**, **[US5]** — user-story scope
- **[T1]** — Tier 1 (contracted): this whole feature is Tier 1 and a named
  **dogfood** feature (FR-015), so it runs the full serialized evidence
  pipeline for itself even though the capability it ships would route routine
  framework work light.
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read before implementation)

Like feature 041, this feature is **build-tooling only** (`build/Governance` +
`build.fsx`), so no `fs-skia-*` runtime/rendering/viewer/layout/widgets skill
applies. It *consumes* the `fsharp-*` cookbooks as genuine implementation aids
where they materially help:

- **`fsharp-io-globbing`** — the typed `RoutingRule.Matches` predicates are
  fnmatch glob predicates over the `Diff` path set, reusing the repo's existing
  `BuildPaths` semantics (T009). High confidence; the rule table is the heart of
  the selector and is glob-driven.
- **`fsharp-build-orchestration`** — Expecto typed-selector tests, the FAKE
  in-process `Route` edge wiring, the guidance-assertion test, and the
  serialized FAKE gate runs (T008, T010, T012, T014, T015, T017, T019, T023).
  Medium confidence; it is the orchestration / test skill for these tasks.
- **`fsharp-code-generation`** — `ContractView.render` emits the canonical
  `validation.contract.yml` text deterministically from the compiled routing
  policy (T020). Medium-high confidence; this is governance-artifact emission.

**Not assigned** (with reasons, mirroring 041's discipline):
`fsharp-shell-process` — research R2 resolves that the git union-diff at the
`Route` edge **reuses the existing `BuildProcess` wrapper**; no new process
abstraction is introduced, so the shell cookbook is not a material aid here.
`fsharp-graph-algorithms` — escalation is `List.maxBy tierRank` over a typed
lattice; there is no DAG cycle-detection / topo-sort. `fsharp-parsing` — the
currency check is **byte-equality** of the rendered vs on-disk contract text, not
a parse; no governance input is parsed in this feature. The only
governance-workflow skills are `speckit-evidence-graph` (T029) and
`speckit-evidence-audit` (T030).

## Governance risk levels & validation

- **Small** (routine framework-internal `src/**/*.fs`-style edits within this
  feature's own library work): focused `./fake.sh build -t Dev` plus the
  Governance.Tests suite is authoritative.
- **Medium** (the new build-tooling `.fsi`/`.fs`, the `Route` target case, the
  generated contract): focused `Dev` + targeted FAKE governance gates.
- **Broad** (required here because this is a **dogfood** feature, FR-015): the
  full serialized FAKE gate order. Aggregate FAKE results are recorded as
  **non-authoritative**; any race-like or environment-flaky gate failure (the
  documented 039 `FsiTranscripts` / `SkiaViewer.Tests` flakes) is rerun in
  focused isolation, and the focused rerun is the authoritative result.

FAKE-backed commands share repository `.fake` state and are **not** safe to run
concurrently. When more than one FAKE-backed target is needed, run them in the
deterministic serialized order (`Dev` → `GeneratedGuidanceCheck` →
`TemplateCheck` → `GeneratedProductCheck` → graph gate → audit gate), never
concurrently.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record feature Tier 1 and **dogfood** status, affected layer (`build/Governance` + `build.fsx` build-tooling only), public-API impact (no product `.fsi`; new build-tooling `.fsi` required by Principle II), Elmish/MVU applicability (the selector is **pure** and plugs into the existing `build.fsx` `update`/effect interpreter boundary — no new `Model`/`Msg`/`Effect`), and the real-evidence obligations (≥6 typed selector cases, the five `Route` transcripts, the `--enforce` and currency-check demonstrations, `src/**` untouched, full serialized FAKE logs)
- [X] T002 [P] [T1] [skillist: []] Create placeholder evidence files listed by the plan under `specs/042-foundations-two-tier-process/readiness/` (and `readiness/logs/`) so the audit-enforced readiness files are discoverable at setup time: the `Route` transcripts (`route-inner-loop.txt`, `route-escalation.txt`, `route-enforce.txt`, `route-dogfood.txt`), `contract-currency.md`, `governance-tests.md`, `src-untouched.md`, `no-fsx-fsi-fcs.md`, and the governance scaffolds named in T003
- [X] T003 [T1] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files (`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation-authority.md`, `evidence-graph.md`, `evidence-audit.md`), each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T004 [P] [T1] [skillist: []] Place the two curated `.fsi` contracts from `contracts/` (`Routing.fsi`, `ContractView.fsi`) into `build/Governance/`, create their `.fs` companions (skeletons against the signatures), and add the `Routing.fsi`/`Routing.fs`/`ContractView.fsi`/`ContractView.fs` `<Compile>` entries to `FS.Skia.UI.Build.fsproj` **after** `TargetMetadata` and before `Capabilities` (Routing depends on `Targets`) — Principle I/II, no access modifiers in `.fs` (FR-011)
- [X] T005 [P] [T1] [skillist: []] Add the additive `Route` case to the `Targets.Target` union in `Targets.fsi` **and** `Targets.fs` (same position), extend `name`, `directPrerequisites` (`Route -> []`), and `allTargets` so metadata derives automatically; `timeoutClass`/`cost`/`failureOwner` fall through to the `focused`/`low`/`governance` defaults — no existing target's name, deps, outputs, or graph position changes (FR-004/FR-016)
- [X] T006 [T1] [skillist: []] Exercise the draft `Routing.fsi` / `ContractView.fsi` from FSI (representative `select`, `selectForFeature`, `unmetArtifacts`, and `render` calls over literal `Diff` values), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T007 [T1] [skillist: []] Record surface-area baselines for the new `build/Governance` modules and the unsupported-scope handling: an empty/garbage git range or absent merge-base is surfaced explicitly at the `Route` edge (logged diagnostic, never a silent empty diff), and the Stage-5 MVU-engine relocation / build front-end retirement remain out of scope

**Checkpoint**: Foundation ready — the FSI sketch compiles and the five story phases may proceed.

---

## Phase 3: User Story 1 (US1) — a routine framework change runs the light tier (P1)

*Independent test*: with a working tree containing only a `src/Scene/*.fs` edit,
the typed selector resolves `FrameworkAuthor` / `InnerLoop` / `[Dev]` (no surface
check, no full six-target set), asserted directly in `Governance.Tests`, and
`./fake.sh build -t Route` prints exactly that.

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fsharp-build-orchestration] Add failing `tests/Governance.Tests/RoutingTests.fs` with the inner-loop case: `select FrameworkAuthor { ChangedPaths = ["src/Scene/Foo.fs"] }` yields `Tier = InnerLoop` and `Gates = [Dev]` (no `PackageSurfaceCheck`), **and** the empty-diff default `select FrameworkAuthor { ChangedPaths = [] }` → `Tier = InnerLoop` / `Gates = [Dev]` (deterministic, never failing), asserting the **typed** `Selection` values — not string/IO scraping; register the file in `Governance.Tests.fsproj` `<Compile>` before `Program.fs` (fails before `Routing.fs` is implemented; SC-004 / FR-010)

### Implementation

- [X] T009 [US1] [skillist: fsharp-io-globbing] Implement `build/Governance/Routing.fs` against its `.fsi`: the typed rule table (data-model R5; `template/**` and `.specify/**` broadened, F2) with glob `Matches` predicates over `BuildPaths`, `tierRank`, `innerLoopGates` (`[Dev]`; a `src/**/*.fsi` change escalates via the `package-surface` rule, F1), `fullPipelineGates`, `dogfoodFeatureIds` (incl. `"042"`), `isDogfood`, the pure `select` (default-deny unmatched → `Verify`, `maxBy tierRank` escalation incl. the `ConsumerAgent` floor to `FocusedAuthority`, registry-order gate de-dup), `selectForFeature` (dogfood override), `unmetArtifacts`, `enforceDiagnostic`, and `renderSelection` — plain DUs + records + pure functions, no access modifiers (Principle II/III); no `select-tier.fsx`, no `dotnet fsi`, no FCS (SC-006)
- [X] T010 [US1] [skillist: fsharp-build-orchestration] Wire the `StartTarget Targets.Route` print arm into the `build.fsx` `update`/interpret boundary: compute the union `Diff` (R2) at the edge via the existing `BuildProcess` git wrapper (`merge-base HEAD master`…`HEAD` ∪ `status --porcelain --untracked-files=all`), parse the optional `--developer-class consumer-agent` token, resolve the active feature id via the existing `activeFeatureId` helper, call `selectForFeature`, and print `renderSelection`; the empty/no-diff case prints a deterministic inner-loop result rather than failing (Principle IV: I/O stays at the edge, selector stays pure)
- [X] T011 [US1] [skillist: []] Capture SC-001 evidence: `./fake.sh build -t Route` on a working tree containing only a `src/Scene/*.fs` change prints `framework-author` → `inner-loop` → `Dev` (and not the full six-target set), recorded to `readiness/route-inner-loop.txt`

**Checkpoint**: A routine framework change routes light through the real `Route` entry point.

---

## Phase 4: User Story 2 (US2) — a consumer-contract change escalates automatically (P1)

*Independent test*: a `template/base/**` edit escalates and prints
`TemplateCheck` + `GeneratedProductCheck`; an `src/**/*.fsi` edit additionally
requires `PackageSurfaceCheck`; a mixed diff takes the highest tier; an
unrecognised path default-denies to the broad fallback (never an empty success).

### Tests First (Principle I, Principle VI)

- [X] T012 [P] [US2] [skillist: fsharp-build-orchestration] Extend `RoutingTests.fs` with the escalation cases over literal `Diff` values: `src/Lib/Foo.fsi` escalates with `PackageSurfaceCheck` in the gates; `template/base/x` escalates with `TemplateCheck` + `GeneratedProductCheck`; `.specify/templates/x` escalates (generated-guidance); a mixed `src/Scene/Foo.fs` + `template/base/x` diff resolves to the **highest** tier (never `InnerLoop`); an unknown `weird/path.txt` default-denies to `Verify` (never empty); a `ConsumerAgent` floor case — `select ConsumerAgent { ChangedPaths = ["docs/x.md"] }` → `FocusedAuthority` while the same diff under `FrameworkAuthor` → `InnerLoop`; and a broadened-coverage case — `template/capabilities.yml` and `.specify/extensions.yml` now escalate (F2) — all assert typed `Selection` values (SC-002, SC-004 / FR-010)

### Implementation

- [X] T013 [US2] [skillist: []] Capture SC-002 evidence: `./fake.sh build -t Route` on a `template/base/**` tree (escalated gate set incl. `TemplateCheck` + `GeneratedProductCheck`), on an `src/**/*.fsi` tree (adds `PackageSurfaceCheck`), and on an unknown path (broad fallback, never empty), recorded to `readiness/route-escalation.txt` — escalation is already implemented in `select` (T009) and printed by the edge (T010); this story adds the representative test cases and the captured transcripts

**Checkpoint**: Consumer-contract changes escalate by what changed, not by remembering prose.

---

## Phase 5: User Story 3 (US3) — `Route --enforce` blocks an under-evidenced change (P1)

*Independent test*: simulate an `src/**/*.fsi` diff without
`readiness/package-surface-expectations.md`; `Route --enforce` exits non-zero
naming that artifact and the requiring tier; add the artifact and it exits zero.

### Tests First (Principle I, Principle VI)

- [X] T014 [P] [US3] [skillist: fsharp-build-orchestration] Add failing tests for the pure `--enforce` core: `unmetArtifacts present (select FrameworkAuthor {fsi-diff})` returns `package-surface-expectations.md` when absent from `present` and `[]` when present; `enforceDiagnostic` names each missing artifact and the requiring tier — typed assertions, no shelling (SC-003)

### Implementation

- [X] T015 [US3] [skillist: fsharp-build-orchestration] Wire the `--enforce` mode at the `Route` edge in `build.fsx`: build `present` from `File.Exists` over the selected tier's expected artifacts (edge I/O), call the pure `unmetArtifacts`, and on a non-empty result exit non-zero printing `enforceDiagnostic`; in non-enforce mode print the gate list and never fail (FR-005)
- [X] T016 [US3] [skillist: []] Capture SC-003 evidence: `Route --enforce` on a simulated `src/**/*.fsi` change lacking `readiness/package-surface-expectations.md` exits non-zero naming that artifact; once the artifact is present it exits zero; both transcripts recorded to `readiness/route-enforce.txt`

**Checkpoint**: The escalated contract is enforced, not merely documented.

---

## Phase 6: User Story 5 (US5) — dogfood features still exercise the full harness (P2)

*Independent test*: a feature carrying a `dogfood` id resolves through `Route` to
the full gate set even when its only change is a `src/Scene/*.fs` edit that would
otherwise route `inner-loop`.

### Tests First (Principle I, Principle VI)

- [X] T017 [P] [US5] [skillist: fsharp-build-orchestration] Add the dogfood case to `RoutingTests.fs`: `selectForFeature FrameworkAuthor "042" { ChangedPaths = ["src/Scene/Foo.fs"] }` resolves to `fullPipelineGates` / `MaintainerVerify` with `DogfoodForced = true`, even though the same diff routes `InnerLoop` through `select`; assert typed values (SC-005, SC-004 / FR-010)

### Implementation

- [X] T018 [US5] [skillist: []] Capture SC-005 evidence: with feature `042` active (it is in `dogfoodFeatureIds`), `./fake.sh build -t Route` on a would-be inner-loop `src/Scene/*.fs` tree resolves to the full gate set, recorded to `readiness/route-dogfood.txt` — the dogfood override is already resolved at the edge via `selectForFeature` + `activeFeatureId` (T010); this story adds the test case and the captured transcript

**Checkpoint**: The consumer-grade pipeline stays continuously exercised and cannot rot.

---

## Phase 7: Single source of truth — generated `validation.contract.yml` (FR-007)

*Independent test*: `ContractView.render` reproduces the on-disk contract
byte-for-byte; a hand-edit makes the currency check fail with a "regenerate from
`Routing.fs`" diagnostic; regenerating restores currency.

### Tests First (Principle I, Principle VI)

- [X] T019 [P] [T1] [skillist: fsharp-build-orchestration] Add failing `tests/Governance.Tests/ContractViewTests.fs`: `currencyDrift (render rules dogfoodFeatureIds) rules dogfoodFeatureIds = None`, and `currencyDrift <hand-mutated text> rules dogfoodFeatureIds = Some _`; register in `Governance.Tests.fsproj` before `Program.fs` (fails before `ContractView.fs` is implemented; SC-007)

### Implementation

- [X] T020 [T1] [skillist: fsharp-code-generation] Implement `build/Governance/ContractView.fs` against its `.fsi`: the deterministic `render` (schema header, defaults, tiers, `routing_rules` from `Routing.rules`, dogfood ids — stable ordering so byte-equality is the contract) and the pure `currencyDrift`; fold `currencyDrift` **detection** into the existing `TargetMetadataDrift` body and `render` **regeneration** into the existing `RefreshSurfaceBaselines` body at the `build.fsx` edge (research R1) — no new FAKE target beyond `Route`
- [X] T021 [T1] [skillist: []] Capture SC-007 evidence: run `./fake.sh build -t RefreshSurfaceBaselines` to (re)emit `validation.contract.yml` from `Routing.fs`; demonstrate that a scratch hand-edit is rejected by `TargetMetadataDrift` with the regenerate diagnostic and accepted once regenerated, recorded to `readiness/contract-currency.md`

**Checkpoint**: `Routing.fs` is the sole source; the retained contract cannot silently drift.

---

## Phase 8: User Story 4 (US4) — the agent reads a gate list, not 23,000 lines of prose (P2)

*Independent test*: `CLAUDE.md` and `AGENTS.md` instruct "run `Route` first; run
only the gates it prints" and present the serialized six-target order as the
escalated/maintainer-verify path rather than the universal default; a guidance
test asserts both.

### Implementation

- [X] T022 [P] [US4] [skillist: []] Update `CLAUDE.md` and `AGENTS.md` to instruct **"run `Route` first; run only the gates it prints,"** and reframe the blanket serialized six-target order as the `maintainer-verify`/escalated path reserved for consumer-contract and dogfood work — no longer the unconditional default (FR-008)
- [X] T023 [US4] [skillist: fsharp-build-orchestration] Update `tests/Governance.Tests/SequentialFakeGuidanceTests.fs` to assert both guidance files contain the `Route`-first instruction and no longer present the six-target order as the unconditional default (FR-008, SC-008)
- [X] T024 [P] [US4] [skillist: []] Document the tiered process and the `Route` entry point — the tiers, the framework-author/consumer-agent axis, how `Route` selects, and `--enforce` — in `docs/reports/build.md` and `docs/reports/speckit.md` (FR-009)
- [X] T025 [US4] [skillist: []] Capture SC-008 evidence: excerpts confirming the `Route`-first instruction and reframed six-target order in `CLAUDE.md` + `AGENTS.md`, the passing guidance test, and the new `docs/reports/build.md` + `docs/reports/speckit.md` sections, recorded to `readiness/guidance.md`

**Checkpoint**: Guidance points the agent at `Route` first; the six-target order is the escalated path.

---

## Phase 9: Integration & Polish

- [X] T026 [P] [T1] [skillist: []] SC-006: grep over the build/library projects proves no `select-tier.fsx`, no `dotnet fsi` selector, and no `FSharp.Compiler.*` dependency is introduced — the routing logic is compiled F# in `FS.Skia.UI.Build`; recorded to `readiness/no-fsx-fsi-fcs.md`
- [X] T027 [P] [T1] [skillist: []] SC-009: confirm `git diff` over `src/**` is empty (runtime untouched), `PackageSurfaceCheck` and `FsiTranscripts` show no product baseline diff (FR-013), and no new `PackageVersion` exists outside `Directory.Packages.props` (FR-012/FR-014); recorded to `readiness/src-untouched.md`
- [X] T028 [T1] [skillist: []] As a designated dogfood feature (FR-015), run the full serialized FAKE gate sequence in deterministic order, never concurrently — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`, then the final graph and audit gates (T029/T030) — recording aggregate FAKE results as **non-authoritative** and rerunning any race-like or environment-flaky gate failure (documented 039 `FsiTranscripts`/`SkiaViewer.Tests` flakes) in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T029 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm the task graph is acyclic, no dangling refs, no `[S*]` surprises, and that the `skillist` metadata and visible mirrors are valid
- [X] T030 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS with no synthetic evidence to accept (this feature ships none)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This feature ships
**none** (plan: Synthetic evidence — None planned). The selector unit inputs are
real crafted `Diff` values exercising real routing/escalation/default-deny/dogfood
paths, and the scratch hand-edit in T021 is a structural currency-check proof
(SC-007), not a shipped synthetic fixture.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
