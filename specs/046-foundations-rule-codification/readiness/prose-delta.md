# Prose-trim delta (US3, T020, FR-010, SC-006)

Gate-before-prose (FR-008): every deletion below is backed by a seeded-violation proof
under `readiness/seeded-violations/` showing its enforcing gate fails. The four proven
gates are the new `ConstitutionCheck` completeness gate (`constitution-check.md`) plus
the three already-shipped Stage-6.1 gates (`surface-baseline-presence.md`,
`skill-id-resolution.md`, `late-seh-timing.md`).

## Governance-Markdown rule/guidance line count — before vs after

Baseline (Stage-0, pre-feature — matches research R5's ≈ 6,882, **not** the plan's
overstated ~23,000):

| Tree | Before | After | Δ |
|------|-------:|------:|--:|
| `.agents/skills/**/*.md` (canonical) | 4,065 | 4,059 | **−6** |
| `.specify/**/*.md` | 2,817 | 2,817 | 0 |
| **Total governance rule/guidance lines** | **6,882** | **6,876** | **−6** |
| `.claude/skills/**/*.md` (generated mirror of `.agents`) | — | — | **−6** |

The canonical `.agents` tree drops 6 lines; the generated `.claude` mirror drops the
same 6 (byte-identity preserved, FR-009). Net reduction is real (SC-006 satisfied).

### Reproduction commands

```
# governance rule/guidance Markdown line counts
find .agents/skills -name '*.md' | xargs wc -l | tail -1      # -> 4059 (after)
find .specify       -name '*.md' | xargs wc -l | tail -1      # -> 2817
# pre-feature baseline of any file:
git show <merge-base>:.agents/skills/<skill>/SKILL.md | wc -l
# exact per-file deltas this feature introduced:
git diff --numstat -- '.agents/skills/**/*.md' '.claude/skills/**/*.md'
#   8  15  .agents/skills/speckit-implement/SKILL.md   (net -7)
#   1   0  .agents/skills/speckit-plan/SKILL.md        (net +1)
#   (identical pair under .claude/skills)
```

## Per-invocation skill-byte load (the trimmed skills, canonical `.agents`)

| Skill | Before (bytes) | After (bytes) | Δ |
|-------|---------------:|--------------:|--:|
| `speckit-implement/SKILL.md` | 10,774 | 10,235 | **−539** |
| `speckit-plan/SKILL.md` | 6,166 | 6,451 | +285 |
| **Net** | | | **−254** |

```
wc -c .agents/skills/speckit-implement/SKILL.md .agents/skills/speckit-plan/SKILL.md
```

## What was deleted, and why it was safe

1. **Late-`[SEH]` design-phase-timing rule** (`speckit-implement/SKILL.md`, Synthetic-
   evidence disclosures). The 16-line restatement (the "Non-eligible synthetic cases…"
   and "Eligible examples…" enumerations) was condensed to an 8-line statement that
   keeps the rule's intent and the currency-pinned tokens (`[SEH]`,
   `synthetic-error-handling-approved`, `implementation-time relabeling`) and now points
   at the enforcing `EvidenceAudit` gate. Proof: `seeded-violations/late-seh-timing.md`.

2. **Constitution-Check completeness** — a one-line pointer was added to
   `speckit-plan/SKILL.md` ("Key rules") stating the completeness rule is now
   machine-enforced by `GeneratedGuidanceCheck`, so reviewers no longer hand-verify it.
   Proof: `seeded-violations/constitution-check.md`.

## What was retained, and why (SC-006 justification)

The bulk of the rule prose for the **skill-id-resolution** and **late-`[SEH]`** rules in
`speckit-implement` / `speckit-tasks` is **retained deliberately**: it doubles as genuine
*when-to-use* author guidance, and its exact tokens are **pinned by the feature 041–044
generation-currency term-checks** (`Guidance.validateTaskSkillistGuidance`). Those checks
require the guidance to continue *stating* each rule, so the safe forward-deletion set is
the redundant enumerations only — which is what was trimmed. The **surface-baseline
presence** rule has no honour-system restatement in `.agents/skills/**` (it lives in the
capability catalog + `Capabilities.fs`), so there was no skill prose to delete; its gate
remains proven (`seeded-violations/surface-baseline-presence.md`). This keeps the trim
honest and non-regressive while still producing a net reduction.
