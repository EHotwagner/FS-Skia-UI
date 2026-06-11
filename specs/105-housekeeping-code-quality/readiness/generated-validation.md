# Generated Validation (feature 105, housekeeping code-quality)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

Feature 105 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products: it is
a behaviour-preserving internal refactor of `src/**` `.fs` bodies (helper consolidation, redundant
`private` removal, internal closed-set DUs with string boundaries) with **zero** `template/**`
change and **zero** `.fsi` surface move. A generated project consuming `FS.Skia.UI.Controls` /
`FS.Skia.UI.Scene` / `FS.Skia.UI.SkiaViewer` therefore resolves the **same** package surface as
before — `package-resolution=resolved`, `package-mismatch=false`, `exact-package-match=true` — and
renders and behaves identically (byte-identical lowering/evidence output).

`generated-tests-exist=false` / `generated-tests-ran=not-applicable` because feature 105 introduces
no new generated-project test; `authoritative=false` because `GeneratedProductCheck` is **not** the
authoritative signal for this framework-internal change. `GeneratedProductCheck` is known to fail
locally for an **environment** reason (the generated Verify cannot resolve a feature; `Map.empty`
env) — that is recorded as a **non-authoritative environment-class** failure, not a product defect
(see [aggregate-hang-diagnostics.md](./aggregate-hang-diagnostics.md)). The authoritative signal is
`EvidenceAudit verdict=PASS` plus the unchanged framework suites under `Dev` and the routed
controls-public-surface gates.

## Route printout (T021)

```
developer-class=framework-author
tier=agent-ready
gates=Dev, PackageSurfaceCheck, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
dogfood-forced=false
matched-rules=controls-public-surface, evidence-governance, specify-catchall, docs-only
```

`Route` **escalated** to the `controls-public-surface` set, exactly as the plan predicted (the
feature-101/102 precedent: **any** `src/Controls/**/*.fs` edit escalates even with zero `.fsi`
delta). This is gate selection, not a surface delta — `git diff -- 'src/**/*.fsi'` is empty
(see [zero-surface-delta.md](./zero-surface-delta.md)). Per "run only the gates `Route` prints",
the 14 gates above are the authoritative set, run **sequentially** (shared `.fake` state).
