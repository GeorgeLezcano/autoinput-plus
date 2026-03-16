#!/usr/bin/env bash

# =============================================================================
# publish.sh
#
# Publishes the WPF application as a standalone executable.
#
# The output is placed under:
#
#   artifacts/publish/AutoInputPlus.Wpf/
#
# The publish configuration:
#
#   - Release build
#   - Self-contained runtime
#   - Single-file executable
#   - Windows x64 runtime
#
# Example:
#
#   ./scripts/publish.sh
#
# =============================================================================

set -euo pipefail
source "$(cd -- "$(dirname "${BASH_SOURCE[0]}")" && pwd)/common.sh"

ensure_repo_root
ensure_dotnet
create_artifact_dirs

OUTPUT_DIR="${PUBLISH_DIR}/AutoInputPlus.Wpf"

log "Publishing WPF application..."

rm -rf "${OUTPUT_DIR}"
mkdir -p "${OUTPUT_DIR}"

dotnet publish "${WPF_PROJECT}" \
    --configuration "${CONFIGURATION}" \
    --framework "${FRAMEWORK_WPF}" \
    --runtime "${RUNTIME_WINDOWS}" \
    --self-contained true \
    -p:PublishSingleFile=true \
    -p:IncludeNativeLibrariesForSelfExtract=true \
    -p:EnableCompressionInSingleFile=true \
    --output "${OUTPUT_DIR}"

log "Publish completed."
log "Output directory: ${OUTPUT_DIR}"