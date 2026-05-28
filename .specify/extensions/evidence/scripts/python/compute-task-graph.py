#!/usr/bin/env python3
"""
compute-task-graph.py — DAG compute + synthetic-evidence propagation.

Parses specs/<feature>/tasks.md and specs/<feature>/tasks.deps.yml, builds
the dependency graph, validates it (acyclic, no dangling refs, every id
present in both files), computes the effective status per task under the
propagation rule, and renders task-graph.json and task-graph.md under
specs/<feature>/readiness/.

Propagation rule:
  effective(T) =
    'S'  if declared(T) == 'S'
    'S*' if declared(T) == 'X' and any d in deps(T): effective(d) in {'S','S*'}
    declared(T) otherwise

Phase-checkpoint edges are auto-injected: every task in Phase N+1 implicitly
depends on the last foundation task of Phase N (the task right before the
"Checkpoint:" line in Phase N). Authors only write non-phase cross-edges
in tasks.deps.yml.

Exit codes:
  0 — graph is valid and readiness artifacts written
  2 — validation failure (cycles, dangling refs, missing ids)
  3 — filesystem / parse failure

Usage:
  compute-task-graph.py <feature-dir>
  compute-task-graph.py specs/042-user-signup

Dependencies: stdlib only. No pyyaml requirement — we parse the simple
id -> [ids] YAML shape ourselves.
"""

from __future__ import annotations

import json
import os
import re
import sys
from collections import defaultdict, deque
from dataclasses import dataclass, field
from pathlib import Path
from datetime import datetime
from typing import Dict, List, Optional, Set, Tuple

# ----------------------------------------------------------------------------
# Data model
# ----------------------------------------------------------------------------

TASK_ID_RE = re.compile(r"\bT\d{3,4}\b")

# Matches a task line: "- [ ] T015 [P] [US1] Implement ..."
# Captures: box content, id, rest-of-line
TASK_LINE_RE = re.compile(
    r"""^\s*
        -\s*\[(?P<box>[ X\-FS*])\]\s+
        (?P<id>T\d{3,4})\b
        (?P<rest>.*)$
    """,
    re.VERBOSE,
)

PHASE_HEADER_RE = re.compile(r"^\s*##\s+Phase\s+(\d+)\s*:", re.IGNORECASE)
CHECKPOINT_RE = re.compile(r"^\s*\*\*Checkpoint", re.IGNORECASE)

# Parsed box char → declared status
BOX_TO_STATUS = {
    " ": "pending",
    "X": "done",
    "S": "synthetic",
    "F": "failed",
    "-": "skipped",
    "*": "invalid",  # "[*]" shouldn't appear; S* is computed
}


@dataclass
class Task:
    id: str
    declared: str  # pending | done | synthetic | failed | skipped
    effective: str = ""  # filled by propagation; adds 'auto-synthetic'
    phase: Optional[int] = None
    story: Optional[str] = None  # "US1" etc.
    tier: Optional[str] = None   # "T1" or "T2"
    parallel: bool = False       # [P] annotation
    line_no: int = 0
    title: str = ""
    skillist_mirror: Optional[List[str]] = None
    skillist: List[str] = field(default_factory=list)
    explicit_deps: List[str] = field(default_factory=list)
    phase_deps: List[str] = field(default_factory=list)
    skill_match_assessments: List[dict] = field(default_factory=list)
    seh_annotation: bool = False
    seh_approval_label: bool = False
    design_source: str = ""
    synthetic_input_class: str = ""
    expected_error_behavior: str = ""
    seh_rationale: str = ""
    seh_acceptance_status: str = ""
    seh_diagnostics: List[str] = field(default_factory=list)

    @property
    def all_deps(self) -> List[str]:
        # Explicit first, then phase-edges, deduped, preserving order.
        seen: Set[str] = set()
        result: List[str] = []
        for d in self.explicit_deps + self.phase_deps:
            if d not in seen:
                seen.add(d)
                result.append(d)
        return result


@dataclass
class GraphResult:
    tasks: Dict[str, Task]
    errors: List[str]
    warnings: List[str]
    root_cause: Dict[str, List[str]]  # task_id -> list of upstream [S] ids

    @property
    def has_errors(self) -> bool:
        return len(self.errors) > 0


# ----------------------------------------------------------------------------
# Parsers
# ----------------------------------------------------------------------------

