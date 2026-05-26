# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-26T16:49:59.7742301+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `8592 MB`
- process-count: `118`
- zombie-process-count: `101`
- thread-limit: `115795`
- thread-headroom: `115677`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `270`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 8592 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 118 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 101 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
