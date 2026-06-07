# Tasks: Close Out the Typed-Controls Front-Door Plan Loose Ends

**Feature branch**: `074-typed-controls-plan-closeout`
**Spec**: `specs/074-typed-controls-plan-closeout/spec.md`
**Plan**: `specs/074-typed-controls-plan-closeout/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/evidence-graph.md` for the propagated view.

**No `[S]` / `[S*]` / `[SEH]` is planned for this feature** (plan Constitution
Check → Synthetic evidence). This is a documentation / governance-only close-out:
the catalog-generation worked example, the refreshed plan report, and the new
`fs-skia-reconciliation` skill are all authored against real, shipped behavior on
`main`; the "tests" are the real skill-currency / skill-quality / contract-path
governance gates and the per-story independent reading tests. No mocks, fakes,
placeholders, or in-memory substitutes are introduced. `EvidenceAudit` must be
PASS with no disclosures.

## Vertical-slice rule (US phases)

The user-reachable entry point for every story here is a **maintainer reading a
governed artifact**: the `fsharp-code-generation` skill (US1), the refreshed
implementation-plan report (US2), and the new `fs-skia-reconciliation` skill
(US3). A `[US*]` task may only be marked `[X]` when the artifact is actually
authored, regenerated where applicable, and its independent reading test passes
(the reader can answer the acceptance questions without reading source). A
canonical `.agents` edit that has not been regenerated to its `.claude` peer (so
`SkillSyncCheck` would fail) does **not** satisfy `[X]`. Principle IV (MVU) is
**not applicable** — no stateful, I/O-bearing, or behavior-changing code is
touched; `Reconcile` stays `module internal` and unwired (FR-010).

## Success-criterion → assertion mapping

- **SC-001** (reader names `catalogFacts`, both generated artifacts, the regen
  target, and the drift gate without reading source) → US1 independent reading
  test: T009.
- **SC-002** (every edited/added skill's `.agents` ↔ `.claude` peer in sync, zero
  drift) → `SkillSyncCheck` after regeneration for both skills: T008, T015.
- **SC-003** (every plan status claim matches `git log` on `main`; zero
  `fs-skia-project` references) → US2 cross-check against git history: T010, T012.
- **SC-004** (reader answers "is it dead code?", "what are the diff invariants?",
  "what would wiring take?" from the skill alone) → US3 independent reading test:
  T016.
- **SC-005** (public package surface baseline shows zero delta) → surface-delta
  confirmation: T018.
- **SC-006** (every gate `./fake.sh build -t Route` prints passes) → routed gate
  run: T017.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T2]** — Tier 2 (internal / documentation-governance) change; this whole
  feature is Tier 2, so the annotation is omitted on individual lines.

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors its
structured `skillist` value with `[skillist: ...]` (`[skillist: []]` when empty).

## Governance risk level

This feature's governance risk is **small / focused**: a skill-source change plus
a historical-report refresh, with **no** `Routing.fs` rule, package, public
`.fsi`, or runtime change. The focused validation is exactly the gate list
`./fake.sh build -t Route` prints for the diff (e.g. `Dev`, `SkillSyncCheck`,
`SkillQualityCheck`, `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`,
`GeneratedGuidanceCheck`, `TemplateDrift`), run sequentially because `.fake`
state is shared. Broad validation (the serialized six-target maintainer-verify
order) is **not** required and is only invoked if `Route` escalates. Aggregate or
multi-target runs are recorded as **non-authoritative** in
`readiness/aggregate-hang-diagnostics.md`; the authoritative verdict is each
focused gate's own result.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Record the feature classification in `readiness/runtime-limitations.md`: Tier 2 (internal/documentation-governance), affected paths (`.agents/skills/fsharp-code-generation`, new `.agents/skills/fs-skia-reconciliation`, the plan report; regenerated `.claude` peers), public-API impact = none (SC-005), MVU applicability = N/A, and the evidence obligations (skill-currency for both skills + the refreshed plan report)
- [X] T002 [P] [skillist: []] Scaffold `specs/074-typed-controls-plan-closeout/readiness/` with the audit-enforced governance placeholders discoverable before implementation: `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation-authority.md`, `skill-loading-evidence-workflow.md`, `audit-diagnostics.md`, `readiness-contract-discovery.md`, `framework-guidance.md`, `evidence-vocabulary.md`, `evidence-graph.md`, and `evidence-audit.md` (each naming its authoritative command, artifact path, failure class, and next action)

