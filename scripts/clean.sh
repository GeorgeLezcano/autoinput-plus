#!/usr/bin/env bash

# =============================================================================
#
# Cleans build outputs and removes generated artifacts.
#
# This script performs the following:
#
#   1. Runs dotnet clean on the solution
#   2. Removes the artifacts directory
#
# Useful when you want a completely fresh build environment.
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

ensure_repo_root
ensure_dotnet

log "Cleaning solution..."
dotnet clean "${SOLUTION_FILE}"

log "Removing artifacts directory..."
rm -rf "${ARTIFACTS_DIR}"

log "Clean completed."