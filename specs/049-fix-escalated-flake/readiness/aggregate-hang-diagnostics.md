# Aggregate Hang Diagnostics (T013 — single authoritative run)

After this feature the escalated aggregate is **authoritative for the graphics-backend
flake class**: the deterministic dual-display guard (unit-proven) removes the
previously-required workaround, so the prior "treat the aggregate as a
**non-authoritative aggregate** and do a manual **focused rerun** of the affected step"
caveat no longer applies to this flake class (FR-006 / FR-009 / SC-003). Captured from a
single sequential run on the headless host (both `WAYLAND_DISPLAY=wayland-0` and
`DISPLAY=:1` advertised) with **no** manual `env -u WAYLAND_DISPLAY` prefix.

validation_verdict:
  target: escalated serialized order — Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck (EvidenceGraph/EvidenceAudit are T015/T016)
  verdict: PASS — every target exited 0; deterministic single run, no graphics-backend host crash and no graphics-init stall
  stage: all four targets completed; no stage hung or aborted
  elapsed duration: Dev 00:01:42 (Test 22s); GeneratedGuidanceCheck Ok; TemplateCheck Ok; GeneratedProductCheck run total ~3m44s (GeneratedProductCheck target 00:01:34)
  last observed command: ./fake.sh build -t GeneratedProductCheck
  graphics_backend_crash: none — no `libdecor-gtk.so` teardown crash in any target log; every nested `dotnet test` reported exit-code=0
  generated_product_stall: none — GeneratedProductCheck completed within its normal envelope (~94s); the previously observed ~20-minute graphics-init hang did not recur (SC-002)
  startup_normalization: "graphics-env normalization: DualDisplay detected — removed WAYLAND_DISPLAY; set GDK_BACKEND=x11, SDL_VIDEODRIVER=x11" logged once at the head of every target log (dev.log, generated-guidance-check.log, template-check.log, generated-product-check.log)
  gui_viewer_tests: SkiaViewer.Tests passed its assertions AND was reported as passing (exit-code=0 in the Dev run's logs/test.txt — no teardown crash turning green to red, FR-004)
  focused rerun:
    required: false — for the graphics-backend flake class the single-run aggregate is authoritative; no manual focused rerun is needed (FR-006/SC-003)
    control_check: env -u WAYLAND_DISPLAY GDK_BACKEND=x11 SDL_VIDEODRIVER=x11 dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj -m:1 --no-build --no-restore -- --sequenced
    control_result: Passed! 48/48, exit 0 (readiness/logs/skiaviewer-control.log) — corroborates that a green run is no longer turned red on teardown (FR-004)
  non-authoritative aggregate: the obsolete "non-authoritative aggregate / rerun by hand" workaround is REMOVED for this flake class; a genuinely race-like failure unrelated to this flake class is still rerun in focused isolation
  final_classification: environmental flake eliminated by deterministic X11 backend selection self-applied by the compiled front-end and propagated to every spawned child; not a product defect
  diagnostic: the deterministic X11 selection (startup ambient normalization + spawn-edge re-application) propagated to dotnet test, FSI, and nested `bash ./fake.sh` descendants; child exit codes are propagated unchanged so genuine regressions still surface (C5/FR-008)
  evidence_logs:
    - readiness/logs/dev.log
    - readiness/logs/test.txt
    - readiness/logs/generated-guidance-check.log
    - readiness/logs/template-check.log
    - readiness/logs/generated-product-check.log
    - readiness/logs/skiaviewer-control.log
  stale_binary_note: an initial Dev run reproduced the libdecor teardown crash because `dotnet run` had not yet recompiled the Governance library with the spawn-edge change; after a clean `dotnet build build/Build.fsproj` the single run passed deterministically. Operators must let `./fake.sh` rebuild before trusting the verdict.
