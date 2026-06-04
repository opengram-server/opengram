# Simple script to create chat themes in MongoDB

Write-Host ""
Write-Host "Creating chat themes in MongoDB..." -ForegroundColor Cyan
Write-Host ""

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Copy seed script to MongoDB container
Write-Host "Copying seed script to container..." -ForegroundColor Gray
docker cp "$scriptDir\seed-chat-themes-unicode.js" compose-mongodb-1:/tmp/seed-themes.js

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to copy seed script!" -ForegroundColor Red
    exit 1
}

# Execute seed script
Write-Host "Creating themes..." -ForegroundColor Gray
docker exec compose-mongodb-1 mongosh tg /tmp/seed-themes.js

if ($LASTEXITCODE -ne 0) {
    Write-Host "Failed to create themes!" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "Done! 8 chat themes created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Themes:" -ForegroundColor Cyan
Write-Host "  - Home (Blue)" -ForegroundColor White
Write-Host "  - Love (Pink)" -ForegroundColor White
Write-Host "  - Party (Orange)" -ForegroundColor White
Write-Host "  - Ocean (Cyan)" -ForegroundColor White
Write-Host "  - Flower (Rose)" -ForegroundColor White
Write-Host "  - Night (Purple)" -ForegroundColor White
Write-Host "  - Fire (Red)" -ForegroundColor White
Write-Host "  - Green (Emerald)" -ForegroundColor White
Write-Host ""
