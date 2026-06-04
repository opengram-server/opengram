#!/usr/bin/env powershell
# Полный перезапуск MongoDB с включённой аутентификацией
# Скрипт корректно сбрасывает состояние MongoDB и поднимает его заново

Write-Host "======================================" -ForegroundColor Green
Write-Host "MongoDB Authentication Fix - Restart" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""

$compose_dir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $compose_dir

Write-Host "[1/5] Stopping all services..." -ForegroundColor Yellow
docker-compose down

Write-Host "[2/5] Removing MongoDB data volume to reinitialize..." -ForegroundColor Yellow
if (Test-Path "./data/mongo") {
    Remove-Item -Recurse -Force "./data/mongo"
    Write-Host "     MongoDB data directory removed" -ForegroundColor Green
} else {
    Write-Host "     MongoDB data directory not found (this is fine)" -ForegroundColor Cyan
}

Write-Host "[3/5] Waiting a moment before starting services..." -ForegroundColor Yellow
Start-Sleep -Seconds 2

Write-Host "[4/5] Building and starting services with MongoDB authentication..." -ForegroundColor Yellow
docker-compose up --build -d

Write-Host "[5/5] Waiting for MongoDB to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "======================================" -ForegroundColor Green
Write-Host "Restart Complete!" -ForegroundColor Green
Write-Host "======================================" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "1. Check MongoDB initialization: docker-compose logs mongodb"
Write-Host "2. Check data-seeder: docker-compose logs data-seeder"
Write-Host "3. Check admin-api: docker-compose logs admin-api"
Write-Host "4. Monitor bot-api: docker-compose logs bot-api-server"
Write-Host ""
Write-Host "MongoDB Authentication Details:" -ForegroundColor Cyan
Write-Host "  Username: admin" -ForegroundColor Gray
Write-Host "  Auth Database: admin" -ForegroundColor Gray
Write-Host "  Target Database: tg" -ForegroundColor Gray
Write-Host ""
