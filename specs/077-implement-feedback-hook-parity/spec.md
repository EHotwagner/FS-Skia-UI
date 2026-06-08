# Feature Specification: Implement-Phase Feedback Hook Parity

**Feature Branch**: `077-implement-feedback-hook-parity`  
**Created**: 2026-06-08  
**Status**: Draft  
**Input**: User description: "create specs for the feedback, add a fix for this feedback hook bug — the `speckit-implement` skill never honors its registered `after_implement` feedback hook the way sibling phase skills do, so per-phase feedback is silently skipped after implementation."

## Context & Problem

Spec Kit phases register declarative hooks (e.g. `after_implement`) in the
extension registry (`.specify/extensions.yml` plus per-extension files such as
`.specify/extensions/feedback/feedback.yml`). These are **agent-honored**
declarations, not Claude Code harness callbacks: the harness only auto-runs
hooks defined in `settings.json`, and it never reads the Spec Kit extension YAML.
A registered hook therefore only fires if the **phase skill** for that phase
instructs the agent to discover and run it.

Five phase skills (`specify`, `plan`, `clarify`, `analyze`, and `checklist`)
contain the modern multi-file hook-discovery block and so honor their hooks.
**Four** phase skills are deficient: `speckit-implement`, `speckit-tasks`, and
`speckit-constitution` contain **no** hook-discovery block at all, and
`speckit-taskstoissues` carries only a **legacy single-file** block (central
`extensions.yml` discovery with no per-extension enumeration and no consolidated
notice). Consequently, in a generated consumer project that installed the
feedback extension (observed in the sibling `Breakout1` dogfood project), the
`after_implement` feedback record was **silently not written** on implementation
completion; it was only captured later when the user explicitly asked. The
omission is invisible at runtime: no warning, no skipped-hook notice — the
feedback simply never happens. (Planning discovery widened the original
"implement + tasks" framing to all four deficient skills; see plan.md.)

This repository's own `extensions.yml` registers only `git` and `evidence`
hooks (no feedback), so the **feedback** defect does not currently *manifest*
here. The block gap is not entirely dormant locally, though: the blockless
`constitution` skill silently skips its **mandatory** `before_constitution`
`git.initialize` hook (benign only because this repo is already initialized).
The deficient phase skills are the canonical source from which consumer-project
skills derive, so the bug ships to every generated project that enables feedback.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Feedback is captured after implementation (Priority: P1)

As a developer running the Spec Kit lifecycle in a project that has the feedback
extension installed, when I complete the `/speckit-implement` phase, the
implement-phase feedback record is captured (or, for an optional hook, visibly
surfaced for me to run) — the same way it already is after `specify`, `plan`,
`clarify`, and `analyze` — so post-implementation feedback is never silently
dropped.

**Independent test**: In a project whose registry declares an `after_implement`
feedback hook, run the implement phase to completion and confirm a feedback
record is produced for the implement phase (or an explicit surfaced/optional or
skipped notice is emitted). Confirm the same run produces no *silent* omission.

### User Story 2 - Every phase honors its registered hooks (Priority: P2)

As a maintainer, I want **all** Spec Kit phase skills that can have registered
`before_*`/`after_*` hooks to perform the same hook discovery and the same
consolidated effective-hooks notice, so that no phase silently skips a registered
hook. This explicitly includes repairing `speckit-tasks`, which has the same
missing-block defect as `speckit-implement` (it captured feedback only by
happenstance, not because the skill instructed it).

**Independent test**: For each phase skill, confirm the skill text contains the
hook-discovery and effective-hooks-notice instructions; running any phase with a
registered hook for that phase produces the corresponding run/surface/skip
notice rather than nothing.

### User Story 3 - The fix reaches generated consumer projects (Priority: P3)

As a consumer who generates a new project with the feedback extension enabled, I
want the corrected phase skills, so that feedback capture works on the first run
without me having to patch skills by hand.

**Independent test**: Generate (or refresh) a consumer project with feedback
enabled and confirm its `speckit-implement` and `speckit-tasks` skills carry the
hook-discovery block; a full lifecycle run captures feedback for every phase
including implement.

### Edge Cases

