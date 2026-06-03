# Phase 1 Data Model: Governance Markdown Rewrite

This feature introduces **no new runtime data types**. The entities below are the
existing `build/Governance/Guidance.fs` governance types that the rewrite must
honor unchanged, plus the prose-size accounting record it populates. They are
documented here as the contract the rewrite is checked against — not as types to
add or modify.

## Entity: ContractToken (existing — preserved unchanged)

```fsharp
type ContractToken = { Token: string; Files: string list }
```

- **Token**: a literal string consumed by tooling, matched case-insensitive
  substring exactly as the pre-055 table did.
- **Files**: the exact files that must each contain `Token` after the rewrite.
- **Rewrite invariant**: for every token, `Token` remains present (substring) in
  every path in `Files`. Removing a token from any home file is a regression
  (FR-003, SC-002). The rewrite never edits this value — it satisfies it.

## Entity: GuidanceObligation (existing — preserved unchanged)

```fsharp
type MatchMode = AnyOf | AllOf
type GuidanceObligation =
    { Id: string; SourceOfTruth: string; Concepts: string list
      Mode: MatchMode; Files: string list }
```

- **AnyOf**: satisfied when ≥1 concept anchor is present in a file (the US1
  unlock — reword freely as long as one anchor survives).
- **AllOf**: satisfied only when *every* concept anchor is present — the fragile
  case; each listed phrase must survive somewhere in each home file.
- **Rewrite invariant**: each obligation still resolves for every file in `Files`
  under its `Mode` (FR-002, SC-002). Concept anchors are presence-of-concept, so
  surrounding prose may shrink/rephrase, but an AllOf phrase may not be deleted.

## Entity: GuidanceCheck (existing — preserved unchanged)

```fsharp
type GuidanceCheck =
    { Tag: string; Tokens: ContractToken list
      Obligations: GuidanceObligation list; Forbidden: ContractToken list }
```

The three live values — `taskSkillistGuidanceCheck`,
`controlsBoundaryGuidanceCheck`, and the per-path
`serializedRunnerObligation`-bearing check — enumerate the full preservation set.
`Forbidden` tokens must stay absent from the combined governed content
(FR-004, SC-005). The rewrite changes none of these values.

## Entity: ProseSizeAccounting (existing record — populated by this feature)

```fsharp
type ProseSizeAccounting =
    { Baseline: int; AgentsSkillsLines: int; SpecifyLines: int
      Current: int; Delta: int; RestatedTarget: string }
```

| Field | Value source | Rewrite rule |
|---|---|---|
| `Baseline` | 6882 (feature 046) | Consumed, never re-derived (Assumptions). |
| `AgentsSkillsLines` | `find .agents/skills -name '*.md' \| xargs wc -l \| tail -1` | Measured post-rewrite. |
| `SpecifyLines` | `find .specify -name '*.md' \| xargs wc -l \| tail -1` | Measured post-rewrite. |
| `Current` | `AgentsSkillsLines + SpecifyLines` | Summed measured count. |
| `Delta` | `Current - Baseline` | Signed; reported, not targeted. |
| `RestatedTarget` | prose string | "lose no meaning, drop every word that earns nothing"; no fixed line count; no ~23,000 figure. |

- **Validation rules**: `renderProseSizeAccounting` is byte-deterministic over
  this record (FR-009, SC-007). Pre-rewrite snapshot: `AgentsSkillsLines`=4072,
  `SpecifyLines`=2817, `Current`=6889, `Delta`=+7. Post-rewrite values are
  measured and recorded in `readiness/prose-size-accounting.md`; SC-001 requires
  `Current` materially below 6889 achieved purely by tightening.

## No state transitions

The rewrite is a static-document edit. There is no stateful workflow, no
lifecycle, and no I/O-bearing transition — `ProseSizeAccounting` is a one-shot
report value, not a state machine. (Principle IV MVU boundary: N/A, see plan.)

## Relationships

```
Guidance.fs (authority, unchanged)
  ├─ ContractToken ──── must remain present in ─── each Token.Files path (.agents/.specify/template/.claude*)
  ├─ GuidanceObligation ─ must stay matchable in ── each Obligation.Files path
  └─ Forbidden ───────── must stay absent from ──── combined governed content
.agents/skills/** ── regenerates (RefreshSurfaceBaselines) ──▶ .claude/skills/** (SkillSyncCheck verifies)
Routing.fs (unchanged) ── generates ──▶ validation.contract.yml (TargetMetadataDrift verifies)
measured corpus ── populates ──▶ ProseSizeAccounting ── renders ──▶ readiness/prose-size-accounting.md
```

\* `.claude` token/obligation home files appear in some `Files` lists (e.g. the
serialized-runner check lists both `.agents` and `.claude` implement/evidence
copies); they are satisfied via regeneration from `.agents`, never hand-edit.
