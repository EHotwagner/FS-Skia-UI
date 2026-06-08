---
name: fs-skia-template-update
description: Update and verify this repository's `dotnet new fs-skia-ui` template after package/version changes. Use when Codex is asked to refresh the local FS Skia UI template, update generated package pins, pack or install `FS.Skia.UI.Template`, validate `dotnet new fs-skia-ui`, or make the template consume the latest locally packed FS.Skia.UI packages.
---

# FS Skia Template Update

## Scope

Update the repo-owned `dotnet new fs-skia-ui` template and verify generated projects consume the current local FS.Skia.UI package versions.

Primary files:

- `template/base/Directory.Packages.props`: generated product package pins.
- `.template.package/FS.Skia.UI.Template.fsproj`: template NuGet package version.
- `artifacts/templates/FS.Skia.UI.Template.<version>.nupkg`: packed template output.
- `specs/*/readiness/template/template-pack.log` and `template-package-contents.md`: may be updated by `TemplatePack`.

## Driven surface

This skill drives the `dotnet new fs-skia-ui` template and the FAKE template
targets in `FS.Skia.UI.Build`. The template-validation surface is the
`FS.Skia.UI.Build` targets `TemplatePack` (packs `FS.Skia.UI.Template`) and
`TemplateCheck` (validates generated projects against the current package pins).
There is no hand-written `.fsi` contract for the template itself; the template's
contract is the generated `Directory.Packages.props` pin set and the
`FS.Skia.UI.Build` target metadata that enforces it.

## Workflow

1. Confirm the working tree and current branch with `git status --short --branch`.
   If the user asked to push, expect to be on `main` unless they said otherwise.

2. Detect current packable package versions.
   - Inspect `*.fsproj` files with `<IsPackable>true</IsPackable>` or `<PackageId>`,
     scanning both `src/` and `build/`. `FS.Skia.UI.Build` lives in
     `build/Governance/FS.Skia.UI.Build.fsproj`, not under `src/`, and is
     pinned by the template like the other repo packages:
     - `rg -n "<PackageId>|<Version>|<IsPackable>" -g "*.fsproj" src build`
   - All repo packages share a single version, so one value drives every pin.
   - Do not invent versions; pins must match the current repo package versions.

3. Update the generated product pin — a single edit (Feature 064 / FR-004).
   - The generated template has exactly **one** FS.Skia.UI version literal:
     the `<FsSkiaUiVersion>` property in `template/base/Directory.Packages.props`.
     Every `FS.Skia.UI.*` `<PackageVersion>` pins `Version="$(FsSkiaUiVersion)"`,
     so bumping that one property moves all of them at once. Edit only that value:
     ```bash
     sed -i 's|<FsSkiaUiVersion>[^<]*</FsSkiaUiVersion>|<FsSkiaUiVersion><new-version></FsSkiaUiVersion>|' template/base/Directory.Packages.props
     ```
   - **Do not** edit `template/base/build.fsx`. Post-064 it carries no `#r`
     version literal: it reads `<FsSkiaUiVersion>` from `Directory.Packages.props`
     at runtime (regex near line 60) and constructs the engine `PackageReference`
     dynamically, so the props property is the single source of truth and the two
     cannot drift by construction. (This supersedes the old Feature 054 two-literal
     parity check.) `GovernanceTests.fs` asserts `build.fsx` resolves the engine
     from `FsSkiaUiVersion`, so reintroducing a literal would fail the build.
   - The single property drives every template-pinned `FS.Skia.UI*`
     `<PackageVersion ... Version="$(FsSkiaUiVersion)">` entry (eleven packages):
     - `FS.Skia.UI.Build`
     - `FS.Skia.UI.SkillSupport`
     - `FS.Skia.UI.Scene`
     - `FS.Skia.UI.Color`
     - `FS.Skia.UI.SkiaViewer`
     - `FS.Skia.UI.Elmish`
     - `FS.Skia.UI.KeyboardInput`
     - `FS.Skia.UI.Layout`
     - `FS.Skia.UI.Controls`
     - `FS.Skia.UI.Controls.Elmish`
     - `FS.Skia.UI.Testing`
   - `FS.Skia.UI.Input` is **packable but not template-pinned** — it ships to the local
     feed (so it appears in the step-5 feed loop below) but the generated
     `Directory.Packages.props` does not reference it, so it is **not** in the list above.
   - After the edit, confirm no stray old-version literal survives elsewhere in
     the template: `grep -rn '<old-version>' template/base` should return nothing.
   - Leave non-repo packages such as `Expecto`, `Microsoft.NET.Test.Sdk`, and `YoloDev.Expecto.TestSdk` unchanged unless the user explicitly asks.

