# Feature Specification: Close Out the Typed-Controls Front-Door Plan Loose Ends

**Feature Branch**: `074-typed-controls-plan-closeout`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "create specs for the ommissions/problems" — the omissions/problems surfaced when auditing `docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md` against `main`. The plan's roadmap (065–073) all shipped, but three close-out items were never completed: (1) the §16.2 commitment to document the feature-066 single-source catalog-generation pattern in the `fsharp-code-generation` skill was not done and is now more load-bearing after 072 catalog expansion; (2) the plan document itself is stale (shows 069 "awaiting" and 070/071+ "Planned") and §16.2 instructs updating a `fs-skia-project` skill that does not exist; (3) the keyed-reconciliation module (067) remains an internal, fully-tested but unwired spike with no recorded decision about its future.

## Overview

This is a **housekeeping / close-out** feature. It carries **no product runtime behavior change** and **no public package-surface change**. It pays down three documentation- and governance-debt items left by the typed-controls front-door programme (features 065–073) so the maintainer-facing record matches `main` and the load-bearing single-source-generation pattern is teachable.

The three items are independent and can land in any order; each is its own user story below.

## Clarifications

### Session 2026-06-06

- Q: Where should the feature-066 catalog-generation pattern be documented (US1)? → A: Fold into the existing `fsharp-code-generation` skill (confirms Assumption A2; no new skill). Rationale: `CatalogGen` lives in `build/Governance`, which matches that skill's declared build-tooling scope, and the plan's §16.3 explicitly allowed folding in.
- Q: How should the keyed-reconciliation (067) knowledge be captured (US3)? → A: Create a **new `fs-skia-reconciliation` capability skill** covering the keyed-VDOM-diff invariants, key handling, and property tests — **not** merely a contributor note (reverses Assumption A5). The skill also records the module's disposition (internal, deliberately unwired, parked).

## User Scenarios & Testing *(mandatory)*

### User Story 1 — Catalog-generation pattern is documented in the code-generation skill (Priority: P1)

A maintainer about to extend the typed control catalog (as feature 072 did, and as future
breadth features will) opens the `fsharp-code-generation` skill to learn the established
single-source pattern: a canonical `catalogFacts` fact table that generates both
`catalog.yml` and `Catalog.fs`, is cross-checked against the `FS.Skia.UI.Controls.Typed`
surface, is regenerated via `RegenerateCatalog` inside `RefreshSurfaceBaselines`, and is
drift-guarded by `ControlsCatalogGenerationCheck`. Today the skill documents only
*governance* artifact emission and a prose note on F# source generation; the catalog
generator — the first **product-surface** generator and the template every later generator
copied — is absent.

**Why this priority**: This is the only *substantive* backlog item. The plan's §16.2 row
flagged it as "load-bearing and undocumented," and feature 072 has since exercised the
pattern again, so the gap is now more acute, not less. A maintainer who can't find the
worked example reinvents (or breaks) the drift contract.

**Independent Test**: A maintainer reads the `fsharp-code-generation` skill cold and can,
without reading source, name the canonical fact table, the two generated artifacts, the
regeneration target, and the drift gate, and state that hand-editing a generated artifact
fails the gate. The skill's canonical `.agents` source and generated `.claude` peer are in
sync, and the skill-quality / sync gates pass.

**Acceptance Scenarios**:

1. **Given** the updated `fsharp-code-generation` skill, **When** a maintainer searches it for catalog generation, **Then** they find a worked example naming `catalogFacts`, `catalog.yml` + `Catalog.fs` as the generated outputs, `RegenerateCatalog` within `RefreshSurfaceBaselines`, and `ControlsCatalogGenerationCheck` as the drift gate.
2. **Given** the worked example, **When** a maintainer follows it to add a catalog row, **Then** the documented steps produce a regenerated pair of artifacts and a passing drift gate without hand-editing generated files.
3. **Given** the edited canonical `.agents` skill source, **When** the surface-baseline regeneration runs, **Then** the generated `.claude` peer is regenerated from it and the skill-sync check reports no drift.

---

### User Story 2 — Implementation-plan document matches `main` (Priority: P2)

