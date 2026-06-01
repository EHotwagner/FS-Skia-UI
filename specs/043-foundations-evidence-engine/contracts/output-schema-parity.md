# Contract — Byte-parity output schema (the parity oracle)

The engine's hardest contract is **byte-for-byte equality** with the current
Python engine's output for identical inputs (FR-007, Invariant 6, SC-001/SC-001a).
This file enumerates every output artifact, its producer, and the fixture that
proves parity. Sign-off requires **0 bytes** of difference on all of them
(DiffPlex), across features 036 / 037 / 038, **before** any Python is deleted
(FR-012).

## Original Stage-0 oracle (already committed)

`tests/Governance.Tests/fixtures/evidence-golden/<F>/`:

| Artifact | Producer (Python) | F# producer | Parity assertion |
|---|---|---|---|
| `task-graph.json` | `compute-task-graph.py` | `Render.taskGraphJson` | 0-byte diff per feature |
| `task-graph.md` | `compute-task-graph.py` | `Render.taskGraphMd` | 0-byte diff per feature |
| `audit-counts.txt` | `run-audit.sh`-derived counts | `Render.auditCounts` | 0-byte diff per feature |

`audit-counts.txt` fields: `real-tasks`, `accepted-seh-tasks`,
`unaccepted-synthetic-tasks`, `auto-synthetic-tasks`, `late-seh-tasks`.
Known fixture values: 036 → `real=32, accepted-seh=1`; 037 → `real=30, all 0`;
038 → `real=38, all 0`.

## Extended oracle — NEW fixtures (FR-017, capture before deletion)

`tests/Governance.Tests/fixtures/evidence-golden/<F>/scans/` — captured from the
**current Python engine** for 036/037/038, then committed:

| Artifact | Producer (Python heredoc in `run-audit.sh`) | F# producer |
|---|---|---|
| `readiness-contract-hits.json` | readiness-contract scan | `Scans.readinessContract` |
| `persistent-launch-hits.json` | persistent-launch scan | `Scans.persistentLaunch` |
| `persistent-gui-runtime-hits.json` | persistent-GUI runtime scan | `Scans.persistentGui` |
| `window-visibility-hits.json` | window-visibility scan | `Scans.windowVisibility` |
| `diff-scan-hits.json` | diff-scan heredoc | `DiffScan.scan` |

(The `audit-status-hits.json` and `seh-audit-summary.json` shapes are exercised
by the re-pointed `AuditStatusRegionTests` / `SyntheticErrorEvidenceTests` against
their committed fixtures; capture as a scan fixture too if a 036/037/038 feature
emits a non-empty region.)

## Byte-parity hazards (must be reproduced exactly)

1. **JSON key ordering** — `compute-task-graph.py` emits tasks **sorted by id**;
   per-task field order is fixed. Emit a deterministic ordered map; match
   field-by-field.
2. **JSON formatting** — match Python `json.dumps` indentation and item/key
   separators exactly (spacing after `:` and `,`).
3. **Markdown** — 4-space nested-list indentation; section order (verdict, skill
   assessments, status counts, SEH classification, Mermaid, ASCII, propagation).
4. **Mermaid** — `classDef` CSS (`fill`, `stroke`, `stroke-width`,
   `stroke-dasharray`) byte-identical; node-class assignment per effective status.
5. **ASCII tree** — status-box glyphs and root-cause markers identical.
6. **Diff-scan** — unified-diff parsing must match the Python line-number and
   added-line extraction; `{base_ref, blocking[], advisory[]}` shape and hit
   fields (`file, line, pattern, severity, reason, match`) identical.
7. **Trailing newline** — present/absent exactly as the Python writes it.

## Parity gate procedure

1. Capture the five new scan outputs from Python for 036/037/038; commit under
   `fixtures/evidence-golden/<F>/scans/` (FR-017). *(real evidence — not synthetic)*
2. Implement F# producers; byte-diff F# output vs every fixture (DiffPlex).
3. Keep Python runnable behind `--legacy-evidence` until **all** diffs are 0 bytes
   (FR-012).
4. On sign-off: delete `compute-task-graph.py`, `audit-status-scan.py`,
   `run-audit.sh` (all 9 heredocs), and the `--legacy-evidence` path (FR-011);
   grep proves zero `python3`/`run-audit.sh` in the evidence path (SC-003).
