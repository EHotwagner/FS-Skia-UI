---
name: fsdocs-setup
description: This skill should be used when the user asks to "set up fsdocs", "initialize documentation", "scaffold docs folder", "configure FSharp.Formatting", "add documentation to my F# project", "create docs site", or needs to set up a documentation pipeline for an F# project from scratch. Covers tool installation, directory structure, MSBuild configuration, and initial content creation.
version: 0.1.0
---

# Initialize FSharp.Formatting Documentation

Set up a complete FSharp.Formatting documentation site for an F# project, from tool installation through first successful build.

**Key capabilities:**
- Install and configure `fsdocs-tool` via dotnet local tool manifest
- Create `docs/` directory with starter content
- Configure MSBuild properties for XML doc generation
- Set up `.gitignore` entries and optional custom templates
- Verify the setup with a clean build

## Overview

FSharp.Formatting (`fsdocs`) generates static documentation sites from three sources: XML doc comments in F# source files, Markdown files in `docs/`, and literate F# scripts (`.fsx`) in `docs/`. This skill handles the one-time project setup so other doc skills can focus on content creation.

The setup must happen at the solution or project root level. The tool reads MSBuild properties, project structure, and the `docs/` directory to produce a complete static site in `output/`.

## Prerequisites

Before starting setup, verify:
- .NET SDK 6.0+ installed (`dotnet --version`)
- A valid `.sln` or `.fsproj` file in the working directory (or a parent)
- Git initialized (for `.gitignore` updates)

## Workflow

### 1. Discover Project Structure

Locate the solution or project file. Identify:
- Solution file (`.sln`) — preferred root for multi-project setups
- Project files (`.fsproj`) — the assemblies that will generate API docs
- Existing `docs/` directory or documentation artifacts
- Existing dotnet tool manifest (`.config/dotnet-tools.json`)

### 2. Install fsdocs-tool

Run the automated setup script to handle tool installation and directory creation.

**Script:** `scripts/setup-fsdocs.sh`

The script handles: creating a dotnet tool manifest if absent, installing `fsdocs-tool`, creating the `docs/` directory structure, and verifying the installation. Run it from the solution/project root.

### 3. Create Initial Content

Copy `templates/index.md` to `docs/index.md` and customize:
- Replace the project name placeholder
- Add a brief project description
- Set up navigation links to planned documentation sections

### 4. Configure MSBuild Properties

Add documentation-related properties to `Directory.Build.props` (create if absent). Use `templates/directory-build.props` as a starting point.

Essential properties:

| Property | Purpose | Example |
|---|---|---|
| `GenerateDocumentationFile` | Enable XML doc output | `true` |
| `WarnOn` | Warn on missing docs | `3390` |
| `RepositoryUrl` | Link source on generated pages | `https://github.com/org/repo` |
| `FsDocsLicenseLink` | Footer license link | `https://github.com/org/repo/blob/main/LICENSE` |
| `FsDocsReleaseNotesLink` | Footer release notes link | `https://github.com/org/repo/blob/main/CHANGELOG.md` |
| `FsDocsLogoSource` | Site logo path | `img/logo.png` |
| `FsDocsFaviconSource` | Site favicon path | `img/favicon.ico` |
| `FsDocsTheme` | Site theme | `default` |
| `FsDocsWarnOnMissingDocs` | Warn on undocumented public APIs | `true` |
| `PackageProjectUrl` | NuGet package URL | `https://github.com/org/repo` |

Only set `GenerateDocumentationFile` and `RepositoryUrl` as required. The rest are optional enhancements.

When a `Directory.Build.props` already exists, merge documentation properties into the existing file rather than overwriting. Read the file first and add a new `<PropertyGroup>` section if needed.

For multi-project solutions where only some projects should generate API docs, set `GenerateDocumentationFile` in `Directory.Build.props` and override it with `false` in specific `.fsproj` files that should be excluded (test projects, build tools, etc.).

### 5. Update .gitignore

Append these entries if not already present:

```
# FSharp.Formatting
output/
.fsdocs/
tmp/
```

### 6. Optional: Custom Template

For custom site chrome, create `docs/_template.html`. This overrides the default FSharp.Formatting template. The template uses substitution variables:

- `{{fsdocs-content}}` — rendered page content
- `{{fsdocs-page-title}}` — current page title
- `{{fsdocs-collection-name}}` — project/collection name
- `{{fsdocs-repository-link}}` — repository URL
- `{{fsdocs-logo-src}}` — logo image source
- `{{fsdocs-logo-link}}` — logo click destination
- `{{fsdocs-release-notes-link}}` — release notes URL
- `{{fsdocs-license-link}}` — license URL
- `{{fsdocs-list-of-namespaces}}` — auto-generated namespace listing for API docs
- `{{fsdocs-list-of-documents}}` — auto-generated document listing for content pages

Only create a custom template when the default appearance is insufficient. The default template is well-designed and responsive.

### 7. Configure CI Integration

Add documentation build to the CI pipeline. Typical steps:

```yaml
# Example GitHub Actions step
- name: Build documentation
  run: |
    dotnet tool restore
    dotnet build -c Release
    dotnet fsdocs build --clean --strict
```

