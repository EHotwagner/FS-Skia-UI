# Tasks: Publish FS.Skia.UI to NuGet.org for Consumer Distribution

**Feature branch**: `064-publish-nuget-distribution`
**Spec**: `specs/064-publish-nuget-distribution/spec.md`
**Plan**: `specs/064-publish-nuget-distribution/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. The classification is assigned during
design/planning/task generation; implementation-time relabeling is forbidden.

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable from
a user-facing entry point and that path was actually exercised — here a fresh
`dotnet restore`/`build`/`test` transcript, a publish dry-run/idempotency
transcript, a pre-publish fail+pass transcript, or the live nuget.org push +
restore, captured under `readiness/`. Governance unit tests passing green alone
do **not** satisfy `[X]` for a `[US*]` task.

For the I/O-bearing publish story (US2), `[X]` also requires Elmish/MVU evidence:
the build front-end is the MVU boundary (`Engine/Model.fs` / `Update.fs` /
`Interpret.fs`). The new `PublishConfig`/effect contract must be exercised, the
pure `update` transition (StartTarget → emitted `PublishPackages` /
`PrePublishValidate` effect) asserted, and the interpreter run against a real
local-directory staging feed (real `dotnet pack`/anonymous read/`dotnet nuget
push --skip-duplicate`). No runtime framework `Model`/`Msg`/`Effect` changes
(Principle IV applies only to the build front-end — see plan §MVU/effect
boundary).

## Success-criterion → assertion mapping

- **SC-001** → the failing-first "no absolute local path in emitted `NuGet.config`"
  test (T010) backing the public-feed-only `writeLocalNuGetConfig` (T011), proven
  by the fresh-consumer restore transcript (T013).
- **SC-002** → the dry-run produces exactly **12** `PublishPlanRow`s with a
  push/skip decision and performs no network push without a credential — asserted
  by the pure-update/plan tests (T014/T015) and the dry-run transcript (T020).
- **SC-003** → idempotency: a real staging push then a re-run skips everything
  (`--skip-duplicate`), proven by T021.
- **SC-004** → exactly **one** literal `FS.Skia.UI` version in the generated
  project; the single-source `<FsSkiaUiVersion>` test (T022) backs the
  single-edit-upgrade transcript (T026).
- **SC-005** → `PrePublishCheck` fails naming the offending package/field for each
  skew class and passes once restored (failing-first tests T027 → transcript T032).
- **SC-006** → no in-repo doc still presents distribution as deferred/local-only
  (T035 sweep over T033/T034).
- **SC-007** → all Route-printed gates pass with the new targets in
  `knownGates`/`validation.contract.yml` (`TargetMetadataDrift`/`SkillSyncCheck`
  green after regen) and `EvidenceAudit verdict=PASS` (T036/T038/T039/T040).
- **SC-008** → all 12 packages live and publicly resolvable on nuget.org via a
  fresh-consumer restore (T041).

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**…**[US5]** — user-story scope
- **[T1]** — Tier 1 (contracted) change

Every task has a matching entry in `tasks.deps.yml`; every task line mirrors its
structured `skillist` as `[skillist: ...]`.

## Governance risk levels

- **small** — a single governance source edit (e.g. one routing rule, one
  knownGates entry): focused validation = `Dev` + the directly-affected
  Governance.Tests.
- **medium** — target registry + contract regeneration: focused validation =
  `Dev`, `GeneratedGuidanceCheck`, and the regenerated `validation.contract.yml`
  currency checks (`TargetMetadataDrift`, `SkillSyncCheck`).
- **broad** — template/`GeneratedProduct.fs` consumer-contract change: broad
  validation (`TemplateCheck`/`GeneratedProductCheck`) is required; the aggregate
  result is **non-authoritative** and recorded as such — the authoritative verdict
  is `EvidenceAudit verdict=PASS`. FAKE-backed gates run **sequentially**.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Run `./fake.sh build -t Route` against the working-tree diff and record the authoritative tier + minimal gate list to `readiness/target-metadata.md`
- [X] T002 [P] [skillist: []] Scaffold `readiness/` audit-enforced placeholder files discoverable before implementation: `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`, `governance-risk-levels.md`, `runtime-limitations.md`, `fresh-consumer-restore.md`, `publish-dry-run.md`, `publish-idempotency.md`, `single-edit-upgrade.md`, `prepublish-check.md`, `validation-contract.md`, `production-publish.md`, `evidence-graph.md`, `evidence-audit.md` (no visual-demo scaffolds — this feature has no rendering/window-visibility surface)
- [X] T003 [P] [skillist: []] Record feature Tier (Tier 1 distribution/consumer-contract change), affected layers (`build/Governance/**`, `template/base/**`, `src/*/*.fsproj`, docs), public-API impact (no runtime `.fsi`; distribution contract only), Elmish/MVU applicability (build front-end MVU — publish effects), and required evidence obligations to `readiness/agent-ready-verdict.md`
- [X] T004 [skillist: []] Record unsupported scope (private-feed Option 2, bespoke upgrade tool, floating ranges, `.snupkg`), the small/medium/broad governance risk levels, and aggregate-hang diagnostics into `readiness/runtime-limitations.md`, `readiness/governance-risk-levels.md`, and `readiness/aggregate-hang-diagnostics.md`

---

## Phase 2: Foundation

- [X] T005 [skillist: []] Draft the new governance public surface as `.fsi`: `build/Governance/PrePublish.fsi` (`PrePublishFinding` { Package; Field; Rule; Detail }, the four-rule `check` surface) and the publish additions to `Engine/Model.fsi` / `Update.fsi` / `Interpret.fsi` (`PublishConfig` fields, `PublishPlanRow`, `PublishPackages` / `PrePublishValidate` effect cases) — no runtime framework `.fsi` change
- [X] T006 [P] [skillist: fsharp-build-orchestration] Register `Publish` and `PrePublishCheck` in `build/Governance/Targets.fs` (DU + `name` + `directPrerequisites` + `allTargets`; bump the registry-count comment near `Targets.fs:58` and any `TargetMetadata`/count test by **+2** — verify the exact current count from `Targets.fs`) and add both to the `knownGates` allowlist in `AgentValidation.fs`
- [X] T007 [P] [skillist: fsharp-build-orchestration] Add a distribution routing rule to `build/Governance/Routing.fs` classifying the publish/pre-publish targets plus `template/base/build.fsx` + `Directory.Packages.props` changes (escalated tier)
- [X] T008 [skillist: []] Exercise the drafted `PrePublish.fsi` + `PublishConfig`/`PublishPlanRow` types from FSI and capture the session transcript to `readiness/fsi-session.txt`
- [X] T009 [skillist: []] Record the surface disposition — the new governance `.fsi` files are internal to `FS.Skia.UI.Build` (not packed), so `PerPackageSurfaceDiff`/`PackageSurfaceCheck` baselines are unaffected; note this in `readiness/target-metadata.md`

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — fresh consumer restores from a public feed (FR-003)

### Tests First (Principle I, Principle VI)

- [X] T010 [P] [US1] [skillist: []] Add a failing-first Governance.Tests assertion that the consumer `NuGet.config` emitted by `writeLocalNuGetConfig` contains **no** absolute local feed path (`/home/.../nuget-local`) and references the public feed only (SC-001)

### Implementation

- [X] T011 [US1] [T1] [skillist: fsharp-code-generation] Change `writeLocalNuGetConfig` in `build/Governance/GeneratedProduct.fs:1372` to emit a **public-feed-only** consumer `NuGet.config` (drop the machine-absolute `local` source); keep the in-repo dev loop's local feed separate
- [X] T012 [US1] [skillist: fs-skia-template-update] Add the staging-feed overlay so `TemplateCheck`'s in-repo restore still resolves `FS.Skia.UI.*` from `~/.local/share/nuget-local` even though the emitted consumer config no longer carries that path
- [X] T013 [US1] [skillist: fs-skia-template-update] Verification: generate the `app` and `governed` profiles, run `dotnet restore`/`build`/`test` from the throwaway local-directory **staging** feed with **no** machine-local path present, and capture the transcript to `readiness/fresh-consumer-restore.md` (SC-001)

**Checkpoint**: A fresh consumer restores entirely from a public/staging feed.

---

## Phase 4: User Story 2 (US2) — maintainer publishes a full release with one safe command (FR-001/002/007)

### Tests First

- [X] T014 [P] [US2] [skillist: fsharp-build-orchestration] Failing-first pure-`update` test: `StartTarget Targets.Publish` emits the `PublishPackages` effect (and `PrePublishCheck` emits `PrePublishValidate`); `update` stays pure with all I/O at the interpreter edge; **and a non-dry-run with `ApiKeyPresent = false` aborts before any push, naming the missing key (data-model PublishConfig "Validation"), while dry-run needs no credential** (FR-002 / spec Edge Case "missing/invalid API key")
- [X] T015 [P] [US2] [skillist: fsharp-build-orchestration] Failing-first test: the dry-run plan is exactly **12** `PublishPlanRow`s (11 libs from `packProjects` + `FS.Skia.UI.Template`) with per-package version + push/skip decision and performs no network push (SC-002)

### Implementation

- [X] T016 [US2] [skillist: []] Add `PublishConfig` fields to `Engine/Model.fs` (FeedUrl/ReadUrl/ApiKeyPresent/DryRun/IsLocalFeed), read from env at the interpreter edge
- [X] T017 [US2] [skillist: []] Add the `StartTarget Publish` / `StartTarget PrePublishCheck` handlers to `Engine/Update.fs` — pure `update` emitting `PublishPackages` / `PrePublishValidate` (reuse `processEffect` for pack)
- [X] T018 [US2] [skillist: fsharp-shell-process] Interpret `PublishPackages` in `Engine/Interpret.fs`: anonymous read of the target feed (nuget.org flat-container `index.json` or local-directory listing), compute skip/push per package, and run `dotnet nuget push --skip-duplicate`; a non-dry-run with no API key aborts fast pushing nothing
- [X] T019 [US2] [skillist: fsharp-build-orchestration] Regenerate `validation.contract.yml` from `Routing.fs` (`RefreshSurfaceBaselines`) so `Publish`/`PrePublishCheck` appear and `TargetMetadataDrift` stays green (FR-007)
- [X] T020 [US2] [skillist: fsharp-shell-process] Verification: run `Publish` in **dry-run** against the local-directory staging feed with **no** credential and capture the 12-row push/skip plan (no network push) to `readiness/publish-dry-run.md` (SC-002)
- [X] T021 [US2] [skillist: fsharp-shell-process] Verification: real push to the staging feed, then a second run **skips** all 12 (idempotent), and a partial-failure re-run pushes only the remainder — capture to `readiness/publish-idempotency.md` (SC-003)

**Checkpoint**: One publish command pushes all 12 packages, idempotently and dry-runnably.

---

## Phase 5: User Story 3 (US3) — single-edit upgrade via one source of version truth (FR-004/005)

### Tests First

- [X] T022 [P] [US3] [skillist: fsharp-parsing] Failing-first test: a generated project has exactly **one** literal `FS.Skia.UI` version value, and `build.fsx`'s engine reference resolves from `<FsSkiaUiVersion>` rather than a second literal (single-source invariant) (SC-004)

### Implementation

- [X] T023 [US3] [T1] [skillist: fs-skia-template-update] Introduce `<FsSkiaUiVersion>` in `template/base/Directory.Packages.props` and rewrite the 11 `<PackageVersion Include="FS.Skia.UI.*">` pins to `Version="$(FsSkiaUiVersion)"`
- [X] T024 [US3] [T1] [skillist: fs-skia-template-update] Make `template/base/build.fsx` read `<FsSkiaUiVersion>` from `Directory.Packages.props` at runtime and `#r` the resolved engine assembly path (R1 technique), dropping the literal `#r "nuget: FS.Skia.UI.Build, <ver>"` version — disclosed with a one-line comment at the use site
- [X] T025 [US3] [skillist: []] Author `template/base/docs/UPGRADING.md` — the single value to change, `dotnet restore`, how to verify, and preview-vs-stable selection (FR-005)
- [X] T026 [US3] [skillist: fs-skia-template-update] Verification: in a generated project, change the one documented version value, run `dotnet restore`, and confirm both the libraries **and** the build engine resolve at the new version with no second edit — capture to `readiness/single-edit-upgrade.md` (SC-004)

