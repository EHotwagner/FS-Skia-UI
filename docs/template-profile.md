# Template Profile

The `fs-skia-ui` template turns this repository into the governed source for
new FS.Skia.UI products. It supports source-directory installation and local
NuGet template package installation.

## Profiles

| Profile | Contents | Exclusions |
|---------|----------|------------|
| `default` | Core library, charts, layout, samples, tests, docs, Spec Kit assets, command wrappers, central package policy, and root surface baselines. | Historical feature specs/readiness evidence, `.git`, build outputs, local artifacts, and template package source. |
| `minimal` | Core library, `BasicViewer`, core tests, package checks, governance tests, docs, Spec Kit assets, command wrappers, and central package policy. | Optional layout, charts, parity, visual/sample gallery scope, historical feature specs/readiness evidence, solution file, and source-only artifacts. |

## Generation Options

```bash
dotnet new install .
dotnet new fs-skia-ui --name MyProduct --profile default
dotnet new fs-skia-ui --name MyProduct.Minimal --profile minimal
```

The template accepts product identity parameters:

- `--rootNamespace`
- `--packagePrefix`
- `--authors`
- `--repositoryUrl`
- `--targetFramework`

Template validation exercises both `default` and `minimal` through the source
directory and the local package artifact created by `TemplatePack`.

## Artifact Boundaries

The local template package is produced under `artifacts/templates/` and must
contain template metadata plus template-owned source files. It must not contain
historical feature directories, feature readiness evidence, `.git`, `bin`,
`obj`, or generated validation roots.

Generated project validation writes logs under
`specs/007-v2-template-packaging/readiness/template/` and isolated generated
roots under `artifacts/template-check/007-v2-template-packaging/`.

## Drift Classification

Template-owned changes include source, samples, tests, docs, Spec Kit templates
and presets, command wrappers, build workflow targets, dependency policy, and
template metadata. A template-owned change must be aligned by at least one of:

- `.template.config/template.json`
- template docs
- dependency policy or dependency docs
- generated guidance templates
- command-surface docs or build target updates
- `readiness/template-deferrals.yml`

Accepted deferrals require `id`, `paths`, `rationale`, `owner`, and
`target_phase`.

## Deferred Scope

V2 validation is non-visual. Full visual evidence, release validation, an
external template repository split, and broader distribution automation remain
deferred roadmap work.
