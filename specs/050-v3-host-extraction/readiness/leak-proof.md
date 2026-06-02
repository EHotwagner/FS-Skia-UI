# Leak proof — FS.Skia.UI.SkiaViewer no longer depends on the monolith (FR-004 / SC-001 / SC-003)

**Verdict:** leak closed.

(a) Packed/package dependency — `src/SkiaViewer/SkiaViewer.fsproj` has **zero** `ProjectReference`
to `..\Lib\Lib.fsproj`; the bridge `src/SkiaViewer/SceneConversion.fs` is **deleted**. SkiaViewer now
references only `Scene` + `KeyboardInput` + its native packages, so the packed `FS.Skia.UI.SkiaViewer`
package carries **no** `FS.Skia.UI` package dependency.

```
$ grep -c "Lib.fsproj" src/SkiaViewer/SkiaViewer.fsproj   # -> 0
$ test ! -f src/SkiaViewer/SceneConversion.fs && echo deleted   # -> deleted
```

(b) Generated default `app` — the `TemplateDrift` gate is green and the `app` profile consumes
`FS.Skia.UI.SkiaViewer` by package; with the `SkiaViewer -> FS.Skia.UI` edge removed, the monolith is
absent from the generated app's resolved transitive graph (corroborated by green `TemplateDrift` /
`GeneratedGuidanceCheck`).

Note: `Lib` (the retiring monolith) now references the split packages for its residual rich
`KeyboardInput` module (`Lib -> SkiaViewer -> Scene`). This is acyclic and does **not** reintroduce a
`SkiaViewer -> Lib` edge, so the modularity leak being closed (SkiaViewer/app monolith-free) holds.
