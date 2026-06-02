# Surface-baseline diff — the only public-surface change in the repo (FR-010 / SC-006)

The monolith aggregate baseline `readiness/surface-baselines/FS.Skia.UI.txt` sheds its 47 contiguous
`FS.Skia.UI.AgentValidation.*` lines (130 → 83 lines). This is the **single** public-surface delta in
the whole change.

## What changed

- Removed: the 47 `FS.Skia.UI.AgentValidation.*` reflection entries (`AgentVerdict`,
  `ValidationContract*`, `ValidationSelection*`, `ValidationGate*`, the failure taxonomy DUs, and their
  `+Tags`/case rows) — the module no longer compiles into the `FS.Skia.UI` package.
- **No** `readiness/surface-baselines/FS.Skia.UI.Build.txt` is created — the build-tooling library is
  excluded from surface tooling (`readiness/per-package-surface-expectations.md`, D4).
- The eight runtime per-package baselines are **byte-unchanged**.

## Authoritative evidence

`./fake.sh build -t PackageSurfaceCheck` runs **clean** (`Status: Ok`): the trimmed baseline matches
the built reflection surface exactly. Log: `readiness/logs/package-surface-check.log`. The `Route`
escalation also runs `PackageSurfaceCheck` as a required gate; both confirm no runtime per-package
baseline drift (the default `app` is byte-unchanged — SC-006).
