# Feature Specification: Skills Quality Uplift & Per-Phase Feedback Loop

**Feature Branch**: `058-skills-quality-feedback`  
**Created**: 2026-06-03  
**Status**: Draft  
**Input**: User description: "improve the quality of all project skills and bring them to the same level. fsharp skills using a library should have access to the libraries api, code examples and important links to do online research. if persistend problems occur extensive online research in forums/redit... is mandatory. fsharp related skills must have a supporting library where custom code helping skill related tasks go. the dotnet new template should be extended with a feedback parameter. if true after each speckit phase the agent is asked if anything went wrong related to the fs-skia-ui process and what would have helped them. also if they had to write any fsharp code related to skill topics that could be generalized and put in the support library. that feedback should live in a feedback folder of the spec."

## Overview

This repository ships agent-facing **skills** — Markdown capability briefs under
`.agents/skills/` (canonical) generated into `.claude/skills/`, plus the
product skills the `dotnet new fs-skia-ui` template installs into generated
projects. Today these skills vary widely in depth: some (e.g.
`fsharp-graph-algorithms`) carry an API walkthrough, runnable examples, cautions,
external research links, and cross-links; others (e.g. `fs-skia-layout-evidence`)
are prose-only with no library API references, no code examples, and no research
links. This feature raises every FS-Skia-UI-authored skill to one consistent
quality bar, gives the F# skills a shared place for reusable helper code that
ships to consumers, and adds an opt-in feedback loop so agents capture
process friction and generalizable code after each Spec Kit phase.

## Clarifications

### Session 2026-06-03

- Q: Which skills are in scope for the quality uplift? → A: **Every**
  FS-Skia-UI-authored skill, across all homes: (1) repo `fsharp-*` capability
  skills (`.agents/skills/fsharp-*`); (2) repo `fs-skia-*` capability skills
  (`fs-skia-layout-evidence`, `fs-skia-template-update`); (3) the package-owned
  product capability skills under `src/*/skill/SKILL.md` (`src/Scene`,
  `src/SkiaViewer`, `src/Elmish`, `src/KeyboardInput`, `src/Layout`,
  `src/Controls`, `src/Testing`), the template `product-skills/fs-skia-*`
  (`fs-skia-scene`, `fs-skia-skiaviewer`, `fs-skia-elmish`,
  `fs-skia-keyboard-input`, `fs-skia-ui-widgets`, `fs-skia-testing`),
  `template/fragments/samples/skill` (`fs-skia-samples`), and the
  template-shipped `template/base/.agents/skills/fs-skia-project`; plus the new
  `fs-skia-feedback-capture`. The vendored upstream `speckit-*` command skills
  are the **only** out-of-scope skills (they synchronize from upstream Spec Kit
  and MUST NOT be flagged or rewritten).
- Q: What shape and shipping scope for the F# skill support library? → A: A
  supporting F# library organized **per skill family**, where the **compiled
  project plus its `.fsi` API documentation ships with the template to the
  consumer** so generated-project agents consume the same helpers (not
  build-tooling-only).
- Q: Where does the new template `feedback` parameter apply? → A: It is a real
  `dotnet new fs-skia-ui --feedback` parameter; when `true`, generated projects
  prompt the agent after each Spec Kit phase and write captured feedback to the
  feature's `feedback/` folder. This repository may opt in to dogfood it.
