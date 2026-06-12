# Runtime limitations + permanent non-goals — feature 107 (governance-skew-doc-hardening)

## Supported runtime

The framework this feature governs runs on a **.NET 10 desktop** host rendering through **Vulkan** via
the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those are out of
scope for the framework and therefore for this feature.

Feature 107 itself adds **no** runtime code path, window, GPU, or wall-clock dependency. Both fixes are
pure text analyses in build tooling (`build/Governance`): FR-001 strips comments before the package-skew
symbol extraction, FR-002 captures the public typed front door recursively into the per-package surface
baseline, and FR-004/FR-005 replace the doc-preservation check's brittle placeholder sample with a
package-agnostic "≥1 preserved `///` summary" signal. The change is platform-independent and introduces
no new runtime failure mode.

## Public-surface scope (no product `.fsi` shape delta)

No product `val`/`type`/`member` signature changes (FR-007). The only `.fsi` edit is a doc-comment on
`build/Governance/PerPackageSurface.fsi` (build tooling, not a product surface). The captured
per-package surface baselines for `FS.Skia.UI.Controls` (+693) and `FS.Skia.UI.SkiaViewer` (+237) grow
**additively** (0 removed) to include their already-public subdirectory `.fsi` surface — a reviewed
surface-capture completeness fix, not a product contract change.

## Out of scope / permanent non-goals

- **The non-Controls boilerplate documentation cleanup** — deferred to a separate future feature; this
  feature only removes the landmine (the placeholder-sentence dependence) that would block it.
- **The feature-106 retrospective's third finding** (a planning artifact under-counting the doc surface,
  186 vs 356) — an authoring/process-discipline issue with no code fix, out of scope.
- **Any version/template change** — the merge flow bumps libraries as usual; this feature changes no
  package identity, version, or template pin (FR-007).
- **No skew or reference-generation architecture redesign** beyond the two narrow fixes; no new gate.

## Failure diagnostics

No new runtime failure path is introduced. The package-skew check still fails the build with an
actionable `readiness/package-skew.md` (per-finding symbol/file/pinned/local) on a genuinely-absent
symbol; the doc-preservation check still fails (naming the package) if reference generation drops `///`
summaries. Both are governance text-analysis failure classes, not product runtime failures.
