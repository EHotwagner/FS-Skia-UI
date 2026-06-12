# Runtime limitations & failure diagnostics (feature 112)

## Documented evidence path

Feature 112 is a per-frame visual-state **stamp mechanism** change proven by deterministic, headless
evidence; a live Vulkan window is **not required** (spec *Unsupported scope* / Assumptions). The
asserted surfaces:

- the internal `ControlRuntime.applyRuntimeVisualStateTargeted` / `runtimeStampFor` seam, exercised from
  `Controls.Tests` via `InternalsVisibleTo` — scene parity vs the full-tree oracle, the touched-node
  count, consumer-set precedence, and the route selection;
- the standing Scene-parity golden suite under `Dev` for at-rest rendered-output + geometry byte-identity.

A live window CAN open via the X11 path ([[live-vulkan-window-x11-path]]), but it is not part of this
feature's required evidence — the runtime-state stamp runs only on the live host, and the deterministic
`Perf.runScript` corpus stamps visual state inline via the model (not via the runtime bridge), so the
authoritative deterministic evidence for the targeted stamp + its count is the `Controls.Tests`
function-level assertion, not a live window. The live `renderRetained` wiring routes its decision through
the pure `runtimeStampFor` helper, which is itself deterministically tested; the live render staying
byte-identical is covered by the Scene-parity suite under `Dev`.

## Out-of-scope / deferred (spec *Unsupported scope*)

This feature is **Phase 4 only**. Explicitly deferred: view/control memoization + stable-dependency
diagnostics (Phase 5); viewport virtualization (Phase 6); damage rects / picture / paint caches
(Phase 7); text / layout-boundary caches (Phase 8); `SkiaViewer` backend / render-thread / compositor
review (Phase 9). The full-tree `applyRuntimeVisualState` stamp is **not removed** (preserved as oracle/
fallback). Narrowing the reconciler DIFF (vs the stamp) is OUT. Features 110/111 (retained routing,
scheduler/view-skip) are unchanged. The targeted path degrades to the full oracle on a model-change /
first / structurally-misaligned frame (never a stale render, FR-006).

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A scene-parity regression fails `Feature112TargetedStampParityTests` (targeted vs full-tree oracle).
- A re-introduced whole-tree stamp makes `RuntimeStateTouchedNodeCount` jump to the node count and fails
  `Feature112TouchedCountTests`.
- A precedence regression fails `Feature112PrecedenceTests` (consumer-set Disabled/Selected must win).

## Platform / runtime support boundary

Feature 112 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are out
of scope. The 112 evidence is GPU-free deterministic stamp/parity assembly, so it does not depend on the
live Vulkan surface.
