# Research: Publish FS.Skia.UI to NuGet.org

All three unknowns from Technical Context are resolved below. No NEEDS CLARIFICATION remains.

## R1 — How `build.fsx` derives its engine reference from `<FsSkiaUiVersion>` at runtime

**Problem.** FR-004 requires **exactly one literal version value** in the generated project (the
`<FsSkiaUiVersion>` MSBuild property in `Directory.Packages.props`), and `build.fsx` must derive its
`FS.Skia.UI.Build` reference from it so the consumer upgrade is a single edit. Today `build.fsx:1` carries
its own literal: `#r "nuget: FS.Skia.UI.Build, 0.1.67-preview.1"`. The blocker: **F# `#r` directive
arguments must be string literals** — you cannot write `#r "nuget: FS.Skia.UI.Build, " + ver` or
interpolate a runtime value into the directive.

**Decision.** `build.fsx` reads `<FsSkiaUiVersion>` from `Directory.Packages.props` at runtime (regex over
the file it sits next to), locates the already-restored `FS.Skia.UI.Build` assembly for that version in the
NuGet global-packages folder
(`~/.nuget/packages/fs.skia.ui.build/<ver>/lib/net10.0/FS.Skia.UI.Build.dll`, honoring
`NUGET_PACKAGES`/`%USERPROFILE%`), and loads it with `System.Reflection.Assembly.LoadFrom` — **no `#r`
literal version**. The evidence entry points it needs are invoked through a thin reflection shim (or, if a
typed `open` is required, a single non-versioned `#r "nuget: FS.Skia.UI.Build"` is kept **only** as a
compile-time shape reference while the *runtime* binding comes from the property-resolved path). The one
literal version in the whole generated project is `<FsSkiaUiVersion>`; the consumer edits it and runs
`dotnet restore`, which restores both the CPM-managed library pins (they reference `$(FsSkiaUiVersion)`)
and the engine package, and `build.fsx` then loads the matching assembly.

**Rationale.** It satisfies "one literal, read at runtime" exactly, works within the `#r`-literal
constraint, and adds no new dependency — the engine is already restored as a CPM package, so the DLL is
guaranteed present after `dotnet restore`. The reflection load is the one spot where Principle III
(idiomatic simplicity) yields to a platform constraint; it is disclosed at the use site with a one-line
comment.

**Alternatives considered.**
- **Versionless `#r "nuget: FS.Skia.UI.Build"`** (no property at all): resolves the *latest* available
  version, not the pinned one — breaks reproducibility and the preview/stable channel guarantee. Rejected.
- **Generation-time substitution** (template writes the version into `build.fsx` at `dotnet new` time):
  reintroduces a second literal in the generated project, so the consumer's upgrade is two edits again —
  defeats FR-004. Rejected.
- **Codegen a `version.fsx` `#load`ed by `build.fsx`**: `#load` has the same literal constraint for `#r`
  and still needs the version threaded in; no improvement over reading the props directly. Rejected.
- **Convert `build.fsx` to a `build.fsproj` console program** with a CPM `<PackageReference>` resolving
  `$(FsSkiaUiVersion)`: cleanest typed binding, but a much larger change to the generated host surface and
  the `TemplateCheck`/`GeneratedProductCheck` contract. Held as a fallback if the reflection shim proves
  awkward in implementation; **not** chosen for the first cut.

## R2 — Staging/test feed for credential-free validation

**Decision.** A **throwaway local-directory feed** — a temp directory **distinct** from
`~/.local/share/nuget-local` (so the in-repo dev loop is untouched). `dotnet pack -o <stagingdir>` populates
it; `dotnet nuget push -s <stagingdir>` "publishes" to it (a directory feed push is a file copy); an
**anonymous read** is a directory listing of `<stagingdir>/<id>.<ver>.nupkg`. For nuget.org the anonymous
read is the **flat-container** API `https://api.nuget.org/v3-flatcontainer/<id-lowercase>/index.json`
(returns published versions; 404 ⇒ not published). Both reads need **no push credential**, satisfying
FR-002's "dry-run without a real publish credential."

**Rationale.** Matches the clarification ("throwaway local directory feed, credential-free, deterministic,
headless"), keeps the existing `~/.local/share/nuget-local` dev loop independent (FR-003 edge: in-repo
development unaffected), and makes dry-run/idempotency exercisable in CI/headless without secrets.

**Alternatives considered.** A hosted private NuGet server (e.g. BaGet) for staging — heavier, needs a
running service, defeats "deterministic, headless." Rejected. Reusing `~/.local/share/nuget-local` as the
staging feed — conflates the dev loop with release validation and risks false positives where a stale
local package masks a real gap. Rejected.

## R3 — Idempotency and the push edge

**Decision.** The push uses `dotnet nuget push <pkg> -s <feed> -k <apikey> --skip-duplicate`, which makes a
re-push of an already-present version a **no-op success** (nuget.org rejects duplicate versions; the flag
turns that rejection into a skip). Independently, **dry-run** computes the per-package skip/push decision by
the R2 anonymous read **before** any push, so it reports the plan with no credential and no network push,
and a partial-failure re-run pushes only the packages the read shows as missing. The pre-publish gate
(FR-006) runs **before** the push and aborts on inconsistency, so a malformed set never reaches the push
edge.

**Rationale.** `--skip-duplicate` is the SDK-native idempotency primitive (no custom version-existence
logic on the push path), while the anonymous read gives a credential-free, accurate dry-run plan and handles
the partial-set edge case. No new dependency.

**Alternatives considered.** Custom "query then push" without `--skip-duplicate`: races (a version could
appear between the read and the push) and reimplements what the flag already guarantees. Rejected as the
sole mechanism, but the anonymous read is still used for the **dry-run plan** where no push happens.

## Metadata baseline (FR-010) — what exists vs. what's missing

`Directory.Build.props` already supplies `Authors`, `Company`, `RepositoryType`, `PackageProjectUrl`,
`PackageLicenseExpression` (`MIT`), `PackageRequireLicenseAcceptance=false`, `GenerateDocumentationFile`.
Each lib `.fsproj` supplies `PackageId`, `Title`, `Description`, `PackageTags`, `Version`. **Missing for a
well-formed public listing**: `RepositoryUrl` (distinct from `PackageProjectUrl`), and a
**`PackageReadmeFile`** backed by a **per-package `README.md`** (none exist today). The template package
fsproj has `Title`/`Description`/`Authors`/`Tags` but no README. FR-010 + the pre-publish check therefore
add: `RepositoryUrl`, `PackageReadmeFile`, and 11 lib READMEs + 1 template README; `PackageIcon` is
optional (add a shared icon if low-cost, else omit). The pre-publish check treats license/repo-url/authors/
description/README as **required** (fail on blank) and tags/icon as recommended.
