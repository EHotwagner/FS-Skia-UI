# Validation contract & escalated gate set (T022)

`Route` escalates this change (it touches governance/build paths). As a V3-programme
dogfood-adjacent governance/build change it runs the full serialized gate set **plus**
`PerPackageSurfaceDiff`, sequentially (FAKE shares `.fake` state — never concurrent).

## Contract currency
`validation.contract.yml` is generated from `Routing.fs` (`ContractView.render`) and is
**unchanged** by this feature: no Routing rule was added (see the runtime-coupling finding
in `runtime-untouched.md`), so `git diff --stat validation.contract.yml` is empty and the
`TargetMetadataDrift` currency check (`ContractView.currencyDrift … = None`) holds.

## Serialized gate results (authoritative; aggregate FAKE results non-authoritative)

| Gate | Result |
|------|--------|
| `Dev` (Restore → Build → SampleContractSmoke → Test → SkillSyncCheck) | **Ok** |
| `PerPackageSurfaceDiff` | **Ok** — zero drift across the 8 packages (SC-004) |
| `GeneratedGuidanceCheck` | **Ok** |
| `TemplateCheck` | **Ok** (pack → install source/package → instantiate → smoke) |
| `GeneratedProductCheck` | **Ok** (on rerun) — first aggregate run hit the known `SkiaViewer.Tests` headless libdecor-gtk test-host crash (`Failed to load plugin 'libdecor-gtk.so'`); per the governance contract that aggregate failure is **non-authoritative**, and the focused rerun `dotnet test tests/SkiaViewer.Tests` is **authoritative: 48/48 passed**. The retry of the full target then passed. |
| `EvidenceGraph` | **ok** — DAG acyclic, no dangling refs, no `[S*]` (T023) |
| `EvidenceAudit` | **PASS** — 0 unaccepted-synthetic, 0 auto-synthetic, 0 late-seh, 0 diff-scan, 0 readiness-contract blockers (T024, SC-008) |

The existing aggregate `PackageSurfaceCheck` remains green and unchanged (FR-011). Logs
under `specs/048-v3-retirement-baseline/readiness/logs/` (gitignored, regenerable).
