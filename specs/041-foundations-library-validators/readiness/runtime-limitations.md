# Runtime Limitations — 041

| Field | Value |
|---|---|
| Authoritative command | `git diff --name-only` over `src/**` (must be empty for this feature) |
| Artifact path | `specs/041-foundations-library-validators/readiness/logs/` |
| Failure class | runtime-untouched-invariant |
| Next action on failure | If any `src/**` path appears, the feature exceeded scope — revert the runtime edit |

## Runtime platform limitations (inherited, unchanged)

This feature adds **no runtime behaviour** and changes none of the platform
limitations the framework carries:

- Supported host: **.NET 10 desktop** (Linux and Windows).
- Rendering stack: **Vulkan** presenter + **SkiaSharp preview**
  (`4.147.0-preview.3.1`). Untouched.
- **unsupported macOS/mobile/browser**: desktop only.
- **no software-renderer fallback**: a host without a working Vulkan/GPU path
  cannot render. Build tooling is console-only and imposes no such requirement.

## This feature's own runtime limitation

**None to disclose.** It edits no `src/**` source (`git diff --stat -- src` is
empty, SC-007), ships nothing in any generated product (FR-012), and adds no
persistent host process. The only executable behaviour added is build-tooling:
two validators (capability catalog + target-metadata drift) and the typed
`Targets.Target` dispatch moved into the compiled `build/Governance` library and
called in-process from `build.fsx`. `YamlDotNet 17.1.0` (already pinned) reads
the capability YAML behind the typed model — a build-tooling dependency, not a
product runtime one. No `FSharp.Compiler.*` is introduced.