4. Bump the template package version in `.template.package/FS.Skia.UI.Template.fsproj`:
   increment the patch segment of `<Version>` by one, preserving the preview
   suffix. Example: `0.1.12-preview.1` -> `0.1.13-preview.1`.

5. Pack the template.
   - Preferred command:
     ```bash
     ./fake.sh build -t TemplatePack
     ```
   - If FAKE fails after NuGet caches were cleared (stale `.fake` cache,
     invalid assembly cache, or missing package folder), remove only `.fake` and rerun once:
     ```bash
     rm -rf .fake
     ./fake.sh build -t TemplatePack
     ```
   - Do not delete source files or reset the repo to fix FAKE cache problems.
   - After bumping, verify every current repo package exists in the local feed
     before validating generated projects. The loop enumerates the **full packable
     set** (twelve projects: the props-pinned eleven plus the packable-but-unpinned
     `Input`); it is the authoritative package list and is checked against the
     packable `.fsproj` set by `TemplateUpdateSkillPackageCheck`, so it cannot drift.
     Loop over the exact package leaves and the version detected in step 2:
     ```bash
     v=<version>
     for pkg in Build Scene Color SkiaViewer Elmish KeyboardInput Input Layout Controls Controls.Elmish Testing SkillSupport; do
       test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.$pkg.$v.nupkg" && echo "OK  $pkg" || echo "MISS $pkg"
     done
     ```
   - Do **not** add a suffix-less bare-Lib feed probe: feature 053 deleted `src/Lib`
     and unpublished the leaf-less `FS.Skia.UI` package, so no such `.nupkg` is produced.
   - If solution-level `dotnet pack` or a prior merge pack missed a packable
     repo package, pack that project explicitly before generated
     restore/test validation. This has happened for
     `FS.Skia.UI.Controls.Elmish`, so prefer:
     ```bash
     dotnet pack src/Controls.Elmish/Controls.Elmish.fsproj -c Release -o ~/.local/share/nuget-local
     ```

6. Install the new template package from `artifacts/templates/`.
   - Uninstall any currently installed `FS.Skia.UI.Template` first; this keeps
     local validation deterministic and handles duplicate template identities:
     ```bash
     dotnet new uninstall FS.Skia.UI.Template
     dotnet new install artifacts/templates/FS.Skia.UI.Template.<version>.nupkg
     ```
   - If no template is installed, `dotnet new uninstall` may report nothing to
     uninstall; continue with the install.

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
     and confirm the default `app` profile pins match current versions:
     - `FS.Skia.UI.Build`
     - `FS.Skia.UI.Scene`
     - `FS.Skia.UI.SkiaViewer`
     - `FS.Skia.UI.Elmish`
     - `FS.Skia.UI.KeyboardInput`
     - `FS.Skia.UI.Layout`
     - `FS.Skia.UI.Controls`
     - `FS.Skia.UI.Controls.Elmish`
   - The template does not necessarily create a `.sln`; restore and test
     project files directly. The test project has its own restore graph, so
     restore it explicitly before using `--no-restore`, or let `dotnet test`
     restore with the local feed sources:
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
   - Instantiate the `governed` profile as a required second check — it is the
     profile that carries `FS.Skia.UI.Testing`:
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
     and confirm the governed profile pins match current versions:
     - `FS.Skia.UI.Build`
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

8. Commit and push after successful template-update validation.
   - Commit at least:
     - `.template.package/FS.Skia.UI.Template.fsproj`
     - `template/base/Directory.Packages.props` (the single `<FsSkiaUiVersion>` bump)
     - any readiness files updated by `TemplatePack`
   - Suggested commit message:
     ```text
     Update fs-skia-ui template package pins
     ```
   - Push with `git push origin <branch>`.

## Validation Notes

- `dotnet new search fs-skia-ui` checks NuGet.org, not the locally installed template; no results there is not a failure for local updates.
- `dotnet new uninstall` with no arguments lists installed local template packages and confirms the `FS.Skia.UI.Template` version.
- Generated app restore should use `~/.local/share/nuget-local` when validating freshly packed local packages.

## Related

- [[fs-skia-project]]
- [[fsharp-build-orchestration]]

## Sources / links

- Custom .NET templates (`dotnet new`): https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates
- F# / .NET documentation: https://learn.microsoft.com/en-us/dotnet/fsharp/

## Persistent problems

When a problem outlasts reasonable in-repo attempts, extensive external research is
**mandatory** — consult **official online docs first** (the F#/.NET docs and the driven
library's own documentation/API reference), then community sources (forums, Reddit, Q&A
sites, issue trackers and changelogs). Record the findings and resolving links in the
feature's `specs/<feature>/feedback/` folder and, for durable lessons, in this skill's
**Sources** line. Offline, the mandate degrades to recording "research blocked — <why>"
rather than hard-failing the phase.
