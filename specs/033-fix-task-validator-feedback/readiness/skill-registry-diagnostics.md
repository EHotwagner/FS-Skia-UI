# Skill Registry Diagnostics

Command:

`dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`

Result:

- PASS: 50 tests passed, 0 failed.
- The synthetic registry mismatch fixture creates `.agents/skills/directory-name/SKILL.md` with declared `name: accepted-skill`.
- A task declaring `skillist: ["directory-name"]` fails validation with the accepted declared id and source path.

Author-facing rule verified:

- The authoritative registry roots are `.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, and `template/fragments/*/skill/SKILL.md`.
- Authors declare the `name:` value from `SKILL.md`, not the directory name when they differ.
