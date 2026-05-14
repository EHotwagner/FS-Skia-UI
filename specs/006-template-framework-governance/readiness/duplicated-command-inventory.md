# Duplicated Command Inventory

Pre-v1 command order appeared in the following places:

| Location | Previous Shape | V1 Handling |
|----------|----------------|-------------|
| `README.md` | Listed raw `dotnet restore`, `dotnet build`, `dotnet test`. | Points to `Dev`, `Verify`, and docs. |
| `tests/Package.Tests/Tests.fs` | Packed into historical feature readiness and ran consumer restore smoke by default. | Packs to the local package source; consumer smoke is explicit `PackageSmoke`. |
| `tests/Package.Tests/SurfaceAreaTests.fs` | Read baselines from `specs/002-skia-feature-parity/readiness/surface-baselines`. | Reads `readiness/surface-baselines/*.txt`. |
| `scripts/refresh-surface-baselines.fsx` | Wrote baselines to a historical feature folder. | Writes `readiness/surface-baselines/*.txt`. |
| `.specify/workflows/speckit/workflow.yml` | Ended after implementation without canonical verification. | Adds a canonical `./fake.sh build -t Ci` verification step. |
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | Named evidence graph/audit but not the canonical FAKE target surface. | Adds target guidance for future generated task lists. |
