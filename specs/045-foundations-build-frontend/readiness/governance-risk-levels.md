# Governance Risk Levels (T003)

- **Authoritative command**: `./fake.sh build -t Route` (prints tier + minimal gate list).
- **Artifact path**: this readiness directory.
- **Failure class**: tier mis-selection ⇒ wrong gate set.

## Risk levels & required evidence

- **small** — routine framework-internal edits within this feature's own `build/Governance/*.fs`
  work: a focused `./fake.sh build -t Dev` plus the `Governance.Tests` suite is authoritative.
- **medium** — the new build-tooling `Engine/*` + `GeneratedProduct`/`Guidance`/`Preflight`
  `.fsi`/`.fs` modules and the grown `build/Program.fs` front-end: focused `Dev` plus the
  per-target parity check and the targeted FAKE gates `Route` prints.
- **broad** — required here: this is a `build.fsx`/launcher/`.config/dotnet-tools.json`/governance-
  path change that `Route` **escalates**. The **required evidence** is the escalated serialized
  six-target **broad validation**: `Dev → GeneratedGuidanceCheck → TemplateCheck →
  GeneratedProductCheck → EvidenceGraph → EvidenceAudit`, run sequentially (never concurrent FAKE).

Aggregate FAKE results are non-authoritative; the known `SkiaViewer.Tests` headless crash and the
`FsiTranscripts` toolchain RED are rerun in focused isolation, and that focused result is
authoritative (SC-002/SC-008). Captured: 2026-06-01T18:10:09Z
