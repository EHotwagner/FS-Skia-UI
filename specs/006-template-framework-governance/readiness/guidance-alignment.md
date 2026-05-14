# Guidance Alignment Review

Reviewed `.agents/skills/speckit-tasks/SKILL.md`.

Decision: no direct skill change is needed for v1. The skill generates from the
project task template, and v1 updates
`.specify/presets/fsharp-opinionated/templates/tasks-template.md` so future
generated tasks reference canonical targets such as `Dev`, `Verify`,
`PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `EvidenceGraph`,
and `EvidenceAudit`.

Repository automation alignment is handled in
`.specify/workflows/speckit/workflow.yml` by adding `./fake.sh build -t Ci`.
