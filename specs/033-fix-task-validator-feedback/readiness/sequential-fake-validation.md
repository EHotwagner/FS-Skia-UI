# Sequential FAKE Validation

FAKE-backed targets were run sequentially because repository `.fake` state is shared.

Results:

| Target | Result | Notes |
|--------|--------|-------|
| `Dev` | PASS | Initial run exposed stale Claude readiness mirrors; rerun passed after dynamic active-plan check and Claude skill mirror were fixed. |
| `GeneratedGuidanceCheck` | PASS | Guidance scans passed. |
| `TemplateCheck` | PASS | Template pack/install/instantiate/smoke path passed. |
| `GeneratedProductCheck` | PASS | First run hit a transient `libdecor-gtk.so` test host crash after the same test project had passed earlier in the log; sequential rerun passed. |
| `EvidenceGraph` | PASS | Graph-only label evidence confirmed. |
| `EvidenceAudit` | PASS | Initial run reported missing readiness-contract files; rerun passed after `governance-risk-levels.md` and `runtime-limitations.md` were added. |

Focused command also passed:

`dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Task validator feedback follow-ups|V3 local skill validation|Synthetic error evidence governance|Generated project validation contract" -m:1 --disable-build-servers`

Focused result: 50 passed, 0 failed.
