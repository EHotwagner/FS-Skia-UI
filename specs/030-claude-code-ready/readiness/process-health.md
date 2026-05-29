# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-29T13:39:56.4948571+00:00`
- target: `Verify`
- platform: `Linux 7.0.10-arch1-1 #1 SMP PREEMPT_DYNAMIC Sat, 23 May 2026 14:21:20 +0000`
- available-memory: `11571 MB`
- process-count: `17`
- zombie-process-count: `0`
- thread-limit: `115795`
- thread-headroom: `115778`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524153`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `209`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 11571 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 17 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 0 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524153 | >= 64 |  |  | pass |  |

Diagnostics:
- none
