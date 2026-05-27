# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-27T17:04:55.9836621+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `14550 MB`
- process-count: `908`
- zombie-process-count: `894`
- thread-limit: `115795`
- thread-headroom: `114887`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `269`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 14550 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 908 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 894 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
