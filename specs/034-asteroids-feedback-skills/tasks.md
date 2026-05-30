# Tasks: Asteroids Feedback Skill Guidance

**Feature branch**: `034-asteroids-feedback-skills`
**Spec**: `specs/034-asteroids-feedback-skills/spec.md`
**Plan**: `specs/034-asteroids-feedback-skills/plan.md`

## Status Legend

- `[ ]` - pending
- `[X]` - done with real evidence
- `[S]` - done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` - failed
- `[-]` - skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.
The classification must be assigned during design, planning, clarification, or
task generation. implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

## Vertical-slice Rule

A task tagged `[US*]` may only be marked `[X]` when the changed guidance,
template output, validator behavior, generated-product scan, public `.fsi`
documentation, generated XML documentation, or packed package artifact is
reachable from the user-facing authoring or package-consumption workflow and was
actually exercised through tests, real repository guidance scans,
generated-product guidance scans, build/package inspection, or readiness
artifacts under `specs/034-asteroids-feedback-skills/readiness/`.

Principle IV is not applicable to runtime MVU because this feature changes
Spec Kit guidance, templates, skills, validation scripts, generated-product
guidance, public `.fsi` XML documentation, package documentation validation, and
governance tests only. Script or command I/O remains at the validation edge;
deterministic matching and classification rules should be covered by focused
tests.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** - design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml` and mirrors the structured
`skillist` value using `[skillist: ...]`. `tasks.deps.yml` uses indented object
metadata with exactly one key per task id; inline maps, duplicate keys,
dangling dependency ids, and mirror mismatches are invalid.

Task graph validator pitfall guidance: titles that request graph validation,
audit validation, task authoring, implementation command loading, or
constitution work imply specific Spec Kit skills. Setup or readiness aggregation
tasks that only cite required filenames use the `Complete readiness notes`
prefix or neutral wording. Avoid the title trigger phrase `window visibility
validation fixture` unless the task owns viewer window evidence.

## Canonical Verification Targets

