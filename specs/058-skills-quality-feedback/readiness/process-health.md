# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-06-03T18:47:52.0749016+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `8458 MB`
- process-count: `33`
- zombie-process-count: `2`
- thread-limit: `115795`
- thread-headroom: `115762`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524152`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `191`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 8458 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 33 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 2 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524152 | >= 64 |  |  | pass |  |

Diagnostics:
- none
