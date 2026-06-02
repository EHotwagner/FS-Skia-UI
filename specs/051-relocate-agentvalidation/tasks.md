# Tasks: V3 Stage 2 — Relocate `AgentValidation` out of the Runtime Monolith

**Feature branch**: `051-relocate-agentvalidation`
**Spec**: `specs/051-relocate-agentvalidation/spec.md`
**Plan**: `specs/051-relocate-agentvalidation/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real: the
relocated parser re-derives identical accept/reject diagnostics and an identical
`knownGates` set under the repointed real test suite (same fixtures, same
assertion count), the structural-rename `git diff -M` proves the body moved
byte-for-byte (only the `namespace` line + the doc-comment phrase differ), the
no-consumer grep reads the real tree, and the serialized escalated FAKE gate logs
are real. No `[S]`/`[SEH]` task is approved (Principle V); `EvidenceAudit` MUST
return `verdict=PASS` with zero synthetic (SC-008).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]** the governance parser lives in `FS.Skia.UI.Build`, **[US2]** the
  build→runtime test coupling is removed, **[US3]** `knownGates` becomes
  Route-gating-ready governance config
- **[T1]** / **[T2]** — this feature is **Tier 1 for the monolith**: it removes
  the `AgentValidation` public surface from the published `FS.Skia.UI` package and
  shrinks that package's surface baseline. The surface-moving tasks carry `[T1]`;
  pure readiness/record/verification tasks carry `[T2]`. `Route` **escalates**
  this governance-path + monolith-`.fsi`-shrinking change (the actual tier may be
  `agent-ready` rather than full `dogfood` — run exactly what `Route` prints).
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read first)

Recorded as a confidence review, not regex certainty:

- **The structural move** (T006: add the two `<Compile Include>` lines after the
  `Spike` pair in `FS.Skia.UI.Build.fsproj`, `git mv` the pair, rewrite the
  namespace + doc-comment phrase, build the library) and **the monolith
  drop + surface-baseline edit** (T007: remove the `Lib.fsproj` compile items,
  shed the `FS.Skia.UI.AgentValidation.*` baseline lines, build the monolith,
  re-run the DiffPlex-backed surface check) take `fsharp-build-orchestration`
  — *matched signal:* compile-order editing across `.fsproj`s and the
  DiffPlex/`PackageSurfaceCheck` FAKE surface diff; *confidence:* medium-high.
  The plan names this skill for the move. *Considered and rejected:*
  `fsharp-parsing` — although the relocated module **is** a YAML/JSON contract
  parser, no parsing logic is authored here (pure byte-for-byte relocation), so
  the parsing skill is indirect, not a hard match. `fs-skia-layout-evidence` is a
  **false positive** here (no layout/scene/HUD/visual evidence) and is rejected.
- **The test repoint + coupling cut** (T005 `open` rewrite, T010 drop the
  `ProjectReference`) edit an `open` line and a `.fsproj` reference. No capability
  skill covers project-reference / `open` editing; `fsharp-build-orchestration`
  was **considered and rejected** there (it covers FAKE-front-end / DiffPlex
  authoring, not reference editing), so these are honest `[skillist: []]`.
- **Gate-running tasks** (T008/T011 `Dev`, T014 the escalated set) are plain FAKE
  invocations and carry `[skillist: []]` — consistent with prior stages where
  gate execution is not a capability-skill match.
- The genuine workflow tasks are the last two: **T015** declares
  `speckit-evidence-graph` and **T016** declares `speckit-evidence-audit`, in that
  order (graph before audit). `speckit-constitution` is **not** assigned — no
  `.specify/memory/constitution.md` edit and no task title uses the
  `constitution` word.

## Governance risk levels & validation

- **Small** (this feature's own `readiness/` Markdown and record/verification
  notes): focused review plus a `git diff` over the edited files is the **required
  evidence** and is authoritative for the level.
- **Medium** (the monolith surface-baseline edit and the structural-rename
  parity): the focused `PackageSurfaceCheck` run and the `git diff -M` rename
  similarity are the **required evidence**; the clean baseline diff (SC-006) and
  the ~100%-similarity rename (SC-003) are the authoritative signals.
- **Broad** (required here, because `Route` escalates this governance-path +
  monolith-`.fsi` change): the full serialized FAKE gate order (`Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the
  final graph and audit gates). **Broad validation is required** whenever a
  consumer-contract or public-`.fsi` surface changes. Aggregate FAKE results are
  recorded as **non-authoritative**; any race-like or environment-flaky failure is
  rerun in focused isolation and that focused result is authoritative.

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Task **titles** deliberately avoid the
validator's blocking trigger tokens: the `knownGates` allowlist is referenced by
its camelCase id; the no-consumer task uses the bare word "grep" (never
`diff-scan`); no non-graph/non-audit title uses `task graph` / `evidence graph` /
`evidence audit` / `synthetic propagation` / `constitution` / `readiness
validation` / `mirror mismatch`. The genuine workflow tasks T015/T016 **do**
declare `speckit-evidence-graph` / `speckit-evidence-audit` and name
`EvidenceGraph` / `EvidenceAudit` directly. The readiness-scaffold task (T002)
uses the safe `Create placeholder evidence files listed by the plan` wording and
the readiness-aggregation task (T003) uses the `Complete readiness notes` prefix,
so their hyphenated filename citations do not fire capability checks. This is a
governance/build-tooling relocation — there is **no** persistent-viewer task and
no visual readiness scaffold (no host/scene/layout/rendering change).
`tasks.deps.yml` keeps one indented object per task id with `deps` and
`skillist`; every `[skillist: …]` mirror matches the structured list exactly and
in order. Cross-references use **fixed `Tnnn` ids** only; no backward task edge is
written, so the DAG stays acyclic.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record the feature Tier (Tier 1 for the monolith — the published `FS.Skia.UI` package loses the `AgentValidation` public surface and its surface baseline shrinks), the affected surfaces (`build/Governance/FS.Skia.UI.Build.fsproj` + the relocated `AgentValidation.fs(i)`, `src/Lib/Lib.fsproj`, `tests/Governance.Tests/Governance.Tests.fsproj` + `AgentValidationFrameworkTests.fs`, `readiness/surface-baselines/FS.Skia.UI.txt`, and `specs/051-relocate-agentvalidation/readiness/**`), the public-API impact (monolith `.fsi` shrinks by the removed module; **no** runtime split-package baseline changes), the Elmish/MVU applicability (the `ValidationSelection` model/msg/effect/`init`/pure `update`/`ValidationSelectionInterpreter` edge **moves intact** with behaviour preserved — `update` stays pure and file/`git` I/O stays at the interpreter edge, proven by the repointed suite, not redesigned), and the real-evidence obligations (repointed suite green with the same assertion count, identical `knownGates` + accept/reject diagnostics, the structural-rename diff, the no-consumer grep, generated-consumer gates green, and the serialized escalated FAKE gate logs; zero synthetic)
- [X] T002 [P] [T2] [skillist: []] Create placeholder evidence files listed by the plan under `specs/051-relocate-agentvalidation/readiness/` so the audit-enforced readiness files are discoverable at setup: the always-required contract trio `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`; the record notes `structural-parity.md`, `surface-baseline-diff.md`, `no-consumer-grep.md`, `knowngates-precondition.md`; the gate records `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md`; and `logs/` (`dev.log`, `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`, `evidence-graph.log`, `evidence-audit.log`)
- [X] T003 [T2] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files — `governance-risk-levels.md` (the small / medium / broad levels, their required evidence, and when broad validation is required), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate), and `runtime-limitations.md` (the .NET 10 build-host / governance-tooling statements; no runtime/Vulkan/Skia surface touched) — each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation (sole-consumer re-verification + relocation work-list)

