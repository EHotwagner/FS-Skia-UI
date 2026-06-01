# Feature Specification: Single-Source Generation of Duplicated Governance Artifacts

**Feature Branch**: `044-foundations-single-source-generation`  
**Created**: 2026-06-01  
**Status**: Draft  
**Input**: User description: "implement the next part of the plan — Stage 2.2–2.5 of `docs/reports/2026-05-31-1049-foundations-implementation-plan.md` (single-source generation), continuing the foundations programme after Stages 0/1/2.1/3/4 (features 039–043)."

## Context & Motivation *(informative)*

The foundations programme has a standing principle the prior reports stated plainly:
**generation beats drift-checking, everywhere.** Wherever the build today maintains *two*
copies of the same information and *checks that they match*, the second copy is a confession
that there are two sources of truth. A drift check can only detect divergence after the fact; a
single source that *generates* the second copy makes divergence structurally impossible.

Three duplication classes still violate this principle and are the scope of this feature:

1. **`.claude/skills` ↔ `.agents/skills`** — 25 byte-identical `SKILL.md` pairs maintained by
   hand. The existing `SkillSyncCheck` (feature 040) only *checks* identity, and only for **6**
   of the 25 capability slugs — the other 19 pairs are unguarded duplication today.
2. **The skillist** — each task's skill list lives in both `tasks.md` (the human-readable
   `[skillist: …]` annotation) and `tasks.deps.yml` (the machine-readable `skillist:` list); the
   evidence audit *drift-checks* that they agree.
3. **The constitution echo** — `.specify/memory/constitution.md` principles are paraphrased into
   `plan-template.md`, `tasks-template.md`, and every generated plan, with no enforcement that the
   restatements stay consistent with the source.

The repository already has two **working models** of the target pattern to follow:
`validation.contract.yml` is generated from `Routing.fs` (feature 042) and target metadata is
derived from the typed `Targets.fs` (feature 041) — in both cases a hand-edit fails with a
"regenerate from source" diagnostic. This feature extends that same single-source discipline to
the three remaining duplication classes.

This is framework-internal tooling and governance work. It touches `.specify/**`, the skill
trees, and governance paths, so it **escalates** under `Routing.fs` to the full validation gate
set; it is not a runtime change.

## Clarifications

### Session 2026-06-01

- Q: How should the generator inject constitution-derived content into templates while preserving genuine hand-written guidance? → A: Marker-delimited regions — generated principle fragments are spliced between explicit `BEGIN/END GENERATED` markers inside existing templates; everything outside the markers is preserved hand-written prose.
- Q: When the reframed skillist currency check runs in EvidenceAudit, what scope does it re-derive and verify? → A: Active feature only — re-derive/check only the current feature's skillist at gate time; the ~43 historical feature directories are not re-derived.
- Q: This feature converts SkillSyncCheck to a generation-currency check — what happens to SkillExamplesCheck? → A: Retire SkillExamplesCheck as redundant once generation guarantees byte-identity between the trees.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Skill trees become one source, generated, fully covered (Priority: P1)

A maintainer (or AI agent) edits a capability skill. Today they must remember to copy the change
byte-for-byte into the mirror tree, and if the skill is not one of the 6 covered slugs, nothing
catches a mistake. After this feature, they edit **one** canonical skill tree, run a generation
target, and the derived tree is reproduced exactly — for **all 25** skills, not just 6. Forgetting
to regenerate fails a currency gate with an actionable "run the generator" diagnostic, not a bare
"these two files differ."

**Why this priority**: It closes the largest and only *partially-guarded* duplication class (19
of 25 pairs are unchecked today), eliminating a live silent-drift risk, and it removes the most
duplicated lines.

**Independent Test**: Edit the canonical tree's copy of any skill (including one of the 19
currently-unguarded slugs) without regenerating; the currency gate fails naming the generator.
Run the generator; the derived tree becomes bit-identical to the canonical source and the gate
passes. Editing the *derived* tree directly is reported as drift to be regenerated away.

**Acceptance Scenarios**:

1. **Given** a canonical skill is edited and the generator has not been run, **When** the currency
   gate runs, **Then** it fails and names the generation target to run.
2. **Given** the generator is run after a canonical edit, **When** the currency gate runs, **Then**
   the derived tree is bit-identical to the canonical tree across all 25 skills and the gate passes.
