#!/usr/bin/env bash

# =============================================================================
#
# Cleans build outputs and removes generated artifacts.
#
# This script performs the following:
#
#   1. Runs dotnet clean on the solution
#   2. Optionally removes the artifacts directory
#
# Default behavior:
#   - Cleans the solution only
#
# Usage:
#   ./scripts/clean.sh
#   ./scripts/clean.sh --artifacts
#   ./scripts/clean.sh --configuration Debug
#
# Options:
#   -a, --artifacts         Also remove the artifacts directory
#   -c, --configuration     Clean configuration
#   -h, --help              Show help
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

REMOVE_ARTIFACTS=false

print_usage() {
    cat <<EOF
Usage: ./scripts/clean.sh [options]

Clean the solution and optionally remove artifacts.

Options:
  -a, --artifacts          Remove artifacts directory
  -c, --configuration VAL  Clean configuration
  -h, --help               Show help

Examples:
  ./scripts/clean.sh
  ./scripts/clean.sh --artifacts
  ./scripts/clean.sh -c Debug

EOF
    print_common_usage
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        -a|--artifacts)
            REMOVE_ARTIFACTS=true
            shift
            ;;
        -c|--configuration)
            require_value "$1" "${2:-}"
            CONFIGURATION="$2"
            shift 2
            ;;
        -h|--help)
            print_usage
            exit 0
            ;;
        *)
            fail "Unknown option: $1"
            ;;
    esac
done

ensure_repo_root
ensure_dotnet

log "Cleaning solution (${CONFIGURATION})..."
dotnet clean "${SOLUTION_FILE}" --configuration "${CONFIGURATION}"

if [[ "${REMOVE_ARTIFACTS}" == true ]]; then
    log "Removing artifacts directory..."
    rm -rf "${ARTIFACTS_DIR}"
fi

log "Clean completed."