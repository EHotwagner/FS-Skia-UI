# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-26T15:12:03.7909559+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `11718 MB`
- process-count: `60`
- zombie-process-count: `48`
- thread-limit: `115795`
- thread-headroom: `115735`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `249`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 11718 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 60 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 48 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
