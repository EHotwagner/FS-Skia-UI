# Research: Claude Code Ready Spec Kit

## Decision: Use `CLAUDE.md` as the Claude Code project instruction entry point and import `AGENTS.md`

**Rationale**: Official Claude Code memory documentation says Claude Code reads `CLAUDE.md`, not `AGENTS.md`, and recommends creating a `CLAUDE.md` that imports `AGENTS.md` when a repository already uses agent instructions for other tools. This preserves a single project instruction source while letting Claude-specific additions live below the import when needed.

**Alternatives considered**: Duplicating AGENTS content into CLAUDE.md was rejected because it creates drift. A symlink was rejected for generated projects because Windows support may require elevated privileges or Developer Mode.

## Decision: Project skills are canonical Claude Code workflow artifacts

**Rationale**: Official Claude Code skills documentation identifies project skills at `.claude/skills/<skill-name>/SKILL.md`, with `SKILL.md` required and YAML frontmatter used for discovery. It also documents live skill detection, project discovery from parent directories, and precedence over same-name command files. This matches the feature clarification that project skills are canonical while command compatibility is optional.

**Alternatives considered**: Generating only `.claude/commands/*.md` was rejected because the spec makes skills canonical and current docs give skills the richer discovery model. Plugin distribution was deferred because generated projects must be ready from project-local files.

## Decision: Generate optional command aliases only from the shared workflow source

**Rationale**: Claude Code docs state files in `.claude/commands/` work like commands and that a same-name skill takes precedence. Command aliases can help users accustomed to slash commands, but they must not become a second manually maintained workflow definition.

**Alternatives considered**: Omitting commands entirely remains valid, but aliases are useful if the generator can prove they share source text with the matching skill. Handwritten command files were rejected.

## Decision: Commit only project-shareable settings in `.claude/settings.json`

**Rationale**: Official settings documentation distinguishes shared project settings at `.claude/settings.json` from local personal settings at `.claude/settings.local.json`, which should not be checked in. The feature therefore uses `.claude/settings.json` for permissions and supported hooks and validates that generated projects do not require local or user-level settings.

**Alternatives considered**: Relying on `~/.claude/settings.json` or `.claude/settings.local.json` was rejected because generated projects must work without personal setup.

## Decision: Hook support must reference project-local scripts with Claude project-root placeholders

**Rationale**: Official hook documentation says hooks live in settings files, supports project settings, and recommends referencing project scripts through `${CLAUDE_PROJECT_DIR}` so commands work regardless of current directory. Hook execution can block or report errors, so validation must prove hook scripts exist, are project-local, and fail with actionable diagnostics.

**Alternatives considered**: Absolute developer-machine paths and user-home hook scripts were rejected because they are not portable or project-shareable. Unsupported hooks should be omitted or disabled with explicit evidence rather than generated optimistically.

## Decision: Drift validation compares rendered Codex and Claude artifacts back to the same source ids

**Rationale**: The spec requires Codex and Claude artifacts to stay synchronized automatically and fail on drift. The existing repository already validates generated guidance and template drift in `build.fsx`, so the least risky path is to extend those patterns with source ids for instructions, workflows, settings, hooks, and generated profile artifacts.

**Alternatives considered**: Manual reviewer checklist comparison was rejected because FR-007 requires validation failure. A Claude-only generator was rejected because it would not prove Codex parity.

## Decision: No new runtime product dependency

**Rationale**: The change is about repository/template agent artifacts, build validation, and generated project files. Existing F#/.NET, FAKE, Expecto, and script validation are enough unless implementation discovers a need for a structured parser that cannot be handled by existing code.

**Alternatives considered**: Adding a YAML/JSON templating package was deferred. If required, it must go through dependency governance.

## Official Sources Retrieved 2026-05-29

- Claude Code skills: `https://code.claude.com/docs/en/skills`
- Claude Code memory/project instructions: `https://code.claude.com/docs/en/memory`
- Claude Code settings: `https://code.claude.com/docs/en/settings`
- Claude Code hooks: `https://code.claude.com/docs/en/hooks`
- Claude Code setup/system requirements: `https://code.claude.com/docs/en/setup`
