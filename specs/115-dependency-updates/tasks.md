# Tasks: Dependency Updates ("update all if possible")

**Feature branch**: `115-dependency-updates`
**Spec**: `specs/115-dependency-updates/spec.md`
**Plan**: `specs/115-dependency-updates/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]`
or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the
evidence audit. See `readiness/task-graph.md` for the propagated view.

This feature plans **zero** synthetic evidence (Principle V): every gate runs
against the real build, real packed libraries, and real generated template. The
Synthetic-Evidence Inventory below is intentionally empty and `[SEH]` is not
used — there is no error-path task with infeasible real input here.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase). FAKE-backed
  targets are never `[P]` against each other — they share `.fake` state and run
  sequentially in the deterministic order.
- **[US1]**, **[US2]**, **[US3]** — user-story scope
- **[T2]** — Tier 2 (internal) change. This feature is Tier 2 for the safe
  product bumps; the spec-kit `.specify/**` asset bump escalates `Route` to the
  consumer-contract/governance gate set (no `.fsi` delta).

## Governance risk levels

- **Small**: a single safe patch/minor pin edit (FSharp.Core, FileSystemGlobbing)
  — focused validation is `Dev`; no broad rerun.
- **Medium**: the spec-kit `.specify/**` asset bump — focused validation adds
  `GeneratedGuidanceCheck` / `TemplateCheck` because it touches a
  consumer-contract path; broad rerun only on a gate-reported drift.
- **Broad**: any held major bump experiment (US2) — requires the full escalated
  serialized FAKE gate set before an adopt decision; broad validation is
  mandatory here because the blast radius is unknown until proven.
- Non-authoritative aggregate results (e.g. a whole-suite run that hangs or is
  inconclusive) are recorded as **non-authoritative** in
  `readiness/aggregate-hang-diagnostics.md` with a focused-rerun block; the
  focused rerun is authoritative.

## Canonical Verification Targets

`Route` is the authority on the gate list for this diff. Run only what it
prints, FAKE-backed targets sequentially in the deterministic order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

`./fake.sh build -t DependencyReport` regenerates central-package governance
output. `./fake.sh build -t Route --enforce` fails an escalated change that is
missing required evidence.

## Success-criterion → assertion mapping

- **SC-001** (4 safe bumps applied, all printed gates green) → T010.
- **SC-002** (zero surface/golden/generated-product diff) → T010 asserts no
  surface-baseline / golden / generated-product change after the safe bumps.
- **SC-003** (every held bump has an auditable adopt/defer disposition, none
  half-applied) → T012–T016 + T021.
- **SC-004** (fresh `dotnet new fs-skia-ui` project restores+builds) → T018.
- **SC-005** (`speckit_version` in `.specify/init-options.json` equals the
  version in use) → T009 + T011.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory wiring (spec + plan + research + data-model + quickstart present) and record Tier, affected paths, public-API impact (none), Elmish/MVU applicability (none for safe bumps), and the real-evidence obligations in `readiness/unsupported-scope.md`
- [X] T002 [P] [skillist: []] Author the audit-enforced readiness scaffolds discoverable before implementation: `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/generated-validation.md` (package-resolution=resolved, package-mismatch=false), `readiness/evidence-graph.md`, and `readiness/evidence-audit.md` (verdict token). Each names its authoritative command, artifact path, failure class, and next action
- [X] T003 [P] [skillist: []] Author the not-applicable visual/window readiness set (this feature renders nothing): `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md`, `readiness/real-image-evidence.md`, `readiness/generated-guidance-validation.md` — each marked not-applicable (version/governance-only change, no scene/window/screenshot surface) with the reason
- [X] T004 [skillist: []] Capture the before-state pin snapshot (`Directory.Packages.props`, `.specify/init-options.json` `speckit_version`, `dotnet --list-sdks`) into `readiness/before-pins.md` as the baseline the after-state diffs against

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Assert the zero-`.fsi`/zero-surface obligation (FR-003): record in `readiness/contract-impact.md` that no `.fsi`, surface baseline, golden, or sample contract is intended to change, and that the surface/golden gates are the enforcing assertion
- [X] T006 [skillist: []] Run `./fake.sh build -t Route` (and `--enforce`) on the clean tree to record the authoritative escalated gate list and a green baseline into `readiness/focused-gates.md` before any pin changes

**Checkpoint**: Foundation ready — baseline gate set and zero-delta obligation recorded; story work may begin.

---

## Phase 3: User Story 1 (US1) — bring safe pins current

