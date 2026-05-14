#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

dotnet tool restore >/dev/null
FAKE_ALLOW_NO_DEPENDENCIES=true dotnet fake "$@"
