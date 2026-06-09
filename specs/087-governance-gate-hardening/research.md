# Phase 0 Research: Governance Gate Hardening

All six items are decisions about the compiled engine `FS.Skia.UI.Build`
(`build/Governance/**`). Grounded in the current code, cited inline.

## R1 — FR-001: a resolvable feature context for `GeneratedProductCheck`

- **Decision**: Provision the generated product tree with a usable
  `.specify/feature.json` carrying a `feature_directory`, so the generated
  `Verify` step resolves a feature instead of hard-failing. Keep the
  step-split as the documented fallback only.
- **Rationale**: The failure originates in `Engine/Model.fs:24` `activeFeatureId`,
  which hard-fails (`"Cannot resolve the active feature… refuses to fall back to
  a placeholder feature"`) when `.specify/feature.json` is missing or has no
  `feature_directory`. The generated product tree currently ships without a
  resolvable feature, so the env hard-fail fires every run and the whole target
  is hand-classified "non-authoritative" — masking a real `Verify`-step defect
  (the 086 near-miss was caught at *Build* only by luck). Giving the generated
  tree a real `feature_directory` (a minimal seeded feature, or a gate-scoped
  `SPECKIT_FEATURE_DIR` env var pointing at one) removes the *environment*
  obstacle without weakening any block — exactly the FR-001 vs FR-011 resolution
  the spec mandates (green only because the env obstacle is removed, never by
  downgrading a product defect).
- **Alternatives considered**: (a) Permanent hand-classification — rejected: it
  is the hazard itself (a perpetual red trains operators to ignore the gate).
  (b) Step-split (run authoritative build/test, skip env-`Verify`) — kept as
  documented fallback per spec Assumptions if a resolvable context proves
  infeasible, but it leaves `Verify` permanently unexercised, so it is second
  choice.
- **Open for Phase 1**: whether the seeded feature lives in `template/base`
  (ships into every generated tree) or is injected by the gate. Prefer the gate
  scoping `SPECKIT_FEATURE_DIR` to avoid shipping spec-kit feature state into
  consumer products.

## R2 — FR-002: independent per-step product-defect vs environment classification

- **Decision**: Classify each generated-product step (Build, Test, Verify)
  independently with a structured signal; an environment classification on one
  step never suppresses a product-defect on another in the same run.
- **Rationale**: Today the target red-lights as a whole and the operator
  hand-waves it. The engine already distinguishes effects per step in
  `Engine/Update.fs` (`GenerateV3Products`, `ScanV3GeneratedProducts`,
  `ValidateGeneratedConsumer`); the fix is to attach a `{ step; classification }`
  result to each and let the overall verdict be the *max severity over
  product-defect steps*, with environment-classified steps reported but
  non-authoritative. This is a pure aggregation change over per-step results.
- **Alternatives considered**: single overall classification (status quo) —
  rejected, it is the masking bug.

## R3 — FR-003/004: static pinned-vs-local package-skew detection

- **Decision**: A static check that compares the **public-API symbols referenced
  by the generated template's source/tests** against the **public surface
  captured for the pinned package version** (the existing surface baselines),
  and fails — naming symbol + file + pinned-vs-local version gap — when a
  referenced symbol is absent from the pinned surface. Each generated-product
  report states its package set explicitly (local-packed vs pinned).
- **Rationale**: The 086 near-miss: `TemplateCheck` builds against the
  locally-packed (unreleased) package (`TemplatePack` → `.nupkg` in
  `model.TemplateArtifactDir`, consumed via `template/base/Directory.Packages.props`),
  while `GeneratedProductCheck` restores the pinned/published version. A generated
  test referencing a new-but-unpublished symbol (`ControlRenderResult.Bounds`)
  compiled under one and failed under the other. The surface baselines already
  capture the framework's public API per package (`readiness/api-surface/`,
  per-package `*.fsi.txt`), so the comparison is **static** — no network restore
  needed (spec Assumption). The pinned version is in
  `template/base/Directory.Packages.props`; the local-packed version is the bump
  target. Symbol references in generated source are extracted by the same
  surface-capture machinery used elsewhere.
- **Alternatives considered**: (a) Run the full pinned restore on every branch —
  rejected: slow, network-dependent, and only catches it after the expensive
  restore. (b) Diff the two package versions' surfaces alone — insufficient: it
  flags *every* new symbol, not just ones the generated product actually
  references. The chosen check is referenced-symbol ∩ (local − pinned).

## R4 — FR-007/008: three-state audit verdict + durable accepted-deferral record

