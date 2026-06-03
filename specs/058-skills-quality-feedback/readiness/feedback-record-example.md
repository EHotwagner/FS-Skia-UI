# Worked Feedback Record Example (T034, SC-007)

One worked `specs/<feature>/feedback/` entry, demonstrating a record that routes a
generalizable-code candidate toward `FS.Skia.UI.SkillSupport`, with official-docs-first
research links (FR-015/FR-017). This is the shape the `fs-skia-feedback-capture` skill
writes on phase completion under `--feedback true`.

```markdown
---
phase: plan
date: 2026-06-03
severity: minor            # none | minor | major | blocker
---

## Process friction
While planning the support-library extraction, the per-family glob→regex matching was
re-derived ad hoc in the routing path tables. A documented, tested `isMatch`/`discover`
helper would have removed the guesswork and the off-by-one on whether `**` crosses `/`.

## Generalizable code
Yes. Skill family/topic: **fsharp-io-globbing**. Candidate helper: a fnmatch-style
`Globbing.isMatch : glob -> path -> bool` (with `**` crossing `/`, `*`/`?` within a
segment) plus `Globbing.discover : root -> globs -> string list` and a DiffPlex-backed
`Globbing.currencyDiff`. Triage destination: `FS.Skia.UI.SkillSupport.Globbing`
(`src/SkillSupport/Globbing.fsi`) — exactly where this candidate landed in US2.

## Research links
- Official: .NET file globbing — <https://learn.microsoft.com/en-us/dotnet/core/extensions/file-globbing>
- Official: F# language reference — <https://learn.microsoft.com/en-us/dotnet/fsharp/>
- Community: DiffPlex (the currency-diff backing) — <https://github.com/mmanela/diffplex>
```

This worked entry demonstrates the full triage path: a friction note + a named skill
family/topic + a concrete candidate helper + official-docs-first research links →
`FS.Skia.UI.SkillSupport` (FR-015 → US2). Offline, the Research links section would
instead read `research blocked — <why>` (FR-018).
