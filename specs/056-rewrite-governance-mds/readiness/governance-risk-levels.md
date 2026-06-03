# Governance Risk Levels — Feature 056

Risk classification for the big rewrite of the governance Markdown corpus
(`.agents/skills/**/*.md`, `.specify/**/*.md`) — Tier 2, governance-internal
prose change with `.claude` regenerated from `.agents`.

## Levels

- **small** — a single framework-internal edit (e.g. one `src/Scene/**` file)
  that routes to the inner-loop tier (`Dev` only). Not this feature.
- **medium** — governance prose tightening localized to a few governed files.
  The **required evidence** for the medium level is the focused validation set
  (`GeneratedGuidanceCheck` plus the recorded obligation-mutation that still
  fails the gate).
- **broad** — a change spanning the whole canonical corpus, template/preset
  twins, the regenerated `.claude` tree, and governance evidence, so `Route`
  escalates to the maintainer-verify tier and **broad validation** (the full
  serialized six-target order) is required.

## This feature

Selected level: **medium → broad at integration.** The diff spans the entire
`.agents/skills/**/*.md` and `.specify/**/*.md` corpus (including the
template/preset twins and `constitution.md`), the regenerated
`.claude/skills/**` tree, and readiness evidence, so `Route` escalates.

- **required evidence** (focused): `GeneratedGuidanceCheck` **green** over the
  rewritten corpus (every C1 token present, every C2 obligation resolved, no C3
  forbidden term) plus the recorded source-of-truth obligation mutation and
  contract-token removal that still **fail** the gate with a file+obligation
  diagnostic, then revert to green.
- **broad validation** is required at integration (Phase 6): the serialized
  order `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` →
  `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`.

## Aggregate FAKE results

Aggregate FAKE runs are recorded as **non-authoritative**; any race-like failure
is rerun in focused isolation as the authoritative result (FAKE shares `.fake`
state and is never run concurrently).
