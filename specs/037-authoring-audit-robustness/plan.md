# Implementation Plan: Fail-Loud Authoring & Audit Robustness

**Branch**: `037-authoring-audit-robustness` | **Date**: 2026-05-30 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/037-authoring-audit-robustness/spec.md`

## Summary

Four real authoring failures shared one trait — they failed silently or
opaquely. This feature hardens each so the framework and the governance process
**fail loudly, point at the real cause, or refuse a misleading pass**:

1. **US1 (P1) — audit feature resolution.** `activeFeatureId` in `build.fsx`
   silently falls back to a hardcoded feature id (`"007-v2-template-packaging"`)
   whenever `.specify/feature.json` is missing or unreadable, so the
   `EvidenceGraph`/`EvidenceAudit` targets can audit the wrong feature and
   report a green pass. Make resolution authoritative from recorded state,
   hard-fail when it cannot resolve, and echo the resolved feature id + real
   task count so a wrong-feature mismatch is visible.
2. **US2 (P1) — prose-vs-violation parsing.** `run-audit.sh` mixes structured
   `key=value` parsing with bare substring scans (`"taskbar-only" in text`,
   `"mismatch" in text`, `"nu1603" in text`) over the *entire* file, including
   prose and negations. Restrict status reads to a designated structured region,
   drop the bare substring blockers, and document a deterministic resolution
   rule — without weakening true-positive blocking.
3. **US3 (P2) — Scene/Controls name collision.** Add
   `[<RequireQualifiedAccess>]` to `ControlEventOrigin` so its `Text` case stops
   leaking into the open namespace and shadowing the scene text construct.
   Update the `.fsi`, surface baselines, the spec 035 decision record, and add a
   previously-failing mixed-open fixture.
4. **US4 (P3) — FSI load entry point.** Emit a generated `.fsx` load script
   alongside the generated app that `#load`s the app plus its transitive
   references, kept in sync with the assembly set; document it and validate it
   in the generated-product checks while preserving benign host-warning
   classification.

**Change classification: Tier 1 (contracted change).** US3 modifies the public
`.fsi` surface of `FS.Skia.UI.Controls`, requiring `.fsi` + surface-baseline
updates and a recorded reversal of the spec 035 "guidance over attributes"
decision (FR-010). US1/US2/US4 are governance-tooling and template-generation
changes with no runtime contract impact.

## Technical Context

