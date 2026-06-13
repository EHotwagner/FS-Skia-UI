# Safe fallback (feature 118, US1 / FR-005 / SC-005)

**What FR-005 requires:** on any direct-path init/wrap failure, fall back to the proven
`OffscreenReadback` present path from that frame onward, emit a `Warning` diagnostic with the
cause, and never crash or present a corrupt frame.

**How it is exercised (real backend, not a mock):** `DirectToSwapchain` is selected on a real
Vulkan GPU; `probeDirectWrap` (`Vulkan.fs:1176`) attempts `SKSurface.Create` on an acquired
swapchain image. On this SkiaSharp build the wrap genuinely fails (returns null — SkiaSharp
#1502), so the real error path is taken — this is a **real forced failure on a real backend**,
not a synthetic substitution.

**Observed behaviour (`FEATURE118_MODE=direct` live run):**
- Exactly one `Warning` diagnostic, `Stage = VulkanSwapchain`, naming the cause and the binding
  limitation (mono/SkiaSharp #1502) and that the viewer uses `OffscreenReadback`.
- The run completes normally: `RESULT: ok frames=40 captured=true` — no crash, no abort, no
  corrupt/garbage frame; 40 frames present via the readback fallback.
- The probe runs **once** at init (`DirectPresentState.Attempted`), so the Warning is emitted
  once, not per frame; `Available` stays false for the swapchain's lifetime → all subsequent
  frames use readback (FR-005 "for that frame onward").

**Decision recorded:** safety wins over performance (the spec's conflict resolution). The
readback path is the already-proven default; degrading to it with an observable `Warning` is the
honest, non-crashing failure mode. No resources are leaked: `initDirectPresent` probes wrap
capability *before* allocating the command pool / transition buffers / semaphores, so a failed
probe allocates nothing.

- **Failure class:** binding-limitation / unsupported direct present → `Warning`, degrade to
  `OffscreenReadback` (not a product defect, not a crash).
- **Next action:** the readback-free direct path becomes available with the OpenGL backend
  (`../audit/opengl-backend-resolution.md`); the same fallback guard then protects any residual
  unsupported-format case.
