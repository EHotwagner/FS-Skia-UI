# Implementation Plan: Big Rewrite of the Governance Markdown Corpus

**Branch**: `056-rewrite-governance-mds` | **Date**: 2026-06-03 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/056-rewrite-governance-mds/spec.md`

## Summary

Feature 055 lifted the prose freeze: literal-substring term tables became
`GuidanceObligation` (semantic, presence-of-concept) + `ContractToken` (verbatim)
values over a pure `evaluateGuidanceCheck`. This follow-up spends that unlock — a
substantive, maximizing prose rewrite of the full canonical governance corpus
(`.agents/skills/**/*.md` ≈4,072 lines and `.specify/**/*.md` ≈2,817 lines; sum
≈6,889 against the corrected ≈6,882 baseline) to be as tight and clear as
possible while losing **no** semantic obligation, **no** machine-contract token,
and **no** currency-check strength.

Technical approach: this is a **prose-only, governance-documentation** change. No
F# product code, no `.fsi` surface, and no governance *rule* changes. The rewrite
edits canonical sources only (`.agents`, `.specify`); `.claude` is regenerated
from `.agents` via `RefreshSurfaceBaselines`. The authority on what may not be cut
is the existing `Guidance.fs` check inventory — `taskSkillistGuidanceCheck`,
`controlsBoundaryGuidanceCheck`, `serializedRunnerObligation`, and the forbidden
lists. Every `ContractToken.Token` survives verbatim in its `Files`; every
`GuidanceObligation`'s concept anchors survive (rephrased prose still matches);
every forbidden term stays absent. Verification leans entirely on existing gates
(`GeneratedGuidanceCheck`, `SkillSyncCheck`, `TargetMetadataDrift`,
`TemplateCheck`) plus three readiness artifacts proving preservation, drift
detection, and the size delta.

## Technical Context

**Language/Version**: F# / .NET (governance tooling unchanged; no product code in scope)
**Primary Dependencies**: None new. Consumes existing `FS.Skia.UI.Build` governance library (`build/Governance/Guidance.fs`).
**Testing**: Existing Expecto governance tests + FAKE gates (`GeneratedGuidanceCheck`, `SkillSyncCheck`, `TargetMetadataDrift`, `TemplateCheck`); plus a deliberate token-removal / obligation-mutation negative check recorded as readiness evidence.
**Target Platform**: Windows and Linux (governance gates are platform-neutral file scans).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **Tier 2 (internal change)** under the constitution's Change
Classification: it alters no public API surface, introduces no dependency, and
changes no observable product behavior. It touches governance *documentation*
that derived-guidance currency checks read, so it escalates on `Route` for
verification, but it is not a contracted (Tier 1) change. No `.fsi` or
surface-area baseline is touched (Principle II unaffected). No stateful/I/O
workflow is introduced (Principle IV N/A). No synthetic evidence is used
(Principle V N/A — all evidence is real gate output over real files).

### Repository Governance Decisions

- **Template ownership**: `.template.config/template.json` is **not** touched.
  No source, sample, test, or command-surface change occurs; only governance
  *prose* under `.agents`/`.specify` is rewritten. `template/**` product-facing
  docs are out of focus but may be tightened opportunistically only when doing so
  changes no rule and trips no gate (spec Assumptions). The `.specify`
  template/preset *twins* (`constitution-template.md` ×2, `tasks-template.md` ×2,
  `plan-template.md` ×2, `spec-template.md` ×2, command/deps copies) are rewritten
  in lockstep: twins identical today stay identical; any intentional divergence
  must still satisfy both files' obligations (FR-007).

- **Dependency impact**: None. `Directory.Packages.props`, `docs/dependencies.md`,
  generated template inclusion, and `DependencyReport` are unaffected. No package
  identity, content, version, or pin changes — this is documentation prose, not a
  packable-project change, so no version bump is implied by the rewrite itself.

- **Command-surface impact**: No `build.fsx`/`scripts/build/**` change; no new or
  modified FAKE target. The rewrite is *verified by* existing targets, it does not
  add any. `RefreshSurfaceBaselines` is **run** (to regenerate `.claude` from
  `.agents`) but its definition is unchanged. FAKE-backed commands share `.fake`
  state and MUST run sequentially in the deterministic escalated order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  Run `./fake.sh build -t Route --enforce` first; run exactly the gates it prints.

- **Generated project impact**: None to generated *behavior*. Generated `Dev`
  behavior, placeholder/excluded-history scans, and selected-Controls guidance are
  unchanged. The only generated artifacts that move are regenerated-not-edited:
  the `.claude` skill tree (from `.agents`, via `RefreshSurfaceBaselines`;
  `SkillSyncCheck`-enforced) and `validation.contract.yml` (from `Routing.fs`,
  which is not edited, so it stays byte-identical; `TargetMetadataDrift` stays
  green).

- **Evidence paths**: Real evidence under `specs/056-rewrite-governance-mds/readiness/`:
  - `prose-size-accounting.md` — baseline ≈6,882, measured post-rewrite
    `.agents/skills` and `.specify` line counts, summed current, signed delta;
    reproduced from the two `find … | xargs wc -l | tail -1` commands
    (`renderProseSizeAccounting` format). (US3, FR-009, SC-001/SC-007)
  - `contract-tokens.md` — enumeration of every preserved `ContractToken` and
    `GuidanceObligation` with a present/absent confirmation per home file post-rewrite. (US2, FR-002/FR-003, SC-002)
  - `rewrite-red-green.md` — the negative proof: a recorded source-of-truth
    obligation mutation (and a token removal) that still **fails**
    `GeneratedGuidanceCheck` with a file+obligation diagnostic, then reverted. (US2, FR-002, SC-003/SC-005)
  - `generated-guidance.md`, `skill-sync-check.md`, `template-drift.md` /
    `template.*`, `validation-contract.md` — green gate transcripts. (SC-004/SC-008)
  - Standard escalated-path artifacts: `aggregate-hang-diagnostics.md`,
    `skill-loading-evidence.md`, `governance-risk-levels.md`,
    `runtime-limitations.md`, `evidence-graph.md`, `evidence-audit.md`.
  Repo-root `readiness/` artifacts that `Route --enforce` resolves
  (`target-metadata.md`, `agent-ready-verdict.md`, `validation-contract.md`,
  `evidence-policy-separation.md`) already exist from prior closeouts; refresh only
  if the escalated tier names a missing one.

- **`.fsi` / contract impact**: No product `.fsi` signature, public doc, surface
  baseline, or sample contract changes. The *governance currency contract* (the
  055 obligation + token sets in `Guidance.fs`/`Guidance.fsi`) is **preserved
  exactly, not altered** — if any obligation or token must change to make the
  prose pass, the rewrite went too far and that edit is out of scope.
  `validation.contract.yml` stays generated from `Routing.fs`.

- **MVU/effect boundary**: N/A. No stateful or I/O-bearing product workflow is
  added or changed; no `Model`/`Msg`/`Effect`/`init`/`update`/interpreter is in
  scope. The work is static prose editing verified by pure file-scan gates.

- **Synthetic evidence**: None. All evidence is real: actual gate runs over the
  actual rewritten files, an actual mutation that actually fails the actual gate.
  No mocks, fakes, fixtures, or in-memory substitutes; no `[S]`/`[SEH]` task is
  anticipated. If any verification step turns out to require a synthetic stand-in,
  Principle V disclosure applies and it is surfaced rather than hidden.

- **Test evidence**: The existing governance test suite and gates are the test
  evidence — they pass on the rewritten corpus (green) and the recorded mutation
  proves they still fail on obligation loss (red→green). No assertion is weakened
  to green a build; any rule that cannot be tightened without breaking a gate is
  left intact. The negative check is failing-first by construction (mutate →
  observe failure → revert → observe pass).

- **Observability**: The size-accounting report is the actionable diagnostic
  (baseline, measured, delta, reproduction commands — byte-deterministic). Gate
  failures already name the file and unmet obligation
  (`{file}: obligation '{id}' ({source}) not reflected [{tag}]`); the rewrite
  preserves that diagnostic surface and the readiness transcripts capture it.
  Missing-artifact-class failures remain enforced by `EvidenceAudit`'s readiness
  contract.

- **Deferred scope**: No product features, visual-parity work, efficiency/timing
  instrumentation, or Charts split. The 055 *currency model itself* is not
  re-derived or amended (retiring a redundant obligation would be a model change —
  out of scope). The ≈6,882 baseline is consumed, not recomputed. No fixed final
  line count is targeted; the achieved number is reported. Root docs
  (`CLAUDE.md`, `AGENTS.md`) and `template/**` are out of focus and only
  opportunistically tightened.

**Post-design re-check**: Phase 1 introduces no data model beyond the existing
`Guidance.fs` types, no new contract beyond the preserved currency contract, and
no `.fsi` change — the Constitution Check above stands unchanged after design.

## Project Structure

Canonical edit surface (in scope):

```
.agents/skills/**/*.md          # skill tree (~4,072 lines) — canonical source
.specify/
  memory/constitution.md        # (currency source; rewrite prose, keep every rule/token)
  templates/*.md                # spec/plan/tasks/constitution templates
  presets/fsharp-opinionated/   # template/command/deps twins
    templates/*.md
    commands/*.md
  extensions/**/*.md            # git/evidence extension docs
```

Regenerated, never hand-edited:

```
.claude/skills/**/*.md          # generated from .agents via RefreshSurfaceBaselines (SkillSyncCheck)
validation.contract.yml         # generated from Routing.fs (TargetMetadataDrift) — Routing.fs NOT edited
```

Authority / verification (read-only in scope):

```
build/Governance/Guidance.fs    # ContractToken / GuidanceObligation / forbidden inventory — the cut-authority
build/Governance/Guidance.fsi   # exported types/values (unchanged)
```

Evidence:

```
specs/056-rewrite-governance-mds/
  readiness/                     # prose-size-accounting, contract-tokens, rewrite-red-green, gate transcripts
  research.md  data-model.md  quickstart.md  contracts/
```
