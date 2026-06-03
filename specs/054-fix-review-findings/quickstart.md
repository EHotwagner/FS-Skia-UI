# Quickstart: Verify the Review-Findings Fixes

Reproducible commands a reviewer runs to confirm each fix. Run from repo root. Per CLAUDE.md,
run `./fake.sh build -t Route` first and run only the gates it prints; FAKE-backed targets run
**sequentially**, never concurrently.

## Pre-change snapshot (baseline evidence)

```bash
# §4.1 pin drift — these two should DIFFER before the fix
grep -oE 'FS\.Skia\.UI\.Build, [0-9][^"]*' template/base/build.fsx
grep -oE 'FS\.Skia\.UI\.Build" Version="[^"]*"' template/base/Directory.Packages.props

# §4.2 FS3261 — clean build count (≈34 across 8 files before the fix)
dotnet build build/Governance/FS.Skia.UI.Build.fsproj --no-incremental 2>&1 \
  | grep -c 'warning FS3261'

# §4.3 stray scratch — non-empty before the fix
git status --porcelain
```

## SC-001 — pin parity holds at head

```bash
script=$(grep -oE '#r "nuget: FS\.Skia\.UI\.Build, [^"]+"' template/base/build.fsx | grep -oE '[0-9][^"]+')
props=$(grep -oE 'FS\.Skia\.UI\.Build" Version="[^"]+"' template/base/Directory.Packages.props | grep -oE '[0-9][^"]+')
test "$script" = "$props" && echo "PASS: $script == $props" || echo "FAIL: $script != $props"
```

## SC-002 — the gate catches a deliberate mismatch (demonstrate live)

```bash
# 1. break it, run the gate -> expect FAIL
sed -i 's/\(FS\.Skia\.UI\.Build, \)[0-9][^"]*/\10.0.0-bad/' template/base/build.fsx
./fake.sh build -t TemplateCheck     # expect: parity assertion FAILS
# 2. restore, run again -> expect PASS
git checkout -- template/base/build.fsx
./fake.sh build -t TemplateCheck     # expect: PASS
```

## SC-003 — a simulated pin bump moves both pins together

```bash
# Follow fs-skia-template-update step 3 (now extended): one bump, both pins equal afterwards.
# (Use a throwaway version, then revert.)
old=$(grep -oE 'FS\.Skia\.UI\.Build" Version="[^"]+"' template/base/Directory.Packages.props | grep -oE '[0-9][^"]+')
# ...run the documented bump to e.g. 0.1.99-preview.1...
# then re-run SC-001 -> expect PASS without a manual second edit; then revert.
```

## SC-004 / SC-005 — zero FS3261, tests green

```bash
dotnet build build/Governance/FS.Skia.UI.Build.fsproj --no-incremental 2>&1 \
  | grep -c 'warning FS3261'          # expect: 0
grep -n 'FS3261' build/Governance/FS.Skia.UI.Build.fsproj   # expect: no WarningsNotAsErrors entry
./fake.sh build -t Dev                # governance build + Governance.Tests green
```

## SC-006 / SC-007 — clean tree, routine diff routes inner-loop

```bash
git status --porcelain               # expect: empty (after the standard flow)
git ls-files --error-unmatch specs/053-v3-monolith-retirement/readiness/package/local-packages.md 2>&1  # expect: not tracked / absent
# touch a framework-internal file only, then:
./fake.sh build -t Route             # expect: tier=inner-loop, gates=Dev (no evidence-governance escalation)
```

## Full escalated gate order (maintainer-verify path)

Because this change touches `template/**`, governance `build/**`, a governance test, and
`.gitignore`, `Route` escalates. Run sequentially:

```bash
./fake.sh build -t Route                     # confirm tier + minimal gate list, run only those
./fake.sh build -t Dev
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
```
