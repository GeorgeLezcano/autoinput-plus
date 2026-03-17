#!/usr/bin/env bash

# =============================================================================
#
# Restores dependencies and builds the entire solution.
#
# This script performs the following steps:
#
#   1. Restores NuGet packages
#   2. Builds the solution
#   3. Optionally runs all tests
#
# Default behavior:
#   - Restore + build only
#
# Usage:
#   ./scripts/build.sh
#   ./scripts/build.sh --tests
#   ./scripts/build.sh --configuration Debug
#   ./scripts/build.sh --no-restore
#
# Options:
#   -t, --tests             Run tests after a successful build
#       --no-restore        Skip dotnet restore
#   -c, --configuration     Build configuration (Debug or Release)
#   -h, --help              Show help
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

RUN_TESTS=false
SKIP_RESTORE=false

print_usage() {
    cat <<EOF
Usage: ./scripts/build.sh [options]

Restore dependencies and build the solution.

Options:
  -t, --tests              Run tests after build
      --no-restore         Skip restore
  -c, --configuration VAL  Build configuration
  -h, --help               Show help

Examples:
  ./scripts/build.sh
  ./scripts/build.sh --tests
  ./scripts/build.sh -c Debug
  ./scripts/build.sh --no-restore

EOF
    print_common_usage
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        -t|--tests)
            RUN_TESTS=true
            shift
            ;;
        --no-restore)
            SKIP_RESTORE=true
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

if [[ "${SKIP_RESTORE}" == false ]]; then
    log "Restoring NuGet packages..."
    dotnet restore "${SOLUTION_FILE}"
else
    log "Skipping restore."
fi

log "Building solution (${CONFIGURATION})..."
dotnet build "${SOLUTION_FILE}" \
    --configuration "${CONFIGURATION}" \
    $([[ "${SKIP_RESTORE}" == true ]] && echo "--no-restore")

if [[ "${RUN_TESTS}" == true ]]; then
    log "Build succeeded. Running tests..."
    "${SCRIPT_DIR}/test.sh" --configuration "${CONFIGURATION}" --no-build
fi

log "Build completed successfully."