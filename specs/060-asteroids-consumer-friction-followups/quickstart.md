# Quickstart: Verifying the Consumer Friction Follow-ups

End-to-end walkthrough that produces the readiness evidence for FR-001/FR-003/FR-005.
Run FAKE-backed targets **sequentially** (shared `.fake` state).

## 0. Route the change

```bash
./fake.sh build -t Route
```

Expect the **maintainer-verify** (escalated) tier and a gate list including `Dev`,
`GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the new
api-surface / `SkillContractPathCheck` / `TemplateUpdateSkillPackageCheck` gates,
`SkillSyncCheck`, `TargetMetadataDrift`, `SkillQualityCheck`, `EvidenceGraph`,
`EvidenceAudit`. Run **only** the gates Route prints.

## 1. Regenerate single-source artifacts (after skill / api-surface edits)

```bash
./fake.sh build -t RefreshSurfaceBaselines   # regen .claude from .agents + api-surface tree
git diff --stat .claude template/base/docs/api-surface
```

## 2. Pack + install the template (FR-002)

Follow `fs-skia-template-update` (now corrected): bump
`.template.package/FS.Skia.UI.Template.fsproj` if needed, then:

```bash
./fake.sh build -t TemplatePack
v=0.1.63-preview.1
for p in Build Scene SkiaViewer Elmish KeyboardInput Input Layout Controls Controls.Elmish Testing SkillSupport; do
  test -f "$HOME/.local/share/nuget-local/FS.Skia.UI.$p.$v.nupkg" && echo "OK  $p" || echo "MISS $p"
done
dotnet new uninstall FS.Skia.UI.Template
dotnet new install artifacts/templates/FS.Skia.UI.Template.<version>.nupkg
```

> Note: no `FS.Skia.UI.$v.nupkg` (bare Lib) line — 053 deleted it. `Input` and
> `SkillSupport` are now in the loop.

## 3. Generate a project and prove FR-001 (feature resolution)

```bash
rm -rf /tmp/asteroids-friction-check
dotnet new fs-skia-ui --name FrictionCheck --output /tmp/asteroids-friction-check \
  --allow-scripts yes --skipGitInit true
cd /tmp/asteroids-friction-check
# active multi-task feature resolves + echoes:
./fake.sh build -t EvidenceGraph   # expect: feature-directory=...  tasks=<N>  (N>1)
# loud failure path:
SPECKIT_FEATURE_DIR=/no/such/dir ./fake.sh build -t EvidenceGraph  # expect loud fail naming the path
```

Capture both transcripts into
`specs/060-asteroids-consumer-friction-followups/readiness/generated-project/feature-resolution.log`.

## 4. Prove FR-003 (api-surface present in generated project)

```bash
ls /tmp/asteroids-friction-check/docs/api-surface/
diff <(sed -e 's/\s*$//' src/Scene/Scene.fsi) \
     /tmp/asteroids-friction-check/docs/api-surface/Scene/Scene.fsi && echo "MATCH"
```

Confirm each product-skill's named path exists. Log →
`readiness/generated-project/api-surface.log`.

## 5. Prove FR-005 (test split survives model swap)

In the generated project, replace `src/Product/Model.fs`/`Program.fs` with a trivial
alternate model, then:

```bash
dotnet test tests/.../*.Tests.fsproj --filter "FullyQualifiedName~Governance"  # still green
# BehaviorTests fail to compile/needs rewrite — expected
```

Log → `readiness/generated-project/test-split.log`.

## 6. Run the routed gates (sequentially) + audit

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit   # expect verdict=PASS for specs/060-...
```

## 7. Confirm currency gates after skill edits

```bash
./fake.sh build -t SkillSyncCheck
./fake.sh build -t TargetMetadataDrift
./fake.sh build -t SkillQualityCheck
```

All green → SC-001…SC-007 satisfied; readiness folder complete.
