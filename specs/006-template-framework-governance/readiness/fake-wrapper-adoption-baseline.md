# FAKE Wrapper Adoption Baseline

Files added for the canonical command surface:

| File | Purpose |
|------|---------|
| `.config/dotnet-tools.json` | Pins repo-local `fake-cli` 5.23.1. |
| `fake.sh` | Bash wrapper that restores local tools and runs `dotnet fake`. |
| `fake.cmd` | Windows command wrapper that runs the same FAKE target graph. |
| `build.fsx` | Owns the canonical target graph and workflow effect boundary. |

Both wrappers invoke the same `build.fsx` graph. No runtime `.fsi` API surface
is introduced.
