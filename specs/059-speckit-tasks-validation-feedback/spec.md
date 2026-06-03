# Feature Specification: Trustworthy `/speckit.tasks` Validation Experience

**Feature Branch**: `059-speckit-tasks-validation-feedback`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "@docs/reports/2026-06-03-2128-speckit-tasks-governance-process-analysis.md"

## Overview

A field report from authoring task artifacts in a generated consumer project
(`AsteroidsDemo3`, feature `001-asteroids-demo`) found that **authoring**
`tasks.md` + `tasks.deps.yml` was easy but **validating** them was untrustworthy.
Eight problems were catalogued; three can produce a *wrong result silently* or
*cost a guaranteed wasted iteration*. The most dangerous is that the documented
happy-path validation command returns a **clean pass against a bundled sample
feature** instead of the author's real feature — a false green.

This feature makes the task-validation experience honest and self-consistent for
the people who follow the bundled guidance: the validation guidance must match
the real validator, the validator must validate the author's actual feature (or
fail loudly), the documented artifact schemas must be the ones the engine
accepts, and the skill-assignment hints must resolve to real registered skills.

The defects live in this repository's **consumer-facing sources** — the bundled
skills (`.agents/skills/**` and their generated `.claude/**` peers), the Spec Kit
presets/templates under `.specify/**`, the project template under `template/**`,
and the template's `build.fsx` — which are what generated consumer projects
inherit.

## Clarifications

### Session 2026-06-03

- Q: How should the template's `build.fsx` resolve which feature to validate (the fix for the false-green default)? → A: Resolve from `.specify/feature.json` with NO sample fallback — matching the framework's own engine (`build/Governance/Engine/Model.fs`); an env var may override; fail loud if unresolved.
- Q: How should a task declare it "owns" gated (graph/audit) evidence so titles can be free-form (FR-010)? → A: Add an explicit per-task `owns:` field in `tasks.deps.yml` as the signal, and **remove** title-trigger matching entirely (no backstop).
- Q: How should the overloaded `fs-skia-layout-evidence` catch-all skill be addressed (FR-012)? → A: Split it into separate registered skills (evidence-mode vs HUD/layout-readability), accepting the new-skill governance + sync work.
- Q: What should happen to the bundled sample feature `generated-evidence-workflow` once resolution no longer defaults to it? → A: Remove it from the template entirely to eliminate the foot-gun.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Validation never silently passes the wrong feature (Priority: P1)

An author (human or agent) finishes `tasks.md` + `tasks.deps.yml` for their
feature, follows the bundled guidance to validate the task graph, and gets a
result that is **about their feature** — or an unambiguous, loud failure telling
them what to specify. They never receive a green verdict computed against a
bundled sample feature.

**Why this priority**: This is the only catalogued issue that yields a confident
*false success*. An author who sees `exit 0 / verdict=ok` reasonably believes
their 33 tasks passed when in fact zero of them were checked.

**Independent Test**: From a feature working context with one authored feature,
run the documented validation step with no extra configuration. Confirm the
reported feature directory and task count correspond to the author's feature, not
a sample, and that the verdict reflects the author's artifacts. Then place the
context in an ambiguous state (no resolvable feature) and confirm the step fails
loudly with an actionable message instead of falling back to a sample.

**Acceptance Scenarios**:

1. **Given** an authored feature recorded in `.specify/feature.json` with valid
   task artifacts, **When** the author runs the documented validation step without
   setting any feature-pointing variable, **Then** validation runs against that
   feature (its directory and task count are echoed) and reports a verdict
   computed from the author's artifacts.
2. **Given** no feature can be resolved (no `.specify/feature.json` and no
   explicit override), **When** the author runs the validation step, **Then** it
   exits non-zero with a message naming what is missing and how to resolve it —
   and never silently validates a bundled sample (the sample no longer ships).
3. **Given** the author explicitly overrides the target feature directory,
   **When** they run the step, **Then** that feature is validated and the choice
   is echoed back.

---

### User Story 2 - The documented validation command works as written (Priority: P2)

An author reads the bundled task skill, copies the validation command verbatim,
and it runs successfully. They are not sent to a script or directory that does
not exist, and two bundled skills do not contradict each other about how to
validate.

**Why this priority**: A stale/non-existent command and a direct contradiction
between two active skills each guarantee a wasted iteration and erode trust in
all the surrounding guidance.

**Independent Test**: Follow only the bundled task skill's validation
instructions in a generated consumer project and confirm the command exists and
runs. Cross-read the task skill and the evidence-graph skill and confirm they
describe the same validation entry point.

