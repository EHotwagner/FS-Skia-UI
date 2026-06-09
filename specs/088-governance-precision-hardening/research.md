# Phase 0 Research: Governance Precision Hardening

All NEEDS CLARIFICATION items are resolved here. Each decision is grounded in the confirmed working
surfaces (see plan.md §Technical Context).

## R1 — Typed `focusedGateContract` exhaustiveness without a silent wildcard (FR-001, FR-002, SC-001, SC-003)

**Decision.** Re-key `Front/Helpers.focusedGateContract` from `match target (string)` to
`match target (Targets.Target)`. Remove the `_ -> { … VerdictCategory = VerificationDegraded }`
wildcard. Every `Target` case gets an explicit arm via two groups:

1. **Routable gates** — keep their current success contracts, now reached by typed case
   (`Targets.PackageSurfaceCheck`, `Targets.GeneratedProductCheck`, …), and **add explicit arms** for
   the gates that today fall through to the degraded wildcard but are genuinely routable
   (`ContrastCheck`, `ControlFidelityCheck`, `PerPackageSurfaceDiff`, `SkillContractPathCheck`,
   `DesignTokenDrift`, `ControlsCatalogGenerationCheck`, `ControlsCatalogDocsCheck`,
   `SkillQualityCheck`, `PhaseHookParityCheck`, `TemplateUpdateSkillPackageCheck`, `SymbolCrossCheck`,
   `TargetMetadataDrift`, `PrePublishCheck`, `Publish`, …). Their contract is constructed the same way
   the wildcard built one (`Command = ./fake.sh build -t <name>`, `LogPath = log "<name>.txt"`) but with
   `VerdictCategory = VerificationSuccess` and a real `ReadinessPath` where one exists — making the
   verdict explicit and correct rather than silently degraded (SC-003).
2. **Non-routable / internal targets** — `Clean`, `Restore`, `Build`, `Test`, `PackLocal`,
   `RefreshSurfaceBaselines`, `SampleContractSmoke`, the `Template*` build steps, `CapabilityCheck`,
   `SkillCheck`, `AgentReady`, `TargetMetadata`, `VerifyPreflight`, `CiPreflight`, `StaleBoundaryScan`,
   `FinalReadiness`, `Verify`, `Ci`, `Route`, `PackageSmoke`, `BuildWorkflowCheck`. These are reachable
   only via `targetMetadata` (which builds a metadata row for every `allTargets` entry), not as focused
   gates. They map to an **explicit named helper** `internalTargetContract target` that reproduces
   today's wildcard contract value (so `targetMetadata` output is byte-identical) but is named, not a
   silent `_`. The exhaustive match still forces every future `Target` case into one group or the other
   — adding a case without classifying it is a **compile error** (SC-001).

**Rationale.** FR-001 demands a compile error on an unwired case; an open wildcard defeats that. Two
named groups preserve the exact current behavior for non-gates (byte-identical `targetMetadata`) while
upgrading routable gates to explicit verdicts (SC-003). The compiler, not a new drift gate, does the
enforcement (the spec's stated highest-worth property).

**Call sites (FR-005).** `Engine/Update.fs` arms pass typed cases:
`focusedGateAssumptionCheck model Targets.GeneratedProductCheck` instead of the literal
`"GeneratedProductCheck"`. `Helpers.targetMetadata` passes `spec.Target` (it already has the typed
`spec`), not `spec.Name`. `focusedGateContractByName` is **not** introduced; a rename becomes a single
edit in `Targets.name` (SC-001 single-source).

**Alternatives considered.** (a) Keep the string match but add a `TargetMetadataDrift`-style currency
gate over the arm set — rejected: re-introduces hand-sync the compiler could enforce. (b) A
`Map<Target, Contract>` built once — rejected: a `Map` is non-exhaustive (a missing key is a runtime
`KeyNotFound`/`None`, not a compile error), defeating FR-001.

## R2 — Routable-gate projection as the single source for `knownGates` and `ProductChecksRun` (FR-003, FR-004, SC-002)

**Decision.** Add to `Targets` a pure **routable-gate projection** derived from the DU, and derive both
hand-maintained lists from it:

- `routableGates : Target list` — the targets that appear as `RequiredGates` in `Routing.rules` plus the
  composite gates (`Verify`, `Ci`). This reproduces `AgentValidation.knownGates` exactly. Validated by a
  failing-first test asserting `routableGates |> List.map name = <prior knownGates literal>` (order
  preserved — `knownGates` is alphabetical-ish; the projection sorts/orders to match, or the prior
  literal is reordered into registry order with the test pinning the new canonical order — chosen:
  **registry order**, since `Route` already dedups into `Targets.allTargets` order, and the test asserts
  set-equality plus a pinned rendered order).
- `productCheckGates : Target list` — the curated subset `Verify`'s verdict reports as `ProductChecksRun`
  (`Dev; PackageSurfaceCheck; FsiTranscripts; ControlsCatalogCheck; ControlsInteractionCheck;
  ControlsRenderingCheck; DependencyReport; TemplateCheck; GeneratedProductCheck; GeneratedGuidanceCheck;
  TemplateDrift; EvidenceAudit`). This is **not** identical to `Verify`'s `directPrerequisites` (which
  also includes `VerifyPreflight`, `PackLocal`, `SampleContractSmoke`, `CapabilityCheck`, `SkillCheck`,
  `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`, `TargetMetadataDrift`). Resolution: define
  `productCheckGates` as `Verify`'s prerequisites **filtered** through a documented predicate
  `isProductCheck` (the gates that produce product-facing evidence, excluding preflight/pack/internal
  steps), ordered to reproduce the literal. A failing-first test pins
  `productCheckGates |> List.map name = <prior ProductChecksRun literal>` byte-for-byte.

