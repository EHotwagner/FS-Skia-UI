# Generated guidance validation (placeholder)

- **Authoritative command**: `./fake.sh build -t GeneratedGuidanceCheck`.
- **Artifact path**: this file records the guidance-currency outcome for the
  escalated path; the faithful renderer is framework-internal and generated
  products consume `Control.render` unchanged (same signature, richer output),
  so **no generated-project content change** is expected.
- **Failure class**: a guidance drift (stale generated skill/guidance vs the
  canonical `.agents` tree) is a governance failure; resolved via
  `RefreshSurfaceBaselines`, not hand-edits.
- **Next action**: run `GeneratedGuidanceCheck` in the escalated serialized order
  (T029) and confirm PASS (no generated-guidance delta from this feature).

_Placeholder created in T002._
