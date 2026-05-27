# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-27T02:03:47.4788506+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `14290 MB`
- process-count: `470`
- zombie-process-count: `454`
- thread-limit: `115795`
- thread-headroom: `115325`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `222`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 14290 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 470 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 454 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
