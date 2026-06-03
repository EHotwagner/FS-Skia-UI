# Quickstart: Single-Source the Duplicated Governance Corpus

How to work on, regenerate, and verify the single-sourced governance corpus.

## The one-line mental model

> Edit the canonical source, run `RefreshSurfaceBaselines`, let the currency gates
> prove every derived copy matches. Never hand-edit a generated copy.

## Change a governed rule (the SC-001 path)

1. Edit the **one** canonical source:
   - a contract token / obligation prose block → the `GovernedBlock` value in the
     canonical store (`build/Governance/**`).
   - a constitution principle body → the placeholder-bearing principle source.
   - an `.agents` skill → `.agents/skills/<slug>/SKILL.md`.
2. Regenerate every derived copy:
   ```sh
   ./fake.sh build -t RefreshSurfaceBaselines
   ```
3. Confirm currency + presence are green:
   ```sh
   ./fake.sh build -t GeneratedGuidanceCheck   # tokens/obligations present
   ./fake.sh build -t TargetMetadataDrift      # generated copies current
   ./fake.sh build -t SkillSyncCheck           # .agents -> .claude peers current
   ```

You should have touched exactly one file (plus regenerated outputs), never a
generated copy by hand.

## Verify the change (escalated maintainer-verify path)

This is a governance change, so `Route` escalates. Run the serialized six-target
order (FAKE-backed targets are sequential — never concurrent):

```sh
./fake.sh build -t Route                  # confirm escalation + gate list
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Prove drift detection still bites (the SC-004 red→green)

Each of these MUST fail, then revert to green after `git checkout` / regenerate.
Record the transcript in `readiness/dedupe-red-green.md`.

```sh
# 1. delete an obligation concept from its canonical source -> GeneratedGuidanceCheck FAIL
# 2. remove a contract token from its canonical block        -> GeneratedGuidanceCheck FAIL
# 3. reintroduce a forbidden term                            -> GeneratedGuidanceCheck FAIL
# 4. NEW: hand-edit a generated copy out of sync             -> TargetMetadataDrift FAIL (names file + source)
git checkout -- <file> && ./fake.sh build -t RefreshSurfaceBaselines   # back to green
```

## Measure the reduction (SC-002 / FR-009)

```sh
./fake.sh build -t Dev   # regenerates prose-size-accounting.md
```

Record the corpus line delta against 056's **6772** baseline and the
files-touched-per-rule-change (N → 1) in `readiness/structural-reduction.md`. The
saving must trace to the duplication catalogue, not to dropped rules.

## Readiness artifacts to produce

Under `specs/057-dedupe-governance-corpus/readiness/`:

- `duplication-catalogue.md` — every instance, class, home files, validator,
  resolution, canonical source, currency gate (FR-001).
- `single-source-demo.md` — one rule changed in one place; all copies updated.
- `dedupe-red-green.md` — the four red→green proofs above (SC-004).
- `silent-drift-audit.md` — every generated artifact paired with its guard (SC-005).
- `generated-consumer-currency.md` — `SkillSyncCheck` / `TemplateDrift` transcripts
  + a generated-project guidance check (SC-007).
- `structural-reduction.md` — line + maintenance-surface accounting (FR-009/SC-002).
- plus the standard escalated readiness-contract artifacts (mirroring
  `specs/056-rewrite-governance-mds/readiness/`).
