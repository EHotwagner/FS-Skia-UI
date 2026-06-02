# Phase 1 Data Model: Codify Remaining Rules, Version the Contract

**Feature**: 046-foundations-rule-codification | **Date**: 2026-06-01

All types are **build-tooling scope** (`FS.Skia.UI.Build`, `net10.0`) — NOT part of the
tracked product runtime surface baselines. Validators are **pure functions** returning
these typed records; I/O lives at the `Front` edge (Principle IV). Findings reuse the
existing `Findings.ValidationFinding` (`build/Governance/Findings.fsi`).

---

## US1 — Constitution-Check completeness (`build/Governance/Guidance.fs`)

### `RequiredDecisionArea`
The canonical, hard-coded set (FR-001, R2). 11 stable identifiers (adding/removing one is a
deliberate code change + test).

```fsharp
type RequiredDecisionArea =
    { Id: string            // stable identifier, e.g. "mvu-boundary"
      DisplayName: string }  // human label for diagnostics, e.g. "MVU/effect boundary"
```

Canonical list (order = report order):
`template-ownership`, `dependency-impact`, `command-surface`, `generated-project`,
`evidence-paths`, `fsi-contract`, `mvu-boundary`, `synthetic-evidence`, `test-evidence`,
`observability`, `deferred-scope`.

### `AreaStatus`
```fsharp
type AreaStatus =
    | Filled                       // body has a real decision (incl. explicit N/A-with-rationale)
    | Empty                        // body blank / heading only
    | StillBoilerplate             // body still the template prompt text (Guidance.planGuidancePrompts)
    | PlaceholderUnresolved        // body contains NEEDS CLARIFICATION / TODO
```
*Validation rule (R3):* an area marked **N/A with rationale** → `Filled`. `Empty`,
`StillBoilerplate`, `PlaceholderUnresolved` are all "unfilled" → contribute a finding.

### `ConstitutionCheckResult`
```fsharp
type ConstitutionCheckResult =
    | TemplateRecognized of areas: (RequiredDecisionArea * AreaStatus) list
    | UnrecognizedTemplateRevision of diagnostic: string   // FR-003: live template no longer maps to identifiers
```
*Derivation:* `findings` = for each `(area, status)` where `status ≠ Filled`, a
`ValidationFinding { ArtifactClass = "constitution-check"; Path = <planPath>;
Rule = area.Id; Message = "<DisplayName> is <status>" }`. A non-empty finding list (or the
`UnrecognizedTemplateRevision` case) fails the build through `GeneratedGuidanceCheck`
(FR-002). Empty finding list + `TemplateRecognized` → pass.

*Validator signatures (illustrative, finalized in `.fsi`):*
```fsharp
val requiredDecisionAreas: RequiredDecisionArea list
val classifyConstitutionCheck: planContent: string -> ConstitutionCheckResult
val constitutionCheckFindings: planPath: string -> ConstitutionCheckResult -> ValidationFinding list
```

---

## US2 — Versioned generated-product contract (`build/Governance/GeneratedProductContract.fs`)

### `ContractSchemaVersion` (FR-004)
```fsharp
type ContractSchemaVersion =
    { Major: int; Minor: int }      // semantic-versioning intent (ADR 0003), decoupled from package Version
```
Rendered as `"schema_version: {Major}.{Minor}"` in `GeneratedProductCheck` output (SC-003).

### `RuleLifecycle` (FR-005)
```fsharp
type RuleLifecycle =
    | Required
    | Deprecated of removalVersion: ContractSchemaVersion   // warn, naming removalVersion, until reached
    | Removed
```

### `StructuralRule`
Wraps each existing generated-product structural check with its lifecycle.
```fsharp
type StructuralRule =
    { RuleId: string
      Lifecycle: RuleLifecycle
      Description: string }
```

### `ContractChangelogEntry` (FR-006)
Typed changelog embedded as data — no sidecar file.
```fsharp
type ContractChangeKind = Added | Deprecated | PromotedToRequired | RuleRemoved

type ContractChangelogEntry =
    { Version: ContractSchemaVersion
      RuleId: string
      Change: ContractChangeKind
      Note: string }
```

### `GeneratedProductContract`
```fsharp
type GeneratedProductContract =
    { SchemaVersion: ContractSchemaVersion
      Rules: StructuralRule list
      Changelog: ContractChangelogEntry list }
```

*Evaluation rule (R4, spec US2 + edge cases):* for a generated product that violates
**only** rule `r`:
- `r.Lifecycle = Required` → **failure**.
- `r.Lifecycle = Deprecated removalVersion` AND `SchemaVersion < removalVersion` →
  **warning** naming `removalVersion` (not a failure).
- `r.Lifecycle = Deprecated removalVersion` AND `SchemaVersion >= removalVersion` →
  **failure** (window closed — spec edge case).
- `r.Lifecycle = Removed` → rule not evaluated.
The `SchemaVersion` MUST be bumped, and a `ContractChangelogEntry` added, whenever a rule's
lifecycle changes in a breaking direction (FR-006).

### `RuleOutcome` (FR-005)
The result of `classifyViolation` for a single rule.
```fsharp
type RuleOutcome =
    | Pass
    | Warn of string   // deprecated rule, window open — message names the removal version
    | Fail
```

*Changelog⇄version consistency invariant (R6, FR-006 / SC-011):* a separate pure check over the
`current` contract MUST hold — every breaking `ContractChangelogEntry` (`PromotedToRequired` /
`RuleRemoved`) has a `Version` strictly greater than the prior schema version, and
`current.SchemaVersion >= ` the maximum `Changelog` entry version. A breaking rule change that
forgets the bump fails this typed test rather than relying on reviewer attention.

*Validator signatures (illustrative):*
```fsharp
val current: GeneratedProductContract
val classifyViolation: contract: GeneratedProductContract -> ruleId: string -> RuleOutcome  // Pass | Warn of string | Fail
val renderContractHeader: GeneratedProductContract -> string   // schema_version + changelog summary
```

---

## US3 — Prose-trim measurement (recorded artifact, not a runtime type)

`prose-delta.md` records: per-rule deletion + its seeded-violation proof path (FR-008);
before/after `.agents/skills/**` + `.specify/**` line counts (baseline ≈ 6,882, R5); the
per-invocation skill-byte load; and the reproduction command for each figure (FR-010).
Generation currency (`.agents` → `.claude` byte-identity) asserted by the existing
`SkillSync`/`SkillTreeGen` check (FR-009), no new type.

## US4 — `.gitignore` (no type)

Scoped pattern(s) for regenerable readiness logs + `readiness*.zip` under
`specs/*/readiness/**`, excluding authored `*.md`. Verified by `git check-ignore`
(FR-011/012, SC-007).
