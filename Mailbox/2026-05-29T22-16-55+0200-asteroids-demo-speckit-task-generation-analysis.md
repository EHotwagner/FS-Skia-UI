# Asteroids Demo — Spec Kit Task-Generation Process Feedback

Date: 2026-05-29T22:16:55+0200
Source app: `/home/developer/projects/AsteroidsDemo2`
Feature: `001-asteroids-demo` (Asteroids arcade demo — real-time MVU game loop,
continuous keyboard input, vector-style Skia rendering, collision/wave state,
deterministic evidence mode)
Validation context: **`/speckit-tasks` only** — generated
`specs/001-asteroids-demo/tasks.md` (33 tasks) + `tasks.deps.yml`, then validated
with `.specify/extensions/evidence/scripts/bash/run-audit.sh
specs/001-asteroids-demo --graph-only` (final `EXIT=0`, `status: 33[ ]`).
No `dotnet build`/`test`, no `/speckit-implement`, no app launch was performed in
this session.

## Scope and honesty caveat

This report is **narrow on purpose**. I only exercised the task-generation phase,
so every finding below is attributable to the **Spec Kit evidence tooling and the
skill registry**, not to FS.Skia.UI runtime/rendering. I have no new
runtime/rendering evidence to add. The spec/plan/contract *content* for this
feature was high quality — no `NEEDS CLARIFICATION`, normative field names in
`data-model.md`, all Phase-0 unknowns resolved in `research.md`, and contracts
that pinned the host-hook and evidence shapes precisely. The friction was
entirely in the validator and skill-resolution ergonomics.

