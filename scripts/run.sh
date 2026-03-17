#!/usr/bin/env bash

# =============================================================================
#
# Runs the WPF application for development.
#
# This script launches the AutoInputPlus desktop application using dotnet run.
#
# Default behavior:
#   - Runs the WPF project in Debug mode
#
# Usage:
#   ./scripts/run.sh
#   ./scripts/run.sh --configuration Release
#   ./scripts/run.sh --no-build
#
# Options:
#       --no-build          Skip build before launch
#   -c, --configuration     Run configuration (default: Debug)
#   -h, --help              Show help
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

SKIP_BUILD=false
CONFIGURATION="${CONFIGURATION:-Debug}"

print_usage() {
    cat <<EOF
Usage: ./scripts/run.sh [options]

Run the WPF application for development.

Options:
      --no-build           Skip build
  -c, --configuration VAL  Run configuration
  -h, --help               Show help

Examples:
  ./scripts/run.sh
  ./scripts/run.sh --no-build
  ./scripts/run.sh -c Release

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

log "Running AutoInputPlus WPF application..."

RUN_ARGS=(
    "--project" "${WPF_PROJECT}"
    "--configuration" "${CONFIGURATION}"
    "--framework" "${FRAMEWORK_WPF}"
)

if [[ "${SKIP_BUILD}" == true ]]; then
    RUN_ARGS+=("--no-build")
fi

dotnet run "${RUN_ARGS[@]}"