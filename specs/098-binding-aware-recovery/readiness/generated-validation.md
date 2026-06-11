# Generated Validation (098, R3)

exact-package-match=true
package-resolution=resolved
package-mismatch=false
generated-tests-exist=false
generated-tests-ran=not-applicable
authoritative=false
failure-class=none

## Why not-applicable

Feature 098 ships **nothing** into the `dotnet new fs-skia-ui` template or generated products (plan
§Generated project impact: N/A; §Dependency impact: N/A — no new dependency). The id unification, `BoundIds`
population, and `nearestAuthored` widening live inside the existing `FS.Skia.UI.Controls` /
`FS.Skia.UI.Controls.Elmish` packages, so a generated project consuming `runInteractiveApp` gains correct
unkeyed-button dispatch **automatically** with no scaffold change, no new selected-Controls guidance, and no
placeholder/excluded-history scan delta. The public additions (`ControlRenderResult.BoundIds`, `boundIdsOf`)
are additive framework surface; a generated project that does not read them renders byte-identically (the
`Scene`/`Bounds` rectangles are unchanged; only unkeyed `ControlId` labels move `Kind → path`, FR-007).

There is therefore no package mismatch to resolve and no generated-project test suite introduced by this
feature. `generated-tests-ran=not-applicable` because this change introduces no new generated-project test;
`authoritative=false` because `GeneratedProductCheck` is not the authoritative signal for this
framework-internal surface move (the authoritative signal is `EvidenceAudit verdict=PASS`). This run,
`GeneratedProductCheck` nonetheless completed `Status: Ok` — recorded in
[generated-guidance-validation.md](./generated-guidance-validation.md).
