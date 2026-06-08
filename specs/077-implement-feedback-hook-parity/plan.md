# Implementation Plan: Implement-Phase Feedback Hook Parity

**Branch**: `077-implement-feedback-hook-parity` | **Date**: 2026-06-08 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/077-implement-feedback-hook-parity/spec.md`

## Summary

Five Spec Kit phase skills (`specify`, `plan`, `clarify`, `analyze`, `checklist`)
already carry the **modern** hook-discovery block: multi-file discovery across the
central `.specify/extensions.yml` plus every `.specify/extensions/*/*.yml`, merge
+ dedupe by `(extension, command)`, and a consolidated **"Effective hooks for
{phase}"** notice. Discovery during planning showed the defect is broader than the
spec's "implement + tasks" framing:

| Phase skill | Current block state |
| --- | --- |
| `specify`, `plan`, `clarify`, `analyze`, `checklist` | modern (compliant) |
| `taskstoissues` | **legacy single-file** (central `extensions.yml` only; no multi-file discovery; no consolidated notice) |
| `implement`, `tasks`, `constitution` | **none at all** |

`constitution` is blockless despite carrying the **mandatory** `before_constitution`
`git.initialize` hook, so it silently skips it the same way `implement` skips
`after_implement` feedback.

> **Compliance-assumption risk (R1).** The five skills marked *modern* above are
> assumed to satisfy all **three strict** guard markers (`.specify/extensions/*/*.yml`
> ≥ 2×, `(extension, command)` dedupe language, exact `## Effective hooks for
> <phase>` heading). The first check is the positive full-corpus assertion (T016);
> there is **no dedicated repair task** for these five if one unexpectedly fails a
> strict marker. Contingency: if T016 fails on a skill assumed compliant, repair it
> in place (same modern-block edit as T013/T017–T019) and re-run
> `RefreshSurfaceBaselines` before proceeding — treat it as an in-scope discovery,
> not new scope. (Spot-check at planning: `analyze` satisfies all three.)

**Approved scope (clarified during planning):** bring **all four** deficient
phase skills — `implement`, `tasks`, `taskstoissues` (legacy→modern), and
`constitution` — up to the modern multi-file block, so all **nine** lifecycle
phases (`specify`, `clarify`, `plan`, `tasks`, `analyze`, `implement`,
`checklist`, `taskstoissues`, `constitution`) honor their registered hooks
identically.

**Approach:** skill-text parity (spec Assumption option **(a)**) — author the
modern block in the canonical `.agents/skills/<phase>/SKILL.md`, regenerate the
derived `.claude/skills/**` tree through `RefreshSurfaceBaselines`. On top of the
text fix, add a **strict** anti-drift governance guard (FR-006) — a new
`PhaseHookParity` rule in `FS.Skia.UI.Build` plus a routed `PhaseHookParityCheck`
gate and a failing-first `Governance.Tests` test — that fails when any in-scope
phase skill lacks the modern markers (multi-file enumeration of
`.specify/extensions/*/*.yml` **and** the `## Effective hooks for {phase}`
notice). The skill copies reach generated consumer projects unchanged through the
existing `template.json` `.agents/skills/**/*` glob (no `template.json` edit
needed).

**Tier:** Tier 1 (contracted change) — it adds a new governance gate to the
public validation contract (`validation.contract.yml`) and to
`AgentValidation.knownGates`, and changes consumer-facing Spec Kit skill text.
No F# product-library `.fsi` surface changes.

## Technical Context

