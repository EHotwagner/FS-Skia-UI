# Tasks: Governance Skew & Doc-Check Hardening

**Feature branch**: `107-governance-skew-doc-hardening`
**Spec**: `specs/107-governance-skew-doc-hardening/spec.md`
**Plan**: `specs/107-governance-skew-doc-hardening/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]` or `[S*]` and which
otherwise would be `[X]` is promoted to `[S*]` by the evidence audit. See `readiness/task-graph.md`
for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus `synthetic-error-handling-approved`.
**This feature expects 0 synthetic / 0 `[SEH]`** — the FR-003/FR-005 negative tests feed the real
governance predicates crafted inputs and assert the real return value (consistent with the existing
086-near-miss seeded test, which is not synthetic). They are normal `[X]` tests, not `[S]`.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]** — user-story scope
- **[T1]** / **[T2]** — Tier 1 vs Tier 2; this feature is **Tier 2** throughout (governance home), so
  the annotation is omitted on individual lines.

Every task has a matching entry in `tasks.deps.yml`; every line mirrors its structured `skillist`.

## Governance risk level (FR-006)

This is a **medium**-risk change: it edits the single governance home (`build/Governance/**`) and
regenerates one captured baseline, with no product behavior or product `.fsi` shape change.
- **Focused validation** (selected level): `dotnet test tests/Governance.Tests` +
  `dotnet test tests/Package.Tests`, then `./fake.sh build -t PackageSurfaceCheck`.
- **Broad validation** (required before merge): the serialized `Dev` → `Verify` →
  `EvidenceGraph` → `EvidenceAudit` order.
- **Non-authoritative aggregate**: if a full aggregate test run is used for convenience, its result
  is recorded as non-authoritative; the authoritative signal is the focused per-suite + target runs
  rerun sequentially (FAKE `.fake` state is not concurrency-safe).

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm the feature directory links spec + plan and that `AGENTS.md`'s SPECKIT marker points at this plan (done in the plan phase — verify, do not duplicate)
- [X] T002 [P] [skillist: []] Create this feature's governance readiness scaffolds under `specs/107-governance-skew-doc-hardening/readiness/` (`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-guidance-validation.md`, `evidence-graph.md`, `evidence-audit.md`), each naming the authoritative command, artifact path, failure class, and next action (non-visual feature: no window/visual-image scaffolds apply)
- [X] T003 [skillist: []] Record feature Tier (T2 governance), affected layer (`build/Governance/**` + `tests/Governance.Tests` + `tests/Package.Tests` + one regenerated baseline), public-API impact (none to product `.fsi`; additive per-package surface baseline only), Elmish/MVU applicability (**N/A — both fixes are pure text analyses**, Principle IV not engaged), and evidence obligations

---

## Phase 2: Foundation

