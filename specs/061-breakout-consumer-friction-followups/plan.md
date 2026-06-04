# Implementation Plan: Breakout-Demo Consumer Friction Follow-ups & Feedback-Prompt Expansion

**Branch**: `061-breakout-consumer-friction-followups` | **Date**: 2026-06-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/061-breakout-consumer-friction-followups/spec.md`

## Summary

Consolidate the BreakoutDemo2 consumer-friction feedback (BD-1…BD-8) plus the new
fourth feedback prompt into one feature against the current merged state
(post-060, `7d4a06d`). The work is **governance, skill, and authoring-template
content** — no package identities change and (per D8) no new public `.fsi`
surface is added. Twelve FRs group into four workstreams:

1. **Discovery & feedback (FR-001/002/003)** — make every `/speckit-*` phase
   skill's hook-discovery **multi-file** (so the per-extension `feedback` hook is
   found and auto-fires), surface skipped optional hooks, and finish the 3→4
   feedback-prompt expansion across the skill, its record schema, the 058 sourcing
   contract, and every stale "three prompts" reference, pinned by a low-cost gate.
2. **Self-describing governance (FR-004/005/007)** — print the **full required
   shape** of each failing readiness file from the data that already enforces it
   (so no consumer must decompile `FS.Skia.UI.Build.dll`), resolve the
   defect-class to one spelling (`product-defect`), and give `EvidenceGraph` an
   explicit terminal `verdict=…` line.
3. **Authoring templates (FR-006/008/009)** — inline the `GeneratedGuidanceCheck`
   pass-criteria in the plan template, name the exact preset tasks-template path
   (generic copy points to it), and correct the `Dev`-vs-`Test`/`Verify` guidance
   in the generated quickstart.
4. **Skill content (FR-010/011)** — extend the duplicate-DU pitfalls note to the
   consumer-internal `GameMode.Launch` vs `Msg.Launch` case, and triage the four
   reusable arcade helpers as documented canonical conventions in `fs-skia-elmish`
   / `fs-skia-layout-readability` (not new SkillSupport API — see D8), recording
   the per-helper decision.

All design decisions deferred by the spec are resolved in
[research.md](./research.md) (D1–D9); entities in [data-model.md](./data-model.md);
interface contracts in [contracts/](./contracts/); verification in
[quickstart.md](./quickstart.md).

**Change classification: Tier 2 (internal/content change).** No public API
surface changes (D8 keeps arcade helpers as documented conventions, not shipped
`.fsi`). The change is consumer-contract-bearing (template/skill/governance),
so Route **escalates** the gate list even though no `.fsi`/baseline updates are
required. If the tasks phase reverses D8 and ships any helper, that helper's task
becomes Tier 1 and pulls in `.fsi` + surface-baseline updates.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); governance lib `FS.Skia.UI.Build`
**Primary Dependencies**: none new. Touches `build/Governance/**` (Evidence scans
+ front-end output), Spec Kit skill prose (`.agents/skills/**` → generated
`.claude/**`), authoring templates (`.specify/**`, `template/**`), and the
template-only feedback skill (`template/feedback/**`).
**Testing**: Expecto governance/unit tests; FAKE targets per Route; FSI/generated-
product evidence; a fresh `dotnet new fs-skia-ui --feedback true` project as the
FR-001/003/004 verification harness.
**Target Platform**: Windows and Linux (no runtime/rendering change).
**Routing baseline**: `tier=focused-authority`; gates `Dev, TemplateCheck,
GeneratedProductCheck, GeneratedGuidanceCheck, SkillContractPathCheck,
TemplateDrift, EvidenceGraph` — broadens to include `EvidenceAudit` (and skill-
sync gates) once governance/readiness files change. **Route is authoritative;
re-run after each change-set.** No `NEEDS CLARIFICATION` remains.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

Re-checked post-design: **PASS.** This is a Tier 2 content/governance change with
no stateful/IO runtime work (Principle IV N/A), no synthetic evidence
(Principle V N/A — all evidence is real: a generated project and real gate runs),
and `.fsi`/baselines untouched (Principle II — no public surface change, D8). The
Spec → FSI → tests → implementation order (Principle I) applies degenerately:
there is no new API to sketch in FSI; the "surface" being changed is governance
output and skill/template prose, exercised by real gate runs and a real generated
project (Principle VI test evidence). Idiomatic-simplicity (Principle III) is
satisfied — the FR-004 fix prints data already computed (no new abstraction), and
the hook-discovery change is a prose enumeration. Observability (Principle VII) is
directly *improved* by FR-004 (self-describing failures) and FR-007 (terminal
verdict).

### Repository Governance Decisions

- **Template ownership**: **Required.** Multiple template surfaces change and must
  be reflected in `.template.config/template.json` content/currency:
  `template/feedback/skill/SKILL.md` (fourth prompt), `template/feedback/extensions/feedback.yml`
  (unchanged but exercised), `template/base/README.md` + `template/base/docs/product.md`
  (FR-006 Dev-vs-Test), `template/product-skills/fs-skia-keyboard-input/SKILL.md`
  (FR-010), and the `.specify` authoring templates (FR-008/009). No new template
  *files* are added unless the optional FR-004 same-source readiness-template
  index backstop is taken (D2); if so, `template.json` `copyOnly`/currency is
  updated. `TemplateCheck`/`TemplateDrift`/`GeneratedProductCheck` are the
  enforcing gates.
- **Dependency impact**: **N/A — no dependency change.** No new NuGet packages,
  no `Directory.Packages.props` edit, no `docs/dependencies.md` / `DependencyReport`
  impact. Rationale: all work is content/prose/governance-output; no library is
  added or version-bumped beyond the standard template-pin bump at merge.
- **Command-surface impact**: **Required.** `build/Governance/**` changes touch
  three targets' output/behavior: `EvidenceGraph` (FR-007 verdict line),
  `EvidenceAudit` (FR-004 per-file readiness schema printout; FR-005 defect-class
  resolution), and a new/extended assertion for the FR-003 prompt count and FR-001
  multi-file discovery (low-cost checks, not new hard gates unless cheap). The
  authoritative gate list is `./fake.sh build -t Route`. FAKE-backed targets run
  **sequentially** (shared `.fake` state); deterministic order is the escalated
  six-target sequence (Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit) plus the Route-listed
  skill-sync gates.
- **Generated project impact**: **Required.** Generated projects change behavior:
  (a) phase skills auto-discover the per-extension feedback hook (FR-001) and
  surface skipped optional hooks (FR-002); (b) the feedback record gains a
  `## Skill gaps` section (FR-003); (c) generated quickstart states the
  Dev-vs-Test/Verify distinction (FR-006); (d) the keyboard pitfalls skill gains
  the consumer-internal DU example (FR-010); (e) `EvidenceAudit`/`EvidenceGraph`
  output is more self-describing (FR-004/007). No change to default/minimal
  generated *file set* (unless the D2 template-index backstop is taken).
- **Evidence paths**: All under
  `specs/061-breakout-consumer-friction-followups/readiness/` —
  `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`,
  `aggregate-hang-diagnostics.md` (Route-required escalated artifacts);
  `feedback-hook-autofire.md` (FR-001/003 verification log from a fresh
  `--feedback true` project, including the written `plan-*.md` record showing the
  fourth section); `readiness-recoverability.md` (FR-004 proof a passing audit is
  reached without decompiling); `arcade-helper-triage.md` (FR-011 per-helper
  decisions); gate logs under `readiness/logs/`.
- **`.fsi` / contract impact**: **N/A — no public surface change.** Rationale:
  per D8 the arcade helpers are documented conventions, not shipped API, so no
  `.fsi` signature, surface-baseline, or compatibility note changes. The FR-004/007
  changes are to governance *output strings*, not public library signatures. If
  tasks reverse D8 and ship a helper, that task escalates to Tier 1 and adds the
  helper's `.fsi` + per-module surface baseline.
- **MVU/effect boundary**: **N/A — no stateful/IO workflow added.** Rationale:
  the governance-output and skill/template edits are pure content changes; the
  arcade helpers under discussion (fixed-step accumulator, collision, rebound)
  are *documented as* pure `update`-side game-loop conventions for consumers, not
  implemented as framework runtime here, so no `Model`/`Msg`/`Effect`/interpreter
  is introduced by this feature.
- **Synthetic evidence**: **None planned — all evidence is real.** Rationale: the
  FR-001/003 proof is a real generated project completing a real phase; FR-004 is
  a real `EvidenceAudit` run; gate evidence is real target output. No mocks,
  fakes, fixtures, or in-memory substitutes; no `[S]`/`[SEH]` tasks anticipated.
  Should any task discover an unavoidable synthetic dependency, Principle V
  disclosure (`[S]` + code/test/spec/PR banners) applies and the audit blocks
  merge until resolved.
- **Test evidence**: Failing-first governance tests: a test pinning the
  `EvidenceGraph` `verdict=ok (...)` token (fails before FR-007), a test pinning
  the per-file readiness schema printout (fails before FR-004), and a
  prompt-count/`## Skill gaps` assertion (fails before FR-003). Plus the generated-
  project smoke harness for FR-001 auto-fire. Governance tests live alongside the
  existing `build/Governance` test suites (Governance.Tests / behavior split).
- **Observability**: This feature is largely an observability improvement.
  Diagnostics: FR-004 makes readiness-contract failures self-describing (file +
  full required tokens/fields/table, not a bare count); FR-007 adds an explicit
  graph verdict line; FR-002 surfaces skipped optional hooks. Log paths unchanged
  (`readiness/logs/<Target>.txt`). Missing-artifact-class failures continue to
  hard-block via the existing readiness contract; no silent failure introduced.
- **Deferred scope**: Shipping the arcade helpers as real `FS.Skia.UI.SkillSupport`
  (or a new GameKit) public API is **explicitly deferred** — D8 documents them as
  canonical conventions; a future feature may promote them (the convention doc is
  its spec). The optional FR-004 same-source template-index backstop (D2) is
  deferred unless cheap. No release/distribution/external-split work. FR-002 and
  FR-011 are delivered as guidance, not new hard merge gates, unless a low-cost
  executable check emerges.

## Project Structure

Edited / authored paths for this feature (project-relative):

```
specs/061-breakout-consumer-friction-followups/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/{hook-discovery,feedback-record,readiness-diagnostic}.md
  readiness/{target-metadata,agent-ready-verdict,skill-loading-evidence,
             aggregate-hang-diagnostics,feedback-hook-autofire,
             readiness-recoverability,arcade-helper-triage}.md

# FR-001/002 — multi-file hook discovery (canonical → generated)
.agents/skills/speckit-{specify,clarify,plan,tasks,analyze,checklist,implement}/SKILL.md
.claude/skills/speckit-*/SKILL.md            # regenerated via RefreshSurfaceBaselines

