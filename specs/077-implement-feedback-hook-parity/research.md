# Phase 0 Research: Implement-Phase Feedback Hook Parity

All NEEDS CLARIFICATION resolved. Decisions below are grounded in the actual
repository state discovered during planning (greps + file reads), not assumption.

## D1 — How phase skills honor registered hooks (mechanism)

- **Decision**: Hooks are **agent-honored** declarations. The Claude Code harness
  only auto-runs hooks defined in `settings.json`; it never reads the Spec Kit
  extension YAML. A registered `before_*`/`after_*` hook fires only if that
  phase's SKILL.md instructs the agent to discover and run it. Fix mechanism =
  **skill-text parity** (spec Assumption option (a)).
- **Rationale**: Matches the five already-working skills; the feedback capture is
  itself a Spec Kit command skill, not a shell command a `settings.json` hook can
  cleanly invoke. Confirmed by reading `.specify/extensions.yml` (registry) and
  the working sibling skills.
- **Alternatives considered**: (b) a real `settings.json` harness hook — rejected
  (cannot invoke a command-skill; diverges from the established working pattern).
  Recorded as rejected in the spec.

## D2 — True scope of the defect (which skills are deficient)

- **Decision**: Bring **four** phase skills to the modern block — `implement`,
  `tasks`, `taskstoissues` (legacy→modern), `constitution` (none→modern). The
  in-scope roster the guard enforces is all **nine** lifecycle phases:
  `specify, clarify, plan, tasks, analyze, implement, checklist, taskstoissues,
  constitution`.
- **Rationale**: Measured state (per-skill marker counts on
  `.agents/skills/speckit-*/SKILL.md`):

  | Phase skill | `extensions.yml` | `multi-file discovery` | `Effective hooks for` | Verdict |
  | --- | --- | --- | --- | --- |
  | specify, plan, clarify, analyze, checklist | ≥3 | 2 | 1 | modern ✅ |
  | taskstoissues | 4 | 0 | 0 | **legacy single-file** ❌ |
  | implement | 0 | 0 | 0 | **none** ❌ |
  | tasks | 0 | 0 | 0 | **none** ❌ |
  | constitution | 0 | 0 | 0 | **none** ❌ |

  The spec's "2 missing" undercounts: `taskstoissues` honors only the central
  `extensions.yml` (fails the FR-001 multi-file and FR-003 consolidated-notice
  standard), and `constitution` is blockless despite the **mandatory**
  `before_constitution` `git.initialize` hook. The registry
  (`.specify/extensions.yml`) defines `before_/after_` keys for exactly these nine
  phases.
- **Clarification (user-confirmed during planning)**: "All 4 deficient" scope +
  "Strict: modern markers" guard. SC-002 restated: previously **4** deficient, now
  **0**.
- **Alternatives considered**: (i) literal "implement + tasks only" — rejected:
  leaves `taskstoissues` legacy and `constitution` blockless, so FR-006 could not
  honestly claim "cannot silently reappear." (ii) "implement + tasks +
  constitution, tolerate taskstoissues legacy" — rejected: a two-tier guard is
  more complex and still allows multi-file misses.

## D3 — The canonical modern block (what each skill must contain)

- **Decision**: Each in-scope phase skill carries a **pre-hook** discovery block
  (`before_<phase>`) and a **post-hook** discovery block (`after_<phase>`), each
  doing multi-file discovery (central `extensions.yml` + every
  `.specify/extensions/*/*.yml`, sorted, parse-tolerant), merge + dedupe by
  `(extension, command)` (first occurrence wins), the optional/mandatory/condition/
  `enabled:false` precedence under `auto_execute_hooks`, and a single consolidated
  `## Effective hooks for <phase>` notice listing each hook's resolved disposition
  (auto-run / surfaced / skipped / condition-deferred). See
  [contracts/modern-hook-block.md](./contracts/modern-hook-block.md).
- **Rationale**: This is the exact shape the five compliant skills already use
  (`speckit-plan` is the template). Reusing it verbatim (adjusting only the phase
  name and the prose anchor — "Outline" for plan, the phase's own first section
  for others) guarantees behavioral parity.
