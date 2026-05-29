# Target Metadata Evidence

Status: PASS

US5 now uses native FAKE target registration for the in-scope validation
targets while preserving stable command names.

## Native Target Discovery

- Command: `./fake.sh build --list`
- Result: passed and listed the native FAKE target registry.
- Evidence: `readiness/logs/t057-native-fake-registration-failed.txt`
- Registered examples: `AgentReady`, `BuildWorkflowCheck`,
  `TargetMetadata`, `TargetMetadataDrift`, `EvidenceGraph`, `EvidenceAudit`,
  `Verify`, and `Ci`.

## Command Compatibility

- Command: `./fake.sh build -t BuildWorkflowCheck`
- Result: passed through the native FAKE graph.
- Evidence: `readiness/logs/t057-build-workflow-check.txt`
- Compatibility retained: stable `./fake.sh build -t <Target>` invocation
  remains valid after replacing the custom script runner with `Target.create`
  and FAKE dependency operators.

## Metadata And Drift Validation

- Command: `./fake.sh build -t TargetMetadataDrift`
- Result: passed.
- Evidence: `readiness/logs/t060-target-metadata-drift.txt` and
  `readiness/logs/t061-target-metadata-docs.txt`.
- Machine-readable metadata: `readiness/target-metadata.json`.
- Drift report: `readiness/target-metadata-drift.md`.

`TargetMetadataDrift` validates runnable FAKE targets, metadata entries,
expected outputs, failure owners, validation contract target references, and
documented target references. The latest report states that the runnable target
registry, target metadata, validation contract target references, and docs are
aligned.

## Environment Note

This container has a runtime-only `/usr/share/dotnet` host in addition to the
working SDK install under `/home/developer/.dotnet`. `fake.sh` sets
`FAKE_SDK_RESOLVER_CUSTOM_DOTNET_PATH=/home/developer/.dotnet` by default so
FAKE 6.1.4 resolves the SDK from the working root in this container. Future
containers should remove the mixed install shape or install a full SDK under
the system dotnet root.
