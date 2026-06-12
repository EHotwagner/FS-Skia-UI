# Contract: Package-skew check (FR-001 / FR-002 / FR-003)

Governance interface in `build/Governance/PackageSkew.fs(.fsi)` and
`build/Governance/PerPackageSurface.fs(.fsi)`. This is **not** a product API surface; it is the
governance contract the new tests pin.

## `PackageSkew.referencedSymbols`

```fsharp
val referencedSymbols :
    trackedPackages: Set<string> -> file: string -> sourceText: string -> (string * string) list
```

**Behavior (post-fix):**
- Comments (`//`, `///`, `(* … *)` nested) in `sourceText` are stripped before extraction; an
  `FS.Skia.UI.*` token that appears **only** inside a comment contributes **no** pair. *(FR-001)*
- A token that appears in both a comment and live code still contributes via its live-code
  occurrence. *(edge case)*
- References rooted at a non-tracked package (e.g. `FS.Skia.UI.Build`) still contribute nothing
  (unchanged `rootedInTracked` semantics).
- Signature shape unchanged. If a shared comment-stripper is exposed, it is additive to the `.fsi`.

## `PerPackageSurface.captureCurrent`

```fsharp
val captureCurrent : packageId: PackageId -> string   // normalized concatenated surface text
```

**Behavior (post-fix):**
- Enumerates `*.fsi` **recursively** (`SearchOption.AllDirectories`) under the package source dir,
  ordered deterministically by relative path. For `FS.Skia.UI.Controls` this now includes
  `src/Controls/Widgets/*.fsi` (the typed front door). *(FR-002)*
- Output is **additive** vs. the prior baseline (symbols added, none removed). The regenerated
  `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` is a reviewed surface-capture change,
  not a product contract change. *(FR-007)*

## `PackageSkew.detectSkew` (unchanged shape)

```fsharp
val detectSkew :
    pinnedVersion: string -> localVersion: string -> pinnedSurface: Set<string>
        -> referenced: (string * string) list -> PackageSkewFinding list
```

**Invariants the tests pin:**
- A referenced symbol absent from `pinnedSurface` still yields a `PackageSkewFinding` — the seeded
  `FS.Skia.UI.Controls.ControlRenderResult.UnreleasedBoundsV087` reference is still detected. *(FR-003)*
- After capture broadening, `open FS.Skia.UI.Controls.Typed` and
  `FS.Skia.UI.Controls.Typed.<Module>.<member>` produce **no** finding. *(FR-002)*
- The narrowing introduces no path by which a genuinely-absent symbol resolves. *(FR-003)*
