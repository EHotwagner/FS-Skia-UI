# Generated guidance validation (T030) — escalated gate run (non-authoritative aggregate)

The full escalated gate list `Route` printed for this Tier-1 surface move, run as **sequential**,
non-concurrent `./fake.sh build -t <target>` invocations (shared `.fake` state). Each verdict below
is the per-target sequential result; the aggregate is **non-authoritative** until the final
`EvidenceAudit verdict=PASS` (see [evidence-audit.md](./evidence-audit.md) and
[aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)).

| Gate | Verdict | Note |
|------|---------|------|
| `Dev` | Ok | full build + all tests (Controls 236/236, Elmish 46/46, incl. the new 094 suites) |
| `PackageSurfaceCheck` | Ok | recaptured controls-public-surface (additive `Focus`) |
| `PerPackageSurfaceDiff` | Ok | recaptured per-package (`Focus` public; internal `routeFocusedKey`) |
| `FsiTranscripts` | Ok | the `Focus` FSI transcript is current |
| `GeneratedGuidanceCheck` | Ok | |
| `TemplateDrift` | Ok | |
| `DesignTokenDrift` | Ok | no new token (focus indicator resolves through E3's `Focused`) |
| `ContrastCheck` | Ok | no new token-derived colour |
| `ControlsCatalogCheck` | Ok | no new catalog control kind |
| `ControlsCatalogGenerationCheck` | Ok | |
| `ControlsInteractionCheck` | Ok | |
| `ControlsRenderingCheck` | Ok | |
| `GeneratedProductCheck` | Ok | incl. `TemplateCheck` / `TemplateInstantiate` / `TemplateSmoke` / `Test` |
| `EvidenceGraph` | Ok | no cycles / dangling refs / `[S*]` |
| `EvidenceAudit` | PASS | authoritative merge-gate verdict |

failure-class: none. All FAKE-backed targets were run sequentially; no concurrent FAKE context.