- **Alternatives considered**: inventing a shorter block — rejected; would diverge
  from the proven pattern and complicate the strict guard.

## D4 — Where the anti-drift guard lives (FR-006)

- **Decision**: A new pure module `build/Governance/PhaseHookParity.fs` (+ curated
  `.fsi`) holds the roster and marker check; a new FAKE target
  `PhaseHookParityCheck` exposes it, registered on the existing `skill-quality`
  routing rule's `RequiredGates` so any `.agents/skills/**` change runs it; a
  failing-first `Governance.Tests/PhaseHookParityTests.fs` provides red→green
  evidence.
- **Rationale**: The existing `SkillQuality` rubric **excludes** `speckit-*`
  skills (`SkillQuality.isInScope`: `not (p.Contains "/speckit-")`), so phase
  skills are not covered by any current content check — a dedicated rule is
  required. The repo's convention for a content rule is module + `.fsi` + target +
  `knownGates` entry + routing entry + Expecto test (mirroring `SkillQuality`),
  with `validation.contract.yml` regenerated from `Routing.fs` and currency
  enforced by `TargetMetadataDrift`. Governance modules already use `.fsi` (42
  exist).
- **Strict marker set** the guard asserts per in-scope phase skill (and its
  `.claude` mirror): (1) multi-file enumeration of `.specify/extensions/*/*.yml`
  present at least twice (proves pre **and** post blocks); (2) dedupe-by
  `(extension, command)` language present; (3) a `## Effective hooks for <phase>`
  consolidated notice present. A legacy single-file block fails; total absence
  fails. Markers chosen as **stable literal substrings** to mirror the existing
  `SkillQuality` literal-detector style (low brittleness, no semantic parsing).
- **Alternatives considered**: (i) `Governance.Tests`-only check (no target) —
  rejected: not guaranteed to run via `Route` on a skill-only diff. (ii) Fold the
  assertion into `SkillSyncCheck` or `SkillQualityCheck` — rejected: conflates
  byte-sync / capability-rubric concerns with phase-hook parity and breaks the
  `speckit-*` exclusion boundary.

## D5 — Propagation to generated consumer projects (FR-008)

- **Decision**: No `template.json` change. The corrected `.agents/skills/<phase>/
  SKILL.md` files reach generated projects through the existing copy-only globs
  (`.agents/skills/` → `.agents/skills/` and `.agents/skills/` → `.claude/skills/`
  in `.template.config/template.json`). After editing canonical `.agents`,
  regenerate `.claude` with `RefreshSurfaceBaselines`.
- **Rationale**: `GeneratedProduct.fs` already asserts generated projects contain
  `speckit-{specify,plan,tasks,implement}` skills in both `.agents` and `.claude`;
  `TemplateCheck`→`TemplateSmoke` and `GeneratedProductCheck` verify presence and
  content of the shipped skills.
- **Caveat**: `GeneratedProductCheck` is known to fail locally for an unrelated
  environment reason (see [[generated-product-check-env-failure]] — no template
  `feature.json`, `Map.empty` env); treat that specific failure as
  non-authoritative and rely on `TemplateCheck`/CI for the propagation proof.

## D6 — Currency / regeneration obligations

- **Decision**: After editing canonical `.agents` skill text, run
  `RefreshSurfaceBaselines` to regenerate `.claude/skills/**` (byte-identical via
  `SkillTreeGen`/`regenerateSkillTree`). After adding the gate to `Routing.fs`,
  regenerate `validation.contract.yml` (same `RefreshSurfaceBaselines`).
- **Rationale**: `SkillSyncCheck` compares `.agents`↔`.claude` byte-for-byte;
  `TargetMetadataDrift` compares `validation.contract.yml` against `Routing.fs`.
  Both are generated-from-single-source, never hand-synced.
- **Gotcha**: `RefreshSurfaceBaselines` skips per-package `.fsi.txt` snapshots
  (not relevant here — no `.fsi` product change), and trailing-newline drift has
  bitten skill regeneration before ([[refresh-surface-baselines-skillist-reference]]);
  verify via `SkillSyncCheck` / `Governance.Tests` after regeneration.
