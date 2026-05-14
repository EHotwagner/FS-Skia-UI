# Clean Copy Verify Evidence

Command:

```bash
./fake.sh build -t Verify
```

Working directory: `/tmp/fs-skia-ui-verify`

Setup: temporary copy of the current working tree with `.git`, `.fake`, `bin`,
`obj`, and generated v1 readiness logs/transcripts/smoke/package/task-graph
outputs excluded before the run.

Result: PASS. `Verify` produced build/test/package logs, FSI transcripts,
sample smoke output, task graph output, evidence audit output, and the final
verify verdict in the temporary readiness directory.

Runtime assumptions:

- .NET SDK: `10.0.300`
- Host runtime: `10.0.8`
- OS platform: Linux x64
