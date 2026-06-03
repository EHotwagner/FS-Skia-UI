# Feature Specification: Single-Source the Duplicated Governance Corpus

**Feature Branch**: `057-dedupe-governance-corpus`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "create specs for the invasive structural rewrite — the next lever is structural, not editorial: collapse those scanner-driven cross-file duplications and the constitution/template/fragment triple-maintenance. But that means changing Guidance.fs and the scanners (which files must carry which tokens)."

## Why this feature exists

Feature 056 tightened the governance prose corpus editorially and reached its
ceiling at a ~1.7% reduction (6889 → 6772 lines). The post-mortem found that the
corpus's real bloat is **structural duplication that a prose pass cannot touch**,
because each duplicated copy is independently load-bearing for a validator:

1. **Per-file contract-token carriage.** `build/Governance/Guidance.fs` requires
   every `ContractToken` to appear verbatim in *each* file of its `Files` list —
   e.g. `[SEH]` and `synthetic-error-handling-approved` are hand-carried in eight
   files; the controls tokens across fragment READMEs, base docs, and `src`.
   Changing one token means editing every home file by hand.
2. **Per-file obligation anchors.** Each `GuidanceObligation`'s `AllOf`/`AnyOf`
   concept phrases must be present in every home file, so the same phrase is
   restated across the template/preset/command/memory copies.
3. **In-file scanner echoes.** Some files carry a *literal second copy* of a list
   they already contain, solely to satisfy a substring scanner — e.g. the
   skill-mapping list appears in `tasks-template.md` and again as the
   "Exact skill phrases for scans:" line, and a third time in the `speckit-tasks`
   SKILL plus its command twin.
4. **Constitution triple-maintenance.** `constitution.md` and the two
   `constitution-template.md` twins hand-maintain near-identical principle prose;
   only the *first sentence* of Principles II/IV/V/VI is auto-generated today (via
   `ConstitutionFragments` splicing into the plan/tasks templates).

The repository already proves the fix pattern works: `.claude/skills/**` is
**generated** from `.agents/skills/**` (one source, byte-identical copies,
`SkillSyncCheck`-guarded), `validation.contract.yml` is generated from
`Routing.fs` (`TargetMetadataDrift`-guarded), and constitution principle fragments
are spliced and currency-checked. This feature **extends that single-source-and-
generate pattern to the four duplication classes above** so each token, phrase,
and principle has exactly one canonical source and the per-file copies are
generated and currency-checked — never hand-synced.

Unlike 056, this feature **does** change `build/Governance/Guidance.fs` and the
scanners (which files must carry which tokens) and the generation/currency
machinery. That is the invasive part, and the reason drift-detection strength is
the dominant risk this spec is written to protect.

## Clarifications

### Session 2026-06-03

- Q: For the per-file token/obligation duplications, what is the intended
  end-state after single-sourcing? → A: Hybrid by consumer — where the scanner
  runs in-repo, delete the copy and have the scanner read the canonical source
  directly; keep a generated + currency-checked copy only where the consumer is a
  shipped template-owned or `.agents`/`.claude` file that cannot reach the source.
- Q: Which artifact becomes the single canonical source of constitution principle
  prose after the collapse? → A: A placeholder-bearing fragment/data file is
  canonical; both the repo's `constitution.md` (placeholders substituted) and the
  two `constitution-template.md` twins (placeholders preserved) are generated from
  it and currency-checked, extending the existing `ConstitutionFragments` splice.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Change a governed rule in exactly one place (Priority: P1)

A framework maintainer needs to add, reword, or remove a governed token,
obligation phrase, or constitution principle. Today they must locate and edit
every home file that carries the duplicate and keep the copies in lockstep by
hand; a missed copy is a drift bug. After this feature, they edit the **single
canonical source** and run the regeneration target; every derived copy updates
identically and a currency gate proves they match.

**Independent test**: change a contract token's canonical definition (and a
constitution principle's body), run the regeneration target, and confirm (a)
every derived home-file copy now reflects the change, (b) no home file was
hand-edited, and (c) `GeneratedGuidanceCheck` plus the generation-currency gates
are green.

### User Story 2 - Drift detection keeps full 055/056 strength (Priority: P1)

A reviewer must be certain that single-sourcing did **not** open a silent drift
hole. Every negative case that failed before must still fail, and a **new** class
of failure must be caught: a hand-edited *generated* copy that diverges from its
canonical source.

**Independent test**: reproduce the 056 red→green proof (delete an obligation
concept, remove a contract token, reintroduce a forbidden term — each still
fails, then reverts to green) **and** add a generated-copy-drift case: hand-edit a
generated copy so it no longer matches its source and confirm a currency gate
fails naming the file and its source, then regenerate and observe green.

