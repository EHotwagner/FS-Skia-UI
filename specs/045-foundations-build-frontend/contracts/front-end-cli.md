# Contract: Front-End CLI / Launcher Argument Forwarding

The compiled front-end must be a **drop-in** replacement for the `dotnet fake` CLI for every
existing invocation. This contract fixes the argument-forwarding semantics (FR-002, SC-003, the
spec's default-target/argument-passing edge case).

## Invocation forms that MUST keep working identically

| Caller invocation | Must resolve to | Notes |
|---|---|---|
| `./fake.sh build -t Dev` | run target `Dev` | `build` + `-t <name>` is the canonical form; `-t` selects the target |
| `fake.cmd build -t TemplateCheck` | run target `TemplateCheck` | Windows launcher, same semantics |
| `./fake.sh build -t Route` | run `Route`, print tier + gates | consumes `Routing.fs` in-process (FR-005) |
| `./fake.sh build -t Route --enforce` | run `Route` in enforce mode; non-zero exit if an escalated change lacks evidence | flags after the target name forward verbatim |
| `dotnet run --project build/Build.fsproj -- <target>` | run `<target>` directly | the underlying call the launchers wrap |
| `./fake.sh` / `dotnet run … --` with no target | run the **default** target (`Dev`) | `Target.runOrDefaultWithArguments "Dev"` preserves today's default |

## Forwarding rules

1. **Everything after `--`** (for `dotnet run`) or **all launcher args** (`"$@"` / `%*`) is passed
   to `Target.runOrDefaultWithArguments`, which already parses FAKE's `build -t <name> [flags]`
   grammar — so the existing argument shapes are honoured without bespoke parsing in `Program.fs`.
2. **Exit code**: the process exit code MUST reflect target success/failure. `fake.cmd` MUST
   preserve the current `%ERRORLEVEL%` propagation; `fake.sh` keeps `set -euo pipefail` so a failing
   `dotnet run` propagates non-zero.
3. **No `dotnet fake` and no `dotnet tool restore` for fake-cli** may remain in either launcher
   (grep-proven, FR-003). If `FAKE_*` environment variables are not read under `dotnet run`, they
   are removed; if still consulted by the Target API, they are retained — verified at implementation
   time, recorded in readiness.
4. **Working directory**: both launchers keep `cd`-ing to the script directory so relative repo
   paths resolve as before.

## Negative contract (must NOT change)

- No new target names, no renamed targets, no changed default target.
- No new required flags; existing CI scripts and agent invocations are unaffected.
- `.config/dotnet-tools.json` no longer lists `fake-cli`; no other tool entry is removed.

## Verification

- Grep proofs: `readiness/logs/no-dotnet-fake.txt`, `no-fake-cli.txt` (absent in launchers/scripts).
- A representative-invocation check (each form above) recorded in the parity/timing evidence.