- Q: What counts as mandatory external research when a problem persists? → A:
  Extensive external research including **official online documentation**
  (F#/.NET docs, the driven library's own docs/API reference) **first**, plus
  community sources — forums, Reddit, Q&A sites, and issue trackers/changelogs.
  Official online docs are a named, first-class source, not optional.
- Q: Exact wording of the per-phase feedback prompt? → A: Three prompts per phase —
  (1) "During the *{phase}* phase, did anything go wrong or cause friction in the
  fs-skia-ui / Spec Kit process — and what would have helped you?"; (2) "Did you
  write any F# code on a skill topic this phase that could be generalized into the
  support library? If yes, name the skill family/topic and the candidate helper
  (and link any external docs/research used)."; (3) a severity signal: "How
  blocking was the friction — none / minor / major / blocker?".

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every FS-authored skill meets one quality bar (Priority: P1)

An agent resolving any FS-Skia-UI-authored skill finds the same kind of
high-signal content it finds in the best existing skill: a clear scope/when-to-use,
the API surface of any library the skill drives, at least one runnable code
example, external links for online research, cross-links to related skills, and a
sources line. No in-scope skill is a thin prose stub.

**Why this priority**: The core ask is uniform quality. Without a defined bar and
a way to detect skills that fall below it, the rest of the feature has no anchor.

**Independent test**: Run the skill-quality check against the in-scope skill set;
it passes only when every in-scope skill contains each required section. Spot-check
the previously-thin `fs-skia-layout-evidence` and confirm it now carries API
references, a runnable example, and research links.

**Acceptance Scenarios**:

1. **Given** the in-scope skill set, **When** the skill-quality check runs,
   **Then** it reports every skill present and every required section satisfied,
   and fails (naming the skill and the missing section) if any skill regresses.
2. **Given** a skill that drives a library, **When** an agent reads it, **Then**
   the skill names the library's public API entry points, shows at least one
   runnable example exercising them, and lists external research links.
3. **Given** the canonical `.agents` skill tree is edited, **When** surface
   baselines are refreshed, **Then** the generated `.claude` tree stays in sync
   (no `SkillSyncCheck` drift).

---

### User Story 2 - F# skills share a support library that ships to consumers (Priority: P2)

When a skill's guidance depends on reusable F# helper code (parsing, globbing,
graph, code-gen, scene/layout helpers, etc.), that code lives in a dedicated
supporting F# library organized by skill family rather than being re-derived ad
hoc in each feature. The compiled library and its `.fsi` API documentation ship
with the template, so an agent working inside a generated project has the same
helpers and the same documented surface the repo skills reference.

**Why this priority**: A shared support library is what makes "code examples +
library API" in the skills real and reusable instead of copy-paste. Shipping it to
consumers closes the loop so generated-project skills are not dangling references.

**Independent test**: Generate a project from the template; confirm the support
library (compiled output + `.fsi` surface) is present and consumable, and that
the **shipped library-backed `fsharp-*` skills'** API references resolve against
that shipped surface.

**Acceptance Scenarios**:

1. **Given** an F# skill that references reusable helper code, **When** an agent
   reads it, **Then** the skill points to the support library's documented API
   (`.fsi`) and a runnable example using it.
2. **Given** the template is packed and a project generated, **When** the
   generated project is inspected, **Then** the support library's compiled
   project and `.fsi` API documentation are present and the generated skills'
   references resolve.
3. **Given** the support library's public surface, **When** the surface check
   runs, **Then** the `.fsi`-documented API matches the shipped surface baseline.

---

### User Story 3 - Opt-in per-phase feedback capture (Priority: P2)

A project owner generates with `dotnet new fs-skia-ui --feedback true`. From then
on, after each Spec Kit phase (specify, clarify, plan, tasks, analyze, implement)
the agent is prompted to record: (a) whether anything went wrong relating to the
fs-skia-ui process and what would have helped, (b) whether they wrote any F# code
on a skill topic that could be generalized into the support library, and (c) a
severity signal for the friction (none / minor / major / blocker). The answers are
written into the feature's `feedback/` folder. With `--feedback false` (the
default) nothing changes.

**Why this priority**: This is the learning loop the user wants; it depends on the
support library (US2) existing as the destination for generalizable code, and on
the phases existing (always true), but not on US1's full rewrite.

**Independent test**: Generate one project with `--feedback true` and one with the
default; run a Spec Kit phase in each; confirm the first prompts and writes a dated
record under `specs/<feature>/feedback/` while the second is unchanged.

**Acceptance Scenarios**:

1. **Given** `--feedback true`, **When** any Spec Kit phase completes, **Then** the
   agent is prompted with the process-friction question, the generalizable-code
   question, and the severity-signal question, and a record is written under the
   feature's `feedback/` folder.
2. **Given** `--feedback false` (default), **When** any Spec Kit phase completes,
   **Then** no feedback prompt fires and generated output is unchanged from
   today's template output.
3. **Given** a recorded feedback entry that names generalizable F# code, **When**
   a maintainer reviews `feedback/`, **Then** the entry identifies the skill topic
   and the candidate helper so it can be triaged into the support library.

---

