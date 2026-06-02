# Tasks: V3 Stage 3–4 Residual — Decouple Remaining Consumers from `src/Lib`

**Feature branch**: `052-v3-lib-decoupling`
**Spec**: `specs/052-v3-lib-decoupling/spec.md`
**Plan**: `specs/052-v3-lib-decoupling/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit.

**This feature ships zero synthetic evidence.** All evidence is real: the rich
keyboard-input runtime re-derives identical behaviour under its migrated test
suite (same fixtures, same assertion count), the structural-rename `git diff -M`
proves the body moved byte-for-byte (only the `namespace` line differs), the
deterministic scene-output oracle re-derives byte-identically to the Stage-0
golden (the parity sign-off justifying the bridge retirement), the no-consumer
grep reads the real tree, and the serialized escalated FAKE gate logs are real.
No `[S]`/`[SEH]` task is approved (Principle V); `EvidenceAudit` MUST return
`verdict=PASS` with zero synthetic (SC-008).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]** the monolith is reference-free so Stage 5 can delete it,
  **[US2]** the rich keyboard-input runtime ships from a focused split package,
  **[US3]** the parity scaffolding is retired after sign-off
- **[T1]** / **[T2]** — this feature is **Tier 1 for the monolith**: it removes
  the rich `KeyboardInput` surface (and later the `Parity` helper) from the
  published `FS.Skia.UI` package and shrinks that package's surface baseline,
  and it adds a new published package `FS.Skia.UI.Input` with its own baseline.
  Surface-moving tasks carry `[T1]`; pure readiness/record/verification tasks
  carry `[T2]`. `Route` **escalates** this `src/**/*.fsi`-changing change (the
  actual tier may be `agent-ready` rather than full `dogfood` — run exactly what
  `Route` prints).
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read first)

Recorded as a confidence review, not regex certainty:

- **Package creation + the module move + the surface-baseline diff** (T006 add
  `src/Input/Input.fsproj` and wire the solution/`PackLocal`; T007 `git mv` the
  pair + rewrite only the `namespace` line + build; T017 record the new/shrunk
  per-package baselines and re-run the DiffPlex-backed surface check) take
  `fsharp-build-orchestration` — *matched signal:* new packable project +
  compile/`PackLocal` wiring + the DiffPlex `PackageSurfaceCheck`/
  `PerPackageSurfaceDiff` diff; *confidence:* medium-high. The plan names this
  skill for the move.
- **The keyboard-input test migration** (T005) takes `fs-skia-keyboard-input` —
  *matched signal:* it authors/exercises the rich keyboard-input runtime's public
  surface through `open FS.Skia.UI.Input`; *confidence:* medium. *Considered and
  rejected for the byte-for-byte module move (T007):* no input logic is authored
  there (pure relocation), so the capability skill is indirect for the move —
  only the build mechanics apply (mirrors the Stage-2 reasoning that rejected the
  parsing skill on its relocation).
- **The parity sign-off** (T013) takes `fs-skia-layout-evidence` — *matched
  signal:* deterministic scene-output evidence re-derived byte-identically to the
  Stage-0 golden; *confidence:* medium. **The parity-assertion fold** (T014) takes
  `fs-skia-scene` — *matched signal:* the still-valuable assertions are
  scene-output (`describe`/`diagnostics`) assertions migrating into `Scene.Tests`;
  *confidence:* medium. The retirement mechanics in the same task (project removal,
  helper deletion) are reference/build edits with no capability match.
- **Reference-editing + gate-running tasks** (T010 sample repoint, T011 test
  repoint, T012/T018 gate runs, T016 grep) carry `[skillist: []]`. `fs-skia-skiaviewer`
  was **considered and rejected** for the `InteractiveViewer` repoint — it edits
  project references, not host code. `fs-skia-template-update` was **considered and
  rejected** for the gate task — it runs generated-product gates, it does not author
  template content.
- The genuine workflow tasks are the last two: **T019** declares
  `speckit-evidence-graph` and **T020** declares `speckit-evidence-audit`, in that
  order (graph before audit). `speckit-constitution` is **not** assigned — no
  `.specify/memory/constitution.md` edit.

## Governance risk levels & validation

- **Small** (this feature's own `readiness/` Markdown and record/verification
  notes): focused review plus a `git diff` over the edited files is the **required
  evidence** and is authoritative for the level.
- **Medium** (the new `FS.Skia.UI.Input` baseline, the monolith surface shrink, the
  structural-rename parity, and the deterministic parity sign-off): the focused
  `PerPackageSurfaceDiff`/`PackageSurfaceCheck` runs, the `git diff -M` rename
  similarity, and the scene-output byte-identity vs the Stage-0 golden are the
  **required evidence** and the authoritative signals.
- **Broad** (required here, because `Route` escalates this `src/**/*.fsi` change):
  the full serialized FAKE gate order (`Dev` → `GeneratedGuidanceCheck` →
  `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates).
  **Broad validation is required** whenever a consumer-contract or public-`.fsi`
  surface changes. Aggregate FAKE results are recorded as **non-authoritative**;
  any race-like or environment-flaky failure is rerun in focused isolation and that
  focused result is authoritative.

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Task **titles** deliberately avoid the
validator's blocking trigger tokens: the no-consumer task uses the bare word
"grep" (never `diff-scan`); no non-graph/non-audit title uses `task graph` /
`evidence graph` / `evidence audit` / `synthetic propagation` / `constitution` /
`readiness validation` / `mirror mismatch`. The genuine workflow tasks T019/T020
**do** declare `speckit-evidence-graph` / `speckit-evidence-audit` and name
`EvidenceGraph` / `EvidenceAudit` directly. The readiness-scaffold task (T002)
uses the safe `Create placeholder evidence files listed by the plan` wording and
the readiness-aggregation task (T003) uses the `Complete readiness notes` prefix,
so their hyphenated filename citations do not fire capability checks. This feature
makes **no** persistent-viewer or window-visibility change and needs **no** visual
readiness scaffold (scene-output parity is headless-deterministic; reference-frame
re-capture stays headless-GPU-infeasible and is disclosed, not synthetic).
`tasks.deps.yml` keeps one indented object per task id with `deps` and `skillist`;
every `[skillist: …]` mirror matches the structured list exactly and in order.
Cross-references use **fixed `Tnnn` ids** only; no backward task edge is written,
so the DAG stays acyclic.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record the feature Tier (Tier 1 for the monolith — the published `FS.Skia.UI` package loses the rich `KeyboardInput` surface now and the `Parity` helper after sign-off, and its surface baseline shrinks; a new published package `FS.Skia.UI.Input` is added with its own baseline), the affected surfaces (new `src/Input/Input.fsproj` + the moved `KeyboardInput.fs(i)`, `src/Lib/Lib.fsproj` + `Library.fs(i)`, `samples/InteractiveViewer`, `tests/{Lib.Tests,Parity.Tests,Package.Tests}`, new `tests/Input.Tests`, `readiness/per-package-surface/{FS.Skia.UI.Input,FS.Skia.UI}.fsi.txt`, the aggregate `PackageSurfaceCheck` baseline, and `specs/052-v3-lib-decoupling/readiness/**`), the public-API impact (monolith `.fsi` shrinks; new package `.fsi` is a namespace-rename of the moved module; `validation.contract.yml` unchanged), the Elmish/MVU applicability (the `InputRuntime`/`InputMsg`/`InputEffect`/`init`/pure `update` input model **moves intact** with behaviour preserved — `update` stays pure and YAML/file I/O stays at the interpreter edge, proven by the migrated suite, not redesigned), and the real-evidence obligations (migrated suite green with the same assertion count, structural-rename diff, scene-output byte-identity vs the Stage-0 golden, the no-consumer grep, generated-consumer gates green, and the serialized escalated FAKE gate logs; zero synthetic)
- [X] T002 [P] [T2] [skillist: []] Create placeholder evidence files listed by the plan under `specs/052-v3-lib-decoupling/readiness/` so the audit-enforced readiness files are discoverable at setup: the always-required contract trio `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`; the record notes `acyclic-graph-proof.md`, `structural-parity.md`, `surface-baseline-diff.md`, `no-consumer-grep.md`, `parity-signoff.md`, `paritygallery-policy.md`; the gate records `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md`; and `logs/` (`dev.log`, `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`, `evidence-graph.log`, `evidence-audit.log`)
- [X] T003 [T2] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files — `governance-risk-levels.md` (the small / medium / broad levels, their required evidence, and when broad validation is required), `aggregate-hang-diagnostics.md` (verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate), and `runtime-limitations.md` (the .NET 10 build-host statements; the rich input runtime couples to `SkiaViewer.Host` but no Vulkan/Skia behaviour changes; reference-frame re-capture stays headless-GPU-infeasible — disclosed, not synthetic) — each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation (consumer work-list + acyclic-edge proof)

