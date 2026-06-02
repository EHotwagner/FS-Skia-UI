# Consumer work-list (R1/R2 re-verification)

The only `ProjectReference` build-consumer of the monolith at Stage-4 tip was
`tests/Package.Tests` (conditional `..\..\src\Lib\Lib.fsproj`). `tests/Lib.Tests`
was already decoupled in feature 052 (it references only `Scene` + `SkiaViewer`,
named in its `.fsproj` comment "no reference to the retiring Lib monolith"). Beyond
the one `ProjectReference`, the monolith survived only as **path-string references**
(the Stage-2 lesson: a deleted file is referenced by *path*, invisible to a
symbol grep).

## Path-string call sites cleared (each verified by the no-consumer grep)

| Site | Action |
|------|--------|
| `tests/Package.Tests/Package.Tests.fsproj:20` | drop conditional `Lib.fsproj` ref → reference `Scene` |
| `tests/Package.Tests/Tests.fs` (PackLocal + smoke) | rewrite monolith asserts → split-package pack entries; negative via parts |
| `tests/Package.Tests/SurfaceAreaTests.fs` | drop `typeof<FS.Skia.UI.ParityReport>` baseline + helper tests; retarget stable-path test to `FS.Skia.UI.Scene` |
| `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs:29` | drop the `FS.Skia.UI` packable-enumeration row |
| `tests/Governance.Tests/DependencyGovernanceTests.fs:176,177` | drop `src/Lib/Lib.fsproj`, `../Lib/Lib.fsproj` from the forbidden list |
| `tests/Governance.Tests/RuntimeOrganizationTests.fs:41` | drop `src/Lib/Library.fs` |
| `tests/Governance.Tests/PublicRecordInvariantTests.fs:9` | drop `src/Lib/Library.fsi` |
| `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs:51` | drop `"src/Lib"` |
| `tests/Governance.Tests/AgentValidationFrameworkTests.fs:491` | repoint stale `src/Lib/AgentValidation.fsi` → `src/Scene/Scene.fsi` |
| `tests/Governance.Tests/RoutingTests.fs:36,109,119` | repoint generic `src/Lib/Foo.fsi` → `src/Scene/Foo.fsi` |
| `tests/Governance.Tests/PerPackageSurfaceTests.fs:25` | "monolith excluded" negative named via parts |
| `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs:135,136,169` | broad-package negative guards named via parts |
| `tests/Governance.Tests/GeneratedProjectValidationTests.fs:209,460` | drop `src/Lib/Lib.fsproj` from forbidden-content lists |
| `tests/Controls.Tests/DiagnosticsTests.fs:14,20,25,32` | repoint diagnostic-string examples → `src/SkiaViewer/...`, `src/Input/KeyboardInput.fs` |
| `build/Governance/Front/Helpers.fs:36` | drop the `("src/Lib/Lib.fsproj","FS.Skia.UI")` packProjects entry |
| `build/Governance/Routing.fs:210-220` | replace the Stage-0 deferral comment with the wired rule (FR-007) |
| `build/Governance/PerPackageSurface.fs:29` | correct the stale monolith-exclusion comment |
| `build/Governance/GeneratedProduct.fs:179,960,1875` | drop dead `src/Lib`/`..\Lib\Lib.fsproj` forbidden entries |
| `FS-Skia-UI.sln:4` + ProjectConfiguration block | remove the `Lib` project + its config lines |
| `readiness/surface-baselines/FS.Skia.UI.txt` | `git rm` the retired aggregate baseline |
| `docs/reports/dependencies.md` | drop the monolith ref + leak note; affirm split list |

## Residue confirmation (R2)

At deletion `src/Lib` held only `Library.fs` (142 LOC), `Library.fsi` (61 LOC), and
`InternalsVisibleTo.fs` (6 LOC) — the `Parity` evidence helper + its types. **No**
`VulkanStartup`/`VulkanResources`/`KeyboardInput`/`AgentValidation` residue remained
(moved in Stages 1–2 / 052). `git ls-files src/Lib` returns nothing after the `git rm`.

failure class: GovernanceConsumerLeak. next action: none — every site cleared, full
`dotnet build FS-Skia-UI.sln` succeeds (0 errors).
