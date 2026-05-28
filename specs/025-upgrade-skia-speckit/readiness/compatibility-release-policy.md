# Compatibility Release Policy

Evidence:

- Compatibility analysis source: `docs/2026-05-27-2217-compatibility-package-analysis.md`
- Consumer inventory: `specs/025-upgrade-skia-speckit/readiness/compatibility-consumer-inventory.md`
- Public-surface map: `specs/025-upgrade-skia-speckit/readiness/compatibility-public-surface-map.md`

near-term posture: `FS.Skia.UI` remains a stable compatibility surface.

User-facing package-choice guidance:

- Prefer focused packages for new generated products and new docs:
  `FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Elmish`,
  `FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Layout`, `FS.Skia.UI.Controls`,
  `FS.Skia.UI.Controls.Elmish`, and `FS.Skia.UI.Testing`.
- Keep `FS.Skia.UI` for existing broad-package samples, compatibility tests,
  and consumers that need legacy viewer/Vulkan presenter behavior.
- Do not remove or deprecate broad-package public members during this upgrade.

unknown external consumers: external usage cannot be proven from repository
evidence, so this upgrade assumes compatibility-sensitive consumers exist and
keeps package identity and public surface stable.

deferred decisions:

- permanent broad package versus facade versus deprecation,
- member-by-member deprecation annotations,
- sample migrations away from `FS.Skia.UI`,
- external consumer telemetry,
- package publishing or release automation.
