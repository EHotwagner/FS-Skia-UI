# `/speckit.tasks` Process & Governance Analysis — Problems Encountered

**Date:** 2026-06-03T21:28:05+0200
**Author:** Claude Code (Opus 4.8) session
**Context:** Running `/speckit.tasks` for feature `001-asteroids-demo` in a
generated consumer project (`AsteroidsDemo3`), which consumes the published
`FS.Skia.UI.*` packages and the bundled Spec Kit skills/extensions.
**Scope:** Friction encountered authoring `tasks.md` + `tasks.deps.yml` and
validating the task DAG via the evidence extension. Findings concern the
**process, governance docs, validator, and skill-assignment machinery** — not
the game feature itself.

---

## Summary

Authoring the two task artifacts was straightforward; **validating** them was
not. Of the problems below, three can produce a *wrong result silently* or
*cost a guaranteed wasted iteration*, and the rest are quality-of-life / design
smells. The single most dangerous issue is that the default graph-validation
invocation returns a **clean pass against the wrong feature** (a bundled
sample), so the documented happy path is a false green.

Severity legend: 🔴 can cause wrong/false-positive result · 🟠 guaranteed
wasted iteration · 🟡 friction / design smell.

| # | Problem | Severity |
|---|---------|----------|
| 1 | Documented validator (`run-audit.sh --graph-only`) does not exist | 🟠 |
| 2 | `EvidenceGraph` silently validates a *sample* feature → false green | 🔴 |
| 3 | `tasks.deps.yml` required schema (`tasks:` wrapper) is undocumented | 🟠 |
| 4 | Skill-registry ids diverge from the guidance that references them | 🔴 |
| 5 | "Compulsory skill evaluation" is substring-on-title, not real review | 🟡 |
| 6 | Governance couples human prose to a substring matcher (trigger phrases) | 🟡 |
| 7 | `fs-skia-layout-evidence` is an overloaded catch-all | 🟡 |
| 8 | Two skills give contradictory instructions about the validator | 🟠 |

---

## 1. 🟠 The documented validation command does not exist

`speckit-tasks/SKILL.md` ("## Validation") instructs:

```bash
.specify/extensions/evidence/scripts/bash/run-audit.sh specs/<FEATURE_ID> --graph-only
```

Neither the script nor the `scripts/bash/` directory exists. The evidence
extension ships only:

```
.specify/extensions/evidence/extension.yml
.specify/extensions/evidence/audit-patterns.yml
.specify/extensions/evidence/commands/speckit.evidence.graph.md
.specify/extensions/evidence/commands/speckit.evidence.audit.md
```

The real validator is the FAKE target `EvidenceGraph`, which runs an in-process
F# engine (`FS.Skia.UI.Build.Evidence.Engine.runGraph`, dispatched from
`build.fsx`). Following the skill literally yields "file not found" and forces
the operator to reverse-engineer the actual entry point.

