# Feature Specification: Governance Gate Hardening

**Feature Branch**: `087-governance-gate-hardening`
**Created**: 2026-06-09
**Status**: Draft
**Input**: User description: "create specs to address the problems encountered with the governance process in the implementation phase you collected earlier."

## Context

This feature hardens the FS.Skia.UI governance/evidence process against six concrete
weaknesses observed end-to-end during the feature-086 implementation + merge. None blocked
086, but together they created one genuine near-miss (a generated-product regression a gate
shape-masked) and recurring friction that erodes trust in the gates. The goal is to make the
gates **trustworthy signals** — a gate that always fails locally, or a "green" that is
unreachable, trains operators to ignore the gate, which is the actual hazard.

### Observed problems (evidence from 086)

1. **`GeneratedProductCheck` always fails locally** at the generated `Verify` step:
   *"Cannot resolve the feature to validate: no SPECKIT_FEATURE_DIR override is set and
   …/app-source/.specify/feature.json has no usable feature_directory entry."* The gate's
   authoritative build+test step passed (29/29) but the overall target red-lit every run and
   had to be hand-classified "non-authoritative." A perpetually-failing gate is dismissed by
   reflex — and the 086 regression surfaced at the *Build* step only by luck; a defect in the
   `Verify` step would have hidden behind the known env-failure.
2. **Local-vs-pinned package skew** — the near-miss. `TemplateCheck` builds the generated
   product against the **locally-packed (unreleased)** package version; `GeneratedProductCheck`
   restores the **pinned/published** version. A generated test using a new-but-unpublished API
   (`ControlRenderResult.Bounds`) **compiled under `TemplateCheck` and failed under
   `GeneratedProductCheck`**. Nothing flagged the skew; it was found only by running the full
   escalated order.
3. **Per-package surface baselines drift on regeneration** — `RefreshSurfaceBaselines` does
   **not** regenerate `readiness/per-package-surface/*.fsi.txt`, and naive regeneration injected
   spurious trailing-newline diffs across unchanged packages, forcing a revert + hand-fix.
4. **`--accept-synthetic` never changes the verdict** — by design the merge audit stays
   `NEEDS-EVIDENCE` whenever any `[S]`/`[S*]` exists, so a clean "PASS" is **unreachable** for any
   feature with a legitimately-deferred artifact. The real gate decision moves entirely to human
   override, and "audit failed" stops meaning anything actionable.
5. **`[S*]` over-propagation via phase-checkpoint edges** — three deferred keystroke tasks
   mechanically contaminated eight unrelated tasks (including the Phase-9 gate tasks) purely
   because every Phase N+1 task gets an auto-injected edge to Phase N's last task. The
   contamination is structural, not semantic.
6. **Skill-loading evidence is weak / late** — the contract enforces only the *form* (one row
   per task/skill, `loaded_at < work_started_at`) and surfaces only when a task flips `[X]`, so
   rows are back-filled with plausible-but-unverifiable hand-authored timestamps. It is closer to
   bookkeeping theater than verifiable evidence.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Generated-product gate gives a trustworthy local verdict (Priority: P1)

As a framework maintainer running the escalated validation order, I run
`./fake.sh build -t GeneratedProductCheck` on a clean tree and get a **green** result when the
generated products genuinely build and test, and a **red** result only for a real product
defect — never a perpetual failure I must hand-wave.

**Independent test**: On a clean checkout, run `GeneratedProductCheck`. It returns success when
the generated products build + test cleanly; introducing a real compile/test defect in the
generated source turns it red with a product-defect (not environment) classification.

### User Story 2 - Pinned-package skew is caught before merge (Priority: P1)

As a maintainer adding new framework surface in a feature branch, I am told **before merge** if
the generated template's source/tests use any framework API that is **not present in the
package version the generated product pins** — so a generated project can never ship that
compiles only against the unreleased local pack.

