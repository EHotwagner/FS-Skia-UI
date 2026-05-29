# Readiness Contract Discovery

Status: complete for focused guidance implementation.

Required readiness files for this feature are listed in `tasks.md` and the
feature contracts so final audit discovery is not deferred to the end.

Discovered surfaces:

- Repository guidance: `README.md`, `docs/build.md`, `docs/testing.md`, `docs/evidence.md`
- Agent guidance: `AGENTS.md`, `CLAUDE.md`, `.agents/skills/*`, `.claude/skills/*`, `.claude/commands/*`
- Generated product guidance: `template/base/README.md`, `template/base/docs/product.md`, generated local skills
- Validation paths: `tests/Governance.Tests/SequentialFakeGuidanceTests.fs`, `build.fsx` generated guidance scan
- Readiness contracts: `contracts/guidance-contract.md`, `contracts/readiness-evidence.md`, `contracts/generated-product-guidance.md`
