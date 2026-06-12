# Generated guidance validation (feature 108)

package-resolution=resolved
package-mismatch=false
guidance-drift=none

`GeneratedGuidanceCheck` validates that the generated-project guidance (AGENTS.md / CLAUDE.md / docs
shipped into `dotnet new fs-skia-ui`) is current. Feature 108 adds two template **doc** assets — the
`scaffold-map.md` interactive host-seam authority note (FR-019) and the new
`template/base/docs/interactive-readiness.md` interactive-feature readiness checklist (FR-020) — and
makes no guidance-behaviour change. The generated guidance therefore stays consistent with the shipped
surface (the host seam authority is the `fs-skia-controls-host` skill + `ControlsElmish.fsi`, exactly
as the note states). No placeholder, excluded-history, or `Dev`-behaviour change in the generated
project.
