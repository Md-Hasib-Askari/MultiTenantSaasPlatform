#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

CONNECTION="${CONNECTION:-Host=localhost;Database=mtsp;Username=mtsp;Password=mtsp}"

case "${1:-}" in
  -h|--help)
    echo "Usage: $0 [migration]          — apply up to migration (or latest if omitted)"
    echo "       $0 revert [migration]   — revert to migration (or 0 for empty)"
    exit 0
    ;;
  revert|--down|-d)
    target="${2:-0}"
    dotnet ef database update "$target" \
      --project src/Infrastructure \
      --startup-project src/Api \
      --connection "$CONNECTION" \
      "${@:3}"
    ;;
  "")
    echo "Usage: $0 [migration]          — apply up to migration (or latest if omitted)"
    echo "       $0 revert [migration]   — revert to migration (or 0 for empty)"
    exit 1
    ;;
  *)
    dotnet ef database update "$1" \
      --project src/Infrastructure \
      --startup-project src/Api \
      --connection "$CONNECTION" \
      "${@:2}"
    ;;
esac