#!/usr/bin/env bash
#
# run-audit.sh — synthetic-evidence audit runner.
#
# Combines two signals:
#   1. Task graph — via compute-task-graph.py. Any [S] (synthetic) or [S*]
#      (auto-synthetic) task counts against merge-readiness.
#   2. Diff scan — greps the workspace diff against the patterns
#      in audit-patterns.yml. Block-severity hits count against
#      merge-readiness. Advisory-severity hits print but do not block.
#
# Exit codes:
#   0 — PASS: no [S], no [S*], no blocking diff-scan hits.
#   2 — NEEDS-EVIDENCE: at least one blocking signal.
#   3 — graph compute failed (cycles, dangling refs, parse errors).
#   4 — usage / filesystem error.
#
# The --accept-synthetic flag is recorded to readiness/synthetic-evidence.json
# but NEVER changes the exit code. The human decides to merge despite the
# failure; the audit still reports failure.
#
# Usage:
#   run-audit.sh <feature-dir>
#   run-audit.sh <feature-dir> --graph-only
#   run-audit.sh <feature-dir> --accept-synthetic "justification text"
#   run-audit.sh <feature-dir> --base <ref>     # override feature-base (default: main|master)
#   run-audit.sh <feature-dir> --patterns <path> # override audit-patterns.yml

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
EXT_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
GRAPH_SCRIPT="$EXT_ROOT/scripts/python/compute-task-graph.py"
DEFAULT_PATTERNS="$EXT_ROOT/audit-patterns.yml"

FEATURE_DIR=""
GRAPH_ONLY=0
ACCEPT_SYNTHETIC=""
BASE_REF=""
PATTERNS_FILE="$DEFAULT_PATTERNS"

die() { echo "run-audit: $*" >&2; exit 4; }

