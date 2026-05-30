# Contract: Audit Status Region & Resolution Rule

Covers FR-004, FR-005, FR-006. Defines the **only** place the evidence audit
reads machine-readable status, and the deterministic rule for resolving a key.

## Authoritative region

A status value is authoritative **only** when it appears inside a fenced code
block whose info string is exactly `audit-status`:

````markdown
```audit-status
exact-package-match=true
package-resolution=resolved
taskbar-only=false
window-visible=true
```
````

- Lines inside the fence are parsed as `key=value` pairs (the existing
  `parse_key_values` shape).
- Prose, markdown bullets, and **any other** fenced block (no label, or a
  different label such as `text`/`fsharp`) are NOT regions. Their contents are
  never read as status — they cannot override the region.

## Deterministic resolution rule

For a given key:

1. **First region wins.** Scanning the file top-to-bottom, the first
   `audit-status` region that declares the key provides its authoritative value.
2. **Duplicate-in-region is an error.** If the same key appears more than once
   *within* the authoritative region, the audit surfaces a parse error and does
   not silently take last-wins.
3. **Prose never wins.** A key appearing in prose/bullets/other blocks is
   ignored even if no region declares it (the key is simply absent).
4. **Malformed is surfaced.** A present-but-malformed entry (missing `=`, empty
   key) is reported as a parse error, never silently treated as passing or
   failing.

This replaces "last textual occurrence" and the whole-file `parse_key_values`
scan.

## Blocking is structured, not substring

The audit blocks on explicit violating values, not on substring presence:

| Blocker | Old (removed) trigger | New structured trigger |
|---|---|---|
| process/taskbar-only | `"taskbar-only" in text` | `taskbar-only=true` (or `taskbar-entry=true` with `window-visible=false`) in region |
| unresolved package mismatch | `"mismatch" in text` / `"nu1603" in text` | `exact-package-match ∉ {true,yes}` or `package-resolution=nu1603` in region |

Consequences:

- A sentence "this is **not** a taskbar-only claim" → no blocker (FR-004,
  scenario 1).
- A prose bullet `- exact-package-match=true: no ...` → ignored; the region's
  value is authoritative (FR-005, scenario 2).
- A region declaring `exact-package-match=false` → still blocks (FR-006,
  scenario 3).

## Acceptance fixtures

- `readiness/audit-fixtures/prose-negation-clean.md` — blocker terms only in
  prose/negation + a clean `audit-status` region → audit PASS.
- `readiness/audit-fixtures/genuine-violation.md` — an `audit-status` region
  declaring a violating value → audit BLOCK.

Documented for authors in both `speckit-evidence-audit` SKILL.md peers.