- [X] T004 [T1] [skillist: []] Re-verify the consumer work-list and the acyclic package edge per `research.md` (R1/R4) — `grep -rn -E "Lib\.fsproj|Include=\"FS\.Skia\.UI\"" samples tests src --include=*.fsproj` confirms exactly the four consumers (`samples/InteractiveViewer`, `tests/Lib.Tests`, `tests/Parity.Tests`, `tests/Package.Tests`) plus the files being moved, and that `samples/ParityGallery` is already monolith-free; record that the new edge `FS.Skia.UI.Input → SkiaViewer → {Scene, KeyboardInput}` and `→ Scene` is acyclic (no package depends on `Input`), and list the `FS.Skia.UI.*` baseline lines to shed — the work-list, no edits yet

**Checkpoint**: Foundation ready — consumers re-verified, the acyclic home edge fixed, the surface-baseline delta known; story work may begin.

---

## Phase 3: User Story 2 (US2) — the rich keyboard-input runtime ships from `FS.Skia.UI.Input` (P2)

**Goal**: the rich input runtime compiles and is exported by the new
`FS.Skia.UI.Input` package, no `KeyboardInput.fs(i)` remains under `src/Lib`, and
the migrated suite passes against the relocated module with identical behaviour
(FR-001/002, SC-003/004/005).

