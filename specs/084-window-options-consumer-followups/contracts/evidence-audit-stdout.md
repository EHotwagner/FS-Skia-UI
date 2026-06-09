# Contract: `EvidenceAudit` stdout summary

The audit summary printed to stdout (`build/Governance/Evidence/GeneratedRunner.fs`
+ `Front/Governance.fs`). FR-008 / FR-009 make it self-sufficient — every blocker
legible without opening `*-hits.json` sidecars or decompiling the build DLL.

## Before (current)

```
verdict=fail
real-tasks=12
unaccepted-synthetic-tasks=0
late-seh-tasks=0
total-blockers=2
```

Per-blocker reasons live only in `readiness/*-hits.json`; `base_ref` is reported
`null` even when `main` is a strict ancestor of HEAD.

## After (required)

```
verdict=fail
real-tasks=12
unaccepted-synthetic-tasks=0
late-seh-tasks=0
diff-scan base_ref: main (merge-base <sha>)        # FR-009: resolved base surfaced
total-blockers=2

blockers:
  [readiness-contract] interactive-visible-window.md
    reason: missing required tokens
    absent-from-file: accessible-window, self-closed-for-evidence
    hit-file: readiness/readiness-contract-hits.json
  [window-visibility] window-options.md
    reason: missing option= rows
    hit-file: readiness/window-visibility-hits.json
```

## Requirements

- **FR-008.** For each blocker, stdout MUST print: validation area, file name,
  one-line `reason`, and the originating hit-file path. The existing per-area
  renderers (`Render.readinessContractDiagnostics` and siblings, `Render.fs:459`)
  already format reason / full-required-set / absent terms — wire them into the
  summary block. No new computation; the `ScanHit` data already carries it.
- **FR-009.** The summary MUST report the diff-scan base ref:
  - resolvable → `diff-scan base_ref: <ref> (merge-base <sha>)`, threaded from the
    base the caller already resolves (`Front/Governance.fs:746`) into
    `DiffScanResult.BaseRef` (`DiffScan.fs:190`, currently hardcoded `None`).
  - not resolvable (brand-new repo) → an explicit line, e.g.
    `diff-scan base_ref: none — no default-branch ancestor; empty diff-scan is by
    absence, not a clean diff`.
- The JSON sidecars remain unchanged (additive stdout only); `base_ref` in the
  diff-scan JSON (`Render.fs:411`) now reflects the resolved value too.

## Verification

Trigger a deliberate readiness gap in a generated project and confirm the audit
stdout alone enumerates each blocker (area, reason, hit-file path) and a non-
misleading base-ref line — no JSON sidecar opened (SC-004).