- **No feedback extension installed** (this repo's current state): hook discovery
  finds no `after_implement` feedback hook and the phase completes normally with
  no feedback record — the discovery block must be a no-op, not an error, when no
  matching hook is registered.
- **Optional vs. mandatory hook**: an optional feedback hook must be *surfaced*
  ("To execute: …"), never force-run; a mandatory hook with
  `auto_execute_hooks: true` is auto-run. The implement/tasks behavior must match
  the precedence rules the sibling skills already follow.
- **Aborted / failed phase**: `after_*` semantics fire only on phase
  *completion*; an aborted or failed implement run writes no feedback record (no
  change to that existing behavior).
- **Disabled or condition-bearing hook**: a hook with `enabled: false` is
  skipped with a visible note; a hook with a non-empty `condition` is
  condition-deferred (not evaluated by the skill).
- **Multi-file hook discovery**: a feedback hook registered only in a
  per-extension file (`.specify/extensions/feedback/feedback.yml`) must be
  discovered, deduped by `(extension, command)`, and honored — identical to the
  sibling skills' discovery.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The `speckit-implement` phase skill MUST perform `before_implement`
  and `after_implement` hook discovery across all extension files (central
  `extensions.yml` plus every `.specify/extensions/*/*.yml`), merging and
  deduping by `(extension, command)`, exactly as the sibling phase skills do.
- **FR-002**: On implement-phase completion, the skill MUST honor each discovered
  `after_implement` hook per the established precedence: a mandatory hook
  (`optional: false`) auto-runs when `auto_execute_hooks: true`; an optional hook
  (`optional: true`) is surfaced and never force-run; a hook with a non-empty
  `condition` is condition-deferred; an `enabled: false` hook is skipped with a
  visible note.
- **FR-003**: The skill MUST emit a single consolidated "effective hooks for
  implement" notice listing each hook's resolved disposition (auto-run /
  surfaced / skipped / condition-deferred), so a skipped or surfaced hook is a
  *visible* decision rather than a silent omission.
- **FR-004**: The same hook-discovery and effective-hooks-notice behavior MUST be
  brought to **every** deficient phase skill, not just `speckit-implement`.
  Planning discovery (see plan.md) showed the defect spans four skills:
  `speckit-tasks` and `speckit-constitution` have **no** block (both must gain
  the modern multi-file block — note `constitution` thereby honors its
  **mandatory** `before_constitution` `git.initialize` hook), and
  `speckit-taskstoissues` has a **legacy single-file** block that MUST be
  upgraded to the modern multi-file form with the consolidated notice.
- **FR-005**: When no hook is registered for a phase, the discovery step MUST be
  a no-op that does not error and does not block phase completion.
- **FR-006**: A governance/validation check MUST fail when any Spec Kit phase
  skill that can carry registered hooks is missing its hook-discovery /
  effective-hooks-notice instructions, so this class of drift cannot silently
  reappear in a future phase skill.
- **FR-007**: The corrected phase skills MUST be authored in the canonical source
  (`.agents/skills/**`) and regenerated to the derived tree (`.claude/skills/**`)
  through the existing generation path so the two do not drift.
- **FR-008**: The fix MUST propagate to the skills carried by generated consumer
  projects, so a newly generated project with the feedback extension enabled
  captures implement-phase (and tasks-phase) feedback without manual patching.
- **FR-009**: The repaired skills MUST NOT change behavior when the feedback
  extension is absent: phases that previously completed with no feedback record
  continue to do so, with no new errors or prompts.

> Interacting / conflicting requirements: FR-002 (honor/auto-run hooks) vs.
> FR-009 (no behavior change when feedback is absent) are reconciled by FR-005 —
> the discovery block only *acts* when a matching hook is registered; with no
> feedback hook present it is a silent no-op. Auto-run applies only to mandatory
> hooks under `auto_execute_hooks: true`; optional hooks are always merely
> surfaced, never force-run, even under auto-execute.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, contents, or versions change. No
  controls/chart/graph/DataGrid authoring change. If the skill-drift guard is
  implemented inside `FS.Skia.UI.Build`, that governance assembly is rebuilt but
  its public identity is unchanged — confirmed in planning.
- **Public contract impact**: No `.fsi` signatures or documented public library
  APIs change. The changed surface is Spec Kit phase-skill text and a governance
  rule, not the F# public API.
- **State workflow impact**: No stateful workflow, I/O, command, effect,
  subscription, or interpreter behavior in the product library changes. The only
  "workflow" affected is the Spec Kit lifecycle's hook-honoring step.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering,
  screenshots, Vulkan, Skia, or visual output is touched.
- **Evidence obligations**: Behavioral proof that, in a project with a registered
  `after_implement` feedback hook, completing the implement phase yields a
  feedback record / surfaced notice (not silence); and that the drift guard fails
  on a phase skill with the block removed and passes when present.
