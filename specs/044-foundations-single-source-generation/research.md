# Phase 0 Research: Single-Source Generation (Stage 2.2–2.5)

All NEEDS CLARIFICATION are resolved below. The spec recorded three clarifications on 2026-06-01
(marker-delimited constitution regions; active-feature-only skillist scope; retire
`SkillExamplesCheck`). This document settles the remaining planning-level decisions, each grounded
in the existing feature-042 (`ContractView`) / feature-041 (`TargetMetadata`) single-source
precedents the spec instructs us to mirror.

---

## R1 — The single regeneration entry point: reuse `RefreshSurfaceBaselines`, do not proliferate targets

- **Decision**: Generation for the skill tree and the constitution fragments folds into the existing
  `RefreshSurfaceBaselines` target (build.fsx:920), which **already** emits
  `WriteFile(validation.contract.yml, ContractView.render …)`. No new generation target is added.
  Currency is enforced by **reusing existing gates**: `SkillSyncCheck` (skills),
  `TargetMetadataDrift` (constitution fragments, alongside the existing contract-currency check),
  and the active-feature evidence audit (skillist).
- **Rationale**: This is exactly the feature-042 mechanism, which the spec names as the model to
  follow. A single, already-muscle-memory regeneration command keeps diagnostics uniform ("run
  `./fake.sh build -t RefreshSurfaceBaselines`") and avoids surface bloat. FR-015 ("new generation
  targets MUST be added to the typed `Targets` model") is honored vacuously — no mistyped target is
  introduced, and the existing typed targets are reused — while `SkillExamplesCheck`'s **removal**
  from the typed DU is the FR-015-governed target-model edit (a compile error if a reference is
  missed).
- **Alternatives considered**:
  - *Dedicated `GenerateSkills` / `GenerateConstitution` targets* — cleaner per-artifact diagnostic
    wording, but proliferates the typed `Targets` DU and the FAKE registration, and diverges from
    the contract precedent for no functional gain. Rejected; may be revisited if a maintainer wants
    finer-grained diagnostics.
  - *Regenerate-on-read* — rejected by spec (FR-013): derived artifacts stay committed; currency is
    a gate, not a read-time side effect.
- **Skillist nuance**: `tasks.md` is **per-feature**, not a repo baseline, so its annotation
  regeneration is scoped to the **active feature** and lives in the evidence path
  (`SkillistView` invoked for the active feature), not in the repo-wide `RefreshSurfaceBaselines`.
  See R3.

## R2 — Skill-tree canonical direction and enumeration

- **Decision**: `.agents/skills/` is **canonical** (the Codex source per `CLAUDE.md`);
  `.claude/skills/` is **generated**. The generator **enumerates** every `SKILL.md` under the
  canonical tree (today 25) and reproduces each at the mirror path; coverage is by enumeration, with
  **no hardcoded slug list**. `SkillSync.expectedSlugs` (the 6-slug allowlist) is **deleted** — it
  is the precise mechanism that left 19 of 25 pairs unguarded (spec Edge Case "partial coverage
  regression").
- **Rationale**: Matches the spec Assumption and `CLAUDE.md`'s "Codex source / generated peer"
  framing. Enumeration is what makes SC-002 (add a skill → zero allowlist edits) hold by
  construction.
- **Generation = byte copy**: "Generate" here means reproduce the canonical bytes at the derived
  path (copy-generation, FR-003). The pure core takes a list of `(relPath, bytes)` read from the
  canonical tree and returns the derived `(relPath, bytes)` plan (path-translated, identity content),
  so it is unit-testable without touching the repo tree (FR-014). The I/O edge (`RefreshSurfaceBaselines`)
  enumerates, reads, and writes.
- **Alternatives**: symlink tree — rejected by spec (cross-platform; copy-generation only).
- **Empty/malformed input**: a missing or unreadable canonical `SKILL.md`, or an empty canonical
  tree, fails the generator with a clear diagnostic (Principle VII) rather than emitting a partial
  derived tree that would then pass identity checks (spec Edge Case).

## R3 — Skillist canonical direction and the currency reframe

- **Decision**: `tasks.deps.yml` `skillist:` is **canonical** (decision D6: high-churn,
  agent-authored, logic-free instance data stays as data); the `tasks.md` `[skillist: …]`
  annotation is the **derived view**. A pure `SkillistView.renderAnnotation` produces the
  `[skillist: a, b]` token from a deps skillist; a pure `SkillistView.spliceAnnotation` replaces the
  `[skillist: …]` token on an existing `tasks.md` task line **in place**, leaving all other prose on
  the line and the rest of the file byte-unchanged.
- **The reframe**: feature-043's `Evidence/Audit.fs` already compares the parsed `tasks.md` mirror
  against the deps skillist (`mirror <> sk → error`) and is **already scoped to the active feature**
  (the engine reads only `specs/<activeFeature>/{tasks.md,tasks.deps.yml}` resolved from
  `.specify/feature.json`). The comparison value is unchanged; what changes is **framing and
  diagnostic**: from "tasks.md mirror […] does not match tasks.deps.yml […]" (a symmetric peer
  complaint) to an asymmetric currency message — "the `tasks.md [skillist: …]` view for `<task>` is
  stale relative to its canonical `tasks.deps.yml` source; regenerate it" — naming the regeneration
  action (FR-006/FR-007/FR-012).
- **Why this satisfies SC-004 (no historical regression) automatically**: because the audit is and
  remains active-feature-scoped, the ~43 historical feature directories are never re-derived. The
  diagnostic wording change is the only behavior change for features whose representations already
  agree (they continue to pass).
- **Rendering into agent-authored markdown** (spec Assumption "confirm the mechanics"): the splice
  is a **token replacement on the matched task line**, anchored by the same
  `\[skillist:\s*(\[\]|[^\]]*)\]` regex feature-043 already parses with. It does not reflow, reorder,
  or reformat the line — it swaps only the bracketed token's contents — so surrounding human prose
  (task title, `[P]`, tier annotations) is preserved verbatim. A task line missing the annotation
  entirely is reported (not silently inserted) since omitted metadata is invalid per the
  constitution's Local Agent Skills section.

## R4 — Constitution fragment granularity and the marker mechanism

- **Decision**: A **small, fixed set of principle-summary fragments** is generated from
  `.specify/memory/constitution.md` and spliced between explicit `<!-- BEGIN GENERATED:
  constitution/<fragment-id> -->` and `<!-- END GENERATED: constitution/<fragment-id> -->` HTML
  comment markers inside `plan-template.md` and `tasks-template.md`. The generator replaces only the
  text **between** a marker pair; everything outside every marker pair is preserved byte-for-byte
  (FR-008/FR-010).
- **Fragment scope (pragmatic split, per spec Assumption)**: only the **verbatim/near-verbatim
  principle restatements** become generated fragments — concretely the principle-name + one-line
  summary echoes the templates carry (e.g. the "Tests First (Principle I, Principle VI)" and
  Elmish/MVU `[X]`-evidence summaries in `tasks-template.md`). Genuine *instructional* prose (how to
  fill a section, task-numbering conventions, the Synthetic-Evidence Inventory table shape) is
  **not** a constitution echo and stays hand-written outside the markers. The exact fragment set and
  their target regions are enumerated in [data-model.md](./data-model.md) and locked by the
  Phase-1 contract; content rewriting/trimming is explicitly Stage 6.
- **Extraction is structural, not NLP**: `ConstitutionFragments` parses the constitution's
  `### <Principle>` headings and derives each fragment's summary line from a deterministic rule
  (heading text + first sentence / a curated mapping table in the module), so re-running the
  generator is reproducible and a principle edit deterministically changes the fragment. No
  free-form paraphrase generation.
- **Currency**: a pure `ConstitutionFragments.currencyDrift (onDiskTemplate) (constitution) :
  string option` re-derives the expected marker-region contents and compares against what is
  committed between the markers, returning `Some "<template> constitution fragment is stale —
  regenerate via ./fake.sh build -t RefreshSurfaceBaselines"` on drift. Folded into
  `TargetMetadataDrift` next to `ContractView.currencyDrift` (same home, same precedent).
- **Alternatives**: file `include`/transclusion — rejected because Spec Kit templates are consumed
  as flat Markdown with no include mechanism; marker-splice keeps the templates self-contained and
  readable while making the generated regions unambiguous and machine-replaceable.

## R5 — Resolving the byte-identity vs provenance-header tension (FR-003 ⟂ FR-011)

- **The tension**: FR-003 requires every derived `SKILL.md` to be **byte-identical** to its
  canonical counterpart; FR-011 requires every generated artifact to carry a **provenance header**.
  A per-file header inside the derived `SKILL.md` would break byte-identity.
- **Decision**: byte-identity wins for the skill `SKILL.md` files; provenance for the skill tree is
  carried by a **single tree-level provenance manifest** committed at the derived tree root
  (e.g. `.claude/skills/GENERATED.md` or a `.manifest` file) naming the canonical source
  (`.agents/skills/`) and the regeneration command (`./fake.sh build -t RefreshSurfaceBaselines`),
  plus the regeneration banner already present in the build's currency diagnostic. The 25 derived
  `SKILL.md` stay byte-identical.
- **Per-artifact provenance otherwise**: the constitution fragments carry provenance **in the marker
  comments themselves** (`BEGIN GENERATED: constitution/<id>` already names the source class); the
  regenerated `validation.contract.yml` already carries its `# GENERATED from … Routing.fs` header.
  The skillist annotation cannot carry a header (it is an inline token), so its provenance is the
  documented canonical-source rule (deps is canonical) surfaced by the currency diagnostic.
- **Rationale**: FR-011's intent (a reader/contributor can always discover the source and the
  regeneration command, and edits to a derived artifact are caught) is fully met per artifact class
  without violating FR-003. Interpreting "header" as "machine-readable provenance discoverable for
  the artifact" — file-level where possible, tree-level where a per-file header would break a
  stronger requirement — is the minimal coherent reading.

## R6 — Retiring `SkillExamplesCheck` cleanly

- **Decision**: Remove the `SkillExamplesCheck` `Target` DU case and every derived reference
  (`name`, `directPrerequisites`, `allTargets`/`dispatchTargets`, the `build.fsx` `SkillExamplesGate`
  effect + `runSkillExamplesGate`, and the `SkillExamples.fsi/.fs` module), plus its Governance.Tests
  suite. Because the typed `Targets` DU is exhaustively matched, a missed reference is a **compile
  error** (the safety FR-004/FR-015 rely on).
- **Coherence follow-through**: removing a target changes the derived `requiredTargetNames` /
  `targetDependencyRows`, which feed `ContractView.render` and `TargetMetadataDrift`. Therefore
  `validation.contract.yml` **must be regenerated** in the same change and `TargetMetadataDrift` must
  stay green (the currency check would otherwise trip). This is verified by running
  `RefreshSurfaceBaselines` then `TargetMetadataDrift` in the serialized sequence.
- **Rationale**: the spec's clarification 3 makes retirement explicit — once generation guarantees
  byte-identity, compiling tangled `fsharp` blocks from the skills as a *separate* peer check adds no
  guarantee the currency check doesn't already provide. (Note: the tangled-block *compile* coverage
  itself is content-validation, out of scope here; if a maintainer wants to keep compile-checking the
  cookbook snippets, that is a separable follow-up and does not block this retirement.)

