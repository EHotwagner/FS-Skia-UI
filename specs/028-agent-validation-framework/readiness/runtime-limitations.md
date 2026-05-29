# Runtime Limitations

Status: PASS

- .NET 10 desktop: repository builds and tests currently run on the installed
  .NET 10 SDK/runtime, with .NET 6 SDK present for FAKE package compatibility.
- Vulkan: graphical viewer evidence may depend on host Vulkan availability and
  should report unsupported host diagnostics when unavailable.
- SkiaSharp preview: rendering behavior may depend on the current SkiaSharp
  package/runtime combination and must remain covered by focused rendering or
  generated evidence gates.
- unsupported macOS/mobile/browser: this repository's current governed
  validation is desktop/non-visual focused; macOS, mobile, and browser hosts
  are not claimed by this feature.
- no software-renderer fallback: unsupported graphical hosts must report
  unsupported-environment diagnostics rather than silently claiming equivalent
  software-rendered evidence.

Container note: this container also has a runtime-only system dotnet host under
`/usr/share/dotnet`. `fake.sh` sets
`FAKE_SDK_RESOLVER_CUSTOM_DOTNET_PATH=/home/developer/.dotnet` so FAKE 6.1.4
uses the SDK-bearing dotnet root in this environment.
