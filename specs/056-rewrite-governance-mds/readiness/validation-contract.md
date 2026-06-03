# Validation Contract Currency — Feature 056

Single-source-generation currency (FR-008, SC-004). The escalated `Route`
docs/evidence-governance rules require this artifact. Authoritative commands:
`./fake.sh build -t TargetMetadataDrift` and `./fake.sh build -t SkillSyncCheck`
— both **green** after the rewrite.

## Single-source-generation invariants held green

- **`validation.contract.yml` ← `Routing.fs`** — `Routing.fs` is **not edited**
  by this feature, so `validation.contract.yml` is **byte-unchanged**
  (`git status` clean for that path) and `TargetMetadataDrift` stays current.
- **constitution principle fragments ← `.specify/memory/constitution.md`** — the
  rewrite did not alter the first sentence of Principles II/IV/V/VI (the fragment
  sources), so `RefreshSurfaceBaselines` re-spliced them as a no-op and
  `TargetMetadataDrift`'s fragment-currency check stays green. Only Principle III
  prose (not a fragment source) was tightened.
- **`.claude/skills/**` ← `.agents/skills/**`** — the canonical `.agents` tree was
  rewritten, then `RefreshSurfaceBaselines` regenerated `.claude` as a
  byte-identical reproduction across all 26 enumerated files; `SkillSyncCheck` is
  green (see `skill-sync-check.md`).

## What changed (generation inputs)

- `.agents/skills/**/*.md` — prose tightened (canonical source).
- `.specify/**/*.md` — prose tightened (`constitution.md`, `constitution-template.md`
  twins, `extensions/git/README.md`); template/preset twins kept in lockstep.
- `.claude/skills/**/*.md` — regenerated, never hand-edited.

No product `.fsi` surface, `Routing.fs`, `validation.contract.yml`, package
identity, version, or runtime behavior changed.
