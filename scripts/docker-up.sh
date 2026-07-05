#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

if [[ "${1:-}" == "-h" || "${1:-}" == "--help" ]]; then
  echo "Usage: $0 [args] — start local infrastructure (Postgres, Redis, pgAdmin)"
  exit 0
fi

docker compose -f docker/docker-compose.yml up -d "$@"
