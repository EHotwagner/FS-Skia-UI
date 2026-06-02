# Quickstart — Validate the `AgentValidation` relocation

This is a governance/build-tooling relocate-and-repoint. No host, scene, layout, or
rendering behaviour changes; generated products are unaffected.

## What changed

- `src/Lib/AgentValidation.fsi` + `.fs` → `build/Governance/AgentValidation.fsi` + `.fs`,
  namespace `FS.Skia.UI.AgentValidation` → `FS.Skia.UI.Build.AgentValidation`.
- `src/Lib/Lib.fsproj` no longer compiles the two files.
- `build/Governance/FS.Skia.UI.Build.fsproj` compiles them (after the `Spike` pair).
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` `open` repointed to the new
  namespace.
- `tests/Governance.Tests/Governance.Tests.fsproj` drops its `ProjectReference` to
  `src/Lib/Lib.fsproj`.
- `readiness/surface-baselines/FS.Skia.UI.txt` drops the 48 `FS.Skia.UI.AgentValidation.*`
  lines (monolith surface shrinks; no new baseline gained — `FS.Skia.UI.Build` is excluded
  from surface tooling).

## Run the gates `Route` prints (authoritative)

```sh
./fake.sh build -t Route          # confirm tier + minimal gate list for this diff
./fake.sh build -t Route --enforce
```

`Route` is expected to **escalate** (governance paths + the monolith's public `.fsi`
shrinks). Run exactly what it prints. FAKE-backed targets share `.fake` state — run them
**sequentially**, never concurrently. The expected escalated order:

```sh
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```

## Spot checks (map to Success Criteria)

```sh
# SC-001: no AgentValidation under the monolith; it compiles in the governance lib
git ls-files src/Lib/AgentValidation.*            # -> (empty)
git ls-files build/Governance/AgentValidation.*   # -> the two relocated files

# SC-004: no monolith reference from the governance tests; no stale namespace anywhere
grep -n "Lib.fsproj" tests/Governance.Tests/Governance.Tests.fsproj   # -> (empty)
grep -rn "FS.Skia.UI.AgentValidation" --include=*.fs --include=*.fsi --include=*.fsproj --include=*.fsx .  # -> (empty)

# SC-005: knownGates lives in the governance library
grep -rn "knownGates" build/Governance/AgentValidation.fs            # -> defined here
grep -rn "knownGates" src/Lib                                         # -> (empty)

# SC-007: the contract is untouched (currency vs Routing.fs preserved)
git status validation.contract.yml                                    # -> unmodified
```

## Behaviour-parity check (SC-002 / SC-003)

```sh
# The repointed governance suite is the parity oracle — same fixtures, same assertion count.
./fake.sh build -t Dev    # builds + runs Governance.Tests, incl. AgentValidationFrameworkTests

# Structural parity: the move is a near-100% rename (only the namespace line + doc-comment
# phrase differ).
git diff -M --stat        # -> AgentValidation.fs(i) shown as renamed src/Lib -> build/Governance
```

`EvidenceAudit` must return **PASS** on real, zero-synthetic evidence (SC-008).
