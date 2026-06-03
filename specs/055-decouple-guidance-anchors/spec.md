# Feature Specification: Decouple Author-Guidance Prose from Generation-Currency Anchors

**Feature Branch**: `055-decouple-guidance-anchors`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "5.5 Decouple author-guidance prose from generation-currency anchors — The reason governance Markdown can't shrink (§4.4) is that term-checks pin specific phrases. Separating 'genuine guidance an agent reads' from 'term anchors the currency check requires' would let the prose actually reach the 'low hundreds' goal — or, more honestly, let the goal be restated against the corrected ≈6,882 baseline."

## Context & Problem

The governance library enforces that generated and authored guidance Markdown
stays current by asserting that specific literal phrases appear in specific
files. The clearest example is the `validateTaskSkillistGuidance` term-anchor
table in `build/Governance/Guidance.fs` (≈120 phrase entries across ~11 files
such as `.specify/templates/tasks-template.md`,
`.specify/memory/constitution.md`, and the `speckit-tasks` / `speckit-implement`
skill copies). Sibling checks (`controls-boundary-guidance`,
`sequential-fake-guidance`) work the same way: a hand-curated list of required
(and forbidden) substrings, each surfaced as a `missing` term finding.

This single mechanism serves **two distinct purposes that are currently
conflated**:

1. **Currency anchoring** — proving that when the *source of truth* changes (a
   constitution rule, a routing capability, a skill contract), the *derived*
   guidance Markdown was regenerated/edited in lockstep and did not silently
   drift stale.
2. **Author guidance** — the actual prose a human or agent reads to understand
   how to do the work.

Because the same phrase does double duty, the prose **cannot shrink**: deleting
or rewording a sentence to make the guidance tighter trips a `missing term`
failure even when the guidance is *more* correct afterward. The 047 closeout
recorded this directly: the "Governance Markdown → low hundreds of lines" target
was **not met** (≈6,876–6,882 lines remain), and the after-baseline rationale
states the remaining prose "now doubles as pinned author-guidance that the
041–044 generation-currency term-checks depend on, so it cannot shrink freely
without decoupling guidance prose from the term anchors."

The goal number itself is also now known to have been anchored to a wrong
figure: the original "~23,000 lines / 21:1" estimate was an over-estimate, and
feature 046 established the corrected rule+guidance baseline at **≈6,882 lines**.
So an honest outcome is *either* genuine prose reduction once decoupled, *or* a
restated target measured against the corrected baseline — not a number chased
against a figure everyone agrees was wrong.

This feature separates the two concerns so currency can be proven without
freezing prose, and the prose-size goal can be made meaningful.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Tighten guidance prose without tripping the currency gate (Priority: P1)

