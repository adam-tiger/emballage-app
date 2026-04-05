#!/usr/bin/env pwsh
# Script PowerShell — Tests Phoenix Emballages (Windows/Linux/Mac)
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$RootDir = $PSScriptRoot

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "   Phoenix Emballages — Test Runner"     -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# ── Tests unitaires .NET ──────────────────────────────────────────────────────
Write-Host ""
Write-Host "=== Tests Unitaires .NET ===" -ForegroundColor Yellow
Set-Location "$RootDir\src"
dotnet test "$RootDir\tests\Phoenix.UnitTests" `
  --collect:"XPlat Code Coverage" `
  --results-directory "$RootDir\TestResults\unit" `
  --logger "console;verbosity=normal" `
  --no-restore

# ── Tests d'intégration .NET (Testcontainers) ────────────────────────────────
Write-Host ""
Write-Host "=== Tests Integration .NET (Testcontainers) ===" -ForegroundColor Yellow
dotnet test "$RootDir\tests\Phoenix.IntegrationTests" `
  --collect:"XPlat Code Coverage" `
  --results-directory "$RootDir\TestResults\integration" `
  --logger "console;verbosity=normal" `
  --no-restore

# ── Tests Angular (Vitest) ────────────────────────────────────────────────────
Write-Host ""
Write-Host "=== Tests Angular (Vitest) ===" -ForegroundColor Yellow
Set-Location "$RootDir\phoenix-frontend"
npm run test -- --watch=false --coverage

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "   Tous les tests passes ✅"             -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