FAKE-backed commands share repository `.fake` state and must run sequentially
when more than one is needed. Use this deterministic order for broad validation:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t PackLocal`
6. `./fake.sh build -t EvidenceGraph`
7. `./fake.sh build -t EvidenceAudit`

Risk level: medium governance risk. Focused validation is required for changed
governance tests, task guidance scans, generated-product guidance scans,
template checks, public `.fsi` documentation scans, generated XML doc checks,
packed package XML doc checks, and direct graph/audit output capture. Broad
validation is required after touching shared templates, generated product
guidance, command surfaces, or packable framework project documentation output.
Aggregate FAKE results are non-authoritative until the named focused readiness
evidence files are refreshed.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record governance scope, medium risk level, deferred runtime package scope, and no-runtime-API-shape impact
- [X] T002 [P] [skillist: []] Complete readiness notes for required feature evidence placeholders and acceptance cues
- [X] T003 [P] [skillist: fs-skia-layout-evidence] Classify Asteroids feedback findings by framework runtime, generated template workflow, documentation discoverability, and consumer authoring owner
- [X] T004 [P] [skillist: []] Inventory packable `src/*/*.fsproj` projects and compiled public `.fsi` files that require XML documentation coverage
- [X] T005 [skillist: []] Record Principle IV non-applicability and focused versus broad validation obligations

---

## Phase 2: Foundation

- [X] T006 [P] [skillist: speckit-tasks] Add failing guidance tests for specialized and multi-skill assignment patterns in generated visual demo task lists
- [X] T007 [P] [skillist: speckit-tasks] Add failing guidance tests for required visual-demo readiness scaffold file enumeration and field cues
- [X] T008 [P] [SEH] synthetic-error-handling-approved [skillist: []] Add malformed readiness field fixtures for missing key/value acceptance cues
- [X] T009 [P] [SEH] synthetic-error-handling-approved [skillist: []] Add visual proof rejection fixtures for metadata-only reports, 1x1 fallback images, and layout-only bounds claims
- [X] T010 [P] [skillist: speckit-tasks] Add generated-product guidance tests for advisory FS.Skia.UI skill discovery without hard-failing valid task lists
- [X] T011 [P] [skillist: fs-skia-layout-evidence] Add feedback-classification coverage for framework, template workflow, documentation, and consumer-authoring owner categories
- [X] T012 [P] [skillist: []] Add failing XML documentation coverage tests for public `.fsi` summaries, parameters, returns, remarks, and examples with no local repo capability skill
- [X] T013 [P] [skillist: []] Add failing generated XML and packed NuGet XML documentation validation tests with no local repo capability skill
- [X] T014 [skillist: []] Define shared rule boundaries for skill assignment, readiness scaffolds, visual proof classes, feedback owner categories, public documentation coverage, XML artifact validation, and scan result reporting

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Assign Implementation Skills Before Work Starts

### Tests First

- [X] T015 [P] [US1] [skillist: speckit-tasks] Add task guidance scan tests for scene rendering, screenshot capture, layout readability, persistent viewer launch, deterministic evidence mode, generated-package validation, graph validation, audit validation, and debug-loop skills
- [X] T016 [P] [US1] [skillist: speckit-tasks] Add multi-skill ordering tests for implementation-before-evidence, graph-before-audit, and debug-before-broad-rerun guidance

### Implementation

- [X] T017 [US1] [skillist: speckit-tasks] Update repository and preset task guidance with visual demo skill assignment patterns and visible mirror requirements
- [X] T018 [US1] [skillist: speckit-tasks, fs-skia-template-update] Update generated product task guidance copies with the same skill assignment and no-skill rationale patterns
- [X] T019 [US1] [skillist: speckit-tasks] Capture `skill-assignment-guidance.md` with resolved skill ids, resolved `SKILL.md` paths, matched signals, confidence, ambiguity, and reviewer disposition

**Checkpoint**: US1 validates independently through skill assignment guidance scans.

---

## Phase 4: User Story 2 - Scaffold Hidden Evidence Contracts

### Tests First

- [X] T020 [P] [US2] [skillist: speckit-tasks] Add guidance tests that generated visual demo tasks enumerate visual, window, governance, aggregate hang, runtime limitation, generated validation, and real-image readiness files
- [X] T021 [P] [US2] [skillist: speckit-evidence-audit] Add audit contract coverage for expected readiness terms and fields without making final audit failures the discovery mechanism

### Implementation

- [X] T022 [US2] [skillist: speckit-tasks] Update task templates and task-generation guidance to scaffold audit-required readiness files early in the author workflow
- [X] T023 [US2] [skillist: speckit-tasks, fs-skia-template-update] Update generated product guidance to list authoritative commands, artifact paths, failure classes, and next-action fields for each readiness scaffold
- [X] T024 [US2] [skillist: speckit-tasks] Capture `readiness-scaffold-coverage.md` with all required readiness paths and field cues discoverable from guidance

**Checkpoint**: US2 validates independently through readiness scaffold coverage evidence.

---

## Phase 5: User Story 3 - Preserve Evidence Honesty For Visual Proofs

### Tests First

- [X] T025 [P] [US3] [SEH] synthetic-error-handling-approved [skillist: fs-skia-layout-evidence] Add negative visual evidence fixtures for ASCII screenshot reports, fallback PNG substitution, and layout-bounds-only claims
- [X] T026 [P] [US3] [skillist: fs-skia-layout-evidence] Add real guidance scan coverage for decodable image, dimensions, non-trivial content, renderer mode, fallback use, and unsupported reason fields

### Implementation

- [X] T027 [US3] [skillist: fs-skia-layout-evidence] Update visual evidence guidance to separate screenshot proof, rasterized scene proof, layout readability proof, fallback classification, and unsupported proof
- [X] T028 [US3] [skillist: fs-skia-layout-evidence, fs-skia-template-update] Update generated product evidence guidance so metadata-only reports and layout-only checks cannot satisfy visual proof tasks
- [X] T029 [US3] [skillist: fs-skia-layout-evidence] Capture `visual-evidence-honesty.md` with accepted and rejected proof examples tied to real guidance paths

**Checkpoint**: US3 validates independently through visual evidence honesty scans.

---

## Phase 6: User Story 4 - Classify Framework, Template, And Consumer Findings

### Tests First

- [X] T030 [P] [US4] [skillist: fs-skia-layout-evidence] Add classification tests for at least four framework findings and at least three non-framework findings from the Asteroids feedback report
- [X] T031 [P] [US4] [skillist: fs-skia-layout-evidence] Add guidance tests for persistent-window blocking, display/session availability, auto-close smoke needs, and host-warning classification

### Implementation

- [X] T032 [US4] [skillist: fs-skia-layout-evidence] Add or update feedback classification guidance with owner categories, source observation, deferred scope, and bounded next action
- [X] T033 [US4] [skillist: fs-skia-layout-evidence] Capture `feedback-classification.md` with the Asteroids findings mapped to framework runtime, generated template workflow, documentation discoverability, and consumer authoring follow-ups

**Checkpoint**: US4 validates independently through feedback classification evidence.

---

## Phase 7: User Story 5 - Improve Skill Discovery And API Documentation For API And Host Friction

### Tests First

- [X] T034 [P] [US5] [skillist: speckit-tasks] Add guidance tests for API surface discovery, host/window behavior, warning classification, and name-collision guidance in task metadata
- [X] T035 [P] [US5] [skillist: fs-skia-layout-evidence] Add coverage that benign, blocking, and deferred compile or host warnings remain explicitly classified
- [X] T036 [P] [US5] [skillist: []] Add public `.fsi` documentation validation tests for every packable framework package with no local repo capability skill
- [X] T037 [P] [US5] [skillist: []] Add package artifact validation tests that generated XML docs are non-empty and included in each corresponding `.nupkg` with no local repo capability skill

### Implementation

- [X] T038 [US5] [skillist: speckit-tasks] Update task guidance to point API discovery and host-friction work toward available local skills or documented no-skill rationales
- [X] T039 [US5] [skillist: fs-skia-layout-evidence] Update host-warning and name-collision guidance with framework, documentation, or consumer-authoring ownership outcomes
- [X] T040 [US5] [skillist: []] Add comprehensive XML documentation comments to public `.fsi` surfaces in packable framework packages with no runtime API shape changes
- [X] T041 [US5] [skillist: []] Add or update hard documentation validation for missing `.fsi` XML comments, empty generated XML files, and missing XML entries in packed NuGet artifacts
- [X] T042 [US5] [skillist: speckit-tasks] Capture `generated-guidance-validation.md` with scan command, scanned files, observed and missing terms, advisory-only status, failure classification, and next action
- [X] T043 [US5] [skillist: []] Capture `xml-documentation-validation.md` with project paths, `.fsi` paths, generated XML paths, packed artifact entries, failures, and next actions

**Checkpoint**: US5 validates independently through generated guidance and XML documentation evidence.

---

## Phase 8: Integration & Polish

- [X] T044 [skillist: speckit-tasks, fs-skia-layout-evidence] Run focused governance tests for skill assignment, readiness scaffolding, visual proof honesty, feedback classification, and advisory guidance behavior
- [X] T045 [skillist: []] Run focused XML documentation and package artifact validation tests for public `.fsi`, generated XML, and packed `.nupkg` coverage
- [X] T046 [skillist: speckit-evidence-graph] Run direct graph validation for `specs/034-asteroids-feedback-skills` and refresh graph readiness artifacts
- [X] T047 [skillist: []] Run `./fake.sh build -t Dev` sequentially and record any non-authoritative aggregate result
- [X] T048 [skillist: speckit-tasks] Run `./fake.sh build -t GeneratedGuidanceCheck` sequentially after guidance edits
- [X] T049 [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` sequentially after template-owned edits
- [X] T050 [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck` sequentially after generated product guidance edits
- [X] T051 [skillist: []] Run `./fake.sh build -t PackLocal` sequentially after XML documentation edits and inspect packed XML entries
- [X] T052 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` sequentially and confirm graph output is current
- [X] T053 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` sequentially and document PASS or every accepted synthetic override
- [X] T054 [skillist: []] Reconcile required readiness files, risk-level notes, synthetic evidence disclosures, XML documentation evidence, package artifact evidence, and follow-up owner categories before handoff

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T008 | Missing readiness key/value fields are malformed guidance inputs that need isolated validator coverage before real guidance scans exist. | `specs/034-asteroids-feedback-skills/readiness/readiness-scaffold-coverage.md` | N/A | synthetic-error-handling-approved | `specs/034-asteroids-feedback-skills/contracts/readiness-scaffold-coverage.md` | malformed or incomplete readiness scaffold fields | Validator or guidance scan rejects missing required terms, key/value fields, authoritative command, failure class, and next action. | accepted-seh-complete |
| T009 | Metadata-only, fallback, and layout-only proof claims are explicit negative visual-proof classes and cannot be represented as successful real screenshots. | `specs/034-asteroids-feedback-skills/readiness/visual-evidence-honesty.md` | N/A | synthetic-error-handling-approved | `specs/034-asteroids-feedback-skills/contracts/visual-evidence-honesty.md` | invalid visual proof report or placeholder image claim | Guidance rejects the proof claim unless a decodable image has expected dimensions and non-trivial content. | accepted-seh-complete |
| T025 | Rejection examples require controlled invalid screenshot and fallback inputs while the real guidance scan proves the accepted paths. | `specs/034-asteroids-feedback-skills/readiness/visual-evidence-honesty.md` | N/A | synthetic-error-handling-approved | `specs/034-asteroids-feedback-skills/contracts/visual-evidence-honesty.md` | ASCII screenshot report, 1x1 fallback PNG, or layout-bounds-only report | Guidance classifies the claim as unsupported or incomplete visual proof and names the next action. | accepted-seh-complete |
