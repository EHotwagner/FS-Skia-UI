# Quickstart: Validating Feature 046

**Feature**: 046-foundations-rule-codification | **Date**: 2026-06-01

All FAKE-backed commands run **sequentially** (shared `.fake` state). This feature
**escalates** — run the full serialized six-target set.

## 0. Confirm the route (escalated)

```bash
./fake.sh build -t Route            # expect: escalated / maintainer-verify, full gate list
./fake.sh build -t Route --enforce  # fails if a required evidence artifact is missing
```

## 1. Constitution-Check gate (US1 / SC-001) — fail → fix → pass

```bash
# Unit tests (no build run needed — pure parser):
dotnet test tests/Governance.Tests --filter ConstitutionCheck

# Live gate on this feature's own plan.md (folded into GeneratedGuidanceCheck):
./fake.sh build -t GeneratedGuidanceCheck            # PASS with a complete plan.md
# Seed a violation: blank one required area in plan.md, rerun -> FAIL naming that area
# Restore -> PASS again.  (capture under readiness/seeded-violations/)
```

Expect the failure diagnostic to name the **exact area id** and the `plan.md` path; an
N/A-with-rationale area must pass; a future/renamed template must yield the distinct
`unrecognized template revision` diagnostic, not a false pass.

## 2. Versioned generated-product contract (US2 / SC-002, SC-003)

```bash
dotnet test tests/Governance.Tests --filter GeneratedProductContract   # deprecation-window transitions
./fake.sh build -t GeneratedProductCheck    # current generated project green; schema_version visible in output
```

`warn → promote → fail`: a product violating only a `Deprecated` rule passes with a warning
naming the removal version; after bumping `schema_version` and promoting the rule to
`Required`, the same product fails. The typed changelog records both transitions.

## 3. Prose trim (US3 / SC-004, SC-005, SC-006)

```bash
# Baseline (pre-trim) rule/guidance Markdown line count (~6,882 today, NOT 23,000 — see spec A2):
find .agents/skills .specify -name '*.md' | xargs wc -l | tail -1

# After deleting code-enforced rules (gate-before-prose, FR-008) and regenerating .claude:
./fake.sh build -t GeneratedGuidanceCheck   # .agents -> .claude byte-identity / currency stays green (FR-009)
find .agents/skills .specify -name '*.md' | xargs wc -l | tail -1   # record the delta
```

Each deleted rule must have a seeded-violation proof that its enforcing gate fails (FR-008);
record line/byte deltas + reproduction commands in `readiness/prose-delta.md`.

## 4. `.gitignore` evidence hygiene (US4 / SC-007)

```bash
# A freshly generated readiness zip/log is ignored:
git check-ignore -v specs/046-foundations-rule-codification/readiness/readiness.zip
# A previously-committed evidence file remains tracked (control):
git ls-files --error-unmatch <some/previously/committed/readiness-file>
```

No committed evidence is removed; no history is rewritten (D3 / FR-012).

## 5. Full serialized escalated set (SC-010)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit          # expect verdict=PASS, zero synthetic
```

## 6. Invariant guard (SC-009)

```bash
git diff --stat -- 'src/**'   # expect: empty (no product runtime / .fsi / baseline change)
```
