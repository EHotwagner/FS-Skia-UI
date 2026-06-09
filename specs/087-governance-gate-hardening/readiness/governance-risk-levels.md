# Governance risk levels — feature 087

Risk classification for the change surface, mapped to required evidence and the
broad validation each level demands. This feature changes only the compiled
governance engine `FS.Skia.UI.Build` (`build/Governance/**`) plus the generated
`template/base/docs/evidence-formats.md` and the `Routing.fs`-derived
`validation.contract.yml` — no `src/**/*.fsi` public surface, no product runtime
change.

| Level | Scope in 087 | Required evidence | Broad validation |
|-------|--------------|-------------------|------------------|
| small | a single pure-function change with a focused `Governance.Tests` case (T024 `Graph.propagate` over `ExplicitDeps`, T020 three-state `Audit.verdict`) | the targeted Expecto/FsCheck test green; no broad rerun | the focused `Governance.Tests` case |
| medium | a gate's effect shaping or schema/contract change (T009/T010 `GeneratedProductCheck`, T013/T014 skew + `PackageSet`, T017 `RefreshSurfaceBaselines`, T021 audit records, T027/T028 provenance) | the owning FAKE target green plus its `Governance.Tests` | the owning FAKE target (`GeneratedProductCheck`, `TemplateCheck`, `RefreshSurfaceBaselines`, `EvidenceAudit`) |
| broad | `Routing.fs`/contract regeneration or the FR-011 cross-gate sweep (T030–T032) | the full serialized six-target order; non-authoritative aggregate results recorded with per-step classification | escalated six-target serialized order + EvidenceGraph + EvidenceAudit |

Authoritative tier: this feature escalates to **broad validation** because it
touches governance paths (`build/Governance/**`), the generated
`template/base/docs/evidence-formats.md`, and regenerates `validation.contract.yml`
from `Routing.fs`. Run only the gates `Route` prints; the escalated serialized
order is the broad-validation path. Broad validation is required only when
governance routing or multiple gates change in one run (T030–T032); focused
single-function changes use small/medium validation.
