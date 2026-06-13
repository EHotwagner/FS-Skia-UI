# Aggregate Hang Diagnostics

validation_verdict:
  target: Dev
  verdict: aggregate product tests PASS; the single failing assertion is the PRE-EXISTING, out-of-scope template package-pin lag (non-authoritative aggregate result), not a feature-116 regression
  stage: Test aggregate
  elapsed duration: Dev ran build + the full multi-project test aggregate to completion
  last observed command: dotnet test tests/Governance.Tests/Governance.Tests.fsproj
  timeout_policy: Smoke.Tests / SkiaViewer.Tests run the Expecto executable directly (bypassing the VSTest/YoloDev adapter) to dodge the libdecor-gtk crash under the dual Wayland/X11 display
  recommended focused rerun: dotnet test tests/Governance.Tests/Governance.Tests.fsproj
  focused rerun:
    command: dotnet run --project tests/Controls.Tests/Controls.Tests.fsproj -c Debug --no-build -- --filter-test-list "Feature 116"
    focused rerun result: 23 Feature 116 Controls tests + 5 Feature 116 Elmish metrics tests passed; full suites 430 Controls + 148 Elmish green
    evidence_path: specs/116-paint-cache-damage-rects/readiness/logs/test.txt
  investigated_failure:
    command: Governance.Tests "template package pins match current repository package posture"
    result: FS.Skia.UI.Scene template pin (via $(FsSkiaUiVersion)) = 0.1.121-preview.1, repository package version = 0.1.122-preview.1 — a one-version template pin lag
  control_check:
    command: git diff HEAD -- template/base/Directory.Packages.props
    result: empty — the 0.1.121 pin is UNCHANGED from the base branch; feature 116 did not touch any version pin (its only template edit is the regenerated api-surface Types.fsi doc)
  final_classification: pre-existing, out-of-scope template-pin lag (the libs were bumped to 0.1.122-preview.1 by base commit 127a6a9d; the template pin was not advanced). Resolved by the `speckit-merge` version bump + template re-pin, exactly as features 107/108/115 documented. NOT a feature-116 product regression.
  diagnostic: Every product test project passes (Lib 30, Scene 28, Color 15, Elmish 148, KeyboardInput 16, Layout 28, Controls 430, Testing 38, Parity 21, Smoke 3, SkillSupport 33; Governance 556/557 with the single template-pin-posture assertion the only failure). The authoritative feature-116 signal is the full controls-public-surface gate set + EvidenceAudit verdict=PASS with 0 synthetic.
