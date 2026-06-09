# Window visibility (084)

- **Authoritative command**: `./fake.sh build -t EvidenceAudit` (window-visibility scan over the seven readiness files).
- **Artifact path**: `readiness/interactive-visible-window.md`, `window-state-diagnostics.md`, `window-options.md`, `close-reason-separation.md`, `real-image-evidence.md`, `generated-validation.md`, `evidence-audit.md`.
- **Failure class**: a missing required file, missing token, taskbar/process-only substitution, or unsupported-host-only visible-window claim is a window-visibility blocker.
- **Next action**: the window-visibility scan passes (0 hits) with honest render-only/deferred records; the real visible-window launch is captured on a display-capable host (see `interactive-visible-window.md`).

The complete seven-file window-visibility contract is now discoverable from the
shipped `docs/evidence-formats.md` (FR-007/SC-003), single-sourced from
`EvidenceFormatSchema.windowVisibilityFiles`.