### User Story 4 - Persistent problems trigger mandatory external research (Priority: P3)

When work on a skill topic hits a problem that does not resolve after reasonable
in-repo attempts, the skills instruct the agent that extensive external research
is **mandatory** — starting with **official online documentation** (F#/.NET docs
and the driven library's own docs/API reference) and extending to community
sources (forums, Reddit, Q&A sites, issue trackers/changelogs) — and the findings
plus the resolving links are recorded so the skill or support library can be
improved.

**Why this priority**: A quality refinement on top of US1; valuable but not a
blocker for the uplift, library, or feedback loop.

**Independent test**: Read any in-scope F# skill; confirm it states the
persistent-problem research mandate and points at where research findings/links
are recorded.

**Acceptance Scenarios**:

1. **Given** an in-scope F# skill, **When** an agent reads it, **Then** it states
   that persistent problems make extensive external research mandatory and names
   where to record findings and links.
2. **Given** a feedback entry created after a hard problem, **When** reviewed,
   **Then** it carries the research links that resolved (or attempted to resolve)
   the issue.

---

### Edge Cases

- A skill drives **no** library (pure process guidance): the library-API/example
  requirements are satisfied by an explicit "no backing library" declaration
  rather than fabricated examples, so the quality check does not force irrelevant
  content.
- The support library has **no** existing helper for a skill family yet: the skill
  may reference the library's intended home for that family and the feedback loop
  is the channel that populates it; the quality bar must not demand non-existent
  API.
- `--feedback true` but a Spec Kit phase is aborted/fails midway: no partial or
  misleading feedback record is written; the prompt only fires on phase
  completion.
- A generated project is offline when a skill says external research is mandatory:
  the mandate degrades to "record that research was blocked and why" rather than
  hard-failing the phase.
- Vendored `speckit-*` skills are explicitly excluded; the quality check must not
  flag them as failures or rewrite them.

## Requirements *(mandatory)*

### Functional Requirements

**Skill quality bar (US1)**

- **FR-001**: The project MUST define a single, explicit skill-quality bar (a
  rubric of required sections) that applies to **every** FS-Skia-UI-authored
  skill, regardless of home: the repo `fsharp-*`/`fs-skia-*` capability skills
  (`.agents/skills/**`), the package-owned product capability skills
  (`src/*/skill/SKILL.md`), the template-shipped skills
  (`template/product-skills/fs-skia-*`, `template/fragments/**/skill/SKILL.md`,
  `template/base/.agents/skills/**`), and `fs-skia-feedback-capture`. Only the
  vendored upstream `speckit-*` command skills are out of scope.
- **FR-002**: Every in-scope skill MUST contain: a scope / when-to-use section; for
  library-driven skills, the driven library's public API entry points; at least
  one runnable code example (or an explicit "no backing library" declaration where
  none applies); external research links; cross-links to related skills; and a
  sources line.
- **FR-003**: The project MUST provide an automated check that verifies every
  in-scope skill satisfies FR-002 and FAILS naming the specific skill and missing
  section when one does not, so quality cannot silently regress.
- **FR-004**: The quality check MUST exclude the vendored `speckit-*` command
  skills and MUST NOT modify or flag them.
- **FR-005**: Edits MUST be made to the canonical `.agents` skill tree and the
  generated `.claude` tree kept in sync via the existing generation path (no
  hand-syncing; no `SkillSyncCheck` drift).

**Support library (US2)**

- **FR-006**: The project MUST provide a supporting F# library that holds reusable
  helper code for the F# skill topics, organized per skill family.
- **FR-007**: The support library MUST expose a documented public surface via
  `.fsi` API documentation that the F# skills reference for their API and examples.
- **FR-008**: The support library's compiled project AND its `.fsi` API
  documentation MUST ship with the `dotnet new fs-skia-ui` template so generated
  projects consume the same helpers and documented surface the repo skills cite.
- **FR-009**: F# skills that reference reusable helper code MUST point at the
  support library's documented API rather than restating ad-hoc code, and their
  runnable examples MUST exercise that surface.
- **FR-010**: The support library's public surface MUST be governed by the
  existing per-package surface baseline mechanism so its `.fsi` cannot drift from
  the shipped surface.

**Feedback parameter & per-phase capture (US3)**

- **FR-011**: The template MUST accept a boolean `feedback` parameter
  (`dotnet new fs-skia-ui --feedback`), defaulting to `false`.
- **FR-012**: With `feedback=false`, generated output MUST be identical to the
  post-feature template with the feedback parameter disabled — i.e. the
  `feedback` flag itself adds no prompts, hooks, command skill, or files. The
  feature's non-feedback deltas (the shipped `FS.Skia.UI.SkillSupport` package,
  the shipped library-backed `fsharp-*` skills, and the rewritten product
  skills) are present in **both** `--feedback` branches and are not
  feedback-induced changes.
