# FSharp.Formatting Spike

- command: `dotnet tool install fsdocs-tool --tool-path ./.tools/fsdocs && ./.tools/fsdocs/fsdocs build --projects src/Scene/Scene.fsproj src/Controls/Controls.fsproj src/SkiaViewer/SkiaViewer.fsproj --output artifacts/fsdocs-spike/036-archive-readiness-api-docs --strict`
- log path: `specs/036-archive-readiness-api-docs/readiness/logs/fsdocs-spike.txt`
- blocker: fsdocs was not adopted as a committed dependency for this feature, and no generated fsdocs sample is allowed to replace the curated `.fsi` contract without dependency governance and sample comparison.
- reason: the current source-shaped reference already proves F# spelling, record fields, union cases, parameter labels, XML documentation, package-adjacent discoverability, and no reflection or repository-source authoring fallback. fsdocs can be secondary or hybrid, but is not authoritative from this spike.
- next action: run the command in an explicit dependency-governed spike if browsable HTML/LLM output is desired, then compare committed samples against `api-reference-generator-evaluation.md`.

Result: blocked as an authoritative replacement; acceptable only as secondary or hybrid documentation until the required dimensions pass.
