# Tasks: V3 Stage 5 Closeout — Delete `src/Lib`, Decommission `FS.Skia.UI`, Enforce & Measure

**Feature branch**: `053-v3-monolith-retirement`
**Spec**: `specs/053-v3-monolith-retirement/spec.md`
**Plan**: `specs/053-v3-monolith-retirement/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit.

**This feature ships zero synthetic evidence.** Every signal is real: the
rewritten `Package.Tests` packaging-contract suite asserts the live split-package
pack shape; the deletion is a real `git rm` proven by a real repo-wide grep; the
`PerPackageSurfaceDiff` enforcement proof is a **real reverted `.fsi` edit** that
fails the gate; the cleanliness gate runs against a real generated `app`; and the
serialized escalated FAKE gate logs are real. No `[S]`/`[SEH]` task is approved
(Principle V); `EvidenceAudit` MUST return `verdict=PASS` with zero synthetic
(SC-009). Reference-screenshot re-capture stays headless-GPU-infeasible and is
disclosed as a Principle V infeasibility (corroboration-only — the deterministic
scene-output oracle preserved in the split-package suites is authoritative); it is
**not** synthetic evidence.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]** the monolith is gone (`src/Lib` deleted, `FS.Skia.UI` unpublished),
  **[US2]** the per-package surface baselines are an enforced merge gate,
  **[US3]** a generated `app` is asserted clean,
  **[US4]** a V2 consumer has a migration guide
- **[T1]** / **[T2]** — this feature is **Tier 1 (contracted change)**: it removes
  the public `FS.Skia.UI` package identity, changes `validation.contract.yml` (a
  contract artifact), and edits public-`.fsi` routing. Surface/contract-moving
  tasks carry `[T1]`; pure readiness/record/verification tasks carry `[T2]`.
  `Route` **escalates** this change (it touches governance `Routing.fs`,
  public-`.fsi` routing, the pack flow, and dependency docs) — run exactly the
  gates `Route` prints.
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read first)

Recorded as a confidence review, not regex certainty:

- **The packaging-contract test rewrite** (T005 re-authors `Package.Tests` against
  the split-package `packProjects`/`PackLocal` shape) takes
  `fsharp-build-orchestration` — *matched signal:* it asserts the pack/`PackLocal`
  packaging contract and the split-package pack entries; *confidence:* medium. The
  plan names this surface as a build/pack contract.
- **The `PerPackageSurfaceDiff` Route-gating wiring** (T014 adds the gate to the
  `package-surface` rule's `RequiredGates` + `knownGates` and regenerates the
  contract) and **the enforcement proof** (T016 runs the DiffPlex-backed
  per-package surface check and proves an unrecorded `.fsi` edit fails it) take
  `fsharp-build-orchestration` — *matched signal:* routing-rule/`knownGates`
  governance config + the `PerPackageSurfaceDiff` DiffPlex surface diff;
  *confidence:* medium-high.
- **The cleanliness-gate authoring** (T017 extends `GeneratedProductCheck` with a
  forbidden-content scan over a generated `app`) takes `fsharp-io-globbing` —
  *matched signal:* fnmatch-style forbidden-path globbing + generated-tree file
  discovery in compiled F#; *confidence:* medium. *Considered and rejected:*
  `fs-skia-template-update` — it refreshes/validates the template after package
  changes, it does not author a governance validator; the template content is
  already clean (no monolith pin), so no template-update work applies here.
- **Reference-editing, deletion, doc-authoring, and gate-run tasks** (T006 helper
  removal + ref drop, T007 path-string sweep, T008 `git rm`, T009 unpublish, T010
  pin verify, T011/T015/T018 gate runs, T012 grep, T019 migration doc, T020 ADR,
  T021 ParityGallery policy, T022 after-baseline, T023 escalated gates) carry
  `[skillist: []]`. *Considered and rejected:* `fs-skia-layout-evidence` for T021
  — the Scene-only scene-output oracle is **preserved** per ADR 0010, not
  re-derived; T021 only records the decision and cleans stale governance list
  entries, so the capability skill is indirect. `fsharp-code-generation` was
  **considered and rejected** for T014 — `validation.contract.yml` is regenerated
  by the existing rendering target, not hand-authored by this task.
- The genuine workflow tasks are the last two: **T024** declares
  `speckit-evidence-graph` and **T025** declares `speckit-evidence-audit`, in that
  order (graph before audit). `speckit-constitution` is **not** assigned — no
  `.specify/memory/constitution.md` edit this stage.

## Governance risk levels & validation

- **Small** (this feature's own `readiness/` Markdown and record/verification
  notes): focused review plus a `git diff` over the edited files is the **required
  evidence** and is authoritative for the level.
- **Medium** (the rewritten packaging-contract suite, the `PerPackageSurfaceDiff`
  Route-gating + its enforcement proof, the cleanliness gate, and the after-
  measurement metrics): the focused `Dev`/`PerPackageSurfaceDiff`/
  `TargetMetadataDrift`/`GeneratedProductCheck` runs and the named grep are the
  **required evidence** and the authoritative signals for the level.
- **Broad** (required here, because `Route` escalates this governance + public-
  `.fsi` + pack-flow change): the full serialized FAKE gate order (`Dev` →
  `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final
  graph and audit gates). **Broad validation is required** whenever a consumer-
  contract, governance, or public-`.fsi` surface changes. Aggregate FAKE results
  are recorded as **non-authoritative**; any race-like or environment-flaky
  failure is rerun in focused isolation and that focused result is authoritative.

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Task **titles** deliberately avoid the
validator's blocking trigger tokens: the no-consumer task uses the bare word
"grep" (never `diff-scan`); the migration-doc task says "migration guide/doc"
(never `migration blocker`); no non-graph/non-audit title uses `task graph` /
`evidence graph` / `evidence audit` / `synthetic propagation` / `constitution` /
`readiness validation` / `mirror mismatch`. The genuine workflow tasks T024/T025
**do** declare `speckit-evidence-graph` / `speckit-evidence-audit` and name
`EvidenceGraph` / `EvidenceAudit` directly. The readiness-scaffold task (T002)
uses the safe `Create placeholder evidence files listed by the plan` wording and
the readiness-aggregation task (T003) uses the `Complete readiness notes` prefix,
so their hyphenated filename citations do not fire capability checks. This feature
makes **no** persistent-viewer or window-visibility change and needs **no** visual
readiness scaffold (the scene-output parity is headless-deterministic and already
preserved; reference-frame re-capture stays headless-GPU-infeasible and is
disclosed, not synthetic). `tasks.deps.yml` keeps one indented object per task id
with `deps` and `skillist`; every `[skillist: …]` mirror matches the structured
list exactly and in order. Cross-references use **fixed `Tnnn` ids** only; no
backward task edge is written, so the DAG stays acyclic.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record the feature Tier (Tier 1 contracted change — the public `FS.Skia.UI` package identity is removed, `validation.contract.yml` changes, public-`.fsi` routing is edited), the affected surfaces (`src/Lib/Library.fs(i)` + `InternalsVisibleTo.fs` + `Lib.fsproj` deleted; `tests/Package.Tests`, `tests/Governance.Tests/{AsteroidsFeedbackSkillGuidanceTests,DependencyGovernanceTests,RuntimeOrganizationTests,PublicRecordInvariantTests,ControlsBoundaryCompositionTests,AgentValidationFrameworkTests,RoutingTests}`, `tests/Controls.Tests/DiagnosticsTests`, `build/Governance/{Routing.fs,AgentValidation.fs,PerPackageSurface.fs,GeneratedProduct.fs,Front/Helpers.fs}`, `FS-Skia-UI.sln`, `validation.contract.yml`, `docs/{migration,adr,reports}`, and `specs/053-v3-monolith-retirement/readiness/**`), the public-API impact (the `Parity`/`ParityReport` monolith surface is deleted; the `package-surface` rule gains `PerPackageSurfaceDiff` in `required_gates`; no split-package `.fsi` moves), the Elmish/MVU applicability (**N/A** — no stateful/I/O workflow, command, effect, subscription, or interpreter behaviour changes this stage; all runtime moved and was parity-proven in Stages 1–4), and the real-evidence obligations (rewritten packaging-contract suite green, repo-wide no-consumer grep, real reverted `PerPackageSurfaceDiff` enforcement proof, cleanliness gate green on a generated `app`, the migration doc + ADR 0012 + after-baseline, and the serialized escalated FAKE gate logs; zero synthetic)
- [X] T002 [P] [T2] [skillist: []] Create placeholder evidence files listed by the plan under `specs/053-v3-monolith-retirement/readiness/` so the audit-enforced readiness files are discoverable at setup: the always-required contract trio `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`; the record notes `consumer-work-list.md`, `no-consumer-grep.md`, `per-package-surface-enforcement.md`, `cleanliness-gate.md`, `acyclic-graph-proof.md`, `paritygallery-policy.md`, `closeout-docs.md`; the gate records `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md`; and `logs/` (`dev.log`, `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`, `per-package-surface-diff.log`, `target-metadata-drift.log`, `evidence-graph.log`, `evidence-audit.log`)
- [X] T003 [T2] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files — `governance-risk-levels.md` (the small / medium / broad levels, their required evidence, and when broad validation is required), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate), and `runtime-limitations.md` (the .NET 10 desktop / Vulkan / SkiaSharp preview statements; unsupported macOS/mobile/browser; no software-renderer fallback; no runtime behaviour changes this stage — deletion + governance only; reference-frame re-capture stays headless-GPU-infeasible, disclosed not synthetic) — each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation (consumer work-list + acyclic-edge proof)

