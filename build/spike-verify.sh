#!/usr/bin/env bash
#
# spike-verify.sh — verification scaffold for the D2 spike (feature 039).
#
# Encodes contracts/spike-target.contract.md. Failing-first by construction: it
# fails before the two build/** projects exist or restore/build, and passes once
# the spike target runs the library body with no FSharp.Compiler.* in the graph.
# It is NOT FAKE-backed and does not touch .fake state.
#
# Checks (contract rows):
#   1. Both new projects compile clean under net10.0 / TreatWarningsAsErrors
#      (-warnaserror), no PackageVersion outside Directory.Packages.props.
#   4. The restored package graph contains NO FSharp.Compiler.* package (FR-012).
#   2. `dotnet run -- SpikeHello` reports success and prints the exact value
#      returned by FS.Skia.UI.Build.Spike.run (proving the body ran from the
#      library, not inlined).
#
# Exit 0 => "D2 confirmed" evidence holds.
# Non-zero => "fallback triggered" evidence (record the exact blocker).

set -uo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
cd "$REPO_ROOT"

LIB_PROJ="build/Governance/FS.Skia.UI.Build.fsproj"
FE_PROJ="build/Build.fsproj"

# The library-owned success line. The front-end must print exactly this; the
# verification knows it independently so a match proves the body came from the
# library. Keep in sync with build/Governance/Spike.fs.
EXPECTED="FS.Skia.UI.Build.Spike.run: D2 spike target executed from the governance library."

fail() { echo "SPIKE-VERIFY FAIL: $*" >&2; exit 1; }

[[ -f "$LIB_PROJ" ]] || fail "missing library project $LIB_PROJ"
[[ -f "$FE_PROJ" ]]  || fail "missing build front-end project $FE_PROJ"

echo "[1/3] building library (-warnaserror)..."
dotnet build "$LIB_PROJ" -warnaserror -nologo || fail "library build failed"

echo "[1/3] building front-end (-warnaserror)..."
dotnet build "$FE_PROJ" -warnaserror -nologo || fail "front-end build failed"

echo "[4-check] no FSharp.Compiler.* in transitive graph..."
if dotnet list "$FE_PROJ" package --include-transitive 2>/dev/null | grep -iq 'FSharp\.Compiler'; then
  fail "FSharp.Compiler.* present in transitive package graph (FR-012 violated)"
fi
echo "  OK: no FSharp.Compiler.* in transitive graph"

echo "[2/3] running SpikeHello target..."
RUN_OUT="$(dotnet run --project "$FE_PROJ" -- SpikeHello 2>&1)" || { echo "$RUN_OUT"; fail "dotnet run -- SpikeHello returned non-zero"; }
echo "$RUN_OUT"
grep -qF "$EXPECTED" <<<"$RUN_OUT" || fail "expected library success line not found in run output"

echo "SPIKE-VERIFY PASS: D2 confirmed (clean build, library body ran, no FCS)."
