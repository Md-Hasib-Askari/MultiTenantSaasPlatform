#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
  echo "Usage: $0 [args] — seed the database"
  exit 0
fi

dotnet run --project src/Api -- --seed "$@"
