# Template Profile

The `fs-skia-ui` template turns this repository into the governed source for
new FS.Skia.UI products. It supports source-directory installation and local
NuGet template package installation.

## Profiles

| Profile | Contents | Exclusions |
|---------|----------|------------|
| `default` | Core library, charts, layout, samples, tests, docs, Spec Kit assets, command wrappers, central package policy, and root surface baselines. | Historical feature specs/readiness evidence, source `.git` metadata, source `.specify/feature.json` active-feature state, build outputs, local artifacts, and template package source. |
| `minimal` | Core library, `BasicViewer`, core tests, package checks, governance tests, docs, Spec Kit assets, command wrappers, and central package policy. | Optional layout, charts, parity, visual/sample gallery scope, historical feature specs/readiness evidence, solution file, and source-only artifacts. |

## Generation Options

```bash
dotnet new install .
dotnet new fs-skia-ui --name MyProduct --profile default --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.Minimal --profile minimal --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.NoGit --skipGitInit true --allow-scripts yes
```

The template accepts product identity parameters:

- `--rootNamespace`
- `--packagePrefix`
- `--authors`
- `--repositoryUrl`
- `--targetFramework`
- `--skipGitInit`

By default, generation creates an initial Git commit for standalone Spec Kit
workflows when the output directory is not already inside a Git worktree and
repairs Unix execute permissions on generated shell scripts. The initial commit
prevents unborn-branch failures in commands such as `/speckit-clarify`. The
.NET CLI prompts before running template scripts unless `--allow-scripts yes`
is supplied. Use `--skipGitInit true` for generated projects that should rely
on an existing parent repository or remain disposable validation artifacts.

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

The root `README.md` is template-owned generated-product documentation, not
only repository landing-page copy. It should describe the project in product
terms before listing build commands: the Elmish/MVU app model, immutable
`Scene` output, the Vulkan/Skia host boundary, package responsibilities,
sample coverage, Spec Kit governance, and the governed template workflow.

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