### User Story 3 - A real structural reduction, honestly accounted (Priority: P2)

A maintainer wants the corpus measurably smaller than 056 achieved, with the
saving coming from *collapsed duplication* rather than dropped content. The
in-file scanner echoes and the triple-maintained constitution prose collapse to
single sources, and the line delta and per-rule maintenance-surface reduction are
reported with reproduction commands.

**Independent test**: measure the corpus line count before and after, confirm a
material reduction beyond 6772 attributable to removed duplication (not removed
rules), and confirm the count of files that must be edited to change a sample
rule dropped from N to 1 (+ regeneration).

### User Story 4 - Generated consumers stay correct (Priority: P2)

Codex and Claude agents, and generated `dotnet new fs-skia-ui` projects, must keep
reading correct governance content. The `.agents`↔`.claude` peers, the
template-owned files, and any newly generated copies stay valid and synchronized.

**Independent test**: `SkillSyncCheck` and `TemplateDrift` are green; a generated
project still receives correct, non-stale governance guidance.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The feature MUST catalogue every structural-duplication instance in
  the governed corpus across the four classes (per-file token carriage, per-file
  obligation anchors, in-file scanner echoes, constitution/template/fragment
  triple-maintenance), each traced to the validator that currently requires it.
- **FR-002**: Each catalogued duplication MUST be reduced to a **single canonical
  source** whose derived copies are **generated** (extending the existing
  `RefreshSurfaceBaselines` / `ConstitutionFragments` / contract-generation
  pattern), not hand-maintained.
- **FR-003**: Every generated copy MUST be covered by a generation-currency check
  that **fails** when the copy diverges from its source — there MUST be no
  generated artifact without a guarding currency gate (no silent drift hole).
- **FR-004**: The full set of governed rules, obligations, and forbidden-absences
  MUST be preserved; `build/Governance/Guidance.fs` MUST remain the single home of
  the rule set. The feature changes **how** tokens/phrases are carried and
  generated, not **which** rules exist.
- **FR-005**: Drift detection MUST retain full strength: a deleted obligation
  concept, a removed contract token, and a reintroduced forbidden term MUST each
  still fail a gate with a file+rule diagnostic, **and** a generated copy edited
  out of sync with its source MUST fail a currency gate.
- **FR-006**: Where a duplication exists only to satisfy a substring scanner, it
  MUST be single-sourced by **consumer location**: where the scanner runs in-repo,
  the copy is removed and the scanner reads the canonical source directly; where
  the consumer is a shipped template-owned or `.agents`/`.claude` file that cannot
  reach the source, a generated copy is written and currency-checked against the
  source. No independent hand-carried echo remains in either case.
- **FR-007**: The constitution/template/fragment triple-maintenance MUST collapse
  to one canonical principle source — a **placeholder-bearing fragment/data file**
  — from which both the repo's `constitution.md` (with placeholders substituted)
  and the two `constitution-template.md` twins (with placeholders preserved) are
  generated and currency-checked, extending the existing `ConstitutionFragments`
  splice rather than introducing a new generation framework.
- **FR-008**: The change MUST route through `Route`'s escalated governance path
  and keep `validation.contract.yml` current; if `Routing.fs` is unedited it MUST
  stay byte-identical, otherwise it MUST be regenerated and currency-checked.
- **FR-009**: The feature MUST report the achieved structural reduction (corpus
  line delta with reproduction commands) **and** the maintenance-surface reduction
  (files-touched-per-rule-change, before vs after), with no fixed target line
  count and no discredited historical figure.
- **FR-010**: Generated consumers MUST stay correct: the `.agents`↔`.claude`
  peers, template-owned files, and any new generated copies remain valid and
  synchronized (`SkillSyncCheck`, `TemplateDrift` green; generated projects
  receive non-stale guidance).
- **FR-011**: The feature MUST distinguish *genuine* per-file duplication (the
  same content required identically in N files, eligible for single-sourcing) from
  content that legitimately differs per file, and only single-source the former.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, content, or version change; no
  controls, chart, graph, or DataGrid authoring change; no legacy Charts package
  migration. This is a governance-tooling and governance-prose change only.
- **Public contract impact**: No product `.fsi` signature, documented public API,
  sample contract, or surface baseline changes. The *governance* contract (which
  files must carry which tokens, and how copies are generated) changes
  deliberately inside `build/Governance/**`; that is an internal governance
  contract, not a public product API.
