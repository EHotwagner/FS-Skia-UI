# Quickstart: Bomberman Demo Feedback Follow-ups

Run from the repository root.

## Focused Checks

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj
dotnet test tests/Scene.Tests/Scene.Tests.fsproj
dotnet test tests/Layout.Tests/Layout.Tests.fsproj
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
```

## Generated And Governance Checks

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

## Required Readiness Evidence

Implementation must produce these reviewer artifacts:

```text
specs/029-bomberman-demo-feedback/readiness/evidence-graph-invocation.md
specs/029-bomberman-demo-feedback/readiness/verify-log-cleanliness.md
specs/029-bomberman-demo-feedback/readiness/screenshot-evidence-probe.md
specs/029-bomberman-demo-feedback/readiness/generated-app-wiring.md
specs/029-bomberman-demo-feedback/readiness/scene-layout-authoring.md
```

`verify-log-cleanliness.md` must include evidence from at least three redirected `Verify` runs and the NUL-byte scan result.

`screenshot-evidence-probe.md` must show that real screenshot capture was attempted before any unsupported result, and on a supported host must link a nonblank screenshot artifact.

## Implementation Order

1. Draft public `.fsi` changes and FSI usage transcripts for any new helper.
2. Add failing semantic/governance tests.
3. Implement package or template changes.
4. Refresh generated guidance, template drift, package surface, and readiness evidence.
5. Run `EvidenceGraph`, `EvidenceAudit`, and `Verify`.
