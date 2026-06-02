# Runtime limitations

command: `./fake.sh build -t Dev` (build host) + `./fake.sh build -t PerPackageSurfaceDiff`
scanned files: `src/**`, `tests/**`, `build/Governance/**`, `readiness/surface-baselines/**`.
observed: the retired `FS.Skia.UI` monolith (`src/Lib`) was deleted; no runtime moved this stage
(all runtime relocated and was parity-proven in Stages 1–4). This is a deletion + governance/
enforcement change only.
failure class: RuntimeLimitation.
next action: none — no runtime behaviour changes this stage; deletion + governance only.

- This is a **.NET 10 desktop** build-host change. The split packages keep their identities and
  couple to the **Vulkan**/Skia host (`FS.Skia.UI.SkiaViewer.Host`) exactly as before; no host,
  **Vulkan**, or **SkiaSharp preview** rendering behaviour changes — the monolith held only the
  `Parity` evidence helper by Stage 4, which retires with it.
- Targets remain **unsupported macOS/mobile/browser**; this feature does not change platform support.
- The deterministic scene-output parity oracle is headless and re-derives byte-identically to the
  Stage-0 golden; it is preserved in the split-package suites and is authoritative. Reference-frame
  re-capture stays headless-GPU-infeasible (disclosed corroboration, not synthetic): there is
  **no software-renderer fallback** for the persistent Vulkan host in CI, so scene-output is the
  authoritative oracle.