**Acceptance Scenarios**:

1. **Given** the bundled task skill's "Validation" section, **When** an author
   runs the command exactly as written, **Then** it resolves to a real,
   working entry point (no "file not found").
2. **Given** both the task skill and the evidence-graph skill are active,
   **When** an author reads each one's validation instructions, **Then** they
   agree on the validation entry point (no skill claims a runner exists that
   another says does not).

---

### User Story 3 - `tasks.deps.yml` validates on the first correct authoring attempt (Priority: P2)

An author writes `tasks.deps.yml` by following the bundled template and skill
guidance exactly, and it passes the schema gate on the first try because the one
structural fact that actually gates validation is documented and exemplified.

**Why this priority**: Following the current guidance exactly still fails,
because the required top-level wrapper and version key are documented only in a
bundled sample, not in the template or skill. Guaranteed wasted iteration for
every hand-author.

**Independent Test**: Author a `tasks.deps.yml` strictly from the template and
skill text (without copying the sample), then validate. It should pass the schema
gate without a structural-shape failure.

**Acceptance Scenarios**:

1. **Given** the deps-file template and skill guidance, **When** an author
   follows them to produce a deps file, **Then** the produced file contains the
   structural shape the engine requires and passes the schema gate.
2. **Given** an author submits a deps file missing the required structural
   wrapper, **When** validation runs, **Then** the error directs them to the
   exact missing structure rather than only reporting downstream per-task
   mismatches.

---

### User Story 4 - Skill hints resolve to real registered skills (Priority: P3)

An author assigning a `skillist` follows the bundled hint tables and the ids they
are told to use resolve to exactly one registered skill — the hints never name a
skill that is not registered, nor a directory name that differs from the
registered identifier.

**Why this priority**: The validator hard-fails on unresolved skill ids, so a
hint that names a nonexistent or mis-cased skill actively steers an author into a
blocking error and gives false confidence that "documented" ids are valid.

**Independent Test**: For every skill id referenced in the bundled hint tables,
confirm it resolves to exactly one registered skill's canonical identifier.
Assign skills by following the hints and confirm validation does not raise an
unresolved-skill error attributable to a hint.

**Acceptance Scenarios**:

1. **Given** the bundled skill-assignment hints, **When** each referenced id is
   checked against the live skill registry, **Then** every id resolves to exactly
   one registered skill's canonical identifier.
2. **Given** an author assigns skills strictly from the hints, **When** they
   validate, **Then** no unresolved-skill failure is caused by following a hint.

---

### User Story 5 - Skill-assignment and title-trigger guidance is honest and low-friction (Priority: P3)

An author reading the validation guidance gets an accurate description of what
the skill assessment actually does and is not forced to fight a hidden English↔︎
matcher contract when writing natural task titles.

**Why this priority**: The docs frame skill assignment as a confidence review,
but in practice it accepts whatever is declared except where a task title
literally contains a trigger phrase. Authors over-trust a green assessment, and
the title-trigger coupling makes natural titles risky to write. This is friction
and brittleness rather than a false pass, hence lower priority.

**Independent Test**: Read the revised guidance and confirm it states plainly
what the assessment checks and what it trusts. Author tasks with natural,
free-form titles and confirm the structured ownership signal — not the title
prose — determines which task owns evidence.

**Acceptance Scenarios**:

1. **Given** the revised skill-assessment guidance, **When** an author reads it,
   **Then** it accurately describes that most assignments are trusted-as-declared
   and what the high-confidence cases actually key off, without overstating the
   review.
2. **Given** the evidence-ownership mechanism, **When** an author writes a
   free-form, natural-language task title, **Then** whether a task "owns"
   graph/audit evidence is determined solely by the explicit `owns:` field in
   `tasks.deps.yml` (the title-trigger matcher having been removed).

---

### Edge Cases

- `.specify/feature.json` missing or unresolvable: validation must refuse and
  explain, not guess or fall back to a sample.
- No feature artifacts present at all: validation must report that clearly rather
  than silently validating a bundled sample (the sample no longer ships).
- An author needs a starting-point example: the canonical correct shape must live
  in the guidance/template itself (FR-006), since the bundled sample feature is
  removed (FR-014).
- A future author paraphrases a task title: evidence ownership is unaffected,
  because ownership is keyed only on the `owns:` field, not title prose.
