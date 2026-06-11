# Aggregate-hang diagnostics — feature 105 (housekeeping code-quality)

  verdict: no hang — the behaviour-preserving refactor adds no runtime, window, GPU, wall-clock,
    or external process; every routed suite ran and passed.
  stage: Test aggregate (Dev)
  elapsed duration: Dev passed in 3 minutes 48.7 seconds (Restore 56.2s, Build 1m30.0s,
    SampleContractSmoke 8.4s, Test 1m14.0s), exit code 0.
  last observed command: ./fake.sh build -t Dev
  timeout_policy: FAKE-backed targets share `.fake` state and are run sequentially, never
    concurrently; a race-like or hung aggregate is re-run sequentially in isolation before any
    product-regression claim.
  recommended focused rerun: dotnet run --project tests/Controls.Tests/Controls.Tests.fsproj -c Debug -- --filter-test-list "Feature 105"
  focused rerun:
    command: dotnet run --project tests/Controls.Tests/Controls.Tests.fsproj -c Debug
    focused rerun result: Controls 337 passed / Elmish 69 passed / Scene 28 passed /
      SkiaViewer 62 passed; the Feature 105 parity guard (8) green; no test edited and no
      parity/golden row moved (SC-005/SC-006).
    evidence_path: specs/105-housekeeping-code-quality/readiness/logs
  final_classification: no hang. Feature 105 changes only internal `src/**` `.fs` bodies
    (helper consolidation, redundant-qualifier removal, internal closed-set DUs with string
    boundaries) with zero `.fsi` delta, so the existing suites pass unchanged under the Dev Test
    aggregate (Restore/Build/SampleContractSmoke/Test all Success). The full routed
    controls-public-surface gate set passed sequentially: Dev, PackageSurfaceCheck, FsiTranscripts,
    GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift,
    ContrastCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck,
    TemplateDrift. Any aggregate-suite result obtained outside the routed focused set is recorded
    as a **non-authoritative aggregate** result and the per-suite Expecto outcomes are authoritative.
  diagnostic: no GUI/adapter hang class applies — no persistent host is launched. No `.fsi`
    surface baseline moved (SC-007); `PackageSurfaceCheck` passed after relocating the no-`.fsi`
    internal `AttrKeys` module into a subdirectory (the paired-signature rule checks
    `src/Controls/*.fs` top-directory only). `GeneratedProductCheck` passed in this environment
    (it can fail elsewhere for an env reason; that would be non-authoritative, see
    generated-validation.md).

note_on_template_revert: a Dev sub-target rewrites this file to a stale 020-asteroids smoke
template (known gotcha). The content above is the authoritative feature-105 diagnostic, re-authored
after the Dev / GeneratedProductCheck runs; EvidenceGraph and EvidenceAudit do not run Dev and so
do not revert it.
