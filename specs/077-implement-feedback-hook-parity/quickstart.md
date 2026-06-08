# Quickstart: verify implement-phase feedback hook parity

How to confirm the fix and exercise the guard. FAKE-backed commands share `.fake`
state — run them **sequentially** in the order below.

## 1. Confirm the four skills now carry the modern block

```bash
for s in implement tasks taskstoissues constitution specify plan clarify analyze checklist; do
  f=".agents/skills/speckit-$s/SKILL.md"
  printf "%-14s multi-file:%s  effective-notice:%s\n" "$s" \
    "$(grep -c '\.specify/extensions/\*/\*\.yml' "$f")" \
    "$(grep -c '## Effective hooks for' "$f")"
done
```

Expected: every phase shows `multi-file: >=2` and `effective-notice: 1`.

## 2. Regenerate the derived `.claude` tree and the validation contract

```bash
./fake.sh build -t RefreshSurfaceBaselines
```

Regenerates `.claude/skills/**` byte-identically from `.agents/skills/**` and
`validation.contract.yml` from `Routing.fs`. (Watch for trailing-newline drift.)

## 3. Run the new guard (and prove it bites)

```bash
./fake.sh build -t PhaseHookParityCheck            # PASS on the repaired tree
cat specs/077-implement-feedback-hook-parity/readiness/phase-hook-parity-check.md
```

Negative proof (failing-first): the Expecto test
`Governance.Tests/PhaseHookParityTests.fs` feeds a block-stripped SKILL.md body
and asserts a `phase-hook-parity` finding is produced (red before the guard logic
exists, green after). Run the suite:

```bash
./fake.sh build -t Dev          # runs Governance.Tests incl. PhaseHookParityTests
```

## 4. Confirm routing escalates skill changes to the new gate

```bash
./fake.sh build -t Route                 # skill diff -> FocusedAuthority
./fake.sh build -t Route --enforce       # fails if required evidence artifact missing
```

`PhaseHookParityCheck` should appear in the printed gate list for the
`skill-quality` rule.

## 5. Confirm the fix reaches generated consumer projects

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck            # TemplateSmoke asserts corrected skills present
./fake.sh build -t GeneratedProductCheck    # note: known non-authoritative local failure
```

`GeneratedProductCheck` fails locally for an unrelated env reason (no template
`feature.json` / `Map.empty` env) — treat that specific failure as
non-authoritative; rely on `TemplateCheck`/CI.

## 6. Behavior-preservation check (this repo: no feedback extension)

This repo registers only `git`/`evidence` hooks. Running a phase here must produce
**no** new error/prompt/feedback file from the added blocks — the discovery is a
silent no-op when no matching hook is registered (FR-005 / FR-009 / SC-006).

## Full escalated serial order (maintainer-verify)

```
1. ./fake.sh build -t Dev
2. ./fake.sh build -t PhaseHookParityCheck
3. ./fake.sh build -t GeneratedGuidanceCheck
4. ./fake.sh build -t TemplateCheck
5. ./fake.sh build -t GeneratedProductCheck
6. ./fake.sh build -t EvidenceGraph
7. ./fake.sh build -t EvidenceAudit
```
