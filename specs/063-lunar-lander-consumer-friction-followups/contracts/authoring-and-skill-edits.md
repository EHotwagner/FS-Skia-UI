# Contract: Authoring guidance + diagnostic edits (FR-004/005/006/007/008)

All skill edits are made in canonical `.agents/skills/**` and regenerated to
`.claude/**` via `./fake.sh build -t RefreshSurfaceBaselines`
(`SkillSyncCheck`/`SkillQualityCheck`/`TargetMetadataDrift` green).

## FR-004/005 — `speckit-implement` skill + readiness diagnostic

- `.agents/skills/speckit-implement/SKILL.md`: add a pre-implementation pointer to
  read `docs/evidence-formats.md` **before** writing readiness/evidence files; and
  enrich the skill-loading-evidence step to state it reads from
  `specs/<feature>/readiness/` (not repo-root), needs one row per (task,
  declared-skill) with `.agents/skills/<id>/SKILL.md` paths and
  `loaded_at < work_started_at`, and is **enforced only once tasks flip to `[X]`**.
- `build/Governance/Evidence/Render.fs:471-480`: relabel `required-tokens:` →
  `full-required-set:` and `missing:` → `absent-from-file:` so one absent token does
  not read as "all missing." No data shape change.

**Verify**: a readiness-contract failure with one absent token prints both labels
distinctly (SC-004); the implement skill body names the reference + the
skill-loading location/timing.

## FR-006 — `speckit-plan` skill + `scaffold-map.md`

- `.agents/skills/speckit-plan/SKILL.md`: pre-planning pointer to read
  `docs/scaffold-map.md` before designing a generated product's game model.
- `template/base/docs/scaffold-map.md`: add an "API surface authority" note —
  shipped `.fsi` / `docs/api-surface/` is authoritative; agent-generated API
  summaries are supporting reference only.

**Verify**: plan flow references `scaffold-map.md`; the map carries the
`.fsi`-authoritative note (SC-005).

## FR-007 — `speckit-specify` skill (URL source snapshot)

- `.agents/skills/speckit-specify/SKILL.md` (step 3 "Create the spec feature
  directory"): when the feature input is an external URL, after fetching, snapshot
  the source into `specs/<feature>/source-spec.md` (record the URL in a header) and
  reference the in-repo snapshot. Local-file / inline input → explicit no-op.

**Verify**: specifying from a URL yields an in-repo snapshot (or documented
reproducible fetch step); local input creates no redundant copy (SC-005, edge case).

## FR-008 — evidence-path token (no code change)

Recorded disposition: a template-wide check confirms **no generated artifact
template seeds a divergent `evidence/` token** (`.specify/templates/spec-template.md`
references neither path; `tasks-template.md` uses `readiness/` consistently;
`template/base/docs/**` seeds no `specs/<feature>/evidence/`). The consumer's
analyze-time drift was consumer-authoring, self-reconciled to `readiness/`. **Close
with no code change**; the readiness log records the template-scan result. (SC-005)
