# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-06-14T06:43:37.7270826+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `4137 MB`
- process-count: `110`
- zombie-process-count: `0`
- thread-limit: `115791`
- thread-headroom: `115681`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524174`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `238`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 4137 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 110 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 0 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524174 | >= 64 |  |  | pass |  |

Diagnostics:
- none
