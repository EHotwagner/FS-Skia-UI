# Aggregate-hang diagnostics — feature 106 (controls-api-discoverability)

  verdict: no hang — the feature adds no runtime, window, GPU, wall-clock, or external process;
    every routed suite ran and passed.
  stage: Test aggregate (Dev), within GeneratedProductCheck
  elapsed duration: GeneratedProductCheck completed in 4 minutes 40.7 seconds (Test 1m16.6s),
    Status Ok, exit code 0; the standalone ControlsDocCoverageCheck ran in 0.08s.
  last observed command: ./fake.sh build -t GeneratedProductCheck
  timeout_policy: FAKE-backed targets share `.fake` state and are run sequentially, never
    concurrently; a race-like or hung aggregate is re-run sequentially in isolation before any
    product-regression claim.
  recommended focused rerun: dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -c Debug -- --filter-test-list "Feature 106"
  focused rerun:
    command: dotnet run --project tests/Governance.Tests/Governance.Tests.fsproj -c Debug
    focused rerun result: Governance.Tests 579 passed / Controls.Tests 343 passed (incl. the
      13 TypedLoweringTests parity cases) / the Feature 106 doc-coverage gate tests 6 passed.
    evidence_path: specs/106-controls-api-discoverability/readiness/logs
  final_classification: no hang. Feature 106 changes `///` documentation on the Controls public
    `.fsi` surface (zero signature-shape delta), migrates the generated starter to the typed
    Props front door (lowers structurally equal to the legacy builders), bundles a consumer
    catalog reference + README pointer, and adds the pure `ControlsDocCoverageCheck` gate. The
    routed controls-public-surface set passed sequentially: Dev, ControlsDocCoverageCheck
    (findings=0), GeneratedProductCheck (incl. TemplateCheck PASS, package-skew clean),
    RefreshSurfaceBaselines. Any aggregate-suite result obtained outside the routed focused set
    is recorded as a **non-authoritative aggregate** result and the per-suite Expecto outcomes
    are authoritative.
  diagnostic: no GUI/adapter hang class applies — no persistent host is launched (this is not a
    graphical-viewer feature). The new gate is a pure `.fsi`-text analysis; `GeneratedProductCheck`
    passed in this environment (an environment-class failure elsewhere would be non-authoritative,
    see generated-product.md).

note_on_template_revert: a Dev sub-target can rewrite this file to a stale 020-asteroids smoke
template (known gotcha). The content above is the authoritative feature-106 diagnostic, re-authored
after the GeneratedProductCheck run; EvidenceGraph and EvidenceAudit do not run Dev and so do not
revert it.
