# Runtime limitations

- authoritative command: `./fake.sh build -t Dev` (build + the repointed governance/semantic suites)
- artifact path: `specs/051-relocate-agentvalidation/readiness/logs/dev.log`
- failure class: BuildHostTooling vs ProductDefect — this is a build/governance-tooling relocation; no
  runtime/Vulkan/Skia/host code path is touched.
- next action: on any race-like aggregate FAKE failure, re-run the affected FAKE-backed command in
  focused isolation; the repointed `AgentValidationFrameworkTests` suite (same assertion count) is the
  authoritative behavioural-parity oracle.

Statements:

- The relocated `AgentValidation` capability is **.NET 10 (`net10.0`) build-host governance tooling**,
  compiled into `FS.Skia.UI.Build`; it is BCL-only (`System.Diagnostics`/`IO`/`Text.Json`) and never
  shipped to a generated product.
- **No runtime surface is touched**: no Vulkan, no SkiaSharp, no persistent viewer host, no scene /
  layout / rendering code changes. There is no GPU, window, or display dependency in this feature.
- Target platforms for the build host are **Windows and Linux**; the move runs identically on both.
- Headless CI fully covers this change — there is no persistent-window or reference-frame capture in
  scope, so no environment is recorded as infeasible (Principle V): all evidence is real and reproducible.

Runtime platform invariants (unchanged by this feature, recorded for the readiness contract):

- The viewer host target platform is **.NET 10 desktop** (`net10.0`), **Windows and Linux** only.
- Rendering is **Vulkan** via **SkiaSharp preview** native assets.
- There is **no software-renderer fallback**; startup fails fast with a structured `RenderDiagnostic`
  when Vulkan is unavailable.
- **unsupported macOS/mobile/browser**: macOS, mobile, and browser hosts are not supported for the
  persistent viewer host. None of these runtime paths are modified by this build-tooling relocation.
