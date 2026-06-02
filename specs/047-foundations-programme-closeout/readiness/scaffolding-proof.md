# Scaffolding proof (US1 — FR-001 / FR-002, SC-001)

Committed grep-proof that the tracked tree contains **no interim scaffolding**. Each
pattern carries its exact reproducible command and the recorded result. For the two
token patterns the naive grep matches non-scaffolding prose, enforcement scan-strings,
assert-the-absence comments, and legitimate live-FAKE text — so the proof records the
full match set and then the **scoped** result (the allowlist excluded), which is zero.

**Pinned context.** Captured at `git_commit = 4276bd061d95d47c61deb141a3b4bb65ccebe4e0`
(short `4276bd0`), branch `047-foundations-programme-closeout`, toolchain dotnet
`10.0.300`. Excluded **by design (always)**: gitignored build output (`artifacts/`,
`.fake/`) and the by-design generated consumer front-end `template/base/build.fsx`
(spec Context, FR-001).

## Section 1 — File-existence proofs (dead artifacts, T005)

Each command prints nothing → the artifact does not exist in the tracked tree.

| # | Pattern | Command | Result | Verdict |
|---|---|---|---|---|
| 1 | root `build.fsx` | `git ls-files build.fsx` | *(empty)* | `clean` |
| 2 | `select-tier.fsx` | `git ls-files 'scripts/build/select-tier.fsx' '**/select-tier.fsx'` | *(empty)* | `clean` |
| 3 | `run-audit.sh` | `git ls-files '**/run-audit.sh'` | *(empty)* | `clean` |
| 4 | `.specify/**/*.py` | `git ls-files '.specify/**/*.py'` | *(empty)* | `clean` |

```bash
git ls-files build.fsx                                          # (empty)
git ls-files 'scripts/build/select-tier.fsx' '**/select-tier.fsx'  # (empty)
git ls-files '**/run-audit.sh'                                  # (empty)
git ls-files '.specify/**/*.py'                                 # (empty)
# corroboration: zero tracked .py and zero dead evidence scripts
git ls-files '*.py' | wc -l                                     # 0
git ls-files '**/compute-task-graph.py' '**/audit-status-scan.py' '**/run-audit.sh' | wc -l  # 0
```

All four dead artifacts confirmed gone (deleted in features 043/045). Verdict: `clean`.

## Section 2 — Scoped token proofs (flag / runner, T006)

### Pattern 5 — `--legacy-evidence` flag

```bash
# full token grep:
git grep -n -- '--legacy-evidence'
# scoped (allowlist excluded):
git grep -n -- '--legacy-evidence' -- . ':!specs' \
  ':!docs/reports/2026-05-31-1049-foundations-implementation-plan.md'   # (empty)
```

`raw_result`: every match is in `specs/043-foundations-evidence-engine/**` (frozen
feature history) or the foundations implementation-plan history doc. `scoped_result`:
**zero**. Every retained match is **allowlist class 1 (frozen feature-history prose)** —
these *record* that the flag was removed at parity sign-off (feature 043 T029). Verdict:
`clean`.

### Pattern 6 — `fake-cli` / `dotnet fake` / `FSharp.Compiler.*`

```bash
# full token grep (template/base/build.fsx excluded by design):
git grep -nE 'fake-cli|dotnet fake|FSharp\.Compiler\.' -- . ':!template/base/build.fsx'   # 301 matches
# scoped (every allowlist class excluded):
git grep -nE 'fake-cli|dotnet fake|FSharp\.Compiler\.' -- . \
  ':!template/base/build.fsx' ':!template' ':!specs' \
  ':!docs/reports/2026-05-31-1049-foundations-implementation-plan.md' \
  ':!docs/reports/2026-05-31-0908-foundations-rewrite-analysis.md' \
  ':!docs/reports/2026-05-31-1714-foundations-fsharp-capabilities-and-libraries.md' \
  ':!docs/reports/_baselines' ':!docs/adr/0002-build-front-end-form.md' \
  ':!docs/adr/0005-configuration-representation.md' \
  ':!build/Governance/Guidance.fs' ':!build/Program.fs' ':!build/Governance/Preflight.fs' \
  ':!Directory.Packages.props' ':!Container' ':!.claude/skills' ':!.agents/skills' \
  ':!.specify/templates' ':!.specify/presets' ':!AGENTS.md' ':!CLAUDE.md' \
  ':!docs/reports/evidence.md' ':!docs/reports/dependencies.md' ':!docs/reports/build.md' \
  ':!tests/Governance.Tests' ':!src/Testing/Testing.fs'                 # (empty)
```

