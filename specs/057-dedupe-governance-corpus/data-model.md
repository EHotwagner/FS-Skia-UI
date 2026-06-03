# Phase 1 Data Model: Single-Source the Duplicated Governance Corpus

The feature is a governance-tooling refactor, so its "data model" is the set of
compiled F# values and generated artifacts that own and verify governed content.
No product domain types change. All types live under `build/Governance/**` and are
exercised by `tests/Governance.Tests/**`.

## Entities

### DuplicationInstance (catalogue row — FR-001)

One occurrence of identical governed content carried in more than one place.

| Field | Meaning |
| --- | --- |
| `Id` | stable slug, e.g. `seh-token`, `controls-skia-rendered`, `skill-phrases-echo`, `principle-v-body` |
| `Class` | one of `TokenCarriage` \| `ObligationAnchor` \| `InFileEcho` \| `ConstitutionPrinciple` |
| `Content` | the duplicated text (or token) |
| `HomeFiles` | every file currently carrying the copy |
| `RequiringValidator` | the gate/check that requires it today (`task-skillist-guidance`, `controls-boundary-guidance`, `evaluateGuidanceCheck`, `ConstitutionFragments`, …) |
| `Resolution` | `DeleteScanCanonical` (in-repo scanner reads source) \| `GenerateAndCheck` (splice + currency) |
| `CanonicalSource` | the single owner after the refactor |
| `CurrencyGate` | the gate that fails on drift of the generated copy |

The catalogue is the FR-001 deliverable, written to
`readiness/duplication-catalogue.md`. Seed rows (from Phase 0):

- `seh-token` / `synthetic-error-handling-approved` — TokenCarriage, **9 home
  files** (2 tasks-templates, `speckit-tasks` SKILL + `speckit.tasks.md`,
  `speckit-implement` SKILL + `speckit.implement.md`, 3 constitution files) →
  GenerateAndCheck.
- controls tokens (`FS.Skia.UI.Controls`, `Control<'msg>`, `DataGrid`,
  `FS.Skia.UI.Controls.Elmish`) — TokenCarriage across 4–7 files → GenerateAndCheck
  where multi-file identical.
- `controls-skia-rendered`, `controls-no-charts-shim`, the skillist obligations —
  ObligationAnchor across twin files → GenerateAndCheck.
- `skill-phrases-echo`, `readiness-phrases-echo`, `visual-proof-echo`,
  `owner-phrases-echo` — InFileEcho → DeleteScanCanonical (in-repo) /
  GenerateAndCheck (shipped/agent files).
- `principle-{ii,iv,v,vi,vii,…}-body` + `local-agent-skills` — ConstitutionPrinciple
  across the 3 constitution files → GenerateAndCheck.

### GovernedBlock (new canonical store — FR-002)

The canonical, single-source value for a generated prose block.

| Field | Meaning |
| --- | --- |
| `Id` | matches the marker id used in `BEGIN/END GENERATED: gov/<id>` |
| `CanonicalText` | the one true text (placeholder-bearing where it has per-target variants) |
| `Targets` | list of `(filePath, RenderMode)` |
| `Tokens` | the `ContractToken`s this block satisfies (cross-ref into `Guidance.fs`) |
| `Obligations` | the `GuidanceObligation` ids this block satisfies |

`RenderMode = Verbatim | Substituted of Map<placeholderId,string>`. Verbatim is the
twin/templated case (placeholders preserved); `Substituted` is the
`constitution.md` case (placeholders filled with repo values).

This generalizes today's `ConstitutionFragments.PrincipleFragment`
(`{ FragmentId; SourceHeading; RenderedText }`, `build/Governance/ConstitutionFragments.fs`)
from first-sentence-only to whole-block, and adds the substitution map for the
constitution twins.

### GeneratedCopy / CurrencyGuard (FR-003, SC-005)

Reuses existing currency machinery; no new shape required if folded into
`TargetMetadataDrift`. The audit pairs every generated artifact with its guard:

| Generated artifact | Canonical source | Currency guard |
| --- | --- | --- |
| spliced `gov/<id>` regions in templates/SKILL/command/constitution files | `GovernedBlock.CanonicalText` | `TargetMetadataDrift` (new fold) |
| `constitution.md` + 2 `constitution-template.md` twins (full body) | placeholder-bearing principle source | `TargetMetadataDrift` (extended `ConstitutionFragments` fold) |
| `.claude/skills/**` peers of edited `.agents` files | `.agents/skills/**` | `SkillSyncCheck` |
| `validation.contract.yml` (unchanged) | `Routing.fs` | `TargetMetadataDrift` |

`readiness/silent-drift-audit.md` enumerates this table as the SC-005 deliverable —
every generated artifact has a non-empty guard cell.

### Untouched governed types (FR-004 — single home of the rule set)

`ContractToken`, `MatchMode`, `GuidanceObligation`, `GuidanceCheck`, and
`evaluateGuidanceCheck` (`build/Governance/Guidance.fs:98–184`) keep their
shapes. The `Files` lists and the *carriage* change; the rule *set* does not. The
forbidden-term lists (assembled by substring concatenation to hide stale terms)
are unchanged.

## State / lifecycle

The generation lifecycle is the existing one, extended:

```
edit canonical (GovernedBlock / placeholder principle source / .agents skill)
  → ./fake.sh build -t RefreshSurfaceBaselines   (splice + render + skill tree)
  → derived copies updated identically in place
  → TargetMetadataDrift / SkillSyncCheck assert each copy == fresh render
  → GeneratedGuidanceCheck asserts every token/obligation still present
```

The **new failure transition** (FR-005, SC-004): hand-edit a generated copy so it
diverges from canonical → `TargetMetadataDrift` (or the new fold) fails naming the
file **and** its source → regenerate → green.

## Validation rules (carried into tests)

- Every 056 negative case still fails (deleted obligation concept; removed contract
  token; reintroduced forbidden term) — `tests/Governance.Tests/GuidanceValidatorTests.fs`
  already encodes these; preserve them.
- New generated-copy-drift case fails a currency gate naming file + source —
  mirror `ConstitutionFragmentsTests.fs` (`currency`/`currencyDrift`) and
  `SkillSyncTests.fs` for the new `GovernedBlock` currency.
- No generated artifact lacks a guard (SC-005) — an enumerated test/audit over the
  GeneratedCopy table.
- Genuine vs legitimate-difference discrimination (FR-011): only identical-content
  multi-file duplication is single-sourced; per-target variants are expressed as
  placeholders, not collapsed away.
