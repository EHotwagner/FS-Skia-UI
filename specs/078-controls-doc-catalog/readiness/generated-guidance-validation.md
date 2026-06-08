# Generated-Guidance Validation (078)

- status: PENDING (placeholder — filled at T028 after `GeneratedGuidanceCheck`)
- authoritative-command: `./fake.sh build -t GeneratedGuidanceCheck`
- artifact-path: `specs/078-controls-doc-catalog/readiness/generated-guidance-validation.md`
- failure-class: `generated-guidance-drift`
- next-action: run the gate; record PASS/FAIL. This feature edits no generated
  guidance source (`.specify/templates/**`, preset/fragment guidance), so
  `GeneratedGuidanceCheck` is expected to be unaffected; it is run as part of the
  serialized governance suite for completeness.
