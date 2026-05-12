#!/usr/bin/env bash
set -euo pipefail

output="${1:-specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt}"
mkdir -p "$(dirname "$output")"

if [[ -z "${XDG_RUNTIME_DIR:-}" ]]; then
  export XDG_RUNTIME_DIR="/tmp/fs-skia-ui-runtime-$(id -u)"
  mkdir -p "$XDG_RUNTIME_DIR"
  chmod 700 "$XDG_RUNTIME_DIR"
fi

dotnet run --project samples/BasicViewer/BasicViewer.fsproj -- --smoke --first-frame 2>&1 | tee "$output"
