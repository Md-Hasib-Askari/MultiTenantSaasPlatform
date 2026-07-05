#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

case "${1:-}" in
  -h|--help)
    echo "Usage: $0 <name>      — add a new migration"
    echo "       $0 remove      — undo the last migration"
    exit 0
    ;;
  remove|--undo|-u)
    dotnet ef migrations remove \
      --project src/Infrastructure \
      --startup-project src/Api \
      "${@:2}"
    ;;
  "")
    echo "Usage: $0 <name>      — add a new migration"
    echo "       $0 remove      — undo the last migration"
    exit 1
    ;;
  *)
    dotnet ef migrations add "$1" \
      --project src/Infrastructure \
      --startup-project src/Api \
      --output-dir Migrations
    ;;
esac