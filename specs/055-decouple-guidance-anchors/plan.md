# Implementation Plan: Decouple Author-Guidance Prose from Generation-Currency Anchors

**Branch**: `055-decouple-guidance-anchors` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/055-decouple-guidance-anchors/spec.md`

## Summary

The three guidance currency-checks in `build/Governance/Guidance.fs`
(`validateTaskSkillistGuidance`, `validateControlsBoundaryGuidance`,
`validateSerializedRunnerGuidance`) today enforce currency by asserting that a
hand-curated list of **literal substrings** appears (or, for the forbidden list,
does not appear) in each governed Markdown file. That single mechanism conflates
two purposes — proving derived guidance stays current with its source of truth,
and freezing the exact prose a human reads — so prose cannot be shortened or
reworded without tripping a `missing term` finding even when the result is
*more* correct.

This change splits each mixed-purpose anchor list into two explicit categories:

1. **Machine-contract tokens** — literal strings consumed by tooling/parsers
   (`[skillist: []]`, `[SEH]`, `synthetic-error-handling-approved`, `skillist:`,
   `deps:`, `Control<'msg>`, package identifiers). These stay matched
   **verbatim**, exactly as today.
2. **Semantic obligations** — a rule a source of truth imposes that some derived
   guidance file must reflect. Each obligation carries a small set of *alternative
   concept anchors* and is satisfied when **any** anchor is present, so rewording
   or shortening the surrounding prose still passes as long as the concept
   survives — while deleting the concept entirely (the "forgot to update derived
   guidance" case) still fails.

The validators are refactored into a **pure core** (over an in-memory
`path → content` map) plus the existing thin IO wrapper, so the red→green
behavior (US1 passes / US2 fails) is provable by unit tests feeding synthetic-but-
realistic edits, with the real-repository scan (`runGeneratedGuidanceScan`)
unchanged as the gate entry point. A prose-size accounting step reports the
corrected ≈6,882-line baseline, the current measured count, and the restated
goal, and the canonical baseline record is updated so the discredited ~23,000 /
"low hundreds" figure is no longer cited as the live target.

This is a **Tier 2 (internal change)** to `FS.Skia.UI.Build` governance logic: no
public product `.fsi` surface, package identity, or runtime behavior changes. The
only `.fsi` surface that could change is the curated `Guidance.fsi`, whose single
gate entry point `runGeneratedGuidanceScan` is **unchanged**; the new
token/obligation types are internal helpers and need not be exported unless a
test references them (in which case they are added to `Guidance.fsi` deliberately
with a test).

## Technical Context

**Language/Version**: F# / .NET (`net10.0`), compiled `FS.Skia.UI.Build` governance library
**Primary Dependencies**: none new — Expecto + FsCheck (existing test stack), FAKE front-end (existing)
**Testing**: Expecto over the real repository guidance (`tests/Governance.Tests/`), plus new pure-core unit tests feeding reworded/drifted content
**Target Platform**: Windows and Linux (governance-internal; no platform-specific behavior)

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: No `.template.config/template.json` change. This is a
  governance-internal refactor of `FS.Skia.UI.Build` validators plus prose edits
  to already-governed guidance files; no template assets, samples, or
  command-surface are added or removed. The `dotnet new fs-skia-ui` template's
  contents are untouched. **N/A with rationale**: the change does not add or
  reclassify any template-owned asset.

- **Dependency impact**: None. No new NuGet package, no `Directory.Packages.props`
  entry, no `docs/dependencies.md` row, and no `DependencyReport` coverage change.
  The work uses only the existing Expecto/FsCheck test stack and standard library
  IO/globbing already present in the build front-end.

- **Command-surface impact**: No new FAKE target. The decoupling lives entirely
  inside the existing `GeneratedGuidanceCheck` gate (which already hosts these
  three validators via `runGeneratedGuidanceScan`). `TargetMetadataDrift` /
  `validation.contract.yml` and `SkillSyncCheck` must stay **green and current**:
  because `Routing.fs` rules are not edited, `validation.contract.yml` does not
  regenerate; because no `.agents`/`.claude` skill source is hand-synced, the
  `.claude` tree stays a current reproduction. FAKE-backed commands share `.fake`
  state and MUST run sequentially in the deterministic order below; only non-FAKE
  reads/tests may run in parallel:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  The authoritative gate list is whatever `./fake.sh build -t Route --enforce`
  prints for this diff; the change spans `.specify/**` (specify-catchall +
  generated-guidance), `docs/**` (docs-only), and `build/Governance/**` +
  `tests/**`, so `Route` is expected to **escalate**. We run exactly the gates
  `Route` prints, defaulting to the full serialized six-target order above when it
  escalates to the maintainer-verify path (per the spec's build-target impact).

- **Generated project impact**: None. Default/minimal generated contents, selected
  Controls guidance, generated local skills, validation logs, placeholder/excluded-
  history scans, and generated `Dev` behavior are all unchanged. The
  `controls-boundary-guidance` check still governs the same generated-product
  guidance files; its **forbidden** (stale-term) behavior is preserved verbatim so
  no removed-Charts language can re-enter generated guidance.

- **Evidence paths**: All real evidence lands under
  `specs/055-decouple-guidance-anchors/readiness/`:
  - `prose-size-accounting.md` — before/after governance-prose line counts vs the
    corrected ≈6,882 baseline, the measured delta, and the restated target
    (FR-007, FR-008, SC-005), with the exact `find … | wc -l` reproduction commands.
  - `decoupling-red-green.md` — the US1 (rewording passes) and US2 (source-of-truth
    drift fails) red→green transcript, captured from the new pure-core unit tests
    and a before/after of the literal-table behavior (SC-001, SC-002).
  - `contract-tokens.md` — the enumerated machine-contract-token set proving each
    token is still literally enforced (SC-004).
  - `evidence-policy-separation.md` — the specify-catchall / generated-guidance
    artifact required by `Route` for `.specify/**` edits.
  - `validation-contract.md` — the docs-only artifact, recording that
    `validation.contract.yml` and the `.claude` skill tree are unchanged/current.
  - `task-graph.md`, `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`,
    `focused-gates.md`, `generated-guidance.md` — standard escalated-path artifacts.
  - If `Route` escalates to maintainer-verify: `target-metadata.md` and
    `agent-ready-verdict.md`.
  The canonical goal record updated for FR-008 is
  `docs/reports/_baselines/2026-06-02-foundations-after.md` (row 5) and its peer
  `specs/047-foundations-programme-closeout/contracts/after-baseline.md`.

- **`.fsi` / contract impact**: No product-package `.fsi` or surface baseline
  changes (Tier 2). The only governance signature that may change is
  `build/Governance/Guidance.fsi`: its sole gate entry point
  `runGeneratedGuidanceScan` is **unchanged**. If the new
  `ContractToken` / `GuidanceObligation` types and the pure evaluator are exercised
  directly by unit tests, they are added to `Guidance.fsi` as a deliberate,
  test-backed surface addition (build-tooling scope, not a tracked product surface
  baseline). The governance **currency contract** affected — which obligations are
  required in which derived files — is documented in `contracts/` and remains
  single-sourced in `build/Governance/**`.

- **MVU/effect boundary**: N/A. This is a pure-validation refactor with no stateful
  workflow, no I/O orchestration beyond the existing read-file-and-collect-findings
  pattern, no retries, and no user interaction. The validators are pure functions
  from content to findings; the only I/O is reading governed files, already
  isolated in the front-end IO wrapper. No `Model`/`Msg`/`Effect`/`update`/
  interpreter boundary is introduced.

- **Synthetic evidence**: None expected. All currency-check evidence is **real**:
  the validators run over the actual repository guidance, and the red→green tests
  feed real governed-file content with realistic edits (a genuine shortened
  paragraph, a genuine removed concept), not canned fixtures standing in for an
  unavailable capability. No `[S]`/`[S*]`/`[SEH]` task is anticipated. If any test
  must use a constructed malformed input to exercise an error path, it will be
  disclosed per Principle V and routed through task review, not relabeled at
  implementation time.

- **Test evidence**: Failing-first semantic tests in `tests/Governance.Tests/`:
  (a) a US1 test asserting a reworded-but-concept-preserving edit **passes** the new
  evaluator where the old literal table failed (SC-001); (b) a US2 test asserting
  source-of-truth obligation drift **fails** with a diagnostic naming the file and
  unmet obligation (SC-002); (c) an SC-004 test asserting removal of any
  machine-contract token still fails; (d) an SC-006 regression that the real-
  repository scan still PASSes. Existing
  `GuidanceValidatorTests`, `ControlsBoundaryGeneratedGuidanceTests`, and
  `SequentialFakeGuidanceTests` are updated to the new model and must stay green.

- **Observability**: Failure diagnostics remain actionable and preserve the existing
  finding-tag taxonomy (`task-skillist-guidance`, `controls-boundary-guidance`,
  `sequential-fake-guidance`). Obligation findings name the file, the obligation id,
  and the source of truth (e.g. `"{file}: obligation 'skillist-structured' (constitution:Local Agent Skills) not reflected [task-skillist-guidance]"`);
  contract-token findings name the file and the missing/forbidden token. The
  `GeneratedGuidanceCheck` report is still written byte-deterministically to its
  output path and the gate still `failwithf`s with the collected findings on any
  failure (no silent pass). Prose-size accounting emits an explicit report rather
  than a silently truncated count.

- **Deferred scope**: The large-scale prose rewrite that actually shrinks the corpus
  toward any restated target is an explicit **bounded follow-up** — this feature
  lifts the freeze and makes the goal honest, it does not mandate a final line
  count (per the spec Assumptions). Out of scope and deferred: efficiency/timing
  instrumentation (§5.4), the Charts package split (§5.2), any new product feature,
  and any visual-parity work. No external-repository split or distribution
  automation is involved.

## Project Structure

```
build/Governance/
  Guidance.fs            # the three validators → split into contract-token + obligation model + pure core
  Guidance.fsi           # gate entry point unchanged; new types added only if test-referenced
  Findings.fs[i]         # ValidationFinding record — finding-tag taxonomy preserved (reused, not changed)
tests/Governance.Tests/
  GuidanceValidatorTests.fs              # real-repo PASS regression (SC-006) + US1/US2/SC-004 pure-core tests
  ControlsBoundaryGeneratedGuidanceTests.fs  # updated to token/obligation model; forbidden-term behavior retained
  SequentialFakeGuidanceTests.fs         # updated to the split model
.specify/                # governed guidance files (templates, presets, memory) — prose may be tightened here
  templates/…, presets/fsharp-opinionated/…, memory/constitution.md
docs/reports/_baselines/2026-06-02-foundations-after.md   # FR-008 restated-goal record (row 5)
specs/047-foundations-programme-closeout/contracts/after-baseline.md  # peer goal record
specs/055-decouple-guidance-anchors/
  research.md, data-model.md, quickstart.md, contracts/, readiness/
```

## Phase 0 — Research

See [research.md](./research.md): resolves how a "semantic obligation" is encoded
(any-of concept anchors), how drift detection stays at least as strong as the
literal table, which pinned strings are contract tokens vs prose, the prose-size
measurement methodology, and the single-source-generation invariants the change
must keep green.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md): `ContractToken`, `GuidanceObligation`, the pure
  evaluator signature, and the per-check obligation/token tables.
- [contracts/guidance-currency-contract.md](./contracts/guidance-currency-contract.md):
  the governance currency contract — required obligations per derived file, the
  contract-token set, finding-tag taxonomy, and the pass/fail rules.
- [quickstart.md](./quickstart.md): how to run the gate, the red→green demo, and
  the prose-size accounting step.
