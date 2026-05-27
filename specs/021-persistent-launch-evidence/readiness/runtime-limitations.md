# Runtime Limitations

Feature: `021-persistent-launch-evidence`

- supported-host evidence requires a desktop session capable of creating a real
  SkiaViewer/Silk.NET window.
- .NET 10 desktop support is the target runtime for this feature.
- Vulkan is the primary graphics backend for the current SkiaViewer host.
- SkiaSharp preview packages are part of the current rendering stack.
- unsupported macOS/mobile/browser platforms are outside this feature scope.
- no software-renderer fallback is provided by this feature.
- current host facts for the successful launch included `WAYLAND_DISPLAY`,
  `DISPLAY`, `XDG_RUNTIME_DIR`, and `DBUS_SESSION_BUS_ADDRESS`.
- external title/window tools are optional diagnostics and are not authoritative
  over viewer-owned first-frame/window facts.
- deterministic render hashes, layout evidence, and image evidence remain
  separate proof types and do not replace persistent-launch evidence.
- malformed artifact parser fixtures are synthetic error-handling evidence only.