**Fix:** Replace the `run-audit.sh` reference in `speckit-tasks/SKILL.md` with
`./fake.sh build -t EvidenceGraph` (and note the env var from #2). Remove the
"If the evidence extension is not installed" fallback's implication that a shell
runner is the canonical path.

---

## 2. 🔴 `EvidenceGraph` defaults to a sample feature → false green

`build.fsx` resolves the feature directory from environment variables and
otherwise **defaults to a bundled sample**:

```fsharp
// build.fsx ~L66-75
[ "SPECKIT_FEATURE_DIR"; "GENERATED_EVIDENCE_FEATURE_DIR" ]
...
| Some featureDir -> featureDir
| None ->
    let featureDir = path [ specsDir; "generated-evidence-workflow" ]
    ...
```

The first run of `./fake.sh build -t EvidenceGraph` (no env var) reported:

```
feature-directory=.../specs/generated-evidence-workflow
status=ok
exit-code=0
- verdict=ok
- tasks=1
```

That is a **clean pass that validated none of the 33 authored tasks**. The only
signal that anything was wrong was the echoed `feature-directory=` and
`tasks=1`. The correct invocation is:

```bash
SPECKIT_FEATURE_DIR="specs/001-asteroids-demo" ./fake.sh build -t EvidenceGraph
```

No skill, command doc, or template mentions this env var. An operator who runs
the documented target and sees `exit 0 / verdict=ok` will reasonably believe
their feature passed.

**Fix (priority):** When run inside a feature branch, infer the feature dir from
the branch name (`001-asteroids-demo`) or from the most recently modified
`specs/*/tasks.md`; fail loudly (non-zero, explicit message) if multiple
candidates exist rather than falling back to a sample. At minimum, the skills
must document `SPECKIT_FEATURE_DIR` as required.

---

## 3. 🟠 The `tasks.deps.yml` schema is documented incorrectly

The template and `speckit-tasks/SKILL.md` devote ~40 lines to deps-file shape:

> `tasks.deps.yml` MUST use one key per task id; the required object shape is
> one entry per task id with indented `deps` and `skillist` fields; inline maps
> such as `T001: { deps: [], skillist: [] }` … are invalid.

Following that exactly (top-level `Tnnn:` keys, indented fields, no inline maps)
**fails**:

```
tasks.deps.yml: missing or malformed 'tasks' mapping
tasks.md declares T001 but tasks.deps.yml has no key for it
... (×33)
```

The engine requires the entries to be **nested under a top-level `tasks:`
mapping**, with a `schema_version`, as seen only in the bundled sample
(`specs/generated-evidence-workflow/tasks.deps.yml`):

```yaml
schema_version: "1.0"
tasks:
  T001:
    deps: []
    skillist: []
```

The one structural fact that actually gates validation (the `tasks:` wrapper) is
absent from every piece of guidance; all the documented detail is about
indentation style that the parser is comparatively lenient on. This is a
guaranteed wasted iteration for anyone authoring by hand.

**Fix:** Document the `schema_version` + `tasks:` wrapper in
`tasks.deps-template.yml` and the SKILL.md, ideally with a complete minimal
example. Consider having the parser emit a more directive error
("expected top-level `tasks:` mapping; found bare task keys") since it clearly
detects this exact case.

---

## 4. 🔴 Skill-registry ids diverge from the guidance that references them

The authoritative id for `skillist` is each skill file's `name:` field, but the
template's advisory hints refer to **directory names** that don't resolve, and
to a skill that doesn't exist:

- **Directory ≠ name:** `.agents/skills/fs-skia-ui-widgets/SKILL.md` has
  `name: asteroidsdemo3-widgets`. The hint table says "controls/forms/charts →
  `fs-skia-ui-widgets`", but only `asteroidsdemo3-widgets` would resolve. This
  is precisely the trap the SKILL.md warns about — embedded in the framework's
  own hints.
- **Nonexistent skill:** the hints route layout tasks to `fs-skia-layout`,
  which is **not registered**. The only layout skill is
  `fs-skia-layout-evidence`. Following the hint literally produces an
  unresolved-skill validation failure.

Because the validator hard-fails on unresolved skill ids ("Declared skill ids
resolve to exactly one readable skill file"), the advisory hints can actively
steer an author into a blocking error.

**Fix:** Regenerate the hint tables from the live registry (`name:` values), or
rename the widgets skill so directory and `name:` agree; add `fs-skia-layout`
as an alias of `fs-skia-layout-evidence` or correct the hint.

---

## 5. 🟡 "Compulsory skill evaluation" is substring-on-title, not real review

The SKILL.md frames skill assignment as a "confidence review … matched signals,
confidence, ambiguity, reviewer disposition." In practice the engine's
assessment came back `declared` / `accepted-empty` for **31 of 33 tasks** —
i.e. it accepted whatever was declared without judgement. It produced a genuine
high-confidence verdict for only two tasks (T032, T033), and only because their
**titles literally contain** trigger strings:

```
| T032 | speckit-evidence-graph | high | structured task metadata | accepted |
| T033 | speckit-evidence-audit | high | evidence audit          | accepted |
```

So the "evaluation" is substring matching on task titles plus trust-the-author
for everything else. Whether `fs-skia-elmish` vs `fs-skia-scene` was the right
call for a given implementation task was never actually checked. The ceremony in
the docs overstates what the validator does.

**Fix:** Either implement real per-task signal extraction (verbs/nouns →
capability), or downgrade the documentation to describe the trigger-phrase
backstop honestly so authors don't over-trust a green assessment.

---

## 6. 🟡 Governance couples human-readable prose to a substring matcher

A large fraction of `speckit-tasks/SKILL.md` and the template is "title trigger
phrase" hazard warnings: do not write `task graph`, `evidence audit`,
`window visibility validation fixture`, `persistent GUI runtime`, etc. on a task
unless it *owns* that evidence, because the validator blocks on the phrase
appearing in the title. Consequences:

- Writing a natural checklist and satisfying the validator are in tension. The
  natural title "Validate the task graph" must be reworded everywhere **except**
  the one task that should match — where the phrase must be deliberately
  re-introduced.
- The contract is between English and a regex, which is brittle: a future author
  paraphrasing a title can silently flip a task into or out of a blocking group.
- It imposes high cognitive load. Much of the surrounding doc (the `audit-status`
  structured-region spec, blocking-value rules like `taskbar-only=true`) is
  irrelevant to authoring tasks but must be read to know what to avoid.

**Fix:** Move the "this task owns graph/audit evidence" signal out of the title
and into an explicit field in `tasks.deps.yml` (e.g. `owns: [graph-validation]`),
so titles stay free-form and the matcher reads structured intent instead of
prose.

---

## 7. 🟡 `fs-skia-layout-evidence` is an overloaded catch-all

The hint table routes *layout readability*, *deterministic evidence mode*,
*host-warning classification*, and generically *"evidence tasks"* all to
`fs-skia-layout-evidence`. It ended up assigned to ~9 of 33 tasks. When one
skill is the documented answer to four loosely-related concerns, the assignment
carries little information and the "minimal ordered set" guidance can't
meaningfully discriminate.

**Fix:** Split evidence-mode guidance from HUD/layout-readability guidance, or
accept that this skill is a catch-all and stop presenting its assignment as a
precise signal.

---

## 8. 🟠 Two skills give contradictory validator instructions

- `speckit-tasks/SKILL.md`: run the shell script `run-audit.sh … --graph-only`.
- `speckit-evidence-graph/SKILL.md`: "The graph computes **in-process** in
  compiled F# … there is no Python or shell audit runner."

Both are bundled and active. An operator reading the task skill first is sent
down a path the graph skill explicitly says does not exist.

**Fix:** Make `speckit-tasks` defer to `speckit-evidence-graph` for the
validation command rather than restating (an outdated) one.

---

## What worked well

To keep this balanced:

- **Engine error messages are precise and actionable.** "missing or malformed
  'tasks' mapping" and the per-id "tasks.md declares Tnnn but … has no key for
  it" pinpointed both failures exactly.
- **Phase-checkpoint auto-injection behaves as documented.** Writing only
  non-phase cross-edges and letting the engine inject Phase N→N+1 edges worked
  on the first correct run; the rendered mermaid/ASCII graph matched intent.
- **Determinism / speed.** Once the schema was correct, validation was fast and
  reproducible, and the `task-graph.json` / `task-graph.md` outputs were useful.

---

## Recommended fix priority

1. **#2** — stop `EvidenceGraph` from defaulting to a sample feature; this is the
   only issue that yields a confident *false success*.
2. **#4** — reconcile skill-registry ids with the hint tables (false success on
   skill coverage + can cause blocking errors).
3. **#1 / #8 / #3** — fix the stale/contradictory validator command and document
   the real `tasks.deps.yml` schema; each is a guaranteed wasted iteration.
4. **#5 / #6 / #7** — design-level cleanups to reduce brittleness and overstated
   ceremony; lower urgency but they compound author friction over time.

---

## Reproduction notes

- Repo: `AsteroidsDemo3` (generated consumer), branch `001-asteroids-demo`.
- Artifacts authored: `specs/001-asteroids-demo/tasks.md` (33 tasks, 8 phases),
  `specs/001-asteroids-demo/tasks.deps.yml`.
- Failing first validation (bare `Tnnn:` keys) and false-green sample run
  reproduced as described above.
- Passing run:
  `SPECKIT_FEATURE_DIR="specs/001-asteroids-demo" ./fake.sh build -t EvidenceGraph`
  → `verdict=ok, tasks=33, exit 0`.
