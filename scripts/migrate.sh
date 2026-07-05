#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

CONNECTION="${CONNECTION:-Host=localhost;Database=mtsp;Username=mtsp;Password=mtsp}"

case "${1:-}" in
  -h|--help)
    echo "Usage: $0 add <name>              — add a new migration"
    echo "       $0 remove                  — undo the last migration"
    echo "       $0 apply [migration]        — apply up to migration (or latest if omitted)"
    echo "       $0 list                    — list all migrations"
    echo "       $0 revert [migration]       — revert to migration (or 0 for empty)"
    exit 0
    ;;
  add)
    shift
    dotnet ef migrations add "$1" \
      --project src/Infrastructure \
      --startup-project src/Api \
      --output-dir Migrations
    ;;
  remove)
    shift
    dotnet ef migrations remove \
      --project src/Infrastructure \
      --startup-project src/Api \
      "$@"
    ;;
  apply)
    shift
    dotnet ef database update "${1:-}" \
      --project src/Infrastructure \
      --startup-project src/Api \
      --connection "$CONNECTION" \
      "${@:2}"
    ;;
  revert)
    shift
    target="${1:-0}"
    dotnet ef database update "$target" \
      --project src/Infrastructure \
      --startup-project src/Api \
      --connection "$CONNECTION" \
      "${@:2}"
    ;;
  list|status)
    dotnet ef migrations list \
      --project src/Infrastructure \
      --startup-project src/Api \
      --connection "$CONNECTION"
    ;;
  "")
    echo "Usage: $0 <command> [args]"
    echo ""
    echo "Commands:"
    echo "  add <name>       — add a new migration"
    echo "  remove           — undo the last migration"
    echo "  apply [name]     — apply up to migration (or latest if omitted)"
    echo "  list             — list all migrations"
    echo "  revert [name]    — revert to migration (or 0 for empty)"
    exit 1
    ;;
  *)
    echo "Unknown command: $1"
    echo "Usage: $0 <command> [args]"
    echo "Run '$0 --help' for details."
    exit 1
    ;;
esac
