# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-28T10:34:07.9110559+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `12829 MB`
- process-count: `1307`
- zombie-process-count: `1292`
- thread-limit: `115795`
- thread-headroom: `114488`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `313`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 12829 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 1307 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 1292 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
