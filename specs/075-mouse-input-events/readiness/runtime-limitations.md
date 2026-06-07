# Runtime limitations

## Feature scope: an additive, host-independent coordination surface

This feature adds a pure pointer-coordination front door (`FS.Skia.UI.Controls`
`Pointer` — `init`/`toMsg`/`update`/`replay`), an MVU bridge
(`FS.Skia.UI.Controls.Elmish` `interpretPointerEffect`/`interpretPointerOutcome`),
and a host `ViewerEvent` contract extension (`FS.Skia.UI.SkiaViewer` —
mouse-button identity on press/release plus `PointerScrolled`/`PointerExited`).
`Pointer.update` is a pure reducer over the existing `Layout.hitTestComputed` and
`ControlRuntime`; it introduces no new layout, rendering, screenshot, Vulkan, or
Skia behavior. All interaction evidence is deterministic (scripted `PointerMsg`
sequences → asserted effects), independent of any GPU/window system.

## Inherited product runtime limitations (unchanged by this feature)

The shipped product runtime targets **.NET 10 desktop** on Windows and Linux and
renders through **Vulkan** on a **SkiaSharp preview** native build.
Platforms remain **unsupported macOS/mobile/browser**, and there is
**no software-renderer fallback**. This feature changes none of that.

## Host trigger limitation: `PointerExited`

The deterministic FR-007 cancel path is driven by the `WindowExited`/`FocusLost`
`PointerMsg` values and is fully proven by the reducer tests. At the host edge,
this Silk.NET version exposes **no mouse-leave event on `IMouse`**; the available,
reliable trigger is the window's `FocusChanged` (blur), which `Vulkan.fs` wires to
`PointerExited`. A consumer may also dispatch `FocusLost` directly for an
application-level focus handoff. This is a host-API constraint, not a defect — the
contract and the deterministic cancel behavior are unaffected.

## Sample visual proof: GPU/Vulkan smoke caveat

The `PointerInteractionGallery` sample's authoritative evidence is its
deterministic contract smoke (`readiness/sample-smoke/PointerInteractionGallery.txt`),
which exercises hover/click/secondary/drag/scroll through the public front door
with no GPU. Any window screenshot follows evidence-mode render-only honesty
rules; a Vulkan/window-system failure under a headless session is classified as an
environment condition, not a product defect.
