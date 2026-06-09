# Contract: routing classification (FR-008, FR-009, FR-010, SC-004)

## Classification table (the acceptance oracle for `Route`)

| Staged diff                                              | Expected routing                                  |
|---------------------------------------------------------|---------------------------------------------------|
| `template/**/*.md` only (not `skill/SKILL.md`)          | `RequiredGates = [ EvidenceGraph ]` (pinned); **excludes** `GeneratedProductCheck`, `TemplateCheck` and all heavy gates |
| `src/Controls/**/*.md` only                             | `RequiredGates = [ EvidenceGraph ]` (pinned); **excludes** `GeneratedProductCheck` and heavy controls gates |
| `template/<real source>` (e.g. a `.fsx`/`.json`/fragment)| full template set **unchanged** (`TemplateCheck; GeneratedProductCheck; SkillContractPathCheck`) |
| `src/Controls/**/*.fsi` (or any non-doc source)         | full controls/package-surface set **unchanged**   |
| mixed: `src/Controls/a.md` + `src/Controls/b.fsi`       | full set (re-escalated by the non-doc path)       |
| `build.fsx` / `scripts/build/**` / `validation.contract.yml` / `.specify/**` / `build/Governance/**` (any, incl. `.md`) | **not relaxed** — existing rules apply (FR-009) |

## Pinned doc-only gate set

The two new doc-only rules (`controls-docs`, `template-docs`) use the **exact**
`RequiredGates = [ EvidenceGraph ]` — no heavy gate, no `Dev`. `EvidenceGraph` is
retained so a doc-only change still validates the task DAG. This value is pinned
(not illustrative): the regenerated `validation.contract.yml` (T020) and the
`Route` selection tests (T016) assert this exact list, so the doc-only contract
diff is deterministic and reviewable.

## Invariants

- Composition unchanged: max-tier (`tierRank`), union of `RequiredGates`, registry-order dedup
  (`internalDedupInRegistryOrder`). New rules **add**; relaxation comes from the source rules' refined
  matcher (match heavy gates only when a non-doc path is present), never from removing gates in `select`.
- The rendered `paths:` view in `validation.contract.yml` stays the single source for each rule's globs;
  the doc-exclusion is documented at the rule and reflected in the regenerated contract.
- FR-010 dependency-chain tightening is opportunistic: applied **only** with a written coverage-
  equivalence argument; otherwise deferred. No change to any gate's effective coverage.

## Intentional contract diff (Tier 2)

`validation.contract.yml` is **regenerated** (new sub-targets in `Targets`, new routing rules) — an
intentional, rationale-documented diff (distinct from Tier 1/3 byte-identity). `TargetMetadataDrift`
MUST pass against the regenerated file.
