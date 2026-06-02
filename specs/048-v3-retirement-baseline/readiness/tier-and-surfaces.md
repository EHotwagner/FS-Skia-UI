# Feature 048 — Tier, surfaces, MVU applicability, real-evidence obligations (T001)

## Tier

- **Tier 1 for the governance/build surface only** — adds one new curated
  `build/Governance/PerPackageSurface.fsi`, the `PerPackageSurfaceDiff` target, a
  `Routing.fs` rule, and new baseline artifacts.
- **Tier 2-equivalent for the runtime** — no runtime `.fsi`, no package identity/version,
  no rendering behaviour change (FR-010 / FR-011 / SC-007).
- `Route` **escalates** the change (governance/build paths); as a V3-programme dogfood
  feature it runs the full serialized gate set **plus** `PerPackageSurfaceDiff`.

## Affected surfaces

- `docs/reports/_baselines/2026-06-02-v3-before.md`
- `tests/Parity.Tests/fixtures/v3-host-golden/**`
- `readiness/per-package-surface/**`
- `readiness/per-package-surface-expectations.md`
- `build/Governance/PerPackageSurface.fs(i)`
- `build/Governance/Targets.fs(i)` / `Engine/Model.fs(i)` / `Engine/Update.fs`
  (a `Routing.fs` rule was **deferred** — see the runtime-coupling finding in
  `runtime-untouched.md` and `readiness/per-package-surface-expectations.md`)
- `build/Governance/Engine/Interpret.fs` / `build/Governance/Front/Governance.fs`
- `build/Governance/FS.Skia.UI.Build.fsproj` (compile order + DiffPlex PackageReference)
- `tests/Governance.Tests/PerPackageSurfaceTests.fs`
- `tests/Parity.Tests/**` (new scene-output re-derivation test)
- `docs/adr/0007`–`0011`
- `specs/048-v3-retirement-baseline/readiness/**`

## Public-API impact

No runtime `.fsi` change. Exactly one new **governance** `.fsi`
(`build/Governance/PerPackageSurface.fsi`). The per-package baselines are **captured**
descriptive artifacts of the unchanged public surfaces, not modifications to them.

## Elmish/MVU applicability

**N/A.** The capability is a **pure** `diff` with file reads confined to a thin edge
interpreter (`captureCurrent` / `loadBaselines` / `runReport`). There is no
`Model`/`Msg`/`Cmd`/subscription and no stateful workflow, so Principle IV's MVU
ceremony is not warranted (research.md D6). Purity is asserted by unit tests on the pure
`diff`; the edge is exercised by a real-filesystem interpreter test.

## Real-evidence obligations (zero synthetic)

- SHA-pinned baseline report with per-metric reproduction commands (SC-001/002).
- Byte-identical parity scene-output golden re-derivation (SC-003).
- Eight zero-drift per-package baselines (SC-004).
- A real, reverted one-package seeded drift (SC-005).
- ADRs 0007–0011, cross-linked from the programme plan (SC-006).
- The runtime-untouched proof (`git diff --stat -- src/` empty, SC-007).
- The serialized escalated FAKE gate logs; `EvidenceAudit` PASS with zero synthetic (SC-008).