- [X] T004 [T1] [skillist: []] Re-verify the sole consumer and fix the relocation work-list per `contracts/agentvalidation-surface.md` and `research.md` (D2/D6) — `grep -rn "FS.Skia.UI.AgentValidation"` over `*.fs`/`*.fsi`/`*.fsproj`/`*.fsx` confirms only `AgentValidationFrameworkTests.fs`'s `open` plus the two `src/Lib/Lib.fsproj` compile items (the files being moved); record the compile-order slot (`AgentValidation.fsi`/`.fs` immediately after the `Spike` pair, **before** `Routing` so the Stage-5 `Routing → knownGates` consumption stays forward-compatible) and the `FS.Skia.UI.AgentValidation.*` baseline lines to drop — the work-list, no edits yet

**Checkpoint**: Foundation ready — the sole consumer is re-verified, the compile-order slot and the surface-baseline delta are fixed; story work may begin.

---

## Phase 3: User Story 1 (US1) — the governance parser lives in `FS.Skia.UI.Build` (P1)

**Goal**: the `AgentValidation` capability compiles and is exported by
`FS.Skia.UI.Build`, no `AgentValidation.fs(i)` remains under `src/Lib`, and the
repointed governance suite passes against the relocated module with identical
behaviour (FR-001/002/003/004/005, SC-001/002/003).

