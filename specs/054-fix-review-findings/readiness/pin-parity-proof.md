# Pin-Parity Proof — Feature 054 (US1, SC-001)

**Authoritative command:**

```bash
script=$(grep -oE '#r "nuget: FS\.Skia\.UI\.Build, [^"]+"' template/base/build.fsx | grep -oE '[0-9][^"]+')
props=$(grep -oE 'FS\.Skia\.UI\.Build" Version="[^"]+"' template/base/Directory.Packages.props | grep -oE '[0-9][^"]+')
test "$script" = "$props"
```

## Before (FR-004 drift, failing-first)

- `template/base/build.fsx` `#r` literal: `0.1.45-preview.1`
- `template/base/Directory.Packages.props` `FS.Skia.UI.Build` `PackageVersion`: `0.1.56-preview.1`
- → **FAIL** (drift confirmed by the strengthened parity assertion, which named
  both versions: *"template build.fsx #r pins FS.Skia.UI.Build 0.1.45-preview.1
  but Directory.Packages.props pins 0.1.56-preview.1"*).

## After (FR-001 / FR-004 fix)

- `build.fsx` `#r` aligned to `0.1.56-preview.1`.
- Result: `PASS: 0.1.56-preview.1 == 0.1.56-preview.1`.
- The strengthened `GeneratedProjectValidationTests` test
  *"template build.fsx engine pin equals Directory.Packages.props pin"* now
  passes (Failed: 0, Passed: 1).

**Failure class:** version drift between a derived `#r` literal and the
source-of-truth props pin. **Artifact:** this file. **Next action:** kept current
automatically by the extended `fs-skia-template-update` flow (see
[[simulated-bump-proof]]) and enforced by the parity gate (see
[[deliberate-mismatch-gate]]).