# FR-003 — fourth feedback prompt (template-only skill + 058 contract + stale refs)
template/feedback/skill/SKILL.md
specs/058-skills-quality-feedback/contracts/feedback-capture.md
specs/058-skills-quality-feedback/{spec.md,research.md,plan.md,tasks.md,
                                   readiness/template-feedback-true.md,readiness/task-graph.*}

# FR-004/005/007 — self-describing governance output
build/Governance/Evidence/Scans.fs           # surface required tokens on failure; defect-class spelling
build/Governance/Evidence/Audit.fs           # carry per-file shape through to the verdict
build/Governance/Front/Governance.fs         # print per-file schema; EvidenceGraph verdict line
build/Governance/SkillQuality.fs             # (FR-003) optional prompt-count assertion
build/.../*.Tests                            # failing-first governance tests

# FR-006/008/009 — authoring templates
.specify/presets/fsharp-opinionated/templates/{plan-template.md,tasks-template.md,tasks-deps-template.yml}
.specify/templates/{plan-template.md,tasks-template.md}   # generic copies: pointer to preset
.agents/skills/speckit-tasks/SKILL.md         # name exact preset paths (FR-009)
template/base/README.md  template/base/docs/product.md    # Dev-vs-Test (FR-006)

# FR-010 — consumer-internal DU pitfall
template/product-skills/fs-skia-keyboard-input/SKILL.md

# FR-011 — arcade-helper conventions (documented, not shipped)
.agents/skills/fs-skia-layout-readability/SKILL.md         # reserveHudBand
src/Elmish/skill/SKILL.md                                  # fixed-step / collision / rebound
```

## Phase sequencing notes

- **FR-003 is partly in-flight**: the working tree already carries the 3→4
  wording in `template/feedback/skill/SKILL.md` and the 058 contract — the tasks
  phase finishes the stale-reference sweep (D6) and adds the count assertion.
- **Regenerate before validating**: any `.agents/skills/**` edit must be followed
  by `RefreshSurfaceBaselines` so `.claude/**` mirrors it (`SkillSyncCheck`).
- **Route after each workstream** and run only the gates it prints; never run
  FAKE-backed targets concurrently.
- **D8 reversibility gate**: if the tasks phase elects to *ship* any arcade helper
  instead of documenting it, that task re-classifies to Tier 1 and must add the
  `.fsi` + surface baseline; flag it at task generation, do not relabel at
  implementation time.
