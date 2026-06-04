# Data Model: Breakout-Demo Consumer Friction Follow-ups

**Feature**: `061-breakout-consumer-friction-followups`
**Date**: 2026-06-04

This feature changes governance contracts, skill prose, and authoring templates —
not a runtime domain. The "entities" below are the structured artifacts the FRs
constrain. Each lists fields, validation rules (traced to FRs/SCs), and any state
transition.

---

## E1 — Hook discovery set (FR-001, FR-002)

The merged set of hooks a phase skill considers at a phase boundary.

| Field | Type | Source |
|-------|------|--------|
| `extension` | string | hook entry |
| `command` | string | hook entry (e.g. `speckit.feedback.capture`) |
| `enabled` | bool (default true) | hook entry |
| `optional` | bool | hook entry |
| `condition` | string \| null | hook entry (not evaluated by the skill) |
| `prompt`, `description` | string | hook entry |
| `sourceFile` | path | `.specify/extensions.yml` **or** `.specify/extensions/<ext>/<ext>.yml` |

**Construction (was: single-file; now: multi-file).**
1. Read `.specify/extensions.yml` if present → `hooks.<before|after>_<phase>`.
2. Enumerate `.specify/extensions/*/*.yml`, parse each → same key.
3. Merge all entries; **dedup by `(extension, command)`** (first wins; identical
   re-declaration is not double-run).
4. Drop `enabled: false`. Do not evaluate `condition`.

**Validation rules.**
- VR-1 (FR-001): a hook present only in a per-extension file (the `feedback`
  extension) MUST appear in the merged set. *(SC-001)*
- VR-2 (edge): the same hook in both the central and a per-extension file appears
  **once**. *(no double-run)*
- VR-3 (FR-002): an `optional: true` hook that is discovered but not run SHOULD
  produce a one-line phase-end notice. Discovery is mandatory; execution stays
  optional.

---

## E2 — Feedback capture record (FR-003)

The per-phase record written to `specs/<feature>/feedback/<phase>-<date>.md`.

**Front matter.** `phase`, `date`, `severity` (`none|minor|major|blocker`).

**Sections (3 → 4 prompts).**

| # | Prompt (`{phase}` substituted) | Record section |
|---|--------------------------------|----------------|
| 1 | "…did anything go wrong or cause friction… what would have helped you?" | `## Process friction` |
| 2 | "…F# code… generalized into the support library? … skill family/topic + candidate helper…" | `## Generalizable code` |
| 3 | **NEW** "What additional or new skills would have been helpful during the *{phase}* phase? … topic + what the missing skill should cover, or 'none'." | **`## Skill gaps`** |
| 4 | "How blocking was the friction — none / minor / major / blocker?" | front-matter `severity` |
| — | (research links, when applicable) | `## Research links` |

**Validation rules.**
- VR-4 (FR-003/SC-002): the skill prompt set, the record-template example, AND
  the 058 sourcing contract all state **four** prompts; **no surviving "three
  prompts" reference** anywhere (058 spec/readiness/research/plan/tasks included).
- VR-5 (edge): prompt 3 answered "none" still writes a well-formed `## Skill
  gaps` section (parity with the existing "Generalizable code: none" path).
- VR-6 (D6 check): a gate asserts exactly four enumerated prompts and the
  presence of `## Skill gaps` in the generated feedback skill.

---

## E3 — Readiness-contract file requirement (FR-004, FR-005)

A single source-of-truth record (already F# data in
`build/Governance/Evidence/Scans.fs`) describing what each readiness file must
contain. This feature **surfaces** it, it does not redefine it.

| Field | Type | Notes |
|-------|------|-------|
| `fileName` | string | e.g. `governance-risk-levels.md` |
| `requiredTokens` | string list | the literal tokens that must appear |
| `requiredFields` | string list | for field-list files (e.g. `supported-host-persistent-launch.txt`) |
| `requiredTableHeader` | string option | for table files (e.g. skill-loading-evidence columns) |
| `requiredClasses` | string list | window-visibility: includes `product-defect` |
| `blocking` | bool | readiness-contract failures hard-block |

**Failing-file diagnostic (the FR-004 surface).** When a file is missing/partial,
the audit hit MUST print: `fileName` + the full `requiredTokens` (+ fields/table
header) — i.e. the complete expected shape, not just the first missing token or a
bare count. Sourced from the same `requiredTokens` list that enforces the rule
(`MissingTerms` already carries it).

