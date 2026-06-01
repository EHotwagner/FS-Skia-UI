# Contract: Golden Parity Oracle

The merge gate that makes deleting `build.fsx` safe (FR-012, SC-002). Capture a golden baseline of
every target's output from the **current `build.fsx` path** *before* relocation; after the move,
diff the new front-end's output against it. The relocation is **not** shipped until the parity set
is a zero-byte diff (modulo the normalization and exclusions below). Mirrors the capture-then-diff
discipline that gated the Stage-4 Python port.

## Comparison set

Every registered target in `Targets.dispatchTargets`, partitioned into three classes:

### Class A — deterministic governance reports/artifacts (byte-diff after normalization)
The bulk of targets (`CapabilityCheck`, `TargetMetadata`, `TargetMetadataDrift`,
`DependencyReport`, `GeneratedGuidanceCheck`, `GeneratedProductCheck`, `EvidenceGraph`,
`EvidenceAudit`, `Route`, `PackageSurfaceCheck`, regeneration targets, …). Compared **byte-for-byte
after normalization**:
- **Timestamps** → replaced with a fixed token.
- **Absolute paths** → repository-root-relativized.
- **Run ordering** → sorted wherever the script already sorts (no new nondeterminism introduced).

### Class B — test-shelling / log targets (verdict + report, not raw stdout)
Targets that shell `dotnet test` or emit timestamped/ordered logs (`Test`, `Verify`/`Ci`
aggregates, anything wrapping a test run). Compared by **verdict** (pass/fail status + counts) **+
the deterministic report block**, **not** raw stdout. The clarification chose this because their
stdout carries irreducible nondeterminism (timing, ordering).

### Class C — excluded, pre-existing-RED (enumerated + stash-control justified)
Excluded from the byte-diff because they are RED for **feature-independent** reasons on this
toolchain, proven by a **stash control** (they fail identically with this feature's edits stashed —
the same disclosure feature 039 used):
- **`FsiTranscripts`** — `scripts/controls-prelude.fsx` exits 1 on this toolchain (runtime/env-side).
- **`TemplateCheck`** — its `Test` step hits the known `SkiaViewer.Tests` libdecor-gtk headless flake.

Exclusions MUST be **listed and justified** in `readiness/parity/exclusions.md`; a silent exclusion
is a contract violation.

## Evidence layout

```
specs/045-foundations-build-frontend/readiness/parity/
├── exclusions.md                 # the Class-C list + stash-control proof
├── <target>/baseline/<reports>   # captured from build.fsx before relocation
├── <target>/after/<reports>      # captured from the compiled front-end
└── <target>/diff.txt             # normalized diff (expected: empty for Class A/B)
```

## Pass condition (SC-002)

For every Class-A and Class-B target, `diff.txt` is empty after normalization. Any non-empty diff
blocks the migration until resolved (fix the relocation, never weaken the oracle). Class-C targets
are not diffed; their stash-control identity is recorded instead.

## Relationship to other gates

- This oracle proves **behaviour parity** (the real gate).
- Build wall-clock (`readiness/logs/build-timing.md`) is **recorded, not gated** (R6/SC-007).
- The serialized FAKE gate sequence (`Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `GeneratedProductCheck`, `EvidenceGraph`, `EvidenceAudit`) must be green on the new front-end
  (escalated path, since this change touches `build.fsx`/launchers/governance paths).