- **FR-013**: With `feedback=true`, after each Spec Kit phase (specify, clarify,
  plan, tasks, analyze, implement) the agent MUST be prompted with three prompts,
  with the phase name substituted into the first: (a) "During the *{phase}* phase,
  did anything go wrong or cause friction in the fs-skia-ui / Spec Kit process —
  and what would have helped you?"; (b) "Did you write any F# code on a skill topic
  this phase that could be generalized into the support library? If yes, name the
  skill family/topic and the candidate helper (and link any external docs/research
  used)."; (c) a severity signal: "How blocking was the friction — none / minor /
  major / blocker?".
- **FR-014**: Feedback answers MUST be written into a `feedback/` folder under the
  active feature directory (`specs/<feature>/feedback/`), one record per phase,
  dated and identifying the phase.
- **FR-015**: A feedback entry that names generalizable F# code MUST capture enough
  to triage it into the support library (the skill topic and the candidate helper),
  MUST record the severity signal (none / minor / major / blocker), and — when
  created after a hard problem — MUST capture the external research links involved.
- **FR-016**: The feedback prompt MUST fire only on phase completion; an aborted or
  failed phase MUST NOT write a partial or misleading record.

**Persistent-problem research mandate (US4)**

- **FR-017**: In-scope F# skills MUST state that when a problem persists after
  reasonable in-repo attempts, extensive external research is mandatory, naming
  **official online documentation first** (F#/.NET docs and the driven library's
  own docs/API reference) then community sources (forums, Reddit, Q&A sites,
  issue trackers/changelogs), and MUST name where findings and resolving links are
  recorded.
- **FR-018**: The research mandate MUST degrade gracefully offline — recording that
  research was blocked and why — rather than hard-failing a phase.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: A **new supporting F# library** is added; its compiled
  project plus `.fsi` API documentation becomes part of the template-consumed
  surface, so package identities and the template's package pins change. No
  existing package is renamed. Controls/chart/graph/DataGrid authoring paths are
  not touched (no legacy Charts migration).
- **Public contract impact**: New `.fsi` public surface for the support library;
  per-package surface baselines change. Skill content references this documented
  surface. No existing public `.fsi` signatures are altered.
- **State workflow impact**: No runtime stateful workflow, command, effect,
  subscription, or interpreter behavior changes. The only new "flow" is the
  authoring-time per-phase feedback prompt wired through the Spec Kit
  extensions/hooks, which does not affect product runtime.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering,
  screenshot, Vulkan, Skia, or visual-output change. Host warning classification
  is unchanged.
- **Evidence obligations**: Skill-quality check evidence (all in-scope skills
  pass); template generation evidence for both `--feedback` values (default output
  unchanged; `true` produces the per-phase prompts + `feedback/` folder); support
  library surface-baseline evidence; readiness notes for the new package and the
  skill edits.
- **Unsupported scope**: Out of scope — rewriting or re-leveling the vendored
  `speckit-*` command skills; any visual/screenshot, release, platform,
  distribution, or roadmap change; changing product runtime behavior; building a
  new CI/online-research automation beyond the documented mandate.
- **Build-target impact**: Expected to touch / add gates for: `Route` (escalates —
  consumer-contract: `template/**`, new `.fsi`, governance), a skill-quality gate
  (new or extended `GeneratedGuidanceCheck`/`SkillSyncCheck`),
  `RefreshSurfaceBaselines`, `TemplateCheck`, `GeneratedProductCheck`,
  `PerPackageSurfaceDiff`/`PackageSurfaceCheck`, `PackLocal`, `EvidenceGraph`, and
  `EvidenceAudit`.

## Success Criteria *(mandatory)*

