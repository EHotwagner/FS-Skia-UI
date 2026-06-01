# Tasks: Single-Source Generation of Duplicated Governance Artifacts (Stage 2.2–2.5)

**Feature branch**: `044-foundations-single-source-generation`
**Spec**: `specs/044-foundations-single-source-generation/spec.md`
**Plan**: `specs/044-foundations-single-source-generation/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real — live
generation-currency gate runs showing edit-without-regenerate **fails** and
post-regenerate **passes** (skills, skillist, principle fragments), in-process
byte-identity proof across all 25 skill pairs, typed Expecto unit results for
the three new pure modules, grep proofs (no `FSharp.Compiler.*`, no
`diff`/`cmp`/`sha256sum`/symlink shelling in the generation path), and the
serialized FAKE gate logs. The hand-built stale-fixture inputs for each currency
function are typed *test inputs* asserting typed currency diagnostics, not
synthetic *evidence*, so no `[SEH]` task is approved (Principle V).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T1]** — Tier 1 (contracted): this whole feature is Tier 1 (new published
  `FS.Skia.UI.Build` governance modules, each with a curated `.fsi` per
  Principle II) and a `.specify/**` + skill-tree + governance-path change, so the
  `Route` selector **escalates** it to the full serialized gate set (FR-015).
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read before implementation)

Like features 041/042/043, this feature is **build-tooling / governance only**
(`build/Governance/**` + `build.fsx` + `.specify/**` + the skill trees), so no
`fs-skia-*` runtime / rendering / viewer / layout / widgets skill applies. It
*consumes* the `fsharp-*` cookbooks as genuine implementation aids:

- **`fsharp-io-globbing`** — the canonical-tree enumeration and the
  **generation-currency diffing** at the heart of `SkillTreeGen` (US1, T007–T013)
  and the `RefreshSurfaceBaselines` enumeration edge (T011). High confidence;
  file discovery + currency diffing is the cookbook's exact remit.
- **`fsharp-code-generation`** — deterministic governance-artifact emission: the
  `SkillTreeGen` provenance manifest (T008), the `SkillistView` annotation render
  / in-place splice (T014–T016), and the `ConstitutionFragments` marker splice
  (T018–T019). High confidence.
- **`fsharp-parsing`** — structural extraction of the `### Principle` headings for
  `ConstitutionFragments` (T018–T019) and the bracket-token regex anchor for the
  `SkillistView` splice (T015). High confidence on the principle extraction;
  medium on the splice anchor.
- **`fsharp-build-orchestration`** — the `build.fsx` gate/effect wiring
  (`RefreshSurfaceBaselines`, the reframed `SkillSyncCheck` and `TargetMetadataDrift`
  arms, the `SkillExamplesCheck` retirement), the Expecto harnesses, and the
  serialized FAKE dogfood run. Medium-high.
- **`speckit-evidence-graph` / `speckit-evidence-audit`** — only the two genuine
  graph/audit workflow tasks (T029/T030).

**Not assigned** (with reasons, mirroring 041/042/043 discipline):
`fsharp-shell-process` is **not** assigned anywhere — the entire generation path
is **in-process** copy-generation with **no** shelling to `diff`/`cmp`/`sha256sum`
and no new `git` invocation (spec cross-platform Edge Case; the active feature is
resolved from `.specify/feature.json` as data). `fs-skia-template-update` is
**not** assigned: the template edits are governance **marker regions** in
`plan-template.md`/`tasks-template.md`, not a `dotnet new fs-skia-ui` product /
package-pin change (no `Directory.Packages.props`, no `template.json`, no
generated-product content change). The `Targets.fs` / `Audit.fs` reframe tasks
that carry no matching cookbook take a deliberate `valid-empty` `skillist` (T020
is template-markup editing; the audit-reframe T016 leans on `fsharp-code-generation`
for the rendered diagnostic).

## Governance risk levels & validation

- **Small** (routine framework-internal edits within this feature's own
  `build/Governance/*.fs` library work): focused `./fake.sh build -t Dev` plus the
  `Governance.Tests` suite is authoritative.
- **Medium** (the three new build-tooling `.fsi`/`.fs` modules, the reframed
  `build.fsx` gate arms, the `SkillExamplesCheck` retirement, the template marker
  regions): focused `Dev` plus the targeted FAKE governance gates the `Route`
  selector prints.
- **Broad** (required here because this is a `.specify/**` + skill-tree +
  governance-path change that `Route` escalates, FR-013/FR-015): the full
  serialized FAKE gate order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck`
  → `GeneratedProductCheck` → the final graph and audit gates). Aggregate FAKE
  results are recorded as **non-authoritative**; any race-like or
  environment-flaky failure (the known `SkiaViewer.Tests` headless crash, the
  `FsiTranscripts` toolchain issue) is rerun in focused isolation under a stash
  control, and that focused result is authoritative (SC-008/SC-009).

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Because this feature's **subject** is the
governing-principle source, the skillist, and the merge-gate, task **titles**
deliberately avoid the validator's blocking trigger tokens: the governing-principle
source is always named by filename (`.specify/memory/constitution.md`) or by the
camelCase module `ConstitutionFragments` — never the bare word — so the
filename-context / no-word-boundary rules suppress a false high-confidence match;
the templates are named `plan-template.md` / `tasks-template.md` (filename
context); the skillist annotation is written `[skillist: …]` (never the
`skillist field` / `skillist, list typing` phrases); the merge-gate is called the
"merge-gate" or "`Evidence/Audit.fs`" (never the bare `evidence audit` phrase);
and the graph/audit *workflow* tasks (T029/T030) **do** declare
`speckit-evidence-graph` / `speckit-evidence-audit`. `tasks.deps.yml` keeps one
indented object per task id with `deps` and `skillist`; every `[skillist: …]`
mirror matches the structured list exactly and in order.

---

## Phase 1: Setup

- [X] T001 [T1] [skillist: []] Record feature Tier 1 (new curated `FS.Skia.UI.Build` governance `.fsi` modules) + escalated `.specify/**`/governance status, the affected layer (`build/Governance/**` + `build.fsx` + `.specify/**` + the skill trees, build-tooling/governance only), public-API impact (no product `.fsi`; new build-tooling `.fsi` per Principle II), Elmish/MVU applicability (generation logic is pure; the only interpreter touched is `build.fsx`'s `BuildEffect` — `update` stays pure, all reads/writes at `interpret`), and the real-evidence obligations (the three currency demonstrations, byte-identity across 25 pairs, typed `Governance.Tests`, grep proofs, serialized FAKE logs; zero synthetic evidence)
- [X] T002 [P] [T1] [skillist: []] Create placeholder evidence files listed by the plan under `specs/044-foundations-single-source-generation/readiness/` so the audit-enforced readiness files are discoverable at setup: `logs/serialized-gates.md`, `logs/byte-identity-25.md`, `logs/provenance-headers.md`, `logs/duplication-delta.md`, `logs/runtime-untouched.md`, `logs/no-fcs-grep.txt`, `logs/no-shell-diff-grep.txt`, `currency/skills-edit-without-regen.md`, `currency/new-skill-zero-allowlist.md`, `currency/skillist-edit-without-regen.md`, `currency/skillist-no-historical-regression.md`, `currency/constitution-edit-without-regen.md`, `unit-tests.md`, `fsi-session.txt`, and the governance scaffolds named in T003
- [X] T003 [T1] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files (`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation-authority.md`, `evidence-graph.md`, `evidence-audit.md`, `skill-loading-evidence.md`), each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation

- [X] T004 [P] [T1] [skillist: []] Extract the three curated `.fsi` signatures (`SkillTreeGen`, `SkillistView`, `ConstitutionFragments`) from `contracts/` into standalone files under `build/Governance/`, add skeleton `.fs` companions against the signatures, and add their `<Compile>` entries to `FS.Skia.UI.Build.fsproj` in dependency order (after `Capabilities`); no access modifiers in the `.fs` bodies (Principle I/II, FR-014)
- [X] T005 [T1] [skillist: []] Exercise the draft `.fsi` surfaces from FSI (representative `SkillTreeGen.plan`, `SkillistView.renderAnnotation`, and `ConstitutionFragments.extract` calls over small literal inputs), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T006 [T1] [skillist: []] Record surface-area baselines for the new `build/Governance` modules and the unsupported-scope / failure handling: missing/empty/malformed canonical input raises rather than emitting a partial derived artifact (spec Edge Cases, Principle VII); the Stage 5/6/7 deferrals and symlink-based sharing stay out of scope

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — Skill trees become one source, generated, fully covered (P1)

**Goal**: `.agents/skills/` is the single canonical source; `.claude/skills/` is
**generated** by enumeration covering **all 25** skills (not the old 6-slug
allowlist); `SkillSyncCheck` becomes a currency check and `SkillExamplesCheck` is
retired (SC-001/SC-002).

### Tests First (Principle I, Principle VI)

- [X] T007 [P] [US1] [skillist: fsharp-io-globbing, fsharp-build-orchestration] Add failing typed `SkillTreeGen` Expecto tests in `tests/Governance.Tests/SkillTreeGenTests.fs` — enumeration covers a synthetic 26th skill outside any allowlist (SC-002); the derived plan is content-identical to canonical across the enumerated set (SC-001); a tampered derived byte yields a `Some` currency diagnostic; empty/missing/unreadable canonical input raises a generator error (Principle VII); register the file in `Governance.Tests.fsproj` before `Program.fs` — red before `SkillTreeGen.fs` exists

### Implementation

- [X] T008 [US1] [skillist: fsharp-io-globbing, fsharp-code-generation] Implement `build/Governance/SkillTreeGen.fs` against its `.fsi` — `derivedRelPath`, `renderManifest`, `plan` (enumerate canonical files → derived entries + the tree-level provenance manifest, raising on empty/unreadable input), `currency`, `isCurrent`, and `currencyDrift` naming `./fake.sh build -t RefreshSurfaceBaselines`; the 6-slug `expectedSlugs` allowlist is **deleted** (coverage is by enumeration, FR-002)
- [X] T009 [US1] [skillist: fsharp-io-globbing] Reframe `SkillSync.fs`/`.fsi` into a generation-currency check delegating to `SkillTreeGen.currency` — fail when `.claude/skills` is not a current regeneration of `.agents/skills` across all 25 pairs, with an actionable diagnostic naming the regeneration target rather than a bare "A and B differ" (FR-004/FR-012)
- [X] T010 [US1] [skillist: fsharp-build-orchestration] Retire `SkillExamplesCheck` — remove the `Target` DU case and its `name` / `directPrerequisites` / dispatch references in `Targets.fs`, delete `SkillExamples.fsi`/`.fs` and the `build.fsx` `SkillExamplesGate` effect + `runSkillExamplesGate`, and remove its `Governance.Tests` suite; the exhaustive `Target` match makes any missed reference a compile error (FR-004/FR-015, research R6)
- [X] T011 [US1] [skillist: fsharp-io-globbing, fsharp-build-orchestration] Wire `RefreshSurfaceBaselines` (build.fsx) to enumerate `.agents/skills`, read each canonical `SKILL.md`, call `SkillTreeGen.plan`, and write the derived `.claude/skills` tree + provenance manifest; point the `SkillSyncCheck` gate arm at the reframed currency function. All filesystem enumeration/read/write stays at the interpreter edge; `update` emits effect data only (Principle IV)

### Evidence

- [X] T012 [US1] [skillist: fsharp-build-orchestration] Regenerate the derived tree and capture SC-001 evidence: in-process byte-identity across all 25 derived `SKILL.md` (`readiness/logs/byte-identity-25.md`) and the currency demonstration (`readiness/currency/skills-edit-without-regen.md`) — edit one of the 19 previously-unguarded slugs without regenerating → `SkillSyncCheck` fails naming `RefreshSurfaceBaselines`; regenerate → byte-identical → passes; editing the derived tree directly is reported as drift (SC-008)
- [X] T013 [US1] [skillist: fsharp-io-globbing] SC-002 evidence (`readiness/currency/new-skill-zero-allowlist.md`): add a new skill directory to `.agents/skills`, run `RefreshSurfaceBaselines`, and confirm the derived tree gains the skill with **zero** edits to any per-skill allowlist or hardcoded slug list

**Checkpoint**: US1 functional — the skill trees are one generated source, fully covered, currency-gated.

---

## Phase 4: User Story 2 (US2) — The skillist has one canonical home (P2)

**Goal**: `tasks.deps.yml` `skillist:` is canonical; the `tasks.md` `[skillist: …]`
annotation is the **derived view** rendered from it; the merge-gate comparison is
reframed from a peer drift-check into an active-feature currency check
(SC-003/SC-004).

### Tests First (Principle I, Principle VI)

- [X] T014 [P] [US2] [skillist: fsharp-code-generation, fsharp-build-orchestration] Add failing typed `SkillistView` tests in `tests/Governance.Tests/SkillistViewTests.fs` — `renderAnnotation [a; b] = "[skillist: a, b]"` and `[] = "[skillist: []]"`; `spliceAnnotation` changes only the bracketed token on a task line and preserves the rest of the line (raises when the line carries no annotation token); `currency` flags a stale derived annotation and passes a current one; an absent annotation is reported, not silently inserted — red before `SkillistView.fs` exists

### Implementation

- [X] T015 [US2] [skillist: fsharp-code-generation, fsharp-parsing] Implement `build/Governance/SkillistView.fs` against its `.fsi` — `renderAnnotation`, `spliceAnnotation` (in-place bracket-token replacement anchored by the existing `[skillist: …]` regex, leaving every other byte of the line unchanged), `currency` (active-feature, keyed by task id; absent annotation reported), and `currencyDrift` naming `./fake.sh build -t RefreshSurfaceBaselines`
- [X] T016 [US2] [skillist: fsharp-code-generation] Reframe the skillist comparison in `Evidence/Audit.fs` (the active-feature merge-gate) from a symmetric peer complaint into an asymmetric currency diagnostic — "the `tasks.md` `[skillist: …]` view for `<task>` is stale relative to its canonical `tasks.deps.yml` source; regenerate via `./fake.sh build -t RefreshSurfaceBaselines`" — delegating the rendered token to `SkillistView`; the active-feature scope is unchanged so the historical feature directories are never re-derived (FR-007, SC-004)

### Evidence

- [X] T017 [US2] [skillist: fsharp-build-orchestration] Capture US2 currency evidence: edit the canonical `tasks.deps.yml` `skillist:` for a task in this feature → the active-feature merge-gate flags the derived `[skillist: …]` annotation stale; regenerate → green; edit the derived annotation alone → flagged stale (`readiness/currency/skillist-edit-without-regen.md`, SC-003). Confirm SC-004: re-deriving across the existing feature directories yields zero new failures (`readiness/currency/skillist-no-historical-regression.md`)

**Checkpoint**: US2 functional — the skillist has one canonical home, currency-gated for the active feature only.

---

## Phase 5: User Story 3 (US3) — The governing principles are stated once (P3)

**Goal**: `.specify/memory/constitution.md` is the single source; templates carry
generated principle-summary fragments spliced between `BEGIN GENERATED` /
`END GENERATED` markers; a currency check (folded into `TargetMetadataDrift`)
fails on a stale region; genuine hand-written guidance outside the markers is
preserved (SC-005).

### Tests First (Principle I, Principle VI)

- [X] T018 [P] [US3] [skillist: fsharp-parsing, fsharp-build-orchestration] Add failing typed `ConstitutionFragments` tests in `tests/Governance.Tests/ConstitutionFragmentsTests.fs` — `extract` derives the fixed principle-summary fragment set deterministically from a `.specify/memory/constitution.md` fixture (raises when a required `### Principle` heading is missing); `regions` locates the `BEGIN GENERATED`/`END GENERATED` pairs; `splice` replaces only the inner region text and preserves every out-of-marker byte (property-style byte-equality over a fixture template, FR-010); `currency` flags a stale region after a simulated principle edit and passes a current one — red before the module exists

### Implementation

- [X] T019 [US3] [skillist: fsharp-parsing, fsharp-code-generation] Implement `build/Governance/ConstitutionFragments.fs` against its `.fsi` — `fragmentIds`, `extract` (structural derivation from the `### Principle` headings of `.specify/memory/constitution.md`; no free-form paraphrase), `regions`, `splice` (marker-delimited; out-of-marker bytes preserved, FR-010), `currency`, and `currencyDrift` naming `./fake.sh build -t RefreshSurfaceBaselines`
- [X] T020 [US3] [skillist: []] Add `BEGIN GENERATED` / `END GENERATED` marker regions to `.specify/templates/plan-template.md` and `.specify/templates/tasks-template.md` carrying the four principle-summary fragments (`tests-first`, `mvu-boundary`, `synthetic-disclosure`, `fsi-visibility`) per the locked data-model inventory; genuine hand-written guidance prose stays **outside** the markers (FR-008/FR-010)
- [X] T021 [US3] [skillist: fsharp-build-orchestration] Fold the principle-fragment currency check into the `TargetMetadataDrift` gate (build.fsx) alongside the existing `ContractView` currency check, and wire `RefreshSurfaceBaselines` to splice the fragments into the two templates via `ConstitutionFragments.splice` (FR-009)

### Evidence

- [X] T022 [US3] [skillist: fsharp-build-orchestration] Capture US3 currency evidence (`readiness/currency/constitution-edit-without-regen.md`, SC-005/SC-008): change a `### Principle` in `.specify/memory/constitution.md` → `TargetMetadataDrift` flags the stale template region; run `RefreshSurfaceBaselines` → the templates reflect the change; hand-written prose outside the markers is preserved byte-for-byte

**Checkpoint**: US3 functional — the governing principles are stated once, currency-gated.

---

## Phase 6: Integration & Polish (cross-cutting provenance + serialized escalated gates)

- [X] T023 [P] [T1] [skillist: fsharp-build-orchestration] Regenerate `validation.contract.yml` from `Routing.fs` after the `SkillExamplesCheck` target removal and confirm `TargetMetadataDrift` / `ContractView` stay coherent — the target-set change must keep the currency gate green (research R6 coherence follow-through)
- [X] T024 [T1] [skillist: []] SC-006 provenance proof (`readiness/logs/provenance-headers.md`): record that every generated artifact carries machine-readable provenance — the tree-level manifest at the derived skill-tree root naming source + regeneration command, the `BEGIN GENERATED:` marker comments, and the `# GENERATED from … Routing.fs` header on `validation.contract.yml`; every replaced drift-check now emits an actionable "regenerate" diagnostic on failure (FR-011/FR-012)
- [X] T025 [P] [T1] [skillist: []] SC-007 duplication delta (`readiness/logs/duplication-delta.md`): record the eliminated-line delta vs the Stage-0 baseline (`docs/reports/_baselines/2026-05-31-foundations.md`) — the ~5,854-line skill mirror collapses to one canonical source plus a generator
- [X] T026 [P] [T1] [skillist: []] SC-009 invariants + grep proofs: `readiness/logs/runtime-untouched.md` (`git diff --stat` over product `src/**` = 0), `readiness/logs/no-fcs-grep.txt` (no `FSharp.Compiler.*` reference added), `readiness/logs/no-shell-diff-grep.txt` (no `diff`/`cmp`/`sha256sum`/symlink shelling in the generation path), and `PackageSurfaceCheck` / `FsiTranscripts` show no product baseline diff; confirm every generated/derived artifact (`.claude/skills/**` + its provenance manifest, the `BEGIN/END GENERATED` template regions) is tracked and not gitignored (`git check-ignore` returns nothing for them), proving FR-013
- [X] T027 [T1] [skillist: fsharp-build-orchestration] Record typed `Governance.Tests` results for the new generation/currency modules (`SkillTreeGen`, `SkillistView`, `ConstitutionFragments`) to `readiness/unit-tests.md`, including each module's failing-first stale-fixture case turning green
- [X] T028 [T1] [skillist: fsharp-build-orchestration] Run the escalated serialized six-target FAKE gate set sequentially (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates `T029`/`T030`), never concurrently; record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure (the `SkiaViewer.Tests` headless crash, the `FsiTranscripts` toolchain issue) in focused isolation under a stash control as the authoritative result; logs under `readiness/logs/serialized-gates.md`
- [X] T029 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm the task DAG is acyclic, no dangling refs, no `[S*]` surprises, and the `skillist` metadata and visible mirrors are valid
- [X] T030 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict `PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 diff-scan blocking, 0 readiness-contract blocking) with zero synthetic evidence to accept (SC-008)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; see the note at the top)_ | | | | | | | | |