Use `--strict` in CI to catch warnings that might be missed locally. The `--strict` flag treats warnings as errors, preventing documentation regressions.

For projects deploying docs to GitHub Pages, add a deployment step that publishes the `output/` directory.

### 8. Verify Setup

Run a clean build to confirm everything works:

```
dotnet fsdocs build --clean
```

Check for:
- Successful compilation of all projects
- Generated `output/` directory with `index.html`
- No errors in build output (warnings about missing docs are expected initially)

## Directory Structure After Setup

A fully configured project should have this layout:

```
my-project/
├── .config/
│   └── dotnet-tools.json          # Local tool manifest (committed)
├── .gitignore                     # Updated with fsdocs entries
├── Directory.Build.props          # MSBuild properties for docs
├── docs/
│   ├── index.md                   # Landing page
│   └── img/                       # Images, logos, diagrams
├── src/
│   └── MyLib/
│       └── MyLib.fsproj           # With GenerateDocumentationFile=true
├── output/                        # Generated site (gitignored)
└── MyProject.sln
```

Content files added later by other doc skills go into `docs/`: Markdown files (`.md`) for technical docs, literate scripts (`.fsx`) for tutorials and examples.

## Best Practices

**DO:**
- ✅ Use `Directory.Build.props` for MSBuild properties (applies to all projects in solution)
- ✅ Set `GenerateDocumentationFile` to `true` before writing doc comments
- ✅ Keep `docs/index.md` as a landing page with navigation to other sections
- ✅ Commit `.config/dotnet-tools.json` so collaborators get the tool automatically
- ✅ Run `dotnet tool restore` in CI before building docs

**DON'T:**
- ❌ Set MSBuild properties in individual `.fsproj` files when `Directory.Build.props` exists
- ❌ Create a custom `_template.html` unless the default is genuinely insufficient
- ❌ Add `output/` to source control — it is a build artifact
- ❌ Skip the verification build — catch configuration errors early
- ❌ Install fsdocs-tool globally — use local tool manifest for reproducibility

## Additional Resources

### Utility Scripts
- **`scripts/setup-fsdocs.sh`** — Automated setup: installs fsdocs-tool, creates directory structure, verifies installation

### Templates
- **`templates/index.md`** — Starter index page with placeholder content
- **`templates/directory-build.props`** — MSBuild properties template for documentation configuration

## Implementation Workflow

1. Read project structure — find `.sln`/`.fsproj`, check for existing docs setup
2. Run `scripts/setup-fsdocs.sh` from the project root
3. Customize `docs/index.md` with project-specific content from `templates/index.md`
4. Configure `Directory.Build.props` using `templates/directory-build.props` as reference
5. Append `.gitignore` entries for FSharp.Formatting output directories
6. Run `dotnet fsdocs build --clean` to verify the setup produces a working site

Focus on producing a minimal, working documentation site that other doc skills can build upon.

## Scope / when to use

Use this skill for the **one-time bootstrap** of an FSharp.Formatting site in an F#
project: installing `fsdocs-tool`, scaffolding `docs/`, and wiring MSBuild
properties. Once setup is done, switch to [[fsdocs-build]] to build/preview,
[[fsdocs-api-doc]] to document the API surface, [[fsdocs-examples]] for literate
`.fsx` tutorials, and [[fsdocs-technical]] for architecture/ADR prose. Do **not**
use this skill to author content or re-run builds — it is the installer, not the
authoring loop.

## Driven-library API

This skill drives the external **FSharp.Formatting** toolchain — there is no
FS.Skia.UI backing `.fsi` surface. The driven contract is the `dotnet fsdocs` CLI
(`build`, `watch`) plus the MSBuild property surface read by the tool
(`GenerateDocumentationFile`, `RepositoryUrl`, `FsDocsLicenseLink`,
`FsDocsReleaseNotesLink`, `FsDocsTheme`, `FsDocsWarnOnMissingDocs`, …) and the
`{{fsdocs-*}}` template substitution variables. The tool is pinned via the local
dotnet tool manifest (`.config/dotnet-tools.json`), which is the reproducibility
contract for the whole pipeline.

## Persistent-problem mandate

fsdocs property names, CLI flags, and template variables drift across releases.
Treat the tables here as a starting point and consult the
**official online docs first** for the version you have pinned before assuming a
property or flag is current. When a setup step fails, re-read the live docs rather
than guessing — this is a recurring, version-sensitive surface.

## Related

- [[fsdocs-build]] — build, preview, and troubleshoot once setup is complete
- [[fsdocs-api-doc]] — `///` XML doc comments consumed by the generator
- [[fsdocs-examples]] — literate `.fsx` scripts placed in the scaffolded `docs/`
- [[fsdocs-technical]] — Markdown architecture/ADR pages in `docs/`

## Sources

- FSharp.Formatting documentation: https://fsprojects.github.io/FSharp.Formatting/
- Command-line / `fsdocs` CLI reference: https://fsprojects.github.io/FSharp.Formatting/commandline.html
- FSharp.Formatting repository: https://github.com/fsprojects/FSharp.Formatting
