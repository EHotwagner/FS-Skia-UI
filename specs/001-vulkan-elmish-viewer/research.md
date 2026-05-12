# Research: Vulkan Elmish Viewer

## Decision: Pin SkiaSharp to 4.147.0-preview.2.1

**Rationale**: NuGet lists `SkiaSharp 4.147.0-preview.2.1` as the latest prerelease available on 2026-05-12, with explicit `net10.0` compatibility. Microsoft's .NET blog describes SkiaSharp 4.0 as a major preview aligned to a newer Skia engine, and the GitHub release notes identify the 4.147 preview line as the Skia milestone 147 upgrade.

**Alternatives considered**:

- `4.147.0-preview.1.1`: rejected because NuGet now lists `preview.2.1` as newer.
- Stable `3.119.x`: rejected because the feature explicitly requires the SkiaSharp 4 preview line.

**Sources**:

- https://www.nuget.org/packages/SkiaSharp/4.147.0-preview.2.1
- https://devblogs.microsoft.com/dotnet/welcome-to-skia-sharp-40-preview1/
- https://github.com/mono/SkiaSharp/releases/tag/v4.147.0-preview.1.1

## Decision: Reference explicit Windows and Linux native asset packages

**Rationale**: The spec limits first-version support to Windows and Linux desktop. Pinning `SkiaSharp.NativeAssets.Win32` and `SkiaSharp.NativeAssets.Linux` to the same preview version makes runtime asset ownership explicit and avoids accidental broad platform positioning.

**Alternatives considered**:

- Rely only on transitive SkiaSharp native assets: rejected because the library needs clear platform packaging behavior.
- Include macOS/mobile/browser assets: rejected because those targets are out of scope.

**Sources**:

- https://www.nuget.org/packages/SkiaSharp.NativeAssets.Win32/4.147.0-preview.2.1
- https://www.nuget.org/packages/SkiaSharp.NativeAssets.Linux/4.147.0-preview.2.1

## Decision: Use Silk.NET 2.23.0 for Vulkan, windowing, and input interop

**Rationale**: Silk.NET provides maintained .NET bindings for Vulkan plus high-level windowing and input packages. NuGet identifies `Silk.NET.Windowing 2.23.0` as the current package line and its release notes mention Vulkan 1.4.336 binding updates. This keeps window, input, and Vulkan interop in one dependency family.

**Alternatives considered**:

- Hand-written Vulkan P/Invoke: rejected because it creates high maintenance cost and broad unsafe surface.
- Avalonia/MAUI/WinUI Skia views: rejected because they introduce non-Vulkan fallback behavior or platform-specific UI stacks outside the requested Vulkan-only viewer.
- Silk.NET 3 preview: rejected for first version because the stable 2.23 package line is available and current enough for Vulkan 1.4 bindings.

**Sources**:

- https://www.nuget.org/packages/Silk.NET.Windowing/
- https://www.nuget.org/packages/Silk.NET.Vulkan/
- https://www.nuget.org/packages/Silk.NET.Input/

## Decision: Use Fable.Elmish 4.2.0 concepts and types where suitable

**Rationale**: Elmish documents the Model-View-Update architecture, command dispatch, and subscriptions. The constitution prefers the Elmish package when `Program`, `Cmd`, subscriptions, or renderer integration are useful. `Fable.Elmish 4.2.0` is the latest listed package and targets .NET Standard 2.0, so it is usable from the net10.0 library.

**Alternatives considered**:

- Local MVU algebra only: rejected for the first plan because the user explicitly asked for Elmish, and the constitution prefers the Elmish runtime when useful.
- React/Fable renderer packages: rejected because this is a desktop Vulkan viewer, not a browser UI.

**Sources**:

- https://elmish.github.io/elmish/
- https://elmish.github.io/elmish/docs/subscription.html
- https://www.nuget.org/packages/Fable.Elmish/

## Decision: Treat Vulkan initialization as an edge interpreter responsibility

**Rationale**: The pure `update` function should not create windows, devices, queues, swapchains, or Skia GPU contexts. It should emit effects such as `InitializeRenderer`, `RenderFrame`, `CaptureScreenshot`, and `Shutdown`; the host interpreter executes those effects and dispatches result messages. This satisfies the constitution's MVU boundary and makes startup failures testable as data.

**Alternatives considered**:

- Initialize Vulkan inside `init` or `update`: rejected because it hides I/O in pure transitions and makes semantic tests brittle.
- Let sample apps own all Vulkan setup: rejected because the library must provide a reusable viewer.

## Decision: Use fail-fast diagnostics for unsupported renderer environments

**Rationale**: The specification requires no fallback renderer. Startup must stop before partial viewer display when Vulkan is unavailable and return structured diagnostics naming the failing step.

**Alternatives considered**:

- Continue running with a blank window: rejected because it is silent degradation.
- Fall back to CPU or OpenGL Skia surfaces: rejected by the feature scope.

## Decision: Test through pure transitions, public `.fsi`, packed library, and smoke samples

**Rationale**: Constitution rules require `.fsi` first, semantic tests, and real evidence. Pure MVU tests cover `update`; contract tests load the package/prelude; smoke samples validate packaging and runtime behavior. Vulkan-capable smoke runs should be separated from CI jobs that lack GPU access.

**Alternatives considered**:

- Unit-test private Vulkan helpers only: rejected because it bypasses public behavior.
- Mark headless CI as fully green without real Vulkan evidence: rejected unless explicitly disclosed as synthetic evidence.