- [X] T004 [T2] [skillist: []] Re-verify the consumer work-list and the acyclic package edge per `research.md` (R1/R2) and record it in `readiness/consumer-work-list.md` — confirm the only `ProjectReference` consumer of the monolith is `tests/Package.Tests` and enumerate every **path-string** call site to clear (the ~14 sites: `Package.Tests/Tests.fs`, `AsteroidsFeedbackSkillGuidanceTests.fs`, `DependencyGovernanceTests.fs`, `RuntimeOrganizationTests.fs`, `PublicRecordInvariantTests.fs`, `ControlsBoundaryCompositionTests.fs`, `AgentValidationFrameworkTests.fs`, `RoutingTests.fs`, `Controls.Tests/DiagnosticsTests.fs`, `Front/Helpers.fs`, `Routing.fs:214`, `PerPackageSurface.fs:29`, `GeneratedProduct.fs`, `FS-Skia-UI.sln`); confirm `src/Lib` holds only `Library.fs(i)` + `InternalsVisibleTo.fs` (no Vulkan/KeyboardInput/AgentValidation residue) and that removing it leaves the package dependency graph acyclic with `FS.Skia.UI.Scene` FSharp.Core-only (record in `readiness/acyclic-graph-proof.md`) — the work-list, no edits yet

