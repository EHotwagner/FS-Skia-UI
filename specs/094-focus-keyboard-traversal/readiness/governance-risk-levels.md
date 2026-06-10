# Governance risk levels

This is a **Tier 1** change (public surface moves): a new public `src/Controls/Focus.fsi` and the
internal `routeFocusedKey` contract + `runInteractiveApp` doc on `src/Controls.Elmish/ControlsElmish.fsi`.

- **small** — the pure `Focus.fs` reducer logic (`order` / `traverse` / `route`) and the R1
  `Accessibility.fs` correction (Tab out of per-control `NavigationKeys`; activation-only `Button`
  valid). Focused validation: `Dev` + the targeted `Controls.Tests` suites
  (`Feature094FocusTests`, `AccessibilityTests`). No consumer-contract surface moves here.
- **medium** — the `routeFocusedKey` host wiring + representative widget metadata + retained-identity
  focus binding. Focused validation: `Dev` + the adapter route-probe (`Feature094FocusRoutingTests`),
  the E1 text-seam regression, and the live-retained stability test.
- **broad** — the public `Focus.fsi` + the `Controls.Elmish` `.fsi` surface move escalate to
  controls-public-surface / package-surface. **broad validation** is the **required evidence** before
  merge: the full escalated gate list `Route` prints —
  `Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck,
  ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck,
  ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift,
  EvidenceGraph, EvidenceAudit` — run **sequentially** (shared `.fake` state). The aggregate result
  is **non-authoritative** and recorded as such in
  [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md); the authoritative verdict is
  `EvidenceAudit verdict=PASS`.