def parse_tasks_md(path: Path) -> Tuple[Dict[str, Task], List[str]]:
    """Parse tasks.md into id → Task, plus phase structure for edge injection."""
    errors: List[str] = []
    tasks: Dict[str, Task] = {}
    current_phase: Optional[int] = None
    # Phase -> list of task ids in order (pre-checkpoint); used to compute the
    # "last foundation task" for the next phase's implicit edges.
    phase_tasks: Dict[int, List[str]] = defaultdict(list)
    # Phase -> list of task ids seen BEFORE the first Checkpoint marker.
    phase_foundation: Dict[int, List[str]] = defaultdict(list)
    # Whether we've already passed the first checkpoint in the current phase.
    past_checkpoint = False

    try:
        text = path.read_text(encoding="utf-8")
    except OSError as e:
        return {}, [f"Cannot read {path}: {e}"]

    for i, line in enumerate(text.splitlines(), start=1):
        phase_m = PHASE_HEADER_RE.match(line)
        if phase_m:
            current_phase = int(phase_m.group(1))
            past_checkpoint = False
            continue

        if CHECKPOINT_RE.match(line):
            past_checkpoint = True
            continue

        task_m = TASK_LINE_RE.match(line)
        if not task_m:
            continue

        box = task_m.group("box")
        tid = task_m.group("id")
        rest = task_m.group("rest").strip()

        if box == "*":
            errors.append(
                f"tasks.md:{i}: invalid status [*] on {tid} — "
                f"[S*] is computed, never written manually"
            )
            continue

        declared = BOX_TO_STATUS.get(box, "pending")

        # Extract annotations from the rest of the line.
        parallel = bool(re.search(r"\[P\]", rest))
        story_m = re.search(r"\[(US\d+)\]", rest)
        tier_m = re.search(r"\[(T[12])\]", rest)
        seh_annotation = bool(re.search(r"(?<!`)\[SEH\](?!`)", rest))
        seh_approval_label = bool(re.search(r"(?<!`)synthetic-error-handling-approved(?!`)", rest))

        skillist_m = re.search(r"\[skillist:\s*(?P<value>\[\]|[^\]]*)\]", rest)
        skillist_mirror: Optional[List[str]] = None
        if skillist_m:
            raw_value = skillist_m.group("value").strip()
            if raw_value == "[]":
                skillist_mirror = []
            else:
                skillist_mirror = [x.strip() for x in raw_value.split(",") if x.strip()]

        # Title: strip all [..] annotation brackets, keep the sentence.
        title = re.sub(r"(?<!`)\[(P|US\d+|T[12]|SEH)\](?!`)\s*", "", rest).strip()
        title = re.sub(r"\[skillist:\s*(?:\[\]|[^\]])*\]\s*", "", title).strip()

        if tid in tasks:
            errors.append(f"tasks.md:{i}: duplicate task id {tid}")
            continue

        task = Task(
            id=tid,
            declared=declared,
            effective=declared,
            phase=current_phase,
            story=story_m.group(1) if story_m else None,
            tier=tier_m.group(1) if tier_m else None,
            parallel=parallel,
            line_no=i,
            title=title,
            skillist_mirror=skillist_mirror,
            seh_annotation=seh_annotation,
            seh_approval_label=seh_approval_label,
        )
        tasks[tid] = task

        if current_phase is not None:
            phase_tasks[current_phase].append(tid)
            if not past_checkpoint:
                phase_foundation[current_phase].append(tid)

    # Merge Synthetic-Evidence Inventory metadata when present. This reads the
    # governed Markdown table shape and ignores unrelated tables.
    lines = text.splitlines()
    for index, line in enumerate(lines):
        if line.strip().lower() != "## synthetic-evidence inventory":
            continue
        header: Optional[List[str]] = None
        for row in lines[index + 1:]:
            stripped = row.strip()
            if stripped.startswith("## "):
                break
            if not (stripped.startswith("|") and stripped.endswith("|")):
                continue
            cells = [cell.strip() for cell in stripped.strip("|").split("|")]
            if cells and all(cell and set(cell) <= {"-"} for cell in cells):
                continue
            if header is None:
                header = [cell.lower() for cell in cells]
                continue
            if cells and cells[0].startswith("_("):
                continue
            row_data = {header[i]: cells[i] for i in range(min(len(header), len(cells)))}
            task_id_match = TASK_ID_RE.search(row_data.get("task", ""))
            if not task_id_match:
                continue
            task = tasks.get(task_id_match.group(0))
            if not task:
                continue
            label = row_data.get("label", "")
            task.seh_approval_label = task.seh_approval_label or label == "synthetic-error-handling-approved"
            task.design_source = row_data.get("design source", task.design_source).strip()
            task.synthetic_input_class = row_data.get("synthetic input class", task.synthetic_input_class).strip()
            task.expected_error_behavior = row_data.get("expected error behavior", task.expected_error_behavior).strip()
            task.seh_acceptance_status = row_data.get("acceptance status", task.seh_acceptance_status).strip()
            task.seh_rationale = row_data.get("reason", task.seh_rationale).strip()
        break

    for task in tasks.values():
        if task.declared == "synthetic" and (task.seh_annotation or task.seh_approval_label):
            if not task.seh_annotation:
                task.seh_diagnostics.append("missing [SEH] annotation")
            if not task.seh_approval_label:
                task.seh_diagnostics.append("missing synthetic-error-handling-approved label")
            if not task.design_source:
                task.seh_diagnostics.append("missing design-phase source")
            if not task.synthetic_input_class:
                task.seh_diagnostics.append("missing synthetic input class")
            if not task.expected_error_behavior:
                task.seh_diagnostics.append("missing expected error behavior")
            if not task.seh_rationale:
                task.seh_diagnostics.append("missing rationale")
            if task.declared == "synthetic" and task.seh_acceptance_status != "accepted-seh":
                task.seh_diagnostics.append("missing accepted-seh acceptance status")

    # Inject phase-checkpoint edges.
    # For each task in Phase N, add an implicit dep on the LAST task of
    # Phase N-1's foundation block (the last task before the first Checkpoint).
    # If Phase N-1 had no checkpoint, use its last task overall.
    sorted_phases = sorted(phase_tasks.keys())
    for idx, phase in enumerate(sorted_phases):
        if idx == 0:
            continue  # no predecessor
        prev_phase = sorted_phases[idx - 1]
        foundation = phase_foundation.get(prev_phase) or phase_tasks.get(prev_phase, [])
        if not foundation:
            continue
        anchor = foundation[-1]
        for tid in phase_tasks[phase]:
            if tid != anchor and anchor in tasks:
                tasks[tid].phase_deps.append(anchor)

    return tasks, errors


