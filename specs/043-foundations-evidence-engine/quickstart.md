# Quickstart — Foundations Evidence Engine Port (Stage 4)

How to build, validate, and prove parity for the in-process F# evidence engine.
This is a **dogfood** + consumer-contract feature → the `Route` selector escalates
it to the full serialized gate set. Run FAKE-backed targets **sequentially**
(shared `.fake` state, Invariant 5).

## 1. Build the library

```bash
dotnet build build/Governance/FS.Skia.UI.Build.fsproj   # clean under TreatWarningsAsErrors (0/0)
```

## 2. Run the typed tests (non-FAKE; may run while editing)

```bash
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
```
Covers: cycle detection (≥1 cyclic-graph case), topo sort, synthetic propagation
(FsCheck: monotone; no synthetic roots ⇒ no auto-synthetic), status-region
scanning (first-region-wins, dup-key error), and the re-pointed
`AuditStatusRegion`/`PersistentViewerEvidence`/`SyntheticErrorEvidence` suites
asserting **typed** results (SC-002, FR-014).

## 3. Capture the extended golden fixtures (FR-017 — do BEFORE deleting Python)

For each of 036 / 037 / 038, run the **current Python engine** and commit its five
scan outputs under `tests/Governance.Tests/fixtures/evidence-golden/<F>/scans/`:
`readiness-contract-hits.json`, `persistent-launch-hits.json`,
`persistent-gui-runtime-hits.json`, `window-visibility-hits.json`,
`diff-scan-hits.json`. These are **real** captured evidence (not synthetic).

## 4. Prove byte-parity (the merge gate)

```bash
./fake.sh build -t EvidenceGraph    # F# in-process; writes task-graph.json/.md
./fake.sh build -t EvidenceAudit    # F# in-process; full audit + verdict
```
Diff the F#-regenerated outputs against every golden fixture — **0 bytes** on
`task-graph.json`, `task-graph.md`, audit count block (SC-001) **and** the five
scan outputs (SC-001a). While iterating, the Python path stays available behind
`--legacy-evidence` (FR-012).

## 5. Run the serialized six-target dogfood gate set (sequential)

```bash
./fake.sh build -t Route                     # confirms escalation (dogfood + template/** + .specify/**)
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```
Expect this feature's own audit `verdict=PASS` (0 unaccepted-synthetic, 0
auto-synthetic, 0 late-seh, 0 diff-scan, 0 readiness-contract blocking), zero
synthetic evidence (SC-008). Isolate any pre-existing `SkiaViewer.Tests` /
`FsiTranscripts` flake with a stash control.

## 6. Pack + generated-consumer verification (FR-013 / SC-006)

```bash
./fake.sh build -t PackLocal                 # now includes the published FS.Skia.UI.Build
./fake.sh build -t GeneratedProductCheck     # generated project consumes the packaged engine, no Python
```
Confirm the generated project's `EvidenceGraph`/`EvidenceAudit` produce a valid
verdict via the package reference (no copied `run-audit.sh`/`*.py`).

## 7. Decommission + grep proofs (FR-011 / SC-003 / SC-004 / SC-005)

After parity sign-off, delete `compute-task-graph.py`, `audit-status-scan.py`,
`run-audit.sh`, and the `--legacy-evidence` path, then prove:
```bash
grep -rn 'python3\|run-audit.sh\|compute-task-graph.py\|audit-status-scan.py' --include='*.fs*' --include='*.sh' .  # → 0 in evidence path
grep -rn 'FSharp.Compiler' .                                                                                         # → 0 added
```
Record the language reduction {F#,Bash,Python} → {F#} vs the Stage-0 baseline
(SC-005) in `readiness/logs/language-reduction.md`.