**Checkpoint**: Foundation ready — the last consumer and every path-string site are enumerated, the residue and acyclic edge confirmed; story work may begin.

---

## Phase 3: User Story 1 (US1) — the monolith is gone (P1)

**Goal**: the last consumer is decoupled, `src/Lib` is deleted and out of the
solution, `FS.Skia.UI` is unpublished, and a repo-wide grep shows zero monolith
references with the full suite green (FR-001/002/003/004/005/006, SC-001/002/003).

### Tests First (Principle I, Principle VI)

- [X] T005 [P] [US1] [T1] [skillist: fsharp-build-orchestration] Rewrite `tests/Package.Tests/Tests.fs` packaging-contract assertions against the split packages as the **failing-first** change — replace the `typeof<FS.Skia.UI.ParityReport>.Assembly` / `VulkanResources`/`VulkanStartup` non-export / `PackLocal` `src/Lib/Lib.fsproj` → `FS.Skia.UI` expectations with assertions over the nine split-package pack entries (and keep a real negative such as "Controls does not depend on `..\Lib\Lib.fsproj`"); the suite must go **red on the old monolith expectation before the rewrite, then green** and still assert a real packaging contract (FR-001, SC-003)
### Implementation

- [X] T006 [US1] [T1] [skillist: []] Remove the `Parity` evidence helper and the `ParityStatus`/`EvidenceType`/`ParityEvidenceItem`/`ParityReport` types from `src/Lib/Library.fs` + `Library.fsi` (FR-002), and drop the conditional `..\..\src\Lib\Lib.fsproj` `ProjectReference` from `tests/Package.Tests/Package.Tests.fsproj` (FR-001) — `Package.Tests` now references split packages only
- [X] T007 [P] [US1] [T1] [skillist: []] Path-string sweep across `tests/**` and `build/**` (the Stage-2 lesson — a deleted file is referenced by *path*, not just symbol): **drop** the monolith enumerations (`AsteroidsFeedbackSkillGuidanceTests.fs` packable row, `DependencyGovernanceTests.fs` `src/Lib/Lib.fsproj` entries, `RuntimeOrganizationTests.fs` `src/Lib/Library.fs`, `PublicRecordInvariantTests.fs` `src/Lib/Library.fsi`, `ControlsBoundaryCompositionTests.fs` `"src/Lib"`, `AgentValidationFrameworkTests.fs` stale `src/Lib/AgentValidation.fsi` rule input); **repoint** the generic `.fsi`-example inputs in `RoutingTests.fs` (`src/Lib/Foo.fsi` → `src/Scene/Foo.fsi`); **triage** the `Controls.Tests/DiagnosticsTests.fs` and `build/Governance/GeneratedProduct.fs` diagnostic-string examples (keep those that survive deletion, repoint any that must name a living path) (FR-006)
- [X] T008 [US1] [T1] [skillist: []] `git rm` `src/Lib/Library.fs`, `src/Lib/Library.fsi`, `src/Lib/InternalsVisibleTo.fs`, and `src/Lib/Lib.fsproj`, remove the `Lib` project entry from `FS-Skia-UI.sln`, and `git rm` the monolith's aggregate surface baseline `readiness/surface-baselines/FS.Skia.UI.txt` (the aggregate `PackageSurfaceCheck` baseline retires with the package, per the spec's Public-contract-impact prompt) — `src/Lib` no longer exists on disk and is not in the solution and `PackageSurfaceCheck` no longer enumerates the deleted monolith assembly (FR-003); confirm `git ls-files src/Lib` returns nothing and `readiness/surface-baselines/` lists the nine split packages only
- [X] T009 [US1] [T1] [skillist: []] Stop publishing the monolith — remove the `("src/Lib/Lib.fsproj", "FS.Skia.UI")` entry from `packProjects` (`build/Governance/Front/Helpers.fs`) and the pack-version flow so the list is the nine split packages + `FS.Skia.UI.Build`, and drop the monolith row plus the historical `SkiaViewer → FS.Skia.UI` leak note from `docs/reports/dependencies.md` while affirming the preferred-package list (FR-004)
- [X] T010 [US1] [T2] [skillist: []] Verify (verify-only grep, no edit) that no `Directory.Packages.props` pin (root or `template/base`) and no template package pin names the `FS.Skia.UI` monolith — record the empty result alongside the no-consumer proof (FR-005)
- [X] T011 [US1] [T1] [skillist: []] Run `./fake.sh build -t Dev` — the full suite restores/builds/tests green with **zero** `Lib` references, the rewritten `Package.Tests` is green against the split packages (turning T005 green), the aggregate `PackageSurfaceCheck` baseline is current (no monolith assembly named, no drift after the T008 baseline removal), and nothing pulls the monolith (FR-003, FR-006, SC-002, SC-003)
- [X] T012 [US1] [T2] [skillist: []] Capture the no-consumer grep in `readiness/no-consumer-grep.md` — `grep -rn -E 'Lib\.fsproj|src/Lib|"FS\.Skia\.UI"'` over `src samples tests template build *.sln Directory.Packages.props` returns **zero** hits (programme history under `docs/`/`specs/` excluded); record the command and its empty output as the SC-001 proof (FR-006)

