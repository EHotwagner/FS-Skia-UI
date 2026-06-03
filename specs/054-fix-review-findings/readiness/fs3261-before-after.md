# FS3261 Before / After — Feature 054 (US2)

**Authoritative command** (clean, no incremental reuse):

```bash
dotnet build build/Governance/FS.Skia.UI.Build.fsproj --no-incremental 2>&1 \
  | grep -c 'warning FS3261'
```

## Before (failing-first baseline, FR-005 / SC-004)

Clean `--no-incremental` build emitted **88** raw `FS3261` emissions across
**33 distinct source sites** in **8 files**. (The plan's `~34`/`~N` per-file
figures were estimates from an incremental build; the authoritative clean count
is recorded here per the T004/T009 contract — resolve to 0 whatever the start.)

Raw emissions per file (clean build):

| File | raw FS3261 |
|---|---|
| build/Governance/GeneratedProduct.fs | 22 |
| build/Governance/Front/Governance.fs | 20 |
| build/Governance/Engine/Model.fs | 14 |
| build/Governance/Guidance.fs | 8 |
| build/Governance/Preflight.fs | 6 |
| build/Governance/PerPackageSurface.fs | 6 |
| build/Governance/Front/BuildProcessHealth.fs | 6 |
| build/Governance/Front/BuildProcess.fs | 6 |
| **total** | **88** |

Resolution classes (data-model NullnessSite):

- **NullableBclString** — `Path.GetFileName`/`GetDirectoryName`,
  `Environment.GetEnvironmentVariable`, etc. returning `string | null` used where
  `string` expected → `Option.ofObj |> Option.defaultValue ""` (the repo idiom,
  e.g. `Evidence/Scans.fs:135`). Behaviour-preserving: these inputs come from
  `Directory.GetFiles`/real paths and were never actually null.
- **NullableRef** — `Process.Start : Process | null` in `Front/Governance.fs`
  `routeGitCapture` → `match Process.Start startInfo with null -> Error … | p -> …`
  so it **fails fast** with an explicit `Error` (Constitution VII) instead of
  dereferencing null.
- **SignatureNullness** — `Engine/Model.fs` `featureId` inferred `string | null`
  while the `.fsi` declares `val featureId: string`. Fixed at the source
  (`activeFeatureId` return wrapped non-null); the cascade through `featureId`
  usages (lines 72/78/187/197/201) cleared with no `.fsi` change.

## After (FR-009 / SC-004 / SC-005)

- Clean `--no-incremental` build emits **0** FS3261.
- The project-local `<WarningsNotAsErrors>$(WarningsNotAsErrors);FS3261</…>` was
  **removed** from `build/Governance/FS.Skia.UI.Build.fsproj`. With
  `Nullable=enable` + repo-wide `TreatWarningsAsErrors`, any re-introduced FS3261
  is now a build **error** for this project. The repo-wide `Directory.Build.props`
  policy is unchanged.
- `Build succeeded.` with the escape hatch gone (exit 0).

Regenerable build logs: `readiness/logs/**` (gitignored).
