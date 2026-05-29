# Quickstart: Agent Validation Framework

## 1. Prove validation contract routing

Run the governance tests that seed representative changed-path scenarios for controls, templates, evidence governance, generated guidance, documentation-only, package surface, and build-target contracts.

Expected result: each scenario selects the expected rule ids, required gates, expected artifacts, authority, and failure owner.

## 2. Prove agent-ready verdict behavior

Run `AgentReady` with active feature metadata available.

Expected result: `readiness/agent-verdict.json` and `readiness/agent-verdict.md` identify selected rules, required gates, completed gates, missing gates if any, authority, artifacts, and next command when incomplete.

Run the same path with feature metadata unavailable but git merge-base diff available.

Expected result: the verdict records `changed_path_source.kind=git-merge-base-diff`.

Run the same path with both sources unavailable.

Expected result: the verdict records `status=degraded` and names `./fake.sh build -t Verify` as broad fallback.

## 3. Prove target metadata parity

Run the target metadata drift tests.

Expected result: native FAKE targets, target metadata, docs, and validation contract references agree. Seeded missing-target, missing-metadata, missing-output, and wrong-dependency fixtures fail.

## 4. Prove typed controls front doors

Run Controls semantic tests and FSI transcripts for typed standard controls.

Expected result: typed standard paths cover every existing standard controls module, seeded misspellings fail or are rejected, schema-backed diagnostics name missing/unsupported terms, and custom APIs remain available and visibly classified.

## 5. Prove generated evidence policy separation

Generate a governed app and run normal launch validation.

Expected result: normal launch remains persistent/interactive and does not write readiness artifacts.

Run explicit generated evidence commands.

Expected result: governed reports are produced, authority is stated, unsupported/stale-prerequisite outcomes are classified, and product-owned facts are separated from policy-owned report wording.

## 6. Prove final readiness

Run the focused gates selected by the contract, then:

```bash
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Expected result: required readiness files exist, evidence graph is valid, and audit reports no unresolved synthetic or diff-scan blockers.
