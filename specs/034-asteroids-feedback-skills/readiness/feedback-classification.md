# Feedback Classification

command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter Asteroids`
scanned files: `.agents/skills/fs-skia-layout-evidence/SKILL.md`, `template/base/docs/product.md`, `template/base/tests/Product.Tests/Tests.fs`
observed: framework runtime, generated template workflow, documentation discoverability, and consumer authoring owner categories are present.
missing: none.
failure class: FeedbackClassification.
next action: route each feedback finding to the bounded owner category and record deferred scope when runtime API expansion is out of scope.

| Finding | Owner category | Source observation | Deferred scope | Bounded next action |
|---------|----------------|--------------------|----------------|---------------------|
| Persistent window blocks automated flow | framework runtime | persistent-window blocking | auto-close API shape deferred | classify as blocking warning and require explicit smoke command |
| Desktop display unavailable | framework runtime | display/session availability | renderer fallback deferred | classify as benign warning when environment-only |
| Auto-close smoke needed | generated template workflow | auto-close smoke | runtime auto-close launch API deferred | keep explicit evidence command |
| Screenshot fallback image | generated template workflow | fallback classification | screenshot internals deferred | reject as visual proof |
| Missing skill hints | documentation discoverability | skill assignment absent | none | add local skill guidance |
| Name collision in generated examples | consumer authoring | name-collision guidance | none | document product-owned names |
| Compile warning from host session | consumer authoring | benign warning, blocking warning, deferred warning | none | classify warning outcome explicitly |
