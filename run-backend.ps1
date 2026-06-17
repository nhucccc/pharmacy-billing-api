Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  KHOI DONG BACKEND - PharmacyBilling API" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# 1. Kiem tra SQL Server va RabbitMQ co chay khong
Write-Host "`n[1/3] Kiem tra SQL Server + RabbitMQ..." -ForegroundColor Yellow
$sqlOk = docker ps --filter "name=pharmacy_sqlserver" --filter "status=running" -q
$rabbitOk = docker ps --filter "name=pharmacy_rabbitmq" --filter "status=running" -q

if (-not $sqlOk -or -not $rabbitOk) {
    Write-Host "  SQL Server / RabbitMQ chua chay. Khoi dong..." -ForegroundColor Red
    docker-compose up -d sqlserver rabbitmq
    Write-Host "  Cho 15 giay de khoi dong..." -ForegroundColor Gray
    Start-Sleep -Seconds 15
} else {
    Write-Host "  SQL Server: OK" -ForegroundColor Green
    Write-Host "  RabbitMQ:   OK" -ForegroundColor Green
}

# 2. Apply migration
Write-Host "`n[2/3] Apply database migration..." -ForegroundColor Yellow
Set-Location "C:\BTLFullstack\PharmacyBilling\src\PharmacyBilling.API"
dotnet ef database update --project ..\PharmacyBilling.Infrastructure\ --startup-project . 2>&1 | Select-Object -Last 3

# 3. Chay API
Write-Host "`n[3/3] Khoi dong API..." -ForegroundColor Yellow
Write-Host "  URL: http://localhost:5000" -ForegroundColor Green
Write-Host "  Swagger: http://localhost:5000/swagger" -ForegroundColor Green
Write-Host "  Nhan Ctrl+C de dung`n" -ForegroundColor Gray

Set-Location "C:\BTLFullstack\PharmacyBilling\src\PharmacyBilling.API"
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:5000"
dotnet run --no-launch-profile
