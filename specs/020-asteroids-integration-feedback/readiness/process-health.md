# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-27T12:19:12.2626202+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `14601 MB`
- process-count: `680`
- zombie-process-count: `668`
- thread-limit: `115795`
- thread-headroom: `115115`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `233`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 14601 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 680 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 668 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
