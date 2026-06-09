# Quickstart: validating Governance Precision Hardening

Maintainer walkthrough to verify the three tiers. FAKE-backed commands share `.fake` state — run them
**sequentially**.

## 0. Route the change first

```bash
./fake.sh build -t Route            # authoritative tier + minimal gate list for THIS diff
./fake.sh build -t Route --enforce  # additionally fails on a missing required evidence artifact
```

Tier 1/3 (build/Governance `.fs`/`.fsi`, byte-identical contract) and Tier 2 (which also touches
`validation.contract.yml`) route differently — run `Route` and run only what it prints.

## 1. Tier 1 — typed gate identity (FR-001/002/003/004/005)

- **Compile-error proof (SC-001)**: on a throwaway branch add a dummy `Target` case without a
  `focusedGateContract` arm → the build **fails to compile**. Revert.
- **No degraded routable gate (SC-003)**: governance test enumerates `Targets.routableGates` and asserts
  each resolves to a non-`VerificationDegraded` contract.
- **Derived lists equal prior literals (SC-002)**: tests assert
  `routableGates |> List.map name` set-equals the old `knownGates` and
  `productCheckGates |> List.map name` equals the old `ProductChecksRun` in order.
- **Byte-identity**: `target-metadata.json` and `validation.contract.yml` unchanged.

## 2. Tier 2 — routing precision + split (FR-006/007/008/009/010)

Doc-only relaxation (SC-004):

```bash
# stage a doc-only edit, then:
echo x >> template/base/README.md   # or any template/**/*.md
./fake.sh build -t Route            # gates exclude GeneratedProductCheck / TemplateCheck

# a real source/contract edit still routes to the full set:
#   touch a src/Controls/**/*.fsi  ->  Route lists the heavy controls/package-surface gates
```

Capture `route-before.txt` / `route-after-doconly.txt` / `route-after-source.txt`.

Split (SC-005):

```bash
./fake.sh build -t GeneratedProductStructure     # cheap: generate + structural scan + file-lists; fails fast
./fake.sh build -t GeneratedConsumerValidation    # expensive: consumer restore/build/Verify (depends on structure)
./fake.sh build -t GeneratedProductCheck          # umbrella: SAME evidence + verdict as before the split
```

`validation.contract.yml` is **intentionally regenerated**; `TargetMetadataDrift` must pass:

```bash
./fake.sh build -t TargetMetadataDrift
```

## 3. Tier 3 — code health (FR-011/012/013)

Behavior-preserving — byte-identical artifacts vs. the captured baseline:

```bash
diff -r readiness/behavior-preserving-baseline/ <fresh run artifacts>   # expect zero diffs
```

The five file-list reports + `GeneratedProductValidationPath` + all governance goldens are unchanged; no
`.fsi` / `validation.contract.yml` change.

## 4. Escalated six-target order (sequential)

```bash
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

Plus currency: `./fake.sh build -t TargetMetadataDrift` and `./fake.sh build -t SkillSyncCheck` clean.

## 5. Independent shippability (SC-007)

Confirm each tier’s slice passes its routed gates with the other two absent — Tier 1 byte-identical
contract, Tier 2 intentional contract diff + doc-only relaxation, Tier 3 byte-identical artifacts.
