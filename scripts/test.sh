#!/usr/bin/env bash

# =============================================================================
# test.sh
#
# Executes all test projects in the solution.
#
# Currently this script will run successfully even if no test projects
# exist yet. It becomes useful once unit tests are added to the repository.
#
# Example:
#
#   ./scripts/test.sh
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

ensure_repo_root
ensure_dotnet

log "Running unit tests..."

dotnet test "${SOLUTION_FILE}" \
    --configuration "${CONFIGURATION}" \
    --no-restore

log "Tests completed."