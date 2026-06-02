# Structural parity — the relocation is a near-100% rename (SC-003)

`git diff -M` detects both files as **renames** from `src/Lib` to `build/Governance`; the only
content edits are the single `namespace` line in each file and the doc-comment phrase in the `.fsi`.

## Rename evidence

```
$ git diff -M40 --cached --stat -- build/Governance/AgentValidation.fs(i) src/Lib/AgentValidation.fs(i)
 {src/Lib => build/Governance}/AgentValidation.fs  |  2 +-
 {src/Lib => build/Governance}/AgentValidation.fsi | 88 +++++++++++------------
 2 files changed, 45 insertions(+), 45 deletions(-)
```

- **`AgentValidation.fs`** — 1 changed line: `namespace FS.Skia.UI.AgentValidation` →
  `namespace FS.Skia.UI.Build.AgentValidation`. No `val`/`type`/field/case/body change.
- **`AgentValidation.fsi`** — 45 changed lines = the `namespace` line + 44 identical doc-comment
  rewrites (`"…exposed by this FS.Skia.UI package."` → `"…exposed by the FS.Skia.UI.Build governance
  library."`). Every type, `val`, record field, and union case is byte-identical (D5). No identifier
  added, removed, or retyped.

## Behavioural parity (the real oracle)

The repointed `AgentValidationFrameworkTests` suite (same fixtures, **same assertion count**) builds
and passes against the relocated module under `./fake.sh build -t Dev` — 347 tests, 347 passed. It
exercises the contract-parse accept/reject diagnostics, the `knownGates` allowlist, the
`ValidationSelection` MVU transitions, and `AgentVerdict` (de)serialization, so the relocated parser
derives an **identical** `knownGates` set and **identical** accept/reject diagnostics versus the
pre-move module (SC-003). Evidence: `readiness/logs/dev.log`.
