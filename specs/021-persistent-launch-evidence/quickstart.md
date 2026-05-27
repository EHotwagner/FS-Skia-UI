# Quickstart: Persistent Launch Evidence

## 1. Confirm The Contract Surface

Review and update `.fsi` files before implementation:

```bash
sed -n '1,260p' src/SkiaViewer/SkiaViewer.fsi
sed -n '1,220p' src/Testing/Testing.fsi
```

Expected result: persistent-launch evidence, warning classification, and
generated validation contracts are represented in public signatures before `.fs`
implementation changes.

## 2. Add Failing-First Tests

Run the targeted suites after adding contract tests:

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
```

Expected initial result: tests fail for missing persistent-launch artifact
fields, blocked-stage classification, generated naming guidance, or audit
readiness file discovery.

## 3. Verify Generated Guidance

Run generated guidance and product checks:

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t TemplateCheck
```

Expected result after implementation: generated docs and tests use
`Product.Program.view`, `Product.Program.generatedHost`, and
`Product.Program.update`, and they document persistent-launch evidence
separately from layout evidence.

## 4. Produce Supported-Host Evidence

On a desktop host with graphical prerequisites, run the generated readiness
workflow that writes:

```text
specs/021-persistent-launch-evidence/readiness/persistent-launch-evidence.md
```

The artifact must include:

```text
status
mode
command
window-opened
input-dispatch
exit-path
blocked-stage
classification
category
message
```

For a supported-host pass, it must record real window-opened, first-frame, and
controlled-exit facts.

## 5. Check Evidence Graph And Audit

```bash
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Expected result after implementation: required readiness files are present,
benign warnings do not block passing launch evidence, observation/capture
failures are classified honestly, and no synthetic evidence is used for the
supported-host persistent-launch pass.

## 6. Run Full Verification

```bash
./fake.sh build -t Verify
```

Expected result: framework tests, generated product validation, template checks,
evidence graph, and evidence audit pass together.