@dataclass
class TaskMetadata:
    deps: Optional[List[str]] = None
    skillist: Optional[List[str]] = None
    legacy_bare_list: bool = False


def parse_list_value(value: str) -> Optional[List[str]]:
    value = value.strip()
    if not (value.startswith("[") and value.endswith("]")):
        return None
    inner = value[1:-1].strip()
    if not inner:
        return []
    return [item.strip().strip('"').strip("'") for item in inner.split(",") if item.strip()]


def parse_deps_yml(path: Path) -> Tuple[Dict[str, TaskMetadata], List[str]]:
    """Parse the minimal 'id: [ids]' YAML without pyyaml.

    Supports:
        tasks:
          T001: []
          T015: [T011, T013]
          T022:
            - T019
            - T021
    """
    errors: List[str] = []
    metadata: Dict[str, TaskMetadata] = {}

    try:
        text = path.read_text(encoding="utf-8")
    except OSError as e:
        return {}, [f"Cannot read {path}: {e}"]

    in_tasks = False
    current_id: Optional[str] = None

    task_re = re.compile(r"^\s{2,}(T\d{3,4})\s*:\s*(?P<value>.*)$")
    field_re = re.compile(r"^\s{4,}(deps|skillist)\s*:\s*(?P<value>.*)$")
    list_item_re = re.compile(r"^\s{6,}-\s*(T\d{3,4})\s*$")
    current_field: Optional[str] = None

    for i, raw in enumerate(text.splitlines(), start=1):
        line = raw.rstrip()
        if not line or line.lstrip().startswith("#"):
            continue

        if re.match(r"^\s*tasks\s*:\s*$", line):
            in_tasks = True
            current_id = None
            continue

        # Top-level key that isn't 'tasks:' ends the tasks section.
        if in_tasks and re.match(r"^[A-Za-z_]", line):
            in_tasks = False
            current_id = None
            continue

        if not in_tasks:
            continue

        m = task_re.match(line)
        if m:
            tid = m.group(1)
            value = m.group("value").strip()
            if tid in metadata:
                errors.append(f"tasks.deps.yml:{i}: duplicate key {tid}")
            if value:
                parsed = parse_list_value(value)
                if parsed is None:
                    errors.append(
                        f"tasks.deps.yml:{i}: {tid}: expected object metadata with deps and skillist fields, got '{value}'"
                    )
                    metadata[tid] = TaskMetadata()
                else:
                    metadata[tid] = TaskMetadata(deps=parsed, legacy_bare_list=True)
                current_id = None
                current_field = None
            else:
                metadata[tid] = TaskMetadata()
                current_id = tid
                current_field = None
            continue

        fm = field_re.match(line)
        if fm and current_id is not None:
            field_name = fm.group(1)
            value = fm.group("value").strip()
            parsed = parse_list_value(value)
            if parsed is None:
                errors.append(
                    f"tasks.deps.yml:{i}: {current_id}: {field_name} must be a list"
                )
                current_field = field_name if not value else None
            else:
                if field_name == "deps":
                    metadata[current_id].deps = parsed
                else:
                    metadata[current_id].skillist = parsed
                current_field = None
            continue

        li = list_item_re.match(line)
        if li and current_id is not None and current_field == "deps":
            if metadata[current_id].deps is None:
                metadata[current_id].deps = []
            metadata[current_id].deps.append(li.group(1))
            continue

    return metadata, errors


def discover_skills(repo_root: Path) -> Tuple[Dict[str, List[Path]], List[str]]:
    roots = [repo_root / ".agents" / "skills"]
    if (repo_root / "src").exists():
        roots.extend((repo_root / "src").glob("*/skill"))
    if (repo_root / "template" / "fragments").exists():
        roots.extend((repo_root / "template" / "fragments").glob("*/skill"))
    skills: Dict[str, List[Path]] = defaultdict(list)
    warnings: List[str] = []
    for root in roots:
        if not root.exists():
            continue
        for skill_file in root.glob("*/SKILL.md") if root.name == "skills" else root.glob("SKILL.md"):
            try:
                text = skill_file.read_text(encoding="utf-8")
            except OSError as e:
                warnings.append(f"{skill_file}: skill file is not readable: {e}")
                continue
            name_m = re.search(r"^name:\s*['\"]?([^'\"\n]+)['\"]?\s*$", text, re.MULTILINE)
            skill_id = name_m.group(1).strip() if name_m else skill_file.parent.name
            skills[skill_id].append(skill_file)
    return skills, warnings


