# Audit Diagnostics

Status: complete for focused guidance implementation.

Audit diagnostics must name command order, concurrent FAKE suspicion, `.fake`
race status, and the sequential rerun action before product debugging.

Implemented diagnostics:

- `build.fsx` focused gate summaries now include concurrent FAKE context,
  `.fake` race classification, sequential rerun action, and follow-up
  classification.
- `docs/build.md`, `docs/testing.md`, `docs/evidence.md`,
  `template/base/README.md`, and `template/base/docs/product.md` document the
  same race-like failure triage rule.