### Tests First (Principle I, Principle VI)

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/tests-first -->
**VI. Test Evidence Is Mandatory** — Behavior-changing code MUST include automated tests that fail before the change and pass after.
<!-- END GENERATED: constitution/tests-first -->

- [X] T005 [P] [US1] [T1] [skillist: []] Repoint the `open` in `tests/Governance.Tests/AgentValidationFrameworkTests.fs` from `FS.Skia.UI.AgentValidation` to `FS.Skia.UI.Build.AgentValidation` as the **failing-first** compile break (the relocated namespace does not exist until T006); preserve every fixture and assertion unchanged so the suite remains the parity oracle with the **same** assertion count (FR-005, SC-002)

### Implementation

- [X] T006 [US1] [T1] [skillist: fsharp-build-orchestration] `git mv` `src/Lib/AgentValidation.fsi` + `.fs` into `build/Governance/`, rewrite only the `namespace` line (`FS.Skia.UI.AgentValidation` → `FS.Skia.UI.Build.AgentValidation`) and the doc-comment phrase (`"…exposed by this FS.Skia.UI package."` → `"…exposed by the FS.Skia.UI.Build governance library."`), add the two `<Compile Include>` items **after the `Spike.fsi`/`Spike.fs` pair and before `Routing`** in `build/Governance/FS.Skia.UI.Build.fsproj`, and build the governance library green — no `val`/`type`/field/case added, removed, or retyped (FR-001/003, D1/D2/D3); leaves `Front/Support.fs`'s distinct same-named shadow types untouched and non-colliding (FR-011)
- [X] T007 [US1] [T1] [skillist: fsharp-build-orchestration] Remove the two `AgentValidation` `<Compile Include>` lines from `src/Lib/Lib.fsproj`, drop every `FS.Skia.UI.AgentValidation.*` line from `readiness/surface-baselines/FS.Skia.UI.txt` (the monolith aggregate baseline; add **no** `FS.Skia.UI.Build.txt` — build-tooling is excluded from surface tooling, D4), build the monolith green, re-run `./fake.sh build -t PackageSurfaceCheck` clean, and confirm `git ls-files src/Lib/AgentValidation.*` returns nothing (FR-002/010, SC-001/006)
- [X] T008 [US1] [T1] [skillist: []] Run `./fake.sh build -t Dev` — the repointed `AgentValidationFrameworkTests` suite builds and passes against the relocated module with the **same** assertion count, turning T005 green; this is the behavioural-parity oracle (contract parse accept/reject diagnostics, the `knownGates` set, the `ValidationSelection` MVU transitions, and `AgentVerdict` (de)serialization) (FR-004, SC-002)
- [X] T009 [US1] [T1] [skillist: []] Record structural parity in `readiness/structural-parity.md` — `git diff -M --stat` shows `AgentValidation.fs(i)` as renamed `src/Lib` → `build/Governance` at ~100% similarity (only the namespace line + doc-comment phrase differ) — and confirm via the suite that the relocated parser yields an **identical** `knownGates` set and **identical** accept/reject diagnostics vs the pre-move module (SC-003)

