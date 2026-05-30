# Tasks: Package API Discovery And Name Safety

**Feature branch**: `035-api-discovery-names`
**Spec**: `specs/035-api-discovery-names/spec.md`
**Plan**: `specs/035-api-discovery-names/plan.md`

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
task generation. Implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

## Vertical-Slice Rule

A task tagged `[US*]` may only be marked `[X]` when the package-consumer or
generated-product authoring surface was actually exercised: generated package
reference output, FSI transcript, clean package-consumer restore/build, guidance
scan, or readiness artifact under `specs/035-api-discovery-names/readiness/`.
Unit tests or parser helpers alone do not satisfy `[X]` for a user-story task.

Principle IV is not applicable to runtime MVU because this feature changes
package reference material, public `.fsi` signatures when needed, generated
guidance, package validation, and governance evidence. File/package I/O remains
at script, test, and FAKE command boundaries.

## Task Annotations

- **[P]** - parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** - user-story scope
- **[T1]** / **[T2]** - Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** - design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml` and mirrors the structured
`skillist` value using `[skillist: ...]`. `tasks.deps.yml` uses indented object
metadata with exactly one key per task id; inline maps, duplicate keys,
dangling dependency ids, and mirror mismatches are invalid.

Task graph validator pitfall guidance: titles that clearly request graph
validation, audit validation, task authoring, implementation command loading,
or constitution work imply specific Spec Kit skills. Setup or readiness tasks
that only cite mandated filenames should use safe wording or the
`Complete readiness notes` prefix. Avoid unrelated trigger phrases such as
`window visibility validation fixture` unless the task actually owns viewer
window evidence.

## Canonical Verification Targets

FAKE-backed commands share repository `.fake` state and must run sequentially
when more than one is needed. Use this deterministic order for broad
validation:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t PackLocal`
3. `./fake.sh build -t PackageSurfaceCheck`
4. `./fake.sh build -t FsiTranscripts`
5. `./fake.sh build -t GeneratedGuidanceCheck`
6. `./fake.sh build -t TemplateCheck`
7. `./fake.sh build -t GeneratedProductCheck`
8. `./fake.sh build -t EvidenceGraph`
9. `./fake.sh build -t EvidenceAudit`

Risk level: broad Tier 1 package governance risk. Focused validation is
required for changed `.fsi` contracts, source-shaped reference generation,
package surface reports, generated guidance scans, FSI transcripts, and clean
package-consumer builds. Broad validation is required after package contents,
template guidance, public signatures, surface baselines, or FAKE target wiring
change. Aggregate FAKE results are non-authoritative until the named readiness
files and focused evidence logs are refreshed.

## Skill Evaluation Notes

High-confidence task matches use capability skills by declared `name:`:
Scene authoring uses `fs-skia-scene`; Controls authoring uses
`fs-skia-ui-widgets`; viewer records use `fs-skia-skiaviewer`; package and
generated-product validation use `fs-skia-template-update`; public validation
helpers use `fs-skia-testing`; evidence validation uses
`speckit-evidence-graph` and `speckit-evidence-audit`. Empty skill lists are
valid where tasks are generic planning, reporting, or command execution with no
material capability-specific implementation guidance.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record Tier 1 package-governance scope, deferred runtime scope, and broad-risk validation rules in the readiness notes
- [X] T002 [P] [skillist: []] Complete readiness notes for required feature evidence files: `api-discovery.md`, `name-collision-safety.md`, `generated-consumer-validation.md`, `feedback-classification.md`, `package-reference-material.md`, `package-surface-baseline.md`, `evidence-graph.md`, and `evidence-audit.md`
- [X] T003 [P] [skillist: []] Record required readiness filenames for package commands, artifact paths, failure classes, and next actions
- [X] T004 [skillist: []] Record public API impact, package impact, MVU non-applicability, synthetic-evidence limits, and unsupported runtime/rendering scope

---

## Phase 2: Foundation

