#!/usr/bin/env bash

# =============================================================================
#
# Runs all unit tests in the solution.
#
# This script executes all test projects using dotnet test.
#
# Default behavior:
#   - Runs tests using the current configuration
#   - Builds test projects as needed
#   - Writes TRX results under artifacts/test-results
#
# Usage:
#   ./scripts/test.sh
#   ./scripts/test.sh --no-build
#   ./scripts/test.sh --configuration Debug
#   ./scripts/test.sh --filter "FullyQualifiedName~Core"
#
# Options:
#       --no-build          Skip build step before testing
#   -c, --configuration     Build configuration (Debug or Release)
#   -f, --filter            Optional dotnet test filter expression
#   -h, --help              Show help
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

SKIP_BUILD=false
TEST_FILTER=""

print_usage() {
    cat <<EOF
Usage: ./scripts/test.sh [options]

Run all tests in the solution.

Options:
      --no-build           Skip build
  -c, --configuration VAL  Build configuration
  -f, --filter VAL         dotnet test filter expression
  -h, --help               Show help

Examples:
  ./scripts/test.sh
  ./scripts/test.sh --no-build
  ./scripts/test.sh -c Debug
  ./scripts/test.sh --filter "FullyQualifiedName~Core"

EOF
    print_common_usage
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        --no-build)
            SKIP_BUILD=true
            shift
            ;;
        -c|--configuration)
            require_value "$1" "${2:-}"
            CONFIGURATION="$2"
            shift 2
            ;;
        -f|--filter)
            require_value "$1" "${2:-}"
            TEST_FILTER="$2"
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
create_artifact_dirs

RESULTS_FILE="${TEST_RESULTS_DIR}/tests-${CONFIGURATION}.trx"

log "Running tests (${CONFIGURATION})..."

TEST_ARGS=(
    "${SOLUTION_FILE}"
    "--configuration" "${CONFIGURATION}"
    "--logger" "trx;LogFileName=$(basename "${RESULTS_FILE}")"
    "--results-directory" "${TEST_RESULTS_DIR}"
)

if [[ "${SKIP_BUILD}" == true ]]; then
    TEST_ARGS+=("--no-build")
fi

if [[ -n "${TEST_FILTER}" ]]; then
    TEST_ARGS+=("--filter" "${TEST_FILTER}")
fi

dotnet test "${TEST_ARGS[@]}"

log "All tests passed."
log "Results directory: ${TEST_RESULTS_DIR}"