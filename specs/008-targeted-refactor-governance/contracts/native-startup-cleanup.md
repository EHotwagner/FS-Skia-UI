# Contract: Native Startup and Cleanup

## Scope

This contract covers Vulkan viewer startup, resource acquisition, staged failure handling, cleanup ownership, and shutdown behavior inside the runtime implementation.

## Resource Categories

- Vulkan instance
- Vulkan presentation surface
- Physical/logical device and queues
- Swapchain and swapchain images
- Command pool and command buffers
- Fence/synchronization resources
- Staging buffers and device memory
- Skia GPU context and surfaces

## Startup Stage Rules

Each startup stage must declare:

- stage name and order
- input state required before running
- output state acquired by the stage
- resource ownership transfer point
- failure diagnostic severity and diagnostic stage
- cleanup obligations if the stage or a later stage fails

## Cleanup Rules

- Cleanup runs in reverse acquisition order.
- Each acquired resource is released exactly once on later-stage failure.
- Shutdown after successful startup releases all owned resources exactly once.
- Repeated shutdown or cleanup requests do not double-release resources.
- Failure diagnostics preserve the original native or host error message.

## Test Requirements

- A deterministic injectable failure case exists for every resource category.
- Each failure case records acquired resources, expected release order, observed release order, and diagnostic stage.
- Real native smoke coverage remains in place where the environment supports it.
- Synthetic/instrumented failure tests are disclosed in test names or readiness evidence when fake handles or fake acquisition are used.

## Evidence

- `specs/008-targeted-refactor-governance/readiness/native-startup-cleanup.md`
- `specs/008-targeted-refactor-governance/readiness/native-startup-cleanup-tests.txt`
- `specs/008-targeted-refactor-governance/readiness/native-smoke.txt`
