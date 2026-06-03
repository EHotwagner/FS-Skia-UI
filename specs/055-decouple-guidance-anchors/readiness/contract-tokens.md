# Machine-Contract Tokens (SC-004)

The enumerated machine-contract-token set that remains matched **verbatim**
(case-insensitive substring), separate from the semantic obligations. Each token
is consumed by tooling/parsers and is not free prose; a test removing any of them
still FAILs (`tests/Governance.Tests/GuidanceValidatorTests.fs`, "SC-004 removing
a machine-contract token still fails").

## `task-skillist-guidance` tokens

| Token | Files |
|-------|-------|
| `[skillist: []]` | tasks-template (+ preset twin) |
| `skillist:` | tasks-deps-template.yml |
| `deps:` | tasks-deps-template.yml |
| `[SEH]` | tasks-template, speckit-tasks/implement skill + command twins, constitution (+ template twins) |
| `synthetic-error-handling-approved` | same set as `[SEH]` |
| `loaded_at` | speckit-implement SKILL + command twin |
| `work_started_at` | speckit-implement SKILL + command twin |
| `readiness/skill-loading-evidence.md` | speckit-implement SKILL + command twin |

## `controls-boundary-guidance` tokens

| Token | Files |
|-------|-------|
| `FS.Skia.UI.Controls` | controls/elmish/base/src control guidance |
| `Control<'msg>` | controls README/skill, elmish README, src/Controls skill |
| `DataGrid` | controls README/skill, base README/product, src skill, spec templates |
| `FS.Skia.UI.Controls.Elmish` | controls skill, elmish README, base README/product, src skill |
| `ControlsElmish.program` | elmish README |

### Forbidden / stale tokens (FR-006 — must NOT appear, over combined content)

`FS.Skia.UI.Charts`, `fs-skia-charts`, `chart-only`, `DataGrid as chart`,
`DataGrid-as-chart`, `renderer-neutral`, `renderer neutral`,
`host-loop ownership`, `host loop ownership`. A reintroduced stale term still
FAILs ("FR-006 a reintroduced forbidden/stale term still fails").

## `sequential-fake-guidance`

The four FAKE-serialization facets (`FAKE-backed`, `.fake`, `sequential`, `not
safe to run concurrently`) are now the `fake-sequential` semantic obligation
(`AllOf`, source `CLAUDE.md:FAKE concurrency rule`); the structural regex
assertions (FAKE-command-present, numbered-order requirement, parallelism
non-FAKE caveat) remain unchanged machine logic. No verbatim contract tokens.

## Contract enumeration (per `contracts/guidance-currency-contract.md`)

The currency contract requires at minimum these verbatim tokens: `[skillist: []]`,
`[SEH]`, `synthetic-error-handling-approved`, `skillist:`, `deps:`,
`Control<'msg>`, `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`,
`ControlsElmish.program`, `loaded_at`, `work_started_at`,
`readiness/skill-loading-evidence.md`. All are present in the live check values
(`Guidance.taskSkillistGuidanceCheck`, `Guidance.controlsBoundaryGuidanceCheck`).
