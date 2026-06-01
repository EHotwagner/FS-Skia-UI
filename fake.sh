#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

# Feature 045: the build front-end is a dedicated compiled exe (build/Build.fsproj),
# not the FSX script runner. Arguments (build -t <name> [flags]) forward verbatim to
# Target.runOrDefaultWithArguments. The legacy tool-restore + script-runner invocation is
# gone; the working directory stays the repo root so relative paths resolve as before.
dotnet run --project build/Build.fsproj -- "$@"
