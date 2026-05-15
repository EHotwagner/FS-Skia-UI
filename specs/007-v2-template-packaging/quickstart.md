# Quickstart: Template Packaging and Drift Governance

This quickstart describes the V2 validation path that implementation must make real.

## Prerequisites

- .NET SDK `10.0.300` or compatible `net10.0` SDK
- Bash or Windows command shell
- Git checkout of this repository

## Fast Repository Check

```bash
./fake.sh build -t Dev
```

Expected result: restore, build, and default non-visual tests pass without running template packaging.

## Dependency Governance Check

```bash
./fake.sh build -t DependencyReport
```

Expected outputs:

```text
Directory.Packages.props
docs/dependencies.md
specs/007-v2-template-packaging/readiness/dependencies.md
```

The target fails if a project file reintroduces an unmanaged inline external package version or if dependency metadata is missing.

## Template Validation Check

```bash
./fake.sh build -t TemplateCheck
```

Expected behavior:

1. Pack the local template artifact into `artifacts/templates/`.
2. Install the template from the repository source directory.
3. Install the template from the local `.nupkg`.
4. Generate default and minimal starter projects from each install source.
5. Scan generated projects for unreplaced placeholders.
6. Scan generated projects for excluded historical feature/readiness paths.
7. Run each generated project's fast workflow:

   ```bash
   ./fake.sh build -t Dev
   ```

Expected output root:

```text
specs/007-v2-template-packaging/readiness/template/
```

## Generated Guidance Check

```bash
./fake.sh build -t GeneratedGuidanceCheck
```

Expected result: active project and preset-owned spec/plan templates contain V2 governance prompts for package impact, public contract impact, state workflow, layout/rendering, evidence, unsupported scope, build targets, template ownership, dependencies, command surface, generated project impact, and evidence paths.

## Template Drift Check

```bash
./fake.sh build -t TemplateDrift
```

Expected result: template-owned changes are either aligned with template/docs/policy/guidance/command updates or covered by valid deferrals in:

```text
readiness/template-deferrals.yml
```

Each accepted deferral must include rationale, owner, and target phase.

## Full V2 Verification

```bash
./fake.sh build -t Verify
```

Expected result: existing V1 full verification plus V2 `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, and `TemplateDrift` all pass.
