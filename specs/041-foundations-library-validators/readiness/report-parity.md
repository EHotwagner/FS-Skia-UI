# Golden-diff parity evidence (SC-002 / SC-005 / FR-006)

The three extracted-validator reports were captured from the **pinned pre-extraction
baseline** (HEAD `76414fb`, before any module moved — R1) and committed as the parity
oracle under `tests/Governance.Tests/fixtures/reports-golden/`. After the extraction, the
same three FAKE targets were re-run and each produced report compared byte-for-byte to its
golden fixture (`generated_at_utc` and the absolute repository root normalized to
placeholders for `target-metadata.json` only — R2).

Authoritative commands (serialized; FAKE shares `.fake` state):

```
./fake.sh build -t CapabilityCheck        # → readiness/capability-catalog.md
./fake.sh build -t TargetMetadata         # → readiness/target-metadata.json
./fake.sh build -t TargetMetadataDrift    # → readiness/target-metadata-drift.md
```

SHA-256 byte-equality (post-extraction live report vs committed golden fixture):

| Report | live SHA-256 | golden SHA-256 | diff |
|--------|--------------|----------------|------|
| `capability-catalog.md` | `14e58450…f23767` | `14e58450…f23767` | **0 bytes** |
| `target-metadata-drift.md` | `443326a9…df7e23` | `443326a9…df7e23` | **0 bytes** |
| `target-metadata.json` (repo-root + timestamp normalized, R2) | `e9d05320…2e098a9` | `e9d05320…2e098a9` | **0 bytes** |

In-suite parity coverage (`tests/Governance.Tests/ReportParityTests.fs`, runs under the
`Dev` gate, FR-008a):

- `capability-catalog.md` — rendered through `Capabilities.readCatalog` + `renderReport`
  over the real `template/capabilities.yml`; asserted byte-identical to the fixture.
- `target-metadata-drift.md` — rendered through `TargetMetadata.driftMarkdown []`;
  asserted byte-identical to the fixture (PASS case).
- `target-metadata.json` — the typed model is parsed back out of the golden fixture and
  re-rendered through `TargetMetadata.metadataJson "__GENERATED_AT_UTC__" []`; asserted
  byte-identical to the fixture (every line incl. paths). A separate test asserts the live
  `generated_at_utc` is the only non-deterministic byte and is well-formed (R2).

Bespoke-parser retirement (SC-005): `grep -n "readCapabilityCatalog\b" build.fsx` returns
only the thin edge alias `Capabilities.readCatalog`; the line-by-line YAML state machine,
`emptyCapability`, and the `trimQuotes`/`parseScalar`/`parseInlineList` wrappers are gone.

Failure class if a row drifts: `governance / report-parity`. Next action: re-run the
affected single target in isolation and diff against the committed fixture before any
product debugging (the focused per-target run is authoritative; aggregate FAKE results are
non-authoritative).
