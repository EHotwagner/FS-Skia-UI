# Phase 1 Data Model: Governance Precision Hardening

The "entities" here are typed build-governance values in `FS.Skia.UI.Build` (`build/Governance/**`).
No product/runtime data model is involved. Field-level rules and invariants below are the acceptance
contract for the design.

## `Targets.Target` (single source of gate identity)

The closed DU in `Targets.fs`/`Targets.fsi`. **Additive** changes only (FR-006):

| New case (working name)      | Routable? | `directPrerequisites`                | `timeoutClass` / `cost` / `failureOwner` |
|------------------------------|-----------|--------------------------------------|------------------------------------------|
| `GeneratedProductStructure`  | internal sub-target | `[]`                       | `focused` / `low` / `template`           |
| `GeneratedConsumerValidation`| internal sub-target | `[ GeneratedProductStructure ]` | `medium` / `medium` / `template`    |

- **Rule**: every new case MUST appear in `allTargets` (registry order — appended in a documented
  position adjacent to `GeneratedProductCheck`) so it carries a metadata row and registers in FAKE via
  `dispatchTargets`. `name` and `directPrerequisites` matches are exhaustive ⇒ a new case without an arm
  is a **compile error** (SC-001).
- **Invariant**: `GeneratedProductCheck` remains in the DU and stays a resolvable umbrella (FR-007).

## `TargetSpec`

Unchanged record (`Target`, `Name`, `DirectPrerequisites`, `TimeoutClass`, `Cost`, `FailureOwner`). The
new cases extend `timeoutClass`/`cost`/`failureOwner` matches per the table above. `spec` stays total.

## Routable-gate projection (NEW — FR-003, FR-004, SC-002)

Two pure values in `Targets`, the single source for the previously hand-maintained lists:

- `routableGates : Target list` — the gates a routing rule can require (`Routing.rules` `RequiredGates`)
  plus the composites `Verify`, `Ci`. **Invariant (failing-first test):**
  `routableGates |> List.map name` is set-equal to the prior `AgentValidation.knownGates` literal, and
  renders in a pinned canonical (registry) order.
- `productCheckGates : Target list` — `Verify`'s prerequisites filtered by `isProductCheck` (product-
  facing evidence gates; excludes preflight/pack/internal steps). **Invariant (failing-first test):**
  `productCheckGates |> List.map name` equals the prior `Update.fs:971` `ProductChecksRun` literal
  **byte-for-byte and in order**:
  `[ "Dev"; "PackageSurfaceCheck"; "FsiTranscripts"; "ControlsCatalogCheck"; "ControlsInteractionCheck";
  "ControlsRenderingCheck"; "DependencyReport"; "TemplateCheck"; "GeneratedProductCheck";
  "GeneratedGuidanceCheck"; "TemplateDrift"; "EvidenceAudit" ]`.

Consumers: `AgentValidation.ValidationContract.knownGates = Targets.routableGates |> List.map Targets.name`
(`ValidationGate = string`, type unchanged); `Update.fs` `Verify` verdict
`ProductChecksRun = Targets.productCheckGates |> List.map Targets.name`.

## `FocusedGateContract` (re-keyed — FR-001, FR-002, FR-005)

The per-gate record returned by `Front/Helpers.focusedGateContract` (`TargetName`,
`DirectPrerequisites`, `Command`, `LogPath`, `ReadinessPath`, `StaleAssumptions`, `VerdictCategory`).

- **Signature change**: `focusedGateContract : BuildModel -> Targets.Target -> FocusedGateContract`
  (was `… -> string -> …`). The `match` is over `Targets.Target`, **exhaustive**, **no wildcard**.
- **Arm groups** (R1):
  - *Routable gates* → explicit `VerificationSuccess` contracts (existing arms re-keyed + new explicit
    arms for `ContrastCheck`, `ControlFidelityCheck`, `PerPackageSurfaceDiff`, `SkillContractPathCheck`,
    and the other gates that previously fell through). **Invariant (SC-003)**: no routable gate yields
    `VerificationDegraded`.
  - *Non-routable / internal targets* → a named `internalTargetContract target` helper reproducing the
    **exact** former wildcard value (`Command = $"./fake.sh build -t {name}"`, `LogPath = log
    "{name}.txt"`, `ReadinessPath = None`, `VerdictCategory = VerificationDegraded`). **Invariant**:
    `targetMetadata` output (`target-metadata.json`) is byte-identical to today for these targets.
