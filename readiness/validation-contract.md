# Validation contract — Route is the single source of the gate list

`./fake.sh build -t Route` reads the working-tree diff (branch-vs-`main` merge-base ∪
uncommitted/untracked changes) and prints the authoritative **tier** and **minimal gate list** for
the change. Validation runs exactly the gates `Route` prints — no more, no less.

- The selector is compiled F# in `FS.Skia.UI.Build` (`Routing`); a mistyped gate is a compile error.
- `validation.contract.yml` is **generated from `Routing.fs`**, not hand-edited; currency is enforced
  by `TargetMetadataDrift`.
- `./fake.sh build -t Route --enforce` additionally fails when an escalated change is missing a
  required evidence artifact, naming the artifact and the requiring tier.

## Tiers

- **inner-loop** — routine framework-internal change (e.g. `src/Scene/**/*.fs`): `Dev` only.
- **focused / agent-ready / maintainer-verify** — consumer-contract, public `.fsi`, governance, or
  template changes **escalate** automatically to the broader gate set the matched rules require.

## Required evidence artifacts (escalated tiers)

Each `Routing.fs` rule declares `ExpectedArtifacts` (repo-root `readiness/**`) that `--enforce`
asserts present:

- `readiness/validation-contract.md` (this file) — the routing/validation contract.
- `readiness/evidence-graph.md` — the EvidenceGraph (DAG) policy.
- `readiness/evidence-audit.md` — the EvidenceAudit (merge-gate) policy.
- `readiness/evidence-policy-separation.md` — generated-guidance vs product evidence separation.
- `readiness/package-surface-expectations.md` — public package-surface baseline policy.

Per-feature live gate logs and verdicts live under `specs/<feature>/readiness/`.
