# Phase 0 Research: Governance Markdown Rewrite

All Technical Context unknowns are resolved below. This feature adds no new
technology; "research" here means pinning down the exact preservation contract,
the regeneration mechanics, and the size-accounting reproduction so the rewrite
cannot silently weaken governance.

## Decision: The cut-authority is `Guidance.fs`, not the prose itself

- **Decision**: Treat the values in `build/Governance/Guidance.fs` —
  `taskSkillistGuidanceCheck`, `controlsBoundaryGuidanceCheck`,
  `serializedRunnerObligation`, and the forbidden-token lists — as the single
  authority on what may not be cut. A sentence may be deleted only if doing so
  removes no `ContractToken.Token` from any file in that token's `Files`, leaves
  every `GuidanceObligation`'s concept set still matchable in each of its `Files`
  (AnyOf: at least one anchor; AllOf: all anchors), and reintroduces no forbidden
  term.
- **Rationale**: 055 deliberately moved the "what cannot be cut" knowledge out of
  the prose and into typed values. The spec (FR-003, Assumptions) names this
  inventory as the authority. Re-reading prose to guess intent would reintroduce
  exactly the ambiguity 055 removed.
- **Alternatives considered**: (a) Diff-against-old-prose to infer obligations —
  rejected: the old prose *was* the freeze; inferring from it re-pins wording.
  (b) Re-derive obligations during the rewrite — rejected: amending the currency
  model is explicitly out of scope.

### Enumerated contract tokens that MUST survive verbatim (per home file)

From `taskSkillistGuidanceCheck`: `[skillist: []]`, `skillist:`, `deps:`,
`[SEH]`, `synthetic-error-handling-approved`, `loaded_at`, `work_started_at`,
`readiness/skill-loading-evidence.md`. From `controlsBoundaryGuidanceCheck`:
`FS.Skia.UI.Controls`, `Control<'msg>`, `DataGrid`, `FS.Skia.UI.Controls.Elmish`,
`ControlsElmish.program`. Each only in the files its `Files` list names — the
rewrite must keep the token in those exact files (twins included).

### Semantic obligations that MUST stay matchable (concept anchors)

task-skillist: `skillist-structured`, `skillist-minimal-ordered`,
`skillist-confidence-fields` (AllOf: confidence / matched signals / reviewer
disposition), `skill-breadth` (`small, medium, and broad`),
`aggregate-non-authoritative`, `graph-before-after`, `persistent-launch`,
`seh-discipline`, `tasks-skill-gate` (AllOf), `implement-skill-loading` (AllOf:
five concepts), `constitution-skill-gates` (AllOf), `tasks-post-gen-timing`,
`deps-skillist-doc`. controls-boundary: `controls-skia-rendered`,
`controls-no-charts-shim` (AllOf: `legacy Charts package` + `no compatibility
shim`). sequential-fake: `fake-sequential` (AllOf: `FAKE-backed`, `.fake`,
`sequential`, `not safe to run concurrently`). **AllOf obligations are the
fragile ones** — every listed concept phrase must survive somewhere in each home
file; rewording around them is fine, dropping any one fails.

### Forbidden terms that MUST stay absent

`FS.Skia.UI.Charts`, `fs-skia-charts`, `chart-only`, `DataGrid as chart`,
`DataGrid-as-chart`, `renderer-neutral`, `renderer neutral`, `host-loop
ownership`, `host loop ownership` (controls-boundary), plus the reflection-first
/ repository-source-copy advice phrases. Rephrasing must not reintroduce any.

## Decision: Edit `.agents`, regenerate `.claude`

- **Decision**: All skill-tree edits land in `.agents/skills/**`; run
  `./fake.sh build -t RefreshSurfaceBaselines` to regenerate `.claude/skills/**`;
  never hand-edit `.claude`. `SkillSyncCheck` verifies parity.
- **Rationale**: FR-006 and the constitution's single-source generation rule.
  `serializedRunnerObligation`'s `Files` list includes both `.agents` and
  `.claude` copies of the implement/evidence skills, so a hand-edit that desyncs
  them fails `SkillSyncCheck` *and* risks the obligation in one peer.
- **Alternatives considered**: Editing both trees by hand — rejected: guaranteed
  drift and explicitly forbidden.

## Decision: Twins rewritten in lockstep

- **Decision**: Identical twins today (`constitution-template.md` ×2,
  `tasks-template.md` ×2, etc.) are rewritten to remain byte-identical to each
  other unless an obligation legitimately forces divergence; either way each
  file independently satisfies every obligation/token its `Files` membership
  imposes.
- **Rationale**: FR-007; `active-preset-parity` and per-file token `Files` lists
  catch one-sided edits.
- **Alternatives considered**: Rewrite one twin and copy — acceptable mechanically
  but the copy must still be verified per-file; treat as lockstep, not copy-blind.

## Decision: Size accounting reproduced from `renderProseSizeAccounting` format

- **Decision**: Produce `prose-size-accounting.md` matching the existing
  `renderProseSizeAccounting` layout: corrected baseline (6882), `.agents/skills`
  count, `.specify` count, summed current, signed delta vs baseline, restated
  target, and the two `find … -name '*.md' | xargs wc -l | tail -1` reproduction
  commands. The render function is pure and unit-tested; this feature uses its
  output shape, not a new format.
- **Rationale**: FR-009, SC-007 — byte-deterministic, reproducible, no
  ~23,000-line target. The render value is already in `Guidance.fsi`; the IO
  enumeration that counts lines lives in the front-end / is run by hand from the
  documented commands.
- **Open mechanics**: `renderProseSizeAccounting` is exported but not currently
  wired to a standalone FAKE target (grep of `Front/`/`Targets.fs` found no
  caller). The accounting artifact is produced by running the two documented
  `wc -l` commands and recording them in the render layout — no new target is
  added (Command-surface impact: none).

## Decision: Verification is gate-driven; negative proof is a real mutation

- **Decision**: Green proof = `GeneratedGuidanceCheck` + `SkillSyncCheck` +
  `TargetMetadataDrift` + `TemplateCheck` pass on the rewritten corpus. Red proof
  = mutate one source-of-truth obligation (drop an AllOf concept phrase from a
  home file) and one contract token, observe `GeneratedGuidanceCheck` fail naming
  the file + obligation, then revert. Record both in `rewrite-red-green.md`.
- **Rationale**: SC-002/SC-003/SC-005; proves the rewrite preserved drift
  detection at 055 strength rather than merely making the gate pass.
- **Alternatives considered**: Asserting "gate is green" alone — rejected: a
  weakened gate is also green; the mutation is what proves strength survived.

## Resolved unknowns summary

| Unknown | Resolution |
|---|---|
| What may not be cut? | `Guidance.fs` token/obligation/forbidden inventory (enumerated above). |
| Where to edit? | `.agents` + `.specify` only; regenerate `.claude`. |
| Twin handling? | Lockstep; each file independently satisfies its obligations. |
| Size report format? | `renderProseSizeAccounting` layout, reproduced from two `wc -l` commands. |
| New code/targets? | None. Verified entirely by existing gates. |
| Synthetic evidence? | None; all evidence real. |