**Validation rules.**
- VR-7 (FR-004/SC-003): every readiness file's name, tokens, fields, and table
  shape are recoverable from the audit output (and/or a shipped template)
  **without** decompiling `FS.Skia.UI.Build.dll` or copying a sibling project.
- VR-8 (FR-005/SC-004): the defect-class concept is required under **one**
  spelling — `product-defect` — across the readiness audit and any source
  governance scan (or the two are documented as deliberately distinct).

---

## E4 — `EvidenceGraph` / `EvidenceAudit` terminal verdict (FR-007)

The terminal line a clean run prints.

| Target | Current terminal token | Required |
|--------|------------------------|----------|
| `EvidenceGraph` | `verdict: ok` / inferred from exit 0 | explicit greppable `verdict=ok (no cycles, no dangling refs, no [S*])` (and `verdict=error (…)` on failure) |
| `EvidenceAudit` | `verdict=PASS` / `verdict=FAIL` | unchanged (already explicit); FR-004 enriches the per-file readiness diagnostics it prints |

- VR-9 (FR-007/SC-005): a clean `EvidenceGraph` prints a single unambiguous
  `verdict=…` token with the clean-pass reasons inline; pinned by a governance
  test.

---

## E5 — Authoring-template self-description (FR-006, FR-008, FR-009)

| Artifact | Required content |
|----------|------------------|
| Plan template *Repository Governance Decisions* block | inline comment stating `GeneratedGuidanceCheck` pass criteria (no empty/boilerplate/`NEEDS CLARIFICATION`/`TODO`; `N/A`-with-rationale = filled) — FR-008 |
| `speckit-tasks` SKILL.md | names exact preset paths `.specify/presets/fsharp-opinionated/templates/tasks-template.md` + `…/tasks-deps-template.yml` — FR-009 |
| Generic `.specify/templates/tasks-template.md` | one-line pointer "authoritative copy: preset path — edit there" — FR-009 |
| Generated quickstart/tasks build guidance | `Dev` = completion-marker/log target (`readiness/logs/Dev.txt`); `Test`/`Verify` (`dotnet test`) = authoritative compile/test path — FR-006 |

- VR-10 (SC-006): plan template carries the pass-criteria inline; `speckit-tasks`
  names the preset path; generic copy points to it.
- VR-11 (SC-005): generated quickstart states the `Dev`-vs-`Test`/`Verify`
  distinction.

**Authoritative-copy rule.** Preset copy is authoritative; generic copy carries
the pointer. Generation-owned blocks (constitution fragments) are edited via
`RefreshSurfaceBaselines`, not by hand.

---

## E6 — Duplicate-DU pitfalls note (FR-010)

Extension of the existing note in
`template/product-skills/fs-skia-keyboard-input/SKILL.md` "Common pitfalls".

| Example | Kind | Status |
|---------|------|--------|
| `ViewerKey.Unknown` vs `ViewerRunBlockedStage.Unknown` | framework-vs-framework | exists (060 FR-007) |
| `GameMode.Launch` vs `Msg.Launch` | **consumer-vs-consumer (co-opened modules)** | **add (FR-010)** |

- VR-12 (SC-007): the note covers the consumer-internal cross-module collision
  with the fully-qualified resolution.

---

## E7 — Arcade-helper triage record (FR-011)

One row per generalizable helper, recorded in this feature's readiness.

| Field | Type |
|-------|------|
| `helper` | string (fixed-step accumulator / collision-reflection / paddle-rebound / reserveHudBand) |
| `disposition` | `ship` \| `document` |
| `home` | skill id or package |
| `reference` | the canonical convention pointer / snippet |

**Decision (per D8):** all four `document`; homes = `fs-skia-elmish`
(game-loop/collision/rebound) and `fs-skia-layout-readability` (`reserveHudBand`).

- VR-13 (SC-008): each helper is either shipped with a skill reference or
  documented as the canonical convention, with the per-helper decision recorded.

---

## Cross-entity invariants

- INV-1 (FR-012): all skill edits land in canonical sources
  (`.agents/skills/**` or `template/feedback/skill/SKILL.md`) and regenerate
  `.claude/**`; `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck`
  stay green.
- INV-2 (SC-009): every Route-printed gate passes; `EvidenceAudit` returns
  `verdict=PASS` for `specs/061-breakout-consumer-friction-followups`.
- INV-3: no package *identity* changes; package *contents* change only via skill/
  template/quickstart edits and (if chosen) a same-source readiness template
  index — no new public `.fsi` surface unless a helper is later shipped (not in
  this feature, per D8).
