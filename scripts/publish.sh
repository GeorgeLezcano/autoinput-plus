#!/usr/bin/env bash

# =============================================================================
#
# Publishes the WPF application as a standalone executable.
#
# The output is placed under:
#
#   artifacts/publish/AutoInputPlus.Wpf/
#
# Default behavior:
#   - Publish only
#   - Release configuration
#   - Self-contained runtime
#   - Single-file executable
#   - Windows x64 runtime
#
# Usage:
#   ./scripts/publish.sh
#   ./scripts/publish.sh --build
#   ./scripts/publish.sh --build --tests
#   ./scripts/publish.sh --configuration Debug
#   ./scripts/publish.sh --runtime win-arm64
#
# Options:
#   -b, --build             Run build before publish
#   -t, --tests             When used with --build, also run tests
#       --no-self-contained Publish framework-dependent output
#   -r, --runtime           Runtime identifier (default: win-x64)
#   -c, --configuration     Publish configuration
#   -h, --help              Show help
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

RUN_BUILD=false
RUN_TESTS=false
SELF_CONTAINED=true

print_usage() {
    cat <<EOF
Usage: ./scripts/publish.sh [options]

Publish the WPF application.

Options:
  -b, --build              Run build before publish
  -t, --tests              When used with --build, also run tests
      --no-self-contained  Publish framework-dependent output
  -r, --runtime VAL        Runtime identifier
  -c, --configuration VAL  Publish configuration
  -h, --help               Show help

Examples:
  ./scripts/publish.sh
  ./scripts/publish.sh --build
  ./scripts/publish.sh --build --tests
  ./scripts/publish.sh -r win-arm64
  ./scripts/publish.sh -c Debug

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
        --no-self-contained)
            SELF_CONTAINED=false
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
create_artifact_dirs

if [[ "${RUN_BUILD}" == true ]]; then
    BUILD_ARGS=(--configuration "${CONFIGURATION}")
    if [[ "${RUN_TESTS}" == true ]]; then
        BUILD_ARGS+=(--tests)
    fi

    log "Running build step before publish..."
    "${SCRIPT_DIR}/build.sh" "${BUILD_ARGS[@]}"
fi

OUTPUT_DIR="${PUBLISH_DIR}/${APP_RELEASE_NAME}"

log "Publishing WPF application..."

rm -rf "${OUTPUT_DIR}"
mkdir -p "${OUTPUT_DIR}"

PUBLISH_ARGS=(
    "${WPF_PROJECT}"
    "--configuration" "${CONFIGURATION}"
    "--framework" "${FRAMEWORK_WPF}"
    "--runtime" "${RUNTIME_WINDOWS}"
    "--output" "${OUTPUT_DIR}"
    "-p:PublishSingleFile=true"
    "-p:IncludeNativeLibrariesForSelfExtract=true"
    "-p:EnableCompressionInSingleFile=true"
)

if [[ "${SELF_CONTAINED}" == true ]]; then
    PUBLISH_ARGS+=("--self-contained" "true")
else
    PUBLISH_ARGS+=("--self-contained" "false")
fi

dotnet publish "${PUBLISH_ARGS[@]}"

log "Publish completed."
log "Output directory: ${OUTPUT_DIR}"
