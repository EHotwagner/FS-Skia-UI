# Template validation (FR-009 / SC-008)

**Verdict:** PASS.

`Route` escalated this consumer-contract change to the agent-ready tier whose gate set includes
`TemplateDrift` and `GeneratedGuidanceCheck`. Both are green:

- `./fake.sh build -t TemplateDrift` -> `Status: Ok`
- `./fake.sh build -t GeneratedGuidanceCheck` -> `Status: Ok`

No `template.json`, fragment, capability, or package-policy authoring changed in this stage (the host
move is internal to the `FS.Skia.UI.SkiaViewer` package the default `app` profile already consumes by
package). The default `app` still restores/builds, and its resolved transitive graph no longer pulls
`FS.Skia.UI` (the monolith) because the `SkiaViewer -> FS.Skia.UI` edge was removed — see
`leak-proof.md`. Template package pins are unchanged in this stage (versions bump at merge, not here).
