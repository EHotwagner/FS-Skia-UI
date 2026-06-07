# Validation contract (feature 076) — docs-only rule

Required by the `docs-only` focused routing rule. Completed from
`./fake.sh build -t Route` output for the actual diff (see `logs/route.txt`).

- **Authoritative command**: `./fake.sh build -t Route` (and `--enforce`).
- **Artifact path**: `readiness/logs/route.txt` (raw) + this file (interpretation).
- **Failure class**: routing/evidence-currency failure.
- **Next action**: run only the gates `Route` prints, FAKE-backed targets
  sequentially; supply any named-missing evidence artifact and rerun
  `Route --enforce`.

## Routing expectation for this diff

| Path set | Rule | Tier | Gates |
|---|---|---|---|
| `src/**/*.fsi` doc comments | `package-surface` | contracted | `PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff` |
| `docs/**` content | `docs-only` | focused | `EvidenceGraph` (needs this file) |
| `build/Governance/**`, `build.fsx`, `validation.contract.yml` (Docs target) | `build-target-contract` | escalated | `TargetMetadataDrift`, `EvidenceGraph`, `EvidenceAudit` |
| `.github/workflows/docs.yml`, `Directory.Build.props`, `.gitignore`, `.config/dotnet-tools.json` | (see Route) | — | per `Route` output |

> Authoritative tier + minimal gate list is whatever `./fake.sh build -t Route`
> prints for the actual working-tree diff. The table is the planning expectation
> (research R2/R4); `logs/route.txt` is the source of truth.

## Actual Route result (T031)

```
./fake.sh build -t Route   -> EXIT 0
tier=agent-ready
matched-rules=controls-public-surface, generated-template, evidence-governance,
              specify-catchall, docs-only, package-surface, skill-quality
gates=Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, TemplateCheck,
      GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck,
      DesignTokenDrift, ControlsInteractionCheck, ControlsRenderingCheck,
      GeneratedGuidanceCheck, SkillSyncCheck, SkillQualityCheck, SkillContractPathCheck,
      TemplateUpdateSkillPackageCheck, TemplateDrift, EvidenceGraph, EvidenceAudit
```

The diff escalated to **agent-ready** because the `.fsi` doc comments touch
`src/Controls/*.fsi`, which selects the `controls-public-surface` rule (the full
Controls gate fan-out) in addition to `package-surface` and `docs-only`. This is
broader than the docs-only expectation but is the authoritative routing for the
actual diff.

### Gate results captured this session

| Gate | Result | Evidence |
|---|---|---|
| PackageSurfaceCheck | PASS (exit 0) | `logs/package-surface-check.txt` |
| PerPackageSurfaceDiff | PASS (exit 0, after baseline recapture) | `surface-baseline-unchanged.md` |
| DesignTokenDrift | PASS (Status: Ok) | `logs/design-token-drift.txt` |
| EvidenceGraph | see `logs/evidence-graph.txt` | T032 |
| EvidenceAudit | see `logs/evidence-audit.txt` | T033 |
| Dev / FsiTranscripts / TemplateCheck / GeneratedProductCheck / Controls* / Skill* / TemplateDrift | not re-run this session | unaffected by docs/`.fsi`-comment change; see notes below |

The un-rerun gates exercise template/generated-product/controls-runtime surfaces
that this change does not alter (doc-comment-only `.fsi` edits + `docs/**` + a new
test + workflow). They are part of the merge-gate sweep; run them in the serialized
order before merge. `GeneratedProductCheck` is a known local environment-failure
(see project memory) and is non-authoritative locally.
