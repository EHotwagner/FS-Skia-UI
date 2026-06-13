# Misc Tasks

## Open

- [ ] `dotnet fsi` cannot start a script that opens a live window: the native/Skia
  transitive dependencies aren't resolved in the `fsi` context. Managed-only scripts
  (e.g. `scripts/controls-docs-contrast.fsx`) run fine — only window-opening scripts
  are affected. Workarounds: ship a compiled dll, or use a separate `.fsx` loader that
  primes the native search path. (Still current as of 2026-06-13.)
- [ ] `specs/073-add-animations/readiness/fsi/animation-session.fsx` still uses an
  absolute `#r` to a NuGet cache path. Left as-is because it is a frozen readiness
  evidence artifact; rewriting it would churn evidence with no live benefit.

## Resolved (2026-06-13)

- [x] Live scripts no longer use absolute `#r` paths. `scripts/controls-docs-contrast.fsx`
  now uses `#I __SOURCE_DIRECTORY__` with repo-relative `#r` references and derives
  `repoRoot` from `__SOURCE_DIRECTORY__`; verified it regenerates `docs/controls-contrast.md`
  byte-identical when run from an unrelated working directory.
- [x] `.agents`/`.claude` `fs-skia-reconciliation/SKILL.md` front matter no longer fails
  strict YAML parsing. The unquoted `description` contained `feature 103: ` (colon-space),
  which YamlDotNet rejects as an invalid mapping; replaced with an em-dash to match the
  surrounding style. Both peer copies updated identically and re-verified with YamlDotNet.
- [x] Stale `aggregate-hang-diagnostics.md` re-author note dropped — it was a one-off
  log line from a prior feature's gate run, not a standing task.
