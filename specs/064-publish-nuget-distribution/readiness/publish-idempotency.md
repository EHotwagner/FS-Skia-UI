# Publish idempotency (US2, SC-003)

A **real** push to a throwaway local-directory staging feed, then a re-run that **skips** all
12 packages, plus a **partial-set** re-run that pushes only the missing remainder. Real
`dotnet nuget push --skip-duplicate` over real `.nupkg` artifacts; no nuget.org credential
(the local-directory feed ignores the key).

## Round 1 — real push to an empty staging feed

```bash
rm -rf /tmp/staging-feed-064 && mkdir -p /tmp/staging-feed-064
FSSKIA_PUBLISH_FEED=/tmp/staging-feed-064 FSSKIA_PUBLISH_API_KEY=dummy-local-key \
  dotnet fsi -e 'FS.Skia.UI.Build.Engine.Interpret.runTarget FS.Skia.UI.Build.Targets.Publish'
```

Plan: `packages: 12 (push 12, skip 0)` — every row `absent → Push`. After the run the staging
feed holds all 12 `.nupkg` files (FS.Skia.UI.Scene/SkiaViewer/Elmish/KeyboardInput/Input/
Controls.Elmish/Testing/Layout/Controls/Build/SkillSupport @ 0.1.67-preview.1 + Template @
0.1.86-preview.1).

## Round 2 — re-run skips all 12 (idempotent)

Same command again. The anonymous read finds every version present ⇒ `packages: 12 (push 0,
skip 12)`; nothing is pushed:

```
publish: skip FS.Skia.UI.Scene@0.1.67-preview.1 (already on the feed)
publish: skip FS.Skia.UI.SkiaViewer@0.1.67-preview.1 (already on the feed)
... (all 12 skipped) ...
publish: skip FS.Skia.UI.Template@0.1.86-preview.1 (already on the feed)
```

## Partial-set — re-run pushes only the missing remainder

Delete two packages from the feed (`FS.Skia.UI.Scene`, `FS.Skia.UI.Template`) and re-run:

```
- packages: 12 (push 2, skip 10)
| `FS.Skia.UI.Scene`     | `0.1.67-preview.1` | absent  | Push |
| `FS.Skia.UI.SkiaViewer`| `0.1.67-preview.1` | present | Skip |
| ... (8 more) ...       |                    | present | Skip |
| `FS.Skia.UI.Template`  | `0.1.86-preview.1` | absent  | Push |
```

## Verdict

- Re-running for versions already on the feed **skips** them with no error (SC-003).
- A partial-failure re-run pushes **only** the remaining missing packages, never duplicating
  the succeeded ones (SC-003 edge).
- The push edge is `dotnet nuget push <pkg> -s <feed> -k <key> --skip-duplicate`; the skip
  decision is computed by the credential-free anonymous read **before** the push.
