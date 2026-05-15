# Contract: V2 Template Workflow

This contract defines the V2 template command surface. The concrete implementation must extend the existing FAKE target graph invoked through repo-local wrappers.

## Command Invocation

```bash
./fake.sh build -t <Target>
fake.cmd build -t <Target>
```

Both wrappers must run the same target graph. Target names are case-sensitive in documentation and tests.

## Required V2 Targets

| Target | Purpose | Required Inputs | Required Outputs | Pass Criteria |
|---|---|---|---|---|
| `TemplatePack` | Build the local NuGet template artifact. | `.template.config/template.json`, `.template.package/FS.Skia.UI.Template.fsproj`, template-owned source. | `artifacts/templates/FS.Skia.UI.Template.*.nupkg`, package log. | Package exists and contains template metadata plus template-owned files. |
| `TemplateInstallSource` | Install the template from the repository source directory. | Repository root with `.template.config/template.json`. | Source-install log. | `dotnet new install` succeeds or fails with actionable diagnostics. |
| `TemplateInstallPackage` | Install the template from the local packaged artifact. | `TemplatePack` output. | Package-install log. | `dotnet new install` succeeds from `.nupkg`. |
| `TemplateInstantiate` | Create generated projects for every required artifact/profile pair. | Installed source template, installed package template, profile options. | Generated default/minimal projects in isolated temp roots. | Four generated projects exist: source/default, source/minimal, package/default, package/minimal. |
| `TemplateSmoke` | Validate generated projects. | Generated projects. | Placeholder scan, excluded-history scan, generated `Dev` logs, summary verdict. | No unreplaced placeholders, no excluded history, and generated `Dev` succeeds for every generated project. |
| `TemplateCheck` | Full V2 template validation. | `TemplatePack`, `TemplateInstallSource`, `TemplateInstallPackage`, `TemplateInstantiate`, `TemplateSmoke`. | Complete template readiness evidence. | All source and packaged artifact checks pass for default and minimal profiles. |
| `DependencyReport` | Validate central dependency governance. | `Directory.Packages.props`, project files, `docs/dependencies.md`. | Dependency report and no-inline-version scan. | All direct versions are central or documented validation-only exceptions, and all metadata is present. |
| `GeneratedGuidanceCheck` | Validate generated spec and plan guidance. | `.specify/templates/*`, preset-owned templates. | Guidance check report. | Required V2 governance prompts are present in generated spec and plan templates. |
| `TemplateDrift` | Detect template-owned drift. | Template profile, changed paths, deferral records. | `template-drift.md` and verdict. | Drift is aligned with template/docs/policy/guidance/commands or covered by valid deferrals. |

## V1 Target Extension Rules

- `Dev` remains the fast local restore/build/default-test path and must not require template packaging.
- `Verify` is extended to include `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, and `TemplateDrift` after existing V1 verification.
- `Ci` continues to delegate to `Verify` instead of duplicating command order.
- `Clean` may remove target-owned generated template temp output and logs, but must not delete source files, historical evidence, local package outputs, or template package source.

## Template Profiles

| Profile | Required Contents | Exclusions |
|---|---|---|
| `default` | Governed framework starter with configured template options, docs, command wrappers, tests, samples, Spec Kit assets, dependency policy, and package checks. | Historical source feature directories and source-repository-only readiness evidence. |
| `minimal` | Core library, one basic sample, core tests, package checks, docs, and Spec Kit governance assets. | Optional layout, charts, parity, and visual sample scope. |

## Required Artifact Matrix

`TemplateCheck` must validate all rows:

| Artifact Kind | Profile |
|---|---|
| Source directory | `default` |
| Source directory | `minimal` |
| Local package | `default` |
| Local package | `minimal` |

## Readiness Artifact Contract

| Artifact Class | Stable Path |
|---|---|
| Template package log | `specs/007-v2-template-packaging/readiness/logs/template-pack.txt` |
| Source install log | `specs/007-v2-template-packaging/readiness/logs/template-install-source.txt` |
| Package install log | `specs/007-v2-template-packaging/readiness/logs/template-install-package.txt` |
| Generated project logs | `specs/007-v2-template-packaging/readiness/template/<artifact-kind>/<profile>/*.txt` |
| Placeholder scan | `specs/007-v2-template-packaging/readiness/template/placeholder-scan.md` |
| Excluded-history scan | `specs/007-v2-template-packaging/readiness/template/excluded-history-scan.md` |
| Template check verdict | `specs/007-v2-template-packaging/readiness/template/template-check.md` |
| Dependency report | `specs/007-v2-template-packaging/readiness/dependencies.md` |
| Generated guidance report | `specs/007-v2-template-packaging/readiness/generated-guidance.md` |
| Template drift report | `specs/007-v2-template-packaging/readiness/template-drift.md` |
| Local template package | `artifacts/templates/FS.Skia.UI.Template.*.nupkg` |

## Safe Failure Requirements

- Missing `dotnet` template support, failed restore, failed package install, or network restore failure must produce an explicit diagnostic.
- Placeholder failures must name the affected token and file.
- Excluded-history failures must name the forbidden path.
- Optional profile exclusions must fail if they leave broken references, tests, or docs.
- Template drift failures must name the changed path and the missing alignment action.
