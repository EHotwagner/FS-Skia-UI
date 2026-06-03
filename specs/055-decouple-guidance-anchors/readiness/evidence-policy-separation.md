# Evidence Policy Separation — Feature 055

The `Route` specify-catchall / generated-guidance rules require this artifact for
`.specify/**` edits. This feature edits governed guidance under `.specify/**`
(the `tasks-template.md` twins) and the `build/Governance/**` validators that
enforce their currency.

## Generated vs authored separation

- **Generated, not hand-synced** — `validation.contract.yml` is generated from
  `Routing.fs` (unchanged here, so it does not regenerate) and the `.claude`
  skill tree is generated from the canonical `.agents` tree. No skill source was
  hand-synced. No `.agents/skills/**` prose was tightened in this feature (only
  `.specify/templates/**`), so `RefreshSurfaceBaselines` produces no `.claude`
  delta and `SkillSyncCheck` stays a byte-identical reproduction.
- **Authored guidance** — the `.specify/templates/tasks-template.md` twins are
  authored guidance whose currency is now enforced by semantic obligations, not
  frozen prose. Tightening the prose (dropping the redundant `skill set` wording)
  is an authored edit that the decoupled currency check correctly passes.

## Currency policy after decoupling

- Machine-contract tokens stay matched verbatim (see
  [contract-tokens.md](./contract-tokens.md)).
- Semantic obligations are checked by presence-of-concept (`AnyOf`/`AllOf`), so
  authored prose may be reworded/shortened without tripping a currency failure
  while genuine source-of-truth drift still fails (see
  [decoupling-red-green.md](./decoupling-red-green.md)).
- The forbidden/stale-term policy is preserved verbatim (FR-006).