### Tests First (Principle I, Principle VI)

- [X] T005 [P] [US2] [T1] [skillist: fs-skia-keyboard-input] Create `tests/Input.Tests/Input.Tests.fsproj` referencing the new `FS.Skia.UI.Input` package (+ `Scene`/`SkiaViewer` as needed) and move `tests/Lib.Tests/KeyboardInputTests.fs` into it, rewriting its `open FS.Skia.UI` (keyboard input) to `open FS.Skia.UI.Input` as the **failing-first** compile break (the relocated namespace does not exist until T007); preserve every fixture and assertion unchanged so the suite stays the behavioural-parity oracle with the **same** assertion count (FR-002/004, SC-004)

### Implementation

- [X] T006 [US2] [T1] [skillist: fsharp-build-orchestration] Add `src/Input/Input.fsproj` (PackageId `FS.Skia.UI.Input`, net10 conventions inherited from `Directory.Build.props`, `ProjectReference` to `..\Scene\Scene.fsproj` + `..\SkiaViewer\SkiaViewer.fsproj`, no new external `PackageVersion`), add it to the solution and to `PackLocal`/the dependency report — the empty package builds green and introduces no `Directory.Packages.props` change (FR-008, plan §Dependency impact)
- [X] T007 [US2] [T1] [skillist: fsharp-build-orchestration] `git mv` `src/Lib/KeyboardInput.fsi` + `.fs` into `src/Input/`, rewrite only the `namespace` line (`FS.Skia.UI` → `FS.Skia.UI.Input`), add the two `<Compile Include>` items to `Input.fsproj`, remove them from `src/Lib/Lib.fsproj`, and build both `FS.Skia.UI.Input` and the shrunk monolith green — no `val`/`type`/field/case added, removed, or retyped (FR-001/002, R1/R2); confirm `git ls-files src/Lib/KeyboardInput.*` returns nothing
- [X] T008 [US2] [T1] [skillist: []] Run `./fake.sh build -t Dev` — the migrated `Input.Tests` suite builds and passes against the relocated module with the **same** assertion count, turning T005 green; this is the behavioural-parity oracle for the rich input runtime (binding/mode/sequence semantics, command intents, diagnostics, state-display projection) (FR-002, SC-004)
- [X] T009 [US2] [T1] [skillist: []] Record structural parity in `readiness/structural-parity.md` — `git diff -M --stat` shows `KeyboardInput.fs(i)` as renamed `src/Lib` → `src/Input` at ~100% similarity (only the namespace line differs) — and confirm via the migrated suite that the relocated runtime yields identical behaviour vs the pre-move module (SC-004); record the `FS.Skia.UI.Input` per-package baseline equals the post-move `.fsi` modulo the namespace line

