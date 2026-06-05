# Quickstart: NuGet.org distribution

Three audiences: a **fresh consumer** installing, a **consumer upgrading**, and a **maintainer** cutting a
release.

## Fresh consumer — install & scaffold from the public feed (US1)

```bash
dotnet new install FS.Skia.UI.Template          # from nuget.org, no repo clone
dotnet new fs-skia-ui -o MyGame                 # scaffold (app / governed / … profile)
cd MyGame
dotnet restore                                  # resolves FS.Skia.UI.* from nuget.org only
dotnet build
dotnet test
```
The generated `NuGet.config` references **nuget.org only** — no machine-local feed path. Works on a host
with **no** `~/.local/share/nuget-local` and **no** repo checkout (SC-001).

## Consumer — upgrade with one edit (US3)

In the generated project, edit the **single** version value in `Directory.Packages.props`:
```xml
<FsSkiaUiVersion>0.1.68-preview.1</FsSkiaUiVersion>   <!-- the only FS.Skia.UI version literal -->
```
Then:
```bash
dotnet restore        # libraries AND the build engine move to the new version together
```
- **Preview vs stable** is explicit in the value (`-preview.N` ⇒ preview channel; a bare `x.y.z` ⇒ stable).
  Choose a published version on the channel you want; you never silently cross channels.
- Pins stay **exact** — no floating ranges; the upgrade is a deliberate single edit (see
  `docs/UPGRADING.md` shipped in the project).

## Maintainer — cut a release (US2/US4/US5)

```bash
# 1. bump + pack (libs share one version; template versioned independently)
./fake.sh build -t PackLocal
./fake.sh build -t TemplatePack

# 2. dry-run the publish plan — no credential, no network push
FSSKIA_PUBLISH_DRYRUN=1 ./fake.sh build -t Publish
#   -> prints 12 rows (11 libs + template): PackageId | Version | feed-state | Push/Skip

# 3. pre-publish consistency gate (aborts naming the offender on any skew)
./fake.sh build -t PrePublishCheck

# 4. real push (maintainer credential; idempotent via --skip-duplicate)
FSSKIA_PUBLISH_API_KEY=<key> ./fake.sh build -t Publish
```
Re-running step 4 for versions already on the feed **skips** them (no error). To publish to a **staging**
feed instead of nuget.org, set `FSSKIA_PUBLISH_FEED=/tmp/staging-feed` — same target, no code change.

## Validate in-repo (before any live push)

The publish machinery is validated against a **throwaway local-directory staging feed** with no credential:
dry-run, idempotency, the pre-publish fail+pass, and a fresh-consumer restore all run headless. The
irreversible **first production push to nuget.org** (FR-008/SC-008) is the maintainer's final step once the
gate is green — it permanently claims the `FS.Skia.UI.*` package IDs and lands the current `-preview`
versions (libs `0.1.67-preview.1`, template `0.1.86-preview.1`) on the **preview** channel.