**Checkpoint**: A consumer upgrade is one edit.

---

## Phase 6: User Story 4 (US4) — release verified internally consistent before any push (FR-006/010)

### Tests First

- [X] T027 [P] [US4] [skillist: fsharp-build-orchestration] Failing-first tests: `PrePublishCheck` produces a finding **naming the offending package/field** for each skew class — `PinParity` (template pin ≠ shipped version), `EnginePinMatch` (build-engine pin ≠ lib version), `NoMachineLocalPath` (absolute local path in emitted config), `RequiredMetadata` (blank license/repo/authors/description/README) (SC-005)

### Implementation

- [X] T028 [US4] [skillist: fsharp-parsing] Implement `build/Governance/PrePublish.fs` (the four rules over `template/base/Directory.Packages.props`, `build.fsx`, the emitted `NuGet.config`, and each packable + template `.fsproj`), each finding naming expected-vs-actual
- [X] T029 [US4] [skillist: fsharp-build-orchestration] Wire the `PrePublishCheck` target to compose with / extend `TemplateCheck`, aborting the publish and naming the offender on any finding
- [X] T030 [P] [US4] [T1] [skillist: fsharp-io-globbing] Add per-package `README.md` ×11 and `RepositoryUrl` / license expression / authors / description / `PackageReadmeFile` (+ tags/icon where applicable) metadata to each `src/*/<Pkg>.fsproj` (FR-010)
- [X] T031 [P] [US4] [T1] [skillist: fs-skia-template-update] Add the template package `README.md` and required metadata to `.template.package/FS.Skia.UI.Template.fsproj` (FR-010)
- [X] T032 [US4] [skillist: fsharp-build-orchestration] Verification: introduce a deliberate skew, confirm `PrePublishCheck` fails naming the specific package/field, restore consistency, confirm it passes and publish cannot proceed while it fails — capture fail+pass to `readiness/prepublish-check.md` (SC-005)

