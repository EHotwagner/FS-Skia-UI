# Runtime Limitations

This feature validates generated desktop window presentation for the .NET 10 desktop runtime using the repo-owned Vulkan presenter and SkiaSharp preview packages.

## Supported Runtime

- .NET 10 desktop generated app.
- Vulkan-capable desktop session.
- SkiaSharp preview rendering stack used by the repository packages.
- Linux Wayland/X11 and Windows desktop sessions are supported when native display sockets, GPU/driver, and input focus are available.

## Unsupported Runtime

- unsupported macOS/mobile/browser targets.
- Headless sessions without `DISPLAY` or `WAYLAND_DISPLAY` for normal interactive windows.
- Browser, mobile, and non-desktop generated hosts.

## Renderer Limits

There is no software-renderer fallback for claiming supported-host visible-window success. Unsupported or unavailable Vulkan/desktop presentation must be reported as an environment/session, renderer, or window-visibility diagnostic rather than a successful interactive launch.
