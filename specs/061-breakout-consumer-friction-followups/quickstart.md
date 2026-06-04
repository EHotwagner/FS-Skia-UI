# Quickstart: Verifying 061 Consumer-Friction Follow-ups

**Feature**: `061-breakout-consumer-friction-followups`
**Date**: 2026-06-04

This walkthrough proves the user-visible outcomes. Run `./fake.sh build -t Route`
first and run only the gates it prints; FAKE-backed targets run **sequentially**
(shared `.fake` state). All skill edits land in canonical sources
(`.agents/skills/**` or `template/feedback/skill/SKILL.md`) and are regenerated
with `RefreshSurfaceBaselines` before validating.

## 0. Routing

```bash
./fake.sh build -t Route          # authoritative tier + minimal gate list
./fake.sh build -t Route --enforce  # additionally fail on missing evidence artifacts
```

Baseline at planning time printed `tier=focused-authority`, gates
`Dev, TemplateCheck, GeneratedProductCheck, GeneratedGuidanceCheck,
SkillContractPathCheck, TemplateDrift, EvidenceGraph`. The list **broadens** to
include `EvidenceAudit` once `build/Governance/**` and the readiness tree change —
re-run Route after each change-set.

## 1. Multi-file hook discovery + fourth prompt (FR-001/002/003 → SC-001/002)

Generate a project with feedback enabled, with the hook **only** in the
per-extension file:

```bash
dotnet new fs-skia-ui -n FrictionProbe --feedback true --allow-scripts yes </dev/null
ls FrictionProbe/.specify/extensions/feedback/feedback.yml   # present
test ! -e FrictionProbe/.specify/extensions.yml && echo "no central file — good"
```

Complete a phase (e.g. `/speckit-plan`) and confirm — **without an explicit
nudge** — that the `after_plan` feedback hook is discovered and a record is
written:

```bash
cat FrictionProbe/specs/<feature>/feedback/plan-*.md
# expect: front-matter severity + ## Process friction + ## Generalizable code
#         + ## Skill gaps  (the NEW fourth-prompt section)  + ## Research links
grep -c '^[1-4]\.' FrictionProbe/.../feedback/skill/SKILL.md   # → 4 prompts
grep -rn 'three prompts' specs/058-skills-quality-feedback/ template/ build/  # → no hits
```

Skipping the optional hook should print the FR-002 one-line "registered but not
run" notice.

## 2. Readiness contract discoverable without decompiling (FR-004/005 → SC-003/004)

In a fresh project with no passing sibling, trigger readiness-contract failures
and read the required shape straight from the audit:

```bash
./fake.sh build -t EvidenceAudit 2>&1 | sed -n '/readiness-contract:/,/missing:/p'
# expect per failing file: fileName + full required-tokens (+ fields/table header)
```

Confirm a passing audit is reachable using only that output — no
`FS.Skia.UI.Build.dll` decompilation, no sibling copy. Confirm the defect class
is one spelling (`product-defect`) across the audit and the source scan:

```bash
grep -rn 'product-defect' build/Governance/
grep -rn '.*-defect' build/Governance/ .specify/   # → no residual <project>-defect rule
```

## 3. Dev-vs-Test guidance + graph verdict line (FR-006/007 → SC-005)

```bash
grep -rn 'Dev' template/base/README.md template/base/docs/product.md
# expect: Dev = completion-marker/log target; Test/Verify (dotnet test) = real compile/test path
./fake.sh build -t EvidenceGraph 2>&1 | grep 'verdict=ok'
# expect: verdict=ok (no cycles, no dangling refs, no [S*])
```

## 4. Self-describing authoring templates (FR-008/009 → SC-006)

```bash
grep -n 'GeneratedGuidanceCheck' .specify/presets/fsharp-opinionated/templates/plan-template.md
# expect inline pass-criteria comment in the Repository Governance Decisions block
grep -n 'presets/fsharp-opinionated/templates' .agents/skills/speckit-tasks/SKILL.md
grep -n 'authoritative' .specify/templates/tasks-template.md   # pointer to preset copy
```

## 5. Consumer-internal DU pitfall (FR-010 → SC-007)

```bash
grep -n 'Launch' template/product-skills/fs-skia-keyboard-input/SKILL.md
# expect: GameMode.Launch vs Msg.Launch example + fully-qualified resolution
```

## 6. Arcade-helper triage recorded (FR-011 → SC-008)

```bash
cat specs/061-breakout-consumer-friction-followups/readiness/arcade-helper-triage.md
# expect one row per helper (fixed-step accumulator, collision/reflection,
# paddle rebound, reserveHudBand): disposition=document, home skill, reference
grep -rn 'reserveHudBand\|fixed-step' .agents/skills/fs-skia-layout-readability/ \
  src/Elmish/skill/ 2>/dev/null   # canonical conventions documented in-skill
```

## 7. Final gate sweep (SC-009)

Run the gates Route prints, sequentially. The escalated path is:

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
# also: SkillContractPathCheck, TemplateDrift, SkillSyncCheck, TargetMetadataDrift,
#       SkillQualityCheck — whichever Route lists after the change-set
```

`EvidenceAudit` must return `verdict=PASS` for
`specs/061-breakout-consumer-friction-followups`, and `SkillSyncCheck` /
`TargetMetadataDrift` / `SkillQualityCheck` stay green after `RefreshSurfaceBaselines`.

## Evidence artifacts (readiness/)

At minimum the Route-required escalated-tier artifacts —
`target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`,
`aggregate-hang-diagnostics.md` — plus:
- a verification log proving FR-001 (auto-fire) and FR-003 (fourth section);
- proof the FR-004 readiness requirements are recoverable without decompiling;
- `arcade-helper-triage.md` (per-helper ship-vs-document decisions, FR-011).