CAPABILITY_EXPECTATIONS = [
    ("speckit-evidence-graph", re.compile(r"(task graph|evidence graph|readiness validation|tasks\.deps\.yml|structured task metadata|mirror mismatch|skillist field|skillist, list typing|obvious capability|multi-skill dependency order|migration blocker|validator diagnostics|EvidenceGraph)", re.I)),
    ("speckit-evidence-audit", re.compile(r"(evidence audit|diff-scan|synthetic propagation|readiness-blocking|EvidenceAudit)", re.I)),
    ("speckit-tasks", re.compile(r"(/speckit\.tasks|speckit\.tasks|task-generation|task templates|tasks-template|tasks template|generated task guidance|post-generation skill evaluation)", re.I)),
    ("speckit-implement", re.compile(r"(/speckit\.implement|speckit\.implement|implementation-loading|implementation skill|implementation command|load each|skill-load|before implementation)", re.I)),
    ("speckit-constitution", re.compile(r"(constitution|constitutional)", re.I)),
]

SKILL_PREREQUISITES = {
    "speckit-evidence-audit": ["speckit-evidence-graph"],
    "speckit-implement": ["speckit-tasks"],
}


# ----------------------------------------------------------------------------
# Graph validation + propagation
# ----------------------------------------------------------------------------

def validate_and_merge(
    tasks: Dict[str, Task],
    metadata: Dict[str, TaskMetadata],
    repo_root: Path,
) -> List[str]:
    errors: List[str] = []

    md_ids = set(tasks.keys())
    yml_ids = set(metadata.keys())

    only_md = md_ids - yml_ids
    only_yml = yml_ids - md_ids

    for tid in sorted(only_md):
        errors.append(
            f"tasks.md declares {tid} but tasks.deps.yml has no key for it"
        )
    for tid in sorted(only_yml):
        errors.append(
            f"tasks.deps.yml declares {tid} but tasks.md has no task line"
        )

    # Check every dep reference points to a known task.
    for tid, meta in metadata.items():
        if tid not in tasks:
            continue  # already errored
        if meta.legacy_bare_list:
            errors.append(
                f"tasks.deps.yml: {tid}: existing bare-list metadata must be migrated to object form with deps and skillist"
            )
        if meta.deps is None:
            errors.append(f"{tid}: missing deps field in tasks.deps.yml")
            dlist = []
        else:
            dlist = meta.deps
        for d in dlist:
            if d not in tasks:
                errors.append(
                    f"tasks.deps.yml: {tid} depends on {d}, which does not exist"
                )
            elif d == tid:
                errors.append(f"tasks.deps.yml: {tid} depends on itself")

    # Merge explicit deps into tasks.
    skills, skill_warnings = discover_skills(repo_root)
    errors.extend(skill_warnings)

    for tid, meta in metadata.items():
        if tid not in tasks:
            continue
        task = tasks[tid]
        if meta.skillist is None:
            errors.append(f"{tid}: missing structured skillist in tasks.deps.yml")
        else:
            task.skillist = meta.skillist

        if task.skillist_mirror is None:
            errors.append(f"{tid}: missing tasks.md skillist mirror")
        elif meta.skillist is not None and task.skillist_mirror != meta.skillist:
            errors.append(
                f"{tid}: tasks.md mirror [{', '.join(task.skillist_mirror)}] does not match tasks.deps.yml [{', '.join(meta.skillist)}]"
            )

        for skill_id in meta.skillist or []:
            matches = skills.get(skill_id, [])
            if not matches:
                errors.append(f"{tid}: declared skill {skill_id} is not readable or not registered")
            elif len(matches) > 1:
                joined = ", ".join(str(p.relative_to(repo_root)) for p in matches)
                errors.append(f"{tid}: declared skill {skill_id} is ambiguous: {joined}")

        expected = []
        if not re.search(r"^Complete readiness notes", task.title, re.I):
            expected = [skill_id for skill_id, pattern in CAPABILITY_EXPECTATIONS if pattern.search(task.title)]
        for skill_id in expected:
            assessment = {
                "task_id": tid,
                "declared_skillist": meta.skillist or [],
                "candidate_skill_id": skill_id,
                "matched_signals": ["task-text"],
                "confidence": "high",
                "ambiguity": None,
                "reviewer_disposition": "accepted" if skill_id in (meta.skillist or []) else None,
                "diagnostic": f"{tid}: task text matches {skill_id}",
            }
            task.skill_match_assessments.append(assessment)
            if skill_id not in (meta.skillist or []):
                errors.append(f"{tid}: high-confidence skill match omitted declared skill {skill_id}; matched_signals=task-text")

        if not expected:
            task.skill_match_assessments.append(
                {
                    "task_id": tid,
                    "declared_skillist": meta.skillist or [],
                    "candidate_skill_id": None,
                    "matched_signals": [],
                    "confidence": "none",
                    "ambiguity": None,
                    "reviewer_disposition": "accepted-empty" if not (meta.skillist or []) else "declared",
                    "diagnostic": f"{tid}: no high-confidence capability signal detected",
                }
            )

        listed = meta.skillist or []
        positions = {skill_id: index for index, skill_id in enumerate(listed)}
        for skill_id, prerequisites in SKILL_PREREQUISITES.items():
            if skill_id not in positions:
                continue
            for prerequisite in prerequisites:
                if prerequisite in positions and positions[prerequisite] > positions[skill_id]:
                    errors.append(f"{tid}: skillist order places {skill_id} before prerequisite {prerequisite}")

    for tid, meta in metadata.items():
        if tid in tasks:
            tasks[tid].explicit_deps = [d for d in (meta.deps or []) if d in tasks]

    return errors


