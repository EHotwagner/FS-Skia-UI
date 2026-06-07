# Readiness Contract Discovery — Typed-Controls Plan Closeout (074)

The required shape of each enforced readiness file is recoverable **before** triggering a gate
failure from the generated reference `docs/evidence-formats.md` (single-sourced from
`FS.Skia.UI.Build.Evidence.EvidenceFormatSchema`, currency-checked by `TargetMetadataDrift`).
This feature's readiness artifacts and their authoritative commands / failure classes:

| Artifact | Authoritative command | Failure class | Next action |
| --- | --- | --- | --- |
| `governance-risk-levels.md` | `./fake.sh build -t Route` | readiness-contract | record small/medium/broad bands + required evidence + broad validation |
| `aggregate-hang-diagnostics.md` | the affected FAKE target | readiness-contract | record verdict/stage/elapsed/last command/focused rerun/non-authoritative aggregate |
| `runtime-limitations.md` | `./fake.sh build -t Route` | readiness-contract | record the .NET 10 desktop / Vulkan / SkiaSharp preview boundary + unsupported scope |
| `skill-loading-evidence.md` | task implementation | skill-loading | one row per (task, declared-skill); `loaded_at` < `work_started_at` |
| `evidence-graph.md` | `./fake.sh build -t EvidenceGraph` | evidence-graph | refresh `task-graph.json`; confirm no cycles / dangling refs / `[S*]` surprises |
| `evidence-audit.md` | `./fake.sh build -t EvidenceAudit` | evidence-audit | confirm PASS; no `[S]`/`[S*]` disclosures, no `--accept-synthetic` overrides |

The enforced token contracts for the blocking readiness-contract files
(`governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`) and the
`skill-loading-evidence.md` columns are listed in `docs/evidence-formats.md`. The window-visibility
and SEH classes are **not engaged** by this feature (no interactive window, no synthetic error
handling).