---

## Phase 2: Foundation

- [X] T003 [skillist: []] Read the read-only reference material and pin the facts the skills must state accurately — `build/Governance/CatalogGen.fsi` (US1: `catalogFacts`, `catalog.yml`/`Catalog.fs`, `RegenerateCatalog`, `ControlsCatalogGenerationCheck`, splice markers, the `FS.Skia.UI.Controls.Typed` cross-check) and `src/Controls/Reconcile.fsi` (US3: `module internal`, `diff`/`apply`, key-then-positional matching, `NodePatch`/`ChildOp` set, `KeyCollision`, disposition) — without editing either file
- [X] T004 [skillist: []] Record the skill single-source contract (C1) in `readiness/skill-loading-evidence-workflow.md`: `.agents` is canonical, `.claude` is generated by `./fake.sh build -t RefreshSurfaceBaselines`, the peer is never hand-edited, discovery is by `SKILL.md` frontmatter `name:`, and `SkillSyncCheck` fails on any drift

**Checkpoint**: Foundation ready — the three stories may proceed in parallel.

---

## Phase 3: User Story 1 (US1) — Catalog-generation pattern documented

- [X] T005 [US1] [skillist: fsharp-code-generation] Author the feature-066 single-source catalog-generation worked example in `.agents/skills/fsharp-code-generation/SKILL.md`: name the canonical `catalogFacts : TypedCatalogFact list`, the two generated artifacts (`catalog.yml` + `Catalog.fs`), `RegenerateCatalog` within `RefreshSurfaceBaselines`, and the `ControlsCatalogGenerationCheck` drift gate (FR-001); explain the `Module`/required-attribute cross-check against the `FS.Skia.UI.Controls.Typed` surface and state that hand-editing a generated `typed-catalog/<id>` region fails the gate while rows outside the markers are untouched (FR-003)
- [X] T006 [US1] [skillist: []] Add a `Related` link to `[[fs-skia-typed-controls]]` in the same skill, re-attributing the "typed authoring is the preferred front door" guidance to the skill that actually carries it (supports FR-005 re-attribution)
- [X] T007 [US1] [skillist: []] Regenerate the generated peer with `./fake.sh build -t RefreshSurfaceBaselines` so `.claude/skills/fsharp-code-generation/SKILL.md` is rebuilt from the canonical source (never hand-edited) (FR-002)
- [X] T008 [US1] [skillist: []] Verify the skill governance gates for `fsharp-code-generation`: `SkillSyncCheck` reports zero drift (SC-002), `SkillQualityCheck` and `SkillContractPathCheck` pass; capture results to `readiness/skill-loading-evidence-workflow.md`
- [X] T009 [US1] [skillist: []] Confirm the US1 independent reading test (SC-001): a maintainer reading the updated skill cold can name the fact table, both generated artifacts, the regeneration target, and the drift gate, and can state that hand-editing a generated artifact fails the gate — record the walk-through in `readiness/skill-loading-evidence-workflow.md`

**Checkpoint**: US1 complete — the catalog-generation pattern is teachable from the skill alone.

---

## Phase 4: User Story 2 (US2) — Implementation-plan document matches `main`

