# Research: Foundations Two-Tier Development Process (Stage 1)

All unknowns below were resolved before Phase 1. The spec's clarification session
(`spec.md` §Clarifications) already fixed the big architectural choices (compiled F# now;
union-of-branch-and-working-tree diff; YAML generated from `Routing.fs`; typed dogfood list);
this file records the remaining engineering decisions the plan depends on.

## R1 — Where the generation-currency check and regeneration live

**Decision.** Split detection from regeneration, both reusing one pure renderer:

- `ContractView.render : RoutingModel -> string` is a **pure** function emitting the canonical
  `validation.contract.yml` text from the compiled routing policy.
- **Detection** folds into the existing **`TargetMetadataDrift`** target: at the `build.fsx` edge it
  reads the on-disk `validation.contract.yml` and compares it to `ContractView.render <model>`; a
  mismatch yields a typed `ContractDrift` finding whose diagnostic says *"validation.contract.yml is
  stale — regenerate from Routing.fs (./fake.sh build -t RefreshSurfaceBaselines)."*
- **Regeneration** folds into the existing **`RefreshSurfaceBaselines`** target (the repo's
  established "rewrite generated baselines" entry point): its body additionally writes
  `ContractView.render <model>` to `validation.contract.yml`.

**Rationale.** Keeps the *only* new FAKE target as `Route` (FR-004 additive guarantee), mirrors the
041 pattern of changing an existing target's **body** (not its name/deps/graph position), and places
contract regeneration alongside the other refresh-generated artifacts. Both `TargetMetadataDrift` and
`RefreshSurfaceBaselines` already exist with metadata rows, so no new metadata/dispatch wiring is
introduced beyond `Route`.

**Alternatives considered.** (a) A dedicated `ContractCurrencyCheck` target — rejected: adds a second
new target + metadata row + dispatch wiring for no benefit over folding into `TargetMetadataDrift`,
which already reads the contract file. (b) Read-behind-model (never materialize the YAML, have
`build.fsx`/`AgentReady`/`TargetMetadataDrift` read `Routing.fs` directly) — rejected: the spec
requires the file be **retained** so existing consumers keep reading a coherent artifact unchanged
(FR-007); emit-and-currency-check satisfies that while still making `Routing.fs` the sole source.

## R2 — Acquiring the `Diff` at the edge; keeping the selector pure

**Decision.** Git invocation stays entirely at the `Route` target body (the interpreter edge). The
edge computes the **union** path set (FR-002a) as:

1. `git merge-base HEAD <default-branch>` → `base` (default branch resolved as `master`, the repo
   default; falls back to `HEAD` with a logged diagnostic if no merge-base, so an empty/garbage range
   never silently yields an empty diff).
2. `git diff --name-only <base>...HEAD` (committed branch changes vs merge-base).
3. `git status --porcelain --untracked-files=all` → modified **and** untracked working-tree paths.

The union of (2) and (3) becomes a `Diff` value (a `Set`/`list` of repo-relative paths). The selector
`Routing.select` is a **pure** function of that `Diff`; tests construct `Diff` values directly and
never shell out (FR-002a, "selection MUST be a pure function of the resulting path set").

**Rationale.** Most-defensive enforcement: nothing already committed and nothing still unsaved can
escape escalation. Purity makes the ≥6 Governance.Tests cases (FR-010) trivial — they assert
`select developerClass diff = expectedSelection` over literal path sets. Git shelling reuses the
existing `BuildProcess` wrapper already used throughout `build.fsx`; no new process abstraction and no
`fsharp-shell-process`-style module is required for this feature.

**Alternatives considered.** Branch-diff-only or working-tree-only — rejected by clarification (a
committed-but-unmerged escalation path, or an unsaved edit, would slip through). Invoking git inside
the selector — rejected: would make the selector impure and untestable without a repo fixture.

## R3 — Total order over tiers (so "highest applicable tier wins")

**Decision.** Give `Tier` a total severity rank via a pure `tierRank : Tier -> int`:

```
InnerLoop(0) < FocusedAuthority(1) < AgentReady(2) < MaintainerVerify(3) < AutomationFinal(4)
```

Legacy `Tier1`/`Tier2` map onto this lattice (`Tier2 → InnerLoop` rank, `Tier1 → AgentReady` rank) so
the retained YAML tiers stay representable. Escalation (FR-003) is `List.maxBy tierRank` over the base
tier plus every matched rule's tier; the resolved gate set is the union of the matched rules' gates
(de-duplicated, in `Targets.allTargets` registry order for deterministic output).

**Rationale.** A single total order makes "escalate, never de-escalate" a one-liner and unit-provable.
Registry-order de-dup gives byte-stable `Route` output for the readiness evidence (SC-001/SC-002).

