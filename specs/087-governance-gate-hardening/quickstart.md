# Quickstart: Governance Gate Hardening

How a maintainer exercises each of the six fixes and reads its verdict. All
commands are FAKE-backed governance targets — run them **sequentially** (shared
`.fake` state). `Route` first decides the minimal gate set; this governance-path
change escalates to the six-target order.

```sh
./fake.sh build -t Route          # confirm tier + minimal gates for the diff
./fake.sh build -t Route --enforce # fail if an escalated change is missing required evidence
```

## FR-001/002 — `GeneratedProductCheck` gives a trustworthy local verdict

```sh
./fake.sh build -t GeneratedProductCheck
```
- **Expect (clean tree)**: green — the generated `Verify` step resolves a feature
  context and runs; no hand-classified "non-authoritative" failure (SC-001).
- **Expect (seeded product defect + concurrent env obstacle)**: red with a
  **product-defect** classification on the defective step; the env-classified
  step is reported but does not suppress the product defect (SC-002).
- Evidence: `readiness/generated-product-check-green.txt`,
  `readiness/generated-product-defect-classification.txt`.

## FR-003/004 — pinned-vs-local package skew caught before merge

```sh
./fake.sh build -t TemplateCheck         # report states packageSet: LocalPacked
./fake.sh build -t GeneratedProductCheck # report states packageSet: Pinned
```
- **Expect (real tree)**: zero skew findings; each report names its package set.
- **Expect (seeded unpinned-API reference)**: blocking finding naming symbol +
  file + pinned-vs-local version gap — produced statically, no network restore
  (SC-003/004).
- Evidence: `readiness/package-skew-clean.txt`, `readiness/package-skew-seeded.txt`.

## FR-005/006 — complete, idempotent surface-baseline refresh

```sh
./fake.sh build -t RefreshSurfaceBaselines
git status --porcelain                    # after an additive .fsi change: only that package's per-package baseline diffs
./fake.sh build -t RefreshSurfaceBaselines
git status --porcelain                    # second run on unchanged tree: empty (zero churn)
```
- **Expect**: per-package baselines (`readiness/per-package-surface/*.fsi.txt`)
  regenerate alongside cross-package/api-surface/skill baselines; a no-op rerun
  leaves `git status` clean — no trailing-newline churn (SC-005/006).
- Evidence: `readiness/refresh-surface-baselines-idempotent.txt`.

## FR-007/008 — three-state merge-audit verdict

```sh
./fake.sh build -t EvidenceAudit
```
- **Expect**: `seh-audit-summary.json` `verdict` is one of `Pass`,
  `PassWithAcceptedDeferrals`, `Fail`, with separated
  `acceptedSyntheticCount` / `unacceptedSyntheticCount` and durable
  `acceptedDeferrals` records (justification + real-evidence path + awaited
  capability). Three seeded inputs produce the three distinct verdicts (SC-007).
- `PassWithAcceptedDeferrals` is reachable **only** with zero unaccepted
  synthetic and zero blocking hits.
- Evidence: `readiness/audit-three-verdicts.txt` + sample `seh-audit-summary.json`.

## FR-009 — synthetic propagation follows real dependencies

```sh
./fake.sh build -t EvidenceGraph
```
- **Expect**: marking a leaf `[S]` task whose output nothing consumes propagates
  `[S*]` to **zero** unrelated later-phase tasks — a phase-checkpoint edge alone
  no longer contaminates (SC-008).
- Evidence: `readiness/synthetic-propagation-no-phase-edge.txt`.

## FR-010 — captured-vs-asserted skill-loading evidence

- Skill-loading-evidence rows carry a `provenance` column (`captured` |
  `asserted`); a declared-but-unloaded skill is reported **at the declaring
  task's implementation point**, before any `[X]` flip (SC-009).
- Surfaced by `EvidenceGraph` / `EvidenceAudit` over `skill-loading-evidence.md`.
- Evidence: `readiness/skill-loading-evidence-provenance.md` + at-implementation gap report.

## FR-011 — true-positive gates still block

- Seed a real violation of each: diff-scan hit, non-additive surface change,
  window-visibility / persistent-launch contract, synthetic-honesty disclosure.
  Each must still block (SC-010) — no genuine block relaxed to obtain a green.
- Evidence: `readiness/true-positive-gates-still-block.txt`.

## Governance.Tests (pure engine)

```sh
./fake.sh build -t Dev   # runs Governance.Tests — verdict, propagation, skew, provenance, idempotence
```
- Property tests assert the FR-011 invariants: accepted deferral never masks an
  unaccepted synthetic or blocking hit; a phase-edge-only downstream of an `[S]`
  leaf is never `[S*]`.
