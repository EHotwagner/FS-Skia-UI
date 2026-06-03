# Phase 0 Research — `/speckit.tasks` Validation Trust

All "NEEDS CLARIFICATION" from Technical Context resolved below. Each item:
Decision · Rationale · Alternatives considered.

## R1 — Template feature resolution (FR-002/003/004/014)

**Decision**: Replace `ensureGeneratedEvidencePackage()` in
`template/base/build.fsx` with a resolver that mirrors the framework engine
`build/Governance/Engine/Model.fs activeFeatureId`:

1. If `SPECKIT_FEATURE_DIR` (override) is set and points to an existing
   directory, use it and echo the choice (FR-005).
2. Else read `<root>/.specify/feature.json`, parse `"feature_directory"`, and
   use it.
3. Else **fail loud**: non-zero exit, message naming `.specify/feature.json`,
   the `"feature_directory"` key, and the `SPECKIT_FEATURE_DIR` override
   (FR-003). No sample is generated; the `generated-evidence-workflow`
   synthesiser and the `GENERATED_EVIDENCE_FEATURE_DIR` fallback are deleted
   (FR-014).
4. After resolution, echo `feature-directory=…` and `tasks=<n>` (FR-004).

**Rationale**: The engine already does exactly this with no fallback; matching
it makes the template self-consistent with the framework and eliminates the only
false-green path. `.specify/feature.json` is the single source of truth and is
written by `/speckit.specify`; a fresh project that never specified a feature
legitimately fails loud (US1 scenario 2, SC-002).

**Alternatives considered**: (a) Infer feature from branch name or most-recent
`tasks.md` — rejected by the clarification (ambiguity-prone, can guess wrong).
(b) Keep a sample but warn — rejected: any silent sample is a foot-gun (FR-014).

**Wrinkle**: `.template.config/template.json` excludes `feature.json` from the
consumer copy, so a freshly generated project has none until it specifies — the
loud-fail is therefore the *expected first-run* state, and the guidance must say
so (quickstart).

## R2 — The `owns:` field vocabulary and semantics (FR-010)

**Decision**: Add `owns: [ … ]` as an optional per-task list in
`tasks.deps.yml`. The vocabulary is the **gated-evidence capability set** that
the removed `capabilityTriggerGroups` previously inferred from titles:

| `owns:` value        | Meaning (task owns this gated evidence)            | Implied skill (validated, not auto-added) |
|----------------------|---------------------------------------------------|-------------------------------------------|
| `graph-validation`   | Owns task-graph/readiness validation evidence     | `speckit-evidence-graph`                  |
| `evidence-audit`     | Owns synthetic-propagation / diff-scan audit       | `speckit-evidence-audit`                  |
| `task-generation`    | Owns `/speckit.tasks` task-generation guidance     | `speckit-tasks`                           |
| `implementation-loading` | Owns `/speckit.implement` skill-loading guidance | `speckit-implement`                     |
| `constitution`       | Owns constitution authoring                        | `speckit-constitution`                    |

Enforcement (replacing the title scan): when a task declares
`owns: [graph-validation]`, the engine requires the implied skill to be present
in that task's `skillist` (and reports if omitted) — but it **never** scans the
title. Tasks that own nothing declare no `owns:` (or `owns: []`) and their skill
assignment is trusted-as-declared. Unknown `owns:` values are a directive error.

**Rationale**: Preserves the one genuinely useful guarantee of the old matcher
(the task that owns graph/audit evidence really declares the matching skill)
while making titles fully free-form and the signal structured (FR-010, SC-006).
Keeps the `owns→skill` coupling explicit and documented (FR-009 honesty).

**Alternatives considered**: (a) `owns:` purely informational with no skill
coupling — rejected: loses the "the graph/audit owner declares its skill"
guarantee the trigger matcher provided. (b) Free-form `owns:` strings — rejected:
unbounded vocabulary defeats the directive-error goal; a closed set keeps errors
actionable.

## R3 — Splitting `fs-skia-layout-evidence` (FR-012)

**Decision**: Split the catch-all into **two** registered `.agents` skills along
its distinct concerns:

- `fs-skia-evidence-mode` — deterministic-evidence-mode guidance,
  host-warning classification, `ReadableLayout` / `DeterministicRenderOnly` /
  `UnsupportedLayoutInspection` proof levels (the evidence-discipline half).
- `fs-skia-layout-readability` — generated game HUD/status layout readability,
  gameplay-region bounds, public scene/host/update naming guidance (the
  layout-design half).

The original `fs-skia-layout-evidence` id is retired; every hint table,
`capabilities.yml` reference, `Related` link, and the deps template example are
updated. New `sources` entries in `.template.config/template.json` copy both new
skills to `.agents/skills/` and `.claude/skills/` so consumers register them.

