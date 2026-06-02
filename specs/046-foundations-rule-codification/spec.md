# Feature Specification: Codify Remaining Rules, Trim Prose, Version the Contract

**Feature Branch**: `046-foundations-rule-codification`
**Created**: 2026-06-01
**Status**: Draft
**Input**: User description: "implement the next part of the plan — Stage 6 of `docs/reports/2026-05-31-1049-foundations-implementation-plan.md` (codify remaining bucket-(a) prose rules into self-enforcing library gates, trim bucket-(b) prose, version the generated-product contract, evidence-hygiene `.gitignore`), continuing the foundations programme after Stages 0–5 (features 039–045)."

## Context & Motivation *(informative)*

This is **Stage 6** of the foundations programme, run after the keystone completed in Stage 5
(feature 045 deleted `build.fsx` and relocated all build/governance logic into the compiled,
tested `FS.Skia.UI.Build` library). The companion analysis
(`docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md`) framed the governance Markdown as
the project's last large instance of the very anti-pattern the framework exists to abolish: *a
checkable invariant enforced by prose a nondeterministic agent is trusted to honour, instead of
code that fails the build.* Stage 6 converts the remaining such rules into self-enforcing library
gates, trims the prose those gates make redundant, versions the consumer-facing generated-product
contract so it can evolve without a hard break, and closes a small evidence-hygiene gap.

**What earlier stages already did (verified, so this feature does NOT redo it).** Three of the four
bucket-(a) rules the plan named under Stage 6.1 are *already* enforced as build-failing gates by
prior features and must stay that way:

- **`[SEH]` design-phase-only timing** — a late `[SEH]` tag already drives `verdict=Fail` in the
  in-process `EvidenceAudit` (`build/Governance/Evidence/Audit.fs`; the `late-seh-tasks` count
  feeds the blocker total). Done in feature 043.
- **Skill-id resolution / no-dangling-ids** — a task referencing an unregistered or ambiguous
  skill id already drives the graph to `verdict=error` (`Evidence/Audit.fs` → `Evidence/Engine.fs`).
  Done in feature 043.
- **Surface-baseline presence per public capability** — a missing/non-existent `surfaceBaseline`
  already emits a build-failing finding (`build/Governance/Capabilities.fs`). Done in feature 041.

So the genuinely-remaining Stage-6 work is the **fourth** bucket-(a) rule (Constitution-Check
completeness, which today is prose + manual review only), the **contract-versioning** item (6.4),
the **prose trim** that follows from rules now living in code (6.2/6.3), and the small
**evidence-hygiene `.gitignore`** edit (6.5).

**A correction to the plan's baseline figure.** The plan's headline "~23,000 lines of governance
Markdown, 21:1 prose-to-code" counted the whole corpus including `specs/**` and the then-duplicated
`.claude`+`.agents` mirror. After feature 044 single-sourced `.claude` from `.agents`, the
*rule/guidance* Markdown the agent actually reads is ~6,900 lines (`.agents/skills/**` ≈ 4,065 +
`.specify/**` ≈ 2,817). The "trim to the low hundreds" target was always about *rule* prose, not
all guidance; this feature reframes 6.2/6.3 as "delete the rule prose that code now enforces, keep
genuine rationale/intent" and records the measured before/after delta rather than chasing the
overstated 23,000 figure.

This is framework-tooling work that **escalates** (governance paths + a `template/**` /
generated-product-contract change) to the full serialized gate set via `Route`, and is a
**dogfood** candidate. The runtime architecture (`Scene → SkiaViewer → Elmish`) and the product's
public `.fsi` surface are explicitly untouched.

## Clarifications

### Session 2026-06-01

- Q: Source of truth for the required Constitution-Check decision areas? → A: A hard-coded typed list of stable area identifiers in the `FS.Skia.UI.Build` validator owns the canonical required set; the live `plan-template.md` structure is read only to detect an unrecognized template revision and emit that diagnostic (adding/removing an area is a code change + test).
- Q: How is a present Constitution-Check area judged "unfilled"? → A: An area is unfilled if its body is empty, OR still contains the template's verbatim boilerplate prompt text (e.g. "Decide whether…"), OR carries a NEEDS CLARIFICATION / TODO placeholder. An area explicitly marked N/A-with-rationale counts as filled.
- Q: Form/location of the machine-readable generated-product contract changelog? → A: Typed changelog entries embedded as data in the versioned-contract F# module under `build/Governance/`, surfaced in `GeneratedProductCheck` output (no separate file to keep in sync; covered by typed-result tests).

