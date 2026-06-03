# Silent-Drift Audit (US2, SC-005)

Every generated governance artifact in scope paired with the currency gate that
guards it. **No artifact has an empty guard cell** (no silent drift hole, FR-003).

| Generated artifact | Canonical source | Currency guard | Status |
| --- | --- | --- | --- |
| `gov/visual-proof-phrases` region in `.agents/skills/fs-skia-layout-evidence/SKILL.md` | `GovernedBlocks visual-proof-phrases` | `TargetMetadataDrift` (057 fold) | green |
| `gov/visual-proof-phrases` region in `template/base/docs/product.md` | `GovernedBlocks visual-proof-phrases` | `TargetMetadataDrift` (057 fold) | green |
| `gov/owner-phrases` region in `.agents/skills/fs-skia-layout-evidence/SKILL.md` | `GovernedBlocks owner-phrases` | `TargetMetadataDrift` (057 fold) | green |
| `gov/owner-phrases` region in `template/base/docs/product.md` | `GovernedBlocks owner-phrases` | `TargetMetadataDrift` (057 fold) | green |
| `.claude/skills/fs-skia-layout-evidence/SKILL.md` (peer carrying both regions) | `.agents/skills/fs-skia-layout-evidence/SKILL.md` | `SkillSyncCheck` | green |
| `.claude/skills/speckit-tasks/SKILL.md` (peer, echo removed) | `.agents/skills/speckit-tasks/SKILL.md` | `SkillSyncCheck` | green |
| `.specify/memory/constitution.md` (concrete render) | placeholder-bearing `.specify/templates/constitution-template.md` | `TargetMetadataDrift` (057 fold) | green |
| `.specify/presets/fsharp-opinionated/templates/constitution-template.md` (verbatim twin) | placeholder-bearing `.specify/templates/constitution-template.md` | `TargetMetadataDrift` (057 fold) | green |
| spliced constitution principle-fragment regions (`plan-template.md`, `tasks-template.md`) | `.specify/memory/constitution.md` | `TargetMetadataDrift` (044 fold) | green |
| `validation.contract.yml` | `Routing.fs` | `TargetMetadataDrift` (042 fold) | green (byte-unchanged) |

## Enumeration guarantee (the pairing is computed, not hand-listed)

The 057 fold iterates `GovernedBlocks.governedBlocks` and currency-checks **every**
`(file, mode)` in each block's `Targets`, so a new generated copy cannot be added
without a guard: adding a `Target` automatically adds a `TargetMetadataDrift`
check for it (`build/Governance/Engine/Update.fs`, `governedBlockDiagnostics`). The
`GovernedBlocksTests` "canonical store" test asserts every block declares at least
one target and a marker id, so an unguarded (target-less) block fails CI.

## Verification

- `./fake.sh build -t TargetMetadataDrift` → `Status: Ok` (all gov-block copies,
  constitution fragments, and `validation.contract.yml` current).
- `./fake.sh build -t SkillSyncCheck` → green (`.agents`→`.claude` peers current;
  recorded in `skill-sync-check.md` at integration).
- Red→green proof that the guard actually bites: `dedupe-red-green.md`.