**Language/Version**: F# / .NET (`net10.0`); governance assembly `FS.Skia.UI.Build`
under `build/Governance/**`
**Primary Dependencies**: Existing governance stack — `Findings`, `Routing`,
`Targets`, `AgentValidation`, the `Engine` (`Update`/`Interpret`) MVU loop,
Expecto for `Governance.Tests`. No new package dependency.
**Testing**: Expecto (`tests/Governance.Tests/`), FAKE targets
(`PhaseHookParityCheck`, `SkillSyncCheck`, `TemplateCheck`,
`GeneratedProductCheck`), plus the regenerated `.claude` tree verification.
**Target Platform**: Windows and Linux (governance build, platform-agnostic text +
F#).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: No `.template.config/template.json` edit is required.
  Phase-skill SKILL.md files already ship to generated projects via the existing
  `.agents/skills/**/*` copy-only glob (and the parallel `.claude/skills/**` copy);
  editing canonical `.agents/skills/<phase>/SKILL.md` and regenerating `.claude`
  is sufficient for propagation. No new template file, capability, or
  command-surface entry is added.
- **Dependency impact**: N/A — no dependency change. No `Directory.Packages.props`,
  `docs/dependencies.md`, or `DependencyReport` change; no new NuGet package.
  `FS.Skia.UI.Build` is rebuilt and packed (its assembly changes) but takes no new
  dependency; per [[build-package-version-drift-gotcha]] the Build package version
  is bumped and packed alongside any src libs touched (none here, so Build alone).
- **Command-surface impact**: One new FAKE target `PhaseHookParityCheck` is added
  to `build.fsx`/`scripts/build/**` and to `FS.Skia.UI.Build` (`Targets.fs` variant
  + `allTargets` + `name` + `directPrerequisites` + metadata; `AgentValidation.knownGates`;
  `Engine/Update.fs` + `Engine/Interpret.fs` effect wiring; `Routing.fs`
  `skill-quality` rule `RequiredGates`). `validation.contract.yml` is regenerated
  from `Routing.fs` (`TargetMetadataDrift` enforces currency). FAKE-backed commands
  share `.fake` state and run sequentially; deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t PhaseHookParityCheck`
  3. `./fake.sh build -t GeneratedGuidanceCheck`
  4. `./fake.sh build -t TemplateCheck`
  5. `./fake.sh build -t GeneratedProductCheck`
  6. `./fake.sh build -t EvidenceGraph`
  7. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: Generated projects receive the four corrected
  phase skills through the unchanged copy globs; no change to default/minimal
  generated contents, selected-Controls guidance, placeholder/excluded-history
  scans, or generated `Dev` behavior beyond the corrected skill text. The guard
  runs in **this** repo's governance build, not in generated projects.
- **Evidence paths**: `readiness/phase-hook-parity-check.md` (guard report; PASS
  list + any failing skill/marker), `readiness/skill-sync.md` (`.agents`↔`.claude`
  currency after regeneration), `readiness/template-check.md` /
  `readiness/generated-product-check.md` (corrected skills present in generated
  output), and the `Governance.Tests` red→green log under the feature
  `readiness/` for the failing-first guard test. All paths are
  `specs/077-implement-feedback-hook-parity/readiness/`.
- **`.fsi` / contract impact**: New governance module
  `build/Governance/PhaseHookParity.fs` ships with a curated
  `PhaseHookParity.fsi` (Principle II — every public Build module has an `.fsi`;
  42 already exist). The machine-readable **validation contract**
  (`validation.contract.yml`) changes: a new `PhaseHookParityCheck` gate entry on
  the `skill-quality` routing rule. No product-library public `.fsi` changes.
- **MVU/effect boundary**: No *new* MVU surface. The guard is a **pure** function
  `PhaseHookParity.checkCorpus : ParsedPhaseSkill list -> Findings.ValidationFinding
  list` (plus `renderReport`), interpreted through the **existing** governance
  Engine boundary: a new `Msg` (`StartTarget PhaseHookParityCheck`) in
  `Engine/Update.fs` emits a `PhaseHookScan` effect that `Engine/Interpret.fs`
  executes (reads the SKILL.md files) and folds back. Principle IV is satisfied by
  reusing the existing pure-`update` / edge-interpreter split; no filesystem access
  occurs inside the pure check.
- **Synthetic evidence**: The guard's **positive** evidence is real — the guard
  runs against the actual repaired `.agents/skills/<phase>/SKILL.md` files and
  passes. The **negative/failing-first** test feeds a synthetic block-stripped
  skill body (an in-memory string with the modern markers removed) to prove the
  guard fails on a deficient skill; this is an explicit error-path fixture and is
  a candidate `[SEH]` (`synthetic-error-handling-approved`) — keeping a
  permanently-broken real skill in the tree is infeasible. The `[SEH]`
  classification is decided at **task generation** (Principle V forbids
  implementation-time relabeling); the row will name design source, synthetic
  input class (block-stripped SKILL.md body), expected error behavior (guard
  emits a finding / target fails), and `accepted-seh` status.
- **Test evidence**: Failing-first `Governance.Tests/PhaseHookParityTests.fs`:
  (1) positive — every roster phase skill (and its `.claude` mirror) passes the
  modern-marker check; (2) negative — a block-stripped body yields a finding
  (red before the guard logic exists / green after). A `SkillSyncCheck`-backed
  assertion confirms `.agents`↔`.claude` parity after `RefreshSurfaceBaselines`.
  `TemplateCheck`/`GeneratedProductCheck` confirm the corrected skills reach
  generated output.
- **Observability**: The guard emits an actionable report
  (`readiness/phase-hook-parity-check.md`) listing each in-scope phase skill as
  PASS/FAIL with the **named missing marker** (e.g. "missing `## Effective hooks
  for tasks` notice" or "missing multi-file `.specify/extensions/*/*.yml`
  enumeration"), and fails loudly (`failwith`) — no silent pass. The roster of
  in-scope phases is explicit data, so an unrecognized/absent phase skill is a
  named failure, not a silent skip.
- **Deferred scope**: Out of scope (separate Breakout1 follow-ups, per spec):
  readiness-contract discoverability for `EvidenceAudit`, a collision/fixed-
  timestep physics skill, the `SkillSupport.Hud.reserveHudBand` scaffold pointer,
  wiring `SymbolCrossCheck` into generated `build.fsx`, and any change to the
  feedback capture prompts / record format / extension registry schema. The
  feedback **extension** itself (`template/feedback/**`) is not modified; this
  feature only makes the phase skills honor whatever hooks are registered.

**Initial Constitution Check: PASS.** No principle violation. Tier 1 obligations
(`.fsi` for the new Build module, validation-contract update, surface/baseline
currency, failing-first tests) are all planned. Simplicity (Principle III): the
new full FAKE target (vs. a test-only check) is justified because FR-006 requires
the guard to run as a **routed gate** on `.agents/skills/**` changes — a
`Governance.Tests`-only check would not be guaranteed to run via `Route` on a
skill-only diff — and the repo's "single home of all rules" convention requires
target registration. The guard logic itself is one pure function over a fixed
roster; no clever F# features are used.

## Project Structure

```
specs/077-implement-feedback-hook-parity/
  spec.md
  plan.md                # this file
  research.md            # Phase 0 — decisions & alternatives
  data-model.md          # Phase 1 — entities (roster, markers, findings)
  contracts/
    phase-hook-parity.md # Phase 1 — the guard contract (roster, markers, gate)
    modern-hook-block.md # Phase 1 — the canonical block each skill must contain
  quickstart.md          # Phase 1 — how to run/verify the guard
  readiness/             # evidence (created during /speckit.implement)

# Skill text (canonical source — edited; .claude regenerated)
.agents/skills/speckit-implement/SKILL.md      # add modern block (pre+post)
.agents/skills/speckit-tasks/SKILL.md          # add modern block (pre+post)
.agents/skills/speckit-taskstoissues/SKILL.md  # upgrade legacy -> modern
.agents/skills/speckit-constitution/SKILL.md   # add modern block (pre+post)
.claude/skills/speckit-*/SKILL.md              # regenerated via RefreshSurfaceBaselines

# Governance guard (new rule + gate)
build/Governance/PhaseHookParity.fsi           # new — curated public surface
build/Governance/PhaseHookParity.fs            # new — pure roster + marker check
build/Governance/Governance.fsproj             # add the two compile entries
build/Governance/Targets.fs                    # PhaseHookParityCheck variant + metadata
build/Governance/AgentValidation.fs            # add to knownGates
build/Governance/Routing.fs                    # add gate to skill-quality RequiredGates
build/Governance/Engine/Update.fs              # StartTarget case + PhaseHookScan effect
build/Governance/Engine/Interpret.fs           # PhaseHookScan handler
build/Governance/Front/Governance.fs           # runPhaseHookParityCheck entrypoint + report
validation.contract.yml                        # regenerated from Routing.fs
build.fsx / scripts/build/**                    # register the FAKE target

# Tests
tests/Governance.Tests/PhaseHookParityTests.fs # new — failing-first positive+negative
```

## Phase 0 / Phase 1 outputs

- Phase 0: [research.md](./research.md)
- Phase 1: [data-model.md](./data-model.md), [contracts/](./contracts/),
  [quickstart.md](./quickstart.md); AGENTS.md plan reference updated to this plan.

**Post-Design Constitution Re-check: PASS** — design introduces a pure check
behind the existing effect boundary, a curated `.fsi`, a regenerated validation
contract, and failing-first tests; no new principle tension.
