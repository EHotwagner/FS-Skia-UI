---
name: speckit-merge
description: Squash-merge all feature branches into the default branch (main or master), delete them, and push. Use when the user says "land this", "ship the feature", "merge the feature branches", "squash-merge and push", or anything that means consolidating feature work onto the trunk. After a successful merge, every packable library project (the `src/**` libs + `build/Governance/FS.Skia.UI.Build`, NOT the `dotnet new` template) MUST be packed with a bumped version number.
metadata:
  short-description: Squash-merge feature branches, bump+pack, and push
---

# speckit-merge

Consolidate feature branches onto the trunk (`main` or `master`) via
squash-merge, push to origin, and — after a successful merge — bump the
patch version of every packable **library** project (NOT the `dotnet new`
template, which is a separate version track owned by `/fs-skia-template-update`)
and produce a local NuGet package. The bump-and-pack step is **mandatory**
whenever any packable library project is present; skip it only when the repo has
no packable library projects at all.

## When to use

- User says "ship it", "land this branch", "squash-merge", "merge the
  features", "land all feature branches", etc.
- After a feature has passed `/speckit.evidence.audit` with verdict PASS
  (or explicit `--accept-synthetic` override).

## Preconditions — check before doing anything destructive

1. Working tree is clean (`git status --porcelain` is empty). Refuse and
   ask the user to commit or stash if not.
2. On a git repo. Refuse if not.
3. The evidence audit, if applicable, has PASSED for each feature branch
   being merged. If `readiness/synthetic-evidence.json` exists with an
   unlogged override, surface that to the user before proceeding.

## Steps

### 1. Detect trunk

```bash
TRUNK=""
if git show-ref --verify --quiet refs/heads/main; then TRUNK=main
elif git show-ref --verify --quiet refs/heads/master; then TRUNK=master
else
  echo "No main or master branch; ask the user which branch is the trunk."
  exit 2
fi
```

### 2. Enumerate feature branches

Every local branch that is NOT the trunk is a candidate feature branch.
List them and confirm with the user before merging more than one.

```bash
git for-each-ref --format '%(refname:short)' refs/heads/ | grep -v "^$TRUNK$"
```

### 3. Switch to trunk and pull

```bash
git checkout "$TRUNK"
git pull --ff-only origin "$TRUNK" 2>/dev/null || true
```

### 4. For each feature branch — squash-merge and delete

```bash
for BRANCH in <list>; do
  git merge --squash "$BRANCH"
  # Abort and prompt the user if there are conflicts.
  if ! git diff --cached --quiet; then
    git commit -m "Merge $BRANCH (squash)"
    git branch -D "$BRANCH"
  fi
done
```

**Conflict handling.** If a squash-merge reports conflicts, do NOT resolve
automatically. Abort with `git merge --abort`, tell the user which branch
conflicted, and ask them to resolve manually. Never take sides.

### 5. Push to origin

```bash
git push origin "$TRUNK"
```

### 6. NuGet pack — MANDATORY after a successful merge

After step 5 succeeds, this step is **required**. Skip it only when the
repo contains zero packable library projects. Detect them with:

```bash
PACKABLE=$(grep -lE '<IsPackable>\s*true|<PackageId>' $(find . -name '*.fsproj') \
  | grep -v '/\.template\.package/')
```

**The merge bump covers the repo *library* packages ONLY — never the
`dotnet new` template.** `.template.package/FS.Skia.UI.Template.fsproj` is a
**separate version track** owned by the `/fs-skia-template-update` flow (it pins
the libraries via `template/base/Directory.Packages.props`, which the merge does
not touch). Bumping the template here would push its version ahead of its pins
and create drift, so the `grep -v` above excludes it — and you MUST NOT bump it
in this step even if you bump versions by hand. The authoritative library set is
exactly the projects `PackLocal` packs (`packProjects` in
`build/Governance/Front/Helpers.fs`): the `src/**` libraries plus
`build/Governance/FS.Skia.UI.Build.fsproj` (which lives under `build/`, not
`src/` — do not miss it). `FS.Skia.UI.Build` and the `src` libs share one
version, so they all bump together.

If `PACKABLE` is empty, skip. Otherwise, for **every packable library project
(template excluded)**, you MUST:

1. Read the current `<Version>` from the `.fsproj` (FS.Skia.UI projects pin
   it per-project, e.g. `0.1.37-preview.1`). If absent, insert
   `<Version>0.1.0</Version>` into the first `<PropertyGroup>`.
2. Increment the **patch** segment by 1 (always — never reuse or
   decrement), preserving any preview suffix. The version must strictly
   increase on every merge so downstream FSI consumers see a fresh package.
3. Update the `<Version>` element in place.
4. Produce the local packages. The canonical mechanism is the FAKE target,
   which packs **all** packable projects to `~/.local/share/nuget-local`
   in Release:

   ```bash
   ./fake.sh build -t PackLocal
   ```

   Equivalently, pack a single project with
   `dotnet pack <proj> -c Release -o ~/.local/share/nuget-local`. If the
   pack fails, stop and surface the error — do not push a half-bumped repo.
   If `PackLocal` (or a packing run) **stalls** — a `dotnet` build sitting at
   ~0% CPU with no compiler child — the cause is usually concurrent builds of
   overlapping projects deadlocking on `obj/`/`bin` locks. Recover by
   `dotnet build-server shutdown` (and, if needed, killing stray `dotnet`
   processes), then build the library set **sequentially** once
   (`dotnet build <leaf> -c Release -p:UseSharedCompilation=false -nodeReuse:false`
   for the leaf projects, which pulls the whole graph) and pack each with
   `dotnet pack <proj> -c Release --no-build -o ~/.local/share/nuget-local`.
5. Commit the version bumps: `Bump packable project versions`.
6. Push the bump commit.

The merge is not "done" until every packable **library** project (template
excluded) has been bumped, packed, committed, and pushed.

> FAKE note: `./fake.sh` / `dotnet fake` share repository `.fake` state and are
> not safe to run concurrently. Run FAKE-backed pack/validation commands one at
> a time, in deterministic order; never in parallel with another FAKE target.

### 7. Clear NuGet caches (F# libraries only)

FSI caches resolved packages aggressively. If a new version was just
produced for downstream FSI scripts, clear both caches:

```bash
dotnet nuget locals http-cache --clear
dotnet nuget locals global-packages --clear
```

## Safety rails

- NEVER force-push.
- NEVER bypass pre-commit hooks (`--no-verify`) unless the user explicitly
  requests it and owns the reason.
- NEVER delete a branch that hasn't been successfully squash-merged.
- If the trunk has diverged from origin (local `git pull --ff-only` would
  fail), stop and ask the user. Do not attempt a rebase or merge of the
  trunk automatically.
- If the version bump commit fails to push (e.g., someone else bumped
  first), stop and surface the conflict. Do not retry.

## After running

Report to the user:
- Which branches were squash-merged, in order.
- Any branches that were skipped and why (conflicts, audit failure, user
  cancelled).
- The new versions of packed projects, if any.
- Whether origin was pushed successfully.
