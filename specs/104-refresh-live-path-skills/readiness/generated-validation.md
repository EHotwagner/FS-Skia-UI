# Generated Validation (feature 104, live-path skill currency)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

Feature 104 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products: it is
a documentation-currency (skill-honesty) pass — the `.agents/skills/fs-skia-reconciliation` refresh,
the `src/Controls/skill/SKILL.md` E3/E4 edit, and the NEW `.agents/skills/fs-skia-controls-host`,
plus the governance-generated `.claude/skills/**` mirror and `template/base/docs/skillist-reference.md`
— with **no** `template/**` product asset, sample, command-surface, or generated-content change, and
**no** `.fsi` surface move. A generated project consuming `FS.Skia.UI.Controls` /
`FS.Skia.UI.Controls.Elmish` therefore resolves the **same** package surface as before —
`package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true` — and renders and
behaves identically (byte-identical output).

`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because feature 104 introduces no
new generated-project test; `authoritative=false` because `GeneratedProductCheck` is not the
authoritative signal for this framework-internal documentation change (the authoritative signal is
`EvidenceAudit verdict=PASS` plus the unchanged framework suites under `Dev` and the skill gates
`SkillQualityCheck`/`SkillSyncCheck`). Any local `GeneratedProductCheck` environment failure is
recorded as **non-authoritative** environment-class, not a product defect (see
[aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)).

## Route printout (T020)

```
developer-class=framework-author
tier=agent-ready
gates=Dev, GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, PhaseHookParityCheck, SkillContractPathCheck, TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=evidence-governance, specify-catchall, docs-only, template-docs, skill-quality
```

`Route` did **not** escalate to the `controls-public-surface` set. The feature-101/102 escalation fires
on `src/Controls/**/*.fs(i)` edits; feature 104's only `src/Controls/**` edit is
`src/Controls/skill/SKILL.md` (Markdown in the `skill/` subdir), which routes via `skill-quality` /
`docs-only`, not the controls source rule. Per "run only the gates `Route` prints", the authoritative
gate set is the 10 gates above — the dedicated skill-currency gates `SkillQualityCheck` /
`SkillSyncCheck` plus `Dev` and the docs/template/evidence governance gates.