This corroborates and deepens item #5 of
`2026-05-29T21-05-37+0200-sokoban-demo-fs-skia-ui-feedback.md` ("`speckit-tasks`
validator gotchas"). The same trigger-phrase trap has now bitten two consecutive
generated demos, which argues it is systemic rather than author-specific.

All file/line citations are against
`.specify/extensions/evidence/scripts/python/compute-task-graph.py` as vendored
into the generated project (mirrored from the Spec Kit F# tooling preset).

## Findings

### 1. `CAPABILITY_EXPECTATIONS` does substring (not word-boundary) matching on task titles — false positive on a mandated filename (BLOCKING; cost one fail/fix cycle)

This is the one that actually failed my first `--graph-only` run:

```text
VALIDATION FAILED
  T001: high-confidence skill match omitted declared skill speckit-implement; matched_signals=task-text
✗ Graph compute failed.
```

The matcher (`compute-task-graph.py:433`, enforced at `:523`–`:538`) regex-scans
each task **title** and, on a match, *requires* the corresponding skill in that
task's `skillist`. The `speckit-implement` pattern (`:437`) is:

```python
("speckit-implement", re.compile(
  r"(/speckit\.implement|speckit\.implement|implementation-loading|"
  r"implementation skill|implementation command|load each|skill-load|"
  r"before implementation)", re.I)),
```

My T001 is a Phase-1 Setup task that creates the audit-mandated readiness
placeholders — including `skill-loading-evidence-workflow.md`, whose name comes
**straight from the tasks-template's own required-readiness list**
(`tasks-template.md:133`). The token `skill-load` matches as a **substring** of
`skill-loading`, so a Setup task was told it must declare the *implementation*
skill.

Root cause: the alternatives are unanchored. `skill-load`, `load each`, and
`before implementation` will match inside longer legitimate words/filenames.
Because the matched string is frequently a **filename the audit itself mandates**,
this is a built-in collision, not an author error.

Suggested fixes (any one resolves the failure class):
- Anchor the tokens: `\bskill-load\b`, `\bload each\b`, etc.
- Strip backtick-quoted code spans / filenames from the title before searching —
  task titles routinely cite mandated filenames and module names.
- Match on a normalized "intent" field rather than the free-text title.

### 2. The escape hatch is undocumented (cost: source-diving)

The fix I ultimately used was to prefix the title with `Complete readiness
notes`, because of `compute-task-graph.py:523`:

```python
if not re.search(r"^Complete readiness notes", task.title, re.I):
    expected = [skill_id for skill_id, pattern in CAPABILITY_EXPECTATIONS ...]
```

A title starting with `Complete readiness notes` suppresses **all** capability
expectations for that task. This is the intended escape valve for exactly the
"setup task that names readiness files" case — but it appears **only in the
Python**, nowhere in `tasks-template.md` or the `speckit-tasks` SKILL.md. The
only way to discover it is to read the validator. An author who doesn't read the
source will instead "fix" the failure by mis-declaring `speckit-implement` on a
setup task, which is semantically wrong and pollutes the skill metadata.

Suggested fix: document the `Complete readiness notes` prefix (and the full
trigger list — see #4) in the task template and skill, next to the existing
"title trigger phrase" warning.

### 3. Two divergent skill registries; the skill *id* is the frontmatter `name:`, not the directory (latent BLOCKING)

The Claude-visible skill list is sourced from `.claude/skills/`. The audit's
`discover_skills()` (`compute-task-graph.py:410`–`428`) instead reads:

```python
roots = [repo_root / ".agents" / "skills"]   # + src/*/skill, template/fragments/*/skill
...
skill_id = name_m.group(1).strip() if name_m else skill_file.parent.name
```

Two consequences a generated-app author can easily trip on:
- **Different root.** If `.claude/skills/` and `.agents/skills/` ever drift, the
  ids I can *see* won't be the ids the validator *accepts*. (They are mirror
  copies today, so it worked — but nothing enforces that.)
- **Directory name ≠ id.** The id is the `name:` field. In this repo
  `.agents/skills/fs-skia-ui-widgets/SKILL.md` declares `name:
  asteroidsdemo2-widgets`. So a `skillist: ["fs-skia-ui-widgets"]` would fail
  with `declared skill ... is not readable or not registered`
  (`compute-task-graph.py:517`), even though that directory plainly exists.

I only avoided this by grepping every `name:` field up front rather than trusting
the directory listing.

Suggested fixes:
- Have the audit also discover `.claude/skills/`, or document `.agents/skills/` +
  `name:` as the single authority and stop shipping a second visible copy.
- Emit a friendlier error that says "directory `X` resolves to id `Y` via its
  `name:` field; declare `Y`" when a directory exists but the id doesn't.

### 4. The documented "trigger phrase" examples don't match the enforced set

`tasks-template.md` (and the `speckit-tasks` skill) warn against example phrases
like *"persistent GUI runtime"* and *"window visibility validation fixture"*.
**Neither appears in `CAPABILITY_EXPECTATIONS`.** The phrases that actually
hard-fail a title are (`compute-task-graph.py:434`–`438`):

- `speckit-evidence-graph`: `task graph`, `evidence graph`, `readiness
  validation`, `tasks.deps.yml`, `structured task metadata`, `mirror mismatch`,
  `skillist field`, `EvidenceGraph`, …
- `speckit-evidence-audit`: `evidence audit`, `diff-scan`, `synthetic
  propagation`, `readiness-blocking`, `EvidenceAudit`
- `speckit-tasks`: `/speckit.tasks`, `task templates`, `tasks-template`,
  `post-generation skill evaluation`, …
- `speckit-implement`: `skill-load`, `load each`, `before implementation`, …
- `speckit-constitution`: `constitution`, `constitutional`

So the author-facing guidance trains avoidance of the *wrong* words while the
real landmines (e.g. the bare word **`constitution`** in any title, or
**`before implementation`**) go unmentioned. This directly shaped how I had to
phrase the graph/audit tasks (T032/T033) and forced me to keep "constitution" out
of titles entirely.

Suggested fix: publish the actual enforced token list in the template, generated
from the same constant the validator uses (single source of truth), so docs can't
drift from enforcement.

### 5. Asymmetric enforcement: only `speckit-*` skills are title-validated; `fs-skia-*` capability choices are trust-based

The validator checks that every declared skill resolves uniquely
(`:512`–`:519`), that the `tasks.md` mirror equals the structured `skillist`, and
that `SKILL_PREREQUISITES` ordering holds (`:441`, e.g. graph-before-audit). But
`CAPABILITY_EXPECTATIONS` only covers the `speckit-*` skills. **Which
`fs-skia-*` skill is correct for a task is never checked** — only that the mirror
matches the list. So my choice of `fs-skia-scene` vs `fs-skia-skiaviewer` vs
`fs-skia-layout-evidence` for each task (T007/T009/T027, etc.) is entirely
judgment with no backstop. That's defensible, but the "compulsory skill
evaluation" step the skill describes is, in practice, only mechanically enforced
for the Spec Kit skills, not the FS.Skia.UI capability skills it most wants right.

Suggested improvement (optional): add lightweight capability expectations for the
`fs-skia-*` skills (e.g. titles touching `View`/scene rendering → expect
`fs-skia-scene`; `MapKey`/`Tick`/viewer host → `fs-skia-skiaviewer`), even as
advisory (non-blocking) diagnostics, to give authors a signal.

### 6. Cosmetic: `--graph-only` still prints the audit banner

`run-audit.sh ... --graph-only` prints `=== speckit.evidence.audit ===` and
`[1/3] Computing task graph...` then `graph-only mode; skipping diff scan.` The
banner says "audit" for a graph-only invocation, which is mildly confusing when
scanning logs for which gate actually ran. Minor.

## What worked well

- **Auto-injected phase-checkpoint edges** are a genuinely good ergonomic. I only
  authored intra-phase test→impl edges and a couple of cross-edges in
  `tasks.deps.yml`; phase sequencing (each phase depending on the prior phase's
  last pre-`**Checkpoint` task) fell out for free. The script header documents
  this clearly.
- **Lockstep `tasks.md` ↔ `tasks.deps.yml` + visible-mirror equality** caught
  exactly the drift it is meant to, and the error messages for
  missing-key/dangling-dep are precise.
- **`SKILL_PREREQUISITES` ordering** (graph-before-audit) and the
  object-shaped-metadata requirement are sound and produced no false positives.
- Once the trigger-phrase trap (#1/#2) was understood, the second validation run
  passed clean with no further iteration: 33 tasks, no cycles, no dangling refs,
  all skill ids resolved.
- The generated-task discipline (vertical-slice rule, the "distinct persistent
  graphical launch task reachable from the default executable" requirement)
  mapped cleanly onto the Asteroids user stories and forced T016 to be a real
  `Viewer.runApp` launch rather than a bounded-smoke substitute — good guardrail.

## Prioritized recommendations for the Spec Kit / FS.Skia.UI tooling

1. **Anchor `CAPABILITY_EXPECTATIONS` tokens to word boundaries and/or strip
   code spans from titles** (`compute-task-graph.py:434`). Fixes the #1 blocking
   false positive on the audit's own mandated filenames. (Highest value, smallest
   change.)
2. **Document the `Complete readiness notes` escape hatch and the *actual*
   enforced trigger-token list** in `tasks-template.md` + the `speckit-tasks`
   skill, generated from the validator constant so they can't drift. (Issues #2,
   #4.)
3. **Reconcile the skill registries** — make the audit read `.claude/skills/`, or
   declare `.agents/skills/` + `name:` the single authority; and improve the
   "not registered" error to name the resolved id when the directory exists.
   (Issue #3.)
4. **(Optional) advisory capability expectations for `fs-skia-*` skills** so the
   most important skill choices get a signal, not just the Spec Kit ones.
   (Issue #5.)
5. **(Cosmetic) relabel the `--graph-only` banner.** (Issue #6.)

## Cross-reference

- Recurrence of the title-regex trap first noted in
  `2026-05-29T21-05-37+0200-sokoban-demo-fs-skia-ui-feedback.md` §5. Two demos in
  a row hit it; recommend prioritizing recommendation #1.
