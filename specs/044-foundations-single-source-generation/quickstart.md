# Quickstart: Single-Source Generation (Stage 2.2–2.5)

How to exercise and verify the three generation-currency checks once feature 044 lands. This is a
`.specify/**` + skill-tree + governance change, so `Route` **escalates** it to the full serialized
gate set — run FAKE targets **sequentially** (Invariant 5), never concurrently.

## The single regeneration command

```bash
./fake.sh build -t RefreshSurfaceBaselines
```

Regenerates the derived skill tree (`.claude/skills/` from `.agents/skills/`), the constitution
fragments spliced into the templates, and the `validation.contract.yml` view from `Routing.fs`.
(The skillist annotation regenerates per-feature via the evidence path — see US2 below.)

## US1 — skill trees (currency)

```bash
# Edit any canonical skill — including one of the 19 slugs the old 6-slug check never guarded:
$EDITOR .agents/skills/fsharp-parsing/SKILL.md

# Without regenerating, the currency gate fails and names the regeneration command:
./fake.sh build -t SkillSyncCheck          # FAIL: ".claude/skills is stale — run RefreshSurfaceBaselines"

# Regenerate; the derived tree becomes byte-identical across all 25 pairs:
./fake.sh build -t RefreshSurfaceBaselines
./fake.sh build -t SkillSyncCheck          # PASS

# Adding a brand-new skill needs zero allowlist edits (coverage is by enumeration):
mkdir -p .agents/skills/fs-skia-new/ ; $EDITOR .agents/skills/fs-skia-new/SKILL.md
./fake.sh build -t RefreshSurfaceBaselines # .claude/skills/fs-skia-new/SKILL.md appears
```

Editing the **derived** tree directly (`.claude/skills/**`) is reported as drift to be regenerated
away (it is never the source).

## US2 — skillist (currency, active feature only)

```bash
# Canonical source is tasks.deps.yml; the tasks.md [skillist: …] annotation is the derived view.
$EDITOR specs/<active-feature>/tasks.deps.yml      # change a task's skillist:

# The active-feature evidence audit flags the derived annotation as stale (not a peer mismatch):
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit                   # FAIL: "<task> tasks.md skillist view is stale; regenerate"

# Regenerate the derived annotation, then the audit is green again.
# Editing the derived annotation alone (without the deps source) is likewise flagged stale.
```

Historical feature directories are **never** re-derived, so this introduces zero new audit failures
on features whose representations already agree (SC-004).

## US3 — constitution (currency)

```bash
$EDITOR .specify/memory/constitution.md            # change a principle

# Without regenerating, the currency check (folded into TargetMetadataDrift) flags the stale region:
./fake.sh build -t TargetMetadataDrift             # FAIL: "<template> constitution fragment is stale; regenerate"

./fake.sh build -t RefreshSurfaceBaselines
./fake.sh build -t TargetMetadataDrift             # PASS — templates now reflect the change
```

Genuine hand-written guidance prose lives **outside** the `BEGIN GENERATED`/`END GENERATED` markers
and is preserved unchanged by regeneration.

## Full escalated gate sequence (exit gate)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Invariant spot-checks

- `git diff --stat` over product `src/**` = **0** (runtime untouched, SC-009).
- `./fake.sh build -t PackageSurfaceCheck` + `FsiTranscripts`: **no product baseline diff**.
- grep: no `FSharp.Compiler.*`, no `diff`/`cmp`/`sha256sum`/symlink shelling in the generation path.
- All 25 derived `SKILL.md` byte-identical to canonical (in-process proof; SC-001).
