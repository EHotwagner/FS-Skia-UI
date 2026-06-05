# Publish dry-run (US2, SC-002)

The `Publish` target run in **dry-run** against a throwaway local-directory staging feed with
**no credential** and **no network push** — it performs an anonymous read of the feed and
prints the per-package push/skip plan over all **12** packages (11 `packProjects` libs +
`FS.Skia.UI.Template`).

## Command

```bash
FSSKIA_PUBLISH_FEED=/tmp/staging-feed-064 FSSKIA_PUBLISH_DRYRUN=1 \
  dotnet fsi -e 'FS.Skia.UI.Build.Engine.Interpret.runTarget FS.Skia.UI.Build.Targets.Publish'
# (the production invocation is `FSSKIA_PUBLISH_DRYRUN=1 ./fake.sh build -t Publish`)
```

No `FSSKIA_PUBLISH_API_KEY` is set — dry-run never needs a push credential (FR-002). The
12-row plan is computed by an anonymous read (directory listing for a local feed; nuget.org
flat-container `index.json` for the public feed).

## Transcript (empty staging feed ⇒ all 12 Push)

```
# Publish plan

- feed: `/tmp/staging-feed-064`
- read-url: `/tmp/staging-feed-064`
- mode: dry-run (no push)
- api-key-present: false
- local-feed: true
- packages: 12 (push 12, skip 0)

| PackageId | Version | feed-state | Decision |
|-----------|---------|------------|----------|
| `FS.Skia.UI.Scene` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.SkiaViewer` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Elmish` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.KeyboardInput` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Input` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Controls.Elmish` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Testing` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Layout` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Controls` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Build` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.SkillSupport` | `0.1.67-preview.1` | absent | Push |
| `FS.Skia.UI.Template` | `0.1.86-preview.1` | absent | Push |

publish: dry-run — no network push performed (credential not required).
```

## Verdict

- **Exactly 12 rows** (11 libs from `packProjects` + the template) — SC-002 invariant.
- Each row carries its version and a push/skip decision.
- **No network push** and **no credential** — dry-run is credential-free (FR-002).
- The MVU boundary is exercised end-to-end: the pure `update (StartTarget Publish)` emits the
  `PublishPackages` effect (asserted by Feature064PublishTests T014), and the interpreter
  performed the anonymous read + plan render at the edge.
