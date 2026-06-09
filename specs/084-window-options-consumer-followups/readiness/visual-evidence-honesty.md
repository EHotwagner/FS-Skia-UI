# Visual-evidence honesty (084)

- **Authoritative command**: `dotnet fsi` against the built `FS.Skia.UI.SkiaViewer` (surface) / generated executable on a display host (window).
- **Artifact path**: `readiness/fsi-session.txt` (surface), `readiness/real-image-evidence.md` (decodable launch image, deferred to display host).
- **Failure class**: a metadata-only/1×1 fallback claimed as visual proof, or a render-only result reported as a visible window, is a visual-evidence-honesty defect.
- **Next action**: keep window claims honest — `deferred`/`render-only` on this framework dev host; real decodable windowed-fullscreen screenshot on a display-capable host at merge.

This feature opens no desktop window from the framework repo itself; the new
windowed-fullscreen default + Honored reclassification are proven against the built
library (real). No false visible-window claim is made (spec Edge case: headless →
honest render-only).
