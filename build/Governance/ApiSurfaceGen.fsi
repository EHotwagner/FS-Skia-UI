// ApiSurfaceGen.fsi — single-source generator + currency for the emitted
// `docs/api-surface/<PkgLeaf>/<file>.fsi` tree (feature 060, FR-003; Principle II).
//
// The generated tree is derived from `template/capabilities.yml` `contracts:` so the
// surface a consumer reads in a generated project is byte-identical to the real
// framework `.fsi`. The plan/currency are pure; the file reads/writes stay at the
// build interpreter edge (Principle IV).
module FS.Skia.UI.Build.ApiSurfaceGen

open FS.Skia.UI.Build.Capabilities

/// One emitted contract file in the generated `docs/api-surface/` tree.
type ApiSurfaceEntry =
    { PackageId: string
      /// last dotted segment of the package id (e.g. `Scene`) — the directory name.
      PkgLeaf: string
      /// repo-relative source `.fsi` (e.g. `src/Scene/Scene.fsi`).
      SourceFsi: string
      /// repo-relative emitted path (e.g. `template/base/docs/api-surface/Scene/Scene.fsi`).
      EmittedRelPath: string
      /// generated-project-relative path a skill names (e.g. `docs/api-surface/Scene/Scene.fsi`).
      ProjectRelPath: string
      Profiles: string list }

/// Repo-relative root of the emitted tree under the template base.
val emittedRoot: string

/// Generated-project-relative root a capability/product skill names.
val projectRoot: string

/// Derive the (deterministic, sorted) emitted entries from the capability catalog. Pure.
val plan: capabilities: CapabilityRow list -> ApiSurfaceEntry list

/// Currency diagnostics (empty == current). `readBytes` resolves a repo-relative
/// forward-slash path to its bytes; `existingEmitted` is every repo-relative file
/// currently under `emittedRoot`. Reports missing/stale emitted files and orphans. Pure.
val currency:
    entries: ApiSurfaceEntry list -> readBytes: (string -> byte[] option) -> existingEmitted: string list -> string list
