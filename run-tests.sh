#!/bin/bash
set -e

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

echo "========================================"
echo "   Phoenix Emballages — Test Runner"
echo "========================================"

# ── Tests unitaires .NET ──────────────────────────────────────────────────────
echo ""
echo "=== Tests Unitaires .NET ==="
cd "$ROOT_DIR/src"
dotnet test "$ROOT_DIR/tests/Phoenix.UnitTests" \
  --collect:"XPlat Code Coverage" \
  --results-directory "$ROOT_DIR/TestResults/unit" \
  --logger "console;verbosity=normal" \
  --no-restore

# ── Tests d'intégration .NET (Testcontainers PostgreSQL) ─────────────────────
echo ""
echo "=== Tests Intégration .NET (Testcontainers) ==="
dotnet test "$ROOT_DIR/tests/Phoenix.IntegrationTests" \
  --collect:"XPlat Code Coverage" \
  --results-directory "$ROOT_DIR/TestResults/integration" \
  --logger "console;verbosity=normal" \
  --no-restore

# ── Tests Angular (Vitest) ────────────────────────────────────────────────────
echo ""
echo "=== Tests Angular (Vitest) ==="
cd "$ROOT_DIR/phoenix-frontend"
npm run test -- --watch=false --coverage

echo ""
echo "========================================"
echo "   Tous les tests passés ✅"
echo "========================================"
