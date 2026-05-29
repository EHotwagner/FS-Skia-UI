# Evidence Vocabulary

- Real evidence: repository or generated product validation that reads the actual filesystem artifacts.
- Synthetic evidence: deliberately malformed or drifted fixture input. No successful Claude readiness task is marked complete using synthetic-only evidence.
- Drift report: failure output naming `scope`, `sourceId`, `workflowId`, `expectedPath`, `actualPath`, `differenceSummary`, and `repairAction`.