- [X] T010 [US2] [skillist: []] Cross-check the actual merged state of roadmap features 065–073 against `git log` on `main` (gather each squash commit) and 073's "motion" delivery, so the refresh asserts facts rather than guesses (input to SC-003)
- [X] T011 [US2] [skillist: []] Refresh only the forward-looking/status regions of `docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md` — status header + status-by-feature table (065–073 merged with squash commits, no lingering "awaiting"/"Planned", FR-004), §13 roadmap (073 animations recorded as the delivered "motion" item, FR-007), and §16 skills backlog (catalog-generation item marked done → US1; remove the `fs-skia-project` reference and re-attribute "typed is preferred" to `fs-skia-typed-controls`, FR-005; record shipped `fs-skia-typed-controls`/`fs-skia-design-tokens`/`fs-skia-reconciliation` vs. folded `fs-skia-catalog-generation` → `fsharp-code-generation`, FR-006) — leaving the §1-onward provenance body unedited (A4)
- [X] T012 [US2] [skillist: []] Confirm the US2 independent test (SC-003): every status claim in the progress table and §13 roadmap matches `git log` on `main` with zero contradictions, and the document contains zero `fs-skia-project` references — record the cross-check in `readiness/audit-diagnostics.md`

**Checkpoint**: US2 complete — the plan report matches `main` with no unfollowable instructions.

---

## Phase 5: User Story 3 (US3) — Keyed-reconciliation capability skill

- [X] T013 [US3] [skillist: []] Create `.agents/skills/fs-skia-reconciliation/SKILL.md` with required frontmatter (`name: fs-skia-reconciliation`, one-line `description`, `compatibility` noting the internal/no-public-surface scope, `metadata.{author,source}`) so `SkillRegistry` discovers it by `name:` (FR-008)
- [X] T014 [US3] [skillist: []] Author the skill body teaching the keyed-VDOM-diff invariants — key-first-then-positional child matching, `Kind`-mismatch ⇒ whole-subtree `Replace`, the `NodePatch`/`ChildOp` operation set with `UpdatePatch`/`FieldChange`/`AttrChange`, the `KeyCollision` duplicate-key diagnostic, and the totality/determinism/identity-at-rest/round-trip properties — and recording the module **disposition**: `module internal`, property-tested via `InternalsVisibleTo("Controls.Tests")`, deliberately unwired, parked, with live-render-path integration named as deferred out-of-scope future work plus the integration point it would touch (FR-009, FR-010)
- [X] T015 [US3] [skillist: []] Regenerate with `./fake.sh build -t RefreshSurfaceBaselines` so the `.claude/skills/fs-skia-reconciliation/SKILL.md` peer and the skill index (`GENERATED.md` / skillist-reference) are produced from the canonical source, then verify `SkillSyncCheck` (zero drift, SC-002), `SkillQualityCheck`, and `SkillContractPathCheck` pass for the new skill
- [X] T016 [US3] [skillist: []] Confirm the US3 independent reading test (SC-004): a maintainer reading the skill cold can state it is a deliberately-parked internal spike (not dead code), name the diff invariants and operation set, and explain that render-path wiring is a separate out-of-scope future feature — record the walk-through in `readiness/skill-loading-evidence-workflow.md`

**Checkpoint**: US3 complete — the parked reconciliation spike's invariants and disposition are teachable from the skill alone.

---

## Phase 6: Integration & Polish

- [X] T017 [skillist: []] Run `./fake.sh build -t Route` for the branch diff to get the authoritative tier + minimal gate list, then run **only** the printed gates sequentially (`.fake` state is shared) — capturing each verdict to `readiness/focused-gates.md` and `readiness/generated-validation-authority.md` (SC-006); record any aggregate/multi-target timing as non-authoritative in `readiness/aggregate-hang-diagnostics.md`
- [X] T018 [skillist: []] Confirm the public package surface baseline shows zero delta attributable to this feature (SC-005) — documentation/governance only, no `.fsi` or surface change — and record the result in `readiness/generated-validation-authority.md`
- [X] T019 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm the DAG has no cycles, no dangling refs, valid `skillist` metadata, and no `[S*]` surprises; write the rendered graph to `readiness/evidence-graph.md`
- [X] T020 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no `[S]`/`[S*]` disclosures and no `--accept-synthetic` overrides; write the audit result to `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none — all tasks land `[X]` against real governance-gate and independent-reading evidence)_ | | | | | | | | |
