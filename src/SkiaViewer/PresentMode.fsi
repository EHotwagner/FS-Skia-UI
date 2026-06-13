namespace FS.Skia.UI.SkiaViewer

[<RequireQualifiedAccess>]
/// Selects how the live viewer presents each rendered frame. The default
/// (`OffscreenReadback`) is today's offscreen-render-plus-GPU→CPU-readback path and is
/// byte-identical to the pre-feature baseline; `DirectToSwapchain` is an opt-in path that
/// renders the Skia scene straight onto the acquired Vulkan swapchain image with no
/// per-frame readback, staging buffer/command pool, or `vkQueueWaitIdle` stall.
type ViewerPresentMode =
    /// Offscreen render then GPU→CPU readback then CPU→GPU upload to the swapchain image
    /// (the default, unchanged present path). Also the on-demand evidence/screenshot routine.
    | OffscreenReadback
    /// Render directly onto the acquired swapchain image via a backend render target; no
    /// per-frame readback, staging buffer/command pool, or full-queue stall. Opt-in;
    /// degrades safely to `OffscreenReadback` with a `Warning` diagnostic on init failure.
    | DirectToSwapchain
