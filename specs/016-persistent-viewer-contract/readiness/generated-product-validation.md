# Generated Product Validation

Category: `UnsupportedHost`
Elapsed: `00:00:26.3726640`
Command context: `./fake.sh build -t PackLocal && ./fake.sh build -t GeneratedProductCheck`
Generated consumer root: `/home/developer/projects/FS-Skia-UI/artifacts/generated-products/016-persistent-viewer-contract/app-source`
Local package feed: `/home/developer/.local/share/nuget-local`

## Evidence

- Restore log: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/restore.log`
- Semantic test log: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/semantic-tests.log`
- Bounded smoke log: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/bounded-smoke.log`
- Bounded smoke evidence: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/bounded-smoke.txt`
- Scene evidence log: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/scene-evidence.log`
- Scene evidence output: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/headless-scene-evidence.txt`
- Persistent launch diagnostics log: `/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/generated-consumer-validation/persistent-launch-diagnostics.log`

## Diagnostics

- generated consumer restore from local packages: ok
- generated consumer semantic tests: ok
- generated consumer bounded smoke: ok
- bounded viewer smoke unsupported
- generated consumer scene evidence: ok
- headless scene evidence captured
- generated consumer persistent launch diagnostics: ok
- persistent launch diagnostics captured separately from bounded evidence

## T046 Package Compatibility Validation

Package and generated-consumer compatibility validation passed with the
persistent default launch and explicit bounded helper paths.

Verification:

- `./fake.sh build -t PackLocal` passed.
- `./fake.sh build -t GeneratedProductCheck` passed.
- `./fake.sh build -t TemplateCheck` passed.

## T047 Documentation And Dependency Governance

Dependency governance passed with no new runtime dependency requirement for the
persistent viewer contract. Documentation updates were limited to evidence,
generated-app, and migration guidance.

Verification:

- `./fake.sh build -t DependencyReport` passed.
- `readiness/dependencies.md` reports Central Package Management PASS and no legacy Charts package/project references.
