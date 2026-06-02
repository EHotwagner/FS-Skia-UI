# Tasks: Codify Remaining Rules, Trim Prose, Version the Contract (Stage 6)

**Feature branch**: `046-foundations-rule-codification`
**Spec**: `specs/046-foundations-rule-codification/spec.md`
**Plan**: `specs/046-foundations-rule-codification/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

**This feature ships zero synthetic evidence.** All evidence is real — typed
Expecto unit tests over the real plan-parser and the real versioned-contract
model; deliberate **seeded violations** that prove each enforcing gate actually
fails before its prose is deleted; a live `fail → fix → pass` of the new
governance-decision-completeness gate on this feature's own `plan.md`; the real
`.agents → .claude` generation-currency check; real `git check-ignore` output
with a still-tracked control; and the serialized escalated FAKE gate logs. No
`[S]`/`[SEH]` task is approved (Principle V); `EvidenceAudit` must return
`verdict=PASS` with zero synthetic (SC-010).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]** governance-decision-completeness gate, **[US2]** versioned
  generated-product contract, **[US3]** prose trim, **[US4]** `.gitignore`
- **[T1]** / **[T2]** — overall this feature is **Tier 2 (internal build-tooling
  / governance)**; the **US2** story is **[T1]** because it changes a *consumer*
  contract (the generated-product structural contract gains `schema_version` + a
  deprecation window). No product `.fsi` / surface-baseline change (SC-009).
  Because it touches governance paths (`build/Governance/**`, `.agents/skills/**`,
  `.specify/**`) and a generated-product-contract surface, `Route` **escalates**
  it; it is run as a **dogfood** feature through the full serialized set.
- **[SEH]** — design-approved synthetic error-handling task (none in this feature)

Every task has a matching entry in `tasks.deps.yml`. Each task line mirrors its
structured `skillist` as `[skillist: ...]`; `[skillist: []]` means no capability
skill applies.

## Skill-assignment note (read first)

Like features 041–045 this feature is **build-tooling / governance only**
(`build/Governance/**` + `tests/Governance.Tests/**` + `.agents/skills/**` +
`.gitignore` + the active spec tree), so **no** `fs-skia-*` runtime / rendering /
viewer / layout / widgets skill applies — there is no scene, window, Elmish
runtime, input, or visual surface in scope. It *consumes* the `fsharp-*`
cookbooks as genuine implementation aids:

- **`fsharp-parsing`** — the new governance-decision-completeness validator parses
  the active feature's `plan.md` *Repository Governance Decisions* section with the
  existing `Guidance.markdownSections` parser (T010), and its typed-result test
  exercises that grammar (T008). High confidence (the exact remit: "Parse
  governance inputs … tasks.md line grammar … in compiled F#").
- **`fsharp-code-generation`** — the versioned-contract module embeds typed
  `schema_version` / rule-lifecycle / changelog data and renders the contract
  header surfaced in `GeneratedProductCheck` output (T014/T015). Medium-high.
- **`fsharp-build-orchestration`** — the Expecto typed-result tests (T008/T013),
  folding the new finding into the existing `GeneratedGuidanceCheck` gate (T011),
  and the serialized escalated FAKE run (T024). High confidence.
- **`speckit-evidence-graph` / `speckit-evidence-audit`** — only the two genuine
  graph/audit workflow tasks (T025/T026).

**Not assigned** (with reasons): `fsharp-graph-algorithms` — this feature touches
no DAG / topo-sort / synthetic-propagation logic (that is the evidence engine,
feature 043). `fsharp-io-globbing` — the generated-project tree discovery already
exists in `GeneratedProduct.fs`; US2 only *consults* a new typed contract, it adds
no new globbing. `fsharp-shell-process` — no new `git`/process wrapping (the
`.gitignore` checks and seeded-violation captures run existing tools). `fs-skia-template-update`
— there is **no** `dotnet new fs-skia-ui` product / package-pin / `template.json`
change; contract *versioning* is typed F# data in `build/Governance/`, not a
template asset edit. `speckit-constitution` — the new gate is governance **code**
(a `plan.md` parser folded into `GeneratedGuidanceCheck`), **not** an edit to
`.specify/memory/constitution.md` or its templates; the task lines therefore use
the camelCase `ConstitutionCheck` identifier (the F# type/module/test name) so the
graph validator's `constitution` trigger group is not falsely fired (the same
camelCase discipline feature 044 used for `ConstitutionFragments`).

The prose-trim, `.gitignore`, fixture, readiness, and live-run-and-capture tasks
take a deliberate `valid-empty` `skillist` (markdown/`git` edits and gate-run
evidence capture carry no F# cookbook).

## Governance risk levels & validation

- **Small** (routine framework-internal edits inside this feature's own
  `build/Governance/*.fs` / `tests/Governance.Tests/*.fs` work): focused
  `./fake.sh build -t Dev` plus the `Governance.Tests` suite is authoritative and
  the **required evidence** for the level.
- **Medium** (the new `Guidance.fs` validator surface, the new
  `GeneratedProductContract.fs(/.fsi)` module, and the `GeneratedProduct.fs`
  consult-point): focused `Dev` plus the targeted gates `Route` prints
  (`GeneratedGuidanceCheck`, `GeneratedProductCheck`) and the typed unit tests.
- **Broad** (required here because this is a governance-path + generated-product-
  contract + `.agents/skills/**` change that `Route` escalates): the full
  serialized FAKE gate order (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck`
  → `GeneratedProductCheck` → the final graph and audit gates). **Broad
  validation is required** whenever a code-enforced rule's prose is deleted or the
  generated-product contract changes. Aggregate FAKE results are recorded as
  **non-authoritative**; any race-like or environment-flaky failure (the known
  `SkiaViewer.Tests` headless crash) is rerun in focused isolation and that
  focused result is authoritative.

## Pre-graph-gate pitfall guidance

Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`)
before declaring this phase complete. Task **titles** deliberately avoid the
validator's blocking trigger tokens: the new gate is named by its camelCase F#
identifier `ConstitutionCheck` (never the space/hyphen forms `Constitution Check`
/ `Constitution-Check`, which *would* fire the `constitution` trigger group); the
bare phrases `evidence graph` / `task graph` / `evidence audit` / `diff-scan` /
`synthetic propagation` / `validator diagnostics` are never used on a non-graph/
non-audit task; the graph/audit *workflow* tasks (T025/T026) **do** declare
`speckit-evidence-graph` / `speckit-evidence-audit` and name `EvidenceGraph` /
`EvidenceAudit` directly; and the readiness-aggregation task (T003) uses the
`Complete readiness notes` prefix so its filename citations
(`evidence-graph.md`, `evidence-audit.md`) do not fire the graph/audit capability
checks. There is **no** viewer, persistent-launch, or window-visibility work, so
no such trigger phrase appears. `tasks.deps.yml` keeps one indented object per
task id with `deps` and `skillist`; every `[skillist: …]` mirror matches the
structured list exactly and in order.

---

## Phase 1: Setup

- [X] T001 [T2] [skillist: []] Record feature Tier 2 (internal build-tooling / governance, escalated by `Route` to the full serialized set as a `build/Governance/**` + `.agents/skills/**` + `.specify/**` + generated-product-contract change), the affected layer (`build/Governance/Guidance.fs(/.fsi)`, new `build/Governance/GeneratedProductContract.fs(/.fsi)`, `build/Governance/GeneratedProduct.fs(/.fsi)`, `tests/Governance.Tests/**`, `.agents/skills/**`, `.gitignore`), the public-API impact (no product `.fsi`/surface-baseline change — SC-009; the **consumer** generated-product contract gains `schema_version` + a deprecation window, US2/T1), the Elmish/MVU applicability (product runtime untouched; the new validators are **pure functions** returning typed results with file I/O confined to the existing `interpret`/`Front` edge — Principle IV satisfied, product MVU **not applicable**), and the real-evidence obligations (typed unit tests, seeded-violation proofs, live gate fail→fix→pass, generation-currency green, `git check-ignore` proof, prose-delta measurement, serialized escalated FAKE logs; zero synthetic)
- [X] T002 [P] [T2] [skillist: []] Create placeholder evidence files listed by the plan under `specs/046-foundations-rule-codification/readiness/` so the audit-enforced readiness files are discoverable at setup: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md`, `evidence-policy-separation.md`, `prose-delta.md`, `gitignore-check.md`, `unit-tests.md`, `fsi-session.txt`, `seeded-violations/` (one note per deleted rule), and `logs/` (`dev.log`, `generated-guidance-check.log`, `template-check.log`, `generated-product-check.log`, `evidence-graph.log`, `evidence-audit.log`)
- [X] T003 [T2] [skillist: []] Complete readiness notes for the feature's required readiness placeholder files (`governance-risk-levels.md` with the small/medium/broad levels, their required evidence, and when broad validation is required; `aggregate-hang-diagnostics.md` with verdict / stage / elapsed duration / last observed command / focused rerun / non-authoritative aggregate; `runtime-limitations.md` with the .NET 10 desktop / Vulkan / SkiaSharp preview / unsupported macOS/mobile/browser / no software-renderer fallback statements; and `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md`, `evidence-policy-separation.md`), each naming its authoritative command, artifact path, failure class, and next action

---

## Phase 2: Foundation (`.fsi` surfaces fixed first)

- [X] T004 [P] [T2] [skillist: []] Draft the `ConstitutionCheck` validator surface in `build/Governance/Guidance.fsi` — `requiredDecisionAreas: RequiredDecisionArea list`, `classifyConstitutionCheck: string -> ConstitutionCheckResult`, `constitutionCheckFindings: string -> ConstitutionCheckResult -> Findings.ValidationFinding list`, and the `RequiredDecisionArea` / `AreaStatus` / `ConstitutionCheckResult` types from `data-model.md`; add the matching skeleton bodies in `Guidance.fs` (no access modifiers, Principle I/II) so the module compiles `TreatWarningsAsErrors`-clean against the new signature
- [X] T005 [P] [T1] [skillist: []] Draft the new `build/Governance/GeneratedProductContract.fsi` surface — `ContractSchemaVersion`, `RuleLifecycle` (`Required | Deprecated of removalVersion | Removed`), `StructuralRule`, `ContractChangeKind`, `ContractChangelogEntry`, `GeneratedProductContract`, `RuleOutcome` (`Pass | Warn of string | Fail`), plus `current`, `classifyViolation: GeneratedProductContract -> string -> RuleOutcome` and `renderContractHeader: GeneratedProductContract -> string` (from `data-model.md` / `contracts/generated-product-contract.md`); add the skeleton `.fs` companion and its `<Compile>` entry to `FS.Skia.UI.Build.fsproj` ahead of `GeneratedProduct`
- [X] T006 [T2] [skillist: []] Exercise the draft `.fsi` surfaces from FSI (a representative `classifyConstitutionCheck` over a small literal complete/incomplete plan body, and a `classifyViolation` over a small literal contract with a `Required` and a `Deprecated` rule), capturing the session transcript to `readiness/fsi-session.txt`
- [X] T007 [T2] [skillist: []] Record surface-area handling for the new/changed **build-tooling** `.fsi` modules and the unsupported-scope / failure diagnostics: these are build-tooling `.fsi` (not tracked product baselines — `PackageSurfaceCheck`/`FsiTranscripts` show **no** product baseline diff, intentional per Principle II), the contract header renders an explicit `schema_version`, and the unrecognized-template-revision path emits a distinct actionable diagnostic rather than a false pass

**Checkpoint**: Foundation ready — `.fsi` surfaces fixed; story implementation may begin.

---

## Phase 3: User Story 1 (US1) — the Constitution Check stops being honour-system prose (P1)

**Goal**: a build-failing gate parses the active feature's `plan.md` *Repository
Governance Decisions* section and fails, naming each missing/unfilled decision
area, surfaced through the **existing** `GeneratedGuidanceCheck` (no new FAKE
target). N/A-with-rationale counts as filled; an unrecognized template revision
yields a distinct diagnostic (SC-001, FR-001/002/003).

### Tests First (Principle I, Principle VI)

- [X] T008 [P] [US1] [skillist: fsharp-parsing, fsharp-build-orchestration] Add failing-first typed unit tests in `tests/Governance.Tests/ConstitutionCheckTests.fs` asserting `ConstitutionCheckResult` / `Findings.ValidationFinding` values (no string matching): all 11 areas filled → pass; each unfilled variant (empty / still-boilerplate / `NEEDS CLARIFICATION`-or-`TODO` placeholder) → a finding naming the exact area id and the plan path; an area marked N/A-with-rationale → filled/pass; a live template that no longer maps to the typed identifiers → the `UnrecognizedTemplateRevision` case — register the file in `Governance.Tests.fsproj` before `Program.fs` and record the failing-first (RED) capture in `readiness/unit-tests.md`
- [X] T009 [P] [US1] [skillist: []] Add the `plan.md` parser fixtures under `tests/Governance.Tests/fixtures/` — a complete plan, a missing/blanked-area plan, a still-boilerplate-prompt plan, a `NEEDS CLARIFICATION`/`TODO` plan, an N/A-with-rationale plan, and a future/renamed-template plan — covering every case asserted by T008

### Implementation

- [X] T010 [US1] [skillist: fsharp-parsing] Implement `requiredDecisionAreas`, `classifyConstitutionCheck`, and `constitutionCheckFindings` in `build/Governance/Guidance.fs` against the `.fsi` — parse the *Repository Governance Decisions* section with the existing `markdownSections` / `tryHeading` helpers, key off the **hard-coded 11 stable area identifiers** (not exact headings), reuse `Guidance.planGuidancePrompts` as the still-boilerplate sentinels, treat N/A-with-rationale as `Filled`, and emit the `UnrecognizedTemplateRevision` diagnostic when the live `plan-template.md` no longer maps to the identifiers (FR-001/003); turn T008 GREEN
- [X] T011 [US1] [skillist: fsharp-build-orchestration] Fold the `ConstitutionCheck` findings into `Guidance.runGeneratedGuidanceScan` so a non-empty unfilled-area set (or the unrecognized-template-revision case) contributes build-failing `ValidationFinding`s to the existing `GeneratedGuidanceCheck` aggregate report, each naming the area display name and the `plan.md` path; a complete plan adds zero findings (FR-002, A5 — no new top-level target)
- [X] T012 [US1] [skillist: []] Live `fail → fix → pass` for the `ConstitutionCheck` gate on this feature's own `plan.md` via `./fake.sh build -t GeneratedGuidanceCheck` — PASS complete → blank one required area → FAIL naming that exact area → restore → PASS; capture the three runs under `readiness/seeded-violations/constitution-check.md` (SC-001)

**Checkpoint**: User Story 1 is fully functional and independently testable.

---

## Phase 4: User Story 2 (US2) — the generated-product contract can evolve without a hard break (P1)

**Goal**: the generated-product structural contract carries a `schema_version`
and a deprecation window — a `Deprecated`-only violation warns (naming the removal
version) until that version ships, then hard-fails; a typed changelog records the
transitions; a current generated project stays green (SC-002/SC-003, FR-004/005/006).

### Tests First

- [X] T013 [P] [US2] [T1] [skillist: fsharp-build-orchestration] Add failing-first typed unit tests in `tests/Governance.Tests/GeneratedProductContractTests.fs` asserting the `RuleOutcome` of `classifyViolation` over a literal contract: a `Required` rule violated → `Fail`; a `Deprecated removalVersion` rule violated while `SchemaVersion < removalVersion` → `Warn` naming the removal version; the same rule once `SchemaVersion >= removalVersion` → `Fail` (window closed); a rule promoted `Deprecated → Required` → `Fail`; a `Removed` rule → not evaluated; and assert the typed `Changelog` records each transition and `renderContractHeader` exposes the `schema_version` — register before `Program.fs`, capture RED in `readiness/unit-tests.md`
- [X] T014 [US2] [T1] [skillist: fsharp-code-generation] Implement `build/Governance/GeneratedProductContract.fs` against its `.fsi` — the typed `ContractSchemaVersion`, the per-rule `RuleLifecycle`, the `current` contract value wrapping the existing structural-rule ids, the embedded typed `Changelog`, `classifyViolation` (the `Required`/`Deprecated`/window-closed/`Removed` evaluation rule from `data-model.md` R4), and `renderContractHeader` (schema_version + changelog summary); turn T013 GREEN
- [X] T015 [US2] [T1] [skillist: fsharp-code-generation] Consult the versioned contract from `GeneratedProduct.runScanV3GeneratedProducts` so a product that violates **only** a `Deprecated` rule emits a warning (naming the removal version) instead of a finding, while `Required`/window-closed rules still fail, and surface `renderContractHeader` in the `GeneratedProductCheck` output — the existing structural checks stay behaviour-identical for `Required` rules (no product regression)
- [X] T016 [US2] [T1] [skillist: []] Live `GeneratedProductCheck` evidence (SC-002/SC-003) — `./fake.sh build -t GeneratedProductCheck` on a current generated project stays green with the `schema_version` visible in output; demonstrate `warn → promote → fail` (a product violating only a `Deprecated` rule warns; after bumping `schema_version` and promoting the rule to `Required` the same product fails, with the changelog recording both); capture under `readiness/logs/generated-product-check.log`
- [X] T027 [US2] [T1] [skillist: fsharp-build-orchestration] Add a failing-first typed unit test in `tests/Governance.Tests/GeneratedProductContractTests.fs` asserting changelog⇄`SchemaVersion` consistency over the `current` contract — every breaking `ContractChangelogEntry` (`PromotedToRequired` / `RuleRemoved`) has a `Version` strictly greater than the prior schema version, and `current.SchemaVersion` is ≥ the maximum changelog-entry version — then implement that pure consistency check in `build/Governance/GeneratedProductContract.fs` so a breaking rule change that forgets the bump fails the test instead of relying on reviewer attention; turn it GREEN and record the RED capture in `readiness/unit-tests.md` (FR-006, SC-011, gate-enforces the C1 bump obligation)

**Checkpoint**: User Story 2 is fully functional and independently testable.

---

## Phase 5: User Story 3 (US3) — prose the code now enforces is removed (P2)

**Goal**: every rule a build-failing gate now enforces is deleted from
`.agents/skills/**` (replaced by a one-line pointer where useful), genuine
rationale/intent kept, `.claude/skills/**` regenerated byte-identically, and the
line/byte delta recorded — **gate-before-prose** (FR-007/008/009/010, SC-004/005/006).

- [X] T017 [P] [US3] [skillist: []] Prove the three already-shipped Stage-6.1 gates still block, via seeded violations, before any prose is deleted (FR-008): the late-`[SEH]` design-phase-timing rule (`Evidence/Audit.fs` `late-seh-tasks`), the skill-id resolution / no-dangling-id rule (`Evidence/Engine.fs`), and the surface-baseline-presence rule (`Capabilities.fs`); seed one violation per rule, confirm each fails the build, restore, and record the three proofs under `readiness/seeded-violations/`
- [X] T018 [US3] [skillist: []] Trim the code-enforced rule prose from `.agents/skills/**` — delete the rule statements now enforced by the four proven gates (the new `ConstitutionCheck` completeness gate plus the three Stage-6.1 gates from T017), replacing each with a one-line pointer to its enforcing gate where useful, and **keep** genuine rationale / intent / when-to-use guidance; perform a deletion only for a rule with a recorded seeded-violation proof (T012, T017)
- [X] T019 [US3] [skillist: []] Regenerate `.claude/skills/**` byte-identically from the trimmed `.agents/skills/**` via `./fake.sh build -t RefreshSurfaceBaselines`, then confirm the feature-044 generation-currency / skill-sync check stays green via `./fake.sh build -t GeneratedGuidanceCheck` (FR-009, SC-005) — the two skill trees remain byte-identical
- [X] T020 [US3] [skillist: []] Record the before/after governance-Markdown line count and the per-invocation skill-byte load versus the Stage-0 baseline (≈ 6,882 rule/guidance lines today, **not** the overstated ~23,000 — spec A2), with the reproduction command for each figure, in `readiness/prose-delta.md`; justify every rule-prose passage retained as genuine guidance (FR-010, SC-006)

**Checkpoint**: User Story 3 complete — code-enforced prose gone, skill trees byte-identical, delta recorded.

---

## Phase 6: User Story 4 (US4) — future regenerable evidence stays out of git (P3)

**Goal**: a forward-looking `.gitignore` rule excludes regenerable readiness logs
and `readiness*.zip` archives without ignoring authored evidence; no committed
evidence removed, no history rewritten (FR-011/012, SC-007, decision D3).

- [X] T021 [US4] [skillist: []] Add scoped forward-looking patterns to `.gitignore` for regenerable readiness logs and `readiness*.zip` archives, scoped to `specs/*/readiness/logs/**` and `specs/*/readiness/**/readiness*.zip` so authored non-regenerable evidence (all `*.md` notes **and** the `fsi-session.txt` transcript) stays tracked — never a broad non-`.md` sweep (FR-011); make **no** removal of committed evidence and **no** history rewrite (FR-012/D3)
- [X] T022 [US4] [skillist: []] Capture the `.gitignore` proof in `readiness/gitignore-check.md` — `git check-ignore -v` on a freshly generated `readiness*.zip` / readiness log shows it ignored, and `git ls-files --error-unmatch` on **two** previously-committed evidence files — a `*.md` note and the non-`.md` `fsi-session.txt` transcript — shows both still tracked (controls proving the scope spares authored `.txt`, not only `.md`); confirm no committed evidence was removed (SC-007)

**Checkpoint**: User Story 4 complete — regenerable evidence ignored going forward, committed evidence untouched.

---

## Phase 7: Integration & Polish (invariants, serialized escalated gates)

- [X] T023 [P] [T2] [skillist: []] SC-009 standing-invariants proof to `readiness/logs/runtime-untouched.md` — `git diff --stat` over product `src/**` = 0 (runtime / `.fsi` untouched), `PackageSurfaceCheck` / `FsiTranscripts` show no product baseline diff, generated consumers stay governed, no new `PackageVersion` lives outside `Directory.Packages.props`, and invariants 1–6 hold (FR-014)
- [X] T024 [T2] [skillist: fsharp-build-orchestration] Run the escalated serialized six-target FAKE gate set sequentially (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → the final graph and audit gates T025/T026), never concurrently; record aggregate FAKE results as **non-authoritative** and rerun any race-like or environment-flaky failure (the known `SkiaViewer.Tests` headless crash) in focused isolation as the authoritative result; logs under `readiness/logs/serialized-gates.md`
- [X] T025 [skillist: speckit-evidence-graph] Run the in-process compiled-F# graph gate (`./fake.sh build -t EvidenceGraph`) — confirm the DAG is acyclic, no dangling refs, no `[S*]` surprises, and the `skillist` metadata and visible mirrors are valid
- [X] T026 [skillist: speckit-evidence-audit] Run the merge-gate audit (`./fake.sh build -t EvidenceAudit`) — confirm `verdict=PASS` (0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 diff-scan blocking, 0 readiness-contract blocking) with zero synthetic evidence to accept (SC-010)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — this feature ships zero synthetic evidence; every test asserts real typed results and every gate proof is a real seeded failure)_ | | | | | | | | |
