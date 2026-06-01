# SC-004 — typed selector Governance.Tests

`tests/Governance.Tests/RoutingTests.fs` and `ContractViewTests.fs` call the real
compiled `Routing.select` / `selectForFeature` / `unmetArtifacts` /
`enforceDiagnostic` and `ContractView.render` / `currencyDrift` functions over
**literal `Diff` values** and assert the **typed `Selection`** (tier + gate list)
— not string/IO scraping. They failed to compile before `Routing.fs` /
`ContractView.fs` existed (Principle I, failing-first).

RoutingTests.fs cases (≥6 required; 14 provided):

1. `src/Scene/*.fs` only → `InnerLoop` / `[Dev]` (no surface check)  [SC-001]
2. empty diff → `InnerLoop` / `[Dev]` (deterministic default)        [SC-001 edge]
3. `src/**/*.fsi` → `FocusedAuthority`, gates incl. `PackageSurfaceCheck` [SC-002/F1]
4. `template/base/**` → escalates, gates incl. `TemplateCheck`+`GeneratedProductCheck` [SC-002]
5. `.specify/templates/**` → escalates (generated-guidance)          [SC-002]
6. mixed `src/Scene/*.fs` + `template/base/**` → highest tier wins   [edge/FR-003]
7. unknown `weird/path.txt` → `MaintainerVerify` / `[Verify]` (default-deny) [SC-002 edge]
8. `ConsumerAgent` floor raises an inner-loop diff to `FocusedAuthority` [FR-002]
9. broadened coverage: `template/capabilities.yml` + `.specify/extensions.yml` escalate [F2]
10. dogfood `selectForFeature ... "042" ...` → `fullPipelineGates`/`MaintainerVerify` [SC-005]
11. non-dogfood feature id leaves the diff's selection intact
12. `unmetArtifacts` reports the missing artifact when absent / clears when present [SC-003]
13. `enforceDiagnostic` names the missing artifact and the requiring tier [SC-003]

ContractViewTests.fs cases:
- `currencyDrift (render …) … = None` (fresh contract is current)    [SC-007]
- `currencyDrift <hand-mutated> … = Some _` (stale detected)         [SC-007]
- `render` names the gate targets its consumers reference + dogfood ids

Authoritative command: `dotnet test tests/Governance.Tests` (non-FAKE, safe).
Result: **Passed! — Failed: 0, Passed: 313, Total: 313** (311 pre-existing + the
2 new feature-042 test lists wired before `Program.fs`).
Artifact path: this file. Failure class: governance. Next action: none — green.
