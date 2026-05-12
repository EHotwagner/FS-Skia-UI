# Evidence Obligations

Feature: Vulkan Elmish Viewer

## Scope

- Tier: Tier 1 contracted public API.
- Public API impact: `src/Lib/Library.fsi` is the authority for public surface changes.
- MVU applicability: Required for viewer workflow, effects, subscriptions, diagnostics, screenshot capture, and host interpretation.
- Rendering scope: Vulkan-only. No OpenGL, CPU, software, browser, mobile, or fallback renderer path is permitted.
- Supported OS scope: Windows and Linux desktop only.

## Real-Evidence Requirements

- Setup and foundation tasks require real filesystem/build/test evidence.
- User-story tasks marked `[US*]` require a user-reachable exercise under `readiness/`.
- Vulkan smoke evidence must identify renderer path, first-frame timing, and fallback absence.
- Interpreter evidence must exercise real dependencies where safe. Synthetic fixtures require `[S]` status and Principle V disclosures.
- Package evidence must include a packed library consumer path, not only project references.

## Synthetic-Evidence Policy

Any mock, fake, in-memory substitute, canned response, unconnected interpreter, hardcoded future data source, or placeholder implementation forces `[S]` for the directly affected task. The code use site must include `SYNTHETIC:`, tests must carry the `Synthetic` token, and `tasks.md` must list the task in the Synthetic-Evidence Inventory.
