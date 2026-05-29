# Default Text Glyph Capture

- Command: `dotnet test tests/Testing.Tests/Testing.Tests.fsproj`
- User-facing path: `FS.Skia.UI.Testing.DefaultTextGlyphEvidence.validate` validates a readiness-local PNG screenshot artifact produced by the public screenshot capture path.
- Screenshot artifact path: generated during the test under the OS temp directory; readiness contract requires production runs to write under `specs/032-sokoban-feedback-followups/readiness/artifacts/default-text-glyph.png`.
- Dimensions checked: positive decoded PNG dimensions, optional expected width/height.
- Font resolution: `SKTypeface.Default` in the executable test fixture.
- Fallback used: `false` for the passing glyph test; production evidence must record `fallback-used`.
- Glyph coverage metric: asserted greater than `0.015`.
- Solid-block metric: asserted below `0.82`; solid block fixtures are rejected as `solid-block-default-text`.
- Placeholder/tofu metric: asserted below `0.55`; tofu-like box fixtures are rejected as `placeholder-default-text`.
- Status: ok for glyph-shaped PNG evidence.
- Runtime limitations: unsupported hosts must emit `unsupported-host-reason` and cannot claim default text readability.
- Diagnostics: `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` passed with 38 tests on 2026-05-29.

