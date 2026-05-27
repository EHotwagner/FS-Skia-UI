# Generated Product Usage

Status: changed and validated.

Generated Asteroids-style products reserve a HUD/status region and gameplay
region, expose a `--layout-evidence` command, and validate layout readability
separately from deterministic render metadata.

Validated generated-product paths:

- Default layout evidence: `readiness/generated-layout-1280x720.txt`
- Constrained layout evidence: `readiness/generated-layout-640x480.txt`
- Packed generated consumer validation:
  `readiness/generated-product-validation.md`
- Persistent launch diagnostics:
  `readiness/supported-host-persistent-launch.txt`

Generated docs and examples use the app-owned names:

- `Product.Program.view`
- `Product.Program.generatedHost`
- `Product.Program.update`

Bounded scene/image evidence is retained as rendering evidence only and is not
used as a substitute for supported-host persistent launch evidence.