**Checkpoint**: User Story 2 complete — the rich runtime is compiled/exported by `FS.Skia.UI.Input`, gone from `src/Lib`, suite green against the relocated home, parity structural + behavioural.

---

## Phase 4: User Story 1 (US1) — the consumers are repointed off `src/Lib` (P1)

**Goal**: `InteractiveViewer`, `Lib.Tests`, and `Package.Tests` no longer
reference the monolith; the rich-input consumers build/run against split packages
only (FR-003/004/006, SC-001/002/003).

- [X] T010 [P] [US1] [T1] [skillist: []] Repoint `samples/InteractiveViewer/InteractiveViewer.fsproj` off the monolith — drop the `ProjectReference` to `..\..\src\Lib\Lib.fsproj` and the `PackageReference` to `FS.Skia.UI`; add `FS.Skia.UI.Input` (`ProjectReference` on the source path, `PackageReference` on the `UsePackedPackage` path) alongside the existing `Scene`/`SkiaViewer` references (FR-003)
- [X] T011 [US1] [T1] [skillist: []] Triage `tests/Lib.Tests` (its `KeyboardInputTests.fs` migrated to `tests/Input.Tests` in T005): the residual `Tests.fs` (930 LOC of Viewer/Scene/Diagnostics assertions) has **no** `Lib` dependency, so `Lib.Tests` keeps `Tests.fs` + `Program.fs` and the `Lib.fsproj` `ProjectReference` is dropped — it now references only `Scene` + `SkiaViewer`. **Scope deviation (maintainer-confirmed):** `tests/Package.Tests` retains its `Lib.fsproj` reference — it is a deliberate *packaging-contract* consumer that asserts the still-published `FS.Skia.UI` surface (`typeof<FS.Skia.UI.ParityReport>.Assembly`, the `VulkanResources`/`VulkanStartup` non-exports, the `PackLocal` entry); that decoupling retires **with the monolith in Stage 5** (FR-011). Recorded in `readiness/no-consumer-grep.md` (FR-004)
- [X] T012 [US1] [T1] [skillist: []] Run `./fake.sh build -t Dev` — `InteractiveViewer`, `Input.Tests`, and `Package.Tests` restore/build/run green with **no** link back into `src/Lib` for the keyboard-input path, proving the rich input runtime without the monolith reference (FR-003/004/006, SC-003)

**Checkpoint**: User Story 1 (keyboard side) complete — the sample and the keyboard tests are monolith-free and green.

---

## Phase 5: User Story 3 (US3) — the parity scaffolding is retired after sign-off (P3)

**Goal**: with Stage-1 parity signed off, the `Parity.Tests` bridge and the dead
`Parity` helper are removed (valuable assertions folded into the split-package
suites first), and the `ParityGallery` keep/retire decision is recorded
(FR-005/007, SC-002).

