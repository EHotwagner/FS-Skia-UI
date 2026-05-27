# Compatibility Impact

Status: broad Tier 1 impact documented.

Affected surfaces:

- Public Scene and Testing signatures.
- Package surface baselines for changed Scene/Testing contracts.
- Generated product source and generated product tests.
- Template docs and public generated-app guidance.
- Evidence graph/audit readiness outputs.

Compatibility notes:

- Existing game mechanics are not rewritten beyond reserving HUD/gameplay
  regions and constraining active gameplay entities to the gameplay region.
- Deterministic render hashes remain valid render metadata but no longer imply
  layout readability.
- Unsupported host or font/layout facts are explicit diagnostics and do not
  claim readable layout proof.
- No release automation, migration automation, new game engine, or unrelated
  controls/chart/DataGrid compatibility work is included.

Broad validation:

- Focused package, generated product, guidance, template, graph, and audit
  evidence is recorded under `readiness/`.
- `Verify` remains the broad aggregate validation target for T038 and must be
  recorded separately from focused evidence if it fails before product checks.