def parse_skill_loading_evidence(path: Path) -> Tuple[List[dict], List[str]]:
    if not path.exists():
        return [], []
    rows: List[dict] = []
    errors: List[str] = []
    try:
        text = path.read_text(encoding="utf-8")
    except OSError:
        return [], []
    for line in text.splitlines():
        trimmed = line.strip()
        if not trimmed.startswith("|") or "---" in trimmed or "Task | Skill id" in trimmed:
            continue
        cells = [cell.strip().strip("`") for cell in trimmed.strip("|").split("|")]
        if len(cells) < 8:
            continue
        task_id = cells[0]
        skill_id = cells[1]
        if re.search(r"\bT\d{3,4}\s*-\s*T\d{3,4}\b", task_id):
            errors.append(f"collapsed task range row is invalid: {task_id}")
        if "," in task_id or re.search(r"\bT\d{3,4}\b.*\bT\d{3,4}\b", task_id):
            errors.append(f"multi-task prose row is invalid: {task_id}")
        if "," in skill_id or re.search(r"\b(and|or)\b", skill_id, re.IGNORECASE):
            errors.append(f"multi-skill prose row is invalid: {skill_id}")
        rows.append(
            {
                "task_id": task_id,
                "declared_skill_id": skill_id,
                "resolved_skill_path": cells[2],
                "load_result": cells[3],
                "loaded_at": cells[4],
                "work_started_at": cells[5],
                "evidence_path": cells[6],
                "exception": cells[7],
            }
        )
    return rows, errors


def parse_utc(value: str) -> Optional[datetime]:
    try:
        return datetime.fromisoformat(value.replace("Z", "+00:00"))
    except ValueError:
        return None


def expected_skill_loading_rows(tasks: Dict[str, Task], completed_only: bool = True) -> List[Tuple[str, str]]:
    """Return one row per task and skill pairing required by skillist metadata."""
    expected: List[Tuple[str, str]] = []
    for task in tasks.values():
        if completed_only and task.declared not in ("done", "synthetic"):
            continue
        for skill_id in task.skillist:
            expected.append((task.id, skill_id))
    return expected


def missing_skill_loading_rows(expected: List[Tuple[str, str]], observed: Set[Tuple[str, str]]) -> List[Tuple[str, str]]:
    return [row for row in expected if row not in observed]


def generate_skill_loading_evidence_template(tasks: Dict[str, Task], skills: Dict[str, List[Path]]) -> str:
    lines = [
        "# Skill Loading Evidence",
        "",
        "| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |",
        "|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|",
    ]
    for task_id, skill_id in expected_skill_loading_rows(tasks, completed_only=False):
        paths = skills.get(skill_id, [])
        resolved = str(paths[0]) if len(paths) == 1 else "(unresolved)"
        lines.append(
            f"| {task_id} | {skill_id} | `{resolved}` | loaded | <loaded_at> | <work_started_at> | `readiness/skill-loading-evidence.md` | none |"
        )
    return "\n".join(lines) + "\n"


def validate_skill_loading_evidence(tasks: Dict[str, Task], repo_root: Path, feature_dir: Path) -> List[str]:
    errors: List[str] = []
    evidence_path = feature_dir / "readiness" / "skill-loading-evidence.md"
    rows, row_errors = parse_skill_loading_evidence(evidence_path)
    errors.extend(row_errors)
    row_by_key: Dict[Tuple[str, str], dict] = {}
    duplicate_keys: Set[Tuple[str, str]] = set()
    for row in rows:
        key = (row["task_id"], row["declared_skill_id"])
        if key in row_by_key:
            duplicate_keys.add(key)
        else:
            row_by_key[key] = row
    for task_id, skill_id in sorted(duplicate_keys):
        errors.append(f"{task_id}: duplicate skill-loading evidence row for {skill_id}; duplicate rows do not mask missing required rows")
    skills, skill_warnings = discover_skills(repo_root)
    errors.extend(skill_warnings)

    expected_rows = expected_skill_loading_rows(tasks)
    observed_rows = set(row_by_key.keys())

    for task_id, skill_id in missing_skill_loading_rows(expected_rows, observed_rows):
        errors.append(f"{task_id}: declared skill {skill_id} has no pre-work load evidence")

    for task_id, skill_id in expected_rows:
            task = tasks[task_id]
            row = row_by_key.get((task.id, skill_id))
            if row is None:
                continue
            if row["load_result"] != "loaded" and not row["exception"]:
                errors.append(f"{task.id}: declared skill {skill_id} has incomplete reviewer exception")
            loaded_at = parse_utc(row["loaded_at"])
            work_started_at = parse_utc(row["work_started_at"])
            if loaded_at is None or work_started_at is None:
                errors.append(f"{task.id}: declared skill {skill_id} has invalid load/work timestamp")
            elif loaded_at == work_started_at:
                errors.append(f"{task.id}: skill {skill_id} equal timestamps are invalid; loaded_at must be earlier than work_started_at")
            elif loaded_at > work_started_at:
                errors.append(f"{task.id}: skill {skill_id} loaded_at must be earlier than work_started_at (loaded_at={row['loaded_at']} work_started_at={row['work_started_at']})")
            resolved = Path(row["resolved_skill_path"])
            resolved_path = resolved if resolved.is_absolute() else repo_root / resolved
            matches = skills.get(skill_id, [])
            if not resolved_path.exists() or not resolved_path.is_file():
                errors.append(f"{task.id}: declared skill {skill_id} evidence path is unreadable: {row['resolved_skill_path']}")
            elif len(matches) != 1 or matches[0].resolve() != resolved_path.resolve():
                errors.append(f"{task.id}: declared skill {skill_id} evidence path does not match resolved skill path")

    return errors


