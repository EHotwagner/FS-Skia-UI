# Agent-ready verdict — feature 064 (publish NuGet distribution)

## Feature classification

- **Tier**: Tier 1 (distribution / consumer-contract change). The authoritative tier +
  gate list is whatever `./fake.sh build -t Route` prints for the actual diff; with the
  new `distribution` routing rule (T007) the change-set escalates to the
  **maintainer-verify** serialized order.
- **Affected layers**:
  - `build/Governance/**` — new `Publish` + `PrePublishCheck` targets (`Targets.fs`),
    `PrePublish.fs(i)` validator, publish effects (`Engine/Model.fs(i)`,
    `Update.fs(i)`, `Interpret.fs(i)`), distribution routing rule (`Routing.fs`),
    `knownGates` (`AgentValidation.fs`), public-feed `writeLocalNuGetConfig`
    (`GeneratedProduct.fs`).
  - `template/base/**` — single-source `<FsSkiaUiVersion>` in `Directory.Packages.props`,
    `build.fsx` runtime version read, `docs/UPGRADING.md`.
  - `src/*/*.fsproj` + `src/*/README.md` — FR-010 package metadata + per-package READMEs.
  - `.template.package/**` — template package README + metadata + version bump.
  - `docs/**` — `docs/distribution.md`, ADR 0001 update.

## Public-API / contract impact

- **No runtime `.fsi` signature change.** Principle II is satisfied: new governance public
  modules (`PrePublish.fsi`, publish effect/case additions to the Engine `.fsi` files) each
  carry a curated `.fsi`. These are internal to `FS.Skia.UI.Build`, not new packable runtime
  surface, so `PerPackageSurfaceDiff` / `PackageSurfaceCheck` baselines are unaffected.
- The changed public surface is the **distribution contract** — recorded in `contracts/`.

## Elmish/MVU applicability (Principle IV)

The build front-end is the MVU boundary (`Engine/Model.fs` / `Update.fs` / `Interpret.fs`).
`Publish` / `PrePublishCheck` are new `Msg` (`StartTarget`) handled by a **pure** `update`
that emits `PublishPackages` / `PrePublishValidate` effects; the **interpreter** performs the
anonymous feed read, the skip/push decision, and `dotnet nuget push --skip-duplicate`.
`PublishConfig` is read from the environment at the interpreter edge. Real interpreter
evidence = the dry-run / idempotency staging-feed transcripts.

## Required evidence obligations

- Route escalated-tier set: `target-metadata.md`, `agent-ready-verdict.md`,
  `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`.
- `fresh-consumer-restore.md` (US1, SC-001), `publish-dry-run.md` (US2, SC-002),
  `publish-idempotency.md` (US2, SC-003), `single-edit-upgrade.md` (US3, SC-004),
  `prepublish-check.md` (US4, SC-005), `validation-contract.md` / `evidence-graph.md` /
  `evidence-audit.md` (governance currency, SC-007), `production-publish.md`
  (maintainer-gated, FR-008/SC-008).

## Synthetic evidence

None anticipated. The staging feed is a real throwaway local-directory feed exercised with
real `dotnet pack` / anonymous read / `dotnet nuget push --skip-duplicate`; the pre-publish
fail case is a real negative test over deliberately-skewed real files; the production push
is a real maintainer step.
