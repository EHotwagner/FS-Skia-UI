# Evidence Policy Separation — Feature 077

The `Route` specify-catchall / generated-guidance / generated-template rules require this
artifact when a change touches `.specify/**`-adjacent governance surface. Feature 077 edits the
vendored Spec Kit phase skills under `.agents/skills/speckit-*/**` and the
`build/Governance/**` validators that enforce their currency, and regenerates the derived
`.claude` tree plus `validation.contract.yml`.

## Generated vs authored separation

- **Generated, not hand-synced** — `validation.contract.yml` is **generated from `Routing.fs`**
  (the new `PhaseHookParityCheck` gate was added to the `skill-quality` rule in `Routing.fs`,
  then the contract was regenerated via `RefreshSurfaceBaselines`; `TargetMetadataDrift` reports
  no drift). The `.claude/skills/**` tree is **generated from the canonical `.agents/skills/**`
  tree** — the four repaired phase skills were edited only in `.agents`, then `.claude` was
  regenerated, so `SkillSyncCheck` stays a byte-identical reproduction (no hand-sync).
- **Authored** — the canonical `.agents/skills/speckit-{implement,tasks,taskstoissues,
  constitution}/SKILL.md` hook-discovery blocks are authored text, deliberately identical
  (modulo `<phase>`/anchor) to the five already-compliant sibling skills.

## Policy separation (guard vs text)

- The **rule** lives in one home: `build/Governance/PhaseHookParity.fs` (pure) + the
  `Routing.fs`/`Targets.fs`/Engine wiring. A mistyped gate is a compile error.
- The **skill text** is the data the rule checks. The guard (`PhaseHookParityCheck`) and the
  text are kept in separate layers: the guard never edits skills, and the skills never encode
  routing — anti-drift is enforced by the gate, not by hand-reconciliation.

## Currency policy

- Machine-contract tokens (the three strict markers) stay matched verbatim by the guard's
  literal detectors.
- Generated artifacts (`.claude` mirror, `validation.contract.yml`) are reproduced from their
  single sources; genuine source drift still fails (`SkillSyncCheck` / `TargetMetadataDrift`).
