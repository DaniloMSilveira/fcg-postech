#!/bin/bash

# Caminho para o projeto de testes
TEST_PROJECT="../tests/FCG.UnitTests/FCG.UnitTests.csproj" 

# Pastas de saída do relatório
COVERAGE_OUTPUT="../coverage"
REPORT_OUTPUT="../coverage/report"

echo "🧹 Limpando diretórios antigos de cobertura..."
rm -rf "$COVERAGE_OUTPUT"
rm -rf "$REPORT_OUTPUT"

echo "🧪 Executando testes com cobertura..."
dotnet test "$TEST_PROJECT" \
  --collect:"XPlat Code Coverage" \
  --results-directory "$COVERAGE_OUTPUT" \
  --no-build \
  --verbosity normal

echo "📊 Gerando relatório com ReportGenerator..."
reportgenerator \
  -reports:"$COVERAGE_OUTPUT/**/coverage.cobertura.xml" \
  -targetdir:"$REPORT_OUTPUT" \
  -reporttypes:Html \
  -verbosity:Info

echo "✅ Relatório gerado em: $REPORT_OUTPUT/index.html"

# Abrindo no navegador (descomente de acordo com seu sistema)

# Linux
# xdg-open "$REPORT_OUTPUT/index.html"

# macOS
# open "$REPORT_OUTPUT/index.html"

# WSL (Windows Subsystem for Linux)
# wslview "$REPORT_OUTPUT/index.html"

# Git Bash no Windows
start "$REPORT_OUTPUT/index.html"
