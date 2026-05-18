# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-18T09:39:47.8194761+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `20805 MB`
- process-count: `105`
- zombie-process-count: `88`
- thread-limit: `115795`
- thread-headroom: `115690`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524167`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `169`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 20805 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 105 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 88 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524167 | >= 64 |  |  | pass |  |

Diagnostics:
- none
