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
   - If FAKE fails after NuGet caches were cleared with a stale `.fake` cache,
     invalid assembly cache, or missing package folder, remove only `.fake` and rerun once:
     ```bash
     rm -rf .fake
     ./fake.sh build -t TemplatePack
     ```
   - Do not delete source files or reset the repo to fix FAKE cache problems.
   - After package version bumps, also verify that every current repo package
     exists in the local feed before validating generated projects. Check the
     exact package IDs and versions detected in step 2, for example:
     ```bash
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.Scene.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.SkiaViewer.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.Elmish.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.KeyboardInput.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.Layout.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.Controls.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.Controls.Elmish.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.Testing.<version>.nupkg"
     test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.<version>.nupkg"
     ```
   - If solution-level `dotnet pack` or a prior merge pack missed a packable
     repo package, pack that project explicitly before running generated
     restore/test validation. This has happened for
     `FS.Skia.UI.Controls.Elmish`, so prefer:
     ```bash
     dotnet pack src/Controls.Elmish/Controls.Elmish.fsproj -c Release -o ~/.local/share/nuget-local
     ```

6. Install the new template package.
   - Use the package created under `artifacts/templates/`.
   - Prefer uninstalling any currently installed `FS.Skia.UI.Template` before
     installing the freshly packed package. This keeps local validation
     deterministic and also handles duplicate template identities:
     ```bash
     dotnet new uninstall FS.Skia.UI.Template
     dotnet new install artifacts/templates/FS.Skia.UI.Template.<version>.nupkg
     ```
   - If no existing template is installed, `dotnet new uninstall` may report
     that there is nothing to uninstall; continue with the install.

7. Verify `dotnet new fs-skia-ui`.
   - Instantiate the default `app` profile into `/tmp` with git init disabled:
     ```bash
     rm -rf /tmp/fs-skia-ui-template-update-check
     dotnet new fs-skia-ui \
       --name TemplateUpdateCheck \
       --output /tmp/fs-skia-ui-template-update-check \
       --allow-scripts yes \
       --skipGitInit true
     ```
   - Inspect `/tmp/fs-skia-ui-template-update-check/Directory.Packages.props`
     and confirm the default `app` profile pins match the current package
     versions:
     - `FS.Skia.UI.Scene`
     - `FS.Skia.UI.SkiaViewer`
     - `FS.Skia.UI.Elmish`
     - `FS.Skia.UI.KeyboardInput`
     - `FS.Skia.UI.Layout`
     - `FS.Skia.UI.Controls`
     - `FS.Skia.UI.Controls.Elmish`
   - The template does not necessarily create a `.sln`; restore and test
     project files directly. The test project has its own restore graph, so
     restore the test project explicitly before using `--no-restore`, or allow
     `dotnet test` to restore with the local feed sources:
     ```bash
     dotnet restore /tmp/fs-skia-ui-template-update-check/src/TemplateUpdateCheck/TemplateUpdateCheck.fsproj \
       --source "$HOME/.local/share/nuget-local" \
       --source https://api.nuget.org/v3/index.json
     dotnet restore /tmp/fs-skia-ui-template-update-check/tests/TemplateUpdateCheck.Tests/TemplateUpdateCheck.Tests.fsproj \
       --source "$HOME/.local/share/nuget-local" \
       --source https://api.nuget.org/v3/index.json
     dotnet test /tmp/fs-skia-ui-template-update-check/tests/TemplateUpdateCheck.Tests/TemplateUpdateCheck.Tests.fsproj \
       --no-restore \
       --logger "console;verbosity=minimal"
     ```
   - Instantiate the `governed` profile as a required second check because it
     is the profile that carries `FS.Skia.UI.Testing`:
     ```bash
     rm -rf /tmp/fs-skia-ui-template-update-governed-check
     dotnet new fs-skia-ui \
       --name TemplateGovernedCheck \
       --profile governed \
       --output /tmp/fs-skia-ui-template-update-governed-check \
       --allow-scripts yes \
       --skipGitInit true
     ```
   - Inspect
     `/tmp/fs-skia-ui-template-update-governed-check/Directory.Packages.props`
     and confirm the governed profile pins match the current package versions:
     - `FS.Skia.UI.Scene`
     - `FS.Skia.UI.Testing`
   - Restore and test the governed profile through the same local-feed path:
     ```bash
     dotnet restore /tmp/fs-skia-ui-template-update-governed-check/src/TemplateGovernedCheck/TemplateGovernedCheck.fsproj \
       --source "$HOME/.local/share/nuget-local" \
       --source https://api.nuget.org/v3/index.json
     dotnet restore /tmp/fs-skia-ui-template-update-governed-check/tests/TemplateGovernedCheck.Tests/TemplateGovernedCheck.Tests.fsproj \
       --source "$HOME/.local/share/nuget-local" \
       --source https://api.nuget.org/v3/index.json
     dotnet test /tmp/fs-skia-ui-template-update-governed-check/tests/TemplateGovernedCheck.Tests/TemplateGovernedCheck.Tests.fsproj \
       --no-restore \
       --logger "console;verbosity=minimal"
     ```
   - Optionally instantiate `headless-scene` and `sample-pack` for a lighter
     package-pin smoke check when template conditionals changed:
     ```bash
     dotnet new fs-skia-ui --name TemplateHeadlessCheck --profile headless-scene --output /tmp/fs-skia-ui-template-update-headless-check --allow-scripts yes --skipGitInit true
     dotnet new fs-skia-ui --name TemplateSamplePackCheck --profile sample-pack --output /tmp/fs-skia-ui-template-update-sample-pack-check --allow-scripts yes --skipGitInit true
     ```

8. Commit and push.
   - Always commit and push after a successful template update validation.
   - Commit at least:
     - `.template.package/FS.Skia.UI.Template.fsproj`
     - `template/base/Directory.Packages.props`
     - any readiness files updated by `TemplatePack`
   - Suggested commit message:
     ```text
     Update fs-skia-ui template package pins
     ```
   - Push with `git push origin <branch>`.

## Validation Notes

- `dotnet new search fs-skia-ui` checks NuGet.org, not the local installed template. It may return no results and is not a failure for local template updates.
- Use `dotnet new uninstall` with no arguments to list installed local template packages and confirm `FS.Skia.UI.Template` version.
- Generated app restore should use `~/.local/share/nuget-local` when validating freshly packed local packages.
