# Generated Guidance Check

PASS: active and preset-owned spec/plan templates include required governance prompts in the expected Markdown sections.
PASS: generated Controls guidance covers Skia-rendered controls, rich text, chart controls, graph controls, DataGrid, Controls.Elmish adapter wiring, and legacy Charts replacement notes without stale generated terms.

Validated prompt classes:
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `package impact`
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `public contract impact`
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `state workflow impact`
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `layout/rendering impact`
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `evidence obligations`
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `unsupported scope`
- `.specify/templates/spec-template.md` section `Framework Governance Prompts` prompt `build-target impact`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `package impact`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `public contract impact`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `state workflow impact`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `layout/rendering impact`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `evidence obligations`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `unsupported scope`
- `.specify/presets/fsharp-opinionated/templates/spec-template.md` section `Framework Governance Prompts` prompt `build-target impact`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `template ownership`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `dependency impact`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `command-surface impact`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `generated project impact`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `evidence paths`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `.fsi`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `MVU/effect boundary`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `synthetic evidence`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `test evidence`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `observability`
- `.specify/templates/plan-template.md` section `Repository Governance Decisions` prompt `deferred scope`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `template ownership`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `dependency impact`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `command-surface impact`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `generated project impact`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `evidence paths`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `.fsi`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `MVU/effect boundary`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `synthetic evidence`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `test evidence`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `observability`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` section `Repository Governance Decisions` prompt `deferred scope`

Deferred roadmap boundaries checked: visual evidence, release validation, external repository split, and distribution automation remain outside V2 pass/fail scope.

## T078 Generated Guidance Gate

| Gate | Log | Verdict | Duration |
|------|-----|---------|----------|
| `./fake.sh build -t GeneratedGuidanceCheck` | `readiness/logs/t078-generated-guidance-check.txt` | PASS | 1s |

The T078 guidance gate validates active and preset-owned spec/plan governance
prompts plus generated Controls guidance for Skia-rendered Controls, rich text,
chart controls, graph controls, DataGrid, Controls.Elmish adapter wiring, and
removed Charts / legacy Charts replacement notes.
