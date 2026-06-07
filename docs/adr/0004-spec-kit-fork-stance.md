---
title: ADR 0004 — Spec Kit fork stance (D4)
---

# ADR 0004 — Spec Kit fork stance (D4)

- **Status**: Accepted
- **Date**: 2026-05-31
- **Decision source**: foundations plan
  (`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`); this ADR
  records the resolved stance, grounded in the repo's existing `.specify/`
  overlay architecture.

## Context

The repository builds on upstream **Spec Kit**. It already customises Spec Kit
through an **overlay**, not a hard fork: vanilla Spec Kit assets live under
`.specify/templates/` and `.specify/scripts/`, while repo-specific behaviour is
layered as **extensions** (`.specify/extensions/evidence`,
`.specify/extensions/git`) and **presets**
(`.specify/presets/fsharp-opinionated/`), with the skill mirror under
`.claude/skills` ↔ `.agents/skills`. The question: should the programme **hard
fork** Spec Kit upstream, or continue the overlay/extension model?

## Decision

**Do not hard-fork upstream Spec Kit. Track upstream and customise through the
existing extension + preset overlay.** Concretely:

1. Keep vanilla Spec Kit assets vendored under `.specify/` so upstream updates
   remain **mergeable** with minimal conflict surface.
2. Express all repo-specific behaviour as **extensions** (`.specify/extensions/*`
   with their own `extension.yml`, scripts, and commands) and **presets**
   (`.specify/presets/*`), never by editing vendored upstream files in place
   where an overlay point exists.
3. The Codex (`.agents/skills`) and Claude (`.claude/skills`) skill sets are
   maintained as **byte-identical synchronized peers** (validated as such), not
   divergent forks.

## Alternatives considered

- **Hard fork upstream Spec Kit (rejected):** maximal flexibility but forfeits
  upstream improvements and creates an unbounded long-term merge/maintenance
  burden; contradicts the overlay model the repo already runs successfully.
- **Vanilla Spec Kit with no customisation (rejected):** cannot express the
  evidence-graph/audit governance, the F#-opinionated preset, or the local skill
  mirror the programme depends on.
- **Per-feature ad-hoc patches (rejected):** unversioned drift; the extension +
  preset structure exists precisely to avoid this.

## Consequences / rationale

- Upstream Spec Kit improvements remain adoptable via merge.
- Governance behaviour is isolated in named, versionable extensions/presets with
  their own manifests — discoverable and testable.
- The synchronized skill-mirror invariant keeps Codex and Claude behaviour
  identical and is machine-checkable.

## Stages shaped

- **Stage 1** (two-tier process) and **Stage 6** (single-source generation) build
  on the extension/preset overlay rather than a fork.
- Future Spec Kit upstream bumps are merge-and-reconcile, not re-fork.

## Verification in feature 039

Stance recorded only; no Spec Kit assets, extensions, or presets are modified by
this feature (plan Constitution Check: "no Spec Kit assets" changed).
