---
title: Distribution & release
category: Guides
categoryindex: 8
---

# Distribution & release

FS.Skia.UI is distributed as **public NuGet.org packages** — 11 `FS.Skia.UI.*` libraries
plus the `FS.Skia.UI.Template` project template. An external developer consumes it without
cloning this repository. (This supersedes the "distribution deferred / local-feed-only"
narrative; see [ADR 0001](./adr/0001-governance-library-placement-and-distribution.md).)

## Consumer install flow

```bash
dotnet new install FS.Skia.UI.Template      # from nuget.org, no repo clone
dotnet new fs-skia-ui -o MyApp              # profiles: app, headless-scene, governed, sample-pack
cd MyApp
dotnet restore                              # resolves FS.Skia.UI.* from nuget.org only
dotnet build
dotnet test
```

The generated `NuGet.config` references the **public nuget.org feed only** — no machine-local
path — so a fresh checkout restores on any machine.

## Feeds and the preview/stable channel

- **Public feed**: `https://api.nuget.org/v3/index.json`.
- **Channel is explicit in the version value**: `-preview.N` (or `-rc.N`) ⇒ preview;
  a bare `MAJOR.MINOR.PATCH` ⇒ stable. The packages are published on the **preview** channel
  (libraries `0.1.68-preview.1`, template `0.1.87-preview.1`).
- A **private** or **staging** feed is a configuration change only — add it to `NuGet.config`,
  or for the publish target set `FSSKIA_PUBLISH_FEED`. No code change.

## Consumer upgrade — one edit

Every generated project pins all `FS.Skia.UI.*` packages **and** the in-process build engine
to a single `<FsSkiaUiVersion>` in `Directory.Packages.props`. Change that one value, run
`dotnet restore`, and libraries + engine move together. Full procedure ships in the generated
project at `docs/UPGRADING.md`.

## Maintainer release + publish flow

### Recommended: CI publish via GitHub Actions trusted publishing

The production push runs in CI with **no stored API key**, using NuGet
[Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
(GitHub OIDC). `.github/workflows/publish.yml` exchanges the job's signed OIDC token for a
short-lived (~1 h, single-use) nuget.org key via `NuGet/login@v1`, exposed as `NUGET_API_KEY`
— the exact env var the `Publish` target already reads, so there is **zero** publish-specific
code and nothing to leak or rotate.

1. **Bump** the version (`<FsSkiaUiVersion>` for the libs; the template version independently)
   and merge to `main`.
2. **Actions → "Publish to nuget.org" → Run workflow** (manual `workflow_dispatch` only).
3. The job pauses at the protected **`release`** environment for **maintainer approval**, then
   runs the full pre-publish gate (`PrePublishCheck` → `PackLocal`/`TemplatePack`) and pushes
   all 12 packages with `--skip-duplicate`.

One-time setup: a nuget.org Trusted Publishing policy (owner / repo / workflow `publish.yml` /
env `release`), the `release` environment with required reviewers, and `user: <nuget profile>`
in the workflow. The CI runner provisions a headless graphics stack (Xvfb + Mesa lavapipe +
GLFW libs) so the Silk.NET/Vulkan pre-publish GUI suite runs without a display or GPU.

### Manual / local fallback

```bash
# 1. bump the version (libs share one version; the template is versioned independently)
#    then pack the artifacts
./fake.sh build -t PackLocal        # 11 libraries -> ~/.local/share/nuget-local
./fake.sh build -t TemplatePack     # FS.Skia.UI.Template -> artifacts/templates

# 2. dry-run the publish plan — no credential, no network push
FSSKIA_PUBLISH_DRYRUN=1 ./fake.sh build -t Publish
#    -> 12 rows (11 libs + template): PackageId | Version | feed-state | Push/Skip

# 3. pre-publish consistency gate — aborts naming the offender on any skew
./fake.sh build -t PrePublishCheck

# 4. real push — credential from the NUGET_API_KEY environment variable (like `gh` reads
#    GH_TOKEN); the key is never placed on a command line. Idempotent via the pre-read + --skip-duplicate.
NUGET_API_KEY=<key> ./fake.sh build -t Publish
```

The credential follows the same **ambient-environment** model as `gh`: `dotnet nuget push`
reads `NUGET_API_KEY` (NuGet 7.6+, .NET SDK 10.0.300+), so the key is **never** passed as a
command-line argument — there is nothing to leak. (`FSSKIA_PUBLISH_API_KEY` is accepted as an
alias and is likewise forwarded only via the environment.)

- **Idempotent**: re-running for versions already on the feed **skips** them (no error); a
  partial-failure re-run pushes only the remainder.
- **Pre-publish gate**: verifies the template's pins reference the shipped versions, the
  build-engine pin matches (single-source), the consumer config carries no machine-local path,
  and every package + the template carry license / repository URL / authors / description /
  README. A malformed or inconsistent release **cannot** be pushed.
- **First production push is irreversible**: a published version can only be unlisted, never
  deleted, and the push permanently claims the `FS.Skia.UI.*` package-ID namespace. It is the
  maintainer's final step, gated behind a green `PrePublishCheck`.

## Publish target reference

| Env var | Default | Meaning |
|---------|---------|---------|
| `FSSKIA_PUBLISH_FEED` | `https://api.nuget.org/v3/index.json` | target feed (nuget.org URL, a private feed, or a local directory for staging). |
| `NUGET_API_KEY` | _(unset)_ | push credential, read from the environment by `dotnet nuget push` (NuGet 7.6+); required **only** for a real push, never for dry-run. Never placed on a command line. |
| `FSSKIA_PUBLISH_API_KEY` | _(unset)_ | alias for the credential (forwarded to the child as `NUGET_API_KEY`); use `NUGET_API_KEY` directly when possible. |
| `FSSKIA_PUBLISH_DRYRUN` | _(unset)_ | when set, plan only — no network push. |
| `FSSKIA_PUBLISH_GH_RELEASE_TAG` | _(unset)_ | when set on a real push, also attach the 12 `.nupkg` to the named **GitHub Release** as an archival supplement (uses the ambient `gh auth` token). |

The publish machinery is validated against a throwaway **local-directory staging feed**
(credential-free) before any live nuget.org push.

**Credential safety:** the push credential uses the same **ambient-environment** model as `gh`
(`gh` reads `GH_TOKEN`; `dotnet nuget push` reads `NUGET_API_KEY`), so the key is **never** on a
command line — nothing to leak. As belt-and-braces, captured output is also redacted before it
is written to any log. The `gh` release upload likewise authenticates via the stored `gh auth`
token.

## GitHub Release artifacts (optional supplement)

Setting `FSSKIA_PUBLISH_GH_RELEASE_TAG=<tag>` on a real push additionally uploads the 12
`.nupkg` to a GitHub Release (`gh release create`/`upload --clobber`). This is **archival
only** — GitHub Releases is **not** a NuGet feed, so consumers still install/restore from
**nuget.org** (and GitHub Packages is excluded because it requires authentication even to read
public packages, breaking the no-setup consumer restore). The GitHub upload is best-effort: a
`gh` failure is logged but never fails the (already-completed) nuget.org push.
