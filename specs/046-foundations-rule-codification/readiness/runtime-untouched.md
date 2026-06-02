# SC-009 standing-invariants proof (T023, FR-014)

Authored summary (kept at the readiness root so it stays tracked; raw gate `.log` files
live under the now-gitignored `readiness/logs/`).

| Invariant | Check | Result |
|-----------|-------|--------|
| Product runtime / `.fsi` untouched | `git diff --name-only -- 'src/**' \| wc -l` | **0** |
| No product surface-baseline change | `git diff --name-only -- 'readiness/surface-baselines/**' \| wc -l` | **0** |
| No `PackageVersion` outside CPM | `git diff -- '**/*.fsproj' \| grep PackageVersion` | **none** |
| `PackageSurfaceCheck` / `FsiTranscripts` product baseline diff | (build-tooling `.fsi` only; no tracked product baseline) | **no diff** |
| Generated consumers stay governed | `GeneratedProductCheck` green with `schema_version` header | see `logs/generated-product-check.log` |

Invariants 1–6 hold: the change is confined to `build/Governance/**`,
`tests/Governance.Tests/**`, `.agents`/`.claude` skills, `.gitignore`, and the active spec
tree. No product runtime, command, effect, subscription, interpreter, package-version, or
visual-path change. The new `.fsi` are **build-tooling scope** (`FS.Skia.UI.Build`), not
tracked product runtime baselines (Principle II).
