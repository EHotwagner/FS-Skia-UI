# Contract: Behavior Preservation

## Stable Surfaces

The cleanup must preserve:

- public `.fsi` signatures and surface baselines,
- package IDs and generated package identities,
- generated profile names and generated command names,
- evidence report field names, status vocabulary, output paths, and exit-code
  meanings,
- FAKE target names, target dependency semantics, report outputs, failure
  messages, and readiness artifact paths,
- public viewer diagnostics, host classification, visual evidence, and
  screenshot evidence behavior.

## Validation

Each phase must compare touched behavior against its baseline evidence. Any
intentional contract change is outside this feature and requires a separate
Tier 1 specification and plan.

## Failure Conditions

- A generated command is renamed or removed.
- A report field disappears, changes meaning, or changes status vocabulary.
- A FAKE target changes name, dependency semantics, or readiness path.
- A public `.fsi` or surface baseline changes without moving to a Tier 1
  feature.
- Unsupported screenshot behavior claims screenshot proof instead of explicit
  unsupported evidence.
