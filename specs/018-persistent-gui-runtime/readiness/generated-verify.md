# Generated Verify Evidence

generated-tests-exist=true
generated-tests-ran=true
generated-verify-ran=true
authoritative=true
failure-class=none
focused-rerun-command=dotnet fsi build.fsx -t Test
focused-rerun-command=./fake.sh build -t GeneratedProductCheck
focused-rerun-command=dotnet test artifacts/generated-products/018-persistent-gui-runtime/app-source/tests/Product.Tests/Product.Tests.fsproj --no-restore -m:1

Evidence logs:
- `specs/018-persistent-gui-runtime/readiness/logs/t038-product-tests.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t040-generated-test-target.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t040-generated-product-check.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t044-template-check.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t044-generated-product-check.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t050-app-source-restore.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t050-app-source-test.txt`
- `specs/018-persistent-gui-runtime/readiness/logs/t050-generated-product-check-after-supported-host-normalization.txt`
- `specs/018-persistent-gui-runtime/readiness/generated-product-validation.md`

`GeneratedProductCheck` now restores the generated app from the local package
feed, runs the generated `Verify` target through the generated wrapper, executes
the generated tests, captures scene evidence, and records compiled persistent
launch diagnostics. The headless-scene profile uses a scene-only generated
program instead of app/viewer source, so generated verification is no longer
blocked by the prior profile/source mismatch.
