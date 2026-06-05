# Implementation Plan: Publish FS.Skia.UI to NuGet.org for Consumer Distribution

**Branch**: `064-publish-nuget-distribution` | **Date**: 2026-06-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/064-publish-nuget-distribution/spec.md`

## Summary

Turn the project from **local-feed-only** into a **published NuGet.org distribution**. Four
coupled deliverables: (1) a feed-agnostic, idempotent, dry-runnable **`Publish`** target that pushes
all 11 `FS.Skia.UI.*` libraries + the `FS.Skia.UI.Template` package (FR-001/002); (2) a
**`PrePublishCheck`** gate that aborts a malformed/inconsistent release naming the offending
package/field (FR-006/010); (3) a **fresh-consumer config** — the generated `NuGet.config` drops the
machine-absolute local path and a **single MSBuild version property** (`<FsSkiaUiVersion>`) collapses
the dual pin so a consumer upgrade is one edit (FR-003/004/005); and (4) the **governance wiring +
docs + first live nuget.org push** that make the capability real and discoverable (FR-007/008/009/011).
The publish *machinery* is validated against a **throwaway local-directory staging feed** with no
credential; the irreversible **first production push** to nuget.org is the maintainer-triggered final
step once the gate is green (FR-008, SC-008).

Technical spine, confirmed against the codebase:

- The emitted consumer config is `writeLocalNuGetConfig` in `build/Governance/GeneratedProduct.fs:1372`
  — it hardcodes `<add key="local" value="{model.LocalPackageDir}" />` (an absolute
  `~/.local/share/nuget-local` path). FR-003 removes that source from the **consumer-emitted** config.
- The dual pin is `template/base/Directory.Packages.props` (`<PackageVersion Include="FS.Skia.UI.*"
  Version="0.1.67-preview.1" />` ×11) **and** `template/base/build.fsx:1`
  (`#r "nuget: FS.Skia.UI.Build, 0.1.67-preview.1"`). FR-004 collapses both onto one
  `<FsSkiaUiVersion>` property.
- The pack set is `packProjects` in `build/Governance/Front/Helpers.fs:29` (11 entries) — the exact set
  `Publish` pushes, plus the template package (`.template.package/FS.Skia.UI.Template.fsproj`).
