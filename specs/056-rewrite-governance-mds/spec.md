# Feature Specification: Big Rewrite of the Governance Markdown Corpus

**Feature Branch**: `056-rewrite-governance-mds`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "follow up from last feature. big rewrite of the mds"

## Context & Problem

Feature 055 *decoupled* author-guidance prose from generation-currency anchors:
the literal-substring term tables that used to pin exact wording are gone,
replaced by **semantic obligations** (checked by meaning/presence-of-concept)
plus an enumerated set of **machine-contract tokens** (still matched verbatim).
055 deliberately stopped at lifting the freeze and recorded the rest as a bounded
follow-up — its own Assumptions state "the actual large-scale prose rewrite can be
a follow-up once the freeze is lifted," and its prose-size accounting notes the
feature "deliberately does not mandate a final line count."

This is that follow-up. With the freeze lifted, the governance Markdown corpus —
≈6,889 lines today against the corrected ≈6,882 baseline (feature 046) — can
finally be tightened. The corpus grew under the old regime where every sentence
doubled as a pinned anchor, so it accumulated redundancy that no longer needs to
exist:

- The `.agents/skills/**/*.md` skill tree (≈4,072 lines) carries verbose,
  repetitive SKILL.md prose (the largest single files run 250–367 lines each).
- The `.specify/**/*.md` set (≈2,817 lines) contains near-duplicate
  template/preset twins — `templates/constitution-template.md` (328) and
  `presets/fsharp-opinionated/templates/constitution-template.md` (328) are
  essentially the same document, as are the two `tasks-template.md` copies
  (315 / 300).

The goal of this feature is a **substantive, maximizing prose rewrite**: make the
guidance as tight and clear as possible across the full canonical corpus while
losing **no** semantic obligation, **no** machine-contract token, and **no**
currency-check strength. No fixed final line count is mandated; the binding
constraint is "lose no meaning, drop every word that earns nothing," and the
prose-size accounting reports whatever reduction results. The canonical edit
surface is `.agents` (skill tree) and `.specify` (templates, constitution,
presets); `.claude` is regenerated from `.agents`, not hand-edited.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Read tighter, clearer guidance (Priority: P1)

A maintainer or agent opens a governance guidance file (a SKILL.md, a template,
the constitution) and finds it materially shorter and clearer than before:
redundancy, restating, and ceremony removed, while every rule, obligation, and
contract token it must convey is still present and unambiguous.

**Independent test**: Pick a rewritten file, diff it against its pre-feature
version, and confirm (a) it is shorter, (b) every semantic obligation and machine-
contract token the currency contract attributes to that file is still present,
and (c) a reader can still extract every rule it previously conveyed. The
`GeneratedGuidanceCheck` gate passes on the rewritten file.

**Why this priority**: This is the feature. The whole point is tighter guidance;
without it nothing else matters.

### User Story 2 - Currency and contract tokens survive the rewrite (Priority: P1)

A maintainer runs the governance gates after the rewrite. Every semantic
obligation still resolves, every enumerated machine-contract token is still
present verbatim, every forbidden/stale term is still absent, and all
single-source-generated artifacts (`validation.contract.yml`, the `.claude` skill
tree) are still current.

**Independent test**: Run `GeneratedGuidanceCheck`, `SkillSyncCheck`,
`TargetMetadataDrift`, and the generated-artifact currency checks at the feature
SHA and observe green. Separately, mutate a source-of-truth obligation without
updating the rewritten guidance and confirm the gate still **fails** — proving the
rewrite preserved, not weakened, drift detection.

**Why this priority**: A rewrite that drops an obligation or a contract token, or
that lets a generated artifact drift, is a regression that silently weakens the
governance contract. The rewrite is only acceptable if currency stays exactly as
strong as 055 left it.

### User Story 3 - Honest, updated size accounting (Priority: P2)

A maintainer or reviewer wants to know what the rewrite actually achieved. They
read the prose-size accounting and see the corrected ≈6,882 baseline, the new
measured line count, and the reduction delta — the real number, not a target
chased or a figure padded.

**Independent test**: Run the size-accounting step and read a report stating the
baseline, the post-rewrite measured guidance-prose line count, and the achieved
reduction. The report is byte-deterministic and reproduces from the documented
commands.

**Why this priority**: Closes the honesty loop 055 opened. Depends on US1/US2
landing first; it measures the rewritten world.

### Edge Cases

