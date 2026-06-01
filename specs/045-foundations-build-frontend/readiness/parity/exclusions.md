# Parity Oracle — Class-C Exclusions & Approach (T004 / T017 / FR-012 / SC-002)

## Parity approach for this relocation

The relocation is a **verbatim byte-range extraction** of every report-producing function from
`build.fsx` and `scripts/build/*.fsx` into compiled modules (see the per-module header comments
citing the original `build.fsx` line ranges). The report **text** is therefore identical *by
construction* — the string literals and rendering code were moved unchanged, only repackaged into
modules with added `namespace`/`open` headers. The only intentional runtime-behaviour changes are:

1. `repositoryRoot`: was `__SOURCE_DIRECTORY__` (a compile-time constant valid only for the FSX
   script at the repo root); now discovered at runtime by walking up from the working directory for
   the `.specify/feature.json` marker. The launchers `cd` to the repo root, so this resolves to the
   identical path. Verified: `./fake.sh build -t Route` prints the same tier/gates the FSX path did.
2. Argument forwarding: the launchers still pass `build -t <name> [flags]`; `Program.fs` strips the
   leading `build` token (the `dotnet fake` CLI consumed it) so `runOrDefaultWithArguments` parses
   identically. Verified: `Route` runs alone (not the Dev default chain).

Focused spot-checks confirm identical report output: `Route` (tier/gate selection), the relocated
validators exercised by the new unit tests (`Guidance.runGeneratedGuidanceScan`,
`Preflight.collectProcessHealth`, `GeneratedProduct.runDependencyOwnershipReport`) produce their
expected report headings/markers, and the full 304-test `Governance.Tests` suite — which asserts the
front-end's command contract, target graph, report-output strings, and artifact failures — is green
against the relocated sources.

**Honest scope note (per the user-accepted "parity may be incomplete" decision):** a full
target-by-target `baseline/` capture from the live `dotnet fake` path *before* relocation was NOT
performed, because the launcher/tool rewire had already removed the `fake-cli` path. Parity here
rests on (a) verbatim extraction = identical text by construction, (b) the green governance
contract suite, and (c) the focused spot-checks above — not on a stored pre/post byte-diff per target.

## Class-C — excluded, pre-existing-RED (enumerated)

These targets are RED for feature-independent toolchain reasons and are excluded from any byte-diff:

- **`FsiTranscripts`** — `scripts/controls-prelude.fsx` exits 1 on this toolchain (runtime/env-side),
  independent of this relocation.
- **`TemplateCheck`** — its `Test` step hits the known `SkiaViewer.Tests` libdecor-gtk headless
  flake (see project memory). Authoritative result is a focused rerun, per SC-002/SC-008.

Captured: 2026-06-01T14:44:26Z
