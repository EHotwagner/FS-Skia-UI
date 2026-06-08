# Contract: `PhaseHookParityCheck` gate (FR-006)

The anti-drift governance guard. A new rule in `FS.Skia.UI.Build`, surfaced as a
routed FAKE target, that fails when any in-scope Spec Kit phase skill lacks the
modern hook-discovery block.

## Roster (in-scope phases)

```
specify  clarify  plan  tasks  analyze  implement  checklist  taskstoissues  constitution
```

Derived from the `before_*`/`after_*` keys in `.specify/extensions.yml`. A roster
phase whose `.agents/skills/speckit-<phase>/SKILL.md` (or its `.claude` mirror) is
absent/unreadable ⇒ **named failure**.

## Pass criteria (strict — all three markers per in-scope skill)

1. `.specify/extensions/*/*.yml` appears **≥ 2×** (pre + post multi-file discovery).
2. `(extension, command)` dedupe language present.
3. `## Effective hooks for <phase>` consolidated notice present.

A legacy single-file block (central `extensions.yml` only) ⇒ FAIL. Total absence
⇒ FAIL. Per-marker, per-skill failures are named in the report.

## Gate wiring (single home of all rules)

| Concern | File | Edit |
| --- | --- | --- |
| Pure logic | `build/Governance/PhaseHookParity.fs` (+ `.fsi`) | new module: `roster`, `checkCorpus`, `renderReport` |
| Compile order | `build/Governance/Governance.fsproj` | add `.fsi`+`.fs` after `Findings`, before `Routing` |
| Target variant | `build/Governance/Targets.fs` | `\| PhaseHookParityCheck` in DU + `allTargets` + `name` + `directPrerequisites [Build]` + timeout/cost/owner metadata |
| Known gates | `build/Governance/AgentValidation.fs` | add `"PhaseHookParityCheck"` to `knownGates` |
| Routing | `build/Governance/Routing.fs` | add `Targets.PhaseHookParityCheck` to the `skill-quality` rule `RequiredGates` |
| Effect loop | `build/Governance/Engine/Update.fs` | `StartTarget PhaseHookParityCheck` → `PhaseHookScan` effect |
| Interpreter | `build/Governance/Engine/Interpret.fs` | `PhaseHookScan` handler: read roster SKILL.md → `checkCorpus` → write report → `failwith` if findings |
| Front entry | `build/Governance/Front/Governance.fs` | `runPhaseHookParityCheck` (mirrors `SkillQualityCheck` runner) |
| Contract | `validation.contract.yml` | regenerated from `Routing.fs` via `RefreshSurfaceBaselines` (`TargetMetadataDrift` enforces currency) |
| FAKE target | `build.fsx` / `scripts/build/**` | register `PhaseHookParityCheck` |

## Report contract (`readiness/phase-hook-parity-check.md`)

```
# Phase-hook parity check (PhaseHookParityCheck)

Checked 9 in-scope phase skill(s).
- PASS: <n>
- FAIL: <m>

## Failing skills           (only when m > 0)
- `tasks` — missing: ## Effective hooks for tasks notice, multi-file .specify/extensions/*/*.yml enumeration

## Per-skill results
- ✅ `specify`
- ❌ `tasks` — missing: …
```

Failure exits non-zero (`failwith`) after the report is written — no silent pass.

## Routing expectation

A change to `.agents/skills/**` (the `skill-quality` rule) escalates to
`FocusedAuthority` and now requires `PhaseHookParityCheck` alongside
`SkillQualityCheck`, `SkillSyncCheck`, `SkillContractPathCheck`,
`TemplateUpdateSkillPackageCheck`. Verify with `./fake.sh build -t Route`.
