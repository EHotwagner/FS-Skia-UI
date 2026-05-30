# US2 Evidence — Audit-Status Region Resolution

Covers FR-004, FR-005, FR-006, SC-003. The audit reads machine-readable status
**only** from a fenced `audit-status` region; prose, negations, markdown bullets,
and illustrative code blocks are never read as status.

## Mechanism

`.specify/extensions/evidence/scripts/python/audit-status-scan.py` is the
authoritative scanner. `run-audit.sh` runs it (step `[2e/3] Audit-status region
scan`) over readiness files that contain an `audit-status` region, excluding
`audit-fixtures/` and `audit-rejections/` (those are exercised in isolation).
The bare substring blockers (`taskbar-only` / `mismatch` / `nu1603` in text) were
dropped from the conditional GUI/window scans in favour of structured key reads.

Deterministic resolution rule (see `contracts/audit-status-region-contract.md`):
first region wins; duplicate key in region → parse error; prose never wins;
malformed entry → parse error.

## Fixture results

Full transcript: `readiness/logs/audit-status-fixtures.txt`.

| Fixture | Region content | Result | Why |
|---|---|---|---|
| `prose-negation-clean.md` | clean region; blocker terms only in prose/negation/bullets/illustrative block | **PASS** (exit 0) | prose never read as status (FR-004); prose bullet `- exact-package-match=true: no…` and the `text` block do not override the region (US2 scenario 2) |
| `genuine-violation.md` | `exact-package-match=false`, `package-resolution=nu1603`, `taskbar-only=true`, `taskbar-entry=true`+`window-visible=false` | **BLOCK** (exit 2) | structured violating values still block (FR-006, no true-positive regression) |
| `duplicate-key.md` | `exact-package-match` declared twice | **BLOCK** (exit 2) | duplicate-in-region parse error (rule 2) |
| `malformed-key.md` | bare key (no `=`) and empty key (`=true`) | **BLOCK** (exit 2) | malformed-entry parse error (rule 4) |

## No regression

- 037's own `EvidenceAudit` run: `audit-status: 0 blocking`, `verdict=PASS`
  (037 declares no `audit-status` region of its own).
- `tests/Governance.Tests/AuditStatusRegionTests.fs` (6 tests) exercises the real
  scanner against the committed fixtures, including a failing-first contrast
  showing the prose fixture contains every blocker term a naive substring
  scanner would block on.
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` (29 tests, features
  016/018/011) still pass after dropping the substring blockers — those fixtures
  block via structured fields (`exact-match=false`, `taskbar-entry=true` with
  `window-visible=observed:false`).
