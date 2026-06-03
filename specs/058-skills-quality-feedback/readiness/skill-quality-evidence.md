# SkillQualityCheck — gate-bites + PASS evidence (T019, T026)

The `SkillQualityCheck` gate was run via `./fake.sh build -t SkillQualityCheck` over the
full in-scope corpus (24 skills; vendored `speckit-*` excluded, FR-004). The per-run
report is written to `specs/058-skills-quality-feedback/readiness/skill-quality-check.md`.

## Demonstrated FAIL (the gate bites — T019)

Before the corpus was raised to the rubric, the gate FAILED with a non-zero exit and
named each offending skill + its missing section(s). Representative rows from that run:

```
Checked 24 in-scope skill(s); vendored `speckit-*` excluded (FR-004).
- PASS: 0
- FAIL: 24

- `fsharp-shell-process` — missing: Scope / when-to-use, Persistent-problem mandate
- `fs-skia-template-update` — missing: Driven-library API, Persistent-problem mandate, Related, Sources
- `template/product-skills/fs-skia-testing/SKILL.md` [External research links]: skill 'fs-skia-testing' is missing the required rubric section: External research links
- ... (every in-scope skill named with its specific missing rubric rows)
```

This proves Principle VII behaviour: the gate fails loud, naming the **skill slug** and
the **specific missing section**, and never silently passes.

## Final PASS (the bar is met — T026)

After raising the `fsharp-*`, `fs-skia-*`, `src/*/skill`, `template/product-skills/*`,
`template/fragments/*/skill`, and `template/base/.agents/skills/fs-skia-project` skills to
the rubric, the gate PASSES:

```
Checked 24 in-scope skill(s); vendored `speckit-*` excluded (FR-004).
- PASS: 24
- FAIL: 0
```

`./fake.sh build -t SkillQualityCheck` exits 0 (Status: Ok). Every in-scope skill now
carries Scope, a driven-library API reference, a runnable example, ≥2 external research
links, the FR-017 persistent-problem mandate, `[[slug]]` Related cross-links, and a
Sources line.