**Language/Version**: F# / .NET `net10.0`; governance tooling in Bash + Python 3
(`run-audit.sh`, `compute-task-graph.py`); build orchestration in FAKE
(`build.fsx`).
**Primary Dependencies**: No new packages. No package identity/version changes.
**Testing**: Expecto (`tests/Package.Tests`), FAKE targets (`Dev`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`), FSI transcripts, generated-product evidence,
plus governance fixtures under the feature readiness directory.
**Target Platform**: Windows and Linux (governance tooling runs on the Linux dev
container; FSI load script must tolerate headless hosts).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: **Changes required.** US4 adds a generated `.fsx` load
  script to the template output. The generation source lives under
  `template/base/` (emitted by `GenerateV3Products` in `build.fsx`), and
  `.template.config/template.json` must include the new file in generated
  content. US1/US2/US3 do not change the template package itself.
- **Dependency impact**: **None.** No `Directory.Packages.props`,
  `docs/dependencies.md`, or `DependencyReport` changes. No new dependency. The
  FSI load script references the *already-pinned* package/assembly set; it adds
  no new package.
- **Command-surface impact**: `EvidenceGraph` and `EvidenceAudit` behavior
  changes (feature resolution + parsing robustness). `GeneratedGuidanceCheck`,
  `TemplateCheck`, and `GeneratedProductCheck` change to emit/validate the FSI
  load script. `Dev` and `PackageSurfaceCheck` must continue to pass (US3
  baseline refresh). FAKE-backed targets run **sequentially** in the documented
  order — never concurrently (shared `.fake` state).
- **Generated project impact**: Generated products gain a `.fsx` load script and
  matching guidance in `README.md` / `docs/product.md`. Generated-product file
  lists and guidance scans must accept and require the new file. No change to
  selected Controls guidance beyond US3/US4 authoring notes.
- **Evidence paths**: see [Evidence Plan](#evidence-plan) — all under
  `specs/037-authoring-audit-robustness/readiness/`.
- **`.fsi` / contract impact**: **Yes (Tier 1).** `ControlEventOrigin` in
  `src/Controls/Types.fsi` gains `[<RequireQualifiedAccess>]`. Surface baselines
  `readiness/surface-baselines/FS.Skia.UI.Controls.txt` and
  `FS.Skia.UI.txt` are refreshed. The spec 035 decision record is updated with
  the reversal + rationale (FR-010). No other public surface changes. Scene DU
  constructors and the shared bounds record remain guidance-governed.
- **MVU/effect boundary**: **N/A.** No stateful or I/O-bearing runtime workflow
  changes (spec: "State workflow impact: None"). The governance tooling is a
  batch CLI, not an Elmish program; the FSI load script is static generated
  text.
- **Synthetic evidence**: The US2 false-positive/true-positive fixtures are
  *illustrative governance fixtures*, not synthetic product substitutes — they
  are the real input class the audit must parse. They are disclosed as fixtures
  (banner comment), but the audit runs against them for real, producing real
  verdicts, so no `[S]` product substitution is introduced. If any task lands on
  a stub before the real fixture exists, it is marked `[S]` until the real
  fixture and real audit run replace it. No `[SEH]` anticipated.
- **Test evidence**: failing-first where applicable — the US2 prose fixture must
  produce a *block* under today's substring scanner (demonstrating the bug)
  before the fix, then *pass* after; the genuine-violation fixture must block
  both before and after (no true-positive regression). US3 adds an FSI compile
  of the previously-failing open order. US1 adds a resolution test (real feature
  → real task count; unresolved → hard-fail).
- **Observability**: Audit must echo the resolved feature id, its real task
  count, and any recorded-vs-scanned mismatch; the unresolved-feature path emits
  a prominent non-suppressible warning and a non-zero exit (FR-002). The FSI
  load script path must keep benign host-warning classification intact (no
  silent suppression of real failures).
- **Deferred scope**: No broad audit-rule redesign beyond the parsing robustness
  here; no new rendering features; no release/distribution changes; no new
  platforms. The Scene DU + bounds record remedy stays guidance-only this
  feature (only `ControlEventOrigin` gets the attribute).

**Gate result: PASS.** The single contract change (US3) is explicitly chosen and
recorded per FR-010; all other work is tooling/template hardening with no
contract impact. No unjustified constitution violations.

## Project Structure

### Source / tooling touched

```
build.fsx                                      # activeFeatureId fail-loud (US1);
                                               #   FSI-load emission + guidance/
                                               #   product scan (US4)
.specify/feature.json                          # authoritative active-feature source (US1)
.specify/scripts/bash/common.sh                # get_feature_paths resolution parity (US1)
.specify/extensions/evidence/scripts/bash/run-audit.sh
                                               # structured-region status parsing,
                                               #   drop bare substring blockers (US2)
.specify/extensions/evidence/scripts/python/compute-task-graph.py
                                               # task-count echo / unresolved hard-fail (US1)

src/Controls/Types.fs                          # [<RequireQualifiedAccess>] ControlEventOrigin (US3)
src/Controls/Types.fsi                          # matching .fsi attribute (US3)
readiness/surface-baselines/FS.Skia.UI.Controls.txt   # refreshed baseline (US3)
readiness/surface-baselines/FS.Skia.UI.txt            # refreshed merged baseline (US3)
scripts/refresh-surface-baselines.fsx          # used to regenerate baselines (US3)

template/base/...                              # generated .fsx load script source (US4)
template/base/README.md, template/base/docs/product.md
                                               # FSI-load guidance (US4)
.template.config/template.json                 # include generated .fsx in output (US4)

.claude/skills/speckit-evidence-graph|audit/SKILL.md  # documented resolution rule (US1/US2)
.agents/skills/speckit-evidence-graph|audit/SKILL.md  # synchronized Codex peers
specs/035-api-discovery-names/readiness/name-collision-safety.md
                                               # recorded reversal + rationale (FR-010)
```

### Evidence / fixtures (new)

```
specs/037-authoring-audit-robustness/
├── plan.md, research.md, data-model.md, quickstart.md
├── contracts/
│   ├── audit-status-region-contract.md        # the authoritative-region format + rule (US2)
│   ├── control-event-origin-contract.md        # .fsi surface delta + baseline (US3)
│   └── fsi-load-script-contract.md             # generated .fsx shape + sync rule (US4)
└── readiness/
    ├── logs/                                    # evidence-graph.txt, evidence-audit.txt
    ├── audit-fixtures/
    │   ├── prose-negation-clean.md              # blocker terms only in prose/negation (US2)
    │   └── genuine-violation.md                 # real violating status (US2 true-positive)
    ├── feature-resolution.md                    # resolved id + real task count (US1)
    ├── fsi/                                      # mixed-open compile + FSI load transcript
    └── fsi-load-script.md                        # generated .fsx load evidence (US4)
```

## Phase 0: Research

See [research.md](./research.md). Decisions resolved:

- **R1 — authoritative active-feature source (US1).** Treat
  `.specify/feature.json` `feature_directory` as the single authoritative
  source. Remove the hardcoded `"007-v2-template-packaging"` fallback in
  `activeFeatureId`; on missing/unreadable/empty, hard-fail with a prominent
  message. Keep `common.sh get_feature_paths` resolution order consistent
  (env override → feature.json → branch-prefix) but make the "no real feature"
  terminal state a failure, not a stub.
- **R2 — structured status region format (US2).** A fenced code block whose info
  string declares the audit status language (e.g. ```` ```audit-status ````) is
  the only authoritative region; `key=value` lines are read **only** inside it.
  Resolution rule: first declared region wins; duplicate keys within it are a
  parse error (not last-wins); prose/markdown bullets are never read. Bare
  substring blockers (`taskbar-only`, `mismatch`, `nu1603` `in text`) are
  removed in favor of structured fields with explicit violating values.
