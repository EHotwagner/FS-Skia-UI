---
name: fsdocs-build
description: This skill should be used when the user asks to "build docs", "build documentation", "run fsdocs", "preview docs", "watch docs", "fsdocs build errors", "fix doc build", or needs to build, preview, or troubleshoot an FSharp.Formatting documentation site. Covers build commands, watch mode, common errors, and build flags.
version: 0.1.0
---

# Build and Preview FSharp.Formatting Documentation

Build a documentation site with `dotnet fsdocs`, preview it locally, and troubleshoot common build issues.

**Key capabilities:**
- Build documentation with appropriate flags for the situation
- Launch watch mode for live preview during authoring
- Diagnose and fix common fsdocs build errors
- Handle eval mode for literate scripts with executable output

## Overview

FSharp.Formatting builds static HTML from three inputs: API docs (from compiled assemblies and XML doc comments), Markdown files in `docs/`, and literate `.fsx` scripts in `docs/`. The build process compiles projects, extracts documentation, and renders everything into `output/`.

## Workflow

### 1. Verify Tool Installation

Confirm `fsdocs-tool` is available:

```
dotnet tool restore
dotnet fsdocs --help
```

If not installed, use the `fsdocs-setup` skill first.

### 2. Build Projects in Release Mode

API documentation is generated from compiled assemblies. Build in Release mode to ensure the output matches what users will see:

```
dotnet build -c Release
```

This step is essential — fsdocs reads the compiled DLLs and their XML doc files.

### 3. Run fsdocs Build

Standard build:

```
dotnet fsdocs build --clean
```

Build with script evaluation (for literate `.fsx` files that use `include-output`, `include-it`, or `include-value`):

```
dotnet fsdocs build --clean --eval
```

### 4. Check Build Output

Review the build output for:
- **Errors** — compilation failures, missing references, broken cross-references
- **Warnings** — missing doc comments, unresolved `cref` links
- **Generated pages** — confirm expected files appear in `output/`

### 5. Report Results

Summarize what was generated: number of API pages, content pages, any errors or warnings that need attention.

## Build Flags Reference

| Flag | Purpose | When to Use |
|---|---|---|
| `--clean` | Remove previous output before building | Always on first build or when changing structure |
| `--eval` | Evaluate `.fsx` scripts and capture output | When literate scripts use output directives |
| `--strict` | Treat warnings as errors | CI builds, pre-release verification |
| `--noapidocs` | Skip API doc generation | When only editing content pages |
| `--nonpublic` | Include non-public members in API docs | Internal documentation builds |
| `--nodefaultcontent` | Skip default content generation | When using fully custom content |
| `--input <dir>` | Override input directory | Non-standard project layouts |
| `--output <dir>` | Override output directory | Custom deployment paths |
| `--properties <props>` | Pass MSBuild properties | Override `Directory.Build.props` values |

## Watch Mode

For live preview during authoring:

```
dotnet fsdocs watch
```

This starts a local server (default port 8901) and rebuilds on file changes. Open `http://localhost:8901` in a browser.

Watch mode options:
- `--port <n>` — use a different port
- `--eval` — evaluate scripts on each rebuild (slower but shows output)
- `--noapidocs` — skip API regeneration for faster content iteration

Typical authoring workflow: run `dotnet fsdocs watch --noapidocs` for fast content iteration, then do a full `dotnet fsdocs build --clean --eval` before committing.

## Common Errors and Fixes

### Missing XML Documentation File
**Symptom:** API pages are empty or missing.
**Cause:** `GenerateDocumentationFile` not set to `true` in project properties.
**Fix:** Add `<GenerateDocumentationFile>true</GenerateDocumentationFile>` to `Directory.Build.props`.

### Compilation Failure During Build
**Symptom:** `fsdocs build` fails with compile errors.
**Cause:** Projects don't compile in Release mode, or a dependency is missing.
**Fix:** Run `dotnet build -c Release` separately to isolate the issue.

### Broken Cross-References
**Symptom:** Warnings about unresolved `cref` links in build output.
**Cause:** Incorrect `cref` syntax or referencing non-public members.
**Fix:** Use fully qualified names: `cref:T:Namespace.TypeName` for types, `cref:M:Namespace.TypeName.MethodName` for methods.