- **Decision**: Replace the binary `Audit.verdict` (`Pass`/`Fail`, `Audit.fs:402`)
  with three states: `Pass` (no synthetic, no blocking), `PassWithAcceptedDeferrals`
  (the only findings are synthetic deferrals each carrying recorded written
  justification, and all diff-scan/contract/window/persistent hits are zero), and
  `Fail` (any unaccepted synthetic, or any blocking hit). Record each accepted
  deferral as durable structured data (justification, task id, real-evidence
  path, awaited host capability) in `readiness/synthetic-evidence.json`, and
  surface unaccepted-vs-accepted synthetic counts separately in
  `seh-audit-summary.json`.
- **Rationale**: Today a clean PASS is *unreachable* for any feature with a
  legitimately-deferred artifact — the audit stays blocking whenever any
  `[S]`/`[S*]` exists, so "audit failed" stops being actionable and the decision
  moves entirely to human override. A distinct, durable PASS-with-accepted-
  deferrals category keeps "audit clean" meaningful and records the override as
  data. The existing `[SEH]`/`accepted-seh` mechanism already proves the pattern
  (an accepted annotation lets the audit pass); FR-007 generalizes it to a
  reported *verdict state* with a structured deferral record. `--accept-synthetic`
  is **retained** with its written-justification requirement (it is not yet wired
  per the exploration; this feature implements it consistently with the new
  state). Crucially (FR-011): the new state can never pass an *unaccepted*
  synthetic or a blocking hit.
- **Alternatives considered**: keep binary + rely on human override log —
  rejected, that is the status quo the spec calls out as meaningless.

## R5 — FR-009: `[S*]` propagation over real data dependencies only

- **Decision**: Compute taint propagation over `ExplicitDeps` only, not over
  `allDeps` (`ExplicitDeps @ PhaseDeps`). Retain `PhaseDeps` (auto-injected
  phase-checkpoint edges) for ordering, toposort, and cycle detection — they just
  no longer carry synthetic contamination.
- **Rationale**: `Graph.fs:33` `allDeps t = t.ExplicitDeps @ t.PhaseDeps`, and
  `Graph.propagate` (`Graph.fs:128`) filters taint over `allDeps`. So every
  Phase N+1 task — which `TaskParser.fs:344` auto-injects an edge to Phase N's
  last task — inherits `[S*]` from any deferred Phase-N task. In 086 three
  deferred keystroke tasks contaminated eight unrelated tasks (incl. the Phase-9
  gate tasks) purely structurally. Splitting the taint source (`ExplicitDeps`)
  from the ordering source (`allDeps`) makes propagation name *real* contamination.
  Cycle detection and toposort (`Graph.fs:58,85`) keep using `allDeps` so
  ordering correctness is unchanged.
- **Alternatives considered**: stop injecting phase edges entirely — rejected:
  they are still useful for ordering/visualization (spec Assumption keeps them);
  only their taint-carrying is the defect.

## R6 — FR-010: captured-vs-asserted skill-loading provenance + at-implementation gap

- **Decision**: Add a `provenance` field to each skill-loading-evidence row
  marking a **captured** load (observed during the run) vs a **manually-asserted**
  one, and surface a missing/declared-but-unloaded skill **at the point the
  declaring task is implemented**, not deferred to the `[X]` flip.
- **Rationale**: `Audit.fs:228` `validateSkillLoadingEvidence` enforces only the
  *form* (8-column row, `loaded_at < work_started_at`, ISO-8601) and only when a
  task flips `[X]` — so rows are back-filled with plausible-but-unverifiable
  hand-authored timestamps (bookkeeping theater). The schema lives in
  `Evidence/EvidenceFormatSchema.fs` (single source) and is documented in
  `docs/evidence-formats.md`. Adding a `provenance` column (`captured` |
  `asserted`) makes the honesty visible; surfacing the gap at implementation time
  (when the declaring task's `skillist` should have been loaded) catches the miss
  early. The "observable signal of skill loads" is the implementer recording the
  load before code changes (Constitution §IV implementation gate) — `captured`
  marks rows tied to that recorded action; `asserted` marks hand-authored ones.
- **Alternatives considered**: a fully automated load-capture harness — out of
  scope here (the spec asks only for a provenance *marker* + earlier surfacing,
  not a new capture runtime).

## Cross-cutting — FR-011: preserve every true-positive block

- All six changes are additive or scope-narrowing in the *taint/verdict* sense
  only. The diff-scan, additive surface-baseline enforcement, window-visibility /
  persistent-launch contracts, and synthetic-honesty disclosures keep blocking on
  real violations. Verified by a dedicated `true-positive-gates-still-block`
  evidence run (SC-010) that seeds a real violation of each and confirms it still
  blocks. This is the invariant every task is checked against.
