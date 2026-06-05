# Contract: `Publish` target

**Type**: governed FAKE target (build-engine command surface, not a runtime API).

## Invocation

```
FSSKIA_PUBLISH_DRYRUN=1 ./fake.sh build -t Publish     # plan only, no credential, no network push
FSSKIA_PUBLISH_FEED=<dir-or-url> FSSKIA_PUBLISH_API_KEY=<key> ./fake.sh build -t Publish   # real push
```

## Inputs (environment, never committed)

| Var | Default | Meaning |
|-----|---------|---------|
| `FSSKIA_PUBLISH_FEED` | `https://api.nuget.org/v3/index.json` | target feed (nuget.org URL, a private feed URL, or a local directory for staging). |
| `NUGET_API_KEY` | _(unset)_ | push credential read from the environment by `dotnet nuget push` (NuGet 7.6+); **required only for a real push**. Never placed on a command line (like `gh` + `GH_TOKEN`). |
| `FSSKIA_PUBLISH_API_KEY` | _(unset)_ | alias for the credential, forwarded to the child as `NUGET_API_KEY`. |
| `FSSKIA_PUBLISH_DRYRUN` | _(unset)_ | when set, plan only — no network push. |
| `FSSKIA_PUBLISH_GH_RELEASE_TAG` | _(unset)_ | when set on a real push, also attach the 12 `.nupkg` to the named GitHub Release (archival supplement via `gh`; nuget.org stays the consumer feed). |

## Behavior

1. Depends on `PrePublishCheck` (aborts the whole publish if inconsistent), `PackLocal`, `TemplatePack`
   (so the 12 `.nupkg` artifacts exist).
2. For each of the **12** packages (11 `packProjects` + `FS.Skia.UI.Template`): **anonymous-read** the
   target feed to decide `Push` vs `Skip` (version already present ⇒ `Skip`).
3. **Dry-run** (`FSSKIA_PUBLISH_DRYRUN`): print the 12-row plan (`PackageId | Version | feed-state |
   Push/Skip`), perform **no** push, succeed **without** a credential.
4. **Real push**: `dotnet nuget push <pkg> -s $FEED -k $APIKEY --skip-duplicate` per `Push` row;
   `--skip-duplicate` makes an already-present version a no-op success (idempotent).
5. **Missing credential on a real push**: fail **fast** with a clear message, push **nothing**.

## Guarantees

- **Idempotent**: re-running for versions already on the feed skips them with no error (SC-003);
  a partial-failure re-run pushes only the remaining missing packages.
- **Feed-agnostic**: the same target targets nuget.org, a private feed, or a local staging directory by
  `FSSKIA_PUBLISH_FEED` alone — no code change (FR-001).
- **Credential-free dry-run** and pre-publish gate (FR-002, FR-006).

## Evidence

`readiness/publish-dry-run.md` (12-row plan, no credential, no push), `readiness/publish-idempotency.md`
(push-to-staging then second-run-skips-all), `readiness/production-publish.md` (maintainer-gated live
nuget.org push, FR-008/SC-008).