`raw_result`: 301 matches. `scoped_result`: **zero**. Every retained match is in one of
the named allowlist classes below — none is removed FSX-runner scaffolding.

#### Allowlist — non-scaffolding match classes (each retained match named)

1. **Frozen feature-history prose (class 1)** — `specs/**` (especially
   `specs/043-foundations-evidence-engine/**`), the foundations implementation-plan and
   analysis history docs, the `docs/reports/_baselines/2026-05-31-*` Stage-0 baselines,
   and ADR `0002`/`0005` (which *record* the D2 / D6 FCS-removal decisions). Rewriting
   merged history is out of scope and dishonest.
2. **Governance-library enforcement scan-strings (class 2)** — `build/Governance/Guidance.fs`
   regex literals that *detect* `dotnet fake` in docs to enforce the `Route`-first rule,
   mirrored by `tests/Governance.Tests/SequentialFakeGuidanceTests.fs`. The token is the
   policed thing, not a live invocation.
3. **Assert-the-absence comments / dependency-manifest notes (class 3)** —
   `Directory.Packages.props` comments stating `FSharp.Compiler.Service` is **NOT**
   shipped (verified: no `<PackageVersion>` references it); `docs/reports/dependencies.md`
   `FSharp.Compiler.*`-absence rows; the `tests/Governance.Tests/CommandContractTests.fs`
   assertions that **prove `fake-cli` / `dotnet fake` / `dotnet tool restore` are gone**.
   The one stale phrase in `dependencies.md` ("Keep aligned with the `fake-cli` local
   tool and `build.fsx.lock`") is now historical (the `Fake.Core.Target` *library*
   remains; the CLI tool and lock are gone); it is **retained-with-reason** — dependency-
   doc edits are outside this feature's declared scope (plan Constitution Check forbids a
   `docs/reports/dependencies.md` change), flagged here as a trivial follow-up.
4. **Legitimate live-FAKE entry-point text / FAKE-serialization guidance (class 4)** —
   `./fake.sh` / `fake.cmd` / `dotnet fake` named as current FAKE command forms in
   diagnostics and concurrency guidance: `build/Program.fs`, `build/Governance/Preflight.fs`
   (and its `ProcessReliabilityContractTests`), the `.agents/skills/**` ↔ `.claude/skills/**`
   FAKE-serialization guidance, `.specify/templates/**` + `.specify/presets/**` template
   prompts, `AGENTS.md`, `CLAUDE.md`, `docs/reports/build.md` (line 16 only), and
   `docs/reports/evidence.md`. Valid usage, not the removed FSX-runner scaffolding.
5. **By-design generated consumer content (class 5)** — `template/**` (the generated
   product's own `fake.sh`/`dotnet fake` guidance in `template/base/README.md`,
   `template/base/docs/product.md`, and the generated skill mirrors). The generated
   consumer front-end is explicitly out of scope (spec Context, FR-001) — same rationale
   as the always-excluded `template/base/build.fsx`.
6. **Out-of-scope product-source string (retained-with-reason)** —
   `src/Testing/Testing.fs:396,403` carry a now-stale `RemediationCommand =
   "dotnet fake run build.fsx --target PackLocal"` string. This is **product runtime
   source**, which FR-010 / SC-006 forbid this feature from touching
   (`git diff -- 'src/**'` must be empty). It is retained with this reason and flagged as
   a follow-up for a future runtime-touching feature; it is not interim build scaffolding.

## Section 3 — Residual handling (T007, FR-002)

The fully-scoped grep above is **empty**, so the only residuals the sweep surfaced are
the operative stale references in `docs/reports/build.md`, corrected by the US3 doc pass
(T011), not here:

| Residual | Location | Disposition | Verdict |
|---|---|---|---|
| `fake-cli` local-tool + `build.fsx` target-graph restore claim | `docs/reports/build.md` (was lines 23–25) | wrappers now run the compiled `build/Build.fsproj` front-end (feature 045); corrected in T011 | `residual-removed` |
| `branch-vs-`master`` stale `Route` diff description | `docs/reports/build.md` (line 168) | corrected to `branch-vs-`main`` in T011 (default branch renamed 2026-06-01) | `residual-removed` |

All other matches are `clean` (allowlisted by class above) or `retained-with-reason`
(dependency-doc note + product-source string, both out of this feature's declared edit
scope). No pattern is left "asserted non-zero" without an allowlist justification.

## Acceptance (SC-001)

Every pattern's proof command, re-run at the pinned SHA, yields its recorded result:
empty for the four file-existence proofs, and zero for the two scoped token greps after
the named allowlist. The two operative `build.md` residuals are corrected by T011.
