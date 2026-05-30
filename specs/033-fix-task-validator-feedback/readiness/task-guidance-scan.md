# Task Guidance Scan

Command:

`dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`

Result:

- PASS: 50 tests passed, 0 failed.
- Guidance files scanned:
  - `.specify/templates/tasks-template.md`
  - `.specify/presets/fsharp-opinionated/templates/tasks-template.md`
  - `.agents/skills/speckit-tasks/SKILL.md`
  - `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`

Enforced groups documented:

- graph validation
- evidence audit
- task generation
- implementation loading
- constitution

Readiness prefix documented: `Complete readiness notes`.

Safe examples documented:

- `Complete readiness notes for skill-loading evidence workflow placeholder`
- `Record required readiness filenames for later verification`
- `Create placeholder evidence files listed by the plan`
