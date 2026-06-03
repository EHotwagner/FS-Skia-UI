# Contract: Guidance Currency (post-decoupling)

This is the governance **currency contract** — the behavioral interface the
`GeneratedGuidanceCheck` gate must honor after author-guidance prose is decoupled
from generation-currency anchors. It is single-sourced in `build/Governance/**`;
no consumer-facing product `.fsi` or surface baseline is affected.

## Categories enforced

1. **Machine-contract tokens** (`ContractToken`) — matched verbatim
   (case-insensitive substring), per governed file. Set MUST include at minimum:
   `[skillist: []]`, `[SEH]`, `synthetic-error-handling-approved`, `skillist:`,
   `deps:`, `Control<'msg>`, `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`,
   `ControlsElmish.program`, `loaded_at`, `work_started_at`,
   `readiness/skill-loading-evidence.md`. (SC-004)
2. **Semantic obligations** (`GuidanceObligation`) — checked by presence-of-concept
   (`AnyOf`/`AllOf` over short concept anchors), not by exact wording. (FR-002)
3. **Forbidden tokens** — stale terms that MUST NOT appear (controls boundary).
   Behavior preserved verbatim. (FR-006)

## Pass/fail rules

| Condition | Result |
|---|---|
| Every `ContractToken` present in each of its `Files` | required for PASS |
| Every `GuidanceObligation` satisfied (per `Mode`) in each of its `Files` | required for PASS |
| No `Forbidden` token present in combined governed content | required for PASS |
| A reworded/shortened prose edit that preserves all obligation concepts | **PASS** (SC-001) |
| A source-of-truth obligation removed/altered without updating derived guidance | **FAIL**, diagnostic names file + obligation id + source (SC-002) |
| Any `ContractToken` removed | **FAIL** (SC-004) |
| Any `Forbidden` token reintroduced | **FAIL** (FR-006) |
| A governed file missing | **FAIL** (`missing file`) |

## Finding-tag taxonomy (preserved — FR-005)

- `task-skillist-guidance`
- `controls-boundary-guidance`
- `sequential-fake-guidance`

Obligation finding format:
`"{file}: obligation '{id}' ({source}) not reflected [{tag}]"`
Token finding format (unchanged):
`"{file}: missing `{token}` [{tag}]"` /
`"generated controls guidance contains stale term `{token}` [{tag}]"`

## Coverage obligations

- **Twins** — when an obligation/token applies to a template and its
  `fsharp-opinionated` preset copy (and command/memory copies), every twin appears
  in `Files`; drift in one twin still fails. (spec edge case)
- **All three sites migrated** — `task-skillist-guidance`,
  `controls-boundary-guidance`, and `sequential-fake-guidance` are all converted;
  no list still freezes prose purely as a currency proxy. (SC-003)
- **No weakening of obligation drift** — every obligation the pre-055 literal table
  encoded maps to exactly one obligation row whose anchor set includes that anchor's
  core concept; a test removing the concept fails. (FR-003, SC-002)

## Single-source-generation invariants (FR-010)

- `validation.contract.yml` is generated from `Routing.fs` and stays byte-current
  (`TargetMetadataDrift` green) — not edited by this change.
- `.claude/skills/**` stays a byte-identical generated reproduction of
  `.agents/skills/**` (`SkillSyncCheck` green); regenerate via
  `RefreshSurfaceBaselines` after any canonical skill-prose edit.

## Prose-size accounting (FR-007, FR-008, SC-005)

A report states: corrected baseline ≈6,882; current measured guidance-prose line
count (`.agents/skills/**/*.md` + `.specify/**/*.md`); the delta; and the restated
target. The canonical baseline/goal record no longer cites ~23,000 as the live
target.
