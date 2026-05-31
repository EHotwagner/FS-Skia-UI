# Quickstart: Reproduce & Verify the Foundations Baseline + Spike

Exact commands a reviewer runs from the repo root to reproduce every artifact this feature commits and to confirm nothing regressed. Run FAKE-backed steps **sequentially** (never concurrently — they share `.fake` state).

## 0. Pin the commit

```bash
git rev-parse HEAD   # must equal the git_commit recorded in docs/reports/_baselines/2026-05-31-foundations.md
```

## 1. Reproduce the baseline counts (FR-001 / SC-001)

```bash
# build.fsx size
wc -l build.fsx

# governance Markdown volume
find .claude/skills .agents/skills -name '*.md' | xargs wc -l | tail -1
wc -l .specify/memory/constitution.md
git ls-files 'specs/**/*.md' | xargs wc -l | tail -1

# language LOC mix
git ls-files '*.fs' '*.fsi' '*.fsx' | xargs wc -l | tail -1      # F#
git ls-files '*.sh'                  | xargs wc -l | tail -1      # Bash
git ls-files '*.py'                  | xargs wc -l | tail -1      # Python
```

Each number must match the value (and command) recorded in the baseline doc.

## 2. Build the two new projects with zero warnings (FR-005 / SC-003)

```bash
dotnet build build/Governance/FS.Skia.UI.Build.fsproj -warnaserror
dotnet build build/Build.fsproj -warnaserror
```

Both must succeed with **zero warnings** under `net10.0` / `TreatWarningsAsErrors`.

## 3. Confirm no FSharp Compiler Services dependency (FR-012)

```bash
dotnet list build/Build.fsproj package --include-transitive | grep -i 'FSharp.Compiler' && echo "FCS PRESENT — fallback path" || echo "OK: no FCS"
```

Expected: `OK: no FCS`.

## 4. Run the spike target (FR-006 / SC-004)

```bash
dotnet run --project build/Build.fsproj -- SpikeHello
```

Expected: the target runs to success and prints the message returned by `FS.Skia.UI.Build.Spike.run` (proving the body executed from the library). The outcome is recorded in `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md` as exactly `"D2 confirmed"` or `"fallback triggered"`.

## 5. Reproduce the golden fixtures byte-for-byte (FR-003 / SC-002)

For each feature `F` ∈ {`038-authoring-guidance-consistency`, `037-authoring-audit-robustness`, `017-synthetic-error-evidence`}, run the existing evidence path and diff against the committed fixture:

```bash
# (example for one feature; repeat for all three — run sequentially)
./fake.sh build -t EvidenceGraph     # regenerates task-graph.json / task-graph.md for the active feature
./fake.sh build -t EvidenceAudit     # regenerates the audit status/count block

# then diff regenerated outputs against the committed fixtures:
diff readiness/task-graph.json   tests/Governance.Tests/fixtures/evidence-golden/F/task-graph.json
diff readiness/task-graph.md     tests/Governance.Tests/fixtures/evidence-golden/F/task-graph.md
# and compare the captured audit counts against audit-counts.txt
```

Expected: empty diffs for all three files across all three features (100% byte-for-byte). A non-empty diff means the fixture is not yet a valid oracle — re-capture deterministically or substitute the feature and record it.

## 6. Confirm no regression (FR-009 / FR-010 / SC-006)

```bash
# runtime untouched
git diff --name-only HEAD~1 | grep -E '^src/(Scene|SkiaViewer|Elmish|KeyboardInput|Layout|Controls|Controls.Elmish|Lib)/' && echo "RUNTIME TOUCHED — fail" || echo "OK: runtime untouched"

# canonical serialized validation sequence (run in order, never concurrent)
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Plus the surface gates (`PackageSurfaceCheck` / `FsiTranscripts`) — all green, **no baseline diff**.

## 7. Confirm the ADRs and meta-process exist (SC-005 / SC-007)

```bash
ls docs/adr/000*.md   # five ADRs: D1, D2, contract-versioning, D4, D6
```

The programme meta-process (default tier + named dogfood features Stage 1 and Stage 4) is recorded in `plan.md` §Programme Meta-Process and linked from the baseline doc.

---

**Done when**: every step above passes — baseline reproducible and SHA-pinned, both projects build clean with no FCS, spike outcome recorded unambiguously, all golden fixtures reproduce byte-for-byte, the serialized sequence is green with no surface diff, and all five ADRs + the meta-process record are present.