### Script Evaluation Failures
**Symptom:** `.fsx` files fail during `--eval`.
**Cause:** Missing `#r` references, runtime errors in scripts, or scripts depending on external state.
**Fix:** Ensure all assembly references are present. Use `(*** do-not-eval ***)` for scripts that cannot be evaluated.

### Missing Assembly References in .fsx
**Symptom:** `Type not found` or `Namespace not opened` during eval.
**Cause:** Literate scripts need explicit assembly references.
**Fix:** Add `#r "../src/MyProject/bin/Release/net8.0/MyProject.dll"` at the top of the script.

### Stale Output
**Symptom:** Changes don't appear in the built site.
**Cause:** Cached build output.
**Fix:** Use `--clean` to force a fresh build.

## Best Practices

**DO:**
- ✅ Always build projects in Release mode before running fsdocs
- ✅ Use `--clean` when structure or navigation has changed
- ✅ Use `--noapidocs` during content-only authoring for faster iteration
- ✅ Run a full `--eval` build before committing to verify all scripts work
- ✅ Check build output for warnings — they often indicate broken cross-references

**DON'T:**
- ❌ Skip the Release build step — fsdocs needs compiled assemblies
- ❌ Use `--eval` in watch mode unless actively working on script output
- ❌ Ignore cross-reference warnings — they produce broken links in the site
- ❌ Commit the `output/` directory — it is a build artifact
- ❌ Run fsdocs from a subdirectory — always run from the solution root

## Additional Resources

### Utility Scripts
- **`scripts/fsdocs-build.sh`** — Build wrapper with error handling, Release compilation, and result reporting

## Implementation Workflow

1. Verify `fsdocs-tool` is installed (`dotnet tool restore`)
2. Build all projects in Release mode (`dotnet build -c Release`)
3. Run `dotnet fsdocs build` with appropriate flags for the situation
4. Review build output for errors and warnings
5. Report results — pages generated, issues found, suggested fixes

Focus on producing a successful build and clear diagnostics when issues arise.

## Scope / when to use

Use this skill to **build, preview, or troubleshoot** an existing FSharp.Formatting
site — running `dotnet fsdocs build`, watch-mode previews, choosing flags, and
diagnosing build/eval errors. If the project has no fsdocs setup yet, run
[[fsdocs-setup]] first. To create the content that this skill builds, use
[[fsdocs-api-doc]] (XML doc comments), [[fsdocs-examples]] (literate `.fsx`), or
[[fsdocs-technical]] (Markdown). This skill is the build/diagnose loop, not a
content authoring skill.

## Driven-library API

This skill drives the external **FSharp.Formatting** `dotnet fsdocs` CLI — there is
no FS.Skia.UI backing `.fsi` surface. The driven contract is the CLI verb +
flag surface: `build` and `watch`, with `--clean`, `--eval`, `--strict`,
`--noapidocs`, `--nonpublic`, `--nodefaultcontent`, `--input`, `--output`,
`--properties`, and (watch) `--port`. It depends on compiled assemblies plus their
emitted XML doc files, so `dotnet build -c Release` is a precondition of every
build.

## Persistent-problem mandate

fsdocs CLI flags and error messages change across tool versions, and broken
`cref` cross-references fail silently. Consult the **official online docs first**
for the pinned tool version before relying on a flag documented here, and re-read
the live docs when a build error does not match the troubleshooting table — this is
a recurring, version-sensitive surface.

## Related

- [[fsdocs-setup]] — install `fsdocs-tool` and scaffold `docs/` before building
- [[fsdocs-api-doc]] — author the `///` comments that become API pages
- [[fsdocs-examples]] — literate `.fsx` scripts built with `--eval`
- [[fsdocs-technical]] — Markdown pages picked up by the build

## Sources

- FSharp.Formatting documentation: https://fsprojects.github.io/FSharp.Formatting/
- Command-line / `fsdocs` CLI reference: https://fsprojects.github.io/FSharp.Formatting/commandline.html
- FSharp.Formatting repository: https://github.com/fsprojects/FSharp.Formatting
