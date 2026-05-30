# Governance Scope

Feature risk level: medium governance risk.

In scope:

- Validator behavior in `.specify/extensions/evidence/scripts/python/compute-task-graph.py`.
- Task-authoring guidance in repository and preset task templates.
- Skill registry diagnostics for `skillist` ids.
- Advisory FS.Skia.UI capability guidance.
- Graph-only command and generated report labels.

Out of scope:

- Runtime FS.Skia.UI APIs and `.fsi` public signatures.
- Package identities, package versions, and runtime package dependencies.
- Rendering, viewer behavior, and generated demo behavior.
- Runtime Elmish/MVU state workflows.

Package impact: none for runtime packages. Template/guidance content changed only.

Required evidence:

- Focused governance tests.
- Direct graph-only validator fixture runs.
- Real guidance scans over repository files.
- Sequential FAKE-backed validation when broad targets are run.