- **SC-001**: 100% of the in-scope FS-Skia-UI-authored skills satisfy the
  skill-quality bar (every required section present); the automated check passes
  for the full set and fails (naming skill + section) on any regression.
- **SC-002**: The quality variance is eliminated — the previously-thin skills
  (verified via `fs-skia-layout-evidence`) carry the same section set
  (API references, ≥1 runnable example, ≥2 external research links, related links,
  sources) as the richest existing skill.
- **SC-003**: Every in-scope F# skill that drives a library links to that library's
  documented API and includes at least one runnable example exercising it; skills
  with no backing library carry an explicit "no backing library" declaration
  instead.
- **SC-004**: A supporting F# library exists and, after the template is packed and
  a project generated, its compiled project and `.fsi` API documentation are
  present in the generated project and the generated skills' API references
  resolve against it.
- **SC-005**: `dotnet new fs-skia-ui --feedback true` produces a project that, on
  completion of each of the Spec Kit phases, prompts the three feedback prompts
  (process-friction, generalizable-code, severity) and writes a dated,
  phase-identified record under `specs/<feature>/feedback/`.
- **SC-006**: `dotnet new fs-skia-ui` with the default `--feedback false`
  produces output identical to the same post-feature template with feedback
  disabled — the `feedback` flag induces zero diff (no markers, files, or
  whitespace). The feature's package/skill deltas appear in both `--feedback`
  values.
- **SC-007**: 100% of in-scope F# skills state the persistent-problem mandatory
  external-research rule and name where findings/links are recorded; at least one
  worked feedback example demonstrates a generalizable-code candidate routed toward
  the support library.

## Assumptions

- "Bring them to the same level" means a defined section-rubric bar, not identical
  length; skills remain free to differ in depth as long as every required section
  is present (legitimate per-skill variation, not collapse).
- The six Spec Kit phases subject to per-phase feedback are specify, clarify, plan,
  tasks, analyze, and implement (the phases that have lifecycle hooks today). Git
  and evidence sub-hooks are not separate "phases" for feedback purposes.
- The per-phase prompt is delivered through the existing Spec Kit
  extensions/hooks mechanism (`.specify/extensions.yml` `after_*` hooks), the same
  surface the git/evidence hooks already use, rather than a new bespoke runtime.
- "Per skill family" support-library organization means per-family modules/areas
  within the shipped library; the exact number of compiled projects is a planning
  decision, but the compiled output + `.fsi` that ships to consumers is the
  governing contract.
- The support library is build/authoring-helper scoped in purpose but, per the
  user's decision, is packaged and shipped to consumers; it does not introduce new
  product runtime behavior into the generated app.
- External research is recorded as links/notes inside the `feedback/` folder (and,
  for durable lessons, the skill's sources line); the feature does not build an
  automated scraper.

## Dependencies

- Existing single-source skill generation (`.agents` → `.claude` via
  `RefreshSurfaceBaselines`, enforced by `SkillSyncCheck`).
- Existing template packaging and validation path (`TemplateCheck`,
  `GeneratedProductCheck`, template package pins).
- Existing per-package surface baseline mechanism (`PerPackageSurfaceDiff` /
  `PackageSurfaceCheck`) for the new library's `.fsi`.
- Existing Spec Kit extensions/hooks (`.specify/extensions.yml`) for the per-phase
  feedback prompt.
- Existing `Route` selector / governance rules in `FS.Skia.UI.Build`.

## Key Entities

- **Skill**: A capability brief in the `.agents`/`.claude` tree or template
  `product-skills/`; has a scope, optional driven-library API, examples, research
  links, related links, and sources. Subject to the quality bar.
- **Skill-quality bar**: The rubric of required sections an in-scope skill must
  satisfy; enforced by an automated check.
- **Support library**: The shipped F# library of reusable skill-topic helpers,
  organized per skill family, with a `.fsi`-documented public surface.
- **Feedback record**: A dated, phase-identified entry under
  `specs/<feature>/feedback/` capturing process friction, what would have helped,
  generalizable-code candidates, a severity signal (none / minor / major /
  blocker), and (when relevant) external research links.
- **Feedback parameter**: The template boolean `feedback` controlling whether the
  per-phase prompts and `feedback/` capture are active in a generated project.
