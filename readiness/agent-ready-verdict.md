# Agent-ready verdict

The `maintainer-verify` tier aggregates the escalated gate set into an agent-ready verdict.
For the V3 Stage 5 monolith retirement (feature 053) the change escalates because it touches
governance `Routing.fs`, public-`.fsi` routing, the pack flow, dependency docs, and the
generated `validation.contract.yml`.

- **Authoritative command**: `./fake.sh build -t AgentReady` (aggregates the escalated gates).
- **Verdict**: ready. The required gates were run sequentially (never concurrently; FAKE
  shares `.fake` state) and pass:
  - `Dev` — full restore/build/test green (all suites; the SkiaViewer libdecor-gtk teardown
    crash is a documented dual-display headless flake, green under forced X11 — the focused
    rerun is authoritative).
  - `PackageSurfaceCheck` — green; the monolith aggregate baseline retired, nine split
    baselines current.
  - `PerPackageSurfaceDiff` — zero drift across nine packages; the gate bites on an
    unrecorded `.fsi` edit (`readiness/per-package-surface-enforcement.md`).
  - `TargetMetadataDrift` — contract current vs `Routing.fs`.
  - `FsiTranscripts` — green (the gated preludes run; monolith-loading orphan scripts removed).
  - `GeneratedGuidanceCheck`, `TemplateCheck`, `TemplateDrift`, `GeneratedProductCheck` — green;
    the generated `app` is asserted clean.
  - `DependencyReport` — the package graph is acyclic and `FS.Skia.UI.Scene` stays
    FSharp.Core-only.
  - `EvidenceGraph` / `EvidenceAudit` — see `readiness/evidence-graph.md` /
    `readiness/evidence-audit.md`.
- **Non-authoritative aggregate**: aggregate FAKE results are recorded as non-authoritative;
  any race-like/environment-flaky failure is rerun focused and that focused result is
  authoritative.
- **Failure class**: AgentReady. **Next action**: rerun the named failing gate in focused
  isolation before product debugging.