- [X] T007 [US1] [skillist: []] Bump `FSharp.Core` `10.1.300 → 10.1.301` in `Directory.Packages.props`
- [X] T008 [US1] [skillist: []] Bump `Microsoft.Extensions.FileSystemGlobbing` `10.0.8 → 10.0.9` in `Directory.Packages.props`
- [X] T009 [US1] [skillist: []] Bump `speckit_version` `0.8.16 → 0.10.2` in `.specify/init-options.json`. This is a recorded-version edit: the repo's skill/command tree is canonical under `.agents/**` and the `.claude/**` tree is generated **from it** (not vendored from spec-kit upstream), so the bump does not pull upstream assets. Only if a `.agents/**` source asset is actually touched, regenerate the `.claude` tree with `./fake.sh build -t RefreshSurfaceBaselines` and let `SkillSyncCheck` confirm currency — do not hand-edit generated trees (SC-005)
- [X] T010 [US1] [skillist: []] Run the routed gate set sequentially (`Route` → `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck` → `EvidenceGraph` → `EvidenceAudit`) and confirm all printed gates green with **zero** surface-baseline, golden, and generated-product diff; capture logs under `readiness/logs/` (SC-001, SC-002, FR-002, FR-003)
- [X] T011 [US1] [skillist: []] Record the US1 outcome in `readiness/us1-validation.md`: the four safe bumps `applied`, the .NET SDK float (`10.0.301`, no `global.json`) acknowledged for completeness, and `speckit_version` matching the version in use (FR-001, FR-007)

**Checkpoint**: User Story 1 shippable — safe pins current, all routed gates green, zero contract delta.

---

## Phase 4: User Story 2 (US2) — adopt-or-defer each held major bump

- [X] T012 [P] [US2] [skillist: []] YamlDotNet `17.1.0 → 18.0.0`: apply the single pin, run the full routed gate set, then either keep it (`adopted`, all gates green, no source change) or `git checkout -- Directory.Packages.props` and record `deferred(<failing gate + symptom>)`. No half-applied state may remain (FR-004, FR-005)
- [X] T013 [P] [US2] [skillist: []] Fable.Elmish `4.2.0 → 5.0.2`: apply, run the full routed gate set, then adopt (gates green, no source change) or revert and record the deferral reason — highest blast radius (Controls.Elmish MVU runtime) (FR-004, FR-005)
- [X] T014 [US2] [skillist: []] Test-stack cluster Expecto `10.2.2 → 11.0.0` + Microsoft.NET.Test.Sdk `17.11.1 → 18.6.0` + YoloDev.Expecto.TestSdk `0.15.3 → 1.0.0`: bump the three together (they interlock), run the full routed gate set, then adopt the whole cluster or revert the whole cluster and record the reason — never a partial cluster (FR-004, FR-005)
- [X] T015 [US2] [skillist: []] FSharp.Core 11.x line (`11.0.101-preview5`): record `deferred` as out-of-scope (tied to a newer F#/SDK, not drop-in on `net10.0`); not attempted, per spec
- [X] T016 [US2] [skillist: []] Confirm no partially-applied breaking bump remains (`git status` / `git diff Directory.Packages.props` shows only adopted pins) and record the per-bump adopt/defer dispositions in `readiness/us2-validation.md` (SC-003, FR-005)

**Checkpoint**: User Story 2 shippable — every held bump has an auditable adopt-or-defer disposition; the tree is clean.

---

## Phase 5: User Story 3 (US3) — template stays consistent

- [X] T017 [US3] [skillist: fs-skia-template-update] Refresh the consumer-facing template pins (`template/**`) only if the adopted bumps make a generated project inconsistent, via the `fs-skia-template-update` skill (regenerate pins; do not hand-edit beyond what the skill governs) (FR-006)
- [X] T018 [US3] [skillist: fs-skia-template-update] Run `./fake.sh build -t TemplateCheck` then `./fake.sh build -t GeneratedProductCheck` and confirm a freshly generated `dotnet new fs-skia-ui` project restores and builds against the updated pins; capture evidence in `readiness/us3-validation.md` (SC-004, FR-006)

**Checkpoint**: User Story 3 shippable — generated project restores and builds against the current pins.

---

## Phase 6: Integration & Polish

- [X] T019 [skillist: []] Refresh `docs/reports/dependencies.md` pin notes to match the final pins and run `./fake.sh build -t DependencyReport` so its generated output reflects the new central-package versions
- [X] T020 [skillist: []] Finalize per-package outcomes in `research.md` and `data-model.md` (each row `applied` / `adopted` / `deferred(reason)` / `unchanged`), and confirm the out-of-scope deferrals (SkiaSharp 4.147 preview line, FAKE lock at 6.1.4, FSharp.Core 11.x) are recorded (SC-003)
- [X] T021 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; refresh `readiness/task-graph.md`
- [X] T022 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with **zero** synthetic markers (none are used); record the verdict token in `readiness/evidence-audit.md`

---

## Synthetic-Evidence Inventory

No synthetic evidence is planned or permitted for this feature (Principle V):
all evidence is the real build, real packed libraries, and real generated
project. This table stays empty.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