**Checkpoint**: User Story 1 complete — the last consumer is decoupled, `src/Lib` deleted and out of the solution, the monolith unpublished, suite green, grep clean.

---

## Phase 4: User Story 2 (US2) — the per-package surface baselines are an enforced merge gate (P2)

**Goal**: a public-`.fsi` change Route-selects `PerPackageSurfaceDiff`, the rule
is rendered into `validation.contract.yml`'s `required_gates`, the gate is on the
`knownGates` allowlist, and an unrecorded per-package `.fsi` edit fails the gate
(FR-007/013, SC-004).

### Tests First (Principle I, Principle VI)

- [X] T013 [P] [US2] [T1] [skillist: []] Add the **failing-first** Expecto assertion to `tests/Governance.Tests/RoutingTests.fs` that a diff touching `src/<InScopePackage>/**/*.fsi` returns a selection whose `Gates` contains `Targets.PerPackageSurfaceDiff` (alongside `PackageSurfaceCheck`/`FsiTranscripts`) at `Tier = FocusedAuthority`, using a live package path input (`src/Scene/Foo.fsi`); it is red until the rule is extended in T014 (C1)

### Implementation

- [X] T014 [US2] [T1] [skillist: fsharp-build-orchestration] Add `Targets.PerPackageSurfaceDiff` to the existing `package-surface` rule's `RequiredGates` in `build/Governance/Routing.fs:201`, add `"PerPackageSurfaceDiff"` to the `knownGates` allowlist in `build/Governance/AgentValidation.fs`, correct the stale `knownGates` comment at `Routing.fs:214` (FR-013) and the stale monolith-exclusion comment at `PerPackageSurface.fs:29`, and regenerate `validation.contract.yml` from `Routing.fs` so the rule + its rendering + the allowlist entry land together (FR-007)
- [X] T015 [US2] [T1] [skillist: []] Run `./fake.sh build -t Dev` then `./fake.sh build -t TargetMetadataDrift` — the RoutingTests assertion is green (turning T013 green), `validation.contract.yml`'s `package-surface` rule lists `PerPackageSurfaceDiff` in `required_gates`, and the contract is current vs `Routing.fs` (zero drift) (FR-007)
- [X] T016 [US2] [T1] [skillist: fsharp-build-orchestration] Capture the enforcement proof in `readiness/per-package-surface-enforcement.md` — `./fake.sh build -t PerPackageSurfaceDiff` is green at zero drift; a real, reverted one-line edit to one package's public `.fsi` (e.g. `src/Scene/<a public .fsi>`) without a baseline update **fails** the gate naming the drifted package; regenerating that package's `readiness/per-package-surface/<PackageId>.fsi.txt` baseline makes it pass; both the edit and the baseline are reverted (real evidence, SC-004)