- **State workflow impact**: No product stateful workflow, I/O, command, effect,
  subscription, or interpreter behavior changes. Governance generation and
  scanning stay pure file-scan + file-generation over the repository.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering,
  screenshot, Vulkan, Skia, or visual output is touched.
- **Evidence obligations**: Real evidence under
  `specs/057-dedupe-governance-corpus/readiness/` — a duplication catalogue with
  per-instance source/guard mapping, a single-source change demonstration, the
  red→green negative proof (056 cases plus the new generated-copy-drift case), a
  generated-consumer currency transcript (`SkillSyncCheck`/`TemplateDrift`), and
  the structural-reduction + maintenance-surface accounting; plus the standard
  escalated readiness-contract artifacts.
- **Unsupported scope**: The feature does **not** re-derive or expand the **set**
  of governed rules (no new obligation/token semantics); it does not touch product
  features, visual-parity work, the Charts split, or package versioning. No
  version bump is implied by the governance refactor itself.
- **Build-target impact**: `RefreshSurfaceBaselines` gains new generated artifacts;
  `GeneratedGuidanceCheck`, `TargetMetadataDrift` (and/or `TemplateDrift`), and
  `SkillSyncCheck` change to add currency guards for the new generated copies.
  `EvidenceGraph`/`EvidenceAudit` readiness vocabulary may extend. No new top-level
  FAKE target is required unless a generated artifact needs its own gate.

## Success Criteria *(mandatory)*

- **SC-001**: Changing any one governed contract token, obligation phrase, or
  constitution principle requires editing **exactly one** canonical source file
  (plus running the regeneration target), down from the current N home files.
- **SC-002**: The corpus is measurably smaller than 056's 6772 lines, with the
  reduction attributable to collapsed duplication (verified against the duplication
  catalogue), not to removed rules. No fixed target; the achieved number is
  reported honestly.
- **SC-003**: 100% of governed tokens and obligations are preserved —
  `GeneratedGuidanceCheck` is green over the regenerated corpus.
- **SC-004**: Drift-detection strength is provably retained: every 056 negative
  case still fails, and a generated copy edited out of sync with its source fails a
  currency gate naming the file and source — both demonstrated red→green.
- **SC-005**: Zero silent drift holes: an enumerated audit shows every generated
  governance artifact paired with the currency gate that guards it; none is
  ungoverned.
- **SC-006**: No governed rule, obligation, or forbidden-absence is lost (reviewer
  attestation backed by the green gate set).
- **SC-007**: Generated consumers stay byte-correct: `SkillSyncCheck` and
  `TemplateDrift` are green, and a generated `dotnet new fs-skia-ui` project
  receives correct, non-stale governance guidance.

## Key Entities

- **Duplication instance**: one occurrence of identical governed content carried in
  more than one place (token in N files, obligation phrase in N files, in-file
  echo, or constitution/template principle copy), with the validator that requires
  it and a proposed canonical source.
- **Canonical source**: the single file (or `Guidance.fs` value, or constitution
  principle) that owns a piece of governed content after the refactor.
- **Generated copy**: a derived rendering of a canonical source spliced/written
  into a consumer file by `RefreshSurfaceBaselines`, guarded by a currency check.
- **Currency guard**: the gate (`SkillSyncCheck`, `TargetMetadataDrift`,
  `TemplateDrift`, or a new check) that fails when a generated copy drifts from its
  source.

## Assumptions

- `build/Governance/Guidance.fs`, the scanners, `RefreshSurfaceBaselines`, and the
  currency checks **are editable** in this feature — the defining difference from
  056, which held them read-only. The change escalates on `Route`.
- The single-source-and-generate pattern already in the repo (`.agents`→`.claude`,
  `Routing.fs`→`validation.contract.yml`, `ConstitutionFragments`) is the model to
  extend; no new generation framework is introduced if the existing one suffices.
- Some governed content legitimately differs per file (e.g. template placeholders);
  the feature single-sources only genuine identical-content duplication (FR-011).
- This is a Tier 2 internal governance change (no public product `.fsi`/surface/
  package/version/runtime change) that escalates for verification because it
  touches governance paths.
- This feature builds on 056 (the tightened corpus) and 055 (the
  `ContractToken`/`GuidanceObligation` model); the duplication it targets is
  precisely what an editorial pass could not remove.

## Dependencies

- 056 (`056-rewrite-governance-mds`) — the tightened corpus this feature reduces
  further; structurally, 057 starts where 056's editorial ceiling stopped.
- 055 (`055-decouple-guidance-anchors`) — established the
  `ContractToken`/`GuidanceObligation` model and `evaluateGuidanceCheck`; this
  feature changes the *carriage* of those values, not the model's rule set.