## User Scenarios & Testing *(mandatory)*

### User Story 1 - The Constitution Check stops being honour-system prose (Priority: P1)

A maintainer or AI agent authors or edits a feature `plan.md`. The plan template carries a
"Constitution Check" section enumerating the required decision areas (template-ownership impact,
dependency impact, command-surface impact, generated-project impact, evidence paths, `.fsi`/contract
impact, MVU boundary, synthetic-evidence stance, test evidence, observability, deferred scope).
Today nothing fails if a decision area is left blank, missing, or a stray placeholder — the section
is enforced only by asking the agent to read prose and comply.

After this feature, a governance gate parses the decision-area bullets in the active feature's
`plan.md` Repository Governance Decisions subsection (under Constitution Check) and **fails the
build** when a required decision area is absent or unfilled, naming each missing area. A complete plan passes; an incomplete one is rejected with an actionable diagnostic.

**Independent test:** Take a `plan.md` with all required Constitution-Check areas filled → the gate
passes. Remove or blank one required area → the gate fails, naming exactly that area. Restore it →
the gate passes again. Asserted by typed unit tests over the parser/validator (no build run
required to test) plus a live gate run on the active feature.

### User Story 2 - The generated-product contract can evolve without a hard break (Priority: P1)

A template author changes the structure a generated `dotnet new fs-skia-ui` project must have.
Today the ~800 lines of generated-product structural checks (`build/Governance/GeneratedProduct.fs`)
are unversioned and monolithic: every expectation is hard-coded, there is no `schema_version`, and
any change is an immediate hard break with no migration window.

After this feature, the generated-product contract carries a `schema_version` and a documented
**deprecation window**: a structural rule can be marked deprecated (still checked, emits a warning
naming the removal version) for at least one version before it becomes a hard requirement or is
removed, and the contract version + a machine-readable change log record the evolution. A
generated project built against the prior contract version still validates during the deprecation
window; the version bump and changelog entry are required when a breaking structural rule changes.

**Independent test:** Mark a structural rule deprecated → a generated product that violates only
that rule passes with a warning naming the removal version, not a failure. Bump the contract
`schema_version` and promote the rule to required → the same product now fails. The changelog
records both transitions. Asserted by typed unit tests over the versioned contract model plus
`GeneratedProductCheck` on a real generated project.

### User Story 3 - Prose the code now enforces is removed; agent context shrinks (Priority: P2)

An AI agent loads the governance skills (`.agents/skills/**`, mirrored to `.claude/skills/**`) on
every invocation, paying attention and token cost to read rules. Many of those rules are now
enforced deterministically by the library (Stage 6.1 gates + the new Constitution-Check gate), so
the agent reading them is redundant cost.

After this feature, every rule that a build-failing gate now enforces is **removed from the skill
prose** (replaced, where useful, by a one-line pointer to the enforcing gate), while genuine
rationale / intent / when-to-use guidance is kept. The before/after governance-Markdown line count
and the per-invocation skill-byte load are measured and recorded. A rule is only deleted from prose
**after** its code gate exists and is proven to fail on violation.

**Independent test:** For each deleted rule, show (a) the enforcing gate fails on a seeded violation
and (b) the rule's prose is gone, with the skill files still byte-identical across `.agents` →
generated `.claude` (the 044 single-source generation stays current). Record the line-count and
skill-byte delta vs the Stage-0 baseline.

### User Story 4 - Future regenerable evidence stays out of git (Priority: P3)

A contributor runs the evidence/readiness gates, which produce regenerable logs and `readiness*.zip`
archives under `specs/*/readiness/`. Today `.gitignore` covers `bin/`, `obj/`, `artifacts/`, and a
few specific paths, but not these regenerable evidence logs/zips, so they can be committed by
accident.

After this feature, `.gitignore` excludes future regenerable readiness logs/zips so they are not
committed going forward. Per decision D3, **no existing committed evidence is removed and no history
is rewritten** — this is purely a forward-looking ignore rule.

**Independent test:** A freshly generated `readiness*.zip` / readiness log under a feature's
`readiness/` tree shows as ignored by `git status` / `git check-ignore`; previously-committed
evidence is untouched (still tracked).

