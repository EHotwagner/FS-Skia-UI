# Template Profile

The `fs-skia-ui` template turns this repository into the governed source for
new FS.Skia.UI products. It supports source-directory installation and local
NuGet template package installation.

## V3 Capability Profiles

The V3 product generator composes `template/base/` with selected fragments from
`template/fragments/` according to `template/capabilities.yml` and
`template/profiles/*.yml`.

| Profile | Contents | Exclusions |
|---------|----------|------------|
| `app` | Product app, product tests, docs, command wrappers, full product Spec Kit governance, selected local skills, and package references for Scene, SkiaViewer, Elmish, KeyboardInput, Layout, and Charts. | Framework implementation projects, framework samples/galleries, historical specs, framework readiness evidence, framework docs, framework README copy, template package source, and generated validation roots. |
| `headless-scene` | Product app and tests for Scene-only authoring with full product governance and selected Scene skill guidance. | Viewer, Elmish, keyboard, layout, charts, samples, and framework maintenance checks unless explicitly selected later. |
| `governed` | Scene plus Testing capability with full product governance assets. | Viewer, Elmish, keyboard, layout, charts, and samples unless selected by the profile. |
| `sample-pack` | Sample-oriented product row with Scene, SkiaViewer, Elmish, and Samples selected. | Samples remain excluded from the default app profile. |

Every profile includes a generated-product Spec Kit install: `.specify/`
templates, scripts, workflows, extensions, a product-oriented constitution, and
project-local `speckit-*` skills. Source-only active state such as
`.specify/feature.json` and this framework repository's constitution are not
copied.

`GeneratedProductCheck` validates the source and packaged `app` rows plus the
source `headless-scene`, `governed`, and `sample-pack` rows. Each row records a
file list under the active feature `readiness/generated-file-lists/` directory
and command logs under `readiness/generated-product-verify/`.

## Generation Options

```bash
dotnet new install .
dotnet new fs-skia-ui --name MyProduct --profile app --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.SceneOnly --profile headless-scene --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.Governed --profile governed --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.Samples --profile sample-pack --allow-scripts yes
dotnet new fs-skia-ui --name MyProduct.NoGit --skipGitInit true --allow-scripts yes
```

Generated project and module names are derived from `--name`. The template also
accepts product metadata and compatibility parameters:

- `--rootNamespace` for compatibility with existing generation commands
- `--packagePrefix` reserved for future generated product packages
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

`TemplateCheck` exercises the V3 profiles through the source directory and the
local package artifact created by `TemplatePack`. `GeneratedProductCheck`
exercises the same capability matrix through the repository generator and
selected capability inputs.

## Artifact Boundaries

The local template package is produced under `artifacts/templates/` and must
contain template metadata plus template-owned source files. It must not contain
historical feature directories, feature readiness evidence, `.git`, `bin`,
`obj`, or generated validation roots.

Template validation writes logs under the active feature `readiness/template/`
directory and isolated generated roots under
`artifacts/template-check/<active-feature>/`. Generated product validation
writes file lists under `readiness/generated-file-lists/`, command logs under
`readiness/generated-product-verify/`, and isolated roots under
`artifacts/generated-products/<active-feature>/`.

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

Current validation is non-visual. Full visual evidence, release validation, an
external template repository split, and broader distribution automation remain
deferred roadmap work.
