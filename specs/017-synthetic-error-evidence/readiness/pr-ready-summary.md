# PR-Ready Summary

Changed governance surfaces:

- `.specify/extensions/evidence/scripts/python/compute-task-graph.py`
- `.specify/extensions/evidence/scripts/bash/run-audit.sh`
- `.specify/templates/tasks-template.md`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md`
- `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`
- `.specify/presets/fsharp-opinionated/commands/speckit.implement.md`
- `.agents/skills/speckit-tasks/SKILL.md`
- `.agents/skills/speckit-implement/SKILL.md`
- `.specify/memory/constitution.md`
- `.specify/templates/constitution-template.md`
- `.specify/presets/fsharp-opinionated/templates/constitution-template.md`
- `docs/evidence.md`
- `docs/speckit.md`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs`
- `build.fsx`

Synthetic evidence disclosures:

- T012 `[S] [SEH]`: malformed parser input fixture.
- T019 `[S] [SEH]`: accepted audit fixture for corrupt/error-path input.
- T024 `[S] [SEH]`: readiness command capture for accepted fixture.

Residual risk:

- Broad `Verify` was attempted but hung in `Smoke.Tests`; focused governance
  targets passed and are the authoritative evidence for this governance-only
  feature.

Package/API/runtime impact:

- No package identity, `.fsi` public surface, generated product runtime,
  renderer, or platform support changes.
