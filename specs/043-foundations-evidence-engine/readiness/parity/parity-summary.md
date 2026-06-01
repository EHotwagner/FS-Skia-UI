# SC-001 / SC-001a — byte-parity proof

The compiled F# evidence engine (`FS.Skia.UI.Build.Evidence`) reproduces the
Python engine's output **byte-for-byte** for identical inputs across the
036/037/038 golden fixtures. Each `*.diff` file in this tree is **0 bytes** (no
divergence). Proven two ways:

1. An FSI parity harness running `Engine.runGraph` / `Engine.runAudit` over the
   committed `specs/<F>/tasks.md` + `tasks.deps.yml` + the repo skill registry and
   byte-comparing the result to `tests/Governance.Tests/fixtures/evidence-golden/<F>/`.
2. The `EvidenceGoldenParityTests` Expecto suite (US1), which asserts the same
   byte-equality (DiffPlex renders the first divergence on any non-zero diff) and
   passes green in the `Dev` gate.

Artifacts proven 0-byte per feature:

- `task-graph.json`, `task-graph.md`, `audit-counts.txt` (original Stage-0 oracle)
- `scans/readiness-contract-hits.json`, `scans/persistent-launch-hits.json`,
  `scans/persistent-gui-runtime-hits.json`, `scans/window-visibility-hits.json`,
  `scans/diff-scan-hits.json` (extended FR-017 oracle)

The only nondeterminism in the Python engine — the `recorded-feature-vs-scanned`
warning driven by `.specify/feature.json` — is supplied to the F# engine as data
(`RecordedFeature`), so feeding the scanned feature's own name reproduces the
committed `warnings: []` exactly.
