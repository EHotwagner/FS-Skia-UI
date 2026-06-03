# Governance Risk Levels — Feature 057

Risk classification for the **structural** single-sourcing of the duplicated
governance corpus — collapsing per-file token/obligation carriage, in-file
scanner echoes, and the constitution/template/fragment triple-maintenance onto
canonical sources with generated, currency-checked copies. Tier 2,
governance-internal change that edits `build/Governance/**` (the rule *carriage*,
not the rule *set*) and the governed corpus, with `.claude` regenerated from
`.agents`.

## Levels

- **small** — a single framework-internal edit (e.g. one `src/Scene/**` file)
  that routes to the inner-loop tier (`Dev` only). Not this feature.
- **medium** — governance change localized to a few governed files. The
  **required evidence** for the medium level is the focused validation set
  (`GeneratedGuidanceCheck` + `TargetMetadataDrift` over the regenerated corpus,
  plus the recorded drift mutation that still fails the gate).
- **broad** — a change spanning the compiled governance front-end
  (`build/Governance/**`), the whole canonical corpus, the template/preset twins
  and `constitution.md`/`constitution-template.md`, the regenerated
  `.claude/skills/**` tree, and governance evidence, so `Route` escalates to the
  maintainer-verify tier and **broad validation** (the full serialized
  six-target order) is required.

## This feature

Selected level: **medium → broad at integration.** The diff spans the compiled
governance front-end (`build/Governance/**`: `Guidance.fs` carriage,
`ConstitutionFragments`, the new `GovernedBlocks` store, `TargetMetadata`,
`Engine/Update.fs`), the governed corpus (`.agents/skills/**`, `.specify/**`
including the template/preset twins and `constitution.md`/`constitution-template.md`,
`template/base/docs/product.md`, `src/Controls/skill/SKILL.md`), the regenerated
`.claude/skills/**` tree, and readiness evidence, so `Route` escalates.

- **required evidence** (focused): `GeneratedGuidanceCheck` **green** over the
  regenerated corpus (every contract token present, every obligation resolved,
  no forbidden term) and `TargetMetadataDrift` **green** (every generated copy
  current with its canonical source), plus the recorded drift mutations — a
  deleted obligation concept, a removed contract token, a reintroduced forbidden
  term, and a hand-edited generated copy out of sync with its source — that each
  still **fail** the relevant gate with a file+source diagnostic, then revert to
  green.
- **broad validation** is required at integration (Phase 7): the serialized
  order `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.

## Aggregate FAKE results

Aggregate FAKE runs are recorded as **non-authoritative**; any race-like failure
is rerun in focused isolation as the authoritative result (FAKE shares `.fake`
state and is never run concurrently).