# --- arg parsing ------------------------------------------------------------
while [[ $# -gt 0 ]]; do
  case "$1" in
    --graph-only) GRAPH_ONLY=1; shift ;;
    --accept-synthetic)
      [[ $# -lt 2 ]] && die "--accept-synthetic requires justification text"
      ACCEPT_SYNTHETIC="$2"; shift 2 ;;
    --base)
      [[ $# -lt 2 ]] && die "--base requires a ref"
      BASE_REF="$2"; shift 2 ;;
    --patterns)
      [[ $# -lt 2 ]] && die "--patterns requires a path"
      PATTERNS_FILE="$2"; shift 2 ;;
    -h|--help)
      sed -n 's/^# \{0,1\}//p' "${BASH_SOURCE[0]}" | head -30; exit 0 ;;
    -*)
      die "unknown flag: $1" ;;
    *)
      [[ -n "$FEATURE_DIR" ]] && die "feature-dir already set to '$FEATURE_DIR'"
      FEATURE_DIR="$1"; shift ;;
  esac
done

[[ -z "$FEATURE_DIR" ]] && die "usage: run-audit.sh <feature-dir> [flags]"
[[ ! -d "$FEATURE_DIR" ]] && die "$FEATURE_DIR is not a directory"

FEATURE_DIR="$(cd "$FEATURE_DIR" && pwd)"
READINESS_DIR="$FEATURE_DIR/readiness"
mkdir -p "$READINESS_DIR"

# --- 1. graph compute -------------------------------------------------------
if [[ $GRAPH_ONLY -eq 1 ]]; then
  echo "=== speckit.evidence.graph (graph validation only) ==="
else
  echo "=== speckit.evidence.audit ==="
fi
echo "feature: $FEATURE_DIR"
echo

if [[ $GRAPH_ONLY -eq 1 ]]; then
  echo "[graph validation] Computing task graph only; merge-gate diff scan remains in EvidenceAudit."
else
  echo "[1/3] Computing task graph..."
fi
if ! python3 "$GRAPH_SCRIPT" "$FEATURE_DIR"; then
  echo
  echo "✗ Graph compute failed. See $READINESS_DIR/task-graph.md for details." >&2
  exit 3
fi
echo

if [[ $GRAPH_ONLY -eq 1 ]]; then
  echo "graph validation PASS; graph-only mode skipped diff scan and synthetic merge-gate checks."
  echo "Run EvidenceAudit for full merge-gate validation."
  exit 0
fi

# --- 2. read graph effective statuses ---------------------------------------
GRAPH_JSON="$READINESS_DIR/task-graph.json"
[[ ! -f "$GRAPH_JSON" ]] && die "task-graph.json not produced"

# Extract counts of synthetic and auto-synthetic tasks. Design-approved [SEH]
# tasks stay synthetic in the graph but are not readiness blockers when their
# governed metadata is complete and design-phase provenance is present.
SYNTHETIC_COUNT=$(python3 -c "
import json,sys
d = json.load(open('$GRAPH_JSON'))
print(sum(1 for t in d['tasks'] if t['effective'] == 'synthetic'))
")
AUTO_SYNTHETIC_COUNT=$(python3 -c "
import json,sys
d = json.load(open('$GRAPH_JSON'))
print(sum(1 for t in d['tasks'] if t['effective'] == 'auto-synthetic'))
")
SEH_SUMMARY_JSON="$READINESS_DIR/seh-audit-summary.json"
python3 - "$GRAPH_JSON" "$SEH_SUMMARY_JSON" <<'PYEOF'
import json, sys
from pathlib import Path

graph = json.load(open(sys.argv[1]))
out = Path(sys.argv[2])

non_eligible_terms = [
    "convenience mock",
    "incomplete integration",
    "unavailable product capability",
    "missing host support",
    "placeholder output",
    "speed-only",
    "ordinary in-memory",
    "unsupported-host substitute",
]
late_terms = [
    "implementation",
    "readiness cleanup",
    "after audit",
    "after-failure",
    "late",
]

accepted = []
unaccepted = []
late = []
diagnostics = []

for task in graph["tasks"]:
    seh = task.get("seh", {})
    effective = task.get("effective")
    task_id = task.get("id")
    is_synthetic = effective in {"synthetic", "auto-synthetic"}
    is_accepted = bool(seh.get("accepted"))
    design_source = (seh.get("design_source") or "").lower()
    input_class = (seh.get("synthetic_input_class") or "").lower()
    rationale = (seh.get("rationale") or "").lower()
    acceptance_status = (seh.get("acceptance_status") or "").lower()

    if is_accepted:
        accepted.append(task)
    elif is_synthetic:
        unaccepted.append(task)

    if seh.get("annotation") or seh.get("approval_label"):
        failed_rules = list(seh.get("diagnostics") or [])
        if any(term in design_source or term in acceptance_status for term in late_terms):
            failed_rules.append("late [SEH] classification")
            late.append(task)
        if any(term in input_class or term in rationale for term in non_eligible_terms):
            failed_rules.append("non-eligible synthetic evidence class")
        if failed_rules:
            diagnostics.append({
                "task": task_id,
                "failed_rule": "; ".join(sorted(set(failed_rules))),
                "observed": {
                    "status": effective,
                    "seh": seh.get("annotation"),
                    "label": seh.get("approval_label"),
                    "acceptance_status": seh.get("acceptance_status"),
                },
                "source": seh.get("design_source") or "(missing)",
                "required_action": "Return to design/task generation and record valid [SEH] classification before implementation.",
            })

payload = {
    "accepted_seh_tasks": [t["id"] for t in accepted],
    "unaccepted_synthetic_tasks": [t["id"] for t in unaccepted],
    "auto_synthetic_tasks": [t["id"] for t in graph["tasks"] if t.get("effective") == "auto-synthetic"],
    "late_seh_tasks": [t["id"] for t in late],
    "diagnostics": diagnostics,
}
out.write_text(json.dumps(payload, indent=2) + "\n", encoding="utf-8")
PYEOF
ACCEPTED_SEH_COUNT=$(python3 -c "import json; d=json.load(open('$SEH_SUMMARY_JSON')); print(len(d['accepted_seh_tasks']))")
UNACCEPTED_SYNTHETIC_COUNT=$(python3 -c "import json; d=json.load(open('$SEH_SUMMARY_JSON')); print(len(d['unaccepted_synthetic_tasks']))")
LATE_SEH_COUNT=$(python3 -c "import json; d=json.load(open('$SEH_SUMMARY_JSON')); print(len(d['late_seh_tasks']))")
INVALID_SEH_COUNT=$(python3 -c "import json; d=json.load(open('$SEH_SUMMARY_JSON')); print(len(d['diagnostics']))")

# --- 3. readiness contract scan --------------------------------------------
echo "[2/3] Readiness contract scan..."

READINESS_CONTRACT_HITS_JSON="$READINESS_DIR/readiness-contract-hits.json"
python3 - "$READINESS_DIR" "$READINESS_CONTRACT_HITS_JSON" <<'PYEOF'
import json, re, sys
from pathlib import Path

readiness = Path(sys.argv[1])
out = Path(sys.argv[2])
checks = [
    ("governance-risk-levels.md", ["small", "medium", "broad", "required evidence", "broad validation"], "governance risk level evidence is incomplete"),
    ("aggregate-hang-diagnostics.md", ["verdict", "stage", "elapsed duration", "last observed command", "focused rerun", "non-authoritative aggregate"], "aggregate timeout verdict evidence is incomplete"),
    ("runtime-limitations.md", [".NET 10 desktop", "Vulkan", "SkiaSharp preview", "unsupported macOS/mobile/browser", "no software-renderer fallback"], "runtime limitation evidence is incomplete"),
]
hits = []
for file_name, terms, reason in checks:
    path = readiness / file_name
    if not path.exists():
        hits.append({
            "path": str(path),
            "status": "missing",
            "reason": f"missing {file_name}",
            "missing_terms": terms,
            "missing_sections": [],
            "blocking": True,
            "validation_area": "readiness-contract",
        })
        continue
    text = path.read_text(encoding="utf-8")
    missing = [term for term in terms if term.lower() not in text.lower()]
    if missing:
        hits.append({
            "path": str(path),
            "status": "incomplete",
            "reason": reason,
            "missing": missing,
            "missing_terms": missing,
            "missing_sections": [],
            "blocking": True,
            "validation_area": "readiness-contract",
        })
out.write_text(json.dumps(hits, indent=2) + "\n", encoding="utf-8")
print(f"  readiness-contract: {len(hits)} blocking")
for hit in hits:
    missing_terms = ",".join(hit.get("missing_terms") or [])
    missing_sections = ",".join(hit.get("missing_sections") or [])
    print(
        f"    [BLOCK] validation-area=readiness-contract path={hit['path']} status={hit['status']} "
        f"reason={hit['reason']} missing-terms={missing_terms} missing-sections={missing_sections} blocking=true",
        file=sys.stderr,
    )
PYEOF
READINESS_CONTRACT_HITS=$(python3 -c "import json; print(len(json.load(open('$READINESS_CONTRACT_HITS_JSON'))))")
echo

# --- 4. persistent launch evidence scan -------------------------------------
echo "[2b/3] Persistent launch evidence scan..."

PERSISTENT_LAUNCH_HITS_JSON="$READINESS_DIR/persistent-launch-hits.json"
python3 - "$FEATURE_DIR" "$READINESS_DIR" "$PERSISTENT_LAUNCH_HITS_JSON" <<'PYEOF'
import json, re, sys
from pathlib import Path

feature = Path(sys.argv[1])
readiness = Path(sys.argv[2])
out = Path(sys.argv[3])

required_fields = [
    "status",
    "mode",
    "command",
    "window-opened",
    "input-dispatch",
    "exit-path",
    "blocked-stage",
    "classification",
    "category",
    "message",
]

def read_text(path):
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except OSError:
        return ""

feature_text = "\n".join(
    read_text(feature / name)
    for name in ["spec.md", "plan.md", "tasks.md"]
)

requires_persistent = any(
    marker in feature_text.lower()
    for marker in [
        "supported-host persistent launch",
        "persistent graphical launch",
        "persistent viewer contract",
        "mode=persistent-window",
        "mode=interactive-window",
    ]
)

hits = []
artifacts = []
bounded_helpers = []
unsupported_launches = []

def parse_key_values(text):
    values = {}
    for line in text.replace("\r\n", "\n").split("\n"):
        stripped = line.strip()
        if not stripped or stripped.startswith("#") or "=" not in stripped:
            continue

        # Generated product commands often emit compact stdout such as
        # `status=ok mode=interactive-window ...`; parse every token while
        # still allowing free-text values until the next key.
        matches = list(re.finditer(r"(?<!\S)([A-Za-z0-9_-]+)=([^=]*?)(?=\s+[A-Za-z0-9_-]+=|$)", stripped))
        if matches:
            for match in matches:
                values[match.group(1).strip().lower()] = match.group(2).strip()
        else:
            key, value = stripped.split("=", 1)
            values[key.strip().lower()] = value.strip()
    return values

def missing_keys(values, required):
    return [key for key in required if key not in values or not values.get(key, "").strip()]

if requires_persistent:
    for path in sorted(readiness.rglob("*")):
        if not path.is_file() or path.suffix.lower() not in {".txt", ".md", ".log"}:
            continue

        text = read_text(path)
        lower = text.lower()
        values = parse_key_values(text)
        relative = str(path.relative_to(readiness))
        relative_lower = relative.lower()

        if relative_lower.startswith("audit-fixtures/") or relative_lower.startswith("audit-rejections/"):
            continue
        if relative_lower in {
            "governance-risk-levels.md",
            "aggregate-hang-diagnostics.md",
            "runtime-limitations.md",
            "task-graph.md",
        }:
            continue

        has_bounded_helper = (
            values.get("smoke") == "bounded-viewer"
            or values.get("mode") == "persistent-evidence"
            or any(key in values for key in ["frames-rendered", "frame-count", "scene-evidence-format"])
        )
        if has_bounded_helper:
            bounded_helpers.append(relative)

        is_persistent = values.get("mode") in {"interactive-window", "persistent-window"} or "mode=interactive-window" in lower or "mode=persistent-window" in lower
        status = values.get("status", "")
        if is_persistent:
            artifacts.append((relative, values))
            if status == "unsupported":
                unsupported_launches.append(relative)
            if status == "ok" and "supported-host-persistent-launch" in relative_lower:
                missing = [field for field in required_fields if not values.get(field)]
                if missing:
                    hits.append({
                        "path": relative,
                        "reason": "missing persistent launch fields",
                        "missing": missing,
                    })

    supported_ok = [
        (relative, values)
        for relative, values in artifacts
        if values.get("status") == "ok"
        and values.get("mode") in {"interactive-window", "persistent-window"}
        and values.get("window-opened") == "true"
        and values.get("input-dispatch") in {"true", "verified", "false", "not-verified"}
        and values.get("exit-path") == "true"
        and all(values.get(field) for field in required_fields)
    ]

    if not supported_ok:
        hits.append({
            "path": str(readiness / "supported-host-persistent-launch.txt"),
            "reason": "missing supported-host persistent launch evidence",
            "required": required_fields,
        })

        if bounded_helpers:
            hits.append({
                "path": ", ".join(bounded_helpers),
                "reason": "bounded-only substitution",
            })

        if unsupported_launches:
            hits.append({
                "path": ", ".join(unsupported_launches),
                "reason": "unsupported-host-only persistent launch evidence",
            })

out.write_text(json.dumps(hits, indent=2) + "\n", encoding="utf-8")
print(f"  persistent-launch: {len(hits)} blocking")
for hit in hits:
    missing = hit.get("missing")
    detail = f" missing={','.join(missing)}" if missing else ""
    print(f"    [BLOCK] {hit['path']} ({hit['reason']}){detail}", file=sys.stderr)
PYEOF
PERSISTENT_LAUNCH_HITS=$(python3 -c "import json; print(len(json.load(open('$PERSISTENT_LAUNCH_HITS_JSON'))))")
echo

# --- 5. persistent GUI runtime readiness scan -------------------------------
echo "[2c/3] Persistent GUI runtime readiness scan..."

PERSISTENT_GUI_RUNTIME_HITS_JSON="$READINESS_DIR/persistent-gui-runtime-hits.json"
python3 - "$FEATURE_DIR" "$READINESS_DIR" "$PERSISTENT_GUI_RUNTIME_HITS_JSON" <<'PYEOF'
import json, re, sys
from pathlib import Path

feature = Path(sys.argv[1])
readiness = Path(sys.argv[2])
out = Path(sys.argv[3])

def read_text(path):
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except OSError:
        return ""

def parse_key_values(text):
    values = {}
    for line in text.replace("\r\n", "\n").split("\n"):
        stripped = line.strip()
        if not stripped or stripped.startswith("#") or "=" not in stripped:
            continue
        matches = list(re.finditer(r"(?<!\S)([A-Za-z0-9_-]+)=([^=]*?)(?=\s+[A-Za-z0-9_-]+=|$)", stripped))
        if matches:
            for match in matches:
                values[match.group(1).strip().lower()] = match.group(2).strip()
        else:
            key, value = stripped.split("=", 1)
            values[key.strip().lower()] = value.strip()
    return values

def missing_keys(values, required):
    return [key for key in required if key not in values or not values.get(key, "").strip()]

feature_text = "\n".join(read_text(feature / name) for name in ["spec.md", "plan.md", "tasks.md"]).lower()
requires_runtime_scan = any(
    marker in feature_text
    for marker in [
        "persistent gui runtime",
        "persistent-gui-runtime",
        "interactive-lifecycle.md",
        "evidence-launch-mode.md",
        "game-visual-evidence.md",
        "package-resolution.md",
        "generated-verify.md",
    ]
)

required_files = [
    "interactive-lifecycle.md",
    "evidence-launch-mode.md",
    "container-session-diagnostics.md",
    "package-resolution.md",
    "generated-verify.md",
    "game-visual-evidence.md",
    "task-workflow-guidance.md",
    "evidence-audit.md",
]

hits = []
if requires_runtime_scan:
    for file_name in required_files:
        if not (readiness / file_name).exists():
            hits.append({
                "path": str(readiness / file_name),
                "reason": "missing required readiness files",
                "missing": [file_name],
            })

    interactive_path = readiness / "interactive-lifecycle.md"
    interactive_text = read_text(interactive_path)
    interactive_lower = interactive_text.lower()
    interactive_values = parse_key_values(interactive_text)
    if interactive_path.exists():
        mode = interactive_values.get("mode", "")
        self_closed = interactive_values.get("self-closed-for-evidence", "")
        first_frame_only = interactive_values.get("first-frame-only", "")
        if mode == "persistent-evidence" or self_closed == "true" or first_frame_only == "true":
            hits.append({
                "path": str(interactive_path),
                "reason": "bounded-only substitution for interactive evidence",
            })

    package_path = readiness / "package-resolution.md"
    package_text = read_text(package_path)
    package_lower = package_text.lower()
    package_values = parse_key_values(package_text)
    if package_path.exists():
        missing_package_keys = missing_keys(package_values, ["exact-match", "requested-version", "resolved-version", "package-source"])
        if missing_package_keys:
            hits.append({
                "path": str(package_path),
                "reason": "missing readiness acceptance keywords",
                "missing": missing_package_keys,
            })

        exact = package_values.get("exact-match", "").lower()
        if exact not in {"true", "yes"} or "nu1603" in package_lower or "mismatch" in package_lower:
            hits.append({
                "path": str(package_path),
                "reason": "unresolved package mismatch",
            })

    generated_path = readiness / "generated-verify.md"
    generated_text = read_text(generated_path)
    generated_values = parse_key_values(generated_text)
    if generated_path.exists():
        missing_generated_keys = missing_keys(generated_values, ["generated-tests-exist", "generated-tests-ran", "authoritative", "failure-class"])
        if missing_generated_keys:
            hits.append({
                "path": str(generated_path),
                "reason": "missing readiness acceptance keywords",
                "missing": missing_generated_keys,
            })

        tests_exist = generated_values.get("generated-tests-exist", "").lower()
        tests_ran = generated_values.get("generated-tests-ran", "").lower()
        authoritative = generated_values.get("authoritative", "").lower()
        if tests_exist in {"true", "yes"} and tests_ran not in {"true", "yes"}:
            hits.append({
                "path": str(generated_path),
                "reason": "generated tests exist but did not run",
            })
        elif authoritative in {"false", "no"}:
            hits.append({
                "path": str(generated_path),
                "reason": "generated verification is non-authoritative",
            })

    visual_path = readiness / "game-visual-evidence.md"
    visual_text = read_text(visual_path)
    visual_lower = visual_text.lower()
    visual_values = parse_key_values(visual_text)
    if visual_path.exists():
        missing_visual_keys = missing_keys(visual_values, ["supported-host", "evidence-kind", "board-readable", "input-or-progress-observed"])
        if missing_visual_keys:
            hits.append({
                "path": str(visual_path),
                "reason": "missing readiness acceptance keywords",
                "missing": missing_visual_keys,
            })

        supported = visual_values.get("supported-host", "").lower() in {"true", "yes"}
        kind = visual_values.get("evidence-kind", "").lower()
        board = visual_values.get("board-readable", "").lower()
        progress = visual_values.get("input-or-progress-observed", "").lower()
        text_only = kind in {"metadata", "text", "scene-metadata", "text-only"} or "text-only visual metadata" in visual_lower
        if supported and text_only:
            hits.append({
                "path": str(visual_path),
                "reason": "text-only visual metadata on supported host",
            })
        elif kind in {"screenshot", "pixel-readback"} and (board not in {"true", "yes"} or progress not in {"true", "yes"}):
            hits.append({
                "path": str(visual_path),
                "reason": "visual game evidence missing board/input proof",
            })

out.write_text(json.dumps(hits, indent=2) + "\n", encoding="utf-8")
print(f"  persistent-gui-runtime: {len(hits)} blocking")
for hit in hits:
    missing = hit.get("missing")
    detail = f" missing={','.join(missing)}" if missing else ""
    print(f"    [BLOCK] {hit['path']} ({hit['reason']}){detail}", file=sys.stderr)
PYEOF
PERSISTENT_GUI_RUNTIME_HITS=$(python3 -c "import json; print(len(json.load(open('$PERSISTENT_GUI_RUNTIME_HITS_JSON'))))")
echo

# --- 5b. window visibility readiness scan ----------------------------------
echo "[2d/3] Window visibility readiness scan..."

WINDOW_VISIBILITY_HITS_JSON="$READINESS_DIR/window-visibility-hits.json"
python3 - "$FEATURE_DIR" "$READINESS_DIR" "$WINDOW_VISIBILITY_HITS_JSON" <<'PYEOF'
import json, re, sys
from pathlib import Path

feature = Path(sys.argv[1])
readiness = Path(sys.argv[2])
out = Path(sys.argv[3])

def read_text(path):
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except OSError:
        return ""

def parse_key_values(text):
    values = {}
    for line in text.replace("\r\n", "\n").split("\n"):
        stripped = line.strip()
        if not stripped or stripped.startswith("#") or "=" not in stripped:
            continue
        matches = list(re.finditer(r"(?<!\S)([A-Za-z0-9_-]+)=([^=]*?)(?=\s+[A-Za-z0-9_-]+=|$)", stripped))
        if matches:
            for match in matches:
                values[match.group(1).strip().lower()] = match.group(2).strip()
        else:
            key, value = stripped.split("=", 1)
            values[key.strip().lower()] = value.strip()
    return values

def missing_keys(values, required):
    return [key for key in required if key not in values or not values.get(key, "").strip()]

def truthy(value):
    return value.lower() in {"true", "yes", "observed:true", "verified", "ok"}

def falsy(value):
    return value.lower() in {"false", "no", "observed:false", "missing", "none"}

feature_text = "\n".join(read_text(feature / name) for name in ["spec.md", "plan.md", "tasks.md"]).lower()
requires_window_visibility = any(
    marker in feature_text
    for marker in [
        "fix window visibility",
        "interactive-visible-window.md",
        "close-reason-separation.md",
        "real-image-evidence.md",
        "generated-validation.md",
    ]
)

required_files = [
    "interactive-visible-window.md",
    "close-reason-separation.md",
    "window-state-diagnostics.md",
    "window-options.md",
    "real-image-evidence.md",
    "generated-validation.md",
    "evidence-audit.md",
]

hits = []
if requires_window_visibility:
    for file_name in required_files:
        if not (readiness / file_name).exists():
            hits.append({
                "path": str(readiness / file_name),
                "reason": "missing required window visibility readiness file",
                "missing": [file_name],
            })

    interactive_path = readiness / "interactive-visible-window.md"
    interactive_text = read_text(interactive_path)
    interactive_lower = interactive_text.lower()
    interactive_values = parse_key_values(interactive_text)
    if interactive_path.exists():
        missing = missing_keys(interactive_values, ["status", "mode", "window-visible", "accessible-window", "first-frame-presented", "self-closed-for-evidence"])
        if missing:
            hits.append({
                "path": str(interactive_path),
                "reason": "missing visible-window readiness fields",
                "missing": missing,
            })

        status_ok = interactive_values.get("status", "").lower() in {"ok", "pass", "success"}
        taskbar_only = truthy(interactive_values.get("taskbar-entry", "")) and (
            falsy(interactive_values.get("window-visible", ""))
            or falsy(interactive_values.get("accessible-window", ""))
        )
        process_only = truthy(interactive_values.get("process-running", "")) and (
            "process/taskbar-only" in interactive_lower
            or "taskbar-only" in interactive_lower
            or truthy(interactive_values.get("process-only", ""))
        )
        if status_ok and (taskbar_only or process_only):
            hits.append({
                "path": str(interactive_path),
                "reason": "process/taskbar-only visible-window substitution",
            })

    close_path = readiness / "close-reason-separation.md"
    close_text = read_text(close_path)
    close_values = parse_key_values(close_text)
    if close_path.exists():
        close_reason = close_values.get("close-reason", "").lower()
        user_close = truthy(close_values.get("user-close-observed", ""))
        evidence_close = truthy(close_values.get("evidence-close-observed", "")) or close_reason in {"evidence-close", "framework-close", "host-close", "bounded-evidence-close"}
        if evidence_close and user_close:
            hits.append({
                "path": str(close_path),
                "reason": "evidence close reported as user close",
            })

    diagnostics_path = readiness / "window-state-diagnostics.md"
    diagnostics_text = read_text(diagnostics_path)
    diagnostics_lower = diagnostics_text.lower()
    diagnostics_values = parse_key_values(diagnostics_text)
    if diagnostics_path.exists():
        required_classes = ["environment-session", "window-visibility", "app-lifecycle", "product-defect"]
        missing_classes = [
            cls for cls in required_classes
            if f"diagnostic-class={cls}" not in diagnostics_lower and f"failure-class={cls}" not in diagnostics_lower
        ]
        if missing_classes:
            hits.append({
                "path": str(diagnostics_path),
                "reason": "missing diagnostic classes",
                "missing": missing_classes,
            })

        required_facts = ["native-handle", "visible", "focusable", "renderable-surface", "input-devices"]
        missing_facts = missing_keys(diagnostics_values, required_facts)
        if missing_facts:
            hits.append({
                "path": str(diagnostics_path),
                "reason": "missing observable-vs-unsupported native facts",
                "missing": missing_facts,
            })

        status_ok = diagnostics_values.get("status", "").lower() in {"ok", "pass", "success"}
        diagnostic_taskbar_only = truthy(diagnostics_values.get("taskbar-entry", "")) or "taskbar-only" in diagnostics_lower
        if status_ok and diagnostic_taskbar_only:
            hits.append({
                "path": str(diagnostics_path),
                "reason": "process/taskbar-only success claim",
            })

        unsupported_only = (
            truthy(diagnostics_values.get("unsupported-host-only", ""))
            or ("unsupported-host-only" in diagnostics_lower and "observed:true" not in diagnostics_lower)
            or (
                diagnostics_values.get("visible", "").lower() == "unsupported"
                and diagnostics_values.get("renderable-surface", "").lower() == "unsupported"
                and "observed:true" not in diagnostics_lower
            )
        )
        interactive_status_ok = interactive_values.get("status", "").lower() in {"ok", "pass", "success"}
        interactive_visible_unsupported = interactive_values.get("window-visible", "").lower() in {"unsupported", "unavailable"}
        if unsupported_only and (interactive_status_ok or interactive_visible_unsupported):
            hits.append({
                "path": str(diagnostics_path),
                "reason": "unsupported-host-only visible-window claim",
            })

    options_path = readiness / "window-options.md"
    options_text = read_text(options_path)
    options_lower = options_text.lower()
    if options_path.exists():
        required_options = ["resize", "maximize", "startup-state", "startup-position", "backend"]
        missing_options = [
            option for option in required_options
            if f"option={option}" not in options_lower and f"{option}=" not in options_lower and f"{option.replace('-', '_')}=" not in options_lower
        ]
        if missing_options:
            hits.append({
                "path": str(options_path),
                "reason": "missing option rows",
                "missing": missing_options,
            })

        unsupported_requested = any(
            token in options_lower
            for token in [
                "requested=opengl",
                "requested=software",
                "requested=minimized",
                "requested=fullscreen",
                "unsupported-setting=true",
            ]
        )
        unsupported_diagnosed = any(
            token in options_lower
            for token in [
                "status=unsupported",
                "status=degraded",
                "status=failed",
                "diagnostic-class=window-options",
                "failure-class=window-options",
            ]
        )
        if unsupported_requested and not unsupported_diagnosed:
            hits.append({
                "path": str(options_path),
                "reason": "silently ignored unsupported window option",
            })

        option_failure_hidden = (
            ("status=failed" in options_lower or "failed=true" in options_lower)
            and ("diagnostic-class=app-lifecycle" in options_lower or "failure-class=app-lifecycle" in options_lower)
            and "diagnostic-class=window-options" not in options_lower
            and "failure-class=window-options" not in options_lower
        )
        if option_failure_hidden:
            hits.append({
                "path": str(options_path),
                "reason": "window-options failure hidden under app-lifecycle",
            })

    image_path = readiness / "real-image-evidence.md"
    image_text = read_text(image_path)
    image_lower = image_text.lower()
    image_values = parse_key_values(image_text)
    if image_path.exists():
        image_kind = image_values.get("evidence-kind", "").lower()
        image_status = image_values.get("status", "").lower()
        requested = truthy(image_values.get("requested-image-evidence", "")) or image_kind in {"screenshot", "image"}
        metadata_only = image_values.get("artifact-kind", "").lower() in {"metadata", "hash", "text", "text-only"} or "metadata-only screenshot" in image_lower
        decodable_value = image_values.get("artifact-decodable", image_values.get("image-decodable", image_values.get("decodable", ""))).lower()
        undecodable = falsy(decodable_value)
        unsupported_screenshot = image_kind == "screenshot" and image_status == "unsupported"
        if requested and (metadata_only or undecodable):
            hits.append({
                "path": str(image_path),
                "reason": "metadata-only screenshot claim",
            })
        if unsupported_screenshot and not image_values.get("unsupported-host-reason", "").strip():
            hits.append({
                "path": str(image_path),
                "reason": "unsupported screenshot missing unsupported-host reason",
            })
        if unsupported_screenshot and (
            truthy(image_values.get("proves-desktop-visibility", ""))
            or truthy(decodable_value)
            or image_values.get("artifact-kind", "").lower() == "image"
            or image_values.get("image-artifact", "").strip()
        ):
            hits.append({
                "path": str(image_path),
                "reason": "unsupported screenshot cannot prove desktop visibility",
            })
        if image_kind in {"image", "screenshot", "pixel-readback", "metadata-hash"}:
            if "proves-scene-rendering" not in image_values or "proves-desktop-visibility" not in image_values:
                hits.append({
                    "path": str(image_path),
                    "reason": "missing visual evidence proof fields",
                })
        if image_kind == "pixel-readback" and truthy(image_values.get("proves-desktop-visibility", "")):
            hits.append({
                "path": str(image_path),
                "reason": "pixel-readback cannot prove desktop visibility",
            })
        if requested and decodable_value and not (truthy(decodable_value) or falsy(decodable_value)):
            hits.append({
                "path": str(image_path),
                "reason": "corrupt image metadata record",
            })
        artifact_path = image_values.get("image-artifact", image_values.get("artifact-path", "")).strip()
        if artifact_path and (Path(artifact_path).is_absolute() or ".." in Path(artifact_path).parts):
            hits.append({
                "path": str(image_path),
                "reason": "hostile artifact path",
            })

    generated_path = readiness / "generated-validation.md"
    generated_text = read_text(generated_path)
    generated_lower = generated_text.lower()
    generated_values = parse_key_values(generated_text)
    if generated_path.exists():
        missing = missing_keys(generated_values, ["exact-package-match", "generated-tests-ran", "authoritative", "failure-class"])
        if missing:
            hits.append({
                "path": str(generated_path),
                "reason": "missing generated validation fields",
                "missing": missing,
            })

        exact = generated_values.get("exact-package-match", "").lower()
        if exact not in {"true", "yes"} or "nu1603" in generated_lower or "package mismatch" in generated_lower:
            hits.append({
                "path": str(generated_path),
                "reason": "unresolved package mismatch",
            })

        tests_exist = generated_values.get("generated-tests-exist", "").lower()
        tests_ran = generated_values.get("generated-tests-ran", "").lower()
        if tests_exist in {"true", "yes"} and tests_ran not in {"true", "yes"}:
            hits.append({
                "path": str(generated_path),
                "reason": "missing generated test execution",
            })

out.write_text(json.dumps(hits, indent=2) + "\n", encoding="utf-8")
print(f"  window-visibility: {len(hits)} blocking")
for hit in hits:
    missing = hit.get("missing")
    detail = f" missing={','.join(missing)}" if missing else ""
    print(f"    [BLOCK] {hit['path']} ({hit['reason']}){detail}", file=sys.stderr)
PYEOF
WINDOW_VISIBILITY_HITS=$(python3 -c "import json; print(len(json.load(open('$WINDOW_VISIBILITY_HITS_JSON'))))")
echo

# --- 6. diff scan -----------------------------------------------------------
echo "[3/3] Diff scan against workspace feature base..."

# Resolve feature base.
if [[ -z "$BASE_REF" ]]; then
  if git -C "$FEATURE_DIR" show-ref --verify --quiet refs/heads/main; then
    BASE_REF="main"
  elif git -C "$FEATURE_DIR" show-ref --verify --quiet refs/heads/master; then
    BASE_REF="master"
  else
    echo "  warning: neither main nor master exists; using HEAD~1"
    BASE_REF="HEAD~1"
  fi
fi

# Repo root (diff must be run from the top-level of the repo). Fixture replays
# under readiness/audit-rejections intentionally exercise copied readiness
# artifacts, not the live workspace diff.
if [[ "$FEATURE_DIR" == *"/readiness/audit-rejections/"* ]]; then
  echo "  warning: fixture replay; skipping workspace diff scan"
  REPO_ROOT=""
elif REPO_ROOT="$(git -C "$FEATURE_DIR" rev-parse --show-toplevel 2>/dev/null)"; then
  :
else
  echo "  warning: not in a git repo; skipping diff scan"
  REPO_ROOT=""
fi

DIFF_HITS_JSON="$READINESS_DIR/diff-scan-hits.json"
BLOCK_HITS=0
ADVISORY_HITS=0

if [[ -n "$REPO_ROOT" ]]; then
  # Dump the current workspace diff once; re-scan per pattern in Python for
  # readable reporting. Compare the working tree to the feature merge-base so
  # pre-commit implementation passes audit the code that is actually present.
  DIFF_FILE="$(mktemp)"
  trap 'rm -f "$DIFF_FILE"' EXIT
  MERGE_BASE="$(git -C "$REPO_ROOT" merge-base "$BASE_REF" HEAD 2>/dev/null || true)"
  if [[ -z "$MERGE_BASE" ]]; then
    MERGE_BASE="$BASE_REF"
  fi
  git -C "$REPO_ROOT" diff "$MERGE_BASE" --unified=0 >"$DIFF_FILE" || true

  OVERRIDES_FILE=""
  if [[ -f "$REPO_ROOT/.specify/audit-patterns.overrides.yml" ]]; then
    OVERRIDES_FILE="$REPO_ROOT/.specify/audit-patterns.overrides.yml"
  fi

  set +e
  python3 - "$PATTERNS_FILE" "$DIFF_FILE" "$DIFF_HITS_JSON" "$OVERRIDES_FILE" <<'PYEOF'
import json, re, sys
from pathlib import Path

patterns_path = Path(sys.argv[1])
diff_path = Path(sys.argv[2])
out_path = Path(sys.argv[3])
overrides_path = Path(sys.argv[4]) if len(sys.argv) > 4 and sys.argv[4] else None

# Minimal YAML parsing — we only need the patterns + whitelist blocks.
# This is NOT a general YAML parser; it's tuned to the audit-patterns.yml
# shape defined in the extension. pyyaml isn't a hard dep.

def parse(text):
    patterns = []
    whitelist = []
    severity_overrides = {}
    state = None
    cur = None

    def flush():
        nonlocal cur, state
        if not cur:
            return
        if state == "patterns":
            patterns.append(cur)
        elif state == "whitelist" and "pattern_id" in cur:
            whitelist.append(cur)
        cur = None

    for raw in text.splitlines():
        line = raw.rstrip()
        if not line or line.lstrip().startswith("#"):
            continue
        if line.startswith("patterns:"):
            flush()
            state = "patterns"; continue
        if line.startswith("whitelist:"):
            flush()
            state = "whitelist"; continue
        if line.startswith("severity_overrides:"):
            flush()
            state = "severity_overrides"; continue
        if state == "patterns":
            if line.startswith("  - "):
                flush()
                cur = {}
                # first field is on the same line
                field = line[4:]
                if ":" in field:
                    k, v = field.split(":", 1)
                    cur[k.strip()] = v.strip().strip("'\"")
            elif line.startswith("    ") and cur is not None and ":" in line:
                k, v = line.strip().split(":", 1)
                cur[k.strip()] = v.strip().strip("'\"")
        elif state == "whitelist":
            if line.startswith("  - "):
                flush()
                cur = {}
                field = line[4:]
                if ":" in field:
                    k, v = field.split(":", 1)
                    cur[k.strip()] = v.strip().strip("'\"")
            elif line.startswith("    ") and cur is not None and ":" in line:
                k, v = line.strip().split(":", 1)
                cur[k.strip()] = v.strip().strip("'\"")
        elif state == "severity_overrides":
            m = re.match(r"^\s{2,}([A-Za-z0-9_-]+)\s*:\s*(block|advisory)\s*$", line)
            if m:
                severity_overrides[m.group(1)] = m.group(2)
    flush()
    return patterns, whitelist, severity_overrides

patterns, whitelist, overrides = parse(patterns_path.read_text())
if overrides_path and overrides_path.exists():
    _, _, per_project_overrides = parse(overrides_path.read_text())
    overrides.update(per_project_overrides)
for p in patterns:
    if p["id"] in overrides:
        p["severity"] = overrides[p["id"]]

# Parse diff to get (path, line_no, content) tuples.
hits = []
cur_file = None
cur_lineno = 0
for raw in diff_path.read_text(errors="replace").splitlines():
    if raw.startswith("+++ b/"):
        cur_file = raw[6:]
        continue
    if raw.startswith("+++ "):
        cur_file = raw[4:]
        continue
    m = re.match(r"^@@ -\d+(?:,\d+)? \+(\d+)(?:,\d+)? @@", raw)
    if m:
        cur_lineno = int(m.group(1)) - 1
        continue
    if raw.startswith("+") and not raw.startswith("+++"):
        cur_lineno += 1
        content = raw[1:]
        if cur_file is None:
            continue
        for p in patterns:
            try:
                if re.search(p["regex"], content):
                    wl_hit = False
                    import fnmatch
                    for wl in whitelist:
                        if wl.get("pattern_id") != p["id"]:
                            continue
                        fg = wl.get("file_glob")
                        lr = wl.get("line_regex")
                        if fg and not fnmatch.fnmatch(cur_file, fg):
                            continue
                        if lr and not re.search(lr, content):
                            continue
                        wl_hit = True; break
                    if wl_hit:
                        continue
                    hits.append({
                        "file": cur_file,
                        "line": cur_lineno,
                        "pattern": p["id"],
                        "severity": p.get("severity", "block"),
                        "reason": p.get("reason", ""),
                        "match": content.strip()[:120],
                    })
            except re.error:
                pass
    elif raw.startswith(" "):
        cur_lineno += 1

# Summary to stderr for the caller, and write JSON.
block = [h for h in hits if h["severity"] == "block"]
adv = [h for h in hits if h["severity"] == "advisory"]

print(f"  diff-scan: {len(block)} blocking, {len(adv)} advisory")
out_path.write_text(json.dumps({
    "base_ref": None,
    "blocking": block,
    "advisory": adv,
}, indent=2) + "\n")

# Print blockers to stderr so the audit output is rich.
for h in block:
    print(f"    [BLOCK] {h['pattern']} at {h['file']}:{h['line']}  ({h['reason']})", file=sys.stderr)
    print(f"            {h['match']}", file=sys.stderr)
for h in adv:
    print(f"    [adv]   {h['pattern']} at {h['file']}:{h['line']}  ({h['reason']})", file=sys.stderr)

# Return count for the shell caller via exit code: 0 = no blockers, 2 = blockers.
sys.exit(0 if not block else 2)
PYEOF

  DIFF_EXIT=$?
  set -e
  if [[ $DIFF_EXIT -eq 2 ]]; then
    BLOCK_HITS=$(python3 -c "import json; d=json.load(open('$DIFF_HITS_JSON')); print(len(d['blocking']))")
  elif [[ $DIFF_EXIT -ne 0 ]]; then
    die "diff-scan helper failed (exit $DIFF_EXIT)"
  fi

  if [[ -f "$DIFF_HITS_JSON" ]]; then
    BLOCK_HITS=$(python3 -c "import json; d=json.load(open('$DIFF_HITS_JSON')); print(len(d['blocking']))")
    ADVISORY_HITS=$(python3 -c "import json; d=json.load(open('$DIFF_HITS_JSON')); print(len(d['advisory']))")
  fi
else
  echo "  diff scan skipped (not a git repo)"
  cat > "$DIFF_HITS_JSON" <<'JSON'
{
  "base_ref": null,
  "blocking": [],
  "advisory": []
}
JSON
fi
echo

# --- 4. verdict -------------------------------------------------------------
SYNTHETIC_JSON="$READINESS_DIR/synthetic-evidence.json"

if [[ -n "$ACCEPT_SYNTHETIC" ]]; then
  python3 - "$SYNTHETIC_JSON" "$GRAPH_JSON" "$DIFF_HITS_JSON" <<PYEOF
import json, sys
from datetime import datetime, timezone
out = sys.argv[1]; graph = sys.argv[2]; diff = sys.argv[3]
payload = {
    "schema_version": "1.0",
    "accept_synthetic": {
        "timestamp": datetime.now(timezone.utc).isoformat(),
        "justification": """$ACCEPT_SYNTHETIC""",
    },
    "graph_summary": {
        "synthetic": $SYNTHETIC_COUNT,
        "auto_synthetic": $AUTO_SYNTHETIC_COUNT,
    },
    "diff_scan": json.load(open(diff)),
}
with open(out, "w") as f:
    json.dump(payload, f, indent=2)
    f.write("\n")
PYEOF
fi

TOTAL_BLOCKERS=$((UNACCEPTED_SYNTHETIC_COUNT + INVALID_SEH_COUNT + BLOCK_HITS + READINESS_CONTRACT_HITS + PERSISTENT_LAUNCH_HITS + PERSISTENT_GUI_RUNTIME_HITS + WINDOW_VISIBILITY_HITS))

echo "=== Verdict ==="
if [[ $TOTAL_BLOCKERS -eq 0 ]]; then
  echo "verdict=PASS"
  echo "real-tasks=$(python3 -c "import json; d=json.load(open('$GRAPH_JSON')); print(sum(1 for t in d['tasks'] if t['effective'] == 'done'))")"
  echo "accepted-seh-tasks=$ACCEPTED_SEH_COUNT"
  echo "unaccepted-synthetic-tasks=0"
  echo "auto-synthetic-tasks=$AUTO_SYNTHETIC_COUNT"
  echo "late-seh-tasks=0"
  echo "diff-scan-hits=$BLOCK_HITS"
  echo "message=PASS — accepted [SEH] tasks remain synthetic but are design-approved; no unaccepted synthetic tasks, readiness contract hits, or blocking diff-scan hits."
  exit 0
fi

echo "verdict=FAIL"
echo "real-tasks=$(python3 -c "import json; d=json.load(open('$GRAPH_JSON')); print(sum(1 for t in d['tasks'] if t['effective'] == 'done'))")"
echo "accepted-seh-tasks=$ACCEPTED_SEH_COUNT"
echo "unaccepted-synthetic-tasks=$UNACCEPTED_SYNTHETIC_COUNT"
echo "auto-synthetic-tasks=$AUTO_SYNTHETIC_COUNT"
echo "late-seh-tasks=$LATE_SEH_COUNT"
echo "diff-scan-hits=$BLOCK_HITS"
echo "message=FAIL — unresolved synthetic evidence, late [SEH] classification, readiness contract hits, persistent launch/runtime hits, or blocking diff-scan hits remain."
python3 - "$SEH_SUMMARY_JSON" <<'PYEOF'
import json, sys
d = json.load(open(sys.argv[1]))
for diagnostic in d.get("diagnostics", []):
    print(
        "diagnostic="
        + diagnostic["task"]
        + " failed-rule="
        + diagnostic["failed_rule"]
        + " source="
        + diagnostic["source"]
        + " required-action="
        + diagnostic["required_action"]
    )
PYEOF
echo "  synthetic tasks (declared):     $SYNTHETIC_COUNT"
echo "  auto-synthetic tasks (computed): $AUTO_SYNTHETIC_COUNT"
echo "  accepted [SEH] tasks:           $ACCEPTED_SEH_COUNT"
echo "  unaccepted synthetic tasks:      $UNACCEPTED_SYNTHETIC_COUNT"
echo "  late [SEH] tasks:                $LATE_SEH_COUNT"
if [[ $BLOCK_HITS -gt 0 ]]; then
  echo "  blocking diff-scan hits:         (see above)"
fi
if [[ $READINESS_CONTRACT_HITS -gt 0 ]]; then
  echo "  readiness contract hits:         $READINESS_CONTRACT_HITS"
fi
if [[ $PERSISTENT_LAUNCH_HITS -gt 0 ]]; then
  echo "  persistent launch hits:          $PERSISTENT_LAUNCH_HITS"
fi
if [[ $PERSISTENT_GUI_RUNTIME_HITS -gt 0 ]]; then
  echo "  persistent GUI runtime hits:     $PERSISTENT_GUI_RUNTIME_HITS"
fi
if [[ $WINDOW_VISIBILITY_HITS -gt 0 ]]; then
  echo "  window visibility hits:          $WINDOW_VISIBILITY_HITS"
fi
if [[ $ADVISORY_HITS -gt 0 ]]; then
  echo "  advisory diff-scan hits:         $ADVISORY_HITS (printed, not blocking)"
fi

echo
if [[ -n "$ACCEPT_SYNTHETIC" ]]; then
  echo "  --accept-synthetic recorded: $ACCEPT_SYNTHETIC"
  echo "  logged to $SYNTHETIC_JSON"
  echo "  audit still reports failure; merge is a human decision."
else
  echo "  To override, rerun with:  --accept-synthetic \"justification\""
fi

exit 2