### Edge Cases

- A `plan.md` that uses a future/older plan-template revision with renamed decision areas — the
  Constitution-Check validator MUST key off its hard-coded stable area identifiers rather than
  brittle exact-string headings, and degrade to a clear "unrecognized template revision" diagnostic
  rather than a false pass.
- A Constitution-Check area intentionally marked "N/A with rationale" MUST count as filled (the rule
  is completeness-of-decision, not forcing a non-applicable choice).
- A deprecated structural rule whose removal version has already shipped MUST become a hard failure
  (the deprecation window is closed), not a perpetual warning.
- Trimming prose MUST NOT remove a rule whose gate is only a *graph* `error` vs an *audit* `Fail` —
  both are build-failing; neither is. (Verify the gate actually blocks before deleting prose.)
- The `.gitignore` rule MUST NOT accidentally ignore non-regenerable, intentionally-committed
  readiness evidence (e.g. authored `*.md` notes); scope the ignore to logs/zips only.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The system MUST provide a Constitution-Check completeness validator in
  `FS.Skia.UI.Build` that parses the active feature's `plan.md` **Repository Governance
  Decisions** subsection (the decision-area bullets within the Constitution Check section) and
  returns a typed result identifying any required decision area that is absent or unfilled. The
  canonical set of required decision areas is a **hard-coded typed list of stable area identifiers**
  owned by the validator (adding/removing an area is a deliberate code change + test); the
  `plan-template.md` structure is consulted only to detect an unrecognized template revision
  (FR-003), not to derive the required set. An area is **unfilled** when its body is empty, OR still
  contains the plan template's verbatim boilerplate prompt text (e.g. "Decide whether…"), OR carries
  a NEEDS CLARIFICATION / TODO placeholder.
- **FR-002**: A non-empty set of missing/unfilled Constitution-Check areas MUST cause a build
  failure through an existing escalated gate (e.g. surfaced where plan validation already runs),
  with a diagnostic naming each missing area and the file it was expected in. A complete section
  MUST pass.
- **FR-003**: The Constitution-Check validator MUST treat an area explicitly marked not-applicable
  (with rationale) as filled, and MUST key off the validator's hard-coded stable area identifiers
  (not brittle exact-heading string matching) so a benign template-wording change does not produce a
  false pass or false fail. When the live `plan-template.md` structure no longer maps to the typed
  identifier set, the validator MUST emit an "unrecognized template revision" diagnostic rather than
  a false pass.
- **FR-004**: The generated-product contract (`build/Governance/GeneratedProduct.fs` structural
  checks) MUST carry an explicit `schema_version`.
- **FR-005**: The generated-product contract MUST support a deprecation window: a structural rule
  can be marked deprecated, in which case a violation of *only* that rule emits a warning naming the
  target removal version rather than failing, for at least one contract version before the rule
  becomes a hard requirement or is removed.
- **FR-006**: A machine-readable change log MUST record generated-product contract version
  transitions (rule added / deprecated / promoted-to-required / removed), implemented as **typed
  changelog entries embedded as data in the versioned-contract F# module** under
  `build/Governance/` and surfaced in `GeneratedProductCheck` output (no separate sidecar file to
  keep in sync). The `schema_version` MUST be bumped when a breaking structural rule changes.
  This bump obligation is itself **gate-enforced**, not honour-system: a typed consistency check
  fails when a breaking changelog entry (`PromotedToRequired` / `RuleRemoved`) lacks a matching
  version bump — every such entry's `Version` MUST exceed the prior schema version, and
  `current.SchemaVersion` MUST be ≥ the maximum changelog-entry version (SC-011).
- **FR-007**: Every governance rule that a build-failing gate now enforces (the Stage-6.1 gates
  already shipped, plus the new Constitution-Check gate) MUST be removed from the skill prose
  (`.agents/skills/**`), optionally replaced by a one-line pointer to the enforcing gate; genuine
  rationale/intent/when-to-use guidance MUST be retained.
- **FR-008**: A rule MUST NOT be deleted from prose unless its enforcing gate is demonstrated to
  fail on a seeded violation (the gate-before-prose ordering from the plan's Stage 6 risk
  mitigation).
