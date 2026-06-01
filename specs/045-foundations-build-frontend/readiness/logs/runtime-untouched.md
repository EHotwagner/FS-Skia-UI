# SC-008 Standing-Invariants Proof (T024)

| Invariant | Check | Result |
|---|---|---|
| Runtime untouched | `git status --porcelain src/` | **0 changed files** |
| No new package identity | `git status --porcelain Directory.Packages.props` | unchanged |
| Product surface baseline | only build-tooling `.fsi` added under build/Governance; no `src/**/*.fsi` change | no product baseline diff |
| Generated consumers | `TemplateCheck`/`GeneratedProductCheck`/`GeneratedGuidanceCheck` relocated behaviour-identically (verbatim) | byte-identical by construction |

The relocation touches only `build/**`, the launchers, `.config/dotnet-tools.json`, `tests/Governance.Tests`,
and the feature spec dir. The product runtime (`src/Scene → SkiaViewer → Elmish`, Controls, etc.) and
the product public `.fsi` surface are unchanged.

Captured: 2026-06-01T14:44:26Z