A maintainer (or a future agent) opening
`docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md` sees a
status that matches reality: features 065–073 are all merged, the §16 skills backlog
reflects what was actually delivered (the `fs-skia-typed-controls` and `fs-skia-design-tokens`
skills landed; the catalog-generation documentation is now done per US1), and the document
contains **no** instruction referencing a `fs-skia-project` skill that does not exist in the
corpus. The §13 roadmap shows 073 (animations) as the delivered "motion" item.

**Why this priority**: A stale plan misleads the next reader into thinking work is
outstanding when it shipped, or into trying to follow an unfollowable instruction. It is
pure record-correction with no code risk, so it ranks below the substantive US1 but above
the optional US3 decision.

**Independent Test**: A reader cross-checks every status claim in the plan's progress table
and §13 roadmap against `git log` on `main` and finds no contradiction, and finds no
reference to a non-existent skill.

**Acceptance Scenarios**:

1. **Given** the refreshed plan, **When** a reader compares the status table to `git log`, **Then** every roadmap feature 065–073 is marked merged with its squash commit, and none is marked "awaiting" or "planned" that is in fact merged.
2. **Given** the refreshed plan §16, **When** a reader looks for skill instructions, **Then** every skill named exists in the corpus (no `fs-skia-project` reference), and the catalog-generation backlog item is marked done (pointing at US1's result) rather than open.
3. **Given** the refreshed plan §13, **When** a reader looks for the "motion / Later" item, **Then** feature 073 (animations) is recorded as delivered rather than unscheduled.

---

### User Story 3 — Keyed-reconciliation has a capability skill (Priority: P3)

A maintainer working with the keyed-VDOM-diff module (`src/Controls/Reconcile.fs(i)`, feature
067) can open a dedicated **`fs-skia-reconciliation` capability skill** that teaches the
diff's invariants, key-matching rules, the `NodePatch`/`ChildOp` operation set, and the
property-test approach (totality, determinism, identity-at-rest, key-collision diagnostics).
The skill also records the module's **disposition**: it is `module internal`,
property-tested, **deliberately unwired** from the live render path, and **parked** —
intentional, not abandoned — with live-render-path integration named as deferred future work.

**Why this priority**: The module is internal (`module internal Reconcile`) and carries no
public-surface or consumer weight, so it is the lowest-urgency item. But a dedicated skill
makes the parked spike's invariants teachable and gives the eventual wiring feature a
documented foundation rather than source-comment archaeology. Actually wiring it into the
render path remains explicitly **out of scope** (a separate future feature).

**Independent Test**: A maintainer reading the `fs-skia-reconciliation` skill cold can state
the diff's key-matching rule, the operation set, the totality/determinism invariants, the
module's internal+unwired+parked status, and that render-path integration is deferred —
without inferring any of it from source comments. The skill's canonical `.agents` source and
generated `.claude` peer are in sync and the skill-quality / sync gates pass.

**Acceptance Scenarios**:

1. **Given** the `fs-skia-reconciliation` skill, **When** a maintainer asks "is `Reconcile` dead code?", **Then** the skill states it is a deliberately-parked internal spike, property-tested, unwired by design, not abandoned.
2. **Given** the skill, **When** a maintainer asks "what would wiring it in take?", **Then** the skill names the render/diff integration point and confirms that work is a separate, out-of-scope future feature.
3. **Given** the skill, **When** a contributor needs the diff invariants, **Then** the skill states the key-first-then-positional matching rule, the `NodePatch`/`ChildOp` operation set, and the totality/determinism/identity-at-rest properties.
4. **Given** the edited canonical `.agents/skills/fs-skia-reconciliation` source, **When** the surface-baseline regeneration runs, **Then** the generated `.claude` peer is regenerated from it and the skill-sync check reports no drift.

### Edge Cases

- If the canonical `.agents` skill source is edited but the generated `.claude` peer is not regenerated, the skill-sync gate MUST fail rather than silently drift (this is the existing contract; US1 must not weaken it).
- The plan document is historical provenance; refresh MUST correct the status/forward-looking sections without rewriting the preserved original plan text (§1 onward) that the document explicitly retains for provenance.
- If a future decision reverses US3 (wire reconciliation in), the recorded disposition MUST be the thing that gets updated, so there is a single source for the module's status.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The `fsharp-code-generation` skill MUST include a worked example of the feature-066 single-source catalog-generation pattern, naming the canonical `catalogFacts` fact table, the two generated artifacts (`catalog.yml` and `Catalog.fs`), the `RegenerateCatalog` step within `RefreshSurfaceBaselines`, and the `ControlsCatalogGenerationCheck` drift gate.
- **FR-002**: The catalog-generation documentation MUST be authored in the canonical `.agents/skills/fsharp-code-generation` source, and the generated `.claude` peer MUST be regenerated from it (never hand-edited), keeping the skill-sync contract satisfied.
- **FR-003**: The catalog-generation documentation MUST explain the cross-check between the `catalogFacts` `Module`/required-attribute facts and the `FS.Skia.UI.Controls.Typed` surface, and MUST state that hand-editing a generated catalog artifact fails the drift gate.
- **FR-004**: The implementation-plan document's progress/status table MUST be updated so every roadmap feature 065–073 reflects its actual merged state on `main`, with no feature shown as "awaiting" or "planned" that is in fact merged.
- **FR-005**: The implementation-plan document MUST NOT instruct updating or reference a `fs-skia-project` skill (which does not exist); §16.2 MUST be corrected to name only skills that exist, and the typed-authoring-is-preferred guidance MUST be attributed to the skill that actually carries it.
- **FR-006**: The implementation-plan document's §16 skills backlog MUST mark the catalog-generation documentation item as done (referencing this feature's US1 outcome) and reflect which proposed skills shipped: `fs-skia-typed-controls`, `fs-skia-design-tokens`, and now `fs-skia-reconciliation` (US3); and which were intentionally folded rather than created standalone (`fs-skia-catalog-generation` folded into `fsharp-code-generation`).
- **FR-007**: The implementation-plan document's §13 roadmap MUST record feature 073 (animations) as the delivered "motion" item rather than an unscheduled "Later" item.
- **FR-008**: A new `fs-skia-reconciliation` capability skill MUST exist, authored in the canonical `.agents/skills/fs-skia-reconciliation` source with its generated `.claude` peer regenerated from it (never hand-edited), satisfying the skill-sync, skill-quality, and skill-contract-path contracts.
- **FR-009**: The `fs-skia-reconciliation` skill MUST teach the keyed-VDOM-diff invariants — key-first-then-positional child matching, the `NodePatch`/`ChildOp` operation set, and the totality/determinism/identity-at-rest and key-collision-diagnostic properties — and MUST record the module's disposition: internal, property-tested, deliberately unwired from the render path, and parked, with live-render-path integration named as deferred future work (including the integration point that work would touch).
- **FR-010**: This feature MUST NOT change any public `.fsi` signature, package identity, package version semantics beyond the routine post-merge bump, or runtime behavior; it is documentation/governance only. In particular, the `Reconcile` module stays `module internal` and is **not** wired into the render path by this feature.