`AgentValidation.knownGates` becomes `Targets.routableGates |> List.map Targets.name`
(`ValidationGate = string`, so the type is unchanged). `Update.fs:971` `ProductChecksRun` becomes
`Targets.productCheckGates |> List.map Targets.name`. `Ci`'s `ProductChecksRun = [ "Verify" ]` stays a
single composite literal (or `[ Targets.name Targets.Verify ]`) — trivially single-sourced.

**Rationale.** SC-002 requires zero hand-maintained gate-name string lists for these two; both become
projections of `Targets`. The byte-identity tests (equal to prior literals) satisfy FR-003/FR-004's "no
behavior change" and the edge case that not every `Target` is routable (we project, not dump the raw DU).

**Alternatives considered.** Deriving `productCheckGates` directly from `directPrerequisites Verify`
without a filter — rejected: it would change the reported list (adds preflight/pack/internal), violating
byte-identity. The documented `isProductCheck` filter is the minimal honest projection.

## R3 — Doc-only routing relaxation via matcher refinement, not glob negation (FR-008, FR-009, FR-010)

**Problem.** `Routing.select` composes matched rules by **union of `RequiredGates`** and **max tier**.
The broad rules `generated-template` (`template/**`) and `controls-public-surface` (`src/Controls/**`)
already match `.md` files, so a *new* narrow doc-only rule cannot *remove* the heavy gates they
contribute — union only adds.

**Decision.** Refine the heavy rules' **match semantics** so they match only when the diff contains at
least one **non-doc** path under their tree, and add a dedicated **doc-only** rule for the doc paths:

- Introduce `internalRuleMatcherExcludingDocs (treePatterns) (docPatterns)` (or an `internalSourceRule`
  helper): the predicate is "some changed path matches a tree pattern **and is not** a doc path." Doc
  paths = `**/*.md` (and `docs/img/**` for image-only edits under the controls tree, if applicable),
  excluding `**/skill/SKILL.md` and `**/README.md` already routed by `skill-quality`/`generated-guidance`
  (those keep their own rules and tiers — no weakening).
