# Evidence vocabulary (085)

Reminder of the parsing contract the validators enforce (FR-015), recorded so the
feature's evidence is authored against it rather than reverse-engineered from a
gate failure:

- Machine-readable evidence tokens are read **only** from `key=value` lines (and,
  for the authoritative status region, a fenced block whose info string is exactly
  `audit-status`). A markdown **table** carrying the same token names does **not**
  satisfy the validators.
- Blocking is **structured, not substring**: the audit blocks on explicit
  violating values (`exact-package-match` ∉ {true,yes}, `package-resolution=nu1603`,
  `taskbar-only=true`, or `taskbar-entry=true` with `window-visible=false`) — never
  on substring presence of `taskbar-only` / `mismatch` / `nu1603` in prose.
- Required-token files in this feature: `governance-risk-levels.md`,
  `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, and the
  window-visibility class (`interactive-visible-window.md`,
  `close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`,
  `real-image-evidence.md`, `generated-validation.md`, `evidence-audit.md`).

The exact required-token list per class is single-sourced in
`template/base/docs/evidence-formats.md` (read it before authoring).