> Interacting / conflicting requirements: FR-004/FR-006 (refresh the plan) vs. the plan's
> own provenance rule (it preserves original text from §1 onward). Resolution: refresh only
> the forward-looking/status sections (progress table, §13 roadmap, §16 backlog); leave the
> preserved original plan body unedited, matching the document's stated provenance contract.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, contents, or version changes beyond the routine post-merge version bump that the `speckit-merge` flow applies to packable projects. No catalog *content* change (no new control rows); only the *documentation* of the catalog-generation pattern changes.
- **Public contract impact**: None. No `.fsi` signature, documented public API, sample contract, or surface baseline changes. The reconciliation module stays `internal`. (Skill SKILL.md files are governed artifacts, not package public surface.)
- **State workflow impact**: None. No stateful workflow, I/O, command, effect, subscription, or interpreter behavior changes.
- **Layout/rendering impact**: None. The reconciliation module remains unwired from the render path by explicit decision (US3); no layout, charts, DataGrid, rendering, screenshot, Vulkan, or Skia behavior changes.
- **Evidence obligations**: Skill-currency evidence (canonical `.agents` ↔ generated `.claude` in sync) for both the updated `fsharp-code-generation` skill and the new `fs-skia-reconciliation` skill; the refreshed plan document. The standard readiness artifacts for the routed tier.
- **Unsupported scope**: Wiring keyed reconciliation into the live render/diff path is **out of scope** (deferred to a separate future feature) — the `fs-skia-reconciliation` skill documents the parked module, it does not wire it. Creating a standalone `fs-skia-catalog-generation` skill is **out of scope** (catalog generation is folded into `fsharp-code-generation`). No new product controls, tokens, or animation work.
- **Build-target impact**: A skill-source change routes (via `./fake.sh build -t Route`) to the skill/focused-authority gate set (e.g. `Dev`, `SkillSyncCheck`, `SkillQualityCheck`, `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`) and `RefreshSurfaceBaselines` must be run to regenerate the `.claude` peer. No `Routing.fs` rule change is required. Run `Route` first and run only the gates it prints.

