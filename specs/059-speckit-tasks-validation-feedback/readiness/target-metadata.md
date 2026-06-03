# Target Metadata — 059-speckit-tasks-validation-feedback

## Feature classification (T003)

- **Tier**: **Tier 1 (contracted)**. No application public `.fsi` change, but the
  **consumer authoring contract** changes: `tasks.deps.yml` gains a per-task
  `owns:` field, the title-trigger matcher is removed, a bundled skill is split
  into two registered ids, the runtime sample feature is removed, and hint tables
  change. `./fake.sh build -t Route` escalates to the **agent-ready** tier
  (consumer-contract paths: `template/**`, `.specify/**`, `.agents/skills/**`,
  `build/Governance/**/*.fsi`) with gates `Dev, TemplateCheck,
  GeneratedProductCheck, GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck,
  TemplateDrift, EvidenceGraph, EvidenceAudit`.
- **Affected layers**: compiled governance engine (`build/Governance/Evidence/**`,
  `Guidance.fs`, `GovernedBlocks.fs`, `GeneratedProduct.fs`) + consumer template
  (`template/**`, `.template.config` blanket skill copy) + bundled skills
  (`.agents/skills/**` + generated `.claude/**` peers) + presets/templates
  (`.specify/**`). **No product runtime, layout, rendering, Vulkan, or Skia
  change.**
- **Public-contract impact**: `DepsParser.fsi` (`DepsEntry` gains
  `Owns: string list option`) and `Audit.fsi` (drop `expectedCapabilityMatches`).
  The versioned `contracts/tasks-deps-schema.md` is the shipped consumer contract.
- **Principle IV (Elmish/MVU)**: **N/A** — governance tooling and documentation,
  no product `Model`/`Msg`/`Effect`/`Cmd`/`init`/`update`/interpreter. The only
  file read is at the `build.fsx` interpreter edge.
- **Synthetic evidence (Principle V)**: **none planned**. The parser/audit tests
  run real code against real strings; the golden fixtures and a real generated
  consumer validating its own feature are the evidence. No `[SEH]` approved.

## Required evidence obligations

- `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`,
  `runtime-limitations.md`, `generated-validation-authority.md`,
  `skill-loading-evidence-workflow.md` — the readiness scaffolds (T002).
- `task-graph.json` / `task-graph.md` — `EvidenceGraph` output (T036).
- `evidence-audit.md` — `EvidenceAudit` merge-gate verdict (T037).
- `target-metadata.md`, `agent-ready-verdict.md` — escalated-path artifacts.

## Per-target verdicts (T035 / T036 / T037)

Authoritative per-target verdicts (each FAKE target run individually and
sequentially; the aggregate sweep is non-authoritative — see
`aggregate-hang-diagnostics.md`):

| Target | Verdict | Notes |
| --- | --- | --- |
| `RefreshSurfaceBaselines` | **Ok** | regenerated `.claude` tree, constitution, `validation.contract.yml` after the split + owns changes |
| `Dev` | **Ok** | full FAKE test aggregate green; `Governance.Tests` 404/404 (incl. owns/parser/hint/resolver + regenerated golden parity) |
| `GeneratedGuidanceCheck` | **Ok** | generated guidance current; `skillist-ownership-honesty` obligation reflects free-form + `owns:` |
| `SkillSyncCheck` | **Ok** | `.agents` → `.claude` currency after the split (old peer removed, two new peers added) |
| `SkillQualityCheck` | **Ok** | both split skills pass the quality bar |
| `TemplateDrift` | **Ok** | |
| `TargetMetadataDrift` | **Ok** | contract/metadata current (no routing-rule change) |
| `TemplateCheck` | **Ok** | `TemplatePack` → `Build` → `TemplateInstantiate` → `Test` (391) → `TemplateSmoke` all green |
| `GeneratedProductCheck` | **Ok** | generated consumer validated; ships no sample feature (FR-014) |
| `EvidenceGraph` | **Ok** | no cycles / dangling / `[S*]`; dogfooded `owns:` resolves (38 tasks) |
| `EvidenceAudit` | **PASS** | 38 real tasks, 0 blockers (0 synthetic, 0 diff-scan, 0 readiness-contract, 0 gui-runtime) |

Plus the captured generated-consumer transcript (`consumer-validation-transcript.md`)
demonstrating SC-001/SC-002/SC-003 against a real `app-source` product: loud-fail
with no feature, and the `SPECKIT_FEATURE_DIR` override echoing `feature-directory`
+ `tasks=2`. The merge gate is `EvidenceAudit` (**PASS**); all escalated gates ran
green this session, each FAKE target individually and sequentially.