**Checkpoint**: User Story 2 complete — `PerPackageSurfaceDiff` is Route-selected on a public-`.fsi` change, rendered into the contract, allowlisted, and proven to bite.

---

## Phase 5: User Story 3 (US3) — a generated app is clean (P2)

**Goal**: a cleanliness gate asserts a generated default `app` (and `governed`
profile) references packages rather than copying framework internals, and rejects
a planted `samples/`/docs/`specs/`/README copy (FR-008, SC-005/006).

- [X] T017 [P] [US3] [T1] [skillist: fsharp-io-globbing] Extend `GeneratedProductCheck` in `build/Governance/GeneratedProduct.fs` with cleanliness assertions — a generated default `app` (and the `governed` profile) contains **no** `samples/`, **no** framework documentation set (`docs/`), **no** historical `specs/`, **no** framework `README` copy (root `README.md`), and **references** the split packages rather than copying framework projects; pin the exact forbidden top-level globs (`samples/`, `docs/`, `specs/`, root `README.md`) so failure naming is deterministic, and the gate **fails naming the offending artifact** when any of those are planted (FR-008, C3)
- [X] T018 [US3] [T1] [skillist: []] Run `./fake.sh build -t TemplateCheck` then `./fake.sh build -t GeneratedProductCheck` — the cleanliness assertions are green on a freshly generated default `app` referencing split packages only, and red on a planted `samples/`/docs/`specs/`/README copy; record both outcomes in `readiness/cleanliness-gate.md` (FR-008/014, SC-005/006)

