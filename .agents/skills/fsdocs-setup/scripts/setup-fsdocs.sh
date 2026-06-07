#!/usr/bin/env bash
set -euo pipefail

# setup-fsdocs.sh — Initialize FSharp.Formatting for a project
# Run from the solution or project root directory.
#
# What this script does:
#   1. Creates a dotnet tool manifest if absent
#   2. Installs fsdocs-tool as a local tool
#   3. Creates the docs/ directory structure
#   4. Verifies the installation
#
# What this script does NOT do (Claude handles these):
#   - Configure MSBuild properties (requires reading project files)
#   - Create docs content (requires project context)
#   - Modify .gitignore (may need merge with existing entries)

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# --- Color output helpers ---
info()  { printf '\033[1;34m[info]\033[0m  %s\n' "$1"; }
ok()    { printf '\033[1;32m[ok]\033[0m    %s\n' "$1"; }
warn()  { printf '\033[1;33m[warn]\033[0m  %s\n' "$1"; }
err()   { printf '\033[1;31m[error]\033[0m %s\n' "$1" >&2; }

# --- Check prerequisites ---
if ! command -v dotnet &>/dev/null; then
    err "dotnet SDK not found. Install .NET SDK 6.0+ first."
    exit 1
fi

info "dotnet SDK version: $(dotnet --version)"

# Find solution or project root
if ls ./*.sln &>/dev/null 2>&1; then
    ROOT_MARKER="$(ls ./*.sln | head -1)"
    info "Found solution file: $ROOT_MARKER"
elif ls ./*.fsproj &>/dev/null 2>&1; then
    ROOT_MARKER="$(ls ./*.fsproj | head -1)"
    info "Found project file: $ROOT_MARKER"
else
    warn "No .sln or .fsproj found in current directory."
    warn "Proceeding anyway — ensure you are in the correct directory."
fi

# --- Step 1: Create dotnet tool manifest ---
if [ ! -f ".config/dotnet-tools.json" ]; then
    info "Creating dotnet tool manifest..."
    dotnet new tool-manifest
    ok "Tool manifest created at .config/dotnet-tools.json"
else
    ok "Tool manifest already exists"
fi

# --- Step 2: Install fsdocs-tool ---
if dotnet tool list --local 2>/dev/null | grep -q "fsdocs-tool"; then
    CURRENT_VERSION=$(dotnet tool list --local 2>/dev/null | grep "fsdocs-tool" | awk '{print $2}')
    ok "fsdocs-tool already installed (version $CURRENT_VERSION)"
else
    info "Installing fsdocs-tool..."
    dotnet tool install --local fsdocs-tool
    INSTALLED_VERSION=$(dotnet tool list --local 2>/dev/null | grep "fsdocs-tool" | awk '{print $2}')
    ok "fsdocs-tool installed (version $INSTALLED_VERSION)"
fi

# --- Step 3: Create docs/ directory structure ---
if [ ! -d "docs" ]; then
    info "Creating docs/ directory..."
    mkdir -p docs
    ok "Created docs/"
else
    ok "docs/ directory already exists"
fi

# Create img/ subdirectory for logos, diagrams, etc.
if [ ! -d "docs/img" ]; then
    mkdir -p docs/img
    ok "Created docs/img/"
fi

# --- Step 4: Verify installation ---
info "Verifying fsdocs-tool installation..."
if dotnet fsdocs --help &>/dev/null; then
    ok "fsdocs-tool is working"
else
    err "fsdocs-tool verification failed. Check installation."
    exit 1
fi

echo ""
ok "FSharp.Formatting setup complete!"
echo ""
info "Next steps (handled by Claude):"
info "  1. Create docs/index.md with project-specific content"
info "  2. Configure Directory.Build.props with documentation properties"
info "  3. Add .gitignore entries for output/, .fsdocs/, tmp/"
info "  4. Run 'dotnet fsdocs build --clean' to verify"
