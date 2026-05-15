# Native Startup Cleanup Evidence

This file is updated by implementation and verification tasks.

| Stage | Resource Category | Owner | Transfer Point | Cleanup Rule | Evidence |
|-------|-------------------|-------|----------------|--------------|----------|
| VulkanInstance | Vulkan instance | startup ledger | after successful instance acquisition | release once in reverse acquisition order | `NativeStartupCleanupTests` |
| VulkanSurface | presentation surface | startup ledger | after successful surface acquisition | release once before instance release | `NativeStartupCleanupTests` |
| VulkanDevice | logical device and queues | startup ledger | after successful device acquisition | release once before surface release | `NativeStartupCleanupTests` |
| VulkanSwapchain | swapchain and images | startup ledger | after successful swapchain acquisition | release once before device release | `NativeStartupCleanupTests` |
| FrameRender | command pool/buffers, fences, staging buffers/memory | frame owner | per-frame acquisition | release once before frame exits | `NativeStartupCleanupTests` |
| SkiaContext | Skia GPU context and surfaces | startup ledger | after successful Skia context acquisition | dispose before swapchain destruction | `NativeStartupCleanupTests` |

Synthetic fixture disclosure: deterministic acquisition failures use symbolic
resource handles and do not touch a real Vulkan driver. Real native smoke is
recorded separately in `native-smoke.txt` when supported by the environment.

Verification result: focused native startup cleanup tests passed in
`specs/008-targeted-refactor-governance/readiness/logs/lib-tests.txt`.

Real smoke result: `native-smoke.txt` records a successful Vulkan startup,
first frame, and `fallback-used=false`.
