#!/usr/bin/env bash
set -euo pipefail

# fsdocs-build.sh — Build FSharp.Formatting documentation with error handling
# Run from the solution or project root directory.
#
# Usage:
#   ./fsdocs-build.sh              # Standard build
#   ./fsdocs-build.sh --eval       # Build with script evaluation
#   ./fsdocs-build.sh --clean      # Clean build (removes previous output)
#   ./fsdocs-build.sh --watch      # Start watch mode for live preview
#   ./fsdocs-build.sh --noapidocs  # Skip API doc generation (faster)
#   ./fsdocs-build.sh --strict     # Treat warnings as errors
#
# Flags can be combined: ./fsdocs-build.sh --clean --eval --strict

# --- Color output helpers ---
info()  { printf '\033[1;34m[info]\033[0m  %s\n' "$1"; }
ok()    { printf '\033[1;32m[ok]\033[0m    %s\n' "$1"; }
warn()  { printf '\033[1;33m[warn]\033[0m  %s\n' "$1"; }
err()   { printf '\033[1;31m[error]\033[0m %s\n' "$1" >&2; }

# --- Parse flags ---
FSDOCS_FLAGS=()
WATCH_MODE=false
DO_CLEAN=false

for arg in "$@"; do
    case "$arg" in
        --watch)
            WATCH_MODE=true
            ;;
        --clean)
            DO_CLEAN=true
            FSDOCS_FLAGS+=("--clean")
            ;;
        --eval|--strict|--noapidocs|--nonpublic|--nodefaultcontent)
            FSDOCS_FLAGS+=("$arg")
            ;;
        --port|--input|--output|--properties)
            # These take a value — pass through as-is
            FSDOCS_FLAGS+=("$arg")
            ;;
        *)
            # Pass unknown args through (may be values for preceding flags)
            FSDOCS_FLAGS+=("$arg")
            ;;
    esac
done

# --- Check prerequisites ---
if ! command -v dotnet &>/dev/null; then
    err "dotnet SDK not found."
    exit 1
fi

if ! dotnet tool list --local 2>/dev/null | grep -q "fsdocs-tool"; then
    err "fsdocs-tool not installed. Run 'dotnet tool restore' or use the doc-setup skill."
    exit 1
fi

# Restore tools in case of fresh clone
info "Restoring dotnet tools..."
dotnet tool restore 2>/dev/null || true

# --- Build projects in Release mode ---
info "Building projects in Release mode..."
if dotnet build -c Release; then
    ok "Release build succeeded"
else
    err "Release build failed. Fix compilation errors before building docs."
    exit 1
fi

# --- Run fsdocs ---
if [ "$WATCH_MODE" = true ]; then
    info "Starting fsdocs watch mode..."
    info "Open http://localhost:8901 in your browser"
    info "Press Ctrl+C to stop"
    echo ""
    dotnet fsdocs watch "${FSDOCS_FLAGS[@]}"
else
    info "Running fsdocs build..."
    if [ ${#FSDOCS_FLAGS[@]} -gt 0 ]; then
        info "Flags: ${FSDOCS_FLAGS[*]}"
    fi

    BUILD_OUTPUT=$(dotnet fsdocs build "${FSDOCS_FLAGS[@]}" 2>&1) || {
        err "fsdocs build failed:"
        echo "$BUILD_OUTPUT"
        exit 1
    }

    echo "$BUILD_OUTPUT"

    # --- Report results ---
    echo ""
    ok "Documentation build complete!"

    if [ -d "output" ]; then
        PAGE_COUNT=$(find output -name "*.html" 2>/dev/null | wc -l)
        info "Generated $PAGE_COUNT HTML pages in output/"
    else
        warn "No output/ directory found — check build output for errors"
    fi

    # Check for warnings in build output
    WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -ci "warning" || true)
    if [ "$WARNING_COUNT" -gt 0 ]; then
        warn "$WARNING_COUNT warning(s) in build output — review above"
    fi
fi
