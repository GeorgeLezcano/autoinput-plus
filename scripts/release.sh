#!/usr/bin/env bash

# =============================================================================
#
# Creates a distributable release archive for the application.
#
# Steps performed:
#
#   1. Optionally builds the solution
#   2. Optionally runs tests
#   3. Publishes the application
#   4. Compresses the publish output into a ZIP archive
#
# The resulting archive is stored in:
#
#   artifacts/release/
#
# Usage:
#   ./scripts/release.sh
#   ./scripts/release.sh --build
#   ./scripts/release.sh --build --tests
#   ./scripts/release.sh --runtime win-arm64
#
# Options:
#   -b, --build             Run build before release
#   -t, --tests             When used with --build, also run tests
#   -r, --runtime           Runtime identifier
#   -c, --configuration     Release configuration
#   -h, --help              Show help
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

RUN_BUILD=false
RUN_TESTS=false

print_usage() {
    cat <<EOF
Usage: ./scripts/release.sh [options]

Create a distributable ZIP archive.

Options:
  -b, --build              Run build before release
  -t, --tests              When used with --build, also run tests
  -r, --runtime VAL        Runtime identifier
  -c, --configuration VAL  Release configuration
  -h, --help               Show help

Examples:
  ./scripts/release.sh
  ./scripts/release.sh --build
  ./scripts/release.sh --build --tests
  ./scripts/release.sh -r win-arm64

EOF
    print_common_usage
}

while [[ $# -gt 0 ]]; do
    case "$1" in
        -b|--build)
            RUN_BUILD=true
            shift
            ;;
        -t|--tests)
            RUN_TESTS=true
            shift
            ;;
        -r|--runtime)
            require_value "$1" "${2:-}"
            RUNTIME_WINDOWS="$2"
            shift 2
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

if [[ "${RUN_TESTS}" == true && "${RUN_BUILD}" == false ]]; then
    fail "--tests requires --build"
fi

ensure_repo_root
ensure_dotnet
ensure_zip
create_artifact_dirs

PUBLISH_ARGS=(--configuration "${CONFIGURATION}" --runtime "${RUNTIME_WINDOWS}")

if [[ "${RUN_BUILD}" == true ]]; then
    PUBLISH_ARGS+=(--build)
fi

if [[ "${RUN_TESTS}" == true ]]; then
    PUBLISH_ARGS+=(--tests)
fi

"${SCRIPT_DIR}/publish.sh" "${PUBLISH_ARGS[@]}"

VERSION="$(date +%Y%m%d-%H%M%S)"
SOURCE_DIR="${PUBLISH_DIR}/AutoInputPlus.Wpf"
ZIP_FILE="${RELEASE_DIR}/AutoInputPlus-${VERSION}.zip"

log "Creating release archive..."

rm -f "${ZIP_FILE}"

(
    cd "${SOURCE_DIR}"
    zip -r "${ZIP_FILE}" .
)

log "Release archive created:"
log "${ZIP_FILE}"