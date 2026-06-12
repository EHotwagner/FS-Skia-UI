# Aggregate-hang diagnostics — feature 107 (governance-skew-doc-hardening)

  verdict: no hang — the feature adds no runtime, window, GPU, wall-clock, or external process; both
    fixes are pure text analyses in build tooling, and every focused suite ran to completion.
  stage: Test (Dev) — dotnet test tests/Governance.Tests + tests/Package.Tests, plus PackageSurfaceCheck.
  elapsed duration: PackageSurfaceCheck completed in ~1m53s (Status Ok); Governance.Tests ran in ~12s
    (556/557; the 1 failure is the pre-existing version-pin test); Package.Tests ran in 85ms (35/35).
  last observed command: ./fake.sh build -t Dev (and ./fake.sh build -t PackageSurfaceCheck)
  timeout_policy: FAKE-backed targets share `.fake` state and are run sequentially, never concurrently;
    a race-like or hung aggregate is re-run sequentially in isolation before any product-regression claim.
  focused rerun:
    command: dotnet test tests/Governance.Tests; dotnet test tests/Package.Tests
    focused rerun result: Governance.Tests 556/557 (sole failure = pre-existing template version-pin
      test, fails at HEAD, unrelated); Package.Tests 35/35 incl. FR-004/FR-005; PackageSurfaceCheck Ok.
    evidence_path: specs/107-governance-skew-doc-hardening/readiness/logs
  final_classification: no hang. Feature 107 changes only build-tooling text analysis
    (PackageSkew comment-strip + PerPackageSurface recursive capture + the doc-preservation assertion)
    and regenerates two additive per-package baselines. Any aggregate-suite result obtained outside the
    focused per-suite/per-target runs is recorded as a **non-authoritative aggregate** result and the
    per-suite Expecto outcomes are authoritative.
  diagnostic: no GUI/adapter hang class applies — no persistent host is launched (this is not a
    graphical-viewer feature). The changed checks are pure functions over source/reference text.
