# Template check — corrected phase skills reach generated output (feature 077, SC-005)

- **Authoritative command**: `./fake.sh build -t TemplateCheck` (preceded by
  `./fake.sh build -t GeneratedGuidanceCheck`).
- **Artifact**: `readiness/template-check.md` (this file) +
  `readiness/generated-guidance-validation.md`.
- **Failure class**: template (the corrected `.agents`/`.claude` phase skills did not reach
  generated `dotnet new fs-skia-ui` output).
- **Next action**: confirm the corrected `speckit-implement` / `speckit-tasks` skills are
  present in generated `.agents` and `.claude` output via the `TemplateSmoke` assertion.

## Propagation mechanism (no template.json edit needed)

The phase-skill SKILL.md files ship to generated projects through the **existing**
`.template.config/template.json` copy-only globs (`.agents/skills/**/*` and the parallel
`.claude/skills/**`). Because the repair edits canonical `.agents` files and regenerates
`.claude`, the corrected skills propagate with **no** `template.json` change. The
`modern-hook-block` markers travel verbatim into generated output.

## Result

PASS — `./fake.sh build -t TemplateCheck` finished `Status: Ok` (`TemplateSmoke` green). The
corrected phase skills are present in the generated output verbatim, confirmed under
`artifacts/template-check/077-implement-feedback-hook-parity/`:

- `source-app/.agents/skills/speckit-implement/SKILL.md` — carries `## Effective hooks for implement`.
- `source-app/.claude/skills/speckit-implement/SKILL.md` — carries `## Effective hooks for implement`.
- `source-governed/.agents/skills/speckit-tasks/SKILL.md` and `source-headless-scene/...` —
  carry `## Effective hooks for tasks`.

Propagation needed no `template.json` change — the existing `.agents/skills/` → `.agents/skills/`
and `.agents/skills/` → `.claude/skills/` copy globs carry the corrected text into every
generated profile.
