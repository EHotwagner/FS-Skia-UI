# Consumer Validation Transcript (SC-001 / SC-002 / SC-003) — 059

Captured against a **real generated consumer** produced by `GeneratedProductCheck`
(`artifacts/generated-products/059-speckit-tasks-validation-feedback/app-source`),
driving the inherited template `build.fsx` validation target through its public
entry point (`dotnet fsi build.fsx -t EvidenceGraph`).

The generated consumer ships **no** `.specify/feature.json` (excluded by the
template) and **no** bundled sample feature (`generated-evidence-workflow` removed,
FR-014).

## 1. Ambiguous/absent feature → loud fail (SC-002, FR-003)

Command: `dotnet fsi build.fsx -t EvidenceGraph` (no override, no feature.json)

```
System.Exception: Cannot resolve the feature to validate: no SPECKIT_FEATURE_DIR
override is set and .../app-source/.specify/feature.json has no usable
"feature_directory" entry. Run /speckit.specify to record a feature, or set
SPECKIT_FEATURE_DIR to the feature directory to validate. Validation never falls
back to a bundled sample.
```

Exit code: **1** (non-zero, actionable, names `.specify/feature.json` and the
`SPECKIT_FEATURE_DIR` override; no sample pass).

## 2. Override resolves the author's feature, echoes dir + count (SC-001, FR-002/FR-004/FR-005)

A `tasks.deps.yml` authored strictly from the documented schema (the
`schema_version` + `tasks:` wrapper + per-task `owns:` field — SC-003, authored
without copying any sample) plus a two-task `tasks.md`:

Command: `SPECKIT_FEATURE_DIR="specs/001-demo" dotnet fsi build.fsx -t EvidenceGraph`

```
feature-source=SPECKIT_FEATURE_DIR override
feature-directory=.../app-source/specs/001-demo
tasks=2
```

The echoed directory and `tasks=2` correspond to the author's feature, validated
on the **first** attempt — the deps file written from the template/skill text
passed the schema gate with no structural-shape failure (SC-003). No false-green
sample run occurred (SC-001 = 0).
