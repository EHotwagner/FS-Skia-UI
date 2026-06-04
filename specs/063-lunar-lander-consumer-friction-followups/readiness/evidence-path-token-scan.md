# Evidence-path token scan (FR-008, D8, SC-005)

**Disposition: consumer-authoring-only — close with NO code change.**

A template-wide scan confirms **no generated artifact template seeds a divergent
`evidence/` token**; every readiness/evidence artifact path in the shipped templates
uses `readiness/`. The drift the consumer saw at analyze time was a consumer-authoring
slip they self-reconciled to `readiness/` (the merged spec uses `readiness/`).

## Scan results (re-run during T028)

| Target | Finding |
|---|---|
| `.specify/templates/spec-template.md` | references **neither** `readiness/` nor `evidence/` as a path (0 / 0). |
| `.specify/templates/tasks-template.md` | uses `readiness/` consistently (15 occurrences); **no** `specs/<feature>/evidence/` path. |
| `template/base/docs/**` | seeds **no** `specs/<feature>/evidence/` path. |
| Whole `.specify/templates/` + `template/` (broad `evidence/` path-token grep, excluding the `evidence-formats.md` filename and the `docs/evidence` skill name) | **no** divergent `evidence/` directory token. |

Commands used (the real templates, no throwaway harness):

```bash
grep -c "readiness/" .specify/templates/spec-template.md          # 0
grep -c "readiness/" .specify/templates/tasks-template.md         # 15
grep -rEn "[^-a-z]evidence/" .specify/templates/ template/base/docs/ \
  | grep -vE "evidence-formats|docs/evidence|readiness"           # (no matches)
grep -rn "specs/.*/evidence/" .specify/templates/ template/       # (no matches)
```

## Conclusion

The spec scoped FR-008 as "confirm whether a template seeds the divergent token, and if
so unify it; if purely consumer-authoring, record and close." The investigation confirms
the latter, so the honest disposition is to **record and close with no code change** — no
canonical-token mechanism is invented for a non-existent template defect. If a future
template is ever found seeding `evidence/`, it is unified then.