- **Unsupported scope**: This feature is limited to **hook-honoring parity and
  its anti-drift guard**. It does NOT address the other Breakout1 feedback items
  (readiness-contract discoverability, a collision/physics skill, the
  `reserveHudBand` scaffold pointer, the missing `SymbolCrossCheck` target) —
  those are separate candidate features. It does not redesign the feedback
  capture content, the feedback file format, or the extension registry schema.
- **Build-target impact**: Likely touches the governance/validation target set
  that enforces skill currency (e.g. a `SkillSyncCheck`-adjacent rule and
  `RefreshSurfaceBaselines` regeneration). `GeneratedGuidanceCheck` /
  `TemplateCheck` may need to assert the corrected skills reach generated
  projects. Exact targets are a planning decision.

## Success Criteria *(mandatory)*

- **SC-001**: In a project with a registered `after_implement` feedback hook,
  100% of completed implement-phase runs produce a feedback record or an explicit
  surfaced/skip notice — zero silent omissions.
- **SC-002**: All Spec Kit phase skills that can carry registered hooks contain
  the hook-discovery and consolidated effective-hooks-notice instructions (0
  phase skills missing the block, where previously 4 were deficient — 3 with no
  block at all: `implement`, `tasks`, `constitution`; and 1 legacy single-file:
  `taskstoissues`).
- **SC-003**: A governance/validation run fails when any in-scope phase skill has
  its hook-discovery block removed, and passes when all are present — proving the
  drift cannot silently recur.
- **SC-004**: The canonical (`.agents`) and derived (`.claude`) skill trees are
  in sync after the change (no skill-sync drift reported).
- **SC-005**: A freshly generated consumer project with feedback enabled captures
  feedback for every lifecycle phase including implement and tasks, with no
  manual skill edits.
- **SC-006**: When no feedback hook is registered (this repository's current
  configuration), the lifecycle behaves exactly as before — no new errors,
  prompts, or feedback files.

## Assumptions

- **Fix mechanism (deferred to planning).** The user offered two options: (a) add
  the `after_implement` hook reference into the phase skill so the agent honors
  it like its siblings, or (b) add a real `settings.json` harness hook. The
  assumed direction is **(a)** — restore skill-text parity with the five working
  phase skills — because it matches the established, working pattern and because
  the feedback capture is itself a Spec Kit command skill, not a shell command
  that a `settings.json` hook can cleanly invoke. Option (b) is recorded as a
  rejected alternative; planning may revisit. The **anti-drift governance guard**
  (FR-006) is added on top of (a) so the parity cannot silently regress, which a
  pure skill-text edit alone would not guarantee.
- The defect spans four phase skills — `speckit-implement`, `speckit-tasks`,
  `speckit-constitution` (all blockless), and `speckit-taskstoissues` (legacy
  single-file). All four are repaired together since they share the same
  missing-/deficient-block root cause; the original "implement + tasks" framing
  was widened by planning discovery.
- `speckit-tasks` capturing feedback in the Breakout1 run despite lacking the
  block was incidental (agent-initiative), not evidence the skill is correct.
- The canonical authoring location is `.agents/skills/**` with `.claude/skills/**`
  generated from it; consumer-project skills derive from the same source through
  the existing template/generation path.
- "Phase skills that can carry registered hooks" for FR-006/SC-002 means the
  Spec Kit lifecycle phases that have `before_*`/`after_*` keys in the registry
  model (specify, clarify, plan, tasks, analyze, implement, checklist,
  taskstoissues, constitution); the guard's exact roster is finalized in planning.

## Out of Scope

- Readiness-contract discoverability for `EvidenceAudit` (separate Breakout1
  feedback item).
- A collision / fixed-timestep physics skill, and the `SkillSupport.Hud.
  reserveHudBand` scaffold pointer (separate Breakout1 feedback items).
- Wiring the `SymbolCrossCheck` analyzer target into generated `build.fsx`
  (separate Breakout1 `analyze`-phase feedback item).
- Changing the feedback capture prompts, the feedback record format, or the
  extension registry schema.

## Dependencies

- The Spec Kit extension model (`.specify/extensions.yml` and per-extension
  `.specify/extensions/*/*.yml`) and the feedback extension
  (`fs-skia-feedback-capture` command skill).
- The skill-generation / currency machinery that keeps `.claude` in sync with
  `.agents` (`RefreshSurfaceBaselines`, `SkillSyncCheck`) and the template /
  generated-product path that ships skills to consumer projects.
