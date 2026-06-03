# Quickstart — validating task artifacts (corrected experience)

This is the author-facing walkthrough the corrected guidance must deliver. It
doubles as the manual acceptance script for SC-001…SC-006.

## 1. Author the two files

In your feature directory (e.g. `specs/001-my-feature/`):

- `tasks.md` — story-grouped tasks with `[skillist: …]` mirrors.
- `tasks.deps.yml` — **must** start with `schema_version` + a `tasks:` wrapper:

```yaml
schema_version: "1.0"
tasks:
  T001:
    deps: []
    skillist: []
  T002:
    deps: [T001]
    skillist: ["fs-skia-layout-readability"]
    owns: []            # owns nothing — most tasks
  T033:
    deps: [T032]
    skillist: ["speckit-evidence-audit"]
    owns: ["evidence-audit"]   # this task owns the audit evidence
```

Titles are free-form — they are never scanned for capability phrases.

## 2. Validate the graph (canonical command)

```bash
./fake.sh build -t EvidenceGraph
```

There is **no** `run-audit.sh` and **no** shell/python runner — the graph
computes in-process in compiled F#. The target resolves your feature from
`.specify/feature.json`. To validate a different feature, override:

```bash
SPECKIT_FEATURE_DIR="specs/001-my-feature" ./fake.sh build -t EvidenceGraph
```

**Confirm the echo matches your feature** (SC-001):

```
feature-directory=…/specs/001-my-feature
tasks=33
verdict=ok
```

If you see a sample/other directory or a surprising `tasks=` count, stop — it is
not validating your feature.

## 3. Expected failure modes (acceptance)

| Situation | Expected (SC-002/SC-003) |
|-----------|--------------------------|
| no `.specify/feature.json`, no override | non-zero exit, message naming `.specify/feature.json`, the `"feature_directory"` key, and the `SPECKIT_FEATURE_DIR` override — **never** a sample pass |
| bare `Tnnn:` keys (forgot the `tasks:` wrapper) | one directive error pointing at the missing `tasks:` mapping, not 33 buried "no key" errors |
| `owns: ["graph-validation"]` but `speckit-evidence-graph` not in that task's `skillist` | directive error naming the required skill |
| hint id from the guidance | resolves to exactly one registered skill (0 unresolved-skill failures, SC-004) |

## 4. Two skills, one answer (SC-005)

The `speckit-tasks` skill's Validation section and the `speckit-evidence-graph`
skill name the **same** entry point (`./fake.sh build -t EvidenceGraph`). They do
not contradict each other.

## 5. Maintainer verification (this repo)

Run `./fake.sh build -t Route` for the authoritative gate list, then the
escalated order sequentially (FAKE is not concurrency-safe):

```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Regenerate `.claude` peers + `validation.contract.yml` after touching skills:
`./fake.sh build -t RefreshSurfaceBaselines` (currency enforced by
`SkillSyncCheck` / `TargetMetadataDrift`). SC-007 = all currency gates green and
a regenerated consumer inherits every fix.
