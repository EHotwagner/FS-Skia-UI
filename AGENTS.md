For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan:
<!-- SPECKIT START -->
specs/093-visual-state-style-layer/plan.md
<!-- SPECKIT END -->

## Run `Route` first; run only the gates it prints

Before validating a change, run `./fake.sh build -t Route`. It reads the
working-tree diff (the union of the branch-vs-`main` merge-base diff and the
uncommitted/untracked changes) and prints the authoritative **tier** and the
**minimal gate list** for *this* change. Run only the gates it prints.

- A routine framework-internal change (e.g. `src/Scene/**/*.fs`) routes to the
  light **inner-loop** tier — `Dev` only.
- Consumer-contract changes (`template/**`, `.specify/**`, public `src/**/*.fsi`,
  `build.fsx`/`scripts/build/**`, governance paths) **escalate** automatically.
- `./fake.sh build -t Route --enforce` additionally fails when an escalated
  change is missing its required evidence artifacts, naming the artifact and the
  requiring tier.

The selector is compiled F# in `FS.Skia.UI.Build` (`Routing`); a mistyped gate
is a compile error. `FS.Skia.UI.Build` (`build/Governance/**`) is the **single
home of all rules**, and governance artifacts are **generated from a single
source, not hand-synced**: `validation.contract.yml` is generated from
`Routing.fs` (currency-checked by `TargetMetadataDrift`), and the `.claude` skill
tree is generated from the canonical `.agents` tree (`SkillSyncCheck`-enforced),
so neither can drift.

## The serialized six-target order (escalated / maintainer-verify path)

The full serialized order below is the **escalated `maintainer-verify` path**,
reserved for consumer-contract changes and **dogfood** features (such as `042`).
It is no longer the unconditional default — run it only when `Route` escalates to
it. FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Agents may
parallelize safe non-FAKE file reads and checks, but must run FAKE-backed tests
and FAKE targets sequentially when more than one is needed.

Use the deterministic FAKE-backed order:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

If a FAKE-backed failure looks race-like or the concurrent FAKE context is
unknown, rerun the affected FAKE-backed commands sequentially before product
debugging.
