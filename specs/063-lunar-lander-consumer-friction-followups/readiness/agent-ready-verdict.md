# AgentReady Verdict

Status: `degraded`

- authority: `focused-authoritative`
- tier: `Tier 1` — driven by **FR-010** (new public `FS.Skia.UI.SkillSupport.Wrap` `.fsi`
  surface + a new per-package surface baseline line) and **FR-003** (a new governance
  target + `knownGates` entry). The renderer fix (FR-001/002) is Tier-2 internal (the
  shared painter is **non-public** — no SkiaViewer surface change) but is
  consumer-observable evidence output, so consumer-contract change-sets escalate the rest.
- affected-layers: `src/SkiaViewer/**` (new non-public shared `SceneRenderer` + delegation),
  `src/SkillSupport/**` (new `Wrap` module), `build/Governance/**` (new `SymbolCrossCheck`
  target + readiness-diagnostic relabel), Spec Kit phase skills (`.agents/skills/**` →
  generated `.claude/**`), generated docs (`template/base/docs/scaffold-map.md`).
- public-API impact: **FR-010 only** — new curated `Wrap.fsi` + per-package baseline. The
  shared `SceneRenderer` module is non-public so SkiaViewer's surface baseline is unchanged.
- elmish-mvu: `N/A` — no framework `Model`/`Msg`/`Effect`/`init`/`update`/interpreter is
  added or changed. The renderer is a pure draw walk; `wrapDeltaX` is a pure helper a
  consumer threads through *their* `update`.
- required-gates: `EvidenceGraph`, `EvidenceAudit` (plus the Route-escalated gates per
  change-set: `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`,
  `SkillSyncCheck`, `SkillQualityCheck`, `TargetMetadataDrift`, and — FR-010 —
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff`).
- required-evidence: `target-metadata.md`, `agent-ready-verdict.md`,
  `skill-loading-evidence.md`, `aggregate-hang-diagnostics.md`, the before/after image
  capture (`renderer-image-evidence.md`), `symbol-cross-check.md`, the `Wrap` unit-test output
  + the updated per-package baseline, the FR-008 template-scan record.
- next-command: `./fake.sh build -t EvidenceAudit`
- diagnostic: AgentReady reports `degraded` because the aggregate `Verify`/`Ci` umbrella
  cannot bootstrap the `dotnet-fake` global tool in this sandbox (see
  `runtime-limitations.md`). Every constituent gate Route prints is run individually and
  sequentially; the authoritative merge gate `EvidenceAudit` is the verdict. This is a
  non-authoritative aggregate limitation, not a gate failure.
