# Target metadata — feature 064 (publish NuGet distribution)

## Route classification (T001)

`./fake.sh build -t Route` against the working-tree diff. Route reads the union of
the branch-vs-`main` merge-base diff and the uncommitted/untracked changes and prints
the authoritative tier + minimal gate list.

**Initial run (spec/readiness only, before code/template edits):**

```
developer-class=framework-author
tier=agent-ready
gates=Dev, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only
```

**Authoritative run (after the full change-set lands — recorded at T038):** see the
[T038 re-run](#t038-authoritative-route-re-run) below. The change-set adds the new
`distribution` routing rule (T007) classifying `build/Governance/**` publish targets +
`template/base/build.fsx` + `Directory.Packages.props` to the escalated
**maintainer-verify** path, so the gate list expands to the serialized order:
`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
`EvidenceGraph`, `EvidenceAudit`.

## Feature tier + obligations

- **Tier**: Tier 1 (distribution / consumer-contract change). No runtime `.fsi` change.
- **Affected layers**: `build/Governance/**` (new `Publish`/`PrePublishCheck` targets,
  `PrePublish.fs(i)`, publish effects, routing rule, knownGates), `template/base/**`
  (single-source `<FsSkiaUiVersion>`, `build.fsx` runtime read, `UPGRADING.md`),
  `src/*/*.fsproj` + `src/*/README.md` (FR-010 metadata), `.template.package/**`,
  `docs/**`.
- **Public-API impact**: none on the runtime `.fsi` surface; the changed public contract
  is the *distribution* contract (consumer `NuGet.config`, single-source pin, package
  metadata, install/update docs).

## Surface disposition (T009)

The new governance `.fsi` files (`build/Governance/PrePublish.fsi`, the publish additions
to `Engine/Model.fsi` / `Update.fsi` / `Interpret.fsi`) are **internal to
`FS.Skia.UI.Build`** — `FS.Skia.UI.Build` packs as a governance engine, but these modules
are part of the build front-end consumed in-process, not new public *runtime* surface of
the packable game libraries. `PerPackageSurfaceDiff` / `PackageSurfaceCheck` baselines
(driven by `src/**/*.fsi` of the nine surface-baselined packages) are **unaffected**: no
`src/**/*.fsi` changed. The packable-set metadata (FR-010 `.fsproj` `RepositoryUrl` /
`PackageReadmeFile` + READMEs) changes packaging inputs, not type surface.

## T038 — Route-printed gates run sequentially (non-authoritative aggregate)

The full change-set adds the `distribution` routing rule (maintainer-verify), so the
serialized gate order applies. Run individually and sequentially (shared `.fake` state):

| Gate | Result |
|------|--------|
| `Dev` | **Ok** (full test suite incl. 463 Governance.Tests + the new Feature064PublishTests, green) |
| `GeneratedGuidanceCheck` | **Ok** |
| `TemplateDrift` | regenerated current via RefreshSurfaceBaselines |
| `TargetMetadataDrift` / `SkillSyncCheck` | **Ok** (contract + skill tree current with the 2 new targets) |
| `TemplateCheck` | **Ok** (app/headless/governed/sample-pack generated + built + tested) |
| `GeneratedProductCheck` | **expected-fail / non-authoritative** — `app/source generated Verify` fails because a freshly scaffolded product has no feature, so the in-process EvidenceGraph correctly fails loud (feature 059 feature-less-scaffold non-regression; the engine reflection binding worked end-to-end). |

Per SC-007 the `GeneratedProductCheck` aggregate is **non-authoritative**; the authoritative
release verdict is **`EvidenceAudit verdict=PASS`** for `specs/064-publish-nuget-distribution`
(see [evidence-audit.md](./evidence-audit.md)).
