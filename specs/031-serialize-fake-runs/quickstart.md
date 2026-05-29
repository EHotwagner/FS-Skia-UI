# Quickstart: Serialize FAKE Runs

Use this sequence when validating the feature. Do not run any two FAKE-backed commands at the same time; they can race on shared `.fake` state.

1. `dotnet tool restore`
2. `./fake.sh build -t Dev`
3. `./fake.sh build -t GeneratedGuidanceCheck`
4. `./fake.sh build -t TemplateCheck`
5. `./fake.sh build -t GeneratedProductCheck`
6. `./fake.sh build -t EvidenceGraph`
7. `./fake.sh build -t EvidenceAudit`

Run `./fake.sh build -t Verify` only after the focused commands above are clean, or use it as the single broad FAKE-backed command for a final pass. Do not run `Verify` concurrently with any other `fake.sh`, `fake.cmd`, or `dotnet fake` invocation.

Record the actual command order in:

```text
specs/031-serialize-fake-runs/readiness/sequential-fake-validation.md
```

If a FAKE-backed command fails and another FAKE-backed command may have been running, rerun the affected FAKE-backed commands sequentially before classifying the failure as a product regression.
