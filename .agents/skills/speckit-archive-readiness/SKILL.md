---
name: speckit-archive-readiness
description: Archive historical Spec Kit readiness evidence into specs/archive, leaving only a README stub and refreshed archive inventory. Use before a squash-merge commit is finalized, after a feature is no longer current gate evidence, or when the user asks to archive unnecessary readiness/evidence files.
metadata:
  short-description: Archive historical readiness evidence
---

# speckit-archive-readiness

Archive historical feature readiness artifacts so old logs, transcripts, JSON
hits, screenshots, and generated pass/fail exhaust stop polluting active
checkout search while remaining available as audit context.

## What Counts As Unnecessary

Archive only `specs/<feature>/readiness/**` for a feature that is no longer the
current gate target. After a feature is squash-merged, its readiness files are
historical audit context and must not be cited as current pass/fail evidence.

Do not archive:

- the active feature before its merge commit is being finalized;
- root `readiness/**`;
- `specs/archive/**`;
- protected archive-governance feature `036-archive-readiness-api-docs`;
- package API reference material unless the user explicitly asks.

## Command

Use the deterministic script:

```bash
dotnet fsi scripts/archive-readiness.fsx --feature <feature-id>
```

For multiple historical features:

```bash
dotnet fsi scripts/archive-readiness.fsx --feature 065-typed-controls-front-door --feature 066-typed-catalog-generation
```

For review before writing:

```bash
dotnet fsi scripts/archive-readiness.fsx --dry-run --feature <feature-id>
```

The script writes:

- `specs/archive/<feature>/readiness.zip`
- `specs/archive/<feature>/README.md`
- `specs/<feature>/readiness/README.md`
- `specs/archive/readiness-archives.json`
- refreshed `036-archive-readiness-api-docs` archive inventory/current evidence
  map/stale-reference scan.

## Merge Hook

During `/speckit-merge`, run this after `git merge --squash <branch>` and before
the squash commit:

```bash
FEATURES=$(git diff --cached --name-only | sed -nE 's#^specs/([0-9]{3}-[^/]+)/readiness/.*#\1#p' | sort -u)
for FEATURE in $FEATURES; do
  dotnet fsi scripts/archive-readiness.fsx --feature "$FEATURE"
  git add -A "specs/$FEATURE/readiness" "specs/archive/$FEATURE" \
    "specs/archive/readiness-archives.json" \
    "specs/036-archive-readiness-api-docs/readiness/archive-inventory.md" \
    "specs/036-archive-readiness-api-docs/readiness/current-evidence-map.md" \
    "specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.md" \
    "specs/036-archive-readiness-api-docs/readiness/stale-reference-scan.json"
done
```

If the script fails, stop the merge before committing. Do not delete a feature
branch until the squash commit, archive conversion, and normal merge checks have
all completed.

## Restore

Historical evidence remains recoverable:

```bash
python3 -m zipfile -e specs/archive/<feature>/readiness.zip .
```
