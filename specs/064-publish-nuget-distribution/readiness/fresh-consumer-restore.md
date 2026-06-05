# Fresh-consumer restore (US1, SC-001)

A project generated from the new template restores + builds + tests with the **public-feed-only**
`NuGet.config` (no machine-local path), with the local feed supplied only by a **separate
validation overlay** during in-repo validation — never in the shipped consumer config.

## Public-feed-only consumer config (SC-001)

`GeneratedProduct.consumerNuGetConfigContent` now emits:

```xml
<configuration>
  <packageSources>
    <clear />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
```

No `nuget-local`, no `key="local"`, no absolute path — asserted by the failing-first test
`Feature064PublishTests.T010` (now green).

## In-repo validation overlay (FR-003 conflict resolution)

`runGeneratedConsumerValidation` ships the public-only `NuGet.config` into the generated
project **and** writes a separate `nuget.validation.config` (local feed + nuget.org), then
restores `Product.Tests` with `--configfile nuget.validation.config`. The local path never
leaks into the consumer config. TemplateCheck's instantiated projects (which carry no
`NuGet.config`) restore `FS.Skia.UI.*` from the user-level local feed — the in-repo dev loop.

## TemplateCheck — generate + restore + build + test (all profiles) — PASS

```
./fake.sh build -t TemplateCheck   -> Status: Ok
# verdict.md: PASS: source/package V3 app, headless-scene, governed, and sample-pack
#             generated projects passed non-visual validation.
```

The `app` and `governed` profiles (source and package install) were generated, restored,
built, and Dev-validated against the local/staging feed with the public-feed-only consumer
config — proving a fresh consumer restores without a machine-local path.

## Single-source engine binding works in generated projects

The generated `build.fsx` resolves `FS.Skia.UI.Build` from `<FsSkiaUiVersion>` at runtime
(restore + `Assembly.LoadFrom` + an `AssemblyResolve` over the package cache for the dependency
closure) and reflection-invokes the engine's `GeneratedRunner`. This was exercised end-to-end:
in `GeneratedProductCheck`, `GeneratedRunner.runGeneratedEvidenceGraph` was successfully invoked
in a generated product (it restored the engine, resolved its deps, and ran).

## GeneratedProductCheck aggregate — non-authoritative (SC-007)

`GeneratedProductCheck`'s `app/source generated Verify` step **expected-fails**: a freshly
scaffolded product has **no feature** (`/.specify/feature.json` carries no `feature_directory`
until `/speckit.specify` runs), so the generated EvidenceGraph correctly **fails loud** — "no
SPECKIT_FEATURE_DIR override is set and …/feature.json has no usable feature_directory entry …
Validation never falls back to a bundled sample." This is the documented feature-less-scaffold
non-regression (feature 059); the engine binding worked (it reached and surfaced the loud
failure). The authoritative release verdict is `EvidenceAudit verdict=PASS` (see
[evidence-audit.md](./evidence-audit.md)).
