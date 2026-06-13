# Smoke: forced staleness — a render-affecting change re-records (T028, SC-005 / FR-010)

**Authoritative command:** `dotnet run --project tests/SkiaViewer.Tests` (`PictureReplayCache` tests)
and `dotnet run --project tests/Controls.Tests` (`Feature120FingerprintTests`).
**Failure class:** product-defect.

## What was proven

- **Fingerprint flips on any render-affecting change** (`Feature120FingerprintTests`): geometry,
  color, text, opacity, and transform changes each produce a different `RetainedRender.hashScene`
  value. The key proof: two 200-element charts differing only at index 150 — which the superseded
  truncating `sprintf "%A"` digest stringifies **identically** (asserted) — produce **different**
  fingerprints, so the structural-collision the old key suffered cannot cause a stale hit (SC-005).
- **A changed fingerprint re-records, never a stale hit** (`PictureReplayCache` test): painting a
  boundary with `CacheId=0 Fingerprint=100` then `CacheId=0 Fingerprint=200` forces a second record
  (the direct walk runs again), `Hits = 0`, `Records = 2`, and a single resident entry (the replaced
  picture is disposed, not accumulated). No stale pixels are ever presented across the change.
