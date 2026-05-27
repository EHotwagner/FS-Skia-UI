# Public Surface

Status: changed and validated.

This feature changes the public Scene and Testing contracts for layout evidence
and generated validation:

- `src/Scene/Scene.fsi` exposes layout proof levels, named regions, text and
  gameplay bounds, overlap diagnostics, unsupported reasons, and measurement
  modes.
- `src/Testing/Testing.fsi` exposes generated layout validation and host warning
  classification result types.
- Public guidance uses `Product.Program.view`, `Product.Program.generatedHost`,
  and `Product.Program.update` for generated app consumer signatures.

Evidence:

- FSI contract exercise: `readiness/public-contract-guidance.md`
- Package surface review: `readiness/layout-evidence.md`
- Surface baseline verification: `readiness/evidence-audit.md`
- Focused tests: `tests/Scene.Tests/Tests.fs` and `tests/Testing.Tests/Tests.fs`