def detect_cycles(tasks: Dict[str, Task]) -> List[List[str]]:
    """DFS-based cycle detection. Returns list of cycles (each a list of ids)."""
    WHITE, GRAY, BLACK = 0, 1, 2
    color: Dict[str, int] = {tid: WHITE for tid in tasks}
    stack: List[str] = []
    cycles: List[List[str]] = []

    def visit(tid: str) -> None:
        color[tid] = GRAY
        stack.append(tid)
        for d in tasks[tid].all_deps:
            if d not in color:
                continue
            if color[d] == GRAY:
                # Cycle: extract from stack position of d
                try:
                    idx = stack.index(d)
                    cycles.append(stack[idx:] + [d])
                except ValueError:
                    cycles.append([d, tid])
            elif color[d] == WHITE:
                visit(d)
        color[tid] = BLACK
        stack.pop()

    for tid in tasks:
        if color[tid] == WHITE:
            visit(tid)

    return cycles


def is_accepted_seh(t: Task) -> bool:
    return (
        t.declared == "synthetic"
        and t.seh_annotation
        and t.seh_approval_label
        and t.seh_acceptance_status == "accepted-seh"
        and not t.seh_diagnostics
    )


def topo_sort(tasks: Dict[str, Task]) -> List[str]:
    """Kahn's algorithm. Assumes no cycles (caller checks)."""
    indeg: Dict[str, int] = {tid: 0 for tid in tasks}
    for tid, task in tasks.items():
        for d in task.all_deps:
            if d in tasks:
                indeg[tid] += 1

    # Reverse adjacency: for each d, who depends on d?
    rev: Dict[str, List[str]] = defaultdict(list)
    for tid, task in tasks.items():
        for d in task.all_deps:
            if d in tasks:
                rev[d].append(tid)

    queue = deque(sorted(tid for tid, n in indeg.items() if n == 0))
    order: List[str] = []
    while queue:
        tid = queue.popleft()
        order.append(tid)
        for nxt in sorted(rev[tid]):
            indeg[nxt] -= 1
            if indeg[nxt] == 0:
                queue.append(nxt)
    return order


def propagate(
    tasks: Dict[str, Task],
    topo: List[str],
) -> Dict[str, List[str]]:
    """Compute effective status + root-cause map.

    effective(T) =
      'synthetic'       if declared(T) == 'synthetic'
      'auto-synthetic'  if declared(T) == 'done' and any dep is synthetic/auto
      declared(T)       otherwise

    root_cause[T] = sorted list of upstream ids (direct deps) that are
    synthetic or auto-synthetic, only populated when effective(T) is
    auto-synthetic.
    """
    root_cause: Dict[str, List[str]] = {}
    for tid in topo:
        t = tasks[tid]
        tainted_deps = [
            d for d in t.all_deps
            if d in tasks and tasks[d].effective in ("synthetic", "auto-synthetic")
            and not is_accepted_seh(tasks[d])
        ]
        if t.declared == "synthetic":
            t.effective = "synthetic"
        elif t.declared == "done" and tainted_deps:
            t.effective = "auto-synthetic"
            root_cause[tid] = sorted(tainted_deps)
        else:
            t.effective = t.declared
    return root_cause


# ----------------------------------------------------------------------------
# Rendering
# ----------------------------------------------------------------------------

STATUS_BOX = {
    "pending": "[ ]",
    "done": "[X]",
    "synthetic": "[S]",
    "failed": "[F]",
    "skipped": "[-]",
    "auto-synthetic": "[S*]",
}

STATUS_MERMAID_CLASS = {
    "pending": "pending",
    "done": "done",
    "synthetic": "synthetic",
    "failed": "failed",
    "skipped": "skipped",
    "auto-synthetic": "autoSynthetic",
}


def render_json(
    tasks: Dict[str, Task],
    root_cause: Dict[str, List[str]],
    errors: List[str],
    warnings: List[str],
    cycles: List[List[str]],
) -> str:
    payload = {
        "schema_version": "1.0",
        "verdict": "ok" if not errors and not cycles else "error",
        "errors": errors,
        "warnings": warnings,
        "cycles": cycles,
        "tasks": [
            {
                "id": t.id,
                "declared": t.declared,
                "effective": t.effective,
                "phase": t.phase,
                "story": t.story,
                "tier": t.tier,
                "parallel": t.parallel,
                "title": t.title,
                "skillist": t.skillist,
                "skillist_mirror": t.skillist_mirror,
                "skill_match_assessments": t.skill_match_assessments,
                "explicit_deps": t.explicit_deps,
                "phase_deps": t.phase_deps,
                "root_cause": root_cause.get(t.id, []),
                "seh": {
                    "annotation": t.seh_annotation,
                    "approval_label": t.seh_approval_label,
                    "design_source": t.design_source,
                    "synthetic_input_class": t.synthetic_input_class,
                    "expected_error_behavior": t.expected_error_behavior,
                    "rationale": t.seh_rationale,
                    "acceptance_status": t.seh_acceptance_status,
                    "diagnostics": t.seh_diagnostics,
                    "accepted": is_accepted_seh(t),
                },
            }
            for t in sorted(tasks.values(), key=lambda x: x.id)
        ],
    }
    return json.dumps(payload, indent=2) + "\n"


