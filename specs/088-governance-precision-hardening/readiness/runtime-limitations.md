# Runtime Limitations — Feature 088

`GeneratedProductCheck` (and its new `GeneratedConsumerValidation` sub-target) drive a real
consumer restore/build/`Verify` over generated projects. Locally this can fail for
**environment** reasons (the generated `Verify` cannot resolve an active feature: no template
`.specify/feature.json` + a `Map.empty` environment), which is a **non-authoritative
environment-failure**, NOT a product defect. Such a result is recorded in `logs/` and does not
block the agent-ready verdict; the authoritative evidence for this change is `Dev` +
`TargetMetadataDrift` + the Feature 088 governance tests, plus the byte-identical umbrella
composition asserted by the pure-transition tests.

## Platform runtime envelope (unchanged by this feature)

This feature is **build-tooling only** (`build/Governance/**`) — no product runtime, GPU, or
window surface change. The product runtime envelope it inherits is recorded here for the
readiness contract:

- **.NET 10 desktop** is the supported host (`net10.0`, Windows + Linux desktop).
- Live windows render through **Vulkan** via the **SkiaSharp preview** native backend.
- **unsupported macOS/mobile/browser** — those targets are out of scope; no headed window path
  is validated there.
- **no software-renderer fallback** — a headless/over-SSH environment without a GPU/display
  cannot present a live window (render-target PNG evidence still works headlessly). This is why
  a local `GeneratedProductCheck` consumer run can surface an environment-failure rather than a
  product defect.
