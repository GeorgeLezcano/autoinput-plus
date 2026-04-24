#!/usr/bin/env bash

# =============================================================================
#
# Shared configuration and helper utilities for AutoInputPlus scripts.
#
# This file defines:
#   - repository paths
#   - solution location
#   - build configuration
#   - artifact directories
#   - logging helpers
#   - common argument parsing helpers
#
# All other scripts should source this file instead of duplicating logic.
#
# Example usage inside another script:
#
#   source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"
#
# =============================================================================

set -euo pipefail

# Resolve script directory
SCRIPT_DIR="$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

# Repository root
REPO_ROOT="$(cd -- "${SCRIPT_DIR}/.." && pwd)"

# Solution file
SOLUTION_FILE="${REPO_ROOT}/AutoInputPlus.sln"

# Main WPF project
WPF_PROJECT="${REPO_ROOT}/src/AutoInputPlus.Wpf/AutoInputPlus.Wpf.csproj"

APP_RELEASE_NAME="${APP_RELEASE_NAME:-AutoInputPlus}"

# Artifacts directory (build outputs)
ARTIFACTS_DIR="${REPO_ROOT}/artifacts"
PUBLISH_DIR="${ARTIFACTS_DIR}/publish"
RELEASE_DIR="${ARTIFACTS_DIR}/release"
INSTALLER_DIR="${ARTIFACTS_DIR}/installer"
TEST_RESULTS_DIR="${ARTIFACTS_DIR}/test-results"

# Build configuration
CONFIGURATION="${CONFIGURATION:-Release}"

# Target framework
FRAMEWORK_WPF="${FRAMEWORK_WPF:-net9.0-windows}"

# Runtime identifier
RUNTIME_WINDOWS="${RUNTIME_WINDOWS:-win-x64}"

# -----------------------------------------------------------------------------
# Logging helpers
# -----------------------------------------------------------------------------

log() {
    echo "[AutoInputPlus] $*"
}

warn() {
    echo "[AutoInputPlus][WARN] $*" >&2
}

fail() {
    echo "[AutoInputPlus][ERROR] $*" >&2
    exit 1
}

# -----------------------------------------------------------------------------
# Environment validation
# -----------------------------------------------------------------------------

ensure_repo_root() {
    [[ -f "${SOLUTION_FILE}" ]] || fail "Solution file not found: ${SOLUTION_FILE}"
}

ensure_dotnet() {
    command -v dotnet >/dev/null 2>&1 || fail "dotnet SDK not installed"
}

ensure_zip() {
    command -v zip >/dev/null 2>&1 || fail "zip utility not installed"
}

# -----------------------------------------------------------------------------
# Artifact directories
# -----------------------------------------------------------------------------

create_artifact_dirs() {
    mkdir -p "${ARTIFACTS_DIR}"
    mkdir -p "${PUBLISH_DIR}"
    mkdir -p "${RELEASE_DIR}"
    mkdir -p "${INSTALLER_DIR}"
    mkdir -p "${TEST_RESULTS_DIR}"
}

# -----------------------------------------------------------------------------
# Common argument helpers
# -----------------------------------------------------------------------------

print_common_usage() {
    cat <<EOF
Common options:
  -c, --configuration <Debug|Release>   Build configuration
  -h, --help                            Show help

Environment overrides:
  CONFIGURATION=Debug
  FRAMEWORK_WPF=net9.0-windows
  RUNTIME_WINDOWS=win-x64
EOF
}

require_value() {
    local option_name="${1:-}"
    local option_value="${2:-}"

    [[ -n "${option_value}" ]] || fail "Missing value for ${option_name}"
}

is_windows() {
    case "$(uname -s)" in
        MINGW*|MSYS*|CYGWIN*) return 0 ;;
        *) return 1 ;;
    esac
}