def render_mermaid(tasks: Dict[str, Task]) -> str:
    lines = ["```mermaid", "graph TD"]
    for t in sorted(tasks.values(), key=lambda x: x.id):
        label = t.title.replace('"', "'")[:50]
        node_id = t.id
        lines.append(f'  {node_id}["{node_id} {label}"]:::{STATUS_MERMAID_CLASS[t.effective]}')
    for t in sorted(tasks.values(), key=lambda x: x.id):
        for d in t.all_deps:
            if d in tasks:
                lines.append(f"  {d} --> {t.id}")
    lines.extend([
        "  classDef pending fill:#eeeeee,stroke:#999",
        "  classDef done fill:#c8e6c9,stroke:#2e7d32",
        "  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px",
        "  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3",
        "  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px",
        "  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3",
        "```",
    ])
    return "\n".join(lines)


def render_ascii(tasks: Dict[str, Task], root_cause: Dict[str, List[str]]) -> str:
    lines = []
    for t in sorted(tasks.values(), key=lambda x: x.id):
        box = STATUS_BOX[t.effective]
        marker = ""
        if is_accepted_seh(t):
            marker = "   ← accepted [SEH]"
        elif t.effective == "auto-synthetic":
            marker = "   ← auto-synthetic"
        elif t.effective == "synthetic":
            marker = "   ← root cause"
        lines.append(f"{t.id} {box} {t.title}{marker}")
        if t.id in root_cause:
            for rc in root_cause[t.id]:
                rc_box = STATUS_BOX[tasks[rc].effective]
                lines.append(f"    └── {rc} {rc_box} {tasks[rc].title}")
    return "\n".join(lines)


def render_markdown(
    feature_dir: Path,
    tasks: Dict[str, Task],
    root_cause: Dict[str, List[str]],
    errors: List[str],
    warnings: List[str],
    cycles: List[List[str]],
) -> str:
    out = [f"# Task Graph — {feature_dir.name}", ""]

    # Verdict block
    if errors or cycles:
        out.append("## ✗ Graph validation failed")
        if cycles:
            out.append("")
            out.append("### Cycles detected")
            for cy in cycles:
                out.append(f"- {' → '.join(cy)}")
        if errors:
            out.append("")
            out.append("### Errors")
            for e in errors:
                out.append(f"- {e}")
        out.append("")
    else:
        out.append("## ✓ Graph is acyclic and consistent")
        out.append("")

    if warnings:
        out.append("### Warnings")
        for w in warnings:
            out.append(f"- {w}")
        out.append("")

    out.append("## Skill Match Assessments")
    out.append("")
    out.append("| Task | Candidate | Confidence | Signals | Reviewer disposition | Diagnostic |")
    out.append("|------|-----------|------------|---------|----------------------|------------|")
    for t in sorted(tasks.values(), key=lambda x: x.id):
        for assessment in t.skill_match_assessments:
            candidate = assessment.get("candidate_skill_id") or "(none)"
            signals = ", ".join(assessment.get("matched_signals") or [])
            disposition = assessment.get("reviewer_disposition") or "(required)"
            diagnostic = assessment.get("diagnostic") or ""
            out.append(f"| {t.id} | {candidate} | {assessment.get('confidence')} | {signals} | {disposition} | {diagnostic} |")
    out.append("")

    # Counts by effective status
    counts: Dict[str, int] = defaultdict(int)
    accepted_seh = 0
    unaccepted_synthetic = 0
    for t in tasks.values():
        counts[t.effective] += 1
        if t.effective == "synthetic" and is_accepted_seh(t):
            accepted_seh += 1
        elif t.effective in ("synthetic", "auto-synthetic"):
            unaccepted_synthetic += 1
    out.append("## Status counts (effective)")
    out.append("")
    out.append("| Status | Count |")
    out.append("|--------|-------|")
    for key in ("pending", "done", "synthetic", "auto-synthetic", "failed", "skipped"):
        if counts[key] or key in ("synthetic", "auto-synthetic"):
            out.append(f"| {STATUS_BOX[key]} {key} | {counts[key]} |")
    out.append(f"| accepted [SEH] synthetic | {accepted_seh} |")
    out.append(f"| unaccepted synthetic | {unaccepted_synthetic} |")
    out.append("")

    if accepted_seh or unaccepted_synthetic:
        out.append("## Synthetic Error-Handling Classification")
        out.append("")
        out.append("| Task | Accepted | Label | Design source | Synthetic input class | Expected error behavior | Diagnostics |")
        out.append("|------|----------|-------|---------------|-----------------------|-------------------------|-------------|")
        for t in sorted(tasks.values(), key=lambda x: x.id):
            if t.effective not in ("synthetic", "auto-synthetic") and not (t.seh_annotation or t.seh_approval_label):
                continue
            diagnostics = "; ".join(t.seh_diagnostics)
            out.append(
                f"| {t.id} | {'yes' if is_accepted_seh(t) else 'no'} | {'yes' if t.seh_approval_label else 'no'} | {t.design_source or '(missing)'} | {t.synthetic_input_class or '(missing)'} | {t.expected_error_behavior or '(missing)'} | {diagnostics or '(none)'} |"
            )
        out.append("")

    # Mermaid
    out.append("## Graph")
    out.append("")
    out.append(render_mermaid(tasks))
    out.append("")

    # ASCII view
    out.append("## ASCII view")
    out.append("")
    out.append("```")
    out.append(render_ascii(tasks, root_cause))
    out.append("```")
    out.append("")

    # Propagation report
    if root_cause:
        out.append("## Propagation report")
        out.append("")
        out.append(
            "The following tasks are marked `[S*]` because at least one of "
            "their dependencies is synthetic-only. Clearing the upstream "
            "`[S]` tasks (real evidence) will automatically clear these."
        )
        out.append("")
        for tid in sorted(root_cause.keys()):
            rcs = root_cause[tid]
            rc_str = ", ".join(rcs)
            out.append(f"- **{tid}** ([S*]) ← {rc_str}")
        out.append("")

    return "\n".join(out) + "\n"


