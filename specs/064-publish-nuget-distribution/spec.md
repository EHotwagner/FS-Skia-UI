# Feature Specification: Publish FS.Skia.UI to NuGet.org for Consumer Distribution

**Feature Branch**: `064-publish-nuget-distribution`
**Created**: 2026-06-04
**Status**: Ready
**Input**: User description: "what are my options to distribute the project to consumers... create specs for [option] 1" — i.e. the **Public NuGet.org** distribution path: publish the packable libraries and the `dotnet new fs-skia-ui` template to nuget.org so an external consumer can install and update without cloning the repo.

## Context & Motivation *(informative)*

Today the project is **local-feed only**. The 11 packable libraries pack via
`./fake.sh build -t PackLocal` into `~/.local/share/nuget-local`; the template
packs via `TemplatePack`. There is **no `Publish`/push target** — nothing reaches
an external consumer. ADR 0001 (`docs/adr/0001-...distribution.md`) records that a
**published NuGet package is the D1 end-state** but explicitly **defers** the
pack-and-publish step ("Distribution... is exercised only in Stage 4/5... this
feature creates the project; it does not pack or publish it"). Every consumer-facing
spec since (e.g. 060–063) has named *distribution* as out-of-scope. **This feature
is where distribution is finally tackled.**

Two concrete facts make the gap real and are the spine of this spec:

1. **The generated feed config cannot work for an external consumer.** The
   `NuGet.config` emitted into every generated project (by
   `build/Governance/GeneratedProduct.fs`) hardcodes a **machine-absolute local
   path** as its first source:
   ```xml
   <packageSources>
     <clear />
     <add key="local" value="/home/developer/.local/share/nuget-local" />
     <add key="nuget" value="https://api.nuget.org/v3/index.json" />
   </packageSources>
   ```
   A consumer who never cloned this repo has no such directory, and the `FS.Skia.UI.*`
   packages are not on nuget.org, so `dotnet restore` **fails** in a fresh checkout.
   Distribution requires the generated project to restore from a **public feed only**.

2. **There is no update path for a generated project.** Pins are baked in at
   generation time in **two** places that must agree — the library versions in
   `template/base/Directory.Packages.props` and the build-engine pin
   `#r "nuget: FS.Skia.UI.Build, <ver>"` in `template/base/build.fsx`. A consumer
   who wants a newer release must hand-edit both, in sync, with no tooling. There is
   no `npm update` equivalent.

**Versioning today**: all 11 packable libraries share one version
(`0.1.67-preview.1`, bumped together by the "Bump packable project versions" flow);
the template is versioned independently (`0.1.86-preview.1`). This feature keeps
that model — it does **not** unify the library/template version relationship — but
collapses the *duplicate pin* so a consumer upgrade is a single edit.

**Scope note.** Per the one-feature-per-`/speckit-specify` rule, this is **one**
feature delivering the NuGet.org distribution path (publish capability + a fresh
consumer's install/restore + a documented single-edit update story + the governance
wiring), not one spec per sub-task. The **private-feed** alternative (Option 2) and a
bespoke consumer-upgrade tool (Option 3+) are explicitly **out of scope** here,
though the publish capability is built feed-agnostic so a private feed is a
configuration change, not a redesign.

**Change classification.** **Tier 1 (consumer-contract change)** is expected: it
touches `template/**` (generated `NuGet.config`, `Directory.Packages.props`,
`build.fsx`, README), `build/**` governance (a new publish target + pre-publish
gate, `Targets.fs` / `knownGates` / `Routing.fs` / `validation.contract.yml`),
package metadata on every packable project, and ADR/docs. The authoritative tier and
gate list is whatever `./fake.sh build -t Route` prints for the actual diff.

## Clarifications

### Session 2026-06-04

- Q: What concretely is the "staging/test feed" all pre-production validation runs against? → A: A throwaway **local directory feed** (a temp dir distinct from `~/.local/share/nuget-local`) — credential-free, deterministic, headless.
- Q: What is the first live nuget.org release version/channel (FR-008)? → A: Push the **current `-preview` versions unchanged** (libs `0.1.67-preview.1`, template `0.1.86-preview.1`) — first public release is on the **preview** channel.
- Q: How does dry-run (no publish credential) compute its per-package push-vs-skip decision? → A: Dry-run performs an **anonymous read** of the target feed (nuget.org flat-container API / local-directory listing) to compute real skip/push decisions — no *push* credential required.
- Q: What mechanism collapses the dual pin so a consumer upgrade is one edit (FR-004)? → A: A single **MSBuild property** (e.g. `<FsSkiaUiVersion>` in `Directory.Packages.props`) that the package pins reference and that `build.fsx` reads at runtime to form its `#r` — exactly one literal version in the project.
- Q: Is the FR-010 package README shared across packages or per-package? → A: **Per-package README** — each of the 11 libraries and the template package carries its own tailored README file.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A fresh consumer installs and scaffolds entirely from a public feed (Priority: P1)

A developer who has **never cloned this repo** installs the template from the public
feed (`dotnet new install FS.Skia.UI.Template`), scaffolds a project
(`dotnet new fs-skia-ui`), and runs `dotnet restore` / `dotnet build` / `dotnet test`
**successfully** — every `FS.Skia.UI.*` package and the build engine resolve from the
public feed, with **no dependency on a machine-local `nuget-local` directory** the
consumer doesn't have.

**Independent test**: On a machine (or container) with **no** `~/.local/share/nuget-local`
and **no** repo checkout, install the published template, scaffold each profile
(`app`, `governed`), and confirm restore+build+test succeed pulling only from the
public feed. Confirm the generated `NuGet.config` contains **no** absolute local
path.

### User Story 2 - A maintainer publishes a full release with one safe command (Priority: P1)

A maintainer cuts a release by running a single publish target that pushes **all 11
packable libraries and the template package** to the configured public feed (default
nuget.org), authenticated by an API key supplied via environment/secret (never
committed). The push is **idempotent** — a version already present on the feed is
skipped, not re-pushed, and does not fail the run — and supports a **dry-run** that
reports exactly what *would* be pushed vs skipped before anything leaves the machine.

**Independent test**: Run the publish target in **dry-run** against a test/staging
feed and confirm it lists all 12 packages with the version to be pushed and the
skip/push decision per package, performs no network push, and is exercisable without
a real nuget.org credential. Run it for real against a staging feed and confirm a
second run of the same versions skips everything (idempotent).

### User Story 3 - An existing generated project upgrades with a single edit (Priority: P2)

A consumer with a project generated at an older release moves to a newer FS.Skia.UI
release by changing **one** version value and running `dotnet restore` — the library
pins **and** the build-engine `#r` pin update together (single source of truth), so
the dual-pin drift that previously required two synchronized hand-edits is gone. The
generated project ships a short **documented upgrade procedure** stating exactly which
value to change and how preview vs stable versions are selected.

**Independent test**: In a generated project, change the single documented version
value to a newer published release, run `dotnet restore`, and confirm both the
library packages and the build engine resolve at the new version with no second edit;
confirm the generated README/docs describes this procedure.

### User Story 4 - The release is verified internally consistent before any push (Priority: P2)

Before any package leaves the machine, a **pre-publish consistency check** verifies
the set is coherent: the template's pins reference the exact library versions being
shipped, the build-engine pin matches, no machine-local feed leaks into the generated
config, and every packable project + the template carry the metadata a public package
listing requires. If any check fails, the publish **aborts** naming the offending
package/field — a malformed or internally inconsistent release can never be pushed.

**Independent test**: Introduce a deliberate skew (e.g. bump a lib version without
updating the template pin, or blank a required package metadata field) and confirm the
pre-publish check fails and names the specific package/field; restore consistency and
confirm it passes.

### User Story 5 - Distribution and release flow are documented, superseding "deferred" (Priority: P3)

A maintainer or evaluating consumer finds, in-repo, documentation of the **consumer
install flow** (`dotnet new install` → `dotnet new fs-skia-ui` → restore/build), the
**feeds and preview/stable channel**, and the **maintainer release+publish flow**
(bump → pack → pre-publish check → publish). ADR 0001's "distribution deferred" note is
updated to record that the publish path is now implemented.

**Independent test**: Confirm a distribution/release doc (and the ADR update) names the
consumer install commands, the public feed, the preview-vs-stable selection, and the
maintainer publish sequence, and that no doc still describes distribution as deferred
or local-feed-only as the consumer story.

### Edge Cases

- **Already-published version**: re-running publish for a version already on the feed
  must **skip** it (no error), since nuget.org rejects duplicate versions; only a
  version bump produces a new push.
- **Missing/invalid API key**: a real push with no credential must fail **fast** with a
  clear message and push **nothing**, while dry-run and the pre-publish check run
  **without** a credential.
- **Partial-set state**: if some packages publish and a later one fails (network), the
  idempotent re-run must push only the remainder, never duplicate the succeeded ones.
- **Preview vs stable**: a consumer selecting a stable release must not silently get a
  `-preview` build and vice-versa; the documented update procedure and the feed/version
  selection must make the channel explicit (current libraries are `-preview.1`).
- **In-repo development unaffected**: the local-feed dev loop (`PackLocal` →
  `~/.local/share/nuget-local`) used to validate the template inside this repo must
  keep working; only the **generated consumer** config must drop the absolute local path.
- **Template installed but a lib version not yet on the feed**: the pre-publish check
  must catch a template whose pins point at versions not being shipped in the same
  release, so a consumer never installs a template that can't restore.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: A **publish capability** — a new FAKE target (e.g. `Publish`) — MUST push
  all 11 packable library packages **and** the `FS.Skia.UI.Template` package to a
  **configurable** NuGet feed, defaulting to **nuget.org**, authenticated by an API key
  read from environment/secret and **never committed** to the repo. The feed URL MUST
  be a parameter so the same target can target a private/staging feed without code
  change. (Primary capability; supersedes ADR 0001's deferred publish.)
- **FR-002**: Publishing MUST be **idempotent and dry-runnable**: a package version
  already present on the target feed is **skipped** (not re-pushed, not an error), and a
  **dry-run / no-push mode** MUST report the per-package push-vs-skip decision and the
  version to be pushed without performing any network push. Dry-run MUST be exercisable
  **without** a real publish credential; it computes the per-package skip/push decision by
  an **anonymous read** of the target feed (nuget.org flat-container API, or a directory
  listing for the local-directory feed), which requires no *push* credential.
- **FR-003**: Every **generated project MUST restore entirely from a public feed** with
  **no dependency on a machine-local feed path**. The generated `NuGet.config` emitted by
  `build/Governance/GeneratedProduct.fs` MUST NOT contain an absolute local path
  (`/home/.../nuget-local`); a fresh consumer with no repo checkout MUST be able to
  `dotnet restore` successfully once the packages are published. The **in-repo** local-feed
  development loop (`PackLocal` → `~/.local/share/nuget-local`, used to validate the
  template before release) MUST be preserved separately and MUST NOT be what a generated
  consumer depends on. (Closes the machine-absolute-path defect.)
- **FR-004**: The generated project's package pins and the build-engine pin MUST derive
  from a **single source of version truth** so a consumer upgrade is **one edit**: the
  existing dual pin — library versions in `template/base/Directory.Packages.props` and the
  `#r "nuget: FS.Skia.UI.Build, <ver>"` in `template/base/build.fsx` — MUST be unified to
  **one MSBuild property** (e.g. `<FsSkiaUiVersion>` defined in
  `template/base/Directory.Packages.props`) that the package pins reference **and** that
  `build.fsx` reads at runtime to construct its `#r`, so there is exactly **one literal
  version value** in the generated project and the consumer's upgrade is a single edit —
  eliminating the drift that today requires two synchronized hand-edits.
- **FR-005**: A **documented consumer update path** MUST ship **in the generated project**
  (README/docs) describing how to move an existing generated project to a newer FS.Skia.UI
  release — which single value to change, `dotnet restore`, how to verify — and how
  **preview vs stable** versions are selected. (Removes the "no upgrade path" gap.)
- **FR-006**: A **pre-publish consistency check** MUST run before any push and **abort the
  publish** on failure, naming the offending package/field. It MUST verify at minimum: the
  template's library pins reference the exact versions being shipped in this release; the
  build-engine pin matches the library version (FR-004 single-source); the generated
  `NuGet.config` carries no machine-local path (FR-003); and required package metadata
  (FR-010) is present on every package. This check SHOULD compose with / extend the
  existing `TemplateCheck` and pin-parity validation rather than duplicate them.
- **FR-007**: The publish target and pre-publish gate MUST be **wired into governance** as
  real, discoverable targets: registered in `build/Governance/Targets.fs`, added to the
  `knownGates` allowlist, classified by `Routing.fs`, and reflected in the generated
  `validation.contract.yml` so `TargetMetadataDrift` / `SkillSyncCheck` stay green and
  `./fake.sh build -t Route` classifies a distribution change correctly.
- **FR-008**: The **first live production push to nuget.org IS part of this feature's
  done-definition.** After the capability (FR-001/002), the fresh-consumer config (FR-003),
  the single-source pin (FR-004), and the pre-publish gate (FR-006) are validated against a
  staging/test feed, the maintainer MUST perform the **real push** of all 12 packages to
  **public nuget.org**, and the feature is **not done** until the published packages are
  **publicly resolvable**: a fresh consumer (no repo, no local feed) installs the template
  and the libraries from nuget.org and restores+builds successfully. Because this push is
  **irreversible** (a published version can only be unlisted, never deleted) and
  **permanently claims** the `FS.Skia.UI.*` package-ID namespace, it has explicit
  **preconditions**: (a) a nuget.org account/API key supplied via secret by the maintainer;
  (b) the pre-publish consistency gate (FR-006) green; (c) the version being pushed is the
  intended **first public release** version — the **current `-preview` versions unchanged**
  (libs `0.1.67-preview.1`, template `0.1.86-preview.1`), i.e. the first public release lands
  on the **preview** channel, and the maintainer confirms no placeholder/staging version is
  pushed. The live push step depends on the
  maintainer's credential and therefore completes **with maintainer action**, not headless
  automated validation, but its **outcome** (packages live and consumer-resolvable) is the
  acceptance bar (SC-008).
- **FR-009**: Distribution documentation MUST be authored/updated: a distribution+release
  doc (and an **ADR 0001 update**, or a successor ADR) describing the **consumer install
  flow** (`dotnet new install FS.Skia.UI.Template`; `dotnet new fs-skia-ui`; restore/build),
  the **public feed and preview/stable channel**, and the **maintainer release+publish
  flow** (bump → pack → pre-publish check → publish), **superseding** the current
  "distribution deferred" / local-feed-only narrative.
- **FR-010**: Every packable project **and** the template package MUST carry the **package
  metadata a public listing requires/recommends** — at minimum license expression, repository
  URL, authors, description, and a **per-package README** (each of the 11 libraries and the
  template package carries its own tailored README file); plus tags/icon where applicable — so
  published packages are well-formed on nuget.org. Missing **required** metadata MUST fail the
  FR-006 pre-publish check.
- **FR-011**: All consumer-contract edits MUST keep governance green: `.agents/skills/**` is
  canonical (regenerate `.claude` via `RefreshSurfaceBaselines`); the new target(s) land in
  `knownGates` and `validation.contract.yml` is regenerated so `TargetMetadataDrift` stays
  green; template/`GeneratedProduct.fs` changes keep `TemplateCheck` / `GeneratedProductCheck`
  / `TemplateDrift` green; and the template package version is bumped/packed/installed so the
  distribution changes (public feed config, single-source pin, update docs) reach generated
  consumers.

> Interacting / conflicting requirements: FR-003 (generated consumer restores from the
> **public** feed only) and the preserved **in-repo** local-feed dev loop pull in opposite
> directions — resolution: they are **two distinct configs**. The generated consumer
> `NuGet.config` references the public feed only; the repo's own template-validation loop
> keeps using `~/.local/share/nuget-local` and is never emitted into a consumer project.
> FR-001 (push to a public feed) and FR-008 (the *first live* production push) compose: the
> publish *machinery* is validated against a staging feed first, then the **real** push to
> nuget.org completes the feature (FR-008 / SC-008). The staging validation is the gate; the
> live push is the irreversible final step, performed by the maintainer with their credential
> once the pre-publish gate (FR-006) is green.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: **Package identities do not change** (same 11 `FS.Skia.UI.*` IDs +
  `FS.Skia.UI.Template`), but their **distribution** changes from local-feed-only to a
  published public feed, and their **metadata** (FR-010: license/repo/authors/description/
  README) changes. The template package contents change (generated `NuGet.config` → public
  feed, single-source pin, update docs) and the template version is bumped/packed/installed.
  Generated package **consumers** change: they now restore from nuget.org and upgrade via a
  single edit. No legacy Charts-package migration is involved.
- **Public contract impact**: No runtime `.fsi` signatures change. The **public surface
  that changes is the *distribution* contract** — the generated `NuGet.config`, the
  single-source version pin in `Directory.Packages.props` + `build.fsx`, the package
  metadata, and the documented install/update flow. New build **targets** (publish,
  pre-publish gate) are added to the governance/Route surface, not the runtime API surface.
- **State workflow impact**: None. No interpreter, effects, commands, subscriptions, or
  runtime model behavior changes; this is packaging, feed configuration, and release tooling.
- **Layout/rendering impact**: None. No layout, charts, DataGrid, rendering, screenshot,
  Vulkan, Skia, or visual-output change; no unsupported-environment diagnostic change.
- **Evidence obligations**: Real evidence under
  `specs/064-publish-nuget-distribution/readiness/` — at minimum the Route-required
  escalated-tier artifacts (target-metadata, agent-ready verdict, skill-loading-evidence,
  aggregate-hang-diagnostics), plus a **fresh-consumer restore proof** that a project
  generated with the new template restores+builds+tests from a public/staging feed with
  **no** machine-local path (US1), a **publish dry-run transcript** showing the per-package
  push/skip plan over all 12 packages (US2), a **pre-publish-check failure+pass transcript**
  for a deliberately-skewed set (US4), and a **single-edit upgrade proof** (US3). For FR-008,
  a **production-publish proof**: the live push transcript and a fresh-consumer restore+build
  against **nuget.org** (not staging) confirming all 12 packages are publicly resolvable (SC-008).
- **Unsupported scope**: The **private-feed** distribution option (Option 2) and a bespoke
  automated consumer-upgrade tool/`dotnet tool` (beyond the single-edit + docs) are out of
  scope. Floating/wildcard version ranges in generated projects are out of scope (pins stay
  exact; upgrade stays an explicit edit). Unifying the library and template **version
  relationship** is out of scope (the single-version-libs + independent-template model is
  kept). SourceLink / symbol packages (`.snupkg`) are out of scope unless planning finds them
  low-cost. The irreversible **first production push to nuget.org** is **in scope** (FR-008,
  SC-008) but depends on the maintainer's nuget.org credential and is the final, manually
  triggered step after the staging-validated gate is green.
- **Build-target impact**: A new **`Publish`** target and a **pre-publish consistency**
  target/check are added to `build/Governance/Targets.fs` (FR-001/006/007), with `knownGates`
  / `Routing.fs` / `validation.contract.yml` updated (FR-011). `PackLocal` / `TemplatePack`
  may be referenced/composed by publish but their local-feed behavior is unchanged.
  `TemplateCheck` / `GeneratedProductCheck` / `TemplateDrift` change for the generated
  `NuGet.config` + single-source pin + docs. `TargetMetadataDrift` / `SkillSyncCheck` must
  stay green after regeneration. The authoritative gate list is whatever
  `./fake.sh build -t Route` prints.

## Success Criteria *(mandatory)*

- **SC-001**: On a host with **no** `~/.local/share/nuget-local` and **no** repo checkout,
  installing the template and scaffolding the `app` and `governed` profiles yields projects
  that `dotnet restore` + `dotnet build` + `dotnet test` **successfully** from a public/staging
  feed, and the generated `NuGet.config` contains **no absolute local path**. (US1, FR-003)
- **SC-002**: A single `Publish` invocation pushes all **12** packages (11 libs + template),
  and a **dry-run** of the same invocation lists every package with its version and push/skip
  decision, performs **no** network push, and runs **without** a publish credential. (US2,
  FR-001/002)
- **SC-003**: Re-running publish for versions already on the target feed **skips** them with no
  error (idempotent), and a partial-failure re-run pushes only the remainder. (US2 edge cases,
  FR-002)
- **SC-004**: In a generated project, changing the **one** documented version value and running
  `dotnet restore` upgrades **both** the library packages and the build engine to the new
  version with **no second edit**, and the generated docs describe this procedure including
  preview-vs-stable selection. (US3, FR-004/005)
- **SC-005**: The **pre-publish consistency check** fails — naming the offending package/field —
  for a deliberately skewed release (template pin ≠ shipped lib version, OR a blank required
  metadata field, OR a machine-local path in the generated config), and **passes** once
  consistency is restored; the publish **cannot proceed** while it fails. (US4, FR-006/010)
- **SC-006**: A distribution/release doc and the **ADR 0001 update** name the consumer install
  commands, the public feed, the preview/stable channel, and the maintainer publish sequence;
  **no** in-repo doc still presents distribution as deferred or local-feed-only as the consumer
  story. (US5, FR-009)
- **SC-007**: All Route-printed gates for this change pass — including the new publish /
  pre-publish targets wired into `knownGates` / `validation.contract.yml`, `TargetMetadataDrift`
  / `SkillSyncCheck` green after regeneration, and `TemplateCheck` green on the new generated
  config + single-source pin. `GeneratedProductCheck`'s aggregate is **non-authoritative** (a
  feature-less scaffold legitimately reports an expected-fail non-regression); the authoritative
  release verdict is `EvidenceAudit verdict=PASS` for `specs/064-publish-nuget-distribution`,
  which SC-007 requires. (FR-007/011)
- **SC-008**: The first release of all **12** packages is **live on public nuget.org** and
  **publicly resolvable**: a fresh consumer with **no** repo checkout and **no** local feed
  runs `dotnet new install FS.Skia.UI.Template` + `dotnet new fs-skia-ui` + `dotnet restore`
  + `dotnet build` against **nuget.org** and succeeds, the packages appear in nuget.org search
  with complete metadata (FR-010), and the pushed version is the intended first public release
  on the confirmed **preview** channel (libs `0.1.67-preview.1`, template `0.1.86-preview.1`,
  unchanged). (US1 against production, FR-008)

## Assumptions

- "Create specs for **1**" refers to **Option 1 (Public NuGet.org)** from the prior
  distribution discussion — the standard `dotnet new install` + nuget.org path — not the
  private-feed (Option 2) or local-only (Option 3) alternatives, which are out of scope.
- The **single-version-libraries + independently-versioned-template** model is kept as-is
  (libs `0.1.67-preview.1`, template `0.1.86-preview.1`); this feature collapses the *duplicate
  pin* for a one-edit consumer upgrade but does **not** unify the lib/template version
  relationship or introduce a centralized version file beyond what single-source requires.
- The publish target is built **feed-agnostic** (parameterized feed URL + API-key env var) with
  nuget.org as the default, so a private/staging feed is a configuration change. All capability
  validation (dry-run, idempotency, pre-publish gate, fresh-consumer restore) is exercisable
  against a **staging/test feed** without a production nuget.org credential; the concrete
  staging feed for in-repo validation is a **throwaway local directory feed** (a temp dir
  distinct from `~/.local/share/nuget-local`), credential-free and deterministic.
- **The first live production push to nuget.org is in scope (FR-008, SC-008).** A real
  nuget.org **account, API key, and the permanent `FS.Skia.UI.*` package-ID-namespace claim**
  must be provided by the maintainer and cannot be created/secured from headless automation; the
  push is therefore the feature's **final, maintainer-triggered** step, gated behind the
  staging-validated pre-publish check (FR-006). It is **irreversible** — a published version can
  only be unlisted, never deleted — so the maintainer confirms the intended first-release version
  before pushing. The intended first release is the **current `-preview` versions unchanged**
  (libs `0.1.67-preview.1`, template `0.1.86-preview.1`) on the **preview** channel. Everything upstream of the push (capability, dry-run,
  idempotency, fresh-consumer restore, gate) is validated against a staging/local feed first; the
  acceptance bar is the **published** packages being publicly resolvable (SC-008).
- Generated projects keep **exact** version pins (no floating ranges); upgrading stays an
  explicit, documented single edit — chosen for reproducibility over auto-update.
- The local-feed development loop (`PackLocal` → `~/.local/share/nuget-local`) remains the way
  the template is validated **inside this repo**; only the **consumer-emitted** `NuGet.config`
  drops the local path. The two configs are independent.
- ADR 0001 explicitly deferred publishing as the "D1 end-state... exercised only in Stage 4/5";
  this feature is the realization of that deferred step, so updating (not contradicting) ADR 0001
  is the correct governance move.
- Package metadata (FR-010) targets a well-formed **public** listing; SourceLink and symbol
  packages (`.snupkg`) are a recommended-but-optional enhancement left to planning unless
  low-cost.
- This is **one** consolidated feature (per the one-feature-per-`/speckit-specify` rule), not one
  spec per sub-task.
