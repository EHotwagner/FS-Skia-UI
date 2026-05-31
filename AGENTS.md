<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan:
specs/040-foundations-capability-skills/plan.md
<!-- SPECKIT END -->

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Agents may
parallelize safe non-FAKE file reads and checks, but must run FAKE-backed tests
and FAKE targets sequentially when more than one is needed.

Use a deterministic FAKE-backed order, for example:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

If a FAKE-backed failure looks race-like or the concurrent FAKE context is
unknown, rerun the affected FAKE-backed commands sequentially before product
debugging.
