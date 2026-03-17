#!/usr/bin/env bash

# =============================================================================
#
# Runs the WPF application in Debug mode for development.
#
# This script is intended for local development and launches the
# AutoInputPlus desktop application using dotnet run.
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

ensure_repo_root
ensure_dotnet

log "Running AutoInputPlus WPF application..."

dotnet run \
    --project "${WPF_PROJECT}" \
    --configuration Debug \
    --framework "${FRAMEWORK_WPF}"