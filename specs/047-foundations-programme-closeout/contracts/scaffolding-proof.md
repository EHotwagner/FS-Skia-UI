# Contract — Scaffolding Proof Scope (FR-001 / FR-002, SC-001)

Defines what counts as a passing proof that no **interim scaffolding** remains, given that the
naive token patterns match non-scaffolding prose. The proof lives in
`readiness/scaffolding-proof.md`.

## Patterns and their proof commands

| # | Pattern | Proof kind | Command (reproducible) | Pass condition |
|---|---|---|---|---|
| 1 | root `build.fsx` | file-existence | `git ls-files build.fsx` | empty output |
| 2 | `scripts/build/select-tier.fsx` | file-existence | `git ls-files 'scripts/build/select-tier.fsx' '**/select-tier.fsx'` | empty output |
| 3 | `run-audit.sh` | file-existence | `git ls-files '**/run-audit.sh'` | empty output |
| 4 | `.specify/**/*.py` | file-existence | `git ls-files '.specify/**/*.py'` | empty output |
| 5 | `--legacy-evidence` flag | scoped-grep | full: `git grep -n -- '--legacy-evidence'`; scoped: same excluding the allowlist | scoped = zero |
| 6 | `fake-cli` / `dotnet fake` / `FSharp.Compiler.*` | scoped-grep | full: `git grep -nE 'fake-cli\|dotnet fake\|FSharp\.Compiler\.' -- . ':!template/base/build.fsx'`; scoped: same excluding the allowlist | scoped = zero |

## Excluded by design (always)

- Gitignored build output (`artifacts/`, `.fake/`) — not tracked.
- The by-design generated consumer front-end `template/base/build.fsx` — a legitimate thin
  front-end, explicitly out of scope (spec Context, FR-001).

## Allowlist — non-scaffolding match classes (named at each retained match)

A scoped-grep match is **allowlisted** (not a residual) iff it is one of:

1. **Frozen feature-history prose** — files under `specs/**` (especially
   `specs/043-foundations-evidence-engine/**`) and
   `docs/reports/2026-05-31-1049-foundations-implementation-plan.md`. These *record* the removal;
   rewriting merged history is out of scope and dishonest.
2. **Governance-library enforcement scan-strings** — `build/Governance/Guidance.fs` regex literals
   that *detect* `dotnet fake` in docs to enforce the `Route`-first rule. The token is the policed
   thing, not a live invocation.
3. **Assert-the-absence comments** — `Directory.Packages.props` comments stating
   `FSharp.Compiler.Service` is NOT shipped. (Verified: no `<PackageVersion>` / paket entry
   references it.)
4. **Legitimate live-FAKE entry-point text** — `./fake.sh`/`fake.cmd`/`dotnet fake` as the *current*
   FAKE entry points in diagnostics/comments (`build/Program.fs`, `build/Governance/Preflight.fs`).
   These are valid usage, not the removed FSX-runner scaffolding.

## Residual handling (FR-002)

Any match **outside** the allowlist is a genuine residual: it is removed (dead script/flag) or
corrected (stale operative doc reference, e.g. `branch-vs-master` in `docs/reports/build.md`), and
the proof command re-run until the scoped result is zero. Each correction is recorded with its
`verdict = residual-removed`.

## Acceptance (SC-001)

Every pattern's proof command, when re-run at the pinned SHA, yields its recorded result: empty for
file-existence proofs, zero for scoped greps after the named allowlist. No pattern is left
"asserted non-zero" without an allowlist justification.
