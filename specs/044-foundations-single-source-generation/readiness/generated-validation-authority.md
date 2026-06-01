# Generated Validation Authority — Feature 044

Every replaced drift-check is now a **generation-currency** check whose authoritative
command is `./fake.sh build -t RefreshSurfaceBaselines` (the single regeneration entry
point). The currency gates are reused, not proliferated:

| Generated/derived artifact | Canonical source | Currency gate | Failure class | Next action |
|----------------------------|------------------|---------------|---------------|-------------|
| `.claude/skills/**` (+ `GENERATED.md` manifest) | `.agents/skills/**` | `SkillSyncCheck` | governance | `./fake.sh build -t RefreshSurfaceBaselines` |
| `.specify/templates/{plan,tasks}-template.md` BEGIN/END GENERATED regions | `.specify/memory/constitution.md` | `TargetMetadataDrift` | governance | `./fake.sh build -t RefreshSurfaceBaselines` |
| `tasks.md` `[skillist: …]` view (active feature) | `tasks.deps.yml` `skillist:` | `EvidenceAudit` (active-feature merge-gate) | governance | `./fake.sh build -t RefreshSurfaceBaselines` |
| `validation.contract.yml` | `Routing.fs` | `TargetMetadataDrift` | governance | `./fake.sh build -t RefreshSurfaceBaselines` |

Authoritative command: `./fake.sh build -t Route` (prints the escalated gate set).
Artifact path: `specs/044-foundations-single-source-generation/readiness/`.
