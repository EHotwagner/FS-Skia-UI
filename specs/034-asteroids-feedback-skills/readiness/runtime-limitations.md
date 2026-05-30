# Runtime Limitations

command: `./fake.sh build -t Dev`
scanned files: `specs/034-asteroids-feedback-skills/plan.md`, `.agents/skills/fs-skia-layout-evidence/SKILL.md`, generated product guidance.
observed: this feature changes guidance, validation, readiness, and XML documentation only.
missing: none.
failure class: none.
next action: route runtime expansion requests to framework runtime follow-up work.

Current runtime boundaries:

- .NET 10 desktop is the supported validation platform for this repository.
- Vulkan remains the primary rendering backend covered by existing host checks.
- SkiaSharp preview remains the graphics dependency family in scope.
- unsupported macOS/mobile/browser platforms remain out of scope.
- no software-renderer fallback is added by this feature.

Deferred runtime scope includes stroke rasterization changes, text
rasterization changes, screenshot capture internals, host resize APIs,
auto-close persistent launch APIs, release publishing, runtime API shape
changes, and implementing a new Asteroids demo.
