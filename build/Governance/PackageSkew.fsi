// PackageSkew.fsi — feature 087 (US2, FR-003/004), Principle II.
//
// A STATIC pinned-vs-local package-skew check: it compares the public-API symbols a
// generated template's source/tests reference against the captured public surface of the
// pinned package (the committed per-package `.fsi.txt` surface baselines a consumer
// relies on). A referenced FS.Skia.UI symbol that is ABSENT from that surface compiles
// against the locally-packed/unreleased package under `TemplateCheck` yet would fail
// against the pinned/published package under `GeneratedProductCheck` — the 086 near-miss
// (`ControlRenderResult.Bounds`). Pure comparison; the file reads live at the interpreter
// edge. NO network restore (the surface is already captured).
module FS.Skia.UI.Build.PackageSkew

open FS.Skia.UI.Build.Evidence

/// Extract the public-API symbol names declared in a captured `.fsi` surface baseline:
/// type/record/union/module/namespace-segment names, `member`/`val`/`abstract`/`new`
/// member names, union-case names, and record-field labels. This is the closed set of
/// symbols a consumer can resolve against the pinned/published package surface. Pure.
val surfaceSymbols: fsiText: string -> Set<string>

/// Extract the FS.Skia.UI public-API symbol references a generated source file makes, as
/// `(symbol, file)` pairs, scoped to `trackedPackages` (the surface-tracked consumer
/// package ids). Conservative by design (no type inference): a reference rooted at a
/// tracked package contributes its trailing symbol leaf; a reference rooted at a non-tracked
/// package — notably the build-tooling `FS.Skia.UI.Build` — contributes nothing. The real
/// tree (whose references all resolve in the captured surface) yields no finding while an
/// injected reference to a non-existent FS.Skia.UI symbol is caught. Pure.
val referencedSymbols: trackedPackages: Set<string> -> file: string -> sourceText: string -> (string * string) list

/// FR-003: a referenced symbol present in neither the pinned surface yields a
/// `PackageSkewFinding` naming the symbol, the referencing file, and the pinned-vs-local
/// version gap. `referenced ∩ (everything − pinnedSurface)` over the FS.Skia.UI references.
/// The real tree (all references ⊆ pinned surface) yields an empty list. Pure.
val detectSkew:
    pinnedVersion: string ->
    localVersion: string ->
    pinnedSurface: Set<string> ->
    referenced: (string * string) list ->
        PackageSkewFinding list

/// Render the skew findings as a deterministic human-readable report block (one line per
/// finding naming symbol + file + pinned-vs-local version gap), or a clean line when empty.
val renderFindings: pinnedVersion: string -> localVersion: string -> findings: PackageSkewFinding list -> string
