#!/usr/bin/env python3
"""
audit-status-scan.py — authoritative audit-status region scanner (spec 037, US2).

Machine-readable status values are read ONLY from a fenced code block whose info
string is exactly ``audit-status`` (FR-005). Prose, markdown bullets, and any
other fenced block are never read as status, so a blocker term in explanatory
text or a negation cannot raise a false block (FR-004).

Deterministic resolution rule (audit-status-region-contract.md):
  1. First region wins — the first ``audit-status`` region declaring a key is
     authoritative.
  2. Duplicate key within the region is a parse error (never silent last-wins).
  3. Prose never wins — keys outside the region are not read.
  4. A malformed entry (missing '=', empty key) is a parse error, never silently
     treated as passing or failing.

Blocking is structured, not substring (FR-006):
  - process/taskbar-only:  taskbar-only=true, or taskbar-entry=true with
    window-visible=false.
  - unresolved package mismatch:  exact-package-match not in {true,yes}, or
    package-resolution=nu1603.

Usage:
  audit-status-scan.py <file> [<file> ...]

Exit codes:
  0 — every scanned file is clean (no blocking value, no parse error).
  2 — at least one file has a blocking value or a parse error.
  3 — usage error.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path
from typing import Dict, List, Tuple

FENCE_OPEN_RE = re.compile(r"^\s*```audit-status\s*$")
FENCE_ANY_RE = re.compile(r"^\s*```")

TRUE_SET = {"true", "yes"}
FALSE_SET = {"false", "no"}


def extract_first_region(text: str) -> Tuple[List[str], bool, bool]:
    """Return (region_lines, found, closed) for the first audit-status region."""
    lines = text.replace("\r\n", "\n").split("\n")
    in_region = False
    region: List[str] = []
    for line in lines:
        if not in_region:
            if FENCE_OPEN_RE.match(line):
                in_region = True
            continue
        # Inside the region: a closing fence ends it.
        if FENCE_ANY_RE.match(line):
            return region, True, True
        region.append(line)
    if in_region:
        return region, True, False  # opened but never closed
    return [], False, False


def parse_region(region_lines: List[str]) -> Tuple[Dict[str, str], List[str]]:
    """Parse key=value lines inside the region. Return (values, parse_errors)."""
    values: Dict[str, str] = {}
    errors: List[str] = []
    for raw in region_lines:
        line = raw.strip()
        if not line or line.startswith("#"):
            continue
        if "=" not in line:
            errors.append(f"malformed entry (no '='): {line!r}")
            continue
        key, value = line.split("=", 1)
        key = key.strip().lower()
        value = value.strip()
        if not key:
            errors.append(f"malformed entry (empty key): {line!r}")
            continue
        if key in values:
            errors.append(f"duplicate key in audit-status region: {key}")
            continue
        values[key] = value
    return values, errors


def blocking_reasons(values: Dict[str, str]) -> List[str]:
    reasons: List[str] = []
    if values.get("taskbar-only", "").lower() in TRUE_SET:
        reasons.append("process/taskbar-only declared (taskbar-only=true)")
    if (
        values.get("taskbar-entry", "").lower() in TRUE_SET
        and values.get("window-visible", "").lower() in FALSE_SET
    ):
        reasons.append("process/taskbar-only (taskbar-entry=true with window-visible=false)")
    exact = values.get("exact-package-match")
    if exact is not None and exact.lower() not in TRUE_SET:
        reasons.append(f"unresolved package mismatch (exact-package-match={exact})")
    if values.get("package-resolution", "").lower() == "nu1603":
        reasons.append("unresolved package mismatch (package-resolution=nu1603)")
    return reasons


def scan_file(path: str) -> dict:
    p = Path(path)
    try:
        text = p.read_text(encoding="utf-8", errors="replace")
    except OSError as e:
        return {"path": path, "status": "unreadable", "blocking": True, "reasons": [str(e)]}
    region, found, closed = extract_first_region(text)
    if not found:
        return {"path": path, "status": "no-region", "blocking": False, "reasons": []}
    values, errors = parse_region(region)
    if not closed:
        errors.append("malformed audit-status region (unterminated fence)")
    reasons = list(errors) + blocking_reasons(values)
    return {
        "path": path,
        "status": "block" if reasons else "pass",
        "blocking": bool(reasons),
        "reasons": reasons,
        "values": values,
    }


def main(argv: List[str]) -> int:
    if len(argv) < 2:
        print("usage: audit-status-scan.py <file> [<file> ...]", file=sys.stderr)
        return 3
    any_block = False
    for path in argv[1:]:
        result = scan_file(path)
        if result["status"] == "no-region":
            print(f"PASS {path}: no audit-status region (prose is never read as status)")
            continue
        if result["blocking"]:
            any_block = True
            print(f"BLOCK {path}:")
            for reason in result["reasons"]:
                print(f"  - {reason}")
        else:
            print(f"PASS {path}: audit-status region resolves clean")
    return 2 if any_block else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