- [X] T005 [P] [skillist: fs-skia-template-update, fs-skia-scene, fs-skia-ui-widgets, fs-skia-skiaviewer, fs-skia-testing] Add failing package-reference coverage tests for curated `.fsi` signatures, source-shaped names, XML summaries, omitted-symbol reasons, and sampled symbol counts
- [X] T006 [P] [skillist: fs-skia-scene, fs-skia-ui-widgets] Add collision inventory tests for Scene and Controls overlapping names, decision records, and explicit qualification guidance
- [S] T007 [P] [SEH] synthetic-error-handling-approved [skillist: fs-skia-template-update] Add malformed generated-guidance scanner fixtures that reject reflection-first or repository-source-copy authoring advice
- [X] T008 [P] [skillist: fs-skia-template-update, fs-skia-testing] Add clean package-consumer validation tests for local package restore, no project references, no copied `src/`, no reflection authoring source, and actionable diagnostics
- [X] T009 [P] [skillist: fs-skia-scene, fs-skia-ui-widgets, fs-skia-skiaviewer] Add FSI transcript tests for Scene primitives, `Paint` helpers, geometry records, viewer records, keyboard cases, and Controls-adjacent declarations
- [X] T010 [skillist: fs-skia-template-update] Define repository-owned reference generation inputs, package output locations, report schema, and FAKE target boundaries without adding runtime dependencies

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 - Discover F# Authoring Shapes From Packages

### Tests First

- [X] T011 [P] [US1] [skillist: fs-skia-template-update, fs-skia-scene, fs-skia-ui-widgets, fs-skia-skiaviewer] Add failing tests that prove packaged reference output preserves F# authoring spellings for Scene, Controls, viewer, geometry, paint, and keyboard samples
- [X] T012 [P] [US1] [skillist: fs-skia-template-update] Add package inclusion or package-adjacent discoverability tests that locate one reference index per packable framework package

### Implementation

- [X] T013 [US1] [skillist: fs-skia-template-update] Implement curated `.fsi` reference extraction and deterministic Markdown/report output with symbol counts, samples, omissions, diagnostics, package id, and version
- [X] T014 [US1] [skillist: fs-skia-template-update] Wire reference generation into local packaging or package-adjacent validation reports and preserve deterministic artifact paths
- [X] T015 [US1] [skillist: fs-skia-scene, fs-skia-ui-widgets, fs-skia-skiaviewer] Add source-shaped construction examples for Scene primitives, `Paint`, viewer records, geometry records, Controls front doors, and Controls.Elmish adapters
- [X] T016 [US1] [skillist: fs-skia-template-update] Capture `readiness/api-discovery.md` and `readiness/package-reference-material.md` with package ids, versions, source `.fsi` paths, reference paths, sampled symbol counts, omitted-symbol reasons, and no-reflection confirmation

**Checkpoint**: US1 validates independently through packaged reference output and package-consumer discovery evidence.

---

## Phase 4: User Story 2 - Avoid Scene And Controls Name Collisions

### Tests First

- [X] T017 [P] [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets] Add failing mixed Scene/Controls compile samples that expose open-order-sensitive names unless explicit qualification or contract changes resolve them
- [X] T018 [P] [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets] Add collision-decision tests requiring every identified overlap to have a public-contract safety decision or explicit consumer guidance

### Implementation

- [X] T019 [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets] Build the collision inventory across public `.fsi` signatures with owner namespaces, symbol kinds, risk levels, observed or plausible failure, and validation scenario
- [X] T020 [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets] Apply selected `.fsi` contract changes only where the inventory justifies qualification attributes or safer front-door APIs
- [X] T021 [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets] Implement corresponding `.fs` bodies for any selected public contract changes and preserve FSI-first semantic test ordering
- [X] T022 [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets, fs-skia-template-update] Update generated examples and guidance to qualify collision-prone Scene records, Controls modules, event origins, and builder helpers explicitly
- [X] T023 [US2] [skillist: fs-skia-scene, fs-skia-ui-widgets] Capture `readiness/name-collision-safety.md` with each collision name, decision, compatibility note, guidance path, surface baseline path, and compile scenario result

