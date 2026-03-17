#!/usr/bin/env bash

# =============================================================================
#
# Creates a distributable release archive for the application.
#
# Steps performed:
#
#   1. Publishes the application
#   2. Compresses the publish output into a ZIP archive
#
# The resulting archive is stored in:
#
#   artifacts/release/
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

ensure_repo_root
ensure_dotnet
ensure_zip
create_artifact_dirs

"${SCRIPT_DIR}/publish.sh"

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