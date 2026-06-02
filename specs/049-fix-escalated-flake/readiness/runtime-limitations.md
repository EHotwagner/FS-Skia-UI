# Runtime limitations (T003)

This feature changes **only** how the build front-end selects a graphics backend in
headless/unsupported environments; it changes no product runtime path and no visual
output. The standing runtime-limitation statements are reproduced here for
completeness and remain unchanged.

- **.NET 10 desktop**: the framework targets `net10.0` desktop hosts.
- **Vulkan**: the SkiaSharp viewer backend renders through Vulkan.
- **SkiaSharp preview**: built on a preview SkiaSharp; APIs may shift.
- **Unsupported**: macOS, mobile, and browser/WASM hosts are **not supported**
  (unsupported macOS/mobile/browser). This feature adds **no** new host support.
- **No software-renderer fallback**: there is no software rasteriser fallback; a host
  without a working graphics surface cannot run the viewer. This feature does **not**
  add such a fallback (explicitly out of scope).

## Deterministic graphics-backend selection in headless / dual-display environments

The project's headless host advertises **both** a Wayland display
(`WAYLAND_DISPLAY`) and an X11/Xvfb display (`DISPLAY`) at once. The graphics stack
then prefers the Wayland path and tries to load `libdecor-gtk.so`, which cannot
initialize in the container — causing a teardown crash and a ~20-minute
graphics-init stall. The build front-end now classifies the ambient display state and,
**only** when both displays are advertised (DualDisplay), forces the already-working
X11 path: it removes `WAYLAND_DISPLAY` and sets `GDK_BACKEND=x11` /
`SDL_VIDEODRIVER=x11`. This normalization is self-applied at front-end startup and
re-applied at every process-spawn edge, so it propagates to `dotnet test`, FSI, and
nested `bash ./fake.sh build -t <target>` descendants.

- Authoritative command: `./fake.sh build -t GeneratedProductCheck` (and the full
  escalated serialized order) run once on the headless host with no manual env prefix.
- Artifact path: `readiness/logs/generated-product-check.log`,
  `readiness/aggregate-hang-diagnostics.md`, `readiness/graphics-env-contract.md`.
- Failure class: a graphics-initializing step that still cannot start fails fast
  within its existing bounded timeout with a diagnostic naming the probable
  graphics-backend initialization failure (never an indefinite hang) — distinct from
  a product regression, whose nonzero exit code is propagated unchanged.
- Next action: read the timeout diagnostic; if it names a probable backend-init
  failure, treat it as an environment limitation (this file); otherwise treat the
  nonzero exit as a genuine product/test failure.

## Safety boundary on already-working hosts (T007, FR-007 / SC-004)

The dual-display guard mutates the environment **only** for the DualDisplay
classification. Under **WaylandOnly** (real Wayland desktop), **X11Only** (already on
the working path), and **Neither** (non-Linux / headless-no-display), `classifyDisplay`
returns the corresponding state and `normalizeGraphicsEnv` is the **identity** — so
backend selection, behavior, and visual output are **unchanged** from before this
feature on every host that already works. No host gains new support; no host loses any.

Authoritative evidence: the green single-display / no-display unit cases in
`tests/Governance.Tests/GraphicsEnvironmentTests.fs` —
`WaylandOnly is identity`, `X11Only is identity`, `Neither is identity`, `empty map is
total and unchanged`, and the `preserves every key outside the three named keys`
FsCheck property — all pass (8/8 in the `Graphics environment normalization` list,
196 ms). Determinism is therefore guaranteed by the pure guard (unit-proven), not by
repetition (SC-001).