- **FR-009**: After prose trimming, the `.agents/skills/**` → generated `.claude/skills/**`
  single-source generation (feature 044) MUST remain current — the byte-identity / generation-
  currency check MUST pass.
- **FR-010**: The system MUST record the before/after governance-Markdown line count and the
  per-invocation skill-byte load versus the Stage-0 baseline, with the reproduction command for
  each figure.
- **FR-011**: `.gitignore` MUST exclude future regenerable readiness logs and `readiness*.zip`
  archives, scoped to `specs/*/readiness/logs/**` and `specs/*/readiness/**/readiness*.zip`, so
  every authored non-regenerable readiness file (all `*.md` notes **and** the `fsi-session.txt`
  transcript) stays tracked. The ignore MUST NOT match authored evidence merely because it is not
  `*.md`.
- **FR-012**: Existing committed evidence MUST NOT be removed and git history MUST NOT be rewritten
  (decision D3 — forward-looking ignore only).
- **FR-013**: All new and changed gates MUST be covered by typed unit tests in
  `tests/Governance.Tests/**` that assert typed results (not string matching), including the
  Constitution-Check pass/fail cases and the contract-versioning deprecation-window transitions.
- **FR-014**: The change MUST hold every standing programme invariant (1–6): product public `.fsi`
  surface unchanged, runtime untouched, generated consumers still fully governed, net10/CPM
  conventions, FAKE serialized sequencing, and evidence-output parity.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: `FS.Skia.UI.Build` gains the Constitution-Check validator and the versioned
  generated-product contract; it is a packed/published package (D1) consumed by generated projects,
  so its version is bumped on merge per the normal release flow. No other package identity changes.
  No controls/chart/graph/DataGrid authoring change; no Charts-package migration guidance involved.
- **Public contract impact**: No product `.fsi` signatures, documented public APIs, sample
  contracts, or product surface baselines change. New/changed *build-tooling* `.fsi` modules in
  `build/Governance/**` are curated per Principle II. The **generated-product contract** (a consumer
  contract) changes by gaining `schema_version` + a deprecation window — additive, with a migration
  window by design.
- **State workflow impact**: No product Elmish/MVU runtime, I/O, command, effect, subscription, or
  interpreter behaviour changes. The build-side MEL `update` stays a pure `Msg × Model → Model ×
  Effect list`; any new gate's I/O lives at the `interpret` edge (Principle IV).
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshot, Vulkan,
  Skia, or visual-output change; no unsupported-environment diagnostic change.
- **Evidence obligations**: Required real evidence — typed unit tests for the Constitution-Check
  gate (pass/fail/N-A/template-revision cases) and the contract-versioning deprecation-window
  transitions and the changelog⇄`schema_version` consistency gate (SC-011); a seeded-violation
  proof per deleted prose rule (FR-008); the governance-Markdown
  line-count + skill-byte before/after deltas (FR-010); the 044 generation-currency check green
  after trimming (FR-009); `git check-ignore` proof for the new `.gitignore` rule with a
  previously-committed-evidence-untouched control (FR-011/FR-012); the serialized escalated gate
  logs (`Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` →
  `EvidenceGraph` → `EvidenceAudit`); `EvidenceAudit` `verdict=PASS` with zero synthetic.
- **Unsupported scope**: No visual/screenshot work, no release/publishing beyond the routine
  version bump, no platform/distribution change, no roadmap or V3 modular-package work. The
  Stage-6.1 gates already shipped (`[SEH]` timing, skill-id resolution, surface-baseline presence)
  are **not** re-implemented — only verified still-blocking and used as the basis for prose
  deletion. No rewrite of guidance/rationale prose (bucket (b)) beyond removing code-enforced rules.
- **Build-target impact**: `GeneratedProductCheck` changes (versioned contract + deprecation
  window). Plan/Constitution validation surfaces a new failure mode through an existing escalated
  gate (no new top-level target unless the typed `Targets` registry requires one for the
  Constitution-Check gate; prefer folding into an existing gate per the 044/042 precedent).
  `GeneratedGuidanceCheck` and the 044 generation-currency check are exercised by the prose trim.
  `Dev`, `EvidenceGraph`, `EvidenceAudit`, `TemplateCheck`, `Route` behaviour otherwise unchanged.

## Success Criteria *(mandatory)*

- **SC-001**: Removing or blanking any one required Constitution-Check decision area in the active
  feature's `plan.md` causes a build-failing gate to fail, naming that exact area; a complete
  section passes. Demonstrated live (fail → fix → pass).
