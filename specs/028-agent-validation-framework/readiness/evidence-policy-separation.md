# Evidence Policy Separation Evidence

Status: source-level separation verified for generated template product code.

Normal launch proof:

- `template/base/src/Product/Program.fs` no longer imports
  `Product.EvidenceCommands`.
- `Program.fs` delegates explicit evidence flags through
  `Product.EvidenceCommands.tryRunEvidenceCommand`.
- The default branch still calls `Viewer.runApp viewerOptions generatedHost`.
- `Program.fs` does not contain explicit evidence flag strings such as
  `--launch-evidence`, `--bounded-smoke`, `--scene-evidence`, or
  `--image-evidence`.

Evidence command proof:

- `template/base/src/Product/EvidenceCommands.fs` owns explicit evidence
  command dispatch.
- `GeneratedEvidenceWorkflow` records command authority, product-owned facts,
  policy-owned report paths, skipped gates, unsupported outcomes, and next
  commands.
- `GeneratedEvidenceFixture` contains the approved SEH negative fixtures for
  missing generated artifact and unsupported-host classification.

Verification logs:

- `readiness/logs/t037-program-evidence-free.txt`
- `readiness/logs/t038-evidence-command-workflows.txt`
- `readiness/logs/t039-template-membership-tests.txt`
- `readiness/logs/t040-generated-guidance-docs.txt`

Scope note:

Raw `template/base` source is not a compiled generated product because it still
contains mutually exclusive template profile branches. Full generated
instantiation and package validation remain assigned to `TemplateCheck` in the
integration phase.
