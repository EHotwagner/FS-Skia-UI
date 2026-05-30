# Audit Fixture — Prose/Negation Clean (US2)

This fixture exercises FR-004 / FR-005: blocker terms appear **only** in prose,
negations, markdown bullets, and an illustrative (non-`audit-status`) code block.
None of them are authoritative, so the audit must resolve PASS.

Explanatory prose that mentions the blocker terms without declaring a violation:

- This is **not** a taskbar-only claim; the window is genuinely visible.
- A package mismatch (e.g. an `nu1603` warning) is what we are explicitly
  avoiding here — there is no actual mismatch.
- The terms `taskbar-only`, `mismatch`, and `nu1603` appear in this sentence as
  explanation, not as machine-readable status.

A markdown bullet that names a status key in prose — it must NOT override the
authoritative region (US2 scenario 2):

- exact-package-match=true: no, this bullet is prose and is ignored.

An illustrative fenced block showing what a *bad* value would look like — its
info string is not `audit-status`, so it is never read as status:

```text
exact-package-match=false
taskbar-only=true
package-resolution=nu1603
```

The single authoritative machine-readable region:

```audit-status
exact-package-match=true
package-resolution=resolved
taskbar-only=false
taskbar-entry=false
window-visible=true
```

Expected: audit resolves the region values (all passing) and ignores every prose
and illustrative occurrence above → **PASS**.
