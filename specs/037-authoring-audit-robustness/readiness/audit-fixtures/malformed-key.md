# Audit Fixture — Malformed Entry Parse Error (US2)

A present-but-malformed entry inside the region (no `=`, or an empty key) is a
parse error — never silently treated as passing or failing
(audit-status-region-contract.md rule 4).

```audit-status
exact-package-match
=true
window-visible=true
```

Expected: **BLOCK** with malformed-entry parse errors (the bare `exact-package-match`
line has no `=`, and the `=true` line has an empty key).
