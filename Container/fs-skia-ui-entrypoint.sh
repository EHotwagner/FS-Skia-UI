#!/usr/bin/env bash
set -euo pipefail

if [[ -z "${XDG_RUNTIME_DIR:-}" ]]; then
    export XDG_RUNTIME_DIR="/tmp/runtime-$(id -u)"
    mkdir -p "$XDG_RUNTIME_DIR"
    chmod 700 "$XDG_RUNTIME_DIR"
fi

mkdir -p "$HOME/.local/share/nuget-local" "$HOME/projects" /workspace

if [[ -n "${GIT_USER_NAME:-}" ]]; then
    git config --global user.name "$GIT_USER_NAME"
fi

if [[ -n "${GIT_USER_EMAIL:-}" ]]; then
    git config --global user.email "$GIT_USER_EMAIL"
fi

if ! dotnet nuget list source | grep -q 'local-feed'; then
    dotnet nuget add source "$HOME/.local/share/nuget-local" --name local-feed >/dev/null
fi

exec "$@"