# ----------------------------------------------------------------------------
# Main
# ----------------------------------------------------------------------------

def main(argv: List[str]) -> int:
    if len(argv) != 2:
        print("usage: compute-task-graph.py <feature-dir>", file=sys.stderr)
        return 3

    feature_dir = Path(argv[1]).resolve()
    if not feature_dir.is_dir():
        print(f"error: {feature_dir} is not a directory", file=sys.stderr)
        return 3

    tasks_md = feature_dir / "tasks.md"
    deps_yml = feature_dir / "tasks.deps.yml"
    readiness_dir = feature_dir / "readiness"
    readiness_dir.mkdir(parents=True, exist_ok=True)

    errors: List[str] = []
    warnings: List[str] = []

    if not tasks_md.exists():
        errors.append(f"{tasks_md} does not exist")
    if not deps_yml.exists():
        errors.append(f"{deps_yml} does not exist")

    tasks: Dict[str, Task] = {}
    deps: Dict[str, TaskMetadata] = {}

    if tasks_md.exists():
        tasks, task_errs = parse_tasks_md(tasks_md)
        errors.extend(task_errs)
    if deps_yml.exists():
        deps, dep_errs = parse_deps_yml(deps_yml)
        errors.extend(dep_errs)

    if tasks and deps is not None:
        repo_root = feature_dir
        for parent in [feature_dir, *feature_dir.parents]:
            if (parent / ".git").exists() or (parent / ".specify").exists():
                repo_root = parent
                break
        errors.extend(validate_and_merge(tasks, deps, repo_root))
        errors.extend(validate_skill_loading_evidence(tasks, repo_root, feature_dir))
        skills, skill_warnings = discover_skills(repo_root)
        warnings.extend(skill_warnings)
        try:
            (readiness_dir / "skill-loading-evidence.template.md").write_text(
                generate_skill_loading_evidence_template(tasks, skills),
                encoding="utf-8",
            )
        except OSError as e:
            errors.append(f"failed to write skill-loading evidence template: {e}")

    cycles: List[List[str]] = []
    root_cause: Dict[str, List[str]] = {}
    if tasks and not errors:
        cycles = detect_cycles(tasks)
        if not cycles:
            order = topo_sort(tasks)
            if len(order) != len(tasks):
                errors.append(
                    f"topological sort incomplete: expected {len(tasks)} tasks, "
                    f"got {len(order)}. A cycle may have been missed."
                )
            else:
                root_cause = propagate(tasks, order)

    # Write artifacts even on failure — the audit wants to see the report.
    json_out = readiness_dir / "task-graph.json"
    md_out = readiness_dir / "task-graph.md"

    try:
        json_out.write_text(render_json(tasks, root_cause, errors, warnings, cycles))
        md_out.write_text(
            render_markdown(feature_dir, tasks, root_cause, errors, warnings, cycles)
        )
    except OSError as e:
        print(f"error: failed to write readiness artifacts: {e}", file=sys.stderr)
        return 3

    # Console summary
    print(f"task-graph: {len(tasks)} tasks parsed")
    print(f"  wrote {json_out}")
    print(f"  wrote {md_out}")

    if errors or cycles:
        print("", file=sys.stderr)
        print("VALIDATION FAILED", file=sys.stderr)
        for c in cycles:
            print(f"  cycle: {' -> '.join(c)}", file=sys.stderr)
        for e in errors:
            print(f"  {e}", file=sys.stderr)
        return 2

    # Summary line
    counts: Dict[str, int] = defaultdict(int)
    for t in tasks.values():
        counts[t.effective] += 1
    summary = ", ".join(
        f"{counts[k]}{STATUS_BOX[k]}"
        for k in ("done", "synthetic", "auto-synthetic", "pending", "failed", "skipped")
        if counts[k]
    )
    print(f"  status: {summary}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
