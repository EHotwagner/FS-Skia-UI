# Production publish — maintainer-gated (FR-008, SC-008)

status: **pending maintainer action** — this task (T041) legitimately remains `[ ]` after
`EvidenceAudit verdict=PASS`. The audit gates on `[S]`/`[S*]`/diff-scan, **not** pending
maintainer steps; a green audit is therefore not by itself "feature complete" per SC-008.

The first live push to public **nuget.org** is **irreversible** (a published version can only
be unlisted, never deleted) and **permanently claims** the `FS.Skia.UI.*` package-ID namespace.
It depends on the maintainer's nuget.org account + API key, which cannot be created or secured
from headless automation — so it is the maintainer's final, manually-triggered step once the
staging-validated gate is green (it is).

## Preconditions (all met)

- [x] Publish capability validated against a staging feed — dry-run + idempotency
      ([publish-dry-run.md](./publish-dry-run.md), [publish-idempotency.md](./publish-idempotency.md)).
- [x] Fresh-consumer config is public-feed only ([fresh-consumer-restore.md](./fresh-consumer-restore.md)).
- [x] Single-source pin ([single-edit-upgrade.md](./single-edit-upgrade.md)).
- [x] Pre-publish gate green ([prepublish-check.md](./prepublish-check.md)).
- [x] `EvidenceAudit verdict=PASS` ([evidence-audit.md](./evidence-audit.md)).
- [ ] nuget.org account + API key supplied by the maintainer (secret; not in repo).

## Intended first release (current `-preview` versions, unchanged)

- 11 libraries: `0.1.67-preview.1`
- `FS.Skia.UI.Template`: `0.1.86-preview.1`
- Channel: **preview** (the `-preview.N` suffix is explicit).

## Maintainer runbook

```bash
# 1. pack the artifacts (current versions, unchanged)
./fake.sh build -t PackLocal
./fake.sh build -t TemplatePack

# 2. pre-publish consistency gate (must be green)
./fake.sh build -t PrePublishCheck

# 3. live push to nuget.org — credential via the NUGET_API_KEY env var (never on a command
#    line, like `gh` + GH_TOKEN); idempotent. (FSSKIA_PUBLISH_FEED defaults to nuget.org.)
NUGET_API_KEY=<nuget.org-key> ./fake.sh build -t Publish

# 3b. (optional) also archive the 12 .nupkg on a GitHub Release (uses ambient `gh auth`):
NUGET_API_KEY=<nuget.org-key> FSSKIA_PUBLISH_GH_RELEASE_TAG=v0.1.67-preview.1 \
  ./fake.sh build -t Publish
#    GitHub Releases is archival only — consumers still install from nuget.org.
```

## Acceptance bar (SC-008) — to capture here after the push

A fresh consumer with **no** repo checkout and **no** local feed:

```bash
dotnet new install FS.Skia.UI.Template      # from nuget.org
dotnet new fs-skia-ui -o MyApp
cd MyApp && dotnet restore && dotnet build   # all 12 packages resolve from nuget.org
```

…succeeds, and all 12 packages appear in nuget.org search with complete metadata (FR-010).
Capture the live push transcript + the fresh-consumer restore-against-nuget.org transcript here.
