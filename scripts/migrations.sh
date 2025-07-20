#!/bin/bash

ROOT_DIR="$(cd "$(dirname "$0")/.." && pwd)"

echo "🔐 Processando migrations - Infra.Security (Identity)..."
cd "$ROOT_DIR/src/FCG.Infra.Security" || { echo "❌ Falha ao acessar pasta FCG.Infra.Security!"; exit 1; }
if dotnet ef database update -c IdentityDataContext -s "$ROOT_DIR/src/FCG.API/"; then
    echo "✅ Migrations aplicadas com sucesso!"
else
    echo "❌ Falha ao aplicar as migrations!"
    exit 1
fi

echo ""
echo "📦 Processando migrations - Infra.Data (Entidades)..."
cd "$ROOT_DIR/src/FCG.Infra.Data" || { echo "❌ Falha ao acessar pasta FCG.Infra.Data!"; exit 1; }
if dotnet ef database update -c FCGDataContext -s "$ROOT_DIR/src/FCG.API/"; then
    echo "✅ Migrations aplicadas com sucesso!"
else
    echo "❌ Falha ao aplicar as migrations!"
    exit 1
fi
