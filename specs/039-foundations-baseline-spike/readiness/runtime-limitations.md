# Runtime Limitations — 039

| Field | Value |
|---|---|
| Authoritative command | `git diff --name-only` over `src/**` (must be empty) |
| Artifact path | `specs/039-foundations-baseline-spike/readiness/logs/` |
| Failure class | runtime-untouched-invariant |
| Next action on failure | If any `src/**` path appears, the feature has exceeded scope — revert the runtime edit |

## Runtime platform limitations (inherited, unchanged by this feature)

This feature adds **no runtime behaviour** and changes none of the platform
limitations the framework already carries. They are restated here for the
readiness contract:

- Supported host: **.NET 10 desktop** (Linux and Windows). The new build-tooling
  front-end is a **.NET 10 desktop** `dotnet run` exe.
- Rendering stack: **Vulkan** presenter + **SkiaSharp preview**
  (`4.147.0-preview.3.1`) native assets. This feature does not touch the
  **Vulkan** path or the **SkiaSharp preview** packages.
- **unsupported macOS/mobile/browser**: the runtime targets desktop only;
  macOS, mobile, and browser are **unsupported macOS/mobile/browser** targets.
- **no software-renderer fallback**: there is **no software-renderer fallback**;
  a host without a working Vulkan/GPU path cannot render. Build tooling
  (`build/Build.fsproj`) is console-only and imposes no such requirement.

## This feature's own runtime limitation

**None to disclose.** It edits no `src/**` source, adds no persistent host
process, and introduces no stateful/I-O runtime workflow. **Principle IV
(MVU/effect boundary) is therefore Not Applicable** to every task (see
`readiness/effects-boundary.md` / T003 record). The only executable added is the
build-tooling front-end, whose single spike target prints one identifiable line
and exits — build tooling, not product runtime.