- **R3 — RequireQualifiedAccess blast radius (US3).** Confirmed only
  `ControlEventOrigin` is affected; its sibling DUs already carry the attribute.
  Audit existing repo usages of unqualified `Pointer/Keyboard/Text/Focus/...`
  origin cases and qualify them. Scene `Text` constructor and `LayoutBounds`
  remain guidance-governed.
- **R4 — FSI load script generation strategy (US4).** Emit a static `.fsx` at
  generation time from the pinned package set in `Directory.Packages.props` +
  the generated `Product` output assembly, so the script stays in sync with the
  app's assembly set without being a hand-maintained list. Headless/benign host
  warnings stay classified per the spec 021 host-warning contract.

## Phase 1: Design & Contracts

- **Data model**: [data-model.md](./data-model.md) — entities: Active-Feature
  Resolution, Audit Status Region, Status Key/Value, Blocker Condition,
  Generated FSI Load Script, Surface-Baseline Delta.
- **Contracts**: under [contracts/](./contracts/):
  - `audit-status-region-contract.md` — the authoritative-region grammar,
    deterministic resolution rule, and the violating-value set the audit blocks
    on (replaces substring scanning). (FR-004, FR-005, FR-006)
  - `control-event-origin-contract.md` — the `.fsi` delta, baseline lines, and
    the spec 035 reversal rationale. (FR-007, FR-010)
  - `fsi-load-script-contract.md` — generated `.fsx` filename, `#load`/`#r`
    shape, in-sync derivation, and the benign-warning preservation rule.
    (FR-009)
- **Agent context update**: `AGENTS.md` SPECKIT block repointed to this plan.

## Evidence Plan

All paths under `specs/037-authoring-audit-robustness/readiness/`:

| Obligation | Evidence |
|---|---|
| US1 correct resolution + true task count | `feature-resolution.md`, `logs/evidence-audit.txt` showing resolved id and real task count |
| US1 unresolved hard-fail | transcript of a run with no resolvable feature → non-zero exit + warning |
| US2 no false block | `audit-fixtures/prose-negation-clean.md` audited → PASS |
| US2 sustained true block | `audit-fixtures/genuine-violation.md` audited → BLOCK |
| US3 mixed-open compile | `fsi/` compile of previously-failing open order |
| US3 surface delta | refreshed `readiness/surface-baselines/FS.Skia.UI.Controls.txt` (+ merged) |
| US4 FSI load | `fsi-load-script.md` + real FSI load transcript for a generated app |

## Constitution Re-Check (post-design)

No new violations introduced by the design. The Tier 1 contract change is
isolated to `ControlEventOrigin`, recorded, and baseline-tracked. Governance and
template changes are observable and fail-loud. **PASS.**

## Validation Order (FAKE-backed, sequential)

Per repository constraints, run sequentially — never concurrently:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `PackageSurfaceCheck` for the US3 baseline refresh. If any failure looks
race-like, rerun the affected FAKE-backed commands sequentially before product
debugging.

## Complexity Tracking

No constitution deviations requiring justification. The one contract change is
the minimal targeted remedy chosen in Clarifications; the alternative
(guidance-only, per spec 035) was tried and proved insufficient — the reversal
is documented per FR-010.