- **SC-002**: A generated product violating *only* a deprecated structural rule passes
  `GeneratedProductCheck` with a warning naming the removal version; after the rule is promoted to
  required and `schema_version` bumped, the same product fails. The contract changelog records both.
- **SC-003**: The generated-product contract exposes a `schema_version` discoverable in its output,
  and `GeneratedProductCheck` on a current generated project remains green (consumer contract intact).
- **SC-004**: Every rule deleted from skill prose has an accompanying seeded-violation proof that
  its enforcing gate fails (FR-008); no rule is deleted without one.
- **SC-005**: After trimming, the `.agents` → `.claude` generation-currency check is green and the
  two skill trees are byte-identical (FR-009).
- **SC-006**: The governance-Markdown rule-prose line count is reduced versus the pre-feature count,
  and the reduction (with the per-invocation skill-byte delta) is recorded against the Stage-0
  baseline with reproduction commands. Any rule prose retained as genuine guidance is justified.
- **SC-007**: A newly generated `readiness*.zip` / readiness log under `specs/*/readiness/**` is
  ignored by git (`git check-ignore` proof); a previously-committed evidence file remains tracked
  (control); no history is rewritten.
- **SC-008**: All new/changed gates are covered by typed unit tests asserting typed results;
  `tests/Governance.Tests` is green.
- **SC-009**: No product `.fsi` / surface-baseline / `PackageVersion`-outside-CPM change
  (`PackageSurfaceCheck`/`FsiTranscripts` show no product baseline diff); `git diff` over product
  `src/**` = 0.
- **SC-010**: The full serialized escalated gate sequence passes and `EvidenceAudit` returns
  `verdict=PASS` with zero synthetic evidence; invariants 1–6 hold.
- **SC-011**: A typed consistency test fails when a breaking changelog entry
  (`PromotedToRequired` / `RuleRemoved`) lacks a matching `schema_version` bump (its `Version` ≤
  the prior schema, or `current.SchemaVersion` < the maximum changelog-entry version); the current
  contract passes it. Asserted on typed values in
  `tests/Governance.Tests/GeneratedProductContractTests.fs` (FR-006, FR-013).

## Assumptions

- **A1**: The plan's Stage-6.1 rules `[SEH]` timing, skill-id resolution, and surface-baseline
  presence are already enforced as build-failing gates (verified in `Evidence/Audit.fs`,
  `Evidence/Engine.fs`, `Capabilities.fs`); this feature treats them as done and only verifies they
  still block before deleting their prose. The one remaining un-codified bucket-(a) rule is
  Constitution-Check completeness (FR-001/002/003).
- **A2**: The governance-Markdown corpus the agent reads is ~6,900 lines today (post-044
  single-sourcing), not the plan's headline ~23,000 (which counted `specs/**` and the eliminated
  `.claude`/`.agents` duplication). The "low hundreds" target applies to *rule* prose, not all
  guidance; success is measured as a recorded reduction with retained-guidance justification, not a
  fixed absolute line count.
- **A3**: The generated-product contract version policy follows ADR 0003 (contract versioning,
  written in feature 039); this feature implements that policy, it does not redefine it.
- **A4**: Per the established one-stage-per-feature rhythm (039–045) and the maintainer's selection,
  feature 046 covers all of Stage 6 (FR-001 through FR-012) as a single feature.
- **A5**: Prefer folding new enforcement into existing gates/targets (the 042/044 precedent) over
  adding a new top-level FAKE target, unless the typed `Targets` registry makes a dedicated gate
  cleaner.
- **A6**: This is a governance + generated-product-contract change, so `Route` escalates it to the
  full serialized gate set; it is treated as a dogfood feature that runs the full pipeline for
  itself.

## Out of Scope

- Re-implementing the already-enforced Stage-6.1 gates (`[SEH]` timing, skill-id resolution,
  surface-baseline presence).
- Stage 7 work (interim-scaffolding removal, the final before/after measurement report, the
  new-normal documentation pass, the dogfood retrospective).
- Any product runtime, `.fsi` surface, layout/rendering, or visual change.
- History rewriting or removal of existing committed evidence (D3 — forward-looking `.gitignore`
  only).
- V3 modular-package split or any roadmap/distribution change.
