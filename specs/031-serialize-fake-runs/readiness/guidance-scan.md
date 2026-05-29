# Guidance Scan

Status: complete for focused guidance implementation.

Required concepts: FAKE-backed command class, shared `.fake` race risk,
sequential execution, deterministic command order, and safe non-FAKE
parallelism distinction.

Validation command:

```text
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore --logger "console;verbosity=minimal"
```

Result: PASS, 225 tests.

Repository guidance checked:

| Path | FAKE-backed command class | `.fake` race risk | Sequential execution | Deterministic order | Non-FAKE parallelism |
|------|---------------------------|-------------------|----------------------|---------------------|----------------------|
| `README.md` | found | found | found | found | found |
| `docs/build.md` | found | found | found | found | found |
| `docs/testing.md` | found | found | found | found | found |
| `docs/evidence.md` | found | found | found | found | found |
| `AGENTS.md` | found | found | found | found | found |
| `CLAUDE.md` | found | found | found | found | found |
| `.agents/skills/speckit-implement/SKILL.md` | found | found | found | found | found |
| `.agents/skills/speckit-evidence-graph/SKILL.md` | found | found | found | found | found |
| `.agents/skills/speckit-evidence-audit/SKILL.md` | found | found | found | found | found |
| `.claude/skills/speckit-implement/SKILL.md` | found | found | found | found | found |
| `.claude/skills/speckit-evidence-graph/SKILL.md` | found | found | found | found | found |
| `.claude/skills/speckit-evidence-audit/SKILL.md` | found | found | found | found | found |
| `.specify/templates/tasks-template.md` | found | found | found | found | found |
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | found | found | found | found | found |
| `.specify/templates/plan-template.md` | found | found | found | found | found |
| `.specify/presets/fsharp-opinionated/templates/plan-template.md` | found | found | found | found | found |
| `template/base/README.md` | found | found | found | found | found |
| `template/base/docs/product.md` | found | found | found | found | found |
| `template/base/.agents/skills/fs-skia-project/SKILL.md` | found | found | found | found | found |
| `template/base/.claude/skills/fs-skia-project/SKILL.md` | found | found | found | found | found |

Repairs completed:

- Converted multi-command FAKE examples to numbered sequential lists.
- Added shared `.fake` state and not-safe-to-run-concurrently warnings.
- Added non-FAKE parallelism exceptions.
- Added race-like failure triage wording and sequential rerun requirements.
- Serialized `dotnet fake` invocations in governance test support so tests do
  not race on `.fake`.
