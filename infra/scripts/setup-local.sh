#!/usr/bin/env bash
# Automated local setup for Legal Ai Ar (Linux / macOS).
# Uses shared cloud DEV Azure services — no local SQL Server or Azurite.
# See docs/onboarding/README.md for the full guide.
#
# Usage:
#   ./infra/scripts/setup-local.sh
#   ./infra/scripts/setup-local.sh --verify-connectivity
#   ./infra/scripts/setup-local.sh --skip-frontend

set -euo pipefail

VERIFY_CONNECTIVITY=false
SKIP_FRONTEND=false

for arg in "$@"; do
  case "$arg" in
    --verify-connectivity) VERIFY_CONNECTIVITY=true ;;
    --skip-frontend) SKIP_FRONTEND=true ;;
    -h|--help)
      echo "Usage: $0 [--verify-connectivity] [--skip-frontend]"
      exit 0
      ;;
    *) echo "Unknown option: $arg" >&2; exit 1 ;;
  esac
done

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
cd "$REPO_ROOT"

step() { echo ""; echo "=== $1 ==="; }
ok()   { echo "  OK  $1"; }
warn() { echo "  WARN  $1"; }
fail() { echo "  FAIL  $1" >&2; exit 1; }

version_ge() {
  local current="$1" required="$2"
  [ "$(printf '%s\n' "$required" "$current" | sort -V | head -n1)" = "$required" ]
}

step "Legal Ai Ar — Local Setup"
echo "Repo root: $REPO_ROOT"

step "1. Prerequisites"

command -v git >/dev/null 2>&1 || fail "Git not installed"
ok "Git: $(git --version)"

command -v dotnet >/dev/null 2>&1 || fail ".NET SDK not installed"
DOTNET_VER="$(dotnet --version)"
version_ge "$DOTNET_VER" "10.0" || fail ".NET SDK $DOTNET_VER — need 10.0+"
ok ".NET SDK: $DOTNET_VER"

command -v node >/dev/null 2>&1 || fail "Node.js not installed"
NODE_VER="$(node --version | sed 's/^v//')"
version_ge "$NODE_VER" "22.0" || fail "Node.js v$NODE_VER — need 22+"
ok "Node.js: v$NODE_VER"

command -v docker >/dev/null 2>&1 || fail "Docker not installed"
ok "Docker: $(docker --version)"

if command -v az >/dev/null 2>&1; then
  ok "Azure CLI: $(az version 2>/dev/null | head -n1 || az --version | head -n1)"
else
  warn "Azure CLI not found (optional)"
fi

step "2. Environment file (.env)"

if [ ! -f .env ]; then
  cp .env.example .env
  ok "Created .env from .env.example"
  warn "Fill DB_SECRET, STORAGE_KEY, SEARCH_KEY, OPENAI_KEY — ask Tech Lead for DEV values."
else
  ok ".env already exists (not overwritten)"
fi

if grep -qE 'DB_SECRET|STORAGE_KEY|SEARCH_KEY|OPENAI_KEY' .env 2>/dev/null; then
  warn ".env still contains placeholder secrets — cloud features will fail until replaced."
fi

step "3. Backend (.NET)"

cd "$REPO_ROOT/backend"
dotnet restore LegalAiAr.sln
dotnet build LegalAiAr.sln -c Release --no-restore
ok "Backend restore + build succeeded"

step "4. Frontend (Angular)"

if [ "$SKIP_FRONTEND" = false ]; then
  cd "$REPO_ROOT/frontend"
  if ! npm ci; then
    warn "npm ci failed — if AppKit packages fail, complete JFrog login: docs/onboarding/appkit-npm-access.md"
    exit 1
  fi
  ok "Frontend dependencies installed"
else
  warn "Skipped frontend (--skip-frontend)"
fi

if [ "$VERIFY_CONNECTIVITY" = true ]; then
  step "5. Azure connectivity (optional)"
  if [ -f "$REPO_ROOT/infra/scripts/verify-azure-connectivity.ps1" ]; then
    warn "Run verify-azure-connectivity.ps1 from PowerShell on Windows, or use the .NET tool manually."
  else
    warn "Connectivity script not found — skipped"
  fi
fi

step "Setup complete"
cat <<EOF

Next steps:
  1. Fill secrets in .env (if not done) — see docs/onboarding/README.md §3
  2. Backend:  cd backend && dotnet run --project src/api/LegalAiAr.Api
  3. Frontend: cd frontend && npm start
  4. Verify:   Swagger http://localhost:5088/swagger  ·  SPA http://localhost:4200
  5. Optional: docker compose -f docker-compose.app.yml up -d  (app in containers)

Troubleshooting: docs/onboarding/troubleshooting.md

EOF
