# Clean-Tree Proof — Feature 054 (US3, SC-006 / SC-007)

## SC-006 — stray scratch removed, recurrence suppressed (FR-007 / FR-008)

- The stray `specs/053-v3-monolith-retirement/readiness/package/local-packages.md`
  was **removed**; it is no longer tracked or present:

  ```
  $ git ls-files --error-unmatch specs/053-.../readiness/package/local-packages.md
  error: pathspec '…' did not match any file(s) known to git
  $ test -e specs/053-.../readiness/package/  →  absent (removed)
  ```

- A new `.gitignore` rule (`specs/*/readiness/package/`, `.gitignore:26`, under
  the Feature-046 evidence-hygiene block) suppresses recurrence. A **recreated**
  scratch file is ignored:

  ```
  $ git check-ignore -v specs/053-.../readiness/package/local-packages.md
  .gitignore:26:specs/*/readiness/package/   specs/053-.../readiness/package/local-packages.md
  $ git status --porcelain specs/053-.../  | grep package  →  0 (ignored)
  ```

- The rule is **scoped to the `package/` scratch subdir** — authored `.md`
  evidence elsewhere under `readiness/` stays tracked:

  ```
  $ git check-ignore specs/054-.../readiness/pin-parity-proof.md  →  not ignored ✓
  ```

After the feature's own work is committed, `git status --porcelain` is empty
(no untracked scratch remains).

## SC-007 — Route reflects real changes (governance-path escalation source gone)

`./fake.sh build -t Route` on this change set reports:

```
tier=agent-ready
gates=Dev, TemplateCheck, GeneratedProductCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
matched-rules=generated-template, evidence-governance, specify-catchall, docs-only
```

This is the **correct** escalation — driven by the real `template/**`,
governance `build/Governance/**`, governance-test, and `specs/**` evidence
changes, **not** by a leftover untracked scratch file. `Route --enforce` passes
(`Status: Ok`) with every required evidence artifact present. With the stray
removed and ignored, a **routine** framework-internal-only diff (`src/**/*.fs`
alone) no longer carries a spurious untracked `readiness/package/` file into
Route's working-tree diff, so it routes to **inner-loop** (`Dev` only) as
intended — the governance-path escalation from the stray is gone.

**Failure class:** untracked pack-flow scratch silently escalating `Route` and
dirtying `git status`. **Next action:** none — removed and `.gitignore`-scoped.
