# Audit Fixture — Genuine Violation (US2)

This fixture exercises FR-006: a real violating value declared inside the
authoritative `audit-status` region must hard-block, before and after the
robustness changes (no true-positive regression).

The authoritative machine-readable region declares a violating value:

```audit-status
exact-package-match=false
package-resolution=nu1603
taskbar-only=true
taskbar-entry=true
window-visible=false
```

Expected: the audit reads the region and **BLOCKS** — `exact-package-match` is
not in {true,yes}, `package-resolution=nu1603`, and `taskbar-only=true`
(reinforced by `taskbar-entry=true` with `window-visible=false`).
