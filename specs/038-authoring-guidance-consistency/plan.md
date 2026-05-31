# Implementation Plan: Authoring Guidance Consistency

**Branch**: `038-authoring-guidance-consistency` | **Date**: 2026-05-30 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/038-authoring-guidance-consistency/spec.md`

## Summary

An author building a real app on FS.Skia.UI hit a cluster of *authoring-surface*
friction points — almost none were runtime defects. This feature hardens the
guidance and public surface so an author who follows the project's own hints,
skills, and API — **without reflecting DLLs or reading scattered design
reports** — succeeds on the first try. The generated consumer project has
absolute priority (SC-001); framework-repo-process items (FR-001, FR-011) are
P3 and must never block consumer-facing work.

Work, in priority order:

1. **US1 (P1) — every advertised skill id resolves.** A repeatable guard fails
   when any skill id advertised in the task-generation hints / scan phrases does
   not resolve to a declared skill `name:`, when a skill's directory / `name:` /
   advertised id disagree, or when the `.agents/` and `.claude/` peers drift. The
   one live dangling id today — `speckit-debug-loop`
   (`.agents/skills/speckit-tasks/SKILL.md:145,149` + `.claude/` mirror) — is
   removed/repointed so the guard passes. (FR-001/002/003, SC-007)
2. **US2 (P1) — read the API without reflecting DLLs.** A freshly generated
   project bundles the real public `.fsi` signatures for the packages its profile
   consumes (derived verbatim from `capabilities.yml` `contracts:` per capability)
   as its local authoritative API reference, so an author can read any union
   case's exact field order locally. (FR-004, SC-002)
3. **US3 (P2) — names don't collide on `open`.** `[<RequireQualifiedAccess>]` on
   `ViewerWindowStartupState` (its bare `Normal` collides) and on the enumerated
   viewer/input `update`/`init`-bearing surfaces so a consumer can define their
   own `Normal`/`update`/`init` after `open`. Breaking is acceptable: migration
   note, version bump, all generated samples updated. (FR-008, SC-003)
4. **US4 (P2) — consumer-facing, domain-agnostic generated guidance.** Generated
   starter tests and starter app carry no demo-specific (game-title) identifiers
   (`Tetris`, `Score`, `Level`, `next piece`, `board`); generated skills carry at
   least one consumer-runnable usage snippet and no references to framework-only
   paths/targets. (FR-005/006/007, SC-004)
5. **US5 (P3) — canonical effects page.** One page names both effect categories
   (application commands at the MVU edge vs viewer effects at the host boundary),
   the boundary, and the `update`→host wiring — bundled into the generated
   project so it is reachable without scattered reports or source. (FR-009, SC-005)
6. **US6 (P3) — consistent scene constructors.** Add additive, self-describing
   constructors/helpers for `Rectangle`/`PaintedRectangle`/`Text` so the
   tuple-arity footgun is removed, with the existing positional constructors
   retained. (FR-010, SC-006)
7. **FR-001 / FR-011 (P3, framework-repo dev-process).** The skill-resolution
   guard (FR-001) and an evidence-gate regression guard asserting the gates still
   target `.specify/feature.json` and do not fire on incidental filename mentions
   (FR-011, behavior established by feature 037). Strictly after all consumer
   work; never blocking it.

**Change classification: Tier 1 (contracted change).** FR-008 modifies the
public `.fsi` surface (`[<RequireQualifiedAccess>]`) — a recorded, possibly
breaking change requiring `.fsi` + surface-baseline updates, a migration note,
and a version bump. FR-010 adds public scene constructors additively (`.fsi` +
baseline updates, no removals). FR-004 bundles real `.fsi` signatures into
generated output. The rest is guidance/template/governance-tooling hardening
with no runtime behavior change.

## Technical Context

**Language/Version**: F# / .NET `net10.0`; governance tooling in FAKE
(`build.fsx`) plus Bash/Python evidence scripts.
**Primary Dependencies**: No new packages; no package *identity* change. Package
*versions* bump on merge (FR-008/FR-010 touch `src/` public surface).
**Testing**: Expecto (`tests/*`), FAKE targets (`Dev`, `GeneratedGuidanceCheck`,
`TemplateCheck`, `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`,
`PackageSurfaceCheck`), FSI transcripts, generated-product evidence, governance
fixtures under the feature readiness directory.
**Target Platform**: Windows and Linux (governance tooling and generated-product
checks run on the Linux dev container; FAKE-backed targets run **sequentially**).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: **Changes required.** FR-004 bundles per-profile `.fsi`
  references into generated output; FR-006 adds consumer-runnable snippets to
  generated skills; FR-007 neutralizes demo identifiers in the generated starter
  app + tests; FR-009 bundles the canonical effects page into generated `docs/`.
  Generation source lives under `template/base/` and `template/fragments/`,
  emitted by `runGenerateV3Products`/`generateV3Product` in `build.fsx`;
  `.template.config/template.json` must include any new generated files.
- **Dependency impact**: **None.** No `Directory.Packages.props` (root),
  `docs/dependencies.md`, or `DependencyReport` changes. No new dependency. The
  bundled `.fsi` references the *already-pinned* package surface; they add no
  package. Generated `template/base/Directory.Packages.props` version pins follow
  the normal merge-time bump, not this feature's design.
- **Command-surface impact**: `GeneratedGuidanceCheck` and `TemplateCheck` change
  to assert skill-id resolution (FR-001/002/003), the bundled API reference
  (FR-004), consumer-runnable snippets (FR-006), domain-agnostic starter tests
  (FR-007), and the reachable effects page (FR-009). `GeneratedProductCheck`
  file-lists accept/require the new generated artifacts. `Dev` and
  `PackageSurfaceCheck` change with the `.fsi` updates (FR-008/FR-010).
  `EvidenceGraph`/`EvidenceAudit` gain/keep the feature.json-targeting regression
  guard (FR-011). FAKE-backed targets run **sequentially** in the documented
  order — never concurrently (shared `.fake` state).
- **Generated project impact**: Generated products gain (a) bundled `.fsi`
  signatures for their profile's capabilities as the local API reference, (b) a
  canonical effects page under `docs/`, (c) consumer-runnable skill snippets, and
  (d) domain-agnostic starter app + tests. Selected Controls guidance is
  unchanged beyond the snippet/path corrections.
- **Evidence paths**: see [Evidence Plan](#evidence-plan) — all under
  `specs/038-authoring-guidance-consistency/readiness/`.
- **`.fsi` / contract impact**: **Yes (Tier 1).** FR-008 adds
  `[<RequireQualifiedAccess>]` to `ViewerWindowStartupState`
  (`src/SkiaViewer/SkiaViewer.fsi:43-48`) and to the enumerated viewer/input
  `update`/`init`-bearing surfaces (final set fixed in research R3). FR-010 adds
  self-describing scene constructors/helpers to the `Scene` module
  (`src/Scene/Scene.fsi`) additively. Surface baselines
  `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`,
  `FS.Skia.UI.Scene.txt`, the merged `FS.Skia.UI.txt`, and any affected input
  baseline are refreshed. A migration note is recorded (FR-008).
- **MVU/effect boundary**: **No behavior change.** FR-009 *documents* the existing
  boundary (`Model`/`Msg`/`Effect`/`init`/pure `update`/edge interpreter already
  exist in `src/Elmish` and `src/SkiaViewer`); it adds no stateful workflow,
  command, effect, subscription, or interpreter behavior. The RQA hardening
  (FR-008) is a naming/visibility change, not a transition change.
- **Synthetic evidence**: The US1 dangling-id and drift fixtures and the FR-011
  filename-mention fixture are *illustrative governance fixtures* — the real
  input class the guards must classify — not synthetic product substitutes. The
  guards run against them for real and produce real verdicts; no `[S]` product
  substitution is introduced. If any task lands on a stub before its real fixture
  / real generated-project run exists, it is marked `[S]` until replaced. No
  `[SEH]` anticipated.
- **Test evidence**: failing-first throughout — introduce a deliberately dangling
  / drifted id and confirm the resolution guard *fails*, then passes on the
  corrected repo (SC-007); a mixed-`open` consumer fixture defining its own
  `Normal`/`update`/`init` must fail to compile (collision) before FR-008 and
  compile after (SC-003); a generated-project scan must find demo identifiers /
  framework-only paths before FR-005/007 and none after (SC-004); the effects
  page must be absent-from-generated-project before FR-009 and present after
  (SC-005); both new and existing scene constructors compile after FR-010
  (SC-006); an FR-011 filename-mention fixture must not trigger required evidence.
- **Observability**: The resolution guard names each unresolved/drifted id, the
  file:line that advertises it, and the `.agents`/`.claude` peer disagreement;
  the API-reference check names any bundled-signature gap; the FR-011 guard echoes
  the resolved feature id and why a filename mention did/didn't trigger. No silent
  pass: a missing artifact class is a loud failure.
- **Deferred scope**: Out of scope — mouse/pointer host input; a headless/software
  raster backend; dotnet fsi window/font usability; **removing or breaking any
  existing public scene constructor** (FR-010 is additive only). No audit-rule
  redesign beyond the FR-011 regression guard; no Charts package migration change.

**Gate result: PASS.** The contract changes (FR-008 RQA, FR-010 additive
constructors, FR-004 bundled signatures) are explicitly chosen, recorded, and
baseline-tracked per the spec's Clarifications; all other work is
guidance/template/governance hardening with observable, fail-loud behavior. No
unjustified constitution violations.

## Project Structure

### Source / contracts touched

```
src/SkiaViewer/SkiaViewer.fs / .fsi        # [<RequireQualifiedAccess>] ViewerWindowStartupState
                                           #   + enumerated update/init surfaces (US3, FR-008)
src/KeyboardInput/KeyboardInput.fs / .fsi  # input update/init surface RQA if in collision set (US3)
src/Elmish/Elmish.fs / .fsi                # ElmishAdapter init/update — confirm/qualify (US3)
src/Scene/Scene.fs / .fsi                  # additive self-describing constructors/helpers (US6, FR-010)
readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt
readiness/surface-baselines/FS.Skia.UI.Scene.txt
readiness/surface-baselines/FS.Skia.UI.txt            # merged (US3/US6)
readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt   # if input surface qualified (US3)
readiness/surface-baselines/FS.Skia.UI.Elmish.txt          # if Elmish update/init surface qualified (US3)
scripts/refresh-surface-baselines.fsx                 # regenerate baselines
```

### Template / generated-project surface touched

```
template/base/src/Product/Model.fs, View.fs, EvidenceCommands.fs, LayoutEvidence.fs
                                           # neutralize demo identifiers (US4, FR-007)
template/base/tests/Product.Tests/Tests.fs # domain-agnostic starter tests (US4, FR-007)
template/fragments/*/skill/SKILL.md +
  template/fragments/*/README.md           # consumer-runnable snippet, no framework-only paths (US4)
template/base/docs/effects-boundary.md     # NEW canonical effects page, bundled (US5, FR-009)
template/base/docs/api-surface/            # NEW bundled .fsi reference per profile (US2, FR-004)
.template.config/template.json             # include new generated files (US2/US5)
```

### Governance tooling touched

```
build.fsx                                  # GeneratedGuidanceCheck/TemplateCheck:
                                           #   skill-id resolution guard (US1, FR-001/002/003);
                                           #   bundled-.fsi emission + check (US2, FR-004);
                                           #   snippet + no-framework-path scan (US4, FR-005/006);
                                           #   demo-identifier scan (US4, FR-007);
                                           #   effects-page reachability (US5, FR-009);
                                           #   feature.json-targeting regression guard (FR-011)
.agents/skills/speckit-tasks/SKILL.md      # remove/repoint speckit-debug-loop (US1, FR-001)
.claude/skills/speckit-tasks/SKILL.md      # synchronized peer
docs/reports/generated-apps.md             # repointed/aligned with the new canonical effects page
```

### Spec artifacts (this feature)

```
specs/038-authoring-guidance-consistency/
├── plan.md, research.md, data-model.md, quickstart.md
├── contracts/
│   ├── skill-resolution-contract.md        # id↔name↔dir agreement + peer sync rule (US1)
│   ├── generated-api-reference-contract.md  # bundled-.fsi shape + per-profile derivation (US2)
│   ├── name-collision-hardening-contract.md # RQA delta + baseline + migration note (US3)
│   ├── generated-guidance-contract.md       # consumer-facing/domain-agnostic rules (US4)
│   ├── effects-boundary-contract.md         # canonical page contents + wiring (US5)
│   └── scene-constructor-contract.md        # additive self-describing constructors (US6)
└── readiness/
    ├── logs/                                # evidence-graph.txt, evidence-audit.txt, dev/check logs
    ├── skill-resolution.md                  # guard output: pass on corrected repo (US1)
    ├── skill-resolution-fixtures/           # dangling + drifted + peer-mismatch fixtures (US1)
    ├── generated-api-reference.md           # bundled .fsi present + union-case shape read locally (US2)
    ├── name-collision-migration.md          # migration note + version bump record (US3)
    ├── fsi/                                  # mixed-open compile (before fail / after pass) (US3, US6)
    ├── generated-guidance.md                # no demo ids / no framework-only paths / snippet present (US4)
    ├── effects-boundary.md                   # single page reachable from generated project (US5)
    └── feature-targeting-regression.md      # filename-mention fixture does not trigger (FR-011)
```

## Phase 0: Research

See [research.md](./research.md). Decisions resolved:

- **R1 — skill-id resolution model (US1).** Build the advertised-id set from the
  hint/scan-phrase lines in `speckit-tasks/SKILL.md` and the harness
  "available skills" surface; resolve each against the declared `name:` of every
  skill under `src/*/skill`, `.agents/skills/*`, `.claude/skills/*`, and
  `template/fragments/*/skill`. Fail on any unresolved id, on any
  directory/`name:`/advertised-id disagreement, and on any `.agents`↔`.claude`
  peer drift. Remove the `speckit-debug-loop` reference (no such skill exists);
  there is no debug-loop skill to repoint to.
- **R2 — local API-reference form (US2).** Bundle the real public `.fsi` files
  verbatim, selected per generated profile from `capabilities.yml` `contracts:`
  for each capability the profile includes, into a generated `docs/api-surface/`
  tree. Derivation is mechanical (copy-from-source at generation time), never
  hand-maintained, so signatures stay in lockstep. The check fails if a consumed
  package's signatures are missing or drift from source.
- **R3 — name-collision blast radius (US3).** Confirmed: `ViewerWindowStartupState`
  has a bare `Normal` and needs `[<RequireQualifiedAccess>]`. Enumerate the
  `update`/`init`-bearing surfaces a consumer could `open` into collision
  (`Viewer`, `ElmishAdapter`, input modules) and decide per surface whether it is
  already module-qualified or needs RQA / rename guidance. Apply the hardening
  consistently across the public surface, refresh baselines, update all generated
  samples, and record the migration note + version bump.
- **R4 — additive scene constructors (US6).** Add self-describing helpers
  consistent with `rectangleWithPaint`/`PaintedRectangle` — a `Rect`-based and/or
  named-argument constructor for `Rectangle` and `Text` so an arity slip is
  prevented or yields a clear error — without removing the existing positional DU
  cases or the `Scene.rectangle`/`Scene.text` helpers. Decide helper-functions vs
  named-field DU additions; both are additive.
- **R5 — demo-identifier neutralization (US4).** The `app`-profile starter is a
  Tetris-style demo (`Score`/`Level`/board). Replace game-title-specific
  identifiers (`Tetris`, `board`, `piece`, `score`, `level`) in the generated
  starter app + tests with domain-agnostic equivalents, preserving the generic
  game-starter shape (HUD region, gameplay region, primary-interaction counter)
  so `fs-skia-layout-evidence` stays meaningful. Define the forbidden-identifier
  scan list.
- **R6 — canonical effects page + FR-011 guard.** Promote the scattered effects
  content (`docs/reports/generated-apps.md`, `runtime-design.md`) into one
  canonical page covering both effect categories, the boundary, and the
  `update`→host wiring, bundled into the generated project's `docs/`. FR-011: add
  a regression guard asserting the gates resolve via `.specify/feature.json` and
  do not fire required evidence from a bare filename mention in `tasks.md`
  (behavior shipped in 037).

## Phase 1: Design & Contracts

- **Data model**: [data-model.md](./data-model.md) — entities: Skill Identity
  (dir / `name:` / advertised id), Advertised-Id Set, Skill Peer Pair, Bundled
  API Reference, Collision-Prone Public Name, Scene Constructor Variant, Canonical
  Effects Page, Feature-Targeting Guard.
- **Contracts**: under [contracts/](./contracts/) — one per user story (listed in
  Project Structure above), each naming the exact rule, the failing-first fixture,
  and the FR/SC it satisfies.
- **Agent context update**: `AGENTS.md` SPECKIT block repointed to this plan.

## Evidence Plan

All paths under `specs/038-authoring-guidance-consistency/readiness/`:

| Obligation | Evidence |
|---|---|
| US1 ids resolve; guard fails on dangling/drift | `skill-resolution.md`; `skill-resolution-fixtures/` audited → FAIL; corrected repo → PASS (SC-007) |
| US1 `.agents`↔`.claude` peers agree | peer-comparison output in `skill-resolution.md` (FR-003) |
| US2 local API reference present + reflection-free | `generated-api-reference.md` showing bundled `.fsi` and a union-case field order read locally (SC-002) |
| US3 no collision after `open` | `fsi/` compile of a consumer defining `Normal`/`update`/`init` — FAIL before, PASS after (SC-003) |
| US3 surface delta + migration | refreshed baselines; `name-collision-migration.md` (note + version bump) |
| US4 domain-agnostic + consumer-facing | `generated-guidance.md`: zero demo ids, zero framework-only paths, ≥1 runnable snippet (SC-004) |
| US5 single reachable effects page | `effects-boundary.md`: one page in the generated project covering both categories + wiring (SC-005) |
| US6 both constructor forms compile | `fsi/` compile of new + existing scene constructors (SC-006) |
| FR-011 targeting regression | `feature-targeting-regression.md`: feature.json target echoed; filename mention does not trigger (SC-008) |
| SC-001 governing | generated project builds, tests, and produces evidence using only local references — `logs/` |

## Constitution Re-Check (post-design)

No new violations introduced by the design. The Tier 1 contract changes (FR-008
RQA, FR-010 additive constructors) are explicitly chosen, recorded, and
baseline-tracked; FR-004 bundles real signatures with mechanical derivation (no
hand-maintained drift). The effects work is documentation only; no MVU behavior
changes. Governance and template changes are observable and fail-loud. The
consumer-priority ordering (SC-001 governing; FR-001/FR-011 last and
non-blocking) is preserved. **PASS.**

## Validation Order (FAKE-backed, sequential)

Per repository constraints, run sequentially — never concurrently:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Plus `PackageSurfaceCheck` for the FR-008/FR-010 baseline refresh. If any failure
looks race-like, rerun the affected FAKE-backed commands sequentially before
product debugging.

## Complexity Tracking

No constitution deviations requiring justification. The contract changes are the
minimal targeted remedies named in the spec's Clarifications: FR-008 is the
breaking-but-accepted RQA hardening (migration note + version bump per
Clarifications), FR-010 is additive-only, and FR-004 reuses the existing
`capabilities.yml` contract map rather than introducing a new signature-derivation
mechanism.
