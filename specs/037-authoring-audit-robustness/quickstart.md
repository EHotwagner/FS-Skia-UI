# Quickstart: Validating Fail-Loud Authoring & Audit Robustness

How to reproduce each fixed failure and confirm the new fail-loud behavior. Run
FAKE-backed targets **sequentially** (shared `.fake` state).

## US1 — Audit resolves the real feature, or hard-fails

1. Confirm `.specify/feature.json` points at the active feature:
   `{"feature_directory": "specs/037-authoring-audit-robustness"}`.
2. Run the graph + audit:
   ```
   ./fake.sh build -t EvidenceGraph
   ./fake.sh build -t EvidenceAudit
   ```
3. **Expect**: the log echoes the resolved feature id and its **real** task
   count (not a 1-task stub), recorded in `readiness/feature-resolution.md`.
4. Negative path: with no resolvable feature (e.g. an unreadable
   `feature.json`), re-run and **expect a non-zero/blocking exit** with a
   prominent warning naming the expected source — never a silent green pass.

## US2 — Prose mentions don't block; real violations still do

1. False-positive fixture — `readiness/audit-fixtures/prose-negation-clean.md`
   mentions `taskbar-only`, `package mismatch`, `nu1603` only inside
   sentences/negations, plus a clean ```` ```audit-status ```` region.
   **Expect**: audit PASS (no phantom blockers).
2. True-positive fixture — `readiness/audit-fixtures/genuine-violation.md`
   declares a violating value (e.g. `exact-package-match=false`) *inside* the
   `audit-status` region. **Expect**: audit BLOCK.
3. Resolution rule check: a key in a prose bullet must NOT override the
   region's value; a duplicate key inside the region is a surfaced parse error.

## US3 — Mixed Scene/Controls compiles in the previously-failing order

1. Build the library: `./fake.sh build -t Dev`.
2. Compile the fixture under `readiness/fsi/` that opens `FS.Skia.UI.Scene`
   then `FS.Skia.UI.Controls` (Controls last) and constructs a scene text node
   unqualified. **Expect**: it compiles to the scene construct (or fails naming
   the colliding symbols) — never the opaque "value is not a function / has type
   ControlEventOrigin" error.
3. Surface baseline: `./fake.sh build -t PackageSurfaceCheck` passes with the
   refreshed `FS.Skia.UI.Controls.txt` / `FS.Skia.UI.txt` reflecting
   `[<RequireQualifiedAccess>] ControlEventOrigin`.

## US4 — Load a generated app into FSI in one step

1. Generate products and run the generated-product checks:
   ```
   ./fake.sh build -t GeneratedGuidanceCheck
   ./fake.sh build -t TemplateCheck
   ./fake.sh build -t GeneratedProductCheck
   ```
2. In a freshly generated app, run the emitted `.fsx` load script in FSI.
   **Expect**: the app and its transitive references load with **zero** manual
   reference edits; benign host warnings stay classified benign, real failures
   stay fatal. Transcript recorded in `readiness/fsi-load-script.md`.

## Full sequential validation

```
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

If any failure looks race-like, rerun the affected FAKE-backed commands
sequentially before product debugging.
