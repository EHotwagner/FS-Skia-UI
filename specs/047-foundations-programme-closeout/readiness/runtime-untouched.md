# Runtime-untouched standing-invariants proof (T017, FR-010, SC-006)

Authored summary (kept at the readiness root so it stays tracked; regenerable gate
`.log` files live under the gitignored `readiness/logs/`). Captured at the pinned SHA
`4276bd0`, branch `047-foundations-programme-closeout`.

| Invariant | Check | Result |
|-----------|-------|--------|
| Product runtime / `.fsi` untouched (working tree) | `git diff --name-only -- 'src/**' \| wc -l` | **0** |
| Product runtime / `.fsi` untouched (branch vs `main` merge-base) | `git diff --name-only $(git merge-base HEAD main)..HEAD -- 'src/**' \| wc -l` | **0** |
| No product surface-baseline change | `git diff --name-only -- '**/surface-baselines/**' \| wc -l` | **0** |
| No `PackageVersion` outside `Directory.Packages.props` | `git diff -- '**/*.fsproj' \| grep PackageVersion` | **none** |
| `PackageSurfaceCheck` / `FsiTranscripts` product baseline diff | no product `src/**` or surface-baseline change → no diff | **no diff** |

The change is confined to documentation/measurement/verification-record surfaces:
`docs/reports/_baselines/2026-06-02-foundations-after.md`,
`docs/adr/0006-foundations-programme-closeout.md`, the five contributor docs
(`README.md`, `docs/reports/build.md`, `docs/reports/speckit.md`, `CLAUDE.md`,
`AGENTS.md`), `.specify/schedules/foundations-dogfood-pipeline.yml`,
`.specify/feature.json`, the foundations implementation-plan history doc, and this
feature's `specs/047-foundations-programme-closeout/**` tree. No product runtime,
command, effect, subscription, interpreter, package-version, or visual-path change
(SC-006). The runtime architecture (`Scene → SkiaViewer → Elmish`) is **unchanged**
(Section A row 11 of the after-baseline).