**Independent test**: Add a generated-template test that references a framework symbol absent
from the pinned package version. A governance check fails naming the symbol, the file, and the
pinned-vs-local version gap — without needing to run the slow full restore.

### User Story 3 - Surface-baseline refresh is complete and idempotent (Priority: P2)

As a maintainer who changed a public `.fsi`, I run one refresh command and **all** surface
baselines (cross-package, api-surface tree, skill tree, **and** per-package) are regenerated;
re-running it on an unchanged tree produces **zero** diff (no trailing-newline churn).

**Independent test**: After an additive `.fsi` change, run the refresh once → only the changed
package's per-package baseline diffs (additive). Run it again with no source change → `git
status` is clean.

### User Story 4 - The merge audit can express an accepted-deferral PASS (Priority: P2)

As a maintainer merging a feature with a legitimately-deferred artifact, the audit reports a
distinct, durable **"PASS with accepted deferrals"** state (recorded with written justification)
that is clearly separated from **unaccepted** synthetic/blocking findings — so "audit clean"
remains meaningful and the human override is recorded as data, not just a flag in a log.

**Independent test**: Run the audit on a feature whose only synthetic findings are accepted
(with justification) and whose diff-scan is clean → the audit's machine-readable verdict
distinguishes this from a feature with unaccepted synthetic findings.

### User Story 5 - Synthetic propagation reflects real dependencies (Priority: P3)

As a maintainer reading the task graph, a deferred `[S]` task only propagates `[S*]` to tasks
that **actually depend on its output**, not to every later-phase task via an auto-injected
phase-checkpoint edge — so the propagated view names real contamination, not bookkeeping order.

**Independent test**: Mark a leaf task `[S]` whose output nothing consumes; confirm no unrelated
later-phase task is recomputed `[S*]` solely through a phase-checkpoint edge.

### User Story 6 - Skill-loading evidence is captured, not hand-authored (Priority: P3)

As a maintainer, the skill-loading evidence rows are produced from an observable signal of skill
loads (not retroactively typed), and any gap is surfaced **when the skill should have loaded**,
not only when a task later flips `[X]`.

**Independent test**: Complete a task that declares a skill without loading it; the gap is
reported at implementation time (not deferred to the `[X]` flip), and authored rows carry a
provenance marker distinguishing captured from manually-asserted load times.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `GeneratedProductCheck` MUST provide each generated product a **resolvable feature
  context** (e.g., a generated `.specify/feature.json` with a usable `feature_directory`, or a
  scoped `SPECKIT_FEATURE_DIR`) so the generated `Verify` step can run, and MUST return a
  **green** verdict when every generated product builds, tests, and verifies cleanly on a clean
  local tree.
- **FR-002**: When a generated-product step fails, the gate MUST classify the failure as either
  **product-defect** (authoritative, blocking) or **environment** (non-authoritative) using a
  structured signal, and MUST NOT let an environment classification suppress or hide a
  product-defect failure occurring in the same run (each step classified independently).
- **FR-003**: The governance process MUST detect, **before merge**, when the generated template's
  product source or tests reference any `FS.Skia.UI.*` public API that is **absent from the
  package version the generated `Directory.Packages.props` pins** (the local-vs-pinned skew),
  naming the symbol, the file, and the pinned version. The check MUST NOT require a full network
  restore to produce its verdict.
- **FR-004**: The two generated-product validation paths MUST make their **package source
  explicit** in their reports — `TemplateCheck` (local-packed/unreleased) vs
  `GeneratedProductCheck` (pinned/published) — so an operator can see which package set produced
  a given pass/fail and cannot mistake one for the other.
- **FR-005**: `RefreshSurfaceBaselines` (or a single documented refresh entry point) MUST
  regenerate **per-package** surface baselines (`readiness/per-package-surface/*.fsi.txt`) in
  addition to the cross-package, api-surface, and skill baselines it already covers.
- **FR-006**: Surface-baseline regeneration MUST be **idempotent**: re-running it on a tree with
  no source change MUST produce **zero** byte diff in any baseline file (no trailing-newline or
  whitespace churn), so a baseline diff always signals a real surface change.
- **FR-007**: The merge audit MUST expose a machine-readable verdict that **distinguishes** three
  states: (a) clean PASS (no synthetic, no blocking hits); (b) **PASS-with-accepted-deferrals**
  (the only findings are synthetic deferrals each carrying recorded written justification, and
  diff-scan/contract hits are zero); and (c) FAIL (unaccepted synthetic, or any blocking
  diff-scan/contract/window-visibility/persistent-launch hit).
- **FR-008**: An accepted deferral MUST be recorded as **durable structured data** (justification,
  task id, real-evidence path, the host capability it awaits) — not solely as a logged CLI flag —
  and the audit MUST surface unaccepted vs accepted synthetic counts separately.
- **FR-009**: Synthetic (`[S*]`) propagation MUST follow **real data dependencies** declared in
  `tasks.deps.yml`, not auto-injected phase-checkpoint ordering edges; a phase-checkpoint edge
  alone MUST NOT mark a downstream task `[S*]`.
- **FR-010**: Skill-loading evidence rows MUST carry a **provenance field** distinguishing a
  **captured** load (observed during the run) from a **manually-asserted** one, and the
  missing-skill-load condition MUST be surfaced **at the point the declaring task is
  implemented**, not deferred until the task flips `[X]`.
- **FR-011**: All changes MUST preserve every existing **true-positive** gate behavior — the
  diff-scan, surface-baseline additive enforcement, window-visibility/persistent-launch contracts,
  and synthetic-honesty disclosures MUST keep blocking on real violations (no relaxation of a
  genuine block to make a gate "pass").

> Interacting / conflicting requirements: FR-001 (make `GeneratedProductCheck` go green locally)
> vs FR-011 (never relax a genuine block) — resolve as: the gate goes green only because the
> *environment* obstacle (unresolvable feature) is removed, never by downgrading a product-defect
> to non-blocking. FR-007's PASS-with-accepted-deferrals vs FR-011 — an accepted deferral is a
> distinct PASS *category*, never a way to pass an *unaccepted* synthetic or a blocking hit.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identity* change. The governance engine
  `FS.Skia.UI.Build` (`build/Governance/**`) changes (gate logic, audit verdict model, baseline
  refresh, skew detection). `FS.Skia.UI.Build` is packable and bumps/packs with the libs per the
  established merge flow. No `Directory.Packages.props` consumer-pin change beyond what the
  template normally carries; no new NuGet dependency.
- **Public contract impact**: No `src/**/*.fsi` public-surface change expected. The "contract"
  here is governance-internal: `Routing.fs`-derived `validation.contract.yml`, target metadata,
  the audit's machine-readable verdict schema, and `docs/evidence-formats.md` (the
  skill-loading-evidence provenance field + the accepted-deferral record shape) — all generated
  from their single sources and currency-checked by `TargetMetadataDrift`/`SkillSyncCheck`.
- **State workflow impact**: The evidence engine's audit/graph computation changes (verdict
  states, propagation rule). These are pure functions over `tasks.md` / `tasks.deps.yml` /
  `readiness/`; no new host I/O or effect algebra. Synthetic-propagation and verdict logic stay
  pure and property-testable.
- **Layout/rendering impact**: None. No Scene/Controls/Layout/SkiaViewer rendering, Vulkan, Skia,
  or screenshot behavior changes.
- **Evidence obligations**: `readiness/` artifacts proving each fix — a `GeneratedProductCheck`
  green run log on a clean tree (FR-001/002); a skew-detection check that fails on a seeded
  unpinned-API reference and passes on the real tree (FR-003/004); a no-op
  `RefreshSurfaceBaselines` rerun showing zero git diff (FR-005/006); an audit run showing the
  three distinct verdicts on seeded inputs (FR-007/008); a task-graph showing no
  phase-edge-only `[S*]` propagation (FR-009); a skill-loading-evidence file with the provenance
  field + an at-implementation-time gap report (FR-010); and the existing gate suite still
  blocking on seeded real violations (FR-011). Governance.Tests cover the pure engine changes.
- **Unsupported scope**: No change to product/runtime behavior, no new platforms, no
  distribution/release automation, no Spec Kit *workflow command* surface (the `/speckit-*` skill
  text) beyond what FR-010's earlier-gap-surfacing requires. Not retrofitting historical features'
  readiness. The keyboard-injection harness itself (the 086 deferral) is out of scope here — this
  feature fixes the *process*, not that artifact.
- **Build-target impact**: `GeneratedProductCheck` (FR-001/002/004), `TemplateCheck` (FR-004),
  `EvidenceAudit` (FR-007/008/009), `EvidenceGraph` (FR-009), `RefreshSurfaceBaselines`
  (FR-005/006) change. `Route` may re-route as governance paths change. `Dev`/`Verify`/`Ci`
  unaffected except via the shared engine build.

## Success Criteria *(mandatory)*

- **SC-001**: On a clean local checkout, `GeneratedProductCheck` returns a success verdict for a
  product set that genuinely builds and tests, with **zero** hand-classified "non-authoritative"
  failures required.
- **SC-002**: Seeding a real compile or test defect into a generated product turns
  `GeneratedProductCheck` red with a **product-defect** classification, even when an environment
  obstacle is also present in the same run.
- **SC-003**: A generated-template source/test reference to a framework API absent from the pinned
  package version is reported as a blocking finding (naming symbol + file + version gap) **before
  merge**, and the real tree reports zero such findings — produced without a full network restore.
- **SC-004**: Every generated-product validation report states which package set (local-packed vs
  pinned) it used; an operator can determine the source of any pass/fail from the report alone.
- **SC-005**: One refresh command regenerates per-package baselines along with the others; after
  it, only genuinely-changed surfaces differ.
- **SC-006**: Running the refresh twice in a row on an unchanged tree leaves `git status` clean
  (zero spurious diffs).
- **SC-007**: The audit's machine-readable output distinguishes clean-PASS,
  PASS-with-accepted-deferrals, and FAIL on three seeded inputs, and the accepted-deferral
  justification is recoverable as structured data.
- **SC-008**: A leaf `[S]` task whose output nothing consumes propagates `[S*]` to **zero**
  unrelated later-phase tasks.
- **SC-009**: Skill-loading-evidence rows record load provenance (captured vs asserted), and a
  declared-but-unloaded skill is reported at the task's implementation point, before any `[X]`
  flip.
- **SC-010**: The existing true-positive gates (diff-scan, additive surface enforcement,
  window-visibility/persistent-launch, synthetic-honesty) still block on seeded real violations —
  no genuine block was relaxed to obtain a green.

## Assumptions

- The "user" of this feature is the framework maintainer / agent operating the governed workflow;
  outcomes are gate behaviors, not end-product runtime behaviors.
- FR-001 is satisfied by giving the generated product a resolvable feature context (preferred:
  the generated tree carries a usable `feature.json`, or the gate sets a scoped
  `SPECKIT_FEATURE_DIR`); splitting the authoritative build/test step from the env-dependent
  `Verify` step is an acceptable alternative if feature resolution proves infeasible.
- FR-003's skew check compares the generated source's referenced public API against the pinned
  package version's known public surface (already captured as surface baselines), so it can run
  statically without a network restore.
- FR-007's "accepted deferral" remains synthetic (the artifact is still deferred); the new state
  changes only how the verdict is *reported/categorized*, never whether the deferred evidence is
  real. `--accept-synthetic` continuing to require written justification is retained.
- FR-009 changes only synthetic *propagation* over the graph; auto-injected phase-checkpoint edges
  may still exist for ordering/visualization but no longer carry synthetic contamination.
- Single repo, single trunk (`main`); no change to the merge/version-bump flow itself.