- A file is a template/preset **twin** (e.g. `constitution-template.md` and its
  `fsharp-opinionated` copy) → both are rewritten in lockstep; if the two are
  meant to stay identical they must remain identical, and if one legitimately
  diverges that divergence must be intentional and still satisfy both files'
  obligations.
- A `.claude` skill file is edited by hand instead of regenerated from `.agents`
  → `SkillSyncCheck` must catch the drift; the rewrite must touch the canonical
  `.agents` source and regenerate.
- A sentence looks like removable ceremony but actually carries a machine-contract
  token or a semantic obligation → it must NOT be deleted; the contract-token
  enumeration and obligation list from 055 are the authority on what cannot be cut.
- Prose is shortened so aggressively that an obligation becomes ambiguous or
  unreadable → this fails US1's "reader can still extract every rule" bar even if
  the currency gate's keyword still technically matches; tightness must not cost
  comprehension.
- A forbidden/stale term is reintroduced while rephrasing → must still be rejected
  (055's forbidden-list behavior is preserved).
- The rewrite reduces a file below any incidental length other tooling assumed →
  no gate may depend on a *minimum* prose length; if one does, that is itself a
  hidden freeze and is out of spec for this feature to satisfy by padding.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The guidance Markdown across the full canonical corpus
  (`.agents/skills/**/*.md` and `.specify/**/*.md`, including template/preset
  twins and the constitution) MUST be rewritten to be tighter and clearer,
  removing redundancy, repetition, and ceremony that carries no rule.
- **FR-002**: The rewrite MUST preserve **every** semantic obligation defined by
  the 055 currency model; no obligation may become unresolved as a result of the
  rewrite.
- **FR-003**: The rewrite MUST preserve **every** enumerated machine-contract
  token verbatim (e.g. `[skillist: []]`, `[SEH]`,
  `synthetic-error-handling-approved`, `skillist:`, `deps:`, `Control<'msg>`,
  `FS.Skia.UI.Controls`, `loaded_at`, `work_started_at`,
  `readiness/skill-loading-evidence.md`), per the 055 contract-token enumeration.
- **FR-004**: The rewrite MUST preserve the forbidden/stale-term behavior: no
  rephrasing may reintroduce a term on the forbidden list.
- **FR-005**: Every rewritten rule MUST remain extractable by a human reader —
  tightness may not render any obligation ambiguous, incomplete, or unreadable.
- **FR-006**: Edits MUST be made to the canonical source tree (`.agents`,
  `.specify`); the `.claude` skill tree MUST be regenerated from `.agents` (via
  `RefreshSurfaceBaselines`) and never hand-edited, so `SkillSyncCheck` stays
  green.
- **FR-007**: Template/preset twins MUST be rewritten consistently: twins that are
  meant to be identical stay identical; any intentional divergence still satisfies
  both files' obligations.
- **FR-008**: All single-source-generated governance artifacts
  (`validation.contract.yml` from `Routing.fs`, the `.claude` skill tree from
  `.agents`) MUST remain generated, current, and green after the rewrite — none
  hand-synced.
- **FR-009**: The prose-size accounting MUST be updated to report the corrected
  ≈6,882 baseline, the post-rewrite measured guidance-prose line count, and the
  achieved reduction delta, reproducibly and byte-deterministically.
- **FR-010**: The rewrite MUST NOT change what the guidance *says to do* — the
  rules, obligations, and workflows themselves are unchanged; only their wording,
  length, and redundancy change. No new rule is introduced and none is dropped.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, contents, versions, or generated
  consumers change. This is a documentation/governance-prose change. No
  Charts/DataGrid migration is involved.
- **Public contract impact**: No product `.fsi` signatures or surface baselines
  change. The governance currency contract (semantic obligations + machine-
  contract tokens established by 055) is **preserved exactly**, not altered; if
  any obligation/token set must change, that is out of scope and signals the
  rewrite went too far. `validation.contract.yml` stays generated from
  `Routing.fs`.
- **State workflow impact**: None. No stateful product workflow, I/O, command,
  effect, subscription, or interpreter behavior changes.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering,
  screenshot, Vulkan, Skia, or visual-output behavior changes.
- **Evidence obligations**: Real evidence required under
  `specs/056-rewrite-governance-mds/readiness/`: an updated prose-size accounting
  (before/after line counts and achieved reduction), a demonstration that
  obligations and contract tokens still resolve after the rewrite (US2 evidence),
  and a record that source-of-truth drift still fails post-rewrite. Standard
  aggregate-hang-diagnostics and skill-loading-evidence apply per the escalated
  path.
- **Unsupported scope**: No new product features, no visual-parity work, no
  efficiency/timing instrumentation, no Charts split. This feature does not change
  the governance *rules* or the 055 currency *model* — only the wording and size
  of the guidance prose. It does not re-derive the baseline figure (055/046
  established it). It mandates no fixed final line count.
- **Build-target impact**: `GeneratedGuidanceCheck` is the primary affected gate.
  `SkillSyncCheck` (`.claude` regenerated from `.agents`), `TargetMetadataDrift` /
  `validation.contract.yml`, and `TemplateCheck` (templates rewritten) must stay
  current and green. Because the change touches governance paths, `.specify/**`,
  and public guidance templates, `Route` is expected to **escalate** to the
  maintainer-verify path (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`).

## Success Criteria *(mandatory)*

- **SC-001**: The measured guidance-prose line count across the full canonical
  corpus is **materially lower** than the ≈6,889-line pre-feature count, with the
  reduction recorded in the prose-size accounting; the reduction is achieved
  purely by tightening prose, never by deleting an obligation or token.
- **SC-002**: 100% of the semantic obligations and machine-contract tokens
  enumerated by feature 055 still resolve after the rewrite —
  `GeneratedGuidanceCheck` is green, and a test removing any enumerated contract
  token still fails.
- **SC-003**: Mutating a source-of-truth obligation without updating the rewritten
  guidance still fails `GeneratedGuidanceCheck` with a diagnostic naming the file
  and unmet obligation — proving the rewrite preserved drift detection at 055
  strength.
- **SC-004**: The `.claude` skill tree is regenerated (not hand-edited) and
  `SkillSyncCheck` is green; `validation.contract.yml` remains generated from
  `Routing.fs` and `TargetMetadataDrift` is green.
- **SC-005**: No forbidden/stale term is present in the rewritten corpus, and a
  test reintroducing one still fails.
- **SC-006**: Every rewritten file remains complete and readable — a reviewer can
  point to each previously conveyed rule in the rewritten text (no rule lost to
  over-compression).
- **SC-007**: The prose-size accounting reports the corrected ≈6,882 baseline, the
  post-rewrite measured count, and the achieved reduction; it no longer cites the
  discredited ~23,000-line figure as a target.
- **SC-008**: The full escalated six-target order is green at the feature SHA, and
  `Route --enforce` reports all required evidence artifacts present.

## Assumptions

- "Maximize reduction" means: cut every word that earns nothing, but never at the
  cost of a lost obligation, a lost contract token, or a rule a reader can no
  longer extract. No fixed final line count is a success bar; the achieved number
  is reported, not targeted (per the user's chosen ambition and 055's stance).
- Scope is the **full canonical corpus**: `.agents/skills/**/*.md` and
  `.specify/**/*.md` (templates, constitution, presets/twins). `.claude` is out of
  scope as an edit surface — it is regenerated from `.agents`. Root docs
  (`CLAUDE.md`, `AGENTS.md`, ≈98 lines) and `template/**` product-facing docs are
  not the focus but may be tightened opportunistically if doing so changes no
  rule and trips no gate.
- The 055 contract-token enumeration and semantic-obligation set are the
  **authority** on what may not be cut; this feature consumes them and does not
  redefine them. If the rewrite reveals an obligation that is itself redundant,
  retiring it is out of scope (that would be a currency-model change, not a prose
  rewrite).
- The corrected baseline to report against is the ≈6,882 lines established by
  features 046/055; this feature does not re-derive it.
- Template/preset twins that are identical today are assumed meant to stay
  identical unless their respective obligations require divergence.

## Key Entities

- **Governance Markdown corpus**: the full canonical guidance prose under
  `.agents/skills/**/*.md` and `.specify/**/*.md` that this feature rewrites.
- **Semantic obligation**: a 055 rule that a derived guidance file must reflect,
  checked by meaning rather than exact wording — preserved, never dropped.
- **Machine-contract token**: a literal string consumed by tooling (bracketed
  tags, structured keys, exact field names) that must survive the rewrite
  verbatim, per the 055 enumeration.
- **Template/preset twin**: a pair of near-identical files (e.g. a template and
  its `fsharp-opinionated` preset copy) rewritten in lockstep.
- **Generated artifact**: `validation.contract.yml` (from `Routing.fs`) and the
  `.claude` skill tree (from `.agents`) — regenerated, never hand-synced.
- **Prose-size accounting**: the byte-deterministic report of baseline, post-
  rewrite count, and achieved reduction.
