# Quickstart: Governance Markdown Rewrite

How to perform and verify the rewrite. No new tooling — existing gates do the
verification.

## 0. Route first

```bash
./fake.sh build -t Route            # prints tier + minimal gate list for this diff
./fake.sh build -t Route --enforce  # additionally fails on missing escalated evidence
```

This change touches `.specify/**` + governance guidance, so `Route` is expected to
**escalate** to the maintainer-verify path. Run exactly the gates it prints.

## 1. Establish the baseline snapshot (before any edit)

```bash
find .agents/skills -name '*.md' | xargs wc -l | tail -1   # ~4072
find .specify       -name '*.md' | xargs wc -l | tail -1   # ~2817
# sum ~6889 vs corrected baseline 6882
```

## 2. Rewrite canonical sources only

Edit `.agents/skills/**/*.md` and `.specify/**/*.md` for tightness/clarity.
Largest targets first: `speckit-checklist` (367), `fsharp-parsing` (341),
`speckit-specify` (325) SKILLs; the `constitution-template`/`tasks-template`
twins (328/315 ×2). **Never edit `.claude` by hand.** Per edit, hold the contract
in [contracts/governance-currency-contract.md](./contracts/governance-currency-contract.md):

- keep every C1 token verbatim in its home files (twins included),
- keep every C2 concept anchor matchable (AllOf phrases are non-negotiable),
- reintroduce no C3 forbidden term,
- keep every rule a reader can still extract (C5).

Rewrite identical twins in lockstep so they stay identical (or diverge only with
intent, each still satisfying its obligations).

## 3. Regenerate `.claude` from `.agents`

```bash
./fake.sh build -t RefreshSurfaceBaselines   # regenerate .claude skill tree from .agents
```

## 4. Verify (escalated six-target order — FAKE is sequential, never concurrent)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck     # tokens + obligations + forbidden + constitution-check completeness
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# plus SkillSyncCheck + TargetMetadataDrift (folded into Dev / printed by Route) green
```

## 5. Negative proof (drift detection survived)

```bash
# mutate ONE source-of-truth obligation: delete an AllOf concept phrase from a home
# file (e.g. remove "no compatibility shim" from src/Controls/skill/SKILL.md)
./fake.sh build -t GeneratedGuidanceCheck     # MUST fail, naming file + obligation id
git checkout -- <that file>                   # revert
# repeat for one contract token removal; then revert
```

Record the failing diagnostic + the green-after-revert in
`readiness/rewrite-red-green.md`.

## 6. Produce evidence

Under `specs/056-rewrite-governance-mds/readiness/`:

- `prose-size-accounting.md` — baseline 6882, measured `.agents`/`.specify`
  counts, summed current, signed delta, restated target, the two `wc -l`
  reproduction commands (`renderProseSizeAccounting` layout).
- `contract-tokens.md` — every C1 token + C2 obligation confirmed present/matchable per home file.
- `rewrite-red-green.md` — the step-5 mutation failure + revert-green.
- `generated-guidance.md`, `skill-sync-check.md`, `template-drift.md`,
  `validation-contract.md` — green gate transcripts.
- Standard escalated artifacts: `aggregate-hang-diagnostics.md`,
  `skill-loading-evidence.md`, `governance-risk-levels.md`,
  `runtime-limitations.md`, `evidence-graph.md`, `evidence-audit.md`.

## Done when

All Route-printed gates green at the feature SHA, `Route --enforce` reports all
required evidence present (SC-008), the recorded mutation still fails (SC-003),
and the size accounting shows a real reduction (SC-001/SC-007).