- Existing task files relied on title-trigger matching: migration guidance must
  let them re-express ownership via the `owns:` field (FR-010).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The bundled task-validation guidance MUST document a validation
  entry point that exists and runs as written in a generated consumer project;
  references to a non-existent shell runner or script directory MUST be removed.
- **FR-002**: The template's `build.fsx` MUST resolve the feature to validate
  from `.specify/feature.json` (the same source the framework's own engine
  `build/Governance/Engine/Model.fs` already uses), with **no** fallback to any
  bundled sample feature. An explicit selection via the `SPECKIT_FEATURE_DIR`
  environment variable MAY override this resolution; the sample-era
  `GENERATED_EVIDENCE_FEATURE_DIR` variable is removed together with the sample
  (FR-014). (`Model.fs` itself has no override — it resolves purely from
  `.specify/feature.json`; the override is a template-`build.fsx` affordance.)
- **FR-003**: When the target feature cannot be resolved (no `.specify/feature.json`
  and no explicit override), validation MUST fail loudly (non-zero, explicit,
  actionable message naming what is missing and how to resolve it) instead of
  falling back to any sample.
- **FR-004**: Validation MUST echo the resolved feature directory and task count
  so the author can confirm what was validated; the documented success criteria
  MUST require the author to verify these match their feature.
- **FR-005**: The override mechanism for explicitly selecting the feature to
  validate (the `SPECKIT_FEATURE_DIR` environment variable) MUST be documented in
  the bundled guidance where the validation command appears, alongside the default
  `.specify/feature.json` resolution.
- **FR-014**: The bundled sample feature `generated-evidence-workflow` MUST be
  removed from the template so it can never be the silent default target of a
  validation run. Any canonical correct-shape example previously sourced from it
  (deps-file shape, etc.) MUST be preserved as documentation/examples within the
  guidance instead (see FR-006).
- **FR-006**: The `tasks.deps.yml` template and skill guidance MUST document and
  exemplify the exact structural shape the engine requires (including the
  top-level wrapper and version key, plus the per-task `owns:` field from
  FR-010), with a complete minimal example embedded in the guidance itself, so an
  author following the guidance alone — with the sample feature now removed
  (FR-014) — produces a file that passes the schema gate.
- **FR-007**: When a deps file omits the required top-level structural wrapper,
  the validation error MUST direct the author to that specific missing structure
  rather than only emitting downstream per-task "no key" mismatches.
- **FR-008**: Every skill id referenced by the bundled skill-assignment hint
  tables MUST resolve to exactly one registered skill's canonical identifier; a
  hint MUST NOT reference an unregistered skill or a directory name that differs
  from the registered identifier.
- **FR-009**: The bundled validation guidance MUST describe the skill-assessment
  behavior honestly — that most declared assignments are trusted as-declared and
  what the high-confidence cases key off — without framing it as a per-task
  capability review it does not perform.
- **FR-010**: Whether a task "owns" graph/audit (or other gated) evidence MUST be
  declared through an explicit per-task `owns:` field in `tasks.deps.yml`. The
  existing title-trigger matcher MUST be **removed** entirely so that evidence
  ownership is determined solely by the structured `owns:` field and task titles
  are fully free-form. Migration guidance MUST be provided for any existing task
  files that relied on title-trigger behavior.
- **FR-011**: The bundled task skill and the evidence-graph skill MUST give
  non-contradictory validation instructions; the task skill MUST defer to the
  evidence-graph skill for the canonical validation command rather than restating
  a divergent one.
- **FR-012**: The overloaded `fs-skia-layout-evidence` catch-all MUST be split
  into separate registered skills along its distinct concerns (at minimum,
  deterministic-evidence-mode guidance vs HUD/layout-readability guidance), so
  each skill assignment carries a precise signal. The split MUST keep the
  canonical-vs-generated skill tree in sync and update every hint table and
  reference accordingly.
- **FR-013**: All fixes MUST be applied to the canonical source artifacts such
  that their generated peers stay in sync (canonical `.agents` skills regenerate
  their `.claude` peers; presets/templates and the template's `build.fsx`
  propagate to generated consumer projects), so a freshly generated consumer
  project inherits the corrected guidance and behavior.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, or version changes are
  required by the feature itself. (Routine maintainer-merge version bumps of
  packable projects are out of this feature's scope and governed by the merge
  process.) No controls/chart/graph/DataGrid authoring changes; no Charts
  migration guidance.
