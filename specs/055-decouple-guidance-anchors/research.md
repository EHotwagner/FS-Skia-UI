# Phase 0 Research: Decouple Author-Guidance Prose from Currency Anchors

## R1 — How to encode a "semantic obligation" so rewording passes but drift fails

**Decision**: Represent each prose obligation as a record with a stable `Id`, a
named `SourceOfTruth`, and a small list of **alternative concept anchors**; the
obligation is satisfied for a file when **any** anchor is present (case-insensitive
substring), and fails (one finding) when **none** is.

```fsharp
type GuidanceObligation =
    { Id: string            // stable handle, used in diagnostics
      SourceOfTruth: string // names where the rule originates
      Concepts: string list // any-of; presence of any one satisfies the obligation
      Files: string list }  // derived guidance files that must reflect it
```

**Rationale**:
- **US1 (rewording passes)** — Today a single long literal substring
  (`"before and after every status change"`, `"minimal ordered skill set"`) must
  appear verbatim; shortening the sentence trips `missing term`. Under any-of
  concept anchors, the obligation lists 1–3 short, semantically core anchors
  (e.g. for "the aggregate is non-authoritative": `["non-authoritative aggregate"]`,
  or for "skillist is structured": `["skillist"]`). A maintainer can rewrite the
  surrounding paragraph freely; the gate passes as long as the concept word
  survives. This is exactly the unlock the §4.4 honesty gap describes.
- **US2 (source-of-truth drift fails)** — The obligation set is the *single place*
  obligations live (`build/Governance/Guidance.fs`). Adding a constitution rule or
  routing capability means adding an obligation row, which the derived guidance
  must then reflect or fail. Removing the concept from a derived file (the "forgot
  to update" case) leaves **no** anchor present → the obligation fails, naming the
  file and the obligation. Detection of *obligation* drift is therefore at least as
  strong as the literal table (FR-003): every obligation the old table encoded maps
  to exactly one obligation row whose anchor set includes the old anchor's core
  token, so the concept cannot be deleted without tripping the gate.

**Alternatives considered**:
- *Derive obligations from the source-of-truth prose by NLP / fuzzy match* —
  rejected: violates Principle III (idiomatic simplicity), non-deterministic,
  unbounded scope. The spec's Assumptions explicitly bless a structured obligation
  list keyed per file over machine-reading arbitrary prose.
- *Generate the obligation list from constitution/Routing at build time* —
  rejected for this feature: the constitution's "Local Agent Skills" rules are not
  yet a machine-readable schema, and the three checks have heterogeneous sources
  (constitution, package reality, FAKE-concurrency rule). A hand-maintained
  obligation table in `build/Governance/**` is still single-sourced and currency-
  checked, satisfies FR-010, and is the smaller, honest step. Full source-derivation
  is a possible later refinement, noted as deferred.
- *Keep exact substrings but add a "loose match" flag* — rejected: that is the same
  freeze with extra ceremony; it does not separate the two concerns the spec names.

## R2 — Which pinned strings are machine-contract tokens vs prose

**Decision**: A pinned string is a **contract token** (stays verbatim) only when
tooling/parsers actually consume it; otherwise it is prose (becomes an obligation
concept). Concretely, contract tokens are:

- Bracketed task/legend tags: `[skillist: []]`, `[SEH]`
- Kebab-case machine flags: `synthetic-error-handling-approved`
- YAML keys / structured fields: `skillist:`, `deps:`
- Typed/identifier tokens in the controls boundary: `Control<'msg>`,
  `FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, `ControlsElmish.program`,
  and the `forbidden` removed-Charts identifiers (`FS.Skia.UI.Charts`,
  `fs-skia-charts`, and the assembled stale phrases)
- Evidence-schema field names in `speckit-implement`: `loaded_at`, `work_started_at`,
  `readiness/skill-loading-evidence.md`

Everything else in the three tables is prose: `"structured"`, `"After task
generation"`, `"minimal ordered skill set"`, `"confidence"`, `"matched signals"`,
`"reviewer disposition"`, `"small, medium, and broad"`, `"non-authoritative
aggregate"`, `"before and after every status change"`, `"persistent launch rules"`,
`"MUST reject viewer-backed default executable paths"`, `"Skia-rendered"`,
`"legacy Charts package"`, `"no compatibility shim"`, the `sequential-fake`
required terms (`"FAKE-backed"`, `".fake"`, `"sequential"`, `"not safe to run
concurrently"`), etc.

**Ambiguity rule** (from the spec Assumptions): resolve toward "contract token" only
when tooling parses the string; otherwise treat as prose. `.fake` and `FAKE-backed`
read like tokens but are *prose anchors for a human rule* (the FAKE-concurrency
constraint) — they are not parsed, so they become obligation concepts; however the
`sequential-fake` check's **regex-driven** structural assertions (a numbered command
list, the parallelism caveat) are inherently machine logic and stay as-is.

**Rationale**: Edge case in the spec — `[skillist: []]` is consumed by the evidence
audit/task parser, `[SEH]` and `synthetic-error-handling-approved` gate the audit,
YAML keys are parsed by `tasks.deps.yml` loaders, and the controls identifiers are
real F# symbols. Re-padding or rewording any of these would break tooling, so they
must stay literal (SC-004).

## R3 — `controls-boundary-guidance` forbidden-list and combined-content semantics

**Decision**: Preserve the forbidden (stale-term) list **verbatim** and keep the
"concatenate all governed files then search the combined string" semantics for it.
Forbidden terms are contract-shaped (removed package/skill identifiers and assembled
stale phrases) and must never reappear (FR-006, SC edge case). Required terms split:
the identifiers become contract tokens (still required verbatim, still over the
combined content), and `"Skia-rendered"` / `"legacy Charts package"` /
`"no compatibility shim"` become obligation concepts.

