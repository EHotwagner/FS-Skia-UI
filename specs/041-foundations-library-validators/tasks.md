# Tasks: Foundations Governance Library — First Real Validators

**Feature branch**: `041-foundations-library-validators`
**Spec**: `specs/041-foundations-library-validators/spec.md`
**Plan**: `specs/041-foundations-library-validators/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real — the
captured Stage-0 golden fixtures, byte-identical rendered reports, typed-finding
unit tests over real crafted typed values, the recorded `build.fsx` line-count
delta, and the serialized FAKE gate logs (plan: Evidence obligations — real
evidence only; Principle V — no `[S]`/`[SEH]` anticipated). The scratch-branch
compile-error demonstration in T012 is a structural proof (SC-003), not a
shipped synthetic fixture.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- **[T2]** — Tier 2 (internal refactor/extraction); this whole feature is Tier 2
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read before implementation)

Unlike feature 040 (which *authored* the `fsharp-*` cookbooks and therefore
could not self-reference them), this feature *consumes* them as genuine
implementation aids, so they appear in `skillist` where they materially help:

- **`fsharp-parsing`** — the capability-catalog typed model reads
  `template/capabilities.yml` through `YamlDotNet` behind the model; the
  two-shape YAML caution applies directly (T014). High confidence.
- **`fsharp-code-generation`** — the structured-finding render helpers and the
  `TargetMetadata` JSON/Markdown renderers emit report text via
  StringBuilder / `Utf8JsonWriter` (T006, T011). Medium-high confidence.
- **`fsharp-build-orchestration`** — golden-fixture byte-parity (DiffPlex),
  Expecto typed-finding tests, and FAKE in-process wiring (T001, T009, T012,
  T013, T015, T016, T017, T018). Medium confidence; it is the orchestration /
  parity skill for these tasks.

`fsharp-graph-algorithms`, `fsharp-io-globbing`, and `fsharp-shell-process` are
**not** assigned: the typed `Target` DU is plain DU + records + a total `spec`
projection (no cycle-detection/topo-sort), no new globbing is introduced, and no
new shelling is added. The only governance-workflow skills are
`speckit-evidence-graph` (T022) and `speckit-evidence-audit` (T023). No
`fs-skia-*` skill applies — no runtime, rendering, viewer, layout, widgets, or
visual output is touched (FR-011, SC-007).

## Pitfall guidance (read before `EvidenceGraph`)

- `tasks.deps.yml` uses **one object-shaped key per task id** with indented
  `deps` and `skillist` fields — never inline maps like
  `T001: { deps: [], skillist: [] }`.
- Every `Tnnn` in `tasks.md` appears exactly once as a key in `tasks.deps.yml`;
  every dependency uses an exact `Tnnn` id; every `[skillist: ...]` mirror
  matches the structured list exactly and in order.
- Implementation task titles deliberately avoid the Spec Kit title-trigger
  phrases (`task graph`, `evidence graph`, `synthetic propagation`,
  `diff-scan`, `readiness validation`, `constitution`, etc.) so the
  parity/wiring work is not misread as a graph/audit/task-generation workflow.
  The parity tasks say "golden-diff" / "byte-equality", never "diff-scan".
  T022/T023 use the graph/audit trigger phrases intentionally and carry the
  matching evidence skills.

## Canonical Verification Targets (serialized — FAKE shares `.fake` state)

Run FAKE-backed targets **sequentially**, never concurrently (`CLAUDE.md` /
`AGENTS.md`). This feature adds **no new FAKE target** (parity is a
`Governance.Tests` assertion, FR-008a). Serialized order:

1. `./fake.sh build -t Dev` (runs Governance.Tests incl. parity + typed-finding tests)
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Governance risk level for this feature is **small→medium** (build-tooling only;
internal refactor; no new FAKE target; reuses the already-present `YamlDotNet`;
new `build/Governance` `.fsi`). Focused validation for the selected level = the
`Dev` gate (parity + typed-finding tests) plus the serialized order above;
broad validation (full `Verify`) is required only if a gate failure looks
race-like or the concurrent FAKE context is unknown. Aggregate FAKE results are
recorded as **non-authoritative**; the focused per-gate rerun is authoritative.

---

## Phase 1: Setup

- [X] T001 [T2] [skillist: fsharp-build-orchestration] Capture the parity oracle FIRST (R1): on the pinned pre-extraction baseline run `CapabilityCheck`, `TargetMetadata`, and `TargetMetadataDrift`, then commit their outputs as `tests/Governance.Tests/fixtures/reports-golden/capability-catalog.md`, `target-metadata.json`, and `target-metadata-drift.md`; note the captured `generated_at_utc` value for the R2 normalization
- [X] T002 [P] [T2] [skillist: []] Record the 041 pre-extraction `build.fsx` baseline line count (`wc -l build.fsx`, expected 4,839 at post-040 HEAD) into `readiness/build-fsx-line-delta.md` as the SC-001 before-count (the 039 Stage-0 count was 4,688; 040 grew the file)
- [X] T003 [T2] [skillist: []] Record feature Tier 2, affected layer (`build/Governance` + `build.fsx` build-tooling only), public-API impact (no product `.fsi`; new build-tooling `.fsi` required by Principle II), Elmish/MVU applicability (plugs into the existing `build.fsx` `update`/effect boundary — no new `Model`/`Msg`/`Effect`), and the real-evidence obligations (golden-diff = 0, ≥6 typed findings, ≥800-line shrink, `src/**` untouched)
- [X] T004 [skillist: []] Complete readiness notes for the feature's required readiness placeholder files under `specs/041-foundations-library-validators/readiness/` (governance-risk-levels, aggregate-hang-diagnostics, runtime-limitations, generated-validation-authority, evidence-graph, evidence-audit), each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T005 [T2] [skillist: []] Place the four curated `.fsi` contracts from `contracts/` (`Findings.fsi`, `Targets.fsi`, `TargetMetadata.fsi`, `Capabilities.fsi`) into `build/Governance/` and add the `.fsi`/`.fs` pairs to `FS.Skia.UI.Build.fsproj` `<Compile>` in order (after Spike/SkillSync/SkillExamples) — Principle I/II, no access modifiers in `.fs` (FR-007)
- [X] T006 [T2] [skillist: fsharp-code-generation] Implement `build/Governance/Findings.fs` against its `.fsi`: the uniform `ValidationFinding` record (moved verbatim), the `finding` constructor, and the detail-line render helper reproducing `writeFindingsOrPass`'s `` - `{Path}` [{Rule}]: {Message} `` format byte-for-byte (FR-004) — `Findings` is a pure data/render type with no failing-first test of its own; it is covered transitively by the validator suites (T009/T013) and the parity assertion (T015), which fail if its record shape or render format regress (Principle VI: transitive coverage)
- [X] T007 [P] [T2] [skillist: []] Exercise the draft `.fsi` from FSI (representative finding-render, `spec`-projection, and catalog-read calls), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T008 [P] [T2] [skillist: []] Record surface-area baselines for the new `build/Governance` modules and the unsupported-scope handling: a missing library DLL reference at extraction time is surfaced explicitly as the Stage-5 trigger (edge case 3), never a silent inline fallback

**Checkpoint**: Foundation ready — the two validator stories may proceed.

---

## Phase 3: User Story 1 (US1) — target metadata can no longer drift, because it is derived

*Independent test*: introduce a target reference that previously produced a
`TargetMetadataDrift` diagnostic (a runnable target with no metadata row); with
the typed model the inconsistency fails to compile or is caught by a library
unit test, without running the full build.

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fsharp-build-orchestration] Add failing `tests/Governance.Tests/TargetMetadataTests.fs` asserting ≥3 `TargetMetadataDrift` typed cases (e.g. `MissingMetadata`, `MissingExpectedOutput`/`MissingFailureOwner`/`DependencyDivergence`, `MissingRunnableTarget`) over crafted typed inputs — not string matching (fails before `TargetMetadata.fs` exists; SC-004)

### Implementation

- [X] T010 [US1] [skillist: []] Implement `build/Governance/Targets.fs` against its `.fsi`: the typed `Target` discriminated union (one case per runnable target, preserving order), `allTargets`, the **total** `spec : Target -> TargetSpec`, and the derived `requiredTargetNames` / `targetDependencyRows` views computed from `spec` (FR-001) — plain DU + records, no cleverness (Principle III)
- [X] T011 [US1] [skillist: fsharp-code-generation] Implement `build/Governance/TargetMetadata.fs` against its `.fsi`: `TargetMetadata` computed from `TargetSpec`, the `TargetMetadataDrift` DU, the pure `validateMetadataDrift` / `validateAgainstRepo` (preserving contract-drift and docs-drift diagnostics), `driftDiagnostic`, `metadataJson` with `generatedAtUtc` as an explicit parameter (R2), and `driftMarkdown` — every existing diagnostic category and message string preserved (FR-002)
- [X] T012 [US1] [skillist: fsharp-build-orchestration] Convert **all** `StartTarget "..."` dispatch arms in `build.fsx` to dispatch on `Targets.Target`, and derive `requiredTargets` / `targetDependencyRows` / metadata from `Targets.spec` rather than maintaining them alongside it; demonstrate on a scratch branch that a renamed/mistyped target now fails to compile (FR-001, SC-003) — the persistent SC-003 evidence is the committed T009 typed-finding test; the scratch-branch compile error is the transient structural half. The MVU engine and build front-end form remain Stage 5 (FR-001a)

**Checkpoint**: Target identity/deps/metadata derive from one typed source; drift is structurally unrepresentable.

---

## Phase 4: User Story 4 (US4) — the bespoke capability YAML parser is gone

*Independent test*: the capability catalog is read through the library's typed
model; a catalog input the old parser tolerated through an indentation quirk
produces the same parsed model and report; `CapabilityCheck` still produces the
golden-identical report.

### Tests First (Principle I, Principle VI)

- [X] T013 [P] [US4] [skillist: fsharp-build-orchestration] Add failing `tests/Governance.Tests/CapabilityCatalogTests.fs` asserting ≥3 catalog error-class `ValidationFinding` rule ids (e.g. `displayName`, `dependency`, default-app-set / missing-surface-baseline) over crafted typed rows — not string matching (fails before `Capabilities.fs` exists; SC-004)

### Implementation

- [X] T014 [US4] [skillist: fsharp-parsing] Implement `build/Governance/Capabilities.fs` against its `.fsi`: the `CapabilityRow` model (15 fields), `readCatalog` reading `template/capabilities.yml` via `YamlDotNet` behind the typed model (no new dependency, YAML file retained — FR-003/FR-012), `validateRows` as a pure function with the `File.Exists` surface-baseline check **injected** (testable without disk), and `renderReport` reproducing the `# Capability Catalog` PASS table; preserve every existing typed rule id and message

**Checkpoint**: Catalog is a typed model; validation is pure over typed values (parser retired in T016).

---

## Phase 5: User Story 2 (US2) — the build computes its validators in-process, with the same output

*Independent test*: run the three targets on the pinned baseline; diff each
produced report against its Stage-0 golden fixture. The diff is empty.

### Tests First (Principle I, Principle VI)

- [X] T015 [P] [US2] [skillist: fsharp-build-orchestration] Add failing `tests/Governance.Tests/ReportParityTests.fs` asserting byte-equality of the three rendered reports vs `fixtures/reports-golden/` — `capability-catalog.md` and `target-metadata-drift.md` fully, `target-metadata.json` for every line except the `generated_at_utc` value (asserted present + well-formed, R2) — under the existing `Dev`/test gate, no new FAKE target (FR-008a)

### Implementation

- [X] T016 [US2] [skillist: fsharp-build-orchestration] `#load` the four new modules into `build.fsx` and rewrite the `CapabilityCheck` / `TargetMetadata` / `TargetMetadataDrift` interpret cases to call `FS.Skia.UI.Build.*` in-process, passing edge-read inputs (contract/docs references, surface-baseline existence, `DateTimeOffset.UtcNow`); delete the bespoke `readCapabilityCatalog` line-by-line parser and the moved inline validators (FR-005, SC-005) — targets keep their names, deps, outputs, and graph positions (FR-013)
- [X] T017 [US2] [skillist: fsharp-build-orchestration] Run `CapabilityCheck` / `TargetMetadata` / `TargetMetadataDrift` on the pinned baseline and confirm golden-diff parity = 0 bytes for all three; grep confirms the bespoke parser no longer exists in `build.fsx`; record the empty-diff parity reports under `readiness/` (SC-002, SC-005, FR-006)

**Checkpoint**: The three reports are byte-identical to the Stage-0 fixtures; logic moved with no observable change.

---

## Phase 6: User Story 3 (US3) — the moved rules are unit-tested against typed errors

*Independent test*: the new suites call the extracted validators with crafted
inputs and assert the exact typed finding per violation class; they fail if a
validator stops emitting that finding.

- [X] T018 [US3] [skillist: fsharp-build-orchestration] Re-point any existing `tests/Governance.Tests` cases that previously asserted strings/behaviours of the moved logic at the real library functions, and confirm ≥6 typed-finding cases pass in total (≥3 catalog error classes + ≥3 target-metadata drift classes), all green (FR-008, SC-004)

**Checkpoint**: The moved rules have fast, precise typed-finding coverage that the script-trapped logic could not have.

---

## Phase 7: Integration & Polish

- [F] T019 [T2] [skillist: []] Record the post-extraction `build.fsx` line count into `readiness/build-fsx-line-delta.md`; confirm the shrink is ≥800 lines vs the 041 pre-extraction baseline (4,839, SC-001), `git diff` over `src/**` is empty (runtime untouched, SC-007), and no new `PackageVersion` exists outside `Directory.Packages.props` (FR-010/FR-012) — **PARTIAL/FAIL on SC-001 only**: line count recorded (4839 → 4454 = **385-line shrink**), `git diff src/**` empty (SC-007 ✔), no new `PackageVersion` (FR-010/FR-012 ✔), but the **≥800-line target is not met**. Per research R3 the `focusedGateContract`/`BuildModel` path machinery (the bulk of the target-metadata code) deliberately stays at the build.fsx interpreter edge (Principle IV); moving it is the out-of-scope Stage-5 MEL relocation (FR-001a). The ≥800 figure over-counted Stage-3's extractable surface. Diagnostics left in place: `readiness/build-fsx-line-delta.md` (SC-001 variance section). Not retried/padded.
- [X] T020 [P] [T2] [skillist: []] Confirm `PackageSurfaceCheck` and `FsiTranscripts` show no baseline diff — no product public surface changes (FR-011, SC-006)
- [X] T021 [skillist: []] Run the serialized FAKE gate order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`) for the small→medium governance risk level, recording aggregate FAKE results as non-authoritative and rerunning any race-like or environment-flaky gate failure (documented 039 `FsiTranscripts`/`SkiaViewer.Tests` flakes) in focused isolation as the authoritative result (SC-006)
- [X] T022 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm the task graph is acyclic, no dangling refs, no `[S*]` surprises, and that the `skillist` metadata and visible mirrors are valid
- [X] T023 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS with no synthetic evidence to accept (this feature ships none)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This feature ships
**none** (plan: Synthetic evidence — None). The scratch-branch compile-error
demonstration in T012 is a structural proof (SC-003), not a shipped synthetic
fixture.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