3. **Given** a new skill directory is added to the canonical tree, **When** the generator runs,
   **Then** the derived tree gains the corresponding skill with no per-skill allowlist edit
   required (coverage is by enumeration of the canonical tree, not a hardcoded slug list).
4. **Given** the derived tree is hand-edited out of band, **When** the currency gate runs, **Then**
   it reports the derived tree as stale and regeneration restores identity.

---

### User Story 2 - The skillist has one canonical home (Priority: P2)

When authoring `tasks.md` and `tasks.deps.yml` for a feature, a maintainer or agent currently
writes each task's skill list twice and the evidence audit fails if the two disagree. After this
feature the skillist is authored in exactly **one** canonical location and the other representation
is derived from it, so the two can no longer disagree — the drift-check becomes a
generation-currency check.

**Why this priority**: It removes a recurring per-feature authoring tax and a frequent
audit-failure mode, on the most-edited governance files. Lower than US1 only because the duplicated
volume is smaller and the existing drift-check already prevents incorrect merges (it is annoying,
not unguarded).

**Independent Test**: For a sample feature, change the skillist in the canonical source only; run
the generator/renderer; the derived representation updates to match and the evidence audit passes.
Change the derived representation alone (without the canonical source); the currency gate flags it
as stale rather than silently accepting a second source of truth.

**Acceptance Scenarios**:

1. **Given** a task's skillist is edited in the canonical source, **When** the renderer runs,
   **Then** the derived representation matches and `EvidenceGraph`/`EvidenceAudit` pass.
2. **Given** the derived representation is edited but the canonical source is not, **When** the
   currency gate runs, **Then** it reports the derived representation as stale and names the action
   to regenerate.
3. **Given** the canonical and derived representations agree, **When** the evidence audit runs,
   **Then** the skillist comparison reports currency (not merely "matched") with no false failures
   on historical features.

---

### User Story 3 - The constitution is stated once (Priority: P3)

When a principle in `.specify/memory/constitution.md` changes, a maintainer should not have to hunt
through templates and generated plans for paraphrases that now contradict the source. After this
feature, constitution content appears in exactly one place; templates carry **generated fragments
spliced into `BEGIN/END GENERATED` marker regions** from that single source rather than restating
it by hand, and a check fails if a region's content drifts from the source.

**Why this priority**: Real maintenance value but the smallest duplicated volume of the three and
the least mechanical (some template prose is genuine guidance, not a verbatim echo), so it is
sequenced last.

**Independent Test**: Edit a principle in the constitution; without regenerating, the currency gate
flags the affected template/fragment as stale; after regeneration the templates reflect the change
and the gate passes. Genuine guidance prose that is *not* a constitution echo is left untouched by
the generator.

**Acceptance Scenarios**:

1. **Given** a constitution principle is changed, **When** the currency gate runs without
   regeneration, **Then** it flags the derived fragment/template as stale.
2. **Given** regeneration is run, **When** the gate runs, **Then** templates reflect the changed
   principle and the gate passes.
3. **Given** template prose that is genuine guidance (not a constitution restatement), **When** the
   generator runs, **Then** that prose is preserved unchanged.

---

### Edge Cases

- **Bidirectional confusion**: A contributor edits the *derived* artifact intending it as the
  source. The currency gate must make the direction unambiguous (provenance header on every
  generated file naming the source and the regeneration command), so the edit is caught and the
  fix is obvious.
- **Cross-platform generation**: Generation must not rely on symlinks or POSIX-only tooling; the
  derived tree/files must be reproducible bit-identically on every supported contributor platform
  (copy-generation, in-process, no shelling to `diff`/`cmp`/`sha256sum`).
