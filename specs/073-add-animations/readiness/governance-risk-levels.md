# Governance risk levels — Add Animations (073)

The implementation tasks split into three governance risk bands. **broad validation**
is required only because a new public `src/**/*.fsi` (`src/Scene/Animation.fsi`,
`src/Elmish/AnimationTick.fsi`) escalates the route to the `package-surface` rule.
FAKE-backed targets share `.fake` state — run them **sequentially**, never
concurrently. `Route` is authoritative.

## Small

The framework-internal edits: the pure interpolation/lowering bodies in
`src/Scene/Animation.fs` (easing, tween, transform, opacity folding, identity-at-rest)
and the test edits in `tests/Scene.Tests/AnimationTests.fs`. A focused
`./fake.sh build -t Dev` is sufficient evidence for this band.

## Medium

The contracted additions: the new public `.fsi` surface (`Animation` module +
`AnimationState`/`Tween`/`Transform`/`Easing` types in Scene; the `AnimationTick`
message + `Animation.tickSubscription` in Elmish), the regenerated surface baselines,
and the captured parity goldens. The **required evidence** for this band is
`PackageSurfaceCheck` (additive-only delta), `FsiTranscripts` (the
`readiness/fsi/animation-session.txt` exercise), and `PerPackageSurfaceDiff`
(additive-only per-package `.fsi.txt` snapshots).

## Broad

Close-out. **broad validation** re-runs `./fake.sh build -t Route --enforce` on the
full implementation diff and runs exactly the gates it prints — the serialized
FAKE-backed order (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
`GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`), finishing on
`EvidenceGraph` + `EvidenceAudit`. Aggregate results (e.g. `GeneratedProductCheck`'s
known local environment failure) are non-authoritative and recorded under
`readiness/logs/`.
