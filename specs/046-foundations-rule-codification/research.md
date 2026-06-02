# Phase 0 Research: Codify Remaining Rules, Trim Prose, Version the Contract

**Feature**: 046-foundations-rule-codification
**Date**: 2026-06-01

All open questions were resolved in the spec's **Clarifications / Session 2026-06-01**
and the foundations programme decisions (D1–D6, ADR 0003). There are **no remaining
NEEDS CLARIFICATION**. This document consolidates the decisions that shape the design,
each grounded in the actual code paths confirmed during reconnaissance.

---

## R1 — Host gate for the Constitution-Check completeness validator

- **Decision**: Fold the new validator into the **existing `GeneratedGuidanceCheck`**
  gate via `Guidance.runGeneratedGuidanceScan` (`build/Governance/Guidance.fs:1`,
  surfaced at `build/Governance/Front/Helpers.fs:255`). Do **not** add a new top-level
  FAKE target.
- **Rationale**: `Guidance.fs` already (a) owns a `markdownSections` parser
  (`Guidance.fs:187`), (b) validates the `plan-template.md` prompt structure
  (`validateGuidanceTemplate`, `Guidance.fs:232`), and (c) aggregates `ValidationFinding`s
  into one byte-identical report + `failwithf`. This is literally "where plan validation
  already runs" (FR-002), and matches A5 / the 042/044 precedent of folding new
  enforcement into existing gates. `GeneratedGuidanceCheck` is already in the escalated
  serialized set, so escalation/Route behaviour is unchanged.