- **Partial coverage regression**: The skill generator must enumerate the canonical tree so a newly
  added skill is covered automatically; it must not silently cover only a hardcoded subset (the
  exact gap that lets 19 of today's 25 pairs go unchecked).
- **Historical features**: The skillist currency check is scoped to the active feature only and
  does not re-derive the ~43 existing feature directories, so it cannot retroactively fail audits on
  directories whose `tasks.md`/`tasks.deps.yml` already agree.
- **Empty / malformed canonical input**: A missing or unparsable canonical source must fail the
  generator with a clear diagnostic rather than emitting an empty or partial derived artifact that
  would then pass identity checks.
- **Generated artifacts under version control**: Derived files remain committed (not gitignored) so
  the working tree is always coherent for an agent reading them mid-task; currency is enforced at
  gate time, not by regenerating on read.

## Requirements *(mandatory)*

### Functional Requirements

**Skill-tree single source (US1)**

- **FR-001**: The system MUST designate exactly one skill tree as canonical (`.agents/skills/`,
  the Codex source named in `CLAUDE.md`) and MUST provide a generation target that reproduces the
  derived tree (`.claude/skills/`) from it.
- **FR-002**: Generation MUST cover **every** skill directory present in the canonical tree by
  enumeration (currently 25), not a hardcoded subset, so adding a skill requires no allowlist edit.
- **FR-003**: After generation, every derived `SKILL.md` MUST be byte-identical to its canonical
  counterpart, verified in-process (no shelling to external `diff`/`cmp`/`sha256sum`).
- **FR-004**: The existing `SkillSyncCheck` MUST be converted into a **generation-currency** check:
  it MUST fail when the derived tree is not a current regeneration of the canonical tree, and its
  diagnostic MUST name the generation target to run. The companion `SkillExamplesCheck` (feature
  040) MUST be **retired** — once generation guarantees byte-identity between the trees its
  peer-comparison becomes redundant; its removal MUST update `Targets`, the build front-end, and
  any tests that reference it.

**Skillist single source (US2)**

- **FR-005**: The skillist for each task MUST have exactly one canonical representation; the other
  representation MUST be derived from it. The canonical/derived direction is an implementation
  decision deferred to planning (see Assumptions), but exactly one MUST be authoritative.
- **FR-006**: The system MUST provide a renderer/generator that produces the derived skillist
  representation from the canonical one, and a currency check that fails when the derived
  representation is stale, naming the regeneration action.
- **FR-007**: The skillist comparison currently performed by the evidence audit MUST be reframed
  from a peer drift-check into a generation-currency check scoped to the **active feature only** —
  it re-derives and verifies the current feature's skillist at gate time and MUST NOT re-derive the
  ~43 historical feature directories, so it introduces no false failures on existing directories
  whose representations already agree.

**Constitution single source (US3)**

- **FR-008**: Constitution principle content MUST exist in exactly one place
  (`.specify/memory/constitution.md`); the **templates** (`plan-template.md`/`tasks-template.md`)
  MUST carry generated principle fragments derived from it rather than independently restating
  principle text, so that **plans generated from those templates inherit the single-source
  fragments**. The generated content MUST be delimited by explicit `BEGIN GENERATED`/`END GENERATED`
  marker regions spliced into the existing template files; the generator replaces only the content
  **between** the markers.
- **FR-009**: The system MUST provide generation for the constitution-derived marker regions and a
  currency check that fails when a region's content is stale relative to the constitution source,
  naming the regeneration action.
- **FR-010**: Genuine guidance prose that is not a verbatim restatement of constitution principles
  MUST live **outside** the marker regions and MUST be preserved and MUST NOT be clobbered by
  generation.

**Cross-cutting: the generation-currency pattern (2.5)**

- **FR-011**: Every generated artifact introduced or converted by this feature MUST carry a
  machine-readable provenance header naming its source and the exact command to regenerate it.
- **FR-012**: Each drift-check this feature replaces MUST be converted to a generation-currency
  check whose failure message is **actionable** ("run target X to regenerate"), distinct from a
  bare "A and B differ" message.
- **FR-013**: Generated/derived artifacts MUST remain committed to version control (not gitignored)
  so the working tree is coherent for readers; currency is enforced only at gate time.
- **FR-014**: New generation logic MUST live in the compiled `FS.Skia.UI.Build` governance library
  as unit-testable modules, following the established `ContractView`/`TargetMetadata` single-source
  precedent; pure generation/comparison logic MUST be separable from filesystem I/O so it can be
  unit-tested without touching the repo tree (Principle IV).
- **FR-015**: Generation MUST be wired through the existing typed `Targets` model and the build
  front-end (a mistyped target name is a compile error). This feature adds **no new generation
  target**: regeneration reuses the existing `RefreshSurfaceBaselines` entry point, and currency
  reuses existing gates (`SkillSyncCheck`, `TargetMetadataDrift`, the evidence audit). The only
  target-set change is the `SkillExamplesCheck` **removal**, after which target metadata MUST be kept
  current so `TargetMetadataDrift` and the generated `validation.contract.yml` stay coherent.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, or version changes. `FS.Skia.UI.Build` gains
  internal generation modules but its consumer-facing contract is unaffected; no new
  `PackageVersion` outside `Directory.Packages.props`. No controls/chart/graph/DataGrid authoring
  change.
- **Public contract impact**: No product `.fsi` signatures, public APIs, sample contracts, or
  surface baselines change. The only `.fsi` edits are curated signatures for new governance-library
  generation modules (build-tooling surface, not product surface).
- **State workflow impact**: No change to stateful workflow, I/O, commands, effects, subscriptions,
  or interpreter behavior in the runtime. Build-side generation logic is pure with a thin I/O edge.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshots, Vulkan,
  Skia, visual output, or unsupported-environment diagnostics are touched.
- **Evidence obligations**: Real evidence required — generation-currency gate runs showing
  edit-without-regenerate fails and post-regenerate passes (skills, skillist, constitution);
  byte-identity proof for the skill trees across all 25 pairs; the standard serialized FAKE gate
  sequence green (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`); Governance.Tests unit tests for the new generation/currency
  modules. No synthetic evidence.
- **Unsupported scope**: No runtime/visual/release/platform/distribution change. Not in scope:
  trimming or rewriting skill/constitution *content* (Stage 6), porting any further Python/Bash,
  the dedicated build front-end / MEL extraction (Stage 5), contract versioning, evidence-bloat
  cleanup. Symlink-based sharing is explicitly out (copy-generation only).
- **Build-target impact**: Adds **no new target**. Regeneration of the derived skill tree and the
  constitution fragments folds into the existing `RefreshSurfaceBaselines`; the skillist annotation
  regenerates in the active-feature evidence path. `SkillSyncCheck` is converted into a
  generation-currency check, the constitution-fragment currency check folds into
  `TargetMetadataDrift`, and the now-redundant `SkillExamplesCheck` is **retired**.
  `EvidenceGraph`/`EvidenceAudit` skillist handling is reframed to currency. `TargetMetadataDrift`
  and the generated `validation.contract.yml` must reflect the **`SkillExamplesCheck` removal**. As a
  change to `.specify/**` + governance paths, `Route` escalates this to the full gate set.

## Success Criteria *(mandatory)*

- **SC-001**: Editing any one of the 25 canonical skills (including one of the 19 not covered by
  the old 6-slug check) without regenerating fails the currency gate; after regeneration the
  derived tree is bit-identical across all 25 pairs and the gate passes.
- **SC-002**: Adding a new skill directory to the canonical tree and regenerating produces the
  derived skill with **zero** edits to any per-skill allowlist or hardcoded slug list.
- **SC-003**: For a sample feature, editing the canonical skillist and regenerating updates the
  derived representation and keeps `EvidenceGraph`/`EvidenceAudit` green; editing the derived
  representation alone is flagged as stale.
- **SC-004**: Re-deriving the skillist introduces zero new audit failures across all existing
  feature directories (no historical regression).
- **SC-005**: Constitution principle content appears in exactly one file; changing a principle and
  regenerating updates the **governed template regions** (`plan-template.md`/`tasks-template.md`
  `BEGIN/END GENERATED` markers), so future generated plans inherit the single-source fragments. The
  count of independent principle restatements **inside the governed templates** is reduced to zero
  (the regions become generated includes). Paraphrases in already-generated plan instances and
  untrimmed template prose are **out of scope here** (Stage 6 content rewrite).
- **SC-006**: Every artifact this feature generates carries a provenance header naming its source
  and regeneration command; every drift-check it replaces now emits an actionable "regenerate"
  diagnostic on failure.
- **SC-007**: Duplicated-line count for the targeted classes drops measurably versus the Stage-0
  baseline; the eliminated-lines delta is recorded (target: the ~5,854-line skill mirror collapses
  to one source + a generator).
- **SC-008**: Each replaced drift-check is proven to be a true currency check by a scratch
  demonstration: source-edit-without-regenerate fails, regenerate passes (shown for skills,
  skillist, and constitution).
- **SC-009**: All standing invariants hold — product public surface unchanged
  (`PackageSurfaceCheck`, `FsiTranscripts` no baseline diff), runtime untouched (`git diff` over
  `src/**` = 0), generated consumers still fully governed (`TemplateCheck`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck` green), net10 conventions honoured, FAKE
  sequencing respected, evidence output vocabulary/counts unchanged.

## Key Entities *(include if feature involves data)*

- **Canonical skill tree** — `.agents/skills/`; the authoritative set of `SKILL.md` files (25
  today). The single source from which the derived tree is produced.
- **Derived skill tree** — `.claude/skills/`; a generated, committed, bit-identical reproduction of
  the canonical tree; never hand-edited.
- **Skillist (canonical vs derived)** — the per-task ordered list of skill ids; one representation
  (`tasks.deps.yml` `skillist:` or the `tasks.md` `[skillist: …]` annotation) is authoritative, the
  other derived.
- **Constitution source** — `.specify/memory/constitution.md`; the single statement of governing
  principles. **Constitution-derived fragments** — generated includes/sections that templates
  reference instead of restating.
- **Generation-currency check** — a gate that regenerates the derived artifact from its source and
  fails (with a "regenerate" diagnostic) if the committed derived artifact differs; replaces a
  peer-to-peer drift-check.
- **Provenance header** — a machine-readable banner on each generated file naming its source and
  regeneration command.

## Assumptions

- **Canonical skill direction**: `.agents/skills/` is canonical and `.claude/skills/` is generated,
  consistent with `CLAUDE.md` calling the Codex artifacts the source. If maintainer preference is
  the reverse, only the generation direction flips; requirements are unchanged.
- **Skillist canonical direction (deferred to planning)**: The plan leaves "render `tasks.md` from
  `tasks.deps.yml`, or vice-versa" open. The working assumption, per decision D6 (high-churn,
  agent-authored, logic-free instance data stays as data), is that `tasks.deps.yml` is the canonical
  skillist source and the `tasks.md` `[skillist: …]` annotation is the derived view. Planning will
  confirm direction and the mechanics of rendering an inline annotation into agent-authored
  markdown without disturbing surrounding prose.
- **Constitution generation granularity**: A pragmatic split is assumed — a small set of
  principle-summary fragments is generated from the constitution and *included/referenced* by
  templates, while genuine instructional prose stays hand-written. Full content rewrite/trim is
  Stage 6, not here.
- **Generated files stay committed** (not gitignored), matching the existing
  `validation.contract.yml` generation model, so agents reading them mid-task always see coherent
  content.
- **Copy-generation, not symlinks**, for cross-platform reproducibility.
- This feature follows the established single-source precedents already in the repo
  (`ContractView` from `Routing.fs`; metadata from `Targets.fs`) rather than inventing a new
  mechanism.

## Dependencies

- **Feature 040** (`SkillSyncCheck`, `SkillExamplesCheck`) — the byte-identity stopgap: `SkillSyncCheck`
  is upgraded into generation-currency and `SkillExamplesCheck` is retired as redundant.
- **Feature 041** (typed `Targets`, `TargetMetadata`) — the typed-target model new generation
  targets register against.
- **Feature 042** (`Routing.fs`, `ContractView`, `Route`) — the single-source generation precedent
  to mirror, and the router that escalates this `.specify/**`/governance change to the full gate
  set; the generated `validation.contract.yml` must stay coherent as targets are added.
- **Feature 043** (evidence engine in F#: `TaskParser`, `DepsParser`, `Audit`) — owns the current
  skillist drift-check that US2 reframes to a currency check.
- **Stage-0 baseline** (`docs/reports/_baselines/2026-05-31-foundations.md`) — the duplication-line
  baseline that SC-007's eliminated-lines delta is measured against.

## Out of Scope

- Trimming, rewriting, or reducing the *content* of skills, the constitution, or templates
  (Stage 6).
- The dedicated build front-end, MEL-engine extraction, or `build.fsx` retirement (Stage 5).
- Generated-product contract versioning / `schema_version` (Stage 6).
- Any further Python/Bash porting beyond what features 039–043 already delivered.
- Evidence-artifact / `.gitignore` hygiene (Stage 6.5).
- Any runtime, rendering, packaging, or public-`.fsi` product-surface change.
- Symlink-based or platform-specific sharing schemes.