**Alternatives considered.** A hand-maintained per-pair precedence table — rejected as a second source
of ordering truth and error-prone. Picking the rule with the most gates — rejected: gate count is not
a faithful proxy for authority (a 6-gate focused rule could outrank a 2-gate maintainer-verify rule).

## R4 — `--enforce` artifact presence

**Decision.** `Routing` exposes a pure `unmetArtifacts : present:Set<string> -> Selection -> string
list` returning the selected tier's `expected_artifacts` that are absent. The `Route` body, in
`--enforce` mode, builds `present` from `File.Exists` over the rules' expected artifacts (edge I/O),
calls the pure function, and exits non-zero with a diagnostic naming each missing artifact **and** the
requiring tier when the list is non-empty; otherwise exits zero. Non-enforce mode prints the gate list
and never fails (FR-005).

**Rationale.** Keeps presence logic pure/testable (SC-003 simulates the diff and toggles artifact
presence), with the only I/O being `File.Exists` at the edge.

## R5 — Mapping the existing YAML `routing_rules` to typed rules

**Decision.** Each current `routing_rules` entry becomes a typed `RoutingRule` literal in `Routing.fs`
with: a glob-`Matches : Diff -> bool` predicate (reusing the existing `BuildPaths`/`fsharp-io-globbing`
fnmatch semantics already in the repo), an assigned `Tier`, a `RequiredGates : Targets.Target list`
(so a renamed/mistyped gate is a **compile error**, SC-006/FR-001), and `ExpectedArtifacts`. Tier
assignments: `controls-public-surface`, `generated-template`, `generated-guidance`, `docs-only`,
`package-surface` → `FocusedAuthority`; `evidence-governance` → `AgentReady`; `build-target-contract`
(incl. `validation.contract.yml`) → `MaintainerVerify`. The `framework-author` default base tier is
`InnerLoop` with gates `[Dev]` only — a public `src/**/*.fsi` change **escalates** via the
`package-surface` rule (FocusedAuthority) rather than adding a check to inner-loop (F1).
`ConsumerAgent` raises the base floor to `FocusedAuthority`. `template/**` and `.specify/**` match as
**broadened** globs (full consumer-contract coverage, F2: `generated-template` → `template/**`, plus a
`specify-catchall` → `.specify/**`). Unmatched paths default-deny to the broad fallback (`Verify`),
preserving `unknown_gate_rejection` (FR-002).

**Rationale.** Derived from today's YAML — with `template/**` and `.specify/**` broadened to full
consumer-contract coverage (F2) — so the generated `ContractView.render` reproduces the file's existing
consumers' expectations; typing the gate lists is the whole point of moving the source
of truth into compiled F#.

## R6 — Developer-class flag & dogfood resolution at the edge

**Decision.** `DeveloperClass` defaults to `FrameworkAuthor`; the `Route` body parses an optional
`--developer-class consumer-agent` token from the FAKE target arguments to raise the floor explicitly.
`ConsumerAgent` raises the base tier floor to `FocusedAuthority` (`maxBy tierRank`); consumer-contract
**paths** escalate regardless of class (the path rules are evaluated for every class). The dogfood set is a typed `dogfoodFeatureIds : string list` literal in `Routing.fs`
containing `"042"`; the `Route` body resolves the active feature id via the existing
`activeFeatureId repositoryRoot` helper (already reading `.specify/feature.json`) and, when it is in
the list, forces the full serialized pipeline gate set regardless of the diff's tier (FR-006).

**Rationale.** Light path is the default with no required argument (clarification); dogfood policy
lives with routing policy in the compiled source of truth (ADR D6), build-time-checked and unit-
testable (SC-005), and is reflected in the generated YAML view.

## Summary of resolved decisions

| # | Decision |
|---|----------|
| R1 | Pure `ContractView.render`; **detection** in `TargetMetadataDrift`, **regen** in `RefreshSurfaceBaselines`; `Route` is the only new target |
| R2 | Git union-diff computed at the `Route` edge; selector pure over the path set; tests never shell out |
| R3 | Total `tierRank`; escalation = `maxBy tierRank`; gates de-duped in registry order |
| R4 | Pure `unmetArtifacts`; `--enforce` does `File.Exists` at the edge, exits non-zero naming artifact + tier |
| R5 | YAML `routing_rules` → typed `RoutingRule` literals (`template/**`, `.specify/**` broadened, F2; `.fsi` escalates, not inner-loop, F1); gate lists are `Targets.Target` (mistype = compile error) |
| R6 | Default `FrameworkAuthor`; `--developer-class` flag; typed `dogfoodFeatureIds` incl. `"042"`, resolved via `activeFeatureId` |
