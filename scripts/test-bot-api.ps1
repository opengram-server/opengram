# Скрипт PowerShell для проверки Bot API
$TOKEN = "999999999:ABCdefGHIjklMNOpqrsTUVwxyz123456"
$BASE_URL = "http://localhost:8081/bot$TOKEN"

Write-Host "Testing MyTelegram Bot API" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "1. Testing getMe..." -ForegroundColor Yellow
Invoke-RestMethod -Uri "$BASE_URL/getMe" | ConvertTo-Json

Write-Host ""
Write-Host "2. Testing getUpdates..." -ForegroundColor Yellow
Invoke-RestMethod -Uri "$BASE_URL/getUpdates" | ConvertTo-Json

Write-Host ""
Write-Host "3. Testing getWebhookInfo..." -ForegroundColor Yellow
Invoke-RestMethod -Uri "$BASE_URL/getWebhookInfo" | ConvertTo-Json

Write-Host ""
Write-Host "4. Testing sendMessage..." -ForegroundColor Yellow
$body = @{
    chat_id = 2010001
    text = "Hello from test bot!"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$BASE_URL/sendMessage" -Method Post -Body $body -ContentType "application/json" | ConvertTo-Json

Write-Host ""
Write-Host "Tests completed" -ForegroundColor Green
