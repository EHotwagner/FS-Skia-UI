# Evidence Obligations

Tier: Tier 1 contracted governance and command-surface change.

Runtime API impact: none. No packaged F# `.fsi` surface is added or changed by
this feature.

Build workflow obligation: `build.fsx` exposes `BuildModel`, `BuildMsg`,
`BuildEffect`, `init`, pure `update`, and an interpreter that executes process
and filesystem effects at the edge.

Required v1 artifact classes:

| Class | Path |
|-------|------|
| Build/test/package logs | `specs/006-template-framework-governance/readiness/logs/*.txt` |
| Public contract FSI transcripts | `specs/006-template-framework-governance/readiness/fsi/*.txt` |
| Package surface baselines | `readiness/surface-baselines/*.txt` |
| Package surface status | `specs/006-template-framework-governance/readiness/logs/package-surface-check.txt` |
| Sample smoke output | `specs/006-template-framework-governance/readiness/sample-smoke/*.txt` |
| Task graph output | `specs/006-template-framework-governance/readiness/task-graph.json` and `.md` |
| Evidence audit verdict | `specs/006-template-framework-governance/readiness/logs/evidence-audit.txt` |

Deferred roadmap categories: template packaging, dependency governance,
generated spec/plan hardening, layout evidence, visual evidence, package
consumer smoke, and release validation.
