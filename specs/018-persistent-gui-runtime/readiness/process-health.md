# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-26T21:15:29.8161836+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `4829 MB`
- process-count: `117`
- zombie-process-count: `99`
- thread-limit: `115795`
- thread-headroom: `115678`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `253`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 4829 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 117 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 99 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