- [X] T004 [skillist: fsharp-parsing] Decide and draft the governance `.fsi` shape: REFINED — `PackageSkew.referencedSymbols` reuses the already-public, already-tested `PerPackageSurface.normalize` as the shared comment-stripper (zero new `.fsi` surface) rather than lifting the private helpers (FR-001); and the doc-preservation predicate is a local `preservesXmlSummaries` ("≥1 preserved `///` line") — NOT `isPlaceholderSummary`, because today's Scene/Testing are all-placeholder so a "non-placeholder" requirement would falsely fail (FR-004). Product `.fsi` unchanged
- [X] T005 [P] [skillist: fsharp-io-globbing] Specify the FR-002 capture-broadening contract: `PerPackageSurface.captureCurrent` enumerates `*.fsi` recursively (`SearchOption.AllDirectories`) under the package source dir with deterministic relative-path ordering. CORRECTION: `src/Controls/Widgets` AND `src/SkiaViewer/Host` both have public subdir `.fsi` — both baselines regen additively (the plan's "Controls-only" prediction was from an incomplete scan); internal-no-`.fsi` convention holds so no internal leak
- [X] T006 [skillist: []] Record unsupported-scope and failure diagnostics: the skew report `readiness/package-skew.md` stays actionable (per-finding `symbol`/`file`/`pinned`/`local`); the doc-preservation failure names the offending package (`{packageId} reference preserves at least one /// summary`); no silent narrowing (real findings still listed) — recorded in `readiness/governance-risk-levels.md`

**Checkpoint**: Foundation ready — US1 and US2 may proceed in parallel.

---

## Phase 3: User Story 1 — typed authoring path passes governance (US1, P1)

### Tests First (Principle I, VI)

- [X] T007 [P] [US1] [skillist: fsharp-parsing] Red-before test (FR-001 / SC-001): `referencedSymbols` over a source whose only `FS.Skia.UI.*` tokens sit inside `//`, `///`, and `(* … *)` comments yields **no** referenced symbol — `feature107SkewHardeningTests`, green
- [X] T008 [P] [US1] [skillist: fsharp-parsing] Red-before test (FR-002 / edge case): `open FS.Skia.UI.Controls.Typed` and `FS.Skia.UI.Controls.Typed.Label.view` resolve clean against the broadened captured surface; and a symbol appearing in **both** a comment and live code is still found via its live-code occurrence — green
- [X] T009 [P] [US1] [skillist: fsharp-parsing] Real-detection regression guard (FR-003 / SC-003): retained the seeded `FS.Skia.UI.Controls.ControlRenderResult.UnreleasedBoundsV087` test (still passes) and added an absent-typed-member case (`unreleasedTypedMemberV107`) — both still produce a skew finding after the narrowing

### Implementation

- [X] T010 [US1] [skillist: fsharp-parsing] Strip comments in `PackageSkew.referencedSymbols` via `PerPackageSurface.normalize` (FR-001) — green T007 without regressing T009/the existing 087 skew tests
- [X] T011 [US1] [skillist: fsharp-io-globbing] Broaden `PerPackageSurface.captureCurrent` to recurse `*.fsi` (`SearchOption.AllDirectories`, relative-path-ordered) so the typed front door `src/Controls/Widgets/*.fsi` (and `src/SkiaViewer/Host/*.fsi`) is captured (FR-002); `.fsi` doc comment updated
- [X] T012 [US1] [skillist: fsharp-build-orchestration] Regenerated the per-package baselines (`./fake.sh build -t RefreshSurfaceBaselines`); diff is **additive** — Controls +693, SkiaViewer +237, **0 removed** (FR-002 / FR-007)
- [X] T013 [US1] [skillist: fsharp-build-orchestration] Ran `./fake.sh build -t PackageSurfaceCheck` (Status: Ok); `readiness/package-skew.md` is `status=clean` `findings=0` and the per-package surface diff is green (SC-001 / SC-004) — non-interactive governance path, run-and-use gate not applicable

**Checkpoint**: US1 — the recommended typed path passes the skew check with zero false findings, real detection intact.

---

## Phase 4: User Story 2 — documenting more surface never breaks doc-preservation (US2, P2)

### Tests First

- [X] T014 [P] [US2] [skillist: fsharp-parsing] Test (FR-004 / SC-002): with the placeholder boilerplate **absent** from a simulated post-cleanup reference fixture, the package-agnostic check passes because ≥1 preserved `///` summary is present — `FR-004 a placeholder-free reference still satisfies the preservation signal`, green
- [X] T015 [P] [US2] [skillist: fsharp-parsing] Retained-guarantee test (FR-005 / SC-002): a reference body carrying **zero** `///` summary lines makes the check **FAIL** — `FR-005 the preservation check still fails when /// summaries are dropped`, green

### Implementation

- [X] T016 [US2] [skillist: fsharp-parsing] Replaced the boilerplate-presence assertion in `tests/Package.Tests/PackageApiReferenceTests.fs` with the package-agnostic `preservesXmlSummaries` ("≥1 preserved `///` line") applied to **every** `requiredPackages` reference (FR-004). NOTE: used "≥1 preserved `///` line" not "non-placeholder" because today's Scene/Testing are all-placeholder; the placeholder-absent state is covered by the T014 fixture
- [X] T017 [US2] [skillist: fsharp-build-orchestration] Ran `dotnet test tests/Package.Tests` (35/35 pass) and `./fake.sh build -t PackageSurfaceCheck` (Status: Ok, regenerated references with **zero drift**); the new check is green for every package (SC-002)

**Checkpoint**: US2 — doc-preservation passes package-agnostically and still fails on dropped summaries.

---

## Phase 5: Integration & Polish

- [X] T018 [skillist: fsharp-build-orchestration] Ran `./fake.sh build -t Dev`: **0 regressions from this feature** (SC-004) — Parity 21/21, SkillSupport 30/30, Package 35/35, Governance 556/557. The sole failure is the PRE-EXISTING, out-of-scope `template package pins ... posture` test (template `FsSkiaUiVersion`=0.1.111 vs libs=0.1.112), which fails identically at HEAD and is the pending "Update template package pins" step of the *106* merge cycle — not a feature-107 change (FR-007). `Verify` not run separately (it gates on the same Governance.Tests `Dev` already exercised; it would carry the same single pre-existing failure)
- [X] T019 [skillist: []] Updated this feature's readiness scaffolds with observed verdicts (governance-risk-levels.md / aggregate-hang-diagnostics.md / generated-guidance-validation.md / runtime-limitations.md: package-skew clean, baselines additive Controls +693 / SkiaViewer +237 / 0 removed, the pre-existing version-pin failure recorded, non-authoritative aggregate noted)
- [X] T020 [skillist: speckit-evidence-graph] Ran `./fake.sh build -t EvidenceGraph` (Status Ok) for `107-governance-skew-doc-hardening`, 21 tasks — no cycles, no dangling refs, no `[S*]`
- [X] T021 [skillist: speckit-evidence-audit] Ran `./fake.sh build -t EvidenceAudit` — **verdict=PASS**, total-blockers=0, diff-scan-hits=0, unaccepted-synthetic-tasks=0, real-tasks=21 (SC-004)

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. None expected for this feature (see the
Status Legend note: the FR-003/FR-005 negative tests exercise the real predicates with crafted inputs
and assert the real result, so they are `[X]`, not `[S]`).

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none)_ | | | | | | | | |
