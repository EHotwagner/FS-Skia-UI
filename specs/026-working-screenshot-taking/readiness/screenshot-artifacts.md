# Screenshot Artifacts

Status: pending implementation.

This file will list screenshot evidence records and PNG artifacts that reviewers
can inspect without a local rerun. Metadata-only, deterministic-scene-only,
manual, synthetic, blank, unreadable, or out-of-readiness artifacts are not
accepted screenshot proof.

## Accepted Artifacts

| Artifact | Record | Width | Height | Pixel validation | Capture source | Proof |
|----------|--------|-------|--------|------------------|----------------|-------|
| `specs/026-working-screenshot-taking/readiness/artifacts/working-screenshot-record.png` | `specs/026-working-screenshot-taking/readiness/artifacts/working-screenshot-record.txt` | 320 | 200 | `PixelContentNonBlank` | `LiveViewerWindow` | `proves-screenshot=True` |