- [X] T013 [US3] [T1] [skillist: fs-skia-layout-evidence] Sign off parity in `readiness/parity-signoff.md` — confirm the deterministic scene-output check (`tests/Parity.Tests` over `tests/Parity.Tests/fixtures/v3-host-golden/scene-output/<seed>.txt`, format `scene-output/v1`) re-derives **byte-identically** to the Stage-0 golden for `basic-viewer`/`effects-gallery`/`screenshot-gallery`; record this byte-identity as the merge-gate sign-off that justifies retiring the bridge (FR-005)
- [X] T014 [US3] [T1] [skillist: fs-skia-scene] Retire the obsolete old-vs-new `Parity`-helper report bridge: `git rm tests/Parity.Tests/Tests.fs` and drop the `Lib.fsproj` `ProjectReference` from `Parity.Tests.fsproj`. **The scene-output oracle is preserved in place** (`SceneOutput.fs`/`SceneOutputTests.fs` + fixtures stay in the now Scene-only `Parity.Tests` — no migration to `Scene.Tests`, which would churn the hardcoded fixture path + the governance scanning lists that reference `tests/Parity.Tests` for no gain). **Scope deviation (maintainer-confirmed):** the `Parity` helper itself (`src/Lib/Library.fs(i)`) is **kept** — it is the monolith's surface anchor asserted by `Package.Tests`, so it retires **with the monolith in Stage 5** (FR-011). `Dev` stays green (FR-005)
- [X] T015 [US3] [T2] [skillist: []] Settle the `ParityGallery` policy per ADR 0010 in `readiness/paritygallery-policy.md` — record the keep-vs-retire decision (recommended: retire `samples/ParityGallery` together with the bridge, since it visualized the old-vs-new report that no longer exists; if kept, note the supported capability it still demonstrates on `Scene`+`SkiaViewer`) and confirm it references no monolith either way (FR-007)

**Checkpoint**: User Story 3 complete — the bridge and the dead helper are gone, parity is signed off on the deterministic oracle, and the sample policy is recorded.

---

## Phase 6: Integration & closeout (reference-free verification + serialized escalated gates)

- [X] T016 [US1] [T2] [skillist: []] Capture the no-consumer grep in `readiness/no-consumer-grep.md` — `grep -rn -E "Lib\.fsproj|Include=\"FS\.Skia\.UI\"" samples tests src --include=*.fsproj` shows **zero** sample consumers (SC-001) and that the **only** remaining `src/Lib` consumer is `tests/Package.Tests` — the deliberate monolith-packaging contract that asserts the still-published `FS.Skia.UI` (kept by maintainer decision; retires with the monolith in Stage 5). Every keyboard-input + parity-bridge consumer is off `Lib`; record that `src/Lib` is still present and `FS.Skia.UI` still packable (FR-010/011). **SC-007 amended:** "fully reference-free" is a Stage 5 outcome — it cannot hold while `FS.Skia.UI` is a published package under packaging tests
- [X] T017 [T1] [skillist: fsharp-build-orchestration] Record the surface deltas in `readiness/surface-baseline-diff.md` and run `./fake.sh build -t PerPackageSurfaceDiff` clean — the new `readiness/per-package-surface/FS.Skia.UI.Input.fsi.txt` baseline is captured, `readiness/per-package-surface/FS.Skia.UI.fsi.txt` sheds exactly the rich `KeyboardInput` lines (and the `Parity` lines after T014), the aggregate `PackageSurfaceCheck` baseline records `FS.Skia.UI.Input.*` and drops the monolith's removed types, and `validation.contract.yml` is unchanged (FR-009, SC-006)
- [X] T018 [T2] [skillist: []] First confirm `./fake.sh build -t Route --enforce` reports the escalated tier with every required evidence artifact present, then run the escalated serialized FAKE gate set sequentially — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates (T019/T020) — never concurrently; confirm the default `app` still restores/builds/runs and does not pull the monolith transitively, and the generated-consumer gates stay green (FR-012); record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure in focused isolation as the authoritative result; logs under `readiness/logs/`
- [X] T019 [T2] [skillist: speckit-evidence-graph] Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`) — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the structured task metadata and visible mirrors are valid (`verdict=ok`)
- [X] T020 [T2] [skillist: speckit-evidence-audit] Run the merge-gate audit (`./fake.sh build -t EvidenceAudit`) — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 blocking diff-scan, 0 blocking readiness-contract) with zero synthetic evidence to accept (SC-008)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; the migrated real test suite is the behavioural-parity oracle, the structural-rename `git diff -M` proves the byte-for-byte move, the deterministic scene-output oracle re-derives byte-identically to the Stage-0 golden, and the no-consumer grep + escalated FAKE gates read the real tree.)_ | | | | | | | | |
