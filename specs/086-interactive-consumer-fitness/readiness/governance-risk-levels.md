# Governance risk levels — feature 086

Risk classification for the change surface, mapped to required evidence and the
broad validation each level demands.

| Level | Scope in 086 | Required evidence | Broad validation |
|-------|--------------|-------------------|------------------|
| small | framework-internal `src/Scene/**.fs`, `src/Controls/**.fs`, `src/SkiaViewer/**.fs` behavior (multi-axis layout, bounds, scene primitives, key warm-up) | package semantic/property tests green; render-target PNG decode | `./fake.sh build -t Dev` |
| medium | additive public `.fsi` deltas (Scene `Translate`/`SizedText`, Controls `Bounds`/`hitTest`/`Stack.orientation`) | per-package + cross-package surface baselines recaptured (additive only) | `RefreshSurfaceBaselines` + PackageSurfaceCheck |
| broad | `template/**` neutral scaffold + controls-family pointer-host default + generalized governance assertions | neutral-scaffold grep, TemplateCheck, GeneratedProductCheck, live-window + pointer-dispatch + keystroke evidence | escalated six-target serialized order + EvidenceGraph + EvidenceAudit |

Authoritative tier: this feature escalates to **broad validation** because it touches
`template/**`, public `src/**/*.fsi`, and governance paths. Run only the gates `Route`
prints; the escalated serialized order is the broad-validation path.
