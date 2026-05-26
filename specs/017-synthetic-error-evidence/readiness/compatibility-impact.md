# Compatibility Impact

Compatibility impact is limited to governance artifacts.

- Existing ordinary `[S]` and `[S*]` synthetic evidence remains blocking.
- Existing `--accept-synthetic` override behavior remains available and is not
  weakened.
- Valid design-approved `[SEH]` tasks remain visibly `[S]` while being accepted
  by `EvidenceAudit` when all required metadata is present and no other
  blockers remain.
- Implementation-time relabeling, non-eligible convenience fixtures, and late
  cleanup labels remain readiness failures.

No runtime product compatibility, renderer compatibility, package consumer
compatibility, or platform support behavior changes are expected.
