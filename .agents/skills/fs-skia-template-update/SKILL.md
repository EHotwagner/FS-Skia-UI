---
name: fs-skia-template-update
description: Update and verify this repository's `dotnet new fs-skia-ui` template after package/version changes. Use when Codex is asked to refresh the local FS Skia UI template, update generated package pins, pack or install `FS.Skia.UI.Template`, validate `dotnet new fs-skia-ui`, or make the template consume the latest locally packed FS.Skia.UI packages.
---

# FS Skia Template Update

## Scope

Update the repo-owned `dotnet new fs-skia-ui` template and verify that generated projects consume the current local FS.Skia.UI package versions.

Primary files:

- `template/base/Directory.Packages.props`: generated product package pins.
- `.template.package/FS.Skia.UI.Template.fsproj`: template NuGet package version.
- `artifacts/templates/FS.Skia.UI.Template.<version>.nupkg`: packed template output.
- `specs/*/readiness/template/template-pack.log` and `template-package-contents.md`: may be updated by `TemplatePack`.

## Workflow

1. Confirm the working tree and current branch.
   - Use `git status --short --branch`.
   - If the user asked to push, expect to be on `master` unless they said otherwise.

2. Detect current packable package versions.
   - Inspect `src/*/*.fsproj` files with `<IsPackable>true</IsPackable>` or `<PackageId>`.
   - Use the latest versions already committed or freshly packed, for example:
     - `rg -n "<PackageId>|<Version>|<IsPackable>" -g "*.fsproj" src`
   - Do not invent versions. The template pins should match the current package versions for the repo packages.

3. Update generated product pins.
   - Edit `template/base/Directory.Packages.props`.
   - Update every `FS.Skia.UI*` `<PackageVersion ... Version="...">` entry to the current package version:
     - `FS.Skia.UI.Scene`
     - `FS.Skia.UI.SkiaViewer`
     - `FS.Skia.UI.Elmish`
     - `FS.Skia.UI.KeyboardInput`
     - `FS.Skia.UI.Layout`
     - `FS.Skia.UI.Controls`
     - `FS.Skia.UI.Controls.Elmish`
     - `FS.Skia.UI.Testing`
   - Leave non-repo packages such as `Expecto`, `Microsoft.NET.Test.Sdk`, and `YoloDev.Expecto.TestSdk` unchanged unless the user explicitly asks.

4. Bump the template package version.
   - Edit `.template.package/FS.Skia.UI.Template.fsproj`.
   - Increment the patch segment of `<Version>` by one, preserving the preview suffix.
   - Example: `0.1.12-preview.1` -> `0.1.13-preview.1`.

5. Pack the template.
   - Preferred command:
     ```bash
     ./fake.sh build -t TemplatePack
     ```
   - If FAKE fails after NuGet caches were cleared with a stale `.fake` cache or missing package folder, remove only `.fake` and rerun:
     ```bash
     rm -rf .fake
     ./fake.sh build -t TemplatePack
     ```
   - Do not delete source files or reset the repo to fix FAKE cache problems.

6. Install the new template package.
   - Use the package created under `artifacts/templates/`.
   - If the SDK reports duplicate `FS.Skia.UI.Template` identities, uninstall the old template package and reinstall the new one:
     ```bash
     dotnet new uninstall FS.Skia.UI.Template
     dotnet new install artifacts/templates/FS.Skia.UI.Template.<version>.nupkg
     ```

7. Verify `dotnet new fs-skia-ui`.
   - Instantiate into `/tmp` with git init disabled:
     ```bash
     rm -rf /tmp/fs-skia-ui-template-update-check
     dotnet new fs-skia-ui \
       --name TemplateUpdateCheck \
       --output /tmp/fs-skia-ui-template-update-check \
       --allow-scripts yes \
       --skipGitInit true
     ```
   - Inspect `/tmp/fs-skia-ui-template-update-check/Directory.Packages.props` and confirm the `FS.Skia.UI*` versions match the current package versions.
   - The template does not necessarily create a `.sln`; restore project files directly:
     ```bash
     dotnet restore /tmp/fs-skia-ui-template-update-check/src/TemplateUpdateCheck/TemplateUpdateCheck.fsproj \
       --source "$HOME/.local/share/nuget-local" \
       --source https://api.nuget.org/v3/index.json
     dotnet test /tmp/fs-skia-ui-template-update-check/tests/TemplateUpdateCheck.Tests/TemplateUpdateCheck.Tests.fsproj \
       --no-restore \
       --logger "console;verbosity=minimal"
     ```

8. Commit and push only if requested.
   - Commit at least:
     - `.template.package/FS.Skia.UI.Template.fsproj`
     - `template/base/Directory.Packages.props`
     - any readiness files updated by `TemplatePack`
   - Suggested commit message:
     ```text
     Update fs-skia-ui template package pins
     ```
   - Push with `git push origin <branch>` when the user requested publishing.

## Validation Notes

- `dotnet new search fs-skia-ui` checks NuGet.org, not the local installed template. It may return no results and is not a failure for local template updates.
- Use `dotnet new uninstall` with no arguments to list installed local template packages and confirm `FS.Skia.UI.Template` version.
- Generated app restore should use `~/.local/share/nuget-local` when validating freshly packed local packages.
