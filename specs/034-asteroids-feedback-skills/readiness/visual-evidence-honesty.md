# Visual Evidence Honesty

command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter Asteroids`
scanned files: `.agents/skills/fs-skia-layout-evidence/SKILL.md`, `template/base/docs/product.md`, `template/base/tests/Product.Tests/Tests.fs`
observed: accepted proof requires decodable image, image dimensions, non-trivial content, renderer mode, fallback classification, and unsupported reason fields.
missing: none.
failure class: VisualEvidenceHonesty.
next action: reject unsupported or incomplete visual proof and rerun the authoritative visual evidence command.

Accepted proof examples:

| Proof class | Required facts | Disposition |
|-------------|----------------|-------------|
| screenshot proof | decodable image, image dimensions, non-trivial content, renderer mode | accepted |
| rasterized scene proof | decodable image, image dimensions, non-trivial content, renderer mode | accepted |
| layout readability proof | HUD bounds, gameplay bounds, overlap diagnostics | accepted as layout proof only |

Rejected proof examples:

| Proof class | Reason |
|-------------|--------|
| metadata-only reports | metadata-only reports do not satisfy visual proof |
| 1x1 fallback images | 1x1 fallback images do not satisfy visual proof |
| layout-only bounds claims | layout-only bounds claims do not satisfy visual proof |
| ASCII screenshot reports | ASCII screenshot reports do not satisfy visual proof |
| fallback PNG substitution | fallback classification must remain explicit and unsupported when it lacks real image content |
