#!/usr/bin/env bash

# =============================================================================
#
# Restores dependencies and builds the entire solution.
#
# This script performs the following steps:
#
#   1. Restores NuGet packages
#   2. Builds the solution
#
# It is intended for local development or CI usage.
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

ensure_repo_root
ensure_dotnet

log "Restoring NuGet packages..."
dotnet restore "${SOLUTION_FILE}"

log "Building solution (${CONFIGURATION})..."
dotnet build "${SOLUTION_FILE}" \
    --configuration "${CONFIGURATION}" \
    --no-restore

log "Build completed successfully."