**Rationale**: The forbidden behavior is the regression guard the spec calls out as
must-preserve; it is already a token/anti-token mechanism, not prose, so it needs no
decoupling — only explicit classification.

## R4 — `sequential-fake-guidance` structural assertions

**Decision**: Keep the regex-driven structural checks (FAKE command present ⇒
required terms present; multiple commands ⇒ numbered order; parallelism mention ⇒
non-FAKE caveat) unchanged. Reclassify the four `serializedRunnerRequiredTerms` as
an obligation ("FAKE-backed commands run sequentially, never concurrent") whose
concept anchors are those four terms (any-of would weaken it here, so this
obligation keeps **all-of** semantics — see R5). The structural regex logic is
machine contract and stays.

## R5 — Per-obligation matching mode (any-of vs all-of)

**Decision**: An obligation declares its match mode. Most prose obligations use
**any-of** (the unlock for US1). A few obligations that genuinely require *several*
distinct concepts present together — e.g. the FAKE-sequential rule, which is
meaningless unless all four facets appear — use **all-of** over their concept set.
This keeps drift detection strong where the rule is genuinely conjunctive without
re-freezing wording (each facet still matches a short concept anchor, not a full
sentence).

```fsharp
type MatchMode = AnyOf | AllOf
type GuidanceObligation = { Id; SourceOfTruth; Concepts; Mode; Files }
```

**Rationale**: Avoids the trap where collapsing a conjunctive rule to any-of would
let a file satisfy the obligation while dropping three of four required facets — a
real weakening of US2. `AllOf` over short anchors is still far looser than the
literal long-substring table (you may reword each facet's sentence freely).

## R6 — Pure core for testability (red→green)

**Decision**: Refactor each validator into `evaluate : (string -> string option)
-> finding list` over a `path → content option` lookup (a pure function of an
in-memory content map), with the existing front-end IO wrapper supplying real file
reads. Tests construct a content map with realistic edits to demonstrate
red→green without mutating the working tree.

**Rationale**: The existing tests run the validator over the real repo (good for
SC-006 regression) but cannot show a reworded edit passing without editing real
files. A pure core lets US1/US2 be unit-tested deterministically and keeps the
real-repo scan as the integration regression. Mirrors the repo's existing
"pure plan + front-end IO enumeration" pattern (e.g. `SkillTreeGen` plan vs
`Governance.fs` enumeration).

## R7 — Prose-size accounting methodology and the restated goal

**Decision**: Reuse feature 046's established methodology verbatim:
governance-prose lines = `find .agents/skills -name '*.md' | xargs wc -l` +
`find .specify -name '*.md' | xargs wc -l`. The corrected baseline is **≈6,882**
(046; 047 after-trim 6,876). The accounting report (`readiness/prose-size-accounting.md`)
states baseline, current measured count, delta, and the **restated target**.

The restated goal (FR-008) adopts the spec's second branch: **retire the
"low hundreds" / ~23,000 figure as a live target.** Rationale recorded in the
canonical record: the original figure was an acknowledged over-estimate; the honest
target is measured against ≈6,882, and the actual reduction is a bounded follow-up
now that the freeze is lifted. The canonical record
(`docs/reports/_baselines/2026-06-02-foundations-after.md` row 5, and the peer
`after-baseline.md`) is edited so it no longer cites ~23,000 as the live target and
points at the new accounting report.

**Rationale**: The spec is explicit that this feature makes the goal *honest and
measurable*, not that it mandates a specific final count; chasing a number everyone
agrees was wrong is the failure mode being corrected.

## R8 — Single-source-generation invariants to keep green (FR-010)

**Decision / findings**:
- `validation.contract.yml` is generated from `Routing.fs`. We do **not** edit
  `Routing.fs` rules, so the contract does not regenerate and `TargetMetadataDrift`
  stays green. Confirmed by `ContractView.currencyDrift` comparing on-disk bytes to
  `ContractView.render Routing.rules Routing.dogfoodFeatureIds`.
- The `.claude/skills` tree is a byte-identical generated reproduction of canonical
  `.agents/skills`. If prose in an `.agents/skills/*/SKILL.md` is tightened, the
  matching `.claude` copy must be regenerated via
  `./fake.sh build -t RefreshSurfaceBaselines`; `SkillSyncCheck` enforces currency.
  Any `.specify` preset twin of a template (e.g. `tasks-template.md` and its
  `fsharp-opinionated` copy) must be edited in lockstep — the obligation `Files`
  lists both, so drift in one is still caught (spec edge case).
- No constitution-fragment generated region (`BEGIN/END GENERATED`) is altered
  unless a fragment's prose is tightened, in which case `RefreshSurfaceBaselines`
  re-splices and `TargetMetadataDrift` validates currency.

## R9 — Route escalation for this diff

**Findings**: The diff spans `.specify/**` (matches `specify-catchall` +
`generated-guidance` → FocusedAuthority, artifact `evidence-policy-separation.md`,
gates `GeneratedGuidanceCheck` + `TemplateDrift`), `docs/**` (matches `docs-only` →
EvidenceGraph, artifact `validation-contract.md`), and `build/Governance/**` +
`tests/**` (matched by no rule but, because the overall match set is non-empty, they
ride the escalated selection rather than the empty-match maintainer-verify
fallback). Net expected tier is **focused-authority/agent-ready**; the spec author
predicts escalation toward the maintainer-verify serialized order. The authoritative
list is whatever `./fake.sh build -t Route --enforce` prints at the feature SHA — we
run exactly that and prepare evidence for the artifacts it names (SC-006).
