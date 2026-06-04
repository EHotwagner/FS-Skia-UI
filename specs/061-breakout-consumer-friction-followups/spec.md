# Feature Specification: Breakout-Demo Consumer Friction Follow-ups & Feedback-Prompt Expansion

**Feature Branch**: `061-breakout-consumer-friction-followups`
**Created**: 2026-06-04
**Status**: Draft
**Input**: User description: "create specs from the feedback from the feedback hook in the sibling repo breakoutdemo2 in specs/001.../feedback... also add a new question to the feedback hook along the line: what additional/new skills would have been helpful during this phase."

## Context & Triage *(informative)*

A consumer implemented the Breakout arcade demo in a generated `FS.Skia.UI`
project (`BreakoutDemo2`) on packages `0.1.64-preview.1` / template `0.1.83` —
the exact artifacts feature **060** merged (`7d4a06d`). The per-phase feedback
hook captured three records under
`BreakoutDemo2/specs/001-breakout-game/feedback/` (`plan-2026-06-04.md`,
`tasks-2026-06-04.md`, `implement-2026-06-04.md`). Because the project was
generated from the post-060 packages, every finding below is against the
**current merged state**, not a stale build. 060's own deliverables landed and
were used (the consumer grounded API discovery in the emitted
`docs/api-surface/**` `.fsi` tree that 060's FR-003 shipped) — these are the
*next* layer of friction.

Each finding is triaged against feature `060-asteroids-consumer-friction-followups`
(the last merged feature):

| # | Sev | Finding | Status vs. 060 / current source |
|---|-----|---------|----------------------------------|
| BD-1 | major | The per-phase `feedback` capture hook never auto-fires; it ran only after the user explicitly asked, at **three consecutive phases** (plan/tasks/implement). The hook is registered (`enabled: true`) but lives in `.specify/extensions/feedback/feedback.yml`, while every `/speckit-*` phase skill's documented hook-discovery scans only the single file `.specify/extensions.yml`, so the hook is invisible; being `optional: true`, the omission is silent. | **Open & new.** 060 did not touch hook discovery. Confirmed in current source: `.claude/skills/speckit-plan/SKILL.md` (and the other phase skills, including specify) instruct a single-file `.specify/extensions.yml` scan, and the template ships the feedback hook separately under `template/feedback/extensions/feedback.yml`. Systemic across all `after_*` phases. |
| BD-2 | blocker | `EvidenceAudit` hard-failed repeatedly on a readiness-contract grammar of **exact literal tokens / file names / field lists** that no skill, template, or contract documents and that the failing audit does not print in full. The consumer resolved it only by (a) extracting UTF-16 string literals from the compiled `FS.Skia.UI.Build.dll` and (b) copying a passing sibling project (`BreakoutDemo1`). A project with no sibling and no willingness to decompile would be stuck. | **Open & new.** Highest impact. The engine clearly knows the grammar (it emits `skill-loading-evidence.template.md` for one file) but ships templates for none of the others and the diagnostics name the missing token but never the full required shape per file. |
| BD-2a | minor | Within BD-2, the **same concept has two required spellings in two gates**: the readiness audit requires the class `product-defect` in `window-state-diagnostics.md`, while the source governance scan requires `breakoutdemo2-defect` (the project-name-prefixed class) for the same idea. | **Open & new.** Two gates, two spellings, one concept. |
| BD-3 | minor | `./fake.sh build -t Dev` only writes `readiness/logs/Dev.txt` — it does not compile. The real compile/test path is `Test`/`Verify` (`dotnet test`). Tasks/quickstart say "build with `Dev`", which is misleading; the consumer compiled directly with `dotnet build`/`dotnet test` to get real error feedback. | **Open & new.** Quickstart/tasks guidance only. |
| BD-4 | minor | Consumer-internal DU collision: `GameMode.Launch` and `Msg.Launch` both declared; a bare `Launch` binds to the last-declared type (`Msg`), producing ten misleading "expected GameMode but has type Msg" errors. Fixed by fully qualifying. | **Partially covered by 060 FR-007.** 060 added a duplicate-DU-case pitfalls note, but its example is *framework-vs-framework* (`ViewerKey.Unknown` vs `ViewerRunBlockedStage.Unknown`). The consumer-internal cross-module case (the consumer's own two DUs) is not called out. Extend, don't re-add. |
| BD-5 | minor | The `plan.md` template ships the *Repository Governance Decisions* block as boilerplate bullets, but the machine-enforced `GeneratedGuidanceCheck` pass criteria (no empty/boilerplate/`NEEDS CLARIFICATION`/`TODO`; `N/A`-with-rationale counts as filled) is documented only in the skill's "Key rules", not in the template the author edits. | **Open & new.** A boilerplate bullet silently fails the build later. |
| BD-6 | minor | `speckit-tasks` SKILL.md says "start from the preset's `tasks-template.md` / `tasks-deps-template.yml`" but never states the path, and **two copies exist** (`.specify/templates/tasks-template.md` and `.specify/presets/fsharp-opinionated/templates/{tasks-template.md,tasks-deps-template.yml}`). An operator could pick the wrong (generic) copy. | **Open & new.** Both copies confirmed present in current source. |
| BD-7 | minor | A successful `./fake.sh build -t EvidenceGraph` prints only `feature-source`/`feature-directory`/`tasks=N` with no `OK`/`PASS`/`no cycles` line; success must be inferred from exit code `0`. | **Open & new.** A terminal verdict line would make a clean pass self-evident. |
| BD-8 | n/a (generalizable) | Game-agnostic helpers re-implemented every arcade demo, flagged as `FS.Skia.UI.SkillSupport` candidates: **fixed-step accumulator** (`1/120 s`, capped steps/tick) deterministic `step` driver; **AABB / circle-vs-rect collision + single-reflection-per-step** resolution (axis by normalized penetration); **paddle-rebound angle with a `|Dy|` floor**; **HUD-band reservation** (`gameplayRegion = surface − reserved band`, clamp gameplay, overdraw HUD last → a `reserveHudBand` helper). | 060 FR-008 added the HUD/gameplay *pattern doc* to `fs-skia-layout-readability`; the ask here is the reusable **helper**, plus the `fs-skia-elmish` MVU helpers. Triage for SkillSupport. |
| USER | n/a | **Add a fourth feedback prompt** to the capture hook: "what additional/new skills would have been helpful during this phase." | New deliverable. Expands the skill's prompt set 3 → 4 and the record schema. |

**Scope note.** Per the house pattern (one consolidated "consumer friction
follow-ups" feature per demo, e.g. 060/034/022) and the single-feature rule, this
is **one** feature consolidating all BreakoutDemo2 feedback plus the new feedback
prompt — not one spec per friction item.

**Change classification.** **Tier 2 (internal/content change)** — governance,
skill, and authoring-template content; no package identities change and no public
`.fsi` surface is added (the arcade helpers are documented as conventions, not
shipped — see plan D8). If the tasks/implementation phase reverses that decision
to *ship* any helper, that helper's task escalates to **Tier 1** and pulls in its
`.fsi` + per-module surface baseline.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Per-phase feedback is captured automatically (Priority: P1)

A consumer working a feature in a generated project finishes a Spec Kit phase
(plan/tasks/implement/…). The registered `feedback` capture hook fires on phase
**completion** without the user having to ask, because the phase skill's
hook-discovery sees hooks regardless of which file ships them. The captured
record now also asks what additional/new skills would have helped.

**Independent test**: In a generated project with the feedback extension present
only under `.specify/extensions/feedback/feedback.yml`, complete a phase and
confirm the `after_<phase>` feedback hook is discovered and run (a record written
to `specs/<feature>/feedback/<phase>-<date>.md`) without an explicit user nudge,
and that the record contains the new fourth section.

### User Story 2 - Readiness-contract requirements are discoverable without decompiling (Priority: P1)

A consumer hitting an `EvidenceAudit` readiness-contract failure can learn the
**full required shape** of each failing readiness file — its name, required
tokens/fields, and table structure — from a shipped template or from the audit's
own diagnostics, and never needs to decompile `FS.Skia.UI.Build.dll` or copy a
sibling project. The same concept is required under a single consistent spelling
across the audit and the source governance scan.

**Independent test**: In a freshly generated project with no passing sibling,
trigger the readiness-contract failures and confirm the required files, tokens,
fields, and table shapes are obtainable from shipped templates and/or the audit
output alone; confirm the defect-class concept resolves to one consistent
spelling (or the two spellings are documented as deliberately distinct).

### User Story 3 - Accurate build/test and graph-verdict guidance (Priority: P2)

A consumer reads the generated quickstart/tasks (`README.md` / `docs/product.md`)
and understands that `Dev` is a completion-marker/log target while `Test`/`Verify`
(`dotnet test`) is the authoritative compile/test path that yields real error
feedback; and a clean `EvidenceGraph` run prints an explicit verdict so success is
self-evident.

**Independent test**: Read the generated `README.md` / `docs/product.md` and
confirm they state the `Dev`-vs-`Test`/`Verify` distinction; run `EvidenceGraph`
on a clean graph and confirm an explicit terminal verdict line is printed.

### User Story 4 - Self-describing authoring templates (Priority: P2)

An author editing `plan.md` sees the `GeneratedGuidanceCheck` pass criteria
inline in the Governance Decisions block, and an author generating tasks is told
the exact preset-relative template path, so neither leaves a boilerplate bullet
that fails later nor picks the wrong duplicate template.

**Independent test**: Confirm the `plan.md` template carries the pass-criteria as
an inline comment; confirm `speckit-tasks` SKILL.md names the
`.specify/presets/fsharp-opinionated/templates/…` path and the generic
`.specify/templates/tasks-template.md` carries a pointer to the authoritative
preset copy.

### User Story 5 - Pitfalls note covers consumer-internal DU collisions (Priority: P2)

A consumer following the capability-skill pitfalls is warned that duplicate DU
case names — including across the consumer's **own** co-opened modules
(`GameMode.Launch` vs `Msg.Launch`), not just framework-vs-framework — bind a
bare case to the last-declared type, with the fully-qualified resolution.

**Independent test**: Read the duplicate-DU pitfalls note and confirm it covers
the consumer-internal cross-module collision case with the qualification remedy.

### User Story 6 - Reusable arcade helpers triaged into SkillSupport (Priority: P3)

A consumer building any arcade demo can reach the deterministic game-loop and
collision/layout primitives from `FS.Skia.UI.SkillSupport` (or a clearly
documented skill convention) instead of re-implementing the fixed-step
accumulator, collision/reflection, paddle rebound, and HUD-band reservation each
time.

**Independent test**: Confirm each named helper is either shipped in
`FS.Skia.UI.SkillSupport` with a skill reference, or explicitly documented in the
relevant skill as the canonical convention, with the planning phase recording the
ship-vs-document decision per helper.

### Edge Cases

- A future extension ships its hooks in its own `.specify/extensions/<ext>/<ext>.yml`
  file → multi-file discovery must find it too, deduped by `(extension, command)`,
  with no double-run when the same hook also appears in `.specify/extensions.yml`.
- The feedback hook stays `optional: true` → a registered-but-not-run optional
  hook SHOULD be surfaced as a one-line phase-end notice so skipping is a visible
  decision, not a silent default.
- A readiness file is partially correct (right name, missing one token) → the
  audit/template must make the *missing* token recoverable without trial-and-error
  rounds.
- A new packable game-helper candidate appears later → SkillSupport triage should
  reference the skill family/topic so the candidate is findable, not lost.
- The new fourth feedback prompt is answered "none" → the record still writes a
  well-formed section (parity with the existing "Generalizable code: none" path).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Every `/speckit-*` phase skill's hook-discovery step MUST be
  **multi-file**: after reading `.specify/extensions.yml`, it MUST also enumerate
  `.specify/extensions/*/*.yml` (and/or the extension registry) and merge every
  extension's `hooks.<before|after>_<phase>` entries, deduped by
  `(extension, command)`, before deciding what to run — so a hook registered only
  in a per-extension file (the `feedback` extension today) is discovered and runs
  on phase completion. Fixed in the canonical phase-skill sources so generated
  projects inherit it. (Closes BD-1.)
- **FR-002**: A registered-but-not-run **optional** hook SHOULD be surfaced at
  phase end as a one-line notice, so a skipped optional hook is a visible decision
  rather than a silent default. (Addresses BD-1 contributing factor.)
- **FR-003**: The `fs-skia-feedback-capture` skill MUST add a **fourth** prompt —
  "What additional or new skills would have been helpful during this *{phase}*
  phase?" — and the written record schema MUST gain a matching section (e.g.
  `## Skill gaps`). The canonical skill source (`template/feedback/skill/SKILL.md`),
  its record-template example, the contract it is sourced from
  (`specs/058-skills-quality-feedback/contracts/feedback-capture.md`), and any
  test/governance assertion pinning the prompt set MUST be updated together so the
  prompt count is internally consistent (3 → 4). (Closes the USER ask.)
- **FR-004**: A consumer MUST be able to learn the **full required shape** of each
  readiness-contract file — exact file name, required literal tokens, required
  fields, and table structure — without decompiling `FS.Skia.UI.Build.dll` or
  copying a sibling project. Satisfied by **either** shipping the requirements as
  generated templates under `specs/<feature>/readiness/*.template.md` (extending
  the existing single `skill-loading-evidence.template.md` to the full set:
  `governance-risk-levels`, `aggregate-hang-diagnostics`, `runtime-limitations`,
  `window-state-diagnostics`, the window-visibility file set, and
  `supported-host-persistent-launch.txt`), **or** having `EvidenceAudit` print the
  complete expected schema for each failing file (not just the missing token), or
  both. The planning phase selects the approach. (Closes BD-2.)
- **FR-005**: The readiness audit and the source governance scan MUST require the
  defect-class concept under **one consistent spelling/derivation** (resolve
  `product-defect` vs the project-prefixed `<project>-defect`, e.g.
  `breakoutdemo2-defect`), or — if the two spellings are intentional — document
  the distinction so a consumer is not whipsawed between two required spellings
  for one idea. (Closes BD-2a.)
- **FR-006**: Generated quickstart/tasks guidance MUST state that `Dev` is a
  completion-marker / log-writer target and that the authoritative compile/test
  path is `Test`/`Verify` (`dotnet test`) — the path that yields real
  compiler/test error feedback. The "generated quickstart" surfaces are
  `template/base/README.md` and `template/base/docs/product.md` (plus the
  tasks-template build guidance). (Closes BD-3.)
- **FR-007**: A clean `./fake.sh build -t EvidenceGraph` MUST print an explicit
  terminal verdict line (e.g. `verdict=ok (no cycles, no dangling refs, no [S*])`)
  so a passing run is self-evident without inspecting the exit code. (Closes BD-7.)
- **FR-008**: The `plan.md` template MUST inline the `GeneratedGuidanceCheck` pass
  criteria (no empty/boilerplate/`NEEDS CLARIFICATION`/`TODO`; `N/A`-with-rationale
  counts as filled) as a template comment in the Repository Governance Decisions
  block, so the gate is self-describing where the author edits. (Closes BD-5.)
- **FR-009**: The `speckit-tasks` SKILL.md MUST name the exact preset-relative
  template path (`.specify/presets/fsharp-opinionated/templates/tasks-template.md`
  and `…/tasks-deps-template.yml`), and the generic
  `.specify/templates/tasks-template.md` MUST carry a one-line pointer to the
  authoritative preset copy, so the duplicate cannot be selected by mistake.
  (Closes BD-6.)
- **FR-010**: The duplicate-DU-case pitfalls note (added by 060 FR-007) MUST be
  extended to cover **consumer-internal** DU case-name collisions across co-opened
  modules — e.g. `GameMode.Launch` vs `Msg.Launch`, where a bare case binds to the
  last-declared type — with the fully-qualified resolution. (Closes BD-4.)
- **FR-011**: The generalizable arcade helpers MUST be triaged into
  `FS.Skia.UI.SkillSupport` (shipped with a skill reference) **or** explicitly
  documented in the relevant skill as the canonical convention, per-helper, with
  the decision recorded: the **fixed-step accumulator** (`1/120 s`, capped
  steps/tick) deterministic `step` driver; **AABB / circle-vs-rect collision with
  single-reflection-per-step** resolution (axis by normalized penetration);
  **paddle-rebound angle with a `|Dy|` floor**; and **HUD-band reservation**
  (`reserveHudBand`: gameplay region = surface − reserved band, clamp gameplay,
  overdraw HUD last). (Addresses BD-8.)
- **FR-012**: Because the `.agents` skill tree is canonical and `.claude` is
  generated, all skill edits MUST be made in `.agents/skills/**` (or, for the
  template-only feedback skill, in `template/feedback/skill/SKILL.md`) and
  regenerated (`RefreshSurfaceBaselines`), keeping `SkillSyncCheck` /
  `TargetMetadataDrift` / `SkillQualityCheck` green.

> Interacting / conflicting requirements: FR-002 ("surface skipped optional
> hooks") vs the hooks staying `optional: true` (FR-001 only *discovers* them) —
> resolution: discovery is mandatory, execution stays optional, and the notice in
> FR-002 reports the skip; it does not force the hook to run. FR-004's two
> satisfiers (ship templates vs. audit-prints-schema) are **alternatives**, not
> both-required; planning picks one (or both) and the success criterion checks the
> *outcome* (recoverable-without-decompiling), not the mechanism.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identities* change. Package *contents* change if
  readiness templates are emitted into generated projects (FR-004) and if
  SkillSupport gains the arcade helpers (FR-011). The template package version is
  bumped/packed/installed so generated projects pick up the multi-file hook
  discovery, the fourth feedback prompt, and the quickstart/template edits
  (FR-001/003/006/008). Generated package **consumers** change (new feedback
  section, possibly new readiness templates).
- **Public contract impact**: No framework `.fsi` *signatures* change unless
  FR-011 ships new `SkillSupport` helpers, which would add public API
  (surface-baseline update). The consumer-facing skill/template/contract content
  changes (FR-003/004/006/008/009/010).
- **State workflow impact**: No interpreter/effects/command behavior change. The
  fixed-step accumulator / collision helpers (FR-011), if shipped, are pure
  game-loop utilities, not host runtime changes.
- **Layout/rendering impact**: No rendering-engine change. `reserveHudBand`
  (FR-011), if shipped, is a layout-region helper consistent with 060's documented
  HUD/gameplay pattern; no framework visual output changes.
- **Evidence obligations**: Real evidence under
  `specs/061-breakout-consumer-friction-followups/readiness/` — at minimum the
  Route-required escalated-tier artifacts (target-metadata, agent-ready verdict,
  skill-loading-evidence, aggregate-hang-diagnostics), plus a verification log in a
  freshly generated project proving FR-001 (feedback hook auto-fires) and FR-003
  (the record contains the fourth section), and — for FR-004 — proof the readiness
  requirements are recoverable without decompiling.
- **Unsupported scope**: No new game/demo is shipped; no new framework runtime
  capability, platform, release, or distribution change. Renaming framework DU
  cases for BD-4 is out of scope — the extended pitfalls note (FR-010) is the
  remedy. The optional-hook notice (FR-002) and the arcade-helper triage (FR-011)
  are addressed as guidance/SkillSupport, not as new hard merge gates, unless
  planning finds a low-cost executable check.
- **Build-target impact**: `TemplateCheck` / `GeneratedProductCheck` /
  `TemplateDrift` likely change to cover the fourth feedback prompt, the
  quickstart edit, and any emitted readiness templates. `GeneratedGuidanceCheck`,
  `EvidenceGraph` (FR-007 verdict line), and `EvidenceAudit` (FR-004/FR-005)
  change to emit the verdict line / resolve the defect-class spelling. New checks
  may be needed for FR-001 (multi-file hook discovery) and FR-003 (prompt-count
  consistency). `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck`
  must stay green after skill edits (FR-012). The authoritative gate list is
  determined by `./fake.sh build -t Route`.

## Success Criteria *(mandatory)*

- **SC-001**: In a generated project whose feedback hook is registered only under
  `.specify/extensions/feedback/feedback.yml`, completing a phase writes a feedback
  record without an explicit user nudge — demonstrated in a verification log.
  (BD-1)
- **SC-002**: The written feedback record contains a fourth section answering
  "what additional/new skills would have helped", and the skill's prompt set,
  record-template example, and contract all state four prompts (no surviving
  "three prompts" reference). (USER ask, FR-003)
- **SC-003**: In a freshly generated project with no passing sibling, every
  readiness-contract file's required name, tokens, fields, and table shape are
  obtainable from a shipped template and/or the audit's own output — verified by
  reaching a passing `EvidenceAudit` without decompiling `FS.Skia.UI.Build.dll` or
  copying another project. (BD-2)
- **SC-004**: The defect-class concept resolves to a single consistent spelling
  across the readiness audit and the source governance scan (or the two are
  documented as deliberately distinct). (BD-2a)
- **SC-005**: The generated quickstart (`README.md` / `docs/product.md`) states
  the `Dev`-vs-`Test`/`Verify` distinction, and a clean `EvidenceGraph` run prints
  an explicit verdict line. (BD-3, BD-7)
- **SC-006**: The `plan.md` template carries the `GeneratedGuidanceCheck`
  pass-criteria inline, and `speckit-tasks` SKILL.md names the exact preset
  template path with the generic copy pointing to it. (BD-5, BD-6)
- **SC-007**: The duplicate-DU pitfalls note covers the consumer-internal
  cross-module collision (`GameMode.Launch` vs `Msg.Launch`) with the qualified
  resolution. (BD-4)
- **SC-008**: Each generalizable arcade helper (fixed-step accumulator,
  collision/reflection, paddle rebound, HUD-band reservation) is either shipped in
  `FS.Skia.UI.SkillSupport` with a skill reference or documented in the relevant
  skill as the canonical convention, with the per-helper decision recorded. (BD-8)
- **SC-009**: All Route-printed gates for this change pass, including
  `SkillSyncCheck` / `TargetMetadataDrift` after `.agents` (and feedback-skill)
  edits are regenerated, and `EvidenceAudit` returns `verdict=PASS` for
  `specs/061-breakout-consumer-friction-followups`.

## Assumptions

- BreakoutDemo2 was generated from the 060-merged packages (`0.1.64-preview.1` /
  template `0.1.83`), so all findings are against the current merged state; this
  feature does not re-merge or re-verify 060's deliverables (the consumer already
  exercised 060's `docs/api-surface` emission successfully).
- "Recoverable without decompiling" (FR-004) is satisfied by shipping readiness
  templates **or** by the audit printing the full per-file schema, or both; the
  planning phase selects the mechanism, and SC-003 checks the outcome.
- The fourth feedback prompt (FR-003) is added to the template-only canonical
  `template/feedback/skill/SKILL.md` (and its 058 contract); the feedback skill is
  not part of the `.agents`/`.claude` skill tree, so `SkillSyncCheck` does not
  govern it, but `TemplateCheck`/`GeneratedProductCheck` may pin its content.
- FR-002 (skipped-optional-hook notice) and FR-011 (arcade-helper triage) are
  delivered as guidance / SkillSupport additions, not as new hard merge gates,
  unless planning finds a low-cost executable check.
- "One feature, not one-per-item" is the correct reading of "create specs" given
  the consolidated consumer-friction-followups house pattern and the
  one-feature-per-`/speckit-specify` rule.
- The hook-discovery fix (FR-001) is made in the canonical `/speckit-*` phase-skill
  sources so generated projects inherit it; the same gap exists in this repo's own
  phase skills and is fixed there too.
