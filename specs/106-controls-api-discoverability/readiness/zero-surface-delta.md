# Zero `.fsi` signature-shape delta (T019)

## Only `///` comment lines changed across the Controls public surface

```
# every changed +/- line in src/Controls/**/*.fsi that is NOT a /// comment or blank line:
$ git diff -- 'src/Controls/**/*.fsi' | grep -E "^[+-]" | grep -vE "^[+-]{3} " \
    | grep -vE "^[+-]\s*///" | grep -vE "^[+-]\s*$" | wc -l
0
```

No `val` / `type` / `member` declaration was added, removed, or retyped. The 31 changed
`src/Controls/**/*.fsi` files differ only in their `///` summary lines (placeholder boilerplate
→ substantive, member-specific documentation). This is the contracted doc-only change: the
public contract *shape* is unchanged, so per-package baselines stay byte-stable (see
`surface-baselines.md`).

## Comprehensive scope (maintainer decision 2026-06-12)

All three placeholder wordings were removed, not only the `function` variant the plan
originally budgeted:

```
$ grep -rcE "Public contract (function|type|module|value) exposed by this FS\.Skia\.UI package\." \
    src/Controls --include=*.fsi | grep -v ':0$' | wc -l
0      (zero files retain any placeholder variant; 356 placeholder lines removed)
```

## Doc comments carry no governance/evidence tokens (FR-014 / diff-scan safety)

```
$ git diff -- 'src/Controls/**/*.fsi' | grep '^+' | grep -E '\.md\b|TODO|FIXME|NotImplementedException'
(none)
```

No added/retained doc comment introduces a literal evidence filename or a bare gate/status
token that the window-visibility or diff-scan audits could misparse as a behaviour/status
signal.