- Add doc-only rules: `template-docs` (`template/**/*.md` minus the skill/guidance paths) → a
  documentation-appropriate minimal set (e.g. `[ Targets.EvidenceGraph ]`, matching the existing
  `docs-only` rule's lightness) at `FocusedAuthority`; `controls-docs` (`src/Controls/**/*.md`) → likewise
  minimal. These intentionally **exclude** `GeneratedProductCheck`/`TemplateCheck`/the heavy controls set.
- **Conservative exclusions (FR-009):** `build.fsx`, `scripts/build/**`, `validation.contract.yml`,
  `.specify/**`, and all `build/Governance/**` paths are **not** relaxed — their existing rules
  (`build-target-contract`, `specify-catchall`, `generated-guidance`) keep matching `.md` and `.fs`
  alike. A comment in build infrastructure can change build semantics, so doc-only relaxation never
  applies there.

**Mixed-change correctness (edge case).** Because composition is union/max-tier and the refined heavy
rule matches whenever *any* non-doc path is present, a commit touching both `a.md` and `b.fsi` under
`src/Controls/**` still triggers `controls-public-surface` (the `.fsi` is non-doc) **and**
`package-surface`, re-escalating to the full set. Relaxation only takes effect for a **purely** doc diff.
A property test asserts: doc-only diff → light set (no `GeneratedProductCheck`); doc+source diff → full
set unchanged; pure source diff → full set unchanged.

**FR-010 (dependency-chain tightening).** Treated as **opportunistic and optional** (`SHOULD`). Candidates:
`TemplateSmoke -> [TemplateInstantiate; Test]` and `GeneratedProductCheck -> [CapabilityCheck; SkillCheck;
Dev; TemplateCheck]`. Any tightening proceeds **only** if a coverage-equivalence argument is written
(the removed edge's work is already transitively guaranteed or genuinely unconsumed by the body);
otherwise it is **deferred**, not risked. The default plan keeps these chains as-is and revisits during
implementation with the six-target order as the regression oracle.

**Rationale.** The matcher refinement keeps the rendered `paths:` view honest (the `Paths` list still
drives the contract; the *predicate* gains a doc-exclusion clause that the contract renderer documents),
needs no glob-negation grammar in `internalGlobToRegex`, and leans on the proven union/max-tier
composition for the mixed-change guarantee.

**Alternatives considered.** (a) Add `!`-negation to the glob engine — rejected: larger, riskier change
to a shared primitive; the `paths:` rendering would need negation semantics too. (b) Post-filter the
composed gate set to drop heavy gates when the diff is doc-only — rejected: hides the decision in
`select` rather than in named rules, and is brittle against future heavy gates.

## R4 — `GeneratedProductCheck` split seam (FR-006, FR-007, SC-005)

**Decision.** Add two additive `Target` cases (working names) and recompose:

- `GeneratedProductStructure` — cheap structural sub-target. `StartTarget` arm emits
  `GenerateV3Products; ScanV3GeneratedProducts; RequireFiles("generated product file-list reports", [ …
  the five `*-source.txt`/`*-package.txt` file-list paths … ])` + its focused-gate summary. No consumer
  restore/build. `directPrerequisites = []` (independent; fails fast).
- `GeneratedConsumerValidation` — expensive sub-target. Arm emits `ValidateGeneratedConsumer;
  RequireFiles("generated consumer validation report", [ model.GeneratedProductValidationPath ])` + its
  summary. `directPrerequisites = [ GeneratedProductStructure ]` so structure runs first and the
  consumer step never runs on a structurally-broken generation (SC-005 fail-fast + ordering).
- `GeneratedProductCheck` (umbrella) — `directPrerequisites` gains the two sub-targets; its `StartTarget`
  arm no longer emits the raw effects itself but composes/aggregates so the **same** evidence artifacts
  (all five file-lists + `GeneratedProductValidationPath`) and the **same** verdict are produced
  (FR-007). The umbrella keeps `CapabilityCheck; SkillCheck` (and today's `Dev; TemplateCheck` via the
  metadata `directPrerequisites`) as before — effective coverage unchanged.

Routing rules and `Verify` keep referencing `Targets.GeneratedProductCheck` (the umbrella) — no
downstream reference breaks (edge case in spec). `Targets.fs` updates: DU cases, `name`,
`directPrerequisites`, `allTargets` (registry order — append after `GeneratedProductCheck` or in a
documented position; `TargetMetadataDrift` enforces the regenerated `validation.contract.yml` matches).
`timeoutClass`/`cost`/`failureOwner` classify the structural sub-target as `focused`/`low`/`template` and
the consumer sub-target as `medium`/`medium`/`template` (matching today's `GeneratedProductCheck`
`medium` split between cheap/expensive halves). The new cases register automatically via
`Targets.dispatchTargets` in `build/Program.fs`.

**Rationale.** Reuses the three existing effects unchanged; the split is purely in `Targets` (identity +
deps) and `Update.fs` (which arm emits which effect). The umbrella-as-prereq-composition keeps the
public target name resolvable and its evidence identical (SC-005). Because `allTargets` order changes,
`validation.contract.yml` / `target-metadata.json` regenerate — an **intentional** Tier 2 diff with
rationale (distinct from Tier 1/3 byte-identity).

**Alternatives considered.** Making `GeneratedProductCheck` literally re-emit all effects *and* depend on
the sub-targets — rejected: double-runs the scans. Composition via prerequisites runs each effect once.

## R5 — Tier 3 behavior-preserving extraction (FR-011, FR-012, FR-013, SC-006)

**Decision.** Extract the overlap between `scanGeneratedRow` (`GeneratedProduct.fs:148`) and
`scanV3GeneratedRow` (`:1010`) into shared, pure helpers — at minimum **file enumeration** (both walk
`Directory.EnumerateFiles(row.Root, "*", AllDirectories)` with bin/obj/readiness filtering) and
**package-reference / forbidden-path validation** (both pin a `forbidden`/`forbiddenFrameworkPaths` list
and a `missing`/required-file list and emit findings). The two callers keep their distinct row shapes and
distinct finding sets; only the common validators are shared. Consolidate the **paired NuGet-config
templates** (FR-012) into one source rendered twice. **Acceptance is byte-identity** (FR-013/SC-006): a
pre-refactor baseline of the five file-list reports + `GeneratedProductValidationPath` + the governance
golden outputs is captured, and the post-refactor run must reproduce them byte-for-byte, with **no**
`.fsi`/`validation.contract.yml` change. Large re-splits and table-driven dispatch are **out of scope**
(FR-014) and explicitly deferred.

**Rationale.** Lowest-leverage, highest-churn tier; the only safe acceptance signal is byte-identity
against a captured baseline, so the work is bounded to the named duplication with that oracle.

**Alternatives considered.** Decomposing `GeneratedProduct.fs` by domain now — rejected by FR-014 as a
large speculative rewrite; deferred to a follow-up if requested.

## Cross-cutting: independent shippability (SC-007)

Each tier is a self-contained mergeable slice. Tier 1 touches `Helpers.fs`/`AgentValidation.fs`/`Update.fs`
call sites/`Targets` projection and passes all routed gates with no Tier 2/3 present (byte-identical
contract). Tier 2 adds sub-targets + routing rules (intentional contract diff) independent of Tier 3.
Tier 3 is a pure refactor independent of Tier 1/2. Task ordering (Phase 2) keeps the tiers separable so
any one can land first.