## Success Criteria *(mandatory)*

- **SC-001**: A maintainer reading the `fsharp-code-generation` skill cold can, without reading source, correctly name the canonical catalog fact table, both generated artifacts, the regeneration target, and the drift gate (US1 independent test passes).
- **SC-002**: Every edited or added skill's canonical `.agents` source and generated `.claude` peer are in sync — the skill-sync check reports zero drift after the change (covers both `fsharp-code-generation` and the new `fs-skia-reconciliation`).
- **SC-003**: Cross-checking every status claim in the plan document's progress table and §13 roadmap against `main`'s git history yields zero contradictions, and the document contains zero references to a non-existent `fs-skia-project` skill.
- **SC-004**: The `fs-skia-reconciliation` skill exists, passes the skill-quality gate, and lets a reader answer "is it dead code?", "what are the diff invariants?", and "what would wiring it take?" without inferring from source comments.
- **SC-005**: The public package surface baseline shows zero delta attributable to this feature (documentation/governance only).
- **SC-006**: Every gate printed by `./fake.sh build -t Route` for this branch passes.

## Assumptions

- **A1**: The keyed-reconciliation module stays parked (a tested internal spike); actually wiring it into the render path is a separate future feature, not part of this close-out. This is the lower-risk default consistent with 067 having deliberately shipped the module unwired. The `fs-skia-reconciliation` skill (per the clarification) documents that parked state; it does not wire it. (If the maintainer instead wants it wired now, that is a distinct, larger feature and should be specified separately.)
- **A2**: The catalog-generation documentation is folded into the existing `fsharp-code-generation` skill rather than created as a new `fs-skia-catalog-generation` skill — confirmed in the clarification session; matches the plan's own §16.3 "(or fold into `fsharp-code-generation`)" allowance and avoids skill-corpus sprawl. `CatalogGen` lives in `build/Governance`, which is within that skill's declared build-tooling scope.
- **A3**: The "typed authoring is the preferred front door" guidance that §16.2 attributed to the non-existent `fs-skia-project` skill is in fact carried by `fs-skia-typed-controls`; the plan correction re-attributes it there.
- **A4**: The plan document's preserved original body (§1 onward) is provenance and is left unedited; only its status/progress/roadmap/backlog sections are refreshed.
- **A5**: *(Superseded by the 2026-06-06 clarification.)* The reconciliation knowledge is captured as a **new `fs-skia-reconciliation` capability skill** (not merely a contributor note), authored canonically in `.agents` and regenerated to `.claude`.

## Dependencies

- The feature-066 catalog-generation implementation (`build/Governance/CatalogGen.*`, `ControlsCatalogGenerationCheck`, `RegenerateCatalog`) on `main` — the worked example documents existing, shipped behavior; it does not change it.
- The skill single-source generation pipeline (`.agents` → `.claude` via `RefreshSurfaceBaselines`, enforced by `SkillSyncCheck`).
- The feature-067 reconciliation module (`src/Controls/Reconcile.*`) and its 067 readiness evidence on `main` — the new `fs-skia-reconciliation` skill documents the module's invariants and status; it does not change the module.
- The skill registry / quality / contract-path / sync governance (`SkillSyncCheck`, `SkillQualityCheck`, `SkillContractPathCheck`) — the new `fs-skia-reconciliation` skill is discovered dynamically from its `SKILL.md` frontmatter `name:`; no hardcoded skill list to edit.

## Out of Scope (Non-Goals)

- Wiring keyed reconciliation into the live render/diff path (separate future feature) — the new `fs-skia-reconciliation` skill documents the parked module, it does not wire it.
- Creating a standalone `fs-skia-catalog-generation` skill (catalog generation is folded into `fsharp-code-generation`).
- Any change to product controls, the typed surface, design tokens, animations, or the catalog *contents*.
- Any public `.fsi` / package-surface change or non-routine version change.
- Rewriting the preserved historical body of the implementation-plan document.
