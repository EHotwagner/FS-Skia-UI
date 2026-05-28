# Template Split Validation

Status: pending phase evidence.

## Batch Evidence Log

| Task | Command | Exit code | Risk | Changed ownership area | Pre-existing failure attribution | Verdict |
|------|---------|-----------|------|------------------------|----------------------------------|---------|
| T016 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated source-shape tests | none | PASS baseline and post-source-shape expectation. |
| T017 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated profile validation | none | PASS; TemplateCheck instantiated and smoked supported generated profiles. |
| T018-T023 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated source split | none | PASS after splitting model, view, layout evidence, evidence commands, window options, and entrypoint responsibilities. |
| T018-T023 | `./fake.sh build -t GeneratedGuidanceCheck` | 0 | medium | generated source split | none | PASS after generated source split. |
| T018-T023 | `./fake.sh build -t TemplateDrift` | 0 | medium | generated source split | none | PASS after generated source split. |
| T024 | `./fake.sh build -t TemplateCheck` | 0 | medium | generated source split final verification | none | PASS. |
| T024 | `./fake.sh build -t GeneratedGuidanceCheck` | 0 | medium | generated source split final verification | none | PASS. |
| T024 | `./fake.sh build -t TemplateDrift` | 0 | medium | generated source split final verification | none | PASS. |
| follow-up | `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj` | 1 | medium | raw unexpanded template source | template-conditional source is not a generated product | UNSUPPORTED RAW COMMAND: unexpanded template files contain mutually exclusive `//#if` profile branches; validate generated products instead. |
| follow-up | `rm -rf /tmp/fs-skia-ui-split-check && dotnet new fs-skia-ui --name SplitCheck --output /tmp/fs-skia-ui-split-check --allow-scripts yes --skipGitInit true` | 0 | medium | generated template package output | none | PASS; generated app includes `Model.fs`, `View.fs`, `LayoutEvidence.fs`, `EvidenceCommands.fs`, `WindowOptions.fs`, and `Program.fs`. |
| follow-up | `dotnet build /tmp/fs-skia-ui-split-check/src/SplitCheck/SplitCheck.fsproj --no-restore -v minimal` | 0 | medium | generated template package output | none | PASS; fresh generated app builds with split compile order. |
| follow-up | `dotnet test /tmp/fs-skia-ui-split-check/tests/SplitCheck.Tests/SplitCheck.Tests.fsproj --no-restore --logger "console;verbosity=minimal"` | 0 | medium | generated template package output | none | PASS; generated app tests pass. |

## Source Split Summary

- `Model.fs` owns product state, messages, initial state, reducer/update
  behavior, and app-owned state helpers.
- `View.fs` owns rendering descriptions and product view construction.
- `LayoutEvidence.fs` owns generated layout evidence helpers and validation.
- `EvidenceCommands.fs` owns generated evidence command implementations and
  local report writing.
- `WindowOptions.fs` owns viewer/window option parsing and diagnostics for
  viewer-enabled profiles.
- `Program.fs` keeps entrypoint, command dispatch, and public generated
  `Product.Program.*` forwarding names.
- `Product.fsproj` compile order is
  `Model.fs`, `View.fs`, `LayoutEvidence.fs`, `EvidenceCommands.fs`,
  profile-conditioned `WindowOptions.fs`, then `Program.fs`.
- `build.fsx` generated product project writing now emits the same split
  compile order for app-profile generated products, and template package
  validation requires every split source file in the package payload.

## Raw Template Test Note

`dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj` builds the
unexpanded template source directly. That is not a supported generated-product
evidence path because the template source intentionally contains mutually
exclusive `//#if` profile branches. The real user-facing path is `dotnet new`
or `TemplateCheck`, which expands profile branches before compiling. A fresh
`dotnet new fs-skia-ui` app generated after this fix includes and builds the
split files successfully.

## US2 Verdict

PASS. Supported generated profiles still instantiate and smoke through
`TemplateCheck`; generated guidance and drift checks remain green; command
names, report fields, generated output paths, exit-code meanings, package IDs,
and generated profile names were not changed.
