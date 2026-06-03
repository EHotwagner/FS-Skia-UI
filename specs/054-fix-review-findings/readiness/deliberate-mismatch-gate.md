# Deliberate-Mismatch Gate Proof — Feature 054 (US1, SC-002)

The strengthened parity assertion (`GeneratedProjectValidationTests` →
`TemplateCheck` / `GeneratedProductCheck`) must FAIL when the `#r` engine pin
diverges from the props `PackageVersion`, naming both versions.

**Authoritative command** (focused; the same assertion `TemplateCheck` runs):

```bash
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-build \
  --filter "FullyQualifiedName~engine pin equals"
```

## Live demonstration

1. **Break** `build.fsx` `#r` → `0.0.0-bad`:

   ```
   #r "nuget: FS.Skia.UI.Build, 0.0.0-bad"
   ```

   → gate **FAILS**, naming both versions:

   ```
   template build.fsx #r pins FS.Skia.UI.Build 0.0.0-bad but
   Directory.Packages.props pins 0.1.56-preview.1 — they must match exactly.
   Failed!  - Failed: 1, Passed: 0
   ```

2. **Restore** `#r` → `0.1.56-preview.1`:

   ```
   #r "nuget: FS.Skia.UI.Build, 0.1.56-preview.1"
   ```

   → gate **PASSES**: `Passed!  - Failed: 0, Passed: 1`.

**Failure class:** the gate would previously miss this (the old check was the
prefix-only `Expect.stringContains "#r \"nuget: FS.Skia.UI.Build"`, version
ignored). The exact-equality assertion (FR-003) catches it. **Next action:**
none — the gate is now binding.
