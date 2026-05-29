# US1 Tests First Evidence

Tasks: T015, T016, T017
Captured: 2026-05-29T11:54:00+02:00

## Added Coverage

- `template/base/tests/Product.Tests/Tests.fs`
  - generated evidence graph/audit bash invocation assertions
  - generated `Verify` redirected text-output assertions
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs`
  - repository governance assertions for bash invocation, no executable-mode repair, text-only log writing, and report diagnostics

## Executed Verification

```text
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated evidence" --logger "console;verbosity=minimal"
```

Result:

- Exit code: 0
- Passed: 6
- Failed: 0

## Non-Authoritative Raw Template Test Attempt

```text
dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --filter "generated evidence|generated Verify" --logger "console;verbosity=minimal"
```

Result:

- Exit code: 1
- The raw template project failed to compile before tests because uninstantiated template conditionals produce duplicate and unresolved generated product types.
- This raw-template run is not the authoritative generated-product validation path. The authoritative generated path remains `GeneratedProductCheck` / generated checkout validation in later US1 tasks.
