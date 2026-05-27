# Process Health Evidence

## Verify preflight

- timestamp-utc: `2026-05-27T19:27:32.9345981+00:00`
- target: `Verify`
- platform: `Arch Linux`
- available-memory: `14301 MB`
- process-count: `1080`
- zombie-process-count: `1064`
- thread-limit: `115795`
- thread-headroom: `114715`
- file-descriptor-limit: `524288`
- file-descriptor-headroom: `524184`
- dotnet-startup: `pass`
- fake-bootstrap: `pass`
- preflight-elapsed-ms: `287`
- fail-fast: `False`
- unsupported-signals: (none)

| Rule id | Signal | Actual | Default | Override | Reason | Decision | Diagnostic |
|---------|--------|--------|---------|----------|--------|----------|------------|
| `process-health.available-memory` | available-memory-mb | 14301 | >= 128 |  |  | pass |  |
| `process-health.process-count` | process-count | 1080 | <= 4096 |  |  | pass |  |
| `process-health.zombie-count` | zombie-process-count | 1064 | <= 2048 |  |  | pass |  |
| `process-health.file-descriptor-headroom` | file-descriptor-headroom | 524184 | >= 64 |  |  | pass |  |

Diagnostics:
- none
