# Contract: Multi-file extension hook discovery

**Feature**: `061-breakout-consumer-friction-followups` · **FR-001, FR-002**
**Surface**: the "Check for extension hooks" step in every `/speckit-*` phase
skill (`.agents/skills/speckit-{specify,clarify,plan,tasks,analyze,checklist}/SKILL.md`
and the `before_implement`/`after_implement` discovery in `speckit-implement`).

## Inputs

- The generated project root, containing zero or more of:
  - `.specify/extensions.yml` (central, Spec-Kit-core-owned)
  - `.specify/extensions/<ext>/<ext>.yml` (per-extension; the `feedback`
    extension ships `.specify/extensions/feedback/feedback.yml`)
- The current phase `<phase>` and direction `<before|after>`.

## Discovery algorithm (normative)

```
hooks := []
if exists(.specify/extensions.yml):
    hooks ++= parse(.specify/extensions.yml).hooks["<dir>_<phase>"]   # may be absent → []
for f in sorted(glob(.specify/extensions/*/*.yml)):
    if parseable(f):
        hooks ++= parse(f).hooks["<dir>_<phase>"]                     # absent key → []
    # unparseable file → skip silently (parity with central-file rule)
hooks := dedupe(hooks, key = (extension, command))                    # first occurrence wins
hooks := [h for h in hooks if h.enabled != false]
# condition is NOT evaluated by the skill (left to HookExecutor)
```

## Output / behavior

- Each surviving hook is rendered exactly as today (mandatory →
  `EXECUTE_COMMAND`; optional → the optional-hook block with prompt).
- **FR-002 notice**: for every `optional: true` hook that is discovered but not
  executed this phase, emit one line, e.g.:
  `Note: optional hook {extension}:{command} is registered but was not run (skipped).`

## Conformance

| ID | Assertion |
|----|-----------|
| HD-1 | A `feedback` hook present only under `.specify/extensions/feedback/feedback.yml` is discovered for `after_<phase>`. *(SC-001)* |
| HD-2 | The same hook declared in both `.specify/extensions.yml` and a per-extension file is rendered once. |
| HD-3 | `enabled: false` hooks are excluded; `condition` is not evaluated. |
| HD-4 | A skipped optional hook produces the FR-002 one-line notice. |
| HD-5 | Behavior is unchanged for projects with only `.specify/extensions.yml`. |
| HD-6 | The fix exists in the canonical `.agents` source and is mirrored byte-for-byte into `.claude` by `RefreshSurfaceBaselines` (`SkillSyncCheck` green). |

## Out of scope

- Evaluating `condition` expressions (still the HookExecutor's job).
- Changing the `optional`/`enabled` semantics of any hook (the feedback hook
  stays `optional: true`; FR-001 only makes it *discoverable*).
