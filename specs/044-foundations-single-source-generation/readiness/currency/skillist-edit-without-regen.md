# Skillist Currency: canonical edit flags the derived view stale (SC-003)

`tasks.deps.yml` `skillist:` is canonical; the `tasks.md` `[skillist: …]` annotation is
the **derived view**. Feature 043's symmetric peer comparison in `Evidence/Audit.fs` is
reframed (T016) into an **asymmetric currency diagnostic** delegating the rendered token to
`SkillistView`. The check is **active-feature scoped** (the engine only reads
`specs/044-.../{tasks.md,tasks.deps.yml}`), so historical features are never re-derived
(FR-007, SC-004). The diagnostic surfaces at `EvidenceGraph` (it runs the merge that
includes the skillist check), and therefore also gates `EvidenceAudit`.

## Edit the CANONICAL `tasks.deps.yml` skillist for a 044 task → flagged stale

```
$ # append fsharp-code-generation to T009's deps skillist, no regen
$ ./fake.sh build -t EvidenceGraph    # exit 1
# task-graph.md:
- T009: the tasks.md [skillist: …] view is stale relative to its canonical tasks.deps.yml
  source; regenerate via ./fake.sh build -t RefreshSurfaceBaselines
  (expected [skillist: fsharp-io-globbing, fsharp-code-generation])
```

Asymmetric (names the **derived** view as stale relative to the **canonical** source),
names the task, the **expected rendered annotation**, and the regeneration command (FR-012).

## Regenerate the derived view via `SkillistView.spliceAnnotation` → green

```
$ dotnet fsi  # using FS.Skia.UI.Build.SkillistView.spliceAnnotation on the T009 line:
REGEN: - [ ] T009 [US1] [skillist: fsharp-io-globbing, fsharp-code-generation] Reframe `SkillSync.fs` …
$ ./fake.sh build -t EvidenceGraph    # exit 0 → PASS (derived view now matches canonical)
```

The splice replaced **only** the bracketed token on the T009 line, leaving the rest of the
line (title, `[US1]`, file refs) byte-for-byte intact.

## Edit the DERIVED annotation alone → flagged stale

```
$ # canonical deps left at [fsharp-io-globbing]; edit tasks.md T009 to add fsharp-parsing
$ ./fake.sh build -t EvidenceGraph    # exit 1
- T009: the tasks.md [skillist: …] view is stale relative to its canonical tasks.deps.yml
  source; regenerate via ./fake.sh build -t RefreshSurfaceBaselines
  (expected [skillist: fsharp-io-globbing])
```

After each step the canonical and derived files were restored; the baseline
`./fake.sh build -t EvidenceGraph` returns exit 0 (PASS).

**Verdict: PASS** — a canonical-source edit flags the derived `[skillist: …]` view stale,
regeneration via the `SkillistView` splice makes it green, and a derived-only edit is also
flagged stale (SC-003 / SC-008).
