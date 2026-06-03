# Quickstart: Decoupled Guidance Anchors

## Run the gate

```bash
# Authoritative tier + minimal gate list for this diff
./fake.sh build -t Route --enforce

# The guidance currency gate that hosts the three decoupled validators
./fake.sh build -t GeneratedGuidanceCheck
```

If `Route` escalates (this diff touches `.specify/**`, `docs/**`, and governance
code), run the gates it prints — defaulting to the serialized maintainer-verify
order, sequentially (FAKE shares `.fake` state):

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Demonstrate the unlock (US1 — rewording passes)

Shorten a governed paragraph while keeping its obligation concept, then run the
gate — it passes where the pre-055 literal table failed:

```bash
dotnet test tests/Governance.Tests/ --filter "Guidance"
# US1 test: reworded-but-concept-preserving content PASSes evaluateGuidanceCheck
```

## Demonstrate drift is still caught (US2 — source drift fails)

```bash
# US2 test: content with an obligation concept removed FAILs with a diagnostic
# naming the file and the unmet obligation id + source.
```

## Confirm contract tokens stay literal (SC-004)

```bash
# SC-004 test: removing any ContractToken (e.g. "[skillist: []]") still FAILs.
```

## Prose-size accounting (FR-007 / SC-005)

```bash
find .agents/skills -name '*.md' | xargs wc -l | tail -1   # .agents/skills lines
find .specify       -name '*.md' | xargs wc -l | tail -1   # .specify lines
# sum vs corrected baseline 6,882 → readiness/prose-size-accounting.md
```

The restated goal lives in `docs/reports/_baselines/2026-06-02-foundations-after.md`
(row 5) and `specs/047-foundations-programme-closeout/contracts/after-baseline.md`:
the "low hundreds" / ~23,000 figure is retired as the live target; tracking is
against ≈6,882 with the actual reduction a bounded follow-up.

## Keep generation current (FR-010)

```bash
# After tightening prose in any .agents/skills/*/SKILL.md, regenerate the .claude mirror:
./fake.sh build -t RefreshSurfaceBaselines
./fake.sh build -t SkillSyncCheck          # byte-identical reproduction
./fake.sh build -t TargetMetadataDrift     # validation.contract.yml still current
```