- Targets are a closed F# DU in `build/Governance/Targets.fs`; routing is `build/Governance/Routing.fs`;
  pack effects are emitted in `build/Governance/Engine/Update.fs` (`StartTarget Targets.PackLocal` at
  line 90, `TemplatePack` at 166) and run by the interpreter. `Publish`/`PrePublishCheck` slot in here.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (compiled `FS.Skia.UI.Build` governance front-end; F# scripts for `build.fsx`)
**Primary Dependencies**: existing only — `dotnet pack` / `dotnet nuget push` CLI, NuGet flat-container HTTP API for anonymous feed reads. **No new package dependency.**
**Testing**: Expecto governance tests (`tests/Governance.Tests`), FAKE targets run via the compiled front-end, generated-product evidence (`TemplateCheck` / `GeneratedProductCheck`), and a local-directory staging feed for publish dry-run/idempotency.
**Target Platform**: Linux + Windows (publish target is OS-agnostic; staging feed is a temp directory).

**Resolved unknowns** (see [research.md](./research.md)):
- **R1** — how `build.fsx` derives its engine reference from `<FsSkiaUiVersion>` at runtime given the `#r`-literal constraint. **Resolved**: build.fsx reads the property from `Directory.Packages.props` at runtime and `#r`s the resolved engine assembly path; recommended technique + alternatives in research.
- **R2** — staging feed mechanism for credential-free dry-run/idempotency and fresh-consumer restore. **Resolved**: throwaway local-directory feed (temp dir distinct from `~/.local/share/nuget-local`), anonymous read = directory listing; nuget.org read = flat-container `index.json`.
- **R3** — idempotency mechanism. **Resolved**: `dotnet nuget push --skip-duplicate` for the push edge, plus a pre-push anonymous read so dry-run reports skip/push without a credential.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

This is a **Tier 1 (contracted change)** — it changes the *distribution* contract (consumer
`NuGet.config`, single-source pin, package metadata) and adds governed build targets — but **no runtime
`.fsi` signature changes**. Principle II (visibility in `.fsi`) applies to the new
`build/Governance/PrePublish.fs(i)` and any `Publish` effect additions: every new public module gets a
curated `.fsi`. Principle IV (MVU) is satisfied because the build front-end is already an MVU engine
(`Engine/Model.fs` / `Update.fs` / `Interpret.fs`): `Publish`/`PrePublishCheck` are new `Msg`/`Effect`
cases handled by a pure `update` with I/O (pack, anonymous-read, push) at the interpreter edge.
Principle VI: failing-first governance tests for the new targets/validators. Principle V: the
**live nuget.org push (FR-008)** depends on a maintainer credential and is **not** synthesised — it is a
real maintainer-gated step; the staging-feed validations are real (local directory feed), not mocks, so
**no `[S]` disclosure is anticipated**. Any unavoidable placeholder (e.g. a metadata fixture) gets `[S]`.

### Repository Governance Decisions

- **Template ownership**: **Changes required.** `template/base/Directory.Packages.props` (single
  `<FsSkiaUiVersion>` property + pins referencing it), `template/base/build.fsx` (read property at runtime,
  drop the literal `#r` version), the consumer-emitted `NuGet.config` (public-feed-only, produced by
  `GeneratedProduct.fs`, not a static template file), a new **consumer upgrade doc** under
  `template/base/docs/` (FR-005), and **per-package README files** (FR-010). `.template.config/template.json`
  is updated only if new shipped files (upgrade doc, READMEs included in generated output) need explicit
  inclusion/`copyOnly` — verified during implementation; the template **package** version is
  bumped/packed/installed (FR-011) so distribution changes reach consumers.
- **Dependency impact**: **N/A for new packages** — no new `PackageReference` is added; `dotnet nuget push`
  and the flat-container HTTP read use the SDK + `System.Net.Http` already present. `Directory.Packages.props`
  (repo + template) changes only to introduce the single-source version property. `DependencyReport`
  coverage is unchanged (no new dependency to report).
- **Command-surface impact**: **Changes required.** Two new targets — `Publish` and `PrePublishCheck` —
  land in `build/Governance/Targets.fs` (DU + `name` + `directPrerequisites` + `allTargets`), are
  dispatched from the front-end, classified by `Routing.fs`, added to the `knownGates` allowlist
  (`AgentValidation.fs`), and regenerated into `validation.contract.yml` so `TargetMetadataDrift` /
  `SkillSyncCheck` stay green. `TemplateCheck` / `GeneratedProductCheck` change for the new consumer
  config + single-source pin. FAKE-backed commands run **sequentially** in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
  The authoritative tier + gate list is whatever `./fake.sh build -t Route` prints for the actual diff.
- **Generated project impact**: **Changes required.** The generated `NuGet.config` references the
  **public feed only** (no absolute local path); `Directory.Packages.props` carries one `<FsSkiaUiVersion>`
  that all `FS.Skia.UI.*` pins **and** `build.fsx`'s engine reference derive from; a shipped upgrade doc
  describes the single-edit upgrade + preview/stable selection. The **in-repo** template-validation loop
  keeps using `~/.local/share/nuget-local` via a **staging-feed overlay** applied by `TemplateCheck`, so
  validation restores succeed before the packages are on nuget.org — the consumer config no longer carries
  that path.
- **Evidence paths**: All under `specs/064-publish-nuget-distribution/readiness/`:
  - Route escalated-tier set: `target-metadata.md`, `agent-ready-verdict.md`, `skill-loading-evidence.md`,
    `aggregate-hang-diagnostics.md`.
  - `fresh-consumer-restore.md` — a project generated with the new template restores+builds+tests from the
    **staging** feed with **no** machine-local path (US1, SC-001).
  - `publish-dry-run.md` — per-package push/skip plan over all 12 packages, no credential, no network push (US2, SC-002).
  - `publish-idempotency.md` — real push to staging then a second run skips everything (US2, SC-003).
  - `prepublish-check.md` — fail+pass transcript for a deliberately-skewed set (US4, SC-005).
  - `single-edit-upgrade.md` — one version-value edit upgrades libs **and** engine (US3, SC-004).
  - `production-publish.md` — **maintainer-gated**: live nuget.org push transcript + fresh-consumer restore
    against nuget.org confirming all 12 packages publicly resolvable (FR-008, SC-008).
  - `validation-contract.md`, `evidence-graph.md`, `evidence-audit.md` — governance currency (SC-007).
- **`.fsi` / contract impact**: **No runtime `.fsi` change.** New governance `.fsi` files:
  `build/Governance/PrePublish.fsi` (pre-publish validator surface) and any new effect/case exposed from
  `Engine/Model.fsi` / `Update.fsi` / `Interpret.fsi` for the publish flow. The changed **public contract**
  is the *distribution* contract (consumer config, single-source pin, package metadata, install/update
  docs) — recorded in `contracts/` here, not in runtime surface baselines.
- **MVU/effect boundary**: The build front-end is the MVU boundary. **Model**: existing
  `Engine/Model.fs` `Model` (add publish config fields: feed URL, api-key presence, dry-run flag — read
  from env at the edge). **Msg**: `StartTarget Targets.Publish`, `StartTarget Targets.PrePublishCheck`.
  **Effect**: new `PublishPackages` (and reuse `processEffect` for pack) + a `PrePublishValidate` effect;
  pure `update` emits them, the **interpreter** (`Interpret.fs`) performs the anonymous feed read, the
  skip/push decision, and the `dotnet nuget push`. `update` stays pure; all I/O (pack, read, push) is at
  the edge. Real interpreter evidence = the dry-run/idempotency transcripts.
- **Synthetic evidence**: **None anticipated.** Staging-feed validations are real (local directory feed +
  real `dotnet pack`/restore). The live nuget.org push is a real maintainer step, not a mock. The only
  candidate `[S]` is a *deliberately-skewed* metadata/pin fixture used to prove the pre-publish check
  **fails** (US4) — that is a real negative test over real files, not synthetic evidence, but if any
  fixture is purely literal it carries `[S]` + the `Synthetic` token per Principle V.
- **Test evidence**: Failing-first Expecto governance tests for: `PrePublishCheck` catching each skew class
  (pin≠shipped version, blank required metadata, machine-local path in emitted config, build-engine pin
  mismatch); the emitted consumer `NuGet.config` containing **no** absolute local path; the single-source
  property being honored by both the pins and `build.fsx`; `Publish` dry-run producing a 12-row plan with
  no push; idempotent skip on a re-run. Plus target-registry/contract tests (`Targets`, `validation.contract.yml`).
- **Observability**: `Publish` and `PrePublishCheck` emit structured diagnostics: the dry-run plan (12
  rows: package, version, feed-state, push/skip), and pre-publish failures that **name the offending
  package and field** and abort. Missing-credential on a real push fails **fast** with a clear message and
  pushes nothing. Logs under `model.LogDir` / readiness paths above; missing required artifact-classes fail
  the gate.
- **Deferred scope**: Out of scope — the **private-feed** option (Option 2); a bespoke automated
  consumer-upgrade tool / `dotnet tool` beyond single-edit + docs; floating/wildcard version ranges in
  generated projects (pins stay exact); unifying the library/template **version relationship**;
  SourceLink / `.snupkg` symbol packages (revisit only if research finds them low-cost). The
  irreversible **first production push** is **in scope** but is the final maintainer-triggered step.

## Project Structure

Edited / created paths for this feature (repo-relative):

```
build/Governance/
  Targets.fs                      # + Publish, PrePublishCheck DU cases / name / prereqs / allTargets
  Routing.fs                      # + a distribution rule classifying the publish/pre-publish + template/build changes
  PrePublish.fs / PrePublish.fsi  # NEW: pre-publish consistency validator (pin parity, metadata, no-local-path)
  GeneratedProduct.fs             # writeLocalNuGetConfig -> public-feed-only consumer config; single-source pin emit
  AgentValidation.fs              # + Publish, PrePublishCheck in knownGates allowlist
  Engine/
    Model.fs / Model.fsi          # + publish config fields + PublishPackages / PrePublishValidate effect cases
    Update.fs / Update.fsi        # + StartTarget Publish / PrePublishCheck handlers (pure, emit effects)
    Interpret.fs / Interpret.fsi  # + interpret PublishPackages (anonymous read, skip/push) + PrePublishValidate
  Front/Helpers.fs                # packProjects reused as the push set; staging-feed plumbing if needed

template/base/
  Directory.Packages.props        # + <FsSkiaUiVersion>, pins reference $(FsSkiaUiVersion)
  build.fsx                       # read <FsSkiaUiVersion> at runtime; drop literal #r version
  docs/UPGRADING.md               # NEW: consumer single-edit upgrade + preview/stable selection (FR-005)

src/*/<Pkg>.fsproj                # + RepositoryUrl, PackageReadmeFile metadata (FR-010), one per lib
src/*/README.md                   # NEW: per-package README ×11 (FR-010)
.template.package/
  FS.Skia.UI.Template.fsproj      # + README / metadata; version bump (FR-011)
  README.md                       # NEW: template package README (FR-010)

docs/adr/0001-...distribution.md  # UPDATE: publish path now implemented (supersede "deferred")
docs/distribution.md              # NEW: consumer install + maintainer release/publish flow (FR-009)
validation.contract.yml           # regenerated from Routing.fs (RefreshSurfaceBaselines)
specs/064-publish-nuget-distribution/readiness/**   # evidence artifacts (see Evidence paths)
```

## Phasing (implementation order, story-grouped)

The publish *machinery* and consumer config are independent of the live push; build them and validate
against staging first, then the maintainer performs the irreversible production push.

1. **P1 — fresh-consumer public feed + single-source pin (US1/US3, FR-003/004/005).** Emit public-feed-only
   `NuGet.config`; introduce `<FsSkiaUiVersion>`; make `build.fsx` read it at runtime (R1 resolution); ship
   `UPGRADING.md`; add the staging-feed overlay so `TemplateCheck` still restores. Failing-first tests for
   "no absolute local path" + "single literal version".
2. **P1 — Publish capability (US2, FR-001/002/007).** Add `Publish` target + `PublishPackages` effect:
   feed-agnostic (env feed URL + api-key), idempotent (`--skip-duplicate`), dry-run (anonymous read, 12-row
   plan, no credential). Wire into `Targets.fs` / `Routing.fs` / `knownGates` / `validation.contract.yml`.
3. **P2 — Pre-publish consistency gate (US4, FR-006/010).** `PrePublish.fs(i)` + `PrePublishCheck` target:
   pin parity, build-engine pin == lib version, no machine-local path in emitted config, required metadata
   present per package. Compose with `TemplateCheck`; abort naming offender. Add per-package READMEs + metadata.
4. **P3 — Docs + ADR (US5, FR-009).** `docs/distribution.md` + ADR 0001 update superseding "deferred".
5. **Governance green (FR-011, SC-007).** Regenerate contract + `.claude` skill tree; bump/pack/install the
   template package; run the Route-printed gates sequentially; `EvidenceAudit` PASS.
6. **FR-008 / SC-008 — production push (maintainer-gated).** After the gate is green, the maintainer supplies
   the nuget.org credential and pushes the **current `-preview` versions unchanged** (libs `0.1.67-preview.1`,
   template `0.1.86-preview.1`) on the **preview** channel; capture `production-publish.md` proving all 12
   packages publicly resolvable from a fresh consumer.

## Complexity Tracking

- **R1 (build.fsx single-source) is the principal design risk.** F# `#r` directive arguments must be
  literals, so `build.fsx` cannot interpolate `<FsSkiaUiVersion>` directly into `#r "nuget: …, <ver>"`.
  research.md selects a working technique (read the property at runtime, `#r` the resolved engine assembly
  path from the restored package) and lists alternatives; this is justified complexity in service of the
  consumer's single-edit upgrade (FR-004) and is the one place where idiomatic-simplicity (Principle III)
  yields to the directive constraint — disclosed at the use site with a one-line comment.
- Everything else reuses existing machinery (MVU effect engine, `packProjects`, target registry, routing
  rules, contract regeneration) — no new abstraction.
```
