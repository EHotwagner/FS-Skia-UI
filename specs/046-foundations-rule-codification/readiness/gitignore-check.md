# `.gitignore` evidence-hygiene proof (US4, T022, FR-011/012, SC-007)

Forward-looking patterns added to `.gitignore` (scoped — no broad non-`.md` sweep):

```
specs/*/readiness/logs/**
specs/*/readiness/**/readiness*.zip
```

## Regenerable artifacts ARE ignored (FR-011)

```
$ echo x > specs/046-foundations-rule-codification/readiness/logs/sample.log
$ echo x > specs/046-foundations-rule-codification/readiness/readiness-sample.zip
$ git check-ignore -v specs/046-foundations-rule-codification/readiness/logs/sample.log \
                       specs/046-foundations-rule-codification/readiness/readiness-sample.zip
.gitignore:21:specs/*/readiness/logs/**            specs/046-foundations-rule-codification/readiness/logs/sample.log
.gitignore:22:specs/*/readiness/**/readiness*.zip  specs/046-foundations-rule-codification/readiness/readiness-sample.zip
```

(sample files removed after the check.)

## Authored evidence is SPARED — both `*.md` notes AND the `.txt` transcript

```
$ git check-ignore -v specs/046-foundations-rule-codification/readiness/prose-delta.md \
                       specs/046-foundations-rule-codification/readiness/fsi-session.txt
(no output; exit 1 = NOT ignored)
```

This proves the scope spares authored `.txt` (the `fsi-session.txt` transcript), not only
`.md` — the patterns never do a broad non-`.md` sweep.

## No committed evidence removed, no history rewrite (FR-012)

Previously-committed evidence files from earlier features remain tracked (control):

```
$ git ls-files --error-unmatch specs/045-foundations-build-frontend/readiness/aggregate-hang-diagnostics.md \
                                specs/009-v3-modular-framework/readiness/generated-file-lists/app-source.txt
specs/045-foundations-build-frontend/readiness/aggregate-hang-diagnostics.md
specs/009-v3-modular-framework/readiness/generated-file-lists/app-source.txt
```

Both a committed `*.md` note and a committed non-`.md` `.txt` evidence file remain tracked;
the new ignore rule is purely forward-looking and rewrites no history (D3).
