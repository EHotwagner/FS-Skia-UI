# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-28T21:44:55.3302430+00:00`
- target: `Verify`
- platform: `Linux 7.0.10-arch1-1 #1 SMP PREEMPT_DYNAMIC Sat, 23 May 2026 14:21:20 +0000`
- available-memory: `14415 MB`
- process-count: `1581`
- zombie-process-count: `1560`
- thread-limit: `115795`
- thread-headroom: `114214`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524171`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `328`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 14415 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 1581 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 1560 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524171 | >= 64 |  |  | pass |  |

Diagnostics:
- none
