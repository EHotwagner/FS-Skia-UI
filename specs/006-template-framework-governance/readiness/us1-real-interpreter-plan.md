# US1 Real Interpreter Evidence Plan

US1 is operator-facing. Real evidence comes from the wrapper entry points:

1. Run `./fake.sh build -t Dev` and retain `readiness/logs/restore.txt`,
   `build.txt`, `test.txt`, and `dev-verdict.txt`.
2. Run `./fake.sh build -t PackageSurfaceCheck` and retain
   `readiness/logs/package-surface-check.txt`.
3. Run `./fake.sh build -t EvidenceGraph` and retain
   `readiness/task-graph.json`, `readiness/task-graph.md`, and
   `readiness/logs/evidence-graph.txt`.
4. Run the focused `BuildWorkflowCheck` target through `fake.sh`; this asserts
   pure `update` behavior and emitted effects without executing production
   process effects.

These commands exercise the user-reachable command surface rather than internal
helpers.