**Checkpoint**: User Story 3 complete — the cleanliness gate is present, green on a clean generated `app`, and proven to reject planted framework artifacts.

---

## Phase 6: User Story 4 (US4) — a V2 consumer can migrate + closeout measurement (P3)

**Goal**: the V2→V3 migration guide, ADR 0012, the ParityGallery/oracle policy,
and the after-measurement report are published (FR-009/010/011/012, SC-007).

- [X] T019 [P] [US4] [T2] [skillist: []] Author the V2→V3 migration guide `docs/migration/v2-to-v3.md` — a table mapping the old `FS.Skia.UI` surface to the split packages (`.Scene`/`.SkiaViewer`/`.Elmish`/`.KeyboardInput`/`.Input`/`.Layout`/`.Controls`), how to move an app's package references, the removed-`SceneConversion` note, and the rich keyboard-input → `FS.Skia.UI.Input` mapping (note that `.Controls.Elmish` and `.Testing` have no monolith public-surface predecessor and are intentionally absent from the surface map) (FR-009)
- [X] T020 [P] [T2] [skillist: []] Author `docs/adr/0012-monolith-retirement-closeout.md` — status Accepted; records the completed retirement (`src/Lib` deleted, `FS.Skia.UI` unpublished, per-package gate enforced, cleanliness gate added) and links the programme ADRs 0007–0011 (FR-011)
- [X] T021 [P] [T2] [skillist: []] Settle the `ParityGallery` / Scene-only scene-output oracle residue per ADR 0010 in `readiness/paritygallery-policy.md` — record that the oracle is **preserved** in the split-package suites and the keep-vs-retire decision for `samples/ParityGallery`, and clean governance scanning lists that still name `tests/Parity.Tests` where they assume the retired bridge (FR-012)
- [X] T022 [T1] [skillist: []] Author the after-measurement report `docs/reports/_baselines/2026-06-02-v3-after.md` mirroring the Stage-0 before-baseline — pin SHA; `src/Lib` LOC → 0; monolith transitive-pull → none; duplicate-type count → 0; package count (nine split + build engine); per-package baselines present (9); generated-`app` cleanliness asserted — **each metric with its reproduction command** (FR-010, SC-007); link the migration doc + ADR 0012 from the implementation plan

**Checkpoint**: User Story 4 complete — the migration guide, ADR, ParityGallery policy, and after-measurement report are published with reproduction commands.

---

## Phase 7: Integration & closeout (escalated serialized gates)

- [X] T023 [T2] [skillist: []] First confirm `./fake.sh build -t Route --enforce` reports the escalated tier with every required evidence artifact present, then run the escalated serialized FAKE gate set sequentially — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `DependencyReport` (post-deletion: the package graph is acyclic and `FS.Skia.UI.Scene` is FSharp.Core-only, verified from project references — FR-015, SC-008) → the final graph and audit gates (T024/T025) — never concurrently; confirm the default `app` restores/builds/runs referencing split packages only and pulls no monolith transitively (FR-014, SC-006); record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T024 [T2] [skillist: speckit-evidence-graph] Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`) — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the structured task metadata and visible mirrors are valid (`verdict=ok`)
- [X] T025 [T2] [skillist: speckit-evidence-audit] Run the merge-gate audit (`./fake.sh build -t EvidenceAudit`) — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept (SC-009)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; the rewritten packaging-contract suite, the real `git rm` + repo-wide grep, the real reverted `PerPackageSurfaceDiff` enforcement edit, the cleanliness gate on a real generated `app`, and the escalated FAKE gate logs are all real. Reference-screenshot re-capture is a disclosed Principle V infeasibility, corroboration-only — the deterministic scene-output oracle is authoritative — not synthetic evidence.)_ | | | | | | | | |
