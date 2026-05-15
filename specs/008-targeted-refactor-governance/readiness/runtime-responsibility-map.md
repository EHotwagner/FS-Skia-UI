# Runtime Responsibility Map

## Current Responsibility Areas

| Area | Current Location | Accepted Strategy | Reviewer Notes |
|------|------------------|-------------------|----------------|
| Public facade and primitive model | `src/Lib/Library.fsi`, `src/Lib/Library.fs` | Keep facade stable. | Public type and module names remain in the existing signature. |
| Scene state and diagnostics | `src/Lib/Library.fs` modules `Diagnostics`, `Scene`, `Parity` | Named-section fallback. | These modules are tightly coupled to public discriminated unions and records. Moving them would risk public-surface churn. |
| Drawing helpers | `src/Lib/Library.fs` inside `VulkanHost` | Named-section fallback. | Skia conversion and draw helpers depend on private scene representation. |
| Native resource ownership | `src/Lib/VulkanResources.fsi`, `src/Lib/VulkanResources.fs` | Paired internal helper files. | Pure ownership ledger supports deterministic cleanup tests without public API changes. |
| Startup stage model | `src/Lib/VulkanStartup.fsi`, `src/Lib/VulkanStartup.fs` | Paired internal helper files. | Stage order and synthetic failure cases are testable through friend assembly access. |
| Frame flow and screenshots | `src/Lib/Library.fs` inside `VulkanHost` | Named-section fallback. | Depends on native handles and the host loop; physical split is deferred unless a later spec accepts the compile-order cost. |
| Viewer hosting | `src/Lib/Library.fs` module `Viewer` and `VulkanHost.run` | Named-section fallback plus flatter startup pipeline. | `Viewer.run` remains the public entry point. |

## Internal Helper Contract Strategy

`VulkanResources` and `VulkanStartup` are assembly-internal modules declared in
paired signature files. They are compiled before `Library.fs` and exposed to
`Lib.Tests` through `InternalsVisibleTo`, but they must not appear in package
surface baselines.

The broader `Library.fs` split falls back to named sections for this feature
because the public facade, private scene representation, Skia drawing helpers,
and host loop share many local types. The accepted refactor still reduces review
risk by isolating native ownership and startup-stage logic without moving public
contracts.
