# Tasks: FsDocs Documentation Site on GitHub Pages

**Feature branch**: `076-fsdocs-documentation-site`
**Spec**: `specs/076-fsdocs-documentation-site/spec.md`
**Plan**: `specs/076-fsdocs-documentation-site/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. None is anticipated for this feature:
literate `.fsx` examples are real and build-evaluated, the closing analyses are
authored opinion grounded in real ADRs/reports, and there is no malformed-input
or forced error-path obligation (plan "Synthetic evidence: None expected").

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the user-reachable surface
was actually exercised. For this documentation feature the user-facing surface is
the **built site**: a `[US*]` task is `[X]` only when the relevant page/reference
renders in a real `dotnet fsdocs build` (captured under `readiness/`), not merely
when the source Markdown/`.fsi`/`.fsx` was written. Principle IV (MVU) is **N/A**
— this feature is documentation + doc-build configuration with no
`Model`/`Msg`/`Effect` of its own.

## Success-criterion → assertion mapping

- **SC-001** (zero empty API stubs) → verified by **source coverage**: every
  `val`/`member`/`abstract`/`type` in the published `.fsi` carries a `///` summary
  (recorded in `readiness/api-coverage.md`). NOTE: fsdocs 22.1.0 `--strict` +
  `FsDocsWarnOnMissingDocs` does **not** fail on a missing member summary (it only
  emits non-fatal **parameter**-level warnings and exits 0) — so the source-coverage
  audit, not a build-failure gate, is the enforceable SC-001 signal. Driven to 0
  undocumented authored members in T011/T012.
- **SC-002** (every architecture page closes with both-sided analysis) → the
  `Governance.Tests` analysis-section check (T006) over `docs/architecture/**`
  and the deep-dive section indexes; enforced by T017.
- **SC-007 / FR-004** (public contract unchanged) → `PackageSurfaceCheck` +
  `PerPackageSurfaceDiff` report no diff after the `.fsi` doc work (T030).
- **SC-009** (literate examples evaluate) → `--strict` fails on a broken `.fsx`
  (T024, T025, T027).
- **SC-003 / SC-004 / SC-008** (comprehension & navigation outcomes — practitioner
  can state each governance touchpoint's speckit phase; consumer can describe the
  token→typed-control flow and its speckit phase; first-time visitor reaches their
  role entry point in ≤ 2 steps) have **no mechanical gate**; they are verified by
  human review and the verdict is recorded in `readiness/manual-sc-verification.md`
  — SC-003 at T020, SC-004 at T022/T023, SC-008 at T008.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US5]** — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change. This
  feature is a mixed change: `.fsi` doc comments route to `package-surface`
  (contracted) and `docs/**` to `docs-only`; the authoritative tier + minimal
  gate list MUST come from `./fake.sh build -t Route` for the actual diff (T031).

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors the
structured `skillist` value as `[skillist: ...]` (`[skillist: []]` when empty).

## Governance risk levels & readiness discovery