- **Alternatives considered**:
  - *New `ConstitutionCheck` top-level target* (the Explore recon's first suggestion) —
    rejected: adds a target, a registry slot, `validation.contract.yml` churn, and a new
    `Route` gate for no benefit; violates A5.
  - *Preflight.fs* — rejected: Preflight is process-health/bootstrap only
    (`Preflight.fsi`), not guidance/plan parsing.

## R2 — Source of truth for the required Constitution-Check decision areas

- **Decision** (spec clarification): A **hard-coded typed list of stable area
  identifiers** in the validator owns the canonical required set. The live
  `plan-template.md` structure is read **only** to detect an unrecognized template
  revision and emit that diagnostic. Adding/removing an area is a code change + test.
- **The 11 required areas** (from the plan template's *Repository Governance Decisions*,
  `plan.md:21–60`): `template-ownership`, `dependency-impact`, `command-surface`,
  `generated-project`, `evidence-paths`, `fsi-contract`, `mvu-boundary`,
  `synthetic-evidence`, `test-evidence`, `observability`, `deferred-scope`.
- **Rationale**: Keying off stable identifiers (not exact headings) survives benign
  template-wording changes (edge case in spec); a renamed/removed template area trips the
  "unrecognized template revision" diagnostic (FR-003) instead of a false pass.
- **Alternatives considered**: *Derive the required set from the live template* — rejected
  by the clarification: it makes the rule self-weakening (delete an area from the template
  and the gate silently stops requiring it).

## R3 — "Unfilled" detection

- **Decision** (spec clarification): An area is **unfilled** if its body is empty, OR
  still contains the template's verbatim boilerplate prompt text (e.g. "Decide
  whether…"), OR carries a `NEEDS CLARIFICATION` / `TODO` placeholder. An area explicitly
  marked **N/A with rationale** counts as **filled** (FR-003 edge case).
- **Rationale**: Completeness-of-decision, not forcing a non-applicable choice. The
  boilerplate-prompt strings are already enumerated in `Guidance.planGuidancePrompts`
  (`Guidance.fs:46`) — reuse them as the "still-boilerplate" sentinels so there is one
  source for the prompt text.
- **Implementation note**: parse with the existing `markdownSections` +
  `tryHeading`/`trySection` helpers (`Guidance.fs:174,214`); the per-area body is the text
  between its bullet and the next bullet/section.

## R4 — Generated-product contract version + deprecation window + changelog

- **Decision** (spec clarification + ADR 0003): The contract version, deprecation
  metadata, and change log are **typed values embedded as data in a versioned-contract F#
  module under `build/Governance/`**, surfaced in `GeneratedProductCheck` output. **No
  separate sidecar file.** A structural rule carries a lifecycle state
  (`Required | Deprecated{removalVersion} | Removed`); a `Deprecated` rule violated *in
  isolation* emits a **warning** naming the removal version instead of failing, until its
  removal version ships (then it hard-fails — spec edge case).
- **Rationale**: ADR 0003 mandates an explicit contract version decoupled from the package
  marketing version, additive-by-default with deprecate-then-remove. Embedding the version
  + changelog as typed data (vs YAML/JSON sidecar) follows D6 (compiled F# config,
  build-time-checked, no drift) and the clarification ("no separate file to keep in sync;
  covered by typed-result tests"). The current checks live in `GeneratedProduct.fs`
  (`runScanV3GeneratedProducts`); the new model wraps each structural rule with its
  lifecycle state rather than rewriting the ~800 lines of checks.
- **Alternatives considered**:
  - *Schema version in `template/capabilities.yml`* (Explore recon suggestion) — rejected:
    contradicts the clarification (typed F# data, not YAML) and D6.
  - *Separate `changelog.json` sidecar* — rejected by the clarification (drift risk).

## R5 — Prose trim: gate-before-prose ordering

- **Decision**: A rule's prose is deleted from `.agents/skills/**` **only after** its
  enforcing gate is demonstrated to fail on a seeded violation (FR-008). The three
  Stage-6.1 rules (`[SEH]` timing → `Evidence/Audit.fs`; skill-id resolution →
  `Evidence/Engine.fs`; surface-baseline presence → `Capabilities.fs:124`) are already
  build-failing; the new Constitution-Check gate is the fourth. After each deletion the
  `.agents` → `.claude` currency check must stay green.
- **Rationale**: The plan's Stage-6 risk mitigation ("only delete a prose rule once its
  code gate exists and is proven to fail, rule by rule"). The currency check is
  `SkillSync` / `SkillTreeGen` (`build/Governance/SkillSync.fs`,`SkillTreeGen.fs`),
  surfaced through `GeneratedGuidanceCheck`/the skill-sync gate; `RefreshSurfaceBaselines`
  regenerates `.claude` byte-identically from `.agents`.
- **Measurement** (FR-010): baseline is `.agents/skills/**` ≈ 4,065 lines + `.specify/**`
  ≈ 2,817 lines ≈ **6,882** rule/guidance Markdown lines today (post-044), **not** the
  plan's overstated ~23,000 (which counted `specs/**` + the eliminated `.claude`/`.agents`
  duplication — see spec A2). Record before/after line count and per-invocation skill-byte
  load with the reproduction command (`find .agents/skills -name '*.md' | xargs wc -l`).

## R6 — `.gitignore` evidence hygiene (D3, forward-looking only)

- **Decision**: Add scoped ignore patterns for **future** regenerable readiness logs and
  `readiness*.zip` archives under `specs/*/readiness/**`. Scope to logs/zips so authored
  `*.md` notes stay tracked. **No** removal of committed evidence, **no** history rewrite
  (D3 / FR-012).
- **Rationale**: The current `.gitignore` already ignores
  `specs/*/readiness/generated-consumer-validation/nuget-packages/` but not the
  regenerable logs/zips; D3 resolved this to a one-line forward-looking edit. Verify with
  `git check-ignore` on a freshly generated zip, with a previously-committed evidence file
  as the still-tracked control (SC-007).
- **Alternatives considered**: *Leave `.gitignore` unchanged* (Explore recon suggestion) —
  rejected: directly contradicts FR-011/SC-007, which require the new ignore rule.

## R7 — Testing approach

- **Decision**: Typed-result Expecto tests in `tests/Governance.Tests/**` assert the
  returned `ValidationFinding`/typed records, **never** string matching (FR-013). New
  files follow the `{Module}Tests.fs` convention (e.g. `ConstitutionCheckTests.fs`,
  `GeneratedProductContractTests.fs`), registered in `Program.fs` / discovered by the
  Expecto runner; fixtures under `tests/Governance.Tests/fixtures/`.
- **Rationale**: Matches the established pattern (`ConstitutionFragmentsTests.fs`,
  `GeneratedProductValidatorTests.fs`, `CapabilityCatalogTests.fs`) and Principle VI. The
  Constitution-Check validator is a pure parser → can be unit-tested with no build run
  (spec US1 "no build run required to test"); the live gate run on this feature's own
  `plan.md` is the integration evidence.