## R7 — Test strategy (failing-first, typed, no repo-tree dependence)

- **Decision**: every new pure function gets Expecto unit tests asserting **typed** outputs on
  in-memory inputs, with at least one **failing-first** stale-fixture case per currency function:
  - `SkillTreeGen`: enumeration covers a synthetic set including a **26th** skill not in any allowlist
    (SC-002); the derived plan is content-identical to canonical (SC-001); a tampered derived byte
    yields a `Some` currency diagnostic; empty/missing canonical input yields a generator error.
  - `SkillistView`: `renderAnnotation [a;b] = "[skillist: a, b]"` and `[] = "[skillist: []]"`;
    `spliceAnnotation` on a sample task line changes only the token and preserves the rest;
    currency flags a stale derived annotation and passes a current one.
  - `ConstitutionFragments`: fragment extraction is deterministic; `splice` preserves all
    out-of-marker bytes (a property-style assertion over a fixture template); currency flags a stale
    region after a simulated principle edit.
- **Rationale**: mirrors the feature-042 `ContractViewTests` / feature-043 typed-result discipline
  (FR-014, Principle IV) so the logic is provable without a live repo and the FAKE-target runs supply
  the integration/interpreter evidence (SC-008).

---

## Resolved unknowns summary

| Unknown | Resolution |
|---|---|
| Where does regeneration live? | Existing `RefreshSurfaceBaselines` (skills + constitution); active-feature evidence path (skillist). No new target. (R1) |
| Skill canonical direction & coverage? | `.agents/skills/` canonical; enumerate all 25; delete the 6-slug allowlist. (R2) |
| Skillist canonical direction & reframe? | `tasks.deps.yml` canonical; reframe the existing active-feature `Audit.fs` comparison's diagnostic; in-place token splice for the derived annotation. (R3) |
| Constitution granularity & mechanism? | Small fixed fragment set; `BEGIN/END GENERATED` HTML-comment marker splice in `plan-template.md`/`tasks-template.md`; structural extraction. (R4) |
| Byte-identity vs provenance header conflict? | Byte-identity wins for `SKILL.md`; tree-level provenance manifest; marker/header provenance for the others. (R5) |
| How to retire `SkillExamplesCheck` safely? | Remove the typed DU case + all derived refs (compile-enforced); regenerate the contract; keep `TargetMetadataDrift` green. (R6) |
| Test approach? | Typed Expecto unit tests, failing-first stale fixtures, no repo-tree dependence. (R7) |