- **Public contract impact**: No application library `.fsi`/public API changes
  expected, but the **consumer-facing authoring contract** changes materially:
  the `tasks.deps.yml` schema gains a per-task `owns:` field (FR-010), bundled
  skills are split/renamed (FR-012), and the bundled sample feature is removed
  (FR-014). Surface baselines / contract tokens / skill-registry baselines will
  likely need regeneration; the deps-file schema doc is now a versioned contract.
- **State workflow impact**: No application runtime/I/O/command/effect/
  subscription changes. However, the **validation engine** changes behavior:
  removing the title-trigger matcher and reading the new `owns:` field (FR-010)
  is a change to the compiled evidence/agent-validation logic, and the template's
  `build.fsx` feature-directory resolution changes (FR-002). These affect the
  governance tool, not application/runtime state.
- **Layout/rendering impact**: No layout, charts, DataGrid, rendering,
  screenshot, Vulkan, Skia, or visual-output changes. The `fs-skia-layout-evidence`
  skill is touched only as documentation/guidance, not its layout-evidence
  semantics.
- **Evidence obligations**: Real evidence is the corrected, self-consistent
  guidance plus a demonstration that a generated consumer project validates its
  own feature (correct feature dir + task count echoed; deps file authored from
  template alone passes; hint ids resolve). Required readiness/evidence artifacts
  named by `./fake.sh build -t Route --enforce` for the escalated paths MUST be
  produced. Skill currency must remain green (`SkillSyncCheck`,
  `TargetMetadataDrift`).
- **Unsupported scope**: No new validator engine capabilities beyond
  feature-directory resolution and error-message directiveness; no redesign of the
  synthetic-propagation or audit semantics; no visual/release/platform/
  distribution/roadmap deliverables. Rewriting the skill-assessment into a real
  per-task capability extractor is explicitly out of scope (FR-009 only requires
  honest documentation).
- **Build-target impact**: Changes to `template/**`, `.specify/**`, and bundled
  skills are consumer-contract paths that **escalate** under `Route`. Likely gates:
  `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`; skill regeneration via
  `RefreshSurfaceBaselines` with currency enforced by `SkillSyncCheck` /
  `TargetMetadataDrift`. The authoritative gate list MUST be taken from
  `./fake.sh build -t Route` for the actual diff.

## Success Criteria *(mandatory)*

- **SC-001**: In a freshly generated consumer project with exactly one authored
  feature, an author following only the bundled guidance validates the task graph
  and the reported feature directory and task count match their feature on the
  **first** attempt (0 false-green sample runs).
- **SC-002**: When the target feature is ambiguous or absent, validation fails
  with a non-zero result and an actionable message 100% of the time, and never
  reports a verdict computed against a bundled sample.
- **SC-003**: An author authoring `tasks.deps.yml` strictly from the template and
  skill text (not copied from the sample) passes the schema gate on the first
  attempt — eliminating the previously guaranteed wasted iteration.
- **SC-004**: 100% of skill ids referenced in the bundled hint tables resolve to
  exactly one registered skill; following the hints produces 0 unresolved-skill
  validation failures.
- **SC-005**: The validation command copied verbatim from the bundled task skill
  runs successfully (0 "file not found"), and the task skill and evidence-graph
  skill describe the same validation entry point (0 contradictions).
- **SC-006**: An author can mark a task as owning gated evidence purely via the
  `owns:` field, write natural free-form titles for all tasks, and have evidence
  ownership never flip based on title wording (title-trigger matching is gone).
- **SC-007**: All canonical-vs-generated currency gates remain green after the
  change (no skill-sync or target-metadata drift), and a regenerated consumer
  project inherits every corrected behavior above.

## Assumptions

- The validator engine remains the in-process compiled F# evidence engine invoked
  via the FAKE `EvidenceGraph` target (and `EvidenceAudit` for the merge gate);
  this feature corrects how it is *documented and pointed at a feature*, not its
  core graph/synthetic algorithms.
- Feature resolution uses `.specify/feature.json` as the single source of truth
  (matching the framework's own `Model.fs`), with an explicit override allowed and
  a loud failure when neither resolves — no best-effort guessing and no sample
  fallback.
- The bundled sample feature (`generated-evidence-workflow`) is removed from the
  template entirely; the canonical correct-shape example it provided is relocated
  into the deps-file template/guidance (FR-006) so authors retain a copyable
  reference.
- Fixes are made to canonical sources so generated `.claude` peers and generated
  consumer projects inherit them; hand-editing generated peers is not the
  mechanism.
- Maintainer-merge version bumps of packable projects, if any, are handled by the
  standard merge process and are not a deliverable of this feature.
