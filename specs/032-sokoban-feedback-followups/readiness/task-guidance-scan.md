# Task Guidance Scan

- Command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj`
- Files scanned: `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.agents/skills/speckit-tasks/SKILL.md`, `.claude/skills/speckit-tasks/SKILL.md`.
- Title trigger phrase pitfalls: guidance names `persistent GUI runtime` and `window visibility validation fixture` as phrases that can imply unrelated capability requirements.
- Dependency shape: guidance requires object shape with indented `deps` and `skillist` fields.
- One key per task id: guidance requires exactly one key per `Tnnn` id in `tasks.deps.yml`.
- Skill mirror: guidance requires visible `[skillist: ...]` entries in `tasks.md` to match structured `skillist` metadata exactly.
- Status: ok; graph-only validation is refreshed by `EvidenceGraph`.