**Checkpoint**: A malformed/inconsistent release can never be pushed.

---

## Phase 7: User Story 5 (US5) — distribution + release flow documented, superseding "deferred" (FR-009)

- [X] T033 [P] [US5] [skillist: []] Author `docs/distribution.md`: consumer install flow (`dotnet new install FS.Skia.UI.Template` → `dotnet new fs-skia-ui` → restore/build), the public feed + preview/stable channel, and the maintainer release+publish flow (bump → pack → pre-publish check → publish)
- [X] T034 [P] [US5] [skillist: []] Update `docs/adr/0001-...distribution.md` to record that the publish path is now implemented, superseding the "distribution deferred" note
- [X] T035 [US5] [skillist: []] Verification: sweep in-repo docs confirming none still present distribution as deferred or local-feed-only as the consumer story; record the disposition (SC-006)

**Checkpoint**: Distribution is documented and discoverable.

---

## Phase 8: Integration & Polish

- [X] T036 [skillist: fs-skia-template-update] Regenerate the `.claude` skill tree + `validation.contract.yml` (`RefreshSurfaceBaselines`) and confirm `TargetMetadataDrift` / `SkillSyncCheck` green with the new targets; record currency to `readiness/validation-contract.md` (FR-011)
- [X] T037 [skillist: fs-skia-template-update] Bump, pack, and install the `FS.Skia.UI.Template` package so the distribution changes (public-feed config, single-source pin, `UPGRADING.md`, READMEs) reach generated consumers (FR-011)
- [X] T038 [skillist: fs-skia-template-update] Run the Route-printed gates sequentially — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck` — and record the **non-authoritative** aggregate notes to `readiness/target-metadata.md` (the authoritative verdict is `EvidenceAudit verdict=PASS`)
- [X] T039 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises; write the effective DAG to `readiness/task-graph.md` and `readiness/evidence-graph.md`
- [X] T040 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm `verdict=PASS` for `specs/064-publish-nuget-distribution` with no `[S]`/`[S*]` and no diff-scan hits, and that all Route-printed gates pass; record to `readiness/evidence-audit.md` (SC-007)
- [ ] T041 [US1] [skillist: fs-skia-template-update] **Maintainer-gated** production push (FR-008): after the gate is green, the maintainer supplies the nuget.org credential and pushes the current `-preview` versions unchanged (libs `0.1.67-preview.1`, template `0.1.86-preview.1`) on the preview channel; capture the live push transcript + a fresh-consumer restore against **nuget.org** proving all 12 packages publicly resolvable to `readiness/production-publish.md` (SC-008). This task legitimately remains `[ ]` after `EvidenceAudit verdict=PASS` (T040) — the audit gates on `[S]`/`[S*]`/diff-scan, not pending maintainer steps, so a green audit is **not** by itself "feature complete" per SC-008.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

No synthetic evidence is planned — all evidence is real: the staging feed is a
real throwaway local-directory feed exercised with real `dotnet pack`/anonymous
read/`dotnet nuget push --skip-duplicate`, the pre-publish fail case is a real
negative test over deliberately-skewed real files, and the production push is a
real maintainer step. `[S]` disclosure applies only if a real path proves
infeasible mid-implementation; none anticipated, and no `[SEH]` cases are
foreseen.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
