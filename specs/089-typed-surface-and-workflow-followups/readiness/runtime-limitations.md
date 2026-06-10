# Runtime Limitations — Feature 089

This feature is **governance / docs / Spec-Kit skill tree** only
(`build/Governance/**`, `template/capabilities.yml`, the emitted
`docs/api-surface` tree, `src/Controls/catalog.yml`, and `.agents`/`.claude`
skill sources) — **no product runtime, GPU, or window surface change.** No
Skia/Vulkan code path is touched and no live window is launched.

`GeneratedProductCheck` drives a real consumer restore/build/`Verify` over
generated projects. Locally this can fail for **environment** reasons (the
generated `Verify` cannot resolve an active feature: no template
`.specify/feature.json` + a `Map.empty` environment), which is a
**non-authoritative environment-failure**, NOT a product defect. Such a result is
recorded in `logs/` and does not block the agent-ready verdict; the authoritative
evidence for this change is `Dev` + the currency gates + the Feature 089
governance tests.

## Platform runtime envelope (unchanged by this feature)

The product runtime envelope this feature inherits (and does not modify) is
recorded here for the readiness contract:

- **.NET 10 desktop** is the supported host (`net10.0`, Windows + Linux desktop).
- Live windows render through **Vulkan** via the **SkiaSharp preview** native backend.
- **unsupported macOS/mobile/browser** — those targets are out of scope; no headed
  window path is validated there.
- **no software-renderer fallback** — a headless/over-SSH environment without a
  GPU/display cannot present a live window. This feature opens no window, so this
  limitation does not affect its evidence (the published typed `.fsi` + the
  `TypedModule` catalog token are plain text artifacts proven by currency gates
  and unit tests).
