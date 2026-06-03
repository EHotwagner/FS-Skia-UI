# Structural-Reduction & Maintenance-Surface Accounting (US3, SC-002 / FR-009)

Sole writer of this deterministic accounting (T014). No fixed target line count;
the achieved number is reported honestly against the 056 baseline, with the
reduction traced to the duplication catalogue (collapsed duplication, not dropped
rules — `GeneratedGuidanceCheck` is green, see `contract-tokens.md`).

## Corpus line delta (the 056 measurement scope)

Reproduction:

```bash
find .agents/skills -name '*.md' -exec cat {} + | wc -l
find .specify       -name '*.md' -exec cat {} + | wc -l
```

| Scope | 056 baseline | 057 pre-change | 057 post-change |
| --- | --- | --- | --- |
| `.agents/skills/**/*.md` | — | 3973 | 3977 |
| `.specify/**/*.md` | — | 2800 | 2791 |
| **Combined corpus** | **6772** | **6773** | **6768** |

**Achieved reduction: −4 lines vs the 6772 honest baseline** (−5 vs the 6773
pre-change measurement). Measurably smaller (SC-002).

### Attribution (collapsed duplication, not dropped rules)

| Source | Lines | Catalogue ref |
| --- | --- | --- |
| Deleted in-file redundant `Exact skill phrases for scans:` echoes | `.specify` ×2 standalone + 2 inline | class 3 `skill-phrases-echo` |
| Deleted in-file redundant `Exact readiness phrases for scans:` echoes | `.specify` ×3 standalone | class 3 `readiness-phrases-echo` |
| Added `gov/visual-proof-phrases` + `gov/owner-phrases` markers (single-source overhead) | `.agents` +4 | class 3 `visual-proof-echo` / `owner-phrases-echo` |

Net `.specify` −9, net `.agents/skills` +4 → corpus −5 (from 6773). The marker
lines are the single-source *carriage* cost; the echo deletions are the genuine
reduction. `template/base/docs/product.md` (+4 markers) and `.claude/skills/**`
(regenerated) are outside the corpus measurement scope.

Every governed rule survives: `GeneratedGuidanceCheck` green over the regenerated
corpus (SC-003/SC-006). No rule, obligation, or forbidden-absence was removed —
only the *carriage* of the echoes changed.

## Maintenance-surface delta (files-touched-per-rule-change, before → after)

The headline single-sourcing win (SC-001):

| Rule | Before (N home files) | After |
| --- | --- | --- |
| `gov/visual-proof-phrases` (visual-proof rejection phrases) | 3 (`.agents` SKILL, `.claude` peer, `product.md`) hand-carried | **1** canonical `GovernedBlocks.CanonicalText` + `RefreshSurfaceBaselines` (splices `.agents` SKILL + `product.md`; `.claude` peer via `SkillSyncCheck`) |
| `gov/owner-phrases` (owner / host-warning phrases) | 3 (same) | **1** canonical source + regeneration |
| **constitution principle bodies** (Principles I–VII, Change Classification, Engineering Constraints, Workflow, Governance — ~300 identical lines) | **3** hand-maintained files (`constitution.md` + 2 `constitution-template.md` twins) | **1** canonical placeholder-bearing twin → `constitution.md` (concrete render) + preset twin (verbatim), currency-checked. ~660 lines of derived copy now generated, not hand-synced. |

Changing either phrase set now means editing **one** F# `GovernedBlock` value and
running `./fake.sh build -t RefreshSurfaceBaselines`; the previous N=3 hand-synced
copies are now generated and currency-checked (`TargetMetadataDrift`).

## Constitution class-4 collapse (FR-007) — the headline maintenance-surface win

The constitution/template/fragment triple is the largest duplication: ~300 lines
of identical principle prose hand-maintained in **three** files. After 057 the
placeholder-bearing `.specify/templates/constitution-template.md` is the **single
canonical source**; `constitution.md` (placeholders substituted, editorial
LOCKED/REQUIRED/TAILORABLE comments stripped, generic skill intro/list replaced
with the repo's concrete capability prose — 14 ordered substitution edits) and the
preset twin (verbatim) are **generated and currency-checked**. The concrete render
is **byte-identical** to the committed `constitution.md` (golden test
`GovernedBlocksTests` + idempotent `RefreshSurfaceBaselines`, zero churn). This is
*maintenance-surface* single-sourcing (N 3→1) with **no corpus-line change** —
all three files still physically exist with full content for agents to read; the
win is that a principle edit now touches one canonical source, not three.

## Scope note (honest accounting)

057 single-sources the two catalogued classes that are **genuine identical-content
duplication** end-to-end with the full machinery (`GovernedBlocks.fs` store +
marker-splice render/currency + the constitution full-body render, wired into
`RefreshSurfaceBaselines` and `TargetMetadataDrift`): the cross-file
visual-proof/owner phrase duplication (class 3) and the constitution triple
(class 4). It also removes the in-file-redundant scanner echoes per FR-006 (the
`AsteroidsFeedbackSkillGuidance` scanner now reads canonical prose with incidental
line-wrapping normalized).

The broad class-1 contract-token carriage (`[SEH]` sample N=9, controls tokens)
and class-2 obligation anchors are **NOT collapsed** — and this is a deliberate,
user-confirmed (2026-06-03) **FR-011** decision, not deferred work. On inspection
those tokens/phrases live in genuinely different, file-specific prose in each home
file; `Guidance.fs` only requires the token/concept to be *present* per file, not
identically phrased. Collapsing them into identical `gov/*` blocks would either add
redundant lines (against SC-002) or destroy meaningful per-file prose — exactly the
case FR-011 excludes. They remain governed as before by `evaluateGuidanceCheck` /
`GeneratedGuidanceCheck`. See the FR-011 reclassification in
`duplication-catalogue.md`. The genuine identical-content fraction those classes
touched — the 3 constitution copies of `[SEH]` and the constitution skill-gate
obligation prose — is already single-sourced via the class-4 constitution render.