**Rationale**: The report and FR-012 require precise signals; ~9/33 tasks routed
to one catch-all carries little information. Two skills along the natural seam
(evidence discipline vs layout design) give discriminating assignments.

**Open confirmation for Phase 1**: whether to keep one of the new ids equal to
the old name to minimise churn vs. two fresh names. **Chosen**: two fresh names
(clean seam), accepting the rename cost the clarification already approved.

## R4 — Does the skill split need new build gates? (Command-surface)

**Decision**: No. `.agents/skills/**` changes route (Routing `skill-quality`
rule) to the existing `SkillQualityCheck` + `SkillSyncCheck` gates, which already
enumerate the canonical tree. New skills are validated by those gates plus
`SkillSyncCheck` currency after `RefreshSurfaceBaselines`. Therefore
`build/Governance/Targets.fs`, `Routing.fs`, and `AgentValidation.knownGates`
need **no** new entries. `validation.contract.yml` only regenerates if a routing
*rule* changes (it does not here).

**Rationale**: Confirmed against `Routing.fs` (skill-quality rule paths include
`.agents/skills/**`) and `AgentValidation.knownGates` (gate allowlist is about
`required_gates`, not skill ids). Avoids the known "new gate missing from
knownGates → Governance.Tests unknown-gate" trap from prior features because we
add **no** gate.

**Alternatives considered**: registering each new skill as its own gate —
rejected: skills are not gates; the existing skill gates cover them.

## R5 — Are readiness-blocking scans in FR-010 scope?

**Decision**: Out of scope. FR-010 targets *the title-trigger capability
matcher* (`capabilityTriggerGroups` / `expectedCapabilityMatches` in
`Audit.fs`). The persistent-gui-runtime / window-visibility / audit-status
families are separate readiness/diff scans (`AuditResult` fields, `Scans.fs`,
`StatusRegion.fs`) driven by `readiness/` content and diff hits, not by task
titles. Removing them is neither required by the spec nor safe here.

**Rationale**: The spec is explicit ("the existing title-trigger matcher");
conflating the readiness scans would expand scope beyond the clarified intent.
Phase 1 will confirm by reading those scan inputs to ensure none keys on task
titles; if one does and overlaps the matcher, it is documented, not silently
changed.

## R6 — FR-007 directive wrapper error + `LegacyBareList`

**Decision**: `DepsParser.fs` already emits
`"tasks.deps.yml: missing or malformed 'tasks' mapping"` and flags
`LegacyBareList` for bare per-task object entries. FR-007 is satisfied by
ensuring that when top-level bare `Tnnn:` keys are present **without** a `tasks:`
wrapper, the parser emits the wrapper diagnostic **first and standalone** and
does **not** additionally emit 33 downstream "tasks.md declares Tnnn but
tasks.deps.yml has no key" errors that bury it. Implementation: detect bare
top-level task-id keys at parse time and short-circuit to the directive wrapper
message.

**Rationale**: The report's pain was the directive error being drowned by
downstream noise. Short-circuiting keeps the actionable message visible (SC-003).

**Alternatives considered**: auto-wrap bare keys — rejected: silently accepting
a malformed shape contradicts the "fail directive, not lenient" stance and would
mask author mistakes.

## R7 — Consumer skill set per profile (FR-008 exactness)

**Decision** (from `.template.config/template.json`): every profile registers
all 25 canonical `.agents/skills/*` plus profile-conditional product skills:

- `app`: `fs-skia-scene`, `fs-skia-skiaviewer`, `fs-skia-elmish`,
  `fs-skia-keyboard-input`, `fs-skia-ui-widgets`.
- `headless-scene`: `fs-skia-scene`.
- `governed`: `fs-skia-scene`, `fs-skia-testing`.
- `sample-pack`: `fs-skia-scene`, `fs-skia-skiaviewer`, `fs-skia-elmish`,
  `fs-skia-samples`.

**No** profile registers `fs-skia-layout`; the only layout skill present in all
profiles is the (soon-split) `fs-skia-layout-evidence`. The "layout →
`fs-skia-layout`" hint and the deps-template example `skillist: ["fs-skia-layout"]`
are therefore unresolvable in every consumer and MUST be corrected to the
split readability skill (FR-008). Hints that name profile-conditional skills
(e.g. `fs-skia-ui-widgets`) must be marked as available only in the profiles
that ship them, or the hint-resolution check must be profile-aware.

**Rationale**: Makes the FR-008 fix and its enforcing test exact and grounded in
the actual generation manifest.

**Phase 1 contract**: a hint-resolution check validates every hint id against
the union of canonical skills + all product skills (so a hint is valid if *some*
profile registers it), and the hint tables annotate profile-conditional ids.