A framework maintainer rewrites a verbose paragraph in
`.specify/templates/tasks-template.md` into a shorter, clearer one that still
conveys the same rule. The semantic obligation (e.g. "tasks carry a structured
`skillist`") is unchanged; only the wording and length changed.

**Independent test**: Edit a governed guidance file to shorten prose while
preserving its declared semantic obligations, run the governance currency gate,
and observe it passes — whereas under today's literal-substring table the same
edit fails with a `missing` term finding.

**Why this priority**: This is the core unlock. Without it, none of the
prose-reduction value is reachable; with it alone the feature already delivers.

### User Story 2 - Currency still catches genuine drift (Priority: P1)

A maintainer changes a *source of truth* (adds a constitution rule, a routing
capability, or a skill-contract obligation) but forgets to update the derived
guidance Markdown that should reflect it.

**Independent test**: Mutate a source-of-truth obligation without updating the
corresponding derived guidance and confirm the gate **fails** with an actionable
message naming the obligation and the file expected to reflect it. Conversely, a
synchronized change passes. This proves decoupling did not weaken drift
detection — it must remain at least as strong as the current term-anchor table.

**Why this priority**: Decoupling is only acceptable if it preserves the
protection the anchors provided. A weaker gate is a regression.

### User Story 3 - Honest, measurable prose-size accounting (Priority: P2)

A maintainer (or reviewer) wants to know how close the governance corpus is to
the size goal, measured against the corrected ≈6,882 baseline rather than the
discredited ~23,000 figure.

**Independent test**: Run the size-accounting step and read a report that states
the corrected baseline, the current measured guidance-prose line count, the
delta, and either the reduction achieved or the explicitly restated target with
its rationale.

**Why this priority**: Closes the §4.4 honesty gap. Valuable but dependent on
US1/US2 landing first; it reports on the decoupled world.

### Edge Cases

- A semantic obligation has **no** corresponding prose in any file → the gate
  must report the obligation as unreflected, not silently pass.
- Two files are expected to reflect the **same** obligation (e.g. a template and
  its `fsharp-opinionated` preset twin) → both must be checked; drift in one is
  still caught.
- A phrase that is a genuine machine-contract token (e.g. `[skillist: []]`,
  `[SEH]`, `synthetic-error-handling-approved`, a YAML key like `skillist:`) is
  consumed by tooling and is **not** free prose → it must remain literally
  pinned, distinct from human-readable guidance.
- The corpus is edited so that prose shrinks below the restated target →
  accounting reports success without requiring any phrase to be re-padded.
- A forbidden/stale term reappears in regenerated guidance → must still be
  rejected (the `forbidden`-list behavior is preserved).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The governance system MUST distinguish two categories of pinned
  string: **machine-contract tokens** (literal tokens consumed by tooling or
  parsers, which remain exactly pinned) and **author-guidance prose** (human-
  readable wording whose presence today is enforced only as a currency proxy).
- **FR-002**: For author-guidance prose, the currency check MUST be expressed in
  terms of the **semantic obligation** that the source of truth imposes, not the
  exact wording of the derived prose, so that rewording or shortening prose that
  still satisfies the obligation does not produce a failure.
- **FR-003**: The currency check MUST continue to **fail** when a source-of-truth
  obligation changes (or is added) and the derived guidance is not updated to
  reflect it, with detection at least as strong as the current term-anchor table
  for the obligations that table encodes.
- **FR-004**: Machine-contract tokens (e.g. `[skillist: []]`, `[SEH]`,
  `synthetic-error-handling-approved`, structured keys) MUST remain literally
  enforced and clearly identified as contract tokens, separate from prose
  obligations.
- **FR-005**: Failure diagnostics MUST name the affected file and the unmet
  obligation (or missing contract token) and remain actionable, preserving the
  current `task-skillist-guidance` / `controls-boundary-guidance` /
  `sequential-fake-guidance` finding-tag taxonomy or an equivalent.
- **FR-006**: The "forbidden / stale term" behavior (rejecting terms that must
  NOT appear in regenerated guidance) MUST be preserved.
- **FR-007**: The system MUST provide a prose-size accounting that reports the
  corrected ≈6,882-line baseline, the current measured guidance-prose line
  count, and the delta against the **restated** target (not the original
  ~23,000-line figure).
- **FR-008**: The size goal MUST be **restated** in the canonical baseline/goal
  record: either as a genuine reduction target measured against ≈6,882, or as a
  documented decision that the original "low hundreds" figure is retired with
  rationale — never left anchored to the over-estimate.
- **FR-009**: The decoupling MUST be applied to all existing term-anchor sites
  that mix prose and contract tokens (at minimum the `task-skillist-guidance`,
  `controls-boundary-guidance`, and `sequential-fake-guidance` checks), not just
  one, so no mixed-purpose anchor list remains as a hidden prose freeze.
- **FR-010**: All affected governance artifacts that are generated from a single
  source (e.g. `validation.contract.yml`, the `.claude` skill tree) MUST remain
  generated, not hand-synced, and their currency checks MUST stay green after
  the change.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identities, contents, versions, or generated
  consumers change. This is a governance-internal change to how
  `FS.Skia.UI.Build` enforces guidance currency. No Charts/DataGrid migration is
  involved.
- **Public contract impact**: No product `.fsi` signatures or surface baselines
  change. The "contract" affected is the governance currency contract: which
  obligations are required in guidance Markdown, and the
  `validation.contract.yml` generated from `Routing.fs`. Any change to the term
  taxonomy is reflected through single-source generation, not hand-edited.
- **State workflow impact**: No stateful product workflow, I/O, command, effect,
  subscription, or interpreter behavior changes.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering,
  screenshot, Vulkan, Skia, or visual-output behavior changes.
- **Evidence obligations**: Real evidence required under
  `specs/055-decouple-guidance-anchors/readiness/`: a before/after
  guidance-prose line-count report, a demonstration that prose rewording passes
  while source-of-truth drift fails (US1/US2 red→green), and the restated-goal
  record. Standard aggregate-hang-diagnostics and skill-loading-evidence apply
  per the escalated path.
- **Unsupported scope**: No new product features, no visual-parity work, no
  efficiency/timing instrumentation (that is §5.4), no Charts split (§5.2). This
  feature does not change what guidance *says* about how to build software — only
  how its *currency* is enforced and measured.
- **Build-target impact**: `GeneratedGuidanceCheck` is the primary affected gate
  (it hosts these guidance validators). `TargetMetadataDrift` /
  `validation.contract.yml` generation and `SkillSyncCheck` must stay current.
  Because the change touches governance paths and possibly public guidance
  templates, `Route` is expected to **escalate** to the maintainer-verify path
  (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `EvidenceGraph`, `EvidenceAudit`).

## Success Criteria *(mandatory)*

- **SC-001**: A prose-only edit that shortens or rewords governed guidance while
  preserving every declared semantic obligation passes `GeneratedGuidanceCheck`,
  where the equivalent edit fails today (demonstrated red→green).
- **SC-002**: Removing or altering a source-of-truth obligation without updating
  the derived guidance still fails `GeneratedGuidanceCheck` with a diagnostic
  naming the file and the unmet obligation — proving no loss of drift detection
  versus the current table.
- **SC-003**: 100% of existing mixed-purpose term-anchor sites
  (`task-skillist-guidance`, `controls-boundary-guidance`,
  `sequential-fake-guidance`) are migrated so that no list still freezes prose
  wording purely as a currency proxy.
- **SC-004**: Machine-contract tokens (e.g. `[skillist: []]`, `[SEH]`,
  `synthetic-error-handling-approved`, structured keys) remain enforced and are
  enumerated as contract tokens; a test removing any such token still fails.
- **SC-005**: A size-accounting report exists stating the corrected ≈6,882-line
  baseline, the current measured guidance-prose line count, and the delta against
  the restated target; the canonical baseline/goal record no longer cites the
  ~23,000-line figure as the live target.
- **SC-006**: The full escalated six-target order is green at the feature SHA,
  and `Route --enforce` reports all required evidence artifacts present.

## Assumptions

- The intent is to make prose *able* to shrink and to make the goal *honest*, not
  to mandate a specific final line count in this feature. If a concrete reduction
  target is desired, US3/FR-008 capture restating it against ≈6,882; the actual
  large-scale prose rewrite can be a follow-up once the freeze is lifted.
- "Semantic obligation" is represented in whatever way the governance library
  finds natural (a structured obligation list keyed per file, a small set of
  required-concept descriptors, etc.); the spec fixes the behavior, not the
  encoding. The existing single-source-generation discipline (rules live in
  `build/Governance/**`, artifacts generated not hand-synced) is retained.
- The corrected baseline figure to anchor against is the ≈6,882 lines that
  feature 046 established and the 047 after-baseline recorded; this feature does
  not re-derive it from scratch.
- Tokens already shaped as machine contracts (bracketed tags, YAML keys, exact
  field names) are the natural members of the "must stay literal" set; ambiguous
  cases are resolved toward "contract token" only when tooling actually parses
  them, otherwise treated as prose.

## Key Entities

- **Machine-contract token**: a literal string consumed by tooling/parsers
  (bracketed tags, structured keys, exact field names) that must appear verbatim.
- **Semantic obligation**: a rule imposed by a source of truth that some derived
  guidance file must reflect, checked by meaning/presence-of-concept rather than
  exact wording.
- **Source of truth**: constitution rules, routing capabilities, skill
  contracts — the canonical inputs the derived guidance Markdown must stay
  current with.
- **Derived guidance file**: the templates, presets, skill copies, and memory
  files (e.g. `tasks-template.md`, `constitution.md`, `speckit-tasks/SKILL.md`)
  that currently hold the pinned phrases.
- **Prose-size accounting**: the report measuring guidance-prose line count
  against the corrected ≈6,882 baseline and the restated target.
