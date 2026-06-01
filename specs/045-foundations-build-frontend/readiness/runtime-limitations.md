# Runtime Limitations (T003)

- **Authoritative command**: `./fake.sh build -t Dev` (focused per-project test runs authoritative
  when the aggregate matrix hits the documented headless hang).
- **Artifact path**: `readiness/logs/dev-gate.txt`, `readiness/aggregate-hang-diagnostics.md`.

## Standing runtime constraints (unchanged by this relocation)

The product runtime targets **.NET 10 desktop** with a **Vulkan** rendering backend on a
**SkiaSharp preview** build. The supported host is desktop only: **unsupported macOS/mobile/browser**
targets remain out of scope, and there is **no software-renderer fallback** — headless/CI hosts use
the deterministic scene-readback evidence path, not an interactive window.

This feature is a build-tooling relocation and introduces **no** new runtime limitation: the
compiled front-end runs the same targets the FSX front-end did. The only related note is that
`validateRunnerBootstrap` (VerifyPreflight) still probes `dotnet fake --version`; with `fake-cli`
removed that probe now reports a bootstrap warning rather than success — a known, expected
consequence of the toolchain rewire (validator-text trimming for it is deferred to Stage 6).

The aggregate `Dev` test matrix can hit the documented libdecor-gtk headless hang; per the
non-authoritative-aggregate policy the focused per-project test results (all green) are authoritative.
Captured: 2026-06-01T18:10:09Z