- **Small**: a single `.fsi` doc-comment edit or one Markdown page → focused
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff` (surface) or strict `fsdocs build`
  (content). **Medium**: a section (e.g. all governance pages) → strict build +
  analysis-section check. **Broad**: the full site + publish → `Route` set +
  `EvidenceGraph` + `EvidenceAudit`. Broad validation is required at merge; its
  aggregate results are recorded non-authoritatively under `readiness/` and the
  authoritative per-gate verdict is taken from `Route`.
- Audit-enforced readiness files are scaffolded in T001 before implementation:
  `readiness/validation-contract.md`, `readiness/surface-baseline-unchanged.md`,
  `readiness/api-coverage.md`, `readiness/runtime-limitations.md`,
  `readiness/governance-risk-levels.md`, `readiness/manual-sc-verification.md`,
  and `readiness/logs/` for
  `fsdocs-build.txt`, `route.txt`, `pages-deploy.txt`. Each names its
  authoritative command, artifact path, failure class, and next action.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Create `readiness/` and `readiness/logs/` with audit-enforced placeholder files (`validation-contract.md`, `surface-baseline-unchanged.md`, `api-coverage.md`, `runtime-limitations.md`, `governance-risk-levels.md`, `manual-sc-verification.md`), each naming its authoritative command, artifact path, failure class, and next action. `manual-sc-verification.md` records the human verdict for the comprehension/navigation criteria with no mechanical gate (SC-003, SC-004, SC-008)
- [X] T002 [skillist: fsdocs-setup] Pin `fsdocs-tool` in `.config/dotnet-tools.json`; merge `FsDocs*` properties (site root, source link, theme, `FsDocsWarnOnMissingDocs`) into `Directory.Build.props` without overwriting existing props; add `output/`, `.fsdocs/`, `tmp/` to `.gitignore` (establishes the fsdocs single-site toolchain, FR-001)
- [X] T003 [P] [skillist: fsdocs-setup] Resolve the GitHub Pages base subpath (`/FS-Skia-UI/`) and the fsdocs source-link `RepositoryUrl` per research R2 — override only the fsdocs root/source-link, leaving packable `PackageProjectUrl` metadata untouched
- [X] T004 [P] [skillist: []] Record feature Tier, affected layer, public-API impact (the FR-004 doc-only invariant), MVU-not-applicable, and the required evidence obligations into the readiness notes

---

## Phase 2: Foundation

- [-] T005 [skillist: []] Add a thin FAKE `Docs` target wrapping `dotnet fsdocs build --strict`; regenerate `validation.contract.yml` from `Routing.fs` and update `TargetMetadata` together (build-target-contract escalation — keep `build.fsx`, the generated contract, and metadata in lockstep so `TargetMetadataDrift` stays green)
- [X] T006 [P] [skillist: []] Add a `Governance.Tests` analysis-section check asserting every page under `docs/architecture/**` and each deep-dive section index closes with a delineated analysis naming both implementation strengths AND weaknesses and both design pros AND cons (SC-002 / FR-006)
- [X] T007 [P] [skillist: fsdocs-build] Add `.github/workflows/docs.yml`: `configure-pages` → `dotnet tool restore` → `dotnet fsdocs build --strict` → `upload-pages-artifact` (`output/`) → `deploy-pages`, with `pages: write` / `id-token: write` and a `github-pages` environment; trigger on push to `main` + `workflow_dispatch`; commit no generated output (FR-012)
- [X] T008 [P] [skillist: fsdocs-setup] Create `docs/index.md` landing page with role-based navigation (consumer / contributor / speckit practitioner each reachable in ≤ 2 steps) plus the section skeleton (FR-016 / SC-008)
- [X] T009 [P] [skillist: fsdocs-build] Capture the failing-first strict build — `FsDocsWarnOnMissingDocs` + `--strict` reporting undocumented supported members (red) — to `readiness/logs/fsdocs-build.txt` (the red signal SC-001 will turn green)

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 — Library consumer finds and understands the public API (US1, P1)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: fsdocs-build] Build the supported-member inventory per package from the per-package surface baselines into `readiness/api-coverage.md`, listing every undocumented supported member (failing-first coverage table)

### Implementation

- [X] T011 [US1] [skillist: fsdocs-api-doc] Add `///` XML doc comments on the `.fsi` signature files for every supported public member across the 10 published packages (FR-002 / FR-003); confirm internal-only / unsupported members are excluded from the supported reference (FR-014)
- [X] T012 [US1] [skillist: fsdocs-build] Build the API reference; verify every supported member shows a non-empty `<summary>` (zero stubs), parameters/returns render where applicable, and a known public type resolves in site search; update `readiness/api-coverage.md` to 0 undocumented (SC-001, US1 acceptance scenarios 1 & 3)

**Checkpoint**: User Story 1 — the generated API reference renders with real summaries.

---

## Phase 4: User Story 2 — Newcomer understands the architecture, part by part, with honest analysis (US2, P1)

- [X] T013 [P] [US2] [skillist: fsdocs-technical] Author `docs/architecture/host-skiaviewer.md` (rendering/host) and `docs/architecture/scene.md`, each with an architecture body grounded in the existing ADRs/reports and a closing strengths/weaknesses + pros/cons analysis (FR-005 / FR-006)
- [X] T014 [P] [US2] [skillist: fsdocs-technical] Author `docs/architecture/layout.md` and `docs/architecture/input.md` (Input + KeyboardInput share one page), each with a closing analysis (FR-005 / FR-006)
- [X] T015 [P] [US2] [skillist: fsdocs-technical] Author `docs/architecture/elmish-mvu.md` and `docs/architecture/controls.md` (Controls + Controls.Elmish suite share one page), each with a closing analysis (FR-005 / FR-006)
- [X] T016 [P] [US2] [skillist: fsdocs-technical] Author `docs/architecture/testing-skillsupport.md` and the `docs/architecture/governance.md` overview page, each with a closing analysis (FR-005 / FR-006)
- [X] T017 [US2] [skillist: fsdocs-build] Run the analysis-section governance check (T006) plus a strict build over `docs/architecture/**`; confirm every major part is covered and each page closes with a both-sided analysis (SC-002 / US2 acceptance scenarios 1–3)

**Checkpoint**: User Story 2 — every major part documented and analysis-gated.

---

## Phase 5: User Story 3 — Practitioner learns the governance system and its speckit placement (US3, P1)

- [X] T018 [P] [US3] [skillist: fsdocs-technical] Author `docs/governance/index.md` and `docs/governance/routing-and-gates.md` explaining tier-and-gate selection (the `Route` selector) with practitioner usage guidance (how to run and respond) (FR-007 / FR-008)
- [X] T019 [P] [US3] [skillist: fsdocs-technical] Author `docs/governance/evidence-and-audit.md` (evidence model, `[S]`/`[S*]` propagation, merge-gate audit) and `docs/governance/single-source-generation.md` (`validation.contract.yml` from `Routing.fs`; `.claude` from `.agents`)
- [X] T020 [US3] [skillist: fsdocs-technical] Author `docs/governance/speckit-placement.md` mapping each governance touchpoint to a named speckit phase (specify → clarify → plan → tasks → analyze → implement → merge) with usage guidance, and close the governance section with the strengths/weaknesses + pros/cons analysis (FR-008 / SC-003)

**Checkpoint**: User Story 3 — governance deep dive with speckit-phase mapping.

---

## Phase 6: User Story 4 — Designer/consumer learns the typed control + Penpot workflow and its speckit placement (US4, P2)

- [X] T021 [P] [US4] [skillist: fs-skia-typed-controls, fsdocs-technical] Author `docs/controls-design/typed-front-door.md` — authoring against the typed Props/MVU front door and how it lowers to the legacy builders — with a closing analysis (FR-009)
- [X] T022 [P] [US4] [skillist: fs-skia-design-tokens, fsdocs-technical] Author `docs/controls-design/design-tokens-penpot.md` — the design-token flow from design source (Penpot / DTCG) to the typed control surface, how to author it, and its speckit placement — with a closing analysis (FR-009 / FR-010 / SC-004)
- [X] T023 [P] [US4] [skillist: fsdocs-technical] Author `docs/speckit/process.md` explaining the speckit process itself and the specific phase(s) where custom FS Skia UI components are created and consumed (FR-010 / SC-004)
- [X] T024 [US4] [skillist: fsdocs-examples] Author the build-evaluated literate `docs/examples/typed-control-mvu.fsx` exercising the typed control / MVU front door on GPU-free model/props/lowering paths (FR-017 / SC-009)
- [X] T025 [US4] [skillist: fsdocs-examples] Author the build-evaluated literate `docs/examples/design-token-flow.fsx` exercising the design-token flow on GPU-free paths (FR-017 / SC-009)
- [-] T026 [P] [US4] [skillist: fs-skia-evidence-mode] Ensure any embedded visual/screenshot evidence in the docs follows evidence-mode rules (render-only, no fabricated visuals, benign degradation where rendering is unsupported); record the disposition in `readiness/runtime-limitations.md` (FR-015) — mark `[-]` with rationale if no visuals are embedded

**Checkpoint**: User Story 4 — typed-control/Penpot deep dive with evaluated examples.

---

## Phase 7: User Story 5 — Maintainer publishes and keeps the site current (US5, P2)

- [X] T027 [US5] [skillist: fsdocs-build] Run the full local `dotnet fsdocs build --strict`; confirm a complete static site (API reference + authored technical content) with no build errors and every required `.fsx` evaluated; capture to `readiness/logs/fsdocs-build.txt` (FR-013 / SC-005 / SC-009)
- [ ] T028 [US5] [skillist: fsdocs-build] Trigger the Pages workflow (push to `main` or `workflow_dispatch`); confirm the live GitHub Pages site serves the generated API reference and authored docs, and that a content change republishes with no manual file shuffling; capture the run URL to `readiness/logs/pages-deploy.txt` (SC-005 / SC-006)

**Checkpoint**: User Story 5 — site published and reproducible on GitHub Pages.

---

## Phase 8: Integration & Polish

- [X] T029 [P] [skillist: fsdocs-build] Wire and verify the API ↔ architecture cross-links (each API entry links to its subsystem page and architecture pages link back to relevant API entries); confirm the strict build resolves them with no broken-link warning (FR-011 / C7 / US1 acceptance scenario 2)
- [X] T030 [P] [skillist: []] Verify FR-004: run `./fake.sh build -t PackageSurfaceCheck` and `./fake.sh build -t PerPackageSurfaceDiff`; confirm no surface-baseline diff after the `.fsi` doc work; save evidence to `readiness/surface-baseline-unchanged.md` (SC-007)
- [X] T031 [P] [skillist: []] Run `./fake.sh build -t Route` and `./fake.sh build -t Route --enforce` for the actual diff; record the authoritative tier + minimal gate list to `readiness/logs/route.txt` and complete `readiness/validation-contract.md` (required by the `docs-only` focused rule)
- [X] T032 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the feature resolves, no cycles, no dangling refs, and no `[S*]` surprises; record graph before/after
- [X] T033 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS (or document every `--accept-synthetic` override); record the non-authoritative aggregate result under `readiness/`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
