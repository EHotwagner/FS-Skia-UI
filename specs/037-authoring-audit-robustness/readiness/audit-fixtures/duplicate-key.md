# Audit Fixture — Duplicate Key Parse Error (US2)

A key declared more than once inside the authoritative region is a parse error —
the audit must not silently take last-wins (audit-status-region-contract.md rule 2).

```audit-status
exact-package-match=true
exact-package-match=false
window-visible=true
```

Expected: **BLOCK** with a `duplicate key in audit-status region` parse error.