**Checkpoint**: US2 validates independently through mixed Scene/Controls package-consumer compilation and collision evidence.

---

## Phase 5: User Story 3 - Classify API Ergonomics Feedback Correctly

### Tests First

- [X] T024 [P] [US3] [skillist: fs-skia-template-update] Add feedback-classification tests for reflection-based discovery, open-order collision, stale generated guidance, and local authoring examples
- [X] T025 [P] [US3] [skillist: fs-skia-template-update] Add generated guidance scan tests that require package-reference location, no-reflection guidance, no repository-source fallback, and mixed Scene/Controls qualification rules

### Implementation

- [X] T026 [US3] [skillist: fs-skia-template-update] Implement feedback classification records with category, owner, contract-change flag, generated-guidance flag, runtime-scope flag, evidence path, and next action
- [X] T027 [US3] [skillist: fs-skia-template-update] Update generated product guidance, template docs, capability metadata, and repository docs to point agents to package reference material before coding
- [X] T028 [US3] [skillist: fs-skia-template-update] Capture a timed classification checklist or transcript proving representative feedback can be categorized with next action in under 5 minutes
- [X] T029 [US3] [skillist: fs-skia-template-update] Capture `readiness/feedback-classification.md` with representative findings classified into package documentation, public contract ergonomics, generated template workflow, and consumer authoring guidance

**Checkpoint**: US3 validates independently through classification records and generated guidance scans.

---

## Phase 6: Integration & Polish

- [X] T030 [skillist: fs-skia-template-update] Refresh package surface baselines only after intentional `.fsi` or package reference changes, then capture `readiness/package-surface-baseline.md`
- [X] T031 [skillist: fs-skia-template-update] Run `./fake.sh build -t Dev` sequentially and record focused failures before broader reruns
- [X] T032 [skillist: fs-skia-template-update] Run `./fake.sh build -t PackLocal` sequentially to produce the local package feed for consumer validation
- [X] T033 [skillist: fs-skia-template-update] Run `./fake.sh build -t PackageSurfaceCheck` sequentially and reconcile package reference or baseline diagnostics
- [X] T034 [skillist: fs-skia-testing] Run `./fake.sh build -t FsiTranscripts` sequentially and confirm public authoring examples are package-shaped
- [X] T035 [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedGuidanceCheck` sequentially and confirm no reflection-first, source-copy, or open-order-dependent guidance remains
- [X] T036 [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` sequentially after generated guidance or template-owned files change
- [X] T037 [skillist: fs-skia-template-update] Run `./fake.sh build -t GeneratedProductCheck` sequentially and confirm the clean package-consumer scenario compiles
- [X] T038 [skillist: speckit-evidence-graph] Run graph validation with `./fake.sh build -t EvidenceGraph`, confirm no cycles, dangling refs, skill metadata mismatches, or unexpected capability omissions, and refresh `readiness/evidence-graph.md`
- [X] T039 [skillist: speckit-evidence-audit] Run audit validation with `./fake.sh build -t EvidenceAudit`, document PASS or every accepted synthetic override, and refresh `readiness/evidence-audit.md`
- [X] T040 [skillist: []] Reconcile all required readiness files, synthetic disclosures, broad-risk notes, and non-authoritative aggregate results before handoff

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T007 | Scanner/error-path validation needs malformed generated-guidance content that should be rejected without creating a real generated product that recommends reflection or source copying. | `specs/035-api-discovery-names/readiness/generated-consumer-validation.md` | N/A | synthetic-error-handling-approved | `specs/035-api-discovery-names/plan.md` synthetic evidence restrictions | malformed generated guidance / forbidden authoring advice fixture | Guidance scanner fails with diagnostics that name reflection-first or repository-source-copy advice and the required package-reference alternative. | accepted-seh |
