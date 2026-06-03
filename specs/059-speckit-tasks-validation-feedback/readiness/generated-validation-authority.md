# Generated Validation Authority — 059-speckit-tasks-validation-feedback

The single source of truth for which feature a generated consumer validates is
`.specify/feature.json`'s `"feature_directory"` entry, read at the template
`build.fsx` interpreter edge — the same pattern the framework engine uses in
`build/Governance/Engine/Model.fs activeFeatureId`.

- **Authoritative command**: `./fake.sh build -t EvidenceGraph` (graph) /
  `./fake.sh build -t EvidenceAudit` (merge gate). There is **no** `run-audit.sh`
  or any shell/python runner; the engine computes in-process.
- **Override**: `SPECKIT_FEATURE_DIR="specs/<id>"` selects a different feature.
- **Failure class**: when neither `.specify/feature.json` nor the override
  resolves an existing directory, validation **fails loud** (non-zero, naming the
  missing source and the override) and never falls back to a bundled sample —
  the runtime `generated-evidence-workflow` synthesiser is removed (FR-014).
- **Echo**: the validation target prints `feature-directory=…` and `tasks=<n>`;
  the author MUST confirm these match their feature before trusting the verdict
  (FR-004, SC-001).
- **Next action**: if the echo names an unexpected directory or count, stop — the
  run is not validating your feature.