- **Call sites pass typed cases** (FR-005): `Update.fs` arms pass `Targets.<Case>`; `targetMetadata`
  passes `spec.Target`. A gate rename is a single edit in `Targets.name`.

## `RoutingRule` + matcher refinement (FR-008, FR-009, FR-010)

Record unchanged (`Id`, `Paths`, `Matches`, `Tier`, `RequiredGates`, `ExpectedArtifacts`, `TimeoutClass`,
`FailureOwner`). The change is in **how `Matches` is constructed** for the broad source rules and in two
new rules:

| Rule (Id)                  | Paths (doc-exclusion applied to source rules)         | Tier             | RequiredGates (effective)                          |
|----------------------------|-------------------------------------------------------|------------------|----------------------------------------------------|
| `controls-public-surface`* | `src/Controls/**` **but matcher requires ≥1 non-doc** | FocusedAuthority | heavy controls set incl. `GeneratedProductCheck` (unchanged) |
| `generated-template`*      | `template/**` etc. **matcher requires ≥1 non-doc**    | FocusedAuthority | `TemplateCheck; GeneratedProductCheck; SkillContractPathCheck` (unchanged) |
| `controls-docs` (NEW)      | `src/Controls/**/*.md` (excl. `skill/SKILL.md`)       | FocusedAuthority | **pinned** `[ EvidenceGraph ]` — no heavy gates, no `Dev` |
| `template-docs` (NEW)      | `template/**/*.md` (excl. `skill/SKILL.md`, README routed elsewhere) | FocusedAuthority | **pinned** `[ EvidenceGraph ]` — no heavy gates, no `Dev` |

`*` = matcher refined to "matches its heavy gates only when the diff has a non-doc path under the tree."

- **Conservative exclusion (FR-009)**: `build.fsx`, `scripts/build/**`, `validation.contract.yml`,
  `.specify/**`, `build/Governance/**` are **never** relaxed — their existing rules match `.md` and `.fs`
  identically.
- **Composition invariants** (unchanged engine, asserted by tests):
  - doc-only diff under a relaxed tree → light gate set; **excludes** `GeneratedProductCheck` (SC-004).
  - doc+source mixed diff → heavy rule matches (non-doc present) ⇒ full set, **unchanged** from today.
  - pure source/contract diff → full set, **unchanged** from today (SC-004 second clause).
  - max-tier / union-of-gates / registry-order dedup (`internalDedupInRegistryOrder`) all preserved.

## `GeneratedProductCheck` umbrella + sub-target family (FR-006, FR-007, SC-005)

- **Structural sub-target** `GeneratedProductStructure`: emits `GenerateV3Products`,
  `ScanV3GeneratedProducts`, `RequireFiles` over the five `*-source.txt`/`*-package.txt` file-lists.
  Independent (`directPrerequisites = []`); fails fast before any consumer validation.
- **Consumer sub-target** `GeneratedConsumerValidation`: emits `ValidateGeneratedConsumer`, `RequireFiles`
  over `GeneratedProductValidationPath`. Depends on `GeneratedProductStructure`.
- **Umbrella** `GeneratedProductCheck`: `directPrerequisites` gains both sub-targets; produces the
  **identical** set of evidence artifacts (all five file-lists + `GeneratedProductValidationPath`) and the
  **identical** verdict (FR-007). **Invariant (SC-005)**: umbrella evidence/verdict byte-identical to the
  pre-split run; the structural target can run and fail independently of and before the consumer step.
- Reuses the **existing** effects (`GenerateV3Products`/`ScanV3GeneratedProducts`/
  `ValidateGeneratedConsumer`) — no new `Effect`/`Msg` constructor; `Msg.StartTarget of Targets.Target`
  carries the new cases automatically.

## Extracted scan validators (FR-011, FR-012, FR-013)

- Shared pure helpers extracted from `scanGeneratedRow` and `scanV3GeneratedRow`: **file enumeration**
  (root walk + bin/obj/readiness filtering) and **forbidden-path / required-file validation**. The two
  callers keep distinct row shapes and finding sets; only common validators are shared.
- Paired NuGet-config templates consolidated to one rendered source.
- **Invariant (SC-006/FR-013)**: byte-identical scan findings (five file-lists +
  `GeneratedProductValidationPath`) and governance goldens vs. a captured pre-refactor baseline; **no**
  `.fsi` / `validation.contract.yml` change for Tier 3.