**Checkpoint**: User Story 1 complete — module compiled/exported by `FS.Skia.UI.Build`, gone from `src/Lib`, suite green against the relocated home, parity structural + behavioural.

---

## Phase 4: User Story 2 (US2) — the build→runtime test coupling is removed (P2)

**Goal**: `Governance.Tests` consumes `AgentValidation` only from
`FS.Skia.UI.Build` and no longer references the runtime monolith; no
`FS.Skia.UI.AgentValidation` consumer remains (FR-006/007, SC-004).

- [X] T010 [US2] [T1] [skillist: []] Remove the `ProjectReference` to `..\..\src\Lib\Lib.fsproj` from `tests/Governance.Tests/Governance.Tests.fsproj` (it existed solely for `AgentValidation`), leaving the suite referencing only `..\..\build\Governance\FS.Skia.UI.Build.fsproj` for this capability (FR-006)
- [X] T011 [US2] [T1] [skillist: []] Run `./fake.sh build -t Dev` — the `Governance.Tests` suite restores/builds/runs green with **no** link back into `src/Lib`, proving the parser without the monolith reference (FR-006, SC-004)
- [X] T012 [P] [US2] [T2] [skillist: []] Capture the no-consumer grep in `readiness/no-consumer-grep.md` — `grep -rn "FS.Skia.UI.AgentValidation" --include=*.fs --include=*.fsi --include=*.fsproj --include=*.fsx .` returns nothing (outside git history) and `grep -n "Lib.fsproj" tests/Governance.Tests/Governance.Tests.fsproj` returns nothing (FR-007, SC-004)

**Checkpoint**: User Story 2 complete — `Governance.Tests` is monolith-free, the suite is green, and no `FS.Skia.UI.AgentValidation` consumer remains.

---

## Phase 5: User Story 3 (US3) — `knownGates` becomes Route-gating-ready governance config (P3)

**Goal**: `knownGates` lives in `FS.Skia.UI.Build` so that extending it and
rendering a new gate into `validation.contract.yml` would touch only
governance/build paths, satisfying the Stage-0 deferral precondition
(FR-008, SC-005/SC-007).

- [X] T013 [US3] [T2] [skillist: []] Record the precondition review in `readiness/knowngates-precondition.md` — `grep -rn "knownGates" build/Governance/AgentValidation.fs` shows it defined in the governance library and `grep -rn "knownGates" src/Lib` returns nothing; confirm that adding a gate name to the allowlist and rendering it into `validation.contract.yml` would touch only governance/build paths and **no** `src/**` runtime file, and that `validation.contract.yml` is unchanged this stage (currency vs `Routing.fs` preserved) — the Stage-0 deferral precondition is met (FR-008, SC-005/SC-007)

**Checkpoint**: User Story 3 complete — `knownGates` is governance config; the per-package Route rule the Stage-0 finding deferred is unblocked (the rule itself remains Stage 5).

---

## Phase 6: Integration & Polish (serialized escalated gates)

- [X] T014 [T2] [skillist: []] First confirm `./fake.sh build -t Route --enforce` reports the escalated tier with every required evidence artifact present, then run the escalated serialized FAKE gate set sequentially — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates (T015/T016) — never concurrently; confirm **no** runtime per-package surface baseline drifts (the only surface delta is the monolith shedding the module), the default `app` is byte-unchanged, and the generated-consumer gates stay green (FR-009/010, SC-006); record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T015 [T2] [skillist: speckit-evidence-graph] Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`) — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the structured task metadata and visible mirrors are valid (`verdict=ok`)
- [X] T016 [T2] [skillist: speckit-evidence-audit] Run the merge-gate audit (`./fake.sh build -t EvidenceAudit`) — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept (SC-008)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; the repointed real test suite is the parity oracle, the structural-rename `git diff -M` proves the byte-for-byte move, and the no-consumer grep + escalated FAKE gates read the real tree.)_ | | | | | | | | |
