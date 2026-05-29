# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-29T12:14:08.0667417+00:00`
- target: `Verify`
- platform: `Linux 7.0.10-arch1-1 #1 SMP PREEMPT_DYNAMIC Sat, 23 May 2026 14:21:20 +0000`
- available-memory: `12221 MB`
- process-count: `18`
- zombie-process-count: `0`
- thread-limit: `115795`
- thread-headroom: `115777`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524153`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `196`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 12221 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 18 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 0 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524153 | >= 64 |  |  | pass |  |

Diagnostics:
- none
