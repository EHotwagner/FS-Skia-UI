# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: aggregate pass; the only aggregate-run hang observed was the VSTest/YoloDev adapter under a dual Wayland/X11 display, a non-authoritative aggregate result, not a feature-121 failure
  stage: Test aggregate
  elapsed duration: Dev completed after the native-GUI Expecto suites ran via direct Expecto execution (libdecor-gtk adapter crash bypassed)
  last observed command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore -- --sequenced
  timeout_policy: SkiaViewer.Tests and Smoke.Tests bypass the VSTest/YoloDev adapter path and run the Expecto executable directly
  recommended focused rerun: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore -- --sequenced
  focused rerun:
    command: dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore -- --sequenced; dotnet test tests/Controls.Tests/Controls.Tests.fsproj --filter "FullyQualifiedName~Feature 121"
    focused rerun result: the 4 Feature 121 SkiaViewer pacing tests + the 2 Feature 121 Controls idle-tick tests passed; full SkiaViewer (84) and Controls (451) suites green under Dev; the routed controls-public-surface gate set PASS apart from the documented template-pin-lag
    evidence_path: specs/121-live-host-idle-parking/readiness/logs/test.txt
  investigated_failure:
    command: VSTest/YoloDev adapter execution of a native-GUI Expecto suite under a dual Wayland/X11 display
    result: libdecor-gtk plugin failed to init in the adapter testhost (environment, not feature)
  final_classification: VSTest/YoloDev adapter orchestration concern under the headless dual-display environment, not a feature-121, product, or sample failure
  diagnostic: The FAKE Test target runs the native-GUI Expecto suites (SkiaViewer.Tests and Smoke.Tests) via direct Expecto execution to bypass the VSTest/YoloDev adapter testhost (libdecor-gtk crash under a dual Wayland/X11 display); all other test projects continue to use dotnet test. The same headless environment is why the persistent interactive window is not drivable (see runtime-limitations.md).
</content>
